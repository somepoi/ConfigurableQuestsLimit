using Mirror;
using UnityEngine;

public class PlayerClimbing : NetworkBehaviour
{
	private CharacterController _playerController;

	private Player _player;

	private PlayerMove _pMove;

	private PlayerVisual _pVisual;

	private PlayerCombat _pCombat;

	private StatusEntity _statusEntity;

	[Header("Rope / Ladder Climbing")]
	[SerializeField]
	private float _climbSpeed = 9.85f;

	[SerializeField]
	private float _distanceToLadderDetect = 0.0125f;

	[SerializeField]
	private float _turnSpeed = 0.135f;

	public bool _isClimbing;

	public bool _isGoingDown;

	public bool _onRope;

	[Space]
	[SerializeField]
	private float _climbInitBuffer = 0.085f;

	public float _ledgeGrabBuffer;

	public float _climbAttachBuffer;

	[Space]
	private Transform _ropeParent;

	private LadderEndCap[] _ladderParentEndCaps;

	private bool _isMovingOnRope;

	[Header("Ledge Climbing")]
	[SerializeField]
	private Transform _ledgeClimbCastChestPos;

	[SerializeField]
	private Transform _ledgeClimbCastNormalPos;

	[SerializeField]
	private Vector2 _allowedLedgeGrabNormalThreshold = new Vector3(0.75f, 1.5f);

	[SerializeField]
	private float _airTimeNeededToLedgeClimb = 50f;

	[SerializeField]
	private float _ledgeClimbCastLength = 2.35f;

	[SerializeField]
	private float _ledgeForwardCastLength = 0.65f;

	[SerializeField]
	private float _ledgeJumpUpForce = 45.65f;

	[SerializeField]
	private float _ledgeJumpForwardForce = 35.5f;

	private float _ledgePlayerPositionOffset = -1.5f;

	private GroundType _parentGroundTypeObject;

	public bool _onLedge;

	private RaycastHit _hit;

	private void Awake()
	{
		_player = GetComponent<Player>();
		_pMove = GetComponent<PlayerMove>();
		_pVisual = GetComponent<PlayerVisual>();
		_pCombat = GetComponent<PlayerCombat>();
		_playerController = GetComponent<CharacterController>();
		_statusEntity = GetComponent<StatusEntity>();
	}

	private void Update()
	{
		if (!base.isLocalPlayer)
		{
			base.enabled = false;
			return;
		}
		if (_climbAttachBuffer > 0f)
		{
			_climbAttachBuffer -= Time.deltaTime;
		}
		else
		{
			_climbAttachBuffer = 0f;
		}
		if (_ledgeGrabBuffer > 0f)
		{
			_ledgeGrabBuffer -= Time.deltaTime;
		}
		else
		{
			_ledgeGrabBuffer = 0f;
		}
		if (_player._currentPlayerCondition != PlayerCondition.ACTIVE || _player._bufferingStatus)
		{
			_onRope = false;
			_isClimbing = false;
			_ropeParent = null;
			_isGoingDown = false;
			_parentGroundTypeObject = null;
			_ladderParentEndCaps = new LadderEndCap[0];
		}
		else if (_statusEntity._generalHitBuffer > 0f && _isClimbing && _onLedge)
		{
			Init_JumpOffLedgeGrab(20f, -25.5f, 45.5f);
		}
		else
		{
			Handle_RopeClimbControl();
			Handle_LedgeClimbControl();
		}
	}

