using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Mirror;
using Mirror.RemoteCalls;
using UnityEngine;

public class PlayerCasting : NetworkBehaviour
{
	private Player _player;

	private PlayerMove _pMove;

	private PlayerCombat _pCombat;

	private PlayerInventory _pInventory;

	private PlayerVisual _pVisual;

	private PlayerStats _pStats;

	private StatusEntity _pStatus;

	public List<SkillLibrarySlot> _skillLibrary = new List<SkillLibrarySlot>();

	public List<SkillStruct> _playerSkillStructs = new List<SkillStruct>();

	public BonusSkillSlot[] _bonusSkillSlots;

	[Header("Skill Loadout")]
	public SkillLoadOutSlot[] _skillLoadoutSlots = new SkillLoadOutSlot[6];

	[Space]
	public SkillPresetProfile _skillPresetProfile;

	public string[] _setSkillPresetIndex;

	[Header("Player Caster Runtime")]
	public ScriptableSkill _currentCastSkill;

	public ProjectileObject _cachedProjectileObj;

	[SyncVar]
	public StatusEntity _revivingEntity;

	[Space]
	public float _castTimer;

	public float _castTimerMod;

	private float _setCastTime;

	[Space]
	public float _lockControlBuffer;

	public float _castEndBuffer;

	private float _commandBuffer;

	[Space]
	public bool _isQueuedSkill;

	public bool _isCasting;

	public bool _ignoreCost;

	private bool _earlySkillObjectEmitted;

	private bool _initCastInterruptInput;

	private GamepadButton _cachedGamepadButton;

	private KeyCode _cachedInput;

	[Header("Runtime Passive Objects")]
	public List<SkillObjectBehavior> _passiveSkillObjects = new List<SkillObjectBehavior>();

	private bool _locatingPassives;

	[Header("Casting Effect")]
	public PlayerCastEffect _playerCastEffect;

	[Header("Skill Cooldown")]
	public float _cooldownTimer;

	public float _cooldownMod;

	[Space]
	public float _globalCooldown = 0.25f;

	public bool _underGlobalCooldown;

	public List<SkillCooldownSlot> _skillCoolDownSlots = new List<SkillCooldownSlot>();

	protected NetworkBehaviourSyncVar ____revivingEntityNetId;

	public StatusEntity Network_revivingEntity
	{
		get
		{
			return GetSyncVarNetworkBehaviour(____revivingEntityNetId, ref _revivingEntity);
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter_NetworkBehaviour(value, ref _revivingEntity, 1uL, null, ref ____revivingEntityNetId);
		}
	}

	private void Awake()
	{
		_player = GetComponent<Player>();
		_pStatus = _player._statusEntity;
		_pCombat = _player._pCombat;
		_pVisual = _player._pVisual;
		_pStats = _player._pStats;
		_pMove = _player._pMove;
		_pInventory = _player._pInventory;
	}

	public override void OnStartAuthority()
	{
		base.OnStartAuthority();
		StartCoroutine(Init_SkillLibraryLoad());
	}

	private void Update()
	{
		Handle_BufferingParams();
		Handle_SkillCooldowns();
		Handle_ChannelCasting();
		Client_Iterate_SkillLoadout();
		Client_SkillControl();
	}

	private void Handle_BufferingParams()
	{
		if (_lockControlBuffer > 0f)
		{
			_lockControlBuffer -= Time.deltaTime;
		}
		else
		{
			_lockControlBuffer = 0f;
		}
		if (_castEndBuffer > 0f)
		{
			_castEndBuffer -= Time.deltaTime;
		}
		else
		{
			_castEndBuffer = 0f;
		}
		if (_commandBuffer > 0f)
		{
			_commandBuffer -= Time.deltaTime;
		}
		else
		{
			_commandBuffer = 0f;
		}
		if ((bool)_currentCastSkill)
		{
			if (_player._latency < 150)
			{
				_castEndBuffer = 0.2f;
			}
			else if (_player._latency >= 150)
			{
				_castEndBuffer = 0.4f;
			}
			else if (_player._latency >= 200)
			{
				_castEndBuffer = 0.6f;
			}
		}
	}

	private void Handle_SkillCooldowns()
	{
		_globalCooldown = GameManager._current._statLogics._skillGlobalCooldown;
		_underGlobalCooldown = _cooldownTimer > 0f;
		if (_cooldownTimer > 0f)
		{
			_cooldownTimer -= Time.deltaTime;
		}
		else
		{
			_cooldownTimer = 0f;
		}
		if (_player._currentPlayerCondition == PlayerCondition.DEAD)
		{
			_skillCoolDownSlots.Clear();
		}
		for (int i = 0; i < _skillCoolDownSlots.Count; i++)
		{
			if (_skillCoolDownSlots[i]._currentCooldown > 0f)
			{
				_skillCoolDownSlots[i]._currentCooldown -= Time.deltaTime;
				_skillCoolDownSlots[i]._currentCooldown -= _cooldownMod * Time.deltaTime;
				continue;
			}
			for (int j = 0; j < ActionBarManager._current._actionSlots.Length; j++)
			{
				if (ActionBarManager._current._actionSlots[j]._scriptSkill == _skillCoolDownSlots[i]._scriptSkill)
				{
					ActionBarManager._current.Glow_ActionBarIcon(j);
					ActionBarManager._current._actionSlots[j]._skillCoolDown.enabled = false;
					break;
				}
			}
			_skillCoolDownSlots.RemoveAt(i);
		}
	}

	private IEnumerator Init_SkillLibraryLoad()
	{
		yield return new WaitForSeconds(3f);
		while (!_pVisual._playerRaceModel)
		{
			yield return null;
		}
		Init_SkillLibrary();
	}

	public void Init_QuerySkillStructs()
	{
		Cmd_QuerySkillStructs(_playerSkillStructs.ToArray());
	}

