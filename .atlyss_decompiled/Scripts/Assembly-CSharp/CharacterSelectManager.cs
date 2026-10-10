using System.Collections;
using System.Collections.Generic;
using Steamworks;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectManager : MonoBehaviour
{
	private MainMenuManager _mainMenuManager;

	private CharacterCreationManager _characterCreationManager;

	[Header("Character Select Manager")]
	[SerializeField]
	private GameObject _characterSelectPrefab;

	[SerializeField]
	private Transform _characterSelectListContent;

	private readonly List<CharacterSelectListDataEntry> _characterSelectEntries = new List<CharacterSelectListDataEntry>();

	public bool _cachedSteamLobby;

	public bool _isSendingCharacterFile;

	[SerializeField]
	private Text _pageTextCounter;

	[SerializeField]
	private Button _previousSlotPageButton;

	[SerializeField]
	private Button _nextSlotPageButton;

	private int _selectedSlotPage;

	[Header("Character Select / Creation Camera")]
	[SerializeField]
	private Transform _baseCamTransform;

	public Transform _mainCamTransform;

	[Space]
	[SerializeField]
	private Vector3 _defaultCameraRotation = Vector3.zero;

	public Vector3 _defaultCameraPosition = Vector3.zero;

	[Space]
	[SerializeField]
	private float _cameraSensitivity = 12.25f;

	private bool _loadCameraEffect;

	[Header("UI")]
	public InputField _globalNickanmeInput;

	public GameObject _characterSelectCanvasObject;

	[SerializeField]
	private Button _enterGameButton;

	[SerializeField]
	private Text _enterGameButtonText;

	[SerializeField]
	private Button _returnButton;

	[SerializeField]
	private Button _characterDeleteButton;

	[SerializeField]
	private Text _gameModeTypeHeader;

	[Header("Character Delete Prompt")]
	[SerializeField]
	private MenuElement _deleteCharacterPromptElement;

	[SerializeField]
	private InputField _characterDeleteConfirmInput;

	[SerializeField]
	private Button _characterDeleteConfirmButton;

	public bool _deleteCharacterPrompt;

	[Header("Character Display")]
	[SerializeField]
	private GameObject _initSelectEffectObject;

	private RaceModelEquipDisplay _setRaceModelED;

	[Header("Gamepad")]
	[SerializeField]
	private Image _gamepadIco_selectFile;

	[SerializeField]
	private Image _gamepadIco_return;

	[SerializeField]
	private Image _gamepadIco_previousSlotPage;

	[SerializeField]
	private Image _gamepadIco_nextSlotPage;

	[SerializeField]
	private Image _gamepadIco_deleteCharacterButton;

	[SerializeField]
	private Image _gamepadIco_globalNicknameInput;

	[Space]
	[SerializeField]
	private Image _gamepadIco_inputDeletePrompt;

	[SerializeField]
	private Image _gamepadIco_confirmDeletePrompt;

	[SerializeField]
	private Image _gamepadIco_returnDeletePrompt;

	[Header("Audio")]
	[SerializeField]
	private AudioSource _musicAsrc;

	[SerializeField]
	private AudioSource _aSrc_clickSelect;

	[SerializeField]
	private AudioSource _aSrc_confirmSelect;

	public int SelectedSlotPage
	{
		get
		{
			return _selectedSlotPage;
		}
		set
		{
			if (value < 0)
			{
				value = 14;
			}
			else if (value > 14)
			{
				value = 0;
			}
			_selectedSlotPage = value;
			_characterSelectListContent.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 0f + 609f * (float)_selectedSlotPage);
			ProfileDataManager current = ProfileDataManager._current;
			int fileIndex = (ProfileDataManager._current.SelectedFileIndex = 7 * _selectedSlotPage);
			current.Set_FileIndex(fileIndex);
			_aSrc_clickSelect.Play();
		}
	}

	private void Awake()
	{
		_mainMenuManager = GetComponent<MainMenuManager>();
		_characterCreationManager = GetComponent<CharacterCreationManager>();
		_enterGameButton.onClick.AddListener(Select_CharacterFile);
		_previousSlotPageButton.onClick.AddListener(Init_PreviousSlotPage);
		_nextSlotPageButton.onClick.AddListener(Init_NextSlotPage);
	}

	private void Update()
	{
		if (_isSendingCharacterFile)
		{
			_musicAsrc.volume = Mathf.Lerp(_musicAsrc.volume, 0f, 3.5f * Time.deltaTime);
			for (int i = 0; i < _characterCreationManager._raceDisplayModels.Length; i++)
			{
				if (_characterCreationManager._raceDisplayModels[i].isActiveAndEnabled)
				{
					_characterCreationManager._raceDisplayModels[i]._raceAnimator.CrossFade("Idle", 0f, 0);
				}
			}
		}
		Handle_CameraControl();
		Handle_CharacterSelectControl();
		Handle_EquipDisplays();
		Handle_HeaderText();
		_pageTextCounter.text = $"Page ( {_selectedSlotPage + 1} / 15 )";
	}

	private void Handle_HeaderText()
	{
		_globalNickanmeInput.gameObject.SetActive(_mainMenuManager._networkInitCondition != NetworkInitCondition.Singleplayer);
		switch (_mainMenuManager._networkInitCondition)
		{
		case NetworkInitCondition.Singleplayer:
			_gameModeTypeHeader.text = "Singleplayer";
			break;
		case NetworkInitCondition.Join_Multiplayer:
			_gameModeTypeHeader.text = "Join Game";
			break;
		case NetworkInitCondition.Host_Multiplayer:
			if (LobbyListManager._current._lobbyTypeDropdown.value == 0)
			{
				_gameModeTypeHeader.text = "Host Game (Public)";
			}
			else if (LobbyListManager._current._lobbyTypeDropdown.value == 1)
			{
				_gameModeTypeHeader.text = "Host Game (Friends)";
			}
			else if (LobbyListManager._current._lobbyTypeDropdown.value == 2)
			{
				_gameModeTypeHeader.text = "Host Game (Private)";
			}
			break;
		}
		if (_cachedSteamLobby)
		{
			_gameModeTypeHeader.text = "Lobby Connect";
			_mainMenuManager._networkInitCondition = NetworkInitCondition.Join_Multiplayer;
		}
	}

	public void Reset_CameraRotation()
	{
		_baseCamTransform.eulerAngles = _defaultCameraRotation;
		_mainCamTransform.localPosition = _defaultCameraPosition;
	}

	private void Handle_CameraControl()
	{
		if (_mainMenuManager._mainMenuCondition == MainMenuCondition.In_Game || (_mainMenuManager._mainMenuCondition != MainMenuCondition.CharacterSelect && MainMenuManager._current._mainMenuCondition != MainMenuCondition.CharacterCreation) || LobbyListManager._current.lobbyListWindow_element.isEnabled)
		{
			return;
		}
		InputControlManager current = InputControlManager.current;
		if (_loadCameraEffect)
		{
			_mainCamTransform.localRotation = Quaternion.Lerp(_mainCamTransform.localRotation, Quaternion.Euler(new Vector3(-80.12f, 180f, 0f)), 3.65f * Time.deltaTime);
		}
		if (InputControlManager.current._setGamepadType != GamepadType.Keyboard && InputControlManager.current._gamepadMouseCursorBuffer <= 0f)
		{
			float num = GamepadInput.GetAxis(GamepadAxis.RightX) * 0.3f;
			if (num != 0f)
			{
				_baseCamTransform.Rotate(new Vector3(0f, num * _cameraSensitivity, 0f));
				Cursor.visible = false;
				Cursor.lockState = CursorLockMode.Locked;
			}
			else
			{
				Cursor.visible = current._gamepadMouseCursorBuffer > 0f;
			}
			if (GamepadInput.GetButtonDown(GamepadButton.RightStick))
			{
				Reset_CameraRotation();
			}
		}
		else
		{
			float altHoriz_input = current._altHoriz_input;
			if (Input.GetKey(KeyCode.Mouse1))
			{
				_baseCamTransform.Rotate(new Vector3(0f, altHoriz_input * _cameraSensitivity, 0f));
				Cursor.visible = false;
				Cursor.lockState = CursorLockMode.Locked;
			}
			else
			{
				Cursor.visible = true;
				Cursor.lockState = CursorLockMode.None;
			}
		}
	}

	private void Handle_CharacterSelectControl()
	{
		if (_mainMenuManager._mainMenuCondition == MainMenuCondition.CharacterSelect && !LobbyListManager._current._inDisclaimerMode && !LobbyListManager._current._isPasswordInput && !_isSendingCharacterFile)
		{
			Handle_ButtonControl();
		}
		void Handle_ButtonControl()
		{
			if (ProfileDataManager._current._characterFile._isEmptySlot)
			{
				_enterGameButtonText.text = "Create Character";
			}
			else
			{
				_enterGameButtonText.text = "Select Character";
			}
			bool flag = InputControlManager.current._setGamepadType != GamepadType.Keyboard;
			_gamepadIco_selectFile.enabled = flag && !_deleteCharacterPrompt;
			_gamepadIco_return.enabled = flag && !_deleteCharacterPrompt;
			_gamepadIco_previousSlotPage.enabled = flag && !_deleteCharacterPrompt;
			_gamepadIco_nextSlotPage.enabled = flag && !_deleteCharacterPrompt;
			_gamepadIco_deleteCharacterButton.enabled = flag && !_deleteCharacterPrompt && _characterDeleteButton.interactable;
			_gamepadIco_globalNicknameInput.enabled = flag && !_deleteCharacterPrompt && _globalNickanmeInput.gameObject.activeInHierarchy;
			_gamepadIco_inputDeletePrompt.enabled = flag;
			_gamepadIco_confirmDeletePrompt.enabled = flag;
			_gamepadIco_returnDeletePrompt.enabled = flag;
			if (GamepadInput.GetButton(GamepadButton.LeftShoulder))
			{
				_gamepadIco_previousSlotPage.color = Color.white;
			}
			else
			{
				_gamepadIco_previousSlotPage.color = Color.grey;
			}
			if (GamepadInput.GetButton(GamepadButton.RightShoulder))
			{
				_gamepadIco_nextSlotPage.color = Color.white;
			}
			else
			{
				_gamepadIco_nextSlotPage.color = Color.grey;
			}
			_previousSlotPageButton.interactable = !_deleteCharacterPrompt;
			_nextSlotPageButton.interactable = !_deleteCharacterPrompt;
			if (_deleteCharacterPrompt)
			{
				MainMenuManager._current._menuInputBuffer = 0.2f;
				_characterDeleteButton.interactable = false;
				_enterGameButton.interactable = false;
				_returnButton.interactable = false;
				bool buttonDown = GamepadInput.GetButtonDown(GamepadButton.Start);
				bool flag2 = _characterDeleteConfirmInput.text == ProfileDataManager._current._characterFile._nickName;
				bool flag3 = (Input.GetKeyDown(KeyCode.Return) && !flag) || (GamepadInput.GetButtonDown(GamepadButton.Button_South) & flag);
				bool num = Input.GetKeyDown(KeyCode.Escape) || (GamepadInput.GetButtonDown(GamepadButton.Button_East) & flag);
				_characterDeleteConfirmButton.interactable = flag2;
				if (num)
				{
					MainMenuManager._current._menuAcceptAsrc.Play();
					Init_DeleteCharacterPrompt(isTrue: false);
				}
				if (buttonDown & flag)
				{
					if (!_characterDeleteConfirmInput.IsActive())
					{
						_characterDeleteConfirmInput.ActivateInputField();
					}
					if (SteamManager.Initialized && (SteamUtils.IsSteamRunningOnSteamDeck() || SteamUtils.IsSteamInBigPictureMode()))
					{
						SteamUtils.ShowFloatingGamepadTextInput(EFloatingGamepadTextInputMode.k_EFloatingGamepadTextInputModeModeSingleLine, 0, 0, 500, 200);
					}
				}
				if (flag3)
				{
					if (flag2)
					{
						Init_DeleteCharacter();
						Init_DeleteCharacterPrompt(isTrue: false);
					}
					else
					{
						HostConsole._current.Init_NetworkStatusMessage("Enter the correct character name to delete.");
					}
				}
			}
			else
			{
				if (_characterDeleteConfirmInput.IsActive())
				{
					_characterDeleteConfirmInput.DeactivateInputField();
				}
				Handle_SelectionControl();
				for (int i = 0; i < _characterSelectEntries.Count; i++)
				{
					if (!_characterSelectEntries[i]._characterFileSelectButton.interactable)
					{
						_characterSelectEntries[i]._characterFileSelectButton.interactable = true;
					}
				}
				_characterDeleteConfirmInput.text = string.Empty;
				_characterDeleteButton.interactable = !ProfileDataManager._current._characterFile._isEmptySlot;
				_enterGameButton.interactable = true;
				_returnButton.interactable = true;
			}
		}
		void Handle_SelectionControl()
		{
			if (!LobbyListManager._current._lobbyHostWindow_element.isEnabled && !LobbyListManager._current.lobbyListWindow_element.isEnabled && !LobbyListManager._current._selectedLobby && !(MainMenuManager._current._menuInputBuffer > 0f) && !_isSendingCharacterFile && !_deleteCharacterPrompt)
			{
				bool num = InputControlManager.current.DpadUp(DpadHoldType.OnDown) || GamepadInput.GetButtonDown(GamepadButton.LeftStick_Up);
				bool flag = InputControlManager.current.DpadDown(DpadHoldType.OnDown) || GamepadInput.GetButtonDown(GamepadButton.LeftStick_Down);
				bool flag2 = InputControlManager.current.DpadLeft(DpadHoldType.OnDown) || GamepadInput.GetButtonDown(GamepadButton.LeftShoulder);
				bool flag3 = InputControlManager.current.DpadRight(DpadHoldType.OnDown) || GamepadInput.GetButtonDown(GamepadButton.RightShoulder);
				if (num)
				{
					Init_SelectFile(-1);
					_globalNickanmeInput.DeactivateInputField();
				}
				if (flag)
				{
					Init_SelectFile(1);
					_globalNickanmeInput.DeactivateInputField();
				}
				if (flag2)
				{
					SelectedSlotPage--;
					_globalNickanmeInput.DeactivateInputField();
					MainMenuManager._current._menuInputBuffer = 0.12f;
				}
				if (flag3)
				{
					SelectedSlotPage++;
					_globalNickanmeInput.DeactivateInputField();
					MainMenuManager._current._menuInputBuffer = 0.12f;
				}
				if (_globalNickanmeInput.isFocused)
				{
					MainMenuManager._current._menuInputBuffer = 0.4f;
				}
				if (MainMenuManager._current._menuInputBuffer <= 0f)
				{
					bool num2 = Input.GetKeyDown(KeyCode.Return) || GamepadInput.GetButtonDown(GamepadButton.Button_South);
					bool flag4 = _characterDeleteButton.interactable && (Input.GetKeyDown(KeyCode.Delete) || GamepadInput.GetButtonDown(GamepadButton.Button_North));
					bool flag5 = GamepadInput.GetButtonDown(GamepadButton.Start) && _globalNickanmeInput.gameObject.activeInHierarchy;
					bool flag6 = Input.GetKeyDown(KeyCode.Escape) || GamepadInput.GetButtonDown(GamepadButton.Button_East);
					if (num2)
					{
						Select_CharacterFile();
						MainMenuManager._current._menuInputBuffer = 0.12f;
						_globalNickanmeInput.DeactivateInputField();
					}
					if (flag4)
					{
						MainMenuManager._current._menuDeclineAsrc.Play();
						Init_DeleteCharacterPrompt(isTrue: true);
						_globalNickanmeInput.DeactivateInputField();
					}
					if (flag5)
					{
						_globalNickanmeInput.ActivateInputField();
						if (SteamManager.Initialized && (SteamUtils.IsSteamRunningOnSteamDeck() || SteamUtils.IsSteamInBigPictureMode()))
						{
							SteamUtils.ShowFloatingGamepadTextInput(EFloatingGamepadTextInputMode.k_EFloatingGamepadTextInputModeModeSingleLine, 0, 0, 500, 200);
						}
					}
					if (flag6)
					{
						MainMenuManager._current.Set_MenuCondition(0);
						_globalNickanmeInput.DeactivateInputField();
					}
				}
			}
		}
	}

	public void Init_NextSlotPage()
	{
		SelectedSlotPage++;
	}

	public void Init_PreviousSlotPage()
	{
		SelectedSlotPage--;
	}

	private void Init_SelectFile(int _i)
	{
		ProfileDataManager current = ProfileDataManager._current;
		if ((_i != -1 || (ProfileDataManager._current.SelectedFileIndex != 0 && ProfileDataManager._current.SelectedFileIndex % 7 != 0)) && (_i != 1 || ProfileDataManager._current.SelectedFileIndex != 7 * (_selectedSlotPage + 1) - 1))
		{
			current.Set_FileIndex(ProfileDataManager._current.SelectedFileIndex += _i);
			_aSrc_clickSelect.Play();
		}
	}

	public void Apply_CharacterSelectDisplay()
	{
		if (_mainMenuManager._mainMenuCondition != MainMenuCondition.CharacterSelect)
		{
			return;
		}
		ProfileDataManager current = ProfileDataManager._current;
		PlayerAppearance_Profile _pap = current._characterFile._appearanceProfile;
		CharacterCreationManager _ccm = GetComponent<CharacterCreationManager>();
		for (int i = 0; i < _ccm._raceDisplayModels.Length; i++)
		{
			_ccm._raceDisplayModels[i].gameObject.SetActive(value: false);
		}
		for (int j = 0; j < _ccm._scriptablePlayerRaces.Length; j++)
		{
			if (current._characterFile._appearanceProfile._setRaceTag == _ccm._scriptablePlayerRaces[j]._raceName)
			{
				_ccm._raceDisplayModels[j].gameObject.SetActive(value: true);
				Init_Display(_ccm._raceDisplayModels[j]);
			}
		}
		void Init_Display(PlayerRaceModel _setRaceModel)
		{
			PlayerAppearance_Profile _newPA = new PlayerAppearance_Profile();
			_setRaceModelED = _setRaceModel.GetComponent<RaceModelEquipDisplay>();
			_newPA = _setRaceModel._scriptablePlayerRace.Init_ParamsCheck(_pap);
			_setRaceModel._isCharacterCreationDisplay = false;
			Set_CharacterWeights();
			Set_CharacterColors();
			Set_CharacterParts();
			_setRaceModel.Apply_CharacterDisplay();
			_ccm._characterRefreshEffect.Play();
			_ccm._characterRefreshSound.Stop();
			_ccm._characterRefreshSound.Play();
			_setRaceModelED.Remove_HelmDisplay();
			_setRaceModelED.Remove_ChestPieceDisplay();
			_setRaceModelED.Remove_LeggingsDisplay();
			_setRaceModelED.Remove_CapeDisplay();
			_setRaceModelED.Remove_WeaponDisplay();
			_setRaceModelED.Remove_ShieldDisplay();
			void Set_CharacterColors()
			{
				_setRaceModel.SetTextureIndex = _pap._setTexture;
				_setRaceModel._setSkinColorProfile._hue = _newPA._setSkinColorProfile._hue;
				_setRaceModel._setSkinColorProfile._brightness = _newPA._setSkinColorProfile._brightness;
				_setRaceModel._setSkinColorProfile._contrast = _newPA._setSkinColorProfile._contrast;
				_setRaceModel._setSkinColorProfile._saturation = _newPA._setSkinColorProfile._saturation;
				_setRaceModel._setHairColorProfile._hue = _newPA._hairColorProfile._hue;
				_setRaceModel._setHairColorProfile._brightness = _newPA._hairColorProfile._brightness;
				_setRaceModel._setHairColorProfile._contrast = _newPA._hairColorProfile._contrast;
				_setRaceModel._setHairColorProfile._saturation = _newPA._hairColorProfile._saturation;
				_setRaceModel._hairHueIsBodyHue = _newPA._hairIsBodyColor;
				_setRaceModel._setMiscColorProfile._hue = _newPA._miscColorProfile._hue;
				_setRaceModel._setMiscColorProfile._brightness = _newPA._miscColorProfile._brightness;
				_setRaceModel._setMiscColorProfile._contrast = _newPA._miscColorProfile._contrast;
				_setRaceModel._setMiscColorProfile._saturation = _newPA._miscColorProfile._saturation;
			}
			void Set_CharacterParts()
			{
				_setRaceModel.SetEarIndex = _pap._setEar;
				_setRaceModel.SetMiscIndex = _pap._setMisc;
				_setRaceModel.SetTailIndex = _pap._setTail;
				_setRaceModel.SetHairIndex = _pap._setHairStyle;
				_setRaceModel.SetEyeIndex = _pap._setEye;
				_setRaceModel.SetMouthIndex = _pap._setMouth;
				_setRaceModel.Reset_Expression();
			}
			void Set_CharacterWeights()
			{
				_setRaceModel._armatureTransform.localScale = new Vector3(_pap._widthWeight, _newPA._heightWeight, 1f);
				_setRaceModel._headBoneTransform.localScale = new Vector3(_pap._headWidth, 1f, 1f);
				_setRaceModel._muzzleWeight = _newPA._muzzleWeight;
				_setRaceModel._bellyWeight = _newPA._bellyWeight;
				_setRaceModel._bottomWeight = _newPA._bottomWeight;
				_setRaceModel._torsoWeight = _newPA._torsoWeight;
				_setRaceModel._armWeight = _newPA._armWeight;
				_setRaceModel._boobWeight = _newPA._boobWeight;
				_setRaceModel._displayBoobs = _pap._displayBoobs;
				int index = 4;
				if (_setRaceModelED._hasHeadBlendWeights)
				{
					index = 5;
				}
				if (_setRaceModel._bottomWeight < 0f)
				{
					_setRaceModel._baseBodyMesh.SetBlendShapeWeight(1, 0f);
					_setRaceModel._baseBodyMesh.SetBlendShapeWeight(index, _setRaceModel._bottomWeight * -1f);
				}
				else
				{
					_setRaceModel._baseBodyMesh.SetBlendShapeWeight(index, 0f);
				}
				if (_pap._isLeftHanded)
				{
					_setRaceModel.transform.parent.localScale = new Vector3(-1f, 1f, 1f);
				}
				else
				{
					_setRaceModel.transform.parent.localScale = Vector3.one;
				}
			}
		}
	}

	private void Handle_EquipDisplays()
	{
		if (MainMenuManager._current._mainMenuCondition == MainMenuCondition.CharacterCreation || !_setRaceModelED)
		{
			return;
		}
		ProfileDataManager current = ProfileDataManager._current;
		PlayerAppearance_Profile appearanceProfile = current._characterFile._appearanceProfile;
		Animator componentInChildren = _setRaceModelED.GetComponentInChildren<Animator>();
		bool flag = false;
		ScriptableWeapon scriptableWeapon = null;
		bool flag2 = !appearanceProfile._hideVanityHelm && !string.IsNullOrEmpty(current._characterFile._vanityStruct._helmTag);
		bool flag3 = !appearanceProfile._hideVanityCape && !string.IsNullOrEmpty(current._characterFile._vanityStruct._capeTag);
		bool flag4 = !appearanceProfile._hideVanityChest && !string.IsNullOrEmpty(current._characterFile._vanityStruct._chestTag);
		bool flag5 = !appearanceProfile._hideVanityLeggings && !string.IsNullOrEmpty(current._characterFile._vanityStruct._leggingTag);
		for (int i = 0; i < current._characterFile._inventoryProfile.Length; i++)
		{
			if (!current._characterFile._inventoryProfile[i]._isEquipped)
			{
				continue;
			}
			ScriptableItem scriptableItem = GameManager._current.Locate_Item(current._characterFile._inventoryProfile[i]._itemName);
			if (!scriptableItem)
			{
				continue;
			}
			if (scriptableItem._itemType == ItemType.GEAR)
			{
				ScriptableEquipment scriptableEquipment = (ScriptableEquipment)scriptableItem;
				if (scriptableEquipment._equipType == ItemEquipType.ARMOR)
				{
					ScriptableArmor scriptableArmor = (ScriptableArmor)scriptableEquipment;
					if ((scriptableArmor.GetType() == typeof(ScriptableHelm) && (appearanceProfile._hideHelm || flag2)) || (scriptableArmor.GetType() == typeof(ScriptableCape) && (appearanceProfile._hideCape || flag3)) || (scriptableArmor.GetType() == typeof(ScriptableChestpiece) && (appearanceProfile._hideChest || flag4)) || (scriptableArmor.GetType() == typeof(ScriptableLeggings) && (appearanceProfile._hideLeggings || flag5)))
					{
						continue;
					}
					_setRaceModelED.Apply_ArmorDisplay(scriptableArmor);
				}
			}
			if (_isSendingCharacterFile)
			{
				continue;
			}
			if (scriptableItem.GetType() == typeof(ScriptableShield))
			{
				ScriptableShield scriptShield = (ScriptableShield)scriptableItem;
				if (!string.IsNullOrEmpty(current._characterFile._vanityStruct._shieldTag))
				{
					scriptShield = (ScriptableShield)GameManager._current.Locate_Item(current._characterFile._vanityStruct._shieldTag);
				}
				_setRaceModelED.Apply_ShieldDisplay(scriptShield, current._characterFile._inventoryProfile[i]._modifierID);
				_setRaceModelED.Display_TakenOutShield(current._characterFile._inventoryProfile[i]._modifierID);
				if (!flag)
				{
					flag = true;
				}
			}
			if (scriptableItem.GetType() == typeof(ScriptableWeapon))
			{
				if (!current._characterFile._isWeaponLocked)
				{
					if (current._characterFile._inventoryProfile[i]._isAltWeapon && current._characterFile._isAltWeaponEquipped)
					{
						scriptableWeapon = (ScriptableWeapon)scriptableItem;
						_setRaceModelED.Apply_WeaponDisplay(scriptableWeapon, current._characterFile._inventoryProfile[i]._modifierID);
					}
					if (!current._characterFile._inventoryProfile[i]._isAltWeapon && !current._characterFile._isAltWeaponEquipped)
					{
						scriptableWeapon = (ScriptableWeapon)scriptableItem;
						_setRaceModelED.Apply_WeaponDisplay(scriptableWeapon, current._characterFile._inventoryProfile[i]._modifierID);
					}
				}
				else if (!current._characterFile._inventoryProfile[i]._isAltWeapon)
				{
					scriptableWeapon = (ScriptableWeapon)scriptableItem;
					_setRaceModelED.Apply_WeaponDisplay(scriptableWeapon, current._characterFile._inventoryProfile[i]._modifierID);
				}
			}
			if (!scriptableWeapon)
			{
				continue;
			}
			if (scriptableWeapon.weaponType._weaponHandedness == WeaponHandedness.Two_Handed)
			{
				if (componentInChildren.isActiveAndEnabled)
				{
					componentInChildren.CrossFade("Hold_None", 0f, 4);
				}
				_setRaceModelED.Remove_ShieldDisplay();
			}
			else if (componentInChildren.isActiveAndEnabled & flag)
			{
				componentInChildren.CrossFade("Hold_Shield", 0f, 4);
			}
		}
		if (flag2)
		{
			_setRaceModelED.Apply_ArmorDisplay((ScriptableArmor)GameManager._current.Locate_Item(current._characterFile._vanityStruct._helmTag));
		}
		if (flag3)
		{
			_setRaceModelED.Apply_ArmorDisplay((ScriptableArmor)GameManager._current.Locate_Item(current._characterFile._vanityStruct._capeTag));
		}
		if (flag4)
		{
			_setRaceModelED.Apply_ArmorDisplay((ScriptableArmor)GameManager._current.Locate_Item(current._characterFile._vanityStruct._chestTag));
		}
		if (flag5)
		{
			_setRaceModelED.Apply_ArmorDisplay((ScriptableArmor)GameManager._current.Locate_Item(current._characterFile._vanityStruct._leggingTag));
		}
		_setRaceModelED.DyeIndex = appearanceProfile._helmDyeIndex;
	}

	public void Init_DeleteCharacterPrompt(bool isTrue)
	{
		if (isTrue)
		{
			_characterDeleteConfirmInput.ActivateInputField();
		}
		_deleteCharacterPromptElement.isEnabled = isTrue;
		_deleteCharacterPrompt = isTrue;
	}

	public void Init_DeleteCharacter()
	{
		ProfileDataManager._current.Delete_ProfileData(ProfileDataManager._current.SelectedFileIndex);
		_deleteCharacterPrompt = false;
		Apply_CharacterSelectDisplay();
	}

	public void Initalize_CharacterFiles(CharacterFile[] _characterFiles)
	{
		if (_characterFiles.Length != 0)
		{
			for (int i = 0; i < _characterFiles.Length; i++)
			{
				CharacterSelectListDataEntry characterSelectListDataEntry = Create_CharacterSelectSlot(_characterFiles[i]);
				characterSelectListDataEntry._dataID = i;
				characterSelectListDataEntry._fileNumberText.text = $"File {i + 1}";
			}
		}
		CharacterSelectListDataEntry Create_CharacterSelectSlot(CharacterFile _characterFile)
		{
			GameObject obj = Object.Instantiate(_characterSelectPrefab);
			obj.transform.SetParent(_characterSelectListContent);
			obj.transform.position = Vector3.zero;
			obj.transform.localScale = Vector3.one;
			CharacterSelectListDataEntry component = obj.GetComponent<CharacterSelectListDataEntry>();
			component._characterFileData = _characterFile;
			_characterSelectEntries.Add(component);
			return component;
		}
	}

	public void Clear_SelectEntries()
	{
		for (int i = 0; i < _characterSelectEntries.Count; i++)
		{
			Object.Destroy(_characterSelectEntries[i].gameObject);
		}
		_characterSelectEntries.Clear();
	}

	public void Select_CharacterFile()
	{
		if (MainMenuManager._current._menuInputBuffer > 0f)
		{
			return;
		}
		_aSrc_confirmSelect.Play();
		if (ProfileDataManager._current._characterFile._isEmptySlot)
		{
			MainMenuManager._current.Set_MenuCondition(2);
			return;
		}
		StartCoroutine(Refresh_EnterGameButton());
		switch (_mainMenuManager._networkInitCondition)
		{
		case NetworkInitCondition.Singleplayer:
			_mainMenuManager.Init_SingleplayerHost();
			Invoke("Send_CharacterFile", 0.15f);
			break;
		case NetworkInitCondition.Host_Multiplayer:
			_mainMenuManager.Init_MultiplayerHost();
			Invoke("Send_CharacterFile", 0.15f);
			break;
		case NetworkInitCondition.Join_Multiplayer:
			if (AtlyssNetworkManager._current._steamworksMode)
			{
				if (_cachedSteamLobby)
				{
					SteamLobby._current.JoinLobby(SteamLobby._current._cachedSteamID);
					SteamLobby._current._cachedSteamID = default;
				}
				else
				{
					LobbyListManager._current.lobbyListWindow_element.isEnabled = true;
					LobbyListManager._current.Init_RefreshLobbyList();
				}
			}
			break;
		}
		_characterSelectCanvasObject.SetActive(value: false);
	}

	public void Send_CharacterFile()
	{
		StartCoroutine(Process_SendCharacterFile());
	}

	private IEnumerator Process_SendCharacterFile()
	{
		if (_isSendingCharacterFile)
		{
			yield break;
		}
		_isSendingCharacterFile = true;
		ProfileDataManager current = ProfileDataManager._current;
		CharacterCreationManager component = GetComponent<CharacterCreationManager>();
		SettingsManager._current.Save_CharacterSelected();
		for (int i = 0; i < component._scriptablePlayerRaces.Length; i++)
		{
			if (current._characterFile._appearanceProfile._setRaceTag == component._scriptablePlayerRaces[i]._raceName)
			{
				component._raceDisplayModels[i]._raceAnimator.CrossFade("Select_Character", 0.1f, 5);
				component._raceDisplayModels[i]._raceAnimator.CrossFade("Idle", 0f, 0);
				break;
			}
		}
		yield return new WaitForSeconds(0.95f);
		_loadCameraEffect = true;
		yield return new WaitForSeconds(0.08f);
		_initSelectEffectObject.SetActive(value: true);
		yield return new WaitForSeconds(2.3f);
		_mainMenuManager.Set_MenuCondition(4);
		AtlyssNetworkManager._current._profileDataSender.Client_SendCharacterFileData();
		yield return new WaitForSeconds(0.85f);
		_isSendingCharacterFile = false;
	}

	private IEnumerator Refresh_EnterGameButton()
	{
		_enterGameButton.interactable = false;
		_returnButton.interactable = false;
		yield return new WaitForSeconds(0.95f);
		_enterGameButton.interactable = true;
		_returnButton.interactable = true;
	}
}
