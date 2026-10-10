using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Mirror;
using Mirror.RemoteCalls;
using UnityEngine;

public class NetNPC : NetworkBehaviour
{
	private DialogTrigger _dialogTrigger;

	[Header("Friendly NPC Behavior")]
	[SerializeField]
	private DynamicBone[] _jiggleBones = new DynamicBone[0];

	public Animator _animator;

	private string _lastAnimName = string.Empty;

	private Vector3 _originRotation = Vector3.zero;

	[Header("Navigation")]
	[SerializeField]
	private float _moveSpeed = 15.5f;

	[SerializeField]
	private bool _stopNavigation;

	[SerializeField]
	private FriendlyNPCNavigationPoint[] _navigationWaypoints;

	[SerializeField]
	private bool _hasNavigation = true;

	[SyncVar]
	public int _currentWaypoint;

	[SyncVar]
	public bool _isMoving;

	private float _bufferBetweenPoints;

	[Header("Peer Interaction")]
	public Player _localPlayerInteraction;

	[SyncVar]
	public Player _netSyncPlayerInteraction;

	[SerializeField]
	private bool _lookAtInteractedPeer = true;

	private float _peerLookBuffer;

	[Header("Shopkeep Parameters")]
	[SyncVar]
	public int _shopkeepResetTimer;

	[SerializeField]
	private int _setShopKeepResetTimer = 120;

	[SerializeField]
	private float _setShopKeepResetBuffer = 2f;

	[Space]
	public int _gearItems;

	public int _consumableItems;

	public int _tradeItems;

	public int _buybackItems;

	private float _shopkeepResetTimerBuffer;

	public bool _initShopReset;

	public bool _initShopkeepListing;

	public readonly Dictionary<string, ShopkeepItemStruct> _vendorItems = new Dictionary<string, ShopkeepItemStruct>();

	public readonly SyncDictionary<string, ShopkeepItemStruct> _syncBuybackItems = new SyncDictionary<string, ShopkeepItemStruct>();

	protected NetworkBehaviourSyncVar ____netSyncPlayerInteractionNetId;

