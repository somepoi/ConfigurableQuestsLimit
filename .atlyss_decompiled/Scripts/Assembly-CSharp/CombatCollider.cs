using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class CombatCollider : MonoBehaviour
{
	[Header("Runtime")]
	public GameObject _parentObject;

	public ScriptableWeapon _scriptWeapon;

	[Space]
	public SkillObject _skillObject;

	public ProjectileObject _projectileObject;

	public ConditionObject _conditionObject;

	[Space]
	public PlayerStats _isParentPlayer;

	private PlayerStats _hitPlayer;

	public Creep _isParentCreep;

	private Creep _hitCreep;

	[Space]
	[HideInInspector]
	public float _setPhysCritChance;

	[HideInInspector]
	public float _setMagicCritChance;

	[Space]
	public bool _disableCollider;

	public List<DamageBuffer> _damageBufferPool = new List<DamageBuffer>();

	private readonly List<DamageBuffer> _spawnObjectBuffers = new List<DamageBuffer>();

	[Header("Damage Collider Params")]
	public DamageType _dmgType;

	public CombatColliderType _colliderType;

	public ScriptableCombatElement _combatElement;

	public Collider _collider;

	[Header("General Flags")]
	[Tooltip("If true, this hitbox will apply to the parent player or creep when they instantiate it. Typically used for support skill objects for healing or buffs.")]
	public bool _applyToParent;

	[Tooltip("If true, this hitbox only works clientside. Typically used for traps and hazards since they are not netsynced.")]
	public bool _clientsideHitbox;

	private float _clientSideBuffer;

	[Tooltip("If true, this hitbox can stagger an enemy if parried. Projectile objects will disregard this.")]
	public bool _canParryStagger = true;

	[Tooltip("If true, this hitbox damage can be reflected back to the parent of this hitbox. Projectile objects will auto disable this.")]
	public bool _canReflect = true;

	[Tooltip("Resets the critical strike chance roll each time this hitbox re-enables. Should default to true.")]
	public bool _resetCritChanceOnEnable = true;

	[Tooltip("Ignores critical strike chance for both physical and magical counts.")]
	public bool _ignoreCriticalChance;

	[Tooltip("Ignores evasion on all counts.")]
	public bool _ignoreEvasionChance;

	[Header("Player Flags")]
	[Tooltip("Primarily for player characters. Could be possible to have creeps be affected by this as well.")]
	public bool _disableOnHurt;

	[Tooltip("Ignores I frames on player characters.")]
	public bool _ignoreIFrames;

	[Tooltip("Primarily for player characters. Players won't be able to block this attack.")]
	public bool _ignoreBlock;

	[Tooltip("Shatters the player character's blocking / removes all stamina.")]
	public bool _shatterBlock;

	[Header("Creep Flags")]
	[Tooltip("Damage creeps. This is usually left on, and will ignore hurting other creeps if the parent of this hitbox is a creep.")]
	public bool _damageCreeps = true;

	[Tooltip("Disables inheritance of the creep's elemental stat when enabled.")]
	public bool _disableCreepElement;

	[Tooltip("Primarily for creeps. There will not be a chance to hitstun them from this attack.")]
	public bool _noHitstunChance;

	[Tooltip("If true, threat will be applied to creeps even if the collider misses / deals no damage to them.")]
	public bool _applyForceThreat;

	[Header("Collider Damage")]
	public int _damage;

	public Vector2 _damageRange = new Vector2(1f, 5f);

	[Range(0f, 2f)]
	public float _damagePercent = 1f;

	[Range(0f, 150f)]
	public int _threatAmount = 1;

	private int _dmgOutput;

	[Header("Special Conditions & Modifiers")]
	[Tooltip("Percentage of how much of the damage given to the target will be given to the parent of this hitbox on successful hit.")]
	[Range(0f, 1f)]
	public float _siphonHealthPercent;

	[Tooltip("Percentage of how much health the target needs to be at to be fatally killed by this hitbox. Does not apply to elite creeps.")]
	[Range(0f, 1f)]
	public float _fatalKillHealthThreshold;

	[Header("Spawn Object On Hit")]
	[SerializeField]
	private GameObject _spawnObjectOnHit;

	[SerializeField]
	private float _spawnObjectBufferTime = 1.25f;

	[Tooltip("If true, the spawn object will set its position to the hitpoint of where the collision took place. If false, it will follow the target and set itself as a child to it.")]
	[SerializeField]
	private bool _setSpawnObjectToHitpoint = true;

	[Header("Collider Force")]
	public DamageWeight _dmgWeight = DamageWeight.Medium;

	[Range(0f, 85.65f)]
	public float _damageForce;

	[Tooltip("If true, any hit target will fly in the facing direction of the hitbox. If false, the hit target will fly backwards from their own direction.")]
	[SerializeField]
	private bool _applyDirectionalForce;

	[Tooltip("If true, this hitbox will not deal any damage, but will provide force.")]
	[SerializeField]
	private bool _forceOnlyCollider;

	[Header("Collider Timer")]
	[SerializeField]
	private float _timeBeforeColliderEnable;

	[SerializeField]
	private float _timeBeforeColliderDisable;

	[Header("Persistant Damage Buffering")]
	public bool _isPersistantDamage;

	public float _outputBuffer = 0.5f;

	[Header("Total Hits")]
	[Tooltip("Int higher than 0 will disable the collider after the amount.")]
	public int _hitsToDisable;

	[Tooltip("Lowers damage % each hit during runtime. Hit counter resets when the hitbox re-enables.")]
	public float _percentAdjustEachHit;

	private int _currentHitsApplied;

	[Header("Applied Condition")]
	public ConditionSlot _scriptableConditionSlot;

	public bool _forceConditionApply;

	private void Awake()
	{
		_collider = GetComponent<Collider>();
	}

	private void Start()
	{
		Initalize_ColliderData();
	}

	private void OnEnable()
	{
		Initalize_ColliderData();
	}

	private void OnDisable()
	{
		_parentObject = null;
		_isParentPlayer = null;
		_isParentCreep = null;
		_hitPlayer = null;
		_hitCreep = null;
	}

	public void Initalize_ColliderData()
	{
		StopAllCoroutines();
		StartCoroutine(Cache_ColliderData());
	}

	private IEnumerator Cache_ColliderData()
	{
		if (!GameManager._current)
		{
			yield break;
		}
		_damageBufferPool.Clear();
		_spawnObjectBuffers.Clear();
		_disableCollider = false;
		_collider.enabled = false;
		_currentHitsApplied = 0;
		if (_resetCritChanceOnEnable)
		{
			_setPhysCritChance = 0f;
			_setMagicCritChance = 0f;
		}
		Get_ParentObjectInfo();
		if ((bool)_collider)
		{
			yield return new WaitForSeconds(_timeBeforeColliderEnable);
			_collider.enabled = true;
			if (_timeBeforeColliderDisable > 0f)
			{
				yield return new WaitForSeconds(_timeBeforeColliderDisable);
				_collider.enabled = false;
			}
		}
	}

	public void Get_ParentObjectInfo()
	{
		_isParentPlayer = null;
		_isParentCreep = null;
		_hitPlayer = null;
		_hitCreep = null;
		if (!_parentObject)
		{
			if ((bool)GetComponentInParent<PlayerStats>())
			{
				_isParentPlayer = GetComponentInParent<PlayerStats>();
				if ((bool)_isParentPlayer)
				{
					_parentObject = _isParentPlayer.gameObject;
				}
			}
			if ((bool)GetComponentInParent<CreepBehavior>())
			{
				_isParentCreep = GetComponentInParent<CreepBehavior>()._baseCreep;
				if ((bool)_isParentCreep)
				{
					_parentObject = _isParentCreep.gameObject;
				}
			}
		}
		if ((bool)GetComponentInParent<SkillObject>())
		{
			_skillObject = GetComponentInParent<SkillObject>();
		}
		if ((bool)GetComponentInParent<ProjectileObject>())
		{
			_projectileObject = GetComponentInParent<ProjectileObject>();
		}
		if ((bool)GetComponentInParent<ConditionObject>())
		{
			_conditionObject = GetComponentInParent<ConditionObject>();
		}
		if ((bool)_projectileObject && (bool)_projectileObject._parentTransform)
		{
			_parentObject = _projectileObject._parentTransform.gameObject;
		}
		if ((bool)_parentObject)
		{
			if (_parentObject.TryGetComponent<PlayerStats>(out var component))
			{
				_isParentPlayer = component;
			}
			if (_parentObject.TryGetComponent<Creep>(out var component2))
			{
				_isParentCreep = component2;
			}
			if ((bool)_parentObject.GetComponentInParent<CreepBehavior>())
			{
				_isParentCreep = _parentObject.GetComponentInParent<CreepBehavior>()._baseCreep;
			}
			if ((bool)_isParentPlayer && (bool)_scriptWeapon)
			{
				PlayerCombat pCombat = _isParentPlayer._player._pCombat;
				if (pCombat._useDamageTypeOverride)
				{
					_dmgType = pCombat._damageTypeOverride;
				}
				else
				{
					_dmgType = _scriptWeapon.weaponType._combatType;
				}
			}
		}
		if (_combatElement == null)
		{
			_combatElement = GameManager._current._statLogics._defaultCombatElement;
		}
		if (!_parentObject)
		{
			_parentObject = base.gameObject;
		}
		if (_resetCritChanceOnEnable)
		{
			Reset_CritChanceParams();
		}
	}

	private void Reset_CritChanceParams()
	{
		if ((bool)_isParentPlayer)
		{
			_setPhysCritChance = _isParentPlayer._statStruct._criticalRate;
			_setMagicCritChance = _isParentPlayer._statStruct._magicCriticalRate;
		}
		else if ((bool)_isParentCreep)
		{
			if ((bool)_isParentCreep._scriptCreep._combatElement && !_disableCreepElement)
			{
				_combatElement = _isParentCreep._combatElement;
			}
			_setPhysCritChance = _isParentCreep._statStruct._criticalRate;
			_setMagicCritChance = _isParentCreep._statStruct._magicCriticalRate;
		}
	}

	private void Update()
	{
		Handle_BufferParams();
		Handle_ColliderParams();
	}

	private void Handle_BufferParams()
	{
		if (_clientSideBuffer > 0f)
		{
			_clientSideBuffer -= Time.deltaTime;
		}
		else
		{
			_clientSideBuffer = 0f;
		}
		if (!_isPersistantDamage)
		{
			_outputBuffer = 0.09f;
		}
		for (int i = 0; i < _spawnObjectBuffers.Count; i++)
		{
			if (_spawnObjectBuffers[i]._currentOutputBuffer > 0f)
			{
				_spawnObjectBuffers[i]._currentOutputBuffer -= 0.85f * Time.deltaTime;
				continue;
			}
			_spawnObjectBuffers.RemoveAt(i);
			break;
		}
		for (int j = 0; j < _damageBufferPool.Count; j++)
		{
			if (_damageBufferPool[j] != null)
			{
				if (_damageBufferPool[j]._currentOutputBuffer > 0f)
				{
					_damageBufferPool[j]._currentOutputBuffer -= 0.85f * Time.deltaTime;
				}
				if (_damageBufferPool[j]._currentOutputBuffer < 0f)
				{
					_damageBufferPool[j]._currentOutputBuffer = 0f;
					_damageBufferPool.RemoveAt(j);
					Reset_CritChanceParams();
				}
			}
			else
			{
				_damageBufferPool.RemoveAt(j);
				Reset_CritChanceParams();
			}
		}
	}

	private void Handle_ColliderParams()
	{
		if (_disableOnHurt && (bool)_isParentPlayer && _isParentPlayer._player._isHurt)
		{
			_disableCollider = true;
		}
		if (_currentHitsApplied >= _hitsToDisable && _hitsToDisable > 0)
		{
			_disableCollider = true;
		}
		if (!_collider.enabled)
		{
			_currentHitsApplied = 0;
		}
		if ((bool)_isParentCreep)
		{
			if (_isParentCreep.isServer && !_disableCreepElement)
			{
				_combatElement = _isParentCreep._combatElement;
			}
			_dmgType = _isParentCreep._creepAttribute;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!_isPersistantDamage)
		{
			OnTriggerInitialize(other);
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if (_isPersistantDamage)
		{
			OnTriggerInitialize(other);
		}
	}

	private void OnTriggerInitialize(Collider other)
	{
		if (_disableCollider || other == null)
		{
			return;
		}
		for (int i = 0; i < _damageBufferPool.Count; i++)
		{
			if (other == _damageBufferPool[i]._entity)
			{
				return;
			}
		}
		Vector3 hitPoint = other.ClosestPointOnBounds(base.transform.position) + Vector3.up * 1.65f;
		Init_DamageCollide(other.gameObject, hitPoint);
	}

	private void Init_DamageCollide(GameObject _hitEntity, Vector3 _hitPoint)
	{
		if (_disableCollider || !_hitEntity)
		{
			return;
		}
		int recieverLevel = 0;
		bool flag = false;
		if ((bool)_hitEntity.GetComponentInParent<Player>())
		{
			Player componentInParent = _hitEntity.GetComponentInParent<Player>();
			PlayerStats pStats = componentInParent._pStats;
			PlayerPvp pPvp = componentInParent._pPvp;
			if ((_isParentPlayer == pStats && !_applyToParent) || ((bool)_isParentCreep && _colliderType == CombatColliderType.SUPPORT) || ((bool)_isParentCreep && _isParentCreep._currentCreepCondition != CreepCondition.ACTIVE))
			{
				return;
			}
			recieverLevel = pStats._currentLevel;
			if ((bool)_isParentPlayer)
			{
				Player player = _isParentPlayer._player;
				PlayerPvp pPvp2 = player._pPvp;
				switch (_colliderType)
				{
				case CombatColliderType.OFFENSIVE:
					if (componentInParent._statusEntity._deathBuffer >= 1.2f || componentInParent._bufferingStatus || pPvp2._currentVersusFlag != VersusFlag.PVP || pPvp._currentVersusFlag != VersusFlag.PVP || ((bool)player.Network_partyObject && player.Network_partyObject == componentInParent.Network_partyObject))
					{
						return;
					}
					break;
				case CombatColliderType.SUPPORT:
				{
					if (componentInParent._bufferingStatus || componentInParent._currentPlayerCondition != PlayerCondition.ACTIVE)
					{
						return;
					}
					bool num = pPvp2._currentVersusFlag == VersusFlag.PVP || player._playerZoneType == ZoneType.Arena;
					bool flag2 = pPvp._currentVersusFlag == VersusFlag.PVP || componentInParent._playerZoneType == ZoneType.Arena;
					if ((num & flag2) && ((player != componentInParent && componentInParent.Network_partyObject == null) || ((bool)player.Network_partyObject && (bool)componentInParent.Network_partyObject && player.Network_partyObject != componentInParent.Network_partyObject)))
					{
						return;
					}
					break;
				}
				}
			}
			_hitEntity = pStats.gameObject;
			_hitPlayer = pStats;
			Init_ProjectileCallback(pStats.transform);
			if ((bool)componentInParent && _clientsideHitbox && componentInParent.isLocalPlayer && componentInParent.isClient)
			{
				Vector3 setDir = Vector3.zero;
				if (_applyDirectionalForce)
				{
					setDir = base.transform.forward;
					setDir.y = 0f;
				}
				if (_clientSideBuffer <= 0f)
				{
					componentInParent._statusEntity.Cmd_TakeDamage(_damage, _damageForce, _applyDirectionalForce, _shatterBlock, _dmgWeight, _hitPoint, setDir);
					_clientSideBuffer = 0.8f;
				}
			}
			flag = true;
		}
		if ((bool)_hitEntity.GetComponentInParent<CreepBehavior>() && _damageCreeps)
		{
			Creep baseCreep = _hitEntity.GetComponentInParent<CreepBehavior>()._baseCreep;
			if ((_isParentCreep == baseCreep && !_applyToParent) || ((bool)_isParentCreep && _colliderType == CombatColliderType.OFFENSIVE) || ((bool)_isParentPlayer && _colliderType == CombatColliderType.SUPPORT) || baseCreep._statusEntity._deathBuffer >= 0.6f || baseCreep._spawnInBuffer > 0f || !baseCreep._isTargetable)
			{
				return;
			}
			recieverLevel = baseCreep._creepLevel;
			if (!baseCreep)
			{
				return;
			}
			Init_ProjectileCallback(baseCreep.transform);
			_hitEntity = baseCreep.gameObject;
			_hitCreep = baseCreep;
			flag = true;
		}
		if ((bool)_hitEntity.GetComponentInParent<BreakableObject>() && _colliderType != CombatColliderType.SUPPORT)
		{
			Init_ProjectileCallback(_hitEntity.GetComponentInParent<BreakableObject>().transform);
		}
		else
		{
			if (!_hitEntity.transform.root.TryGetComponent<ITakeDamage>(out var component) || (_parentObject == _hitEntity && !_applyToParent) || !flag)
			{
				return;
			}
			for (int i = 0; i < _damageBufferPool.Count; i++)
			{
				if (_damageBufferPool[i]._entity == _hitEntity)
				{
					return;
				}
			}
			DamageBuffer item = new DamageBuffer
			{
				_currentOutputBuffer = _outputBuffer,
				_entity = _hitEntity
			};
			_damageBufferPool.Add(item);
			if (_hitEntity.transform.root.GetComponent<NetworkIdentity>().isServer)
			{
				Server_CalculateDamage(component, recieverLevel, _hitPoint);
			}
			if ((bool)_spawnObjectOnHit)
			{
				Init_SpawnObjectOnHit();
			}
		}
		void Init_SpawnObjectOnHit()
		{
			bool flag3 = false;
			for (int j = 0; j < _spawnObjectBuffers.Count; j++)
			{
				if (_spawnObjectBuffers[j]._entity == _hitEntity)
				{
					flag3 = true;
				}
			}
			if (!flag3)
			{
				GameObject gameObject = PoolManager._current.GenerateObject(_spawnObjectOnHit, "", 2.5f);
				gameObject.transform.SetParent(_hitEntity.transform);
				if ((bool)gameObject.GetComponentInChildren<CombatCollider>(includeInactive: true))
				{
					CombatCollider componentInChildren = gameObject.GetComponentInChildren<CombatCollider>(includeInactive: true);
					componentInChildren._parentObject = _parentObject;
					componentInChildren._skillObject = _skillObject;
					componentInChildren._isParentPlayer = _isParentPlayer;
					componentInChildren._isParentCreep = _isParentCreep;
					componentInChildren._scriptableConditionSlot = _scriptableConditionSlot;
				}
				if ((bool)gameObject.GetComponentInChildren<ConditionObject>(includeInactive: true))
				{
					ConditionObject componentInChildren2 = gameObject.GetComponentInChildren<ConditionObject>(includeInactive: true);
					if ((bool)_isParentPlayer)
					{
						componentInChildren2._statusEntity = _isParentPlayer._player._statusEntity;
					}
					if ((bool)_isParentCreep)
					{
						componentInChildren2._statusEntity = _isParentCreep._statusEntity;
					}
				}
				if (_setSpawnObjectToHitpoint)
				{
					gameObject.transform.SetParent(null);
					gameObject.transform.position = _hitPoint;
				}
				else
				{
					gameObject.transform.localPosition = Vector3.zero;
				}
				DamageBuffer item2 = new DamageBuffer
				{
					_entity = _hitEntity,
					_currentOutputBuffer = _spawnObjectBufferTime
				};
				_spawnObjectBuffers.Add(item2);
			}
		}
	}

	private void Init_ProjectileCallback(Transform _hitTransform)
	{
		if ((bool)_projectileObject)
		{
			_projectileObject.Projectile_EntityCollideCallback(_hitTransform);
		}
	}

	private void Server_CalculateDamage(ITakeDamage _damageable, int _recieverLevel, Vector3 _hitPoint)
	{
		StatLogics _sl = GameManager._current._statLogics;
		if (_clientsideHitbox || _damagePercent <= 0f)
		{
			return;
		}
		int _senderAttack = _damage;
		float damagePercent = _damagePercent;
		int _levelDifference = 0;
		float _senderCritChance = 0f;
		bool _isCriticalHit = false;
		StatStruct _statStruct = default;
		StatStruct _hitEntityStatStruct = default;
		if ((bool)_hitPlayer)
		{
			_hitEntityStatStruct = _hitPlayer._statStruct;
		}
		if ((bool)_hitCreep)
		{
			_hitEntityStatStruct = _hitCreep._statStruct;
		}
		DamageType _setDmgType = _dmgType;
		_damageRange = new Vector2(1f, 5f);
		_dmgOutput = 1;
		Calculate_LevelDifference();
		if ((bool)_isParentPlayer)
		{
			_statStruct = _isParentPlayer._statStruct;
		}
		if ((bool)_isParentCreep)
		{
			_statStruct = _isParentCreep._statStruct;
			int num = (int)((float)_isParentCreep._scriptCreep._baseDamage * 1.5f);
			_damageRange = new Vector2(_isParentCreep._scriptCreep._baseDamage, num + 3);
		}
		Calculate_ScriptWeaponParameters();
		switch (_setDmgType)
		{
		case DamageType.Strength:
			_senderAttack = _statStruct._attackPower;
			_senderCritChance = _setPhysCritChance;
			break;
		case DamageType.Dexterity:
			_senderAttack = _statStruct._dexPower;
			_senderCritChance = _setPhysCritChance;
			break;
		case DamageType.Mind:
			_senderAttack = _statStruct._magicPower;
			_senderCritChance = _setMagicCritChance;
			break;
		case DamageType.Vitality:
			_senderAttack = _statStruct._maxHealth;
			_senderCritChance = _setPhysCritChance;
			break;
		}
		Calculate_SkillObjectParameters();
		Calculate_ConditionObjectParameters();
		if (!_skillObject && !_conditionObject)
		{
			_senderAttack = Random.Range((int)_damageRange.x, (int)_damageRange.y);
		}
		_senderAttack = (int)((float)_senderAttack * (damagePercent + _percentAdjustEachHit * (float)_currentHitsApplied));
		Calculate_Defense();
		if (_dmgOutput <= 0)
		{
			_dmgOutput = 1 + Random.Range(1, 4);
		}
		else
		{
			_dmgOutput += Random.Range(0, 4);
		}
		Calculate_CritChance();
		Calculate_EvasionChance();
		if (_setDmgType == DamageType.Pure)
		{
			_dmgOutput = _damage;
			Server_ApplyDamage(_damageable, 0, _dmgOutput, _isCriticalHit, _combatElement, _hitPoint);
			return;
		}
		if ((bool)_skillObject && _skillObject._scriptSkill._skillRankParams._baseSkillPower <= 0)
		{
			_dmgOutput = 0;
		}
		if (_fatalKillHealthThreshold > 0f)
		{
			if ((bool)_hitCreep && !_hitCreep._scriptCreep._isElite && (float)_hitCreep._statusEntity._currentHealth <= (float)_hitCreep._statStruct._maxHealth * _fatalKillHealthThreshold)
			{
				_dmgOutput += _hitCreep._statusEntity._currentHealth;
			}
			if ((bool)_hitPlayer && (float)_hitPlayer._player._statusEntity._currentHealth <= (float)_hitPlayer._statStruct._maxHealth * _fatalKillHealthThreshold)
			{
				_dmgOutput += _hitPlayer._player._statusEntity._currentHealth;
			}
		}
		Server_ApplyDamage(_damageable, _levelDifference, _dmgOutput, _isCriticalHit, _combatElement, _hitPoint);
		void Calculate_ConditionObjectParameters()
		{
			if ((bool)_conditionObject && (bool)_conditionObject._scriptCondition)
			{
				int num2 = _conditionObject._scriptCondition.Get_ConditionPower(_statStruct, DamageType.Pure, 0, 0.5f);
				_senderAttack = num2;
			}
		}
		void Calculate_CritChance()
		{
			if (!_ignoreCriticalChance && !(_senderCritChance <= 0f) && !(Random.Range(0f, 1f) > _senderCritChance))
			{
				_dmgOutput *= 2;
				if (_dmgOutput < 12)
				{
					_dmgOutput = Random.Range(12, 29);
				}
				_isCriticalHit = true;
			}
		}
		void Calculate_Defense()
		{
			int defense = _hitEntityStatStruct._defense;
			int magicDefense = _hitEntityStatStruct._magicDefense;
			_dmgOutput = _senderAttack;
			float defensePercent = _sl._defensePercent;
			float num2 = 0f;
			if (_colliderType != CombatColliderType.SUPPORT)
			{
				switch (_dmgWeight)
				{
				case DamageWeight.Light:
					num2 = 0f;
					break;
				case DamageWeight.Medium:
					num2 = -0.2f;
					break;
				case DamageWeight.Heavy:
					num2 = -0.4f;
					break;
				}
				float num3 = 0f;
				if ((bool)_scriptWeapon)
				{
					num3 = _sl._weaponDefenseBonus;
				}
				int num4 = defense;
				if (_setDmgType == DamageType.Mind)
				{
					num4 = magicDefense;
				}
				num4 = (int)((float)num4 * defensePercent);
				if (defense > 0)
				{
					_dmgOutput = (int)((float)_senderAttack - (float)num4 * (1f + num2 + num3));
				}
				float num5 = 0.36f;
				bool flag = false;
				if ((bool)_hitCreep && _hitCreep._scriptCreep._isElite)
				{
					flag = true;
				}
				if (!flag)
				{
					if (_levelDifference <= -5)
					{
						num5 = 0.28f;
					}
					else if (_levelDifference == -4)
					{
						num5 = 0.3f;
					}
					else if (_levelDifference == -3)
					{
						num5 = 0.32f;
					}
					else if (_levelDifference == -2)
					{
						num5 = 0.34f;
					}
				}
				if ((bool)_isParentPlayer)
				{
					int num6 = (int)((float)_senderAttack * num5);
					if (_dmgOutput < num6)
					{
						_dmgOutput = num6;
					}
				}
				if ((bool)_isParentCreep)
				{
					int num6 = (int)((float)_isParentCreep._scriptCreep._baseDamage * 0.28f);
					if (_dmgOutput < num6)
					{
						_dmgOutput = num6;
					}
				}
				float num7 = 0f;
				if (_combatElement != GameManager._current._statLogics._defaultCombatElement)
				{
					float elementResistancePercent = GameManager._current._statLogics._elementResistancePercent;
					switch (_combatElement._elementName)
					{
					case "Fire":
						num7 = elementResistancePercent * (float)_hitEntityStatStruct._fireResist;
						break;
					case "Water":
						num7 = elementResistancePercent * (float)_hitEntityStatStruct._waterResist;
						break;
					case "Nature":
						num7 = elementResistancePercent * (float)_hitEntityStatStruct._natureResist;
						break;
					case "Earth":
						num7 = elementResistancePercent * (float)_hitEntityStatStruct._earthResist;
						break;
					case "Holy":
						num7 = elementResistancePercent * (float)_hitEntityStatStruct._holyResist;
						break;
					case "Shadow":
						num7 = elementResistancePercent * (float)_hitEntityStatStruct._shadowResist;
						break;
					}
				}
				if (num7 > 0f)
				{
					_dmgOutput = (int)((float)_dmgOutput - num7);
				}
				else if (num7 < 0f)
				{
					_dmgOutput = (int)((float)_dmgOutput + Mathf.Abs(num7));
				}
			}
		}
		void Calculate_EvasionChance()
		{
			if (_colliderType != CombatColliderType.SUPPORT)
			{
				float num2 = _hitEntityStatStruct._evasion;
				float num3 = 1f;
				if ((bool)_isParentPlayer)
				{
					num3 = _isParentPlayer._player._statusEntity._hitAccuracyPercent;
				}
				if ((bool)_isParentCreep)
				{
					num3 = _isParentCreep._statusEntity._hitAccuracyPercent;
				}
				if (Random.Range(0f, 1f) > num3)
				{
					_isCriticalHit = false;
					_dmgOutput = 0;
				}
				if (!(num2 <= 0f) && !_ignoreEvasionChance && (!_hitPlayer || _hitPlayer._player._currentPlayerAction != PlayerAction.BLOCKING) && (!_hitCreep || !_hitCreep._isParryStaggered))
				{
					float num4 = Random.Range(0f, 1f);
					float num5 = (float)_levelDifference * 0.018f;
					if (_levelDifference < 0)
					{
						num2 += num5 * -1f;
					}
					else if (_levelDifference > 0)
					{
						num2 -= num5;
					}
					if (num2 > 1f)
					{
						num2 = 1f;
					}
					else if (num2 < 0f)
					{
						num2 = 0f;
					}
					if (num4 <= num2)
					{
						_isCriticalHit = false;
						_dmgOutput = 0;
					}
				}
			}
		}
		void Calculate_LevelDifference()
		{
			if ((bool)_isParentPlayer)
			{
				_levelDifference = _recieverLevel - _isParentPlayer._currentLevel;
			}
			if ((bool)_isParentCreep)
			{
				_levelDifference = _recieverLevel - _isParentCreep._creepLevel;
			}
			_levelDifference *= -1;
		}
		void Calculate_ScriptWeaponParameters()
		{
			if (!_scriptWeapon)
			{
				if ((bool)_isParentPlayer)
				{
					int num2 = (int)((float)_statStruct._attackPower * _sl._unarmedAttackPercent);
					if (num2 <= 0)
					{
						num2 = 1;
					}
					_damageRange = new Vector2Int(num2, num2);
				}
			}
			else
			{
				float num3 = 1f;
				for (int i = 0; i < _isParentPlayer._player._pCasting._passiveSkillObjects.Count; i++)
				{
					SkillObjectBehavior skillObjectBehavior = _isParentPlayer._player._pCasting._passiveSkillObjects[i];
					if (skillObjectBehavior.GetType() == typeof(SkillBehavior_WeaponMastery))
					{
						SkillBehavior_WeaponMastery skillBehavior_WeaponMastery = (SkillBehavior_WeaponMastery)skillObjectBehavior;
						for (int j = 0; j < skillBehavior_WeaponMastery._setWeaponTypes.Length; j++)
						{
							if (skillBehavior_WeaponMastery._setWeaponTypes[j] == _scriptWeapon.weaponType)
							{
								num3 += skillBehavior_WeaponMastery._damageBonusIncrease;
							}
						}
					}
				}
				num3 += _isParentPlayer._player._pCombat._weaponTypeDamageBonus;
				int x = _scriptWeapon.Scale_FloorDamage(_statStruct, _setDmgType, num3);
				int y = _scriptWeapon.Scale_CeilingDamage(_statStruct, _setDmgType, num3);
				_damageRange = new Vector2Int(x, y);
			}
		}
		void Calculate_SkillObjectParameters()
		{
			if ((bool)_skillObject)
			{
				int num2 = 0;
				float num3 = 0f;
				if ((bool)_isParentPlayer)
				{
					num3 = _isParentPlayer.Retrieve_SkillBonusPower(_skillObject._scriptSkill);
				}
				if (_skillObject._scriptSkill._skillRankParams._baseSkillPower > 0)
				{
					switch (_skillObject._scriptSkill._skillDamageType)
					{
					case DamageType.Strength:
						num2 = _skillObject._scriptSkill._skillRankParams._baseSkillPower + (int)((float)_statStruct._attackPower * (_sl._powerStatPercent + num3));
						break;
					case DamageType.Dexterity:
						num2 = _skillObject._scriptSkill._skillRankParams._baseSkillPower + (int)((float)_statStruct._dexPower * (_sl._powerStatPercent + num3));
						break;
					case DamageType.Mind:
						num2 = _skillObject._scriptSkill._skillRankParams._baseSkillPower + (int)((float)_statStruct._magicPower * (_sl._powerStatPercent + num3));
						break;
					case DamageType.Vitality:
						num2 = _skillObject._scriptSkill._skillRankParams._baseSkillPower + (int)((float)_statStruct._maxHealth * (_sl._powerStatPercent + num3));
						break;
					}
				}
				if ((bool)_isParentPlayer && _skillObject._scriptSkill._skillRankParams._useWeaponPower && (bool)_isParentPlayer._player._pCombat._equippedWeapon)
				{
					_dmgWeight = _isParentPlayer._player._pCombat._currentScriptableWeaponType._damageWeight;
					num2 += _isParentPlayer._player._pCombat._equippedWeapon.Get_BaseCeilingDamage();
				}
				if ((bool)_isParentCreep)
				{
					num2 += _isParentCreep._scriptCreep._baseDamage;
				}
				_senderAttack = num2;
			}
		}
	}

	private void Server_ApplyDamage(ITakeDamage _damageable, int _passedLevel, int _damageValue, bool _isCriticalHit, ScriptableCombatElement _combatElement, Vector3 _hitPoint)
	{
		if (_damageable == null)
		{
			return;
		}
		StatusEntity statusEntity = null;
		ScriptableSkill scriptableSkill = null;
		if ((bool)_skillObject)
		{
			scriptableSkill = _skillObject._scriptSkill;
		}
		EntityDamageType entityDamageType = EntityDamageType.NULL;
		DamageType dmgType = _dmgType;
		Vector3 forward = base.transform.forward;
		forward.y = 0f;
		bool flag = _projectileObject;
		bool canReflect = !flag && _canReflect;
		if ((bool)_isParentPlayer)
		{
			StatusEntity statusEntity2 = _isParentPlayer._player._statusEntity;
			statusEntity = statusEntity2;
			entityDamageType = EntityDamageType.PLAYER;
			if ((bool)_skillObject)
			{
				statusEntity2.ScriptSkill_CancelConditionCallback(CancelConditionOnCastType.OFFENSIVE);
			}
			if ((bool)_hitPlayer && _colliderType == CombatColliderType.OFFENSIVE)
			{
				_damageValue = (int)((float)_damageValue * GameManager._current._statLogics._pvpOutputPercent);
			}
		}
		else if ((bool)_isParentCreep)
		{
			statusEntity = _isParentCreep._statusEntity;
			entityDamageType = EntityDamageType.CREEP;
		}
		DamageStruct damageStruct = new DamageStruct
		{
			_statusEntity = statusEntity,
			_scriptableSkill = scriptableSkill,
			_combatCollider = this,
			_levelDifference = _passedLevel,
			_damageValue = _damageValue,
			_damageElement = _combatElement,
			_damageType = dmgType,
			_entityDamageType = entityDamageType,
			_colliderType = _colliderType,
			_damageWeight = _dmgWeight,
			_siphonHealthPercent = _siphonHealthPercent,
			_appliedForce = _damageForce,
			_ignoreIFrames = _ignoreIFrames,
			_isCriticalHit = _isCriticalHit,
			_conditionSlot = _scriptableConditionSlot,
			_forceConditionApply = _forceConditionApply,
			_noHitStun = _noHitstunChance,
			_damageDirection = forward,
			_setDamageDirection = _applyDirectionalForce,
			_threatAmount = _threatAmount,
			_forceOnly = _forceOnlyCollider,
			_isProjectileCollider = flag,
			_canReflect = canReflect,
			_ignoreDefenseBlock = _ignoreBlock,
			_colliderHitPoint = _hitPoint,
			_allowParryStagger = _canParryStagger,
			_forceThreat = _applyForceThreat,
			_shatterBlock = _shatterBlock
		};
		if (_colliderType == CombatColliderType.OFFENSIVE)
		{
			_damageable.Take_Damage(damageStruct);
			if ((bool)_hitCreep && _hitCreep._playerTagged == null && (bool)_isParentPlayer && _isParentPlayer._player._statusEntity._currentHealth > 0)
			{
				_hitCreep._playerTagged = _isParentPlayer._player;
				_hitCreep = null;
			}
		}
		else if (_colliderType == CombatColliderType.SUPPORT)
		{
			if ((bool)_hitPlayer)
			{
				_hitPlayer._player._statusEntity.Add_Health(_damageValue);
				_hitPlayer._player._statusEntity.Apply_DamageStructCondition(damageStruct, _successfulBlock: false, _absorbHit: false);
			}
			else if ((bool)_hitCreep)
			{
				_hitCreep._statusEntity.Add_Health(_damageValue);
				_hitCreep._statusEntity.Apply_DamageStructCondition(damageStruct, _successfulBlock: false, _absorbHit: false);
			}
		}
		_currentHitsApplied++;
	}
}
