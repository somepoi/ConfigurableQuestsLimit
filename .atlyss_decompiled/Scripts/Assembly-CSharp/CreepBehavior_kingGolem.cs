using System.Collections;
using UnityEngine;

public class CreepBehavior_kingGolem : CreepBehavior
{
	private enum KingGolemState : byte
	{
		ASLEEP = 0,
		WAKING_UP = 1,
		IDLE = 2,
		SHOCKWAVE = 3,
		RAIL = 4,
		DASHCLAP = 5,
		DEAD = 6
	}

	[Header("King Golem Behavior")]
	[SerializeField]
	private KingGolemState _currentState;

	[Header("Sleep Condition")]
	[SerializeField]
	private Collider _wakeupCollider;

	[SerializeField]
	private Transform _triggerPlayerTransform;

	private bool _wakeUpColliderTriggered;

	[Header("Shockwave Attack")]
	[SerializeField]
	private Transform _shockwaveAimerPos;

	[SerializeField]
	private GameObject _shockwavePrefab;

	[Header("Eye Rail Attack")]
	[SerializeField]
	private Transform _focusedRailTarget;

	[SerializeField]
	private float _backForceBeforeRail = 85.5f;

	[SerializeField]
	private float _timeBeforeRail = 3.5f;

	[SerializeField]
	private float _timeBeforeRotationFreeze = 2f;

	[SerializeField]
	private float _timeBEforeRotationUnlock = 2f;

	[Header("Dash Clap Attack")]
	[SerializeField]
	private float _timeBeforeDash_highHealth = 2.3f;

	[SerializeField]
	private float _timeBeforeDash_lowHealth = 1.6f;

	[SerializeField]
	private float _dashForce = 250f;

	[SerializeField]
	private float _distanceToClapAttack = 30.5f;

	private float _currentDistanceToTarget;

	private bool _summonedMinions;

	private bool _minionSummonMsg;

	private void Start()
	{
		Init_KingGolemState(KingGolemState.ASLEEP);
	}

	public override void Update()
	{
		if (_baseCreep.isServer)
		{
			Handle_KingGolemServerParams();
		}
		_wakeupCollider.enabled = _baseCreep._disableTriggerColliders;
		base.Update();
	}

	public override void Behavior_OnServerUpdate()
	{
		if (_baseCreep._statusEntity._currentHealth <= 0 || _baseCreep._currentCreepCondition != CreepCondition.ACTIVE || _currentState != KingGolemState.IDLE)
		{
			return;
		}
		int num = Random.Range(0, 4);
		if (_currentDistanceToTarget > 250f)
		{
			Init_KingGolemState(KingGolemState.DASHCLAP);
			return;
		}
		switch (num)
		{
		case 1:
			Init_KingGolemState(KingGolemState.SHOCKWAVE);
			break;
		case 2:
			Init_KingGolemState(KingGolemState.RAIL);
			break;
		case 3:
			Init_KingGolemState(KingGolemState.DASHCLAP);
			break;
		}
	}

	public override void Behavior_OnServerFixedUpdate()
	{
	}

	public override void Behavior_OnHurt()
	{
	}

	private void Init_KingGolemState(KingGolemState _setstate)
	{
		_currentState = _setstate;
		switch (_setstate)
		{
		case KingGolemState.ASLEEP:
			_baseCreep._creepAnim.CrossFade("alt_action", 0f, 1);
			if (_baseCreep.isServer)
			{
				_baseCreep._suppressWandererBehavior = true;
			}
			break;
		case KingGolemState.WAKING_UP:
			if (_baseCreep.isServer)
			{
				_baseCreep.Server_ApplyCreepStats();
				_baseCreep.Server_CrossFadeAnim("specialAction_01", 0.2f, 1);
			}
			break;
		case KingGolemState.IDLE:
			if (_baseCreep.isServer)
			{
				_baseCreep._wanderBehavior.enabled = true;
				_baseCreep._suppressWandererBehavior = false;
				_baseCreep._pathFindCtrl.Network_dontLookAtTarget = false;
				_baseCreep.Network_ignoreAggroedTarget = false;
				_focusedRailTarget = null;
				StopAllCoroutines();
			}
			break;
		case KingGolemState.SHOCKWAVE:
			if (_baseCreep.isServer)
			{
				StartCoroutine(Init_Shockwave());
			}
			break;
		case KingGolemState.RAIL:
			if (_baseCreep.isServer)
			{
				StartCoroutine(Init_Rail());
			}
			break;
		case KingGolemState.DASHCLAP:
			if (_baseCreep.isServer)
			{
				StartCoroutine(Init_Dashclap());
			}
			break;
		}
	}

