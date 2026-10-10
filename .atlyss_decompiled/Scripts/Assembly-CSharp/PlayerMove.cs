using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Mirror;
using Mirror.RemoteCalls;
using UnityEngine;

public class PlayerMove : NetworkBehaviour
{
	[CompilerGenerated]
	private sealed class _003CApply_AttackForceParams_003Ed__109 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public float _timeBefore;

		public PlayerMove _003C_003E4__this;

		public float _forceDampen;

		public float _applyForce;

		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		[DebuggerHidden]
		public _003CApply_AttackForceParams_003Ed__109(int _003C_003E1__state)
		{
			this._003C_003E1__state = _003C_003E1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
		}

		private bool MoveNext()
		{
			int num = _003C_003E1__state;
			PlayerMove playerMove = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				_003C_003E2__current = new WaitForSeconds(_timeBefore);
				_003C_003E1__state = 1;
				return true;
			case 1:
				_003C_003E1__state = -1;
				playerMove._attackForceDampen = _forceDampen;
				playerMove._attackForce = _applyForce;
				playerMove._jumpKnockbackDir = Vector3.zero;
				playerMove._jumpKnockbackForce = 0f;
				return false;
			}
		}

		bool IEnumerator.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			return this.MoveNext();
		}

		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}
	}

	[CompilerGenerated]
	private sealed class _003CDashEvent_003Ed__113 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public PlayerMove _003C_003E4__this;

		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		[DebuggerHidden]
		public _003CDashEvent_003Ed__113(int _003C_003E1__state)
		{
			this._003C_003E1__state = _003C_003E1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
		}

		private bool MoveNext()
		{
			int num = _003C_003E1__state;
			PlayerMove playerMove = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				playerMove._currentRevUpSpeed = GameManager._current._statLogics._maxRevUpMoveSpeed;
				playerMove._pCombat._groundAttackControlBuffer = 0.4f;
				playerMove._gravMultiply = playerMove._baseGravityMultiplier;
				playerMove._dashCallbackTimer = 0.34f;
				playerMove._attackForce = 0f;
				playerMove._airTime = 0f;
				playerMove._dashActionBuffer = true;
				playerMove._dashCooldown = true;
				_003C_003E2__current = new WaitForSeconds(0.07f);
				_003C_003E1__state = 1;
				return true;
			case 1:
				_003C_003E1__state = -1;
				playerMove._dashActionBuffer = false;
				_003C_003E2__current = new WaitForSeconds(playerMove.dashTime);
				_003C_003E1__state = 2;
				return true;
			case 2:
				_003C_003E1__state = -1;
				if (playerMove._currentMovementAction == MovementAction.DASH)
				{
					playerMove.Set_MovementAction(MovementAction.IDLE);
				}
				if (playerMove._currentMovementAction == MovementAction.JUMPDASH)
				{
					playerMove.Set_MovementAction(MovementAction.JUMP);
				}
				_003C_003E2__current = new WaitForSeconds(playerMove.dashCoolDown);
				_003C_003E1__state = 3;
				return true;
			case 3:
				_003C_003E1__state = -1;
				playerMove._dashCooldown = false;
				return false;
			}
		}

		bool IEnumerator.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			return this.MoveNext();
		}

		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}
	}

	[CompilerGenerated]
	private sealed class _003CFear_Movement_003Ed__93 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public PlayerMove _003C_003E4__this;

		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		[DebuggerHidden]
		public _003CFear_Movement_003Ed__93(int _003C_003E1__state)
		{
			this._003C_003E1__state = _003C_003E1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
		}

		private bool MoveNext()
		{
			int num = _003C_003E1__state;
			PlayerMove playerMove = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				if (playerMove._inFearMovement)
				{
					return false;
				}
				_003C_003E2__current = new WaitForSeconds(0.08f);
				_003C_003E1__state = 1;
				return true;
			case 1:
				_003C_003E1__state = -1;
				break;
			case 2:
				_003C_003E1__state = -1;
				break;
			}
			if (playerMove._inFearMovement)
			{
				float x = UnityEngine.Random.Range(-1f, 1.2f);
				float z = UnityEngine.Random.Range(-1f, 1.2f);
				playerMove.inputIn = new Vector3(x, 0f, z);
				_003C_003E2__current = new WaitForSeconds(UnityEngine.Random.Range(0.8f, 1.2f));
				_003C_003E1__state = 2;
				return true;
			}
			return false;
		}

		bool IEnumerator.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			return this.MoveNext();
		}

		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}
	}

	private float _returnToSpawnBuffer;

	private Player _player;

	private PlayerClimbing _pClimb;

	private PlayerTargeting _pTargeting;

	private PlayerCasting _pCasting;

	private PlayerVisual _pVisual;

	private PlayerCombat _pCombat;

	private PlayerWater _pWater;

	private PlayerSound _pSound;

	[Header("General Movement")]
	public CharacterController _playerController;

	public MovementAction _currentMovementAction;

	public Vector3 _worldSpaceInput;

	public Vector3 _cachedLastInput;

	public Vector3 inputIn;

	public bool _strafeToggle;

	private bool _strafeToggleWasOn;

	[Space]
	[SyncVar]
	public float _movSpeed;

	public float _currentRevUpSpeed;

	public float _revUpSpeedModifier = 1.15f;

	public float _attackMovementSpeedModifier;

	public float slopeForce = 4f;

	[Header("Movement Locking Conditions")]
	public MovementLockType _movementLockType;

	public float _freezeMovementBuffer;

	public float _lockControlBuffer;

	public float _disableStrafeBuffer;

	public bool _lockControl;

	public bool _lockControlMidair;

	public bool _castStrafeLock;

	private float _movLockBuffer;

	[Header("Gravity")]
	[SerializeField]
	private float gravityScale = 0.8f;

	[SerializeField]
	private float groundDetection = 0.02f;

	[SerializeField]
	private float defaultGravScale = 0.65f;

	[SerializeField]
	private float defaultStepOffset = 1.65f;

	[SerializeField]
	private float defaultSlopeOffset = 90f;

	public float _underwaterGravityScaleMod;

	private Vector3 _gravity;

	[Space]
	public float _airTime;

	public float _airTimeLockBuffer;

	[SerializeField]
	private float _baseGravityMultiplier;

	private float _gravMultiply;

	[Header("Jumping")]
	public float _jumpForce = 20f;

	public float _standardJumpForce = 20f;

	public float _underwaterJumpForce = 26.5f;

	public float _holdJumpAirTimeSubtract = 2.5f;

	public int _currentJumps;

	public int _maxJumps = 2;

	public bool _isJumpInput;

	public bool _disableDoubleJumpAnim;

	private float _holdJumpBuffer;

	[Space]
	public float _jumpForwardForce;

	public float _ctrlJumpForce;

	public float _jumpKnockbackForce;

	public Vector3 _jumpKnockbackDir = Vector3.zero;

	private bool initJumpPress;

	private bool _jumpToLandBuffer;

	[Header("Dash Dodge")]
	public float dashSpeed = 105.5f;

	[SerializeField]
	private float dashTime = 0.45f;

	[SerializeField]
	private float dashCoolDown = 0.65f;

	public float _dashCallbackTimer;

	public bool _dashActionBuffer;

	private bool _dashCooldown;

	private Vector3 _dashDir;

	[Header("Slope Sliding")]
	[SerializeField]
	private float _slideVelocity = 15.65f;

	[SerializeField]
	private float _slopeDetectRadius = 1.25f;

	[SerializeField]
	private float _slopeDetectDistance = 1.5f;

	private Vector3 slopeSlideVelocity;

	private Vector3 _hitPointNormal;

	private bool _isSlidingOnSlope;

	[Header("Attack Force Params")]
	public float _attackForce;

	public float _attackForceDampen;

	public bool _inputDirectionAttackForce = true;

	public bool _isChargeAttackForced;

	private Vector3 _attackForceInput;

	private bool _inFearMovement;

	[SerializeField]
	private float _dampenAttackForceChecker = 0.3f;

	private bool IsSlidingOnSlope
	{
		get
		{
			if (!base.isLocalPlayer)
			{
				return false;
			}
			if (_currentMovementAction == MovementAction.LEDGEGRAB || _currentMovementAction == MovementAction.CLIMB)
			{
				return false;
			}
			if (_pClimb._ledgeGrabBuffer > 0f)
			{
				return false;
			}
			if (base.gameObject.scene.GetPhysicsScene().SphereCast(base.transform.position, _slopeDetectRadius, Vector3.down, out var hitInfo, _slopeDetectDistance, CastLayers.GroundMask))
			{
				_hitPointNormal = hitInfo.normal;
				slopeSlideVelocity = Vector3.ProjectOnPlane(new Vector3(0f, _slideVelocity, 0f), hitInfo.normal);
				return Vector3.Angle(_hitPointNormal, Vector3.up) > defaultSlopeOffset;
			}
			return false;
		}
	}

	public float Network_movSpeed
	{
		get
		{
			return _movSpeed;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _movSpeed, 1uL, null);
		}
	}

	private void Awake()
	{
		_player = GetComponent<Player>();
		_pVisual = GetComponent<PlayerVisual>();
		_pClimb = GetComponent<PlayerClimbing>();
		_pCombat = GetComponent<PlayerCombat>();
		_pTargeting = GetComponent<PlayerTargeting>();
		_pWater = GetComponentInChildren<PlayerWater>();
		_pCasting = GetComponent<PlayerCasting>();
		_pSound = GetComponent<PlayerSound>();
	}

	private void Start()
	{
		if (base.isServer)
		{
			Network_movSpeed = GameManager._current._statLogics._baseMoveSpeed;
		}
		Reset_PlayerControllerParams();
	}

	private void Update()
	{
		if (!base.isOwned)
		{
			_playerController.enabled = false;
		}
		if (base.isOwned && _currentMovementAction != MovementAction.MOVE && IsInvoking("Send_MoveAnim"))
		{
			CancelInvoke("Send_MoveAnim");
		}
		if (base.isLocalPlayer && _player._bufferingStatus)
		{
			Reset_PlayerControllerParams();
		}
		Server_BufferParams();
		Client_LocalPlayerMoveLockControl();
		Client_LocalPlayerControl();
		Client_WorldPositioning();
	}

	[ServerCallback]
	private void Server_BufferParams()
	{
		if (NetworkServer.active && _player._currentPlayerCondition == PlayerCondition.DEAD)
		{
			Reset_MoveSpeed();
		}
	}

	[ClientCallback]
	private void Client_WorldPositioning()
	{
		if (!NetworkClient.active || !base.isLocalPlayer || !base.isOwned)
		{
			return;
		}
		if (_returnToSpawnBuffer > 0f)
		{
			_returnToSpawnBuffer -= Time.deltaTime;
			return;
		}
		_returnToSpawnBuffer = 0f;
		if ((bool)_player.Network_playerMapInstance && !(base.transform.position.y > -300f))
		{
			_playerController.enabled = false;
			if (_player._playerZoneType == ZoneType.Dungeon)
			{
				InstancePattern setInstancePattern = _player.Network_playerMapInstance._patternInstance._setInstancePattern;
				base.transform.SetPositionAndRotation(setInstancePattern._setRespawnPoint.transform.position, setInstancePattern._setRespawnPoint.transform.rotation);
			}
			else
			{
				base.transform.SetPositionAndRotation(_player.Network_playerMapInstance._instanceSpawnPoints[0].transform.position, _player.Network_playerMapInstance._instanceSpawnPoints[0].transform.rotation);
			}
			Init_WorldPositionReset();
		}
	}

	[ClientCallback]
	private void Init_WorldPositionReset()
	{
		if (NetworkClient.active && base.isLocalPlayer && base.isOwned)
		{
			CameraFunction._current.CameraReset();
			_pVisual.Cmd_PlayTeleportEffect();
			_playerController.enabled = true;
			_returnToSpawnBuffer = 0.6f;
			_attackForce = 0f;
			_jumpForwardForce = 0f;
			_ctrlJumpForce = 0f;
			_jumpKnockbackForce = 0f;
			_jumpKnockbackDir = Vector3.zero;
		}
	}

	[ClientCallback]
	private void Client_LocalPlayerMoveLockControl()
	{
		if (NetworkClient.active && base.isLocalPlayer)
		{
			if (_freezeMovementBuffer > 0f)
			{
				_freezeMovementBuffer -= Time.deltaTime;
			}
			else
			{
				_freezeMovementBuffer = 0f;
			}
			if (_lockControlBuffer > 0f)
			{
				_lockControlBuffer -= Time.deltaTime;
			}
			else
			{
				_lockControlBuffer = 0f;
			}
			if (_disableStrafeBuffer > 0f)
			{
				_disableStrafeBuffer -= Time.deltaTime;
			}
			else
			{
				_disableStrafeBuffer = 0f;
			}
			if (_airTimeLockBuffer > 0f)
			{
				_airTimeLockBuffer -= Time.deltaTime;
			}
			else
			{
				_airTimeLockBuffer = 0f;
			}
			if (_movLockBuffer > 0f && RayGroundCheck())
			{
				_movLockBuffer -= Time.deltaTime;
			}
			if (_player._currentGameCondition == GameCondition.IN_GAME && RayGroundCheck() && _movementLockType == MovementLockType.SOFT_LOCK && _movLockBuffer <= 0f)
			{
				_movementLockType = MovementLockType.NONE;
				_movLockBuffer = 0f;
			}
		}
	}

	[ClientCallback]
	public void Reset_PlayerControllerParams()
	{
		if (NetworkClient.active)
		{
			_playerController.slopeLimit = defaultSlopeOffset;
			_playerController.stepOffset = defaultStepOffset;
			_jumpForce = _standardJumpForce;
			_jumpForwardForce = 0f;
			_jumpKnockbackForce = 0f;
			_ctrlJumpForce = 0f;
			_jumpKnockbackDir = Vector3.zero;
			gravityScale = defaultGravScale;
			_gravMultiply = _baseGravityMultiplier;
			if (_airTime > 0f)
			{
				_pSound._aSrcFootstep.PlayOneShot(_pSound._landSounds[(int)GetComponentInChildren<PlayerModelAnimManager>()._walkGroundMaterial], 1f);
				_pVisual._landEffect.Play();
				_airTime = 0f;
			}
			if (_worldSpaceInput == Vector3.zero)
			{
				Set_MovementAction(MovementAction.IDLE);
			}
			else
			{
				Set_MovementAction(MovementAction.MOVE);
			}
		}
	}

	private void Send_MoveAnim()
	{
		_pVisual.Cmd_CrossFadeAnim("Move", 0f, 0);
	}

	[ClientCallback]
	public void Set_MovementAction(MovementAction _mA)
	{
		if (!NetworkClient.active || _mA == _currentMovementAction)
		{
			return;
		}
		if (_mA == MovementAction.FALL)
		{
			MovementAction currentMovementAction = _currentMovementAction;
			if (currentMovementAction == MovementAction.JUMP || currentMovementAction == MovementAction.CLIMB || currentMovementAction == MovementAction.LEDGEGRAB)
			{
				return;
			}
		}
		if (_mA == MovementAction.JUMP)
		{
			_pVisual.Send_CrossFadeAnim("Empty", 0f, 12, LatencyCheck.IGNORE_LATENCY);
			if (_currentMovementAction == MovementAction.JUMPDASH)
			{
				_pVisual.Send_CrossFadeAnim("Fall", 0.12f, 0, LatencyCheck.IGNORE_LATENCY);
				_currentMovementAction = _mA;
				return;
			}
		}
		_currentMovementAction = _mA;
		switch (_currentMovementAction)
		{
		case MovementAction.IDLE:
			_pVisual.Send_CrossFadeAnim("Idle", 0.05f, 0, LatencyCheck.IGNORE_LATENCY);
			break;
		case MovementAction.MOVE:
			if (!IsInvoking("Send_MoveAnim"))
			{
				InvokeRepeating("Send_MoveAnim", 0f, 1.2f);
			}
			_pVisual.Send_CrossFadeAnim("Move", 0f, 0, LatencyCheck.IGNORE_LATENCY);
			_pVisual.Send_CrossFadeAnim("Empty", 0f, 12, LatencyCheck.IGNORE_LATENCY);
			break;
		case MovementAction.DASH:
			_pVisual.Send_CrossFadeAnim("Idle", 0f, 0, LatencyCheck.IGNORE_LATENCY);
			_pVisual.Send_CrossFadeAnim("Dash", 0f, 10, LatencyCheck.IGNORE_LATENCY);
			break;
		case MovementAction.JUMPDASH:
			_pVisual.Send_CrossFadeAnim("Fall", 0f, 0, LatencyCheck.IGNORE_LATENCY);
			_pVisual.Send_CrossFadeAnim("Dash", 0f, 10, LatencyCheck.IGNORE_LATENCY);
			break;
		case MovementAction.FALL:
			_pVisual.Send_CrossFadeAnim("Fall", 0.1f, 0, LatencyCheck.IGNORE_LATENCY);
			if (_currentJumps == 0)
			{
				_currentJumps = 1;
			}
			break;
		case MovementAction.CLIMB:
			_disableDoubleJumpAnim = false;
			_ctrlJumpForce = 0f;
			_currentJumps = 0;
			_pVisual.Send_CrossFadeAnim("Climb", 0f, 12, LatencyCheck.IGNORE_LATENCY);
			break;
		case MovementAction.LEDGEGRAB:
			_disableDoubleJumpAnim = false;
			_ctrlJumpForce = 0f;
			_currentJumps = 0;
			_pVisual.Send_CrossFadeAnim("LedgeHold", 0f, 0, LatencyCheck.IGNORE_LATENCY);
			break;
		case MovementAction.JUMP:
			break;
		}
	}

	[TargetRpc]
	public void Target_SetRotation(Quaternion _rotation)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteQuaternion(_rotation);
		SendTargetRPCInternal(null, "System.Void PlayerMove::Target_SetRotation(UnityEngine.Quaternion)", -1489977804, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	public void Target_LockMovement(MovementLockType _lockType, float _timer)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		GeneratedNetworkCode._Write_MovementLockType(writer, _lockType);
		writer.WriteFloat(_timer);
		SendTargetRPCInternal(null, "System.Void PlayerMove::Target_LockMovement(MovementLockType,System.Single)", 1662520452, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	public void Target_LockLookRotationMidair()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendTargetRPCInternal(null, "System.Void PlayerMove::Target_LockLookRotationMidair()", -1714039421, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientCallback]
	public void Apply_MovementLockCondition(MovementLockType _lockType, float _timer)
	{
		if (NetworkClient.active)
		{
			_movementLockType = _lockType;
			_movLockBuffer = _timer;
		}
	}

	[ServerCallback]
	public void Reset_MoveSpeed()
	{
		if (NetworkServer.active)
		{
			Network_movSpeed = GameManager._current._statLogics._baseMoveSpeed;
		}
	}

	[ServerCallback]
	public void Add_MoveSpeed(float _value)
	{
		if (NetworkServer.active)
		{
			Network_movSpeed = _movSpeed + _value;
			if (_movSpeed < 0f)
			{
				Network_movSpeed = 0f;
			}
		}
	}

	[ClientCallback]
	public void Toggle_FearMovement(bool _isTrue)
	{
		if (NetworkClient.active)
		{
			if (_isTrue)
			{
				StartCoroutine(Fear_Movement());
			}
			_inFearMovement = _isTrue;
		}
	}

	[IteratorStateMachine(typeof(_003CFear_Movement_003Ed__93))]
	[ClientCallback]
	private IEnumerator Fear_Movement()
	{
		if (!NetworkClient.active)
		{
			return null;
		}
		return new _003CFear_Movement_003Ed__93(0)
		{
			_003C_003E4__this = this
		};
	}

	[ClientCallback]
	private void Client_LocalPlayerControl()
	{
		if (NetworkClient.active && base.isLocalPlayer)
		{
			_isSlidingOnSlope = IsSlidingOnSlope;
			if (_isSlidingOnSlope)
			{
				_playerController.Move(slopeSlideVelocity * Time.deltaTime);
			}
			if (_player._bufferingStatus)
			{
				Set_MovementAction(MovementAction.IDLE);
				_attackForce = 0f;
				return;
			}
			if (_movementLockType == MovementLockType.HARD_LOCK)
			{
				Set_MovementAction(MovementAction.IDLE);
				return;
			}
			if (_movementLockType == MovementLockType.SOFT_LOCK && _pCombat._localBlockingInput && _jumpForwardForce == 0f)
			{
				_movementLockType = MovementLockType.NONE;
				return;
			}
			Handle_MovementControl();
			Handle_AttackForceControl();
			Handle_DashControl();
			Handle_JumpControl();
			Handle_Gravity();
		}
	}

	[ClientCallback]
	public void Handle_MovementControl()
	{
		if (NetworkClient.active && !(_freezeMovementBuffer > 0f))
		{
			Handle_MovementInput();
			Handle_StrafeControl();
			Apply_MovementParams();
		}
		void Apply_MovementParams()
		{
			if (_currentMovementAction != MovementAction.CLIMB && _currentMovementAction != MovementAction.LEDGEGRAB && _playerController.enabled)
			{
				if (_currentMovementAction != MovementAction.DASH && _currentMovementAction != MovementAction.JUMPDASH && !_lockControlMidair && !_lockControl && _playerController.enabled)
				{
					_playerController.Move(_worldSpaceInput);
				}
				if (_currentMovementAction == MovementAction.JUMP || _currentMovementAction == MovementAction.JUMPDASH)
				{
					Handle_JumpParams();
				}
				else
				{
					_playerController.Move(12.75f * Time.deltaTime * Vector3.down);
					if (On_WalkableSlope())
					{
						_playerController.Move(Vector3.down * _playerController.height / 2f * slopeForce * Time.deltaTime);
					}
				}
			}
		}
		void Handle_MovementInput()
		{
			if (_player._currentPlayerCondition != PlayerCondition.ACTIVE || _player._bufferingStatus)
			{
				Set_MovementAction(MovementAction.IDLE);
				_worldSpaceInput = Vector3.zero;
			}
			else
			{
				float num = InputControlManager.current._horiz_input;
				float num2 = InputControlManager.current._vert_input;
				if (_lockControlBuffer > 0f)
				{
					num = 0f;
					num2 = 0f;
				}
				else
				{
					inputIn = new Vector3(num, 0f, num2);
				}
				float num3 = _movSpeed + _currentRevUpSpeed + _attackMovementSpeedModifier;
				if (num3 <= 0f)
				{
					inputIn = Vector3.zero;
				}
				inputIn = num3 * Time.deltaTime * inputIn.normalized;
				if (_lockControlBuffer > 0f)
				{
					_worldSpaceInput = inputIn;
				}
				else
				{
					_worldSpaceInput = GameManager._current.CameraRelativeFlatten(inputIn, Vector3.up);
					_worldSpaceInput.y = 0f;
				}
				if (ResetMovementVector())
				{
					_worldSpaceInput = Vector3.zero;
				}
				if (_currentRevUpSpeed < GameManager._current._statLogics._maxRevUpMoveSpeed)
				{
					_currentRevUpSpeed += _revUpSpeedModifier * Time.deltaTime;
				}
				if (_worldSpaceInput == Vector3.zero)
				{
					_currentRevUpSpeed = 0f;
				}
				if (((bool)_pTargeting._targetedEntity || _strafeToggle) && (num != 0f || num2 < 0f) && _currentRevUpSpeed >= GameManager._current._statLogics._maxRevUpMoveSpeed * 0.25f)
				{
					_currentRevUpSpeed = GameManager._current._statLogics._maxRevUpMoveSpeed * 0.25f;
				}
				switch (_currentMovementAction)
				{
				case MovementAction.DASH:
					break;
				case MovementAction.JUMPDASH:
					break;
				case MovementAction.JUMP:
					break;
				case MovementAction.FALL:
					break;
				case MovementAction.CLIMB:
					break;
				case MovementAction.LEDGEGRAB:
					break;
				default:
					if (!_isSlidingOnSlope)
					{
						if (_worldSpaceInput != Vector3.zero)
						{
							Set_MovementAction(MovementAction.MOVE);
						}
						else if (RayGroundCheck())
						{
							Set_MovementAction(MovementAction.IDLE);
						}
					}
					break;
				}
			}
		}
		void Handle_StrafeControl()
		{
			if (_player._inChat || _player._bufferingStatus || (bool)_pTargeting._targetedEntity || _player._pProfession._isProfessionAction)
			{
				_strafeToggle = false;
			}
			else if (_movementLockType == MovementLockType.NONE && !_lockControl)
			{
				switch (_currentMovementAction)
				{
				case MovementAction.CLIMB:
					_strafeToggle = false;
					break;
				case MovementAction.LEDGEGRAB:
					_strafeToggle = false;
					break;
				default:
				{
					if (!InputControlManager.current.isLockDirectionToggled)
					{
						_strafeToggle = Input.GetKey(InputControlManager.current._lockDirection);
					}
					else if (Input.GetKeyDown(InputControlManager.current._lockDirection))
					{
						_strafeToggle = !_strafeToggle;
						_strafeToggleWasOn = _strafeToggle;
					}
					if (GamepadInput.GetButton(GamepadButton.LeftTrigger) || GamepadInput.GetButton(GamepadButton.RightTrigger))
					{
						_strafeToggle = true;
					}
					if (CameraCollision._current.maxDistance <= 1.2f)
					{
						_strafeToggle = true;
					}
					if (TabMenu._current._isOpen)
					{
						_strafeToggle = false;
					}
					if (SettingsManager._current._inputStrafeWhileHoldingWeapon.isOn)
					{
						_strafeToggle = _pCombat._currentSheathCondition == WeaponSheatheCondition.TAKEN_OUT;
					}
					if (SettingsManager._current._inputStrafeWhileCastingOffensiveSkill.isOn)
					{
						if ((bool)_pCasting._currentCastSkill && _pCasting._currentCastSkill._strafeDirection)
						{
							_strafeToggle = true;
						}
						if (_castStrafeLock)
						{
							_strafeToggle = true;
						}
					}
					bool flag = SettingsManager._current._inputStrafeWhileMoveAttack.isOn && (InputControlManager.current._horiz_input != 0f || InputControlManager.current._vert_input != 0f);
					if ((_player._currentPlayerAction == PlayerAction.ATTACKING || _pCombat._isChargingWeapon) & flag)
					{
						_strafeToggle = true;
					}
					else if (_strafeToggleWasOn)
					{
						_strafeToggle = false;
					}
					if (_disableStrafeBuffer > 0f)
					{
						_strafeToggle = false;
					}
					else if (InputControlManager.current.isLockDirectionToggled && _strafeToggleWasOn && !_strafeToggle)
					{
						_strafeToggle = true;
					}
					if (!_lockControlMidair)
					{
						if (_attackForce > 0f && _attackForceInput != Vector3.zero)
						{
							if (!_strafeToggle)
							{
								base.transform.rotation = Quaternion.LookRotation(_attackForceInput);
							}
							else
							{
								Handle_StrafeRotation();
							}
						}
						else if (_strafeToggle)
						{
							Handle_StrafeRotation();
						}
						else if (_worldSpaceInput != Vector3.zero)
						{
							base.transform.rotation = Quaternion.LookRotation(_worldSpaceInput);
						}
					}
					break;
				}
				}
			}
		}
		void Handle_StrafeRotation()
		{
			base.transform.rotation = Quaternion.Euler(0f, CameraFunction._current._RotY, 0f);
		}
		bool ResetMovementVector()
		{
			if (_movementLockType != MovementLockType.NONE)
			{
				return true;
			}
			if (_player._inChat)
			{
				return true;
			}
			if (_lockControl)
			{
				return true;
			}
			return false;
		}
	}

	[ClientCallback]
	private void Handle_JumpParams()
	{
		if (NetworkClient.active && (_currentMovementAction == MovementAction.JUMP || _currentMovementAction == MovementAction.JUMPDASH))
		{
			float num = _jumpForce;
			if (_currentJumps > 1)
			{
				num = _jumpForce - 8f;
			}
			_playerController.Move(num * Time.deltaTime * Vector3.up);
			if (_jumpForwardForce != 0f)
			{
				_playerController.Move(_jumpForwardForce * Time.deltaTime * base.transform.forward);
			}
			if (_ctrlJumpForce != 0f)
			{
				_playerController.Move(_ctrlJumpForce * Time.deltaTime * _cachedLastInput.normalized);
			}
			if (_jumpKnockbackForce != 0f && _jumpKnockbackDir != Vector3.zero)
			{
				_playerController.Move(_jumpKnockbackForce * Time.deltaTime * _jumpKnockbackDir);
			}
			PhysicsScene physicsScene = base.gameObject.scene.GetPhysicsScene();
			if ((_playerController.collisionFlags & CollisionFlags.Above) != CollisionFlags.None || physicsScene.Raycast(base.transform.position + new Vector3(0f, 2.5f, 0f), Vector3.up, 0.65f, CastLayers.GroundMask))
			{
				Reset_PlayerControllerParams();
			}
		}
	}

	[ClientCallback]
	private void Reset_CastStrafeLock()
	{
		if (NetworkClient.active)
		{
			_castStrafeLock = false;
		}
	}

	[ClientCallback]
	public void Apply_CastStrafeLock()
	{
		if (NetworkClient.active)
		{
			if (IsInvoking("Reset_CastStrafeLock"))
			{
				CancelInvoke("Reset_CastStrafeLock");
			}
			Invoke("Reset_CastStrafeLock", 0.85f);
			_castStrafeLock = true;
		}
	}

	private bool On_WalkableSlope()
	{
		if (_currentMovementAction == MovementAction.JUMP || _currentMovementAction == MovementAction.JUMPDASH || _currentMovementAction == MovementAction.FALL)
		{
			return false;
		}
		if (_currentMovementAction == MovementAction.CLIMB || _currentMovementAction == MovementAction.LEDGEGRAB)
		{
			return false;
		}
		if (base.gameObject.scene.GetPhysicsScene().Raycast(base.transform.position, Vector3.down, out var hitInfo, _playerController.height / 2f * 12.5f, CastLayers.GroundMask) && hitInfo.normal != Vector3.up)
		{
			return true;
		}
		return false;
	}

	public bool RayGroundCheck()
	{
		if (_jumpToLandBuffer)
		{
			return false;
		}
		if (base.gameObject.scene.GetPhysicsScene().SphereCast(base.transform.position, 0.25f, Vector3.down, out var _, groundDetection, CastLayers.GroundMask) || _playerController.isGrounded)
		{
			switch (_currentMovementAction)
			{
			case MovementAction.JUMP:
				Reset_PlayerControllerParams();
				break;
			case MovementAction.JUMPDASH:
				Reset_PlayerControllerParams();
				break;
			case MovementAction.FALL:
				Reset_PlayerControllerParams();
				break;
			}
			if (_lockControlMidair)
			{
				_lockControlMidair = false;
			}
			_currentJumps = 0;
			_disableDoubleJumpAnim = false;
			_isJumpInput = false;
			return true;
		}
		return false;
	}

	[ClientCallback]
	public void Handle_Gravity()
	{
		if (!NetworkClient.active)
		{
			return;
		}
		if (!_playerController.enabled)
		{
			_airTime = 0f;
			return;
		}
		if (_currentMovementAction == MovementAction.CLIMB || _currentMovementAction == MovementAction.LEDGEGRAB)
		{
			_airTime = 0f;
			return;
		}
		if (_pClimb._ledgeGrabBuffer > 0f)
		{
			_airTime = 0.8f;
			return;
		}
		_gravity = GameManager.GlobalGravity * (gravityScale + _underwaterGravityScaleMod) * Vector3.up;
		if (!RayGroundCheck() && _airTimeLockBuffer <= 0f)
		{
			_playerController.Move(_airTime * Time.deltaTime * _gravity);
		}
		Handle_AirTime();
		void Handle_AirTime()
		{
			if (_player._bufferingStatus)
			{
				_airTime = 0f;
			}
			else
			{
				if (_airTimeLockBuffer > 0f)
				{
					_airTime = 0f;
				}
				if (!RayGroundCheck() && _airTimeLockBuffer <= 0f)
				{
					_airTime += _gravMultiply * Time.deltaTime;
				}
				if (_airTime < 0f || (RayGroundCheck() && _airTime > 0f))
				{
					_airTime = 0f;
				}
				if (_airTime > 1.5f && _currentMovementAction != MovementAction.JUMPDASH && _currentMovementAction != MovementAction.DASH)
				{
					Set_MovementAction(MovementAction.FALL);
				}
			}
		}
	}

	[ClientCallback]
	public void Handle_JumpControl()
	{
		if (!NetworkClient.active || _freezeMovementBuffer > 0f)
		{
			return;
		}
		if (_ctrlJumpForce > 0f)
		{
			_ctrlJumpForce -= Time.deltaTime;
		}
		else
		{
			_ctrlJumpForce = 0f;
		}
		if (_isJumpInput && (Input.GetKey(InputControlManager.current._jump) || (GamepadInput.GetButton(GamepadButton.Button_South) && !_player._inUI)))
		{
			if (_currentJumps > 1)
			{
				_holdJumpBuffer = 0.6f;
			}
			if (_holdJumpBuffer < 0.6f)
			{
				_airTime -= _holdJumpAirTimeSubtract * Time.deltaTime;
				_holdJumpBuffer += Time.deltaTime;
			}
		}
		if (_canJumpInput() && (Input.GetKeyDown(InputControlManager.current._jump) || (GamepadInput.GetButtonDown(GamepadButton.Button_South) && !_player._inUI)))
		{
			Input_Jump();
		}
		void Input_Jump()
		{
			float force = _standardJumpForce;
			if (!_pWater._isInWater || _pWater._canSwimUp || _currentMovementAction != MovementAction.JUMP)
			{
				if (_pWater._isInWater && _airTime > 1.5f && (_currentMovementAction == MovementAction.JUMP || _currentMovementAction == MovementAction.FALL))
				{
					force = _underwaterJumpForce;
				}
				if (_worldSpaceInput == Vector3.zero)
				{
					Init_Jump(force, 0f, 0f, 0f, _useAnim: true);
				}
				else
				{
					Init_Jump(force, 1.65f, 0f, 0f, _useAnim: true);
				}
				_isJumpInput = true;
				_pCombat._airAttackLock = 0f;
			}
		}
		bool _canJumpInput()
		{
			if (_player._currentPlayerCondition != PlayerCondition.ACTIVE)
			{
				return false;
			}
			if (_player._inChat)
			{
				return false;
			}
			if (_movementLockType != MovementLockType.NONE)
			{
				return false;
			}
			if (_lockControlBuffer > 0f)
			{
				return false;
			}
			if (IsSlidingOnSlope)
			{
				return false;
			}
			if (!_pWater._isInWater && _currentJumps >= _maxJumps)
			{
				return false;
			}
			if (_attackMovementSpeedModifier <= -80f)
			{
				return false;
			}
			if (_lockControlMidair || _lockControl)
			{
				return false;
			}
			if (_jumpToLandBuffer)
			{
				return false;
			}
			if (initJumpPress)
			{
				return false;
			}
			return true;
		}
	}

	[TargetRpc]
	public void Target_InitJump(float _force, float _forwardForce, float _knockbackForce, Vector3 _knockbackDir, float _gravityMultiply)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteFloat(_force);
		writer.WriteFloat(_forwardForce);
		writer.WriteFloat(_knockbackForce);
		writer.WriteVector3(_knockbackDir);
		writer.WriteFloat(_gravityMultiply);
		SendTargetRPCInternal(null, "System.Void PlayerMove::Target_InitJump(System.Single,System.Single,System.Single,UnityEngine.Vector3,System.Single)", 1480569366, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientCallback]
	public void Init_Jump(float _force, float _forwardForce, float _knockbackForce, float _gravityMultiply, bool _useAnim)
	{
		if (!NetworkClient.active || !base.isLocalPlayer)
		{
			return;
		}
		float time = 0f;
		float time2 = 0.1f;
		_holdJumpBuffer = 0f;
		if (_currentJumps > 0 && _gravityMultiply <= 0f)
		{
			_gravityMultiply = 23.5f;
		}
		if (_pWater._isInWater && _airTime > 1.5f)
		{
			time = 1f;
		}
		if (_gravityMultiply > 0f)
		{
			_gravMultiply = _gravityMultiply;
		}
		if (_currentMovementAction == MovementAction.LEDGEGRAB)
		{
			time2 = 0.35f;
		}
		_airTime = 0f;
		_jumpToLandBuffer = true;
		initJumpPress = true;
		if (!IsInvoking("JumpPress"))
		{
			Invoke("JumpPress", time);
		}
		if (!IsInvoking("JumpToLandBuffer"))
		{
			Invoke("JumpToLandBuffer", time2);
		}
		_jumpForce = _force;
		_jumpForwardForce = _forwardForce;
		_jumpKnockbackForce = _knockbackForce;
		_playerController.slopeLimit = 0f;
		_playerController.stepOffset = 0f;
		_currentJumps++;
		if (_useAnim)
		{
			if (_currentJumps > 1)
			{
				StopCoroutine("Init_DoubleJumpAnimPrompt");
				StartCoroutine(Init_DoubleJumpAnimPrompt());
			}
			else
			{
				_pVisual.Send_CrossFadeAnim("Jump", 0f, 0, LatencyCheck.IGNORE_LATENCY);
			}
		}
		Set_MovementAction(MovementAction.JUMP);
	}

	[ClientCallback]
	private void JumpPress()
	{
		if (NetworkClient.active)
		{
			initJumpPress = false;
		}
	}

	[ClientCallback]
	private void JumpToLandBuffer()
	{
		if (NetworkClient.active)
		{
			_jumpToLandBuffer = false;
		}
	}

	private IEnumerator Init_DoubleJumpAnimPrompt()
	{
		if (_disableDoubleJumpAnim || _pClimb._onRope)
		{
			yield break;
		}
		yield return new WaitForSeconds(0.1f);
		if (_pWater._isInWater)
		{
			_pVisual.Send_CrossFadeAnim("SwimJump", 0f, 0, LatencyCheck.IGNORE_LATENCY);
			yield break;
		}
		if (_currentJumps == 2)
		{
			_pVisual.Send_CrossFadeAnim("DoubleJump", 0f, 0, LatencyCheck.IGNORE_LATENCY);
		}
		if (_currentJumps >= 3 && _maxJumps >= 3)
		{
			_pVisual.Send_CrossFadeAnim("TripleJump", 0f, 0, LatencyCheck.IGNORE_LATENCY);
		}
		_pVisual._doubleJumpRingEffect.Play();
	}

	[ClientCallback]
	public void Init_AttackForceApply(float _applyForce, float _forceDampen, float _timeBefore)
	{
		if (NetworkClient.active)
		{
			StartCoroutine(Apply_AttackForceParams(_applyForce, _forceDampen, _timeBefore));
		}
	}

	[IteratorStateMachine(typeof(_003CApply_AttackForceParams_003Ed__109))]
	[ClientCallback]
	private IEnumerator Apply_AttackForceParams(float _applyForce, float _forceDampen, float _timeBefore)
	{
		if (!NetworkClient.active)
		{
			return null;
		}
		return new _003CApply_AttackForceParams_003Ed__109(0)
		{
			_003C_003E4__this = this,
			_applyForce = _applyForce,
			_forceDampen = _forceDampen,
			_timeBefore = _timeBefore
		};
	}

	[ClientCallback]
	public void Handle_AttackForceControl()
	{
		if (!NetworkClient.active || !base.isLocalPlayer)
		{
			return;
		}
		if (_player._bufferingStatus)
		{
			_attackForce = 0f;
			return;
		}
		if (_lockControlBuffer > 0f)
		{
			_attackForce = 0f;
			return;
		}
		if (_player._currentHurtBuffer > 0f)
		{
			_attackForce = 0f;
			return;
		}
		if (_pCombat._localBlockingInput)
		{
			_attackForce = 0f;
			return;
		}
		_attackForce = Mathf.Lerp(_attackForce, 0f, _attackForceDampen * Time.deltaTime);
		if (_attackForce <= _dampenAttackForceChecker && _attackForce > 0f)
		{
			_attackForce = 0f;
		}
		if (_attackForce >= 0f - _dampenAttackForceChecker && _attackForce < 0f)
		{
			_attackForce = 0f;
		}
		if (_pCombat._isChargingWeapon)
		{
			_inputDirectionAttackForce = false;
		}
		else if (_player._currentPlayerAction == PlayerAction.ATTACKING)
		{
			_inputDirectionAttackForce = true;
		}
		if ((_strafeToggle || (bool)_pTargeting._targetedEntity) && _isChargeAttackForced)
		{
			_inputDirectionAttackForce = false;
		}
		if (_attackForceInput != Vector3.zero && _playerController.enabled)
		{
			_playerController.Move(_attackForce * Time.deltaTime * _attackForceInput.normalized);
		}
		if (_attackForce > 0f)
		{
			return;
		}
		if (_inputDirectionAttackForce)
		{
			float horiz_input = InputControlManager.current._horiz_input;
			float vert_input = InputControlManager.current._vert_input;
			_attackForceInput = new Vector3(horiz_input, 0f, vert_input);
			_attackForceInput = GameManager._current.CameraRelativeFlatten(_attackForceInput, Vector3.up);
			if (_pCombat._setWepTypeCombatParams._applyChargeAttackForceOnNoInput && _isChargeAttackForced && horiz_input == 0f && vert_input == 0f)
			{
				_attackForceInput = base.transform.forward;
			}
		}
		else
		{
			_attackForceInput = base.transform.forward;
		}
		if (_disableStrafeBuffer > 0f)
		{
			_attackForceInput = base.transform.forward;
		}
		_attackForceInput.y = 0f;
	}

	[ClientCallback]
	public void Handle_DashControl()
	{
		if (NetworkClient.active)
		{
			if (_dashCallbackTimer > 0f)
			{
				_dashCallbackTimer -= Time.deltaTime;
			}
			else
			{
				_dashCallbackTimer = 0f;
			}
			if (_player._currentPlayerCondition == PlayerCondition.ACTIVE && _currentMovementAction != MovementAction.CLIMB && _currentMovementAction != MovementAction.LEDGEGRAB && _movementLockType == MovementLockType.NONE && !(_freezeMovementBuffer > 0f) && !(_lockControlBuffer > 0f))
			{
				Handle_DashProcess();
				Handle_DashInput();
			}
		}
		void Handle_DashInput()
		{
			if (!(SettingsManager._current._gamepadInputBuffer > 0f) && !_player._bufferingStatus && !_player._isHurt && !_player._inChat && !_player._inUI && !_lockControlMidair && !(_dashDir == Vector3.zero) && !_dashCooldown && InputControlManager.current.IsDashInputDown)
			{
				if (_currentMovementAction == MovementAction.JUMP || _currentMovementAction == MovementAction.FALL)
				{
					Set_MovementAction(MovementAction.JUMPDASH);
					_jumpForwardForce = 0f;
					_jumpForce -= 5f;
				}
				else
				{
					Set_MovementAction(MovementAction.DASH);
				}
				StopCoroutine(DashEvent());
				StartCoroutine(DashEvent());
			}
		}
		void Handle_DashProcess()
		{
			if (DialogManager._current._isDialogEnabled)
			{
				_dashDir = Vector3.zero;
				_worldSpaceInput = Vector3.zero;
			}
			if (_currentMovementAction == MovementAction.DASH || _currentMovementAction == MovementAction.JUMPDASH)
			{
				if (_worldSpaceInput != Vector3.zero)
				{
					_playerController.Move(_worldSpaceInput * dashSpeed);
					_dashDir = _worldSpaceInput;
				}
				else
				{
					_playerController.Move(_dashDir * dashSpeed);
				}
			}
			else
			{
				float horiz_input = InputControlManager.current._horiz_input;
				float vert_input = InputControlManager.current._vert_input;
				Vector3 vector = new Vector3(horiz_input, 0f, vert_input);
				vector = (_movSpeed + _currentRevUpSpeed) * Time.deltaTime * vector.normalized;
				_dashDir = GameManager._current.CameraRelativeFlatten(vector, Vector3.up);
				_dashDir.y = 0f;
			}
		}
	}

	[IteratorStateMachine(typeof(_003CDashEvent_003Ed__113))]
	[ClientCallback]
	private IEnumerator DashEvent()
	{
		if (!NetworkClient.active)
		{
			return null;
		}
		return new _003CDashEvent_003Ed__113(0)
		{
			_003C_003E4__this = this
		};
	}

	public override bool Weaved()
	{
		return true;
	}

	protected void UserCode_Target_SetRotation__Quaternion(Quaternion _rotation)
	{
		base.transform.rotation = _rotation;
	}

	protected static void InvokeUserCode_Target_SetRotation__Quaternion(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			UnityEngine.Debug.LogError("TargetRPC Target_SetRotation called on server.");
		}
		else
		{
			((PlayerMove)obj).UserCode_Target_SetRotation__Quaternion(reader.ReadQuaternion());
		}
	}

	protected void UserCode_Target_LockMovement__MovementLockType__Single(MovementLockType _lockType, float _timer)
	{
		Apply_MovementLockCondition(_lockType, _timer);
	}

	protected static void InvokeUserCode_Target_LockMovement__MovementLockType__Single(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			UnityEngine.Debug.LogError("TargetRPC Target_LockMovement called on server.");
		}
		else
		{
			((PlayerMove)obj).UserCode_Target_LockMovement__MovementLockType__Single(GeneratedNetworkCode._Read_MovementLockType(reader), reader.ReadFloat());
		}
	}

	protected void UserCode_Target_LockLookRotationMidair()
	{
		_lockControlMidair = true;
	}

	protected static void InvokeUserCode_Target_LockLookRotationMidair(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			UnityEngine.Debug.LogError("TargetRPC Target_LockLookRotationMidair called on server.");
		}
		else
		{
			((PlayerMove)obj).UserCode_Target_LockLookRotationMidair();
		}
	}

	protected void UserCode_Target_InitJump__Single__Single__Single__Vector3__Single(float _force, float _forwardForce, float _knockbackForce, Vector3 _knockbackDir, float _gravityMultiply)
	{
		if (!(_attackForce > 0f) || !_player._isHurt)
		{
			_currentJumps = 0;
			_jumpKnockbackDir = _knockbackDir;
			Init_Jump(_force, _forwardForce, _knockbackForce, _gravityMultiply, _useAnim: false);
		}
	}

	protected static void InvokeUserCode_Target_InitJump__Single__Single__Single__Vector3__Single(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			UnityEngine.Debug.LogError("TargetRPC Target_InitJump called on server.");
		}
		else
		{
			((PlayerMove)obj).UserCode_Target_InitJump__Single__Single__Single__Vector3__Single(reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat(), reader.ReadVector3(), reader.ReadFloat());
		}
	}

	static PlayerMove()
	{
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerMove), "System.Void PlayerMove::Target_SetRotation(UnityEngine.Quaternion)", InvokeUserCode_Target_SetRotation__Quaternion);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerMove), "System.Void PlayerMove::Target_LockMovement(MovementLockType,System.Single)", InvokeUserCode_Target_LockMovement__MovementLockType__Single);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerMove), "System.Void PlayerMove::Target_LockLookRotationMidair()", InvokeUserCode_Target_LockLookRotationMidair);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerMove), "System.Void PlayerMove::Target_InitJump(System.Single,System.Single,System.Single,UnityEngine.Vector3,System.Single)", InvokeUserCode_Target_InitJump__Single__Single__Single__Vector3__Single);
	}

	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteFloat(_movSpeed);
			return;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteFloat(_movSpeed);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize(ref _movSpeed, null, reader.ReadFloat());
			return;
		}
		long num = (long)reader.ReadULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _movSpeed, null, reader.ReadFloat());
		}
	}
}
