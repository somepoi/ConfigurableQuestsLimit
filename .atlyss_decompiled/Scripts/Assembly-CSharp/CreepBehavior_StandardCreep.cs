using System.Collections.Generic;
using UnityEngine;

public class CreepBehavior_StandardCreep : CreepBehavior
{
	[Header("Standard Creep")]
	[SerializeField]
	private float _attackRate = 3f;

	[SerializeField]
	private bool _stopMovementWhileAttacking = true;

	[SerializeField]
	private bool _freezeRotationWhileAttacking;

	[SerializeField]
	private float _freezeRotationTimer = 1.15f;

	[SerializeField]
	private float _freezeMovementTimer = 0.85f;

	[SerializeField]
	private float _timeBeforeFreeze = 0.65f;

	private float _currentAttackRateBuffer;

	[Header("Distance Attack")]
	[SerializeField]
	private bool _canDistanceAttack;

	[SerializeField]
	private float _distanceAttackRadius = 15.85f;

	[SerializeField]
	private float _distanceAttackRate = 5f;

	[SerializeField]
	[Range(0f, 1f)]
	private float _chanceToDistanceAttack = 0.45f;

	[SerializeField]
	private float _distanceAttackTimeBeforeFreeze = 0.85f;

	private bool _initDistanceAttack;

	private float _distanceAttackChanceBuffer;

	[Header("Projectiles")]
	public Transform _projectileAimer;

	[SerializeField]
	private GameObject[] _projectiles;

	[Header("Skills")]
	[SerializeField]
	private Creep_SkillCastSlot[] _creepSkillCastSlots = new Creep_SkillCastSlot[0];

	public Transform[] _creepCastEffectPositions = new Transform[0];

	public List<ParticleSystem> _cachedSkillChargeEffects = new List<ParticleSystem>();

	public ScriptableSkill _currentCastSkill;

	public float _castTime;

	public float _setCastTimeModifier;

	private float _castTimeChanceBuffer;

	[Header("Physics")]
	private float _pushForwardBuffer;

	private void OnValidate()
	{
		for (int i = 0; i < _creepSkillCastSlots.Length; i++)
		{
			if (!_creepSkillCastSlots[i]._scriptableSkill)
			{
				_creepSkillCastSlots[i]._castSlotTag = "No Skill Assigned";
				continue;
			}
			_creepSkillCastSlots[i]._castSlotTag = _creepSkillCastSlots[i]._scriptableSkill._skillName + " /" + $" Chance: {_creepSkillCastSlots[i]._chanceToCast * 100f}% /" + $" HP Range: {_creepSkillCastSlots[i]._healthPercentToCast * 100f}% /" + $" Cooldown: {_creepSkillCastSlots[i]._baseCooldownTime}";
		}
	}

	public override void Server_Behavior_OnSpawn()
	{
		base.Server_Behavior_OnSpawn();
		_currentAttackRateBuffer = _attackRate;
	}

	public override void Update()
	{
		base.Update();
		if (_baseCreep._isParryStaggered)
		{
			_currentAttackRateBuffer = 0.6f;
		}
		Handle_ProjectileAimer();
		Handle_CurrentCastSkill();
		void Handle_CurrentCastSkill()
		{
			_baseCreep._creepAnim.SetBool("_isCastingSkill", _currentCastSkill);
			if ((bool)_currentCastSkill && (_baseCreep._isParryStaggered || _baseCreep._currentCreepCondition == CreepCondition.DEAD || !_baseCreep.Network_aggroedEntity))
			{
				_baseCreep.Server_CrossFadeAnim("Empty", 0f, 1);
				_currentCastSkill = null;
			}
			if ((bool)_currentCastSkill)
			{
				_castTime += Time.deltaTime;
			}
			else
			{
				_castTime = 0f;
			}
			if (!_currentCastSkill || _baseCreep._isParryStaggered || _baseCreep._currentCreepCondition == CreepCondition.DEAD)
			{
				_baseCreep._creepSkillChargeASrc.Stop();
				for (int i = 0; i < _cachedSkillChargeEffects.Count; i++)
				{
					_cachedSkillChargeEffects[i].Stop();
					_cachedSkillChargeEffects.RemoveAt(i);
				}
			}
			for (int j = 0; j < _creepSkillCastSlots.Length; j++)
			{
				if (_baseCreep._currentCreepCondition == CreepCondition.DEAD)
				{
					_creepSkillCastSlots[j]._currentCooldownTime = 0f;
				}
				if (_creepSkillCastSlots[j]._currentCooldownTime > 0f)
				{
					_creepSkillCastSlots[j]._currentCooldownTime -= Time.deltaTime;
				}
				else
				{
					_creepSkillCastSlots[j]._currentCooldownTime = 0f;
				}
			}
		}
		void Handle_ProjectileAimer()
		{
			if ((bool)_projectileAimer && (bool)_baseCreep.Network_aggroedEntity)
			{
				Vector3 worldPosition = _baseCreep.Network_aggroedEntity.transform.position + Vector3.up * 1.5f;
				_projectileAimer.LookAt(worldPosition);
			}
		}
	}

