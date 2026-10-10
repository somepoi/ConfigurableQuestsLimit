using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ItemMenuCell : TabMenuCell
{
	[Header("Item Cell")]
	public GameObject _itemCellBackdrop;

	public MenuElement _itemCell_quantityPromptElement;

	public RectTransform _itemCell_quantityPromptRect;

	[SerializeField]
	private InputField _itemCell_quantityDropInputField;

	[SerializeField]
	private Button _applyQuantityButton;

	[SerializeField]
	private Button _cancelQuantityButton;

	[SerializeField]
	private Button _subQuantityButton;

	[SerializeField]
	private Button _addQuantityButton;

	[Header("Currency Display")]
	[SerializeField]
	private Text _currencyDisplayCounter;

	[Space]
	[SerializeField]
	private MenuElement _illusionStoneCounterElement;

	[SerializeField]
	private Text _illusionStoneCounter;

	private int _currentIllusionStones;

	[Header("Equip Cell")]
	[SerializeField]
	private Text _equipTabHeader;

	[SerializeField]
	private Image _shieldErrorIcon;

	[Space]
	[SerializeField]
	private CanvasGroup _mainWeaponSlotGroup;

	[SerializeField]
	private CanvasGroup _altWeaponSlotGroup;

	[Space]
	[SerializeField]
	private Button _equipDisplayButton;

	[Space]
	[SerializeField]
	private Button _equipVanityButton;

	[Space]
	[SerializeField]
	private Button _equipStatsButton;

	[SerializeField]
	private RectTransform _equipDisplayButtonTransform;

	[SerializeField]
	private RectTransform _equipVanityButtonTransform;

	[SerializeField]
	private RectTransform _equipStatsButtonTransform;

	[SerializeField]
	private EquipUISlot[] _equipSlots;

	[Space]
	[SerializeField]
	private Image _dyeSprite;

	[Header("Equip Character Display")]
	public Transform _raceModelDisplayContainer;

	public GameObject _equipViewportBackdrop;

	[Space]
	public PlayerRaceModel[] _playerRaceModelDisplays;

	private PlayerRaceModel _setRaceModel;

	private bool _isRotatingDisplay;

	private float _rotationValue = 9.75f;

	[Space]
	[SerializeField]
	private Button _lockWeaponToggle_button;

	[SerializeField]
	private Image _lockWeaponToggle_icon;

	[SerializeField]
	private Sprite _lockWeapon_lockedSprite;

	[SerializeField]
	private Sprite _lockWeapon_unlockedSprite;

	private float _weaponLockBuffer;

	[Header("Inventory Tabs")]
	public ItemType _currentTab;

	public EquipCellTab _currentEquipCellTab;

	public GameObject _itemCellTabObject;

	public GameObject _equipCellTabDockObject;

	public Button _itemCell_gearButton;

	public Button _itemCell_consumableButton;

	public Button _itemCell_tradeItemButton;

	public Button _itemCell_SortButton;

	public RectTransform _gearButton_transform;

	public RectTransform _consumableButton_transform;

	public RectTransform _tradeButton_transform;

	public RectTransform _sortButton_transform;

	[SerializeField]
	private GameObject[] _tabButtonHighlights;

	[Header("Item Slots")]
	[SerializeField]
	private Transform _slotContainer;

	public ItemSlotUI _currentHighlightedItemSlot;

	public ItemSlotUI[] _itemSlots;

	[Space]
	public Transform _itemCell_gearContent;

	public Transform _itemCell_consumableContent;

	public Transform _itemCell_tradeContent;

	[Space]
	public ItemListDataEntry _currentHighlightedDataEntry;

	public ItemListDataEntry _selectedItemListDataEntry;

	public ItemListDataEntry _heldItemListDataEntry;

	[Header("Item Prompt Window")]
	public MenuElement _itemPromptWindowElement;

	public CanvasGroup _itemCell_itemPromptWindowGroup;

	[SerializeField]
	private Button _equipButton;

	[SerializeField]
	private Button _transmogButton;

	[SerializeField]
	private Button _removeButton;

	[SerializeField]
	private Button _useButton;

	[SerializeField]
	private Button _dropButton;

	[SerializeField]
	private Button _splitButton;

	[SerializeField]
	private Button _cancelButton;

	[SerializeField]
	private Vector3 _promptOffsetPosition = Vector3.zero;

	private bool _isSplittingQuantityItem;

	[Header("Gamepad")]
	public GamepadItemMenuFocus _gamepadItemMenuFocus;

	[SerializeField]
	private RectTransform _gamepad_itemTabCursor;

	[SerializeField]
	private RectTransform _gamepadCursorObject_itemTabs;

	[SerializeField]
	private RectTransform _gamepad_equipmentPanelCursor;

	[SerializeField]
	private RectTransform _gamepadCursorObject_equipTabs;

	[SerializeField]
	private Image _gamepad_itemCursor_leftTriggerIco;

	[SerializeField]
	private Image _gamepad_itemCursor_rightTriggerIco;

	[SerializeField]
	private Image _gamepad_leftTriggerViewportIco;

	[SerializeField]
	private Image _gamepad_rightTriggerViewportIco;

	[SerializeField]
	private Image _gamepad_rightStickPush_sortIco;

	[SerializeField]
	private Image _gamepad_equipPanelSelection;

	[Space]
	[SerializeField]
	private MenuElement _gamepad_southButtonContext;

	[SerializeField]
	private MenuElement _gamepad_eastButtonContext;

	[SerializeField]
	private MenuElement _gamepad_westButtonContext;

	[Space]
	[SerializeField]
	private Text _gamepad_southButtonContext_text;

	[SerializeField]
	private Text _gamepad_eastButtonContext_text;

	[SerializeField]
	private Text _gamepad_westButtonContext_text;

	private int _gamepad_selectedItemSlot;

	private int _gamepad_selectedItemTab;

	private int _gamepad_selectedEquipTab;

	private int _gamepad_selectedEquipSlot;

	[Space]
	[SerializeField]
	private SettingsCell _gamepad_currencyCell;

	private bool _gamepad_currencyCellSelected;

	[Space]
	[SerializeField]
	private Vector2 _gamepad_quantityPromptPos = Vector2.zero;

	[SerializeField]
	private Image _gamepad_quantityIco_left;

	[SerializeField]
	private Image _gamepad_quantityIco_right;

	[SerializeField]
	private Image _gamepad_quantityIco_confirm;

	[SerializeField]
	private Image _gamepad_quantityIco_cancel;

	public bool _gamepad_quantityPromptSelected;

	private float _gamepad_buffer;

	[Header("Audio")]
	public AudioSource _aSrc_itemPick;

	public AudioSource _aSrc_itemPlace;

	public AudioSource _aSrc_sortItems;

	public int Gamepad_SelectedItemSlot
	{
		get
		{
			return _gamepad_selectedItemSlot;
		}
		set
		{
			if (!_heldItemListDataEntry)
			{
				if (_gamepad_selectedItemSlot == 3 && value == 4)
				{
					if (_currentEquipCellTab == EquipCellTab.STATS_PANEL)
					{
						_gamepadItemMenuFocus = GamepadItemMenuFocus.VIEW_EQUIP_TAB;
					}
					else
					{
						_gamepadItemMenuFocus = GamepadItemMenuFocus.EQUIPMENT;
					}
					_gamepad_selectedEquipSlot = 0;
					value = 3;
				}
				else if (_gamepad_selectedItemSlot == 7 && value == 8)
				{
					if (_currentEquipCellTab == EquipCellTab.STATS_PANEL)
					{
						_gamepadItemMenuFocus = GamepadItemMenuFocus.VIEW_EQUIP_TAB;
					}
					else
					{
						_gamepadItemMenuFocus = GamepadItemMenuFocus.EQUIPMENT;
					}
					_gamepad_selectedEquipSlot = 0;
					value = 7;
				}
				else if (_gamepad_selectedItemSlot == 11 && value == 12)
				{
					if (_currentEquipCellTab == EquipCellTab.STATS_PANEL)
					{
						_gamepadItemMenuFocus = GamepadItemMenuFocus.VIEW_EQUIP_TAB;
					}
					else
					{
						_gamepadItemMenuFocus = GamepadItemMenuFocus.EQUIPMENT;
					}
					_gamepad_selectedEquipSlot = 1;
					value = 11;
				}
				else if (_gamepad_selectedItemSlot == 15 && value == 16)
				{
					if (_currentEquipCellTab == EquipCellTab.STATS_PANEL)
					{
						_gamepadItemMenuFocus = GamepadItemMenuFocus.VIEW_EQUIP_TAB;
					}
					else
					{
						_gamepadItemMenuFocus = GamepadItemMenuFocus.EQUIPMENT;
					}
					_gamepad_selectedEquipSlot = 2;
					value = 15;
				}
				else if (_gamepad_selectedItemSlot == 19 && value == 20)
				{
					if (_currentEquipCellTab == EquipCellTab.STATS_PANEL)
					{
						_gamepadItemMenuFocus = GamepadItemMenuFocus.VIEW_EQUIP_TAB;
					}
					else
					{
						_gamepadItemMenuFocus = GamepadItemMenuFocus.EQUIPMENT;
					}
					_gamepad_selectedEquipSlot = 3;
					value = 19;
				}
				else if (_gamepad_selectedItemSlot == 23 && value == 24)
				{
					if (_currentEquipCellTab == EquipCellTab.STATS_PANEL)
					{
						_gamepadItemMenuFocus = GamepadItemMenuFocus.VIEW_EQUIP_TAB;
					}
					else
					{
						_gamepadItemMenuFocus = GamepadItemMenuFocus.EQUIPMENT;
					}
					_gamepad_selectedEquipSlot = 3;
					value = 23;
				}
				else if (_gamepad_selectedItemSlot == 0 && value == -4)
				{
					value = 0;
				}
				else if (_gamepad_selectedItemSlot == 1 && value == -3)
				{
					value = 1;
				}
				else if (_gamepad_selectedItemSlot == 2 && value == -2)
				{
					value = 2;
				}
				else if (_gamepad_selectedItemSlot == 3 && value == -1)
				{
					value = 3;
				}
				else if (_gamepad_selectedItemSlot == 0 && value == -1)
				{
					if (_itemCellTabObject.activeInHierarchy)
					{
						_gamepadItemMenuFocus = GamepadItemMenuFocus.ITEM_TAB;
					}
					value = 0;
				}
				else if (_gamepad_selectedItemSlot == 4 && value == 3)
				{
					if (_itemCellTabObject.activeInHierarchy)
					{
						_gamepadItemMenuFocus = GamepadItemMenuFocus.ITEM_TAB;
					}
					value = 4;
				}
				else if (_gamepad_selectedItemSlot == 8 && value == 7)
				{
					if (_itemCellTabObject.activeInHierarchy)
					{
						_gamepadItemMenuFocus = GamepadItemMenuFocus.ITEM_TAB;
					}
					value = 8;
				}
				else if (_gamepad_selectedItemSlot == 12 && value == 11)
				{
					if (_itemCellTabObject.activeInHierarchy)
					{
						_gamepadItemMenuFocus = GamepadItemMenuFocus.ITEM_TAB;
					}
					value = 12;
				}
				else if (_gamepad_selectedItemSlot == 16 && value == 15)
				{
					if (_itemCellTabObject.activeInHierarchy)
					{
						_gamepadItemMenuFocus = GamepadItemMenuFocus.ITEM_TAB;
					}
					value = 16;
				}
				else if (_gamepad_selectedItemSlot == 20 && value == 19)
				{
					if (_itemCellTabObject.activeInHierarchy)
					{
						_gamepadItemMenuFocus = GamepadItemMenuFocus.ITEM_TAB;
					}
					value = 20;
				}
			}
			if (value < 0)
			{
				value = 0;
			}
			if (value > 23)
			{
				value = 23;
			}
			if (_gamepad_selectedItemSlot != value)
			{
				InputControlManager.current._gamepadMouseCursorBuffer = 0f;
				ToolTipManager._current.Disable_AllTooltips();
				if (TabMenu._current._isOpen && TabMenu._current._selectedMenuCell == this)
				{
					TabMenu._current._gamepadSelectAsrc.Play();
				}
				if (IsInvoking("Init_GamepadToolTip"))
				{
					CancelInvoke("Init_GamepadToolTip");
				}
				Invoke("Init_GamepadToolTip", 0.3f);
			}
			_gamepad_selectedItemSlot = value;
		}
	}

	public int Gamepad_SelectedItemTab
	{
		get
		{
			return _gamepad_selectedItemTab;
		}
		set
		{
			if (value > 2)
			{
				value = 2;
			}
			else if (value < 0)
			{
				value = 0;
			}
			if (_gamepad_selectedItemTab != value)
			{
				if (TabMenu._current._isOpen && TabMenu._current._selectedMenuCell == this)
				{
					TabMenu._current._gamepadSelectAsrc.Play();
				}
				Init_InventoryTab(value);
			}
			_gamepad_selectedItemTab = value;
		}
	}

	public int Gamepad_SelectedEquipTab
	{
		get
		{
			return _gamepad_selectedEquipTab;
		}
		set
		{
			if (value < 0)
			{
				_gamepadItemMenuFocus = GamepadItemMenuFocus.ITEMS;
				_gamepad_selectedItemSlot = 3;
				value = 0;
			}
			if (value > 2)
			{
				value = 2;
			}
			switch (value)
			{
			case 0:
				Display_EquipmentPanel();
				break;
			case 1:
				Display_VanityPanel();
				break;
			case 2:
				Display_StatsPanel();
				break;
			}
			if (_gamepad_selectedEquipTab != value && TabMenu._current._isOpen && TabMenu._current._selectedMenuCell == this)
			{
				TabMenu._current._gamepadSelectAsrc.Play();
			}
			_gamepad_selectedEquipTab = value;
		}
	}

	public int Gamepad_SelectedEquipSlot
	{
		get
		{
			return _gamepad_selectedEquipSlot;
		}
		set
		{
			InputControlManager.current._gamepadMouseCursorBuffer = 0f;
			ToolTipManager._current.Disable_AllTooltips();
			switch (_gamepad_selectedEquipSlot)
			{
			case 0:
				switch (value)
				{
				case -1:
					if (!DialogManager._current._isDialogEnabled)
					{
						_gamepadItemMenuFocus = GamepadItemMenuFocus.VIEW_EQUIP_TAB;
					}
					value = 0;
					break;
				case -4:
					_gamepadItemMenuFocus = GamepadItemMenuFocus.ITEMS;
					value = 0;
					break;
				}
				break;
			case 1:
				if (value == -3)
				{
					_gamepadItemMenuFocus = GamepadItemMenuFocus.ITEMS;
					value = 1;
				}
				break;
			case 2:
				if (value == -2)
				{
					_gamepadItemMenuFocus = GamepadItemMenuFocus.ITEMS;
					value = 2;
				}
				break;
			case 3:
				if (value == -1)
				{
					_gamepadItemMenuFocus = GamepadItemMenuFocus.ITEMS;
					value = 3;
				}
				break;
			case 4:
				if (value == 3)
				{
					if (!DialogManager._current._isDialogEnabled)
					{
						_gamepadItemMenuFocus = GamepadItemMenuFocus.VIEW_EQUIP_TAB;
					}
					value = 4;
				}
				break;
			}
			if (_gamepad_selectedEquipSlot == 5 && value == 4 && (!Player._mainPlayer._pCombat._lockWeapon || Player._mainPlayer._pCombat._isUsingAltWeapon))
			{
				value = 5;
				_gamepadItemMenuFocus = GamepadItemMenuFocus.VIEW_EQUIP_TAB;
			}
			if (_gamepad_selectedEquipSlot == 4 && value == 5 && (Player._mainPlayer._pCombat._lockWeapon || !Player._mainPlayer._pCombat._isUsingAltWeapon))
			{
				value = 6;
			}
			if (value < 0)
			{
				value = 0;
			}
			if (value > 7)
			{
				value = 7;
			}
			if (_gamepad_selectedEquipSlot != value)
			{
				if (TabMenu._current._isOpen && TabMenu._current._selectedMenuCell == this)
				{
					TabMenu._current._gamepadSelectAsrc.Play();
				}
				if (IsInvoking("Init_GamepadToolTip"))
				{
					CancelInvoke("Init_GamepadToolTip");
				}
				Invoke("Init_GamepadToolTip", 0.3f);
			}
			_gamepad_selectedEquipSlot = value;
		}
	}

	private void Init_GamepadToolTip()
	{
		if (!Cursor.visible)
		{
			if (_gamepadItemMenuFocus == GamepadItemMenuFocus.EQUIPMENT)
			{
				_equipSlots[_gamepad_selectedEquipSlot].Init_EquipSlotTooltip();
			}
			else if (_gamepadItemMenuFocus == GamepadItemMenuFocus.ITEMS && (bool)_currentHighlightedDataEntry && (bool)_currentHighlightedDataEntry._scriptableItem)
			{
				_currentHighlightedDataEntry.Init_ItemEntryTooltip();
			}
		}
	}

	public override void Cell_OnAwake()
	{
		Init_PromptWindowButtons();
		_itemCell_gearButton.onClick.AddListener(() =>
		{
			Init_InventoryTab(0);
		});
		_itemCell_consumableButton.onClick.AddListener(() =>
		{
			Init_InventoryTab(1);
		});
		_itemCell_tradeItemButton.onClick.AddListener(() =>
		{
			Init_InventoryTab(2);
		});
		_itemCell_SortButton.onClick.AddListener(() =>
		{
			Sort_Items(_isStorageSort: false);
		});
		_equipDisplayButton.onClick.AddListener(Display_EquipmentPanel);
		_equipVanityButton.onClick.AddListener(Display_VanityPanel);
		_equipStatsButton.onClick.AddListener(Display_StatsPanel);
		_lockWeaponToggle_button.onClick.AddListener(Toggle_WeaponLock);
		void Init_PromptWindowButtons()
		{
			_equipButton.onClick.AddListener(PromptButton_EquipItem);
			_equipButton.onClick.AddListener(Cancel_ItemPromptWindow);
			_transmogButton.onClick.AddListener(PromptButton_TransmogEquip);
			_transmogButton.onClick.AddListener(Cancel_ItemPromptWindow);
			_removeButton.onClick.AddListener(PromptButton_RemoveEquip);
			_removeButton.onClick.AddListener(Cancel_ItemPromptWindow);
			_useButton.onClick.AddListener(PromptButton_UseItem);
			_useButton.onClick.AddListener(Cancel_ItemPromptWindow);
			_dropButton.onClick.AddListener(PromptButton_DropItem);
			_splitButton.onClick.AddListener(PromptButton_SplitItem);
			_applyQuantityButton.onClick.AddListener(Apply_QuantityInput);
			_applyQuantityButton.onClick.AddListener(Cancel_ItemPromptWindow);
			_subQuantityButton.onClick.AddListener(OnClick_SubQuantity);
			_addQuantityButton.onClick.AddListener(OnClick_AddQuantity);
			_cancelQuantityButton.onClick.AddListener(Cancel_QuantityPrompt);
			_cancelQuantityButton.onClick.AddListener(Cancel_ItemPromptWindow);
			_cancelButton.onClick.AddListener(Cancel_ItemPromptWindow);
		}
	}

	private void Start()
	{
		_itemCellBackdrop.SetActive(value: false);
		_itemSlots = _slotContainer.GetComponentsInChildren<ItemSlotUI>(includeInactive: true);
		for (int i = 0; i < _tabButtonHighlights.Length; i++)
		{
			_tabButtonHighlights[i].SetActive(value: false);
		}
	}

	public void Init_ItemPromptWindow(ItemListDataEntry _listEntry)
	{
		if (_listEntry._scriptableItem._itemType == ItemType.GEAR)
		{
			if (_currentEquipCellTab == EquipCellTab.VANITY_PANEL)
			{
				_equipButton.gameObject.SetActive(value: false);
				_removeButton.gameObject.SetActive(value: false);
				_transmogButton.gameObject.SetActive(value: true);
			}
			else
			{
				if (_listEntry._itemData._isEquipped)
				{
					_equipButton.gameObject.SetActive(value: false);
					_removeButton.gameObject.SetActive(value: true);
				}
				else
				{
					_equipButton.gameObject.SetActive(value: true);
					_removeButton.gameObject.SetActive(value: false);
				}
				_transmogButton.gameObject.SetActive(value: false);
			}
			_dropButton.gameObject.SetActive(value: true);
			_useButton.gameObject.SetActive(value: false);
		}
		else
		{
			_useButton.gameObject.SetActive(_listEntry._scriptableItem._itemType == ItemType.CONSUMABLE);
			_dropButton.gameObject.SetActive(value: true);
			_equipButton.gameObject.SetActive(value: false);
			_transmogButton.gameObject.SetActive(value: false);
			_removeButton.gameObject.SetActive(value: false);
		}
		_itemPromptWindowElement.isEnabled = true;
		_selectedItemListDataEntry = _listEntry;
		Cancel_QuantityPrompt();
		Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z) + _promptOffsetPosition;
		_itemPromptWindowElement.transform.position = position;
		_itemPromptWindowElement.transform.SetAsLastSibling();
	}

	private void PromptButton_EquipItem()
	{
		PromptCmd_EquipItem(_selectedItemListDataEntry);
	}

	public void PromptCmd_EquipItem(ItemListDataEntry _listEntry)
	{
		if (!_bufferCommandRequest && (bool)_listEntry && ((ScriptableEquipment)_listEntry._scriptableItem).CanEquipItem(Player._mainPlayer._pStats, _isClickPrompt: true))
		{
			_mainPlayer._pEquipment.Init_EquipItem(_listEntry._itemData);
		}
	}

	private void PromptButton_RemoveEquip()
	{
		PromptCmd_RemoveEquip(_selectedItemListDataEntry);
	}

	public void PromptCmd_RemoveEquip(ItemListDataEntry _listEntry)
	{
		if (!_bufferCommandRequest && (bool)_listEntry)
		{
			_mainPlayer._pEquipment.Init_UnEquipItem(_listEntry._itemData);
		}
	}

	private void PromptButton_TransmogEquip()
	{
		PromptCmd_TransmogEquip(_selectedItemListDataEntry);
	}

	public void PromptCmd_TransmogEquip(ItemListDataEntry _listEntry)
	{
		if (!_bufferCommandRequest)
		{
			if (!AbleToTransmogEquip(_listEntry))
			{
				ErrorPromptTextManager.current.Init_ErrorPrompt("Cannot Transmogrify");
			}
			else
			{
				_mainPlayer._pEquipment.Init_TransmogItem(_listEntry._itemData);
			}
		}
	}

	private void PromptButton_UseItem()
	{
		if (_currentTab == ItemType.CONSUMABLE && (bool)_selectedItemListDataEntry)
		{
			Init_UseConsumable(_selectedItemListDataEntry);
		}
	}

	private void Init_UseConsumable(ItemListDataEntry _ILDE)
	{
		if (_currentTab == ItemType.CONSUMABLE)
		{
			ScriptableConsumable scriptConsumable = (ScriptableConsumable)_ILDE._scriptableItem;
			_mainPlayer._pInventory.Init_UseConsumable(scriptConsumable, _ILDE._itemData);
		}
	}

	public void Enable_QuantityPrompt()
	{
		PromptEnable_QuantityPrompt(_isTrue: true);
	}

	private void PromptEnable_QuantityPrompt(bool _isTrue)
	{
		bool flag = InputControlManager.current._setGamepadType != GamepadType.Keyboard;
		_itemCell_quantityPromptElement.isEnabled = _isTrue;
		_itemCell_quantityDropInputField.text = string.Empty;
		if (_isTrue)
		{
			if (!ShopkeepManager._current._isOpen)
			{
				_itemCell_quantityPromptElement.transform.SetParent(base.transform.parent);
				_itemCell_quantityPromptElement.transform.SetAsLastSibling();
			}
			if (!flag)
			{
				_itemCell_quantityPromptElement.transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z) + _promptOffsetPosition;
			}
			_itemCell_quantityDropInputField.text = "1";
			_itemPromptWindowElement.isEnabled = false;
		}
	}

	public void Cancel_QuantityPrompt()
	{
		PromptEnable_QuantityPrompt(_isTrue: false);
		ShopkeepManager._current._selectedPurchaseListEntry = null;
		if (IsInvoking("OnClick_AddQuantity"))
		{
			CancelInvoke("OnClick_AddQuantity");
		}
		if (IsInvoking("OnClick_SubQuantity"))
		{
			CancelInvoke("OnClick_SubQuantity");
		}
	}

	private void OnClick_AddQuantity()
	{
		int.TryParse(_itemCell_quantityDropInputField.text, out var result);
		result++;
		if ((bool)_selectedItemListDataEntry)
		{
			if (result > _selectedItemListDataEntry._itemData._quantity)
			{
				result = 1;
			}
		}
		else if ((bool)ShopkeepManager._current._selectedPurchaseListEntry)
		{
			if (!ShopkeepManager._current._selectedPurchaseListEntry._isbuybackItem)
			{
				if (result > ShopkeepManager._current._selectedPurchaseListEntry._scriptableItem._maxStackAmount)
				{
					result = 1;
				}
			}
			else if (result > ShopkeepManager._current._selectedPurchaseListEntry._shopStruct._stockQuantity)
			{
				result = 1;
			}
		}
		else if ((bool)ShopkeepManager._current._selectedSellItemListEntry)
		{
			if (result > ShopkeepManager._current._selectedSellItemListEntry._itemData._quantity)
			{
				result = 1;
			}
		}
		else if (result > Player._mainPlayer._pInventory._heldCurrency)
		{
			result = 1;
		}
		_itemCell_quantityDropInputField.text = $"{result}";
	}

	private void OnClick_SubQuantity()
	{
		if (!(_itemCell_quantityDropInputField.text == string.Empty))
		{
			int.TryParse(_itemCell_quantityDropInputField.text, out var result);
			result--;
			if (result <= 0)
			{
				result = (_selectedItemListDataEntry ? _selectedItemListDataEntry._itemData._quantity : (ShopkeepManager._current._selectedPurchaseListEntry ? (ShopkeepManager._current._selectedPurchaseListEntry._isbuybackItem ? ShopkeepManager._current._selectedPurchaseListEntry._shopStruct._stockQuantity : ShopkeepManager._current._selectedPurchaseListEntry._scriptableItem._maxStackAmount) : ((!ShopkeepManager._current._selectedSellItemListEntry) ? Player._mainPlayer._pInventory._heldCurrency : ShopkeepManager._current._selectedSellItemListEntry._itemData._quantity)));
			}
			_itemCell_quantityDropInputField.text = $"{result}";
		}
	}

	private void OnClick_AddTenQuantity()
	{
		int.TryParse(_itemCell_quantityDropInputField.text, out var result);
		result += 10;
		if ((bool)_selectedItemListDataEntry)
		{
			if (result > _selectedItemListDataEntry._itemData._quantity)
			{
				result = 1;
			}
		}
		else if ((bool)ShopkeepManager._current._selectedPurchaseListEntry)
		{
			if (!ShopkeepManager._current._selectedPurchaseListEntry._isbuybackItem)
			{
				if (result > ShopkeepManager._current._selectedPurchaseListEntry._scriptableItem._maxStackAmount)
				{
					result = 1;
				}
			}
			else if (result > ShopkeepManager._current._selectedPurchaseListEntry._shopStruct._stockQuantity)
			{
				result = 1;
			}
		}
		else if ((bool)ShopkeepManager._current._selectedSellItemListEntry)
		{
			if (result > ShopkeepManager._current._selectedSellItemListEntry._itemData._quantity)
			{
				result = 1;
			}
		}
		else if (result > Player._mainPlayer._pInventory._heldCurrency)
		{
			result = 1;
		}
		_itemCell_quantityDropInputField.text = $"{result}";
	}

	private void OnClick_SubTenQuantity()
	{
		if (!(_itemCell_quantityDropInputField.text == string.Empty))
		{
			int.TryParse(_itemCell_quantityDropInputField.text, out var result);
			result -= 10;
			if (result <= 0)
			{
				result = (_selectedItemListDataEntry ? _selectedItemListDataEntry._itemData._quantity : (ShopkeepManager._current._selectedPurchaseListEntry ? (ShopkeepManager._current._selectedPurchaseListEntry._isbuybackItem ? ShopkeepManager._current._selectedPurchaseListEntry._shopStruct._stockQuantity : ShopkeepManager._current._selectedPurchaseListEntry._scriptableItem._maxStackAmount) : ((!ShopkeepManager._current._selectedSellItemListEntry) ? Player._mainPlayer._pInventory._heldCurrency : ShopkeepManager._current._selectedSellItemListEntry._itemData._quantity)));
			}
			_itemCell_quantityDropInputField.text = $"{result}";
		}
	}

	private void Apply_QuantityInput()
	{
		_itemCell_quantityPromptElement.isEnabled = false;
		int.TryParse(_itemCell_quantityDropInputField.text, out var _quantity);
		if (_quantity <= 0)
		{
			ShopkeepManager._current._selectedPurchaseListEntry = null;
			_isSplittingQuantityItem = false;
			return;
		}
		if (ShopkeepManager._current._isOpen)
		{
			Init_ShopQuantityApply();
		}
		else if ((bool)_selectedItemListDataEntry && _isSplittingQuantityItem)
		{
			PromptCmd_SplitItem(_quantity, _selectedItemListDataEntry._itemData);
		}
		else if (!_selectedItemListDataEntry)
		{
			PromptCmd_DropCurrency();
		}
		else
		{
			PromptCmd_DropItem(_quantity, _selectedItemListDataEntry._itemData);
		}
		_itemCell_quantityDropInputField.text = string.Empty;
		_isSplittingQuantityItem = false;
		void Init_ShopQuantityApply()
		{
			ShopkeepManager current = ShopkeepManager._current;
			if ((bool)current._selectedSellItemListEntry)
			{
				if (_quantity > current._selectedSellItemListEntry._scriptableItem._maxStackAmount)
				{
					_quantity = current._selectedSellItemListEntry._scriptableItem._maxStackAmount;
				}
				current.Init_SellItem(_quantity, current._selectedSellItemListEntry._dataID);
			}
			if ((bool)current._selectedPurchaseListEntry)
			{
				if (_quantity > current._selectedPurchaseListEntry._scriptableItem._maxStackAmount)
				{
					_quantity = current._selectedPurchaseListEntry._scriptableItem._maxStackAmount;
				}
				int num = current._selectedPurchaseListEntry._scriptableItem._vendorCost * _quantity;
				if (_mainPlayer._pInventory._heldCurrency - num < 0)
				{
					_quantity = _mainPlayer._pInventory._heldCurrency / current._selectedPurchaseListEntry._scriptableItem._vendorCost;
				}
				if (_mainPlayer._pInventory._heldCurrency >= num)
				{
					current.Init_PurchaseItem(current._selectedPurchaseListEntry._scriptableItem, _quantity, current._selectedPurchaseListEntry._keyID, current._selectedPurchaseListEntry);
				}
				else
				{
					ErrorPromptTextManager.current.Init_ErrorPrompt("Too expensive");
				}
			}
			current.Cancel_SellPrompt();
			current.Cancel_PurchasePrompt();
		}
	}

	private void PromptButton_DropItem()
	{
		if ((bool)_selectedItemListDataEntry)
		{
			_itemPromptWindowElement.isEnabled = false;
			if (_selectedItemListDataEntry._itemData._quantity > 1)
			{
				Enable_QuantityPrompt();
			}
			else
			{
				PromptCmd_DropItem(1, _selectedItemListDataEntry._itemData);
			}
		}
	}

	private void PromptButton_SplitItem()
	{
		if ((bool)_selectedItemListDataEntry)
		{
			_itemPromptWindowElement.isEnabled = false;
			if (_selectedItemListDataEntry._itemData._quantity > 1 && _currentTab != ItemType.GEAR)
			{
				_isSplittingQuantityItem = true;
				Enable_QuantityPrompt();
			}
		}
	}

	public void PromptCmd_DropItem(int _quantity, ItemData _itemData)
	{
		if (Player._mainPlayer._pInventory._withinItemDropBuffer || _bufferCommandRequest)
		{
			return;
		}
		if (_quantity <= 0)
		{
			_quantity = 0;
		}
		ScriptableItem scriptableItem = GameManager._current.Locate_Item(_itemData._itemName);
		PlayerQuesting pQuest = Player._mainPlayer._pQuest;
		ToolTipManager._current.Disable_AllTooltips();
		if (_quantity > _itemData._quantity)
		{
			_quantity = _itemData._quantity;
		}
		if (_itemData._isEquipped)
		{
			_mainPlayer._pEquipment.Init_UnEquipItem(_itemData);
		}
		if ((bool)scriptableItem._scriptableQuest && scriptableItem._abandonQuestWhenDropped)
		{
			for (int i = 0; i < pQuest._scriptableQuests.Count; i++)
			{
				if (!(pQuest._scriptableQuests[i] != scriptableItem._scriptableQuest))
				{
					Player._mainPlayer._chatBehaviour.New_ChatMessage("Abandoned Quest: " + scriptableItem._scriptableQuest._questName);
					Player._mainPlayer._pSound._aSrcPlayerUI.PlayOneShot(Player._mainPlayer._pSound._questAbandonSfx, 1f);
					pQuest.Abandon_Quest(i);
				}
			}
		}
		else
		{
			_mainPlayer._pInventory.Cmd_DropItem(_itemData, _quantity);
			_mainPlayer._pInventory.Remove_Item(_itemData, _quantity);
		}
		_selectedItemListDataEntry = null;
	}

	public void PromptCmd_DropCurrency()
	{
		if (!Player._mainPlayer._pInventory._withinItemDropBuffer)
		{
			int.TryParse(_itemCell_quantityDropInputField.text, out var result);
			_mainPlayer._pInventory.Cmd_DropCurrency(result);
			_itemCell_quantityPromptElement.isEnabled = false;
		}
	}

	public void PromptCmd_SplitItem(int _quantity, ItemData _itemData)
	{
		if (_quantity > _itemData._quantity || _quantity == _itemData._quantity)
		{
			_isSplittingQuantityItem = false;
			return;
		}
		switch (_currentTab)
		{
		case ItemType.CONSUMABLE:
			if (_mainPlayer._pInventory._usedConsumableItemSlots >= _mainPlayer._pInventory._maxInventoryStruct._maxConsumableItemSlots)
			{
				_isSplittingQuantityItem = false;
				return;
			}
			break;
		case ItemType.TRADE:
			if (_mainPlayer._pInventory._usedTradeItemSlots >= _mainPlayer._pInventory._maxInventoryStruct._maxTradeItemSlots)
			{
				_isSplittingQuantityItem = false;
				return;
			}
			break;
		}
		ItemData itemData = new ItemData
		{
			_itemName = _itemData._itemName,
			_isEquipped = false,
			_isAltWeapon = false,
			_modifierID = 0,
			_slotNumber = 0,
			_useDamageTypeOverride = false,
			_maxQuantity = _itemData._maxQuantity,
			_damageTypeOverride = DamageType.Strength,
			_quantity = _quantity
		};
		_mainPlayer._pInventory.Remove_Item(_itemData, _quantity);
		_mainPlayer._pInventory.Add_Item(itemData, _autoStack: false);
		_isSplittingQuantityItem = false;
	}

	public void Cancel_ItemPromptWindow()
	{
		_itemPromptWindowElement.isEnabled = false;
		_selectedItemListDataEntry = null;
	}

	public override void Handle_CellUpdate()
	{
		bool flag = InputControlManager.current._setGamepadType != GamepadType.Keyboard;
		QuickItemActionBarGUI._current._gamepad_quickItemSlotObject.SetActive((TabMenu._current._isOpen && TabMenu._current._selectedMenuCell == this) & flag);
		if (!TabMenu._current._isOpen && _currentTab != ItemType.GEAR)
		{
			Display_EquipmentPanel();
		}
		if (_currentEquipCellTab == EquipCellTab.STATS_PANEL)
		{
			if (TabMenu._current._statsCell._statsInfoPanel.transform.parent != base.transform)
			{
				TabMenu._current._statsCell._statsInfoPanel.SetParent(base.transform);
				TabMenu._current._statsCell._statsInfoPanel.localScale = new Vector2(1.025f, 0.96f);
				TabMenu._current._statsCell._statsInfoPanel.localPosition = new Vector2(95f, -2f);
			}
		}
		else if (TabMenu._current._statsCell._statsInfoPanel.transform.parent != TabMenu._current._statsCell.transform)
		{
			TabMenu._current._statsCell._statsInfoPanel.SetParent(TabMenu._current._statsCell.transform);
			TabMenu._current._statsCell._statsInfoPanel.localScale = new Vector2(1.025f, 0.96f);
			TabMenu._current._statsCell._statsInfoPanel.localPosition = new Vector2(95f, -2f);
		}
		Handle_RaceModelViewport();
		Handle_GamepadControl();
		if (TabMenu._current._selectedMenuCell != this)
		{
			return;
		}
		if (_weaponLockBuffer > 0f)
		{
			_weaponLockBuffer -= Time.deltaTime;
		}
		else
		{
			_weaponLockBuffer = 0f;
		}
		_lockWeaponToggle_button.interactable = _weaponLockBuffer <= 0f;
		_dyeSprite.sprite = GameManager._current._statLogics._armorDyes[Player._mainPlayer._pVisual._playerAppearanceStruct._dyeIndex]._itemIcon;
		if (Player._mainPlayer._pCombat._lockWeapon)
		{
			_lockWeaponToggle_icon.sprite = _lockWeapon_lockedSprite;
		}
		else
		{
			_lockWeaponToggle_icon.sprite = _lockWeapon_unlockedSprite;
		}
		if (!Player._mainPlayer._pCombat._lockWeapon)
		{
			_mainWeaponSlotGroup.interactable = !Player._mainPlayer._pCombat._isUsingAltWeapon;
			_altWeaponSlotGroup.interactable = Player._mainPlayer._pCombat._isUsingAltWeapon;
			_mainWeaponSlotGroup.blocksRaycasts = !Player._mainPlayer._pCombat._isUsingAltWeapon;
			_altWeaponSlotGroup.blocksRaycasts = Player._mainPlayer._pCombat._isUsingAltWeapon;
			if (Player._mainPlayer._pCombat._isUsingAltWeapon)
			{
				_mainWeaponSlotGroup.transform.SetAsFirstSibling();
				_altWeaponSlotGroup.transform.SetAsLastSibling();
				_mainWeaponSlotGroup.alpha = 0.25f;
				_altWeaponSlotGroup.alpha = 1f;
			}
			else
			{
				_altWeaponSlotGroup.transform.SetAsFirstSibling();
				_mainWeaponSlotGroup.transform.SetAsLastSibling();
				_mainWeaponSlotGroup.alpha = 1f;
				_altWeaponSlotGroup.alpha = 0.25f;
			}
		}
		else
		{
			_mainWeaponSlotGroup.interactable = true;
			_altWeaponSlotGroup.interactable = false;
			_mainWeaponSlotGroup.blocksRaycasts = true;
			_altWeaponSlotGroup.blocksRaycasts = false;
			_altWeaponSlotGroup.transform.SetAsFirstSibling();
			_mainWeaponSlotGroup.transform.SetAsLastSibling();
			_mainWeaponSlotGroup.alpha = 1f;
			_altWeaponSlotGroup.alpha = 0.25f;
		}
		if (TabMenu._current._itemTradeMode && !EnchanterManager._current._isOpen)
		{
			Cancel_ItemPromptWindow();
		}
		_equipCellTabDockObject.SetActive(!TabMenu._current._itemTradeMode);
		_itemCell_itemPromptWindowGroup.interactable = _itemPromptWindowElement.isEnabled;
		_itemCell_itemPromptWindowGroup.blocksRaycasts = _itemPromptWindowElement.isEnabled;
		_currencyDisplayCounter.text = $"{_mainPlayer._pInventory._heldCurrency:n0}";
		_itemCellTabObject.SetActive(!EnchanterManager._current._isOpen && _currentEquipCellTab != EquipCellTab.VANITY_PANEL);
		if (_currentEquipCellTab != EquipCellTab.VANITY_PANEL)
		{
			_tabButtonHighlights[(uint)_currentTab].SetActive(value: true);
		}
		else
		{
			for (int i = 0; i < _tabButtonHighlights.Length; i++)
			{
				_tabButtonHighlights[i].SetActive(value: false);
			}
			if (!TabMenu._current._isOpen && _currentEquipCellTab != EquipCellTab.EQUIPMENT_PANEL)
			{
				_currentEquipCellTab = EquipCellTab.EQUIPMENT_PANEL;
				Refresh_EquipCells();
			}
		}
		bool flag2 = (bool)_heldItemListDataEntry || Input.GetKey(KeyCode.LeftShift);
		for (int j = 0; j < _equipSlots.Length; j++)
		{
			if ((bool)_equipSlots[j]._hideArmorToggleButton)
			{
				_equipSlots[j]._hideArmorToggleButton.gameObject.SetActive(!flag2);
			}
		}
		if ((bool)_mainPlayer._pCombat._currentScriptableWeaponType)
		{
			_shieldErrorIcon.enabled = _mainPlayer._pCombat._currentScriptableWeaponType._weaponHandedness == WeaponHandedness.Two_Handed;
		}
		switch (_currentTab)
		{
		case ItemType.GEAR:
			_itemCell_gearContent.localScale = Vector3.one;
			_itemCell_consumableContent.localScale = Vector3.zero;
			_itemCell_tradeContent.localScale = Vector3.zero;
			break;
		case ItemType.CONSUMABLE:
			_itemCell_gearContent.localScale = Vector3.zero;
			_itemCell_consumableContent.localScale = Vector3.one;
			_itemCell_tradeContent.localScale = Vector3.zero;
			break;
		case ItemType.TRADE:
			_itemCell_gearContent.localScale = Vector3.zero;
			_itemCell_consumableContent.localScale = Vector3.zero;
			_itemCell_tradeContent.localScale = Vector3.one;
			break;
		}
		_splitButton.gameObject.SetActive((bool)_selectedItemListDataEntry && _selectedItemListDataEntry._itemData._quantity > 1);
		switch (_currentEquipCellTab)
		{
		case EquipCellTab.EQUIPMENT_PANEL:
			_equipTabHeader.text = "Equipment";
			break;
		case EquipCellTab.VANITY_PANEL:
			_currentTab = ItemType.GEAR;
			_equipTabHeader.text = "Vanity";
			_currentIllusionStones = Player._mainPlayer._pInventory.Check_ItemQuantity(GameManager._current.Locate_Item("Illusion Stone"));
			if ((bool)_selectedItemListDataEntry && _currentTab == ItemType.GEAR)
			{
				_transmogButton.gameObject.SetActive(AbleToTransmogEquip(_selectedItemListDataEntry));
				_transmogButton.interactable = AbleToTransmogEquip(_selectedItemListDataEntry);
			}
			else
			{
				_transmogButton.interactable = false;
			}
			break;
		}
		_illusionStoneCounterElement.isEnabled = _currentEquipCellTab == EquipCellTab.VANITY_PANEL;
		_illusionStoneCounter.text = $"x{_currentIllusionStones}";
		void Handle_RaceModelViewport()
		{
			if (!_mainPlayer._bufferingStatus && (bool)_mainPlayer._pVisual._playerRaceModel)
			{
				for (int k = 0; k < _playerRaceModelDisplays.Length; k++)
				{
					if (_mainPlayer._pVisual._playerRaceModel._scriptablePlayerRace != _playerRaceModelDisplays[k]._scriptablePlayerRace)
					{
						_playerRaceModelDisplays[k].gameObject.SetActive(value: false);
					}
					else
					{
						_setRaceModel = _playerRaceModelDisplays[k];
					}
				}
				if ((bool)_setRaceModel)
				{
					if (!VanityMirrorManager._current._isOpen)
					{
						Apply_RaceModelDisplayParams();
					}
					if (!TabMenu._current._isOpen && !VanityMirrorManager._current._isOpen)
					{
						Reset_RaceModelDisplayRotation();
					}
					if (_isRotatingDisplay)
					{
						_raceModelDisplayContainer.Rotate(_rotationValue * Time.deltaTime * Vector3.up);
					}
					_raceModelDisplayContainer.parent.gameObject.SetActive(TabMenu._current._isOpen || VanityMirrorManager._current._isOpen);
				}
			}
		}
	}

	public void Init_InventoryTab(int _value)
	{
		for (int i = 0; i < _tabButtonHighlights.Length; i++)
		{
			_tabButtonHighlights[i].SetActive(value: false);
		}
		_currentTab = (ItemType)_value;
		Cancel_ItemPromptWindow();
		Cancel_QuantityPrompt();
	}

	public void Sort_Items(bool _isStorageSort)
	{
		ItemSlotUI[] _setItemSlots;
		ListDataEntry[] _setListEntries;
		if (!_heldItemListDataEntry)
		{
			_setItemSlots = _itemSlots;
			_setListEntries = _listDataEntries.ToArray();
			if (_isStorageSort)
			{
				_setItemSlots = ItemStorageManager._current._storageItemSlots;
				ListDataEntry[] array = ItemStorageManager._current._storageListEntries.ToArray();
				_setListEntries = array;
			}
			for (int i = 0; i < _setItemSlots.Length; i++)
			{
				_setItemSlots[i]._filledSlotTab[(uint)_currentTab] = false;
			}
			Sort_EquipmentItems();
			Sort_ConsumableItems();
			Sort_TradeItems();
			ProfileDataManager._current.Save_ProfileData();
			if (_isStorageSort)
			{
				ProfileDataManager._current.Save_ItemStorageData();
			}
			ToolTipManager._current.Disable_AllTooltips();
			if (TabMenu._current._selectedMenuCell == this && TabMenu._current._isOpen)
			{
				_aSrc_sortItems.Play();
			}
		}
		void Sort_ConsumableItems()
		{
			if (_currentTab == ItemType.CONSUMABLE)
			{
				List<ItemListDataEntry> list = new List<ItemListDataEntry>();
				for (int j = 0; j < _setListEntries.Length; j++)
				{
					ItemListDataEntry itemListDataEntry = (ItemListDataEntry)_setListEntries[j];
					if (_currentTab == ItemType.CONSUMABLE && itemListDataEntry._itemTypeTab == _currentTab)
					{
						list.Add(itemListDataEntry);
					}
				}
				list = (from ch in list
					orderby ch._scriptableItem._itemSortTag descending, ch._scriptableItem._itemName, ch._scriptableItem._vendorCost, ch._scriptableItem._maxStackAmount, ch._itemData._quantity
					select ch).Reverse().ToList();
				int num = 0;
				for (int num2 = 0; num2 < list.Count; num2++)
				{
					_ = list[num2];
					list[num2]._parentItemSlotUI = _setItemSlots[num];
					list[num2]._parentItemSlotUI._filledSlotTab[(uint)_currentTab] = true;
					list[num2]._itemData._slotNumber = num;
					num++;
				}
			}
		}
		void Sort_EquipmentItems()
		{
			if (_currentTab == ItemType.GEAR)
			{
				List<ItemListDataEntry> list = new List<ItemListDataEntry>();
				int num = 0;
				for (int j = 0; j < _setListEntries.Length; j++)
				{
					ItemListDataEntry itemListDataEntry = (ItemListDataEntry)_setListEntries[j];
					ScriptableItem scriptableItem = GameManager._current.Locate_Item(itemListDataEntry._itemData._itemName);
					if ((bool)scriptableItem && scriptableItem._itemType == ItemType.GEAR)
					{
						_ = (ScriptableEquipment)scriptableItem;
						if (!itemListDataEntry._itemData._isEquipped)
						{
							list.Add(itemListDataEntry);
						}
					}
				}
				list = (from ch in list
					orderby ch._scriptableItem._itemSortTag descending, ch._scriptableItem._itemRarity, ch._itemData._modifierID
					select ch).Reverse().ToList();
				for (int num2 = 0; num2 < list.Count; num2++)
				{
					list[num2]._parentItemSlotUI = _setItemSlots[num];
					list[num2]._parentItemSlotUI._filledSlotTab[(uint)_currentTab] = true;
					list[num2]._itemData._slotNumber = num;
					num++;
				}
			}
		}
		void Sort_TradeItems()
		{
			if (_currentTab == ItemType.TRADE)
			{
				List<ItemListDataEntry> list = new List<ItemListDataEntry>();
				for (int j = 0; j < _setListEntries.Length; j++)
				{
					ItemListDataEntry itemListDataEntry = (ItemListDataEntry)_setListEntries[j];
					if (_currentTab == ItemType.TRADE && itemListDataEntry._itemTypeTab == _currentTab)
					{
						list.Add(itemListDataEntry);
					}
				}
				list = (from ch in list
					orderby ch._scriptableItem._itemSortTag descending, ch._scriptableItem._itemName, ch._scriptableItem._vendorCost, ch._scriptableItem._maxStackAmount, ch._itemData._quantity
					select ch).Reverse().ToList();
				int num = 0;
				for (int num2 = 0; num2 < list.Count; num2++)
				{
					_ = list[num2];
					list[num2]._parentItemSlotUI = _setItemSlots[num];
					list[num2]._parentItemSlotUI._filledSlotTab[(uint)_currentTab] = true;
					list[num2]._itemData._slotNumber = num;
					num++;
				}
			}
		}
	}

	private void Handle_GamepadControl()
	{
		if (_gamepad_buffer > 0f)
		{
			_gamepad_buffer -= Time.deltaTime;
		}
		else
		{
			_gamepad_buffer = 0f;
		}
		bool _inGamepadMode = InputControlManager.current._setGamepadType != GamepadType.Keyboard && InputControlManager.current._gamepadMouseCursorBuffer <= 0f;
		bool _isInputUp = GamepadInput.GetButtonDown(GamepadButton.RightStick_Up) || InputControlManager.current.DpadUp(DpadHoldType.OnDown);
		bool _isInputDown = GamepadInput.GetButtonDown(GamepadButton.RightStick_Down) || InputControlManager.current.DpadDown(DpadHoldType.OnDown);
		bool _isInputLeft = GamepadInput.GetButtonDown(GamepadButton.RightStick_Left) || InputControlManager.current.DpadLeft(DpadHoldType.OnDown);
		bool _isInputRight = GamepadInput.GetButtonDown(GamepadButton.RightStick_Right) || InputControlManager.current.DpadRight(DpadHoldType.OnDown);
		bool _isInputLeftHeld = GamepadInput.GetButton(GamepadButton.RightStick_Left) || InputControlManager.current.DpadLeft(DpadHoldType.OnHold);
		bool _isInputRightHeld = GamepadInput.GetButton(GamepadButton.RightStick_Right) || InputControlManager.current.DpadRight(DpadHoldType.OnHold);
		bool button = GamepadInput.GetButton(GamepadButton.Button_South);
		if (DialogManager._current._isDialogEnabled)
		{
			_isInputUp = GamepadInput.GetButtonDown(GamepadButton.LeftStick_Up) || GamepadInput.GetButtonDown(GamepadButton.RightStick_Up) || InputControlManager.current.DpadUp(DpadHoldType.OnDown);
			_isInputDown = GamepadInput.GetButtonDown(GamepadButton.LeftStick_Down) || GamepadInput.GetButtonDown(GamepadButton.RightStick_Down) || InputControlManager.current.DpadDown(DpadHoldType.OnDown);
			_isInputLeft = GamepadInput.GetButtonDown(GamepadButton.LeftStick_Left) || GamepadInput.GetButtonDown(GamepadButton.RightStick_Left) || InputControlManager.current.DpadLeft(DpadHoldType.OnDown);
			_isInputRight = GamepadInput.GetButtonDown(GamepadButton.LeftStick_Right) || GamepadInput.GetButtonDown(GamepadButton.RightStick_Right) || InputControlManager.current.DpadRight(DpadHoldType.OnDown);
			_isInputLeftHeld = GamepadInput.GetButton(GamepadButton.LeftStick_Left) || GamepadInput.GetButton(GamepadButton.RightStick_Left) || InputControlManager.current.DpadLeft(DpadHoldType.OnHold);
			_isInputRightHeld = GamepadInput.GetButton(GamepadButton.LeftStick_Right) || GamepadInput.GetButton(GamepadButton.RightStick_Right) || InputControlManager.current.DpadRight(DpadHoldType.OnHold);
		}
		_gamepad_quantityIco_left.enabled = _inGamepadMode;
		_gamepad_quantityIco_right.enabled = _inGamepadMode;
		_gamepad_quantityIco_confirm.enabled = _inGamepadMode;
		_gamepad_quantityIco_cancel.enabled = _inGamepadMode;
		if (_inGamepadMode)
		{
			_itemCell_quantityPromptRect.anchoredPosition = _gamepad_quantityPromptPos;
		}
		else
		{
			_gamepad_southButtonContext.isEnabled = false;
			_gamepad_eastButtonContext.isEnabled = false;
			_gamepad_westButtonContext.isEnabled = false;
		}
		if (_gamepad_quantityPromptSelected)
		{
			Handle_QuantityPromptGamepadControl();
			return;
		}
		if (DialogManager._current._isDialogEnabled && DialogManager._current._currentGamepadTab != DialogManager_GamepadTab.Tab_Menu)
		{
			_gamepad_southButtonContext.isEnabled = false;
			_gamepad_eastButtonContext.isEnabled = false;
			_gamepad_westButtonContext.isEnabled = false;
			_inGamepadMode = false;
		}
		_gamepad_itemTabCursor.gameObject.SetActive(_inGamepadMode && _gamepadItemMenuFocus == GamepadItemMenuFocus.ITEMS && !_gamepad_currencyCellSelected);
		_gamepadCursorObject_itemTabs.gameObject.SetActive(_inGamepadMode && _gamepadItemMenuFocus == GamepadItemMenuFocus.ITEM_TAB);
		_gamepadCursorObject_equipTabs.gameObject.SetActive(_inGamepadMode && _gamepadItemMenuFocus == GamepadItemMenuFocus.VIEW_EQUIP_TAB);
		_gamepad_equipmentPanelCursor.gameObject.SetActive(_inGamepadMode && _gamepadItemMenuFocus == GamepadItemMenuFocus.EQUIPMENT);
		_gamepad_equipPanelSelection.gameObject.SetActive(_inGamepadMode && _gamepadItemMenuFocus == GamepadItemMenuFocus.EQUIPMENT);
		_gamepad_rightStickPush_sortIco.enabled = _inGamepadMode && _gamepadItemMenuFocus != GamepadItemMenuFocus.EQUIPMENT && _currentEquipCellTab != EquipCellTab.VANITY_PANEL && _itemCellTabObject.activeInHierarchy;
		_gamepad_leftTriggerViewportIco.enabled = _inGamepadMode && _gamepadItemMenuFocus == GamepadItemMenuFocus.EQUIPMENT;
		_gamepad_rightTriggerViewportIco.enabled = _inGamepadMode && _gamepadItemMenuFocus == GamepadItemMenuFocus.EQUIPMENT;
		_gamepad_itemCursor_leftTriggerIco.enabled = !_heldItemListDataEntry;
		_gamepad_itemCursor_rightTriggerIco.enabled = !_heldItemListDataEntry;
		if (GamepadInput.GetButton(GamepadButton.LeftTrigger))
		{
			_gamepad_itemCursor_rightTriggerIco.color = Color.white;
		}
		else
		{
			_gamepad_itemCursor_rightTriggerIco.color = Color.grey;
		}
		if (GamepadInput.GetButton(GamepadButton.RightTrigger))
		{
			_gamepad_itemCursor_leftTriggerIco.color = Color.white;
		}
		else
		{
			_gamepad_itemCursor_leftTriggerIco.color = Color.grey;
		}
		if (GamepadInput.GetButton(GamepadButton.RightStick))
		{
			_gamepad_rightStickPush_sortIco.color = Color.white;
		}
		else
		{
			_gamepad_rightStickPush_sortIco.color = Color.grey;
		}
		_gamepad_quantityPromptSelected = _itemCell_quantityPromptElement.isEnabled;
		_gamepad_currencyCell._isSelected = _gamepad_currencyCellSelected;
		if (!TabMenu._current._isOpen || TabMenu._current._selectedMenuCell != this)
		{
			_gamepadItemMenuFocus = GamepadItemMenuFocus.ITEMS;
			_gamepad_selectedItemSlot = 0;
			_gamepad_selectedEquipTab = 0;
			_gamepad_selectedEquipSlot = 0;
			_gamepad_eastButtonContext.isEnabled = false;
			_gamepad_southButtonContext.isEnabled = false;
			_gamepad_westButtonContext.isEnabled = false;
			_gamepad_currencyCellSelected = false;
			_gamepad_quantityPromptSelected = false;
			return;
		}
		if (TabMenu._current._itemTradeMode || TabMenu._current._questPanelMode)
		{
			_gamepad_selectedEquipTab = 0;
		}
		if (!_inGamepadMode)
		{
			return;
		}
		if (_gamepadItemMenuFocus != GamepadItemMenuFocus.EQUIPMENT && _isRotatingDisplay)
		{
			Stop_RaceModelDisplayRotation();
			Reset_RaceModelDisplayRotation();
		}
		if (_gamepad_selectedEquipSlot == 4 && Player._mainPlayer._pCombat._isUsingAltWeapon && !Player._mainPlayer._pCombat._lockWeapon)
		{
			_gamepad_selectedEquipSlot = 5;
		}
		if (_gamepad_selectedEquipSlot == 5 && (!Player._mainPlayer._pCombat._isUsingAltWeapon || Player._mainPlayer._pCombat._lockWeapon))
		{
			_gamepad_selectedEquipSlot = 4;
		}
		if (_currentEquipCellTab == EquipCellTab.STATS_PANEL && _gamepadItemMenuFocus == GamepadItemMenuFocus.EQUIPMENT)
		{
			_gamepadItemMenuFocus = GamepadItemMenuFocus.ITEMS;
		}
		if (GamepadInput.GetButtonDown(GamepadButton.RightStick))
		{
			if (_gamepadItemMenuFocus != GamepadItemMenuFocus.EQUIPMENT)
			{
				Sort_Items(_isStorageSort: false);
			}
			else
			{
				Reset_RaceModelDisplayRotation();
			}
		}
		_gamepad_southButtonContext.isEnabled = DialogManager._current._currentGamepadTab == DialogManager_GamepadTab.Tab_Menu && TabMenu._current._itemTradeMode && _gamepadItemMenuFocus == GamepadItemMenuFocus.ITEMS && (bool)_currentHighlightedDataEntry && !_selectedItemListDataEntry && !_heldItemListDataEntry;
		switch (_gamepadItemMenuFocus)
		{
		case GamepadItemMenuFocus.ITEMS:
			_gamepad_itemTabCursor.anchoredPosition = _itemSlots[_gamepad_selectedItemSlot].GetComponent<RectTransform>().anchoredPosition;
			_gamepad_eastButtonContext.isEnabled = _inGamepadMode && !_heldItemListDataEntry && (bool)_currentHighlightedDataEntry && !TabMenu._current._itemTradeMode;
			_gamepad_eastButtonContext_text.text = "Drop Item";
			if ((bool)_currentHighlightedDataEntry && !_heldItemListDataEntry)
			{
				if (!DialogManager._current._isDialogEnabled && (bool)_currentHighlightedDataEntry && _currentHighlightedDataEntry._itemData._quantity > 1)
				{
					_gamepad_westButtonContext.isEnabled = true;
					_gamepad_westButtonContext_text.text = "Split Item";
				}
				else if (TabMenu._current._itemTradeMode && (bool)_currentHighlightedDataEntry && _currentHighlightedDataEntry._itemData._quantity > 1)
				{
					_gamepad_westButtonContext.isEnabled = true;
					_gamepad_westButtonContext_text.text = "Sell Quantity";
				}
				else
				{
					_gamepad_westButtonContext.isEnabled = false;
				}
			}
			else
			{
				_gamepad_westButtonContext.isEnabled = false;
			}
			if (!DialogManager._current._isDialogEnabled && !_heldItemListDataEntry)
			{
				if (_gamepad_selectedItemSlot <= 23 && _gamepad_selectedItemSlot >= 20 && _isInputDown)
				{
					_gamepad_currencyCellSelected = true;
				}
				if (_gamepad_currencyCellSelected & _isInputUp)
				{
					_gamepad_currencyCellSelected = false;
					_gamepad_buffer = 0.2f;
				}
			}
			if (!button && !_gamepad_currencyCellSelected && _gamepad_buffer <= 0f)
			{
				if (_isInputLeft)
				{
					Gamepad_SelectedItemSlot--;
				}
				if (_isInputRight)
				{
					Gamepad_SelectedItemSlot++;
				}
				if (_isInputUp)
				{
					Gamepad_SelectedItemSlot -= 4;
				}
				if (_isInputDown)
				{
					Gamepad_SelectedItemSlot += 4;
				}
			}
			if ((bool)_currentHighlightedDataEntry && !_heldItemListDataEntry)
			{
				if (TabMenu._current._itemTradeMode)
				{
					switch (DialogManager._current._currentDialogUIPrompt)
					{
					case DialogUIPrompt.SHOP:
						_gamepad_southButtonContext_text.text = "Sell Item";
						break;
					case DialogUIPrompt.ENCHANT:
						_gamepad_southButtonContext_text.text = "Enchant Slot Item";
						break;
					case DialogUIPrompt.STORAGE:
						_gamepad_southButtonContext_text.text = "Put in Storage";
						break;
					}
				}
				if (GamepadInput.GetButtonDown(GamepadButton.Button_South) && TabMenu._current._itemTradeMode)
				{
					switch (DialogManager._current._currentDialogUIPrompt)
					{
					case DialogUIPrompt.SHOP:
						ShopkeepManager._current.Init_SellItem(1, _currentHighlightedDataEntry._dataID);
						break;
					case DialogUIPrompt.ENCHANT:
						EnchanterManager._current.Clear_EnchantOption();
						EnchanterManager._current._setItemData = _currentHighlightedDataEntry._itemData;
						break;
					case DialogUIPrompt.STORAGE:
						_currentHighlightedDataEntry.Init_PutItemIntoStorage(0);
						break;
					}
				}
				if (GamepadInput.GetButtonDown(GamepadButton.LeftTrigger) && InputControlManager.current._gamepadMouseCursorBuffer <= 0f)
				{
					switch (_currentHighlightedDataEntry._itemTypeTab)
					{
					case ItemType.GEAR:
						if (_currentEquipCellTab != EquipCellTab.VANITY_PANEL)
						{
							PromptCmd_EquipItem(_currentHighlightedDataEntry);
						}
						else
						{
							PromptCmd_TransmogEquip(_currentHighlightedDataEntry);
						}
						break;
					case ItemType.CONSUMABLE:
						Init_UseConsumable(_currentHighlightedDataEntry);
						break;
					}
					ToolTipManager._current.Disable_AllTooltips();
				}
			}
			if (GamepadInput.GetButtonDown(GamepadButton.Button_West))
			{
				if (TabMenu._current._itemTradeMode && !_heldItemListDataEntry && _currentHighlightedDataEntry._itemData._quantity > 1)
				{
					ShopkeepManager._current._selectedSellItemListEntry = _currentHighlightedDataEntry;
					Enable_QuantityPrompt();
				}
				if (_currentTab == ItemType.GEAR)
				{
					Init_GamepadToolTip();
				}
			}
			if (!TabMenu._current._itemTradeMode && (bool)_currentHighlightedDataEntry && !_heldItemListDataEntry)
			{
				if (GamepadInput.GetButtonDown(GamepadButton.Button_East))
				{
					if ((bool)_currentHighlightedDataEntry && _currentHighlightedDataEntry._itemData._quantity > 1)
					{
						_selectedItemListDataEntry = _currentHighlightedDataEntry;
						_isSplittingQuantityItem = false;
						Enable_QuantityPrompt();
						ToolTipManager._current.Disable_AllTooltips();
					}
					else
					{
						PromptCmd_DropItem(_currentHighlightedDataEntry._itemData._quantity, _currentHighlightedDataEntry._itemData);
						ToolTipManager._current.Disable_AllTooltips();
					}
				}
				if (GamepadInput.GetButtonDown(GamepadButton.Button_West))
				{
					if (TabMenu._current._itemTradeMode && _currentHighlightedDataEntry._itemData._quantity > 1)
					{
						ShopkeepManager._current._selectedSellItemListEntry = _currentHighlightedDataEntry;
						Enable_QuantityPrompt();
					}
					if (((bool)_currentHighlightedDataEntry && _currentHighlightedDataEntry._itemData._quantity > 1) || ((bool)_heldItemListDataEntry && _heldItemListDataEntry._itemData._quantity > 1))
					{
						_selectedItemListDataEntry = _currentHighlightedDataEntry;
						_isSplittingQuantityItem = true;
						Enable_QuantityPrompt();
						ToolTipManager._current.Disable_AllTooltips();
					}
				}
			}
			if (button)
			{
				if (InputControlManager.current.DpadLeft(DpadHoldType.OnDown))
				{
					if ((bool)_heldItemListDataEntry)
					{
						_heldItemListDataEntry.Set_ToQuickSlot(QuickItemActionBarGUI._current._quickItemSlots[0]);
						Init_GamepadPlaceItem();
					}
					else
					{
						QuickItemActionBarGUI._current._quickItemSlots[0].Clear_ItemData();
					}
				}
				if (InputControlManager.current.DpadRight(DpadHoldType.OnDown))
				{
					if ((bool)_heldItemListDataEntry)
					{
						_heldItemListDataEntry.Set_ToQuickSlot(QuickItemActionBarGUI._current._quickItemSlots[1]);
						Init_GamepadPlaceItem();
					}
					else
					{
						QuickItemActionBarGUI._current._quickItemSlots[1].Clear_ItemData();
					}
				}
				if (InputControlManager.current.DpadUp(DpadHoldType.OnDown))
				{
					if ((bool)_heldItemListDataEntry)
					{
						_heldItemListDataEntry.Set_ToQuickSlot(QuickItemActionBarGUI._current._quickItemSlots[2]);
						Init_GamepadPlaceItem();
					}
					else
					{
						QuickItemActionBarGUI._current._quickItemSlots[2].Clear_ItemData();
					}
				}
				if (InputControlManager.current.DpadDown(DpadHoldType.OnDown))
				{
					if ((bool)_heldItemListDataEntry)
					{
						_heldItemListDataEntry.Set_ToQuickSlot(QuickItemActionBarGUI._current._quickItemSlots[3]);
						Init_GamepadPlaceItem();
					}
					else
					{
						QuickItemActionBarGUI._current._quickItemSlots[3].Clear_ItemData();
					}
				}
			}
			if (GamepadInput.GetButtonDown(GamepadButton.RightTrigger) && InputControlManager.current._gamepadMouseCursorBuffer <= 0f)
			{
				Init_GamepadGrabItem();
			}
			if (GamepadInput.GetButtonUp(GamepadButton.RightTrigger) || InputControlManager.current._gamepadMouseCursorBuffer > 0f)
			{
				Init_GamepadPlaceItem();
			}
			break;
		case GamepadItemMenuFocus.ITEM_TAB:
			_gamepad_eastButtonContext.isEnabled = false;
			_gamepad_westButtonContext.isEnabled = false;
			_gamepad_eastButtonContext_text.text = "Drop Item";
			if (InputControlManager.current._gamepadMouseCursorBuffer <= 0f)
			{
				ToolTipManager._current.Disable_AllTooltips();
			}
			switch (_gamepad_selectedItemTab)
			{
			case 0:
				_gamepadCursorObject_itemTabs.anchoredPosition = _gearButton_transform.anchoredPosition;
				break;
			case 1:
				_gamepadCursorObject_itemTabs.anchoredPosition = _consumableButton_transform.anchoredPosition;
				break;
			case 2:
				_gamepadCursorObject_itemTabs.anchoredPosition = _tradeButton_transform.anchoredPosition;
				break;
			}
			if (_isInputUp)
			{
				Gamepad_SelectedItemTab--;
			}
			if (_isInputDown)
			{
				Gamepad_SelectedItemTab++;
			}
			if (_isInputRight)
			{
				_gamepadItemMenuFocus = GamepadItemMenuFocus.ITEMS;
			}
			break;
		case GamepadItemMenuFocus.VIEW_EQUIP_TAB:
			_gamepad_eastButtonContext.isEnabled = false;
			_gamepad_westButtonContext.isEnabled = false;
			switch (_gamepad_selectedEquipTab)
			{
			case 0:
				_gamepadCursorObject_equipTabs.anchoredPosition = _equipDisplayButtonTransform.anchoredPosition;
				break;
			case 1:
				_gamepadCursorObject_equipTabs.anchoredPosition = _equipVanityButtonTransform.anchoredPosition;
				break;
			case 2:
				_gamepadCursorObject_equipTabs.anchoredPosition = _equipStatsButtonTransform.anchoredPosition;
				break;
			}
			if (_isInputLeft)
			{
				Gamepad_SelectedEquipTab--;
			}
			if (_isInputRight)
			{
				Gamepad_SelectedEquipTab++;
			}
			if (_isInputDown)
			{
				_gamepadItemMenuFocus = GamepadItemMenuFocus.EQUIPMENT;
			}
			break;
		case GamepadItemMenuFocus.EQUIPMENT:
			_gamepad_westButtonContext.isEnabled = (bool)_equipSlots[_gamepad_selectedEquipSlot]._hideArmorToggleButton || _equipSlots[_gamepad_selectedEquipSlot]._equipSlot_tag == "Weapon" || _equipSlots[_gamepad_selectedEquipSlot]._equipSlot_tag == "Alt Weapon";
			if (_equipSlots[_gamepad_selectedEquipSlot]._equipSlot_tag == "Weapon" || _equipSlots[_gamepad_selectedEquipSlot]._equipSlot_tag == "Alt Weapon")
			{
				_gamepad_westButtonContext_text.text = "Weapon Lock";
			}
			else if ((bool)_equipSlots[_gamepad_selectedEquipSlot]._hideArmorToggleButton)
			{
				_gamepad_westButtonContext_text.text = "Show / Hide";
			}
			if (GamepadInput.GetButtonDown(GamepadButton.Button_West))
			{
				if ((bool)_equipSlots[_gamepad_selectedEquipSlot]._hideArmorToggleButton)
				{
					_equipSlots[_gamepad_selectedEquipSlot]._hideArmorToggleButton.onClick.Invoke();
				}
				if (_equipSlots[_gamepad_selectedEquipSlot]._equipSlot_tag == "Weapon" || _equipSlots[_gamepad_selectedEquipSlot]._equipSlot_tag == "Alt Weapon")
				{
					_lockWeaponToggle_button.GetComponent<ButtonSound>().Play_ClickSound();
					Toggle_WeaponLock();
				}
				ToolTipManager._current.Disable_AllTooltips();
			}
			if (_currentEquipCellTab == EquipCellTab.VANITY_PANEL)
			{
				_gamepad_eastButtonContext.isEnabled = _equipSlots[_gamepad_selectedEquipSlot].HasVanity();
				_gamepad_eastButtonContext_text.text = "Remove Vanity";
				if (GamepadInput.GetButtonDown(GamepadButton.Button_East) && _equipSlots[_gamepad_selectedEquipSlot].HasVanity())
				{
					_equipSlots[_gamepad_selectedEquipSlot].Init_RemoveVanity();
				}
			}
			else
			{
				_gamepad_eastButtonContext.isEnabled = _equipSlots[_gamepad_selectedEquipSlot]._setEntry;
				_gamepad_eastButtonContext_text.text = "Remove Equip";
				if (GamepadInput.GetButtonDown(GamepadButton.Button_East) && (bool)_equipSlots[_gamepad_selectedEquipSlot]._setEntry)
				{
					PromptCmd_RemoveEquip(_equipSlots[_gamepad_selectedEquipSlot]._setEntry);
					ToolTipManager._current.Disable_AllTooltips();
				}
			}
			_gamepad_equipmentPanelCursor.transform.position = _equipSlots[_gamepad_selectedEquipSlot].transform.position;
			_gamepad_leftTriggerViewportIco.color = Color.grey;
			_gamepad_rightTriggerViewportIco.color = Color.grey;
			if (GamepadInput.GetButton(GamepadButton.LeftTrigger))
			{
				Start_RaceModelDisplayRotation_Left();
				_gamepad_leftTriggerViewportIco.color = Color.white;
				ToolTipManager._current.Disable_AllTooltips();
			}
			if (GamepadInput.GetButton(GamepadButton.RightTrigger))
			{
				Start_RaceModelDisplayRotation_Right();
				_gamepad_rightTriggerViewportIco.color = Color.white;
				ToolTipManager._current.Disable_AllTooltips();
			}
			if (GamepadInput.GetButtonUp(GamepadButton.LeftTrigger))
			{
				Stop_RaceModelDisplayRotation();
			}
			if (GamepadInput.GetButtonUp(GamepadButton.RightTrigger))
			{
				Stop_RaceModelDisplayRotation();
			}
			if (_isInputLeft)
			{
				Gamepad_SelectedEquipSlot -= 4;
			}
			if (_isInputRight)
			{
				Gamepad_SelectedEquipSlot += 4;
			}
			if (_isInputUp)
			{
				Gamepad_SelectedEquipSlot--;
			}
			if (_isInputDown)
			{
				Gamepad_SelectedEquipSlot++;
			}
			break;
		}
		if (Cursor.visible)
		{
			return;
		}
		if (_gamepadItemMenuFocus == GamepadItemMenuFocus.EQUIPMENT)
		{
			_currentHighlightedItemSlot = null;
			_currentHighlightedDataEntry = null;
			return;
		}
		_currentHighlightedItemSlot = _itemSlots[_gamepad_selectedItemSlot];
		for (int i = 0; i < _listDataEntries.Count; i++)
		{
			ItemListDataEntry itemListDataEntry = (ItemListDataEntry)_listDataEntries[i];
			if (itemListDataEntry._itemTypeTab == _currentTab && itemListDataEntry._parentItemSlotUI == _currentHighlightedItemSlot)
			{
				_currentHighlightedDataEntry = itemListDataEntry;
				break;
			}
			if (i >= _listDataEntries.Count - 1)
			{
				_currentHighlightedDataEntry = null;
			}
		}
		void Handle_QuantityPromptGamepadControl()
		{
			if (!_inGamepadMode)
			{
				_gamepad_quantityPromptSelected = false;
			}
			else if (!TabMenu._current._isOpen || TabMenu._current._selectedMenuCell != this)
			{
				_gamepad_quantityPromptSelected = false;
			}
			else if (GamepadInput.GetButtonDown(GamepadButton.LeftShoulder) || GamepadInput.GetButtonDown(GamepadButton.RightShoulder))
			{
				_gamepad_quantityPromptSelected = false;
			}
			else
			{
				DialogManager._current._gamepad_cancelBuffer = 0.2f;
				_gamepad_eastButtonContext.isEnabled = false;
				_gamepad_southButtonContext.isEnabled = false;
				_gamepad_westButtonContext.isEnabled = false;
				if (InputControlManager.current._gamepadMouseCursorBuffer <= 0f)
				{
					ToolTipManager._current.Disable_AllTooltips();
				}
				if (_isInputLeft)
				{
					OnClick_SubQuantity();
				}
				if (_isInputRight)
				{
					OnClick_AddQuantity();
				}
				if (_isInputUp)
				{
					OnClick_AddTenQuantity();
				}
				if (_isInputDown)
				{
					OnClick_SubTenQuantity();
				}
				if (_isInputRightHeld)
				{
					if (!IsInvoking("OnClick_AddQuantity"))
					{
						InvokeRepeating("OnClick_AddQuantity", 0.2f, 0.08f);
					}
				}
				else if (IsInvoking("OnClick_AddQuantity"))
				{
					CancelInvoke("OnClick_AddQuantity");
				}
				if (_isInputLeftHeld)
				{
					if (!IsInvoking("OnClick_SubQuantity"))
					{
						InvokeRepeating("OnClick_SubQuantity", 0.2f, 0.08f);
					}
				}
				else if (IsInvoking("OnClick_SubQuantity"))
				{
					CancelInvoke("OnClick_SubQuantity");
				}
				if (GamepadInput.GetButtonDown(GamepadButton.Button_East))
				{
					Cancel_QuantityPrompt();
					_selectedItemListDataEntry = null;
					_gamepad_quantityPromptSelected = false;
					ShopkeepManager._current._selectedSellItemListEntry = null;
				}
				if (GamepadInput.GetButtonDown(GamepadButton.Button_South))
				{
					Apply_QuantityInput();
					Cancel_QuantityPrompt();
					_selectedItemListDataEntry = null;
					_gamepad_quantityPromptSelected = false;
					ShopkeepManager._current._selectedSellItemListEntry = null;
				}
			}
		}
	}

	public void Init_GamepadGrabItem()
	{
		if ((bool)_currentHighlightedDataEntry)
		{
			_heldItemListDataEntry = _currentHighlightedDataEntry;
			_currentHighlightedDataEntry._isHeldByPointer = true;
			if ((bool)_currentHighlightedDataEntry._parentItemSlotUI)
			{
				_currentHighlightedDataEntry._parentItemSlotUI._filledSlotTab[(uint)_currentHighlightedDataEntry._itemTypeTab] = false;
				_currentHighlightedDataEntry._originItemSlotUI = _currentHighlightedDataEntry._parentItemSlotUI;
			}
			_currentHighlightedDataEntry._parentItemSlotUI = null;
			_aSrc_itemPick.Stop();
			_aSrc_itemPick.Play();
		}
	}

	public void Init_GamepadPlaceItem()
	{
		if (!_heldItemListDataEntry)
		{
			return;
		}
		bool flag = false;
		if (_heldItemListDataEntry._entryType == ItemListEntryType.STORAGE)
		{
			for (int i = 0; i < ItemStorageManager._current._storageListEntries.Count; i++)
			{
				ItemListDataEntry itemListDataEntry = ItemStorageManager._current._storageListEntries[i];
				if (itemListDataEntry._itemTypeTab == _currentTab && !(itemListDataEntry._parentItemSlotUI != _currentHighlightedItemSlot))
				{
					_heldItemListDataEntry.Override_ItemListEntry(itemListDataEntry);
					flag = true;
					break;
				}
			}
			_heldItemListDataEntry.Init_SaveProfiles();
		}
		else
		{
			for (int j = 0; j < _listDataEntries.Count; j++)
			{
				ItemListDataEntry itemListDataEntry2 = (ItemListDataEntry)_listDataEntries[j];
				if (itemListDataEntry2._itemTypeTab == _currentTab && !(itemListDataEntry2._parentItemSlotUI != _currentHighlightedItemSlot))
				{
					_heldItemListDataEntry.Override_ItemListEntry(itemListDataEntry2);
					flag = true;
					break;
				}
			}
		}
		_heldItemListDataEntry._isHeldByPointer = false;
		if (!flag)
		{
			_heldItemListDataEntry._parentItemSlotUI = _currentHighlightedItemSlot;
		}
		_heldItemListDataEntry = null;
		_aSrc_itemPlace.Stop();
		_aSrc_itemPlace.Play();
	}

	public void Start_RaceModelDisplayRotation_Left()
	{
		_isRotatingDisplay = true;
		_rotationValue = 125.75f;
	}

	public void Start_RaceModelDisplayRotation_Right()
	{
		_isRotatingDisplay = true;
		_rotationValue = -125.75f;
	}

	public void Stop_RaceModelDisplayRotation()
	{
		_isRotatingDisplay = false;
	}

	public void Reset_RaceModelDisplayRotation()
	{
		_raceModelDisplayContainer.rotation = Quaternion.Euler(new Vector3(0f, 25f, 0f));
	}

	public void Apply_RaceModelDisplayParams()
	{
		if (!_mainPlayer)
		{
			return;
		}
		PlayerAppearance_Profile appearanceProfile = ProfileDataManager._current._characterFile._appearanceProfile;
		RaceModelEquipDisplay equipDisplay = _setRaceModel._equipDisplay;
		PlayerAppearance_Profile playerAppearance_Profile = _setRaceModel._scriptablePlayerRace.Init_ParamsCheck(appearanceProfile);
		equipDisplay._hideHelmVisual = _mainPlayer._pVisual._playerAppearanceStruct._hideHelm;
		equipDisplay._hideCapeVisual = _mainPlayer._pVisual._playerAppearanceStruct._hideCape;
		equipDisplay._hideChestVisual = _mainPlayer._pVisual._playerAppearanceStruct._hideChest;
		equipDisplay._hideLeggingsVisual = _mainPlayer._pVisual._playerAppearanceStruct._hideLeggings;
		equipDisplay._hideVanityHelm = _mainPlayer._pVisual._playerAppearanceStruct._hideVanityHelm;
		equipDisplay._hideVanityCape = _mainPlayer._pVisual._playerAppearanceStruct._hideVanityCape;
		equipDisplay._hideVanityChest = _mainPlayer._pVisual._playerAppearanceStruct._hideVanityChest;
		equipDisplay._hideVanityLeggings = _mainPlayer._pVisual._playerAppearanceStruct._hideVanityLeggings;
		_setRaceModel._isCharacterCreationDisplay = false;
		_setRaceModel._armatureTransform.localScale = new Vector3(playerAppearance_Profile._widthWeight, playerAppearance_Profile._heightWeight, 1f);
		_setRaceModel._headBoneTransform.localScale = new Vector3(playerAppearance_Profile._headWidth, 1f, 1f);
		_setRaceModel._displayBoobs = appearanceProfile._displayBoobs;
		_setRaceModel._muzzleWeight = playerAppearance_Profile._muzzleWeight;
		_setRaceModel._bellyWeight = playerAppearance_Profile._bellyWeight;
		_setRaceModel._bottomWeight = playerAppearance_Profile._bottomWeight;
		_setRaceModel._torsoWeight = playerAppearance_Profile._torsoWeight;
		_setRaceModel._boobWeight = playerAppearance_Profile._boobWeight;
		_setRaceModel._armWeight = playerAppearance_Profile._armWeight;
		_setRaceModel.SetTextureIndex = playerAppearance_Profile._setTexture;
		_setRaceModel._setSkinColorProfile._hue = playerAppearance_Profile._setSkinColorProfile._hue;
		_setRaceModel._setSkinColorProfile._brightness = playerAppearance_Profile._setSkinColorProfile._brightness;
		_setRaceModel._setSkinColorProfile._contrast = playerAppearance_Profile._setSkinColorProfile._contrast;
		_setRaceModel._setSkinColorProfile._saturation = playerAppearance_Profile._setSkinColorProfile._saturation;
		_setRaceModel._setHairColorProfile._hue = playerAppearance_Profile._hairColorProfile._hue;
		_setRaceModel._setHairColorProfile._brightness = playerAppearance_Profile._hairColorProfile._brightness;
		_setRaceModel._setHairColorProfile._contrast = playerAppearance_Profile._hairColorProfile._contrast;
		_setRaceModel._setHairColorProfile._saturation = playerAppearance_Profile._hairColorProfile._saturation;
		_setRaceModel._hairHueIsBodyHue = playerAppearance_Profile._hairIsBodyColor;
		_setRaceModel._setMiscColorProfile._hue = playerAppearance_Profile._miscColorProfile._hue;
		_setRaceModel._setMiscColorProfile._brightness = playerAppearance_Profile._miscColorProfile._brightness;
		_setRaceModel._setMiscColorProfile._contrast = playerAppearance_Profile._miscColorProfile._contrast;
		_setRaceModel._setMiscColorProfile._saturation = playerAppearance_Profile._miscColorProfile._saturation;
		equipDisplay.DyeIndex = appearanceProfile._helmDyeIndex;
		_setRaceModel.SetEarIndex = appearanceProfile._setEar;
		_setRaceModel.SetMiscIndex = appearanceProfile._setMisc;
		_setRaceModel.SetTailIndex = appearanceProfile._setTail;
		_setRaceModel.SetHairIndex = appearanceProfile._setHairStyle;
		_setRaceModel.SetEyeIndex = appearanceProfile._setEye;
		_setRaceModel.SetMouthIndex = appearanceProfile._setMouth;
		if ((bool)_mainPlayer._pCombat._equippedWeapon)
		{
			if (!_mainPlayer._pCombat._isUsingAltWeapon)
			{
				if (_mainPlayer._pEquipment._equips["Weapon"]._heldItem != null)
				{
					equipDisplay.Apply_WeaponDisplay(_mainPlayer._pCombat._equippedWeapon, _mainPlayer._pEquipment._equips["Weapon"]._heldItem._modifierID);
				}
				if (_mainPlayer._pCombat._equippedWeapon.weaponType._weaponHandedness == WeaponHandedness.Two_Handed && _setRaceModel._raceAnimator.isActiveAndEnabled)
				{
					_setRaceModel._raceAnimator.CrossFade("Hold_None", 0f, 4);
				}
			}
			else if (_mainPlayer._pEquipment._equips["Alt Weapon"]._heldItem != null)
			{
				equipDisplay.Apply_WeaponDisplay(_mainPlayer._pCombat._equippedWeapon, _mainPlayer._pEquipment._equips["Alt Weapon"]._heldItem._modifierID);
				if (_mainPlayer._pCombat._equippedWeapon.weaponType._weaponHandedness == WeaponHandedness.Two_Handed && _setRaceModel._raceAnimator.isActiveAndEnabled)
				{
					_setRaceModel._raceAnimator.CrossFade("Hold_None", 0f, 4);
				}
			}
		}
		else
		{
			if (_setRaceModel._raceAnimator.isActiveAndEnabled)
			{
				_setRaceModel._raceAnimator.CrossFade("Hold_None", 0f, 2);
				if (!_mainPlayer._pCombat._equippedShield)
				{
					_setRaceModel._raceAnimator.CrossFade("Hold_None", 0f, 4);
				}
			}
			equipDisplay.Remove_WeaponDisplay();
		}
		_setRaceModel.Apply_CharacterDisplay();
	}

	public bool AbleToTransmogEquip(ItemListDataEntry _listEntry)
	{
		ScriptableEquipment scriptableEquipment = (ScriptableEquipment)_listEntry._scriptableItem;
		if (_currentIllusionStones <= 0)
		{
			return false;
		}
		if (scriptableEquipment.GetType() == typeof(ScriptableWeapon))
		{
			return false;
		}
		if (scriptableEquipment.GetType() == typeof(ScriptableRing))
		{
			return false;
		}
		if (scriptableEquipment._equipmentLevel > Player._mainPlayer._pStats._currentLevel)
		{
			return false;
		}
		if (scriptableEquipment.IsTransmoggedItem())
		{
			return false;
		}
		return true;
	}

	public void Refresh_EquipCells()
	{
		if ((bool)_mainPlayer)
		{
			for (int i = 0; i < _equipSlots.Length; i++)
			{
				_equipSlots[i]._equipSlot_Icon.enabled = false;
				_equipSlots[i]._equipSlot_emptyIcon.enabled = true;
			}
			Handle_EquipSlotCells();
		}
		void Handle_EquipSlotCells()
		{
			for (int j = 0; j < _equipSlots.Length; j++)
			{
				if (_mainPlayer._pEquipment._equips.ContainsKey(_equipSlots[j]._equipSlot_tag))
				{
					if (_equipSlots[j]._equipSlot_tag == "Ring")
					{
						_equipSlots[j].gameObject.SetActive(_currentEquipCellTab == EquipCellTab.EQUIPMENT_PANEL);
					}
					else if (_equipSlots[j]._equipSlot_tag == "Weapon")
					{
						_equipSlots[j].gameObject.SetActive(_currentEquipCellTab == EquipCellTab.EQUIPMENT_PANEL);
					}
					else if (_equipSlots[j]._equipSlot_tag == "Alt Weapon")
					{
						_equipSlots[j].gameObject.SetActive(_currentEquipCellTab == EquipCellTab.EQUIPMENT_PANEL);
					}
					for (int k = 0; k < _listDataEntries.Count; k++)
					{
						ItemListDataEntry component = _listDataEntries[k].GetComponent<ItemListDataEntry>();
						if (component._itemData == _mainPlayer._pEquipment._equips[_equipSlots[j]._equipSlot_tag]._heldItem)
						{
							_equipSlots[j]._setEntry = component;
							break;
						}
					}
					if (_currentEquipCellTab == EquipCellTab.EQUIPMENT_PANEL)
					{
						if ((bool)_mainPlayer._pEquipment._equips[_equipSlots[j]._equipSlot_tag]._scriptableEquip)
						{
							_equipSlots[j]._equipSlot_Icon.enabled = true;
							_equipSlots[j]._equipSlot_emptyIcon.enabled = false;
							_equipSlots[j]._equipSlot_Icon.sprite = _mainPlayer._pEquipment._equips[_equipSlots[j]._equipSlot_tag]._scriptableEquip._itemIcon;
						}
					}
					else if (_currentEquipCellTab == EquipCellTab.VANITY_PANEL && (bool)_mainPlayer._pEquipment._equips[_equipSlots[j]._equipSlot_tag]._vanityEquip)
					{
						_equipSlots[j]._equipSlot_Icon.enabled = true;
						_equipSlots[j]._equipSlot_emptyIcon.enabled = false;
						_equipSlots[j]._equipSlot_Icon.sprite = _mainPlayer._pEquipment._equips[_equipSlots[j]._equipSlot_tag]._vanityEquip._itemIcon;
					}
				}
			}
		}
	}

	public void Toggle_WeaponLock()
	{
		if ((bool)Player._mainPlayer && !Player._mainPlayer._bufferingStatus && !(_weaponLockBuffer > 0f))
		{
			Player._mainPlayer._pCombat.Cmd_LockWeapon();
			_weaponLockBuffer = 0.6f;
		}
	}

	private void Display_EquipmentPanel()
	{
		_currentEquipCellTab = EquipCellTab.EQUIPMENT_PANEL;
		Refresh_EquipCells();
		Cancel_ItemPromptWindow();
	}

	private void Display_VanityPanel()
	{
		_currentEquipCellTab = EquipCellTab.VANITY_PANEL;
		Refresh_EquipCells();
		Cancel_ItemPromptWindow();
	}

	private void Display_StatsPanel()
	{
		_currentEquipCellTab = EquipCellTab.STATS_PANEL;
		Refresh_EquipCells();
		Cancel_ItemPromptWindow();
	}

	public void Create_AllItemListDataEntries()
	{
		for (int i = 0; i < _mainPlayer._pInventory._heldItems.Count; i++)
		{
			Create_ItemListData(_mainPlayer._pInventory._heldItems[i]);
		}
		Refresh_EquipCells();
		Init_InventoryTab((int)_currentTab);
	}

	public void Create_ItemListData(ItemData _itemData)
	{
		ScriptableItem scriptableItem = GameManager._current.Locate_Item(_itemData._itemName);
		if ((bool)scriptableItem)
		{
			GameObject gameObject = Object.Instantiate(_listDataObject_prefab);
			ItemListDataEntry component = gameObject.GetComponent<ItemListDataEntry>();
			component._itemData = _itemData;
			component._scriptableItem = scriptableItem;
			_listDataEntries.Add(component);
			component.Apply_ItemDataInfo();
			switch (scriptableItem._itemType)
			{
			case ItemType.GEAR:
				gameObject.transform.SetParent(_itemCell_gearContent);
				break;
			case ItemType.CONSUMABLE:
				gameObject.transform.SetParent(_itemCell_consumableContent);
				break;
			case ItemType.TRADE:
				gameObject.transform.SetParent(_itemCell_tradeContent);
				break;
			}
			gameObject.transform.localScale = Vector3.one;
			Refresh_EquipCells();
			Init_InventoryTab((int)_currentTab);
		}
	}

	public void Destroy_ItemCells_SingleListData(ItemData _itemData)
	{
		for (int i = 0; i < _listDataEntries.Count; i++)
		{
			if (_listDataEntries[i].GetComponent<ItemListDataEntry>()._itemData == _itemData)
			{
				Object.Destroy(_listDataEntries[i].gameObject);
				_listDataEntries.RemoveAt(i);
			}
		}
		Refresh_EquipCells();
		Init_InventoryTab((int)_currentTab);
	}

	public void Disable_ItemTabToolTip()
	{
		ToolTipManager._current._genericToolTip.Disable_ToolTip();
	}

	public void Init_InventoryTooltip(int _tabValue)
	{
		ToolTipManager._current._genericToolTip.Set_TooltipAnchorPos(new Vector2(-100f, 0f));
		if (_tabValue == 3)
		{
			ToolTipManager._current.Apply_GenericToolTip("Sort Items");
			ToolTipManager._current._genericToolTip.Enable_ToolTip();
			return;
		}
		switch ((ItemType)(byte)_tabValue)
		{
		case ItemType.GEAR:
			ToolTipManager._current.Apply_GenericToolTip("Equipment");
			break;
		case ItemType.CONSUMABLE:
			ToolTipManager._current.Apply_GenericToolTip("Consumables");
			break;
		case ItemType.TRADE:
			ToolTipManager._current.Apply_GenericToolTip("Trade Items");
			break;
		}
		ToolTipManager._current._genericToolTip.Enable_ToolTip();
	}

	public void Init_ItemTabToolTip(int _tabValue)
	{
		switch ((EquipCellTab)(byte)_tabValue)
		{
		case EquipCellTab.EQUIPMENT_PANEL:
			ToolTipManager._current.Apply_GenericToolTip("Equipment");
			break;
		case EquipCellTab.VANITY_PANEL:
			ToolTipManager._current.Apply_GenericToolTip("Vanity");
			break;
		case EquipCellTab.STATS_PANEL:
			ToolTipManager._current.Apply_GenericToolTip("Stats");
			break;
		}
		ToolTipManager._current._genericToolTip.Set_TooltipAnchorPos(new Vector2(0f, 50f));
		ToolTipManager._current._genericToolTip.Enable_ToolTip();
	}

	public void Init_StringGenericToolTip(string _string)
	{
		ToolTipManager._current.Apply_GenericToolTip(_string ?? "");
		ToolTipManager._current._genericToolTip.Set_TooltipAnchorPos(new Vector2(-80f, 0f));
		ToolTipManager._current._genericToolTip.Enable_ToolTip();
	}

	public void Reset_ItemCellParams()
	{
		_currentEquipCellTab = EquipCellTab.EQUIPMENT_PANEL;
		Reset_RaceModelDisplayRotation();
		Cancel_ItemPromptWindow();
		Cancel_QuantityPrompt();
		Init_InventoryTab(0);
		Refresh_EquipCells();
		Init_GamepadPlaceItem();
		_gamepadItemMenuFocus = GamepadItemMenuFocus.ITEMS;
		_gamepad_selectedItemSlot = 0;
		_gamepad_selectedItemTab = 0;
		_gamepad_selectedEquipTab = 0;
		_gamepad_selectedEquipSlot = 0;
	}
}