	[Command]
	private void Cmd_QuerySkillStructs(SkillStruct[] _skillStructs)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		GeneratedNetworkCode._Write_SkillStruct_005B_005D(writer, _skillStructs);
		SendCommandInternal("System.Void PlayerCasting::Cmd_QuerySkillStructs(SkillStruct[])", 1786657943, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	private void Rpc_QuerySkillStructs(SkillStruct[] _skillStructs)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		GeneratedNetworkCode._Write_SkillStruct_005B_005D(writer, _skillStructs);
		SendRPCInternal("System.Void PlayerCasting::Rpc_QuerySkillStructs(SkillStruct[])", 832374092, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	private void Init_PassiveObjectCache(SkillStruct[] _skillStructs)
	{
		_playerSkillStructs.Clear();
		_playerSkillStructs.AddRange(_skillStructs);
		_locatingPassives = false;
		StopAllCoroutines();
		StartCoroutine(Set_PassiveSkillEffects(_skillStructs));
	}

	private IEnumerator Set_PassiveSkillEffects(SkillStruct[] _skillStructs)
	{
		if (_locatingPassives)
		{
			yield break;
		}
		_locatingPassives = true;
		if (_player._bufferingStatus)
		{
			yield return null;
		}
		yield return new WaitForSeconds(0.25f);
		Clear_PassiveObjects();
		if (_skillStructs.Length == 0)
		{
			yield break;
		}
		for (int i = 0; i < _skillStructs.Length; i++)
		{
			SkillStruct skillStruct = _skillStructs[i];
			if (!string.IsNullOrEmpty(skillStruct._skillName))
			{
				ScriptableSkill scriptableSkill = GameManager._current.Locate_Skill(_skillStructs[i]._skillName);
				if (scriptableSkill._skillControlType == SkillControlType.Passive && skillStruct._skillUnlocked)
				{
					scriptableSkill.Init_NewSkillObject(scriptableSkill._skillRankParams._skillObject_onCast, _pStatus);
				}
			}
		}
		_locatingPassives = false;
	}

	public void Clear_PassiveObjects()
	{
		for (int i = 0; i < _passiveSkillObjects.Count; i++)
		{
			Object.Destroy(_passiveSkillObjects[i].gameObject);
		}
		_passiveSkillObjects.Clear();
	}

	[Command]
	public void Cmd_SetPassiveRuntimeObject(string _skillName, int _index, bool _isTrue)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteString(_skillName);
		writer.WriteInt(_index);
		writer.WriteBool(_isTrue);
		SendCommandInternal("System.Void PlayerCasting::Cmd_SetPassiveRuntimeObject(System.String,System.Int32,System.Boolean)", 1624301272, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	public void Rpc_SetPassiveRuntimeObject(string _skillName, int _index, bool _isTrue)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteString(_skillName);
		writer.WriteInt(_index);
		writer.WriteBool(_isTrue);
		SendRPCInternal("System.Void PlayerCasting::Rpc_SetPassiveRuntimeObject(System.String,System.Int32,System.Boolean)", -310560179, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	private void Set_PassiveRuntimeObject(string _skillName, int _index, bool _isTrue)
	{
		if (string.IsNullOrWhiteSpace(_skillName) || _passiveSkillObjects.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < _passiveSkillObjects.Count; i++)
		{
			if ((bool)_passiveSkillObjects[i]._skillObject._scriptSkill && _passiveSkillObjects[i]._skillObject._scriptSkill._skillName == _skillName)
			{
				_passiveSkillObjects[i].Activate_RuntimeObject(_index, _isTrue);
			}
		}
	}

	[TargetRpc]
	public void Target_InitSkillLibrary()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendTargetRPCInternal(null, "System.Void PlayerCasting::Target_InitSkillLibrary()", 1158377963, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientCallback]
	public void Init_SkillLibrary()
	{
		if (!NetworkClient.active)
		{
			return;
		}
		ScriptablePlayerRace scriptablePlayerRace = _pVisual._playerRaceModel._scriptablePlayerRace;
		ScriptablePlayerBaseClass scriptablePlayerBaseClass = _pStats._class;
		int syncClassTier = _pStats._syncClassTier;
		ProfileDataManager _pdm = ProfileDataManager._current;
		StatLogics statLogics = GameManager._current._statLogics;
		TabMenu._current._skillsCell.Clear_SkillListData();
		BonusSkillSlot[] bonusSkillProfile = _pdm._characterFile._skillsProfile._bonusSkillProfile;
		_bonusSkillSlots = _pdm._characterFile._skillsProfile._bonusSkillProfile;
		if (_bonusSkillSlots == null || _bonusSkillSlots.Length == 0 || _bonusSkillSlots.Length != GameManager._current._staticBonusSkills.Length)
		{
			_bonusSkillSlots = GameManager._current._staticBonusSkills;
			if (bonusSkillProfile != null && bonusSkillProfile.Length != 0)
			{
				for (int i = 0; i < bonusSkillProfile.Length; i++)
				{
					for (int j = 0; j < _bonusSkillSlots.Length; j++)
					{
						if (bonusSkillProfile[i]._bonusSkillTag == _bonusSkillSlots[j]._bonusSkillTag)
						{
							_bonusSkillSlots[j] = bonusSkillProfile[i];
						}
					}
				}
			}
		}
		if (_pdm._characterFile._skillsProfile._setSkillPresetProfile != null)
		{
			if (_pdm._characterFile._skillsProfile._setSkillPresetProfile._skillPresetIndex.Length != 6)
			{
				_pdm._characterFile._skillsProfile._setSkillPresetProfile._skillPresetIndex = new string[6];
			}
			if (_pdm._characterFile._skillsProfile._setSkillPresetProfile._altSkillPresetIndex.Length != _skillLoadoutSlots.Length)
			{
				_pdm._characterFile._skillsProfile._setSkillPresetProfile._altSkillPresetIndex = new string[6];
			}
			_skillPresetProfile = _pdm._characterFile._skillsProfile._setSkillPresetProfile;
		}
		_playerSkillStructs.Clear();
		_skillLibrary.Clear();
		for (int k = 0; k < statLogics._generalSkills.Length; k++)
		{
			_skillLibrary.Add(new SkillLibrarySlot
			{
				_skillTag = statLogics._generalSkills[k]._skillName,
				_scriptSkill = statLogics._generalSkills[k],
				_skillStructIndex = _skillLibrary.Count,
				_skillTier = SkillTier.NOVICE
			});
			Add_SkillStructToProfile();
		}
		if (scriptablePlayerRace._racialSkill != null)
		{
			if (_bonusSkillSlots != null || _bonusSkillSlots.Length != 0)
			{
				for (int l = 0; l < _bonusSkillSlots.Length; l++)
				{
					if (_bonusSkillSlots[l]._bonusSkillTag == scriptablePlayerRace._racialSkill._skillName)
					{
						_bonusSkillSlots[l]._isLearned = true;
					}
				}
			}
			_skillLibrary.Add(new SkillLibrarySlot
			{
				_skillTag = scriptablePlayerRace._racialSkill._skillName,
				_scriptSkill = scriptablePlayerRace._racialSkill,
				_skillStructIndex = _skillLibrary.Count,
				_skillTier = SkillTier.NOVICE
			});
			Add_SkillStructToProfile();
		}
		if (_bonusSkillSlots != null || _bonusSkillSlots.Length != 0)
		{
			for (int m = 0; m < _bonusSkillSlots.Length; m++)
			{
				if (!_bonusSkillSlots[m]._isLearned)
				{
					continue;
				}
				bool flag = false;
				for (int n = 0; n < _skillLibrary.Count; n++)
				{
					flag = _skillLibrary[n]._skillTag == _bonusSkillSlots[m]._bonusSkillTag;
					if (flag)
					{
						break;
					}
				}
				if (flag)
				{
					continue;
				}
				ScriptableSkill scriptableSkill = GameManager._current.Locate_Skill(_bonusSkillSlots[m]._bonusSkillTag);
				if ((bool)scriptableSkill && scriptableSkill._allowAsBonusSkill)
				{
					if (scriptableSkill._skillRankParams._levelRequirement > _pStats._currentLevel)
					{
						_bonusSkillSlots[m]._isLearned = false;
						continue;
					}
					_skillLibrary.Add(new SkillLibrarySlot
					{
						_skillTag = scriptableSkill._skillName,
						_scriptSkill = scriptableSkill,
						_skillStructIndex = _skillLibrary.Count,
						_skillTier = SkillTier.NOVICE
					});
					Add_SkillStructToProfile();
				}
			}
		}
		if ((bool)scriptablePlayerBaseClass)
		{
			for (int num = 0; num < scriptablePlayerBaseClass._classSkills.Length; num++)
			{
				_skillLibrary.Add(new SkillLibrarySlot
				{
					_skillTag = scriptablePlayerBaseClass._classSkills[num]._skillName,
					_scriptSkill = scriptablePlayerBaseClass._classSkills[num],
					_skillStructIndex = _skillLibrary.Count,
					_skillTier = SkillTier.CLASS
				});
				Add_SkillStructToProfile();
			}
			if (syncClassTier > 0)
			{
				for (int num2 = 0; num2 < scriptablePlayerBaseClass._playerClassTiers[syncClassTier - 1]._classTierSkills.Length; num2++)
				{
					_skillLibrary.Add(new SkillLibrarySlot
					{
						_skillTag = scriptablePlayerBaseClass._playerClassTiers[syncClassTier - 1]._classTierSkills[num2]._skillName,
						_scriptSkill = scriptablePlayerBaseClass._playerClassTiers[syncClassTier - 1]._classTierSkills[num2],
						_skillStructIndex = _skillLibrary.Count,
						_skillTier = SkillTier.SUBCLASS
					});
					Add_SkillStructToProfile();
				}
			}
		}
		bool flag2 = false;
		if (ProfileDataManager._current._characterFile._version != Application.version && GameManager._current._resetSkillPointsOnNewVersion)
		{
			for (int num3 = 0; num3 < GameManager._current._versionIgnoreResetList.Length; num3++)
			{
				if (ProfileDataManager._current._characterFile._version == GameManager._current._versionIgnoreResetList[num3])
				{
					flag2 = true;
					break;
				}
			}
			if (!flag2)
			{
				_pStats.Client_ResetSkillPoints();
			}
		}
		Init_QuerySkillStructs();
		void Add_SkillStructToProfile()
		{
			SkillStruct skillStruct = new SkillStruct();
			List<SkillLibrarySlot> skillLibrary = _skillLibrary;
			skillStruct._skillName = skillLibrary[skillLibrary.Count - 1]._skillTag;
			skillStruct._skillLibraryIndex = _skillLibrary.Count - 1;
			SkillStruct skillStruct2 = skillStruct;
			if (_pdm._characterFile._skillsProfile._setSkillsProfile != null)
			{
				for (int num4 = 0; num4 < _pdm._characterFile._skillsProfile._setSkillsProfile.Length; num4++)
				{
					if (!(_pdm._characterFile._skillsProfile._setSkillsProfile[num4]._skillName != skillStruct2._skillName))
					{
						SkillStruct skillStruct3 = _pdm._characterFile._skillsProfile._setSkillsProfile[num4];
						skillStruct2._skillUnlocked = skillStruct3._skillUnlocked;
						break;
					}
				}
			}
			_playerSkillStructs.Add(skillStruct2);
		}
	}

	[ClientCallback]
	public void Client_Unlock_Skill(int _index)
	{
		ScriptableSkill _scriptSkill;
		if (NetworkClient.active)
		{
			_scriptSkill = _skillLibrary[_index]._scriptSkill;
			if (_pStats._currentSkillPoints > 0 && _pStats._currentSkillPoints >= _scriptSkill._skillRankParams._skillPointCost)
			{
				_pStats._currentSkillPoints -= _scriptSkill._skillRankParams._skillPointCost;
				_playerSkillStructs[_index]._skillUnlocked = true;
				Assign_NewSkillToLoadout();
				ProfileDataManager._current.Save_ProfileData();
				Init_QuerySkillStructs();
			}
		}
		void Assign_NewSkillToLoadout()
		{
			if (_scriptSkill._skillControlType != SkillControlType.Passive)
			{
				for (int i = 0; i < _skillLoadoutSlots.Length; i++)
				{
					ScriptableSkill scriptableSkill = (_pCombat._isUsingAltWeapon ? _skillLoadoutSlots[i]._altScriptSkill : _skillLoadoutSlots[i]._scriptSkill);
					if (!(scriptableSkill != null) && (_skillLoadoutSlots[i]._utilityType == CombatColliderType.NULL || _skillLoadoutSlots[i]._utilityType == _scriptSkill._skillUtilityType))
					{
						for (int j = 0; j < _setSkillPresetIndex.Length; j++)
						{
							if (_setSkillPresetIndex[j] == _scriptSkill._skillName)
							{
								return;
							}
						}
						_setSkillPresetIndex[i] = _scriptSkill._skillName;
						ActionBarManager._current.Glow_ActionBarIcon(i);
						if (_pCombat._isUsingAltWeapon)
						{
							for (int k = 0; k < _skillPresetProfile._skillPresetIndex.Length; k++)
							{
								if (_skillPresetProfile._skillPresetIndex[k] == _scriptSkill._skillName)
								{
									return;
								}
							}
							if (string.IsNullOrWhiteSpace(_skillPresetProfile._skillPresetIndex[i]))
							{
								_skillPresetProfile._skillPresetIndex[i] = _scriptSkill._skillName;
							}
						}
						else
						{
							for (int l = 0; l < _skillPresetProfile._altSkillPresetIndex.Length; l++)
							{
								if (_skillPresetProfile._altSkillPresetIndex[l] == _scriptSkill._skillName)
								{
									return;
								}
							}
							if (string.IsNullOrWhiteSpace(_skillPresetProfile._altSkillPresetIndex[i]))
							{
								_skillPresetProfile._altSkillPresetIndex[i] = _scriptSkill._skillName;
							}
						}
						break;
					}
				}
			}
		}
	}

	[ClientCallback]
	public void Client_Lock_AllSkills()
	{
		if (NetworkClient.active)
		{
			for (int i = 0; i < _playerSkillStructs.Count; i++)
			{
				_playerSkillStructs[i]._skillUnlocked = false;
			}
			Init_QuerySkillStructs();
		}
	}

	[ClientCallback]
	public void Client_Clear_ActionSlots()
	{
		if (NetworkClient.active && _player._playerZoneType == ZoneType.Safe)
		{
			for (int i = 0; i < _skillPresetProfile._skillPresetIndex.Length; i++)
			{
				_skillPresetProfile._skillPresetIndex[i] = string.Empty;
			}
			for (int j = 0; j < _skillPresetProfile._altSkillPresetIndex.Length; j++)
			{
				_skillPresetProfile._altSkillPresetIndex[j] = string.Empty;
			}
			for (int k = 0; k < _skillLoadoutSlots.Length; k++)
			{
				ActionBarManager._current.Glow_ActionBarIcon(k);
				_skillLoadoutSlots[k]._scriptSkill = null;
			}
		}
	}

	[ClientCallback]
	private void Client_Iterate_SkillLoadout()
	{
		if (!NetworkClient.active || !base.isLocalPlayer || _player._bufferingStatus || _skillLibrary.Count <= 0)
		{
			return;
		}
		if (!_pCombat._isUsingAltWeapon)
		{
			_setSkillPresetIndex = _skillPresetProfile._skillPresetIndex;
		}
		else
		{
			_setSkillPresetIndex = _skillPresetProfile._altSkillPresetIndex;
		}
		if (_setSkillPresetIndex == null || _setSkillPresetIndex.Length == 0)
		{
			return;
		}
		for (int i = 0; i < _setSkillPresetIndex.Length; i++)
		{
			if (string.IsNullOrEmpty(_setSkillPresetIndex[i]))
			{
				Reset_Slot(i);
				continue;
			}
			ScriptableSkill scriptableSkill = GameManager._current.Locate_Skill(_setSkillPresetIndex[i]);
			if (!scriptableSkill)
			{
				Reset_Slot(i);
				break;
			}
			if (!_pCombat._isUsingAltWeapon)
			{
				_skillLoadoutSlots[i]._scriptSkill = scriptableSkill;
			}
			else
			{
				_skillLoadoutSlots[i]._altScriptSkill = scriptableSkill;
			}
			for (int j = 0; j < _playerSkillStructs.Count; j++)
			{
				if (j >= _playerSkillStructs.Count - 1 && _playerSkillStructs[j]._skillName != _setSkillPresetIndex[i])
				{
					Reset_Slot(i);
				}
				if (_playerSkillStructs[j]._skillName == _setSkillPresetIndex[i])
				{
					if (!_playerSkillStructs[j]._skillUnlocked)
					{
						Reset_Slot(i);
					}
					else
					{
						_skillLoadoutSlots[i]._skillStructIndex = j;
					}
					break;
				}
			}
		}
		void Reset_Slot(int num)
		{
			if (!_pCombat._isUsingAltWeapon)
			{
				_skillLoadoutSlots[num]._scriptSkill = null;
			}
			else
			{
				_skillLoadoutSlots[num]._altScriptSkill = null;
			}
			_skillLoadoutSlots[num]._skillStructIndex = -1;
		}
	}

	[ClientCallback]
	private bool Check_ConditionToCast()
	{
		if (!NetworkClient.active)
		{
			return default;
		}
		if (_pCombat._currentCommandTimer > 0f)
		{
			return false;
		}
		if (_castEndBuffer > 0f)
		{
			return false;
		}
		if (_isQueuedSkill)
		{
			return false;
		}
		if (_player._currentPlayerAction == PlayerAction.CASTING)
		{
			ErrorPrompt("Not Ready");
			return false;
		}
		if (_player._currentPlayerCondition == PlayerCondition.DEAD)
		{
			ErrorPrompt("You're Dead");
			return false;
		}
		if (_underGlobalCooldown)
		{
			ErrorPrompt("On Cooldown");
			return false;
		}
		if (_pMove._movementLockType != MovementLockType.NONE)
		{
			ErrorPrompt("Not Ready");
			return false;
		}
		if (_lockControlBuffer > 0f)
		{
			ErrorPrompt("Not Ready");
			return false;
		}
		if (_pMove._currentMovementAction == MovementAction.CLIMB || _pMove._currentMovementAction == MovementAction.LEDGEGRAB)
		{
			ErrorPrompt("Not Ready");
			return false;
		}
		if (_pCombat._isChargingWeapon)
		{
			ErrorPrompt("Not Ready");
			return false;
		}
		if (_pCombat._currentCommandTimer > 0f)
		{
			ErrorPrompt("Not Ready");
			return false;
		}
		return true;
		static void ErrorPrompt(string _errorString)
		{
			ErrorPromptTextManager.current.Init_ErrorPrompt(_errorString ?? "");
		}
	}

	[ClientCallback]
	private void Client_SkillControl()
	{
		if (!NetworkClient.active || !_player.isLocalPlayer || _player._inChat || _player._inUI || _player._bufferingStatus || _isQueuedSkill)
		{
			return;
		}
		if (_player._currentPlayerAction == PlayerAction.CASTING && (bool)_currentCastSkill)
		{
			_pMove._currentRevUpSpeed = _currentCastSkill._movementSpeedDecrease;
		}
		if (InputControlManager.current._setGamepadType == GamepadType.Keyboard)
		{
			if (Input.GetKeyDown(InputControlManager.current.actionBar_0))
			{
				Client_SelectSkill(0);
				_cachedInput = InputControlManager.current.actionBar_0;
			}
			if (Input.GetKeyDown(InputControlManager.current.actionBar_1))
			{
				Client_SelectSkill(1);
				_cachedInput = InputControlManager.current.actionBar_1;
			}
			if (Input.GetKeyDown(InputControlManager.current.actionBar_2))
			{
				Client_SelectSkill(2);
				_cachedInput = InputControlManager.current.actionBar_2;
			}
			if (Input.GetKeyDown(InputControlManager.current.actionBar_3))
			{
				Client_SelectSkill(3);
				_cachedInput = InputControlManager.current.actionBar_3;
			}
			if (Input.GetKeyDown(InputControlManager.current.actionBar_4))
			{
				Client_SelectSkill(4);
				_cachedInput = InputControlManager.current.actionBar_4;
			}
			if (Input.GetKeyDown(InputControlManager.current.actionBar_5))
			{
				Client_SelectSkill(5);
				_cachedInput = InputControlManager.current.actionBar_5;
			}
			if (Input.GetKeyDown(InputControlManager.current._recall))
			{
				Client_SelectRecallSkill();
			}
			return;
		}
		if (!GamepadInput.GetButton(GamepadButton.LeftShoulder) && !GamepadInput.GetButton(GamepadButton.RightShoulder) && InputControlManager.current.DpadLeft(DpadHoldType.OnDown))
		{
			Client_SelectRecallSkill();
		}
		if (GamepadInput.GetButton(GamepadButton.LeftShoulder))
		{
			if (GamepadInput.GetButtonDown(GamepadButton.Button_West))
			{
				Client_SelectSkill(0);
				_cachedGamepadButton = GamepadButton.Button_West;
			}
			if (GamepadInput.GetButtonDown(GamepadButton.Button_North))
			{
				Client_SelectSkill(1);
				_cachedGamepadButton = GamepadButton.Button_North;
			}
			if (GamepadInput.GetButtonDown(GamepadButton.Button_East))
			{
				Client_SelectSkill(2);
				_cachedGamepadButton = GamepadButton.Button_East;
			}
		}
		else if (GamepadInput.GetButton(GamepadButton.RightShoulder))
		{
			if (GamepadInput.GetButtonDown(GamepadButton.Button_West))
			{
				Client_SelectSkill(3);
				_cachedGamepadButton = GamepadButton.Button_West;
			}
			if (GamepadInput.GetButtonDown(GamepadButton.Button_North))
			{
				Client_SelectSkill(4);
				_cachedGamepadButton = GamepadButton.Button_North;
			}
			if (GamepadInput.GetButtonDown(GamepadButton.Button_East))
			{
				Client_SelectSkill(5);
				_cachedGamepadButton = GamepadButton.Button_East;
			}
		}
	}

	[ClientCallback]
	private void Client_SelectSkill(int _index)
	{
		if (!NetworkClient.active || _isQueuedSkill)
		{
			return;
		}
		bool num = Check_ConditionToCast();
		bool wepSwapQueued = false;
		if (!num)
		{
			return;
		}
		ScriptableSkill scriptableSkill = (_pCombat._isUsingAltWeapon ? _skillLoadoutSlots[_index]._altScriptSkill : _skillLoadoutSlots[_index]._scriptSkill);
		if (scriptableSkill == null)
		{
			ErrorPrompt("No Skill");
			return;
		}
		for (int i = 0; i < _skillCoolDownSlots.Count; i++)
		{
			if (!(_skillCoolDownSlots[i]._scriptSkill != scriptableSkill))
			{
				ErrorPrompt("On Cooldown");
				return;
			}
		}
		SkillParameters skillRankParams = scriptableSkill._skillRankParams;
		if (_player._statusEntity._summonSkillTag == scriptableSkill._skillName)
		{
			New_CooldownSlot(scriptableSkill);
			_player._statusEntity.Cmd_RemoveSummon();
			return;
		}
		if (!_ignoreCost && _pStatus._currentMana < skillRankParams._manaCost)
		{
			ErrorPrompt("Not Enough Mana");
			return;
		}
		if (!_ignoreCost && _pStatus._currentStamina < skillRankParams._staminaCost)
		{
			ErrorPrompt("Not Enough Stamina");
			return;
		}
		if (!_ignoreCost && _pStatus._currentHealth <= skillRankParams._healthCost)
		{
			ErrorPrompt("Fizzled");
			return;
		}
		if (skillRankParams._checkFullHealth && _pStatus._currentHealth >= _pStats._statStruct._maxHealth)
		{
			ErrorPrompt("Full Health");
			return;
		}
		if (skillRankParams._checkFullMana && _pStatus._currentMana >= _pStats._statStruct._maxMana)
		{
			ErrorPrompt("Full Mana");
			return;
		}
		if (skillRankParams._checkFullStamina && _pStatus._currentStamina >= _pStats._statStruct._maxStamina)
		{
			ErrorPrompt("Full Stamina");
			return;
		}
		if ((bool)skillRankParams._baseRequiredItem && skillRankParams._basedItemCost > 0)
		{
			for (int j = 0; j < _pInventory._heldItems.Count; j++)
			{
				if (_pInventory._heldItems[j]._itemName == skillRankParams._baseRequiredItem._itemName)
				{
					if (_pInventory._heldItems[j]._quantity >= skillRankParams._basedItemCost)
					{
						break;
					}
					ErrorPrompt("Not Enough Of Item");
					return;
				}
				if (j >= _pInventory._heldItems.Count - 1)
				{
					ErrorPrompt("Not Enough Of Item");
					return;
				}
			}
		}
		if (scriptableSkill._weaponTypeRequirements != null && scriptableSkill._weaponTypeRequirements.Length != 0)
		{
			bool flag = false;
			bool flag2 = true;
			for (int k = 0; k < scriptableSkill._weaponTypeRequirements.Length; k++)
			{
				ScriptableWeaponType wepType = scriptableSkill._weaponTypeRequirements[k]._wepType;
				if (wepType == _pCombat._currentScriptableWeaponType)
				{
					flag = true;
					flag2 = false;
					break;
				}
			}
			if (_pCombat._lockWeapon)
			{
				flag2 = false;
			}
			for (int l = 0; l < scriptableSkill._weaponTypeRequirements.Length; l++)
			{
				if (_pCombat._weaponSwapQueued)
				{
					return;
				}
				if (!flag2)
				{
					break;
				}
				ScriptableWeaponType wepType = scriptableSkill._weaponTypeRequirements[l]._wepType;
				if (!_pCombat._isUsingAltWeapon && (bool)_pCombat._foundAltWeaponEquip._scriptableEquip && ((ScriptableWeapon)_pCombat._foundAltWeaponEquip._scriptableEquip).weaponType == wepType)
				{
					_pCombat.Init_QuickSwapWeapon();
					flag = true;
					wepSwapQueued = true;
					break;
				}
				if (_pCombat._isUsingAltWeapon && (bool)_pCombat._foundWeaponEquip._scriptableEquip && ((ScriptableWeapon)_pCombat._foundWeaponEquip._scriptableEquip).weaponType == wepType)
				{
					_pCombat.Init_QuickSwapWeapon();
					flag = true;
					wepSwapQueued = true;
					break;
				}
			}
			if (_pCombat._isDisarmed)
			{
				flag = false;
			}
			if (!flag)
			{
				return;
			}
		}
		if (scriptableSkill._requireShield)
		{
			if (!_pCombat._equippedShield)
			{
				ErrorPrompt("No Shield Equipped");
				return;
			}
			if (_pCombat._currentScriptableWeaponType._weaponHandedness == WeaponHandedness.Two_Handed)
			{
				if (!_pCombat._isUsingAltWeapon)
				{
					if ((bool)_pCombat._foundAltWeaponEquip._scriptableEquip)
					{
						if (((ScriptableWeapon)_pCombat._foundAltWeaponEquip._scriptableEquip).weaponType._weaponHandedness != WeaponHandedness.One_Handed)
						{
							ErrorPrompt("Shield Unusable");
							return;
						}
						_pCombat.Init_QuickSwapWeapon();
						wepSwapQueued = true;
					}
					else
					{
						_pCombat.Init_QuickSwapWeapon();
						wepSwapQueued = true;
					}
				}
				else if (_pCombat._isUsingAltWeapon)
				{
					if ((bool)_pCombat._foundWeaponEquip._scriptableEquip)
					{
						if (((ScriptableWeapon)_pCombat._foundWeaponEquip._scriptableEquip).weaponType._weaponHandedness != WeaponHandedness.One_Handed)
						{
							ErrorPrompt("Shield Unusable");
							return;
						}
						_pCombat.Init_QuickSwapWeapon();
						wepSwapQueued = true;
					}
					else
					{
						_pCombat.Init_QuickSwapWeapon();
						wepSwapQueued = true;
					}
				}
			}
		}
		if (scriptableSkill._skillRankParams._castOnNoConditionOnly)
		{
			for (int m = 0; m < _pStatus._syncConditions.Count; m++)
			{
				if ((bool)scriptableSkill._skillRankParams._selfConditionOutput._scriptableCondition && _pStatus._syncConditions[m]._conditionID == scriptableSkill._skillRankParams._selfConditionOutput._scriptableCondition._conditionID)
				{
					ErrorPrompt("Condition Already Active");
					return;
				}
			}
		}
		StartCoroutine(Queue_SkillCast_Local(scriptableSkill, wepSwapQueued));
		static void ErrorPrompt(string _errorString)
		{
			ErrorPromptTextManager.current.Init_ErrorPrompt(_errorString ?? "");
		}
	}

	[ClientCallback]
	public void Client_SelectRecallSkill()
	{
		if (NetworkClient.active && Check_ConditionToCast())
		{
			if (_player.Network_playerMapInstance._mapName == "Sanctum")
			{
				ErrorPromptTextManager.current.Init_ErrorPrompt("Cannot Recall Here");
				return;
			}
			_currentCastSkill = GameManager._current._statLogics._recallSkill;
			Client_InitCastParams(GameManager._current._statLogics._recallSkill);
		}
	}

	[ClientCallback]
	public void Client_SelectReviveSkill()
	{
		if (NetworkClient.active && Check_ConditionToCast())
		{
			_currentCastSkill = GameManager._current._statLogics._reviveSkill;
			Client_InitCastParams(GameManager._current._statLogics._reviveSkill);
		}
	}

	private IEnumerator Queue_SkillCast_Local(ScriptableSkill _setSkill, bool _wepSwapQueued)
	{
		if (_isQueuedSkill)
		{
			yield break;
		}
		_isQueuedSkill = true;
		if (_wepSwapQueued)
		{
			yield return new WaitForSeconds(0.12f);
			if (!_pCombat._weaponSwapQueued)
			{
				_pCombat.Init_QuickSwapWeapon();
			}
			while (_pCombat._weaponSwapQueued)
			{
				yield return new WaitForSeconds(0.1f);
			}
			if (_player._latency >= 100)
			{
				yield return new WaitForSeconds(0.1f);
			}
			else if (_player._latency >= 150)
			{
				yield return new WaitForSeconds(0.2f);
			}
			else if (_player._latency >= 200)
			{
				yield return new WaitForSeconds(0.3f);
			}
		}
		Client_InitCastParams(_setSkill);
		_isQueuedSkill = false;
	}

	[ClientCallback]
	private void Client_InitCastParams(ScriptableSkill _setSkill)
	{
		if (NetworkClient.active)
		{
			_currentCastSkill = _setSkill;
			_cooldownTimer = _globalCooldown;
			_underGlobalCooldown = true;
			_isCasting = true;
			_currentCastSkill.Init_SkillInput(_pStatus);
			Cmd_InitSkill(_setSkill._skillName);
		}
	}

	[Command]
	public void Cmd_InitSkill(string _skillName)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteString(_skillName);
		SendCommandInternal("System.Void PlayerCasting::Cmd_InitSkill(System.String)", 2092911395, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	private void Rpc_InitSkill(string _skillName)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteString(_skillName);
		SendRPCInternal("System.Void PlayerCasting::Rpc_InitSkill(System.String)", 1125685464, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	private void Handle_ChannelCasting()
	{
		bool _isLocalOwned = base.isLocalPlayer && base.isOwned;
		if (_castTimer < 0f || !_isCasting)
		{
			_castTimer = 0f;
		}
		if (_player._bufferingStatus || _lockControlBuffer > 0f)
		{
			Reset_CurrentCastConditions();
		}
		else if ((bool)_currentCastSkill)
		{
			_pMove._inputDirectionAttackForce = _currentCastSkill._attackForceInputInfluence;
			_setCastTime = _currentCastSkill._skillRankParams._baseCastTime + _castTimerMod;
			if (_setCastTime < 0.12f)
			{
				_setCastTime = 0.12f;
			}
			if (_isLocalOwned && _currentCastSkill == GameManager._current._statLogics._reviveSkill && (_pMove._currentMovementAction != MovementAction.IDLE || ((bool)Network_revivingEntity && Network_revivingEntity._currentHealth > 0)))
			{
				Cmd_InitCastInterrupt();
				_currentCastSkill = null;
			}
			if (_pStatus._currentHealth <= 0 || _player._currentPlayerCondition != PlayerCondition.ACTIVE)
			{
				Reset_CurrentCastConditions();
				return;
			}
			if ((bool)_currentCastSkill)
			{
				if (!_ignoreCost)
				{
					if (_pStatus._currentMana < _currentCastSkill._skillRankParams._manaCost)
					{
						Reset_CurrentCastConditions();
						return;
					}
					if (_pStatus._currentStamina < _currentCastSkill._skillRankParams._staminaCost)
					{
						Reset_CurrentCastConditions();
						return;
					}
					if (_pStatus._currentHealth < _currentCastSkill._skillRankParams._healthCost)
					{
						Reset_CurrentCastConditions();
						return;
					}
				}
				switch (_currentCastSkill._skillControlType)
				{
				case SkillControlType.Standard:
					Handle_StandardCast();
					break;
				case SkillControlType.HoldAndReleaseKey:
					Handle_HoldAndReleaseKeyCast();
					break;
				}
			}
			Handle_CastInterruptInput();
		}
		else
		{
			_cachedProjectileObj = null;
			_earlySkillObjectEmitted = false;
		}
		void Handle_CastInterruptInput()
		{
			if (base.isLocalPlayer && (bool)_currentCastSkill && !_initCastInterruptInput)
			{
				bool flag = GamepadInput.GetButton(GamepadButton.LeftShoulder) || GamepadInput.GetButton(GamepadButton.RightShoulder);
				InputControlManager current = InputControlManager.current;
				if ((((Input.GetKeyDown(current._attack) || Input.GetKeyDown(InputControlManager.current._interact) || (GamepadInput.GetButtonDown(GamepadButton.Button_West) && !flag) || Input.GetKeyDown(current._block)) && !_player._inUI) || GamepadInput.GetButtonDown(GamepadButton.LeftShoulder) || GamepadInput.GetButtonDown(GamepadButton.RightShoulder) || GamepadInput.GetButtonDown(GamepadButton.RightTrigger) || (GamepadInput.GetButtonDown(GamepadButton.LeftTrigger) && !_player._inUI)) && _commandBuffer <= 0f)
				{
					Cmd_InitCastInterrupt();
					_initCastInterruptInput = true;
					_commandBuffer = 0.28f;
				}
			}
		}
		void Handle_HoldAndReleaseKeyCast()
		{
			if (_isLocalOwned)
			{
				if (InputControlManager.current._setGamepadType == GamepadType.Keyboard && !Input.GetKey(_cachedInput))
				{
					Reset_HoldAndReleaseKeyInput();
				}
				if (InputControlManager.current._setGamepadType != GamepadType.Keyboard && !GamepadInput.GetButton(_cachedGamepadButton))
				{
					Reset_HoldAndReleaseKeyInput();
				}
			}
			if (_castTimer >= _setCastTime && !_earlySkillObjectEmitted)
			{
				if (_isLocalOwned && _currentCastSkill._skillRankParams._skillObject_onChannel != null)
				{
					Cmd_SpawnEarlySkillObj();
				}
				_earlySkillObjectEmitted = true;
			}
			else
			{
				_castTimer += Time.deltaTime;
			}
			if ((bool)_cachedProjectileObj && !_cachedProjectileObj.gameObject.activeInHierarchy)
			{
				Server_InterruptCast();
			}
		}
		void Handle_StandardCast()
		{
			if (_castTimer < _setCastTime)
			{
				_castTimer += Time.deltaTime;
			}
			else
			{
				Server_CastSkill();
			}
		}
		void Reset_CurrentCastConditions()
		{
			if ((bool)_currentCastSkill)
			{
				Server_InterruptCast();
			}
			if ((bool)_cachedProjectileObj)
			{
				_cachedProjectileObj.Init_StopProjectile();
			}
			_cachedProjectileObj = null;
		}
		void Reset_HoldAndReleaseKeyInput()
		{
			if (_castTimer >= _setCastTime)
			{
				Cmd_CastInit();
			}
			else
			{
				Cmd_InitCastInterrupt();
				_currentCastSkill = null;
			}
		}
	}

	[Command]
	private void Cmd_SpawnEarlySkillObj()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void PlayerCasting::Cmd_SpawnEarlySkillObj()", 1827649906, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	private void Rpc_SpawnEarlySkillObj()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendRPCInternal("System.Void PlayerCasting::Rpc_SpawnEarlySkillObj()", -1995036249, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	[Command]
	public void Cmd_InitCastInterrupt()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void PlayerCasting::Cmd_InitCastInterrupt()", 1825594386, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ServerCallback]
	public void Server_InterruptCast()
	{
		if (NetworkServer.active && (!_currentCastSkill || !_currentCastSkill._skillRankParams._isUninterruptable))
		{
			_player._currentPlayerAction = PlayerAction.IDLE;
			if ((bool)_cachedProjectileObj)
			{
				_cachedProjectileObj.Init_StopProjectile();
			}
			_cachedProjectileObj = null;
			Rpc_InterruptCast();
			Reset_CastConditions();
		}
	}

	[ClientRpc]
	private void Rpc_InterruptCast()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendRPCInternal("System.Void PlayerCasting::Rpc_InterruptCast()", -2031841461, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	[Command]
	public void Cmd_InterruptEffect()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void PlayerCasting::Cmd_InterruptEffect()", 2066183794, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	private void Rpc_InterruptEffect()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendRPCInternal("System.Void PlayerCasting::Rpc_InterruptEffect()", 559479389, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	private void Init_InterruptEffect()
	{
		_playerCastEffect.Play_InterruptEffect();
	}

	[Command]
	public void Cmd_SetReviveEntity(StatusEntity _statusEntity)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteNetworkBehaviour(_statusEntity);
		SendCommandInternal("System.Void PlayerCasting::Cmd_SetReviveEntity(StatusEntity)", 397328323, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[Command]
	public void Cmd_CastInit()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void PlayerCasting::Cmd_CastInit()", 719425587, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ServerCallback]
	public void Server_CastSkill()
	{
		if (!NetworkServer.active || !_currentCastSkill)
		{
			return;
		}
		_player._currentPlayerAction = PlayerAction.IDLE;
		if ((bool)_cachedProjectileObj)
		{
			if (_cachedProjectileObj._delaySpawnsCallback)
			{
				_cachedProjectileObj.Init_DelaySpawns();
				_cachedProjectileObj.Init_StopProjectile();
			}
			else
			{
				_cachedProjectileObj.Init_StopProjectile();
			}
			_cachedProjectileObj = null;
		}
		_currentCastSkill.Init_SkillBehavior(_pStatus);
		_playerCastEffect.Play_CastEffect();
		New_CooldownSlot(_currentCastSkill);
		Rpc_CastSkill(_currentCastSkill._skillName);
		if ((bool)Network_revivingEntity)
		{
			Network_revivingEntity.Init_RevivePlayer(Network_revivingEntity);
			Network_revivingEntity = null;
		}
		_pStats.Iterate_EquipConditions(EquipConditionCallback.None, _currentCastSkill);
		if (_currentCastSkill._skillUtilityType == CombatColliderType.OFFENSIVE)
		{
			_pStats.Iterate_EquipConditions(EquipConditionCallback.Cast_OffensiveSkill, null);
		}
		else if (_currentCastSkill._skillUtilityType == CombatColliderType.SUPPORT)
		{
			_pStats.Iterate_EquipConditions(EquipConditionCallback.Cast_SupportSkill, null);
		}
		Reset_CastConditions();
	}

	[ClientRpc]
	private void Rpc_CastSkill(string _skillName)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteString(_skillName);
		SendRPCInternal("System.Void PlayerCasting::Rpc_CastSkill(System.String)", -675036793, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	private void New_CooldownSlot(ScriptableSkill _setSkill)
	{
		float num = _setSkill._skillRankParams._baseCooldown;
		if (num < GameManager._current._statLogics._lowestCooldownValue)
		{
			num = GameManager._current._statLogics._lowestCooldownValue;
		}
		SkillCooldownSlot item = new SkillCooldownSlot
		{
			_scriptSkill = _setSkill,
			_currentCooldown = num,
			_setCooldown = num
		};
		_skillCoolDownSlots.Add(item);
	}

	private void Reset_CastConditions()
	{
		_initCastInterruptInput = false;
		_playerCastEffect.Stop_ChannelEffect();
		_currentCastSkill = null;
		Network_revivingEntity = null;
		_isCasting = false;
		_castTimer = 0f;
	}

	public override bool Weaved()
	{
		return true;
	}

	protected void UserCode_Cmd_QuerySkillStructs__SkillStruct_005B_005D(SkillStruct[] _skillStructs)
	{
		Init_PassiveObjectCache(_skillStructs);
		Rpc_QuerySkillStructs(_skillStructs);
	}

	protected static void InvokeUserCode_Cmd_QuerySkillStructs__SkillStruct_005B_005D(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command Cmd_QuerySkillStructs called on client.");
		}
		else
		{
			((PlayerCasting)obj).UserCode_Cmd_QuerySkillStructs__SkillStruct_005B_005D(GeneratedNetworkCode._Read_SkillStruct_005B_005D(reader));
		}
	}

	protected void UserCode_Rpc_QuerySkillStructs__SkillStruct_005B_005D(SkillStruct[] _skillStructs)
	{
		if (!base.isServer)
		{
			Init_PassiveObjectCache(_skillStructs);
		}
	}

	protected static void InvokeUserCode_Rpc_QuerySkillStructs__SkillStruct_005B_005D(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC Rpc_QuerySkillStructs called on server.");
		}
		else
		{
			((PlayerCasting)obj).UserCode_Rpc_QuerySkillStructs__SkillStruct_005B_005D(GeneratedNetworkCode._Read_SkillStruct_005B_005D(reader));
		}
	}

	protected void UserCode_Cmd_SetPassiveRuntimeObject__String__Int32__Boolean(string _skillName, int _index, bool _isTrue)
	{
		_player.Add_PacketCount();
		ScriptableSkill scriptableSkill = GameManager._current.Locate_Skill(_skillName);
		if ((bool)scriptableSkill)
		{
			_pStats.Iterate_EquipConditions(EquipConditionCallback.None, scriptableSkill);
			Set_PassiveRuntimeObject(_skillName, _index, _isTrue);
			Rpc_SetPassiveRuntimeObject(_skillName, _index, _isTrue);
		}
	}

	protected static void InvokeUserCode_Cmd_SetPassiveRuntimeObject__String__Int32__Boolean(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command Cmd_SetPassiveRuntimeObject called on client.");
		}
		else
		{
			((PlayerCasting)obj).UserCode_Cmd_SetPassiveRuntimeObject__String__Int32__Boolean(reader.ReadString(), reader.ReadInt(), reader.ReadBool());
		}
	}

	protected void UserCode_Rpc_SetPassiveRuntimeObject__String__Int32__Boolean(string _skillName, int _index, bool _isTrue)
	{
		Set_PassiveRuntimeObject(_skillName, _index, _isTrue);
	}

	protected static void InvokeUserCode_Rpc_SetPassiveRuntimeObject__String__Int32__Boolean(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC Rpc_SetPassiveRuntimeObject called on server.");
		}
		else
		{
			((PlayerCasting)obj).UserCode_Rpc_SetPassiveRuntimeObject__String__Int32__Boolean(reader.ReadString(), reader.ReadInt(), reader.ReadBool());
		}
	}

	protected void UserCode_Target_InitSkillLibrary()
	{
		Init_SkillLibrary();
	}

	protected static void InvokeUserCode_Target_InitSkillLibrary(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC Target_InitSkillLibrary called on server.");
		}
		else
		{
			((PlayerCasting)obj).UserCode_Target_InitSkillLibrary();
		}
	}

	protected void UserCode_Cmd_InitSkill__String(string _skillName)
	{
		ScriptableSkill scriptableSkill = GameManager._current.Locate_Skill(_skillName);
		if (!scriptableSkill)
		{
			return;
		}
		for (int i = 0; i < _skillCoolDownSlots.Count; i++)
		{
			if (_skillCoolDownSlots[i]._scriptSkill == scriptableSkill)
			{
				return;
			}
		}
		if (scriptableSkill._applySheatheCondition)
		{
			_pCombat.Network_currentSheathCondition = scriptableSkill._setSheatheCondition;
		}
		if (scriptableSkill._skillRankParams._levelRequirement > _pStats._currentLevel)
		{
			Rpc_InterruptCast();
			return;
		}
		_player.Add_PacketCount();
		scriptableSkill.Init_SkillInput(_pStatus);
		Rpc_InitSkill(_skillName);
	}

	protected static void InvokeUserCode_Cmd_InitSkill__String(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command Cmd_InitSkill called on client.");
		}
		else
		{
			((PlayerCasting)obj).UserCode_Cmd_InitSkill__String(reader.ReadString());
		}
	}

	protected void UserCode_Rpc_InitSkill__String(string _skillName)
	{
		ScriptableSkill scriptableSkill = GameManager._current.Locate_Skill(_skillName);
		if ((bool)scriptableSkill)
		{
			_currentCastSkill = scriptableSkill;
			_playerCastEffect.Play_ChannelEffect(scriptableSkill);
		}
	}

	protected static void InvokeUserCode_Rpc_InitSkill__String(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC Rpc_InitSkill called on server.");
		}
		else
		{
			((PlayerCasting)obj).UserCode_Rpc_InitSkill__String(reader.ReadString());
		}
	}

	protected void UserCode_Cmd_SpawnEarlySkillObj()
	{
		_player.Add_PacketCount();
		Rpc_SpawnEarlySkillObj();
	}

	protected static void InvokeUserCode_Cmd_SpawnEarlySkillObj(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command Cmd_SpawnEarlySkillObj called on client.");
		}
		else
		{
			((PlayerCasting)obj).UserCode_Cmd_SpawnEarlySkillObj();
		}
	}

	protected void UserCode_Rpc_SpawnEarlySkillObj()
	{
		if ((bool)_currentCastSkill && !(_currentCastSkill._skillRankParams._skillObject_onChannel == null))
		{
			_currentCastSkill.Init_NewSkillObject(_currentCastSkill._skillRankParams._skillObject_onChannel, _pStatus);
		}
	}

	protected static void InvokeUserCode_Rpc_SpawnEarlySkillObj(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC Rpc_SpawnEarlySkillObj called on server.");
		}
		else
		{
			((PlayerCasting)obj).UserCode_Rpc_SpawnEarlySkillObj();
		}
	}