	public override void Behavior_OnServerFixedUpdate()
	{
		if (CanPushForward())
		{
			if (Random.Range(0, 12) >= 8)
			{
				_baseCreep._creepRB.AddForce(base.transform.forward * 35.5f, ForceMode.Impulse);
			}
			_pushForwardBuffer = Random.Range(1.25f, 3.25f);
		}
		bool CanPushForward()
		{
			if (!_baseCreep.Network_aggroedEntity)
			{
				return false;
			}
			if (_baseCreep._pathFindCtrl._isStoppedAndNearTarget)
			{
				return false;
			}
			if (_baseCreep._parryBuffer > 0f)
			{
				return false;
			}
			if (_currentAttackRateBuffer > 0f)
			{
				return false;
			}
			if (_pushForwardBuffer > 0f)
			{
				return false;
			}
			return true;
		}
	}

	public void StandardCreep_OnAttack()
	{
		if (_onAggroedEntityBuffer || (bool)_currentCastSkill)
		{
			return;
		}
		if (_initDistanceAttack)
		{
			if (!(_distanceAttackChanceBuffer <= 0f))
			{
				return;
			}
			float num = Random.Range(0f, 1f);
			_distanceAttackChanceBuffer = 1.7f;
			if (!(num > _chanceToDistanceAttack))
			{
				if (IsInvoking("Init_FreezeTransforms"))
				{
					CancelInvoke("Init_FreezeTransforms");
				}
				Invoke("Init_FreezeTransforms", _distanceAttackTimeBeforeFreeze);
				_baseCreep.Server_CrossFadeAnim("Distance_Attack", 0f, 1);
				_currentAttackRateBuffer = _distanceAttackRate;
				_baseCreep._attackActionBuffer = 1.25f;
			}
			return;
		}
		if (_creepSkillCastSlots.Length != 0 && _castTimeChanceBuffer <= 0f)
		{
			for (int i = 0; i < _creepSkillCastSlots.Length; i++)
			{
				if (!((float)_baseCreep._statusEntity._currentHealth > (float)_baseCreep._statStruct._maxHealth * _creepSkillCastSlots[i]._healthPercentToCast))
				{
					float num2 = Random.Range(0f, 1f);
					_distanceAttackChanceBuffer = 2.5f;
					if (!(num2 > _creepSkillCastSlots[i]._chanceToCast) && !_creepSkillCastSlots[i]._scriptableSkill.Cannot_CastSkill(_statusEntity) && !(_creepSkillCastSlots[i]._currentCooldownTime > 0f))
					{
						_currentCastSkill = _creepSkillCastSlots[i]._scriptableSkill;
						_creepSkillCastSlots[i]._currentCooldownTime = _creepSkillCastSlots[i]._baseCooldownTime;
						_setCastTimeModifier = _creepSkillCastSlots[i]._castTimeModifier;
						_baseCreep.Rpc_InitSkillChargeEffect(_currentCastSkill._skillName, _setCastTimeModifier);
						_baseCreep.Server_CrossFadeAnim("Skill_Charge", 0.24f, 1);
						return;
					}
				}
			}
		}
		if (IsInvoking("Init_FreezeTransforms"))
		{
			CancelInvoke("Init_FreezeTransforms");
		}
		Invoke("Init_FreezeTransforms", _timeBeforeFreeze);
		_baseCreep.Server_CrossFadeAnim($"Attack0{Random.Range(1, 4)}", 0.1f, 1);
		_baseCreep._attackActionBuffer = 1.25f;
		_baseCreep._pathFindCtrl._currentMovSpeed = _baseCreep._pathFindCtrl._baseMovSpeed;
		_currentAttackRateBuffer = _attackRate;
	}

	private void Stop_Movement()
	{
		_baseCreep._pathFindCtrl.Disable_Pathfinding(_freezeMovementTimer);
		if (!_baseCreep._creepRB.isKinematic)
		{
			_baseCreep._creepRB.velocity = new Vector3(0f, _baseCreep._creepRB.velocity.y, 0f);
		}
	}

	private void Init_FreezeTransforms()
	{
		if (_stopMovementWhileAttacking)
		{
			Stop_Movement();
		}
		if (_freezeRotationWhileAttacking)
		{
			_baseCreep._pathFindCtrl.Disable_LookRotation(_freezeRotationTimer);
		}
	}