	private IEnumerator Init_Shockwave()
	{
		if (_currentState != KingGolemState.SHOCKWAVE)
		{
			yield break;
		}
		yield return new WaitForSeconds(2.5f);
		_baseCreep.Network_ignoreAggroedTarget = true;
		_baseCreep._pathFindCtrl._disablePathing = true;
		_baseCreep._pathFindCtrl.Network_dontLookAtTarget = false;
		_baseCreep.Server_CrossFadeAnim("Attack01", 0f, 1);
		if ((float)_statusEntity._currentHealth <= (float)_baseCreep._statStruct._maxHealth * 0.6f)
		{
			yield return new WaitForSeconds(1.5f);
			_baseCreep.Server_CrossFadeAnim("Empty", 0f, 1);
			yield return new WaitForSeconds(0.4f);
			_baseCreep.Server_CrossFadeAnim("Attack01", 0f, 1);
			if ((float)_statusEntity._currentHealth <= (float)_baseCreep._statStruct._maxHealth * 0.1f)
			{
				yield return new WaitForSeconds(1.5f);
				_baseCreep.Server_CrossFadeAnim("Empty", 0f, 1);
				yield return new WaitForSeconds(2f);
				_baseCreep.Server_CrossFadeAnim("Attack01", 0f, 1);
			}
		}
		if (Random.Range(0, 5) == 4 && (float)_statusEntity._currentHealth <= (float)_baseCreep._statStruct._maxHealth * 0.4f)
		{
			Init_KingGolemState(KingGolemState.RAIL);
			yield break;
		}
		yield return new WaitForSeconds(2.5f);
		Init_KingGolemState(KingGolemState.IDLE);
		_baseCreep.Network_ignoreAggroedTarget = false;
		_baseCreep._pathFindCtrl._disablePathing = false;
	}

	private IEnumerator Init_Rail()
	{
		if (_currentState != KingGolemState.RAIL)
		{
			yield break;
		}
		if (!_focusedRailTarget)
		{
			if (!_baseCreep.Network_aggroedEntity)
			{
				yield break;
			}
			_focusedRailTarget = _baseCreep.Network_aggroedEntity.transform;
		}
		yield return new WaitForSeconds(_timeBeforeRail);
		_baseCreep.Network_ignoreAggroedTarget = true;
		_baseCreep._pathFindCtrl._disablePathing = false;
		switch (Random.Range(0, 5))
		{
		case 2:
			_baseCreep._creepRB.AddForce(base.transform.forward * (0f - _backForceBeforeRail), ForceMode.Impulse);
			break;
		case 3:
			_baseCreep._creepRB.AddForce(base.transform.right * (0f - _backForceBeforeRail), ForceMode.Impulse);
			break;
		case 1:
			_baseCreep._creepRB.AddForce(base.transform.right * _backForceBeforeRail, ForceMode.Impulse);
			break;
		}
		_baseCreep._pathFindCtrl._disablePathing = true;
		_baseCreep.Server_CrossFadeAnim("Attack02", 0f, 1);
		yield return new WaitForSeconds(_timeBeforeRotationFreeze);
		_baseCreep._pathFindCtrl.Network_dontLookAtTarget = true;
		yield return new WaitForSeconds(_timeBEforeRotationUnlock);
		if ((float)_statusEntity._currentHealth <= (float)_baseCreep._statStruct._maxHealth * 0.5f)
		{
			_baseCreep._pathFindCtrl.Network_dontLookAtTarget = false;
			if (Random.Range(0, 3) == 1)
			{
				Init_KingGolemState(KingGolemState.SHOCKWAVE);
				yield break;
			}
			_baseCreep.Server_CrossFadeAnim("Empty", 0f, 1);
			yield return new WaitForSeconds(0.4f);
			_baseCreep.Server_CrossFadeAnim("Attack02", 0f, 1);
			yield return new WaitForSeconds(_timeBeforeRotationFreeze);
			_baseCreep._pathFindCtrl.Network_dontLookAtTarget = true;
			yield return new WaitForSeconds(_timeBEforeRotationUnlock);
		}
		Init_KingGolemState(KingGolemState.IDLE);
		_baseCreep._pathFindCtrl.Network_dontLookAtTarget = false;
		_baseCreep._pathFindCtrl._disablePathing = false;
		_focusedRailTarget = null;
	}

