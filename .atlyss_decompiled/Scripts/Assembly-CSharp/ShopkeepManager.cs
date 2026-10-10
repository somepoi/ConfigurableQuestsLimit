using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopkeepManager : MonoBehaviour
{
	public static ShopkeepManager _current;

	[Header("Runtime Elements")]
	public ScriptableShopkeep _scriptShopkeep;

	public NetNPC _shopKeepNPC;

	[Space]
	public ShopListDataEntry _selectedPurchaseListEntry;

	public ShopListDataEntry _highlightedListEntry;

	public bool _isOpen;

	private int _foundGearItems;

	private int _foundConsumableItems;

	private int _foundTradeItems;

	private int _foundBuyBacks;

	public float _sellPromptBuffer;

	public float _dialogResponseBuffer;

	private float _initBuffer;

	[Space]
	public GameObject _listDataPrefab;

	public List<ShopListDataEntry> _listDataEntries = new List<ShopListDataEntry>();

	[Header("Shopkeep Management")]
	public MenuElement _shopKeepWindowElement;

	[SerializeField]
	private Text _shopkeepHeaderText;

	[SerializeField]
	private Text _shopkeepTabHeaderText;

	[Space]
	[SerializeField]
	private MenuElement _cleanupCounterElement;

	[SerializeField]
	private Text _cleanupCounterText;

	[Space]
	[SerializeField]
	private ShopTab _currentShopTab;

	[SerializeField]
	private RectTransform _tabButtonHighlight;

	[SerializeField]
	private Button _gearTabButton;

	[SerializeField]
	private Button _consumableTabButton;

	[SerializeField]
	private Button _tradeTabButton;

	[SerializeField]
	private Button _buybackButton;

	[Space]
	[SerializeField]
	private GameObject _rerollGamblePanel;

	[SerializeField]
	private Button _rerollGambleButton;

	[Header("Purchase Prompt Window Element")]
	[SerializeField]
	private MenuElement _promptWindowElement;

	[SerializeField]
	private Button _purchaseButton;

	[SerializeField]
	private Button _purchaseQuantityButton;

	[SerializeField]
	private Button _cancelPromptButton;

	[SerializeField]
	private Vector3 _promptOffsetPosition = Vector3.zero;

	[Header("Sell Prompt Window Element")]
	public MenuElement _sellPromptWindowElement;

	public ItemListDataEntry _selectedSellItemListEntry;

	[SerializeField]
	private Button _sellQuantityOneButton;

	[SerializeField]
	private Button _sellQuantityButton;

	[SerializeField]
	private Button _cancelSellPromptButton;

	public GameObject _sellItemField;

	[Header("Content Containers")]
	public Transform _shopkeep_gearContent;

	public Transform _shopkeep_consumableContent;

	public Transform _shopkeep_tradeContent;

	public Transform _shopkeep_buybackContent;

	[Space]
	public Transform _shopkeep_blankSlotContent;

	private readonly List<RectTransform> _collectedBlankSlots = new List<RectTransform>();

	[Header("Audio")]
	[SerializeField]
	private AudioSource _aSrc;

	[SerializeField]
	private AudioClip _openShopSound;

	[SerializeField]
	private AudioClip _closeShopSound;

	[Header("Gamepad")]
	[SerializeField]
	private RectTransform _gamepad_slotCursorRect;

	[SerializeField]
	private RectTransform _gamepad_storeTabCursor;

	[SerializeField]
	private Image _gamepad_rerollGambleIco;

	[Space]
	[SerializeField]
	private MenuElement _contextPromptElement_southButton;

	[SerializeField]
	private Text _contextPrompt_southButtonText;

	[Space]
	[SerializeField]
	private MenuElement _contextPromptElement_westButton;

	[SerializeField]
	private Text _contextPrompt_westButtonText;

	public bool _gamepad_onShopTabs;

	private int _gamepad_currentStoreSlot;

	private int _gamepad_currentShopTab;

	public int Gamepad_CurrentStoreSlot
	{
		get
		{
			return _gamepad_currentStoreSlot;
		}
		set
		{
			if (value == 8 && _gamepad_currentStoreSlot == 7)
			{
				value = 7;
				_gamepad_onShopTabs = true;
			}
			else if (value == 16 && _gamepad_currentStoreSlot == 15)
			{
				value = 15;
				_gamepad_onShopTabs = true;
			}
			else if (value == 24 && _gamepad_currentStoreSlot == 23)
			{
				value = 23;
				_gamepad_onShopTabs = true;
			}
			else if (value == 32 && _gamepad_currentStoreSlot == 31)
			{
				value = 31;
				_gamepad_onShopTabs = true;
			}
			else if (value < 0 && _gamepad_currentStoreSlot == 0)
			{
				value = 7;
				_gamepad_onShopTabs = true;
			}
			else if (value == 7 && _gamepad_currentStoreSlot == 8)
			{
				value = 15;
				_gamepad_onShopTabs = true;
			}
			else if (value == 15 && _gamepad_currentStoreSlot == 16)
			{
				value = 23;
				_gamepad_onShopTabs = true;
			}
			else if (value == 23 && _gamepad_currentStoreSlot == 24)
			{
				value = 31;
				_gamepad_onShopTabs = true;
			}
			if (value < 0)
			{
				value = 0;
			}
			if (value > 31)
			{
				value = 31;
			}
			if (_gamepad_currentStoreSlot != value)
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
			_gamepad_currentStoreSlot = value;
		}
	}

	public int Gamepad_CurrentShopTab
	{
		get
		{
			return _gamepad_currentShopTab;
		}
		set
		{
			if (value < 0)
			{
				value = 0;
			}
			if (value > Enum.GetValues(typeof(ShopTab)).Length - 1)
			{
				value = Enum.GetValues(typeof(ShopTab)).Length - 1;
			}
			if (value == 0 && _foundGearItems <= 0)
			{
				if (_gamepad_currentShopTab > value)
				{
					value--;
				}
				if (_gamepad_currentShopTab < value)
				{
					value++;
				}
			}
			if (value == 1 && _foundConsumableItems <= 0)
			{
				if (_gamepad_currentShopTab > value)
				{
					value--;
				}
				if (_gamepad_currentShopTab < value)
				{
					value++;
				}
			}
			if (value == 2 && _foundTradeItems <= 0)
			{
				if (_gamepad_currentShopTab > value)
				{
					value--;
				}
				if (_gamepad_currentShopTab < value)
				{
					value++;
				}
			}
			if (value == 3 && !_scriptShopkeep._canBuyFromPlayer)
			{
				if (_gamepad_currentShopTab > value)
				{
					value--;
				}
				if (_gamepad_currentShopTab < value)
				{
					if (_foundTradeItems > 0)
					{
						value = 2;
					}
					else if (_foundConsumableItems > 0)
					{
						value = 1;
					}
					else if (_foundGearItems > 0)
					{
						value = 0;
					}
				}
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
			case 3:
				SetTab_Buyback();
				break;
			}
			_gamepad_currentShopTab = value;
		}
	}

	private void Awake()
	{
		_current = this;
	}

	private void Start()
	{
		_shopkeepHeaderText.text = string.Empty;
		_shopkeepTabHeaderText.text = string.Empty;
		_sellItemField.SetActive(value: false);
		_listDataEntries.Clear();
		_collectedBlankSlots.AddRange(_shopkeep_blankSlotContent.GetComponentsInChildren<RectTransform>());
		_collectedBlankSlots.RemoveAt(0);
		Apply_GeneralButtons();
		Apply_PurchasePromptButtons();
		Apply_SellPromptButtons();
		void Apply_GeneralButtons()
		{
			_gearTabButton.onClick.AddListener(SetTab_Gear);
			_gearTabButton.onClick.AddListener(Cancel_SellPrompt);
			_consumableTabButton.onClick.AddListener(SetTab_Consumable);
			_consumableTabButton.onClick.AddListener(Cancel_SellPrompt);
			_tradeTabButton.onClick.AddListener(SetTab_Trade);
			_tradeTabButton.onClick.AddListener(Cancel_SellPrompt);
			_buybackButton.onClick.AddListener(SetTab_Buyback);
			_buybackButton.onClick.AddListener(Cancel_SellPrompt);
			_rerollGambleButton.onClick.AddListener(Reroll_Gambler);
		}
		void Apply_PurchasePromptButtons()
		{
			_purchaseButton.onClick.AddListener(Init_SinglePurchaseitem);
			_purchaseButton.onClick.AddListener(Cancel_PurchasePrompt);
			_purchaseQuantityButton.onClick.AddListener(Init_PurchaseQuantity);
			_cancelPromptButton.onClick.AddListener(Cancel_PurchasePrompt);
		}
		void Apply_SellPromptButtons()
		{
			_sellQuantityOneButton.onClick.AddListener(Init_SellSingleItem);
			_sellQuantityButton.onClick.AddListener(Init_SellQuantity);
			_cancelSellPromptButton.onClick.AddListener(Cancel_SellPrompt);
		}
	}

	private void Handle_TabDisplay()
	{
		_gearTabButton.gameObject.SetActive(_foundGearItems > 0);
		_consumableTabButton.gameObject.SetActive(_foundConsumableItems > 0);
		_tradeTabButton.gameObject.SetActive(_foundTradeItems > 0);
		switch (_currentShopTab)
		{
		case ShopTab.GEAR:
			_shopkeepTabHeaderText.text = "Equipment";
			_shopkeep_gearContent.localScale = Vector3.one;
			_shopkeep_consumableContent.localScale = Vector3.zero;
			_shopkeep_tradeContent.localScale = Vector3.zero;
			_shopkeep_buybackContent.localScale = Vector3.zero;
			_tabButtonHighlight.position = _gearTabButton.transform.position;
			break;
		case ShopTab.CONSUMABLE:
			_shopkeepTabHeaderText.text = "Consumables";
			_shopkeep_gearContent.localScale = Vector3.zero;
			_shopkeep_consumableContent.localScale = Vector3.one;
			_shopkeep_tradeContent.localScale = Vector3.zero;
			_shopkeep_buybackContent.localScale = Vector3.zero;
			_tabButtonHighlight.position = _consumableTabButton.transform.position;
			break;
		case ShopTab.TRADE:
			_shopkeepTabHeaderText.text = "Trade Items";
			_shopkeep_gearContent.localScale = Vector3.zero;
			_shopkeep_consumableContent.localScale = Vector3.zero;
			_shopkeep_tradeContent.localScale = Vector3.one;
			_shopkeep_buybackContent.localScale = Vector3.zero;
			_tabButtonHighlight.position = _tradeTabButton.transform.position;
			break;
		case ShopTab.BUYBACK:
			_shopkeepTabHeaderText.text = "Sold Items";
			_shopkeep_gearContent.localScale = Vector3.zero;
			_shopkeep_consumableContent.localScale = Vector3.zero;
			_shopkeep_tradeContent.localScale = Vector3.zero;
			_shopkeep_buybackContent.localScale = Vector3.one;
			_tabButtonHighlight.position = _buybackButton.transform.position;
			break;
		}
	}

	private void SetTab_Gear()
	{
		Apply_Tab(ShopTab.GEAR);
	}

	private void SetTab_Consumable()
	{
		Apply_Tab(ShopTab.CONSUMABLE);
	}

	private void SetTab_Trade()
	{
		Apply_Tab(ShopTab.TRADE);
	}

	private void SetTab_Buyback()
	{
		Apply_Tab(ShopTab.BUYBACK);
	}

	private void Apply_Tab(ShopTab _setTab)
	{
		_currentShopTab = _setTab;
		if (_setTab != ShopTab.BUYBACK)
		{
			TabMenu._current._itemCell.Init_InventoryTab((int)_currentShopTab);
		}
		Cancel_PurchasePrompt();
		Cancel_SellPrompt();
	}

	private void Handle_ShopkeepPromptWindows()
	{
		if (_sellPromptBuffer > 0f)
		{
			_sellPromptBuffer -= Time.deltaTime;
		}
		else
		{
			_sellPromptBuffer = 0f;
		}
		if (_dialogResponseBuffer > 0f)
		{
			_dialogResponseBuffer -= Time.deltaTime;
		}
		else
		{
			_dialogResponseBuffer = 0f;
		}
		Handle_PurchasePromptWindowButtons();
		void Handle_PurchasePromptWindowButtons()
		{
			_purchaseButton.interactable = _promptWindowElement.isEnabled;
			_purchaseQuantityButton.interactable = _promptWindowElement.isEnabled;
			_cancelPromptButton.interactable = _promptWindowElement.isEnabled;
		}
	}

	public void Init_PurchasePromptWindow(ShopListDataEntry _listEntry)
	{
		ShopkeepItemStruct shopkeepItemStruct = ((!_listEntry._isbuybackItem) ? _shopKeepNPC._vendorItems[_listEntry._keyID] : _shopKeepNPC._syncBuybackItems[_listEntry._keyID]);
		bool active = _listEntry._scriptableItem._itemType != ItemType.GEAR && _listEntry._scriptableItem._maxStackAmount > 1 && shopkeepItemStruct._stockQuantity >= 1;
		_purchaseQuantityButton.gameObject.SetActive(active);
		_promptWindowElement.isEnabled = true;
		_selectedPurchaseListEntry = _listEntry;
		Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z) + _promptOffsetPosition;
		_promptWindowElement.transform.position = position;
	}

	public void Cancel_PurchasePrompt()
	{
		_promptWindowElement.isEnabled = false;
		_selectedPurchaseListEntry = null;
	}

	public void Init_SellPromptWindow(ItemListDataEntry _listEntry)
	{
		bool active = _listEntry._scriptableItem._itemType != ItemType.GEAR && _listEntry._itemData._maxQuantity > 1;
		_sellQuantityButton.gameObject.SetActive(active);
		_sellPromptWindowElement.isEnabled = true;
		_selectedSellItemListEntry = _listEntry;
		Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z) + _promptOffsetPosition;
		_sellPromptWindowElement.transform.position = position;
		_sellPromptWindowElement.transform.SetParent(TabMenu._current.gameObject.GetComponentInChildren<Canvas>().transform);
	}

	public void Cancel_SellPrompt()
	{
		_sellPromptWindowElement.isEnabled = false;
		_selectedSellItemListEntry = null;
	}

	private void Update()
	{
		Handle_TabDisplay();
		Handle_GamepadControl();
		Handle_ShopkeepUIBehavior();
		Handle_ShopkeepPromptWindows();
	}

	private void Init_GamepadToolTip()
	{
		if (!Cursor.visible && (bool)_highlightedListEntry && (bool)_highlightedListEntry._scriptableItem)
		{
			_highlightedListEntry.On_HighlightEntry();
		}
	}

	private void Handle_GamepadControl()
	{
		bool flag = InputControlManager.current._setGamepadType != GamepadType.Keyboard;
		bool flag2 = GamepadInput.GetButtonDown(GamepadButton.LeftStick_Up) || GamepadInput.GetButtonDown(GamepadButton.RightStick_Up) || InputControlManager.current.DpadUp(DpadHoldType.OnDown);
		bool flag3 = GamepadInput.GetButtonDown(GamepadButton.LeftStick_Down) || GamepadInput.GetButtonDown(GamepadButton.RightStick_Down) || InputControlManager.current.DpadDown(DpadHoldType.OnDown);
		bool flag4 = GamepadInput.GetButtonDown(GamepadButton.LeftStick_Left) || GamepadInput.GetButtonDown(GamepadButton.RightStick_Left) || InputControlManager.current.DpadLeft(DpadHoldType.OnDown);
		bool flag5 = GamepadInput.GetButtonDown(GamepadButton.LeftStick_Right) || GamepadInput.GetButtonDown(GamepadButton.RightStick_Right) || InputControlManager.current.DpadRight(DpadHoldType.OnDown);
		if (_gamepad_onShopTabs && InputControlManager.current._gamepadMouseCursorBuffer <= 0f)
		{
			ToolTipManager._current.Disable_AllTooltips();
		}
		_gamepad_slotCursorRect.gameObject.SetActive(flag && !_gamepad_onShopTabs && DialogManager._current._currentGamepadTab == DialogManager_GamepadTab.Prompt_Window);
		_gamepad_storeTabCursor.gameObject.SetActive(flag && _gamepad_onShopTabs && DialogManager._current._currentGamepadTab == DialogManager_GamepadTab.Prompt_Window);
		_gamepad_slotCursorRect.anchoredPosition = _collectedBlankSlots[_gamepad_currentStoreSlot].anchoredPosition;
		_gamepad_storeTabCursor.anchoredPosition = _tabButtonHighlight.anchoredPosition;
		_gamepad_rerollGambleIco.enabled = (bool)_scriptShopkeep && _scriptShopkeep._isGambler && DialogManager._current._currentGamepadTab == DialogManager_GamepadTab.Prompt_Window;
		_contextPromptElement_southButton.isEnabled = !_gamepad_onShopTabs && (bool)_highlightedListEntry && DialogManager._current._currentGamepadTab == DialogManager_GamepadTab.Prompt_Window;
		_contextPromptElement_westButton.isEnabled = (bool)_highlightedListEntry && _highlightedListEntry._scriptableItem._maxStackAmount > 1 && !_gamepad_onShopTabs && !_highlightedListEntry._isGambleItem && DialogManager._current._currentGamepadTab == DialogManager_GamepadTab.Prompt_Window;
		if (!_isOpen)
		{
			_gamepad_currentShopTab = 0;
			_gamepad_currentStoreSlot = 0;
			_gamepad_onShopTabs = false;
			_highlightedListEntry = null;
			return;
		}
		if (!flag || (DialogManager._current._currentDialogUIPrompt != DialogUIPrompt.SHOP && DialogManager._current._currentGamepadTab != DialogManager_GamepadTab.Prompt_Window))
		{
			Gamepad_CurrentStoreSlot = 0;
			_highlightedListEntry = null;
			_gamepad_onShopTabs = false;
			return;
		}
		_contextPrompt_southButtonText.text = "Buy Item";
		if ((bool)_highlightedListEntry && _highlightedListEntry._scriptableItem._maxStackAmount > 1 && !_highlightedListEntry._isGambleItem)
		{
			_contextPrompt_westButtonText.text = "Buy Quantity Item";
		}
		if ((bool)_highlightedListEntry && InputControlManager.current._gamepadMouseCursorBuffer <= 0f)
		{
			_selectedPurchaseListEntry = _highlightedListEntry;
		}
		else if (InputControlManager.current._gamepadMouseCursorBuffer <= 0f)
		{
			_selectedPurchaseListEntry = null;
		}
		if (DialogManager._current._currentGamepadTab != DialogManager_GamepadTab.Prompt_Window)
		{
			if (InputControlManager.current._gamepadMouseCursorBuffer <= 0f)
			{
				_highlightedListEntry = null;
				_selectedPurchaseListEntry = null;
			}
		}
		else
		{
			if (TabMenu._current._itemCell._gamepad_quantityPromptSelected)
			{
				return;
			}
			if (!_gamepad_onShopTabs)
			{
				_gamepad_currentShopTab = (int)_currentShopTab;
			}
			else
			{
				_highlightedListEntry = null;
				if (InputControlManager.current._gamepadMouseCursorBuffer <= 0f)
				{
					_selectedPurchaseListEntry = null;
				}
			}
			if (flag4)
			{
				if (_gamepad_onShopTabs)
				{
					_gamepad_onShopTabs = false;
				}
				else
				{
					Gamepad_CurrentStoreSlot--;
				}
				TabMenu._current._gamepadSelectAsrc.Play();
			}
			if (flag5)
			{
				if (!_gamepad_onShopTabs)
				{
					Gamepad_CurrentStoreSlot++;
				}
				else
				{
					_gamepad_onShopTabs = false;
					Gamepad_CurrentStoreSlot = 0;
				}
				TabMenu._current._gamepadSelectAsrc.Play();
			}
			if (flag3)
			{
				if (_gamepad_onShopTabs)
				{
					Gamepad_CurrentShopTab++;
				}
				else
				{
					Gamepad_CurrentStoreSlot += 8;
				}
				TabMenu._current._gamepadSelectAsrc.Play();
			}
			if (flag2)
			{
				if (_gamepad_onShopTabs)
				{
					Gamepad_CurrentShopTab--;
				}
				else
				{
					Gamepad_CurrentStoreSlot -= 8;
				}
				TabMenu._current._gamepadSelectAsrc.Play();
			}
			for (int i = 0; i < _listDataEntries.Count; i++)
			{
				switch (_currentShopTab)
				{
				case ShopTab.GEAR:
					if (_listDataEntries[i]._scriptableItem._itemType != ItemType.GEAR || _listDataEntries[i]._isbuybackItem)
					{
						continue;
					}
					goto default;
				case ShopTab.CONSUMABLE:
					if (_listDataEntries[i]._scriptableItem._itemType != ItemType.CONSUMABLE || _listDataEntries[i]._isbuybackItem)
					{
						continue;
					}
					goto default;
				case ShopTab.TRADE:
					if (_listDataEntries[i]._scriptableItem._itemType != ItemType.TRADE || _listDataEntries[i]._isbuybackItem)
					{
						continue;
					}
					goto default;
				case ShopTab.BUYBACK:
					if (!_listDataEntries[i]._isbuybackItem)
					{
						continue;
					}
					goto default;
				default:
					if (_gamepad_currentStoreSlot != _listDataEntries[i]._gamepadIndex)
					{
						continue;
					}
					break;
				}
				_highlightedListEntry = _listDataEntries[i];
				break;
			}
			if (!_scriptShopkeep)
			{
				return;
			}
			if ((bool)_selectedPurchaseListEntry && GamepadInput.GetButtonDown(GamepadButton.Button_South))
			{
				if (!Can_AffordPurchase(_selectedPurchaseListEntry._scriptableItem, 1, _selectedPurchaseListEntry))
				{
					return;
				}
				Init_SinglePurchaseitem();
			}
			if (GamepadInput.GetButtonDown(GamepadButton.Button_West))
			{
				if (_scriptShopkeep._isGambler)
				{
					ToolTipManager._current.Disable_AllTooltips();
					_rerollGambleButton.GetComponent<ButtonSound>().Play_ClickSound();
					Reroll_Gambler();
				}
				else if ((bool)_selectedPurchaseListEntry && _selectedPurchaseListEntry._scriptableItem._itemType != ItemType.GEAR && Can_AffordPurchase(_selectedPurchaseListEntry._scriptableItem, 1, _selectedPurchaseListEntry) && _selectedPurchaseListEntry._scriptableItem._maxStackAmount > 1 && !_selectedPurchaseListEntry._isGambleItem)
				{
					Init_PurchaseQuantity();
				}
			}
		}
	}

	private void Init_SellSingleItem()
	{
		if ((bool)_selectedSellItemListEntry)
		{
			Init_SellItem(1, _selectedSellItemListEntry._dataID);
			_selectedSellItemListEntry = null;
		}
	}

	private void Init_SellQuantity()
	{
		if ((bool)_selectedSellItemListEntry)
		{
			TabMenu._current._itemCell.Enable_QuantityPrompt();
			_sellPromptWindowElement.isEnabled = false;
		}
	}

	public void Init_SellItem(int _quantity, int _index)
	{
		if (_sellPromptBuffer > 0f)
		{
			return;
		}
		ScriptableItem scriptableItem = GameManager._current.Locate_Item(Player._mainPlayer._pInventory._heldItems[_index]._itemName);
		if ((bool)scriptableItem)
		{
			int vendorCost = scriptableItem._vendorCost;
			ChatBehaviour chatBehaviour = Player._mainPlayer._chatBehaviour;
			if (!_scriptShopkeep._canBuyFromPlayer)
			{
				ErrorPromptTextManager.current.Init_ErrorPrompt("This vendor doesn't buy items.");
				chatBehaviour.Init_GameLogicMessage("This vendor doesn't buy items.");
				Init_ShopkeeperRejectionMessage();
				Cancel_SellPrompt();
				_sellPromptBuffer = 0.2f;
			}
			else if (vendorCost <= 0)
			{
				ErrorPromptTextManager.current.Init_ErrorPrompt("No Value");
				chatBehaviour.Init_GameLogicMessage("This vendor doesn't want this item.");
				Init_ShopkeeperRejectionMessage();
				Cancel_SellPrompt();
				_sellPromptBuffer = 0.2f;
			}
			else
			{
				Player._mainPlayer._pInventory.Cmd_SellItem(_shopKeepNPC.netIdentity, Player._mainPlayer._pInventory._heldItems[_index], _quantity);
				Player._mainPlayer._pInventory.Remove_Item(Player._mainPlayer._pInventory._heldItems[_index], _quantity);
				_sellPromptBuffer = 0.2f;
				Cancel_SellPrompt();
				SetTab_Buyback();
			}
		}
	}

	private void Init_ShopkeeperRejectionMessage()
	{
		if (DialogManager._current._scriptableDialog._shopkeepRejections != null && DialogManager._current._scriptableDialog._shopkeepRejections.Length != 0 && UnityEngine.Random.Range(0, 2) == 1 && !(_dialogResponseBuffer > 0f))
		{
			DialogManager._current.Start_QuickSentence(DialogManager._current._scriptableDialog._shopkeepRejections[UnityEngine.Random.Range(0, DialogManager._current._scriptableDialog._shopkeepRejections.Length)]);
			_dialogResponseBuffer = 8.25f;
		}
	}

	public void Init_SinglePurchaseitem()
	{
		if ((bool)_selectedPurchaseListEntry)
		{
			Init_PurchaseItem(_selectedPurchaseListEntry._scriptableItem, 1, _selectedPurchaseListEntry._keyID, _selectedPurchaseListEntry);
			_promptWindowElement.isEnabled = false;
			_selectedPurchaseListEntry = null;
		}
	}

	private void Init_PurchaseQuantity()
	{
		if ((bool)_selectedPurchaseListEntry)
		{
			TabMenu._current._itemCell._itemCell_quantityPromptElement.transform.SetParent(_shopKeepWindowElement.transform.parent);
			TabMenu._current._itemCell.Enable_QuantityPrompt();
			_promptWindowElement.isEnabled = false;
		}
	}

	public bool Can_AffordPurchase(ScriptableItem _scriptableItem, int _quantity, ShopListDataEntry _setEntry)
	{
		int num = _scriptableItem._vendorCost * _quantity;
		if (_setEntry._useDedicatedValue)
		{
			num = _setEntry._dedicatedValue;
		}
		int num2 = Player._mainPlayer._pInventory._heldCurrency;
		string text = GameManager._current._statLogics._currencyName;
		if (_setEntry._isGambleItem)
		{
			num = _setEntry._gambleValue;
		}
		if ((bool)_setEntry._specialPriceCost._scriptItem)
		{
			num = _setEntry._specialPriceCost._scriptItemQuantity * _quantity;
			text = _setEntry._specialPriceCost._scriptItem._itemName;
			num2 = Player._mainPlayer._pInventory.Check_ItemQuantity(_setEntry._specialPriceCost._scriptItem);
		}
		if (num > num2)
		{
			ErrorPromptTextManager.current.Init_ErrorPrompt("Not enough " + text);
			return false;
		}
		return true;
	}

	public void Init_PurchaseItem(ScriptableItem _scriptableItem, int _quantity, string _key, ShopListDataEntry _setEntry)
	{
		if (Player._mainPlayer._pInventory.Check_InventoryFull(_scriptableItem, _quantity))
		{
			return;
		}
		if (_setEntry._shopStruct._isbuybackItem && _quantity > _setEntry._shopStruct._stockQuantity)
		{
			_quantity = _setEntry._shopStruct._stockQuantity;
		}
		TabMenu._current._itemCell.Init_InventoryTab((int)_scriptableItem._itemType);
		if (Can_AffordPurchase(_scriptableItem, _quantity, _setEntry))
		{
			if (_currentShopTab == ShopTab.BUYBACK)
			{
				Player._mainPlayer._pInventory.Cmd_PurchaseBuybackItem(_shopKeepNPC, _quantity, _key);
				Player._mainPlayer._pInventory.Play_PurchaseSound();
			}
			else
			{
				Player._mainPlayer._pInventory.Local_PurchaseItem(_shopKeepNPC, _quantity, _key);
			}
			if (DialogManager._current._scriptableDialog._shopkeepResponses != null && DialogManager._current._scriptableDialog._shopkeepResponses.Length != 0 && UnityEngine.Random.Range(0, 5) == 1 && _current._dialogResponseBuffer <= 0f)
			{
				DialogManager._current.Start_QuickSentence(DialogManager._current._scriptableDialog._shopkeepResponses[UnityEngine.Random.Range(0, DialogManager._current._scriptableDialog._shopkeepResponses.Length)]);
				_current._dialogResponseBuffer = 8.25f;
			}
			ToolTipManager._current.Disable_AllTooltips();
			if (_scriptShopkeep._isGambler)
			{
				Reroll_Gambler();
			}
		}
	}

	private void Reroll_Gambler()
	{
		if ((bool)_shopKeepNPC)
		{
			Clear_ShopkeepListings();
			_shopKeepNPC._vendorItems.Clear();
			_shopKeepNPC.Init_DelayedRefreshListing();
		}
	}

	private void Handle_ShopkeepUIBehavior()
	{
		if (!_isOpen)
		{
			if (_listDataEntries.Count > 0)
			{
				Clear_ShopkeepListings();
			}
			_initBuffer = 0f;
		}
		else
		{
			if ((bool)_shopKeepNPC && (bool)_scriptShopkeep)
			{
				_buybackButton.gameObject.SetActive(_scriptShopkeep._canBuyFromPlayer);
			}
			if (_listDataEntries.Count <= 0 && (bool)_shopKeepNPC)
			{
				Refresh_ShopkeepListings(_shopKeepNPC);
			}
			if (_currentShopTab == ShopTab.GEAR && _foundGearItems <= 0)
			{
				if (_foundConsumableItems > 0)
				{
					SetTab_Consumable();
				}
				else if (_foundTradeItems > 0)
				{
					SetTab_Trade();
				}
			}
			if (_initBuffer < 0.4f)
			{
				_initBuffer += Time.deltaTime;
			}
		}
		_shopKeepWindowElement.gameObject.SetActive(_isOpen);
		_shopKeepWindowElement.isEnabled = _isOpen;
		if (!_selectedPurchaseListEntry && _promptWindowElement.isEnabled)
		{
			Cancel_PurchasePrompt();
		}
		if (DialogManager._current._currentDialogUIPrompt == DialogUIPrompt.SHOP)
		{
			if (!_isOpen)
			{
				_aSrc.PlayOneShot(_openShopSound, 1f);
			}
			TabMenu._current._itemTradeMode = true;
			_isOpen = true;
		}
		else
		{
			if (_isOpen)
			{
				_aSrc.PlayOneShot(_closeShopSound, 1f);
				TabMenu._current._itemTradeMode = false;
			}
			_currentShopTab = ShopTab.GEAR;
			_shopKeepNPC = null;
			_isOpen = false;
		}
		if (!TabMenu._current._itemTradeMode)
		{
			Cancel_SellPrompt();
			Cancel_PurchasePrompt();
		}
		if ((bool)_scriptShopkeep)
		{
			_shopkeepHeaderText.text = _scriptShopkeep._shopName ?? "";
			_cleanupCounterElement.isEnabled = (bool)_shopKeepNPC && _shopKeepNPC._shopkeepResetTimer > 0;
			if ((bool)_shopKeepNPC && _shopKeepNPC._shopkeepResetTimer > 0)
			{
				_cleanupCounterText.text = $"{_shopKeepNPC._shopkeepResetTimer}";
			}
			else
			{
				_cleanupCounterText.text = "-";
			}
			if ((bool)_scriptShopkeep)
			{
				_rerollGamblePanel.SetActive(_scriptShopkeep._isGambler);
			}
		}
	}

	public void Refresh_ShopkeepListings(NetNPC _shopkeepNPC)
	{
		if (!_scriptShopkeep)
		{
			return;
		}
		_shopKeepNPC = _shopkeepNPC;
		_foundBuyBacks = 0;
		if (_shopkeepNPC._vendorItems.Count <= 0)
		{
			_foundGearItems = 0;
			_foundConsumableItems = 0;
			_foundTradeItems = 0;
		}
		foreach (KeyValuePair<string, ShopkeepItemStruct> vendorItem in _shopkeepNPC._vendorItems)
		{
			bool flag = false;
			for (int i = 0; i < _listDataEntries.Count; i++)
			{
				if (_listDataEntries[i]._keyID == vendorItem.Key && !_listDataEntries[i]._isbuybackItem && _shopKeepNPC._vendorItems[vendorItem.Key]._stockQuantity > 0)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				Create_ShopListEntry(vendorItem.Key, _isbuybackItem: false, _shopKeepNPC._vendorItems[vendorItem.Key]._isGambleItem, _shopKeepNPC._vendorItems[vendorItem.Key]._gambleValue);
			}
		}
		foreach (KeyValuePair<string, ShopkeepItemStruct> syncBuybackItem in _shopkeepNPC._syncBuybackItems)
		{
			bool flag2 = false;
			for (int j = 0; j < _listDataEntries.Count; j++)
			{
				if (_listDataEntries[j]._keyID == syncBuybackItem.Key && _listDataEntries[j]._isbuybackItem && _shopKeepNPC._syncBuybackItems[syncBuybackItem.Key]._stockQuantity > 0)
				{
					flag2 = true;
					break;
				}
			}
			if (!flag2)
			{
				Create_ShopListEntry(syncBuybackItem.Key, _isbuybackItem: true, _isGambleItem: false, 0);
			}
		}
		if (_initBuffer < 0.24f)
		{
			if (_foundGearItems > 0)
			{
				_currentShopTab = ShopTab.GEAR;
			}
			else if (_foundConsumableItems > 0)
			{
				_currentShopTab = ShopTab.CONSUMABLE;
			}
			else if (_foundTradeItems > 0)
			{
				_currentShopTab = ShopTab.TRADE;
			}
		}
		void Create_ShopListEntry(string _key, bool _isbuybackItem, bool _isGambleItem, int _gambleValue)
		{
			ScriptableItem scriptableItem = null;
			scriptableItem = ((!_isbuybackItem) ? GameManager._current.Locate_Item(_shopkeepNPC._vendorItems[_key]._itemName) : GameManager._current.Locate_Item(_shopkeepNPC._syncBuybackItems[_key]._itemName));
			if ((bool)scriptableItem)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(_listDataPrefab);
				ShopListDataEntry component = gameObject.GetComponent<ShopListDataEntry>();
				component._shopkeepNPC = _shopkeepNPC;
				component._scriptableItem = scriptableItem;
				component._keyID = _key;
				component._isbuybackItem = _isbuybackItem;
				component._isGambleItem = _isGambleItem;
				component._gambleValue = _gambleValue;
				if (!_isbuybackItem && !string.IsNullOrWhiteSpace(_shopKeepNPC._vendorItems[_key]._specialItemCostName))
				{
					ScriptableItem scriptableItem2 = GameManager._current.Locate_Item(_shopkeepNPC._vendorItems[_key]._specialItemCostName);
					if ((bool)scriptableItem2)
					{
						component._specialPriceCost = new StatModifierCost
						{
							_scriptItem = scriptableItem2,
							_scriptItemQuantity = _shopKeepNPC._vendorItems[_key]._specialItemCostQuantity
						};
					}
				}
				ItemData itemData = new ItemData();
				if (_isbuybackItem)
				{
					itemData = new ItemData
					{
						_itemName = _shopkeepNPC._syncBuybackItems[_key]._itemName,
						_modifierID = _shopkeepNPC._syncBuybackItems[_key]._equipModifierID
					};
					gameObject.transform.SetParent(_shopkeep_buybackContent);
					component._gamepadIndex = _foundBuyBacks;
					_foundBuyBacks++;
				}
				else
				{
					itemData = new ItemData
					{
						_itemName = _shopkeepNPC._vendorItems[_key]._itemName,
						_modifierID = _shopkeepNPC._vendorItems[_key]._equipModifierID
					};
					switch (scriptableItem._itemType)
					{
					case ItemType.GEAR:
						gameObject.transform.SetParent(_shopkeep_gearContent);
						component._gamepadIndex = _foundGearItems;
						_foundGearItems++;
						break;
					case ItemType.CONSUMABLE:
						gameObject.transform.SetParent(_shopkeep_consumableContent);
						component._gamepadIndex = _foundConsumableItems;
						_foundConsumableItems++;
						break;
					case ItemType.TRADE:
						gameObject.transform.SetParent(_shopkeep_tradeContent);
						component._gamepadIndex = _foundTradeItems;
						_foundTradeItems++;
						break;
					}
				}
				gameObject.transform.position = Vector2.zero;
				gameObject.transform.localScale = Vector2.one;
				component._itemData = itemData;
				_listDataEntries.Add(component);
				_foundBuyBacks = 0;
				for (int k = 0; k < _listDataEntries.Count; k++)
				{
					if (_listDataEntries[k]._isbuybackItem)
					{
						_listDataEntries[k]._gamepadIndex = _foundBuyBacks;
						_foundBuyBacks++;
					}
				}
			}
		}
	}

	public void Clear_ShopkeepListings()
	{
		for (int i = 0; i < _listDataEntries.Count; i++)
		{
			UnityEngine.Object.Destroy(_listDataEntries[i].gameObject);
		}
		_listDataEntries.Clear();
	}

	public void Delete_ShopkeepListing(string _key)
	{
		for (int i = 0; i < _listDataEntries.Count; i++)
		{
			if (!(_listDataEntries[i]._keyID != _key) && (_listDataEntries[i]._isbuybackItem || _listDataEntries[i]._isGambleItem))
			{
				UnityEngine.Object.Destroy(_listDataEntries[i].gameObject);
				_listDataEntries.RemoveAt(i);
			}
		}
		_foundBuyBacks = 0;
		for (int j = 0; j < _listDataEntries.Count; j++)
		{
			if (_listDataEntries[j]._isbuybackItem)
			{
				_listDataEntries[j]._gamepadIndex = _foundBuyBacks;
				_foundBuyBacks++;
			}
		}
	}

	public void Disable_ShopToolTip()
	{
		ToolTipManager._current._genericToolTip.Disable_ToolTip();
	}

	public void Init_ShopTooltip(int _tabValue)
	{
		ToolTipManager._current._genericToolTip.Set_TooltipAnchorPos(new Vector2(100f, 0f));
		switch (_tabValue)
		{
		case 0:
			ToolTipManager._current.Apply_GenericToolTip("Equipment");
			break;
		case 1:
			ToolTipManager._current.Apply_GenericToolTip("Consumables");
			break;
		case 2:
			ToolTipManager._current.Apply_GenericToolTip("Trade Items");
			break;
		case 3:
			ToolTipManager._current.Apply_GenericToolTip("Sold Items");
			break;
		}
		ToolTipManager._current._genericToolTip.Enable_ToolTip();
	}
}
