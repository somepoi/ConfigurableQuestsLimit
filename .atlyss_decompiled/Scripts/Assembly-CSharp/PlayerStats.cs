using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Mirror;
using Mirror.RemoteCalls;
using UnityEngine;

public class PlayerStats : NetworkBehaviour
{
	[HideInInspector]
	public Player _player;

	private PlayerVisual _pVisual;

	private PlayerSound _pSound;

	private PlayerCombat _pCombat;

	private PlayerCasting _pCasting;

	private PlayerEquipment _pEquipment;

	private StatusEntity _playerStatus;

	private ChatBehaviour _chatBehaviour;

	[Header("General Parameters")]
	[SyncVar]
	public int _currentLevel;

	[SyncVar]
	public int _currentExp;

	private float _commandBuffer;

	[Header("Stat Struct Repository")]
	[SyncVar(hook = "OnStatStructChange")]
	public StatStruct _statStruct;

	[SyncVar]
	public StatStruct _bonusStatStruct;

	[SyncVar]
	public StatStruct _baseStatStruct;

	[HideInInspector]
	public StatStruct _runtimeStatStruct;

	[Header("Class")]
	public ScriptablePlayerBaseClass _class;

	[SyncVar(hook = "OnSyncClassChange")]
	public string _syncClass = string.Empty;

	[SyncVar(hook = "OnSyncClassTierChange")]
	public int _syncClassTier;

	[Header("Points")]
	public int _currentSkillPoints;

	[SyncVar(hook = "OnAttributePointChange")]
	public int _currentAttributePoints;

	[Header("Runtime Stat Data")]
	public List<AttributeStruct> _playerAttributes = new List<AttributeStruct>();

	public List<ProfessionStruct> _playerProfessions = new List<ProfessionStruct>();

	public readonly List<ScriptableEquipment> _appliedEquipment = new List<ScriptableEquipment>();

	public readonly List<EquipCondition_Cooldown> _equipCondition_cooldowns = new List<EquipCondition_Cooldown>();

	public readonly SyncList<AttributeStruct> _syncAttributes = new SyncList<AttributeStruct>();

	public readonly SyncList<ProfessionStruct> _syncProfessions = new SyncList<ProfessionStruct>();

	private FloatingValue _cachedExpFloatValue;