	public int Network_currentWaypoint
	{
		get
		{
			return _currentWaypoint;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _currentWaypoint, 1uL, null);
		}
	}

	public bool Network_isMoving
	{
		get
		{
			return _isMoving;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _isMoving, 2uL, null);
		}
	}

	public Player Network_netSyncPlayerInteraction
	{
		get
		{
			return GetSyncVarNetworkBehaviour(____netSyncPlayerInteractionNetId, ref _netSyncPlayerInteraction);
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter_NetworkBehaviour(value, ref _netSyncPlayerInteraction, 4uL, null, ref ____netSyncPlayerInteractionNetId);
		}
	}

	public int Network_shopkeepResetTimer
	{
		get
		{
			return _shopkeepResetTimer;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _shopkeepResetTimer, 8uL, null);
		}
	}

	private void Awake()
	{
		_dialogTrigger = GetComponentInChildren<DialogTrigger>();
		_originRotation = base.transform.rotation.eulerAngles;
	}

	private void Start()
	{
		_syncBuybackItems.Callback += OnShopkeepBuybackUpdated;
		foreach (KeyValuePair<string, ShopkeepItemStruct> syncBuybackItem in _syncBuybackItems)
		{
			OnShopkeepBuybackUpdated(SyncIDictionary<string, ShopkeepItemStruct>.Operation.OP_ADD, syncBuybackItem.Key, syncBuybackItem.Value);
		}
	}

	private void Update()
	{
		Handle_LookAtPeers();
		Handle_Navigation();
		Handle_ShopkeepRuntime();
		Handle_DynamicBoneToggle();
		void Handle_DynamicBoneToggle()
		{
			for (int i = 0; i < _jiggleBones.Length; i++)
			{
				if ((bool)Player._mainPlayer && Player._mainPlayer._isHostPlayer && Player._mainPlayer.gameObject.scene != base.gameObject.scene)
				{
					_jiggleBones[i].enabled = false;
				}
				else
				{
					_jiggleBones[i].enabled = !SettingsManager._current._jiggleBonesToggle.isOn;
				}
			}
		}
	}

	private void Handle_LookAtPeers()
	{
		if ((bool)_animator)
		{
			_animator.SetBool("_isInteracting", _localPlayerInteraction != null || Network_netSyncPlayerInteraction != null);
			_animator.SetBool("_isMoving", _isMoving);
		}
		if (!_lookAtInteractedPeer)
		{
			_originRotation = base.transform.eulerAngles;
		}
		else if ((bool)_localPlayerInteraction)
		{
			Vector3 forward = Player._mainPlayer.transform.position - base.transform.position;
			forward.y = 0f;
			Quaternion b = Quaternion.LookRotation(forward);
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, b, 6.45f * Time.deltaTime);
			if (base.isServer)
			{
				_peerLookBuffer = 3.2f;
			}
		}
		else if ((bool)Network_netSyncPlayerInteraction)
		{
			Vector3 forward = Network_netSyncPlayerInteraction.transform.position - base.transform.position;
			forward.y = 0f;
			Quaternion b2 = Quaternion.LookRotation(forward);
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, b2, 6.45f * Time.deltaTime);
			if (base.isServer)
			{
				_peerLookBuffer = 3.2f;
			}
		}
		else if (_hasNavigation)
		{
			if (_peerLookBuffer > 0f)
			{
				_peerLookBuffer -= Time.deltaTime;
				if (base.isServer)
				{
					Network_isMoving = false;
				}
				return;
			}
			_peerLookBuffer = 0f;
			if (Vector3.Distance(_navigationWaypoints[_currentWaypoint]._waypoint.position, base.transform.position) > _navigationWaypoints[_currentWaypoint]._distanceToStop && _isMoving)
			{
				Vector3 forward = _navigationWaypoints[_currentWaypoint]._waypoint.position - base.transform.position;
				forward.y = 0f;
				Quaternion b3 = Quaternion.LookRotation(forward);
				base.transform.rotation = Quaternion.Lerp(base.transform.rotation, b3, 17.85f * Time.deltaTime);
			}
		}
		else if (_peerLookBuffer > 0f)
		{
			_peerLookBuffer -= Time.deltaTime;
		}
		else
		{
			Quaternion b4 = Quaternion.Euler(_originRotation);
			base.transform.rotation = Quaternion.Lerp(base.transform.rotation, b4, 17.85f * Time.deltaTime);
		}
	}

	public void Init_ShopResetTimer()
	{
		if (!_initShopReset)
		{
			Network_shopkeepResetTimer = _setShopKeepResetTimer;
			_shopkeepResetTimerBuffer = _setShopKeepResetBuffer;
			_initShopReset = true;
		}
	}

	private void OnShopkeepBuybackUpdated(SyncIDictionary<string, ShopkeepItemStruct>.Operation _op, string _key, ShopkeepItemStruct _shopStruct)
	{
		ScriptableShopkeep scriptableShopkeep = _dialogTrigger._scriptDialogData._scriptableShopkeep;
		bool flag = (bool)Player._mainPlayer && (bool)ShopkeepManager._current._scriptShopkeep && ShopkeepManager._current._isOpen && ShopkeepManager._current._scriptShopkeep == scriptableShopkeep && ShopkeepManager._current._listDataEntries.Count > 0;
		switch (_op)
		{
		case SyncIDictionary<string, ShopkeepItemStruct>.Operation.OP_ADD:
			if (flag)
			{
				ShopkeepManager._current.Refresh_ShopkeepListings(this);
			}
			if (base.isServer)
			{
				_buybackItems++;
			}
			break;
		case SyncIDictionary<string, ShopkeepItemStruct>.Operation.OP_REMOVE:
			if (flag)
			{
				ShopkeepManager._current.Delete_ShopkeepListing(_key);
			}
			if (base.isServer)
			{
				_buybackItems--;
			}
			break;
		case SyncIDictionary<string, ShopkeepItemStruct>.Operation.OP_CLEAR:
			if (flag)
			{
				_vendorItems.Clear();
				StartCoroutine(Reset_ListingAfterClear());
			}
			if (base.isServer)
			{
				_buybackItems = 0;
			}
			break;
		}
	}

	public void Init_DelayedRefreshListing()
	{
		StartCoroutine(Reset_ListingAfterClear());
	}

	private IEnumerator Reset_ListingAfterClear()
	{
		_initShopkeepListing = false;
		ShopkeepManager._current.Clear_ShopkeepListings();
		yield return new WaitForSeconds(0.12f);
		ShopkeepManager._current.Refresh_ShopkeepListings(this);
	}

	private void Handle_ShopkeepRuntime()
	{
		if (!_dialogTrigger || !_dialogTrigger._scriptDialogData._scriptableShopkeep)
		{
			return;
		}
		if (!_initShopkeepListing && ShopkeepManager._current._isOpen)
		{
			Init_ShopkeepListing(_dialogTrigger._scriptDialogData._scriptableShopkeep);
		}
		if (!ShopkeepManager._current._isOpen)
		{
			_initShopkeepListing = false;
			_vendorItems.Clear();
		}
		foreach (KeyValuePair<string, ShopkeepItemStruct> vendorItem in _vendorItems)
		{
			if (_vendorItems[vendorItem.Key]._removeAtEmptyStock && _vendorItems[vendorItem.Key]._stockQuantity <= 0)
			{
				_vendorItems.Remove(vendorItem.Key);
				break;
			}
		}
		foreach (KeyValuePair<string, ShopkeepItemStruct> syncBuybackItem in _syncBuybackItems)
		{
			if (base.isServer && _syncBuybackItems[syncBuybackItem.Key]._removeAtEmptyStock && _syncBuybackItems[syncBuybackItem.Key]._stockQuantity <= 0)
			{
				_syncBuybackItems.Remove(syncBuybackItem.Key);
				break;
			}
		}
		if (!_initShopReset || !base.isServer)
		{
			return;
		}
		if (_shopkeepResetTimer > 0)
		{
			if (_shopkeepResetTimerBuffer > 0f)
			{
				_shopkeepResetTimerBuffer -= Time.deltaTime;
				return;
			}
			Network_shopkeepResetTimer = _shopkeepResetTimer - 1;
			_shopkeepResetTimerBuffer = _setShopKeepResetBuffer;
		}
		else
		{
			_syncBuybackItems.Clear();
			_initShopReset = false;
			_initShopkeepListing = false;
		}
	}

	private void Init_ShopkeepListing(ScriptableShopkeep _scriptShopkeep)
	{
		if (_initShopkeepListing || !_scriptShopkeep)
		{
			return;
		}
		ShopkeepItemTable shopkeepItemTable = _scriptShopkeep._shopkeepItemTables[0];
		for (int i = 0; i < _scriptShopkeep._shopkeepItemTables.Length && Player._mainPlayer._pStats._currentLevel >= _scriptShopkeep._shopkeepItemTables[i]._levelRequirement; i++)
		{
			shopkeepItemTable = _scriptShopkeep._shopkeepItemTables[i];
		}
		for (int j = 0; j < shopkeepItemTable._shopkeepItems.Length; j++)
		{
			ScriptableItem scriptableItem = null;
			int num = 0;
			if ((bool)shopkeepItemTable._shopkeepItems[j]._scriptItem)
			{
				scriptableItem = shopkeepItemTable._shopkeepItems[j]._scriptItem;
			}
			if (shopkeepItemTable._shopkeepItems[j]._isGambleSlot)
			{
				scriptableItem = shopkeepItemTable._shopkeepItems[j]._gambleLootTable._itemDrops[Random.Range(0, shopkeepItemTable._shopkeepItems[j]._gambleLootTable._itemDrops.Length)]._item;
			}
			if (!scriptableItem)
			{
				continue;
			}
			string key = scriptableItem._itemName ?? "";
			if (scriptableItem._itemType == ItemType.GEAR)
			{
				ScriptableEquipment scriptableEquipment = (ScriptableEquipment)scriptableItem;
				if ((int)scriptableEquipment._itemRarity <= 1)
				{
					if (shopkeepItemTable._shopkeepItems[j]._randomizeModifier && (bool)scriptableEquipment._statModifierTable)
					{
						if (Random.Range(0, 2) == 1)
						{
							StatModifierSlot[] statModifierSlots = scriptableEquipment._statModifierTable._statModifierSlots;
							num = statModifierSlots[Random.Range(0, statModifierSlots.Length)]._equipModifier._modifierID;
						}
						else
						{
							num = 0;
						}
					}
					else if ((bool)shopkeepItemTable._shopkeepItems[j]._setStatModifier)
					{
						num = shopkeepItemTable._shopkeepItems[j]._setStatModifier._modifierID;
					}
					if (num > 0)
					{
						key = GameManager._current.Locate_StatModifier(num)._modifierTag + " " + scriptableItem._itemName;
					}
				}
			}
			ItemData itemData = new ItemData
			{
				_itemName = scriptableItem._itemName,
				_isAltWeapon = false,
				_isEquipped = false,
				_maxQuantity = scriptableItem._maxStackAmount,
				_quantity = 0,
				_slotNumber = j,
				_modifierID = num
			};
			string specialItemCostName = string.Empty;
			if ((bool)shopkeepItemTable._shopkeepItems[j]._specialStoreCost._scriptItem)
			{
				specialItemCostName = shopkeepItemTable._shopkeepItems[j]._specialStoreCost._scriptItem._itemName;
			}
			ShopkeepItemStruct value = new ShopkeepItemStruct
			{
				_itemName = scriptableItem._itemName,
				_isbuybackItem = false,
				_isGambleItem = shopkeepItemTable._shopkeepItems[j]._isGambleSlot,
				_gambleValue = shopkeepItemTable._shopkeepItems[j]._gambleValue,
				_equipModifierID = num,
				_stockQuantity = 1,
				_itemData = itemData,
				_specialItemCostName = specialItemCostName,
				_specialItemCostQuantity = shopkeepItemTable._shopkeepItems[j]._specialStoreCost._scriptItemQuantity,
				_dedicatedValue = shopkeepItemTable._shopkeepItems[j]._dedicatedItemValue,
				_useDedicatedValue = shopkeepItemTable._shopkeepItems[j]._useDedicatedItemValue
			};
			if (!_vendorItems.ContainsKey(key))
			{
				_vendorItems.Add(key, value);
			}
		}
		_initShopkeepListing = true;
	}

	public void Local_CrossFade_Anim(string _animName, float _smoothTime, int _layer)
	{
		if ((bool)_animator && !(_lastAnimName == _animName))
		{
			_animator.CrossFade(_animName, _smoothTime, _layer);
			_lastAnimName = _animName;
		}
	}

	[ClientRpc]
	public void Rpc_CrossFade_Anim(string _animName, float _smoothTime, int _layer)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteString(_animName);
		writer.WriteFloat(_smoothTime);
		writer.WriteInt(_layer);
		SendRPCInternal("System.Void NetNPC::Rpc_CrossFade_Anim(System.String,System.Single,System.Int32)", -1921679257, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	[ServerCallback]
	private void Handle_Navigation()
	{
		if (!NetworkServer.active)
		{
			return;
		}
		PhysicsScene _physicsScene = base.gameObject.scene.GetPhysicsScene();
		Handle_YAxisPosition();
		if (_navigationWaypoints == null || _navigationWaypoints.Length == 0 || _stopNavigation)
		{
			return;
		}
		if (_bufferBetweenPoints > 0f)
		{
			_bufferBetweenPoints -= Time.deltaTime;
			return;
		}
		_bufferBetweenPoints = 0f;
		if (_localPlayerInteraction != null || Network_netSyncPlayerInteraction != null)
		{
			_bufferBetweenPoints = 1.5f;
			_lastAnimName = string.Empty;
			return;
		}
		if (_physicsScene.Raycast(base.transform.position, base.transform.forward, 4.5f, CastLayers.PlayerMask) || _peerLookBuffer > 0f)
		{
			_bufferBetweenPoints = 0.8f;
			Network_isMoving = false;
			return;
		}
		if (Vector3.Distance(_navigationWaypoints[_currentWaypoint]._waypoint.position, base.transform.position) > _navigationWaypoints[_currentWaypoint]._distanceToStop)
		{
			Vector3 forward = _navigationWaypoints[_currentWaypoint]._waypoint.position - base.transform.position;
			forward.y = 0f;
			Quaternion b = Quaternion.LookRotation(forward);
			base.transform.rotation = Quaternion.Lerp(base.transform.rotation, b, 17.85f * Time.deltaTime);
			base.transform.Translate(Vector3.forward * (_moveSpeed * Time.deltaTime));
			Network_isMoving = true;
			return;
		}
		_bufferBetweenPoints = _navigationWaypoints[_currentWaypoint]._bufferTime;
		if (_navigationWaypoints[_currentWaypoint]._animationOnStop != string.Empty)
		{
			if (Random.Range(0f, 1f) <= _navigationWaypoints[_currentWaypoint]._chanceToAnimationOnStop)
			{
				Rpc_CrossFade_Anim(_navigationWaypoints[_currentWaypoint]._animationOnStop, 0.085f, 0);
				Network_isMoving = false;
			}
			else
			{
				Network_isMoving = false;
			}
		}
		else
		{
			Network_isMoving = false;
		}
		if (_currentWaypoint + 1 >= _navigationWaypoints.Length)
		{
			Network_currentWaypoint = 0;
		}
		else
		{
			Network_currentWaypoint = _currentWaypoint + 1;
		}
		void Handle_YAxisPosition()
		{
			Vector3 position = base.transform.position;
			position.y += 4.5f;
			if (_physicsScene.Raycast(position, Vector3.down, out var hitInfo, float.PositiveInfinity, CastLayers.GroundMask))
			{
				base.transform.position = new Vector3(base.transform.position.x, hitInfo.point.y, base.transform.position.z);
			}
		}
	}

	public NetNPC()
	{
		InitSyncObject(_syncBuybackItems);
	}

	public override bool Weaved()
	{
		return true;
	}

	protected void UserCode_Rpc_CrossFade_Anim__String__Single__Int32(string _animName, float _smoothTime, int _layer)
	{
		if ((bool)_animator && !(_lastAnimName == _animName))
		{
			_animator.CrossFade(_animName, _smoothTime, _layer);
			_lastAnimName = _animName;
		}
	}

	protected static void InvokeUserCode_Rpc_CrossFade_Anim__String__Single__Int32(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC Rpc_CrossFade_Anim called on server.");
		}
		else
		{
			((NetNPC)obj).UserCode_Rpc_CrossFade_Anim__String__Single__Int32(reader.ReadString(), reader.ReadFloat(), reader.ReadInt());
		}
	}

	static NetNPC()
	{
		RemoteProcedureCalls.RegisterRpc(typeof(NetNPC), "System.Void NetNPC::Rpc_CrossFade_Anim(System.String,System.Single,System.Int32)", InvokeUserCode_Rpc_CrossFade_Anim__String__Single__Int32);
	}

	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteInt(_currentWaypoint);
			writer.WriteBool(_isMoving);
			writer.WriteNetworkBehaviour(Network_netSyncPlayerInteraction);
			writer.WriteInt(_shopkeepResetTimer);
			return;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteInt(_currentWaypoint);
		}
		if ((base.syncVarDirtyBits & 2L) != 0L)
		{
			writer.WriteBool(_isMoving);
		}
		if ((base.syncVarDirtyBits & 4L) != 0L)
		{
			writer.WriteNetworkBehaviour(Network_netSyncPlayerInteraction);
		}
		if ((base.syncVarDirtyBits & 8L) != 0L)
		{
			writer.WriteInt(_shopkeepResetTimer);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize(ref _currentWaypoint, null, reader.ReadInt());
			GeneratedSyncVarDeserialize(ref _isMoving, null, reader.ReadBool());
			GeneratedSyncVarDeserialize_NetworkBehaviour(ref _netSyncPlayerInteraction, null, reader, ref ____netSyncPlayerInteractionNetId);
			GeneratedSyncVarDeserialize(ref _shopkeepResetTimer, null, reader.ReadInt());
			return;
		}
		long num = (long)reader.ReadULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _currentWaypoint, null, reader.ReadInt());
		}
		if ((num & 2L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _isMoving, null, reader.ReadBool());
		}
		if ((num & 4L) != 0L)
		{
			GeneratedSyncVarDeserialize_NetworkBehaviour(ref _netSyncPlayerInteraction, null, reader, ref ____netSyncPlayerInteractionNetId);
		}
		if ((num & 8L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _shopkeepResetTimer, null, reader.ReadInt());
		}
	}
}