	public override void Behavior_OnHurt()
	{
	}

	public override void Reset_Behavior()
	{
		if (IsInvoking("Init_FreezeTransforms"))
		{
			CancelInvoke("Init_FreezeTransforms");
		}
		if (!_currentCastSkill)
		{
			_baseCreep.Server_CrossFadeAnim("Empty", 0f, 1);
		}
		_currentCastSkill = null;
		_castTime = 0f;
		_currentAttackRateBuffer = 0.65f;
		_distanceAttackChanceBuffer = 0.65f;
		_pushForwardBuffer = 0f;
	}

	public override void Behavior_OnServerUpdate()
	{
		Handle_AttackRateBuffering();
		Handle_AttackConditioning();
		void Handle_AttackConditioning()
		{
			if (!_baseCreep._pathFindCtrl._isStoppedAndNearTarget)
			{
				if (_canDistanceAttack && _baseCreep._pathFindCtrl._distanceToTarget <= _distanceAttackRadius && _currentAttackRateBuffer <= 0f)
				{
					_initDistanceAttack = true;
					StandardCreep_OnAttack();
				}
			}
			else
			{
				_initDistanceAttack = false;
				if (!(_baseCreep._pathFindCtrl._target != _baseCreep.Network_aggroedEntity.transform) && !(_baseCreep._knockbackBuffer > 0f) && !_currentCastSkill && _currentAttackRateBuffer <= 0f)
				{
					StandardCreep_OnAttack();
				}
			}
		}
		void Handle_AttackRateBuffering()
		{
			if (_distanceAttackChanceBuffer > 0f)
			{
				_distanceAttackChanceBuffer -= Time.deltaTime;
			}
			else if (_distanceAttackChanceBuffer < 0f)
			{
				_distanceAttackChanceBuffer = 0f;
			}
			if (_castTimeChanceBuffer > 0f)
			{
				_castTimeChanceBuffer -= Time.deltaTime;
			}
			else if (_castTimeChanceBuffer < 0f)
			{
				_castTimeChanceBuffer = 0f;
			}
			if (_currentAttackRateBuffer > 0f)
			{
				_currentAttackRateBuffer -= Time.deltaTime;
			}
			else if (_currentAttackRateBuffer < 0f)
			{
				_currentAttackRateBuffer = 0f;
			}
			if (_pushForwardBuffer > 0f)
			{
				_pushForwardBuffer -= Time.deltaTime;
			}
			else if (_pushForwardBuffer < 0f)
			{
				_pushForwardBuffer = 0f;
			}
			if ((bool)_currentCastSkill)
			{
				if (_castTime >= _currentCastSkill._skillRankParams._baseCastTime + _setCastTimeModifier)
				{
					_baseCreep.Server_CrossFadeAnim("Skill_Cast", 0f, 1);
					_baseCreep.Rpc_InitSkillCastEffect(_currentCastSkill._skillName);
					_currentCastSkill.Init_SkillBehavior(_baseCreep._statusEntity);
					_castTime = 0f;
					_currentCastSkill = null;
				}
				Stop_Movement();
			}
		}
	}

	public void CombatCreep_ShootProjectile(int _index)
	{
		Init_CreepProjectile(_index, _ignoreYAxis: false);
	}

	public void CombatCreep_ShootProjectile_NoYAxis(int _index)
	{
		Init_CreepProjectile(_index, _ignoreYAxis: true);
	}

	private void Init_CreepProjectile(int _index, bool _ignoreYAxis)
	{
		if (!_baseCreep._ignoreAggroedTarget && (bool)_baseCreep.Network_aggroedEntity && !_baseCreep._isParryStaggered)
		{
			GameObject requestedObject = _projectiles[_index];
			GameObject gameObject = PoolManager._current.GenerateObject(requestedObject, string.Empty, 0f);
			gameObject.GetComponentInChildren<ProjectileObject>()._parentTransform = base.transform;
			if ((bool)gameObject.GetComponentInChildren<CombatCollider>())
			{
				CombatCollider componentInChildren = gameObject.GetComponentInChildren<CombatCollider>();
				componentInChildren._parentObject = base.gameObject;
				componentInChildren._combatElement = _baseCreep._combatElement;
			}
			gameObject.transform.SetParent(base.transform);
			gameObject.transform.SetParent(null);
			gameObject.transform.SetAsLastSibling();
			if (_ignoreYAxis)
			{
				gameObject.transform.SetPositionAndRotation(_projectileAimer.position, base.transform.rotation);
			}
			else
			{
				gameObject.transform.SetPositionAndRotation(_projectileAimer.position, _projectileAimer.rotation);
			}
		}
	}
}
