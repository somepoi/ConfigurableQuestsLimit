using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Mirror;
using UnityEngine;

public class ProfileDataManager : MonoBehaviour
{
	public static ProfileDataManager _current;

	[SerializeField]
	private CharacterSelectManager _charSelectManager;

	[Header("Data Pathing / Selection")]
	[SerializeField]
	private string _dataPath = string.Empty;

	[SerializeField]
	private string _hostSettingsDataPath = string.Empty;

	[SerializeField]
	private bool _usePersistantDataPath;

	public bool _loadPreviousCharacter = true;

	private int _selectedFileIndex;

	[Header("Hosting Settings File")]
	public ServerHostSettings_Profile _hostSettingsProfile;

	[Header("Tutorial File")]
	public Tutorial_Profile _tutorialProfile;

	[Header("Item Storage File")]
	public ItemStorage_Profile _itemStorageProfile;

	public ItemStorage_Profile _itemStorageProfile_02;

	public ItemStorage_Profile _itemStorageProfile_03;

	public ItemStorage_Profile _itemStorageProfile_04;

	public ItemStorage_Profile _itemStorageProfile_05;

	public ItemStorage_Profile _itemStorageProfile_06;

	public ItemStorage_Profile _itemStorageProfile_07;

	public ItemStorage_Profile _itemStorageProfile_08;

	[Header("Set Character File")]
	public CharacterFile _characterFile;

	[Header("Saved Character Files")]
	public CharacterFile[] _characterFiles = new CharacterFile[7];

	public int SelectedFileIndex
	{
		get
		{
			return _selectedFileIndex;
		}
		set
		{
			if (value < 0)
			{
				value = _characterFiles.Length - 1;
			}
			if (value > _characterFiles.Length - 1)
			{
				value = 0;
			}
			_selectedFileIndex = value;
		}
	}

