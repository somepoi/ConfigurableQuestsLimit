using System.Collections;
using Mirror;
using UnityEngine;

public class ItemObject : MonoBehaviour, IInteractable
{
	private Net_ItemObject _netItemObj;

	private ItemObjectVisual _itemObjVisual;

	[Header("Item Object General Parameters")]
	public NetworkIdentity _local_taggedID;

	public NetworkIdentity _currency_taggedID;

	[Space]
	public Transform _pickupDestination;

	public Collider _interactableCollider;

	public CapsuleCollider _physicsCollider;

	public bool _canPickUp = true;

	public bool _isPickedUp;

	public bool _localPickup;

	[Header("Item Data")]
	public ScriptableItem _foundItem;

	public ItemData _local_itemData;

	public int _currencyDropAmount;

	[Header("Item Object Physics")]
	public Rigidbody _rb;

	[SerializeField]
	private float _gravityMultiplier = 3.5f;

	[SerializeField]
	private float _jumpForceAtSpawn = 16.5f;

	private bool _canCheckGround;

	[Header("Item Object Timers")]
	[SerializeField]
	private float _tagResetTimer = 6.5f;

	private float _currentTagResetTimer;

	[SerializeField]
	private float _destroyObjectTimer = 35.5f;

	public bool _dontDestroyOnTimer;

	private float _currentDestroyObjectTimer;

	private bool _beginDestroy;

	private bool _initUnspawn;

	private void Awake()
	{
		_itemObjVisual = GetComponent<ItemObjectVisual>();
		_netItemObj = GetComponent<Net_ItemObject>();
	}

	private void OnEnable()
	{
		Init_itemObject();
	}

	public void Client_SetLocalItemData(ItemData _setItemData)
	{
		_local_itemData = _setItemData;
	}

	private void Init_itemObject()
	{
		if (!_netItemObj)
		{
			_rb.isKinematic = false;
		}
		_pickupDestination = null;
		_foundItem = null;
		_initUnspawn = false;
		_beginDestroy = false;
		_canCheckGround = false;
		_localPickup = false;
		_isPickedUp = false;
		_canPickUp = false;
		_physicsCollider.enabled = true;
		base.transform.localScale = Vector3.one;
		_rb.AddForce(Random.Range(-0.8f, 0.8f), _jumpForceAtSpawn, Random.Range(-0.8f, 0.8f), ForceMode.Impulse);
		Invoke("Enable_GroundCheckToVelocityZero", 1.65f);
		_currentTagResetTimer = _tagResetTimer;
		_currentDestroyObjectTimer = _destroyObjectTimer;
	}

	private void Update()
	{
		if ((bool)_netItemObj)
		{
			_rb.isKinematic = !_netItemObj.isServer && !_isPickedUp;
			_local_taggedID = _netItemObj.Network_taggedID;
		}
		else if ((bool)Player._mainPlayer && Player._mainPlayer._bufferingStatus)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		if (TabMenu._current._optionsCell._isDisconnecting)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		if (_currencyDropAmount > 0)
		{
			_itemObjVisual.Apply_ItemObjectVisual();
		}
		else if (_local_itemData._itemName != string.Empty && !_foundItem)
		{
			_foundItem = GameManager._current.Locate_Item(_local_itemData._itemName);
			if (_foundItem != null)
			{
				_itemObjVisual.Apply_ItemObjectVisual();
			}
		}
		Handle_ItemObjectTimer();
		_interactableCollider.enabled = !_localPickup;
		if (_localPickup && !IsInvoking("Reset_LocalItemPickup"))
		{
			Invoke("Reset_LocalItemPickup", 0.85f);
		}
		if (_canPickUp && _local_taggedID != null && _currencyDropAmount > 0 && !_netItemObj)
		{
			Init_PickupItem(_local_taggedID);
		}
	}

	public void UnspawnObject()
	{
		if (!_initUnspawn)
		{
			StartCoroutine(Init_UnspawnObject());
		}
	}

