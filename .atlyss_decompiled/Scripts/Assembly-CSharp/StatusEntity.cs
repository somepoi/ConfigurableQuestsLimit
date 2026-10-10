using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Mirror;
using Mirror.RemoteCalls;
using UnityEngine;

[RequireComponent(typeof(StatusEntityGUI))]
public class StatusEntity : NetworkBehaviour, ITakeDamage
{
	public Player _isPlayer;

	public Creep _isCreep;

	public StatusEntityGUI _statusEntityGUI;

	private PlayerStats _pStats;

	private PlayerCombat _pCombat;

	private ChatBehaviour _pCb;

	[Header("Status Entity Params")]
	[SyncVar(hook = "OnHealthChange")]
	public int _currentHealth;

	[SyncVar]
	public int _currentMana;

	[SyncVar]
	public int _currentStamina;

	[SyncVar]
	public bool _isDamageBuffer;

	[Space]
	[SyncVar]
	public float _generalHitBuffer;

	public float _deathBuffer;

	private float _damageBuffer;

	[Header("Health / Mana Regen")]
	[SyncVar]
	public float _manaRegenRate = 1.25f;

	[SyncVar]
	public float _stamRegenRate = 0.65f;

	public float _regenManaBuffer;

	public float _regenStamBuffer;

	private float _currentManaRegenRate;

	private float _currentStamRegenRate;

	private bool _canRegenMana;

	private bool _canRegenStamina;

	[Header("Conditions / Buffs")]
	public List<ConditionData> _conditionDatas = new List<ConditionData>();

	public readonly SyncList<ConditionStruct> _syncConditions = new SyncList<ConditionStruct>();

	[Header("Special Conditions")]
	[SyncVar]
	public float _damageAbsorbtion;

	[Range(0f, 1f)]
	public float _hitAccuracyPercent = 1f;

	public float _parryBufferApply = 1f;

	[Space]
	[SyncVar]
	public bool _reflectDamage;

	public float _reflectPercent;

	[Space]
	[Range(0f, 1f)]
	public float _staminaUseOnBlockPercent = 1f;

	[Range(0f, 1f)]
	public float _chanceToIgnoreBlockStaminaCost;

	public bool _immuneToKnockback;

	public bool _dampenKnockback;

	public bool _autoParry;

	[Space]
	[SyncVar]
	public string _summonSkillTag = string.Empty;

	[SyncVar]
	public NetworkIdentity _summonAttackTag;

	public SkillObject _summonSkillObject;

	protected uint ____summonAttackTagNetId;