	private IEnumerator Init_Dashclap()
	{
		if (_currentState != KingGolemState.DASHCLAP)
		{
			yield break;
		}
		yield return new WaitForSeconds(2.5f);
		_baseCreep._pathFindCtrl._disablePathing = true;
		_baseCreep.Server_CrossFadeAnim("specialAction_00", 0f, 1);
		if ((float)_statusEntity._currentHealth <= (float)_baseCreep._statStruct._maxHealth * 0.5f)
		{
			yield return new WaitForSeconds(_timeBeforeDash_lowHealth);
		}
		else
		{
			yield return new WaitForSeconds(_timeBeforeDash_highHealth);
		}
		_baseCreep._pathFindCtrl._disablePathing = false;
		_baseCreep.Server_CrossFadeAnim("Distance_Attack", 0f, 1);
		while (_currentDistanceToTarget > _distanceToClapAttack)
		{
			_baseCreep._creepRB.AddForce(base.transform.forward * _dashForce, ForceMode.Impulse);
			_baseCreep._creepRB.velocity = new Vector3(_baseCreep._creepRB.velocity.x, 5f, _baseCreep._creepRB.velocity.z);
			if (!_baseCreep.Network_aggroedEntity)
			{
				yield break;
			}
			yield return null;
		}
		_baseCreep._pathFindCtrl._disablePathing = true;
		_baseCreep._pathFindCtrl.Network_dontLookAtTarget = true;
		_baseCreep._creepRB.velocity = Vector3.zero;
		yield return new WaitForSeconds(0.16f);
		_baseCreep.Server_CrossFadeAnim("Attack03", 0f, 1);
		yield return new WaitForSeconds(0.8f);
		if ((float)_statusEntity._currentHealth <= (float)_baseCreep._statStruct._maxHealth * 0.4f)
		{
			_baseCreep._pathFindCtrl.Network_dontLookAtTarget = false;
			Init_KingGolemState(KingGolemState.SHOCKWAVE);
		}
		else
		{
			Init_KingGolemState(KingGolemState.IDLE);
			_baseCreep._pathFindCtrl.Network_dontLookAtTarget = false;
			_baseCreep._pathFindCtrl._disablePathing = false;
		}
	}

	public void Spawn_Shockwave()
	{
		GameObject gameObject = PoolManager._current.GenerateObject(_shockwavePrefab, string.Empty, 0f);
		gameObject.GetComponentInChildren<ProjectileObject>()._parentTransform = base.transform;
		if ((bool)gameObject.GetComponentInChildren<CombatCollider>())
		{
			gameObject.GetComponentInChildren<CombatCollider>()._parentObject = base.gameObject;
		}
		gameObject.transform.SetParent(base.transform);
		gameObject.transform.SetParent(null);
		gameObject.transform.SetAsLastSibling();
		gameObject.transform.SetPositionAndRotation(_shockwaveAimerPos.position, _shockwaveAimerPos.rotation);
	}

