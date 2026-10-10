using UnityEngine;

public abstract class CreepBehavior : MonoBehaviour
{
	[HideInInspector]
	public Creep _baseCreep;

	[HideInInspector]
	public StatusEntity _statusEntity;

	[HideInInspector]
	public ScriptableCreep _scriptCreep;

	private Collider[] _creepTriggerColliders;

	private CombatCollider[] _creepCombatColliders;

	[Header("General Behavior Params")]
	public float _resetBehaviorOnSpawnTime = 1.25f;

	public bool _attackAnimSmoothReset = true;

	[Header("Dashing")]
	[SerializeField]
	private float _distanceToDash = 45.5f;

	[SerializeField]
	private bool _dashIfNearTarget;

	[Header("Runtime")]
	public bool _onAggroedEntityBuffer;

	private bool IsBehaviorServer()
	{
		if (!_baseCreep.isServer)
		{
			return false;
		}
		return true;
	}

	public virtual void Server_Behavior_OnSpawn()
	{
		if (IsBehaviorServer())
		{
			Invoke("Reset_Behavior", _resetBehaviorOnSpawnTime);
		}
	}

	public virtual void Awake()
	{
		_baseCreep = GetComponentInParent<Creep>();
		_statusEntity = _baseCreep.GetComponent<StatusEntity>();
		_creepTriggerColliders = GetComponentsInChildren<Collider>(includeInactive: true);
		_creepCombatColliders = GetComponentsInChildren<CombatCollider>(includeInactive: true);
		_scriptCreep = _baseCreep._scriptCreep;
		for (int i = 0; i < _creepCombatColliders.Length; i++)
		{
			if ((bool)_creepCombatColliders[i]._collider)
			{
				_creepCombatColliders[i]._collider.enabled = false;
			}
		}
	}

	public virtual void Update()
	{
		if (!_baseCreep)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		if ((bool)_baseCreep.Network_aggroedEntity)
		{
			if ((bool)_baseCreep.Network_aggroedEntity._isPlayer)
			{
				Player isPlayer = _baseCreep.Network_aggroedEntity._isPlayer;
				_onAggroedEntityBuffer = isPlayer._pCombat._setCreepQueue._bufferTime > 0f;
			}
		}
		else
		{
			_onAggroedEntityBuffer = false;
		}
		if ((bool)_baseCreep._creepAnim)
		{
			_baseCreep._creepAnim.SetBool("_attackResetAnim", _attackAnimSmoothReset);
		}
		for (int i = 0; i < _creepTriggerColliders.Length; i++)
		{
			if ((!_creepTriggerColliders[i].TryGetComponent<CombatCollider>(out var component) || !(component != null)) && _creepTriggerColliders[i].gameObject.layer == LayerMask.NameToLayer("Creep_TriggerCollider"))
			{
				_creepTriggerColliders[i].enabled = _baseCreep._currentCreepCondition == CreepCondition.ACTIVE && !_baseCreep._disableTriggerColliders;
			}
		}
		for (int j = 0; j < _creepCombatColliders.Length; j++)
		{
			if ((bool)_creepCombatColliders[j]._collider)
			{
				if (_baseCreep._isParryStaggered)
				{
					_creepCombatColliders[j]._collider.enabled = false;
				}
				if ((bool)_scriptCreep._conditionSlot._scriptableCondition)
				{
					_creepCombatColliders[j]._scriptableConditionSlot = _scriptCreep._conditionSlot;
				}
			}
		}
		Server_HandleBufferingParams();
		void Server_HandleBufferingParams()
		{
			if (IsBehaviorServer() && !_baseCreep._isParryStaggered && !_baseCreep._isActionLocked && (bool)_baseCreep.Network_aggroedEntity)
			{
				Behavior_OnServerUpdate();
			}
		}
	}

	public void FixedUpdate()
	{
		if (IsBehaviorServer())
		{
			Behavior_OnServerFixedUpdate();
		}
	}

	public void Play_CreepAttackSound(int _value)
	{
		if (!Player._mainPlayer || !(Player._mainPlayer.gameObject.scene != base.gameObject.scene))
		{
			_baseCreep._creepDynamicASrc.PlayOneShot(_scriptCreep._generalCreepAudio._attackSfx[_value], 0.45f);
		}
	}

	public void VelocityDash_Forward(float _velocityAmount)
	{
		if (_baseCreep.isServer && (bool)_baseCreep.Network_aggroedEntity && !_baseCreep._ignoreAggroedTarget && !(_baseCreep._parryBuffer > 0f) && (_dashIfNearTarget || !_baseCreep._pathFindCtrl._isStoppedAndNearTarget) && (!(_distanceToDash > 0f) || !(Vector3.Distance(_baseCreep.transform.position, _baseCreep.Network_aggroedEntity.transform.position) > _distanceToDash)))
		{
			_baseCreep._pathFindCtrl.VelocityDash_Forward(_velocityAmount);
			if (!Player._mainPlayer || !(Player._mainPlayer.Network_playerMapInstance != _baseCreep._parentCreepSpawner._mapInstance))
			{
				_baseCreep._dashEffect.SetActive(value: true);
			}
		}
	}

	public abstract void Behavior_OnHurt();

	public abstract void Behavior_OnServerFixedUpdate();

	public abstract void Behavior_OnServerUpdate();

	public abstract void Reset_Behavior();
}
