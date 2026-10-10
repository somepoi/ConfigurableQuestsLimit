using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Mirror;
using Mirror.RemoteCalls;
using UnityEngine;

public class PlayerInventory : NetworkBehaviour
{
	private Player _player;

	private PlayerEquipment _pEquipment;

	private PlayerQuesting _pQuesting;

	private PlayerVisual _pVisual;

	private PlayerSound _pSound;

	private PlayerStats _pStats;

	private StatusEntity _statusEntity;

	private ChatBehaviour _chatBehaviour;

	private ScriptableItem _findScriptItem;

	[Header("General Parameters")]
	public int _usedEquipmentSlots;

	public int _usedConsumableItemSlots;

	public int _usedTradeItemSlots;

	[Space]
	public MaxInventorySlotStruct _maxInventoryStruct;

	[Header("Runtime Inventory")]
	[SyncVar]
	public int _heldCurrency;

	public List<ItemData> _heldItems = new List<ItemData>();

	public List<ConsumableBuffer> _consumableBuffers = new List<ConsumableBuffer>();

	private float _consumableUseBuffer;

	[Space]
	public List<ItemObject> _droppedItemObjects = new List<ItemObject>();

	[SyncVar]
	public bool _withinItemDropBuffer;

	private float _dropItemBuffer;