	private void Handle_KingGolemServerParams()
	{
		if (_baseCreep._statusEntity._currentHealth <= 0 && _currentState != KingGolemState.DEAD)
		{
			_baseCreep.Server_CrossFadeAnim("Empty", 0f, 1);
			_currentState = KingGolemState.DEAD;
			return;
		}
		if (_currentState == KingGolemState.WAKING_UP && (bool)_triggerPlayerTransform)
		{
			Vector3 forward = _triggerPlayerTransform.position - base.transform.position;
			forward.y = 0f;
			_baseCreep.transform.rotation = Quaternion.Lerp(_baseCreep.transform.rotation, Quaternion.LookRotation(forward), 2.5f * Time.deltaTime);
		}
		if (_currentState == KingGolemState.ASLEEP || _currentState == KingGolemState.WAKING_UP)
		{
			_baseCreep.Network_isTargetable = false;
			_baseCreep._disableAggro = true;
			_baseCreep.Network_disableTriggerColliders = true;
			return;
		}
		if (_currentState == KingGolemState.IDLE)
		{
			_baseCreep.Network_isTargetable = true;
			_baseCreep._disableAggro = false;
			_baseCreep.Network_disableTriggerColliders = false;
		}
		if (_baseCreep._isParryStaggered)
		{
			_baseCreep.Server_CrossFadeAnim("Empty", 0f, 1);
		}
		if (!_baseCreep.Network_aggroedEntity && _baseCreep._creepThreatSlots.Count <= 0)
		{
			Reset_Behavior();
		}
		else
		{
			if (_baseCreep._currentCreepCondition != CreepCondition.ACTIVE)
			{
				return;
			}
			if ((bool)_baseCreep.Network_aggroedEntity)
			{
				_currentDistanceToTarget = Vector3.Distance(base.transform.position, _baseCreep.Network_aggroedEntity.transform.position);
			}
			else if (_currentState != KingGolemState.IDLE && _baseCreep._parentCreepSpawner._playersWithinSpawnerRadius.Count <= 0)
			{
				Init_KingGolemState(KingGolemState.IDLE);
			}
			if (!((float)_baseCreep._statusEntity._currentHealth <= (float)_baseCreep._statStruct._maxHealth * 0.5f) || _summonedMinions)
			{
				return;
			}
			for (int i = 0; i < _baseCreep._parentCreepSpawner._linkedSpawners.Length; i++)
			{
				_baseCreep._parentCreepSpawner._linkedSpawners[i].Enable_Spawner();
			}
			if (!_minionSummonMsg)
			{
				string[] array = new string[6] { "He can't have all the fun, you know!", "It seems that he needs our assistance, brother.", "Strong, isn't he? We made him ourselves!", "We're here to crash the party!", "Our construct needs some help, brother...", "How nice, there's adventurers fighting our creation!" };
				for (int j = 0; j < _baseCreep._parentCreepSpawner._playersWithinSpawnerRadius.Count; j++)
				{
					_baseCreep._parentCreepSpawner._playersWithinSpawnerRadius[j]._chatBehaviour.Target_RecieveTriggerMessage(array[Random.Range(0, array.Length)]);
				}
				_minionSummonMsg = true;
			}
			else
			{
				string[] array2 = new string[5] { "Back for more, eh?", "They're back again, brother.", "You think you have another chance?", "We'll be happy to make you suffer again!", "How pitiful!" };
				for (int k = 0; k < _baseCreep._parentCreepSpawner._playersWithinSpawnerRadius.Count; k++)
				{
					_baseCreep._parentCreepSpawner._playersWithinSpawnerRadius[k]._chatBehaviour.Target_RecieveTriggerMessage(array2[Random.Range(0, array2.Length)]);
				}
			}
			_summonedMinions = true;
		}
	}

	public override void Reset_Behavior()
	{
		if (!_baseCreep.isServer || !_wakeUpColliderTriggered)
		{
			return;
		}
		if ((_baseCreep._parentCreepSpawner._playersWithinSpawnerRadius.Count <= 0 || _baseCreep._statusEntity._currentHealth <= 0) && _baseCreep._parentCreepSpawner._spawnedCreeps.Count > 0 && _summonedMinions)
		{
			for (int i = 0; i < _baseCreep._parentCreepSpawner._linkedSpawners.Length; i++)
			{
				_baseCreep._parentCreepSpawner._linkedSpawners[i].Disable_Spawner();
			}
			_summonedMinions = false;
		}
		if (_currentState == KingGolemState.DASHCLAP)
		{
			_baseCreep.Server_CrossFadeAnim("Empty", 0f, 1);
			StopCoroutine("Init_Dashclap");
		}
		Init_KingGolemState(KingGolemState.IDLE);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (_baseCreep.isServer && !(other.gameObject.scene != base.gameObject.scene) && _currentState == KingGolemState.ASLEEP && !_wakeUpColliderTriggered && (bool)other.gameObject.GetComponentInParent<Player>())
		{
			Player componentInParent = other.gameObject.GetComponentInParent<Player>();
			if (componentInParent._currentPlayerCondition == PlayerCondition.ACTIVE && !componentInParent._bufferingStatus)
			{
				_triggerPlayerTransform = componentInParent.transform;
				_wakeUpColliderTriggered = true;
				StartCoroutine(Init_WakeUp());
			}
		}
	}

	private IEnumerator Init_WakeUp()
	{
		yield return new WaitForSeconds(3f);
		Init_KingGolemState(KingGolemState.WAKING_UP);
		yield return new WaitForSeconds(3.2f);
		Init_KingGolemState(KingGolemState.IDLE);
	}
}