	public int Network_currentHealth
	{
		get
		{
			return _currentHealth;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _currentHealth, 1uL, OnHealthChange);
		}
	}

	public int Network_currentMana
	{
		get
		{
			return _currentMana;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _currentMana, 2uL, null);
		}
	}

	public int Network_currentStamina
	{
		get
		{
			return _currentStamina;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _currentStamina, 4uL, null);
		}
	}

	public bool Network_isDamageBuffer
	{
		get
		{
			return _isDamageBuffer;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _isDamageBuffer, 8uL, null);
		}
	}

	public float Network_generalHitBuffer
	{
		get
		{
			return _generalHitBuffer;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _generalHitBuffer, 16uL, null);
		}
	}

	public float Network_manaRegenRate
	{
		get
		{
			return _manaRegenRate;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _manaRegenRate, 32uL, null);
		}
	}

	public float Network_stamRegenRate
	{
		get
		{
			return _stamRegenRate;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _stamRegenRate, 64uL, null);
		}
	}

	public float Network_damageAbsorbtion
	{
		get
		{
			return _damageAbsorbtion;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _damageAbsorbtion, 128uL, null);
		}
	}

	public bool Network_reflectDamage
	{
		get
		{
			return _reflectDamage;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _reflectDamage, 256uL, null);
		}
	}

	public string Network_summonSkillTag
	{
		get
		{
			return _summonSkillTag;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _summonSkillTag, 512uL, null);
		}
	}

	public NetworkIdentity Network_summonAttackTag
	{
		get
		{
			return GetSyncVarNetworkIdentity(____summonAttackTagNetId, ref _summonAttackTag);
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter_NetworkIdentity(value, ref _summonAttackTag, 1024uL, null, ref ____summonAttackTagNetId);
		}
	}

	private void OnHealthChange(int _old, int _new)
	{
		if ((bool)Player._mainPlayer && !Player._mainPlayer._bufferingStatus && !(Player._mainPlayer.gameObject.scene != base.gameObject.scene))
		{
			Apply_PlayerHook();
		}
		void Apply_PlayerHook()
		{
			if ((bool)_isPlayer && !(_isPlayer.Network_playerMapInstance != Player._mainPlayer.Network_playerMapInstance) && _old < _new && _currentHealth < _isPlayer._pStats._statStruct._maxHealth)
			{
				_isPlayer._pVisual.Flash_PlayerRecieveHealth();
			}
		}
	}

	private void Awake()
	{
		_statusEntityGUI = GetComponent<StatusEntityGUI>();
		Creep component2;
		if (base.gameObject.TryGetComponent<Player>(out var component))
		{
			_isPlayer = component;
			_pStats = component._pStats;
			_pCombat = component._pCombat;
			_pCb = component._chatBehaviour;
		}
		else if (base.gameObject.TryGetComponent<Creep>(out component2))
		{
			_isCreep = component2;
		}
	}

	private void Start()
	{
		_syncConditions.Callback += OnConditionsUpdated;
		for (int i = 0; i < _syncConditions.Count; i++)
		{
			OnConditionsUpdated(SyncList<ConditionStruct>.Operation.OP_ADD, i, default(ConditionStruct), _syncConditions[i]);
		}
	}

	public override void OnStartServer()
	{
		base.OnStartServer();
		_parryBufferApply = GameManager._current._statLogics._baseParryApplyBuffer;
		Network_manaRegenRate = GameManager._current._statLogics._baseManaRegenRate;
		Network_stamRegenRate = GameManager._current._statLogics._baseStaminaRegenRate;
	}

	private void Update()
	{
		Server_StatusParams();
		Handle_BufferParams();
		Handle_AppliedConditions();
	}

	private void Handle_BufferParams()
	{
		if (_currentHealth <= 0)
		{
			if (_deathBuffer < 0.8f)
			{
				_deathBuffer += Time.deltaTime;
			}
		}
		else
		{
			_deathBuffer = 0f;
		}
		if (_damageBuffer > 0f)
		{
			_damageBuffer -= Time.deltaTime;
		}
		else
		{
			_damageBuffer = 0f;
		}
		if (_generalHitBuffer > 0f)
		{
			Network_generalHitBuffer = _generalHitBuffer - Time.deltaTime;
		}
		else
		{
			Network_generalHitBuffer = 0f;
		}
		if ((bool)_isPlayer && !_isPlayer._bufferingStatus && !_summonSkillObject && !string.IsNullOrWhiteSpace(_summonSkillTag))
		{
			ScriptableSkill scriptableSkill = GameManager._current.Locate_Skill(_summonSkillTag);
			scriptableSkill.Init_NewSkillObject(scriptableSkill._skillRankParams._skillObject_onCast, this);
		}
		if ((bool)_summonSkillObject && string.IsNullOrWhiteSpace(_summonSkillTag))
		{
			Object.Destroy(_summonSkillObject.gameObject);
		}
	}

	[ServerCallback]
	private void Server_StatusParams()
	{
		if (NetworkServer.active)
		{
			if (_currentMana < 0)
			{
				Network_currentMana = 0;
			}
			if (_currentStamina < 0)
			{
				Network_currentStamina = 0;
			}
			if (_damageAbsorbtion < 0f)
			{
				Network_damageAbsorbtion = 0f;
			}
			Network_isDamageBuffer = _damageBuffer > 0f;
			Handle_PlayerStatus();
			Handle_CreepStatus();
		}
		void Handle_CreepStatus()
		{
			if ((bool)_isCreep)
			{
				if (_currentHealth > _isCreep._statStruct._maxHealth)
				{
					Network_currentHealth = _isCreep._statStruct._maxHealth;
				}
				if (_currentMana > _isCreep._statStruct._maxMana)
				{
					Network_currentMana = _isCreep._statStruct._maxMana;
				}
				if (_isCreep._currentCreepCondition == CreepCondition.ACTIVE)
				{
					if (_currentHealth <= 0)
					{
						_isCreep.BaseCreep_OnDeath();
					}
					if (_currentMana < _isCreep._statStruct._maxMana)
					{
						_currentManaRegenRate += Time.deltaTime;
						if (_currentManaRegenRate >= _manaRegenRate)
						{
							Network_currentMana = _currentMana + 1;
							_currentManaRegenRate = 0f;
						}
					}
					else
					{
						_currentManaRegenRate = 0f;
					}
				}
				if (_isCreep._currentCreepCondition == CreepCondition.SPAWNING)
				{
					Network_currentHealth = _isCreep._statStruct._maxHealth;
					Network_currentMana = _isCreep._statStruct._maxMana;
					Replenish_All();
				}
			}
		}
		void Handle_ManaRegen()
		{
			if (_currentMana < _pStats._statStruct._maxMana)
			{
				if (!_canRegenMana)
				{
					if (_regenManaBuffer < GameManager._current._statLogics._timeBeforeManaRegen)
					{
						_regenManaBuffer += Time.deltaTime;
					}
					else
					{
						_canRegenMana = true;
					}
					_currentManaRegenRate = 0f;
				}
				else
				{
					_currentManaRegenRate += Time.deltaTime;
					if (_currentManaRegenRate >= _manaRegenRate)
					{
						Network_currentMana = _currentMana + 1;
						_currentManaRegenRate = 0f;
					}
				}
			}
			else
			{
				_canRegenMana = false;
				_currentManaRegenRate = 0f;
				_regenManaBuffer = 0f;
			}
		}
		void Handle_PlayerStatus()
		{
			if ((bool)_isPlayer)
			{
				if (_isPlayer._playerZoneType == ZoneType.Safe)
				{
					Network_currentHealth = _pStats._statStruct._maxHealth;
				}
				if (_currentHealth >= _pStats._statStruct._maxHealth)
				{
					Network_currentHealth = _pStats._statStruct._maxHealth;
				}
				if (_currentMana >= _pStats._statStruct._maxMana)
				{
					Network_currentMana = _pStats._statStruct._maxMana;
				}
				if (_currentStamina >= _pStats._statStruct._maxStamina)
				{
					Network_currentStamina = _pStats._statStruct._maxStamina;
				}
				if (_isPlayer._currentPlayerCondition == PlayerCondition.CONNECTING)
				{
					Network_currentHealth = _pStats._statStruct._maxHealth;
					Network_currentMana = _pStats._statStruct._maxMana;
					Network_currentStamina = _pStats._statStruct._maxStamina;
				}
				if (_isPlayer._currentPlayerCondition != PlayerCondition.DEAD && _isPlayer._currentGameCondition == GameCondition.IN_GAME && _currentHealth <= 0)
				{
					Network_currentStamina = 0;
					Network_currentMana = 0;
					_isPlayer.Player_OnDeath();
					_isPlayer.Network_currentPlayerCondition = PlayerCondition.DEAD;
				}
				if (_isPlayer._currentPlayerCondition == PlayerCondition.DEAD)
				{
					if (_currentHealth > 0)
					{
						_isPlayer.Network_currentPlayerCondition = PlayerCondition.ACTIVE;
					}
				}
				else
				{
					Handle_ManaRegen();
					Handle_StaminaRegen();
				}
			}
		}
		void Handle_StaminaRegen()
		{
			if (_pCombat._isBlocking)
			{
				_canRegenStamina = false;
				_regenStamBuffer = 0f;
			}
			else
			{
				_canRegenStamina = _regenStamBuffer >= GameManager._current._statLogics._timeBeforeStaminaRegen && _currentStamina < _pStats._statStruct._maxStamina;
				if (_currentStamina >= _pStats._statStruct._maxStamina)
				{
					_regenStamBuffer = 0f;
				}
				else
				{
					if (_regenStamBuffer < GameManager._current._statLogics._timeBeforeStaminaRegen && !_canRegenStamina)
					{
						_regenStamBuffer += Time.deltaTime;
						_currentStamRegenRate = 0f;
					}
					if (_canRegenStamina)
					{
						_currentStamRegenRate += (float)_pStats._statStruct._maxStamina * 0.12f * Time.deltaTime;
						if (_currentStamRegenRate >= _stamRegenRate)
						{
							Network_currentStamina = _currentStamina + 1;
							_currentStamRegenRate = 0f;
						}
					}
				}
			}
		}
	}

	[Command]
	public void Cmd_RemoveSummon()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void StatusEntity::Cmd_RemoveSummon()", 1227497380, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ServerCallback]
	public void Clear_AllConditions()
	{
		if (NetworkServer.active)
		{
			_syncConditions.Clear();
		}
	}

	[ServerCallback]
	public void ScriptCondition_Callback(ScriptableCondition _scriptCondition, int _value)
	{
		if (!NetworkServer.active)
		{
			return;
		}
		for (int i = 0; i < _conditionDatas.Count; i++)
		{
			if (!(_conditionDatas[i]._scriptableCondition != _scriptCondition) && (bool)_conditionDatas[i]._senderStatusEntity)
			{
				Init_FoundConditionData(_conditionDatas[i]);
			}
		}
		void Init_FoundConditionData(ConditionData _data)
		{
			StatusEntity senderStatusEntity = _data._senderStatusEntity;
			StatusEntityGUI statusEntityGUI = senderStatusEntity._statusEntityGUI;
			if ((bool)senderStatusEntity)
			{
				if (_scriptCondition.GetType() == typeof(ScriptableStatusCondition) && ((ScriptableStatusCondition)_scriptCondition)._siphonHealth && _value > 0 && senderStatusEntity._currentHealth > 0)
				{
					senderStatusEntity.Add_Health(_value);
				}
				int conditionGroupID = _data._scriptableCondition._conditionGroup._conditionGroupID;
				if ((bool)senderStatusEntity._isPlayer)
				{
					if ((bool)_scriptCondition._conditionElement)
					{
						statusEntityGUI.Target_Display_FloatingNumber(this, FloatTextColor.PHYSICAL_DAMAGE, _value, _scriptCondition._conditionElement._elementID, 0);
					}
					else
					{
						statusEntityGUI.Target_Display_FloatingNumber(this, FloatTextColor.PHYSICAL_DAMAGE, _value, 0, conditionGroupID);
					}
					Init_PvpFragMessage(_isPlayer);
				}
			}
		}
	}

	[ServerCallback]
	public void ScriptSkill_CancelConditionCallback(CancelConditionOnCastType _cancelCastType)
	{
		if (!NetworkServer.active)
		{
			return;
		}
		for (int i = 0; i < _syncConditions.Count; i++)
		{
			ScriptableCondition scriptableCondition = GameManager._current.Locate_Condition(_syncConditions[i]._conditionID);
			if ((bool)scriptableCondition && _cancelCastType == scriptableCondition._cancelOnSkillCast)
			{
				_syncConditions.RemoveAt(i);
			}
		}
	}

	[ServerCallback]
	public void Init_OnAttackCallback()
	{
		if (!NetworkServer.active)
		{
			return;
		}
		for (int i = 0; i < _syncConditions.Count; i++)
		{
			ScriptableCondition scriptableCondition = GameManager._current.Locate_Condition(_syncConditions[i]._conditionID);
			if ((bool)scriptableCondition && scriptableCondition._cancelOnAttack)
			{
				_syncConditions.RemoveAt(i);
			}
		}
	}

	private void OnConditionsUpdated(SyncList<ConditionStruct>.Operation op, int _index, ConditionStruct _oldCondition, ConditionStruct _newCondition)
	{
		bool checkIfLocalPlayer = (bool)_isPlayer && base.isOwned && base.isLocalPlayer;
		switch (op)
		{
		case SyncList<ConditionStruct>.Operation.OP_ADD:
			On_ConditionAdd(_newCondition, checkIfLocalPlayer);
			break;
		case SyncList<ConditionStruct>.Operation.OP_SET:
			On_ConditionSet(_index);
			break;
		case SyncList<ConditionStruct>.Operation.OP_REMOVEAT:
			On_ConditionRemoveAt(_oldCondition, checkIfLocalPlayer);
			break;
		case SyncList<ConditionStruct>.Operation.OP_CLEAR:
			On_ConditionListClear(checkIfLocalPlayer);
			break;
		case SyncList<ConditionStruct>.Operation.OP_INSERT:
			break;
		}
	}

	private void On_ConditionAdd(ConditionStruct _newCondition, bool _checkIfLocalPlayer)
	{
		ScriptableCondition scriptableCondition = GameManager._current.Locate_Condition(_newCondition._conditionID);
		if (!scriptableCondition)
		{
			return;
		}
		float num = (float)_newCondition._conditionDuration + 1f;
		StatStruct statStruct = default;
		if ((bool)_isCreep && (bool)scriptableCondition._conditionGroup && _isCreep._scriptCreep._isElite && scriptableCondition._conditionGroup._conditionType == ConditionType.DEBUFF)
		{
			num *= 0.5f;
		}
		if (num <= 0.6f)
		{
			num = 0.6f;
		}
		ConditionData _newConditionData = new ConditionData
		{
			_scriptableCondition = scriptableCondition,
			_currentDuration = num,
			_attribute = _newCondition._attribute,
			_senderStatusEntity = _newCondition._senderEntity,
			_setConditionPower = _newCondition._conditionPower,
			_setRepeatRate = _newCondition._conditionRepeatRate,
			_setDuration = _newCondition._conditionDuration,
			_setConditionPercent = _newCondition._conditionPowerPercent,
			_conditionStruct = _newCondition
		};
		if ((bool)_newCondition._senderEntity)
		{
			if ((bool)_newCondition._senderEntity._isPlayer)
			{
				statStruct = _newCondition._senderEntity._isPlayer._pStats._baseStatStruct;
			}
			else if ((bool)_newCondition._senderEntity._isCreep)
			{
				statStruct = _newCondition._senderEntity._isCreep._statStruct;
			}
		}
		_newConditionData._statStruct = statStruct;
		Init_ConditionObject();
		if (_newConditionData._setRepeatRate <= 0f)
		{
			scriptableCondition.Init_ConditionEffect(this, _newConditionData);
		}
		_conditionDatas.Add(_newConditionData);
		if ((bool)scriptableCondition._conditionIcon)
		{
			if (_checkIfLocalPlayer)
			{
				InGameUI._current.Add_ConditionListEntry(_newConditionData);
			}
			if ((bool)_pCb)
			{
				_pCb.Init_GameLogicMessage("Recieved Condition: " + scriptableCondition._conditionName + ".");
			}
		}
		void Init_ConditionObject()
		{
			if ((bool)_newConditionData._scriptableCondition._conditionObject)
			{
				_newConditionData._conditionEffectObject = PoolManager._current.GenerateObject(_newConditionData._scriptableCondition._conditionObject, string.Empty, 0f);
				_newConditionData._conditionEffectObject.transform.SetParent(base.transform);
				_newConditionData._conditionEffectObject.transform.localPosition = Vector3.zero;
				ConditionObject component = _newConditionData._conditionEffectObject.GetComponent<ConditionObject>();
				component._statusEntity = this;
				if ((bool)_newConditionData._scriptableCondition)
				{
					component._scriptCondition = _newConditionData._scriptableCondition;
				}
				if ((bool)_isCreep)
				{
					_newConditionData._conditionEffectObject.transform.localScale = _isCreep._creepObject.transform.localScale;
				}
				else
				{
					_newConditionData._conditionEffectObject.transform.localScale = base.transform.localScale;
				}
			}
		}
	}

	private void On_ConditionSet(int _index)
	{
		Refresh_Condition(_index);
	}

	public void Refresh_Condition(int _index)
	{
		ConditionData conditionData = _conditionDatas[_index];
		conditionData._currentDuration = conditionData._setDuration;
		if ((bool)conditionData._conditionEffectObject)
		{
			ConditionObject component = conditionData._conditionEffectObject.GetComponent<ConditionObject>();
			conditionData._conditionEffectObject.SetActive(value: false);
			conditionData._conditionEffectObject.SetActive(value: true);
			component._visualContainer.SetActive(value: true);
			if ((bool)conditionData._scriptableCondition)
			{
				component._scriptCondition = conditionData._scriptableCondition;
			}
			conditionData._conditionEffectObject.transform.SetParent(base.transform);
			conditionData._conditionEffectObject.transform.localPosition = Vector3.zero;
		}
		else if ((bool)conditionData._scriptableCondition._conditionObject)
		{
			conditionData._conditionEffectObject = PoolManager._current.GenerateObject(conditionData._scriptableCondition._conditionObject, string.Empty, 0f);
			ConditionObject component2 = conditionData._conditionEffectObject.GetComponent<ConditionObject>();
			conditionData._conditionEffectObject.transform.SetParent(base.transform);
			conditionData._conditionEffectObject.transform.localPosition = Vector3.zero;
			component2._statusEntity = this;
			if ((bool)conditionData._scriptableCondition)
			{
				component2._scriptCondition = conditionData._scriptableCondition;
			}
			if ((bool)_isCreep)
			{
				conditionData._conditionEffectObject.transform.localScale = _isCreep._creepObject.transform.localScale;
			}
			else
			{
				conditionData._conditionEffectObject.transform.localScale = base.transform.localScale;
			}
		}
		if ((bool)_pCb && (bool)_conditionDatas[_index]._scriptableCondition._conditionIcon)
		{
			_pCb.Init_GameLogicMessage("Condition Re-applied: " + conditionData._scriptableCondition._conditionName + ".");
		}
	}

	public void Init_ResetConditionEffect(ScriptableCondition _scriptCondition, ConditionData _conData)
	{
		StartCoroutine(Timer_ResetConditionEffect(_scriptCondition, _conData));
	}

	private IEnumerator Timer_ResetConditionEffect(ScriptableCondition _scriptCondition, ConditionData _conData)
	{
		_conData._isInitialized = false;
		yield return new WaitForSeconds(0.28f);
		_scriptCondition.Init_ConditionEffect(this, _conData);
	}

	private void On_ConditionRemoveAt(ConditionStruct _conditionStruct, bool _checkIfLocalPlayer)
	{
		ScriptableCondition scriptableCondition = GameManager._current.Locate_Condition(_conditionStruct._conditionID);
		if (!scriptableCondition)
		{
			return;
		}
		ConditionData _conDat = new ConditionData();
		int index = 0;
		for (int i = 0; i < _conditionDatas.Count; i++)
		{
			if (_conditionDatas[i]._scriptableCondition == scriptableCondition)
			{
				_conDat = _conditionDatas[i];
				index = i;
				break;
			}
		}
		Init_RemoveConditionObject();
		Remove_ConditionDisplays();
		if ((bool)_pCb && (bool)_conDat._scriptableCondition._conditionIcon)
		{
			_pCb.Init_GameLogicMessage("Condition Removed: " + _conDat._scriptableCondition._conditionName + ".");
		}
		StartCoroutine(End_ConditionEffect(_conDat._scriptableCondition, _conDat));
		_conditionDatas.RemoveAt(index);
		void Init_RemoveConditionObject()
		{
			if ((bool)_conDat._scriptableCondition && (bool)_conDat._scriptableCondition._endConditionObject)
			{
				GameObject gameObject = PoolManager._current.GenerateObject(_conDat._scriptableCondition._endConditionObject, string.Empty, 0f);
				gameObject.transform.SetParent(base.transform);
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.GetComponent<ConditionObject>()._statusEntity = this;
				if ((bool)_isCreep)
				{
					gameObject.transform.localScale = _isCreep._creepObject.transform.localScale;
				}
				else
				{
					gameObject.transform.localScale = base.transform.localScale;
				}
			}
		}
		void Remove_ConditionDisplays()
		{
			if (_checkIfLocalPlayer && (bool)_conDat._scriptableCondition._conditionIcon)
			{
				for (int j = 0; j < InGameUI._current._conditionDataEntries.Count; j++)
				{
					if (InGameUI._current._conditionDataEntries[j]._conditionData == _conDat)
					{
						Object.Destroy(InGameUI._current._conditionDataEntries[j].gameObject);
						InGameUI._current._conditionDataEntries.RemoveAt(j);
					}
				}
			}
			if ((bool)_conDat._conditionEffectObject)
			{
				PoolManager._current.Init_ReturnObjectToContainer(_conDat._conditionEffectObject);
				_conDat._conditionEffectObject.SetActive(value: false);
			}
		}
	}

	private void On_ConditionListClear(bool _checkIfLocalPlayer)
	{
		foreach (ConditionData conditionData in _conditionDatas)
		{
			if (_checkIfLocalPlayer && (bool)conditionData._scriptableCondition._conditionIcon)
			{
				for (int i = 0; i < InGameUI._current._conditionDataEntries.Count; i++)
				{
					if (InGameUI._current._conditionDataEntries[i]._conditionData._scriptableCondition == conditionData._scriptableCondition)
					{
						Object.Destroy(InGameUI._current._conditionDataEntries[i].gameObject);
						InGameUI._current._conditionDataEntries.RemoveAt(i);
					}
				}
			}
			if ((bool)conditionData._conditionEffectObject)
			{
				PoolManager._current.Init_ReturnObjectToContainer(conditionData._conditionEffectObject);
				conditionData._conditionEffectObject.SetActive(value: false);
			}
			StartCoroutine(End_ConditionEffect(conditionData._scriptableCondition, conditionData));
		}
		_conditionDatas.Clear();
	}

	private IEnumerator End_ConditionEffect(ScriptableCondition _scriptCondition, ConditionData _conDat)
	{
		yield return new WaitForSeconds(0.12f);
		_scriptCondition.End_ConditionEffect(this, _conDat);
	}

	private void Handle_AppliedConditions()
	{
		if (_conditionDatas.Count <= 0 || _syncConditions.Count <= 0)
		{
			return;
		}
		if (_currentHealth <= 0 && base.isServer)
		{
			if (_syncConditions.Count > 0)
			{
				_syncConditions.Clear();
			}
		}
		else
		{
			for (int i = 0; i < _conditionDatas.Count && _conditionDatas[i] != null; i++)
			{
				Iterate_Condition(_conditionDatas[i]);
			}
		}
		void Iterate_Condition(ConditionData _conDat)
		{
			Server_ConditionParams();
			Handle_ConditionDuration();
			Handle_ConditionRepeatRate();
			_conDat._scriptableCondition.Update_ConditionEffect(this, _conDat);
			void Handle_ConditionDuration()
			{
				if (_conDat._setDuration > 0 && (!_isPlayer || !_isPlayer._bufferingStatus))
				{
					if (_conDat._currentDuration <= 0f)
					{
						Remove_SyncCondition(_conDat);
					}
					else
					{
						_conDat._currentDuration -= Time.deltaTime;
					}
				}
			}
			void Handle_ConditionRepeatRate()
			{
				if (!(_conDat._setRepeatRate <= 0f) && (!_isPlayer || !_isPlayer._bufferingStatus))
				{
					if (_conDat._currentRepeatRate >= _conDat._setRepeatRate)
					{
						_conDat._scriptableCondition.Init_ConditionEffect(this, _conDat);
						_conDat._currentRepeatRate = 0f;
					}
					else
					{
						_conDat._currentRepeatRate += Time.deltaTime;
					}
				}
			}
			void Server_ConditionParams()
			{
				if (base.isServer)
				{
					if (_currentHealth <= 0)
					{
						Remove_SyncCondition(_conDat);
					}
					else if (_conDat._scriptableCondition._cancelOnAttack)
					{
						if ((bool)_isPlayer)
						{
							PlayerCombat pCombat = _isPlayer._pCombat;
							if (pCombat._meleeHitBox.enabled)
							{
								Init_OnAttackCallback();
							}
							if (pCombat._meleeChargeHitBox.enabled)
							{
								Init_OnAttackCallback();
							}
							if (pCombat._meleeJumpHitBox.enabled)
							{
								Init_OnAttackCallback();
							}
							if (pCombat._meleeJumpLandHitBox.enabled)
							{
								Init_OnAttackCallback();
							}
						}
						else
						{
							_ = (bool)_isCreep;
						}
					}
				}
			}
		}
		void Remove_SyncCondition(ConditionData _conData)
		{
			if (base.isServer)
			{
				for (int j = 0; j < _syncConditions.Count; j++)
				{
					if (_conData._conditionStruct.Equals(_syncConditions[j]))
					{
						_syncConditions.RemoveAt(j);
						break;
					}
				}
			}
		}
	}

	[Command]
	public void Cmd_ReplenishAll()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void StatusEntity::Cmd_ReplenishAll()", 397415994, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ServerCallback]
	public void Replenish_All()
	{
		if (NetworkServer.active)
		{
			if ((bool)_isPlayer)
			{
				Network_currentHealth = _pStats._statStruct._maxHealth;
				Network_currentMana = _pStats._statStruct._maxMana;
				Network_currentStamina = _pStats._statStruct._maxStamina;
				_pCb.Target_GameLogicMessage("You feel completely revitalized.");
			}
			else if ((bool)_isCreep)
			{
				Network_currentHealth = _isCreep._statStruct._maxHealth;
				Network_currentMana = _isCreep._statStruct._maxMana;
			}
		}
	}

	[ServerCallback]
	public void Replenish_Health()
	{
		if (NetworkServer.active)
		{
			if ((bool)_isPlayer)
			{
				Network_currentHealth = _pStats._statStruct._maxHealth;
			}
			else if ((bool)_isCreep)
			{
				Network_currentHealth = _isCreep._statStruct._maxHealth;
			}
		}
	}

	[ServerCallback]
	public void Replenish_Mana()
	{
		if (NetworkServer.active)
		{
			if ((bool)_isPlayer)
			{
				Network_currentMana = _pStats._statStruct._maxMana;
			}
			else if ((bool)_isCreep)
			{
				Network_currentMana = _isCreep._statStruct._maxMana;
			}
		}
	}

	[ServerCallback]
	public void Replenish_Stamina()
	{
		if (NetworkServer.active)
		{
			if ((bool)_isPlayer)
			{
				Network_currentStamina = _pStats._statStruct._maxStamina;
			}
			else if ((bool)_isCreep)
			{
				Network_currentStamina = _isCreep._statStruct._maxStamina;
			}
		}
	}

	[Command]
	public void Cmd_RevivePlayer(StatusEntity _statusEntity)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteNetworkBehaviour(_statusEntity);
		SendCommandInternal("System.Void StatusEntity::Cmd_RevivePlayer(StatusEntity)", 91366272, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ServerCallback]
	public void Init_RevivePlayer(StatusEntity _statusEntity)
	{
		if (NetworkServer.active)
		{
			_statusEntity.Rpc_AngelaTearEffect();
			_statusEntity.Replenish_All();
		}
	}

	[ClientRpc]
	private void Rpc_AngelaTearEffect()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendRPCInternal("System.Void StatusEntity::Rpc_AngelaTearEffect()", -670109881, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	[ServerCallback]
	public void Add_Health(int _value)
	{
		if (NetworkServer.active && _value > 0)
		{
			if ((bool)_isPlayer && _currentHealth < _pStats._statStruct._maxHealth)
			{
				_statusEntityGUI.Target_Display_FloatingNumber(this, FloatTextColor.HEAL, _value, 0, 0);
			}
			Network_currentHealth = _currentHealth + _value;
		}
	}

	[ServerCallback]
	public void Subtract_Health(int _value)
	{
		if (NetworkServer.active && (!_isCreep || !_isCreep._scriptCreep._unkillable) && _currentHealth > 0 && _value > 0)
		{
			Network_currentHealth = _currentHealth - _value;
		}
	}

	[ServerCallback]
	public void Add_Mana(int _value)
	{
		if (NetworkServer.active && _value > 0)
		{
			if ((bool)_isPlayer && _currentMana < _isPlayer._pStats._statStruct._maxMana)
			{
				_statusEntityGUI.Target_Display_FloatingNumber(this, FloatTextColor.MANA, _value, 0, 0);
			}
			Network_currentMana = _currentMana + _value;
			_canRegenMana = false;
			_regenManaBuffer = 0f;
		}
	}

	[ServerCallback]
	public void Subtract_Mana(int _value)
	{
		if (NetworkServer.active && _currentMana > 0 && _value > 0)
		{
			Network_currentMana = _currentMana - _value;
			_canRegenMana = false;
			_regenManaBuffer = 0f;
		}
	}

	[ServerCallback]
	public void Add_Stamina(int _value)
	{
		if (NetworkServer.active && _value > 0)
		{
			Network_currentStamina = _currentStamina + _value;
			_canRegenStamina = false;
			_regenStamBuffer = 0f;
		}
	}

	[Command]
	public void Cmd_SubtractStamina(int _value)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteInt(_value);
		SendCommandInternal("System.Void StatusEntity::Cmd_SubtractStamina(System.Int32)", -6475473, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ServerCallback]
	public void Subtract_Stamina(int _value)
	{
		if (NetworkServer.active && _value > 0)
		{
			Network_currentStamina = _currentStamina - _value;
			_canRegenStamina = false;
			_regenStamBuffer = 0f;
		}
	}

	[ClientRpc]
	public void Rpc_DisplayHitEffect(DamageWeight _hitWeight, int _combatElementID, Vector3 _hitPoint)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		GeneratedNetworkCode._Write_DamageWeight(writer, _hitWeight);
		writer.WriteInt(_combatElementID);
		writer.WriteVector3(_hitPoint);
		SendRPCInternal("System.Void StatusEntity::Rpc_DisplayHitEffect(DamageWeight,System.Int32,UnityEngine.Vector3)", -467444837, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	public void Rpc_DisplayBlockHitEffect(StatusEntity _targetEntity, int _projectedStamina, bool _isParry, bool _isMagicBlock, Vector3 _hitPoint)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteNetworkBehaviour(_targetEntity);
		writer.WriteInt(_projectedStamina);
		writer.WriteBool(_isParry);
		writer.WriteBool(_isMagicBlock);
		writer.WriteVector3(_hitPoint);
		SendRPCInternal("System.Void StatusEntity::Rpc_DisplayBlockHitEffect(StatusEntity,System.Int32,System.Boolean,System.Boolean,UnityEngine.Vector3)", 1554609816, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	public void Rpc_DisplayAbsorbHitEffect()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendRPCInternal("System.Void StatusEntity::Rpc_DisplayAbsorbHitEffect()", 1710853257, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	public void Rpc_DisplayCritHitEffect(StatusEntity _statusEntity, Vector3 _hitPoint)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteNetworkBehaviour(_statusEntity);
		writer.WriteVector3(_hitPoint);
		SendRPCInternal("System.Void StatusEntity::Rpc_DisplayCritHitEffect(StatusEntity,UnityEngine.Vector3)", -1340769894, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	public void Rpc_DisplayMissHitEffect(StatusEntity _statusEntity)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteNetworkBehaviour(_statusEntity);
		SendRPCInternal("System.Void StatusEntity::Rpc_DisplayMissHitEffect(StatusEntity)", 527911973, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	[Command]
	public void Cmd_TakeDamage(int _value, float _force, bool _isDirectional, bool _shatterBlock, DamageWeight _damageWeight, Vector3 _hitPoint, Vector3 _setDir)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteInt(_value);
		writer.WriteFloat(_force);
		writer.WriteBool(_isDirectional);
		writer.WriteBool(_shatterBlock);
		GeneratedNetworkCode._Write_DamageWeight(writer, _damageWeight);
		writer.WriteVector3(_hitPoint);
		writer.WriteVector3(_setDir);
		SendCommandInternal("System.Void StatusEntity::Cmd_TakeDamage(System.Int32,System.Single,System.Boolean,System.Boolean,DamageWeight,UnityEngine.Vector3,UnityEngine.Vector3)", 1768443386, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ServerCallback]
	public virtual void Take_Damage(DamageStruct _dmgStruct)
	{
		if (!NetworkServer.active)
		{
			return;
		}
		if (_dmgStruct._entityDamageType == EntityDamageType.CREEP && (bool)_isCreep)
		{
			return;
		}
		if ((bool)_isPlayer && _isPlayer._pCombat._isBlocking && _isPlayer._pCombat._parryChanceBuffer > 0f && !_dmgStruct._isProjectileCollider)
		{
			Network_generalHitBuffer = 0f;
			_damageBuffer = 0f;
		}
		if (_currentHealth <= 0 || _damageBuffer > 0f || _generalHitBuffer > 0f)
		{
			return;
		}
		int _setDmg = _dmgStruct._damageValue;
		float _damageBufferDefaultValue = 0.126f;
		bool _successfulBlock = false;
		bool _lowLevelBlock = false;
		bool _absorbHit = false;
		FloatTextColor _setFloatTextColor = Set_FloatTextColor();
		if (_dmgStruct._forceConditionApply)
		{
			Apply_DamageStructCondition(_dmgStruct, _successfulBlock: false, _absorbHit: false);
		}
		Handle_CreepTakeDamage();
		Handle_PlayerTakeDamage();
		if (_dmgStruct._colliderType == CombatColliderType.OFFENSIVE)
		{
			if (_dmgStruct._damageValue <= 0)
			{
				if (!_successfulBlock && !_dmgStruct._forceThreat && !_dmgStruct._forceConditionApply)
				{
					Rpc_DisplayMissHitEffect(this);
					if ((bool)_pStats)
					{
						_pStats.Iterate_EquipConditions(EquipConditionCallback.Evade_Hit, null);
					}
				}
			}
			else if (_damageAbsorbtion > 0f && !_successfulBlock)
			{
				_damageBuffer = _damageBufferDefaultValue;
				Rpc_DisplayAbsorbHitEffect();
			}
			else
			{
				if ((bool)_dmgStruct._statusEntity)
				{
					if (!_successfulBlock && (bool)_dmgStruct._statusEntity._pStats)
					{
						_dmgStruct._statusEntity._pStats.Iterate_EquipConditions(EquipConditionCallback.Attack_Hit, _dmgStruct._scriptableSkill);
					}
					if (!_successfulBlock && _dmgStruct._siphonHealthPercent > 0f)
					{
						int value = (int)((float)_setDmg * _dmgStruct._siphonHealthPercent);
						_dmgStruct._statusEntity.Add_Health(value);
					}
				}
				Rpc_DisplayHitEffect(_dmgStruct._damageWeight, _dmgStruct._damageElement._elementID, _dmgStruct._colliderHitPoint);
			}
		}
		if (_dmgStruct._isCriticalHit && _dmgStruct._damageValue > 0)
		{
			if ((bool)_dmgStruct._statusEntity)
			{
				Rpc_DisplayCritHitEffect(_dmgStruct._statusEntity, _dmgStruct._colliderHitPoint);
			}
			if (_currentHealth <= 0)
			{
				Network_currentHealth = -999;
			}
			if ((bool)_dmgStruct._statusEntity._pStats)
			{
				_dmgStruct._statusEntity._pStats.Iterate_EquipConditions(EquipConditionCallback.CriticalStrike_Hit, _dmgStruct._scriptableSkill);
			}
		}
		Apply_DamageStructCondition(_dmgStruct, _successfulBlock, _absorbHit);
		if ((bool)_dmgStruct._statusEntity && _dmgStruct._statusEntity.TryGetComponent<Player>(out var component))
		{
			Init_PvpFragMessage(component);
		}
		void Apply_BlockChance()
		{
			float _blockMod = 0f;
			int num = 0;
			int num2 = 0;
			bool _isMagicDamage = _dmgStruct._damageType == DamageType.Mind;
			if (!_dmgStruct._isCriticalHit && (!_isCreep || !_isCreep._isParryStaggered))
			{
				if ((bool)_isPlayer && !_dmgStruct._ignoreDefenseBlock)
				{
					num = _isPlayer._pStats._statStruct._defense;
					num2 = _isPlayer._pStats._statStruct._magicDefense;
					_blockMod = (float)_dmgStruct._levelDifference * 0.0086f;
				}
				else if ((bool)_isCreep)
				{
					num = _isCreep._statStruct._defense;
					num2 = _isCreep._statStruct._magicDefense;
					_blockMod = (float)_dmgStruct._levelDifference * 0.046f;
				}
				if (!_isMagicDamage && _dmgStruct._damageValue <= num)
				{
					Init_Block();
				}
				else if (_dmgStruct._damageValue <= num2)
				{
					Init_Block();
				}
			}
			void Init_Block()
			{
				if (!_isPlayer || (_isPlayer._currentPlayerAction != PlayerAction.CASTING && !_isPlayer._pCasting._currentCastSkill))
				{
					float num3 = Random.Range(0f, 1f);
					float num4 = 0.12f;
					if (_dmgStruct._levelDifference < 0)
					{
						num4 += _blockMod * -1f;
					}
					if (num4 > 1f)
					{
						num4 = 1f;
					}
					if (num3 <= num4)
					{
						Rpc_DisplayBlockHitEffect(this, 1, _isParry: false, _isMagicDamage, _dmgStruct._colliderHitPoint);
						_successfulBlock = true;
						if ((bool)_pStats)
						{
							_pStats.Iterate_EquipConditions(EquipConditionCallback.Block_Hit, null);
						}
					}
				}
			}
		}
		void Apply_CreepKnockbackParams()
		{
			if (!_isCreep._scriptCreep._immuneToKnockback && !_immuneToKnockback && (!(_isCreep._attackActionBuffer > 0f) || _dmgStruct._isCriticalHit))
			{
				Rigidbody creepRB = _isCreep._creepRB;
				float num = _dmgStruct._appliedForce;
				switch (_dmgStruct._damageWeight)
				{
				case DamageWeight.Light:
					num *= 1.45f;
					break;
				case DamageWeight.Medium:
					num *= 2.95f;
					break;
				case DamageWeight.Heavy:
					num *= 4.75f;
					break;
				}
				float num2 = Mathf.Clamp(num, 0f, 285.65f);
				num2 *= 0.565f;
				if (!(num2 <= 0f))
				{
					_isCreep._pathFindCtrl.Disable_Pathfinding(0.6f);
					Vector3 velocity = base.transform.forward * (0f - num2);
					if (_dmgStruct._setDamageDirection)
					{
						velocity = _dmgStruct._damageDirection * num2;
					}
					if (num2 >= 65.6f)
					{
						_isCreep.Server_CrossFadeAnim("Hurt_Heavy", 0.01f, 2);
						_isCreep.Server_CrossFadeAnim("Empty", 0f, 1);
						_isCreep._knockbackBuffer = 0.15f;
					}
					velocity.y = num2 * 0.492f;
					creepRB.velocity = velocity;
				}
			}
		}
		void Apply_CreepThreat()
		{
			if ((bool)_dmgStruct._statusEntity)
			{
				Player isPlayer = _dmgStruct._statusEntity._isPlayer;
				if ((bool)isPlayer && (!isPlayer || isPlayer._currentPlayerCondition == PlayerCondition.ACTIVE))
				{
					switch (_dmgStruct._damageWeight)
					{
					case DamageWeight.Light:
						_isCreep.Server_AddThreat(_dmgStruct._statusEntity, _dmgStruct._threatAmount);
						break;
					case DamageWeight.Medium:
						_isCreep.Server_AddThreat(_dmgStruct._statusEntity, _dmgStruct._threatAmount + 2f);
						break;
					case DamageWeight.Heavy:
						_isCreep.Server_AddThreat(_dmgStruct._statusEntity, _dmgStruct._threatAmount + 4f);
						break;
					}
				}
			}
		}
		void Handle_CreepTakeDamage()
		{
			Rigidbody _creepRB;
			if ((bool)_isCreep && _isCreep._currentCreepCondition == CreepCondition.ACTIVE && (_dmgStruct._entityDamageType != EntityDamageType.PLAYER || _dmgStruct._colliderType != CombatColliderType.SUPPORT))
			{
				_creepRB = _isCreep._creepRB;
				if (_dmgStruct._damageValue <= 0)
				{
					if (_dmgStruct._forceThreat || _dmgStruct._forceConditionApply)
					{
						_isCreep.Server_AddThreat(_dmgStruct._statusEntity, _dmgStruct._threatAmount);
					}
				}
				else if (_currentHealth <= 0)
				{
					_isCreep.BaseCreep_OnDeath();
				}
				else
				{
					Apply_CreepThreat();
					Apply_BlockChance();
					Apply_CreepHitStun();
					Apply_CreepKnockbackParams();
					if (!_successfulBlock)
					{
						if ((bool)_dmgStruct._statusEntity)
						{
							StatusEntityGUI statusEntityGUI = _dmgStruct._statusEntity._statusEntityGUI;
							if ((bool)statusEntityGUI)
							{
								statusEntityGUI.Target_Display_FloatingNumber(this, _setFloatTextColor, _dmgStruct._damageValue, _dmgStruct._damageElement._elementID, 0);
							}
						}
						_isCreep.Server_BaseCreep_OnHurt();
						Subtract_Health(_setDmg);
					}
				}
			}
			void Apply_CreepHitStun()
			{
				if (!_isCreep._scriptCreep._immuneToHitstun && !_isCreep._scriptCreep._isElite && !_successfulBlock && (!(_isCreep._attackActionBuffer > 0f) || _dmgStruct._isCriticalHit) && !_dmgStruct._noHitStun)
				{
					float num = 0f;
					switch (_dmgStruct._damageWeight)
					{
					case DamageWeight.Light:
						num = Random.Range(-2f, 1f);
						break;
					case DamageWeight.Medium:
						num = Random.Range(-1f, 1f);
						break;
					case DamageWeight.Heavy:
						num = Random.Range(0.612f, 1f);
						break;
					}
					if (_dmgStruct._isCriticalHit)
					{
						num = 1f;
					}
					if (_dmgStruct._levelDifference < 0)
					{
						num -= 0.13f * (float)_dmgStruct._levelDifference;
					}
					if (num >= 0.85f)
					{
						_isCreep.Server_CrossFadeAnim("Empty", 0f, 1);
						_isCreep.Server_CrossFadeAnim("Hurt", 0f, 2);
						_isCreep._creepBehavior.Reset_Behavior();
						if (!_isCreep._scriptCreep._immuneToKnockback)
						{
							switch (_dmgStruct._damageWeight)
							{
							case DamageWeight.Light:
								_creepRB.AddForce(_dmgStruct._damageDirection * 2.95f, ForceMode.Impulse);
								break;
							case DamageWeight.Medium:
								_creepRB.AddForce(_dmgStruct._damageDirection * 7.95f, ForceMode.Impulse);
								break;
							case DamageWeight.Heavy:
								_creepRB.AddForce(_dmgStruct._damageDirection * 32.95f, ForceMode.Impulse);
								_creepRB.AddForce(Vector3.up * 65.95f, ForceMode.Impulse);
								break;
							}
						}
					}
					else if (_dmgStruct._damageWeight == DamageWeight.Heavy && !_isCreep._scriptCreep._immuneToKnockback)
					{
						_creepRB.AddForce(_dmgStruct._damageDirection * 12.95f, ForceMode.Impulse);
						_isCreep.Server_CrossFadeAnim("Hurt", 0f, 2);
					}
				}
			}
		}
		void Handle_DamageAbsorbtion()
		{
			if (_damageAbsorbtion > 0f && _setDmg > 0)
			{
				int num = (int)(_damageAbsorbtion - (float)_setDmg);
				if (num < 0)
				{
					num = 0;
				}
				_setDmg -= (int)_damageAbsorbtion;
				Network_damageAbsorbtion = num;
				_absorbHit = _damageAbsorbtion > 0f;
				if (_setDmg < 1)
				{
					_setDmg = 1;
				}
			}
		}
		void Handle_PlayerTakeDamage()
		{
			PlayerVisual _pVisual;
			PlayerCombat _pCombat;
			PlayerCasting _pCasting;
			PlayerMove _pMove;
			bool _isIframeEvade;
			bool _isParry;
			int _resultStamina;
			if ((bool)_isPlayer && (_dmgStruct._entityDamageType != EntityDamageType.PLAYER || _dmgStruct._colliderType != CombatColliderType.SUPPORT) && _isPlayer._currentPlayerCondition == PlayerCondition.ACTIVE && !_isPlayer._bufferingStatus && !_isPlayer._isHurt)
			{
				_pVisual = _isPlayer._pVisual;
				_pCombat = _isPlayer._pCombat;
				_pCasting = _isPlayer._pCasting;
				_pMove = _isPlayer._pMove;
				_isIframeEvade = false;
				Check_BlockConditions();
				if (_dmgStruct._damageValue <= 0)
				{
					_damageBuffer = _damageBufferDefaultValue;
				}
				else
				{
					Check_IFrameConditions();
					if (_isIframeEvade)
					{
						_damageBuffer = _damageBufferDefaultValue;
					}
					else if (_dmgStruct._forceOnly)
					{
						Apply_PlayerKnockbackParams();
						_damageBuffer = _damageBufferDefaultValue;
					}
					else
					{
						if ((bool)_dmgStruct._statusEntity && (bool)_dmgStruct._statusEntity._isPlayer)
						{
							StatusEntityGUI statusEntityGUI = _dmgStruct._statusEntity._statusEntityGUI;
							if ((bool)statusEntityGUI)
							{
								statusEntityGUI.Target_Display_FloatingNumber(this, _setFloatTextColor, _dmgStruct._damageValue, _dmgStruct._damageElement._elementID, 0);
							}
						}
						if (_successfulBlock && !_lowLevelBlock)
						{
							if ((bool)_pStats)
							{
								_pStats.Iterate_EquipConditions(EquipConditionCallback.Block_Hit, null);
							}
						}
						else
						{
							Handle_DamageAbsorbtion();
							if (!_absorbHit)
							{
								if (_dmgStruct._entityDamageType == EntityDamageType.PLAYER)
								{
									Network_generalHitBuffer = GameManager._current._statLogics._pvpPlayerHitBuffer;
								}
								else
								{
									Network_generalHitBuffer = GameManager._current._statLogics._pvePlayerHitBuffer;
								}
								if (_dmgStruct._isCriticalHit)
								{
									_statusEntityGUI.Target_Display_FloatingNumber(this, FloatTextColor.CRITICAL, _setDmg, _dmgStruct._damageElement._elementID, 0);
								}
								else
								{
									_statusEntityGUI.Target_Display_FloatingNumber(this, _setFloatTextColor, _setDmg, _dmgStruct._damageElement._elementID, 0);
								}
								Apply_PlayerKnockbackParams();
								if (_dmgStruct._damageValue != 0)
								{
									_damageBuffer = _damageBufferDefaultValue;
								}
								if (_dmgStruct._damageValue > 0 && _isPlayer._currentPlayerAction == PlayerAction.CASTING && (bool)_pCasting._currentCastSkill && (_pCasting._currentCastSkill == GameManager._current._statLogics._recallSkill || _pCasting._currentCastSkill == GameManager._current._statLogics._reviveSkill))
								{
									_pCasting.Server_InterruptCast();
								}
								Subtract_Health(_setDmg);
								_isPlayer.Network_isHurt = true;
								if ((bool)_pStats)
								{
									_pStats.Iterate_EquipConditions(EquipConditionCallback.Hurt_Hit, null);
								}
							}
						}
					}
				}
			}
			void Apply_PlayerKnockbackParams()
			{
				float _clampedForce;
				if (!_immuneToKnockback && !_successfulBlock)
				{
					float num = _dmgStruct._appliedForce;
					switch (_dmgStruct._damageWeight)
					{
					case DamageWeight.Light:
						num *= 1.45f;
						break;
					case DamageWeight.Medium:
						num *= 2.95f;
						break;
					case DamageWeight.Heavy:
						num *= 4.75f;
						break;
					}
					_clampedForce = Mathf.Clamp(num, 0f, 64.65f);
					if (_dampenKnockback && !_dmgStruct._forceOnly && (bool)_dmgStruct._statusEntity && _dmgStruct._levelDifference <= 2)
					{
						_clampedForce = 20f;
					}
					if ((bool)_dmgStruct._statusEntity && !_dmgStruct._forceOnly && _dmgStruct._levelDifference <= 5 && (!_dmgStruct._statusEntity._isCreep || !_dmgStruct._statusEntity._isCreep._scriptCreep._isElite))
					{
						_clampedForce = 20f;
					}
					if (_clampedForce < 45.65f)
					{
						_pVisual.IncludeRpc_Crossfade("hurt_small", 0f, 1);
						_isPlayer._currentHurtBuffer = 0.165f;
						Init_KnockbackJump();
					}
					else
					{
						_pVisual.IncludeRpc_Crossfade("hurt_big", 0f, 1);
						_pVisual.Rpc_RandomSpinPlayerModel(95.9f);
						_pVisual.Rpc_PreventRotationLerp(0.65f);
						_isPlayer._currentHurtBuffer = 0.465f;
						_pMove.Target_LockMovement(MovementLockType.SOFT_LOCK, 0.03f);
						if (_pCasting._isCasting || (bool)_pCasting._currentCastSkill)
						{
							_pCasting.Server_InterruptCast();
						}
						_pCombat.Server_InterruptCombat(_resetAnimation: true);
						Init_KnockbackJump();
					}
				}
				void Init_KnockbackJump()
				{
					if (!_pCombat._isAirAttacked || !(_clampedForce < 45.65f))
					{
						if (_dmgStruct._setDamageDirection)
						{
							_pMove.Target_InitJump(_clampedForce, 0f, _clampedForce + Random.Range(0f, 9.5f), _dmgStruct._damageDirection, 45.5f);
						}
						else if (_clampedForce != 0f)
						{
							_pMove.Target_InitJump(_clampedForce, 0f - _clampedForce - Random.Range(0f, 9.5f), 0f, Vector3.zero, 45.5f);
						}
					}
				}
			}
			void Check_BlockConditions()
			{
				if (!_pCombat._isBlocking && !_autoParry)
				{
					if ((_pVisual._visualAnimator.GetCurrentAnimatorStateInfo(3).IsName("Empty") || (bool)_pCasting._currentCastSkill) && !_dmgStruct._ignoreDefenseBlock)
					{
						Apply_BlockChance();
					}
				}
				else
				{
					_isParry = ((_dmgStruct._levelDifference <= 3 && _pCombat._parryChanceBuffer > 0f) || _autoParry) && _dmgStruct._allowParryStagger;
					Init_Reflect();
					int num = 2;
					if ((bool)_dmgStruct._statusEntity && (bool)_dmgStruct._statusEntity._isCreep && _dmgStruct._statusEntity._isCreep._scriptCreep._isElite)
					{
						num = 6;
					}
					if (_dmgStruct._levelDifference > 0)
					{
						num *= (int)((float)_dmgStruct._levelDifference * _staminaUseOnBlockPercent);
					}
					if (_chanceToIgnoreBlockStaminaCost > 0f && Random.Range(0f, 1f) <= _chanceToIgnoreBlockStaminaCost)
					{
						num = 0;
					}
					if (_dmgStruct._shatterBlock)
					{
						num = 9999;
					}
					num = (int)((float)num * _staminaUseOnBlockPercent);
					if (!_isParry)
					{
						Network_currentStamina = _currentStamina - num;
					}
					_resultStamina = _currentStamina;
					Init_PlayerBlockHit();
					Init_BlockKnockback();
				}
			}
			void Check_IFrameConditions()
			{
				if (_dmgStruct._levelDifference < 5 && !_dmgStruct._ignoreIFrames && _isPlayer._inIFrame)
				{
					Subtract_Stamina(3);
					_dmgStruct._damageValue = 0;
					_isIframeEvade = true;
				}
			}
		}
		void Init_ParryHit()
		{
			if ((!(P_1._pCombat._parryChanceBuffer <= 0f) || _autoParry) && _dmgStruct._allowParryStagger && (bool)_dmgStruct._statusEntity)
			{
				bool num = _dmgStruct._statusEntity._isCreep;
				bool flag = _dmgStruct._statusEntity._isPlayer;
				if (num)
				{
					Creep isCreep = _dmgStruct._statusEntity._isCreep;
					CreepBehavior creepBehavior = isCreep._creepBehavior;
					if (isCreep._currentCreepCondition != CreepCondition.ACTIVE || !isCreep._scriptCreep._canParry || isCreep._parryBuffer > 0f || !creepBehavior)
					{
						return;
					}
					if (!_dmgStruct._isProjectileCollider)
					{
						if (_dmgStruct._levelDifference <= 0)
						{
							isCreep._parryBuffer = _parryBufferApply;
						}
						else if (_dmgStruct._levelDifference == 1)
						{
							isCreep._parryBuffer = _parryBufferApply - 0.1f;
						}
						else if (_dmgStruct._levelDifference == 2)
						{
							isCreep._parryBuffer = _parryBufferApply - 0.2f;
						}
						else if (_dmgStruct._levelDifference == 3)
						{
							isCreep._parryBuffer = _parryBufferApply - 1.2f;
						}
						else if (_dmgStruct._levelDifference == 4)
						{
							isCreep._parryBuffer = _parryBufferApply - 1.8f;
						}
						else if (_dmgStruct._levelDifference >= 5)
						{
							isCreep._parryBuffer = 0f;
						}
						isCreep.Rpc_CrossFadeAnim("Hurt_Heavy", 0f, 2);
						isCreep.Rpc_CrossFadeAnim("Empty", 0f, 1);
						isCreep._creepRB.velocity = new Vector3(0f, isCreep._creepRB.velocity.y, 0f);
						isCreep._parryBuffer -= isCreep._scriptCreep._parryStunMod;
					}
				}
				else if (flag)
				{
					Player isPlayer = _dmgStruct._statusEntity._isPlayer;
					PlayerVisual pVisual = isPlayer._pVisual;
					PlayerMove pMove = isPlayer._pMove;
					if (!_dmgStruct._isProjectileCollider && isPlayer._currentPlayerCondition == PlayerCondition.ACTIVE)
					{
						pMove.Target_LockMovement(MovementLockType.SOFT_LOCK, 0.35f);
						pMove.Target_InitJump(42.5f, -35.5f, 0f, Vector3.zero, 90.5f);
						pVisual.Send_CrossFadeAnim("Block_Shattered", 0.01f, 3, LatencyCheck.IGNORE_LATENCY);
						P_1._isParry = true;
					}
				}
				if (P_1._isParry)
				{
					P_1._pCombat._parryChanceBuffer += 0.08f;
					if ((bool)_pStats)
					{
						_pStats.Iterate_EquipConditions(EquipConditionCallback.Parry_Hit, null);
					}
				}
			}
		}
		void Init_PlayerBlockHit()
		{
			string animName = "Block_NoShield_hit";
			if ((bool)P_1._pCombat._equippedShield && P_1._pCombat._currentScriptableWeaponType._weaponHandedness == WeaponHandedness.One_Handed)
			{
				animName = "Block_Shield_hit";
			}
			if (_dmgStruct._setDamageDirection && !P_1._pCombat._isAirAttacked)
			{
				P_1._pMove.Target_SetRotation(Quaternion.LookRotation(-_dmgStruct._damageDirection));
			}
			if (base.isServerOnly)
			{
				if (P_1._resultStamina > 0)
				{
					P_1._pVisual.Local_CrossFadeAnim(animName, 0.01f, 3);
				}
				else
				{
					P_1._pVisual.Local_CrossFadeAnim("Block_Shattered", 0.01f, 3);
				}
			}
			Init_ParryHit();
			Rpc_DisplayBlockHitEffect(this, P_1._resultStamina, P_1._isParry, _dmgStruct._damageType == DamageType.Mind, _dmgStruct._colliderHitPoint);
			P_1._pCombat.Server_InterruptCombat(_resetAnimation: false);
			if ((bool)P_1._pCasting._currentCastSkill || P_1._pCasting._isCasting)
			{
				P_1._pCasting.Server_InterruptCast();
			}
			_successfulBlock = true;
			_lowLevelBlock = _dmgStruct._levelDifference >= 5 || _dmgStruct._shatterBlock;
			if (_lowLevelBlock)
			{
				_setDmg = (int)((float)_setDmg * 0.48f);
			}
			_damageBuffer = 0.3f;
		}
		void Init_Reflect()
		{
			if (_reflectDamage && !_dmgStruct._isProjectileCollider && P_1._isParry)
			{
				int num = 0;
				int num2 = Random.Range(0, 6);
				if ((bool)_dmgStruct._combatCollider._isParentPlayer)
				{
					num = (int)((float)_dmgStruct._damageValue * 0.08f + (float)num2);
					StatusEntity statusEntity = _dmgStruct._combatCollider._isParentPlayer._player._statusEntity;
					statusEntity.Network_currentHealth = statusEntity._currentHealth - (int)((float)_dmgStruct._damageValue * _reflectPercent);
				}
				else if ((bool)_dmgStruct._combatCollider._isParentCreep)
				{
					num = (int)((float)_dmgStruct._combatCollider._isParentCreep._scriptCreep._baseDamage * _reflectPercent) + num2;
					StatusEntity statusEntity2 = _dmgStruct._combatCollider._isParentCreep._statusEntity;
					statusEntity2.Network_currentHealth = statusEntity2._currentHealth - num;
				}
				if ((bool)_isPlayer)
				{
					_isPlayer._statusEntity._statusEntityGUI.Target_Display_FloatingNumber(_dmgStruct._statusEntity, _setFloatTextColor, num, 0, 0);
				}
			}
		}
		FloatTextColor Set_FloatTextColor()
		{
			if (_dmgStruct._isCriticalHit)
			{
				return FloatTextColor.CRITICAL;
			}
			return _dmgStruct._damageType switch
			{
				DamageType.Strength => FloatTextColor.PHYSICAL_DAMAGE, 
				DamageType.Dexterity => FloatTextColor.DEX_DAMAGE, 
				DamageType.Mind => FloatTextColor.MAGICAL_DAMAGE, 
				DamageType.Pure => FloatTextColor.PHYSICAL_DAMAGE, 
				_ => FloatTextColor.PHYSICAL_DAMAGE, 
			};
		}
	}

	[ServerCallback]
	public void Init_PvpFragMessage(Player _playerEntity)
	{
		if (NetworkServer.active && (bool)_isPlayer && _currentHealth <= 0)
		{
			_playerEntity._chatBehaviour.Target_RecieveTriggerMessage("You fragged " + _isPlayer._nickname + ".");
		}
	}

	[ServerCallback]
	public void Apply_DamageStructCondition(DamageStruct _dmgStruct, bool _successfulBlock, bool _absorbHit)
	{
		if (!NetworkServer.active)
		{
			return;
		}
		StatusEntity senderEntity = null;
		if ((bool)_dmgStruct._statusEntity)
		{
			senderEntity = _dmgStruct._statusEntity;
		}
		for (int i = 0; i < _syncConditions.Count; i++)
		{
			if (_dmgStruct._damageValue <= 0)
			{
				break;
			}
			if (_dmgStruct._colliderType != CombatColliderType.SUPPORT || !_isPlayer)
			{
				ScriptableCondition scriptableCondition = GameManager._current.Locate_Condition(_syncConditions[i]._conditionID);
				if ((bool)scriptableCondition && scriptableCondition._cancelOnHit)
				{
					_syncConditions.RemoveAt(i);
				}
			}
		}
		if (_dmgStruct._conditionSlot._scriptableCondition == null || (_dmgStruct._damageValue <= 0 && !_dmgStruct._forceConditionApply) || _successfulBlock || _absorbHit)
		{
			return;
		}
		float num = Random.Range(0f, 1f);
		bool flag = _dmgStruct._conditionSlot._chance >= 1f;
		if (_dmgStruct._levelDifference < 0)
		{
			num -= 0.1f * (float)_dmgStruct._levelDifference;
		}
		if (_dmgStruct._colliderType == CombatColliderType.SUPPORT)
		{
			if ((bool)_dmgStruct._statusEntity._isPlayer && (bool)_isPlayer)
			{
				num = _dmgStruct._conditionSlot._chance;
			}
			if ((bool)_dmgStruct._statusEntity._isCreep && (bool)_isCreep)
			{
				num = _dmgStruct._conditionSlot._chance;
			}
		}
		if (num > _dmgStruct._conditionSlot._chance)
		{
			if (flag)
			{
				ResistCondition_FloatValue(senderEntity, this);
			}
		}
		else
		{
			int conditionPower = _dmgStruct._conditionSlot._conditionPower;
			int conditionDuration = _dmgStruct._conditionSlot._conditionDuration;
			float powerPercent = _dmgStruct._conditionSlot._powerPercent;
			float conditionRepeatRate = _dmgStruct._conditionSlot._conditionRepeatRate;
			_dmgStruct._conditionSlot._scriptableCondition.Apply_ConditionToEntity(senderEntity, this, _dmgStruct._damageType, conditionPower, conditionDuration, powerPercent, conditionRepeatRate);
		}
	}

	public void ResistCondition_FloatValue(StatusEntity _senderEntity, StatusEntity _targetEntity)
	{
		if ((bool)_senderEntity._isPlayer)
		{
			_senderEntity._statusEntityGUI.Target_Display_FloatingNumber(_targetEntity, FloatTextColor.MISS, -7, 0, 0);
		}
	}

	[ServerCallback]
	private void Init_BlockKnockback()
	{
		if (NetworkServer.active && (bool)_isPlayer && !_immuneToKnockback)
		{
			_isPlayer._pMove.Target_LockMovement(MovementLockType.SOFT_LOCK, 0.2f);
			_isPlayer._pMove.Target_InitJump(48.5f, -42.5f, 0f, Vector3.zero, 90.5f);
		}
	}

	public StatusEntity()
	{
		InitSyncObject(_syncConditions);
	}

	public override bool Weaved()
	{
		return true;
	}

	protected void UserCode_Cmd_RemoveSummon()
	{
		if ((bool)_isPlayer)
		{
			_isPlayer.Add_PacketCount();
		}
		Network_summonSkillTag = string.Empty;
	}

	protected static void InvokeUserCode_Cmd_RemoveSummon(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command Cmd_RemoveSummon called on client.");
		}
		else
		{
			((StatusEntity)obj).UserCode_Cmd_RemoveSummon();
		}
	}

	protected void UserCode_Cmd_ReplenishAll()
	{
		Rpc_AngelaTearEffect();
		Replenish_All();
	}

	protected static void InvokeUserCode_Cmd_ReplenishAll(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command Cmd_ReplenishAll called on client.");
		}
		else
		{
			((StatusEntity)obj).UserCode_Cmd_ReplenishAll();
		}
	}

	protected void UserCode_Cmd_RevivePlayer__StatusEntity(StatusEntity _statusEntity)
	{
		Init_RevivePlayer(_statusEntity);
	}

	protected static void InvokeUserCode_Cmd_RevivePlayer__StatusEntity(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command Cmd_RevivePlayer called on client.");
		}
		else
		{
			((StatusEntity)obj).UserCode_Cmd_RevivePlayer__StatusEntity(reader.ReadNetworkBehaviour<StatusEntity>());
		}
	}

	protected void UserCode_Rpc_AngelaTearEffect()
	{
		if ((bool)Player._mainPlayer && !(_isPlayer.Network_playerMapInstance != Player._mainPlayer.Network_playerMapInstance) && (bool)_isPlayer)
		{
			GameObject obj = PoolManager._current.GenerateObject(null, "angelaTearEffect", 12.75f);
			obj.transform.SetParent(_isPlayer.transform);
			obj.transform.SetPositionAndRotation(_isPlayer.transform.position, Quaternion.identity);
			obj.transform.SetParent(null);
			obj.transform.SetAsLastSibling();
		}
	}

	protected static void InvokeUserCode_Rpc_AngelaTearEffect(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC Rpc_AngelaTearEffect called on server.");
		}
		else
		{
			((StatusEntity)obj).UserCode_Rpc_AngelaTearEffect();
		}
	}

	protected void UserCode_Cmd_SubtractStamina__Int32(int _value)
	{
		Subtract_Stamina(_value);
	}

	protected static void InvokeUserCode_Cmd_SubtractStamina__Int32(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command Cmd_SubtractStamina called on client.");
		}
		else
		{
			((StatusEntity)obj).UserCode_Cmd_SubtractStamina__Int32(reader.ReadInt());
		}
	}

	protected void UserCode_Rpc_DisplayHitEffect__DamageWeight__Int32__Vector3(DamageWeight _hitWeight, int _combatElementID, Vector3 _hitPoint)
	{
		if ((bool)Player._mainPlayer && !Player._mainPlayer._bufferingStatus && (!_isPlayer || !Player._mainPlayer || base.isLocalPlayer || !(Player._mainPlayer.Network_playerMapInstance != _isPlayer.Network_playerMapInstance)))
		{
			ScriptableCombatElement scriptableCombatElement = GameManager._current.Locate_CombatElement(_combatElementID);
			GameObject obj = PoolManager._current.GenerateObject(scriptableCombatElement._elementHitEffect, string.Empty, 1.25f);
			obj.transform.position = _hitPoint;
			PoolManager._current.GenerateObject(null, "hurtCloud", 1.25f).transform.position = _hitPoint;
			AudioSource componentInChildren = obj.GetComponentInChildren<AudioSource>();
			componentInChildren.pitch = Random.Range(0.95862f, 1.095862f);
			componentInChildren.volume = Random.Range(0.35982f, 0.411563f);
			switch (_hitWeight)
			{
			case DamageWeight.Light:
				componentInChildren.clip = scriptableCombatElement._lightHitSfx;
				break;
			case DamageWeight.Medium:
				componentInChildren.clip = scriptableCombatElement._mediumHitSfx;
				break;
			case DamageWeight.Heavy:
				componentInChildren.clip = scriptableCombatElement._heavyHitSfx;
				break;
			}
			componentInChildren.Play();
		}
	}

	protected static void InvokeUserCode_Rpc_DisplayHitEffect__DamageWeight__Int32__Vector3(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC Rpc_DisplayHitEffect called on server.");
		}
		else
		{
			((StatusEntity)obj).UserCode_Rpc_DisplayHitEffect__DamageWeight__Int32__Vector3(GeneratedNetworkCode._Read_DamageWeight(reader), reader.ReadInt(), reader.ReadVector3());
		}
	}

	protected void UserCode_Rpc_DisplayBlockHitEffect__StatusEntity__Int32__Boolean__Boolean__Vector3(StatusEntity _targetEntity, int _projectedStamina, bool _isParry, bool _isMagicBlock, Vector3 _hitPoint)
	{
		if (!Player._mainPlayer || Player._mainPlayer._bufferingStatus || !_targetEntity)
		{
			return;
		}
		int num = -2;
		if (_isMagicBlock)
		{
			num = -8;
		}
		if (_isParry)
		{
			num = -6;
		}
		if ((bool)_isPlayer)
		{
			if ((bool)Player._mainPlayer && !base.isLocalPlayer && Player._mainPlayer.Network_playerMapInstance != _isPlayer.Network_playerMapInstance)
			{
				return;
			}
			PlayerVisual pVisual = _isPlayer._pVisual;
			if (_projectedStamina > 0)
			{
				switch (num)
				{
				case -2:
					GameManager._current.Spawn_FloatingText(num, _targetEntity, FloatTextColor.MISS, 9f, 6.5f);
					break;
				case -6:
					GameManager._current.Spawn_FloatingText(num, _targetEntity, FloatTextColor.PHYSICAL_DAMAGE, 9f, 6.5f);
					break;
				case -8:
					GameManager._current.Spawn_FloatingText(num, _targetEntity, FloatTextColor.MAGICAL_DAMAGE, 9f, 6.5f);
					break;
				}
				if (!_pCombat._isChargingWeapon && _isPlayer._currentPlayerAction != PlayerAction.ATTACKING)
				{
					if ((bool)_pCombat._equippedShield && _pCombat._currentScriptableWeaponType._weaponHandedness == WeaponHandedness.One_Handed)
					{
						pVisual.Local_CrossFadeAnim("Block_Shield_hit", 0.01f, 3);
					}
					else
					{
						pVisual.Local_CrossFadeAnim("Block_NoShield_hit", 0.01f, 3);
					}
				}
			}
			else
			{
				switch (num)
				{
				case -6:
					GameManager._current.Spawn_FloatingText(num, _targetEntity, FloatTextColor.PHYSICAL_DAMAGE, 9f, 6.5f);
					break;
				case -8:
					GameManager._current.Spawn_FloatingText(num, _targetEntity, FloatTextColor.MAGICAL_DAMAGE, 9f, 6.5f);
					break;
				default:
					GameManager._current.Spawn_FloatingText(-1, this, FloatTextColor.MISS, 9f, 6.5f);
					break;
				}
				pVisual.Local_CrossFadeAnim("Block_Shattered", 0.01f, 3);
				pVisual._blockShatterEffect.Play();
			}
		}
		else
		{
			switch (num)
			{
			case -2:
				GameManager._current.Spawn_FloatingText(num, _targetEntity, FloatTextColor.MISS, 9f, 6.5f);
				break;
			case -6:
				GameManager._current.Spawn_FloatingText(num, _targetEntity, FloatTextColor.PHYSICAL_DAMAGE, 9f, 6.5f);
				break;
			case -8:
				GameManager._current.Spawn_FloatingText(num, _targetEntity, FloatTextColor.MAGICAL_DAMAGE, 9f, 6.5f);
				break;
			}
		}
		GameObject gameObject = null;
		gameObject = ((num == -8) ? PoolManager._current.GenerateObject(null, "magicBlockHit", 1.25f) : PoolManager._current.GenerateObject(null, "blockHit", 1.25f));
		gameObject.transform.position = _hitPoint;
		PoolManager._current.GenerateObject(null, "hurtCloud", 1.25f).transform.position = _hitPoint;
		AudioSource componentInChildren = gameObject.GetComponentInChildren<AudioSource>();
		componentInChildren.pitch = Random.Range(0.95862f, 1.095862f);
		componentInChildren.volume = Random.Range(0.65982f, 0.711563f);
		if (_isParry)
		{
			PoolManager._current.GenerateObject(null, "parryHit", 1.25f).transform.position = base.transform.position + new Vector3(0f, 2.5f, 0f);
			componentInChildren.pitch = Random.Range(0.8623478f, 0.8912375f);
		}
		componentInChildren.Play();
		if (base.isClientOnly)
		{
			_damageBuffer = 0.45f;
		}
	}

	protected static void InvokeUserCode_Rpc_DisplayBlockHitEffect__StatusEntity__Int32__Boolean__Boolean__Vector3(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC Rpc_DisplayBlockHitEffect called on server.");
		}
		else
		{
			((StatusEntity)obj).UserCode_Rpc_DisplayBlockHitEffect__StatusEntity__Int32__Boolean__Boolean__Vector3(reader.ReadNetworkBehaviour<StatusEntity>(), reader.ReadInt(), reader.ReadBool(), reader.ReadBool(), reader.ReadVector3());
		}
	}

	protected void UserCode_Rpc_DisplayAbsorbHitEffect()
	{
		if ((bool)Player._mainPlayer && !Player._mainPlayer._bufferingStatus && (!Player._mainPlayer || base.isLocalPlayer || !_isPlayer || !(Player._mainPlayer.Network_playerMapInstance != _isPlayer.Network_playerMapInstance)))
		{
			GameObject obj = PoolManager._current.GenerateObject(null, "absorbHit", 1.25f);
			obj.transform.position = base.transform.position + new Vector3(0f, 1.5f, 0f);
			AudioSource componentInChildren = obj.GetComponentInChildren<AudioSource>();
			componentInChildren.pitch = Random.Range(0.95862f, 1.095862f);
			componentInChildren.volume = Random.Range(0.65982f, 0.711563f);
			componentInChildren.Play();
			GameManager._current.Spawn_FloatingText(-5, this, FloatTextColor.MISS, 9f, 6.5f);
		}
	}

	protected static void InvokeUserCode_Rpc_DisplayAbsorbHitEffect(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC Rpc_DisplayAbsorbHitEffect called on server.");
		}
		else
		{
			((StatusEntity)obj).UserCode_Rpc_DisplayAbsorbHitEffect();
		}
	}

	protected void UserCode_Rpc_DisplayCritHitEffect__StatusEntity__Vector3(StatusEntity _statusEntity, Vector3 _hitPoint)
	{
		if ((bool)Player._mainPlayer && !Player._mainPlayer._bufferingStatus && (bool)_statusEntity && (!_isPlayer || !Player._mainPlayer || base.isLocalPlayer || !(Player._mainPlayer.Network_playerMapInstance != _isPlayer.Network_playerMapInstance)))
		{
			if ((bool)Player._mainPlayer && Player._mainPlayer.netId == _statusEntity.netId)
			{
				CameraFunction._current.MyShaker.Shake(CameraFunction._current.ShakePreset);
			}
			GameObject obj = PoolManager._current.GenerateObject(null, "critHit", 1.25f);
			obj.transform.position = _hitPoint;
			AudioSource componentInChildren = obj.GetComponentInChildren<AudioSource>();
			componentInChildren.pitch = Random.Range(0.95862f, 1.095862f);
			componentInChildren.volume = Random.Range(0.65982f, 0.711563f);
			componentInChildren.Play();
		}
	}

	protected static void InvokeUserCode_Rpc_DisplayCritHitEffect__StatusEntity__Vector3(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC Rpc_DisplayCritHitEffect called on server.");
		}
		else
		{
			((StatusEntity)obj).UserCode_Rpc_DisplayCritHitEffect__StatusEntity__Vector3(reader.ReadNetworkBehaviour<StatusEntity>(), reader.ReadVector3());
		}
	}

	protected void UserCode_Rpc_DisplayMissHitEffect__StatusEntity(StatusEntity _statusEntity)
	{
		if ((bool)Player._mainPlayer && !Player._mainPlayer._bufferingStatus && (bool)_statusEntity && (!_isPlayer || !Player._mainPlayer || base.isLocalPlayer || !(Player._mainPlayer.Network_playerMapInstance != _isPlayer.Network_playerMapInstance)))
		{
			GameObject obj = PoolManager._current.GenerateObject(null, "missHit", 1.25f);
			obj.transform.position = base.transform.position + new Vector3(0f, 2.5f, 0f);
			GameManager._current.Spawn_FloatingText(-4, this, FloatTextColor.MISS, 9f, 6.5f);
			AudioSource componentInChildren = obj.GetComponentInChildren<AudioSource>();
			componentInChildren.pitch = Random.Range(0.95862f, 1.095862f);
			componentInChildren.volume = Random.Range(0.65982f, 0.711563f);
			componentInChildren.Play();
		}
	}

	protected static void InvokeUserCode_Rpc_DisplayMissHitEffect__StatusEntity(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC Rpc_DisplayMissHitEffect called on server.");
		}
		else
		{
			((StatusEntity)obj).UserCode_Rpc_DisplayMissHitEffect__StatusEntity(reader.ReadNetworkBehaviour<StatusEntity>());
		}
	}

	protected void UserCode_Cmd_TakeDamage__Int32__Single__Boolean__Boolean__DamageWeight__Vector3__Vector3(int _value, float _force, bool _isDirectional, bool _shatterBlock, DamageWeight _damageWeight, Vector3 _hitPoint, Vector3 _setDir)
	{
		bool ignoreDefenseBlock = _shatterBlock;
		DamageStruct dmgStruct = new DamageStruct
		{
			_damageValue = _value,
			_damageElement = GameManager._current._statLogics._defaultCombatElement,
			_damageType = DamageType.Pure,
			_entityDamageType = EntityDamageType.NULL,
			_colliderType = CombatColliderType.OFFENSIVE,
			_setDamageDirection = _isDirectional,
			_damageWeight = _damageWeight,
			_appliedForce = _force,
			_ignoreIFrames = true,
			_damageDirection = _setDir,
			_ignoreDefenseBlock = ignoreDefenseBlock,
			_colliderHitPoint = _hitPoint,
			_shatterBlock = _shatterBlock,
			_conditionSlot = new ConditionSlot
			{
				_scriptableCondition = null,
				_chance = 0f
			}
		};
		Take_Damage(dmgStruct);
	}

	protected static void InvokeUserCode_Cmd_TakeDamage__Int32__Single__Boolean__Boolean__DamageWeight__Vector3__Vector3(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command Cmd_TakeDamage called on client.");
		}
		else
		{
			((StatusEntity)obj).UserCode_Cmd_TakeDamage__Int32__Single__Boolean__Boolean__DamageWeight__Vector3__Vector3(reader.ReadInt(), reader.ReadFloat(), reader.ReadBool(), reader.ReadBool(), GeneratedNetworkCode._Read_DamageWeight(reader), reader.ReadVector3(), reader.ReadVector3());
		}
	}

	static StatusEntity()
	{
		RemoteProcedureCalls.RegisterCommand(typeof(StatusEntity), "System.Void StatusEntity::Cmd_RemoveSummon()", InvokeUserCode_Cmd_RemoveSummon, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(StatusEntity), "System.Void StatusEntity::Cmd_ReplenishAll()", InvokeUserCode_Cmd_ReplenishAll, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(StatusEntity), "System.Void StatusEntity::Cmd_RevivePlayer(StatusEntity)", InvokeUserCode_Cmd_RevivePlayer__StatusEntity, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(StatusEntity), "System.Void StatusEntity::Cmd_SubtractStamina(System.Int32)", InvokeUserCode_Cmd_SubtractStamina__Int32, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(StatusEntity), "System.Void StatusEntity::Cmd_TakeDamage(System.Int32,System.Single,System.Boolean,System.Boolean,DamageWeight,UnityEngine.Vector3,UnityEngine.Vector3)", InvokeUserCode_Cmd_TakeDamage__Int32__Single__Boolean__Boolean__DamageWeight__Vector3__Vector3, requiresAuthority: true);
		RemoteProcedureCalls.RegisterRpc(typeof(StatusEntity), "System.Void StatusEntity::Rpc_AngelaTearEffect()", InvokeUserCode_Rpc_AngelaTearEffect);
		RemoteProcedureCalls.RegisterRpc(typeof(StatusEntity), "System.Void StatusEntity::Rpc_DisplayHitEffect(DamageWeight,System.Int32,UnityEngine.Vector3)", InvokeUserCode_Rpc_DisplayHitEffect__DamageWeight__Int32__Vector3);
		RemoteProcedureCalls.RegisterRpc(typeof(StatusEntity), "System.Void StatusEntity::Rpc_DisplayBlockHitEffect(StatusEntity,System.Int32,System.Boolean,System.Boolean,UnityEngine.Vector3)", InvokeUserCode_Rpc_DisplayBlockHitEffect__StatusEntity__Int32__Boolean__Boolean__Vector3);
		RemoteProcedureCalls.RegisterRpc(typeof(StatusEntity), "System.Void StatusEntity::Rpc_DisplayAbsorbHitEffect()", InvokeUserCode_Rpc_DisplayAbsorbHitEffect);
		RemoteProcedureCalls.RegisterRpc(typeof(StatusEntity), "System.Void StatusEntity::Rpc_DisplayCritHitEffect(StatusEntity,UnityEngine.Vector3)", InvokeUserCode_Rpc_DisplayCritHitEffect__StatusEntity__Vector3);
		RemoteProcedureCalls.RegisterRpc(typeof(StatusEntity), "System.Void StatusEntity::Rpc_DisplayMissHitEffect(StatusEntity)", InvokeUserCode_Rpc_DisplayMissHitEffect__StatusEntity);
	}

	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteInt(_currentHealth);
			writer.WriteInt(_currentMana);
			writer.WriteInt(_currentStamina);
			writer.WriteBool(_isDamageBuffer);
			writer.WriteFloat(_generalHitBuffer);
			writer.WriteFloat(_manaRegenRate);
			writer.WriteFloat(_stamRegenRate);
			writer.WriteFloat(_damageAbsorbtion);
			writer.WriteBool(_reflectDamage);
			writer.WriteString(_summonSkillTag);
			writer.WriteNetworkIdentity(Network_summonAttackTag);
			return;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteInt(_currentHealth);
		}
		if ((base.syncVarDirtyBits & 2L) != 0L)
		{
			writer.WriteInt(_currentMana);
		}
		if ((base.syncVarDirtyBits & 4L) != 0L)
		{
			writer.WriteInt(_currentStamina);
		}
		if ((base.syncVarDirtyBits & 8L) != 0L)
		{
			writer.WriteBool(_isDamageBuffer);
		}
		if ((base.syncVarDirtyBits & 0x10L) != 0L)
		{
			writer.WriteFloat(_generalHitBuffer);
		}
		if ((base.syncVarDirtyBits & 0x20L) != 0L)
		{
			writer.WriteFloat(_manaRegenRate);
		}
		if ((base.syncVarDirtyBits & 0x40L) != 0L)
		{
			writer.WriteFloat(_stamRegenRate);
		}
		if ((base.syncVarDirtyBits & 0x80L) != 0L)
		{
			writer.WriteFloat(_damageAbsorbtion);
		}
		if ((base.syncVarDirtyBits & 0x100L) != 0L)
		{
			writer.WriteBool(_reflectDamage);
		}
		if ((base.syncVarDirtyBits & 0x200L) != 0L)
		{
			writer.WriteString(_summonSkillTag);
		}
		if ((base.syncVarDirtyBits & 0x400L) != 0L)
		{
			writer.WriteNetworkIdentity(Network_summonAttackTag);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize(ref _currentHealth, OnHealthChange, reader.ReadInt());
			GeneratedSyncVarDeserialize(ref _currentMana, null, reader.ReadInt());
			GeneratedSyncVarDeserialize(ref _currentStamina, null, reader.ReadInt());
			GeneratedSyncVarDeserialize(ref _isDamageBuffer, null, reader.ReadBool());
			GeneratedSyncVarDeserialize(ref _generalHitBuffer, null, reader.ReadFloat());
			GeneratedSyncVarDeserialize(ref _manaRegenRate, null, reader.ReadFloat());
			GeneratedSyncVarDeserialize(ref _stamRegenRate, null, reader.ReadFloat());
			GeneratedSyncVarDeserialize(ref _damageAbsorbtion, null, reader.ReadFloat());
			GeneratedSyncVarDeserialize(ref _reflectDamage, null, reader.ReadBool());
			GeneratedSyncVarDeserialize(ref _summonSkillTag, null, reader.ReadString());
			GeneratedSyncVarDeserialize_NetworkIdentity(ref _summonAttackTag, null, reader, ref ____summonAttackTagNetId);
			return;
		}
		long num = (long)reader.ReadULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _currentHealth, OnHealthChange, reader.ReadInt());
		}
		if ((num & 2L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _currentMana, null, reader.ReadInt());
		}
		if ((num & 4L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _currentStamina, null, reader.ReadInt());
		}
		if ((num & 8L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _isDamageBuffer, null, reader.ReadBool());
		}
		if ((num & 0x10L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _generalHitBuffer, null, reader.ReadFloat());
		}
		if ((num & 0x20L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _manaRegenRate, null, reader.ReadFloat());
		}
		if ((num & 0x40L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _stamRegenRate, null, reader.ReadFloat());
		}
		if ((num & 0x80L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _damageAbsorbtion, null, reader.ReadFloat());
		}
		if ((num & 0x100L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _reflectDamage, null, reader.ReadBool());
		}
		if ((num & 0x200L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _summonSkillTag, null, reader.ReadString());
		}
		if ((num & 0x400L) != 0L)
		{
			GeneratedSyncVarDeserialize_NetworkIdentity(ref _summonAttackTag, null, reader, ref ____summonAttackTagNetId);
		}
	}
}
