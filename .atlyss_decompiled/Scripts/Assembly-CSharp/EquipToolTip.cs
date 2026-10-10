using UnityEngine;
using UnityEngine.UI;

public class EquipToolTip : TooltipElement
{
	[Header("Equip ToolTip")]
	public ScriptableEquipment _setScriptEquip;

	public ItemData _setItemData;

	public float _dataDisplayBuffer;

	private StatStruct _statStruct;

	[SerializeField]
	private Text _compareDisplayText;

	[SerializeField]
	private Image _compareDisplayBackdrop;

	[SerializeField]
	private Image _compareGamepadIco;

	private ScriptableEquipment _cachedCompareEquip;

	private ItemData _cachedCompareItemData;

	[Space]
	[SerializeField]
	private Sprite _gambleIco_gear;

	[SerializeField]
	private Image _equipIconBackground;

	[SerializeField]
	private Image _equippedIcon;

	[SerializeField]
	private Image _errorIcon;

	[SerializeField]
	private Image _dyableIcon;

	[SerializeField]
	private Text _equipToolTipType;

	[Space]
	[SerializeField]
	private GameObject _weaponTypeIconObject;

	[SerializeField]
	private GameObject _damageTypeIconParent;

	[SerializeField]
	private Image _weaponTypeIcon;

	[Space]
	[SerializeField]
	private GameObject _weaponDamageTransmuteTab;

	[SerializeField]
	private Text _weaponDamageTransmuteText;

	[SerializeField]
	private Image _weaponDamageTransmuteIcon;

	[Space]
	[SerializeField]
	private Text _equipClassRequirement;

	[SerializeField]
	private Image _classRequirementIcon;

	[SerializeField]
	private Text _equipLevelRequirement;

	[SerializeField]
	private Text _equipStatsDisplay;

	[Space]
	[SerializeField]
	private GameObject _lvlRequirementTab;

	[SerializeField]
	private GameObject _classRequirementTab;

	[Header("Equip Stat Display")]
	[SerializeField]
	private GameObject _statDisplayTab;

	[SerializeField]
	private GameObject _attackPowerCell;

	[SerializeField]
	private GameObject _dexPowerCell;

	[SerializeField]
	private GameObject _magicPowerCell;

	[SerializeField]
	private GameObject _criticalCell;

	[SerializeField]
	private GameObject _mgkCritCell;

	[SerializeField]
	private GameObject _maxHealthCell;

	[SerializeField]
	private GameObject _maxManaCell;

	[SerializeField]
	private GameObject _maxStaminaCell;

	[SerializeField]
	private GameObject _evasionCell;

	[SerializeField]
	private GameObject _defenseCell;

	[SerializeField]
	private GameObject _mgkDefenseCell;

	[SerializeField]
	private GameObject _resistFireCell;

	[SerializeField]
	private GameObject _resistWaterCell;

	[SerializeField]
	private GameObject _resistNatureCell;

	[SerializeField]
	private GameObject _resistEarthCell;

	[SerializeField]
	private GameObject _resistHolyCell;

	[SerializeField]
	private GameObject _resistShadowCell;

	[SerializeField]
	private Text _attackPowerTag;

	[SerializeField]
	private Text _attackPowerCounter;

	[SerializeField]
	private Text _dexPowerTag;

	[SerializeField]
	private Text _dexPowerCounter;

	[SerializeField]
	private Text _magicPowerTag;

	[SerializeField]
	private Text _magicPowerCounter;

	[SerializeField]
	private Text _criticalTag;

	[SerializeField]
	private Text _criticalCounter;

	[SerializeField]
	private Text _magicCriticalTag;

	[SerializeField]
	private Text _magicCriticalCounter;

	[SerializeField]
	private Text _maxHealthTag;

	[SerializeField]
	private Text _maxHealthCounter;

	[SerializeField]
	private Text _maxManaTag;

	[SerializeField]
	private Text _maxManaCounter;

	[SerializeField]
	private Text _maxStaminaTag;

	[SerializeField]
	private Text _maxStaminaCounter;

	[SerializeField]
	private Text _evasionTag;

	[SerializeField]
	private Text _evasionCounter;

	[SerializeField]
	private Text _defenseTag;

	[SerializeField]
	private Text _defenseCounter;

	[SerializeField]
	private Text _magicDefenseTag;

	[SerializeField]
	private Text _magicDefenseCounter;

	[SerializeField]
	private Text _fireResistTag;

	[SerializeField]
	private Text _fireResistCounter;

	[SerializeField]
	private Text _waterResistTag;

	[SerializeField]
	private Text _waterResistCounter;

	[SerializeField]
	private Text _natureResistTag;

	[SerializeField]
	private Text _natureResistCounter;

	[SerializeField]
	private Text _earthResistTag;

	[SerializeField]
	private Text _earthResistCounter;

	[SerializeField]
	private Text _holyResistTag;

	[SerializeField]
	private Text _holyResistCounter;

	[SerializeField]
	private Text _shadowResistTag;

	[SerializeField]
	private Text _shadowResistCounter;

	[Space]
	[SerializeField]
	private GameObject _vendorValueTab;

	[SerializeField]
	private Text _vendorValueCounter;

	public Image _currencyIcon;

	public ScriptableItem _specialCurrencyItem;

	public bool _isGambleItem;

	public int _vendorValue;

	public override void Disable_ToolTip()
	{
		base.Disable_ToolTip();
		_setScriptEquip = null;
		_specialCurrencyItem = null;
		_setItemData = new ItemData();
	}

