using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
	public static DialogManager _current;

	[Header("Runtime Dialog Trigger Data")]
	public DialogTrigger _currentDialogTrigger;

	public ScriptableDialogData _scriptableDialog;

	public NetNPC _cachedNpc;

	public DialogUIPrompt _currentDialogUIPrompt;

	private Dialog[] _currentDialogBranch;

	private Dialog _currentDialog;

	private int _dialogIndex;

	private bool _loadedDialogProfile;

	[Header("Dialog Parameters")]
	[SerializeField]
	private float TextSpeed = 0.0002f;

	public float _dialogBuffer;

	private readonly Queue<string> _dialogSentences = new Queue<string>();

	private string _dialogReference = string.Empty;

	public bool _isDialogEnabled;

	private bool _canSkipPress = true;

	private int _skipPresses;

	[Header("Dialog Selection Parameters")]
	[SerializeField]
	private MenuElement _dialogSelectionElement;

	[SerializeField]
	private Transform _selectionContainer;

	[SerializeField]
	private GameObject _selectionButtonPrefab;

	private readonly List<Button> _selectionButtons = new List<Button>();

	[Header("Dialog UI")]
	[SerializeField]
	private CanvasGroup dialogCanvasGroup;

	[SerializeField]
	private Animator _dialogCanvasAnim;

	[SerializeField]
	private GameObject _dialogNextIndicator;

	[SerializeField]
	private Text _dialogTextField;

	[Space]
	[SerializeField]
	private Text _characterNameText;

	[SerializeField]
	private GameObject _characterNameCaption;

	[Space]
	[SerializeField]
	private GameObject _facePicObject;

	[SerializeField]
	private Image _facePicImage;

	[SerializeField]
	private Vector2 _facePicDialogTextSize = new Vector2(345f, 85f);

	[SerializeField]
	private Vector2 _noFacePicDialogTextSize = new Vector2(435f, 85f);

	[Header("Gamepad Control")]
	public DialogManager_GamepadTab _currentGamepadTab = DialogManager_GamepadTab.Dialog_Box;

	[SerializeField]
	private MenuElement _gamepadNavigationInfoPanel;

	[SerializeField]
	private Image _gamepad_navIco_leftShoulder;

	[SerializeField]
	private Image _gamepad_navIco_rightShoulder;

	public float _gamepad_cancelBuffer;

	private int _currentGamepadTabSelection;

	private int _currentDialogSelection;

	[Header("Dialog Introduction Runtime Tags")]
	public List<string> _dialogIntroductionTags = new List<string>();

	public int CurrentGamepadTabSelection
	{
		get
		{
			return _currentGamepadTabSelection;
		}
		set
		{
			if (value < 0)
			{
				value = Enum.GetValues(typeof(DialogManager_GamepadTab)).Length - 1;
			}
			if (value > Enum.GetValues(typeof(DialogManager_GamepadTab)).Length - 1)
			{
				value = 0;
			}
			_currentGamepadTabSelection = value;
		}
	}

	public int CurrentDialogSelection
	{
		get
		{
			return _currentDialogSelection;
		}
		set
		{
			if (value < 0)
			{
				value = _selectionButtons.Count - 1;
			}
			if (value > _selectionButtons.Count - 1)
			{
				value = 0;
			}
			_currentDialogSelection = value;
		}
	}

	public bool IsDialogInput
	{
		get
		{
			if (!Input.GetKeyDown(InputControlManager.current._interact) && (!Input.GetKeyDown(InputControlManager.current._attack) || _currentDialogUIPrompt != DialogUIPrompt.NULL) && !GamepadInput.GetButtonDown(GamepadButton.Button_South))
			{
				return GamepadInput.GetButtonDown(GamepadButton.Button_West);
			}
			return true;
		}
	}

	public bool IsHardEndDialogInput
	{
		get
		{
			if (!Input.GetKey(KeyCode.Escape))
			{
				if (GamepadInput.GetButtonDown(GamepadButton.Start))
				{
					return _currentDialogUIPrompt != DialogUIPrompt.VANITY_MIRROR;
				}
				return false;
			}
			return true;
		}
	}

	private void Awake()
	{
		_current = this;
	}

	private void Start()
	{
		Reset_DialogManagerConditions();
	}

	private void Reset_DialogManagerConditions()
	{
		dialogCanvasGroup.alpha = 0f;
		_facePicImage.enabled = false;
		_dialogTextField.text = string.Empty;
		_dialogSelectionElement.isEnabled = false;
		_isDialogEnabled = false;
		_skipPresses = 0;
	}

	private void Update()
	{
		if ((bool)Player._mainPlayer)
		{
			if (!_loadedDialogProfile)
			{
				Init_LoadProfileCheck();
			}
			Handle_DialogRuntime();
		}
	}

	private void Init_LoadProfileCheck()
	{
		if (ProfileDataManager._current._characterFile._dialogIntroProfile != null)
		{
			for (int i = 0; i < ProfileDataManager._current._characterFile._dialogIntroProfile.Length; i++)
			{
				_dialogIntroductionTags.Add(ProfileDataManager._current._characterFile._dialogIntroProfile[i]);
			}
		}
		_loadedDialogProfile = true;
	}

	private void Handle_DialogRuntime()
	{
		if (_dialogBuffer > 0f)
		{
			_dialogBuffer -= Time.deltaTime;
		}
		else
		{
			_dialogBuffer = 0f;
		}
		if (_gamepad_cancelBuffer > 0f)
		{
			_gamepad_cancelBuffer -= Time.deltaTime;
		}
		else
		{
			_gamepad_cancelBuffer = 0f;
		}
		if (!_canSkipPress)
		{
			_skipPresses = 0;
		}
		bool flag = InputControlManager.current._setGamepadType != GamepadType.Keyboard;
		if (_isDialogEnabled)
		{
			CameraFunction._current._unlockedCamera = true;
			if (InputControlManager.current._setGamepadType == GamepadType.Keyboard)
			{
				Cursor.lockState = CursorLockMode.None;
			}
			else if (InputControlManager.current._gamepadMouseCursorBuffer > 0f)
			{
				Cursor.lockState = CursorLockMode.None;
			}
			dialogCanvasGroup.alpha = Mathf.Lerp(dialogCanvasGroup.alpha, 1f, 15.5f * Time.deltaTime);
			if (IsHardEndDialogInput || Player._mainPlayer._isHurt)
			{
				End_Dialog();
			}
			for (int i = 0; i < _selectionButtons.Count; i++)
			{
				_selectionButtons[i].transform.GetChild(0).gameObject.SetActive(flag && _currentDialogSelection == i && _currentGamepadTab == DialogManager_GamepadTab.Dialog_Box);
			}
			_gamepadNavigationInfoPanel.isEnabled = flag && _currentDialogUIPrompt != DialogUIPrompt.NULL;
			if (!flag)
			{
				Cursor.visible = true;
			}
			else
			{
				Handle_GamepadControl();
			}
			if (IsDialogInput)
			{
				if (_dialogTextField.text == _dialogReference && !_dialogSelectionElement.isEnabled)
				{
					Display_NextSentence();
				}
				if (_skipPresses < 2 && _canSkipPress)
				{
					_skipPresses++;
				}
			}
			if (_skipPresses >= 2 && _dialogTextField.text != _dialogReference)
			{
				_dialogTextField.text = _dialogReference;
				Complete_Dialog();
				_skipPresses = 0;
			}
		}
		else
		{
			Clear_DialogSelectionButtons();
			TabMenu._current._itemTradeMode = false;
			dialogCanvasGroup.alpha = 0f;
			_skipPresses = 0;
		}
		void Handle_GamepadControl()
		{
			Cursor.visible = InputControlManager.current._gamepadMouseCursorBuffer > 0f;
			bool flag2 = InputControlManager.current.DpadUp(DpadHoldType.OnDown) || GamepadInput.GetButtonDown(GamepadButton.RightStick_Up) || GamepadInput.GetButtonDown(GamepadButton.LeftStick_Up);
			bool flag3 = InputControlManager.current.DpadDown(DpadHoldType.OnDown) || GamepadInput.GetButtonDown(GamepadButton.RightStick_Down) || GamepadInput.GetButtonDown(GamepadButton.LeftStick_Down);
			bool buttonDown = GamepadInput.GetButtonDown(GamepadButton.LeftShoulder);
			bool buttonDown2 = GamepadInput.GetButtonDown(GamepadButton.RightShoulder);
			if (GamepadInput.GetButton(GamepadButton.LeftShoulder))
			{
				_gamepad_navIco_leftShoulder.color = Color.white;
			}
			else
			{
				_gamepad_navIco_leftShoulder.color = Color.grey;
			}
			if (GamepadInput.GetButton(GamepadButton.RightShoulder))
			{
				_gamepad_navIco_rightShoulder.color = Color.white;
			}
			else
			{
				_gamepad_navIco_rightShoulder.color = Color.grey;
			}
			if (_currentGamepadTab == DialogManager_GamepadTab.Dialog_Box && InputControlManager.current._gamepadMouseCursorBuffer <= 0f)
			{
				ToolTipManager._current.Disable_AllTooltips();
			}
			if (_currentDialogUIPrompt != DialogUIPrompt.NULL)
			{
				if (!TabMenu._current._itemCell._heldItemListDataEntry && _currentDialogUIPrompt != DialogUIPrompt.VANITY_MIRROR)
				{
					if (buttonDown)
					{
						CurrentGamepadTabSelection--;
						if (_currentGamepadTabSelection == 1 && !TabMenu._current._isOpen)
						{
							CurrentGamepadTabSelection--;
						}
						ToolTipManager._current.Disable_AllTooltips();
						TabMenu._current._gamepadSelectAsrc.Play();
					}
					if (buttonDown2)
					{
						CurrentGamepadTabSelection++;
						if (_currentGamepadTabSelection == 1 && !TabMenu._current._isOpen)
						{
							CurrentGamepadTabSelection++;
						}
						ToolTipManager._current.Disable_AllTooltips();
						TabMenu._current._gamepadSelectAsrc.Play();
					}
				}
				_currentGamepadTab = (DialogManager_GamepadTab)_currentGamepadTabSelection;
				if (!TabMenu._current._itemCell._heldItemListDataEntry && GamepadInput.GetButtonDown(GamepadButton.Button_East) && _gamepad_cancelBuffer <= 0f && TabMenu._current._itemCell._gamepadItemMenuFocus != GamepadItemMenuFocus.EQUIPMENT)
				{
					CurrentGamepadTabSelection = 2;
				}
			}
			else
			{
				_currentGamepadTab = DialogManager_GamepadTab.Dialog_Box;
			}
			if (_currentGamepadTab == DialogManager_GamepadTab.Dialog_Box && _selectionButtons.Count > 0)
			{
				if (flag2)
				{
					CurrentDialogSelection++;
				}
				if (flag3)
				{
					CurrentDialogSelection--;
				}
				if (IsDialogInput && _dialogTextField.text == _dialogReference)
				{
					_selectionButtons[_currentDialogSelection].onClick.Invoke();
				}
			}
		}
	}

	public void Start_Dialog(DialogBranch _dialogBranch, DialogTrigger _dialogTrigger)
	{
		if (TabMenu._current._isOpen)
		{
			TabMenu._current.Close_TabMenu();
		}
		_isDialogEnabled = true;
		_currentDialogTrigger = _dialogTrigger;
		if ((bool)_currentDialogTrigger.GetComponentInParent<NetNPC>())
		{
			_cachedNpc = _currentDialogTrigger.GetComponentInParent<NetNPC>();
		}
		_dialogIndex = 0;
		_dialogSentences.Clear();
		Clear_DialogSelectionButtons();
		Player._mainPlayer._pMove._lockControl = true;
		if (Player._mainPlayer._pCombat._currentSheathCondition == WeaponSheatheCondition.TAKEN_OUT)
		{
			Player._mainPlayer._pCombat.Cmd_SheatheWeapon(WeaponSheatheCondition.PUT_AWAY);
		}
		Dialog[] dialogs = _dialogBranch.dialogs;
		foreach (Dialog dialog in dialogs)
		{
			if (dialog != null)
			{
				string item = dialog._dialogInput;
				if (dialog._altInputs != null && dialog._altInputs.Length != 0)
				{
					item = dialog._altInputs[UnityEngine.Random.Range(0, dialog._altInputs.Length)];
				}
				_dialogSentences.Enqueue(item);
			}
		}
		_currentDialogBranch = _dialogBranch.dialogs;
		Display_NextSentence();
	}

	public void Start_QuickSentence(string _sentence)
	{
		StopAllCoroutines();
		_dialogReference = _sentence;
		_canSkipPress = false;
		StartCoroutine(Type_Sentence(_sentence, TextSpeed, _quickSentence: true));
		StartCoroutine(PlayVoxSound());
	}

	private void Display_NextSentence()
	{
		CurrentDialogSelection = 0;
		RectTransform _dialogTextRect;
		if (_dialogIndex >= _currentDialogBranch.Length)
		{
			if (!_dialogSelectionElement.isEnabled)
			{
				End_Dialog();
			}
		}
		else
		{
			if (_dialogSentences.Count == 0)
			{
				return;
			}
			_dialogSelectionElement.isEnabled = false;
			_currentDialogUIPrompt = DialogUIPrompt.NULL;
			TabMenu._current._itemTradeMode = false;
			if (TabMenu._current._isOpen)
			{
				TabMenu._current.Close_TabMenu();
			}
			_currentDialog = _currentDialogBranch[_dialogIndex];
			_dialogTextRect = _dialogTextField.GetComponent<RectTransform>();
			_characterNameCaption.SetActive(!string.IsNullOrEmpty(_scriptableDialog._nameTag));
			_characterNameText.text = _scriptableDialog._nameTag;
			if ((bool)_cachedNpc && (bool)_cachedNpc._animator)
			{
				if (_currentDialog._npcAnimationTag != string.Empty)
				{
					_cachedNpc.Local_CrossFade_Anim(_currentDialog._npcAnimationTag, 0.1f, 2);
					_cachedNpc._animator.SetBool("_isEmoting", value: true);
				}
				else
				{
					_cachedNpc.Local_CrossFade_Anim("Empty", 0.1f, 2);
					_cachedNpc._animator.SetBool("_isEmoting", value: false);
				}
			}
			Set_FacePicParameters();
			string sentence = (_dialogReference = _dialogSentences.Dequeue());
			_skipPresses = 0;
			_canSkipPress = true;
			if (_dialogIndex < _currentDialogBranch.Length)
			{
				_dialogIndex++;
			}
			StopAllCoroutines();
			StartCoroutine(Type_Sentence(sentence, TextSpeed, _quickSentence: false));
			StartCoroutine(PlayVoxSound());
		}
		void Set_FacePicParameters()
		{
			_facePicObject.SetActive(_currentDialog.facepic);
			_facePicImage.enabled = _currentDialog.facepic;
			if ((bool)_currentDialog.facepic)
			{
				if (_currentDialog.facepic != _facePicImage.sprite)
				{
					_dialogCanvasAnim.SetTrigger("init_facepic");
				}
				_facePicImage.sprite = _currentDialog.facepic;
				_dialogTextRect.sizeDelta = _facePicDialogTextSize;
				_dialogTextRect.anchoredPosition = new Vector2(45f, 0f);
			}
			else
			{
				_dialogTextRect.sizeDelta = _noFacePicDialogTextSize;
				_dialogTextRect.anchoredPosition = Vector2.zero;
			}
		}
	}

	private void Create_DialogSelectionButton(int _index, DialogSelection _dialogSelection)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(_selectionButtonPrefab, Vector2.zero, Quaternion.identity);
		Button component = gameObject.GetComponent<Button>();
		Text componentInChildren = gameObject.GetComponentInChildren<Text>();
		gameObject.transform.SetParent(_selectionContainer);
		gameObject.transform.position = Vector3.zero;
		gameObject.transform.localScale = Vector3.one;
		componentInChildren.text = _dialogSelection._selectionCaption;
		gameObject.transform.GetChild(1).gameObject.SetActive(_dialogSelection._selectionIcon);
		if ((bool)_dialogSelection._selectionIcon)
		{
			gameObject.transform.GetChild(1).transform.GetChild(0).GetComponent<Image>().sprite = _dialogSelection._selectionIcon;
		}
		_selectionButtons.Add(component);
		component.onClick.AddListener(() =>
		{
			Select_DialogButton(_index);
		});
	}

	private void Select_DialogButton(int _buttonIndex)
	{
		if (_currentDialog._dialogSelections[_buttonIndex]._setDialogIndex < 0)
		{
			End_Dialog();
			return;
		}
		Clear_DialogSelectionButtons();
		DialogBranch dialogBranch = _scriptableDialog._dialogBranches[_currentDialog._dialogSelections[_buttonIndex]._setDialogIndex];
		Start_Dialog(dialogBranch, _currentDialogTrigger);
	}

	private void Clear_DialogSelectionButtons()
	{
		for (int i = 0; i < _selectionButtons.Count; i++)
		{
			UnityEngine.Object.Destroy(_selectionButtons[i].gameObject);
			_selectionButtons.RemoveAt(i);
		}
	}

	private IEnumerator Type_Sentence(string sentence, float speed, bool _quickSentence)
	{
		_dialogNextIndicator.SetActive(value: false);
		_dialogTextField.text = string.Empty;
		_dialogTextField.text = "";
		if ((bool)_cachedNpc)
		{
			_cachedNpc.Local_CrossFade_Anim("Mouth_Talk", 0f, 1);
		}
		char[] array = sentence.ToCharArray();
		foreach (char c in array)
		{
			_dialogTextField.text += c;
			if (IsDialogInput)
			{
				yield return null;
			}
			else
			{
				yield return new WaitForSeconds(speed);
			}
		}
		while (_dialogTextField.text != _dialogReference)
		{
			yield return null;
		}
		if ((bool)_cachedNpc)
		{
			_cachedNpc.Local_CrossFade_Anim("Mouth_Closed", 0f, 1);
		}
		if (!_quickSentence)
		{
			Complete_Dialog();
		}
	}

	private IEnumerator PlayVoxSound()
	{
		if (_scriptableDialog._voxSounds != null && _scriptableDialog._voxSounds.Length != 0 && (bool)_currentDialogTrigger._voxAsrc)
		{
			AudioSource _aSrc = _currentDialogTrigger._voxAsrc;
			int _voxAmount = 1;
			while (_voxAmount > 0 && _dialogTextField.text != _dialogReference)
			{
				_aSrc.pitch = UnityEngine.Random.Range(0.9784623f, 1.0734295f);
				_aSrc.PlayOneShot(_scriptableDialog._voxSounds[UnityEngine.Random.Range(0, _scriptableDialog._voxSounds.Length)], 0.35f);
				yield return new WaitForSeconds(_scriptableDialog._voxSpeed);
			}
		}
	}

	private void Complete_Dialog()
	{
		StopAllCoroutines();
		if ((bool)_cachedNpc)
		{
			_cachedNpc.Local_CrossFade_Anim("Mouth_Closed", 0f, 1);
		}
		Clear_DialogSelectionButtons();
		if (_currentDialog._dialogSelections.Length != 0)
		{
			for (int i = 0; i < _currentDialog._dialogSelections.Length; i++)
			{
				Create_DialogSelectionButton(i, _currentDialog._dialogSelections[i]);
			}
			_dialogSelectionElement.isEnabled = true;
			CurrentDialogSelection = _selectionButtons.Count - 1;
		}
		_dialogNextIndicator.SetActive(!_dialogSelectionElement.isEnabled);
		_currentDialogUIPrompt = _currentDialog._dialogUI;
		if (!_scriptableDialog)
		{
			return;
		}
		switch (_currentDialogUIPrompt)
		{
		case DialogUIPrompt.NULL:
			CurrentGamepadTabSelection = 2;
			break;
		case DialogUIPrompt.ENCHANT:
			CurrentGamepadTabSelection = 1;
			break;
		case DialogUIPrompt.QUEST:
			QuestSelectionManager._current.Create_QuestListing(_scriptableDialog._scriptableQuests);
			CurrentGamepadTabSelection = 0;
			break;
		case DialogUIPrompt.STORAGE:
			ItemStorageManager._current.Init_StorageListing();
			CurrentGamepadTabSelection = 0;
			break;
		case DialogUIPrompt.VANITY_MIRROR:
			VanityMirrorManager._current.Reset_RaceModelDisplayRotation();
			CurrentGamepadTabSelection = 0;
			break;
		case DialogUIPrompt.WORLDPORTAL:
			WorldPortalManager._current.Refresh_ZoneEntries();
			CurrentGamepadTabSelection = 0;
			break;
		case DialogUIPrompt.SHOP:
			ShopkeepManager._current._scriptShopkeep = _scriptableDialog._scriptableShopkeep;
			ShopkeepManager._current.Refresh_ShopkeepListings(_cachedNpc);
			TabMenu._current._itemCell.Init_InventoryTab(0);
			CurrentGamepadTabSelection = 0;
			break;
		case DialogUIPrompt.DUNGEON_PORTAL:
			if ((bool)_currentDialogTrigger.GetComponentInParent<Portal>())
			{
				Portal componentInParent = _currentDialogTrigger.GetComponentInParent<Portal>();
				DungeonPortalManager current = DungeonPortalManager._current;
				current._scenePortal = componentInParent._scenePortal;
				current._setPortal = componentInParent;
				current.SelectedDungeonOption = 0;
			}
			CurrentGamepadTabSelection = 0;
			break;
		}
	}

	public void End_Dialog()
	{
		if (!Player._mainPlayer || Player._mainPlayer._bufferingStatus || (bool)TabMenu._current._itemCell._heldItemListDataEntry)
		{
			return;
		}
		PlayerMove pMove = Player._mainPlayer._pMove;
		PlayerCombat pCombat = Player._mainPlayer._pCombat;
		PlayerInteract pInteract = Player._mainPlayer._pInteract;
		StopAllCoroutines();
		_dialogTextField.text = string.Empty;
		_dialogSentences.Clear();
		_dialogIndex = 0;
		_facePicImage.enabled = false;
		_facePicImage.sprite = null;
		if ((bool)_cachedNpc)
		{
			pInteract.Cmd_RemoveInteraction(_cachedNpc.netIdentity);
			if ((bool)_cachedNpc._animator)
			{
				_cachedNpc.Local_CrossFade_Anim("Mouth_Closed", 0f, 1);
				_cachedNpc.Local_CrossFade_Anim("Empty", 0.1f, 2);
				_cachedNpc._animator.SetBool("_isEmoting", value: false);
			}
			_cachedNpc._localPlayerInteraction = null;
			_cachedNpc = null;
		}
		_currentDialogBranch = null;
		_dialogReference = string.Empty;
		if ((bool)_currentDialogTrigger)
		{
			_currentDialogUIPrompt = DialogUIPrompt.NULL;
			_currentDialogTrigger._triggerActive = false;
			_currentDialogTrigger = null;
		}
		EventSystem.current.SetSelectedGameObject(null);
		pMove._lockControl = false;
		pCombat._focusWindowBuffer = 0.35f;
		QuestSelectionManager._current.Clear_QuestListing();
		WorldPortalManager._current.Clear_ZoneEntries();
		ShopkeepManager._current._scriptShopkeep = null;
		ShopkeepManager._current.Clear_ShopkeepListings();
		ToolTipManager._current.Disable_AllTooltips();
		TabMenu._current._itemTradeMode = false;
		if (TabMenu._current._isOpen)
		{
			TabMenu._current.Close_TabMenu();
		}
		_isDialogEnabled = false;
		_scriptableDialog = null;
		CameraFunction._current._initCameraOverride = false;
		CameraFunction._current._unlockedCamera = false;
		CameraFunction._current.CameraReset_Lerp();
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		_dialogBuffer = 0.65f;
	}
}