	private void Awake()
	{
		if (_current == null)
		{
			_current = this;
			if (_usePersistantDataPath)
			{
				_dataPath = Application.persistentDataPath;
			}
			else
			{
				_dataPath = Application.dataPath + "/profileCollections/";
			}
			_hostSettingsDataPath = Application.dataPath + "/Settings/";
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void Start()
	{
		Cache_CharacterFiles();
	}

	private void Cache_CharacterFiles()
	{
		for (int i = 0; i < _characterFiles.Length; i++)
		{
			LoadPlayerDataAsync(i);
		}
		_charSelectManager.Initalize_CharacterFiles(_characterFiles);
		Load_ItemStorageData();
	}

	public void Set_FileIndex(int _i)
	{
		SelectedFileIndex = _i;
		_characterFile = _characterFiles[_selectedFileIndex];
		MainMenuManager._current._characterSelectManager.Apply_CharacterSelectDisplay();
	}

	private async void LoadPlayerDataAsync(int _index)
	{
		string _characterProfile = "atl_characterProfile_" + $"{_index}";
		string text = _characterProfile + "_bak";
		string StandardCharacterFilePath = _dataPath + _characterProfile;
		string TempCharacterFilePath = _dataPath + text;
		Task task = AttemptLoadJSONTask(StandardCharacterFilePath, _index);
		Task task2 = Task.Delay(2000);
		Task task3 = await Task.WhenAny(task, task2);
		try
		{
			Load_ProfileData(StandardCharacterFilePath, _index);
		}
		catch (Exception)
		{
			if (File.Exists(TempCharacterFilePath))
			{
				File.Copy(TempCharacterFilePath, StandardCharacterFilePath, overwrite: true);
				HostConsole._current.Init_NetworkStatusMessage("Profile error (" + _characterProfile + "), using backup save file.");
				LoadPlayerDataAsync(_index);
				return;
			}
			Debug.Log("Temp character file does not exist.");
			HostConsole._current.Init_NetworkStatusMessage("Profile error (" + _characterProfile + "), no backup save file found. File deleted.");
			Delete_ProfileData(_index);
		}
		if (task3 == task && task.Status == TaskStatus.RanToCompletion)
		{
			Load_ProfileData(StandardCharacterFilePath, _index);
		}
	}

	private async Task AttemptLoadJSONTask(string filePath, int _index)
	{
		await Task.Run(() =>
		{
			Load_ProfileData(filePath, _index);
		});
	}

	public void Save_ProfileData()
	{
		if (!Directory.Exists(_dataPath))
		{
			Directory.CreateDirectory(_dataPath);
		}
		if ((bool)Player._mainPlayer)
		{
			if (Player._mainPlayer._bufferingStatus || !NetworkClient.isConnected)
			{
				return;
			}
			CharacterFile characterFile = _characterFile;
			PlayerStats pStats = Player._mainPlayer._pStats;
			PlayerInventory pInventory = Player._mainPlayer._pInventory;
			PlayerEquipment pEquipment = Player._mainPlayer._pEquipment;
			PlayerCombat pCombat = Player._mainPlayer._pCombat;
			PlayerCasting pCasting = Player._mainPlayer._pCasting;
			PlayerVisual pVisual = Player._mainPlayer._pVisual;
			PlayerQuesting pQuest = Player._mainPlayer._pQuest;
			characterFile._nickName = Player._mainPlayer._nickname;
			characterFile._version = Application.version;
			characterFile._isNewCharacter = false;
			characterFile._isEmptySlot = false;
			if (!pStats)
			{
				return;
			}
			characterFile._statsProfile._currentLevel = pStats._currentLevel;
			characterFile._statsProfile._currentExp = pStats._currentExp;
			characterFile._statsProfile._classID = pStats._syncClass;
			characterFile._statsProfile._classTier = pStats._syncClassTier;
			characterFile._statsProfile._skillPoints = pStats._currentSkillPoints;
			characterFile._statsProfile._attributePoints = pStats._currentAttributePoints;
			characterFile._statsProfile._attributes = pStats._playerAttributes.ToArray();
			characterFile._statsProfile._professions = pStats._playerProfessions.ToArray();
			if (!pVisual)
			{
				return;
			}
			characterFile._appearanceProfile._hideHelm = pVisual._playerAppearanceStruct._hideHelm;
			characterFile._appearanceProfile._hideCape = pVisual._playerAppearanceStruct._hideCape;
			characterFile._appearanceProfile._hideChest = pVisual._playerAppearanceStruct._hideChest;
			characterFile._appearanceProfile._hideLeggings = pVisual._playerAppearanceStruct._hideLeggings;
			characterFile._appearanceProfile._hideVanityHelm = pVisual._playerAppearanceStruct._hideVanityHelm;
			characterFile._appearanceProfile._hideVanityCape = pVisual._playerAppearanceStruct._hideVanityCape;
			characterFile._appearanceProfile._hideVanityChest = pVisual._playerAppearanceStruct._hideVanityChest;
			characterFile._appearanceProfile._hideVanityLeggings = pVisual._playerAppearanceStruct._hideVanityLeggings;
			characterFile._appearanceProfile._helmDyeIndex = pVisual._playerAppearanceStruct._dyeIndex;
			characterFile._appearanceProfile._isLeftHanded = pVisual._playerAppearanceStruct._isLeftHanded;
			if (!pInventory)
			{
				return;
			}
			characterFile._currency = pInventory._heldCurrency;
			characterFile._inventoryProfile = pInventory._heldItems.ToArray();
			string[] array = new string[4];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = QuickItemActionBarGUI._current._quickItemSlots[i]._setItemName;
			}
			characterFile._quickItemSlotProfile = array;
			if (!pCasting)
			{
				return;
			}
			characterFile._skillsProfile._setSkillsProfile = pCasting._playerSkillStructs.ToArray();
			characterFile._skillsProfile._bonusSkillProfile = pCasting._bonusSkillSlots;
			characterFile._skillsProfile._setSkillPresetProfile = pCasting._skillPresetProfile;
			if (!pCombat)
			{
				return;
			}
			characterFile._isWeaponLocked = pCombat._lockWeapon;
			characterFile._isAltWeaponEquipped = pCombat._isUsingAltWeapon;
			characterFile._vanityStruct = pEquipment._vanitySyncStruct;
			if ((bool)pQuest)
			{
				QuestProgressStruct[] array2 = new QuestProgressStruct[pQuest._questProgressData.Count];
				for (int j = 0; j < pQuest._questProgressData.Count; j++)
				{
					array2[j] = pQuest._questProgressData[j];
				}
				characterFile._questProgressProfile._questProgressStructs = array2;
				string[] finishedQuests = pQuest._finishedQuests.Keys.ToArray();
				characterFile._questProgressProfile._finishedQuests = finishedQuests;
				characterFile._dialogIntroProfile = DialogManager._current._dialogIntroductionTags.ToArray();
				characterFile._waypointAttunementProfile = Player._mainPlayer._waypointAttunements.ToArray();
				string contents = JsonUtility.ToJson(characterFile, prettyPrint: true);
				File.WriteAllText(_dataPath + $"/atl_characterProfile_{_selectedFileIndex}", contents);
			}
		}
		else if (MainMenuManager._current._mainMenuCondition == MainMenuCondition.CharacterCreation)
		{
			_characterFile._isNewCharacter = true;
			_characterFile._isEmptySlot = false;
			string contents2 = JsonUtility.ToJson(_characterFile, prettyPrint: true);
			File.WriteAllText(_dataPath + $"/atl_characterProfile_{_selectedFileIndex}", contents2);
		}
	}

	public void Save_BackupFile()
	{
		string text = "atl_characterProfile_" + $"{_selectedFileIndex}";
		string text2 = text + "_bak";
		string sourceFileName = _dataPath + text;
		string text3 = _dataPath + text2;
		if (!File.Exists(text3))
		{
			File.WriteAllText(contents: JsonUtility.ToJson(_characterFile, prettyPrint: true), path: _dataPath + $"/atl_characterProfile_{_selectedFileIndex}_bak");
		}
		File.Copy(sourceFileName, text3, overwrite: true);
	}

	public void Load_ProfileData(string _filePath, int _index)
	{
		if (!File.Exists(_filePath))
		{
			_characterFiles[_index]._isEmptySlot = true;
			return;
		}
		CharacterFile characterFile = JsonUtility.FromJson<CharacterFile>(File.ReadAllText(_filePath));
		_characterFiles[_index] = characterFile;
	}

	public void Delete_ProfileData(int _index)
	{
		File.Delete(_dataPath + $"/atl_characterProfile_{_index}");
		if (File.Exists(_dataPath + $"/atl_characterProfile_{_index}_bak"))
		{
			File.Delete(_dataPath + $"/atl_characterProfile_{_index}_bak");
		}
		CharacterFile obj = _characterFiles[_index];
		obj._nickName = string.Empty;
		obj._currency = 0;
		PlayerStats_Profile statsProfile = new PlayerStats_Profile
		{
			_classID = string.Empty,
			_currentExp = 0,
			_skillPoints = 0,
			_classTier = 0,
			_currentLevel = 1,
			_attributePoints = 0,
			_attributes = new AttributeStruct[0],
			_professions = new ProfessionStruct[0]
		};
		PlayerAppearance_Profile appearanceProfile = new PlayerAppearance_Profile
		{
			_setSkinColorProfile = new ColorAdjustShader_Profile(),
			_hairColorProfile = new ColorAdjustShader_Profile(),
			_miscColorProfile = new ColorAdjustShader_Profile(),
			_setRaceTag = string.Empty,
			_headWidth = 0f,
			_displayBoobs = false,
			_muzzleWeight = 0f,
			_setTexture = 0,
			_voicePitch = 1f,
			_heightWeight = 0f,
			_torsoWeight = 0f,
			_boobWeight = 0f,
			_armWeight = 0f,
			_bellyWeight = 0f,
			_bottomWeight = 0f,
			_setHairStyle = 0,
			_setEar = 0,
			_setEye = 0,
			_setMouth = 0,
			_setTail = 0,
			_setMisc = 0,
			_hairIsBodyColor = false,
			_helmDyeIndex = 0,
			_hideCape = false,
			_hideHelm = false,
			_hideChest = false,
			_hideLeggings = false,
			_hideVanityHelm = false,
			_hideVanityCape = false,
			_hideVanityChest = false,
			_hideVanityLeggings = false,
			_isLeftHanded = false,
			_widthWeight = 0f
		};
		PlayerCasting_SkillsProfile skillsProfile = new PlayerCasting_SkillsProfile
		{
			_setSkillsProfile = new SkillStruct[0],
			_bonusSkillProfile = GameManager._current._staticBonusSkills,
			_setSkillPresetProfile = new SkillPresetProfile()
		};
		QuestProgressProfile questProgressProfile = new QuestProgressProfile
		{
			_questProgressStructs = new QuestProgressStruct[0],
			_finishedQuests = new string[0]
		};
		obj._inventoryProfile = new ItemData[0];
		obj._vanityStruct = default;
		obj._quickItemSlotProfile = new string[0];
		obj._dialogIntroProfile = new string[0];
		obj._waypointAttunementProfile = new string[0];
		obj._statsProfile = statsProfile;
		obj._appearanceProfile = appearanceProfile;
		obj._skillsProfile = skillsProfile;
		obj._questProgressProfile = questProgressProfile;
		obj._isAltWeaponEquipped = false;
		obj._isWeaponLocked = false;
		obj._isNewCharacter = true;
		Cache_CharacterFiles();
		_charSelectManager.Clear_SelectEntries();
		_charSelectManager.Initalize_CharacterFiles(_characterFiles);
		HostConsole._current.Init_NetworkStatusMessage("Character file deleted...");
	}

	public void Save_ItemStorageData()
	{
		if (!Directory.Exists(_dataPath))
		{
			Directory.CreateDirectory(_dataPath);
		}
		if (!File.Exists(_dataPath + "/atl_itemBank"))
		{
			_itemStorageProfile = new ItemStorage_Profile();
		}
		if (!File.Exists(_dataPath + "/atl_itemBank_01"))
		{
			_itemStorageProfile_02 = new ItemStorage_Profile();
		}
		if (!File.Exists(_dataPath + "/atl_itemBank_02"))
		{
			_itemStorageProfile_03 = new ItemStorage_Profile();
		}
		if (!File.Exists(_dataPath + "/atl_itemBank_03"))
		{
			_itemStorageProfile_04 = new ItemStorage_Profile();
		}
		if (!File.Exists(_dataPath + "/atl_itemBank_04"))
		{
			_itemStorageProfile_05 = new ItemStorage_Profile();
		}
		if (!File.Exists(_dataPath + "/atl_itemBank_05"))
		{
			_itemStorageProfile_06 = new ItemStorage_Profile();
		}
		if (!File.Exists(_dataPath + "/atl_itemBank_06"))
		{
			_itemStorageProfile_07 = new ItemStorage_Profile();
		}
		if (!File.Exists(_dataPath + "/atl_itemBank_07"))
		{
			_itemStorageProfile_08 = new ItemStorage_Profile();
		}
		_itemStorageProfile._heldItemStorage = ItemStorageManager._current._itemDatas.ToArray();
		_itemStorageProfile_02._heldItemStorage = ItemStorageManager._current._itemDatas_01.ToArray();
		_itemStorageProfile_03._heldItemStorage = ItemStorageManager._current._itemDatas_02.ToArray();
		_itemStorageProfile_04._heldItemStorage = ItemStorageManager._current._itemDatas_03.ToArray();
		_itemStorageProfile_05._heldItemStorage = ItemStorageManager._current._itemDatas_04.ToArray();
		_itemStorageProfile_06._heldItemStorage = ItemStorageManager._current._itemDatas_05.ToArray();
		_itemStorageProfile_07._heldItemStorage = ItemStorageManager._current._itemDatas_06.ToArray();
		_itemStorageProfile_08._heldItemStorage = ItemStorageManager._current._itemDatas_07.ToArray();
		string contents = JsonUtility.ToJson(_itemStorageProfile, prettyPrint: true);
		string contents2 = JsonUtility.ToJson(_itemStorageProfile_02, prettyPrint: true);
		string contents3 = JsonUtility.ToJson(_itemStorageProfile_03, prettyPrint: true);
		string contents4 = JsonUtility.ToJson(_itemStorageProfile_04, prettyPrint: true);
		string contents5 = JsonUtility.ToJson(_itemStorageProfile_05, prettyPrint: true);
		string contents6 = JsonUtility.ToJson(_itemStorageProfile_06, prettyPrint: true);
		string contents7 = JsonUtility.ToJson(_itemStorageProfile_07, prettyPrint: true);
		string contents8 = JsonUtility.ToJson(_itemStorageProfile_08, prettyPrint: true);
		File.WriteAllText(_dataPath + "/atl_itemBank", contents);
		File.WriteAllText(_dataPath + "/atl_itemBank_01", contents2);
		File.WriteAllText(_dataPath + "/atl_itemBank_02", contents3);
		File.WriteAllText(_dataPath + "/atl_itemBank_03", contents4);
		File.WriteAllText(_dataPath + "/atl_itemBank_04", contents5);
		File.WriteAllText(_dataPath + "/atl_itemBank_05", contents6);
		File.WriteAllText(_dataPath + "/atl_itemBank_06", contents7);
		File.WriteAllText(_dataPath + "/atl_itemBank_07", contents8);
	}

	public void Load_ItemStorageData()
	{
		if (File.Exists(_dataPath + "/atl_itemBank"))
		{
			ItemStorage_Profile itemStorageProfile = JsonUtility.FromJson<ItemStorage_Profile>(File.ReadAllText(_dataPath + "/atl_itemBank"));
			_itemStorageProfile = itemStorageProfile;
		}
		if (File.Exists(_dataPath + "/atl_itemBank_01"))
		{
			ItemStorage_Profile itemStorageProfile_ = JsonUtility.FromJson<ItemStorage_Profile>(File.ReadAllText(_dataPath + "/atl_itemBank_01"));
			_itemStorageProfile_02 = itemStorageProfile_;
		}
		if (File.Exists(_dataPath + "/atl_itemBank_02"))
		{
			ItemStorage_Profile itemStorageProfile_2 = JsonUtility.FromJson<ItemStorage_Profile>(File.ReadAllText(_dataPath + "/atl_itemBank_02"));
			_itemStorageProfile_03 = itemStorageProfile_2;
		}
		if (File.Exists(_dataPath + "/atl_itemBank_03"))
		{
			ItemStorage_Profile itemStorageProfile_3 = JsonUtility.FromJson<ItemStorage_Profile>(File.ReadAllText(_dataPath + "/atl_itemBank_03"));
			_itemStorageProfile_04 = itemStorageProfile_3;
		}
		if (File.Exists(_dataPath + "/atl_itemBank_04"))
		{
			ItemStorage_Profile itemStorageProfile_4 = JsonUtility.FromJson<ItemStorage_Profile>(File.ReadAllText(_dataPath + "/atl_itemBank_04"));
			_itemStorageProfile_05 = itemStorageProfile_4;
		}
		if (File.Exists(_dataPath + "/atl_itemBank_05"))
		{
			ItemStorage_Profile itemStorageProfile_5 = JsonUtility.FromJson<ItemStorage_Profile>(File.ReadAllText(_dataPath + "/atl_itemBank_05"));
			_itemStorageProfile_06 = itemStorageProfile_5;
		}
		if (File.Exists(_dataPath + "/atl_itemBank_06"))
		{
			ItemStorage_Profile itemStorageProfile_6 = JsonUtility.FromJson<ItemStorage_Profile>(File.ReadAllText(_dataPath + "/atl_itemBank_06"));
			_itemStorageProfile_07 = itemStorageProfile_6;
		}
		if (File.Exists(_dataPath + "/atl_itemBank_07"))
		{
			ItemStorage_Profile itemStorageProfile_7 = JsonUtility.FromJson<ItemStorage_Profile>(File.ReadAllText(_dataPath + "/atl_itemBank_07"));
			_itemStorageProfile_08 = itemStorageProfile_7;
		}
		ItemStorageManager current = ItemStorageManager._current;
		if ((bool)current)
		{
			current._storageSizes[0] = 0;
			current._storageSizes[1] = 0;
			current._storageSizes[2] = 0;
			current._storageSizes_01[0] = 0;
			current._storageSizes_01[1] = 0;
			current._storageSizes_01[2] = 0;
			current._storageSizes_02[0] = 0;
			current._storageSizes_02[1] = 0;
			current._storageSizes_02[2] = 0;
			current._storageSizes_03[0] = 0;
			current._storageSizes_03[1] = 0;
			current._storageSizes_03[2] = 0;
			current._storageSizes_04[0] = 0;
			current._storageSizes_04[1] = 0;
			current._storageSizes_04[2] = 0;
			current._storageSizes_05[0] = 0;
			current._storageSizes_05[1] = 0;
			current._storageSizes_05[2] = 0;
			current._storageSizes_06[0] = 0;
			current._storageSizes_06[1] = 0;
			current._storageSizes_06[2] = 0;
			current._storageSizes_07[0] = 0;
			current._storageSizes_07[1] = 0;
			current._storageSizes_07[2] = 0;
			current._itemDatas.Clear();
			current._itemDatas_01.Clear();
			current._itemDatas_02.Clear();
			current._itemDatas_03.Clear();
			current._itemDatas_04.Clear();
			current._itemDatas_05.Clear();
			current._itemDatas_06.Clear();
			current._itemDatas_07.Clear();
			current._itemDatas.AddRange(_itemStorageProfile._heldItemStorage);
			current._itemDatas_01.AddRange(_itemStorageProfile_02._heldItemStorage);
			current._itemDatas_02.AddRange(_itemStorageProfile_03._heldItemStorage);
			current._itemDatas_03.AddRange(_itemStorageProfile_04._heldItemStorage);
			current._itemDatas_04.AddRange(_itemStorageProfile_05._heldItemStorage);
			current._itemDatas_05.AddRange(_itemStorageProfile_06._heldItemStorage);
			current._itemDatas_06.AddRange(_itemStorageProfile_07._heldItemStorage);
			current._itemDatas_07.AddRange(_itemStorageProfile_08._heldItemStorage);
		}
	}

	public void Save_HostSettingsData()
	{
		if (!Directory.Exists(_hostSettingsDataPath))
		{
			Directory.CreateDirectory(_hostSettingsDataPath);
		}
		string contents = JsonUtility.ToJson(new ServerHostSettings_Profile
		{
			_globalNickname = MainMenuManager._current._characterSelectManager._globalNickanmeInput.text,
			_serverName = AtlyssNetworkManager._current._serverName,
			_serverPassword = AtlyssNetworkManager._current._serverPassword,
			_serverMotd = AtlyssNetworkManager._current._serverMotd,
			_maxAllowedConnections = AtlyssNetworkManager._current.maxConnections,
			_banList = AtlyssNetworkManager._current._bannedClientList.ToArray(),
			_mutedList = AtlyssNetworkManager._current._mutedClientList.ToArray(),
			_streamMode = LobbyListManager._current._streamModeToggle.isOn,
			_hiddenLobbies = LobbyListManager._current._lobbyBlacklist.ToArray()
		}, prettyPrint: true);
		File.WriteAllText(_hostSettingsDataPath + "/hostSettings.json", contents);
		Load_HostSettingsData();
	}

	public void Load_HostSettingsData()
	{
		if (File.Exists(_hostSettingsDataPath + "/hostSettings.json"))
		{
			string json = File.ReadAllText(_hostSettingsDataPath + "/hostSettings.json");
			_hostSettingsProfile = JsonUtility.FromJson<ServerHostSettings_Profile>(json);
			if (string.IsNullOrEmpty(_hostSettingsProfile._serverName) || _hostSettingsProfile._serverName.Contains('<') || _hostSettingsProfile._serverName.Contains('>') || _hostSettingsProfile._serverName.Length > 20)
			{
				_hostSettingsProfile._serverName = "ATLYSS Lobby";
			}
			if (_hostSettingsProfile._maxAllowedConnections < 2)
			{
				_hostSettingsProfile._maxAllowedConnections = 2;
			}
			else if (_hostSettingsProfile._maxAllowedConnections > 16)
			{
				_hostSettingsProfile._maxAllowedConnections = 16;
			}
			MainMenuManager._current.Load_HostSettings();
			AtlyssNetworkManager._current._serverName = _hostSettingsProfile._serverName;
			AtlyssNetworkManager._current._serverPassword = _hostSettingsProfile._serverPassword;
			AtlyssNetworkManager._current._serverMotd = _hostSettingsProfile._serverMotd;
			AtlyssNetworkManager._current.maxConnections = _hostSettingsProfile._maxAllowedConnections;
			LobbyListManager._current._lobbyBlacklist.Clear();
			if (_hostSettingsProfile._hiddenLobbies != null && _hostSettingsProfile._hiddenLobbies.Length != 0)
			{
				LobbyListManager._current._lobbyBlacklist.AddRange(_hostSettingsProfile._hiddenLobbies);
			}
		}
	}

	public void Save_TutorialData()
	{
		if (!Directory.Exists(_dataPath))
		{
			Directory.CreateDirectory(_dataPath);
		}
		string contents = JsonUtility.ToJson(new Tutorial_Profile
		{
			_tutorialTags = TutorialManager._current._finishedTutorialTags.ToArray()
		}, prettyPrint: true);
		File.WriteAllText(_dataPath + "/tutorialProfile.json", contents);
		Load_HostSettingsData();
	}

	public void Load_TutorialData()
	{
		if (File.Exists(_dataPath + "/tutorialProfile.json"))
		{
			string json = File.ReadAllText(_dataPath + "/tutorialProfile.json");
			_tutorialProfile = JsonUtility.FromJson<Tutorial_Profile>(json);
			for (int i = 0; i < _tutorialProfile._tutorialTags.Length; i++)
			{
				TutorialManager._current._finishedTutorialTags.Add(_tutorialProfile._tutorialTags[i]);
			}
		}
	}
}