	protected void UserCode_Cmd_InitCastInterrupt()
	{
		Server_InterruptCast();
	}

	protected static void InvokeUserCode_Cmd_InitCastInterrupt(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command Cmd_InitCastInterrupt called on client.");
		}
		else
		{
			((PlayerCasting)obj).UserCode_Cmd_InitCastInterrupt();
		}
	}

	protected void UserCode_Rpc_InterruptCast()
	{
		if (base.isLocalPlayer)
		{
			ErrorPromptTextManager.current.Init_ErrorPrompt("Interrupted");
			_pMove._currentRevUpSpeed = 0f;
		}
		if (_player._currentPlayerAction != PlayerAction.ATTACKING)
		{
			_player._currentPlayerAction = PlayerAction.IDLE;
		}
		_pVisual.Local_CrossFadeAnim("Empty", 0.12f, 9);
		Init_InterruptEffect();
		if ((bool)_cachedProjectileObj)
		{
			_cachedProjectileObj.Init_StopProjectile();
		}
		_cachedProjectileObj = null;
		Reset_CastConditions();
	}

	protected static void InvokeUserCode_Rpc_InterruptCast(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC Rpc_InterruptCast called on server.");
		}
		else
		{
			((PlayerCasting)obj).UserCode_Rpc_InterruptCast();
		}
	}

	protected void UserCode_Cmd_InterruptEffect()
	{
		Rpc_InterruptEffect();
	}

	protected static void InvokeUserCode_Cmd_InterruptEffect(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command Cmd_InterruptEffect called on client.");
		}
		else
		{
			((PlayerCasting)obj).UserCode_Cmd_InterruptEffect();
		}
	}

	protected void UserCode_Rpc_InterruptEffect()
	{
		Init_InterruptEffect();
	}

	protected static void InvokeUserCode_Rpc_InterruptEffect(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC Rpc_InterruptEffect called on server.");
		}
		else
		{
			((PlayerCasting)obj).UserCode_Rpc_InterruptEffect();
		}
	}

	protected void UserCode_Cmd_SetReviveEntity__StatusEntity(StatusEntity _statusEntity)
	{
		Network_revivingEntity = _statusEntity;
		if ((bool)_player && (bool)_statusEntity._isPlayer)
		{
			_statusEntity._isPlayer._chatBehaviour.Target_RecieveTriggerMessage(_player._nickname + " is attempting to revive you.");
		}
	}

	protected static void InvokeUserCode_Cmd_SetReviveEntity__StatusEntity(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command Cmd_SetReviveEntity called on client.");
		}
		else
		{
			((PlayerCasting)obj).UserCode_Cmd_SetReviveEntity__StatusEntity(reader.ReadNetworkBehaviour<StatusEntity>());
		}
	}

	protected void UserCode_Cmd_CastInit()
	{
		Server_CastSkill();
	}

	protected static void InvokeUserCode_Cmd_CastInit(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command Cmd_CastInit called on client.");
		}
		else
		{
			((PlayerCasting)obj).UserCode_Cmd_CastInit();
		}
	}

	protected void UserCode_Rpc_CastSkill__String(string _skillName)
	{
		if (base.isServer)
		{
			return;
		}
		ScriptableSkill scriptableSkill = GameManager._current.Locate_Skill(_skillName);
		if (!scriptableSkill)
		{
			return;
		}
		_player._currentPlayerAction = PlayerAction.IDLE;
		if ((bool)_cachedProjectileObj)
		{
			if (_cachedProjectileObj._delaySpawnsCallback)
			{
				_cachedProjectileObj.Init_DelaySpawns();
				_cachedProjectileObj.Init_StopProjectile();
			}
			else
			{
				_cachedProjectileObj.Init_StopProjectile();
			}
			_cachedProjectileObj = null;
		}
		_playerCastEffect.Play_CastEffect();
		scriptableSkill.Init_SkillBehavior(_pStatus);
		New_CooldownSlot(scriptableSkill);
		Reset_CastConditions();
	}

	protected static void InvokeUserCode_Rpc_CastSkill__String(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC Rpc_CastSkill called on server.");
		}
		else
		{
			((PlayerCasting)obj).UserCode_Rpc_CastSkill__String(reader.ReadString());
		}
	}

	static PlayerCasting()
	{
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerCasting), "System.Void PlayerCasting::Cmd_QuerySkillStructs(SkillStruct[])", InvokeUserCode_Cmd_QuerySkillStructs__SkillStruct_005B_005D, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerCasting), "System.Void PlayerCasting::Cmd_SetPassiveRuntimeObject(System.String,System.Int32,System.Boolean)", InvokeUserCode_Cmd_SetPassiveRuntimeObject__String__Int32__Boolean, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerCasting), "System.Void PlayerCasting::Cmd_InitSkill(System.String)", InvokeUserCode_Cmd_InitSkill__String, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerCasting), "System.Void PlayerCasting::Cmd_SpawnEarlySkillObj()", InvokeUserCode_Cmd_SpawnEarlySkillObj, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerCasting), "System.Void PlayerCasting::Cmd_InitCastInterrupt()", InvokeUserCode_Cmd_InitCastInterrupt, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerCasting), "System.Void PlayerCasting::Cmd_InterruptEffect()", InvokeUserCode_Cmd_InterruptEffect, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerCasting), "System.Void PlayerCasting::Cmd_SetReviveEntity(StatusEntity)", InvokeUserCode_Cmd_SetReviveEntity__StatusEntity, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerCasting), "System.Void PlayerCasting::Cmd_CastInit()", InvokeUserCode_Cmd_CastInit, requiresAuthority: true);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerCasting), "System.Void PlayerCasting::Rpc_QuerySkillStructs(SkillStruct[])", InvokeUserCode_Rpc_QuerySkillStructs__SkillStruct_005B_005D);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerCasting), "System.Void PlayerCasting::Rpc_SetPassiveRuntimeObject(System.String,System.Int32,System.Boolean)", InvokeUserCode_Rpc_SetPassiveRuntimeObject__String__Int32__Boolean);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerCasting), "System.Void PlayerCasting::Rpc_InitSkill(System.String)", InvokeUserCode_Rpc_InitSkill__String);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerCasting), "System.Void PlayerCasting::Rpc_SpawnEarlySkillObj()", InvokeUserCode_Rpc_SpawnEarlySkillObj);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerCasting), "System.Void PlayerCasting::Rpc_InterruptCast()", InvokeUserCode_Rpc_InterruptCast);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerCasting), "System.Void PlayerCasting::Rpc_InterruptEffect()", InvokeUserCode_Rpc_InterruptEffect);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerCasting), "System.Void PlayerCasting::Rpc_CastSkill(System.String)", InvokeUserCode_Rpc_CastSkill__String);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerCasting), "System.Void PlayerCasting::Target_InitSkillLibrary()", InvokeUserCode_Target_InitSkillLibrary);
	}

	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteNetworkBehaviour(Network_revivingEntity);
			return;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteNetworkBehaviour(Network_revivingEntity);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize_NetworkBehaviour(ref _revivingEntity, null, reader, ref ____revivingEntityNetId);
			return;
		}
		long num = (long)reader.ReadULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize_NetworkBehaviour(ref _revivingEntity, null, reader, ref ____revivingEntityNetId);
		}
	}
}
