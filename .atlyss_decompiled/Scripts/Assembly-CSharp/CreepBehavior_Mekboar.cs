using System.Collections;
using UnityEngine;

public class CreepBehavior_Mekboar : CreepBehavior
{
	[Header("Mekboar Behavior")]
	[SerializeField]
	private float _chargeRevUpTime = 1.5f;

	[SerializeField]
	private float _chargeRestBufferTime = 2.5f;

	[SerializeField]
	private float _chargeColliderDisableBuffer = 0.65f;

	[SerializeField]
	private float _chargeCooldown = 0.45f;

	[SerializeField]
	private ParticleSystem _chargeDustEffect;

	private bool _isChargeAttacking;

	private bool _chargeFocus;

	private float _currentChargeCooldown;

	[Header("Warboar Behavior")]
	[SerializeField]
	private bool _isWarBoar;

	[SerializeField]
	private float _warBoarSpinTime = 1.2f;

	[Header("Projectile Trail")]
	[SerializeField]
	private GameObject _projectileObject;

	public override void Server_Behavior_OnSpawn()
	{
	}

	public override void Behavior_OnHurt()
	{
	}

	public override void Reset_Behavior()
	{
		StopAllCoroutines();
		_baseCreep.Server_CrossFadeAnim("Empty", 0f, 1);
		_baseCreep._creepPushCollider.enabled = true;
		_baseCreep._pathFindCtrl.Network_dontLookAtTarget = false;
		_baseCreep._pathFindCtrl._disablePathing = false;
		_baseCreep._creepPushCollider.gameObject.SetActive(value: true);
		_isChargeAttacking = false;
		_chargeFocus = false;
		_currentChargeCooldown = _chargeCooldown;
		_chargeDustEffect.Stop();
	}

	public override void Behavior_OnServerUpdate()
	{
		if (_currentChargeCooldown > 0f)
		{
			_currentChargeCooldown -= Time.deltaTime;
		}
		else
		{
			_currentChargeCooldown = 0f;
		}
		if (_isChargeAttacking)
		{
			_baseCreep._attackActionBuffer = 0.85f;
		}
	}

	public override void Behavior_OnServerFixedUpdate()
	{
		if (_currentChargeCooldown > 0f && _isChargeAttacking)
		{
			Reset_Behavior();
			return;
		}
		if (_baseCreep._isKnockedBack)
		{
			Reset_Behavior();
			return;
		}
		if ((bool)_baseCreep.Network_aggroedEntity && !_isChargeAttacking && _currentChargeCooldown <= 0f && _baseCreep._pathFindCtrl._isStoppedAndNearTarget)
		{
			StartCoroutine(Mekboar_Charge());
		}
		if (_chargeFocus && _baseCreep._knockbackBuffer <= 0f)
		{
			_baseCreep._creepRB.velocity = new Vector3(0f, _baseCreep._creepRB.velocity.y, 0f);
		}
	}

	private IEnumerator Mekboar_Charge()
	{
		if (!_baseCreep._pathFindCtrl._isGrounded || !_baseCreep.Network_aggroedEntity || _isChargeAttacking || _onAggroedEntityBuffer)
		{
			yield break;
		}
		bool _warBoarSpin = false;
		_isChargeAttacking = true;
		_chargeFocus = true;
		_baseCreep._pathFindCtrl._disablePathing = true;
		if (_isWarBoar)
		{
			if ((float)_baseCreep._statusEntity._currentHealth <= (float)_baseCreep._statStruct._maxHealth * 0.4f)
			{
				_warBoarSpin = true;
			}
			else if (Random.Range(0, 5) >= 3)
			{
				_warBoarSpin = true;
			}
		}
		if (_warBoarSpin)
		{
			_baseCreep.Server_CrossFadeAnim("specialAction_01", 0f, 1);
			yield return new WaitForSeconds(_chargeRevUpTime + 0.58f);
		}
		else
		{
			_baseCreep.Server_CrossFadeAnim("Attack01", 0f, 1);
			yield return new WaitForSeconds(_chargeRevUpTime);
		}
		_chargeFocus = false;
		if (_warBoarSpin)
		{
			_baseCreep._pathFindCtrl._disablePathing = false;
			_baseCreep._pathFindCtrl.Network_dontLookAtTarget = true;
			_baseCreep.Server_CrossFadeAnim("Attack03", 0f, 1);
		}
		else
		{
			_baseCreep.Server_CrossFadeAnim("Attack02", 0f, 1);
		}
		yield return new WaitForSeconds(_chargeColliderDisableBuffer);
		yield return new WaitForSeconds(0.25f);
		_baseCreep._pathFindCtrl.Network_dontLookAtTarget = true;
		yield return new WaitForSeconds(_chargeRestBufferTime);
		if (_warBoarSpin)
		{
			_baseCreep.Server_CrossFadeAnim("Empty", 0.1f, 1);
			_baseCreep._parryBuffer = 1.24f;
		}
		else
		{
			_baseCreep.Server_CrossFadeAnim("Empty", 0f, 1);
		}
		_baseCreep._pathFindCtrl._disablePathing = false;
		_baseCreep._pathFindCtrl.Network_dontLookAtTarget = false;
		yield return new WaitForSeconds(1.25f);
		_currentChargeCooldown = _chargeCooldown;
		_isChargeAttacking = false;
	}

	public void Play_DustEffect()
	{
		_chargeDustEffect.Play();
	}

	public void Init_ProjectileTrail()
	{
		GameObject obj = PoolManager._current.GenerateObject(_projectileObject, "", 0f);
		obj.transform.position = base.transform.position - Vector3.up * 1f;
		obj.transform.SetParent(base.transform);
		obj.transform.SetParent(null);
	}
}
