using System.Collections;
using UnityEngine;

public class CreepBehavior_LordBoar : CreepBehavior
{
	private enum LordBoarAction : byte
	{
		IDLE = 0,
		SIDLE = 1,
		FIRE = 2
	}

	[Header("LordBoar Behavior")]
	[SerializeField]
	private LordBoarAction _currentAction;

	[SerializeField]
	private float _timeBeforeNextAction = 1.2f;

	private float _currentActionBuffer;

	private float _nextActionBuffer;

	[Space]
	[Header("Sidle Behavior")]
	[SerializeField]
	private float _jumpSidle_force = 65.5f;

	[SerializeField]
	private float _backJump_force = 95.5f;

	[SerializeField]
	private float _sideJump_force = 45.5f;

	[Space]
	[Header("Projectiles / Hit Boxes")]
	[SerializeField]
	private Transform _aimer_cannon;

	[SerializeField]
	private Transform _aimer_fixedFront;

	[SerializeField]
	private GameObject _projectile_nature;

	[SerializeField]
	private GameObject _projectile_water;

	[SerializeField]
	private GameObject _projectile_normal;

	public override void Behavior_OnHurt()
	{
	}

	public override void Behavior_OnServerFixedUpdate()
	{
	}

	public override void Behavior_OnServerUpdate()
	{
		if (!_baseCreep.isServer)
		{
			return;
		}
		if (!_baseCreep.Network_aggroedEntity && _baseCreep._creepThreatSlots.Count <= 0)
		{
			Reset_Behavior();
			return;
		}
		if (_currentActionBuffer > 0f)
		{
			_currentActionBuffer -= Time.deltaTime;
		}
		else if (_currentActionBuffer < 0f)
		{
			_currentActionBuffer = 0f;
		}
		if (_currentAction == LordBoarAction.IDLE)
		{
			if (_nextActionBuffer < _timeBeforeNextAction)
			{
				_nextActionBuffer += Time.deltaTime;
				return;
			}
			Init_BoarLordAction();
			_nextActionBuffer = 0f;
		}
		else
		{
			_nextActionBuffer = 0f;
		}
	}

	public override void Reset_Behavior()
	{
		StopAllCoroutines();
		if (_currentAction != LordBoarAction.IDLE)
		{
			_baseCreep.Server_CrossFadeAnim("Empty", 0.095f, 1);
		}
		_currentAction = LordBoarAction.IDLE;
		_currentActionBuffer = Random.Range(0.65f, 1.85f);
		_baseCreep._pathFindCtrl._disablePathing = false;
		_baseCreep._pathFindCtrl.Network_dontLookAtTarget = false;
		_baseCreep.Network_ignoreAggroedTarget = false;
	}

	private void Init_BoarLordAction()
	{
		if (_baseCreep._currentCreepCondition == CreepCondition.ACTIVE && _currentAction == LordBoarAction.IDLE && _currentActionBuffer == 0f && (bool)_baseCreep.Network_aggroedEntity && _baseCreep.isServer)
		{
			StartCoroutine(BoarLordAction_Session());
		}
	}

