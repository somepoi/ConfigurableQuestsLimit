using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnchanterManager : MonoBehaviour
{
	public static EnchanterManager _current;

	[Header("Runtime Elements")]
	public ScriptableEquipment _scriptEquipment;

	public ItemData _setItemData;

	public bool _isOpen;

	private int _setEnchantPrice;

	[Header("Enchant Window")]
	[SerializeField]
	private MenuElement _enchanterMenuElement;

	public MenuElement _enchantSlotElement;

	public MenuElement _enchantResultElement;

	[SerializeField]
	private Transform _enchantMenuContent;

	[Space]
	[SerializeField]
	private Button _enchantItemButton;

	[SerializeField]
	private Text _enchantItemButtonText;

	[Space]
	[SerializeField]
	private Button _applyMightStoneButton;

	[SerializeField]
	private Button _applyAgilityStoneButton;

	[SerializeField]
	private Button _applyFluxStoneButton;

	[Space]
	[SerializeField]
	private Text _mightStoneButtonText;

	[SerializeField]
	private Text _agilityStoneButtonText;

	[SerializeField]
	private Text _fluxStoneButtonText;

	[Space]
	[SerializeField]
	private GameObject _enchantSlotHighlight;

	[SerializeField]
	private Image _enchantSlotOverlay;

	[SerializeField]
	private Image _enchantSlotBackdrop;

	[SerializeField]
	private Image _enchantSlotItemIcon;

	[SerializeField]
	private Image _enchantResultOverlay;

	[SerializeField]
	private Image _enchantResultBackdrop;

	[SerializeField]
	private Image _enchantResultItemIcon;

	[SerializeField]
	private Text _currentEnchantmentText;

	[Space]
	[SerializeField]
	private MenuElement _currencyPriceCell;

	[SerializeField]
	private Text _currencyPriceText;

	[Space]
	[SerializeField]
	private MenuElement _tradeItemPriceCell;

	[SerializeField]
	private Image _tradeItemPriceIcon;

	[SerializeField]
	private Text _tradeItemPriceText;

	[Header("Gamepad Elements")]
	private int _currentCellSelection;

	private SettingsCell[] _gamepadCells = new SettingsCell[0];

	private float _dialogResponseBuffer;

	public int CurrentCellSelection
	{
		get
		{
			return _currentCellSelection;
		}
		set
		{
			if (value < 0)
			{
				value = _gamepadCells.Length - 1;
			}
			if (value > _gamepadCells.Length - 1)
			{
				value = 0;
			}
			_currentCellSelection = value;
		}
	}

	private void Awake()
	{
		_current = this;
		_enchantItemButton.onClick.AddListener(Init_PurchaseEnchant);
		_applyMightStoneButton.onClick.AddListener(() =>
		{
			Init_StoneEnchant(DamageType.Strength);
		});
		_applyAgilityStoneButton.onClick.AddListener(() =>
		{
			Init_StoneEnchant(DamageType.Dexterity);
		});
		_applyFluxStoneButton.onClick.AddListener(() =>
		{
			Init_StoneEnchant(DamageType.Mind);
		});
		_gamepadCells = _enchantMenuContent.GetComponentsInChildren<SettingsCell>();
	}

	private void Update()
	{
		_isOpen = DialogManager._current._currentDialogUIPrompt == DialogUIPrompt.ENCHANT;
		_enchanterMenuElement.isEnabled = _isOpen;
		_enchanterMenuElement.gameObject.SetActive(_isOpen);
		if ((bool)_scriptEquipment && _setItemData._isEquipped)
		{
			Clear_EnchantOption();
		}
		Handle_GamepadControl();
		Handle_LocateScriptEquipment();
		Handle_EnchanterBehavior();
	}

	private void Handle_GamepadControl()
	{
		if (DialogManager._current._currentDialogUIPrompt != DialogUIPrompt.ENCHANT)
		{
			return;
		}
		bool flag = InputControlManager.current._setGamepadType != GamepadType.Keyboard;
		for (int i = 0; i < _gamepadCells.Length; i++)
		{
			_gamepadCells[i]._isSelected = flag && DialogManager._current._currentGamepadTab == DialogManager_GamepadTab.Prompt_Window && i == _currentCellSelection;
		}
		if (flag && DialogManager._current._currentGamepadTab != DialogManager_GamepadTab.Prompt_Window)
		{
			CurrentCellSelection = 0;
			return;
		}
		bool flag2 = InputControlManager.current.DpadDown(DpadHoldType.OnDown) || GamepadInput.GetButtonDown(GamepadButton.RightStick_Down) || GamepadInput.GetButtonDown(GamepadButton.LeftStick_Down);
		bool flag3 = InputControlManager.current.DpadUp(DpadHoldType.OnDown) || GamepadInput.GetButtonDown(GamepadButton.RightStick_Up) || GamepadInput.GetButtonDown(GamepadButton.LeftStick_Up);
		if (!_scriptEquipment)
		{
			CurrentCellSelection = 0;
		}
		else if (_scriptEquipment.GetType() == typeof(ScriptableWeapon))
		{
			if (flag2)
			{
				CurrentCellSelection++;
				TabMenu._current._gamepadSelectAsrc.Play();
			}
			if (flag3)
			{
				CurrentCellSelection--;
				TabMenu._current._gamepadSelectAsrc.Play();
			}
		}
		else
		{
			CurrentCellSelection = 0;
		}
		if (DialogManager._current._currentGamepadTab == DialogManager_GamepadTab.Prompt_Window && GamepadInput.GetButtonDown(GamepadButton.Button_West))
		{
			Clear_EnchantOption();
		}
	}

	private void Handle_LocateScriptEquipment()
	{
		if (!_isOpen)
		{
			Clear_EnchantOption();
			_currencyPriceCell.isEnabled = false;
			_tradeItemPriceCell.isEnabled = false;
		}
		else if (!_scriptEquipment)
		{
			ScriptableItem scriptableItem = GameManager._current.Locate_Item(_setItemData._itemName);
			if (!scriptableItem)
			{
				_scriptEquipment = null;
			}
			else if (scriptableItem._itemType != ItemType.GEAR)
			{
				_scriptEquipment = null;
			}
			else if (string.IsNullOrWhiteSpace(_setItemData._itemName))
			{
				_scriptEquipment = null;
			}
			else
			{
				_scriptEquipment = (ScriptableEquipment)scriptableItem;
			}
		}
	}

	public void Clear_EnchantOption()
	{
		_setItemData = new ItemData
		{
			_itemName = string.Empty
		};
		_scriptEquipment = null;
	}

	private void Handle_EnchanterBehavior()
	{
		if (!_isOpen)
		{
			return;
		}
		TabMenu._current._itemTradeMode = true;
		if (TabMenu._current._itemCell._currentTab != ItemType.GEAR)
		{
			TabMenu._current._itemCell.Init_InventoryTab(0);
		}
		_enchantSlotHighlight.SetActive(TabMenu._current._itemCell._heldItemListDataEntry);
		bool _hasTradeItemCost = (bool)_scriptEquipment && _scriptEquipment._statModifierCost != null && (bool)_scriptEquipment._statModifierCost._scriptItem;
		int _foundTradeQuantity = 0;
		if ((bool)_scriptEquipment)
		{
			_enchantSlotItemIcon.enabled = true;
			_enchantResultItemIcon.enabled = true;
			_enchantSlotItemIcon.sprite = _scriptEquipment._itemIcon;
			_enchantResultItemIcon.sprite = _scriptEquipment._itemIcon;
			_applyMightStoneButton.gameObject.SetActive(_scriptEquipment.GetType() == typeof(ScriptableWeapon));
			_applyAgilityStoneButton.gameObject.SetActive(_scriptEquipment.GetType() == typeof(ScriptableWeapon));
			_applyFluxStoneButton.gameObject.SetActive(_scriptEquipment.GetType() == typeof(ScriptableWeapon));
			if (_scriptEquipment.GetType() == typeof(ScriptableWeapon))
			{
				ScriptableWeapon obj = (ScriptableWeapon)_scriptEquipment;
				bool flag = obj.weaponType._combatType == DamageType.Strength;
				bool flag2 = obj.weaponType._combatType == DamageType.Dexterity;
				bool flag3 = obj.weaponType._combatType == DamageType.Mind;
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				_applyMightStoneButton.interactable = true;
				_applyAgilityStoneButton.interactable = true;
				_applyFluxStoneButton.interactable = true;
				if (_setItemData._useDamageTypeOverride)
				{
					switch (_setItemData._damageTypeOverride)
					{
					case DamageType.Strength:
						_applyMightStoneButton.interactable = false;
						break;
					case DamageType.Dexterity:
						_applyAgilityStoneButton.interactable = false;
						break;
					case DamageType.Mind:
						_applyFluxStoneButton.interactable = false;
						break;
					}
				}
				else if (flag)
				{
					_applyMightStoneButton.interactable = false;
				}
				else if (flag2)
				{
					_applyAgilityStoneButton.interactable = false;
				}
				else if (flag3)
				{
					_applyFluxStoneButton.interactable = false;
				}
				for (int i = 0; i < Player._mainPlayer._pInventory._heldItems.Count; i++)
				{
					if (Player._mainPlayer._pInventory._heldItems[i]._itemName == "Might Stone")
					{
						num += Player._mainPlayer._pInventory._heldItems[i]._quantity;
					}
					if (Player._mainPlayer._pInventory._heldItems[i]._itemName == "Agility Stone")
					{
						num2 += Player._mainPlayer._pInventory._heldItems[i]._quantity;
					}
					if (Player._mainPlayer._pInventory._heldItems[i]._itemName == "Flux Stone")
					{
						num3 += Player._mainPlayer._pInventory._heldItems[i]._quantity;
					}
				}
				if (flag)
				{
					_mightStoneButtonText.text = "Transmute Strength (Free)";
				}
				else
				{
					_mightStoneButtonText.text = $"Transmute Strength (x{num})";
				}
				if (flag2)
				{
					_agilityStoneButtonText.text = "Transmute Dexterity (Free)";
				}
				else
				{
					_agilityStoneButtonText.text = $"Transmute Dexterity (x{num2})";
				}
				if (flag3)
				{
					_fluxStoneButtonText.text = "Apply Flux Stone (Free)";
				}
				else
				{
					_fluxStoneButtonText.text = $"Transmute Mind (x{num3})";
				}
				if (flag)
				{
					_mightStoneButtonText.color = Color.cyan;
				}
				else if (num <= 0)
				{
					_mightStoneButtonText.color = Color.yellow;
				}
				else
				{
					_mightStoneButtonText.color = Color.white;
				}
				if (flag2)
				{
					_agilityStoneButtonText.color = Color.cyan;
				}
				else if (num2 <= 0)
				{
					_agilityStoneButtonText.color = Color.yellow;
				}
				else
				{
					_agilityStoneButtonText.color = Color.white;
				}
				if (flag3)
				{
					_fluxStoneButtonText.color = Color.cyan;
				}
				else if (num3 <= 0)
				{
					_fluxStoneButtonText.color = Color.yellow;
				}
				else
				{
					_fluxStoneButtonText.color = Color.white;
				}
			}
			else
			{
				_applyMightStoneButton.gameObject.SetActive(value: false);
				_applyAgilityStoneButton.gameObject.SetActive(value: false);
				_applyFluxStoneButton.gameObject.SetActive(value: false);
			}
			if (_setItemData._modifierID > 0)
			{
				ScriptableStatModifier scriptableStatModifier = GameManager._current.Locate_StatModifier(_setItemData._modifierID);
				_enchantItemButtonText.text = "Re-roll Enchant";
				_currentEnchantmentText.text = "Current enchantment: " + scriptableStatModifier._modifierTag;
			}
			else
			{
				_enchantItemButtonText.text = "Enchant Item";
				_currentEnchantmentText.text = "No enchantment applied on this item";
			}
			if ((int)_scriptEquipment._itemRarity >= 2 || !_scriptEquipment._statModifierTable)
			{
				_enchantItemButtonText.text = "Cannot enchant";
				_currentEnchantmentText.text = "Item cannot be enchanted";
				_enchantSlotElement.isEnabled = false;
				_enchantResultElement.isEnabled = false;
				_enchantSlotOverlay.color = GameManager._current._statLogics._itemRarityColors[(uint)_scriptEquipment._itemRarity];
				_enchantSlotBackdrop.color = GameManager._current._statLogics._itemRarityColors[(uint)_scriptEquipment._itemRarity];
				_currencyPriceCell.isEnabled = false;
				_tradeItemPriceCell.isEnabled = false;
			}
			else
			{
				_enchantSlotOverlay.color = GameManager._current._statLogics._itemRarityColors[(uint)_scriptEquipment._itemRarity];
				_enchantResultOverlay.color = GameManager._current._statLogics._itemRarityColors[(uint)(_scriptEquipment._itemRarity + 1)];
				_enchantSlotBackdrop.color = GameManager._current._statLogics._itemRarityColors[(uint)_scriptEquipment._itemRarity];
				_enchantResultBackdrop.color = GameManager._current._statLogics._itemRarityColors[(uint)(_scriptEquipment._itemRarity + 1)];
				_enchantSlotElement.isEnabled = true;
				_enchantResultElement.isEnabled = true;
				_currencyPriceCell.isEnabled = true;
				_setEnchantPrice = _scriptEquipment._vendorCost * 3;
				_currencyPriceText.text = $"{_setEnchantPrice} {GameManager._current._statLogics._currencyName}";
				_tradeItemPriceCell.isEnabled = _hasTradeItemCost;
				if (_hasTradeItemCost)
				{
					for (int j = 0; j < Player._mainPlayer._pInventory._heldItems.Count; j++)
					{
						if (Player._mainPlayer._pInventory._heldItems[j]._itemName == _scriptEquipment._statModifierCost._scriptItem._itemName)
						{
							_foundTradeQuantity += Player._mainPlayer._pInventory._heldItems[j]._quantity;
						}
					}
					_tradeItemPriceIcon.sprite = _scriptEquipment._statModifierCost._scriptItem._itemIcon;
					_tradeItemPriceText.text = $"{_foundTradeQuantity}/{_scriptEquipment._statModifierCost._scriptItemQuantity} {_scriptEquipment._statModifierCost._scriptItem._itemName}";
				}
			}
			if (!CanAffordEnchant())
			{
				_currencyPriceText.color = Color.yellow;
				_tradeItemPriceText.color = Color.yellow;
			}
			else
			{
				_currencyPriceText.color = Color.white;
				_tradeItemPriceText.color = Color.white;
			}
			if (!CanAffordEnchant() && (int)_scriptEquipment._itemRarity < 2)
			{
				_enchantItemButtonText.text = "Cannot enchant";
			}
			_enchantItemButton.interactable = (int)_scriptEquipment._itemRarity < 2 && CanAffordEnchant() && (bool)_scriptEquipment._statModifierTable;
		}
		else
		{
			_enchantSlotOverlay.color = Color.grey;
			_enchantSlotBackdrop.color = Color.grey;
			_enchantSlotItemIcon.enabled = false;
			_enchantResultOverlay.color = Color.grey;
			_enchantResultBackdrop.color = Color.grey;
			_enchantResultItemIcon.enabled = false;
			_enchantItemButton.interactable = false;
			_applyMightStoneButton.interactable = false;
			_applyAgilityStoneButton.interactable = false;
			_applyFluxStoneButton.interactable = false;
			_enchantItemButtonText.text = "Insert item to enchant";
			_currentEnchantmentText.text = "";
			_currencyPriceText.text = "";
			_tradeItemPriceText.text = "";
			_currencyPriceCell.isEnabled = false;
			_tradeItemPriceCell.isEnabled = false;
			_enchantSlotElement.isEnabled = false;
			_enchantResultElement.isEnabled = false;
			_applyMightStoneButton.gameObject.SetActive(value: false);
			_applyAgilityStoneButton.gameObject.SetActive(value: false);
			_applyFluxStoneButton.gameObject.SetActive(value: false);
		}
		bool CanAffordEnchant()
		{
			if (_setEnchantPrice > Player._mainPlayer._pInventory._heldCurrency)
			{
				return false;
			}
			if (_hasTradeItemCost && _foundTradeQuantity < _scriptEquipment._statModifierCost._scriptItemQuantity)
			{
				return false;
			}
			return true;
		}
	}

	private void Init_PurchaseEnchant()
	{
		if (!_scriptEquipment || !_isOpen)
		{
			return;
		}
		Player._mainPlayer._pInventory.Cmd_SubtractCurrency(_setEnchantPrice);
		if (_scriptEquipment._statModifierCost != null && (bool)_scriptEquipment._statModifierCost._scriptItem)
		{
			int num = 0;
			PlayerInventory pInventory = Player._mainPlayer._pInventory;
			for (int i = 0; i < pInventory._heldItems.Count; i++)
			{
				if (!(pInventory._heldItems[i]._itemName != _scriptEquipment._statModifierCost._scriptItem._itemName) && num < _scriptEquipment._statModifierCost._scriptItemQuantity)
				{
					int quantity = pInventory._heldItems[i]._quantity;
					if (quantity >= _scriptEquipment._statModifierCost._scriptItemQuantity)
					{
						quantity -= _scriptEquipment._statModifierCost._scriptItemQuantity;
						num = _scriptEquipment._statModifierCost._scriptItemQuantity;
					}
					else
					{
						num += quantity;
						quantity -= quantity;
					}
					if (quantity <= 0)
					{
						pInventory.Remove_Item(pInventory._heldItems[i], 0);
					}
					else
					{
						pInventory._heldItems[i]._quantity = quantity;
					}
				}
			}
		}
		List<int> list = new List<int>();
		for (int j = 0; j < _scriptEquipment._statModifierTable._statModifierSlots.Length; j++)
		{
			ScriptableStatModifier equipModifier = _scriptEquipment._statModifierTable._statModifierSlots[j]._equipModifier;
			if (_setItemData._modifierID != equipModifier._modifierID)
			{
				list.Add(equipModifier._modifierID);
			}
		}
		int num2 = list[Random.Range(0, list.Count)];
		_setItemData._modifierID = num2;
		ScriptableStatModifier scriptableStatModifier = GameManager._current.Locate_StatModifier(num2);
		if (_setItemData._isEquipped)
		{
			Player._mainPlayer._pEquipment.Init_UnEquipItem(_setItemData);
		}
		Player._mainPlayer._chatBehaviour.Client_RecieveTriggerMessage("You got the " + scriptableStatModifier._modifierTag + " enchantment!");
		Player._mainPlayer._pVisual.Cmd_VanitySparkleEffect();
		if (Random.Range(0, 5) == 1 && _dialogResponseBuffer <= 0f)
		{
			DialogManager._current.Start_QuickSentence(DialogManager._current._scriptableDialog._shopkeepResponses[Random.Range(0, DialogManager._current._scriptableDialog._shopkeepResponses.Length)]);
			_dialogResponseBuffer = 8.25f;
		}
		Clear_EnchantOption();
	}

	private void Init_StoneEnchant(DamageType _stoneType)
	{
		if (!_scriptEquipment || !_isOpen || _scriptEquipment.GetType() != typeof(ScriptableWeapon))
		{
			return;
		}
		ScriptableWeapon obj = (ScriptableWeapon)_scriptEquipment;
		PlayerInventory pInventory = Player._mainPlayer._pInventory;
		bool flag = obj.weaponType._combatType == _stoneType;
		string text = string.Empty;
		switch (_stoneType)
		{
		case DamageType.Strength:
			text = "Might Stone";
			break;
		case DamageType.Dexterity:
			text = "Agility Stone";
			break;
		case DamageType.Mind:
			text = "Flux Stone";
			break;
		}
		if ((int)_scriptEquipment._itemRarity > 2)
		{
			ErrorPromptTextManager.current.Init_ErrorPrompt("Cannot Transmute Weapon");
			return;
		}
		bool flag2 = false;
		for (int i = 0; i < pInventory._heldItems.Count; i++)
		{
			if (flag)
			{
				flag2 = true;
				break;
			}
			if (!(pInventory._heldItems[i]._itemName != text))
			{
				pInventory.Remove_Item(pInventory._heldItems[i], 1);
				flag2 = true;
				break;
			}
		}
		if (!flag2)
		{
			switch (_stoneType)
			{
			case DamageType.Strength:
				ErrorPromptTextManager.current.Init_ErrorPrompt("Not enough Might Stones");
				break;
			case DamageType.Dexterity:
				ErrorPromptTextManager.current.Init_ErrorPrompt("Not enough Agility Stones");
				break;
			case DamageType.Mind:
				ErrorPromptTextManager.current.Init_ErrorPrompt("Not enough Flux Stones");
				break;
			}
			return;
		}
		switch (_stoneType)
		{
		case DamageType.Strength:
			Player._mainPlayer._chatBehaviour.Client_RecieveTriggerMessage("Your " + _scriptEquipment._itemName + " now scale off Strength!");
			break;
		case DamageType.Dexterity:
			Player._mainPlayer._chatBehaviour.Client_RecieveTriggerMessage("Your " + _scriptEquipment._itemName + " now scale off Dexterity!");
			break;
		case DamageType.Mind:
			Player._mainPlayer._chatBehaviour.Client_RecieveTriggerMessage("Your " + _scriptEquipment._itemName + " now scale off Mind!");
			break;
		}
		if (flag)
		{
			_setItemData._useDamageTypeOverride = false;
		}
		else
		{
			_setItemData._damageTypeOverride = _stoneType;
			_setItemData._useDamageTypeOverride = true;
		}
		if (Random.Range(0, 5) == 1 && _dialogResponseBuffer <= 0f)
		{
			DialogManager._current.Start_QuickSentence(DialogManager._current._scriptableDialog._shopkeepResponses[Random.Range(0, DialogManager._current._scriptableDialog._shopkeepResponses.Length)]);
			_dialogResponseBuffer = 8.25f;
		}
		if (_setItemData._isEquipped)
		{
			Player._mainPlayer._pEquipment.Init_UnEquipItem(_setItemData);
		}
		Player._mainPlayer._pVisual.Cmd_VanitySparkleEffect();
		Clear_EnchantOption();
	}
}