	public int Network_heldCurrency
	{
		get
		{
			return _heldCurrency;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _heldCurrency, 1uL, null);
		}
	}

	public bool Network_withinItemDropBuffer
	{
		get
		{
			return _withinItemDropBuffer;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _withinItemDropBuffer, 2uL, null);
		}
	}

	[Command]
	public void Cmd_AddCurrency(int _value)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteInt(_value);
		SendCommandInternal("System.Void PlayerInventory::Cmd_AddCurrency(System.Int32)", 737314632, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[Command]
	public void Cmd_SubtractCurrency(int _value)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteInt(_value);
		SendCommandInternal("System.Void PlayerInventory::Cmd_SubtractCurrency(System.Int32)", 141734835, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[Command]
	public void Cmd_DropCurrency(int _amount)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteInt(_amount);
		SendCommandInternal("System.Void PlayerInventory::Cmd_DropCurrency(System.Int32)", 155004750, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ServerCallback]
	public void Add_Currency(int _value)
	{
		if (NetworkServer.active && _value > 0 && _heldCurrency < GameManager._current._statLogics._maxCurrency)
		{
			int num = _heldCurrency + _value;
			if (num >= GameManager._current._statLogics._maxCurrency)
			{
				num = GameManager._current._statLogics._maxCurrency;
			}
			string message = $"Picked up {GameManager._current._statLogics._currencyName}. (+{_value})";
			_chatBehaviour.Target_GameLogicMessage(message);
			Network_heldCurrency = num;
		}
	}

	[ServerCallback]
	public void Subtract_Currency(int _value)
	{
		if (!NetworkServer.active || _value <= 0)
		{
			return;
		}
		if (_heldCurrency <= 0)
		{
			Network_heldCurrency = 0;
			return;
		}
		int num = _heldCurrency - _value;
		if (num <= 0)
		{
			num = 0;
		}
		Network_heldCurrency = num;
	}

	private void Awake()
	{
		_player = GetComponent<Player>();
		_pEquipment = GetComponent<PlayerEquipment>();
		_pQuesting = GetComponent<PlayerQuesting>();
		_pVisual = GetComponent<PlayerVisual>();
		_pSound = GetComponent<PlayerSound>();
		_pStats = GetComponent<PlayerStats>();
		_statusEntity = GetComponent<StatusEntity>();
		_chatBehaviour = GetComponent<ChatBehaviour>();
	}

	public override void OnStartAuthority()
	{
		base.OnStartAuthority();
		StartCoroutine(Init_InventoryLoad());
	}

	private IEnumerator Init_InventoryLoad()
	{
		yield return new WaitForSeconds(3.5f);
		Load_InventoryProfile();
	}

	private void Update()
	{
		Global_BufferParams();
		Server_BufferParams();
		Server_DroppedItemObjectCache();
	}

	private void Global_BufferParams()
	{
		Handle_ConsumableBufferParams();
		void Handle_ConsumableBufferParams()
		{
			if (_consumableUseBuffer > 0f)
			{
				_consumableUseBuffer -= Time.deltaTime;
			}
			else
			{
				_consumableUseBuffer = 0f;
			}
			if (base.isLocalPlayer && _consumableBuffers.Count > 0)
			{
				for (int i = 0; i < _consumableBuffers.Count; i++)
				{
					if (!(_consumableBuffers[i]._bufferTimer > 0f))
					{
						_consumableBuffers.RemoveAt(i);
						break;
					}
					_consumableBuffers[i]._bufferTimer -= Time.deltaTime;
				}
			}
		}
	}

	[ServerCallback]
	private void Server_BufferParams()
	{
		if (NetworkServer.active)
		{
			if (_heldCurrency > GameManager._current._statLogics._maxCurrency)
			{
				Network_heldCurrency = GameManager._current._statLogics._maxCurrency;
			}
			Network_withinItemDropBuffer = _dropItemBuffer > 0f;
			if (_dropItemBuffer > 0f)
			{
				_dropItemBuffer -= Time.deltaTime;
			}
			else
			{
				_dropItemBuffer = 0f;
			}
		}
	}

	[ClientCallback]
	public void Apply_ItemIteration()
	{
		if (!NetworkClient.active || !base.isLocalPlayer)
		{
			return;
		}
		if (_usedEquipmentSlots < 0)
		{
			_usedEquipmentSlots = 0;
		}
		if (_usedConsumableItemSlots < 0)
		{
			_usedConsumableItemSlots = 0;
		}
		if (_usedTradeItemSlots < 0)
		{
			_usedTradeItemSlots = 0;
		}
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		for (int i = 0; i < _heldItems.Count; i++)
		{
			ScriptableItem scriptableItem = GameManager._current.Locate_Item(_heldItems[i]._itemName);
			switch (scriptableItem._itemType)
			{
			case ItemType.GEAR:
				if (!_heldItems[i]._isEquipped)
				{
					num++;
				}
				if (_heldItems[i]._modifierID > 0 && !((ScriptableEquipment)scriptableItem)._statModifierTable)
				{
					_heldItems[i]._modifierID = 0;
				}
				break;
			case ItemType.CONSUMABLE:
				num2++;
				_heldItems[i]._modifierID = 0;
				if (_heldItems[i]._maxQuantity < scriptableItem._maxStackAmount)
				{
					_heldItems[i]._maxQuantity = scriptableItem._maxStackAmount;
				}
				break;
			case ItemType.TRADE:
				num3++;
				_heldItems[i]._modifierID = 0;
				if (_heldItems[i]._maxQuantity < scriptableItem._maxStackAmount)
				{
					_heldItems[i]._maxQuantity = scriptableItem._maxStackAmount;
				}
				break;
			}
			if ((bool)scriptableItem._scriptableQuest && !_player._bufferingStatus && !_pQuesting.Check_HasQuest(scriptableItem._scriptableQuest))
			{
				Remove_Item(_heldItems[i], 0);
			}
		}
		_usedEquipmentSlots = num;
		_usedConsumableItemSlots = num2;
		_usedTradeItemSlots = num3;
	}

	[ServerCallback]
	private void Server_DroppedItemObjectCache()
	{
		if (!NetworkServer.active || _droppedItemObjects.Count <= 0)
		{
			return;
		}
		if (_droppedItemObjects.Count >= GameManager._current._statLogics._maxNetDroppedNetItems)
		{
			_droppedItemObjects[0].UnspawnObject();
			_droppedItemObjects.RemoveAt(0);
		}
		for (int i = 0; i < _droppedItemObjects.Count; i++)
		{
			if (_droppedItemObjects[i] == null)
			{
				_droppedItemObjects.RemoveAt(i);
				break;
			}
		}
	}

	[ClientCallback]
	public void Load_InventoryProfile()
	{
		if (NetworkClient.active)
		{
			_maxInventoryStruct = GameManager._current._statLogics._maxInventorySlotStruct;
			ItemData[] inventoryProfile = ProfileDataManager._current._characterFile._inventoryProfile;
			for (int i = 0; i < inventoryProfile.Length; i++)
			{
				Iterate_ItemData(inventoryProfile[i]);
			}
			TabMenu._current._itemCell.Create_AllItemListDataEntries();
			Apply_ItemIteration();
		}
		void Iterate_ItemData(ItemData _itemData)
		{
			ScriptableItem scriptableItem = GameManager._current.Locate_Item(_itemData._itemName);
			if ((bool)scriptableItem)
			{
				int modifierID = _itemData._modifierID;
				int num = _itemData._slotNumber;
				int num2 = _itemData._quantity;
				bool isEquipped = _itemData._isEquipped;
				bool isAltWeapon = _itemData._isAltWeapon;
				int maxQuantity;
				if (scriptableItem._itemType == ItemType.GEAR)
				{
					num2 = 1;
					maxQuantity = 1;
				}
				else
				{
					if (num2 > scriptableItem._maxStackAmount)
					{
						num2 = scriptableItem._maxStackAmount;
					}
					maxQuantity = scriptableItem._maxStackAmount;
					isEquipped = false;
					isAltWeapon = false;
				}
				if (num < 0)
				{
					num = 0;
				}
				else if (num > GameManager._current._statLogics._maxInventorySlotStruct._maxConsumableItemSlots)
				{
					num = GameManager._current._statLogics._maxInventorySlotStruct._maxConsumableItemSlots;
				}
				ItemData itemData = new ItemData
				{
					_itemName = _itemData._itemName,
					_quantity = num2,
					_maxQuantity = maxQuantity,
					_isEquipped = isEquipped,
					_isAltWeapon = isAltWeapon,
					_slotNumber = num,
					_modifierID = modifierID,
					_damageTypeOverride = _itemData._damageTypeOverride,
					_useDamageTypeOverride = _itemData._useDamageTypeOverride
				};
				_heldItems.Add(itemData);
				if (itemData._isEquipped)
				{
					_pEquipment.Init_EquipItem(itemData);
				}
			}
		}
	}

	public bool Check_InventoryFull(ScriptableItem _scriptableItem, int _quantity)
	{
		switch (_scriptableItem._itemType)
		{
		case ItemType.GEAR:
			if (_usedEquipmentSlots >= _maxInventoryStruct._maxEquipmentSlots)
			{
				ErrorPromptTextManager.current.Init_ErrorPrompt("Equipment Inventory Full");
				return true;
			}
			if (_usedEquipmentSlots + _quantity > _maxInventoryStruct._maxEquipmentSlots)
			{
				ErrorPromptTextManager.current.Init_ErrorPrompt("Equipment Inventory Full");
				return true;
			}
			break;
		case ItemType.CONSUMABLE:
		{
			if (_usedConsumableItemSlots < _maxInventoryStruct._maxConsumableItemSlots)
			{
				break;
			}
			for (int j = 0; j < _heldItems.Count; j++)
			{
				if (_heldItems[j]._itemName == _scriptableItem._itemName && _heldItems[j]._quantity + _quantity <= _heldItems[j]._maxQuantity)
				{
					return false;
				}
			}
			ErrorPromptTextManager.current.Init_ErrorPrompt("Consumable Inventory Full");
			return true;
		}
		case ItemType.TRADE:
		{
			if (_usedTradeItemSlots < _maxInventoryStruct._maxTradeItemSlots)
			{
				break;
			}
			for (int i = 0; i < _heldItems.Count; i++)
			{
				if (_heldItems[i]._itemName == _scriptableItem._itemName && _heldItems[i]._quantity + _quantity <= _heldItems[i]._maxQuantity)
				{
					return false;
				}
			}
			ErrorPromptTextManager.current.Init_ErrorPrompt("Trade Item Inventory Full");
			return true;
		}
		}
		return false;
	}

	public int Check_ItemQuantity(ScriptableItem _scriptableItem)
	{
		int num = 0;
		for (int i = 0; i < _heldItems.Count; i++)
		{
			if (_heldItems[i]._itemName == _scriptableItem._itemName)
			{
				num += _heldItems[i]._quantity;
			}
		}
		return num;
	}

	public ItemData Fetch_HeldItem(ScriptableItem _scriptableItem)
	{
		for (int i = 0; i < _heldItems.Count; i++)
		{
			if (_heldItems[i]._itemName == _scriptableItem._itemName)
			{
				return _heldItems[i];
			}
		}
		return null;
	}

	[Command]
	public void Cmd_PurchaseBuybackItem(NetNPC _shopkeepNPC, int _quantity, string _key)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteNetworkBehaviour(_shopkeepNPC);
		writer.WriteInt(_quantity);
		writer.WriteString(_key);
		SendCommandInternal("System.Void PlayerInventory::Cmd_PurchaseBuybackItem(NetNPC,System.Int32,System.String)", 1353382387, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientCallback]
	public void Local_PurchaseItem(NetNPC _shopkeepNPC, int _quantity, string _key)
	{
		if (NetworkClient.active)
		{
			Init_PurchaseItem(_shopkeepNPC, _shopkeepNPC._vendorItems[_key], _quantity, _key);
		}
	}

	public void Init_PurchaseItem(NetNPC _npc, ShopkeepItemStruct _shopItemStruct, int _quantity, string _key)
	{
		if (_shopItemStruct._isbuybackItem && !base.isServer)
		{
			return;
		}
		ScriptableItem scriptableItem = GameManager._current.Locate_Item(_shopItemStruct._itemName);
		ScriptableItem scriptableItem2 = null;
		string text = GameManager._current._statLogics._currencyName;
		int num = scriptableItem._vendorCost * _quantity;
		int num2 = _heldCurrency;
		bool flag = false;
		if (_shopItemStruct._useDedicatedValue)
		{
			num = _shopItemStruct._dedicatedValue * _quantity;
		}
		if (_shopItemStruct._isGambleItem)
		{
			num = _shopItemStruct._gambleValue * _quantity;
		}
		if (!string.IsNullOrWhiteSpace(_shopItemStruct._specialItemCostName))
		{
			text = _shopItemStruct._specialItemCostName;
			scriptableItem2 = GameManager._current.Locate_Item(_shopItemStruct._specialItemCostName);
			if ((bool)scriptableItem2)
			{
				num = _shopItemStruct._specialItemCostQuantity * _quantity;
				num2 = Check_ItemQuantity(scriptableItem2);
				flag = true;
			}
		}
		if (num <= 0)
		{
			num = 0;
		}
		if (num2 < num)
		{
			if (base.isServer)
			{
				_chatBehaviour.Target_GameLogicMessage("Not enough " + text + ".");
			}
			else
			{
				_chatBehaviour.Init_GameLogicMessage("Not enough " + text + ".");
			}
			return;
		}
		if (flag)
		{
			Remove_Item(Fetch_HeldItem(scriptableItem2), num);
		}
		else if (base.isServer)
		{
			Network_heldCurrency = _heldCurrency - num;
		}
		else
		{
			Cmd_SubtractCurrency(num);
		}
		string message = $"Purchased {scriptableItem._itemName} (x{_quantity}).";
		if (base.isServer)
		{
			_chatBehaviour.Target_GameLogicMessage(message);
		}
		else
		{
			_chatBehaviour.Init_GameLogicMessage(message);
		}
		int stockQuantity = _shopItemStruct._stockQuantity - _quantity;
		ShopkeepItemStruct value = new ShopkeepItemStruct
		{
			_itemName = _shopItemStruct._itemName,
			_itemData = _shopItemStruct._itemData,
			_equipModifierID = _shopItemStruct._equipModifierID,
			_stockQuantity = stockQuantity,
			_isbuybackItem = _shopItemStruct._isbuybackItem,
			_removeAtEmptyStock = _shopItemStruct._removeAtEmptyStock
		};
		ItemData itemData = new ItemData
		{
			_itemName = _shopItemStruct._itemName,
			_quantity = _quantity,
			_maxQuantity = scriptableItem._maxStackAmount,
			_modifierID = _shopItemStruct._equipModifierID,
			_isEquipped = false,
			_isAltWeapon = false,
			_slotNumber = 0
		};
		if (value._isbuybackItem && base.isServer)
		{
			_npc._syncBuybackItems[_key] = value;
			Target_AddItem(itemData);
		}
		if (!value._isbuybackItem)
		{
			Add_Item(itemData, _autoStack: true);
		}
		if (_shopItemStruct._isGambleItem)
		{
			_chatBehaviour.Client_RecieveTriggerMessage("You got: " + _key + "!");
			ShopkeepManager._current.Delete_ShopkeepListing(_key);
		}
		Play_PurchaseSound();
	}

	[Command]
	public void Cmd_SellItem(NetworkIdentity _shopkeepNPC, ItemData _itemData, int _quantity)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteNetworkIdentity(_shopkeepNPC);
		GeneratedNetworkCode._Write_ItemData(writer, _itemData);
		writer.WriteInt(_quantity);
		SendCommandInternal("System.Void PlayerInventory::Cmd_SellItem(Mirror.NetworkIdentity,ItemData,System.Int32)", -1581322829, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientCallback]
	public void Play_PurchaseSound()
	{
		if (NetworkClient.active && (bool)Player._mainPlayer && (!Player._mainPlayer || !(Player._mainPlayer.Network_playerMapInstance != _player.Network_playerMapInstance)))
		{
			_pSound._aSrcGeneral.PlayOneShot(_pSound._purchaseItemSound, 0.25f);
		}
	}

	[ClientRpc]
	private void Rpc_PlaySellEffect()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendRPCInternal("System.Void PlayerInventory::Rpc_PlaySellEffect()", 1155361065, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	[Command]
	public void Cmd_DropItem(ItemData _itemData, int _quantity)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		GeneratedNetworkCode._Write_ItemData(writer, _itemData);
		writer.WriteInt(_quantity);
		SendCommandInternal("System.Void PlayerInventory::Cmd_DropItem(ItemData,System.Int32)", -145871243, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	public void Target_RemoveItem(ItemData _itemData, int _quantity)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		GeneratedNetworkCode._Write_ItemData(writer, _itemData);
		writer.WriteInt(_quantity);
		SendTargetRPCInternal(null, "System.Void PlayerInventory::Target_RemoveItem(ItemData,System.Int32)", -904413019, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientCallback]
	public void Remove_Item(ItemData _itemData, int _quantity)
	{
		if (!NetworkClient.active || _heldItems.Count <= 0 || _itemData == null || !base.isLocalPlayer)
		{
			return;
		}
		_findScriptItem = GameManager._current.Locate_Item(_itemData._itemName);
		if ((bool)_findScriptItem)
		{
			int quantity = _itemData._quantity;
			int num = 0;
			if (_quantity <= 0)
			{
				_quantity = 0;
			}
			if (_itemData._quantity - _quantity < 0)
			{
				num = quantity - _quantity;
			}
			if (_quantity == 0)
			{
				_heldItems.Remove(_itemData);
				TabMenu._current._itemCell.Destroy_ItemCells_SingleListData(_itemData);
			}
			else
			{
				_itemData._quantity -= _quantity;
			}
			if (_itemData._quantity <= 0)
			{
				_heldItems.Remove(_itemData);
				TabMenu._current._itemCell.Destroy_ItemCells_SingleListData(_itemData);
			}
			if (quantity < _quantity)
			{
				Remove_Item(Fetch_HeldItem(_findScriptItem), num * -1);
			}
			_pQuesting.Apply_QuestItemProgress(_findScriptItem);
			if (!_player._bufferingStatus)
			{
				ProfileDataManager._current.Save_ProfileData();
			}
			Apply_ItemIteration();
		}
	}

	[TargetRpc]
	public void Target_AddItem(ItemData _itemData)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		GeneratedNetworkCode._Write_ItemData(writer, _itemData);
		SendTargetRPCInternal(null, "System.Void PlayerInventory::Target_AddItem(ItemData)", -1678644351, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientCallback]
	public void Add_Item(ItemData _itemData, bool _autoStack)
	{
		if (!NetworkClient.active)
		{
			return;
		}
		if (!base.isLocalPlayer)
		{
			return;
		}
		_findScriptItem = GameManager._current.Locate_Item(_itemData._itemName);
		int _quantity;
		ItemData _setItemData;
		if ((bool)_findScriptItem)
		{
			_quantity = _itemData._quantity;
			if (_quantity <= 0)
			{
				_quantity = 1;
			}
			if (_quantity > _findScriptItem._maxStackAmount)
			{
				_quantity = _findScriptItem._maxStackAmount;
			}
			string message = $"Picked up {_findScriptItem._itemName}. (+{_quantity})";
			if (_findScriptItem._itemRarity == ItemRarity.Cosmetic)
			{
				SteamAchievementManager.current.Init_EarnAchievement("ATLYSS_ACHIEVEMENT_08");
			}
			if (_findScriptItem._itemName == "Fisher Overalls")
			{
				SteamAchievementManager.current.Init_EarnAchievement("ATLYSS_ACHIEVEMENT_15");
			}
			switch (_findScriptItem._itemType)
			{
			case ItemType.GEAR:
				Init_AddGearItem();
				break;
			case ItemType.CONSUMABLE:
				Init_AddStackItem();
				break;
			case ItemType.TRADE:
				Init_AddStackItem();
				break;
			}
			_chatBehaviour.Init_GameLogicMessage(message);
			_pQuesting.Apply_QuestItemProgress(_findScriptItem);
			_findScriptItem = null;
			if (!_player._bufferingStatus)
			{
				ProfileDataManager._current.Save_ProfileData();
			}
			Apply_ItemIteration();
		}
		void Init_AddGearItem()
		{
			_setItemData = new ItemData
			{
				_itemName = _findScriptItem._itemName,
				_quantity = 1,
				_maxQuantity = 1,
				_isEquipped = false,
				_isAltWeapon = false,
				_modifierID = _itemData._modifierID,
				_slotNumber = _itemData._slotNumber,
				_useDamageTypeOverride = _itemData._useDamageTypeOverride,
				_damageTypeOverride = _itemData._damageTypeOverride
			};
			_heldItems.Add(_setItemData);
			TabMenu._current._itemCell.Create_ItemListData(_setItemData);
		}
		void Init_AddStackItem()
		{
			for (int i = 0; i < _heldItems.Count; i++)
			{
				if (!_autoStack)
				{
					break;
				}
				if (!(_heldItems[i]._itemName != _findScriptItem._itemName) && _heldItems[i]._quantity + _quantity < _heldItems[i]._maxQuantity + 1)
				{
					_heldItems[i]._quantity = _heldItems[i]._quantity + _quantity;
					return;
				}
			}
			_setItemData = new ItemData
			{
				_itemName = _findScriptItem._itemName,
				_quantity = _quantity,
				_maxQuantity = _findScriptItem._maxStackAmount,
				_isEquipped = false,
				_modifierID = 0,
				_slotNumber = _itemData._slotNumber
			};
			_heldItems.Add(_setItemData);
			TabMenu._current._itemCell.Create_ItemListData(_setItemData);
		}
	}

	[ClientCallback]
	public void Init_UseConsumable(ScriptableConsumable _scriptConsumable, ItemData _itemData)
	{
		if (!NetworkClient.active || _consumableUseBuffer > 0f)
		{
			return;
		}
		for (int i = 0; i < _consumableBuffers.Count; i++)
		{
			if (_scriptConsumable == _consumableBuffers[i]._scriptableConsumable)
			{
				ErrorPromptTextManager.current.Init_ErrorPrompt("Consumable On Cooldown");
				return;
			}
		}
		if (_player._currentPlayerCondition == PlayerCondition.DEAD)
		{
			ErrorPromptTextManager.current.Init_ErrorPrompt("You're Dead");
		}
		else if (_player._playerZoneType != ZoneType.Safe && _scriptConsumable._useOnlyInTown)
		{
			ErrorPromptTextManager.current.Init_ErrorPrompt("Safe Zone Only");
		}
		else if (_player._playerZoneType == ZoneType.Arena)
		{
			ErrorPromptTextManager.current.Init_ErrorPrompt("Cannot use here");
		}
		else if (_pStats._currentLevel < _scriptConsumable._levelRequirement)
		{
			ErrorPromptTextManager.current.Init_ErrorPrompt($"Low Level (Req: Lv-{_scriptConsumable._levelRequirement})");
		}
		else if ((bool)_scriptConsumable._baseClassRequirement && _pStats._class != _scriptConsumable._baseClassRequirement)
		{
			ErrorPromptTextManager.current.Init_ErrorPrompt("Invalid Class");
		}
		else if (_scriptConsumable.CanUseConsumable(_statusEntity, _isButtonPrompt: true))
		{
			_scriptConsumable.Use_Consumable(_statusEntity);
			Cmd_UseConsumable(_itemData);
			if (!_scriptConsumable._unperishable)
			{
				Init_RemoveConsumableItemData(_itemData);
			}
			float num = _scriptConsumable._consumableCooldown;
			if (num <= 0.12f)
			{
				num = 0.12f;
			}
			ConsumableBuffer item = new ConsumableBuffer
			{
				_scriptableConsumable = _scriptConsumable,
				_bufferTimer = num
			};
			_consumableBuffers.Add(item);
		}
	}

	[ClientCallback]
	private void Init_RemoveConsumableItemData(ItemData _itemData)
	{
		if (NetworkClient.active)
		{
			int num = _itemData._quantity - 1;
			if (num <= 0)
			{
				Remove_Item(_itemData, 0);
			}
			else
			{
				_itemData._quantity = num;
			}
			_consumableUseBuffer = 0.35f;
		}
	}

	[Command]
	public void Cmd_UseConsumable(ItemData _itemData)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		GeneratedNetworkCode._Write_ItemData(writer, _itemData);
		SendCommandInternal("System.Void PlayerInventory::Cmd_UseConsumable(ItemData)", 76825874, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	private void Rpc_Init_ConsumableObject(Player _setPlayer, string _consumableTag)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteNetworkBehaviour(_setPlayer);
		writer.WriteString(_consumableTag);
		SendRPCInternal("System.Void PlayerInventory::Rpc_Init_ConsumableObject(Player,System.String)", 241635804, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	private void Spawn_ConsumableObject(Player _setPlayer, string _consumableTag)
	{
		if ((bool)Player._mainPlayer && _setPlayer.Network_playerMapInstance != Player._mainPlayer.Network_playerMapInstance)
		{
			return;
		}
		ScriptableItem scriptableItem = GameManager._current.Locate_Item(_consumableTag);
		if ((bool)scriptableItem && scriptableItem._itemType == ItemType.CONSUMABLE)
		{
			ScriptableConsumable scriptableConsumable = (ScriptableConsumable)scriptableItem;
			GameObject gameObject = PoolManager._current.GenerateObject(scriptableConsumable._consumableObject, "", 2.75f);
			if ((bool)gameObject.GetComponent<ConditionObject>())
			{
				gameObject.GetComponent<ConditionObject>()._statusEntity = _statusEntity;
			}
			gameObject.transform.SetParent(_setPlayer.transform);
			gameObject.transform.SetPositionAndRotation(_setPlayer.transform.position, Quaternion.identity);
			gameObject.transform.SetAsLastSibling();
			if ((bool)gameObject.GetComponentInChildren<CombatCollider>())
			{
				gameObject.GetComponentInChildren<CombatCollider>()._parentObject = base.gameObject;
				gameObject.transform.SetParent(null);
			}
		}
	}

	public override bool Weaved()
	{
		return true;
	}

	protected void UserCode_Cmd_AddCurrency__Int32(int _value)
	{
		if (_value > 0 && _heldCurrency < GameManager._current._statLogics._maxCurrency)
		{
			Add_Currency(_value);
		}
	}

	protected static void InvokeUserCode_Cmd_AddCurrency__Int32(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command Cmd_AddCurrency called on client.");
		}
		else
		{
			((PlayerInventory)obj).UserCode_Cmd_AddCurrency__Int32(reader.ReadInt());
		}
	}

	protected void UserCode_Cmd_SubtractCurrency__Int32(int _value)
	{
		if (_value > 0)
		{
			if (_heldCurrency <= 0)
			{
				Network_heldCurrency = 0;
			}
			else
			{
				Subtract_Currency(_value);
			}
		}
	}

	protected static void InvokeUserCode_Cmd_SubtractCurrency__Int32(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command Cmd_SubtractCurrency called on client.");
		}
		else
		{
			((PlayerInventory)obj).UserCode_Cmd_SubtractCurrency__Int32(reader.ReadInt());
		}
	}

	protected void UserCode_Cmd_DropCurrency__Int32(int _amount)
	{
		if (!_withinItemDropBuffer && _heldCurrency > 0 && _amount > 0)
		{
			if (_amount > _heldCurrency)
			{
				_amount = _heldCurrency;
			}
			Network_heldCurrency = _heldCurrency - _amount;
			ItemData itemData = new ItemData();
			GameManager._current.Server_SpawnNetItemObject(base.gameObject, itemData, null, base.transform.position, _amount, base.gameObject.scene);
			string message = $"Dropped {_amount} {GameManager._current._statLogics._currencyName}.";
			_chatBehaviour.Target_GameLogicMessage(message);
			_dropItemBuffer = 0.8f;
		}
	}

	protected static void InvokeUserCode_Cmd_DropCurrency__Int32(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command Cmd_DropCurrency called on client.");
		}
		else
		{
			((PlayerInventory)obj).UserCode_Cmd_DropCurrency__Int32(reader.ReadInt());
		}
	}

	protected void UserCode_Cmd_PurchaseBuybackItem__NetNPC__Int32__String(NetNPC _shopkeepNPC, int _quantity, string _key)
	{
		Init_PurchaseItem(_shopkeepNPC, _shopkeepNPC._syncBuybackItems[_key], _quantity, _key);
	}

	protected static void InvokeUserCode_Cmd_PurchaseBuybackItem__NetNPC__Int32__String(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command Cmd_PurchaseBuybackItem called on client.");
		}
		else
		{
			((PlayerInventory)obj).UserCode_Cmd_PurchaseBuybackItem__NetNPC__Int32__String(reader.ReadNetworkBehaviour<NetNPC>(), reader.ReadInt(), reader.ReadString());
		}
	}

	protected void UserCode_Cmd_SellItem__NetworkIdentity__ItemData__Int32(NetworkIdentity _shopkeepNPC, ItemData _itemData, int _quantity)
	{
		NetNPC component = _shopkeepNPC.GetComponent<NetNPC>();
		ScriptableItem scriptableItem = GameManager._current.Locate_Item(_itemData._itemName);
		if (!scriptableItem)
		{
			return;
		}
		if (_quantity > _itemData._quantity)
		{
			_quantity = _itemData._quantity;
		}
		int num = _quantity;
		int num2 = 0;
		string text = scriptableItem._itemName;
		int num3 = 0;
		int num4 = (int)((float)scriptableItem._vendorCost * GameManager._current._statLogics._vendorBuybackPercent);
		if (num4 <= 0)
		{
			num4 = 1;
		}
		_chatBehaviour.Target_GameLogicMessage($"Sold {scriptableItem._itemName} (x{_quantity}).");
		Network_heldCurrency = _heldCurrency + num4 * _quantity;
		if (scriptableItem._itemType == ItemType.GEAR && _itemData._modifierID > 0)
		{
			ScriptableEquipment scriptableEquipment = (ScriptableEquipment)scriptableItem;
			ScriptableStatModifier scriptableStatModifier = GameManager._current.Locate_StatModifier(_itemData._modifierID);
			if ((bool)scriptableStatModifier)
			{
				text = scriptableStatModifier._modifierTag + " " + scriptableEquipment._itemName;
				num3 = scriptableStatModifier._modifierID;
			}
		}
		foreach (KeyValuePair<string, ShopkeepItemStruct> syncBuybackItem in component._syncBuybackItems)
		{
			if (component._syncBuybackItems[syncBuybackItem.Key]._itemName == text && component._syncBuybackItems[syncBuybackItem.Key]._equipModifierID == num3)
			{
				num2 = component._syncBuybackItems[syncBuybackItem.Key]._stockQuantity;
				break;
			}
		}
		num += num2;
		ShopkeepItemStruct value = new ShopkeepItemStruct
		{
			_itemName = _itemData._itemName,
			_stockQuantity = num,
			_equipModifierID = _itemData._modifierID,
			_isbuybackItem = true,
			_itemData = _itemData,
			_removeAtEmptyStock = true
		};
		Rpc_PlaySellEffect();
		if (component._syncBuybackItems.ContainsKey(text))
		{
			component._syncBuybackItems[text] = value;
		}
		else if (component._buybackItems < 32)
		{
			component._syncBuybackItems.Add(text, value);
			component.Init_ShopResetTimer();
		}
	}

	protected static void InvokeUserCode_Cmd_SellItem__NetworkIdentity__ItemData__Int32(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command Cmd_SellItem called on client.");
		}
		else
		{
			((PlayerInventory)obj).UserCode_Cmd_SellItem__NetworkIdentity__ItemData__Int32(reader.ReadNetworkIdentity(), GeneratedNetworkCode._Read_ItemData(reader), reader.ReadInt());
		}
	}

	protected void UserCode_Rpc_PlaySellEffect()
	{
		if ((bool)Player._mainPlayer && (!Player._mainPlayer || !(Player._mainPlayer.Network_playerMapInstance != _player.Network_playerMapInstance)))
		{
			_pVisual._coinSellEffect.Play();
			_pVisual._coinSellEffect.GetComponent<AudioSource>().Play();
		}
	}

	protected static void InvokeUserCode_Rpc_PlaySellEffect(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC Rpc_PlaySellEffect called on server.");
		}
		else
		{
			((PlayerInventory)obj).UserCode_Rpc_PlaySellEffect();
		}
	}

	protected void UserCode_Cmd_DropItem__ItemData__Int32(ItemData _itemData, int _quantity)
	{
		if (_withinItemDropBuffer)
		{
			return;
		}
		if (_quantity <= 0)
		{
			_quantity = _itemData._quantity;
		}
		_player.Add_PacketCount();
		ScriptableItem scriptableItem = GameManager._current.Locate_Item(_itemData._itemName);
		if (!scriptableItem)
		{
			Init_VanishMessage();
			return;
		}
		if (scriptableItem._destroyOnDrop)
		{
			Init_VanishMessage();
			return;
		}
		if (_itemData._modifierID > 0 && !GameManager._current.Locate_StatModifier(_itemData._modifierID))
		{
			_itemData._modifierID = 0;
		}
		if (_quantity > scriptableItem._maxStackAmount)
		{
			_quantity = scriptableItem._maxStackAmount;
		}
		int modifierID;
		if (scriptableItem._itemType == ItemType.GEAR)
		{
			modifierID = _itemData._modifierID;
			if ((int)scriptableItem._itemRarity >= 2 && _itemData._modifierID > 0)
			{
				modifierID = 0;
			}
		}
		else
		{
			modifierID = 0;
		}
		ItemData itemData = new ItemData
		{
			_itemName = _itemData._itemName,
			_maxQuantity = _itemData._maxQuantity,
			_quantity = _quantity,
			_isEquipped = false,
			_isAltWeapon = false,
			_modifierID = modifierID,
			_slotNumber = 0,
			_useDamageTypeOverride = _itemData._useDamageTypeOverride,
			_damageTypeOverride = _itemData._damageTypeOverride
		};
		GameManager._current.Server_SpawnNetItemObject(base.gameObject, itemData, null, base.transform.position, 0, base.gameObject.scene);
		string message = $"Dropped {itemData._itemName}. (-{_quantity})";
		_chatBehaviour.Target_GameLogicMessage(message);
		_dropItemBuffer = 0.4f;
		void Init_VanishMessage()
		{
			_player.Add_PacketCount();
			_pVisual.Rpc_PoofSmokeEffect();
			_chatBehaviour.Target_RecieveMessage("The " + _itemData._itemName + " vanishes.");
		}
	}

	protected static void InvokeUserCode_Cmd_DropItem__ItemData__Int32(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command Cmd_DropItem called on client.");
		}
		else
		{
			((PlayerInventory)obj).UserCode_Cmd_DropItem__ItemData__Int32(GeneratedNetworkCode._Read_ItemData(reader), reader.ReadInt());
		}
	}

	protected void UserCode_Target_RemoveItem__ItemData__Int32(ItemData _itemData, int _quantity)
	{
		Remove_Item(_itemData, _quantity);
	}

	protected static void InvokeUserCode_Target_RemoveItem__ItemData__Int32(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC Target_RemoveItem called on server.");
		}
		else
		{
			((PlayerInventory)obj).UserCode_Target_RemoveItem__ItemData__Int32(GeneratedNetworkCode._Read_ItemData(reader), reader.ReadInt());
		}
	}

	protected void UserCode_Target_AddItem__ItemData(ItemData _itemData)
	{
		Add_Item(_itemData, _autoStack: true);
	}

	protected static void InvokeUserCode_Target_AddItem__ItemData(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC Target_AddItem called on server.");
		}
		else
		{
			((PlayerInventory)obj).UserCode_Target_AddItem__ItemData(GeneratedNetworkCode._Read_ItemData(reader));
		}
	}

	protected void UserCode_Cmd_UseConsumable__ItemData(ItemData _itemData)
	{
		if (_player._currentPlayerCondition != PlayerCondition.ACTIVE || _player._playerZoneType == ZoneType.Arena || _consumableUseBuffer > 0f)
		{
			return;
		}
		ScriptableItem scriptableItem = GameManager._current.Locate_Item(_itemData._itemName);
		if (!scriptableItem)
		{
			return;
		}
		ScriptableConsumable scriptableConsumable = (ScriptableConsumable)scriptableItem;
		if (_pStats._currentLevel >= scriptableConsumable._levelRequirement)
		{
			if (!base.isLocalPlayer)
			{
				scriptableConsumable.Use_Consumable(_statusEntity);
			}
			if ((bool)scriptableConsumable._consumableObject)
			{
				Spawn_ConsumableObject(_player, scriptableConsumable._itemName);
				Rpc_Init_ConsumableObject(_player, scriptableConsumable._itemName);
			}
			_consumableUseBuffer = 0.4f;
		}
	}

	protected static void InvokeUserCode_Cmd_UseConsumable__ItemData(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command Cmd_UseConsumable called on client.");
		}
		else
		{
			((PlayerInventory)obj).UserCode_Cmd_UseConsumable__ItemData(GeneratedNetworkCode._Read_ItemData(reader));
		}
	}

	protected void UserCode_Rpc_Init_ConsumableObject__Player__String(Player _setPlayer, string _consumableTag)
	{
		if (!base.isServer)
		{
			Spawn_ConsumableObject(_setPlayer, _consumableTag);
		}
	}

	protected static void InvokeUserCode_Rpc_Init_ConsumableObject__Player__String(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC Rpc_Init_ConsumableObject called on server.");
		}
		else
		{
			((PlayerInventory)obj).UserCode_Rpc_Init_ConsumableObject__Player__String(reader.ReadNetworkBehaviour<Player>(), reader.ReadString());
		}
	}

	static PlayerInventory()
	{
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerInventory), "System.Void PlayerInventory::Cmd_AddCurrency(System.Int32)", InvokeUserCode_Cmd_AddCurrency__Int32, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerInventory), "System.Void PlayerInventory::Cmd_SubtractCurrency(System.Int32)", InvokeUserCode_Cmd_SubtractCurrency__Int32, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerInventory), "System.Void PlayerInventory::Cmd_DropCurrency(System.Int32)", InvokeUserCode_Cmd_DropCurrency__Int32, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerInventory), "System.Void PlayerInventory::Cmd_PurchaseBuybackItem(NetNPC,System.Int32,System.String)", InvokeUserCode_Cmd_PurchaseBuybackItem__NetNPC__Int32__String, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerInventory), "System.Void PlayerInventory::Cmd_SellItem(Mirror.NetworkIdentity,ItemData,System.Int32)", InvokeUserCode_Cmd_SellItem__NetworkIdentity__ItemData__Int32, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerInventory), "System.Void PlayerInventory::Cmd_DropItem(ItemData,System.Int32)", InvokeUserCode_Cmd_DropItem__ItemData__Int32, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerInventory), "System.Void PlayerInventory::Cmd_UseConsumable(ItemData)", InvokeUserCode_Cmd_UseConsumable__ItemData, requiresAuthority: true);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerInventory), "System.Void PlayerInventory::Rpc_PlaySellEffect()", InvokeUserCode_Rpc_PlaySellEffect);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerInventory), "System.Void PlayerInventory::Rpc_Init_ConsumableObject(Player,System.String)", InvokeUserCode_Rpc_Init_ConsumableObject__Player__String);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerInventory), "System.Void PlayerInventory::Target_RemoveItem(ItemData,System.Int32)", InvokeUserCode_Target_RemoveItem__ItemData__Int32);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerInventory), "System.Void PlayerInventory::Target_AddItem(ItemData)", InvokeUserCode_Target_AddItem__ItemData);
	}

	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteInt(_heldCurrency);
			writer.WriteBool(_withinItemDropBuffer);
			return;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteInt(_heldCurrency);
		}
		if ((base.syncVarDirtyBits & 2L) != 0L)
		{
			writer.WriteBool(_withinItemDropBuffer);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize(ref _heldCurrency, null, reader.ReadInt());
			GeneratedSyncVarDeserialize(ref _withinItemDropBuffer, null, reader.ReadBool());
			return;
		}
		long num = (long)reader.ReadULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _heldCurrency, null, reader.ReadInt());
		}
		if ((num & 2L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _withinItemDropBuffer, null, reader.ReadBool());
		}
	}
}
