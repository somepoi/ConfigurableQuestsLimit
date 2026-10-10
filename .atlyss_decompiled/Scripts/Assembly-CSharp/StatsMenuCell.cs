using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsMenuCell : TabMenuCell
{
	[Header("Stats Cell")]
	public Transform _statsInfoPanel;

	[SerializeField]
	private Text _statsCell_attributePointCounter;

	[SerializeField]
	private Text _statsCell_nicknameTag;

	[SerializeField]
	private Text _statsCell_raceTag;

	[SerializeField]
	private Text _statsCell_levelCounter;

	[SerializeField]
	private Text _statsCell_baseClassTag;

	[SerializeField]
	private Text _statsCell_experience;

	[SerializeField]
	private Text _statsCell_maxHealthCounter;

	[SerializeField]
	private Text _statsCell_maxManaCounter;

	[SerializeField]
	private Text _statsCell_maxStaminaCounter;

	[SerializeField]
	private Text _statsCell_attackCounter;

	[SerializeField]
	private Text _statsCell_rangePowerCounter;

	[SerializeField]
	private Text _statsCell_magicPowerCounter;

	[SerializeField]
	private Text _statsCell_critCounter;

	[SerializeField]
	private Text _statsCell_magicCritCounter;

	[SerializeField]
	private Text _statsCell_defenseCounter;

	[SerializeField]
	private Text _statsCell_magicDefenseCounter;

	[SerializeField]
	private Text _statsCell_evasionCounter;

	[SerializeField]
	private Text _statsCell_movementSpeedCounter;

	[SerializeField]
	private Text _statsCell_fireResistCounter;

	[SerializeField]
	private Text _statsCell_waterResistCounter;

	[SerializeField]
	private Text _statsCell_natureResistCounter;

	[SerializeField]
	private Text _statsCell_earthResistCounter;

	[SerializeField]
	private Text _statsCell_holyResistCounter;

	[SerializeField]
	private Text _statsCell_shadowResistCounter;

	[SerializeField]
	private Button _statsCell_applyAttributePointsButton;

	[SerializeField]
	private Button _statsCell_statTabButton;

	[SerializeField]
	private Button _statsCell_professionTabButton;

	[SerializeField]
	private Image _statsCell_applyAttributeGamepadIco;

	[Space]
	public StatStruct _projectionStatStruct;

	public bool _globalApplyingAttributePoints;

	public int _phantomAtbPoints;

	private int _attributeSelect;

	[Space]
	[SerializeField]
	private MenuElement _professionMenuElement;

	[SerializeField]
	private Transform _professionSlotContent;

	[SerializeField]
	private GameObject _professionSlotPrefab;

	public List<ProfessionCellSlot> _professionCellSlots = new List<ProfessionCellSlot>();

	[Space]
	[SerializeField]
	private RectTransform _gamepad_statMenuTabCursor;

	public int AttributeSelect
	{
		get
		{
			return _attributeSelect;
		}
		set
		{
			if (value > _listDataEntries.Count - 1)
			{
				value = _listDataEntries.Count - 1;
			}
			if (value < 0)
			{
				value = 0;
			}
			_attributeSelect = value;
		}
	}

	public override void Cell_OnAwake()
	{
		_statsCell_statTabButton.onClick.AddListener(Disable_ProfessionMenu);
		_statsCell_professionTabButton.onClick.AddListener(Enable_ProfessionMenu);
	}

	public override void Handle_CellUpdate()
	{
		Apply_StatsCellData();
	}

	public void Apply_StatsCellData()
	{
		StatStruct _statStruct;
		if (!TabMenu._current._isOpen)
		{
			Reset_AttributeProjection();
		}
		else if (TabMenu._current._selectedMenuCell != this && TabMenu._current._selectedMenuCell != TabMenu._current._itemCell)
		{
			Disable_ProfessionMenu();
		}
		else if (!_mainPlayer._bufferingStatus)
		{
			_statStruct = _mainPlayer._pStats._statStruct;
			Handle_StatCounterDisplays();
			Handle_AttributeListEntries();
		}
		void Handle_AttributeListEntries()
		{
			if (_phantomAtbPoints <= 0)
			{
				_phantomAtbPoints = 0;
			}
			if (!TabMenu._current._isOpen && _globalApplyingAttributePoints)
			{
				Reset_AttributeProjection();
			}
			_statsCell_applyAttributePointsButton.interactable = _globalApplyingAttributePoints;
			if (_phantomAtbPoints == _mainPlayer._pStats._currentAttributePoints)
			{
				_globalApplyingAttributePoints = false;
			}
			for (int i = 0; i < _listDataEntries.Count; i++)
			{
				AttributeListDataEntry obj = (AttributeListDataEntry)_listDataEntries[i];
				obj._isHighlighted = i == _attributeSelect;
				if (obj._isApplyingAttributePoints && !_globalApplyingAttributePoints)
				{
					_globalApplyingAttributePoints = true;
				}
			}
			if (_globalApplyingAttributePoints)
			{
				_statsCell_attributePointCounter.text = $"{_phantomAtbPoints}";
			}
			else
			{
				_statsCell_attributePointCounter.text = $"{_mainPlayer._pStats._currentAttributePoints}";
				_phantomAtbPoints = _mainPlayer._pStats._currentAttributePoints;
			}
			Handle_StatMenuGamepadControl();
		}
		void Handle_ProjectionDisplay()
		{
			if (_globalApplyingAttributePoints)
			{
				if (_projectionStatStruct._maxHealth > _statStruct._maxHealth)
				{
					_statsCell_maxHealthCounter.text += $" <color=lime>+({_projectionStatStruct._maxHealth - _statStruct._maxHealth})</color>";
				}
				if (_projectionStatStruct._maxMana > _statStruct._maxMana)
				{
					_statsCell_maxManaCounter.text += $" <color=lime>+({_projectionStatStruct._maxMana - _statStruct._maxMana})</color>";
				}
				if (_projectionStatStruct._maxStamina > _statStruct._maxStamina)
				{
					_statsCell_maxStaminaCounter.text += $" <color=lime>+({_projectionStatStruct._maxStamina - _statStruct._maxStamina})</color>";
				}
				if (_projectionStatStruct._attackPower > _statStruct._attackPower)
				{
					_statsCell_attackCounter.text += $" <color=lime>+({_projectionStatStruct._attackPower - _statStruct._attackPower})</color>";
				}
				if (_projectionStatStruct._magicPower > _statStruct._magicPower)
				{
					_statsCell_magicPowerCounter.text += $" <color=lime>+({_projectionStatStruct._magicPower - _statStruct._magicPower})</color>";
				}
				if (_projectionStatStruct._dexPower > _statStruct._dexPower)
				{
					_statsCell_rangePowerCounter.text += $" <color=lime>+({_projectionStatStruct._dexPower - _statStruct._dexPower})</color>";
				}
				if (_projectionStatStruct._criticalRate > _statStruct._criticalRate)
				{
					Text statsCell_critCounter = _statsCell_critCounter;
					statsCell_critCounter.text = statsCell_critCounter.text + " <color=lime>(" + (_projectionStatStruct._criticalRate * 100f).ToString("N2") + "%)</color>";
				}
				if (_projectionStatStruct._magicCriticalRate > _statStruct._magicCriticalRate)
				{
					Text statsCell_magicCritCounter = _statsCell_magicCritCounter;
					statsCell_magicCritCounter.text = statsCell_magicCritCounter.text + " <color=lime>(" + (_projectionStatStruct._magicCriticalRate * 100f).ToString("N2") + "%)</color>";
				}
				if (_projectionStatStruct._defense > _statStruct._defense)
				{
					_statsCell_defenseCounter.text += $" <color=lime>+({_projectionStatStruct._defense - _statStruct._defense})</color>";
				}
				if (_projectionStatStruct._magicDefense > _statStruct._magicDefense)
				{
					_statsCell_magicDefenseCounter.text += $" <color=lime>+({_projectionStatStruct._magicDefense - _statStruct._magicDefense})</color>";
				}
				if (_projectionStatStruct._evasion > _statStruct._evasion)
				{
					Text statsCell_evasionCounter = _statsCell_evasionCounter;
					statsCell_evasionCounter.text = statsCell_evasionCounter.text + " <color=lime>(" + (_projectionStatStruct._evasion * 100f).ToString("N2") + "%)</color>";
				}
			}
		}
		void Handle_StatCounterDisplays()
		{
			_statsCell_nicknameTag.text = _mainPlayer._nickname ?? "";
			_statsCell_levelCounter.text = $"{_mainPlayer._pStats._currentLevel}";
			_statsCell_raceTag.text = _mainPlayer._pVisual._playerAppearanceStruct._setRaceTag ?? "";
			if (_mainPlayer._pStats._currentLevel >= GameManager._current._statLogics._maxMainLevel)
			{
				_statsCell_experience.text = "MAX";
			}
			else
			{
				_statsCell_experience.text = $"{_mainPlayer._pStats._statStruct._experience}";
			}
			if ((bool)_mainPlayer._pStats._class)
			{
				if (_mainPlayer._pStats._syncClassTier <= 0)
				{
					_statsCell_baseClassTag.text = _mainPlayer._pStats._class._className ?? "";
				}
				else
				{
					_statsCell_baseClassTag.text = _mainPlayer._pStats._class._playerClassTiers[_mainPlayer._pStats._syncClassTier - 1]._classTierName ?? "";
				}
			}
			else
			{
				_statsCell_baseClassTag.text = GameManager._current._statLogics._emptyClassName ?? "";
			}
			if (_globalApplyingAttributePoints)
			{
				Handle_StatCounters();
				Handle_ProjectionDisplay();
			}
			else
			{
				Handle_StatCounters();
			}
		}
		void Handle_StatCounters()
		{
			StatStruct statStruct = _mainPlayer._pStats._statStruct;
			StatStruct baseStatStruct = _mainPlayer._pStats._baseStatStruct;
			StatStruct bonusStatStruct = _mainPlayer._pStats._bonusStatStruct;
			string text = "white";
			string text2 = "#5fd7ff";
			string text3 = "#e2725b";
			if (statStruct._maxHealth > baseStatStruct._maxHealth)
			{
				_statsCell_maxHealthCounter.text = "<color=" + text2 + ">";
			}
			else if (statStruct._maxHealth < baseStatStruct._maxHealth || bonusStatStruct._maxHealth < 0)
			{
				_statsCell_maxHealthCounter.text = "<color=" + text3 + ">";
			}
			else if (statStruct._maxHealth == baseStatStruct._maxHealth)
			{
				_statsCell_maxHealthCounter.text = "<color=" + text + ">";
			}
			_statsCell_maxHealthCounter.text += $"{statStruct._maxHealth}";
			_statsCell_maxHealthCounter.text += "</color>";
			if (statStruct._maxMana > baseStatStruct._maxMana)
			{
				_statsCell_maxManaCounter.text = "<color=" + text2 + ">";
			}
			else if (statStruct._maxMana < baseStatStruct._maxMana || bonusStatStruct._maxMana < 0)
			{
				_statsCell_maxManaCounter.text = "<color=" + text3 + ">";
			}
			else if (statStruct._maxMana == baseStatStruct._maxMana)
			{
				_statsCell_maxManaCounter.text = "<color=" + text + ">";
			}
			_statsCell_maxManaCounter.text += $"{statStruct._maxMana}";
			_statsCell_maxManaCounter.text += "</color>";
			if (statStruct._maxStamina > baseStatStruct._maxStamina)
			{
				_statsCell_maxStaminaCounter.text = "<color=" + text2 + ">";
			}
			else if (statStruct._maxStamina < baseStatStruct._maxStamina || bonusStatStruct._maxStamina < 0)
			{
				_statsCell_maxStaminaCounter.text = "<color=" + text3 + ">";
			}
			else if (statStruct._maxStamina == baseStatStruct._maxStamina)
			{
				_statsCell_maxStaminaCounter.text = "<color=" + text + ">";
			}
			_statsCell_maxStaminaCounter.text += $"{statStruct._maxStamina}";
			_statsCell_maxStaminaCounter.text += "</color>";
			int num = statStruct._attackPower;
			if (statStruct._attackPower > baseStatStruct._attackPower)
			{
				_statsCell_attackCounter.text = "<color=" + text2 + ">";
			}
			else if (statStruct._attackPower < baseStatStruct._attackPower || bonusStatStruct._attackPower < 0)
			{
				_statsCell_attackCounter.text = "<color=" + text3 + ">";
			}
			else if (statStruct._attackPower == baseStatStruct._attackPower)
			{
				_statsCell_attackCounter.text = "<color=" + text + ">";
			}
			if (statStruct._attackPower < 0)
			{
				num = 0;
			}
			_statsCell_attackCounter.text += $"{num}";
			_statsCell_attackCounter.text += "</color>";
			int num2 = statStruct._dexPower;
			if (statStruct._dexPower > baseStatStruct._dexPower)
			{
				_statsCell_rangePowerCounter.text = "<color=" + text2 + ">";
			}
			else if (statStruct._dexPower < baseStatStruct._dexPower || bonusStatStruct._dexPower < 0)
			{
				_statsCell_rangePowerCounter.text = "<color=" + text3 + ">";
			}
			else if (statStruct._dexPower == baseStatStruct._dexPower)
			{
				_statsCell_rangePowerCounter.text = "<color=" + text + ">";
			}
			if (num2 < 0)
			{
				num2 = 0;
			}
			_statsCell_rangePowerCounter.text += $"{num2}";
			_statsCell_rangePowerCounter.text += "</color>";
			int num3 = statStruct._magicPower;
			if (statStruct._magicPower > baseStatStruct._magicPower)
			{
				_statsCell_magicPowerCounter.text = "<color=" + text2 + ">";
			}
			else if (statStruct._magicPower < baseStatStruct._magicPower || bonusStatStruct._magicPower < 0)
			{
				_statsCell_magicPowerCounter.text = "<color=" + text3 + ">";
			}
			else if (statStruct._magicPower == baseStatStruct._magicPower)
			{
				_statsCell_magicPowerCounter.text = "<color=" + text + ">";
			}
			if (num3 < 0)
			{
				num3 = 0;
			}
			_statsCell_magicPowerCounter.text += $"{num3}";
			_statsCell_magicPowerCounter.text += "</color>";
			int num4 = statStruct._defense;
			if (statStruct._defense > baseStatStruct._defense)
			{
				_statsCell_defenseCounter.text = "<color=" + text2 + ">";
			}
			else if (statStruct._defense < baseStatStruct._defense || bonusStatStruct._defense < 0)
			{
				_statsCell_defenseCounter.text = "<color=" + text3 + ">";
			}
			else if (statStruct._defense == baseStatStruct._defense)
			{
				_statsCell_defenseCounter.text = "<color=" + text + ">";
			}
			if (num4 < 0)
			{
				num4 = 0;
			}
			_statsCell_defenseCounter.text += $"{num4}";
			_statsCell_defenseCounter.text += "</color>";
			int num5 = statStruct._magicDefense;
			if (statStruct._magicDefense > baseStatStruct._magicDefense)
			{
				_statsCell_magicDefenseCounter.text = "<color=" + text2 + ">";
			}
			else if (statStruct._magicDefense < baseStatStruct._magicDefense || bonusStatStruct._magicDefense < 0)
			{
				_statsCell_magicDefenseCounter.text = "<color=" + text3 + ">";
			}
			else if (statStruct._magicDefense == baseStatStruct._magicDefense)
			{
				_statsCell_magicDefenseCounter.text = "<color=" + text + ">";
			}
			if (num5 < 0)
			{
				num5 = 0;
			}
			_statsCell_magicDefenseCounter.text += $"{num5}";
			_statsCell_magicDefenseCounter.text += "</color>";
			float num6 = _mainPlayer._pMove._movSpeed / GameManager._current._statLogics._baseMoveSpeed * 100f;
			_statsCell_movementSpeedCounter.text = num6.ToString("F1") + "%";
			if (statStruct._evasion > baseStatStruct._evasion)
			{
				_statsCell_evasionCounter.text = "<color=" + text2 + ">";
			}
			else if (statStruct._evasion <= baseStatStruct._evasion)
			{
				_statsCell_evasionCounter.text = "<color=" + text + ">";
			}
			Text statsCell_evasionCounter = _statsCell_evasionCounter;
			statsCell_evasionCounter.text = statsCell_evasionCounter.text + (statStruct._evasion * 100f).ToString("N2") + "%";
			_statsCell_evasionCounter.text += "</color>";
			if (statStruct._criticalRate > baseStatStruct._criticalRate)
			{
				_statsCell_critCounter.text = "<color=" + text2 + ">";
			}
			else if (statStruct._criticalRate <= baseStatStruct._criticalRate)
			{
				_statsCell_critCounter.text = "<color=" + text + ">";
			}
			Text statsCell_critCounter = _statsCell_critCounter;
			statsCell_critCounter.text = statsCell_critCounter.text + (statStruct._criticalRate * 100f).ToString("N2") + "%";
			_statsCell_critCounter.text += "</color>";
			if (statStruct._magicCriticalRate > baseStatStruct._magicCriticalRate)
			{
				_statsCell_magicCritCounter.text = "<color=" + text2 + ">";
			}
			else if (statStruct._magicCriticalRate <= baseStatStruct._magicCriticalRate)
			{
				_statsCell_magicCritCounter.text = "<color=" + text + ">";
			}
			Text statsCell_magicCritCounter = _statsCell_magicCritCounter;
			statsCell_magicCritCounter.text = statsCell_magicCritCounter.text + (statStruct._magicCriticalRate * 100f).ToString("N2") + "%";
			_statsCell_magicCritCounter.text += "</color>";
			if (statStruct._fireResist > baseStatStruct._fireResist)
			{
				_statsCell_fireResistCounter.text = "<color=" + text2 + ">";
			}
			else if (statStruct._fireResist < 0)
			{
				_statsCell_fireResistCounter.text = "<color=" + text3 + ">";
			}
			else
			{
				_statsCell_fireResistCounter.text = "<color=" + text + ">";
			}
			_statsCell_fireResistCounter.text += $"{statStruct._fireResist}";
			_statsCell_fireResistCounter.text += "</color>";
			if (statStruct._waterResist > baseStatStruct._waterResist)
			{
				_statsCell_waterResistCounter.text = "<color=" + text2 + ">";
			}
			else if (statStruct._waterResist < 0)
			{
				_statsCell_waterResistCounter.text = "<color=" + text3 + ">";
			}
			else
			{
				_statsCell_waterResistCounter.text = "<color=" + text + ">";
			}
			_statsCell_waterResistCounter.text += $"{statStruct._waterResist}";
			_statsCell_waterResistCounter.text += "</color>";
			if (statStruct._natureResist > baseStatStruct._natureResist)
			{
				_statsCell_natureResistCounter.text = "<color=" + text2 + ">";
			}
			else if (statStruct._natureResist < 0)
			{
				_statsCell_natureResistCounter.text = "<color=" + text3 + ">";
			}
			else
			{
				_statsCell_natureResistCounter.text = "<color=" + text + ">";
			}
			_statsCell_natureResistCounter.text += $"{statStruct._natureResist}";
			_statsCell_natureResistCounter.text += "</color>";
			if (statStruct._earthResist > baseStatStruct._earthResist)
			{
				_statsCell_earthResistCounter.text = "<color=" + text2 + ">";
			}
			else if (statStruct._earthResist < 0)
			{
				_statsCell_earthResistCounter.text = "<color=" + text3 + ">";
			}
			else
			{
				_statsCell_earthResistCounter.text = "<color=" + text + ">";
			}
			_statsCell_earthResistCounter.text += $"{statStruct._earthResist}";
			_statsCell_earthResistCounter.text += "</color>";
			if (statStruct._holyResist > baseStatStruct._holyResist)
			{
				_statsCell_holyResistCounter.text = "<color=" + text2 + ">";
			}
			else if (statStruct._holyResist < 0)
			{
				_statsCell_holyResistCounter.text = "<color=" + text3 + ">";
			}
			else
			{
				_statsCell_holyResistCounter.text = "<color=" + text + ">";
			}
			_statsCell_holyResistCounter.text += $"{statStruct._holyResist}";
			_statsCell_holyResistCounter.text += "</color>";
			if (statStruct._shadowResist > baseStatStruct._shadowResist)
			{
				_statsCell_shadowResistCounter.text = "<color=" + text2 + ">";
			}
			else if (statStruct._shadowResist < 0)
			{
				_statsCell_shadowResistCounter.text = "<color=" + text3 + ">";
			}
			else
			{
				_statsCell_shadowResistCounter.text = "<color=" + text + ">";
			}
			_statsCell_shadowResistCounter.text += $"{statStruct._shadowResist}";
			_statsCell_shadowResistCounter.text += "</color>";
		}
		void Handle_StatMenuGamepadControl()
		{
			_statsCell_applyAttributeGamepadIco.enabled = InputControlManager.current._setGamepadType != GamepadType.Keyboard && _statsCell_applyAttributePointsButton.interactable;
			_gamepad_statMenuTabCursor.gameObject.SetActive(InputControlManager.current._setGamepadType != GamepadType.Keyboard);
			if (InputControlManager.current._setGamepadType != GamepadType.Keyboard && TabMenu._current.CurrentCellSelection == 1)
			{
				bool num = InputControlManager.current.DpadUp(DpadHoldType.OnDown) || GamepadInput.GetButtonDown(GamepadButton.RightStick_Up);
				bool flag = InputControlManager.current.DpadDown(DpadHoldType.OnDown) || GamepadInput.GetButtonDown(GamepadButton.RightStick_Down);
				if (num)
				{
					AttributeSelect--;
					TabMenu._current._gamepadSelectAsrc.Play();
				}
				if (flag)
				{
					AttributeSelect++;
					TabMenu._current._gamepadSelectAsrc.Play();
				}
				if (_globalApplyingAttributePoints && GamepadInput.GetButtonDown(GamepadButton.Button_South))
				{
					Apply_AttributePoints();
				}
				if (_professionMenuElement.isEnabled)
				{
					_gamepad_statMenuTabCursor.anchoredPosition = _statsCell_professionTabButton.GetComponent<RectTransform>().anchoredPosition;
				}
				else
				{
					_gamepad_statMenuTabCursor.anchoredPosition = _statsCell_statTabButton.GetComponent<RectTransform>().anchoredPosition;
				}
				bool num2 = GamepadInput.GetButtonDown(GamepadButton.RightStick_Left) || InputControlManager.current.DpadLeft(DpadHoldType.OnDown);
				bool flag2 = GamepadInput.GetButtonDown(GamepadButton.RightStick_Right) || InputControlManager.current.DpadRight(DpadHoldType.OnDown);
				if (num2)
				{
					Disable_ProfessionMenu();
					TabMenu._current._gamepadSelectAsrc.Play();
				}
				if (flag2)
				{
					Enable_ProfessionMenu();
					TabMenu._current._gamepadSelectAsrc.Play();
				}
			}
		}
	}

	public void Project_PhantomAttributeStats()
	{
		PlayerStats pStats = Player._mainPlayer._pStats;
		if (pStats.isLocalPlayer)
		{
			_projectionStatStruct = default;
			for (int i = 0; i < GameManager._current._statLogics._statAttributes.Length; i++)
			{
				AttributeListDataEntry attributeListDataEntry = (AttributeListDataEntry)_listDataEntries[i];
				GameManager._current._statLogics._statAttributes[i].Project_AttributeCalculation(pStats, attributeListDataEntry._heldAttributeValue, _projectionStatStruct);
			}
			pStats.Apply_EquipmentStats(_projectionStatStruct);
			pStats.Apply_BonusStats(_projectionStatStruct);
		}
	}

	public void Reset_AttributeProjection()
	{
		if (_globalApplyingAttributePoints)
		{
			for (int i = 0; i < _listDataEntries.Count; i++)
			{
				AttributeListDataEntry obj = (AttributeListDataEntry)_listDataEntries[i];
				obj._isApplyingAttributePoints = false;
				obj._pointsPutIn = 0;
			}
			_projectionStatStruct = default;
			_globalApplyingAttributePoints = false;
			AttributeSelect = 0;
		}
	}

	public void Apply_AttributePoints()
	{
		if (!Player._mainPlayer)
		{
			return;
		}
		PlayerStats component = Player._mainPlayer.GetComponent<PlayerStats>();
		int[] array = new int[_listDataEntries.Count];
		int[] array2 = new int[_listDataEntries.Count];
		for (int i = 0; i < _listDataEntries.Count; i++)
		{
			AttributeListDataEntry attributeListDataEntry = (AttributeListDataEntry)_listDataEntries[i];
			array[i] = attributeListDataEntry._heldAttributeValue;
			array2[i] = attributeListDataEntry._pointsPutIn;
			if (attributeListDataEntry._heldAttributeValue > component._playerAttributes[attributeListDataEntry._dataID]._attributeValue)
			{
				attributeListDataEntry._attributeAnimator.SetTrigger("_applyAttribute");
			}
			attributeListDataEntry._isApplyingAttributePoints = false;
		}
		component.Cmd_ApplyAttributePoints(array, array2);
		_globalApplyingAttributePoints = false;
	}

	public void Create_StatListData(AttributeStruct _attributeStruct)
	{
		GameObject obj = Object.Instantiate(_listDataObject_prefab);
		obj.transform.SetParent(_cell_ScrollViewContent);
		obj.transform.localScale = Vector3.one;
		obj.transform.position = Vector3.zero;
		AttributeListDataEntry component = obj.GetComponent<AttributeListDataEntry>();
		component._dataID = _attributeStruct._attributeID;
		_listDataEntries.Add(component);
	}

	public void Create_ProfessionSlot(ProfessionStruct _professionStruct)
	{
		GameObject obj = Object.Instantiate(_professionSlotPrefab);
		obj.transform.SetParent(_professionSlotContent);
		obj.transform.localScale = Vector3.one;
		obj.transform.position = Vector3.zero;
		ProfessionCellSlot component = obj.GetComponent<ProfessionCellSlot>();
		component._professionID = _professionStruct._professionID;
		component._scriptableProfession = GameManager._current._statLogics._scriptableProfessions[component._professionID];
		component._professionStruct = _professionStruct;
		_professionCellSlots.Add(component);
	}

	public void ToolTip_DisplayBaseStat(string _statTag)
	{
		PlayerStats pStats = _mainPlayer._pStats;
		ToolTipManager._current.Apply_GenericToolTip(string.Empty);
		string text = "<b>Base Stat:</b> <i>";
		switch (_statTag)
		{
		case "MaxHp":
			text += $"{pStats._baseStatStruct._maxHealth}</i> (Max Health)";
			break;
		case "MaxMp":
			text += $"{pStats._baseStatStruct._maxMana}</i> (Max Mana)";
			break;
		case "MaxStam":
			text += $"{pStats._baseStatStruct._maxStamina}</i> (Max Stamina)";
			break;
		case "AttackPow":
			text += $"{pStats._baseStatStruct._attackPower}</i> (Attack Power)";
			break;
		case "RangePow":
			text += $"{pStats._baseStatStruct._dexPower}</i> (Dex Power)";
			break;
		case "MagicPow":
			text += $"{pStats._baseStatStruct._magicPower}</i> (Magic Power)";
			break;
		case "Defense":
			text += $"{pStats._baseStatStruct._defense}</i> (Defense)";
			break;
		case "MagicDef":
			text += $"{pStats._baseStatStruct._magicDefense}</i> (Magic Defense)";
			break;
		case "Crit":
			text = text + (pStats._baseStatStruct._criticalRate * 100f).ToString("N2") + "%</i> (Critical %)";
			break;
		case "MagicCrit":
			text = text + (pStats._baseStatStruct._magicCriticalRate * 100f).ToString("N2") + "%</i> (Magic Critical %)";
			break;
		case "Evasion":
			text = text + (pStats._baseStatStruct._evasion * 100f).ToString("N2") + "%</i> (Evasion %)";
			break;
		}
		ToolTipManager._current._genericToolTip.Set_TooltipAnchorPos(new Vector2(-135f, 0f));
		ToolTipManager._current.Apply_GenericToolTip(text);
		ToolTipManager._current._genericToolTip.Enable_ToolTip();
	}

	public void ToolTip_DisplayString(string _tag)
	{
		ToolTipManager._current._genericToolTip.Set_TooltipAnchorPos(new Vector2(-60f, 60f));
		ToolTipManager._current.Apply_GenericToolTip(_tag);
		ToolTipManager._current._genericToolTip.Enable_ToolTip();
	}

	public void Disable_Tooltip()
	{
		ToolTipManager._current._genericToolTip.Disable_ToolTip();
	}

	public void Enable_ProfessionMenu()
	{
		_professionMenuElement.transform.SetAsLastSibling();
		_professionMenuElement.isEnabled = true;
	}

	public void Disable_ProfessionMenu()
	{
		_professionMenuElement.isEnabled = false;
	}
}
