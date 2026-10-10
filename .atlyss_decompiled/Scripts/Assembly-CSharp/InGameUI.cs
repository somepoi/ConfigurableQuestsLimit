using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
	public static InGameUI _current;

	private Player _mainPlayer;

	private PlayerInventory _pInventory;

	private PlayerStats _pStats;

	private StatusEntity _playerStatus;

	private PlayerPvp _pPvp;

	[Header("In-Game UI Parameters")]
	[SerializeField]
	private CanvasGroup _baseCanvasGroup;

	public RectTransform _heldListEntryCanvas;

	public float _escapeMenuBuffer;

	public bool _displayUI;

	public bool _hideUIHotkeyToggle;

	private bool _bufferPlayerInfo = true;

	[Header("Crosshair")]
	public Image _crosshairImage;

	[Header("FPS/PING Counter")]
	[SerializeField]
	private Text fpsCounter;

	private float _updateFpsCounterTime = 0.2f;

	[Header("Disconnect Game Elements")]
	[SerializeField]
	private Image _disconnectFadeoutImage;

	[Header("Subscene Name Card")]
	[SerializeField]
	private TextMeshProUGUI _text_sceneCardName;

	[SerializeField]
	private TMP_ColorGradient[] _zoneTypeGradients;

	[Header("World Time Display")]
	[SerializeField]
	private Animator _dayNightPendulumAnim;

	[SerializeField]
	private Image _dayNightCycleIcon;

	[SerializeField]
	private Sprite _dayIcon;

	[SerializeField]
	private Sprite _nightIcon;

	[SerializeField]
	private Text _worldTimeCounterText;

	[Header("Portal Caption Prompt")]
	[SerializeField]
	private CanvasGroup _portalCaptionGroup;

	[SerializeField]
	private Text _portalCaptionText;

	[Header("Map Instance Title")]
	public CanvasGroup _mapInstanceTitleGroup;

	[SerializeField]
	private Text _mapNameText;

	[SerializeField]
	private Text _mapZoneTypeText;

	[SerializeField]
	private GameObject _minimapImage;

	private string _reigonTitle = "";

	[Header("Peer Count Element")]
	[SerializeField]
	private MenuElement _peerCountElement;

	[SerializeField]
	private Text _peerCounterText;

	[Header("Context Prompt")]
	[SerializeField]
	private MenuElement _contextPromptElement;

	[SerializeField]
	private Text _contextPromptText;

	[SerializeField]
	private Text _interactKeyText;

	[SerializeField]
	private Image _contentPromptGamepadIco;

	[Header("Player Info Cell")]
	[SerializeField]
	private Text _text_mainPlayerLevel;

	[SerializeField]
	private GameObject _pvpFlagIcon;

	[Header("Player Stats Cell")]
	[SerializeField]
	private Text _text_currentHealthCounter;

	[SerializeField]
	private Text _text_maxHealthCounter;

	[SerializeField]
	private Text _text_currentManaCounter;

	[SerializeField]
	private Text _text_maxManaCounter;

	[SerializeField]
	private Text _text_experienceCounter;

	[SerializeField]
	private Image _barFill_health;

	[SerializeField]
	private Image _barFill_mana;

	[SerializeField]
	private Image _barFill_experience;

	private float _healthFillAmount;

	private float _manaFillAmount;

	private float _expFillAmount;

	[SerializeField]
	private MenuElement _statPointNoticeElement;

	[SerializeField]
	private MenuElement _skillPointNoticeElement;

	[SerializeField]
	private Text _statPointNoticeCounter;

	[SerializeField]
	private Text _statPointHotkeyText;

	[SerializeField]
	private Text _skillPointNoticeCounter;

	[SerializeField]
	private Text _skillPointHotkeyText;

	[Space]
	[SerializeField]
	private GameObject _absorbtionCell;

	[SerializeField]
	private Text _text_absorbtionCounter;

	[Space]
	[SerializeField]
	private MenuElement _professionCellElement;

	[SerializeField]
	private ProfessionCellSlot _professionCell;

	[SerializeField]
	private Text _professionCell_nodeTag;

	[Header("Buffs / Conditions Cell")]
	[SerializeField]
	private GameObject _conditionIconPrefab;

	[SerializeField]
	private Transform _conditionIconContent;

	public List<ConditionListDataEntry> _conditionDataEntries = new List<ConditionListDataEntry>();

	[Header("Bag Stats Cell")]
	[SerializeField]
	private Text _text_currencyCounter;

	[SerializeField]
	private Text _text_gearItemCounter;

	[SerializeField]
	private Text _text_consumableItemCounter;

	[SerializeField]
	private Text _text_tradeItemCounter;

	[SerializeField]
	private float _currencyTallySpeed = 0.085f;

	[HideInInspector]
	public int _currencyTally;

	[Header("Dungeon Elements")]
	public MenuElement _dungeonKeySlotElement;

	public Text _dungeonKeyCounter;

	[Space]
	public MenuElement _bossRoomQueueElement;

	public Text _bossRoomQueueText;

	[Header("PVP Arena Elements")]
	public MenuElement _autoPVPFlagElement;

	public Text _autoPVPFlagCounter;

	[Header("Audio")]
	[SerializeField]
	private AudioSource _currencyTallySrc;

	public string _cachedReigonTag = string.Empty;

	private void Awake()
	{
		if (_current == null)
		{
			_current = this;
			_portalCaptionGroup.alpha = 0f;
			_mapInstanceTitleGroup.alpha = 0f;
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void Update()
	{
		Update_FpsCounter();
		if (TabMenu._current._optionsCell._isDisconnecting)
		{
			_disconnectFadeoutImage.color = Color.Lerp(_disconnectFadeoutImage.color, Color.black, 13.5f * Time.unscaledDeltaTime);
			_displayUI = false;
		}
		else
		{
			_disconnectFadeoutImage.color = Color.clear;
		}
		if (_displayUI)
		{
			_baseCanvasGroup.alpha = 1f;
		}
		else
		{
			_baseCanvasGroup.alpha = 0f;
			_portalCaptionGroup.alpha = 0f;
			if ((bool)_mainPlayer)
			{
				_mainPlayer._chatBehaviour._textFadeTimer = 0f;
			}
		}
		if (!_mainPlayer)
		{
			RefreshParams_LocatePlayer();
			return;
		}
		Handle_InGameUI();
		if (_escapeMenuBuffer > 0f)
		{
			_escapeMenuBuffer -= Time.deltaTime;
		}
		else
		{
			_escapeMenuBuffer = 0f;
		}
	}

	private void Update_FpsCounter()
	{
		if (!_displayUI)
		{
			fpsCounter.text = string.Empty;
			return;
		}
		_updateFpsCounterTime -= Time.deltaTime;
		if (_updateFpsCounterTime <= 0f)
		{
			float f = 1f / Time.unscaledDeltaTime;
			_updateFpsCounterTime = 0.2f;
			string text = $"FPS {Mathf.Round(f)}\n";
			string text2 = $"PING {Math.Round(NetworkTime.rtt * 1000.0)}ms";
			if (SettingsManager._current._hideFPSCounter.isOn)
			{
				text = string.Empty;
			}
			if (SettingsManager._current._hidePingCounter.isOn)
			{
				text2 = string.Empty;
			}
			if (!NetworkClient.active || AtlyssNetworkManager._current._soloMode)
			{
				fpsCounter.text = text ?? "";
			}
			else
			{
				fpsCounter.text = text + text2;
			}
		}
	}

	private void RefreshParams_LocatePlayer()
	{
		Handle_ResetValues();
		if ((bool)Player._mainPlayer)
		{
			_mainPlayer = Player._mainPlayer;
			_pInventory = _mainPlayer._pInventory;
			_pStats = _mainPlayer._pStats;
			_playerStatus = _mainPlayer._statusEntity;
			_pPvp = _mainPlayer._pPvp;
			Invoke("Init_DisableInfoBuffer", 1.45f);
		}
		void Handle_ResetValues()
		{
			_text_currentHealthCounter.text = string.Empty;
			_text_currentManaCounter.text = string.Empty;
			_text_experienceCounter.text = string.Empty;
			_text_absorbtionCounter.text = string.Empty;
			_worldTimeCounterText.text = string.Empty;
			_barFill_health.fillAmount = 0f;
			_barFill_mana.fillAmount = 0f;
			_barFill_experience.fillAmount = 0f;
			_text_currencyCounter.text = string.Empty;
			_currencyTally = 0;
			_displayUI = false;
			_bufferPlayerInfo = true;
			if (IsInvoking("CurrencyTally"))
			{
				CancelInvoke("CurrencyTally");
			}
		}
	}

	private void Init_DisableInfoBuffer()
	{
		_bufferPlayerInfo = false;
	}

	private void Handle_InGameUI()
	{
		if (!GameManager._current || _bufferPlayerInfo)
		{
			return;
		}
		bool flag = DialogManager._current._currentDialogTrigger == null && !Player._mainPlayer._bufferingStatus;
		_baseCanvasGroup.blocksRaycasts = flag;
		_baseCanvasGroup.interactable = flag;
		_minimapImage.SetActive(!SettingsManager._current._hideDungeonMinimap.isOn);
		if (Input.GetKeyDown(InputControlManager.current._hideUI) && !Player._mainPlayer._inChat)
		{
			_hideUIHotkeyToggle = !_hideUIHotkeyToggle;
		}
		_displayUI = !LoadSceneManager._current._isLoadingScreen && !DialogManager._current._currentDialogTrigger && !_hideUIHotkeyToggle;
		if (!Player._mainPlayer._bufferingStatus)
		{
			if (!string.IsNullOrWhiteSpace(_reigonTitle))
			{
				_text_sceneCardName.text = _reigonTitle ?? "";
			}
			else
			{
				_text_sceneCardName.text = Player._mainPlayer.Network_playerMapInstance._mapName ?? "";
			}
			_text_sceneCardName.colorGradientPreset = _zoneTypeGradients[(uint)Player._mainPlayer._playerZoneType];
		}
		_peerCountElement.isEnabled = !AtlyssNetworkManager._current._soloMode && !SettingsManager._current._hidePlayerInstanceCounter.isOn;
		if (_peerCountElement.isEnabled && (bool)Player._mainPlayer.Network_playerMapInstance)
		{
			_peerCounterText.text = $"{Player._mainPlayer.Network_playerMapInstance._peercount}";
		}
		if (TabMenu._current._isOpen)
		{
			_mapInstanceTitleGroup.alpha = 0f;
			StopCoroutine("MapTitleDisplay");
		}
		Handle_PlayerStatus();
		Handle_PointNoticePanels();
		Handle_CurrencyDisplay();
		Handle_DayNightIconDisplay();
		Handle_DungeonKeyDisplay();
		Handle_ArenaElementDisplay();
		Handle_ProfessionCellDisplay();
		if (SettingsManager._current._isOpen)
		{
			CameraFunction._current._unlockedCamera = true;
			_mainPlayer._inUI = true;
			_displayUI = false;
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
		else if (_mainPlayer._currentPlayerCondition != PlayerCondition.ACTIVE)
		{
			CameraFunction._current._unlockedCamera = true;
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
	}

	private void Handle_PlayerStatus()
	{
		GameManager _gm = GameManager._current;
		StatStruct _pStatStruct = _pStats._statStruct;
		_text_mainPlayerLevel.text = $"{_pStats._currentLevel}";
		_pvpFlagIcon.SetActive(_pPvp._currentVersusFlag == VersusFlag.PVP);
		Handle_Healthbar();
		Handle_Manabar();
		Handle_Expbar();
		Handle_AbsorbtionCounter();
		Handle_InventoryCounters();
		void Handle_AbsorbtionCounter()
		{
			_absorbtionCell.SetActive(_playerStatus._damageAbsorbtion > 0f);
			_text_absorbtionCounter.text = $"{_playerStatus._damageAbsorbtion}";
		}
		void Handle_Expbar()
		{
			if (_pStats._currentLevel < _gm._statLogics._maxMainLevel)
			{
				float num = (float)_pStats._currentExp / (float)_pStatStruct._experience * 100f;
				_text_experienceCounter.text = string.Format("EXP {0} / {1} ({2}%)", _pStats._currentExp, _pStats._statStruct._experience, num.ToString("F2"));
				_expFillAmount += ((float)_pStats._currentExp / (float)_pStatStruct._experience - _expFillAmount) * 0.2f;
				if (float.IsNaN(_expFillAmount))
				{
					_expFillAmount = 0f;
				}
				_barFill_experience.fillAmount = _expFillAmount;
			}
			else
			{
				_text_experienceCounter.text = "MAX";
				_barFill_experience.fillAmount = 1f;
			}
		}
		void Handle_Healthbar()
		{
			int num = _playerStatus._currentHealth;
			if (_playerStatus._currentHealth <= 0)
			{
				num = 0;
			}
			if (num > _pStats._statStruct._maxHealth)
			{
				num = _pStats._statStruct._maxHealth;
			}
			_text_currentHealthCounter.text = $"{num}";
			_text_maxHealthCounter.text = $"{_pStatStruct._maxHealth}";
			_healthFillAmount += ((float)_playerStatus._currentHealth / (float)_pStatStruct._maxHealth - _healthFillAmount) * 0.2f;
			if (float.IsNaN(_healthFillAmount))
			{
				_healthFillAmount = 0f;
			}
			_barFill_health.fillAmount = _healthFillAmount;
		}
		void Handle_InventoryCounters()
		{
			int maxEquipmentSlots = GameManager._current._statLogics._maxInventorySlotStruct._maxEquipmentSlots;
			int maxConsumableItemSlots = GameManager._current._statLogics._maxInventorySlotStruct._maxConsumableItemSlots;
			int maxTradeItemSlots = GameManager._current._statLogics._maxInventorySlotStruct._maxTradeItemSlots;
			Color color = new Color(255f, 103f, 0f);
			if (_pInventory._usedEquipmentSlots >= maxEquipmentSlots)
			{
				_text_gearItemCounter.color = color;
			}
			else
			{
				_text_gearItemCounter.color = Color.white;
			}
			if (_pInventory._usedConsumableItemSlots >= maxConsumableItemSlots)
			{
				_text_consumableItemCounter.color = color;
			}
			else
			{
				_text_consumableItemCounter.color = Color.white;
			}
			if (_pInventory._usedTradeItemSlots >= maxTradeItemSlots)
			{
				_text_tradeItemCounter.color = color;
			}
			else
			{
				_text_tradeItemCounter.color = Color.white;
			}
			_text_gearItemCounter.text = $"{_pInventory._usedEquipmentSlots} / {maxEquipmentSlots}";
			_text_consumableItemCounter.text = $"{_pInventory._usedConsumableItemSlots} / {maxConsumableItemSlots}";
			_text_tradeItemCounter.text = $"{_pInventory._usedTradeItemSlots} / {maxTradeItemSlots}";
		}
		void Handle_Manabar()
		{
			int num = _playerStatus._currentMana;
			if (_playerStatus._currentMana <= 0)
			{
				num = 0;
			}
			_text_currentManaCounter.text = $"{num}";
			_text_maxManaCounter.text = $"{_pStatStruct._maxMana}";
			_manaFillAmount += ((float)_playerStatus._currentMana / (float)_pStatStruct._maxMana - _manaFillAmount) * 0.2f;
			if (float.IsNaN(_manaFillAmount))
			{
				_manaFillAmount = 0f;
			}
			_barFill_mana.fillAmount = _manaFillAmount;
		}
	}

	private void Handle_PointNoticePanels()
	{
		_statPointNoticeElement.isEnabled = _pStats._currentAttributePoints > 0 && !TabMenu._current._isOpen && !SettingsManager._current._hideStatPointCounter.isOn;
		_skillPointNoticeElement.isEnabled = _pStats._currentSkillPoints > 0 && !TabMenu._current._isOpen && !SettingsManager._current._hideSkillPointCounter.isOn;
		_statPointNoticeCounter.text = $"{_pStats._currentAttributePoints}";
		_skillPointNoticeCounter.text = $"{_pStats._currentSkillPoints}";
		string text = InputControlManager.current.Convert_KeyCodeName(InputControlManager.current._statMenuHotkey.ToString());
		string text2 = InputControlManager.current.Convert_KeyCodeName(InputControlManager.current._skillMenuHotkey.ToString());
		_statPointHotkeyText.text = text ?? "";
		_skillPointHotkeyText.text = text2 ?? "";
	}

	public void Add_ConditionListEntry(ConditionData _conditionData)
	{
		GameObject obj = UnityEngine.Object.Instantiate(_conditionIconPrefab);
		obj.transform.SetParent(_conditionIconContent);
		obj.transform.position = Vector3.zero;
		ConditionListDataEntry component = obj.GetComponent<ConditionListDataEntry>();
		component._conditionData = _conditionData;
		_conditionDataEntries.Add(component);
	}

	private void Handle_DayNightIconDisplay()
	{
		if ((bool)Player._mainPlayer.Network_playerMapInstance)
		{
			_worldTimeCounterText.text = $"{Player._mainPlayer.Network_playerMapInstance._instanceTime} {Player._mainPlayer.Network_playerMapInstance._instanceClockSetting}";
			switch (Player._mainPlayer.Network_playerMapInstance._instanceWorldTime)
			{
			case WorldTime.DAY:
				_dayNightCycleIcon.sprite = _dayIcon;
				_dayNightPendulumAnim.SetBool("_isNight", value: false);
				break;
			case WorldTime.NIGHT:
				_dayNightCycleIcon.sprite = _nightIcon;
				_dayNightPendulumAnim.SetBool("_isNight", value: true);
				break;
			}
		}
	}

	private void Handle_CurrencyDisplay()
	{
		if (_mainPlayer._bufferingStatus || ShopkeepManager._current._isOpen)
		{
			_currencyTally = _pInventory._heldCurrency;
			_text_currencyCounter.text = $"{_pInventory._heldCurrency:n0}";
		}
		else if (!IsInvoking("CurrencyTally"))
		{
			InvokeRepeating("CurrencyTally", 0f, _currencyTallySpeed);
		}
	}

	private void CurrencyTally()
	{
		if (!_mainPlayer || _mainPlayer._bufferingStatus || ShopkeepManager._current._isOpen)
		{
			return;
		}
		if (_currencyTally < _pInventory._heldCurrency)
		{
			_currencyTallySrc.Stop();
			_currencyTallySrc.Play();
			_currencyTally++;
			if (_pInventory._heldCurrency - _currencyTally > 500)
			{
				_currencyTally = _pInventory._heldCurrency;
			}
		}
		if (_currencyTally > _pInventory._heldCurrency)
		{
			_currencyTallySrc.Stop();
			_currencyTallySrc.Play();
			_currencyTally--;
			if (_pInventory._heldCurrency - _currencyTally < -500)
			{
				_currencyTally = _pInventory._heldCurrency;
			}
		}
		_text_currencyCounter.text = $"{_currencyTally:n0}";
	}

	private void Handle_DungeonKeyDisplay()
	{
		if ((bool)Player._mainPlayer.Network_playerMapInstance)
		{
			_dungeonKeySlotElement.isEnabled = Player._mainPlayer._playerZoneType == ZoneType.Dungeon;
			if ((bool)Player._mainPlayer.Network_playerMapInstance._patternInstance)
			{
				_dungeonKeyCounter.text = $"{Player._mainPlayer.Network_playerMapInstance._patternInstance._dungeonKeysAquired}";
			}
		}
	}

	private void Handle_ArenaElementDisplay()
	{
		_autoPVPFlagElement.isEnabled = _mainPlayer._playerZoneType == ZoneType.Arena;
		if (_pPvp._arena_autoFlagTimer > 0)
		{
			_autoPVPFlagCounter.text = $"{_pPvp._arena_autoFlagTimer}";
		}
		else
		{
			_autoPVPFlagCounter.text = "SET";
		}
	}

	private void Handle_ProfessionCellDisplay()
	{
		_professionCellElement.isEnabled = (bool)_mainPlayer._pProfession._setResourceEntity && !TabMenu._current._isOpen;
		if ((bool)_mainPlayer._pProfession._setResourceEntity)
		{
			ResourceEntity setResourceEntity = _mainPlayer._pProfession._setResourceEntity;
			_professionCell._scriptableProfession = setResourceEntity._scriptableProfession;
			string arg = "<color=yellow>";
			if (setResourceEntity._resourceEntityLevel - _professionCell._professionStruct._professionLvl <= -3)
			{
				arg = "<color=grey>";
			}
			if (setResourceEntity._resourceEntityLevel - _professionCell._professionStruct._professionLvl == -2)
			{
				arg = "<color=lime>";
			}
			if (_professionCell._professionStruct._professionLvl < setResourceEntity._resourceEntityLevel)
			{
				arg = "<color=#e69138>";
			}
			_professionCell_nodeTag.text = $"{setResourceEntity._resourceNodeTag} {arg}(lv-{setResourceEntity._resourceEntityLevel})</color>";
			for (int i = 0; i < GameManager._current._statLogics._scriptableProfessions.Length; i++)
			{
				if (GameManager._current._statLogics._scriptableProfessions[i] == setResourceEntity._scriptableProfession)
				{
					_professionCell._professionID = i;
					break;
				}
			}
		}
		else
		{
			_professionCell._scriptableProfession = null;
			_professionCell._professionExpFill.fillAmount = 0f;
		}
	}

	public void Init_MapTitleDisplay(string _reigonTag)
	{
		if (string.IsNullOrWhiteSpace(_cachedReigonTag) || !(_reigonTag == _cachedReigonTag))
		{
			_mapInstanceTitleGroup.alpha = 0f;
			StopAllCoroutines();
			StartCoroutine(MapTitleDisplay(_reigonTag));
			_cachedReigonTag = _reigonTag;
		}
	}

	private IEnumerator MapTitleDisplay(string _reigonTag)
	{
		_reigonTitle = _reigonTag;
		do
		{
			yield return null;
		}
		while (Player._mainPlayer._currentGameCondition == GameCondition.LOADING_GAME || Player._mainPlayer._currentPlayerCondition != PlayerCondition.ACTIVE || Player._mainPlayer._bufferingStatus);
		if (!string.IsNullOrWhiteSpace(_reigonTag))
		{
			_mapNameText.text = _reigonTag ?? "";
		}
		else
		{
			_mapNameText.text = Player._mainPlayer.Network_playerMapInstance._mapName ?? "";
		}
		_mapZoneTypeText.text = $"- {Player._mainPlayer._playerZoneType} Zone -";
		yield return new WaitForSeconds(0.2f);
		do
		{
			if (TabMenu._current._isOpen)
			{
				yield break;
			}
			_mapInstanceTitleGroup.alpha += Time.deltaTime * 0.85f;
			yield return null;
		}
		while (_mapInstanceTitleGroup.alpha < 1f);
		yield return new WaitForSeconds(1f);
		while (!TabMenu._current._isOpen)
		{
			_mapInstanceTitleGroup.alpha -= Time.deltaTime * 0.85f;
			yield return null;
			if (!(_mapInstanceTitleGroup.alpha > 0f))
			{
				break;
			}
		}
	}

	public void ContextPrompt(bool _isOpen, string _promptString)
	{
		_contentPromptGamepadIco.enabled = InputControlManager.current._setGamepadType != GamepadType.Keyboard;
		_interactKeyText.enabled = InputControlManager.current._setGamepadType == GamepadType.Keyboard;
		_interactKeyText.text = InputControlManager.current.Convert_KeyCodeName(InputControlManager.current._inputProfile._interactInput);
		_contextPromptText.text = _promptString;
		_contextPromptElement.isEnabled = _isOpen;
	}

	public void DisablePortalCaption()
	{
		_portalCaptionGroup.alpha = 0f;
	}

	public void PortalCaptionPrompt(string _caption)
	{
		if (string.IsNullOrWhiteSpace(_caption))
		{
			_portalCaptionGroup.alpha = 0f;
			return;
		}
		if (_mapInstanceTitleGroup.alpha > 0.1f)
		{
			_portalCaptionGroup.alpha = 0f;
			return;
		}
		_portalCaptionText.text = _caption;
		_portalCaptionGroup.alpha = 1f;
	}
}