	private IEnumerator Init_UnspawnObject()
	{
		_initUnspawn = true;
		yield return new WaitForSeconds(0.2f);
		if (_isPickedUp)
		{
			PoolManager._current.GenerateObject(null, "coinPickup", 1.35f).transform.position = base.transform.position;
		}
		yield return new WaitForSeconds(0.6f);
		if ((bool)_netItemObj && _netItemObj.isServer)
		{
			_netItemObj.Rpc_SpawnPuffcloud();
			yield return new WaitForSeconds(0.08f);
			NetworkServer.UnSpawn(base.gameObject);
			Object.Destroy(base.gameObject);
			yield break;
		}
		if ((bool)Player._mainPlayer && !Player._mainPlayer._bufferingStatus && !_isPickedUp)
		{
			GameObject obj = PoolManager._current.GenerateObject(null, "Puffcloud", 1.5f);
			obj.SetActive(value: true);
			obj.transform.SetPositionAndRotation(base.transform.position, Quaternion.identity);
		}
		PoolManager._current.Init_ReturnObjectToContainer(base.gameObject);
	}

	public void Init_PickupItem(NetworkIdentity _netId)
	{
		Player component = _netId.GetComponent<Player>();
		PlayerInventory pInventory = component._pInventory;
		if (_currencyDropAmount > 0)
		{
			pInventory.Cmd_AddCurrency(_currencyDropAmount);
		}
		else
		{
			pInventory.Add_Item(_local_itemData, _autoStack: true);
		}
		_localPickup = true;
		Init_ItemObjectPickup(component.transform);
		UnspawnObject();
	}

	private void Reset_LocalItemPickup()
	{
		_localPickup = false;
	}

	private void FixedUpdate()
	{
		if (!_rb.isKinematic)
		{
			_rb.AddForce(0f, 0f - _gravityMultiplier, 0f, ForceMode.Impulse);
		}
		if (_isPickedUp && (bool)_pickupDestination)
		{
			_physicsCollider.enabled = false;
			Vector3 vector = _pickupDestination.position - base.transform.position;
			_rb.AddRelativeForce(vector.normalized * 14.5f, ForceMode.Force);
		}
		Handle_CheckItemObjectCollider();
	}

	private void Enable_GroundCheckToVelocityZero()
	{
		_canCheckGround = true;
		_canPickUp = true;
	}

	private void Handle_CheckItemObjectCollider()
	{
		if ((!_netItemObj || _netItemObj.isServer) && !_isPickedUp)
		{
			Vector3 origin = new Vector3(base.transform.position.x, base.transform.position.y, base.transform.position.z);
			PhysicsScene physicsScene = base.gameObject.scene.GetPhysicsScene();
			if (physicsScene.Raycast(origin, Vector3.down, 0.85f, CastLayers.ItemObjectMask))
			{
				Vector3 vector = Random.insideUnitSphere * 3.5f;
				vector.y = 0f;
			}
			if (physicsScene.Raycast(origin, Vector3.down, 1.25f, CastLayers.GroundMask) && _canCheckGround)
			{
				_rb.isKinematic = true;
			}
			if (base.transform.position.y <= -125f)
			{
				UnspawnObject();
			}
		}
	}

	private void Handle_ItemObjectTimer()
	{
		if ((!_netItemObj || _netItemObj.isServer) && !_isPickedUp)
		{
			_beginDestroy = true;
			Handle_BeginDestroyBehavior();
		}
		void Handle_BeginDestroyBehavior()
		{
			if (_beginDestroy && !_dontDestroyOnTimer)
			{
				if (_isPickedUp)
				{
					_itemObjVisual._flashVisual = false;
				}
				else
				{
					if (_currentDestroyObjectTimer <= _destroyObjectTimer * 0.06f)
					{
						_itemObjVisual._flashVisual = true;
					}
					if (_currentDestroyObjectTimer > 0f)
					{
						_currentDestroyObjectTimer -= 3.25f * Time.deltaTime;
					}
					else
					{
						UnspawnObject();
					}
				}
			}
		}
	}

	public void Init_ItemObjectPickup(Transform _parentTransform)
	{
		_local_taggedID = _parentTransform.gameObject.GetComponent<NetworkIdentity>();
		_canPickUp = false;
		_isPickedUp = true;
		_itemObjVisual._itemObjectParticle.Stop();
		_pickupDestination = _parentTransform;
		_rb.isKinematic = false;
		_rb.velocity = new Vector3(0f, 12.5f, 0f);
		Invoke("Init_FollowPlayerOnPickup", 0.35f);
	}

	private void Init_FollowPlayerOnPickup()
	{
		_itemObjVisual._followPlayerOnPickup = true;
	}
}