	public override void Update()
	{
		base.Update();
		if (!TabMenu._current)
		{
			Disable_ToolTip();
			return;
		}
		if (!TabMenu._current._isOpen)
		{
			Disable_ToolTip();
			return;
		}
		if (TabMenu._current._selectedMenuCell != TabMenu._current._itemCell)
		{
			Disable_ToolTip();
			return;
		}
		if (TabMenu._current._itemTradeMode)
		{
			if ((bool)_specialCurrencyItem)
			{
				int num = Player._mainPlayer._pInventory.Check_ItemQuantity(_specialCurrencyItem);
				string text = "<color=lime>";
				if (num < _vendorValue)
				{
					text = "<color=#e69138>";
				}
				_vendorValueCounter.text = $"{text}{_specialCurrencyItem._itemName} ({num} / {_vendorValue})</color>";
				_currencyIcon.sprite = _specialCurrencyItem._itemIcon;
			}
			else
			{
				_vendorValueCounter.text = $"{_vendorValue} {GameManager._current._statLogics._currencyName}";
				_currencyIcon.sprite = GameManager._current._statLogics._currencyIcon;
			}
		}
		_vendorValueTab.SetActive(TabMenu._current._itemTradeMode);
		if ((bool)_setScriptEquip)
		{
			Apply_EquipStats(_setScriptEquip, _setItemData);
		}
		bool flag = (bool)_cachedCompareEquip && !_setItemData._isEquipped && !_isGambleItem;
		bool flag2 = Input.GetKey(InputControlManager.current._interact) || GamepadInput.GetButton(GamepadButton.Button_West);
		bool num2 = Input.GetKeyDown(InputControlManager.current._interact) || GamepadInput.GetButtonDown(GamepadButton.Button_West);
		bool flag3 = Input.GetKeyUp(InputControlManager.current._interact) || GamepadInput.GetButtonUp(GamepadButton.Button_West);
		if (num2 | flag3)
		{
			_dataDisplayBuffer = 0f;
		}
		_compareDisplayText.enabled = flag && !flag2;
		_compareDisplayBackdrop.enabled = _compareDisplayText.enabled;
		_compareGamepadIco.enabled = _compareDisplayBackdrop.enabled && InputControlManager.current._setGamepadType != GamepadType.Keyboard;
		if (_compareDisplayText.enabled)
		{
			if (InputControlManager.current._setGamepadType == GamepadType.Keyboard)
			{
				string text2 = InputControlManager.current.Convert_KeyCodeName(InputControlManager.current._interact.ToString());
				_compareDisplayText.text = "Compare Gear [" + text2 + "]";
			}
			else
			{
				string text2 = InputControlManager.current.Convert_KeyCodeName(InputControlManager.current._interact.ToString());
				_compareDisplayText.text = "Compare Gear ";
			}
		}
		if (flag & flag2)
		{
			Apply_EquipStats(_cachedCompareEquip, _cachedCompareItemData);
		}
		if (_toolTipMenuElement.isEnabled)
		{
			_dataDisplayBuffer += Time.deltaTime;
		}
		else
		{
			_dataDisplayBuffer = 0f;
		}
	}