	[ClientCallback]
	private void Handle_LedgeClimbControl()
	{
		if (NetworkClient.active && _player._currentPlayerAction == PlayerAction.IDLE && !(_climbAttachBuffer > 0f) && !_ropeParent && !_pCombat._isAirAttacked && !_pCombat._localAirAttackInput && !_pCombat._isChargingWeapon && !_pCombat._isBlocking && !_pCombat._localBlockingInput && !_pMove._lockControlMidair && !(_pMove._lockControlBuffer > 0f) && !(_pMove._attackForce > 0f))
		{
			if (_onLedge)
			{
				Handle_OnLedge();
			}
			else
			{
				Handle_LedgeDetection();
			}
		}
		void Handle_LedgeDetection()
		{
			if (((_pMove._currentMovementAction != MovementAction.FALL && _pMove._currentMovementAction != MovementAction.JUMP && _pMove._currentMovementAction != MovementAction.JUMPDASH) || !(_pMove._airTime < _airTimeNeededToLedgeClimb)) && !_pMove.RayGroundCheck() && !(_ledgeGrabBuffer > 0f))
			{
				PhysicsScene physicsScene = base.gameObject.scene.GetPhysicsScene();
				if (physicsScene.Raycast(_ledgeClimbCastNormalPos.position, Vector3.down, out _hit, _ledgeClimbCastLength, CastLayers.GroundMask))
				{
					Vector3 position = _ledgeClimbCastChestPos.position;
					position.y = _hit.point.y - 0.2f;
					_ledgeClimbCastChestPos.position = position;
					if (physicsScene.Raycast(_ledgeClimbCastChestPos.position, base.transform.forward, _ledgeForwardCastLength, CastLayers.GroundMask) && (bool)_hit.collider.gameObject.GetComponent<GroundType>())
					{
						_parentGroundTypeObject = _hit.collider.gameObject.GetComponent<GroundType>();
						if (_parentGroundTypeObject._canLedgeClimb && !(_hit.normal.y < _allowedLedgeGrabNormalThreshold.x) && !(_hit.normal.y > _allowedLedgeGrabNormalThreshold.y))
						{
							_ledgePlayerPositionOffset = _pVisual._playerRaceModel._ledgeClimbOffset;
							position.y = _hit.point.y - _ledgePlayerPositionOffset;
							base.transform.position = position;
							_pMove.Set_MovementAction(MovementAction.LEDGEGRAB);
							_pCombat.Cmd_SheatheWeapon(WeaponSheatheCondition.PUT_AWAY);
							_isClimbing = true;
							_onLedge = true;
						}
					}
				}
			}
		}
		void Handle_OnLedge()
		{
			if (!_parentGroundTypeObject)
			{
				_onLedge = false;
				_isClimbing = false;
				_climbAttachBuffer = _climbInitBuffer;
			}
			else
			{
				_pCombat._blockControlBuffer = 0f;
				_ledgeGrabBuffer = 0.12f;
				Vector3 vector = _hit.point - base.transform.position;
				vector.y = 0f;
				base.transform.rotation = Quaternion.LookRotation(vector.normalized);
				if (!_parentGroundTypeObject._canLedgeClimb)
				{
					Init_JumpOffLedgeGrab(20f, -25.5f, 45.5f);
				}
				if (Input.GetKeyDown(InputControlManager.current._jump) || GamepadInput.GetButtonDown(GamepadButton.Button_South))
				{
					if (_parentGroundTypeObject._noForwardForceOnLedgeJump)
					{
						Init_JumpOffLedgeGrab(_ledgeJumpUpForce, 0f, 65f);
					}
					else
					{
						Init_JumpOffLedgeGrab(_ledgeJumpUpForce, _ledgeJumpForwardForce, 65f);
					}
				}
			}
		}
	}

	[ClientCallback]
	private void Init_JumpOffLedgeGrab(float _force, float _fowardForce, float _altGravity)
	{
		if (NetworkClient.active && _onLedge)
		{
			_pMove.Init_Jump(_force, _fowardForce, 0f, _altGravity, _useAnim: true);
			_onLedge = false;
			_isClimbing = false;
			_climbAttachBuffer = _climbInitBuffer;
		}
	}

