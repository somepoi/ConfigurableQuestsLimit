using System.Runtime.InteropServices;
using Mirror;
using Mirror.RemoteCalls;
using UnityEngine;

public class Net_ItemObject : NetworkBehaviour
{
	private NetworkTransformUnreliable _netTransform;

	private ItemObject _itemObj;

	[Header("Network Item Object")]
	[SyncVar]
	public NetworkIdentity _taggedID;

	[SyncVar]
	public bool _isPickedUp;

	[SyncVar]
	public bool _canPickUp = true;

	[Header("Item Data")]
	[SyncVar]
	public int _currencyDropAmount;

	[SyncVar]
	public ItemData _syncItemData = new ItemData();

	protected uint ____taggedIDNetId;

	public NetworkIdentity Network_taggedID
	{
		get
		{
			return GetSyncVarNetworkIdentity(____taggedIDNetId, ref _taggedID);
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter_NetworkIdentity(value, ref _taggedID, 1uL, null, ref ____taggedIDNetId);
		}
	}

	public bool Network_isPickedUp
	{
		get
		{
			return _isPickedUp;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _isPickedUp, 2uL, null);
		}
	}

	public bool Network_canPickUp
	{
		get
		{
			return _canPickUp;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _canPickUp, 4uL, null);
		}
	}

	public int Network_currencyDropAmount
	{
		get
		{
			return _currencyDropAmount;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _currencyDropAmount, 8uL, null);
		}
	}

	public ItemData Network_syncItemData
	{
		get
		{
			return _syncItemData;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _syncItemData, 16uL, null);
		}
	}

	private void Awake()
	{
		_netTransform = GetComponent<NetworkTransformUnreliable>();
		_itemObj = GetComponent<ItemObject>();
	}

	private void Update()
	{
		Server_SyncData();
		_itemObj._rb.isKinematic = !base.isServer;
		_itemObj._currencyDropAmount = _currencyDropAmount;
		_itemObj._local_itemData = _syncItemData;
	}

	[ServerCallback]
	private void Server_SyncData()
	{
		if (!NetworkServer.active)
		{
			return;
		}
		if (_currencyDropAmount > GameManager._current._statLogics._maxCurrency)
		{
			Network_currencyDropAmount = GameManager._current._statLogics._maxCurrency;
		}
		if (_currencyDropAmount <= 0)
		{
			ScriptableItem scriptableItem = GameManager._current.Locate_Item(_syncItemData._itemName);
			if (!scriptableItem)
			{
				NetworkServer.Destroy(base.gameObject);
			}
			else
			{
				if (scriptableItem._itemType == ItemType.GEAR)
				{
					_syncItemData._maxQuantity = 1;
				}
				if (_syncItemData._maxQuantity > scriptableItem._maxStackAmount)
				{
					_syncItemData._maxQuantity = scriptableItem._maxStackAmount;
				}
				if (_syncItemData._quantity > scriptableItem._maxStackAmount)
				{
					_syncItemData._quantity = scriptableItem._maxStackAmount;
				}
			}
			if (_syncItemData._modifierID > 0 && !GameManager._current.Locate_StatModifier(_syncItemData._modifierID))
			{
				_syncItemData._modifierID = 0;
			}
		}
		else
		{
			Network_syncItemData = new ItemData();
		}
	}

	[ClientRpc]
	public void Rpc_NetItemObjectPickup(NetworkIdentity _netID)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteNetworkIdentity(_netID);
		SendRPCInternal("System.Void Net_ItemObject::Rpc_NetItemObjectPickup(Mirror.NetworkIdentity)", -2088878467, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	public void Rpc_SpawnPuffcloud()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendRPCInternal("System.Void Net_ItemObject::Rpc_SpawnPuffcloud()", -1335830936, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	public override bool Weaved()
	{
		return true;
	}

	protected void UserCode_Rpc_NetItemObjectPickup__NetworkIdentity(NetworkIdentity _netID)
	{
		_netTransform.enabled = false;
		_itemObj.Init_PickupItem(_netID);
		_itemObj.Init_ItemObjectPickup(_netID.transform);
		_itemObj.UnspawnObject();
	}

	protected static void InvokeUserCode_Rpc_NetItemObjectPickup__NetworkIdentity(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC Rpc_NetItemObjectPickup called on server.");
		}
		else
		{
			((Net_ItemObject)obj).UserCode_Rpc_NetItemObjectPickup__NetworkIdentity(reader.ReadNetworkIdentity());
		}
	}

	protected void UserCode_Rpc_SpawnPuffcloud()
	{
		if ((bool)Player._mainPlayer && (!Player._mainPlayer._isHostPlayer || !(Player._mainPlayer.gameObject.scene != base.gameObject.scene)) && !Player._mainPlayer._bufferingStatus && !_isPickedUp)
		{
			GameObject obj = PoolManager._current.GenerateObject(null, "Puffcloud", 1.5f);
			obj.SetActive(value: true);
			obj.transform.SetPositionAndRotation(base.transform.position, Quaternion.identity);
		}
	}

	protected static void InvokeUserCode_Rpc_SpawnPuffcloud(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC Rpc_SpawnPuffcloud called on server.");
		}
		else
		{
			((Net_ItemObject)obj).UserCode_Rpc_SpawnPuffcloud();
		}
	}

	static Net_ItemObject()
	{
		RemoteProcedureCalls.RegisterRpc(typeof(Net_ItemObject), "System.Void Net_ItemObject::Rpc_NetItemObjectPickup(Mirror.NetworkIdentity)", InvokeUserCode_Rpc_NetItemObjectPickup__NetworkIdentity);
		RemoteProcedureCalls.RegisterRpc(typeof(Net_ItemObject), "System.Void Net_ItemObject::Rpc_SpawnPuffcloud()", InvokeUserCode_Rpc_SpawnPuffcloud);
	}

	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteNetworkIdentity(Network_taggedID);
			writer.WriteBool(_isPickedUp);
			writer.WriteBool(_canPickUp);
			writer.WriteInt(_currencyDropAmount);
			GeneratedNetworkCode._Write_ItemData(writer, _syncItemData);
			return;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteNetworkIdentity(Network_taggedID);
		}
		if ((base.syncVarDirtyBits & 2L) != 0L)
		{
			writer.WriteBool(_isPickedUp);
		}
		if ((base.syncVarDirtyBits & 4L) != 0L)
		{
			writer.WriteBool(_canPickUp);
		}
		if ((base.syncVarDirtyBits & 8L) != 0L)
		{
			writer.WriteInt(_currencyDropAmount);
		}
		if ((base.syncVarDirtyBits & 0x10L) != 0L)
		{
			GeneratedNetworkCode._Write_ItemData(writer, _syncItemData);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize_NetworkIdentity(ref _taggedID, null, reader, ref ____taggedIDNetId);
			GeneratedSyncVarDeserialize(ref _isPickedUp, null, reader.ReadBool());
			GeneratedSyncVarDeserialize(ref _canPickUp, null, reader.ReadBool());
			GeneratedSyncVarDeserialize(ref _currencyDropAmount, null, reader.ReadInt());
			GeneratedSyncVarDeserialize(ref _syncItemData, null, GeneratedNetworkCode._Read_ItemData(reader));
			return;
		}
		long num = (long)reader.ReadULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize_NetworkIdentity(ref _taggedID, null, reader, ref ____taggedIDNetId);
		}
		if ((num & 2L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _isPickedUp, null, reader.ReadBool());
		}
		if ((num & 4L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _canPickUp, null, reader.ReadBool());
		}
		if ((num & 8L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _currencyDropAmount, null, reader.ReadInt());
		}
		if ((num & 0x10L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _syncItemData, null, GeneratedNetworkCode._Read_ItemData(reader));
		}
	}
}