	public void Apply_EquipStats(ScriptableEquipment _scriptEquip, ItemData _itemData)
	{
		if (!_scriptEquip || _itemData == null || _dataDisplayBuffer >= 0.8f)
		{
			return;
		}
		PlayerStats _pStats = Player._mainPlayer._pStats;
		PlayerEquipment _pEquip = Player._mainPlayer._pEquipment;
		PlayerCombat _pCombat = Player._mainPlayer._pCombat;
		if (_isGambleItem)
		{
			Display_GambleEquipTooltip();
			return;
		}
		ItemRarity _itemRarity = _scriptEquip._itemRarity;
		string _modifierTag = "";
		_statStruct = _scriptEquip._statArray;
		_statStruct = Apply_ModifierToStatStruct(_statStruct, _itemData);
		_toolTipName.text = _modifierTag + _scriptEquip._itemName;
		_toolTipSubName.text = $"[{_itemRarity}]";
		_toolTipIcon.sprite = _scriptEquip._itemIcon;
		if ((bool)_equippedIcon && _itemData != null)
		{
			_equippedIcon.enabled = _itemData._isEquipped;
		}
		_errorIcon.enabled = !_scriptEquip.CanEquipItem(Player._mainPlayer._pStats, _isClickPrompt: false);
		_statDisplayTab.SetActive(value: true);
		_lvlRequirementTab.SetActive(_scriptEquip._equipmentLevel > 1);
		_classRequirementTab.SetActive(_scriptEquip._classRequirement);
		if (_scriptEquip._equipmentLevel > 1)
		{
			_equipLevelRequirement.text = $"Lv-{_scriptEquip._equipmentLevel}";
		}
		else
		{
			_equipLevelRequirement.text = string.Empty;
		}
		if (_pStats._currentLevel < _scriptEquip._equipmentLevel)
		{
			_equipLevelRequirement.color = Color.red;
		}
		else
		{
			_equipLevelRequirement.color = Color.white;
		}
		if ((bool)_scriptEquip._classRequirement)
		{
			_equipClassRequirement.text = "Class: " + _scriptEquip._classRequirement._className;
			_equipClassRequirement.color = Color.white;
			_classRequirementIcon.sprite = _scriptEquip._classRequirement._classIcon;
			if ((bool)_scriptEquip._classRequirement && _pStats._class != _scriptEquip._classRequirement)
			{
				_equipClassRequirement.color = Color.red;
			}
			if (!_pStats._class && (bool)_scriptEquip._classRequirement)
			{
				_equipClassRequirement.color = Color.red;
			}
		}
		_toolTipDescription.text = "";
		if (_scriptEquip._equipConditionActivation._equipConditionSlot != null && (bool)_scriptEquip._equipConditionActivation._equipConditionSlot._scriptableCondition)
		{
			if (!string.IsNullOrWhiteSpace(_toolTipDescription.text))
			{
				_toolTipDescription.text += "\n";
			}
			string equipConditionDescription = _scriptEquip._equipConditionActivation._equipConditionDescription;
			string text = "";
			if (_scriptEquip._equipConditionActivation._equipConditionSlot._conditionDuration > 0)
			{
				text = $"<color=yellow> Lasts for <color=yellow>{_scriptEquip._equipConditionActivation._equipConditionSlot._conditionDuration} sec.</color></color>";
			}
			Text toolTipDescription = _toolTipDescription;
			toolTipDescription.text = toolTipDescription.text + "<color=lime>Equip: " + equipConditionDescription + text + "</color>";
		}
		if (_scriptEquip._equipSkillBonuses != null && _scriptEquip._equipSkillBonuses.Length != 0)
		{
			SkillBonus[] equipSkillBonuses = _scriptEquip._equipSkillBonuses;
			foreach (SkillBonus skillBonus in equipSkillBonuses)
			{
				if (!skillBonus._scriptableSkill)
				{
					continue;
				}
				if (skillBonus._skillPowerBonus > 0f)
				{
					if (!string.IsNullOrWhiteSpace(_toolTipDescription.text))
					{
						_toolTipDescription.text += "\n";
					}
					string skillBonusDescriptor = skillBonus._skillBonusDescriptor;
					Text toolTipDescription2 = _toolTipDescription;
					toolTipDescription2.text = toolTipDescription2.text + "<color=lime>Equip: " + skillBonusDescriptor + "</color>";
				}
				if (skillBonus._skillConditionPowerBonus > 0f)
				{
					if (!string.IsNullOrWhiteSpace(_toolTipDescription.text))
					{
						_toolTipDescription.text += "\n";
					}
					_toolTipDescription.text += $"<color=lime>Equip: Increased condition power of {skillBonus._scriptableSkill._skillName} by {skillBonus._skillConditionPowerBonus * 100f}%.</color>";
				}
				if ((float)skillBonus._skillConditionDurationBonus > 0f)
				{
					if (!string.IsNullOrWhiteSpace(_toolTipDescription.text))
					{
						_toolTipDescription.text += "\n";
					}
					_toolTipDescription.text += $"<color=lime>Equip: Increased condition duration of {skillBonus._scriptableSkill._skillName} by {skillBonus._skillConditionDurationBonus}sec.</color>";
				}
			}
		}
		if (_scriptEquip._equipWeaponTypeBonuses != null && _scriptEquip._equipWeaponTypeBonuses.Length != 0)
		{
			WeaponTypeBonus[] equipWeaponTypeBonuses = _scriptEquip._equipWeaponTypeBonuses;
			foreach (WeaponTypeBonus weaponTypeBonus in equipWeaponTypeBonuses)
			{
				if ((bool)weaponTypeBonus._scriptableWeaponType)
				{
					if (!string.IsNullOrWhiteSpace(_toolTipDescription.text))
					{
						_toolTipDescription.text += "\n";
					}
					string weaponTypeBonusDescriptor = weaponTypeBonus._weaponTypeBonusDescriptor;
					Text toolTipDescription3 = _toolTipDescription;
					toolTipDescription3.text = toolTipDescription3.text + "<color=lime>Equip: " + weaponTypeBonusDescriptor + "</color>";
				}
			}
		}
		if (_scriptEquip._equipType != ItemEquipType.ARMOR)
		{
			_dyableIcon.enabled = false;
		}
		else
		{
			ScriptableArmor scriptableArmor = (ScriptableArmor)_scriptEquip;
			_dyableIcon.enabled = scriptableArmor._canDyeArmor;
		}
		if (!string.IsNullOrWhiteSpace(_toolTipDescription.text) && !string.IsNullOrWhiteSpace(_scriptEquip._itemDescription))
		{
			_toolTipDescription.text += "\n\n";
		}
		_toolTipDescription.text += _scriptEquip._itemDescription;
		_toolTipDescription.gameObject.SetActive(!string.IsNullOrWhiteSpace(_toolTipDescription.text));
		_toolTipName.color = GameManager._current._statLogics._itemRarityColors[(uint)_itemRarity];
		_toolTipSubName.color = GameManager._current._statLogics._itemRarityColors[(uint)_itemRarity];
		_equipIconBackground.color = GameManager._current._statLogics._itemRarityColors[(uint)_itemRarity];
		_equipStatsDisplay.text = string.Empty;
		_equipToolTipType.text = string.Empty;
		_equipToolTipType.color = Color.white;
		bool _priorStat = false;
		if (_scriptEquip.GetType() == typeof(ScriptableHelm))
		{
			_equipToolTipType.text += "Helm (Armor)";
			if (!_pEquip._equips["Helm"]._scriptableEquip)
			{
				_cachedCompareEquip = null;
				_cachedCompareItemData = new ItemData();
			}
			else
			{
				_cachedCompareEquip = _pEquip._equips["Helm"]._scriptableEquip;
				_cachedCompareItemData = _pEquip._equips["Helm"]._heldItem;
			}
		}
		else if (_scriptEquip.GetType() == typeof(ScriptableChestpiece))
		{
			_equipToolTipType.text += "Chestpiece (Armor)";
			if (!_pEquip._equips["Chest"]._scriptableEquip)
			{
				_cachedCompareEquip = null;
				_cachedCompareItemData = new ItemData();
			}
			else
			{
				_cachedCompareEquip = _pEquip._equips["Chest"]._scriptableEquip;
				_cachedCompareItemData = _pEquip._equips["Chest"]._heldItem;
			}
		}
		else if (_scriptEquip.GetType() == typeof(ScriptableLeggings))
		{
			_equipToolTipType.text += "Leggings (Armor)";
			if (!_pEquip._equips["Leggings"]._scriptableEquip)
			{
				_cachedCompareEquip = null;
				_cachedCompareItemData = new ItemData();
			}
			else
			{
				_cachedCompareEquip = _pEquip._equips["Leggings"]._scriptableEquip;
				_cachedCompareItemData = _pEquip._equips["Leggings"]._heldItem;
			}
		}
		else if (_scriptEquip.GetType() == typeof(ScriptableCape))
		{
			_equipToolTipType.text += "Cape (Armor)";
			if (!_pEquip._equips["Cape"]._scriptableEquip)
			{
				_cachedCompareEquip = null;
				_cachedCompareItemData = new ItemData();
			}
			_cachedCompareEquip = _pEquip._equips["Cape"]._scriptableEquip;
			_cachedCompareItemData = _pEquip._equips["Cape"]._heldItem;
		}
		else if (_scriptEquip.GetType() == typeof(ScriptableRing))
		{
			_equipToolTipType.text += "Trinket (Armor)";
			if (!_pEquip._equips["Ring"]._scriptableEquip)
			{
				_cachedCompareEquip = null;
				_cachedCompareItemData = new ItemData();
			}
			else
			{
				_cachedCompareEquip = _pEquip._equips["Ring"]._scriptableEquip;
				_cachedCompareItemData = _pEquip._equips["Ring"]._heldItem;
			}
		}
		_equipStatsDisplay.gameObject.SetActive(value: false);
		_weaponDamageTransmuteTab.SetActive(value: false);
		_damageTypeIconParent.SetActive(_scriptEquip.GetType() == typeof(ScriptableWeapon));
		_weaponTypeIconObject.SetActive(_scriptEquip.GetType() == typeof(ScriptableWeapon));
		if (_scriptEquip.GetType() == typeof(ScriptableWeapon))
		{
			Init_WeaponTooltip((ScriptableWeapon)_scriptEquip);
		}
		else if (_scriptEquip.GetType() == typeof(ScriptableShield))
		{
			Init_ShieldToolTip();
		}
		StatStruct statStruct = default;
		if ((bool)_cachedCompareEquip && _cachedCompareItemData != null)
		{
			statStruct = Apply_ModifierToStatStruct(_cachedCompareEquip._statArray, _cachedCompareItemData);
		}
		if (_statStruct._maxHealth > 0)
		{
			_maxHealthCounter.text = $"<color=lime>+{_statStruct._maxHealth}</color>";
			_maxHealthTag.color = Color.white;
			_priorStat = true;
		}
		if ((bool)_cachedCompareEquip && statStruct._maxHealth > _statStruct._maxHealth)
		{
			_maxHealthCounter.text = $"<color=#e2725b>+{_statStruct._maxHealth}</color>";
			_maxHealthTag.color = Color.white;
		}
		if (_statStruct._maxHealth > _scriptEquip._statArray._maxHealth)
		{
			_maxHealthCounter.text = $"<color=cyan>+{_statStruct._maxHealth}</color>";
			if ((bool)_cachedCompareEquip && statStruct._maxHealth > _statStruct._maxHealth)
			{
				_maxHealthCounter.text = $"<color=#e2725b>+{_statStruct._maxHealth}</color>";
			}
			_maxHealthTag.color = Color.cyan;
		}
		_maxHealthCell.SetActive(_statStruct._maxHealth > 0);
		if (_statStruct._maxMana > 0)
		{
			_maxManaCounter.text = $"<color=lime>+{_statStruct._maxMana}</color>";
			_maxManaTag.color = Color.white;
			_priorStat = true;
		}
		if ((bool)_cachedCompareEquip && statStruct._maxMana > _statStruct._maxMana)
		{
			_maxManaCounter.text = $"<color=#e2725b>+{_statStruct._maxMana}</color>";
			_maxManaTag.color = Color.white;
		}
		if (_statStruct._maxMana > _scriptEquip._statArray._maxMana)
		{
			_maxManaCounter.text = $"<color=cyan>+{_statStruct._maxMana}</color>";
			if ((bool)_cachedCompareEquip && statStruct._maxMana > _statStruct._maxMana)
			{
				_maxManaCounter.text = $"<color=#e2725b>+{_statStruct._maxMana}</color>";
			}
			_maxManaTag.color = Color.cyan;
		}
		_maxManaCell.SetActive(_statStruct._maxMana > 0);
		if (_statStruct._maxStamina > 0)
		{
			_maxStaminaCounter.text = $"<color=lime>+{_statStruct._maxStamina}</color>";
			_maxStaminaTag.color = Color.white;
			_priorStat = true;
		}
		if ((bool)_cachedCompareEquip && statStruct._maxStamina > _statStruct._maxStamina)
		{
			_maxStaminaCounter.text = $"<color=#e2725b>+{_statStruct._maxStamina}</color>";
			_maxStaminaTag.color = Color.white;
		}
		if (_statStruct._maxStamina > _scriptEquip._statArray._maxStamina)
		{
			_maxStaminaCounter.text = $"<color=cyan>+{_statStruct._maxStamina}</color>";
			if ((bool)_cachedCompareEquip && statStruct._maxStamina > _statStruct._maxStamina)
			{
				_maxStaminaCounter.text = $"<color=#e2725b>+{_statStruct._maxStamina}</color>";
			}
			_maxStaminaTag.color = Color.cyan;
		}
		_maxStaminaCell.SetActive(_statStruct._maxStamina > 0);
		if (_statStruct._attackPower > 0)
		{
			_attackPowerCounter.text = $"<color=lime>+{_statStruct._attackPower}</color>";
			_attackPowerTag.color = Color.white;
			_priorStat = true;
		}
		if ((bool)_cachedCompareEquip && statStruct._attackPower > _statStruct._attackPower)
		{
			_attackPowerCounter.text = $"<color=#e2725b>+{_statStruct._attackPower}</color>";
			_attackPowerTag.color = Color.white;
		}
		if (_statStruct._attackPower > _scriptEquip._statArray._attackPower)
		{
			_attackPowerCounter.text = $"<color=cyan>+{_statStruct._attackPower}</color>";
			if ((bool)_cachedCompareEquip && statStruct._attackPower > _statStruct._attackPower)
			{
				_attackPowerCounter.text = $"<color=#e2725b>+{_statStruct._attackPower}</color>";
			}
			_attackPowerTag.color = Color.cyan;
		}
		_attackPowerCell.SetActive(_statStruct._attackPower > 0);
		if (_statStruct._dexPower > 0)
		{
			_dexPowerCounter.text = $"<color=lime>+{_statStruct._dexPower}</color>";
			_dexPowerTag.color = Color.white;
			_priorStat = true;
		}
		if ((bool)_cachedCompareEquip && statStruct._dexPower > _statStruct._dexPower)
		{
			_dexPowerCounter.text = $"<color=#e2725b>+{_statStruct._dexPower}</color>";
			_dexPowerTag.color = Color.white;
		}
		if (_statStruct._dexPower > _scriptEquip._statArray._dexPower)
		{
			_dexPowerCounter.text = $"<color=cyan>+{_statStruct._dexPower}</color>";
			if ((bool)_cachedCompareEquip && statStruct._dexPower > _statStruct._dexPower)
			{
				_dexPowerCounter.text = $"<color=#e2725b>+{_statStruct._dexPower}</color>";
			}
			_dexPowerTag.color = Color.cyan;
		}
		_dexPowerCell.SetActive(_statStruct._dexPower > 0);
		if (_statStruct._magicPower > 0)
		{
			_magicPowerCounter.text = $"<color=lime>+{_statStruct._magicPower}</color>";
			_magicPowerTag.color = Color.white;
			_priorStat = true;
		}
		if ((bool)_cachedCompareEquip && statStruct._magicPower > _statStruct._magicPower)
		{
			_magicPowerCounter.text = $"<color=#e2725b>+{_statStruct._magicPower}</color>";
			_magicPowerTag.color = Color.white;
		}
		if (_statStruct._magicPower > _scriptEquip._statArray._magicPower)
		{
			_magicPowerCounter.text = $"<color=cyan>+{_statStruct._magicPower}</color>";
			if ((bool)_cachedCompareEquip && statStruct._magicPower > _statStruct._magicPower)
			{
				_magicPowerCounter.text = $"<color=#e2725b>+{_statStruct._magicPower}</color>";
			}
			_magicPowerTag.color = Color.cyan;
		}
		_magicPowerCell.SetActive(_statStruct._magicPower > 0);
		if (_statStruct._defense > 0)
		{
			_defenseCounter.text = $"<color=lime>+{_statStruct._defense}</color>";
			_defenseTag.color = Color.white;
			_priorStat = true;
		}
		if ((bool)_cachedCompareEquip && statStruct._defense > _statStruct._defense)
		{
			_defenseCounter.text = $"<color=#e2725b>+{_statStruct._defense}</color>";
			_defenseTag.color = Color.white;
		}
		if (_statStruct._defense > _scriptEquip._statArray._defense)
		{
			_defenseCounter.text = $"<color=cyan>+{_statStruct._defense}</color>";
			if ((bool)_cachedCompareEquip && statStruct._defense > _statStruct._defense)
			{
				_defenseCounter.text = $"<color=#e2725b>+{_statStruct._defense}</color>";
			}
			_defenseTag.color = Color.cyan;
		}
		_defenseCell.SetActive(_statStruct._defense > 0);
		if (_statStruct._magicDefense > 0)
		{
			_magicDefenseCounter.text = $"<color=lime>+{_statStruct._magicDefense}</color>";
			_magicDefenseTag.color = Color.white;
			_priorStat = true;
		}
		if ((bool)_cachedCompareEquip && statStruct._magicDefense > _statStruct._magicDefense)
		{
			_magicDefenseCounter.text = $"<color=#e2725b>+{_statStruct._magicDefense}</color>";
			_magicDefenseTag.color = Color.white;
		}
		if (_statStruct._magicDefense > _scriptEquip._statArray._magicDefense)
		{
			_magicDefenseCounter.text = $"<color=cyan>+{_statStruct._magicDefense}</color>";
			if ((bool)_cachedCompareEquip && statStruct._magicDefense > _statStruct._magicDefense)
			{
				_magicDefenseCounter.text = $"<color=#e2725b>+{_statStruct._magicDefense}</color>";
			}
			_magicDefenseTag.color = Color.cyan;
		}
		_mgkDefenseCell.SetActive(_statStruct._magicDefense > 0);
		if (_statStruct._criticalRate > 0f)
		{
			_criticalCounter.text = "<color=lime>+" + (_statStruct._criticalRate * 100f).ToString("N2") + "%</color>";
			_criticalTag.color = Color.white;
			_priorStat = true;
		}
		if ((bool)_cachedCompareEquip && statStruct._criticalRate > _statStruct._criticalRate)
		{
			_criticalCounter.text = "<color=#e2725b>+" + (_statStruct._criticalRate * 100f).ToString("N2") + "%</color>";
			_criticalTag.color = Color.white;
		}
		if (_statStruct._criticalRate > _scriptEquip._statArray._criticalRate)
		{
			_criticalCounter.text = "<color=cyan>+" + (_statStruct._criticalRate * 100f).ToString("N2") + "%</color>";
			if ((bool)_cachedCompareEquip && statStruct._criticalRate > _statStruct._criticalRate)
			{
				_criticalCounter.text = "<color=#e2725b>+" + (_statStruct._criticalRate * 100f).ToString("N2") + "%</color>";
			}
			_criticalTag.color = Color.cyan;
		}
		_criticalCell.SetActive(_statStruct._criticalRate > 0f);
		if (_statStruct._magicCriticalRate > 0f)
		{
			_magicCriticalCounter.text = "<color=lime>+" + (_statStruct._magicCriticalRate * 100f).ToString("N2") + "%</color>";
			_magicCriticalTag.color = Color.white;
			_priorStat = true;
		}
		if ((bool)_cachedCompareEquip && statStruct._magicCriticalRate > _statStruct._magicCriticalRate)
		{
			_magicCriticalCounter.text = "<color=#e2725b>+" + (_statStruct._magicCriticalRate * 100f).ToString("N2") + "%</color>";
			_magicCriticalTag.color = Color.white;
		}
		if (_statStruct._magicCriticalRate > _scriptEquip._statArray._magicCriticalRate)
		{
			_magicCriticalCounter.text = "<color=cyan>+" + (_statStruct._magicCriticalRate * 100f).ToString("N2") + "%</color>";
			if ((bool)_cachedCompareEquip && statStruct._magicCriticalRate > _statStruct._magicCriticalRate)
			{
				_magicCriticalCounter.text = "<color=#e2725b>+" + (_statStruct._magicCriticalRate * 100f).ToString("N2") + "%</color>";
			}
			_magicCriticalTag.color = Color.cyan;
		}
		_mgkCritCell.SetActive(_statStruct._magicCriticalRate > 0f);
		if (_statStruct._evasion > 0f)
		{
			_evasionCounter.text = "<color=lime>+" + (_statStruct._evasion * 100f).ToString("N2") + "%</color>";
			_evasionTag.color = Color.white;
			_priorStat = true;
		}
		if ((bool)_cachedCompareEquip && statStruct._evasion > _statStruct._evasion)
		{
			_evasionCounter.text = "<color=#e2725b>+" + (_statStruct._evasion * 100f).ToString("N2") + "%</color>";
			_evasionTag.color = Color.white;
		}
		if (_statStruct._evasion > _scriptEquip._statArray._evasion)
		{
			_evasionCounter.text = "<color=cyan>+" + (_statStruct._evasion * 100f).ToString("N2") + "%</color>";
			if ((bool)_cachedCompareEquip && statStruct._evasion > _statStruct._evasion)
			{
				_evasionCounter.text = "<color=#e2725b>+" + (_statStruct._evasion * 100f).ToString("N2") + "%</color>";
			}
			_evasionTag.color = Color.cyan;
		}
		_evasionCell.SetActive(_statStruct._evasion > 0f);
		if (_statStruct._fireResist > 0)
		{
			_fireResistCounter.text = $"<color=lime>+{_statStruct._fireResist}</color>";
			_fireResistTag.color = Color.white;
			_priorStat = true;
		}
		else if (_statStruct._fireResist < 0)
		{
			_fireResistCounter.text = $"<color=#e2725b>{_statStruct._fireResist}</color>";
			_fireResistTag.color = Color.red;
			_priorStat = true;
		}
		if ((bool)_cachedCompareEquip && statStruct._fireResist > _statStruct._fireResist)
		{
			_fireResistCounter.text = $"<color=#e2725b>+{_statStruct._fireResist}</color>";
			_fireResistTag.color = Color.white;
		}
		if (_statStruct._fireResist > _scriptEquip._statArray._fireResist)
		{
			_fireResistCounter.text = $"<color=cyan>+{_statStruct._fireResist}</color>";
			if ((bool)_cachedCompareEquip && statStruct._fireResist > _statStruct._fireResist)
			{
				_fireResistCounter.text = $"<color=#e2725b>+{_statStruct._fireResist}</color>";
			}
			_fireResistTag.color = Color.cyan;
		}
		if (_statStruct._waterResist > 0)
		{
			_waterResistCounter.text = $"<color=lime>+{_statStruct._waterResist}</color>";
			_waterResistTag.color = Color.white;
			_priorStat = true;
		}
		else if (_statStruct._waterResist < 0)
		{
			_waterResistCounter.text = $"<color=#e2725b>{_statStruct._waterResist}</color>";
			_waterResistTag.color = Color.red;
			_priorStat = true;
		}
		if ((bool)_cachedCompareEquip && statStruct._waterResist > _statStruct._waterResist)
		{
			_waterResistCounter.text = $"<color=#e2725b>+{_statStruct._waterResist}</color>";
			_waterResistTag.color = Color.white;
		}
		if (_statStruct._waterResist > _scriptEquip._statArray._waterResist)
		{
			_waterResistCounter.text = $"<color=cyan>+{_statStruct._waterResist}</color>";
			if ((bool)_cachedCompareEquip && statStruct._waterResist > _statStruct._waterResist)
			{
				_waterResistCounter.text = $"<color=#e2725b>+{_statStruct._waterResist}</color>";
			}
			_waterResistTag.color = Color.cyan;
		}
		if (_statStruct._natureResist > 0)
		{
			_natureResistCounter.text = $"<color=lime>+{_statStruct._natureResist}</color>";
			_natureResistTag.color = Color.white;
			_priorStat = true;
		}
		else if (_statStruct._natureResist < 0)
		{
			_natureResistCounter.text = $"<color=#e2725b>{_statStruct._natureResist}</color>";
			_natureResistTag.color = Color.red;
			_priorStat = true;
		}
		if ((bool)_cachedCompareEquip && statStruct._natureResist > _statStruct._natureResist)
		{
			_natureResistCounter.text = $"<color=#e2725b>+{_statStruct._natureResist}</color>";
			_natureResistTag.color = Color.white;
		}
		if (_statStruct._natureResist > _scriptEquip._statArray._natureResist)
		{
			_natureResistCounter.text = $"<color=cyan>+{_statStruct._natureResist}</color>";
			if ((bool)_cachedCompareEquip && statStruct._natureResist > _statStruct._natureResist)
			{
				_natureResistCounter.text = $"<color=#e2725b>+{_statStruct._natureResist}</color>";
			}
			_natureResistTag.color = Color.cyan;
		}
		if (_statStruct._earthResist > 0)
		{
			_earthResistCounter.text = $"<color=lime>+{_statStruct._earthResist}</color>";
			_earthResistTag.color = Color.white;
			_priorStat = true;
		}
		else if (_statStruct._earthResist < 0)
		{
			_earthResistCounter.text = $"<color=#e2725b>{_statStruct._earthResist}</color>";
			_earthResistTag.color = Color.red;
			_priorStat = true;
		}
		if ((bool)_cachedCompareEquip && statStruct._earthResist > _statStruct._earthResist)
		{
			_earthResistCounter.text = $"<color=#e2725b>+{_statStruct._earthResist}</color>";
			_earthResistTag.color = Color.white;
		}
		if (_statStruct._earthResist > _scriptEquip._statArray._earthResist)
		{
			_earthResistCounter.text = $"<color=cyan>+{_statStruct._earthResist}</color>";
			if ((bool)_cachedCompareEquip && statStruct._earthResist > _statStruct._earthResist)
			{
				_earthResistCounter.text = $"<color=#e2725b>+{_statStruct._earthResist}</color>";
			}
			_earthResistTag.color = Color.cyan;
		}
		if (_statStruct._holyResist > 0)
		{
			_holyResistCounter.text = $"<color=lime>+{_statStruct._holyResist}</color>";
			_holyResistTag.color = Color.white;
			_priorStat = true;
		}
		else if (_statStruct._holyResist < 0)
		{
			_holyResistCounter.text = $"<color=#e2725b>{_statStruct._holyResist}</color>";
			_holyResistTag.color = Color.red;
			_priorStat = true;
		}
		if ((bool)_cachedCompareEquip && statStruct._holyResist > _statStruct._holyResist)
		{
			_holyResistCounter.text = $"<color=#e2725b>+{_statStruct._holyResist}</color>";
			_holyResistTag.color = Color.white;
		}
		if (_statStruct._holyResist > _scriptEquip._statArray._holyResist)
		{
			_holyResistCounter.text = $"<color=cyan>+{_statStruct._holyResist}</color>";
			if ((bool)_cachedCompareEquip && statStruct._holyResist > _statStruct._holyResist)
			{
				_holyResistCounter.text = $"<color=#e2725b>+{_statStruct._holyResist}</color>";
			}
			_holyResistTag.color = Color.cyan;
		}
		if (_statStruct._shadowResist > 0)
		{
			_shadowResistCounter.text = $"<color=lime>+{_statStruct._shadowResist}</color>";
			_shadowResistTag.color = Color.white;
			_priorStat = true;
		}
		else if (_statStruct._shadowResist < 0)
		{
			_shadowResistCounter.text = $"<color=#e2725b>{_statStruct._shadowResist}</color>";
			_shadowResistTag.color = Color.red;
			_priorStat = true;
		}
		if ((bool)_cachedCompareEquip && statStruct._shadowResist > _statStruct._shadowResist)
		{
			_shadowResistCounter.text = $"<color=#e2725b>+{_statStruct._shadowResist}</color>";
			_shadowResistTag.color = Color.white;
		}
		if (_statStruct._shadowResist > _scriptEquip._statArray._shadowResist)
		{
			_shadowResistCounter.text = $"<color=cyan>+{_statStruct._shadowResist}</color>";
			if ((bool)_cachedCompareEquip && statStruct._shadowResist > _statStruct._shadowResist)
			{
				_shadowResistCounter.text = $"<color=#e2725b>+{_statStruct._shadowResist}</color>";
			}
			_shadowResistTag.color = Color.cyan;
		}
		_resistFireCell.SetActive(_statStruct._fireResist != 0);
		_resistWaterCell.SetActive(_statStruct._waterResist != 0);
		_resistNatureCell.SetActive(_statStruct._natureResist != 0);
		_resistEarthCell.SetActive(_statStruct._earthResist != 0);
		_resistHolyCell.SetActive(_statStruct._holyResist != 0);
		_resistShadowCell.SetActive(_statStruct._shadowResist != 0);
		_statDisplayTab.SetActive(_priorStat);
		StatStruct Apply_ModifierToStatStruct(StatStruct _setStatStruct, ItemData _setItemData)
		{
			if (_setItemData == null)
			{
				return _setStatStruct;
			}
			if (_setItemData._modifierID == 0)
			{
				return _setStatStruct;
			}
			ScriptableStatModifier scriptableStatModifier = GameManager._current.Locate_StatModifier(_setItemData._modifierID);
			if (!scriptableStatModifier)
			{
				return _setStatStruct;
			}
			_itemRarity++;
			_modifierTag = scriptableStatModifier._modifierTag + " ";
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
			_setStatStruct._maxHealth += num;
			_setStatStruct._maxMana += num2;
			_setStatStruct._maxStamina += num3;
			_setStatStruct._attackPower += num4;
			_setStatStruct._dexPower += num5;
			_setStatStruct._magicPower += num6;
			_setStatStruct._defense += num7;
			_setStatStruct._magicDefense += num8;
			_setStatStruct._criticalRate += scriptableStatModifier._modifierStatStruct._criticalRate * ((float)_scriptEquip._equipmentLevel * scriptableStatModifier._modifierPercentage);
			_setStatStruct._magicCriticalRate += scriptableStatModifier._modifierStatStruct._magicCriticalRate * ((float)_scriptEquip._equipmentLevel * scriptableStatModifier._modifierPercentage);
			_setStatStruct._evasion += scriptableStatModifier._modifierStatStruct._evasion * ((float)_scriptEquip._equipmentLevel * scriptableStatModifier._modifierPercentage);
			_setStatStruct._fireResist += num9;
			_setStatStruct._waterResist += num10;
			_setStatStruct._natureResist += num11;
			_setStatStruct._earthResist += num12;
			_setStatStruct._holyResist += num13;
			_setStatStruct._shadowResist += num14;
			return _setStatStruct;
		}
		void Display_GambleEquipTooltip()
		{
			_toolTipName.text = "Mystery Gear";
			_toolTipSubName.text = "[Unknown]";
			_toolTipIcon.sprite = _gambleIco_gear;
			_lvlRequirementTab.SetActive(value: false);
			_classRequirementTab.SetActive(value: false);
			_statDisplayTab.SetActive(value: false);
			_weaponDamageTransmuteTab.SetActive(value: false);
			_weaponTypeIconObject.SetActive(value: false);
			_toolTipName.color = Color.white;
			_toolTipSubName.color = Color.white;
			_equipIconBackground.color = new Color(176f, 194f, 74f);
			_equipToolTipType.text = "???";
			_toolTipDescription.text = "You can't really see what this is until you buy it.";
			_equippedIcon.enabled = false;
			_errorIcon.enabled = false;
			_dyableIcon.enabled = false;
		}
		void Init_ShieldToolTip()
		{
			_equipToolTipType.text += "Shield (Off Hand)";
			_cachedCompareEquip = _pEquip._equips["Shield"]._scriptableEquip;
			_cachedCompareItemData = _pEquip._equips["Shield"]._heldItem;
		}
		void Init_WeaponTooltip(ScriptableWeapon _scriptWeapon)
		{
			ScriptableCombatElement combatElement = _scriptWeapon._combatElement;
			string arg = "";
			if (combatElement == GameManager._current._statLogics._defaultCombatElement)
			{
				arg = "";
			}
			else if ((bool)combatElement)
			{
				arg = _scriptWeapon._combatElement._elementName + " ";
			}
			DamageType damageType = _scriptWeapon.weaponType._combatType;
			DamageType _overrideType = _scriptWeapon.weaponType._combatType;
			_equipToolTipType.text = $"{_scriptWeapon.weaponType._weaponTypeClassTag} ({damageType} Weapon)";
			_weaponDamageTransmuteTab.SetActive(IsWeaponTypeOverride());
			if (IsWeaponTypeOverride())
			{
				switch (_overrideType)
				{
				case DamageType.Strength:
					_weaponDamageTransmuteIcon.sprite = GameManager._current._statLogics._statAttributes[0]._attributeIcon;
					break;
				case DamageType.Dexterity:
					_weaponDamageTransmuteIcon.sprite = GameManager._current._statLogics._statAttributes[2]._attributeIcon;
					break;
				case DamageType.Mind:
					_weaponDamageTransmuteIcon.sprite = GameManager._current._statLogics._statAttributes[1]._attributeIcon;
					break;
				}
				_weaponDamageTransmuteText.text = $"Damage Transmute: {_overrideType}";
				damageType = _overrideType;
			}
			_cachedCompareEquip = _pCombat._equippedWeapon;
			if (_pCombat._isUsingAltWeapon && !_pCombat._lockWeapon)
			{
				_cachedCompareItemData = _pEquip._equips["Alt Weapon"]._heldItem;
			}
			else
			{
				_cachedCompareItemData = _pEquip._equips["Weapon"]._heldItem;
			}
			switch (_scriptWeapon.weaponType._combatType)
			{
			case DamageType.Strength:
				_weaponTypeIcon.sprite = GameManager._current._statLogics._statAttributes[0]._attributeIcon;
				break;
			case DamageType.Dexterity:
				_weaponTypeIcon.sprite = GameManager._current._statLogics._statAttributes[2]._attributeIcon;
				break;
			case DamageType.Mind:
				_weaponTypeIcon.sprite = GameManager._current._statLogics._statAttributes[1]._attributeIcon;
				break;
			}
			float num = 1f;
			for (int j = 0; j < Player._mainPlayer._pCasting._passiveSkillObjects.Count; j++)
			{
				SkillObjectBehavior skillObjectBehavior = Player._mainPlayer._pCasting._passiveSkillObjects[j];
				if (skillObjectBehavior.GetType() == typeof(SkillBehavior_WeaponMastery))
				{
					SkillBehavior_WeaponMastery skillBehavior_WeaponMastery = (SkillBehavior_WeaponMastery)skillObjectBehavior;
					for (int k = 0; k < skillBehavior_WeaponMastery._setWeaponTypes.Length; k++)
					{
						if (skillBehavior_WeaponMastery._setWeaponTypes[k] == _scriptWeapon.weaponType)
						{
							num += skillBehavior_WeaponMastery._damageBonusIncrease;
						}
					}
				}
			}
			num += _pCombat._weaponTypeDamageBonus;
			int num2 = _scriptWeapon.Scale_FloorDamage(_pStats._statStruct, damageType, num);
			int num3 = _scriptWeapon.Scale_CeilingDamage(_pStats._statStruct, damageType, num);
			if (num2 > _scriptWeapon._weaponDamage || num3 > _scriptWeapon.Get_BaseCeilingDamage())
			{
				_equipStatsDisplay.text += $"<color=#c5e384>({num2} - {num3})</color> {arg}Damage";
				_equipStatsDisplay.text += $"\n<color=grey>({_scriptWeapon._weaponDamage} - {_scriptWeapon.Get_BaseCeilingDamage()}) Base Damage</color>";
			}
			else
			{
				_equipStatsDisplay.text += $"<color=#c5e384>({_scriptWeapon._weaponDamage} - {_scriptWeapon.Get_BaseCeilingDamage()})</color> {arg}Damage";
			}
			_equipStatsDisplay.gameObject.SetActive(value: true);
			_priorStat = true;
			bool IsWeaponTypeOverride()
			{
				if (_pCombat._useDamageTypeOverride && _itemData._isEquipped && ((_pCombat._isUsingAltWeapon && _itemData._isAltWeapon) || (!_pCombat._isUsingAltWeapon && !_itemData._isAltWeapon)))
				{
					_overrideType = _pCombat._damageTypeOverride;
					return true;
				}
				if (_itemData._useDamageTypeOverride)
				{
					_overrideType = _itemData._damageTypeOverride;
					return true;
				}
				return false;
			}
		}
	}
}