	private IEnumerator BoarLordAction_Session()
	{
		_currentAction = LordBoarAction.SIDLE;
		_baseCreep._pathFindCtrl._disablePathing = true;
		if ((float)_baseCreep._statusEntity._currentHealth <= (float)_baseCreep._statStruct._maxHealth * 0.4f)
		{
			yield return new WaitForSeconds(Random.Range(0.68f, 1.45f));
		}
		else
		{
			yield return new WaitForSeconds(Random.Range(0.28f, 0.85f));
		}
		if (!_baseCreep._isParryStaggered)
		{
			int num = Random.Range(0, 3);
			_baseCreep._creepRB.AddForce(base.transform.up * _jumpSidle_force, ForceMode.Impulse);
			_baseCreep.Server_CrossFadeAnim("specialAction_01", 0f, 1);
			switch (num)
			{
			case 0:
				_baseCreep._creepRB.AddForce(-base.transform.forward * _backJump_force, ForceMode.Impulse);
				break;
			case 1:
				_baseCreep._creepRB.AddForce(-base.transform.forward * _backJump_force, ForceMode.Impulse);
				_baseCreep._creepRB.AddForce(-base.transform.right * _sideJump_force, ForceMode.Impulse);
				break;
			case 2:
				_baseCreep._creepRB.AddForce(-base.transform.forward * _backJump_force, ForceMode.Impulse);
				_baseCreep._creepRB.AddForce(base.transform.right * _sideJump_force, ForceMode.Impulse);
				break;
			}
			if ((float)_baseCreep._statusEntity._currentHealth <= (float)_baseCreep._statStruct._maxHealth * 0.5f && Random.Range(0, 3) >= 2)
			{
				yield return new WaitForSeconds(Random.Range(1.25f, 2.35f));
				_baseCreep._creepRB.AddForce(base.transform.up * _jumpSidle_force, ForceMode.Impulse);
				_baseCreep.Server_CrossFadeAnim("specialAction_01", 0f, 1);
				_baseCreep._creepRB.AddForce(base.transform.forward * _backJump_force, ForceMode.Impulse);
			}
			if ((float)_baseCreep._statusEntity._currentHealth <= (float)_baseCreep._statStruct._maxHealth * 0.3f && Random.Range(0, 3) >= 2)
			{
				yield return new WaitForSeconds(Random.Range(1.25f, 2.35f));
				_baseCreep._creepRB.AddForce(base.transform.up * _jumpSidle_force, ForceMode.Impulse);
				_baseCreep.Server_CrossFadeAnim("specialAction_01", 0f, 1);
				_baseCreep._creepRB.AddForce(base.transform.forward * _backJump_force, ForceMode.Impulse);
			}
		}
		yield return new WaitForSeconds(Random.Range(1.15f, 1.65f));
		if (!_baseCreep.Network_aggroedEntity)
		{
			Reset_Behavior();
			yield break;
		}
		_currentAction = LordBoarAction.FIRE;
		switch (Random.Range(0, 4))
		{
		case 0:
			_baseCreep.Server_CrossFadeAnim("Attack01", 0f, 1);
			yield return new WaitForSeconds(1.65f);
			_baseCreep._pathFindCtrl.Network_dontLookAtTarget = true;
			yield return new WaitForSeconds(3.65f);
			_baseCreep._pathFindCtrl.Network_dontLookAtTarget = false;
			if ((float)_baseCreep._statusEntity._currentHealth <= (float)_baseCreep._statStruct._maxHealth * 0.5f)
			{
				_baseCreep.Server_CrossFadeAnim("Distance_Attack", 0f, 1);
				yield return new WaitForSeconds(Random.Range(2.8f, 3.5f));
			}
			break;
		case 1:
			_baseCreep.Server_CrossFadeAnim("Attack02", 0f, 1);
			yield return new WaitForSeconds(Random.Range(2.8f, 3.5f));
			if ((float)_baseCreep._statusEntity._currentHealth <= (float)_baseCreep._statStruct._maxHealth * 0.4f)
			{
				_baseCreep.Server_CrossFadeAnim("Attack01", 0f, 1);
				yield return new WaitForSeconds(2.35f);
				_baseCreep._pathFindCtrl.Network_dontLookAtTarget = true;
				yield return new WaitForSeconds(Random.Range(2.45f, 3.25f));
				_baseCreep._pathFindCtrl.Network_dontLookAtTarget = false;
			}
			break;
		case 2:
			_baseCreep.Server_CrossFadeAnim("Attack03", 0f, 1);
			yield return new WaitForSeconds(Random.Range(2.8f, 3.5f));
			if ((float)_baseCreep._statusEntity._currentHealth <= (float)_baseCreep._statStruct._maxHealth * 0.6f)
			{
				_baseCreep.Server_CrossFadeAnim("Attack03", 0f, 1);
				yield return new WaitForSeconds(Random.Range(2.8f, 3.5f));
			}
			if ((float)_baseCreep._statusEntity._currentHealth <= (float)_baseCreep._statStruct._maxHealth * 0.3f)
			{
				_baseCreep.Server_CrossFadeAnim("Attack02", 0f, 1);
				yield return new WaitForSeconds(Random.Range(2.8f, 3.5f));
			}
			break;
		case 3:
			_baseCreep.Server_CrossFadeAnim("Distance_Attack", 0f, 1);
			yield return new WaitForSeconds(Random.Range(2.8f, 3.5f));
			if ((float)_baseCreep._statusEntity._currentHealth <= (float)_baseCreep._statStruct._maxHealth * 0.4f)
			{
				_baseCreep.Server_CrossFadeAnim("Attack03", 0f, 1);
				yield return new WaitForSeconds(Random.Range(2.8f, 3.5f));
			}
			if ((float)_baseCreep._statusEntity._currentHealth <= (float)_baseCreep._statStruct._maxHealth * 0.6f)
			{
				_baseCreep.Server_CrossFadeAnim("Distance_Attack", 0f, 1);
				yield return new WaitForSeconds(Random.Range(2.8f, 3.5f));
			}
			if ((float)_baseCreep._statusEntity._currentHealth <= (float)_baseCreep._statStruct._maxHealth * 0.3f)
			{
				_baseCreep.Server_CrossFadeAnim("Distance_Attack", 0f, 1);
				yield return new WaitForSeconds(Random.Range(2.8f, 3.5f));
			}
			break;
		}
		_baseCreep._pathFindCtrl.Network_dontLookAtTarget = false;
		_baseCreep._pathFindCtrl._disablePathing = false;
		yield return new WaitForSeconds(Random.Range(1.2f, 2.5f));
		Reset_Behavior();
	}

