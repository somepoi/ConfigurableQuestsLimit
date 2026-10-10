using System.Collections;
using UnityEngine;

public class CreepBehavior_slimeDiva : CreepBehavior
{
	[Header("Slime Diva Behavior")]
	[SerializeField]
	private float _bufferBeforeNextPosition = 1.65f;

	private bool _initBehavior;

	[Header("AOE Slime Spike")]
	[SerializeField]
	private GameObject _aoeAttackObject;

	[Header("Slime Orb")]
	[SerializeField]
	private GameObject _slimeOrbPrefab;

	[SerializeField]
	private Transform _slimeOrbSpawnpoint;

	[Header("Tit Bullets")]
	[SerializeField]
	private GameObject _titBulletPrefab;

	[SerializeField]
	private Transform _titBulletShooter_L;

	[SerializeField]
	private Transform _titBulletShooter_R;

	[SerializeField]
	private float _titBulletRate = 0.25f;

	[SerializeField]
	private int _titBulletAmount = 36;

	[Header("Audio")]
	[SerializeField]
	private AudioSource _tittyBulletAsrc;

	[SerializeField]
	private AudioClip _laughClip;

	[SerializeField]
	private AudioClip _aoeAttackClip;

	public override void Behavior_OnServerFixedUpdate()
	{
		if ((bool)_baseCreep.Network_aggroedEntity && !_initBehavior)
		{
			_baseCreep._creepRB.AddForce(base.transform.forward * 85.5f, ForceMode.Impulse);
			StartCoroutine(SlimeDiva_InitBehavior());
		}
	}

	public override void Server_Behavior_OnSpawn()
	{
		base.Server_Behavior_OnSpawn();
		_baseCreep._creepRB.AddForce(base.transform.forward * 85.5f, ForceMode.Impulse);
		_initBehavior = true;
	}

	public override void Behavior_OnHurt()
	{
	}

	public override void Behavior_OnServerUpdate()
	{
		if ((bool)_baseCreep.Network_aggroedEntity)
		{
			_titBulletShooter_L.LookAt(_baseCreep.Network_aggroedEntity.transform);
			_titBulletShooter_R.LookAt(_baseCreep.Network_aggroedEntity.transform);
		}
	}

	public override void Reset_Behavior()
	{
		StopAllCoroutines();
		_baseCreep.Network_ignoreAggroedTarget = false;
		_initBehavior = false;
	}

	private IEnumerator SlimeDiva_InitBehavior()
	{
		if (!_baseCreep.Network_aggroedEntity || _initBehavior)
		{
			yield break;
		}
		_initBehavior = true;
		while (_baseCreep._parryBuffer > 0f)
		{
			yield return null;
		}
		yield return new WaitForSeconds(Random.Range(0.4f, 0.6f));
		switch (Random.Range(0, 3))
		{
		case 0:
			_baseCreep._creepRB.AddForce(-base.transform.forward * 15.5f, ForceMode.Impulse);
			_baseCreep.Server_CrossFadeAnim("Attack03", 0.05f, 1);
			_baseCreep._attackActionBuffer = 1.25f;
			yield return new WaitForSeconds(2.8f);
			break;
		case 1:
		{
			_baseCreep._creepRB.AddForce(base.transform.forward * 25.5f, ForceMode.Impulse);
			_baseCreep.Server_CrossFadeAnim("Attack01", 0f, 1);
			_baseCreep._attackActionBuffer = 1.25f;
			if (!((float)_statusEntity._currentHealth <= (float)_baseCreep._statStruct._maxHealth * 0.6f))
			{
				break;
			}
			yield return new WaitForSeconds(1.2f);
			int num = Random.Range(0, 2);
			if (num == 1)
			{
				Init_AoeAttackSwing();
				if ((float)_statusEntity._currentHealth <= (float)_baseCreep._statStruct._maxHealth * 0.3f)
				{
					yield return new WaitForSeconds(0.8f);
					Init_AoeAttackSwing();
				}
			}
			break;
		}
		case 2:
			_baseCreep.Server_CrossFadeAnim("Attack02", 0.05f, 1);
			_baseCreep._attackActionBuffer = 1.25f;
			yield return new WaitForSeconds(0.6f);
			break;
		}
		yield return new WaitForSeconds(_bufferBeforeNextPosition);
		_baseCreep.Network_ignoreAggroedTarget = true;
		_baseCreep._pathFindCtrl.CurrentNodeIndex += Random.Range(1, _baseCreep._pathFindCtrl._pathFindNodes.Length);
		_baseCreep._pathFindCtrl._target = _baseCreep._pathFindCtrl._pathFindNodes[_baseCreep._pathFindCtrl.CurrentNodeIndex];
		yield return new WaitForSeconds(0.35f);
		while (!_baseCreep._pathFindCtrl._isStoppedAndNearTarget)
		{
			yield return null;
		}
		_baseCreep.Network_ignoreAggroedTarget = false;
		yield return new WaitForSeconds(0.25f);
		_initBehavior = false;
	}