	[ClientCallback]
	private void Handle_RopeClimbControl()
	{
		if (!NetworkClient.active || _player._currentPlayerCondition != PlayerCondition.ACTIVE || _player._bufferingStatus || _player._inChat || !_isClimbing || !_onRope)
		{
			return;
		}
		_pMove.Set_MovementAction(MovementAction.CLIMB);
		_pMove._attackForce = 0f;
		float vert_input = InputControlManager.current._vert_input;
		float horiz_input = InputControlManager.current._horiz_input;
		_isGoingDown = vert_input < 0f;
		if (vert_input > 0f)
		{
			_playerController.Move(new Vector3(0f, vert_input * _climbSpeed * Time.deltaTime, 0f));
		}
		else
		{
			_playerController.Move(new Vector3(0f, vert_input * _climbSpeed * 2.5f * Time.deltaTime, 0f));
		}
		if (vert_input == 0f)
		{
			if (_isMovingOnRope)
			{
				_pVisual.Cmd_ChangeClimbAnimationSpeed(0f);
				_isMovingOnRope = false;
			}
		}
		else if (!_isMovingOnRope && vert_input > 0f)
		{
			_pVisual.Cmd_ChangeClimbAnimationSpeed(1f);
			_isMovingOnRope = true;
		}
		if (vert_input < 0f)
		{
			_playerController.Move(0.45f * Time.deltaTime * base.transform.right);
		}
		if (horiz_input != 0f && vert_input == 0f)
		{
			_playerController.Move(_turnSpeed * horiz_input * Time.deltaTime * base.transform.right);
		}
		if ((bool)_ropeParent)
		{
			_ladderParentEndCaps = _ropeParent.GetComponentsInChildren<LadderEndCap>(includeInactive: true);
			for (int i = 0; i < _ladderParentEndCaps.Length; i++)
			{
				if (_ladderParentEndCaps[i]._bottomEndcap)
				{
					_ladderParentEndCaps[i].gameObject.SetActive(vert_input < 0f);
				}
				if (!_ladderParentEndCaps[i]._bottomEndcap)
				{
					_ladderParentEndCaps[i].gameObject.SetActive(vert_input > 0f);
				}
			}
			float num = Vector3.Distance(_ropeParent.position, base.transform.position);
			Vector3 vector = _ropeParent.position - base.transform.position;
			vector.y = 0f;
			base.transform.rotation = Quaternion.Lerp(base.transform.rotation, Quaternion.LookRotation(vector.normalized), 12.5f * Time.deltaTime);
			if (_playerController.collisionFlags == CollisionFlags.Above || num > _distanceToLadderDetect)
			{
				_playerController.Move(20.5f * Time.deltaTime * vector);
			}
		}
		if (Input.GetKeyDown(InputControlManager.current._jump) || GamepadInput.GetButtonDown(GamepadButton.Button_South) || _player._isHurt)
		{
			if (vert_input != 0f)
			{
				Init_JumpOffRope(15.5f, 15.5f);
			}
			else
			{
				Init_JumpOffRope(15.5f, -15.5f);
			}
		}
	}

	[ClientCallback]
	public void Init_JumpOffRope(float _upwardsJumpForce, float _forwardsForce)
	{
		if (!NetworkClient.active || _climbAttachBuffer > 0f || !_ropeParent || !_onRope)
		{
			return;
		}
		_isClimbing = false;
		for (int i = 0; i < _ladderParentEndCaps.Length; i++)
		{
			if (_ladderParentEndCaps[i]._bottomEndcap)
			{
				_ladderParentEndCaps[i].gameObject.SetActive(value: false);
			}
			if (!_ladderParentEndCaps[i]._bottomEndcap)
			{
				_ladderParentEndCaps[i].gameObject.SetActive(value: false);
			}
		}
		_ropeParent = null;
		_ladderParentEndCaps = new LadderEndCap[0];
		_isGoingDown = false;
		_onRope = false;
		_pMove.Init_Jump(_upwardsJumpForce, _forwardsForce, 0f, 45f, _useAnim: false);
		_pVisual.Cmd_ChangeClimbAnimationSpeed(1f);
		_climbAttachBuffer = _climbInitBuffer;
	}

	[ClientCallback]
	private void OnTriggerEnter(Collider other)
	{
		if (NetworkClient.active && base.isLocalPlayer && !_isClimbing && _player._currentPlayerAction == PlayerAction.IDLE && _player._currentPlayerCondition == PlayerCondition.ACTIVE && !_pCombat._isBlocking && !_pCombat._localBlockingInput && !_pCombat._isAirAttacked && !_pCombat._localAirAttackInput && !_pCombat._isChargingWeapon && other.gameObject.layer == LayerMask.NameToLayer("Rope") && !other.gameObject.GetComponent<LadderEndCap>() && !(_climbAttachBuffer > 0f))
		{
			_pCombat.Cmd_SheatheWeapon(WeaponSheatheCondition.PUT_AWAY);
			float vert_input = InputControlManager.current._vert_input;
			if (vert_input == 0f || vert_input < 0f)
			{
				_pVisual.Cmd_ChangeClimbAnimationSpeed(0f);
			}
			_climbAttachBuffer = _climbInitBuffer;
			_isClimbing = true;
			_onRope = true;
			_ropeParent = other.gameObject.transform;
		}
	}

	[ClientCallback]
	private void OnTriggerExit(Collider other)
	{
		if (NetworkClient.active && base.isLocalPlayer && _isClimbing && _onRope && !_pCombat._isAirAttacked && other.gameObject.layer == LayerMask.NameToLayer("Rope") && (bool)other.gameObject.GetComponent<LadderEndCap>())
		{
			_onRope = false;
			_isClimbing = false;
			_ropeParent = null;
			_isGoingDown = false;
			Init_JumpOffRope(15.5f, -12.5f);
			_climbAttachBuffer = _climbInitBuffer;
		}
	}

	public override bool Weaved()
	{
		return true;
	}
}
