using System.Runtime.InteropServices;
using Mirror;
using Mirror.RemoteCalls;
using UnityEngine;

public class PushBlock : NetworkBehaviour
{
	private GroundType _groundType;

	private Rigidbody _rb;

	[Header("Pushblock Behaviour")]
	[SyncVar]
	public Player _parentPlayer;

	public Transform _pushblockCube;

	[SerializeField]
	private CombatCollider _pushBlockGravityDmgCollider;

	[SerializeField]
	private ParticleSystem _pushEffect;

	[SerializeField]
	private float _pushBlockRange = 15f;

	[SerializeField]
	private Vector3 _pushblockScale = new Vector3(15f, 15f, 15f);

	private float pushCommandBuffer;

	[Space]
	[SyncVar]
	public Vector3 _pushDestinationPosition;

	[SyncVar]
	public float _pushBuffer;

	[SerializeField]
	private float _pushblockSpeed = 15.5f;

	[SerializeField]
	private float _elevationCheckAdjust = 3.5f;

	public float _timeBetweenPushes = 1.2f;

	public bool _isMoving;

	[Space]
	[SerializeField]
	private float _groundDetection = 6.5f;

	[SerializeField]
	private float _groundPositionAdjust = 1.25f;

	[SyncVar]
	public bool _isGrounded = true;

	private float _gravityAdd;

	[Header("Pushblock Audio")]
	[SerializeField]
	private AudioSource _aSrc_pushblockMove;

	[SerializeField]
	private AudioSource _aSrc_pushblockStop;

	protected NetworkBehaviourSyncVar ____parentPlayerNetId;

	public Player Network_parentPlayer
	{
		get
		{
			return GetSyncVarNetworkBehaviour(____parentPlayerNetId, ref _parentPlayer);
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter_NetworkBehaviour(value, ref _parentPlayer, 1uL, null, ref ____parentPlayerNetId);
		}
	}