	public void Init_NatureProjectile()
	{
		GameObject obj = PoolManager._current.GenerateObject(_projectile_nature, string.Empty, 0f);
		ProjectileObject component = obj.GetComponent<ProjectileObject>();
		CombatCollider componentInChildren = obj.GetComponentInChildren<CombatCollider>();
		component._parentTransform = base.transform;
		componentInChildren._parentObject = base.gameObject;
		obj.transform.SetParent(base.transform);
		obj.transform.SetParent(null);
		obj.transform.SetAsLastSibling();
		obj.transform.SetPositionAndRotation(_aimer_cannon.position, Quaternion.Euler(0f, _aimer_cannon.eulerAngles.y, 0f));
	}

	public void Init_WaterProjectile()
	{
		GameObject obj = PoolManager._current.GenerateObject(_projectile_water, string.Empty, 0f);
		ProjectileObject component = obj.GetComponent<ProjectileObject>();
		CombatCollider componentInChildren = obj.GetComponentInChildren<CombatCollider>();
		component._parentTransform = base.transform;
		componentInChildren._parentObject = base.gameObject;
		obj.transform.SetParent(base.transform);
		obj.transform.SetParent(null);
		obj.transform.SetAsLastSibling();
		obj.transform.SetPositionAndRotation(_aimer_fixedFront.position, Quaternion.Euler(0f, _aimer_fixedFront.eulerAngles.y, 0f));
		if ((bool)_baseCreep.Network_aggroedEntity)
		{
			component._targetTransform = _baseCreep.Network_aggroedEntity.transform;
		}
	}

	public void Init_NormalProjectile()
	{
		GameObject obj = PoolManager._current.GenerateObject(_projectile_normal, string.Empty, 0f);
		ProjectileObject component = obj.GetComponent<ProjectileObject>();
		CombatCollider componentInChildren = obj.GetComponentInChildren<CombatCollider>();
		component._parentTransform = base.transform;
		componentInChildren._parentObject = base.gameObject;
		obj.transform.SetParent(base.transform);
		obj.transform.SetParent(null);
		obj.transform.SetAsLastSibling();
		obj.transform.SetPositionAndRotation(_aimer_fixedFront.position, Quaternion.Euler(0f, _aimer_fixedFront.eulerAngles.y, 0f));
	}
}