	private void Init_AoeAttackSwing()
	{
		_baseCreep.Server_CrossFadeAnim("Empty", 0f, 1);
		_baseCreep.Server_CrossFadeAnim("Attack01", 0f, 1);
	}

	public void SlimeDiva_AoeAttack()
	{
		if (!_baseCreep._ignoreAggroedTarget && (bool)_baseCreep.Network_aggroedEntity)
		{
			LayerMask layerMask = CastLayers.GroundMask;
			base.gameObject.scene.GetPhysicsScene().Raycast(_baseCreep.Network_aggroedEntity.transform.position, Vector3.down, out var hitInfo, float.PositiveInfinity, layerMask);
			GameObject obj = PoolManager._current.GenerateObject(_aoeAttackObject, string.Empty, 0f);
			ProjectileObject[] componentsInChildren = obj.GetComponentsInChildren<ProjectileObject>();
			obj.transform.SetParent(base.transform);
			obj.transform.SetParent(null);
			obj.transform.SetAsLastSibling();
			obj.transform.position = hitInfo.point;
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i]._parentTransform = base.transform;
			}
			_tittyBulletAsrc.pitch = 1f;
			_tittyBulletAsrc.PlayOneShot(_aoeAttackClip, 1f);
		}
	}

	public void SlimeDiva_OrbAttack()
	{
		if (!_baseCreep._ignoreAggroedTarget && (bool)_baseCreep.Network_aggroedEntity)
		{
			GameObject obj = PoolManager._current.GenerateObject(_slimeOrbPrefab, string.Empty, 0f);
			obj.GetComponentInChildren<ProjectileObject>()._parentTransform = base.transform;
			obj.GetComponentInChildren<CombatCollider>()._parentObject = base.gameObject;
			obj.transform.SetParent(base.transform);
			obj.transform.SetParent(null);
			obj.transform.SetAsLastSibling();
			obj.transform.SetPositionAndRotation(_slimeOrbSpawnpoint.position, _slimeOrbSpawnpoint.rotation);
			_tittyBulletAsrc.pitch = 0.95f;
			_tittyBulletAsrc.PlayOneShot(_aoeAttackClip, 1f);
		}
	}

	public void SlimeDiva_TitBulletAttack()
	{
		if (!_baseCreep._ignoreAggroedTarget && (bool)_baseCreep.Network_aggroedEntity)
		{
			StartCoroutine(TitBullet_Shoot());
		}
	}

	private IEnumerator TitBullet_Shoot()
	{
		int _shootAmount = _titBulletAmount;
		do
		{
			_baseCreep._attackActionBuffer = 0.85f;
			if (!_baseCreep.Network_aggroedEntity || _baseCreep._parryBuffer > 0f)
			{
				_baseCreep._creepAnim.CrossFade("Empty", 0f, 1);
				_shootAmount = 0;
			}
			GameObject gameObject = PoolManager._current.GenerateObject(_titBulletPrefab, string.Empty, 0f);
			gameObject.GetComponentInChildren<CombatCollider>()._parentObject = base.gameObject;
			gameObject.transform.SetParent(base.transform);
			gameObject.transform.SetParent(null);
			gameObject.transform.SetAsLastSibling();
			if (_shootAmount % 2 == 0)
			{
				gameObject.transform.SetPositionAndRotation(_titBulletShooter_L.position, _titBulletShooter_L.rotation);
			}
			else
			{
				gameObject.transform.SetPositionAndRotation(_titBulletShooter_R.position, _titBulletShooter_R.rotation);
			}
			_tittyBulletAsrc.pitch = 1f;
			_tittyBulletAsrc.Stop();
			_tittyBulletAsrc.Play();
			_shootAmount--;
			yield return new WaitForSeconds(_titBulletRate);
		}
		while (_shootAmount > 0);
		yield return new WaitForSeconds(0.45f);
		if (Random.Range(0, 2) == 1)
		{
			_tittyBulletAsrc.PlayOneShot(_laughClip, 1f);
		}
	}
}