	public int Network_currentLevel
	{
		get
		{
			return _currentLevel;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _currentLevel, 1uL, null);
		}
	}

	public int Network_currentExp
	{
		get
		{
			return _currentExp;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _currentExp, 2uL, null);
		}
	}

	public StatStruct Network_statStruct
	{
		get
		{
			return _statStruct;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _statStruct, 4uL, OnStatStructChange);
		}
	}

	public StatStruct Network_bonusStatStruct
	{
		get
		{
			return _bonusStatStruct;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _bonusStatStruct, 8uL, null);
		}
	}

	public StatStruct Network_baseStatStruct
	{
		get
		{
			return _baseStatStruct;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _baseStatStruct, 16uL, null);
		}
	}

	public string Network_syncClass
	{
		get
		{
			return _syncClass;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _syncClass, 32uL, OnSyncClassChange);
		}
	}

	public int Network_syncClassTier
	{
		get
		{
			return _syncClassTier;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _syncClassTier, 64uL, OnSyncClassTierChange);
		}
	}

	public int Network_currentAttributePoints
	{
		get
		{
			return _currentAttributePoints;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _currentAttributePoints, 128uL, OnAttributePointChange);
		}
	}

	private void OnAttributePointChange(int _old, int _new)
	{
		if (base.isLocalPlayer && TabMenu._current._statsCell._globalApplyingAttributePoints)
		{
			TabMenu._current._statsCell._phantomAtbPoints += _new - _old;
		}
	}

	private void OnSyncClassChange(string _old, string _new)
	{
		if (string.IsNullOrWhiteSpace(_new))
		{
			return;
		}
		ScriptablePlayerBaseClass scriptablePlayerBaseClass = GameManager._current.Locate_PlayerClass(_new);
		if ((bool)scriptablePlayerBaseClass)
		{
			_class = scriptablePlayerBaseClass;
			if (base.isLocalPlayer && !_player._bufferingStatus)
			{
				_chatBehaviour.Init_GameLogicMessage("<color=yellow>You are now a " + scriptablePlayerBaseClass._className + "!</color>");
				_pCasting.Init_SkillLibrary();
				_player.Steamworks_SetRichPresence();
			}
		}
	}

	private void OnSyncClassTierChange(int _old, int _new)
	{
		if (_new > 0 && (bool)_class && base.isLocalPlayer && !_player._bufferingStatus)
		{
			_chatBehaviour.Init_GameLogicMessage("<color=yellow>You've advanced to a " + _class._playerClassTiers[_new - 1]._classTierName + "!</color>");
			_pCasting.Init_SkillLibrary();
			_player.Steamworks_SetRichPresence();
		}
	}

	private void OnStatStructChange(StatStruct _old, StatStruct _new)
	{
		if (base.isLocalPlayer && TabMenu._current._statsCell._globalApplyingAttributePoints)
		{
			TabMenu._current._statsCell.Project_PhantomAttributeStats();
		}
	}

	private void Awake()
	{
		_player = GetComponent<Player>();
		_pVisual = _player._pVisual;
		_pSound = _player._pSound;
		_pCombat = _player._pCombat;
		_pCasting = _player._pCasting;
		_chatBehaviour = _player._chatBehaviour;
		_playerStatus = _player._statusEntity;
		_pEquipment = _player._pEquipment;
	}

	private void Start()
	{
		_syncAttributes.Callback += OnAttributesUpdated;
		_syncProfessions.Callback += OnProfessionsUpdated;
		for (int i = 0; i < _syncAttributes.Count; i++)
		{
			OnAttributesUpdated(SyncList<AttributeStruct>.Operation.OP_ADD, i, default(AttributeStruct), _syncAttributes[i]);
		}
		for (int j = 0; j < _syncProfessions.Count; j++)
		{
			OnProfessionsUpdated(SyncList<ProfessionStruct>.Operation.OP_ADD, j, default(ProfessionStruct), _syncProfessions[j]);
		}
	}

	public override void OnStartAuthority()
	{
		base.OnStartAuthority();
		_currentSkillPoints = ProfileDataManager._current._characterFile._statsProfile._skillPoints;
	}

	private void Update()
	{
		Client_HandleRuntimeStats();
		Server_HandleRuntimeStats();
	}

	[ClientCallback]
	private void Client_HandleRuntimeStats()
	{
		if (NetworkClient.active)
		{
			if (_commandBuffer > 0f)
			{
				_commandBuffer -= Time.deltaTime;
			}
			else
			{
				_commandBuffer = 0f;
			}
		}
	}

	[ServerCallback]
	private void Server_HandleRuntimeStats()
	{
		if (!NetworkServer.active)
		{
			return;
		}
		for (int i = 0; i < _equipCondition_cooldowns.Count; i++)
		{
			if (_equipCondition_cooldowns[i]._currentCooldown <= 0f)
			{
				_equipCondition_cooldowns.RemoveAt(i);
				break;
			}
			_equipCondition_cooldowns[i]._currentCooldown -= Time.deltaTime;
		}
		int attributePointsEachLevel = GameManager._current._statLogics._attributePointsEachLevel;
		int num = (_currentLevel - 1) * attributePointsEachLevel;
		num += 6;
		if (_currentLevel == 1)
		{
			num = 6;
		}
		if (_currentAttributePoints > num)
		{
			Network_currentAttributePoints = num;
		}
		else if (_currentAttributePoints < 0)
		{
			Network_currentAttributePoints = 0;
		}
		if (_currentExp < 0)
		{
			Network_currentExp = 0;
		}
		if (!_player._bufferingStatus && _currentLevel < GameManager._current._statLogics._maxMainLevel && _currentExp >= _statStruct._experience)
		{
			int experience = _statStruct._experience;
			Network_currentExp = _currentExp - experience;
			OnLevelUp();
		}
	}

	[ServerCallback]
	public void Server_ResetAttributePoints()
	{
		if (NetworkServer.active)
		{
			for (int i = 0; i < _syncAttributes.Count; i++)
			{
				AttributeStruct value = new AttributeStruct
				{
					_attributeName = _syncAttributes[i]._attributeName,
					_attributeID = _syncAttributes[i]._attributeID,
					_attributeValue = 1
				};
				_syncAttributes[i] = value;
			}
			int attributePointsEachLevel = GameManager._current._statLogics._attributePointsEachLevel;
			int num = (_currentLevel - 1) * attributePointsEachLevel;
			num += 6;
			if (_currentLevel == 1)
			{
				num = 6;
			}
			if (num > 0)
			{
				_chatBehaviour.Target_GameLogicMessage($"Obtained Attribute Points. (+{num})");
			}
			Network_currentAttributePoints = num;
		}
	}

	[ServerCallback]
	public void Server_ResetSkillPoints()
	{
		if (NetworkServer.active)
		{
			if (_playerStatus._syncConditions.Count > 0)
			{
				_playerStatus._syncConditions.Clear();
			}
			_pCombat.Network_currentSheathCondition = WeaponSheatheCondition.PUT_AWAY;
			Target_ResetSkillPoints();
		}
	}

	[TargetRpc]
	public void Target_ResetSkillPoints()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendTargetRPCInternal(null, "System.Void PlayerStats::Target_ResetSkillPoints()", -1159970310, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientCallback]
	public void Client_ResetSkillPoints()
	{
		if (!NetworkClient.active)
		{
			return;
		}
		int num = 0;
		int skillPointsEachLevel = GameManager._current._statLogics._skillPointsEachLevel;
		for (int i = 0; i < _currentLevel; i++)
		{
			if (i % 2 == 0)
			{
				num += skillPointsEachLevel;
			}
		}
		if (_currentLevel % 2 == 1)
		{
			num -= skillPointsEachLevel;
		}
		if (num > 0)
		{
			_chatBehaviour.Init_GameLogicMessage("Reset Skill Points.");
			_currentSkillPoints = num;
			_pCasting.Client_Lock_AllSkills();
			_pCasting.Clear_PassiveObjects();
			_pCasting.Client_Clear_ActionSlots();
			ProfileDataManager._current.Save_ProfileData();
		}
	}

	[ServerCallback]
	public void Init_AttributeListing()
	{
		if (NetworkServer.active)
		{
			for (int i = 0; i < GameManager._current._statLogics._statAttributes.Length; i++)
			{
				AttributeStruct item = new AttributeStruct
				{
					_attributeName = GameManager._current._statLogics._statAttributes[i]._attributeName,
					_attributeID = i,
					_attributeValue = 0
				};
				_syncAttributes.Add(item);
			}
		}
	}

	private void OnAttributesUpdated(SyncList<AttributeStruct>.Operation op, int _i, AttributeStruct _oldAttribute, AttributeStruct _newAttribute)
	{
		switch (op)
		{
		case SyncList<AttributeStruct>.Operation.OP_ADD:
			_playerAttributes.Add(_newAttribute);
			if (base.isLocalPlayer)
			{
				TabMenu._current._statsCell.Create_StatListData(_newAttribute);
			}
			break;
		case SyncList<AttributeStruct>.Operation.OP_SET:
			_playerAttributes[_i] = _newAttribute;
			Apply_StatCalculations();
			if (!_player._bufferingStatus && _oldAttribute._attributeValue < _newAttribute._attributeValue)
			{
				_pVisual._attributeEffects[_i].Play();
				_pVisual._attributeCircleEffect.Play();
				if (_pSound._aSrcGeneral.isPlaying)
				{
					_pSound._aSrcGeneral.Stop();
				}
				if (_pSound._aSrcGeneral.isActiveAndEnabled)
				{
					_pSound._aSrcGeneral.PlayOneShot(_pSound._attributeGlyph, 0.65f);
				}
			}
			break;
		case SyncList<AttributeStruct>.Operation.OP_CLEAR:
			_playerAttributes.Clear();
			break;
		case SyncList<AttributeStruct>.Operation.OP_INSERT:
		case SyncList<AttributeStruct>.Operation.OP_REMOVEAT:
			break;
		}
	}

	[Command]
	public void Cmd_ApplyAttributePoints(int[] _attributeValues, int[] _pointsSpent)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		GeneratedNetworkCode._Write_System_002EInt32_005B_005D(writer, _attributeValues);
		GeneratedNetworkCode._Write_System_002EInt32_005B_005D(writer, _pointsSpent);
		SendCommandInternal("System.Void PlayerStats::Cmd_ApplyAttributePoints(System.Int32[],System.Int32[])", 1373349653, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ServerCallback]
	public void Init_ProfessionListing()
	{
		if (NetworkServer.active)
		{
			for (int i = 0; i < GameManager._current._statLogics._scriptableProfessions.Length; i++)
			{
				ProfessionStruct item = new ProfessionStruct
				{
					_professionID = i,
					_professionLvl = 1,
					_professionXP = 0
				};
				_syncProfessions.Add(item);
			}
		}
	}

	[Command]
	public void Cmd_GainProfessionExp(ResourceEntity _resourceEntity, int _exp)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteNetworkBehaviour(_resourceEntity);
		writer.WriteInt(_exp);
		SendCommandInternal("System.Void PlayerStats::Cmd_GainProfessionExp(ResourceEntity,System.Int32)", 318039556, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void OnProfessionsUpdated(SyncList<ProfessionStruct>.Operation op, int i, ProfessionStruct _old, ProfessionStruct _new)
	{
		switch (op)
		{
		case SyncList<ProfessionStruct>.Operation.OP_ADD:
			_playerProfessions.Add(_new);
			if (base.isLocalPlayer)
			{
				TabMenu._current._statsCell.Create_ProfessionSlot(_new);
			}
			break;
		case SyncList<ProfessionStruct>.Operation.OP_SET:
			_playerProfessions[i] = _new;
			break;
		case SyncList<ProfessionStruct>.Operation.OP_CLEAR:
			_playerProfessions.Clear();
			break;
		case SyncList<ProfessionStruct>.Operation.OP_INSERT:
		case SyncList<ProfessionStruct>.Operation.OP_REMOVEAT:
			break;
		}
	}

	[ServerCallback]
	public void GainExp(int _expGain, int _passedLevel)
	{
		if (!NetworkServer.active)
		{
			return;
		}
		if (_currentLevel < GameManager._current._statLogics._maxMainLevel)
		{
			if (_passedLevel < _currentLevel)
			{
				Downscale_ExperienceGain();
			}
			else if (_passedLevel > _currentLevel)
			{
				Upscale_ExperienceGain();
			}
			if (_expGain > 0)
			{
				string message = $"Gained experience. (+{_expGain})";
				_chatBehaviour.Target_GameLogicMessage(message);
				Target_DisplayExpFloatText(_expGain);
				Network_currentExp = _currentExp + _expGain;
			}
		}
		void Downscale_ExperienceGain()
		{
			int num = _currentLevel - _passedLevel;
			float num2 = 1f;
			switch (num)
			{
			case 1:
				num2 = 0.98f;
				break;
			case 2:
				num2 = 0.65f;
				break;
			case 3:
				num2 = 0.48f;
				break;
			case 4:
				num2 = 0.25f;
				break;
			case 5:
				num2 = 0.12f;
				break;
			}
			int num3 = (int)((float)_expGain * num2);
			if (num >= 6)
			{
				num3 = 0;
			}
			_expGain = num3;
		}
		void Upscale_ExperienceGain()
		{
			int num = _passedLevel - _currentLevel;
			float num2 = GameManager._current._statLogics._experienceBonusModifier * (float)num;
			int num3 = (int)((float)_expGain * num2);
			_expGain += num3;
		}
	}

	[ServerCallback]
	public void OnLevelUp()
	{
		if (NetworkServer.active && _currentLevel != GameManager._current._statLogics._maxMainLevel)
		{
			Network_currentLevel = _currentLevel + 1;
			int attributePointsEachLevel = GameManager._current._statLogics._attributePointsEachLevel;
			Network_currentAttributePoints = _currentAttributePoints + attributePointsEachLevel;
			Apply_StatCalculations();
			_playerStatus.Replenish_All();
			HostConsole._current.Init_ServerMessage($"{_player._nickname} leveled up to {_currentLevel}.");
			_chatBehaviour.Target_GameLogicMessage($"<color=yellow>You are now level {_currentLevel}!</color>");
			_chatBehaviour.Target_GameLogicMessage($"Obtained Attribute Points. (+{attributePointsEachLevel})");
			Rpc_LevelUpEffect();
			Target_LocalLevelUpParams(_currentLevel);
			Apply_StatCalculations();
		}
	}

	[ClientRpc]
	private void Rpc_LevelUpEffect()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendRPCInternal("System.Void PlayerStats::Rpc_LevelUpEffect()", -412507747, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	private void Rpc_ProfessionLevelUpEffect()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendRPCInternal("System.Void PlayerStats::Rpc_ProfessionLevelUpEffect()", 515418625, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	private void Target_LocalLevelUpParams(int _projectedLevel)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteInt(_projectedLevel);
		SendTargetRPCInternal(null, "System.Void PlayerStats::Target_LocalLevelUpParams(System.Int32)", 1630782144, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private IEnumerator Init_LocalLevelUpParams(int _projectedLevel)
	{
		while (_projectedLevel != _currentLevel)
		{
			yield return null;
		}
		if (_currentLevel % 2 == 0)
		{
			int skillPointsEachLevel = GameManager._current._statLogics._skillPointsEachLevel;
			_currentSkillPoints += skillPointsEachLevel;
			if ((bool)_chatBehaviour)
			{
				_chatBehaviour.Init_GameLogicMessage($"Obtained Skill Points. (+{skillPointsEachLevel})");
			}
		}
		Init_TutorialMessages();
		ProfileDataManager._current.Save_ProfileData();
		_player.Steamworks_SetRichPresence();
		void Init_TutorialMessages()
		{
			string tutorialTag = "tut_levelUp";
			string message = "[" + InputControlManager.current.Convert_KeyCodeName(InputControlManager.current._tabMenu.ToString()) + "] Toggle Tab Menus\n[" + InputControlManager.current.Convert_KeyCodeName(InputControlManager.current._statMenuHotkey.ToString()) + "] Stat Menu Hotkey\n[" + InputControlManager.current.Convert_KeyCodeName(InputControlManager.current._skillMenuHotkey.ToString()) + "] Skill Menu Hotkey";
			if (InputControlManager.current._setGamepadType != GamepadType.Keyboard)
			{
				tutorialTag = "tut_levelUp_gpad";
				message = "[Start] Toggle Tab Menus\n[Left Shoulder Button] Previous Tab Menu\n[Right Shoulder Button] Next Tab Menu";
			}
			TutorialManager._current.Init_TutorialMessage(message, tutorialTag);
			if (_currentLevel >= 10)
			{
				string tutorialTag2 = "tut_levelUp10";
				string message2 = "You can now recieve a Class Tome from Angela, Sally, or Enok in Sanctum.";
				TutorialManager._current.Init_TutorialMessage(message2, tutorialTag2);
			}
		}
	}

	[TargetRpc]
	private void Target_DisplayExpFloatText(int _value)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteInt(_value);
		SendTargetRPCInternal(null, "System.Void PlayerStats::Target_DisplayExpFloatText(System.Int32)", -1909339316, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientCallback]
	public bool Check_RequestClass(string _classTag, bool _isButtonPrompt)
	{
		if (!NetworkClient.active)
		{
			return default;
		}
		if (_syncClass != string.Empty)
		{
			if (_isButtonPrompt)
			{
				ErrorPromptTextManager.current.Init_ErrorPrompt("Already have a Class Assigned");
			}
			return false;
		}
		if (_currentLevel < GameManager._current._statLogics._baseClassLevelRequirement)
		{
			if (_isButtonPrompt)
			{
				ErrorPromptTextManager.current.Init_ErrorPrompt($"Unable to upgrade to {_classTag}. Level Req: {GameManager._current._statLogics._baseClassLevelRequirement}");
			}
			return false;
		}
		if (_player._playerZoneType != ZoneType.Safe)
		{
			if (_isButtonPrompt)
			{
				ErrorPromptTextManager.current.Init_ErrorPrompt("Cannot assign Class here");
			}
			return false;
		}
		if (_isButtonPrompt)
		{
			switch (_classTag)
			{
			case "Fighter":
				SteamAchievementManager.current.Init_EarnAchievement("ATLYSS_ACHIEVEMENT_04");
				break;
			case "Mystic":
				SteamAchievementManager.current.Init_EarnAchievement("ATLYSS_ACHIEVEMENT_05");
				break;
			case "Bandit":
				SteamAchievementManager.current.Init_EarnAchievement("ATLYSS_ACHIEVEMENT_06");
				break;
			}
		}
		return true;
	}

	[Command]
	public void Cmd_RequestClass(string _classTag)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteString(_classTag);
		SendCommandInternal("System.Void PlayerStats::Cmd_RequestClass(System.String)", -1523801831, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientCallback]
	public bool Check_RequestTier(string _classTag, int _tierIndex, bool _isButtonPrompt)
	{
		if (!NetworkClient.active)
		{
			return default;
		}
		ScriptablePlayerBaseClass scriptablePlayerBaseClass = GameManager._current.Locate_PlayerClass(_classTag);
		if (_tierIndex <= 0)
		{
			return false;
		}
		if (!scriptablePlayerBaseClass)
		{
			return false;
		}
		if (_syncClassTier > 0)
		{
			if (_isButtonPrompt)
			{
				ErrorPromptTextManager.current.Init_ErrorPrompt("Already assigned Subclass");
			}
			return false;
		}
		if (_syncClass == string.Empty)
		{
			if (_isButtonPrompt)
			{
				ErrorPromptTextManager.current.Init_ErrorPrompt("Class Required: " + _classTag);
			}
			return false;
		}
		if (_classTag != _syncClass)
		{
			if (_isButtonPrompt)
			{
				ErrorPromptTextManager.current.Init_ErrorPrompt("Class Required - " + _classTag);
			}
			return false;
		}
		if (_currentLevel < GameManager._current._statLogics._tierClassLevelRequirement)
		{
			if (_isButtonPrompt)
			{
				ErrorPromptTextManager.current.Init_ErrorPrompt($"Level Required - {GameManager._current._statLogics._tierClassLevelRequirement}");
			}
			return false;
		}
		if (_player._playerZoneType != ZoneType.Safe)
		{
			if (_isButtonPrompt)
			{
				ErrorPromptTextManager.current.Init_ErrorPrompt("Cannot assign Class here");
			}
			return false;
		}
		if (_isButtonPrompt)
		{
			if (!(_classTag == "Fighter"))
			{
				if (_classTag == "Mystic")
				{
					switch (_tierIndex)
					{
					case 1:
						SteamAchievementManager.current.Init_EarnAchievement("ATLYSS_ACHIEVEMENT_18");
						break;
					case 2:
						SteamAchievementManager.current.Init_EarnAchievement("ATLYSS_ACHIEVEMENT_17");
						break;
					}
				}
			}
			else if (_tierIndex == 2)
			{
				SteamAchievementManager.current.Init_EarnAchievement("ATLYSS_ACHIEVEMENT_19");
			}
		}
		return true;
	}

	[Command]
	public void Cmd_RequestClassTier(int _classTier)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteInt(_classTier);
		SendCommandInternal("System.Void PlayerStats::Cmd_RequestClassTier(System.Int32)", 1145416412, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ServerCallback]
	public void Apply_StatCalculations()
	{
		if (NetworkServer.active)
		{
			StatLogics statLogics = GameManager._current._statLogics;
			_runtimeStatStruct = default;
			Network_baseStatStruct = default;
			if (_currentLevel <= 0)
			{
				Network_currentLevel = 1;
			}
			else if (_currentLevel == 1)
			{
				Network_currentAttributePoints = 0;
				_currentSkillPoints = 0;
			}
			for (int i = 0; i < statLogics._statAttributes.Length; i++)
			{
				statLogics._statAttributes[i].Apply_AttributeCalculation(this, _playerAttributes[i]._attributeValue, _runtimeStatStruct);
			}
			int experience = (int)GameManager._current._statLogics._experienceCurve.Evaluate(_currentLevel);
			_runtimeStatStruct._experience = experience;
			_runtimeStatStruct._evasion = statLogics._baseEvasion;
			Network_baseStatStruct = _runtimeStatStruct;
			Apply_EquipmentStats(_runtimeStatStruct);
			Apply_BonusStats(_runtimeStatStruct);
			Check_StatParams();
			Network_statStruct = _runtimeStatStruct;
		}
		void Check_StatParams()
		{
			if (_runtimeStatStruct._maxHealth < 1)
			{
				_runtimeStatStruct._maxHealth = 1;
			}
			if (_runtimeStatStruct._maxMana < 1)
			{
				_runtimeStatStruct._maxMana = 1;
			}
			if (_runtimeStatStruct._maxStamina < 1)
			{
				_runtimeStatStruct._maxStamina = 1;
			}
			if (_runtimeStatStruct._attackPower < 1)
			{
				_runtimeStatStruct._attackPower = 1;
			}
			if (_runtimeStatStruct._dexPower < 1)
			{
				_runtimeStatStruct._dexPower = 1;
			}
			if (_runtimeStatStruct._magicPower < 1)
			{
				_runtimeStatStruct._magicPower = 1;
			}
			if (_runtimeStatStruct._defense < 0)
			{
				_runtimeStatStruct._defense = 0;
			}
			if (_runtimeStatStruct._magicDefense < 0)
			{
				_runtimeStatStruct._magicDefense = 0;
			}
			if (_runtimeStatStruct._criticalRate < 0f)
			{
				_runtimeStatStruct._criticalRate = 0f;
			}
			if (_runtimeStatStruct._magicCriticalRate < 0f)
			{
				_runtimeStatStruct._magicCriticalRate = 0f;
			}
			if (_runtimeStatStruct._evasion < 0f)
			{
				_runtimeStatStruct._evasion = 0f;
			}
		}
	}

	public void Apply_BonusStats(StatStruct _struct)
	{
		_struct._maxHealth += _bonusStatStruct._maxHealth;
		_struct._maxMana += _bonusStatStruct._maxMana;
		_struct._maxStamina += _bonusStatStruct._maxStamina;
		_struct._attackPower += _bonusStatStruct._attackPower;
		_struct._dexPower += _bonusStatStruct._dexPower;
		_struct._magicPower += _bonusStatStruct._magicPower;
		_struct._defense += _bonusStatStruct._defense;
		_struct._magicDefense += _bonusStatStruct._magicDefense;
		_struct._criticalRate += _bonusStatStruct._criticalRate;
		_struct._magicCriticalRate += _bonusStatStruct._magicCriticalRate;
		_struct._evasion += _bonusStatStruct._evasion;
		_struct._fireResist += _bonusStatStruct._fireResist;
		_struct._waterResist += _bonusStatStruct._waterResist;
		_struct._natureResist += _bonusStatStruct._natureResist;
		_struct._earthResist += _bonusStatStruct._earthResist;
		_struct._holyResist += _bonusStatStruct._holyResist;
		_struct._shadowResist += _bonusStatStruct._shadowResist;
		if (base.isServer)
		{
			_runtimeStatStruct = _struct;
		}
		if (base.isLocalPlayer)
		{
			TabMenu._current._statsCell._projectionStatStruct = _struct;
		}
	}

	public void Apply_EquipmentStats(StatStruct _struct)
	{
		if (!_pCombat._lockWeapon)
		{
			if (_pCombat._isUsingAltWeapon)
			{
				if ((bool)_pEquipment._equips["Alt Weapon"]._scriptableEquip)
				{
					Apply_EquipStat(_pEquipment._equips["Alt Weapon"]._heldItem, _pEquipment._equips["Alt Weapon"]._scriptableEquip);
					_pCombat.Network_useDamageTypeOverride = _pEquipment._equips["Alt Weapon"]._heldItem._useDamageTypeOverride;
					_pCombat.Network_damageTypeOverride = _pEquipment._equips["Alt Weapon"]._heldItem._damageTypeOverride;
				}
			}
			else if ((bool)_pEquipment._equips["Weapon"]._scriptableEquip)
			{
				Apply_EquipStat(_pEquipment._equips["Weapon"]._heldItem, _pEquipment._equips["Weapon"]._scriptableEquip);
				_pCombat.Network_useDamageTypeOverride = _pEquipment._equips["Weapon"]._heldItem._useDamageTypeOverride;
				_pCombat.Network_damageTypeOverride = _pEquipment._equips["Weapon"]._heldItem._damageTypeOverride;
			}
		}
		else if ((bool)_pEquipment._equips["Weapon"]._scriptableEquip)
		{
			Apply_EquipStat(_pEquipment._equips["Weapon"]._heldItem, _pEquipment._equips["Weapon"]._scriptableEquip);
			_pCombat.Network_useDamageTypeOverride = _pEquipment._equips["Weapon"]._heldItem._useDamageTypeOverride;
			_pCombat.Network_damageTypeOverride = _pEquipment._equips["Weapon"]._heldItem._damageTypeOverride;
		}
		for (int i = 0; i < _appliedEquipment.Count; i++)
		{
			if (_player._pInventory.Fetch_HeldItem(_appliedEquipment[i]) != null)
			{
				if (!_player._pInventory.Fetch_HeldItem(_appliedEquipment[i])._isEquipped)
				{
					_appliedEquipment.RemoveAt(i);
				}
			}
			else
			{
				_appliedEquipment.RemoveAt(i);
			}
		}
		if ((bool)_pEquipment._equips["Helm"]._scriptableEquip)
		{
			Apply_EquipStat(_pEquipment._equips["Helm"]._heldItem, _pEquipment._equips["Helm"]._scriptableEquip);
		}
		if ((bool)_pEquipment._equips["Chest"]._scriptableEquip)
		{
			Apply_EquipStat(_pEquipment._equips["Chest"]._heldItem, _pEquipment._equips["Chest"]._scriptableEquip);
		}
		if ((bool)_pEquipment._equips["Leggings"]._scriptableEquip)
		{
			Apply_EquipStat(_pEquipment._equips["Leggings"]._heldItem, _pEquipment._equips["Leggings"]._scriptableEquip);
		}
		if ((bool)_pEquipment._equips["Cape"]._scriptableEquip)
		{
			Apply_EquipStat(_pEquipment._equips["Cape"]._heldItem, _pEquipment._equips["Cape"]._scriptableEquip);
		}
		if ((bool)_pEquipment._equips["Ring"]._scriptableEquip)
		{
			Apply_EquipStat(_pEquipment._equips["Ring"]._heldItem, _pEquipment._equips["Ring"]._scriptableEquip);
		}
		if ((bool)_pEquipment._equips["Shield"]._scriptableEquip)
		{
			ScriptableWeapon scriptableWeapon = null;
			if (_pCombat._lockWeapon)
			{
				if ((bool)_pEquipment._equips["Weapon"]._scriptableEquip)
				{
					scriptableWeapon = (ScriptableWeapon)_pEquipment._equips["Weapon"]._scriptableEquip;
				}
			}
			else if (_pCombat._isUsingAltWeapon)
			{
				if ((bool)_pEquipment._equips["Alt Weapon"]._scriptableEquip)
				{
					scriptableWeapon = (ScriptableWeapon)_pEquipment._equips["Alt Weapon"]._scriptableEquip;
				}
			}
			else if ((bool)_pEquipment._equips["Weapon"]._scriptableEquip)
			{
				scriptableWeapon = (ScriptableWeapon)_pEquipment._equips["Weapon"]._scriptableEquip;
			}
			if (!scriptableWeapon || !scriptableWeapon.weaponType || scriptableWeapon.weaponType._weaponHandedness == WeaponHandedness.One_Handed)
			{
				Apply_EquipStat(_pEquipment._equips["Shield"]._heldItem, _pEquipment._equips["Shield"]._scriptableEquip);
			}
		}
		if (base.isServer)
		{
			_runtimeStatStruct = _struct;
		}
		if (base.isLocalPlayer)
		{
			TabMenu._current._statsCell._projectionStatStruct = _struct;
		}
		void Apply_EquipStat(ItemData _Idata, ScriptableEquipment _scriptEquip)
		{
			if (_scriptEquip.CanEquipItem(this, _isClickPrompt: false))
			{
				StatStruct statArray = _scriptEquip._statArray;
				if (_Idata._modifierID > 0 && (int)_scriptEquip._itemRarity <= 1)
				{
					ScriptableStatModifier scriptableStatModifier = GameManager._current.Locate_StatModifier(_Idata._modifierID);
					bool flag = false;
					if (!_scriptEquip._statModifierTable)
					{
						return;
					}
					for (int j = 0; j < _scriptEquip._statModifierTable._statModifierSlots.Length; j++)
					{
						if (_scriptEquip._statModifierTable._statModifierSlots[j]._equipModifier == scriptableStatModifier)
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						return;
					}
					int num = (int)((float)scriptableStatModifier._modifierStatStruct._maxHealth * ((float)_scriptEquip._equipmentLevel * scriptableStatModifier._modifierPercentage));
					if (num <= 0 && scriptableStatModifier._modifierStatStruct._maxHealth > 0)
					{
						num++;
					}
					int num2 = (int)((float)scriptableStatModifier._modifierStatStruct._maxMana * ((float)_scriptEquip._equipmentLevel * scriptableStatModifier._modifierPercentage));
					if (num2 <= 0 && scriptableStatModifier._modifierStatStruct._maxMana > 0)
					{
						num2++;
					}
					int num3 = (int)((float)scriptableStatModifier._modifierStatStruct._maxStamina * ((float)_scriptEquip._equipmentLevel * scriptableStatModifier._modifierPercentage));
					if (num3 <= 0 && scriptableStatModifier._modifierStatStruct._maxStamina > 0)
					{
						num3++;
					}
					int num4 = (int)((float)scriptableStatModifier._modifierStatStruct._attackPower * ((float)_scriptEquip._equipmentLevel * scriptableStatModifier._modifierPercentage));
					if (num4 <= 0 && scriptableStatModifier._modifierStatStruct._attackPower > 0)
					{
						num4++;
					}
					int num5 = (int)((float)scriptableStatModifier._modifierStatStruct._dexPower * ((float)_scriptEquip._equipmentLevel * scriptableStatModifier._modifierPercentage));
					if (num5 <= 0 && scriptableStatModifier._modifierStatStruct._dexPower > 0)
					{
						num5++;
					}
					int num6 = (int)((float)scriptableStatModifier._modifierStatStruct._magicPower * ((float)_scriptEquip._equipmentLevel * scriptableStatModifier._modifierPercentage));
					if (num6 <= 0 && scriptableStatModifier._modifierStatStruct._magicPower > 0)
					{
						num6++;
					}
					int num7 = (int)((float)scriptableStatModifier._modifierStatStruct._defense * ((float)_scriptEquip._equipmentLevel * scriptableStatModifier._modifierPercentage));
					if (num7 <= 0 && scriptableStatModifier._modifierStatStruct._defense > 0)
					{
						num7++;
					}
					int num8 = (int)((float)scriptableStatModifier._modifierStatStruct._magicDefense * ((float)_scriptEquip._equipmentLevel * scriptableStatModifier._modifierPercentage));
					if (num8 <= 0 && scriptableStatModifier._modifierStatStruct._magicDefense > 0)
					{
						num8++;
					}
					int num9 = (int)((float)scriptableStatModifier._modifierStatStruct._fireResist * ((float)_scriptEquip._equipmentLevel * scriptableStatModifier._modifierPercentage));
					if (num9 <= 0 && scriptableStatModifier._modifierStatStruct._fireResist > 0)
					{
						num9++;
					}
					int num10 = (int)((float)scriptableStatModifier._modifierStatStruct._waterResist * ((float)_scriptEquip._equipmentLevel * scriptableStatModifier._modifierPercentage));
					if (num10 <= 0 && scriptableStatModifier._modifierStatStruct._waterResist > 0)
					{
						num10++;
					}
					int num11 = (int)((float)scriptableStatModifier._modifierStatStruct._natureResist * ((float)_scriptEquip._equipmentLevel * scriptableStatModifier._modifierPercentage));
					if (num11 <= 0 && scriptableStatModifier._modifierStatStruct._natureResist > 0)
					{
						num11++;
					}
					int num12 = (int)((float)scriptableStatModifier._modifierStatStruct._earthResist * ((float)_scriptEquip._equipmentLevel * scriptableStatModifier._modifierPercentage));
					if (num12 <= 0 && scriptableStatModifier._modifierStatStruct._earthResist > 0)
					{
						num12++;
					}
					int num13 = (int)((float)scriptableStatModifier._modifierStatStruct._holyResist * ((float)_scriptEquip._equipmentLevel * scriptableStatModifier._modifierPercentage));
					if (num13 <= 0 && scriptableStatModifier._modifierStatStruct._holyResist > 0)
					{
						num13++;
					}
					int num14 = (int)((float)scriptableStatModifier._modifierStatStruct._shadowResist * ((float)_scriptEquip._equipmentLevel * scriptableStatModifier._modifierPercentage));
					if (num14 <= 0 && scriptableStatModifier._modifierStatStruct._shadowResist > 0)
					{
						num14++;
					}
					statArray._maxHealth += num;
					statArray._maxMana += num2;
					statArray._maxStamina += num3;
					statArray._attackPower += num4;
					statArray._dexPower += num5;
					statArray._magicPower += num6;
					statArray._defense += num7;
					statArray._magicDefense += num8;
					statArray._criticalRate += scriptableStatModifier._modifierStatStruct._criticalRate * ((float)_scriptEquip._equipmentLevel * scriptableStatModifier._modifierPercentage);
					statArray._magicCriticalRate += scriptableStatModifier._modifierStatStruct._magicCriticalRate * ((float)_scriptEquip._equipmentLevel * scriptableStatModifier._modifierPercentage);
					statArray._evasion += scriptableStatModifier._modifierStatStruct._evasion * ((float)_scriptEquip._equipmentLevel * scriptableStatModifier._modifierPercentage);
					statArray._fireResist += num9;
					statArray._waterResist += num10;
					statArray._natureResist += num11;
					statArray._earthResist += num12;
					statArray._holyResist += num13;
					statArray._shadowResist += num14;
				}
				_struct._maxHealth += statArray._maxHealth;
				_struct._maxMana += statArray._maxMana;
				_struct._maxStamina += statArray._maxStamina;
				_struct._attackPower += statArray._attackPower;
				_struct._dexPower += statArray._dexPower;
				_struct._magicPower += statArray._magicPower;
				_struct._defense += statArray._defense;
				_struct._magicDefense += statArray._magicDefense;
				_struct._criticalRate += statArray._criticalRate;
				_struct._magicCriticalRate += statArray._magicCriticalRate;
				_struct._evasion += statArray._evasion;
				_struct._fireResist += statArray._fireResist;
				_struct._waterResist += statArray._waterResist;
				_struct._natureResist += statArray._natureResist;
				_struct._earthResist += statArray._earthResist;
				_struct._holyResist += statArray._holyResist;
				_struct._shadowResist += statArray._shadowResist;
				if (!_appliedEquipment.Contains(_scriptEquip))
				{
					_appliedEquipment.Add(_scriptEquip);
				}
			}
		}
	}

	[ServerCallback]
	public void Iterate_EquipConditions(EquipConditionCallback _callback, ScriptableSkill _dedicatedSkill)
	{
		if (!NetworkServer.active)
		{
			return;
		}
		foreach (ScriptableEquipment item2 in _appliedEquipment)
		{
			bool flag = false;
			foreach (EquipCondition_Cooldown equipCondition_cooldown in _equipCondition_cooldowns)
			{
				if (equipCondition_cooldown._setActivation == item2._equipConditionActivation)
				{
					flag = true;
					break;
				}
			}
			if (!flag && item2._equipConditionActivation != null && item2._equipConditionActivation._equipConditionSlot != null && (bool)item2._equipConditionActivation._equipConditionSlot._scriptableCondition && item2.IsConditionMet(_player._statusEntity, _callback, _dedicatedSkill))
			{
				StatusEntity statusEntity = _player._statusEntity;
				int conditionDuration = item2._equipConditionActivation._equipConditionSlot._conditionDuration;
				int conditionPower = item2._equipConditionActivation._equipConditionSlot._conditionPower;
				float powerPercent = item2._equipConditionActivation._equipConditionSlot._powerPercent;
				float conditionRepeatRate = item2._equipConditionActivation._equipConditionSlot._conditionRepeatRate;
				if (!(Random.Range(0f, 1f) > item2._equipConditionActivation._equipConditionSlot._chance))
				{
					item2._equipConditionActivation._equipConditionSlot._scriptableCondition.Apply_ConditionToEntity(statusEntity, statusEntity, DamageType.Pure, conditionPower, conditionDuration, powerPercent, conditionRepeatRate);
					EquipCondition_Cooldown item = new EquipCondition_Cooldown
					{
						_setActivation = item2._equipConditionActivation,
						_currentCooldown = item2._equipConditionActivation._conditionCooldown
					};
					_equipCondition_cooldowns.Add(item);
				}
			}
		}
	}

	public float Retrieve_SkillBonusPower(ScriptableSkill _scriptSkill)
	{
		float num = 0f;
		foreach (ScriptableEquipment item in _appliedEquipment)
		{
			if (item._equipSkillBonuses == null || item._equipSkillBonuses.Length == 0)
			{
				continue;
			}
			SkillBonus[] equipSkillBonuses = item._equipSkillBonuses;
			foreach (SkillBonus skillBonus in equipSkillBonuses)
			{
				if (!(skillBonus._scriptableSkill != _scriptSkill))
				{
					num += skillBonus._skillPowerBonus;
				}
			}
		}
		return num;
	}

	public float Retrieve_SkillConditionBonusPower(ScriptableSkill _scriptSkill)
	{
		float num = 0f;
		foreach (ScriptableEquipment item in _appliedEquipment)
		{
			if (item._equipSkillBonuses == null || item._equipSkillBonuses.Length == 0)
			{
				continue;
			}
			SkillBonus[] equipSkillBonuses = item._equipSkillBonuses;
			foreach (SkillBonus skillBonus in equipSkillBonuses)
			{
				if (!(skillBonus._scriptableSkill != _scriptSkill))
				{
					num += skillBonus._skillConditionPowerBonus;
				}
			}
		}
		return num;
	}

	public int Retrieve_SkillConditionBonusDuration(ScriptableSkill _scriptSkill)
	{
		int num = 0;
		foreach (ScriptableEquipment item in _appliedEquipment)
		{
			if (item._equipSkillBonuses == null || item._equipSkillBonuses.Length == 0)
			{
				continue;
			}
			SkillBonus[] equipSkillBonuses = item._equipSkillBonuses;
			foreach (SkillBonus skillBonus in equipSkillBonuses)
			{
				if (!(skillBonus._scriptableSkill != _scriptSkill))
				{
					num += skillBonus._skillConditionDurationBonus;
				}
			}
		}
		return num;
	}

	public float Retrieve_WeaponTypeBonusPower()
	{
		float num = 0f;
		foreach (ScriptableEquipment item in _appliedEquipment)
		{
			if (item._equipWeaponTypeBonuses == null || item._equipWeaponTypeBonuses.Length == 0)
			{
				continue;
			}
			WeaponTypeBonus[] equipWeaponTypeBonuses = item._equipWeaponTypeBonuses;
			foreach (WeaponTypeBonus weaponTypeBonus in equipWeaponTypeBonuses)
			{
				if (!(weaponTypeBonus._scriptableWeaponType != _pCombat._currentScriptableWeaponType))
				{
					num += weaponTypeBonus._weaponTypeBonus;
				}
			}
		}
		return num;
	}

	public PlayerStats()
	{
		InitSyncObject(_syncAttributes);
		InitSyncObject(_syncProfessions);
	}

	public override bool Weaved()
	{
		return true;
	}

	protected void UserCode_Target_ResetSkillPoints()
	{
		Client_ResetSkillPoints();
	}

	protected static void InvokeUserCode_Target_ResetSkillPoints(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC Target_ResetSkillPoints called on server.");
		}
		else
		{
			((PlayerStats)obj).UserCode_Target_ResetSkillPoints();
		}
	}

	protected void UserCode_Cmd_ApplyAttributePoints__Int32_005B_005D__Int32_005B_005D(int[] _attributeValues, int[] _pointsSpent)
	{
		if (_currentAttributePoints <= 0)
		{
			return;
		}
		for (int i = 0; i < _attributeValues.Length; i++)
		{
			if (_currentAttributePoints <= 0)
			{
				break;
			}
			ScriptableStatAttribute scriptableStatAttribute = GameManager._current._statLogics._statAttributes[i];
			AttributeStruct value = new AttributeStruct
			{
				_attributeName = scriptableStatAttribute._attributeName,
				_attributeID = i,
				_attributeValue = _attributeValues[i]
			};
			_syncAttributes[i] = value;
			Network_currentAttributePoints = _currentAttributePoints - _pointsSpent[i];
		}
	}

	protected static void InvokeUserCode_Cmd_ApplyAttributePoints__Int32_005B_005D__Int32_005B_005D(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command Cmd_ApplyAttributePoints called on client.");
		}
		else
		{
			((PlayerStats)obj).UserCode_Cmd_ApplyAttributePoints__Int32_005B_005D__Int32_005B_005D(GeneratedNetworkCode._Read_System_002EInt32_005B_005D(reader), GeneratedNetworkCode._Read_System_002EInt32_005B_005D(reader));
		}
	}

	protected void UserCode_Cmd_GainProfessionExp__ResourceEntity__Int32(ResourceEntity _resourceEntity, int _exp)
	{
		if (!_resourceEntity)
		{
			return;
		}
		if (_resourceEntity._currentResourceCount > 0)
		{
			_resourceEntity._currentResourceCount--;
		}
		ScriptableProfession scriptableProfession = null;
		int i = 0;
		for (int j = 0; j < GameManager._current._statLogics._scriptableProfessions.Length; j++)
		{
			if (GameManager._current._statLogics._scriptableProfessions[j] == _resourceEntity._scriptableProfession)
			{
				i = j;
				scriptableProfession = GameManager._current._statLogics._scriptableProfessions[j];
				break;
			}
		}
		if (_resourceEntity._resourceEntityLevel > _syncProfessions[i]._professionLvl || _syncProfessions[i]._professionLvl >= scriptableProfession._maxProfessionLevel)
		{
			return;
		}
		int num = _syncProfessions[i]._professionLvl - _resourceEntity._resourceEntityLevel;
		float num2 = 1f;
		if (num == 1)
		{
			num2 = 0.9f;
		}
		else if (num == 2)
		{
			num2 = 0.6f;
		}
		else if (num == 3)
		{
			num2 = 0.4f;
		}
		else if (num == 4)
		{
			num2 = 0.2f;
		}
		else if (num >= 5)
		{
			num2 = 0.1f;
		}
		int num3 = (int)((float)_exp * num2);
		if (num3 > 0)
		{
			int num4 = _syncProfessions[i]._professionLvl;
			int num5 = _syncProfessions[i]._professionXP + num3;
			if (num5 >= (int)scriptableProfession._professionCurve.Evaluate(num4))
			{
				num5 -= (int)scriptableProfession._professionCurve.Evaluate(num4);
				num4++;
				HostConsole._current.Init_ServerMessage($"{_player._nickname} leveled {scriptableProfession._professionName} to {num4}.");
				_chatBehaviour.Target_GameLogicMessage($"<color=yellow>You leveled {scriptableProfession._professionName} to {num4}!</color>");
				Rpc_ProfessionLevelUpEffect();
			}
			ProfessionStruct value = new ProfessionStruct
			{
				_professionID = _syncProfessions[i]._professionID,
				_professionLvl = num4,
				_professionXP = num5
			};
			_syncProfessions[i] = value;
			string message = $"Gained {scriptableProfession._professionName} experience. (+{num3})";
			_chatBehaviour.Target_GameLogicMessage(message);
			Target_DisplayExpFloatText(num3);
		}
	}

	protected static void InvokeUserCode_Cmd_GainProfessionExp__ResourceEntity__Int32(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command Cmd_GainProfessionExp called on client.");
		}
		else
		{
			((PlayerStats)obj).UserCode_Cmd_GainProfessionExp__ResourceEntity__Int32(reader.ReadNetworkBehaviour<ResourceEntity>(), reader.ReadInt());
		}
	}

	protected void UserCode_Rpc_LevelUpEffect()
	{
		if ((bool)Player._mainPlayer && !_player._bufferingStatus && !(_player.Network_playerMapInstance != Player._mainPlayer.Network_playerMapInstance))
		{
			GameObject obj = PoolManager._current.GenerateObject(null, "levelUpEffect", 3.25f);
			obj.transform.SetParent(base.transform);
			obj.transform.localPosition = Vector3.zero;
			obj.GetComponentInChildren<LerpScale>().gameObject.transform.localScale = new Vector3(0f, 1f, 1f);
		}
	}

	protected static void InvokeUserCode_Rpc_LevelUpEffect(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC Rpc_LevelUpEffect called on server.");
		}
		else
		{
			((PlayerStats)obj).UserCode_Rpc_LevelUpEffect();
		}
	}

	protected void UserCode_Rpc_ProfessionLevelUpEffect()
	{
		if ((bool)Player._mainPlayer && !_player._bufferingStatus && !(_player.Network_playerMapInstance != Player._mainPlayer.Network_playerMapInstance))
		{
			GameObject obj = PoolManager._current.GenerateObject(null, "professionLevelUpEffect", 3.25f);
			obj.transform.SetParent(base.transform);
			obj.transform.localPosition = Vector3.zero;
		}
	}

	protected static void InvokeUserCode_Rpc_ProfessionLevelUpEffect(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC Rpc_ProfessionLevelUpEffect called on server.");
		}
		else
		{
			((PlayerStats)obj).UserCode_Rpc_ProfessionLevelUpEffect();
		}
	}

	protected void UserCode_Target_LocalLevelUpParams__Int32(int _projectedLevel)
	{
		if (base.isLocalPlayer)
		{
			StartCoroutine(Init_LocalLevelUpParams(_projectedLevel));
		}
	}

	protected static void InvokeUserCode_Target_LocalLevelUpParams__Int32(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC Target_LocalLevelUpParams called on server.");
		}
		else
		{
			((PlayerStats)obj).UserCode_Target_LocalLevelUpParams__Int32(reader.ReadInt());
		}
	}

	protected void UserCode_Target_DisplayExpFloatText__Int32(int _value)
	{
		if ((bool)_cachedExpFloatValue)
		{
			_cachedExpFloatValue._displayTimer = 0.1f;
		}
		FloatingValue floatingValue = (_cachedExpFloatValue = GameManager._current.Spawn_FloatingText(_value, _player._statusEntity, FloatTextColor.EXPERIENCE, 2.98f, 0f));
		floatingValue._returnToPoolTime = 2.45f;
		floatingValue._displayTimer = 0.95f;
	}

	protected static void InvokeUserCode_Target_DisplayExpFloatText__Int32(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC Target_DisplayExpFloatText called on server.");
		}
		else
		{
			((PlayerStats)obj).UserCode_Target_DisplayExpFloatText__Int32(reader.ReadInt());
		}
	}

	protected void UserCode_Cmd_RequestClass__String(string _classTag)
	{
		if (_currentLevel >= GameManager._current._statLogics._baseClassLevelRequirement && (bool)GameManager._current.Locate_PlayerClass(_classTag))
		{
			HostConsole._current.Init_ServerMessage(_player._nickname + " became a " + _classTag + "!");
			Network_syncClass = _classTag;
			Server_ResetSkillPoints();
		}
	}

	protected static void InvokeUserCode_Cmd_RequestClass__String(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command Cmd_RequestClass called on client.");
		}
		else
		{
			((PlayerStats)obj).UserCode_Cmd_RequestClass__String(reader.ReadString());
		}
	}

	protected void UserCode_Cmd_RequestClassTier__Int32(int _classTier)
	{
		if (_currentLevel >= GameManager._current._statLogics._tierClassLevelRequirement && !string.IsNullOrEmpty(_syncClass))
		{
			HostConsole._current.Init_ServerMessage(_player._nickname + " advanced to a " + _class._playerClassTiers[_classTier - 1]._classTierName + "!");
			Network_syncClassTier = _classTier;
			Server_ResetSkillPoints();
		}
	}

	protected static void InvokeUserCode_Cmd_RequestClassTier__Int32(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command Cmd_RequestClassTier called on client.");
		}
		else
		{
			((PlayerStats)obj).UserCode_Cmd_RequestClassTier__Int32(reader.ReadInt());
		}
	}

	static PlayerStats()
	{
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerStats), "System.Void PlayerStats::Cmd_ApplyAttributePoints(System.Int32[],System.Int32[])", InvokeUserCode_Cmd_ApplyAttributePoints__Int32_005B_005D__Int32_005B_005D, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerStats), "System.Void PlayerStats::Cmd_GainProfessionExp(ResourceEntity,System.Int32)", InvokeUserCode_Cmd_GainProfessionExp__ResourceEntity__Int32, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerStats), "System.Void PlayerStats::Cmd_RequestClass(System.String)", InvokeUserCode_Cmd_RequestClass__String, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerStats), "System.Void PlayerStats::Cmd_RequestClassTier(System.Int32)", InvokeUserCode_Cmd_RequestClassTier__Int32, requiresAuthority: true);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerStats), "System.Void PlayerStats::Rpc_LevelUpEffect()", InvokeUserCode_Rpc_LevelUpEffect);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerStats), "System.Void PlayerStats::Rpc_ProfessionLevelUpEffect()", InvokeUserCode_Rpc_ProfessionLevelUpEffect);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerStats), "System.Void PlayerStats::Target_ResetSkillPoints()", InvokeUserCode_Target_ResetSkillPoints);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerStats), "System.Void PlayerStats::Target_LocalLevelUpParams(System.Int32)", InvokeUserCode_Target_LocalLevelUpParams__Int32);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerStats), "System.Void PlayerStats::Target_DisplayExpFloatText(System.Int32)", InvokeUserCode_Target_DisplayExpFloatText__Int32);
	}

	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteInt(_currentLevel);
			writer.WriteInt(_currentExp);
			GeneratedNetworkCode._Write_StatStruct(writer, _statStruct);
			GeneratedNetworkCode._Write_StatStruct(writer, _bonusStatStruct);
			GeneratedNetworkCode._Write_StatStruct(writer, _baseStatStruct);
			writer.WriteString(_syncClass);
			writer.WriteInt(_syncClassTier);
			writer.WriteInt(_currentAttributePoints);
			return;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteInt(_currentLevel);
		}
		if ((base.syncVarDirtyBits & 2L) != 0L)
		{
			writer.WriteInt(_currentExp);
		}
		if ((base.syncVarDirtyBits & 4L) != 0L)
		{
			GeneratedNetworkCode._Write_StatStruct(writer, _statStruct);
		}
		if ((base.syncVarDirtyBits & 8L) != 0L)
		{
			GeneratedNetworkCode._Write_StatStruct(writer, _bonusStatStruct);
		}
		if ((base.syncVarDirtyBits & 0x10L) != 0L)
		{
			GeneratedNetworkCode._Write_StatStruct(writer, _baseStatStruct);
		}
		if ((base.syncVarDirtyBits & 0x20L) != 0L)
		{
			writer.WriteString(_syncClass);
		}
		if ((base.syncVarDirtyBits & 0x40L) != 0L)
		{
			writer.WriteInt(_syncClassTier);
		}
		if ((base.syncVarDirtyBits & 0x80L) != 0L)
		{
			writer.WriteInt(_currentAttributePoints);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize(ref _currentLevel, null, reader.ReadInt());
			GeneratedSyncVarDeserialize(ref _currentExp, null, reader.ReadInt());
			GeneratedSyncVarDeserialize(ref _statStruct, OnStatStructChange, GeneratedNetworkCode._Read_StatStruct(reader));
			GeneratedSyncVarDeserialize(ref _bonusStatStruct, null, GeneratedNetworkCode._Read_StatStruct(reader));
			GeneratedSyncVarDeserialize(ref _baseStatStruct, null, GeneratedNetworkCode._Read_StatStruct(reader));
			GeneratedSyncVarDeserialize(ref _syncClass, OnSyncClassChange, reader.ReadString());
			GeneratedSyncVarDeserialize(ref _syncClassTier, OnSyncClassTierChange, reader.ReadInt());
			GeneratedSyncVarDeserialize(ref _currentAttributePoints, OnAttributePointChange, reader.ReadInt());
			return;
		}
		long num = (long)reader.ReadULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _currentLevel, null, reader.ReadInt());
		}
		if ((num & 2L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _currentExp, null, reader.ReadInt());
		}
		if ((num & 4L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _statStruct, OnStatStructChange, GeneratedNetworkCode._Read_StatStruct(reader));
		}
		if ((num & 8L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _bonusStatStruct, null, GeneratedNetworkCode._Read_StatStruct(reader));
		}
		if ((num & 0x10L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _baseStatStruct, null, GeneratedNetworkCode._Read_StatStruct(reader));
		}
		if ((num & 0x20L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _syncClass, OnSyncClassChange, reader.ReadString());
		}
		if ((num & 0x40L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _syncClassTier, OnSyncClassTierChange, reader.ReadInt());
		}
		if ((num & 0x80L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _currentAttributePoints, OnAttributePointChange, reader.ReadInt());
		}
	}
}
