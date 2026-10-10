using System.Collections;
using UnityEngine;

public class SkillBehavior_bombtrap : SkillObjectBehavior
{
	private PlayerVisual _pVisual;

	[Header("Bomb Trap Skill")]
	[SerializeField]
	private GameObject _bombVisual;

	[SerializeField]
	private GameObject _bombFaceInit;

	[SerializeField]
	private GameObject _bombFacePreExplode;

	[SerializeField]
	private LerpScale _bombLerpScale;

	[SerializeField]
	private float _timeForBombSet = 0.8f;

	[SerializeField]
	private float _setGravityAdd = 45.5f;

	[SerializeField]
	private float _groundDetection = 1.5f;

	[SerializeField]
	private float _naturalTimer = 6f;

	private float _gravityAdd;

	private bool _isBombDiscovered;

	private bool _isBombSet;

	private bool _isGrounded;

	[Space]
	[SerializeField]
	private GameObject _bombExplosionObject;

	[SerializeField]
	private float _timeBeforeExplosion = 0.35f;

	[SerializeField]
	private float _timeBeforeRemoveObject = 1.5f;

	private bool _setToExplode;

	public override void End_SkillBehavior()
	{
		if (!_setToExplode)
		{
			StartCoroutine(Init_BombExplosion());
		}
	}

	public override void Init_SkillBehavior()
	{
		StartCoroutine(Init_BombSet());
		Invoke("End_SkillBehavior", _naturalTimer);
	}

	private IEnumerator Init_BombSet()
	{
		yield return new WaitForSeconds(_timeForBombSet);
		_isBombSet = true;
	}

	public override void Update_SkillBehavior()
	{
		if (!_skillObject._parentPlayer)
		{
			return;
		}
		_pVisual = _skillObject._parentPlayer._pVisual;
		LayerMask layerMask = CastLayers.GroundMask;
		_isGrounded = base.gameObject.scene.GetPhysicsScene().Raycast(base.transform.position, Vector3.down, _groundDetection, layerMask);
		if (!_isGrounded)
		{
			_gravityAdd += _setGravityAdd * Time.deltaTime;
			base.transform.position += _gravityAdd * Time.deltaTime * Vector3.down;
		}
		if (!_setToExplode)
		{
			return;
		}
		MeshRenderer component = _bombVisual.GetComponent<MeshRenderer>();
		if ((bool)component)
		{
			Material material = component.material;
			if (material.HasFloat("_Saturation"))
			{
				material.SetFloat("_Saturation", Mathf.Lerp(material.GetFloat("_Saturation"), 2f, 9.5f * Time.deltaTime));
			}
		}
		_bombLerpScale.setScale = Vector3.one * 6.5f;
	}

	private void OnTriggerStay(Collider other)
	{
		if (_isBombDiscovered || !_isBombSet || !other.gameObject.GetComponentInParent<StatusEntity>() || other.gameObject.GetComponentInParent<StatusEntity>()._currentHealth <= 0)
		{
			return;
		}
		Player componentInParent = other.gameObject.GetComponentInParent<Player>();
		if ((!componentInParent || !(componentInParent.Network_playerMapInstance != _skillObject._parentPlayer.Network_playerMapInstance)) && (!componentInParent || componentInParent._pPvp._currentVersusFlag != VersusFlag.PVE) && (!componentInParent || !componentInParent.Network_partyObject || !(componentInParent.Network_partyObject == _skillObject._parentPlayer.Network_partyObject)) && !(componentInParent == _skillObject._parentPlayer))
		{
			CreepBehavior component = other.gameObject.GetComponent<CreepBehavior>();
			if (!component || !component._baseCreep || component._baseCreep._currentCreepCondition == CreepCondition.ACTIVE)
			{
				StartCoroutine(Init_BombExplosion());
				_isBombDiscovered = true;
			}
		}
	}

	private IEnumerator Init_BombExplosion()
	{
		while (!_isGrounded)
		{
			yield return null;
		}
		_setToExplode = true;
		if ((bool)_bombFaceInit)
		{
			_bombFaceInit.SetActive(value: false);
		}
		if ((bool)_bombFacePreExplode)
		{
			_bombFacePreExplode.SetActive(value: true);
		}
		CombatCollider componentInChildren = _bombExplosionObject.GetComponentInChildren<CombatCollider>();
		if (!_skillObject._parentPlayer)
		{
			Object.Destroy(base.gameObject);
			yield break;
		}
		componentInChildren._parentObject = _skillObject._parentPlayer.gameObject;
		yield return new WaitForSeconds(_timeBeforeExplosion);
		_bombVisual.SetActive(value: false);
		_bombExplosionObject.SetActive(value: true);
		yield return new WaitForSeconds(_timeBeforeRemoveObject);
		Object.Destroy(base.gameObject);
	}
}