	public Vector3 Network_pushDestinationPosition
	{
		get
		{
			return _pushDestinationPosition;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _pushDestinationPosition, 2uL, null);
		}
	}

	public float Network_pushBuffer
	{
		get
		{
			return _pushBuffer;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _pushBuffer, 4uL, null);
		}
	}

	public bool Network_isGrounded
	{
		get
		{
			return _isGrounded;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _isGrounded, 8uL, null);
		}
	}

	protected override void OnValidate()
	{
		base.OnValidate();
		if ((bool)_pushblockCube)
		{
			_pushblockCube.localScale = _pushblockScale;
		}
		Network_pushDestinationPosition = base.transform.position;
	}

	public override void OnStartServer()
	{
		base.OnStartServer();
		Network_pushDestinationPosition = base.transform.position;
	}

	private void Awake()
	{
		_rb = GetComponent<Rigidbody>();
		_groundType = GetComponentInChildren<GroundType>();
		_rb.isKinematic = true;
	}

	private void Update()
	{
		if ((bool)Player._mainPlayer)
		{
			_aSrc_pushblockMove.mute = Player._mainPlayer._bufferingStatus || (Player._mainPlayer._isHostPlayer && Player._mainPlayer.gameObject.scene != base.gameObject.scene);
			_aSrc_pushblockStop.mute = Player._mainPlayer._bufferingStatus || (Player._mainPlayer._isHostPlayer && Player._mainPlayer.gameObject.scene != base.gameObject.scene);
		}
		Handle_PushblockControl();
		Handle_PushblockPositioning();
		if ((bool)Network_parentPlayer && base.isServer)
		{
			Network_parentPlayer.GetComponent<PlayerCombat>().Network_currentSheathCondition = WeaponSheatheCondition.PUT_AWAY;
		}
		if (pushCommandBuffer > 0f)
		{
			pushCommandBuffer -= Time.deltaTime;
		}
	}

	[ClientCallback]
	private void Handle_PushblockControl()
	{
		if (NetworkClient.active && base.isOwned)
		{
			Vector3 position = base.transform.position;
			position.y = Player._mainPlayer.transform.position.y;
			PlayerMove component = Player._mainPlayer.GetComponent<PlayerMove>();
			PlayerInteract component2 = Player._mainPlayer.GetComponent<PlayerInteract>();
			PlayerCombat component3 = Player._mainPlayer.GetComponent<PlayerCombat>();
			PlayerCasting component4 = Player._mainPlayer.GetComponent<PlayerCasting>();
			PlayerVisual component5 = Player._mainPlayer.GetComponent<PlayerVisual>();
			component._playerController.Move(35.5f * Time.deltaTime * Player._mainPlayer.transform.forward);
			component._playerController.Move(50.5f * Time.deltaTime * Vector3.down);
			component.transform.LookAt(position);
			component2._currentCommandBuffer = 1.25f;
			component3._lockControlBuffer = 0.06f;
			component4._lockControlBuffer = 0.06f;
			component._lockControlBuffer = 0.06f;
			component._lockControl = true;
			if (Input.GetKey(InputControlManager.current._up) && pushCommandBuffer <= 0f)
			{
				component5.Local_CrossFadeAnim("pushblock_push", 0.1f, 7);
				Request_PushBlockAction(Player._mainPlayer.transform.forward, 0, _isPull: false);
				pushCommandBuffer = 0.4f;
			}
			if (Input.GetKey(InputControlManager.current._down) && pushCommandBuffer <= 0f)
			{
				component5.Local_CrossFadeAnim("pushblock_pull", 0.1f, 7);
				Request_PushBlockAction(-Player._mainPlayer.transform.forward, 1, _isPull: true);
				pushCommandBuffer = 0.4f;
			}
			if (Input.GetKeyUp(InputControlManager.current._interact) || !_isGrounded || Player._mainPlayer._isHurt || component._airTime >= 0.65f)
			{
				component5.Local_CrossFadeAnim("Empty", 0.1f, 7);
				component5.Send_CrossFadeAnim("Empty", 0.1f, 7, LatencyCheck.IGNORE_LATENCY);
				Invoke("Init_UnlockControl", 0.6f);
				Cmd_RemoveParentPlayer();
			}
		}
	}

	private void Init_UnlockControl()
	{
		PlayerVisual component = Player._mainPlayer.GetComponent<PlayerVisual>();
		PlayerMove component2 = Player._mainPlayer.GetComponent<PlayerMove>();
		component.Local_CrossFadeAnim("Empty", 0.1f, 7);
		component2._lockControl = false;
	}

	[Command]
	private void Cmd_RemoveParentPlayer()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void PushBlock::Cmd_RemoveParentPlayer()", 188038276, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void Handle_PushblockPositioning()
	{
		LayerMask layerMask = LayerMask.GetMask("Ground");
		PhysicsScene physicsScene = base.gameObject.scene.GetPhysicsScene();
		float num = Vector3.Distance(_pushDestinationPosition, base.transform.position);
		_pushBlockGravityDmgCollider.gameObject.SetActive(!_isGrounded);
		if (!_isGrounded)
		{
			_groundType._canLedgeClimb = false;
		}
		if (_isMoving && _isGrounded)
		{
			_groundType._canLedgeClimb = false;
		}
		if (!_isMoving && _isGrounded)
		{
			_groundType._canLedgeClimb = true;
		}
		if (num > 0.1f)
		{
			if (_isGrounded)
			{
				base.transform.position = Vector3.Lerp(base.transform.position, _pushDestinationPosition, _pushblockSpeed * Time.deltaTime);
				_gravityAdd = 0f;
			}
			else
			{
				if (base.isServer)
				{
					Network_isGrounded = physicsScene.Raycast(base.transform.position, Vector3.down, _groundDetection, layerMask);
				}
				_gravityAdd += 45.5f * Time.deltaTime;
				base.transform.position += _gravityAdd * Time.deltaTime * Vector3.down;
			}
			if (!_pushEffect.isPlaying)
			{
				_pushEffect.Play();
			}
			if (!_aSrc_pushblockMove.isPlaying)
			{
				_aSrc_pushblockMove.Play();
			}
			Network_pushBuffer = _timeBetweenPushes;
			_isMoving = true;
		}
		else if (base.transform.position != _pushDestinationPosition)
		{
			base.transform.position = _pushDestinationPosition;
			if (_aSrc_pushblockMove.isPlaying)
			{
				_aSrc_pushblockStop.Play();
				_aSrc_pushblockMove.Stop();
			}
			if (_pushEffect.isPlaying)
			{
				_pushEffect.Stop();
			}
			Network_pushBuffer = _timeBetweenPushes;
			_isMoving = false;
		}
		if (base.isServer && _pushBuffer > 0f)
		{
			Network_pushBuffer = _pushBuffer - Time.deltaTime;
			return;
		}
		if (base.isServer)
		{
			Network_isGrounded = physicsScene.Raycast(base.transform.position, Vector3.down, _groundDetection, layerMask);
		}
		if (!_isGrounded && base.isServer)
		{
			physicsScene.Raycast(base.transform.position, Vector3.down, out var hitInfo, float.PositiveInfinity, layerMask);
			Vector3 point = hitInfo.point;
			point.y += _groundPositionAdjust;
			Network_pushDestinationPosition = new Vector3(_pushDestinationPosition.x, point.y, _pushDestinationPosition.z);
		}
	}

	[Command]
	private void Request_PushBlockAction(Vector3 _dir, int _animIndex, bool _isPull)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteVector3(_dir);
		writer.WriteInt(_animIndex);
		writer.WriteBool(_isPull);
		SendCommandInternal("System.Void PushBlock::Request_PushBlockAction(UnityEngine.Vector3,System.Int32,System.Boolean)", -2128407372, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ServerCallback]
	private void Locate_PushblockPosition(Vector3 _dir, int _animIndex)
	{
		if (!NetworkServer.active || _pushBuffer > 0f)
		{
			return;
		}
		if ((bool)Network_parentPlayer)
		{
			PlayerVisual component = Network_parentPlayer.GetComponent<PlayerVisual>();
			switch (_animIndex)
			{
			case 0:
				component.Send_CrossFadeAnim("pushblock_push", 0.1f, 7, LatencyCheck.IGNORE_LATENCY);
				break;
			case 1:
				component.Send_CrossFadeAnim("pushblock_pull", 0.1f, 7, LatencyCheck.IGNORE_LATENCY);
				break;
			}
		}
		PhysicsScene physicsScene = base.gameObject.scene.GetPhysicsScene();
		LayerMask layerMask = LayerMask.GetMask("Pushblock_Nav");
		LayerMask layerMask2 = LayerMask.GetMask("Interactable");
		LayerMask layerMask3 = LayerMask.GetMask("Ground");
		Vector3 vector = base.transform.position + _dir * _pushBlockRange;
		bool flag = physicsScene.Raycast(vector, Vector3.down, out var hitInfo, float.PositiveInfinity, layerMask) && !physicsScene.Raycast(vector + Vector3.up * _pushBlockRange, Vector3.down, _pushBlockRange, layerMask2);
		if ((!physicsScene.Raycast(base.transform.position + Vector3.down * _elevationCheckAdjust, _dir, out var hitInfo2, _pushBlockRange, layerMask3) || (bool)hitInfo2.collider.gameObject.GetComponentInParent<FloorSwitch>()) && flag)
		{
			Vector3 position = hitInfo.collider.gameObject.transform.transform.position;
			position.y = base.transform.position.y;
			Network_pushDestinationPosition = position;
			Network_pushBuffer = _timeBetweenPushes;
		}
	}

	public override bool Weaved()
	{
		return true;
	}

	protected void UserCode_Cmd_RemoveParentPlayer()
	{
		base.netIdentity.RemoveClientAuthority();
		Network_parentPlayer = null;
	}

	protected static void InvokeUserCode_Cmd_RemoveParentPlayer(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command Cmd_RemoveParentPlayer called on client.");
		}
		else
		{
			((PushBlock)obj).UserCode_Cmd_RemoveParentPlayer();
		}
	}

	protected void UserCode_Request_PushBlockAction__Vector3__Int32__Boolean(Vector3 _dir, int _animIndex, bool _isPull)
	{
		PhysicsScene physicsScene = base.gameObject.scene.GetPhysicsScene();
		LayerMask layerMask = LayerMask.GetMask("Interactable");
		_ = (LayerMask)LayerMask.GetMask("Ground");
		if (_isPull)
		{
			bool flag = true;
			if (physicsScene.Raycast(Player._mainPlayer.transform.position, _dir, out var hitInfo, _pushBlockRange, layerMask) && (bool)hitInfo.collider.gameObject.GetComponent<PushBlock>())
			{
				flag = false;
			}
			if (!flag)
			{
				return;
			}
		}
		Locate_PushblockPosition(_dir, _animIndex);
	}

	protected static void InvokeUserCode_Request_PushBlockAction__Vector3__Int32__Boolean(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command Request_PushBlockAction called on client.");
		}
		else
		{
			((PushBlock)obj).UserCode_Request_PushBlockAction__Vector3__Int32__Boolean(reader.ReadVector3(), reader.ReadInt(), reader.ReadBool());
		}
	}

	static PushBlock()
	{
		RemoteProcedureCalls.RegisterCommand(typeof(PushBlock), "System.Void PushBlock::Cmd_RemoveParentPlayer()", InvokeUserCode_Cmd_RemoveParentPlayer, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PushBlock), "System.Void PushBlock::Request_PushBlockAction(UnityEngine.Vector3,System.Int32,System.Boolean)", InvokeUserCode_Request_PushBlockAction__Vector3__Int32__Boolean, requiresAuthority: true);
	}

	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteNetworkBehaviour(Network_parentPlayer);
			writer.WriteVector3(_pushDestinationPosition);
			writer.WriteFloat(_pushBuffer);
			writer.WriteBool(_isGrounded);
			return;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteNetworkBehaviour(Network_parentPlayer);
		}
		if ((base.syncVarDirtyBits & 2L) != 0L)
		{
			writer.WriteVector3(_pushDestinationPosition);
		}
		if ((base.syncVarDirtyBits & 4L) != 0L)
		{
			writer.WriteFloat(_pushBuffer);
		}
		if ((base.syncVarDirtyBits & 8L) != 0L)
		{
			writer.WriteBool(_isGrounded);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize_NetworkBehaviour(ref _parentPlayer, null, reader, ref ____parentPlayerNetId);
			GeneratedSyncVarDeserialize(ref _pushDestinationPosition, null, reader.ReadVector3());
			GeneratedSyncVarDeserialize(ref _pushBuffer, null, reader.ReadFloat());
			GeneratedSyncVarDeserialize(ref _isGrounded, null, reader.ReadBool());
			return;
		}
		long num = (long)reader.ReadULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize_NetworkBehaviour(ref _parentPlayer, null, reader, ref ____parentPlayerNetId);
		}
		if ((num & 2L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _pushDestinationPosition, null, reader.ReadVector3());
		}
		if ((num & 4L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _pushBuffer, null, reader.ReadFloat());
		}
		if ((num & 8L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _isGrounded, null, reader.ReadBool());
		}
	}
}
