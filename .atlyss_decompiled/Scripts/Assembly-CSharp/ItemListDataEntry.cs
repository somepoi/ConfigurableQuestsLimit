using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemListDataEntry : ListDataEntry, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
	[Header("Item Cell ListData")]
	public ScriptableItem _scriptableItem;

	public ItemType _itemTypeTab;

	public ItemListEntryType _entryType;

	public ItemData _itemData;

	[Space]
	public RectTransform _dataEntryTransform;

	public ItemSlotUI _parentItemSlotUI;

	public ItemSlotUI _originItemSlotUI;

	public int _originSlotNumber;

	public int _setEquipOriginSlotNum;

	public bool _displayDoubleIco;

	private Color _setItemRarityColor;

	private float _holdEntryBuffer;

	private float _quickEquipBuffer;

	private int _currentIllusionStones;

	public bool _isHeldByPointer;

	[Space]
	[SerializeField]
	private MenuElement _listEntryElement;

	[SerializeField]
	private Text _quantityCounter;

	[Space]
	[SerializeField]
	private Image _itemCooldownFill;

	[SerializeField]
	private Image _itemIconBackdrop;

	[SerializeField]
	private Image _itemIcon;

	[SerializeField]
	private Image _dualItemIco_01;

	[SerializeField]
	private Image _dualItemIco_02;

	[SerializeField]
	private Image _selectedHighlightGraphic;

	[SerializeField]
	private Image _errorIcon;

	[SerializeField]
	private Image _transmoggedIcon;

	[Space]
	[SerializeField]
	private GameObject _wepScaleIconObject;

	[SerializeField]
	private Image _wepScaleIco;

	[Space]
	[SerializeField]
	private GameObject _skillIconObject;

	[SerializeField]
	private Image _skillIco;

	public void Apply_ItemDataInfo()
	{
		if (!(_itemData._itemName == string.Empty))
		{
			_itemTypeTab = _scriptableItem._itemType;
			if ((bool)_itemIcon)
			{
				_itemIcon.enabled = true;
				_itemIcon.sprite = _scriptableItem._itemIcon;
			}
			switch (_entryType)
			{
			case ItemListEntryType.INVENTORY:
				Apply_InventoryDataInfo();
				break;
			case ItemListEntryType.STORAGE:
				Apply_StorageDataInfo();
				break;
			}
			if (!_itemData._isEquipped && _entryType != ItemListEntryType.DISPLAY)
			{
				_parentItemSlotUI._filledSlotTab[(uint)_itemTypeTab] = true;
			}
		}
		void Apply_InventoryDataInfo()
		{
			if (!_itemData._isEquipped)
			{
				for (int i = 0; i < TabMenu._current._itemCell._listDataEntries.Count; i++)
				{
					ItemListDataEntry itemListDataEntry = (ItemListDataEntry)TabMenu._current._itemCell._listDataEntries[i];
					if (itemListDataEntry.gameObject != base.gameObject && itemListDataEntry._itemData._slotNumber == _itemData._slotNumber && itemListDataEntry._itemTypeTab == _itemTypeTab)
					{
						Relocate_DataEntryToEmptySlot();
						break;
					}
				}
				_parentItemSlotUI = TabMenu._current._itemCell._itemSlots[_itemData._slotNumber];
			}
		}
		void Apply_StorageDataInfo()
		{
			if (!_itemData._isEquipped)
			{
				for (int i = 0; i < ItemStorageManager._current._storageListEntries.Count; i++)
				{
					ItemListDataEntry itemListDataEntry = ItemStorageManager._current._storageListEntries[i];
					if (itemListDataEntry.gameObject != base.gameObject && itemListDataEntry._itemData._slotNumber == _itemData._slotNumber && itemListDataEntry._itemTypeTab == _itemTypeTab)
					{
						Relocate_DataEntryToEmptySlot();
						break;
					}
				}
			}
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if ((bool)_parentItemSlotUI)
		{
			_parentItemSlotUI._slotButton.Select();
		}
		Init_ItemEntryTooltip();
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (_entryType == ItemListEntryType.INVENTORY && !_itemData._isEquipped)
		{
			TabMenu._current._itemCell._currentHighlightedDataEntry = null;
		}
		ToolTipManager._current.Disable_AllTooltips();
		EventSystem.current.SetSelectedGameObject(null);
		_isHighlighted = false;
	}

	public void Init_ItemEntryTooltip()
	{
		if (_entryType == ItemListEntryType.INVENTORY && !_itemData._isEquipped)
		{
			TabMenu._current._itemCell._currentHighlightedDataEntry = this;
		}
		bool flag = _scriptableItem._itemType == ItemType.GEAR;
		_isHighlighted = true;
		if (TabMenu._current._itemCell._currentEquipCellTab == EquipCellTab.VANITY_PANEL)
		{
			Init_VanityTooltip();
		}
		else if (flag)
		{
			Init_EquipTooltip();
		}
		else
		{
			Init_ItemTooltip();
		}
	}

	private void Init_VanityTooltip()
	{
		if (_transmoggedIcon.enabled)
		{
			ToolTipManager._current.Apply_GenericToolTip(_scriptableItem._itemName + "\n(Can Transmog)");
		}
		else
		{
			ToolTipManager._current.Apply_GenericToolTip(_scriptableItem._itemName + "\n(Cannot Transmog)");
		}
		if (InputControlManager.current._setGamepadType != GamepadType.Keyboard)
		{
			ToolTipManager._current._genericToolTip._anchorReference = base.transform;
		}
		ToolTipManager._current._genericToolTip.Set_TooltipAnchorPos(new Vector3(-55f, 35f, 0f));
		ToolTipManager._current._genericToolTip.Enable_ToolTip();
	}

	private void Init_EquipTooltip()
	{
		float num = -215f;
		if (_entryType == ItemListEntryType.STORAGE)
		{
			num *= -1f;
		}
		if (InputControlManager.current._setGamepadType != GamepadType.Keyboard)
		{
			ToolTipManager._current._equipToolTip._anchorReference = base.transform;
		}
		ToolTipManager._current._equipToolTip.Set_TooltipAnchorPos(new Vector3(num, 0f, 0f));
		ToolTipManager._current._equipToolTip.Enable_ToolTip();
		int num2 = (int)((float)_scriptableItem._vendorCost * GameManager._current._statLogics._vendorBuybackPercent);
		if (num2 <= 0)
		{
			num2 = 1;
		}
		ToolTipManager._current._equipToolTip._vendorValue = num2;
		ToolTipManager._current.Apply_EquipDataToolTip((ScriptableEquipment)_scriptableItem, _isGambleItem: false);
		ToolTipManager._current._equipToolTip._setItemData = _itemData;
	}

	private void Init_ItemTooltip()
	{
		float num = -160f;
		if (_entryType == ItemListEntryType.STORAGE)
		{
			num = 175f;
		}
		int num2 = (int)((float)_scriptableItem._vendorCost * GameManager._current._statLogics._vendorBuybackPercent);
		if (num2 <= 0)
		{
			num2 = 0;
		}
		if (InputControlManager.current._setGamepadType != GamepadType.Keyboard)
		{
			ToolTipManager._current._itemToolTip._anchorReference = base.transform;
		}
		ToolTipManager._current._itemToolTip.Set_TooltipAnchorPos(new Vector3(num, 0f, 0f));
		ToolTipManager._current.Apply_ItemDataToolTip(_scriptableItem, _isGambleItem: false);
		ToolTipManager._current._itemToolTip._setItemQuantity = _itemData._quantity;
		ToolTipManager._current._itemToolTip._vendorValue = num2;
		ToolTipManager._current._itemToolTip.Enable_ToolTip();
		if (_scriptableItem.GetType() == typeof(ScriptableSkillScroll))
		{
			ScriptableSkillScroll scriptableSkillScroll = (ScriptableSkillScroll)_scriptableItem;
			ToolTipManager._current._skillToolTip._skillStruct = new SkillStruct
			{
				_skillUnlocked = false,
				_skillName = scriptableSkillScroll._scriptableSkill._skillName
			};
			if (InputControlManager.current._setGamepadType != GamepadType.Keyboard)
			{
				ToolTipManager._current._skillToolTip._anchorReference = ToolTipManager._current._itemToolTip.transform;
				ToolTipManager._current._skillToolTip.Set_TooltipAnchorPos(new Vector3(num * 1.85f, 0f, 0f));
			}
			else
			{
				ToolTipManager._current._skillToolTip.Set_TooltipAnchorPos(new Vector3(num * 2.85f, 0f, 0f));
			}
			ToolTipManager._current._skillToolTip._scriptSkill = scriptableSkillScroll._scriptableSkill;
			ToolTipManager._current._skillToolTip.Enable_ToolTip();
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		bool _inTradeMode;
		bool _holdingLeftShift;
		if ((_entryType != ItemListEntryType.STORAGE || !(ItemStorageManager._current._commandBuffer > 0f)) && _entryType != ItemListEntryType.DISPLAY && !(_holdEntryBuffer > 0f))
		{
			_inTradeMode = TabMenu._current._itemTradeMode;
			_holdingLeftShift = Input.GetKey(KeyCode.LeftShift);
			ToolTipManager._current.Disable_AllTooltips();
			_holdEntryBuffer = 0.2f;
			switch (eventData.button)
			{
			case PointerEventData.InputButton.Left:
				On_LeftClick();
				break;
			case PointerEventData.InputButton.Right:
				On_RightClick();
				break;
			}
		}
		void Handle_TradeModePointerDown()
		{
			if (ShopkeepManager._current._isOpen)
			{
				if (_holdingLeftShift)
				{
					ShopkeepManager._current.Init_SellItem(_itemData._quantity, _dataID);
					Relocate_ToOriginSlot();
				}
				else
				{
					ShopkeepManager._current._sellItemField.SetActive(value: true);
					ShopkeepManager._current.Cancel_SellPrompt();
				}
			}
			else if (ItemStorageManager._current._isOpen)
			{
				if (_holdingLeftShift && ItemStorageManager._current._commandBuffer <= 0f)
				{
					if (_entryType == ItemListEntryType.STORAGE)
					{
						Init_TakeItemFromStorage();
					}
					else
					{
						Init_PutItemIntoStorage(0);
					}
				}
			}
			else if (EnchanterManager._current._isOpen && _holdingLeftShift)
			{
				EnchanterManager._current.Clear_EnchantOption();
				EnchanterManager._current._setItemData = Player._mainPlayer._pInventory._heldItems[_dataID];
				Relocate_ToOriginSlot();
			}
		}
		void On_LeftClick()
		{
			if (_inTradeMode)
			{
				Handle_TradeModePointerDown();
			}
			TabMenu._current._itemCell._currentHighlightedDataEntry = null;
			TabMenu._current._itemCell._currentHighlightedItemSlot = null;
			TabMenu._current._itemCell._aSrc_itemPick.Stop();
			TabMenu._current._itemCell._aSrc_itemPick.Play();
			if (!(EnchanterManager._current._isOpen & _holdingLeftShift))
			{
				if (((_entryType == ItemListEntryType.INVENTORY) & _holdingLeftShift) && !_inTradeMode)
				{
					if (EnchanterManager._current._isOpen)
					{
						return;
					}
					if (_scriptableItem._itemType == ItemType.GEAR)
					{
						Init_QuickEquipItem();
						return;
					}
					if (_scriptableItem._itemType == ItemType.CONSUMABLE)
					{
						ScriptableConsumable scriptConsumable = (ScriptableConsumable)_scriptableItem;
						Player._mainPlayer._pInventory.Init_UseConsumable(scriptConsumable, _itemData);
						ToolTipManager._current.Disable_AllTooltips();
						return;
					}
				}
				TabMenu._current._itemCell._itemCellBackdrop.SetActive(value: true);
				TabMenu._current._itemCell.Cancel_ItemPromptWindow();
				if ((bool)_parentItemSlotUI)
				{
					_parentItemSlotUI._filledSlotTab[(uint)_itemTypeTab] = false;
					_originItemSlotUI = _parentItemSlotUI;
				}
				_isHeldByPointer = true;
				_parentItemSlotUI = null;
				_originSlotNumber = _itemData._slotNumber;
				if (_entryType == ItemListEntryType.STORAGE)
				{
					ItemStorageManager._current._commandBuffer = 0.2f;
				}
			}
		}
		void On_RightClick()
		{
			if (_inTradeMode)
			{
				if (ShopkeepManager._current._isOpen)
				{
					ShopkeepManager._current.Init_SellPromptWindow(this);
				}
			}
			else
			{
				TabMenu._current._itemCell.Init_ItemPromptWindow(this);
			}
		}
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		if (_entryType == ItemListEntryType.DISPLAY || eventData.button != PointerEventData.InputButton.Left)
		{
			return;
		}
		bool itemTradeMode = TabMenu._current._itemTradeMode;
		if (_entryType != ItemListEntryType.STORAGE && (!TabMenu._current._itemCell._heldItemListDataEntry || !TabMenu._current._isOpen))
		{
			return;
		}
		if ((bool)TabMenu._current._itemCell._currentHighlightedItemSlot)
		{
			if (_entryType == ItemListEntryType.STORAGE)
			{
				Init_TakeItemFromStorage();
			}
			else
			{
				_parentItemSlotUI = TabMenu._current._itemCell._currentHighlightedItemSlot;
				_parentItemSlotUI._filledSlotTab[(uint)_itemTypeTab] = true;
			}
		}
		else if ((bool)ItemStorageManager._current._currentHighlightedItemSlot)
		{
			if (_entryType == ItemListEntryType.STORAGE)
			{
				_parentItemSlotUI = ItemStorageManager._current._currentHighlightedItemSlot;
				_parentItemSlotUI._filledSlotTab[(uint)_itemTypeTab] = true;
			}
			else
			{
				for (int i = 0; i < ItemStorageManager._current._storageItemSlots.Length; i++)
				{
					if (!(ItemStorageManager._current._storageItemSlots[i] != ItemStorageManager._current._currentHighlightedItemSlot))
					{
						Init_PutItemIntoStorage(i);
						break;
					}
				}
			}
		}
		else
		{
			bool num = (bool)eventData.pointerCurrentRaycast.gameObject && eventData.pointerCurrentRaycast.gameObject == TabMenu._current._itemCell._itemCellBackdrop;
			bool flag = (bool)eventData.pointerCurrentRaycast.gameObject && eventData.pointerCurrentRaycast.gameObject == ShopkeepManager._current._sellItemField;
			bool flag2 = (bool)eventData.pointerCurrentRaycast.gameObject && eventData.pointerCurrentRaycast.gameObject == EnchanterManager._current._enchantSlotElement.gameObject;
			bool flag3 = (bool)eventData.pointerCurrentRaycast.gameObject && (bool)eventData.pointerCurrentRaycast.gameObject.GetComponent<EquipUISlot>();
			bool flag4 = (bool)eventData.pointerCurrentRaycast.gameObject && (bool)eventData.pointerCurrentRaycast.gameObject.GetComponent<QuickItemSlot>();
			ItemListDataEntry itemListDataEntry = null;
			if ((bool)eventData.pointerCurrentRaycast.gameObject)
			{
				itemListDataEntry = eventData.pointerCurrentRaycast.gameObject.GetComponent<ItemListDataEntry>();
			}
			bool flag5 = !num && !itemListDataEntry && !flag4 && !flag3;
			if (num)
			{
				Relocate_DataEntryToEmptySlot();
			}
			else if ((bool)itemListDataEntry)
			{
				Override_ItemListEntry(itemListDataEntry);
			}
			if (flag4)
			{
				QuickItemSlot component = eventData.pointerCurrentRaycast.gameObject.GetComponent<QuickItemSlot>();
				Set_ToQuickSlot(component);
			}
			if (flag3 && _entryType != ItemListEntryType.STORAGE)
			{
				eventData.pointerCurrentRaycast.gameObject.GetComponent<EquipUISlot>().Init_EquipOnEntryDrop(this);
			}
			if (flag2)
			{
				EnchanterManager._current.Clear_EnchantOption();
				EnchanterManager._current._setItemData = Player._mainPlayer._pInventory._heldItems[_dataID];
			}
			if (flag5)
			{
				if (itemTradeMode)
				{
					if (flag && ShopkeepManager._current._sellPromptBuffer <= 0f)
					{
						ShopkeepManager._current.Init_SellItem(_itemData._quantity, _dataID);
						TabMenu._current._selectedListID = _dataID;
						Relocate_ToOriginSlot();
					}
					else
					{
						Relocate_ToOriginSlot();
					}
				}
				else if (TabMenu._current._isOpen)
				{
					if (!Player._mainPlayer._pCombat._isDisarmed && !Player._mainPlayer._pInventory._withinItemDropBuffer)
					{
						TabMenu._current._selectedListID = _dataID;
						TabMenu._current._itemCell.PromptCmd_DropItem(0, _itemData);
					}
					else
					{
						Relocate_ToOriginSlot();
					}
				}
				ToolTipManager._current._itemToolTip.Disable_ToolTip();
				ToolTipManager._current._equipToolTip.Disable_ToolTip();
			}
		}
		ShopkeepManager._current._sellItemField.SetActive(value: false);
		TabMenu._current._itemCell._heldItemListDataEntry = null;
		TabMenu._current._itemCell._itemCellBackdrop.SetActive(value: false);
		TabMenu._current._itemCell._aSrc_itemPlace.Stop();
		TabMenu._current._itemCell._aSrc_itemPlace.Play();
		_originItemSlotUI = null;
		_isHeldByPointer = false;
		Init_SaveProfiles();
	}

	public void Init_QuickEquipItem()
	{
		if (_quickEquipBuffer > 0f)
		{
			return;
		}
		ScriptableEquipment scriptableEquipment = (ScriptableEquipment)_scriptableItem;
		if (TabMenu._current._itemCell._currentEquipCellTab == EquipCellTab.VANITY_PANEL)
		{
			if (CanTransmogItem())
			{
				TabMenu._current._itemCell.PromptCmd_TransmogEquip(this);
			}
		}
		else if (scriptableEquipment.CanEquipItem(Player._mainPlayer._pStats, _isClickPrompt: true))
		{
			Player._mainPlayer._pEquipment.Init_EquipItem(_itemData);
			ToolTipManager._current.Disable_AllTooltips();
		}
		TabMenu._current._itemCell.Init_InventoryTab(1);
		TabMenu._current._itemCell.Init_InventoryTab(0);
		_quickEquipBuffer = 0.8f;
	}

	public void Override_ItemListEntry(ItemListDataEntry _otherEntry)
	{
		bool num = _otherEntry._scriptableItem == _scriptableItem && _scriptableItem._itemType != ItemType.GEAR && _otherEntry._itemData._quantity < _otherEntry._itemData._maxQuantity && _itemData._quantity < _itemData._maxQuantity;
		bool flag = _otherEntry._entryType == _entryType;
		if (num)
		{
			StackOn_Entry(_otherEntry);
		}
		else if (flag)
		{
			ItemSlotUI parentItemSlotUI = _otherEntry._parentItemSlotUI;
			_otherEntry._parentItemSlotUI = _originItemSlotUI;
			_parentItemSlotUI = parentItemSlotUI;
			_parentItemSlotUI._filledSlotTab[(uint)_itemTypeTab] = true;
			if ((bool)_otherEntry._parentItemSlotUI)
			{
				_otherEntry._parentItemSlotUI._filledSlotTab[(uint)_itemTypeTab] = true;
			}
		}
	}

	public void Set_ToQuickSlot(QuickItemSlot _quickSlot)
	{
		if (_scriptableItem._itemType == ItemType.CONSUMABLE || _scriptableItem._itemType == ItemType.TRADE)
		{
			_quickSlot.transform.localScale = Vector2.one * 2.5f;
			_quickSlot.Set_ItemToSlot(_scriptableItem);
			_quickSlot._originParentIDLE = this;
		}
		else
		{
			ErrorPromptTextManager.current.Init_ErrorPrompt("Invalid Item");
		}
		Relocate_DataEntryToEmptySlot();
	}

	public void Init_TakeItemFromStorage()
	{
		PlayerInventory pInventory = Player._mainPlayer._pInventory;
		bool flag = false;
		switch (_scriptableItem._itemType)
		{
		case ItemType.GEAR:
			flag = pInventory._usedEquipmentSlots >= GameManager._current._statLogics._maxInventorySlotStruct._maxEquipmentSlots;
			break;
		case ItemType.CONSUMABLE:
			flag = pInventory._usedConsumableItemSlots >= GameManager._current._statLogics._maxInventorySlotStruct._maxConsumableItemSlots;
			break;
		case ItemType.TRADE:
			flag = pInventory._usedTradeItemSlots >= GameManager._current._statLogics._maxInventorySlotStruct._maxTradeItemSlots;
			break;
		}
		if (flag)
		{
			ErrorPromptTextManager.current.Init_ErrorPrompt("Inventory Full");
			return;
		}
		ItemStorageManager._current.Delete_StorageEntry(_itemData);
		int slotNumber = 0;
		for (int i = 0; i < TabMenu._current._itemCell._itemSlots.Length; i++)
		{
			if (TabMenu._current._itemCell._currentHighlightedItemSlot == TabMenu._current._itemCell._itemSlots[i])
			{
				slotNumber = i;
				break;
			}
		}
		ItemData itemData = new ItemData
		{
			_itemName = _itemData._itemName,
			_quantity = _itemData._quantity,
			_maxQuantity = _itemData._maxQuantity,
			_isEquipped = false,
			_isAltWeapon = false,
			_modifierID = _itemData._modifierID,
			_damageTypeOverride = _itemData._damageTypeOverride,
			_useDamageTypeOverride = _itemData._useDamageTypeOverride,
			_slotNumber = slotNumber
		};
		pInventory.Add_Item(itemData, _autoStack: true);
		ItemStorageManager._current._commandBuffer = 0.25f;
		Init_SaveProfiles();
		ProfileDataManager._current.Save_ItemStorageData();
	}

	public void Init_PutItemIntoStorage(int _setItemSlot)
	{
		if (_entryType != ItemListEntryType.INVENTORY)
		{
			return;
		}
		bool flag = false;
		if ((bool)_scriptableItem._scriptableQuest)
		{
			ErrorPromptTextManager.current.Init_ErrorPrompt("Cannot put in Storage");
			Relocate_ToOriginSlot();
			return;
		}
		for (int i = 0; i < ItemStorageManager._current._storageListEntries.Count; i++)
		{
			if (ItemStorageManager._current._storageListEntries[i]._scriptableItem._maxStackAmount > 1 && !(ItemStorageManager._current._storageListEntries[i]._scriptableItem != _scriptableItem) && _itemData._quantity + ItemStorageManager._current._storageListEntries[i]._itemData._quantity <= ItemStorageManager._current._storageListEntries[i]._itemData._maxQuantity)
			{
				ItemStorageManager._current._storageListEntries[i]._itemData._quantity += _itemData._quantity;
				Player._mainPlayer._pInventory.Remove_Item(_itemData, 0);
				Init_SaveProfiles();
				ProfileDataManager._current.Save_ItemStorageData();
				return;
			}
		}
		switch (ItemStorageManager._current._selectedStorageTab)
		{
		case 0:
			flag = ItemStorageManager._current._storageSizes[(uint)_scriptableItem._itemType] >= 48;
			break;
		case 1:
			flag = ItemStorageManager._current._storageSizes_01[(uint)_scriptableItem._itemType] >= 48;
			break;
		case 2:
			flag = ItemStorageManager._current._storageSizes_02[(uint)_scriptableItem._itemType] >= 48;
			break;
		case 3:
			flag = ItemStorageManager._current._storageSizes_03[(uint)_scriptableItem._itemType] >= 48;
			break;
		case 4:
			flag = ItemStorageManager._current._storageSizes_04[(uint)_scriptableItem._itemType] >= 48;
			break;
		case 5:
			flag = ItemStorageManager._current._storageSizes_05[(uint)_scriptableItem._itemType] >= 48;
			break;
		case 6:
			flag = ItemStorageManager._current._storageSizes_06[(uint)_scriptableItem._itemType] >= 48;
			break;
		case 7:
			flag = ItemStorageManager._current._storageSizes_07[(uint)_scriptableItem._itemType] >= 48;
			break;
		}
		if (flag)
		{
			ErrorPromptTextManager.current.Init_ErrorPrompt("Storage Full");
			Relocate_ToOriginSlot();
			return;
		}
		ItemStorageManager._current._commandBuffer = 0.25f;
		ItemStorageManager._current.Create_StorageEntry(_itemData, _scriptableItem, ItemStorageManager._current._storageListEntries.Count, _setItemSlot);
		switch (ItemStorageManager._current._selectedStorageTab)
		{
		case 0:
			ItemStorageManager._current._itemDatas.Add(_itemData);
			break;
		case 1:
			ItemStorageManager._current._itemDatas_01.Add(_itemData);
			break;
		case 2:
			ItemStorageManager._current._itemDatas_02.Add(_itemData);
			break;
		case 3:
			ItemStorageManager._current._itemDatas_03.Add(_itemData);
			break;
		case 4:
			ItemStorageManager._current._itemDatas_04.Add(_itemData);
			break;
		case 5:
			ItemStorageManager._current._itemDatas_05.Add(_itemData);
			break;
		case 6:
			ItemStorageManager._current._itemDatas_06.Add(_itemData);
			break;
		case 7:
			ItemStorageManager._current._itemDatas_07.Add(_itemData);
			break;
		}
		Player._mainPlayer._pInventory.Remove_Item(_itemData, 0);
		Init_SaveProfiles();
		ProfileDataManager._current.Save_ItemStorageData();
	}

	public void StackOn_Entry(ItemListDataEntry _otherEntry)
	{
		if (_entryType == ItemListEntryType.DISPLAY)
		{
			return;
		}
		PlayerInventory pInventory = Player._mainPlayer._pInventory;
		if (!(_otherEntry._scriptableItem == _scriptableItem) || _scriptableItem._itemType == ItemType.GEAR || _otherEntry._itemData._quantity >= _otherEntry._itemData._maxQuantity || _itemData._quantity >= _itemData._maxQuantity)
		{
			return;
		}
		int quantity = _otherEntry._itemData._quantity;
		int value = _itemData._quantity + _otherEntry._itemData._quantity;
		value = Mathf.Clamp(value, 0, _otherEntry._itemData._maxQuantity);
		_otherEntry._itemData._quantity = value;
		_itemData._quantity -= value - quantity;
		if (_itemData._quantity <= 0)
		{
			if (_entryType == ItemListEntryType.STORAGE)
			{
				ItemStorageManager._current.Delete_StorageEntry(_itemData);
			}
			else
			{
				pInventory.Remove_Item(_itemData, 0);
			}
		}
		if (ItemStorageManager._current._isOpen)
		{
			ProfileDataManager._current.Save_ItemStorageData();
		}
	}

	public void Relocate_ToOriginSlot()
	{
		if (_entryType != ItemListEntryType.DISPLAY)
		{
			_itemData._slotNumber = _originSlotNumber;
			if (_entryType == ItemListEntryType.STORAGE)
			{
				_parentItemSlotUI = ItemStorageManager._current._storageItemSlots[_itemData._slotNumber];
			}
			else
			{
				_parentItemSlotUI = TabMenu._current._itemCell._itemSlots[_itemData._slotNumber];
			}
			_parentItemSlotUI._filledSlotTab[(uint)_itemTypeTab] = true;
		}
	}

	public void Relocate_DataEntryToEmptySlot()
	{
		if (_entryType == ItemListEntryType.DISPLAY)
		{
			return;
		}
		if (_entryType == ItemListEntryType.STORAGE)
		{
			for (int i = 0; i < ItemStorageManager._current._storageItemSlots.Length; i++)
			{
				if (!ItemStorageManager._current._storageItemSlots[i]._filledSlotTab[(uint)_itemTypeTab])
				{
					_parentItemSlotUI = ItemStorageManager._current._storageItemSlots[i];
					_parentItemSlotUI._filledSlotTab[(uint)_itemTypeTab] = true;
					_itemData._slotNumber = i;
					break;
				}
			}
			return;
		}
		for (int j = 0; j < TabMenu._current._itemCell._itemSlots.Length; j++)
		{
			if (!TabMenu._current._itemCell._itemSlots[j]._filledSlotTab[(uint)_itemTypeTab])
			{
				_parentItemSlotUI = TabMenu._current._itemCell._itemSlots[j];
				_parentItemSlotUI._filledSlotTab[(uint)_itemTypeTab] = true;
				_itemData._slotNumber = j;
				break;
			}
		}
	}

	public void Init_SaveProfiles()
	{
		if ((bool)Player._mainPlayer && !Player._mainPlayer._bufferingStatus)
		{
			StopCoroutine("Begin_SaveProfiles");
			StartCoroutine(Begin_SaveProfiles());
		}
	}

	private IEnumerator Begin_SaveProfiles()
	{
		if (Player._mainPlayer._bufferingStatus)
		{
			yield break;
		}
		yield return new WaitForSeconds(0.2f);
		if (!Player._mainPlayer._bufferingStatus)
		{
			ProfileDataManager._current.Save_ProfileData();
			if (ItemStorageManager._current._isOpen)
			{
				ProfileDataManager._current.Save_ItemStorageData();
			}
		}
	}

	private void Update()
	{
		Handle_ItemData();
	}

	private void Handle_ItemData()
	{
		if (_holdEntryBuffer > 0f)
		{
			_holdEntryBuffer -= Time.deltaTime;
		}
		else
		{
			_holdEntryBuffer = 0f;
		}
		if (_quickEquipBuffer > 0f)
		{
			_quickEquipBuffer -= Time.deltaTime;
		}
		else
		{
			_quickEquipBuffer = 0f;
		}
		if (!Player._mainPlayer || !TabMenu._current._isOpen || _itemData._itemName == string.Empty)
		{
			return;
		}
		if ((bool)_scriptableItem)
		{
			if (_itemData._maxQuantity > _scriptableItem._maxStackAmount)
			{
				_itemData._maxQuantity = _scriptableItem._maxStackAmount;
			}
			if (_itemData._quantity > _scriptableItem._maxStackAmount)
			{
				_itemData._quantity = _scriptableItem._maxStackAmount;
			}
		}
		if (_itemData._isEquipped)
		{
			_listEntryElement.isEnabled = false;
			if ((bool)_parentItemSlotUI)
			{
				_parentItemSlotUI._filledSlotTab[(uint)_itemTypeTab] = false;
				_parentItemSlotUI = null;
			}
			_setEquipOriginSlotNum = -1;
			if ((bool)TabMenu._current._itemCell._selectedItemListDataEntry && !TabMenu._current._itemCell._selectedItemListDataEntry._itemData._isEquipped)
			{
				_setEquipOriginSlotNum = TabMenu._current._itemCell._selectedItemListDataEntry._originSlotNumber;
			}
			if ((bool)TabMenu._current._itemCell._heldItemListDataEntry)
			{
				_setEquipOriginSlotNum = TabMenu._current._itemCell._heldItemListDataEntry._originSlotNumber;
			}
			return;
		}
		if (!_listEntryElement.isEnabled)
		{
			bool flag = false;
			if (_setEquipOriginSlotNum > -1)
			{
				_originSlotNumber = _setEquipOriginSlotNum;
				flag = true;
			}
			if ((bool)TabMenu._current._itemCell._currentHighlightedDataEntry)
			{
				_originSlotNumber = TabMenu._current._itemCell._currentHighlightedDataEntry._originSlotNumber;
				flag = true;
			}
			else
			{
				for (int i = 0; i < TabMenu._current._itemCell._itemSlots.Length; i++)
				{
					if (!(TabMenu._current._itemCell._itemSlots[i] != TabMenu._current._itemCell._currentHighlightedItemSlot))
					{
						_originSlotNumber = i;
						flag = true;
					}
				}
			}
			if (flag)
			{
				Relocate_ToOriginSlot();
			}
			else
			{
				Relocate_DataEntryToEmptySlot();
			}
			_listEntryElement.isEnabled = true;
		}
		if ((bool)_parentItemSlotUI)
		{
			_parentItemSlotUI._filledSlotTab[(uint)_scriptableItem._itemType] = true;
		}
		_selectedHighlightGraphic.enabled = ShopkeepManager._current._selectedSellItemListEntry == this || TabMenu._current._itemCell._selectedItemListDataEntry == this;
		if ((bool)_quantityCounter)
		{
			if (_itemData._maxQuantity > 1)
			{
				_quantityCounter.text = $"{_itemData._quantity}";
			}
			else
			{
				_quantityCounter.text = string.Empty;
			}
		}
		_itemIcon.enabled = !_displayDoubleIco;
		if ((bool)_dualItemIco_01)
		{
			if (!_displayDoubleIco)
			{
				_dualItemIco_01.color = Color.clear;
			}
			else
			{
				_dualItemIco_01.color = _itemIcon.color;
			}
			_dualItemIco_01.sprite = _itemIcon.sprite;
		}
		if ((bool)_dualItemIco_02)
		{
			if (!_displayDoubleIco)
			{
				_dualItemIco_02.color = Color.clear;
			}
			else
			{
				_dualItemIco_02.color = _itemIcon.color;
			}
			_dualItemIco_02.sprite = _itemIcon.sprite;
		}
		Handle_HoldByPointerBehavior();
		Handle_SlotRepositioning();
		Handle_TransmogEquipConditions();
		Check_ScriptableItemData();
		void Check_ScriptableItemData()
		{
			if (TabMenu._current._itemCell._currentEquipCellTab != EquipCellTab.VANITY_PANEL && (bool)_scriptableItem)
			{
				_setItemRarityColor = GameManager._current._statLogics._itemRarityColors[(uint)_scriptableItem._itemRarity];
				_quantityCounter.enabled = _scriptableItem._itemType == ItemType.CONSUMABLE || _scriptableItem._itemType == ItemType.TRADE;
				if (_scriptableItem._itemType == ItemType.GEAR)
				{
					ScriptableEquipment scriptableEquipment = (ScriptableEquipment)_scriptableItem;
					if (_scriptableItem._itemRarity == ItemRarity.Cosmetic)
					{
						_itemData._modifierID = 0;
					}
					else if (_itemData._modifierID > 0)
					{
						_setItemRarityColor = GameManager._current._statLogics._itemRarityColors[(uint)(scriptableEquipment._itemRarity + 1)];
					}
					_errorIcon.enabled = !scriptableEquipment.CanEquipItem(Player._mainPlayer._pStats, _isClickPrompt: false);
					if (scriptableEquipment.GetType() == typeof(ScriptableWeapon))
					{
						ScriptableWeapon scriptableWeapon = (ScriptableWeapon)scriptableEquipment;
						_displayDoubleIco = scriptableWeapon.weaponType._weaponDisplayMode == WeaponRenderDisplaymode.DUAL;
						if ((bool)_wepScaleIconObject)
						{
							_wepScaleIconObject.SetActive(_itemData._useDamageTypeOverride);
							switch (_itemData._damageTypeOverride)
							{
							case DamageType.Strength:
								_wepScaleIco.sprite = GameManager._current._statLogics._statAttributes[0]._attributeIcon;
								break;
							case DamageType.Dexterity:
								_wepScaleIco.sprite = GameManager._current._statLogics._statAttributes[2]._attributeIcon;
								break;
							case DamageType.Mind:
								_wepScaleIco.sprite = GameManager._current._statLogics._statAttributes[1]._attributeIcon;
								break;
							}
						}
					}
					else if ((bool)_wepScaleIconObject)
					{
						_wepScaleIconObject.SetActive(value: false);
					}
				}
				else if ((bool)_wepScaleIconObject)
				{
					_wepScaleIconObject.SetActive(value: false);
				}
				if (_scriptableItem._itemType == ItemType.CONSUMABLE)
				{
					ScriptableConsumable scriptableConsumable = (ScriptableConsumable)_scriptableItem;
					if (!scriptableConsumable.CanUseConsumable(Player._mainPlayer.GetComponent<StatusEntity>(), _isButtonPrompt: false))
					{
						_itemIcon.color = Color.grey;
					}
					else
					{
						_itemIcon.color = Color.white;
					}
					bool flag2 = scriptableConsumable.GetType() == typeof(ScriptableSkillScroll);
					_skillIconObject.SetActive(flag2);
					if (flag2)
					{
						ScriptableSkillScroll scriptableSkillScroll = (ScriptableSkillScroll)scriptableConsumable;
						_skillIco.sprite = scriptableSkillScroll._scriptableSkill._skillIcon;
					}
					if ((bool)_itemCooldownFill)
					{
						PlayerInventory pInventory = Player._mainPlayer._pInventory;
						_itemCooldownFill.enabled = false;
						for (int j = 0; j < pInventory._consumableBuffers.Count; j++)
						{
							if (pInventory._consumableBuffers[j]._scriptableConsumable._itemName == scriptableConsumable._itemName)
							{
								_itemCooldownFill.enabled = true;
								_itemCooldownFill.fillAmount = pInventory._consumableBuffers[j]._bufferTimer / pInventory._consumableBuffers[j]._scriptableConsumable._consumableCooldown;
							}
						}
					}
					_errorIcon.enabled = false;
				}
				else
				{
					if ((bool)_itemCooldownFill)
					{
						_itemCooldownFill.enabled = false;
					}
					_skillIconObject.SetActive(value: false);
				}
				if (_entryType == ItemListEntryType.DISPLAY || _scriptableItem._itemType == ItemType.TRADE)
				{
					_errorIcon.enabled = false;
				}
				_itemIconBackdrop.color = _setItemRarityColor;
			}
		}
		void Handle_HoldByPointerBehavior()
		{
			if (_entryType != ItemListEntryType.DISPLAY)
			{
				_itemIconBackdrop.raycastTarget = !_isHeldByPointer;
				if (!_isHeldByPointer)
				{
					if ((bool)_parentItemSlotUI)
					{
						_dataEntryTransform.position = _parentItemSlotUI.transform.position;
					}
					else
					{
						Relocate_ToOriginSlot();
					}
					switch (_scriptableItem._itemType)
					{
					case ItemType.GEAR:
						if (_entryType == ItemListEntryType.STORAGE)
						{
							base.transform.SetParent(ItemStorageManager._current._gearTabContainer);
						}
						else
						{
							base.transform.SetParent(TabMenu._current._itemCell._itemCell_gearContent);
						}
						break;
					case ItemType.CONSUMABLE:
						if (_entryType == ItemListEntryType.STORAGE)
						{
							base.transform.SetParent(ItemStorageManager._current._consumableTabContainer);
						}
						else
						{
							base.transform.SetParent(TabMenu._current._itemCell._itemCell_consumableContent);
						}
						break;
					case ItemType.TRADE:
						if (_entryType == ItemListEntryType.STORAGE)
						{
							base.transform.SetParent(ItemStorageManager._current._tradeItemTabContainer);
						}
						else
						{
							base.transform.SetParent(TabMenu._current._itemCell._itemCell_tradeContent);
						}
						break;
					}
				}
				else if (Player._mainPlayer._currentPlayerCondition != PlayerCondition.ACTIVE || Player._mainPlayer._bufferingStatus || (!Input.GetKey(KeyCode.Mouse0) && InputControlManager.current._setGamepadType == GamepadType.Keyboard) || (!GamepadInput.GetButton(GamepadButton.RightTrigger) && InputControlManager.current._setGamepadType != GamepadType.Keyboard && InputControlManager.current._gamepadMouseCursorBuffer <= 0f))
				{
					Reset_HeldItem();
				}
				else
				{
					if (_entryType == ItemListEntryType.STORAGE)
					{
						if (!ItemStorageManager._current._isOpen)
						{
							Reset_HeldItem();
							return;
						}
					}
					else if (!TabMenu._current._isOpen)
					{
						Reset_HeldItem();
						return;
					}
					Set_ItemPositionToCursor();
				}
			}
		}
		void Handle_InventoryRepositioning()
		{
			for (int j = 0; j < Player._mainPlayer._pInventory._heldItems.Count; j++)
			{
				if (_itemData == Player._mainPlayer._pInventory._heldItems[j] && !Player._mainPlayer._pInventory._heldItems[j]._isEquipped && !_itemData._isEquipped)
				{
					_dataID = j;
					for (int k = 0; k < TabMenu._current._itemCell._itemSlots.Length; k++)
					{
						if (!(TabMenu._current._itemCell._itemSlots[k] != _parentItemSlotUI))
						{
							Player._mainPlayer._pInventory._heldItems[j]._slotNumber = k;
							_originSlotNumber = k;
							break;
						}
					}
					break;
				}
			}
		}
		void Handle_SlotRepositioning()
		{
			if (!_isHeldByPointer)
			{
				switch (_entryType)
				{
				case ItemListEntryType.DISPLAY:
					break;
				case ItemListEntryType.STORAGE:
					Handle_StorageRespositioning();
					break;
				case ItemListEntryType.INVENTORY:
					Handle_InventoryRepositioning();
					break;
				}
			}
		}
		void Handle_StorageRespositioning()
		{
			for (int j = 0; j < ItemStorageManager._current._storageItemSlots.Length; j++)
			{
				if (!(ItemStorageManager._current._storageItemSlots[j] != _parentItemSlotUI))
				{
					_itemData._slotNumber = j;
					_originSlotNumber = j;
					break;
				}
			}
		}
		void Handle_TransmogEquipConditions()
		{
			if (TabMenu._current._itemTradeMode)
			{
				_itemIcon.color = Color.white;
				if ((bool)_transmoggedIcon)
				{
					_transmoggedIcon.enabled = false;
				}
			}
			else if (!_transmoggedIcon)
			{
				_itemIcon.color = Color.white;
			}
			else if (TabMenu._current._itemCell._currentEquipCellTab != EquipCellTab.VANITY_PANEL)
			{
				_itemIcon.color = Color.white;
				_transmoggedIcon.enabled = false;
			}
			else
			{
				_currentIllusionStones = Player._mainPlayer._pInventory.Check_ItemQuantity(GameManager._current.Locate_Item("Illusion Stone"));
				if (!CanTransmogItem())
				{
					_itemIcon.color = new Color(1f, 1f, 1f, 0.2f);
					_transmoggedIcon.enabled = false;
				}
				else
				{
					_itemIcon.color = Color.white;
					_transmoggedIcon.enabled = true;
				}
			}
		}
		void Reset_HeldItem()
		{
			TabMenu._current._itemCell._heldItemListDataEntry = null;
			Relocate_ToOriginSlot();
			_isHeldByPointer = false;
		}
		void Set_ItemPositionToCursor()
		{
			bool num = InputControlManager.current._setGamepadType != GamepadType.Keyboard;
			TabMenu._current._itemCell._heldItemListDataEntry = this;
			TabMenu._current._hotkeyBuffer = 0.2f;
			if (num && InputControlManager.current._gamepadMouseCursorBuffer <= 0f)
			{
				_dataEntryTransform.transform.position = TabMenu._current._itemCell._currentHighlightedItemSlot.transform.position;
			}
			else if (Cursor.visible)
			{
				_dataEntryTransform.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
			}
			_dataEntryTransform.SetParent(InGameUI._current._heldListEntryCanvas);
			_dataEntryTransform.localScale = Vector3.one;
		}
	}

	private bool CanTransmogItem()
	{
		if (_currentIllusionStones <= 0)
		{
			return false;
		}
		if (_scriptableItem.IsTransmoggedItem())
		{
			return false;
		}
		if (_scriptableItem._itemType == ItemType.GEAR)
		{
			ScriptableEquipment scriptableEquipment = (ScriptableEquipment)_scriptableItem;
			if (Player._mainPlayer._pStats._currentLevel < scriptableEquipment._equipmentLevel)
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
			return true;
		}
		return false;
	}

	private void OnDestroy()
	{
		if (!_parentItemSlotUI || TabMenu._current._optionsCell._isDisconnecting)
		{
			return;
		}
		if (_isHighlighted)
		{
			if ((bool)ToolTipManager._current._itemToolTip)
			{
				ToolTipManager._current._itemToolTip.Disable_ToolTip();
			}
			if ((bool)ToolTipManager._current._equipToolTip)
			{
				ToolTipManager._current._equipToolTip.Disable_ToolTip();
			}
		}
		_parentItemSlotUI._slotButton.interactable = true;
		_parentItemSlotUI._filledSlotTab[(uint)_itemTypeTab] = false;
		_parentItemSlotUI = null;
	}
}
