using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Mirror;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RaycastPathFinder : NetworkBehaviour
{
	[CompilerGenerated]
	private sealed class _003CFear_Movement_003Ed__42 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public RaycastPathFinder _003C_003E4__this;

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
		public _003CFear_Movement_003Ed__42(int _003C_003E1__state)
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
			RaycastPathFinder raycastPathFinder = _003C_003E4__this;
			int num2;
			switch (num)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				if (raycastPathFinder._inFearMovement)
				{
					return false;
				}
				_003C_003E2__current = new WaitForSeconds(0.08f);
				_003C_003E1__state = 1;
				return true;
			case 1:
				_003C_003E1__state = -1;
				goto IL_0131;
			case 2:
				_003C_003E1__state = -1;
				goto IL_0080;
			case 3:
				{
					_003C_003E1__state = -1;
					goto IL_0131;
				}
				IL_0131:
				if (raycastPathFinder._inFearMovement)
				{
					if (raycastPathFinder._creep._isParryStaggered)
					{
						_003C_003E2__current = null;
						_003C_003E1__state = 2;
						return true;
					}
					goto IL_0080;
				}
				raycastPathFinder._disablePathing = false;
				raycastPathFinder._creep._isActionLocked = false;
				raycastPathFinder._creep.Network_ignoreAggroedTarget = false;
				raycastPathFinder._creep._wanderBehavior._disableWanderBehavior = true;
				raycastPathFinder._creep._creepPushCollider.gameObject.SetActive(value: true);
				return false;
				IL_0080:
				num2 = UnityEngine.Random.Range(0, 5);
				raycastPathFinder._creep.Rpc_CrossFadeAnim("Hurt_Heavy", 0f, 2);
				raycastPathFinder._disablePathing = num2 == 1;
				raycastPathFinder._creep._isActionLocked = true;
				raycastPathFinder._creep.Network_ignoreAggroedTarget = true;
				raycastPathFinder._creep._wanderBehavior._disableWanderBehavior = false;
				raycastPathFinder._creep._creepPushCollider.gameObject.SetActive(value: false);
				raycastPathFinder._creep._creepBehavior.Reset_Behavior();
				raycastPathFinder._creep._wanderBehavior.New_WanderPosition();
				_003C_003E2__current = new WaitForSeconds(UnityEngine.Random.Range(0.8f, 1.2f));
				_003C_003E1__state = 3;
				return true;
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

	private Creep _creep;

	private Rigidbody _rb;

	private PhysicsScene _physicsScene;

	[Header("Raycast Pathfinder")]
	public Transform _target;

	public Collider _pushCollider;

	[Header("Pathfinding")]
	public Transform _blockRayCaster;

	public float marginCollision = 3f;

	[Space]
	public bool _disablePathing;

	[SyncVar]
	public bool _dontLookAtTarget;

	public bool _blocked;

	private static Vector3 XZ = new Vector3(1f, 0f, 1f);

	private Vector3 blockDirection;

	private Vector3 escapeDirection;

	private float _blockedTime;

	private RaycastHit _hit;

	[Header("Pathing Nodes")]
	public Transform[] _pathFindNodes = new Transform[0];

	public float _timeBetweenNodes = 1f;

	public float _currentTimeBetweenNodes;

	public bool _canPatrolNodes = true;

	private int _currentNodeIndex;

	[Header("Movement Params")]
	public float _baseMovSpeed;

	public float _currentMovSpeed;

	public float _turnSpeed;

	[Header("Gravity")]
	public float gravityScale = 8f;

	public float _gravityBuffer;

	public bool _disableGravity;

	[Space]
	public float _groundDetection = 1.5f;

	public bool _isGrounded;

	private Vector3 _gravity = Vector3.zero;

	[Header("Target Stopping Distance")]
	public float _targetStoppingDistance = 6.5f;

	public float _distanceToTarget;

	public bool _isStoppedAndNearTarget;

	private bool _inFearMovement;

	private bool _initDash;

	private float _dashVelocity;

	public int CurrentNodeIndex
	{
		get
		{
			return _currentNodeIndex;
		}
		set
		{
			if (value < 0)
			{
				value = _pathFindNodes.Length - 1;
			}
			if (value > _pathFindNodes.Length - 1)
			{
				value = 0;
			}
			_currentNodeIndex = value;
		}
	}

	public bool Network_dontLookAtTarget
	{
		get
		{
			return _dontLookAtTarget;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _dontLookAtTarget, 1uL, null);
		}
	}

	private void Awake()
	{
		_creep = GetComponent<Creep>();
		_rb = GetComponent<Rigidbody>();
		_rb.isKinematic = !base.isServer;
	}

	private void Start()
	{
		_physicsScene = base.gameObject.scene.GetPhysicsScene();
	}

	private void Update()
	{
		Server_BufferParams();
	}

	[ServerCallback]
	private void Server_BufferParams()
	{
		if (NetworkServer.active)
		{
			if (_baseMovSpeed < 0f)
			{
				_baseMovSpeed = 0f;
			}
			if (_currentMovSpeed < 0f)
			{
				_currentMovSpeed = 0f;
			}
		}
	}

	[ServerCallback]
	private void FixedUpdate()
	{
		if (NetworkServer.active)
		{
			if ((bool)_creep && _creep._parryBuffer > 0f)
			{
				_initDash = false;
			}
			if ((bool)_creep && _creep._currentCreepCondition == CreepCondition.DEAD)
			{
				_currentTimeBetweenNodes = 0f;
			}
			Handle_FollowTarget();
			Handle_Gravity();
		}
	}

	[ServerCallback]
	public void Toggle_FearMovement(bool _isTrue)
	{
		if (NetworkServer.active)
		{
			if (_isTrue)
			{
				StartCoroutine(Fear_Movement());
			}
			_inFearMovement = _isTrue;
		}
	}

	[IteratorStateMachine(typeof(_003CFear_Movement_003Ed__42))]
	[ServerCallback]
	private IEnumerator Fear_Movement()
	{
		if (!NetworkServer.active)
		{
			return null;
		}
		return new _003CFear_Movement_003Ed__42(0)
		{
			_003C_003E4__this = this
		};
	}

	[ServerCallback]
	private void Handle_FollowTarget()
	{
		if (!NetworkServer.active)
		{
			return;
		}
		Handle_PathNodes();
		if (!_target)
		{
			_distanceToTarget = 0f;
			_isStoppedAndNearTarget = false;
			return;
		}
		if (_target.gameObject.scene != base.gameObject.scene)
		{
			_target = null;
			return;
		}
		if (_initDash)
		{
			_rb.AddRelativeForce(_dashVelocity * Vector3.forward, ForceMode.Acceleration);
		}
		Handle_WallBlockResetBuffer();
		Calculate_DistanceToTarget();
		LookAt_Target();
		MoveTowardsTarget();
		void Calculate_DistanceToTarget()
		{
			if (_disablePathing)
			{
				_isStoppedAndNearTarget = false;
			}
			else
			{
				_distanceToTarget = Vector3.Distance(base.transform.position, _target.position);
				_isStoppedAndNearTarget = (bool)_target && _distanceToTarget <= _targetStoppingDistance;
			}
		}
		void Handle_PathNodes()
		{
			if (_pathFindNodes.Length != 0 && _canPatrolNodes)
			{
				if (_isStoppedAndNearTarget && _currentTimeBetweenNodes > 0f)
				{
					_currentTimeBetweenNodes -= Time.deltaTime;
				}
				else if (_isStoppedAndNearTarget && _currentTimeBetweenNodes <= 0f)
				{
					_target = _pathFindNodes[_currentNodeIndex];
					CurrentNodeIndex++;
					_currentTimeBetweenNodes = _timeBetweenNodes;
				}
			}
		}
		void Handle_WallBlockResetBuffer()
		{
			if (_blocked)
			{
				_pushCollider.enabled = false;
				_blockedTime += Time.deltaTime;
			}
			else
			{
				_pushCollider.enabled = true;
				_blockedTime = 0f;
			}
			if (_blockedTime >= 6.5f)
			{
				_blocked = false;
			}
		}
		void LookAt_Target()
		{
			if (!(_target.position != Vector3.zero) && _isStoppedAndNearTarget && !_dontLookAtTarget)
			{
				Vector3 forward = _target.position - base.transform.position;
				forward.y = 0f;
				base.transform.rotation = Quaternion.Lerp(base.transform.rotation, Quaternion.LookRotation(forward), _turnSpeed * Time.deltaTime);
			}
		}
		void MoveTowardsTarget()
		{
			if (!_isStoppedAndNearTarget && (!_creep || !(_creep._parryBuffer > 0f)))
			{
				if (!_dontLookAtTarget)
				{
					Vector3 b = Vector3.Scale(RayPath(_target), XZ);
					base.transform.forward = Vector3.Lerp(base.transform.forward, b, _turnSpeed * Time.deltaTime);
				}
				if (!_disablePathing && (!_creep || !_creep.Network_aggroedEntity || !_creep._scriptCreep._dontFollowAggroTarget || !(_target == _creep.Network_aggroedEntity.transform)))
				{
					_rb.AddRelativeForce(_currentMovSpeed * Vector3.forward, ForceMode.Impulse);
				}
			}
		}
	}

	[ServerCallback]
	private Vector3 RayPath(Transform target)
	{
		if (!NetworkServer.active)
		{
			return default;
		}
		Vector3 position = _blockRayCaster.position;
		Vector3 normalized = (target.position - position).normalized;
		if (!_blocked)
		{
			if (_physicsScene.Raycast(position, normalized, out _hit, marginCollision, CastLayers.PathfinderMask))
			{
				if (_hit.transform == target)
				{
					return normalized;
				}
				if (_hit.distance < marginCollision)
				{
					_blocked = true;
					blockDirection = -_hit.normal;
					Vector3 vector = Vector3.Cross(_hit.normal, Vector3.up);
					if (Vector3.Angle(vector, normalized) < 90f)
					{
						escapeDirection = vector;
					}
					else
					{
						escapeDirection = -vector;
					}
					return escapeDirection;
				}
			}
			return normalized;
		}
		if (_physicsScene.Raycast(position, target.position, out _hit, float.PositiveInfinity, CastLayers.PathfinderMask) && _hit.transform == target)
		{
			_blocked = false;
			return normalized;
		}
		if (_physicsScene.Raycast(position, blockDirection, marginCollision, CastLayers.PathfinderMask))
		{
			if (_physicsScene.Raycast(position, escapeDirection, marginCollision, CastLayers.PathfinderMask))
			{
				escapeDirection = -escapeDirection;
			}
			return escapeDirection;
		}
		_blocked = false;
		if (_physicsScene.Raycast(position, normalized, marginCollision, CastLayers.PathfinderMask))
		{
			return escapeDirection;
		}
		_blocked = false;
		return normalized;
	}

	[ServerCallback]
	private void Refresh_Dash()
	{
		if (NetworkServer.active)
		{
			_initDash = false;
		}
	}

	[ServerCallback]
	public void VelocityDash_Forward(float _velocityAmount)
	{
		if (NetworkServer.active && (!_creep || !(_creep._parryBuffer > 0f)))
		{
			_dashVelocity = _velocityAmount;
			_initDash = true;
			Invoke("Refresh_Dash", 0.35f);
		}
	}

	[ServerCallback]
	private void ReEnable_Pathfinding()
	{
		if (NetworkServer.active)
		{
			_disablePathing = false;
		}
	}

	[ServerCallback]
	public void Disable_Pathfinding(float _timer)
	{
		if (NetworkServer.active)
		{
			_disablePathing = true;
			if (IsInvoking("ReEnable_Pathfinding"))
			{
				CancelInvoke("ReEnable_Pathfinding");
			}
			Invoke("ReEnable_Pathfinding", _timer);
		}
	}

	[ServerCallback]
	private void ReEnable_LookRotation()
	{
		if (NetworkServer.active)
		{
			Network_dontLookAtTarget = false;
		}
	}

	[ServerCallback]
	public void Disable_LookRotation(float _timer)
	{
		if (NetworkServer.active)
		{
			Network_dontLookAtTarget = true;
			if (IsInvoking("ReEnable_LookRotation"))
			{
				CancelInvoke("ReEnable_LookRotation");
			}
			Invoke("ReEnable_LookRotation", _timer);
		}
	}

	[ServerCallback]
	private void Handle_Gravity()
	{
		if (!NetworkServer.active || _rb.isKinematic || _disableGravity)
		{
			return;
		}
		_gravity = GameManager.GlobalGravity * gravityScale * Vector3.up;
		if (_physicsScene.Raycast(base.transform.position, Vector3.down, out var hitInfo, float.PositiveInfinity, CastLayers.GroundMask))
		{
			if (hitInfo.normal != Vector3.up)
			{
				_groundDetection = 0.7f;
			}
			else
			{
				_groundDetection = 0.05f;
			}
			Vector3 point = hitInfo.point;
			float num = Vector3.Distance(base.transform.position, point);
			_isGrounded = num <= _groundDetection;
		}
		if (_gravityBuffer > 0f)
		{
			_gravityBuffer -= Time.deltaTime;
		}
		else
		{
			_gravityBuffer = 0f;
		}
		if (!_isGrounded && _gravityBuffer <= 0f)
		{
			_rb.AddForce(_gravity, ForceMode.Impulse);
		}
	}

	public override bool Weaved()
	{
		return true;
	}

	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteBool(_dontLookAtTarget);
			return;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteBool(_dontLookAtTarget);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize(ref _dontLookAtTarget, null, reader.ReadBool());
			return;
		}
		long num = (long)reader.ReadULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _dontLookAtTarget, null, reader.ReadBool());
		}
	}
}
