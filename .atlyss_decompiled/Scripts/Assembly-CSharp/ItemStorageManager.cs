using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemStorageManager : MonoBehaviour
{
	public static ItemStorageManager _current;

	[Header("Storage UI Management")]
	[SerializeField]
	private MenuElement _storageWindowElement;

	public GameObject _storageEntryPrefab;

	public float _commandBuffer;

	public bool _isOpen;

	[Space]
	public ItemListDataEntry _highlightedListEntry;

	public List<ItemListDataEntry> _storageListEntries = new List<ItemListDataEntry>();

	[SerializeField]
	private CanvasGroup _gearTabGroup;

	[SerializeField]
	private CanvasGroup _consumableTabGroup;

	[SerializeField]
	private CanvasGroup _tradeItemGroup;

	public Transform _gearTabContainer;

	public Transform _consumableTabContainer;

	public Transform _tradeItemTabContainer;

	[Space]
	[SerializeField]
	private ItemType _currentItemTab;

	[SerializeField]
	private Button _gearTabButton;

	[SerializeField]
	private Button _consumableTabButton;

	[SerializeField]
	private Button _tradeItemTabButton;

	[SerializeField]
	private Button _sortItemButton;

	[Space]
	[SerializeField]
	private Text _itemTabHeaderText;

	[SerializeField]
	private Text _counter_gearItemSize;

	[SerializeField]
	private Text _counter_consumableItemSize;

	[SerializeField]
	private Text _counter_tradeItemSize;

	[Header("Storage Item Slots")]
	[SerializeField]
	private Transform _slotContainer;

	public ItemSlotUI _currentHighlightedItemSlot;

	public ItemSlotUI[] _storageItemSlots;

	[Space]
	[SerializeField]
	private RectTransform _storageTabHighlight;

	[SerializeField]
	private Button _storageTabButton_00;

	[SerializeField]
	private Button _storageTabButton_01;

	[SerializeField]
	private Button _storageTabButton_02;

	[SerializeField]
	private Button _storageTabButton_03;

	[SerializeField]
	private Button _storageTabButton_04;

	[SerializeField]
	private Button _storageTabButton_05;

	[SerializeField]
	private Button _storageTabButton_06;

	[SerializeField]
	private Button _storageTabButton_07;

	public int _selectedStorageTab;

	[Space]
	public int[] _storageSizes = new int[3];

	public int[] _storageSizes_01 = new int[3];

	public int[] _storageSizes_02 = new int[3];

	public int[] _storageSizes_03 = new int[3];

	public int[] _storageSizes_04 = new int[3];

	public int[] _storageSizes_05 = new int[3];

	public int[] _storageSizes_06 = new int[3];

	public int[] _storageSizes_07 = new int[3];

	[Space]
	public List<ItemData> _itemDatas = new List<ItemData>();

	public List<ItemData> _itemDatas_01 = new List<ItemData>();

	public List<ItemData> _itemDatas_02 = new List<ItemData>();

	public List<ItemData> _itemDatas_03 = new List<ItemData>();

	public List<ItemData> _itemDatas_04 = new List<ItemData>();

	public List<ItemData> _itemDatas_05 = new List<ItemData>();

	public List<ItemData> _itemDatas_06 = new List<ItemData>();

	public List<ItemData> _itemDatas_07 = new List<ItemData>();

	[Header("Prompt Window")]
	public MenuElement _promptWindowElement;

	[SerializeField]
	private RectTransform _tabButtonHighlight;

	[SerializeField]
	private Button _takeItemButton;

	[SerializeField]
	private Button _storeItemButton;

	[SerializeField]
	private Button _cancelButton;

	[Header("Gamepad")]
	public RectTransform _gamepad_selectionCursor;

	[SerializeField]
	private RectTransform _gamepad_itemTabCursor;

	[SerializeField]
	private RectTransform _gamepad_storageTabCursor;

	[Space]
	[SerializeField]
	private Image _gamepad_leftTriggerIco;

	[SerializeField]
	private Image _gamepad_rightTriggerIco;

	[SerializeField]
	private Image _gamepad_sortStorageIco;

	[Space]
	[SerializeField]
	private MenuElement _gamepad_contextPrompt_southButton;

	[SerializeField]
	private Text _gamepad_contextPrompt_southButtonText;

	[Space]
	[SerializeField]
	private bool _gamepad_selectingItemTabs;

	[SerializeField]
	private bool _gamepad_selectingStorageTabs;

	private readonly List<RectTransform> _storageSlotTransforms = new List<RectTransform>();

	private int _gamepad_currentStorageSlot;

	private int _gamepad_currentItemTab;

	private int _gamepad_currentStorageTab;

	[Header("Audio")]
	[SerializeField]
	private AudioSource _aSrc_switchStorageTab;

	public int Gamepad_CurrentStorageSlot
	{
		get
		{
			return _gamepad_currentStorageSlot;
		}
		set
		{
			if (value == 8 && _gamepad_currentStorageSlot == 7)
			{
				value = 7;
				_gamepad_selectingItemTabs = true;
			}
			else if (value == 16 && _gamepad_currentStorageSlot == 15)
			{
				value = 15;
				_gamepad_selectingItemTabs = true;
			}
			else if (value == 24 && _gamepad_currentStorageSlot == 23)
			{
				value = 23;
				_gamepad_selectingItemTabs = true;
			}
			else if (value == 32 && _gamepad_currentStorageSlot == 31)
			{
				value = 31;
				_gamepad_selectingItemTabs = true;
			}
			else if (value == 32 && _gamepad_currentStorageSlot == 31)
			{
				value = 31;
				_gamepad_selectingItemTabs = true;
			}
			else if (value == 40 && _gamepad_currentStorageSlot == 39)
			{
				value = 39;
				_gamepad_selectingItemTabs = true;
			}
			else if (value == 48 && _gamepad_currentStorageSlot == 47)
			{
				value = 47;
				_gamepad_selectingItemTabs = true;
			}
			else if (value < 0 && _gamepad_currentStorageSlot == 0)
			{
				value = 7;
				_gamepad_selectingItemTabs = true;
			}
			else if (value == 7 && _gamepad_currentStorageSlot == 8)
			{
				value = 15;
				_gamepad_selectingItemTabs = true;
			}
			else if (value == 15 && _gamepad_currentStorageSlot == 16)
			{
				value = 23;
				_gamepad_selectingItemTabs = true;
			}
			else if (value == 23 && _gamepad_currentStorageSlot == 24)
			{
				value = 31;
				_gamepad_selectingItemTabs = true;
			}
			else if (value == 31 && _gamepad_currentStorageSlot == 32)
			{
				value = 39;
				_gamepad_selectingItemTabs = true;
			}
			else if (value == 39 && _gamepad_currentStorageSlot == 40)
			{
				value = 47;
				_gamepad_selectingItemTabs = true;
			}
			if (value < 0)
			{
				value = 0;
			}
			if (value > 47)
			{
				value = 47;
			}
			if (_gamepad_currentStorageSlot != value)
			{
				InputControlManager.current._gamepadMouseCursorBuffer = 0f;
				ToolTipManager._current.Disable_AllTooltips();
				_highlightedListEntry = null;
				if (IsInvoking("Init_GamepadToolTip"))
				{
					CancelInvoke("Init_GamepadToolTip");
				}
				Invoke("Init_GamepadToolTip", 0.3f);
			}
			_gamepad_currentStorageSlot = value;
		}
	}

	public int Gamepad_CurrentItemTab
	{
		get
		{
			return _gamepad_currentItemTab;
		}
		set
		{
			if (value < 0)
			{
				value = 0;
			}
			if (value > 2)
			{
				value = 2;
			}
			switch (value)
			{
			case 0:
				SetTab_Gear();
				TabMenu._current._itemCell.Gamepad_SelectedItemTab = 0;
				break;
			case 1:
				SetTab_Consumable();
				TabMenu._current._itemCell.Gamepad_SelectedItemTab = 1;
				break;
			case 2:
				SetTab_Trade();
				TabMenu._current._itemCell.Gamepad_SelectedItemTab = 2;
				break;
			}
			_gamepad_currentItemTab = value;
		}
	}

	public int Gamepad_CurrentStorageTab
	{
		get
		{
			return _gamepad_currentStorageTab;
		}
		set
		{
			if (value < 0)
			{
				value = 0;
			}
			if (value > 7)
			{
				value = 7;
			}
			if (_gamepad_currentStorageTab != value)
			{
				switch (value)
				{
				case 0:
					SetStorageTab_00();
					break;
				case 1:
					SetStorageTab_01();
					break;
				case 2:
					SetStorageTab_02();
					break;
				case 3:
					SetStorageTab_03();
					break;
				case 4:
					SetStorageTab_04();
					break;
				case 5:
					SetStorageTab_05();
					break;
				case 6:
					SetStorageTab_06();
					break;
				case 7:
					SetStorageTab_07();
					break;
				}
				Clear_StorageEntries();
				Init_StorageListing();
			}
			_gamepad_currentStorageTab = value;
		}
	}

	private void Awake()
	{
		_current = this;
		_storageItemSlots = _slotContainer.GetComponentsInChildren<ItemSlotUI>(includeInactive: true);
		_storageSlotTransforms.AddRange(_slotContainer.GetComponentsInChildren<RectTransform>(includeInactive: true));
		_storageSlotTransforms.RemoveAt(0);
	}

	private void Start()
	{
		Init_AssignButtons();
	}

	private void Init_AssignButtons()
	{
		_gearTabButton.onClick.AddListener(SetTab_Gear);
		_consumableTabButton.onClick.AddListener(SetTab_Consumable);
		_tradeItemTabButton.onClick.AddListener(SetTab_Trade);
		_sortItemButton.onClick.AddListener(SetTab_Sort);
		_storageTabButton_00.onClick.AddListener(SetStorageTab_00);
		_storageTabButton_00.onClick.AddListener(Clear_StorageEntries);
		_storageTabButton_00.onClick.AddListener(Init_StorageListing);
		_storageTabButton_01.onClick.AddListener(SetStorageTab_01);
		_storageTabButton_01.onClick.AddListener(Clear_StorageEntries);
		_storageTabButton_01.onClick.AddListener(Init_StorageListing);
		_storageTabButton_02.onClick.AddListener(SetStorageTab_02);
		_storageTabButton_02.onClick.AddListener(Clear_StorageEntries);
		_storageTabButton_02.onClick.AddListener(Init_StorageListing);
		_storageTabButton_03.onClick.AddListener(SetStorageTab_03);
		_storageTabButton_03.onClick.AddListener(Clear_StorageEntries);
		_storageTabButton_03.onClick.AddListener(Init_StorageListing);
		_storageTabButton_04.onClick.AddListener(SetStorageTab_04);
		_storageTabButton_04.onClick.AddListener(Clear_StorageEntries);
		_storageTabButton_04.onClick.AddListener(Init_StorageListing);
		_storageTabButton_05.onClick.AddListener(SetStorageTab_05);
		_storageTabButton_05.onClick.AddListener(Clear_StorageEntries);
		_storageTabButton_05.onClick.AddListener(Init_StorageListing);
		_storageTabButton_06.onClick.AddListener(SetStorageTab_06);
		_storageTabButton_06.onClick.AddListener(Clear_StorageEntries);
		_storageTabButton_06.onClick.AddListener(Init_StorageListing);
		_storageTabButton_07.onClick.AddListener(SetStorageTab_07);
		_storageTabButton_07.onClick.AddListener(Clear_StorageEntries);
		_storageTabButton_07.onClick.AddListener(Init_StorageListing);
	}

	private void Update()
	{
		_isOpen = DialogManager._current._currentDialogUIPrompt == DialogUIPrompt.STORAGE;
		_storageWindowElement.gameObject.SetActive(_isOpen);
		_storageWindowElement.isEnabled = _isOpen;
		Handle_GamepadControl();
		if (_isOpen)
		{
			TabMenu._current._itemTradeMode = true;
			switch (_selectedStorageTab)
			{
			case 0:
				_counter_gearItemSize.text = $"{_storageSizes[0]}/48";
				_counter_consumableItemSize.text = $"{_storageSizes[1]}/48";
				_counter_tradeItemSize.text = $"{_storageSizes[2]}/48";
				break;
			case 1:
				_counter_gearItemSize.text = $"{_storageSizes_01[0]}/48";
				_counter_consumableItemSize.text = $"{_storageSizes_01[1]}/48";
				_counter_tradeItemSize.text = $"{_storageSizes_01[2]}/48";
				break;
			case 2:
				_counter_gearItemSize.text = $"{_storageSizes_02[0]}/48";
				_counter_consumableItemSize.text = $"{_storageSizes_02[1]}/48";
				_counter_tradeItemSize.text = $"{_storageSizes_02[2]}/48";
				break;
			case 3:
				_counter_gearItemSize.text = $"{_storageSizes_03[0]}/48";
				_counter_consumableItemSize.text = $"{_storageSizes_03[1]}/48";
				_counter_tradeItemSize.text = $"{_storageSizes_03[2]}/48";
				break;
			case 4:
				_counter_gearItemSize.text = $"{_storageSizes_04[0]}/48";
				_counter_consumableItemSize.text = $"{_storageSizes_04[1]}/48";
				_counter_tradeItemSize.text = $"{_storageSizes_04[2]}/48";
				break;
			case 5:
				_counter_gearItemSize.text = $"{_storageSizes_05[0]}/48";
				_counter_consumableItemSize.text = $"{_storageSizes_05[1]}/48";
				_counter_tradeItemSize.text = $"{_storageSizes_05[2]}/48";
				break;
			case 6:
				_counter_gearItemSize.text = $"{_storageSizes_06[0]}/48";
				_counter_consumableItemSize.text = $"{_storageSizes_06[1]}/48";
				_counter_tradeItemSize.text = $"{_storageSizes_06[2]}/48";
				break;
			case 7:
				_counter_gearItemSize.text = $"{_storageSizes_07[0]}/48";
				_counter_consumableItemSize.text = $"{_storageSizes_07[1]}/48";
				_counter_tradeItemSize.text = $"{_storageSizes_07[2]}/48";
				break;
			}
			_storageTabButton_00.interactable = _selectedStorageTab != 0;
			_storageTabButton_01.interactable = _selectedStorageTab != 1;
			_storageTabButton_02.interactable = _selectedStorageTab != 2;
			_storageTabButton_03.interactable = _selectedStorageTab != 3;
			_storageTabButton_04.interactable = _selectedStorageTab != 4;
			_storageTabButton_05.interactable = _selectedStorageTab != 5;
			_storageTabButton_06.interactable = _selectedStorageTab != 6;
			_storageTabButton_07.interactable = _selectedStorageTab != 7;
			Handle_TabVisibility();
		}
		else
		{
			Clear_StorageEntries();
			_currentHighlightedItemSlot = null;
			_commandBuffer = 0.1f;
		}
	}

	private void Init_GamepadToolTip()
	{
		if (!Cursor.visible && (bool)_highlightedListEntry && (bool)_highlightedListEntry._scriptableItem)
		{
			_highlightedListEntry.Init_ItemEntryTooltip();
		}
	}

	private void Handle_GamepadControl()
	{
		bool flag = InputControlManager.current._setGamepadType != GamepadType.Keyboard;
		bool flag2 = GamepadInput.GetButtonDown(GamepadButton.LeftStick_Up) || GamepadInput.GetButtonDown(GamepadButton.RightStick_Up) || InputControlManager.current.DpadUp(DpadHoldType.OnDown);
		bool flag3 = GamepadInput.GetButtonDown(GamepadButton.LeftStick_Down) || GamepadInput.GetButtonDown(GamepadButton.RightStick_Down) || InputControlManager.current.DpadDown(DpadHoldType.OnDown);
		bool flag4 = GamepadInput.GetButtonDown(GamepadButton.LeftStick_Left) || GamepadInput.GetButtonDown(GamepadButton.RightStick_Left) || InputControlManager.current.DpadLeft(DpadHoldType.OnDown);
		bool flag5 = GamepadInput.GetButtonDown(GamepadButton.LeftStick_Right) || GamepadInput.GetButtonDown(GamepadButton.RightStick_Right) || InputControlManager.current.DpadRight(DpadHoldType.OnDown);
		_gamepad_selectionCursor.gameObject.SetActive(flag && !_gamepad_selectingItemTabs && !_gamepad_selectingStorageTabs && DialogManager._current._currentGamepadTab == DialogManager_GamepadTab.Prompt_Window);
		_gamepad_itemTabCursor.gameObject.SetActive(flag && _gamepad_selectingItemTabs && DialogManager._current._currentGamepadTab == DialogManager_GamepadTab.Prompt_Window);
		_gamepad_storageTabCursor.gameObject.SetActive(flag && _gamepad_selectingStorageTabs && DialogManager._current._currentGamepadTab == DialogManager_GamepadTab.Prompt_Window);
		_gamepad_selectionCursor.anchoredPosition = _storageSlotTransforms[_gamepad_currentStorageSlot].anchoredPosition;
		_gamepad_itemTabCursor.anchoredPosition = _tabButtonHighlight.anchoredPosition;
		_gamepad_storageTabCursor.anchoredPosition = _storageTabHighlight.anchoredPosition;
		_gamepad_leftTriggerIco.enabled = flag && DialogManager._current._currentGamepadTab == DialogManager_GamepadTab.Prompt_Window;
		_gamepad_rightTriggerIco.enabled = flag && DialogManager._current._currentGamepadTab == DialogManager_GamepadTab.Prompt_Window;
		_gamepad_sortStorageIco.enabled = flag && DialogManager._current._currentGamepadTab == DialogManager_GamepadTab.Prompt_Window;
		_gamepad_contextPrompt_southButton.isEnabled = flag && !_gamepad_selectingItemTabs && !_gamepad_selectingStorageTabs && (bool)_highlightedListEntry && DialogManager._current._currentGamepadTab == DialogManager_GamepadTab.Prompt_Window;
		if (GamepadInput.GetButton(GamepadButton.LeftTrigger))
		{
			_gamepad_leftTriggerIco.color = Color.white;
		}
		else
		{
			_gamepad_leftTriggerIco.color = Color.grey;
		}
		if (GamepadInput.GetButton(GamepadButton.RightTrigger))
		{
			_gamepad_rightTriggerIco.color = Color.white;
		}
		else
		{
			_gamepad_rightTriggerIco.color = Color.grey;
		}
		if (GamepadInput.GetButton(GamepadButton.RightStick))
		{
			_gamepad_sortStorageIco.color = Color.white;
		}
		else
		{
			_gamepad_sortStorageIco.color = Color.grey;
		}
		if (!_isOpen)
		{
			_gamepad_currentStorageSlot = 0;
			_gamepad_currentStorageTab = 0;
			_highlightedListEntry = null;
			_gamepad_selectingStorageTabs = false;
			_gamepad_selectingItemTabs = false;
		}
		else
		{
			if (DialogManager._current._currentDialogUIPrompt != DialogUIPrompt.STORAGE || !flag)
			{
				return;
			}
			if (_gamepad_selectingItemTabs && InputControlManager.current._gamepadMouseCursorBuffer <= 0f)
			{
				ToolTipManager._current.Disable_AllTooltips();
			}
			if (DialogManager._current._currentGamepadTab != DialogManager_GamepadTab.Prompt_Window)
			{
				_gamepad_currentStorageSlot = 0;
				_gamepad_selectingStorageTabs = false;
				_gamepad_selectingItemTabs = false;
				return;
			}
			if (!_gamepad_selectingItemTabs)
			{
				if (GamepadInput.GetButtonDown(GamepadButton.RightTrigger))
				{
					TabMenu._current._itemCell.Init_GamepadGrabItem();
				}
				if (GamepadInput.GetButtonUp(GamepadButton.RightTrigger))
				{
					TabMenu._current._itemCell.Init_GamepadPlaceItem();
				}
				if (GamepadInput.GetButtonDown(GamepadButton.Button_South) && DialogManager._current._currentGamepadTab == DialogManager_GamepadTab.Prompt_Window && (bool)_highlightedListEntry)
				{
					_highlightedListEntry.Init_TakeItemFromStorage();
					TabMenu._current._gamepadSelectAsrc.Play();
				}
			}
			if (GamepadInput.GetButtonDown(GamepadButton.RightStick))
			{
				SetTab_Sort();
			}
			if (flag4)
			{
				if (_gamepad_selectingItemTabs)
				{
					_gamepad_selectingItemTabs = false;
				}
				else if (_gamepad_selectingStorageTabs)
				{
					Gamepad_CurrentStorageTab--;
				}
				else
				{
					Gamepad_CurrentStorageSlot--;
				}
				TabMenu._current._gamepadSelectAsrc.Play();
			}
			if (flag5)
			{
				if (_gamepad_selectingItemTabs)
				{
					_gamepad_selectingItemTabs = false;
					Gamepad_CurrentStorageSlot = 0;
				}
				else if (_gamepad_selectingStorageTabs)
				{
					Gamepad_CurrentStorageTab++;
				}
				else
				{
					Gamepad_CurrentStorageSlot++;
				}
				TabMenu._current._gamepadSelectAsrc.Play();
			}
			if (flag3)
			{
				if (!_gamepad_selectingStorageTabs)
				{
					if (_gamepad_selectingItemTabs)
					{
						Gamepad_CurrentItemTab++;
					}
					else if (_gamepad_currentStorageSlot >= 40 && _gamepad_currentStorageSlot <= 47)
					{
						_gamepad_selectingStorageTabs = true;
					}
					else if (_gamepad_currentStorageSlot < 40)
					{
						Gamepad_CurrentStorageSlot += 8;
					}
				}
				TabMenu._current._gamepadSelectAsrc.Play();
			}
			if (flag2)
			{
				if (_gamepad_selectingStorageTabs)
				{
					_gamepad_selectingStorageTabs = false;
				}
				else if (_gamepad_selectingItemTabs)
				{
					Gamepad_CurrentItemTab--;
				}
				else if (_gamepad_currentStorageSlot >= 0 && _gamepad_currentStorageSlot <= 7)
				{
					_gamepad_selectingStorageTabs = true;
					Gamepad_CurrentStorageSlot = _gamepad_currentStorageSlot + 40;
				}
				else
				{
					Gamepad_CurrentStorageSlot -= 8;
				}
				TabMenu._current._gamepadSelectAsrc.Play();
			}
			if (!(InputControlManager.current._gamepadMouseCursorBuffer <= 0f))
			{
				return;
			}
			_highlightedListEntry = null;
			TabMenu._current._itemCell._currentHighlightedDataEntry = null;
			for (int i = 0; i < _storageListEntries.Count; i++)
			{
				switch (_currentItemTab)
				{
				case ItemType.GEAR:
					if (_storageListEntries[i]._scriptableItem._itemType != ItemType.GEAR)
					{
						continue;
					}
					goto default;
				case ItemType.CONSUMABLE:
					if (_storageListEntries[i]._scriptableItem._itemType != ItemType.CONSUMABLE)
					{
						continue;
					}
					goto default;
				case ItemType.TRADE:
					if (_storageListEntries[i]._scriptableItem._itemType != ItemType.TRADE)
					{
						continue;
					}
					goto default;
				default:
					if (_gamepad_currentStorageSlot != _storageListEntries[i]._itemData._slotNumber)
					{
						continue;
					}
					break;
				}
				_highlightedListEntry = _storageListEntries[i];
				TabMenu._current._itemCell._currentHighlightedDataEntry = _highlightedListEntry;
				break;
			}
			TabMenu._current._itemCell._currentHighlightedItemSlot = _storageItemSlots[_gamepad_currentStorageSlot];
		}
	}

	private void SetTab_Gear()
	{
		TabMenu._current._itemCell.Init_InventoryTab(0);
	}

	private void SetTab_Consumable()
	{
		TabMenu._current._itemCell.Init_InventoryTab(1);
	}

	private void SetTab_Trade()
	{
		TabMenu._current._itemCell.Init_InventoryTab(2);
	}

	private void SetTab_Sort()
	{
		TabMenu._current._itemCell.Sort_Items(_isStorageSort: true);
	}

	private void SetStorageTab_00()
	{
		_selectedStorageTab = 0;
		_storageTabHighlight.transform.position = _storageTabButton_00.transform.GetChild(0).position;
		_aSrc_switchStorageTab.Play();
	}

	private void SetStorageTab_01()
	{
		_selectedStorageTab = 1;
		_storageTabHighlight.transform.position = _storageTabButton_01.transform.GetChild(0).position;
		_aSrc_switchStorageTab.Play();
	}

	private void SetStorageTab_02()
	{
		_selectedStorageTab = 2;
		_storageTabHighlight.transform.position = _storageTabButton_02.transform.GetChild(0).position;
		_aSrc_switchStorageTab.Play();
	}

	private void SetStorageTab_03()
	{
		_selectedStorageTab = 3;
		_storageTabHighlight.transform.position = _storageTabButton_03.transform.GetChild(0).position;
		_aSrc_switchStorageTab.Play();
	}

	private void SetStorageTab_04()
	{
		_selectedStorageTab = 4;
		_storageTabHighlight.transform.position = _storageTabButton_04.transform.GetChild(0).position;
		_aSrc_switchStorageTab.Play();
	}

	private void SetStorageTab_05()
	{
		_selectedStorageTab = 5;
		_storageTabHighlight.transform.position = _storageTabButton_05.transform.GetChild(0).position;
		_aSrc_switchStorageTab.Play();
	}

	private void SetStorageTab_06()
	{
		_selectedStorageTab = 6;
		_storageTabHighlight.transform.position = _storageTabButton_06.transform.GetChild(0).position;
		_aSrc_switchStorageTab.Play();
	}

	private void SetStorageTab_07()
	{
		_selectedStorageTab = 7;
		_storageTabHighlight.transform.position = _storageTabButton_07.transform.GetChild(0).position;
		_aSrc_switchStorageTab.Play();
	}

	private void Handle_TabVisibility()
	{
		_currentItemTab = TabMenu._current._itemCell._currentTab;
		_gearTabGroup.interactable = _currentItemTab == ItemType.GEAR;
		_gearTabGroup.blocksRaycasts = _currentItemTab == ItemType.GEAR;
		_consumableTabGroup.interactable = _currentItemTab == ItemType.CONSUMABLE;
		_consumableTabGroup.blocksRaycasts = _currentItemTab == ItemType.CONSUMABLE;
		_tradeItemGroup.interactable = _currentItemTab == ItemType.TRADE;
		_tradeItemGroup.blocksRaycasts = _currentItemTab == ItemType.TRADE;
		float alpha = 1f;
		if (_commandBuffer > 0f)
		{
			_commandBuffer -= Time.deltaTime;
			alpha = 0.65f;
		}
		switch (_currentItemTab)
		{
		case ItemType.GEAR:
			_itemTabHeaderText.text = $"Equipment (Tab {_selectedStorageTab + 1})";
			_tabButtonHighlight.position = _gearTabButton.GetComponent<RectTransform>().position;
			_gearTabGroup.alpha = alpha;
			_consumableTabGroup.alpha = 0f;
			_tradeItemGroup.alpha = 0f;
			break;
		case ItemType.CONSUMABLE:
			_itemTabHeaderText.text = $"Consumables (Tab {_selectedStorageTab + 1})";
			_tabButtonHighlight.position = _consumableTabButton.GetComponent<RectTransform>().position;
			_gearTabGroup.alpha = 0f;
			_consumableTabGroup.alpha = alpha;
			_tradeItemGroup.alpha = 0f;
			break;
		case ItemType.TRADE:
			_itemTabHeaderText.text = $"Trade Items (Tab {_selectedStorageTab + 1})";
			_tabButtonHighlight.position = _tradeItemTabButton.GetComponent<RectTransform>().position;
			_gearTabGroup.alpha = 0f;
			_consumableTabGroup.alpha = 0f;
			_tradeItemGroup.alpha = alpha;
			break;
		}
	}

	public void Init_StorageListing()
	{
		ProfileDataManager._current.Load_ItemStorageData();
		if (IsInvoking("Begin_StorageListing"))
		{
			CancelInvoke("Begin_StorageListing");
		}
		Invoke("Begin_StorageListing", 0.12f);
	}

	private void Begin_StorageListing()
	{
		ItemData[] array = null;
		switch (_selectedStorageTab)
		{
		case 0:
			array = _itemDatas.ToArray();
			break;
		case 1:
			array = _itemDatas_01.ToArray();
			break;
		case 2:
			array = _itemDatas_02.ToArray();
			break;
		case 3:
			array = _itemDatas_03.ToArray();
			break;
		case 4:
			array = _itemDatas_04.ToArray();
			break;
		case 5:
			array = _itemDatas_05.ToArray();
			break;
		case 6:
			array = _itemDatas_06.ToArray();
			break;
		case 7:
			array = _itemDatas_07.ToArray();
			break;
		}
		for (int i = 0; i < array.Length; i++)
		{
			ItemData itemData = array[i];
			ScriptableItem scriptableItem = GameManager._current.Locate_Item(itemData._itemName);
			if ((bool)scriptableItem && itemData._quantity > 0 && !scriptableItem._scriptableQuest)
			{
				Create_StorageEntry(itemData, scriptableItem, i, itemData._slotNumber);
			}
		}
	}

	public void Create_StorageEntry(ItemData _itemData, ScriptableItem _scriptItem, int _index, int _slotNumber)
	{
		if (!GameManager._current.Locate_Item(_itemData._itemName))
		{
			return;
		}
		if (_itemData._modifierID > 0 && !GameManager._current.Locate_StatModifier(_itemData._modifierID))
		{
			_itemData._modifierID = 0;
		}
		switch (_selectedStorageTab)
		{
		case 0:
			if (_storageSizes[(uint)_scriptItem._itemType] >= 48)
			{
				return;
			}
			_storageSizes[(uint)_scriptItem._itemType]++;
			break;
		case 1:
			if (_storageSizes_01[(uint)_scriptItem._itemType] >= 48)
			{
				return;
			}
			_storageSizes_01[(uint)_scriptItem._itemType]++;
			break;
		case 2:
			if (_storageSizes_02[(uint)_scriptItem._itemType] >= 48)
			{
				return;
			}
			_storageSizes_02[(uint)_scriptItem._itemType]++;
			break;
		case 3:
			if (_storageSizes_03[(uint)_scriptItem._itemType] >= 48)
			{
				return;
			}
			_storageSizes_03[(uint)_scriptItem._itemType]++;
			break;
		case 4:
			if (_storageSizes_04[(uint)_scriptItem._itemType] >= 48)
			{
				return;
			}
			_storageSizes_04[(uint)_scriptItem._itemType]++;
			break;
		case 5:
			if (_storageSizes_05[(uint)_scriptItem._itemType] >= 48)
			{
				return;
			}
			_storageSizes_05[(uint)_scriptItem._itemType]++;
			break;
		case 6:
			if (_storageSizes_06[(uint)_scriptItem._itemType] >= 48)
			{
				return;
			}
			_storageSizes_06[(uint)_scriptItem._itemType]++;
			break;
		case 7:
			if (_storageSizes_07[(uint)_scriptItem._itemType] >= 48)
			{
				return;
			}
			_storageSizes_07[(uint)_scriptItem._itemType]++;
			break;
		}
		GameObject gameObject = Object.Instantiate(_storageEntryPrefab);
		ItemListDataEntry component = gameObject.GetComponent<ItemListDataEntry>();
		switch (_scriptItem._itemType)
		{
		case ItemType.GEAR:
			gameObject.transform.SetParent(_gearTabContainer);
			break;
		case ItemType.CONSUMABLE:
			gameObject.transform.SetParent(_consumableTabContainer);
			break;
		case ItemType.TRADE:
			gameObject.transform.SetParent(_tradeItemTabContainer);
			break;
		}
		component._dataID = _index;
		component._itemData = _itemData;
		component._scriptableItem = _scriptItem;
		component._entryType = ItemListEntryType.STORAGE;
		component.transform.localScale = Vector3.one;
		component._itemData._slotNumber = _slotNumber;
		component._parentItemSlotUI = _storageItemSlots[_slotNumber];
		component.Apply_ItemDataInfo();
		_storageListEntries.Add(component);
	}

	public void Clear_StorageEntries()
	{
		for (int i = 0; i < _storageListEntries.Count; i++)
		{
			Object.Destroy(_storageListEntries[i].gameObject);
		}
		_storageListEntries.Clear();
	}

	public void Delete_StorageEntry(ItemData _itemData)
	{
		ScriptableItem scriptableItem = GameManager._current.Locate_Item(_itemData._itemName);
		if (!scriptableItem)
		{
			return;
		}
		switch (_selectedStorageTab)
		{
		case 0:
			_itemDatas.Remove(_itemData);
			_storageSizes[(uint)scriptableItem._itemType]--;
			break;
		case 1:
			_itemDatas_01.Remove(_itemData);
			_storageSizes_01[(uint)scriptableItem._itemType]--;
			break;
		case 2:
			_itemDatas_02.Remove(_itemData);
			_storageSizes_02[(uint)scriptableItem._itemType]--;
			break;
		case 3:
			_itemDatas_03.Remove(_itemData);
			_storageSizes_03[(uint)scriptableItem._itemType]--;
			break;
		case 4:
			_itemDatas_04.Remove(_itemData);
			_storageSizes_04[(uint)scriptableItem._itemType]--;
			break;
		case 5:
			_itemDatas_05.Remove(_itemData);
			_storageSizes_05[(uint)scriptableItem._itemType]--;
			break;
		case 6:
			_itemDatas_06.Remove(_itemData);
			_storageSizes_06[(uint)scriptableItem._itemType]--;
			break;
		case 7:
			_itemDatas_07.Remove(_itemData);
			_storageSizes_07[(uint)scriptableItem._itemType]--;
			break;
		}
		for (int i = 0; i < _storageListEntries.Count; i++)
		{
			if (_storageListEntries[i]._itemData == _itemData)
			{
				Object.Destroy(_storageListEntries[i].gameObject);
				_storageListEntries.RemoveAt(i);
				break;
			}
		}
		ProfileDataManager._current.Save_ItemStorageData();
	}

	public void Init_InventoryTooltip(int _tabValue)
	{
		ToolTipManager._current._genericToolTip.Set_TooltipAnchorPos(new Vector2(100f, 0f));
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

	public void Disable_ItemTabToolTip()
	{
		ToolTipManager._current._genericToolTip.Disable_ToolTip();
	}
}
