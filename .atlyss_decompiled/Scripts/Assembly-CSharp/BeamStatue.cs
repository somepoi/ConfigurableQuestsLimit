using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Mirror;
using UnityEngine;

public class BeamStatue : NetworkBehaviour
{
	[CompilerGenerated]
	private sealed class _003CInit_BeamBehavior_003Ed__28 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public BeamStatue _003C_003E4__this;

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
		public _003CInit_BeamBehavior_003Ed__28(int _003C_003E1__state)
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
			BeamStatue beamStatue = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				beamStatue._beginInitBeam = true;
				_003C_003E2__current = new WaitForSeconds(beamStatue._timeBeforeBeamInit);
				_003C_003E1__state = 1;
				return true;
			case 1:
				_003C_003E1__state = -1;
				beamStatue.Network_isShootingBeam = true;
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

	[Header("Dungeon Entity - Beam Statue")]
	[SyncVar(hook = "On_FocusedTargetChange")]
	public Player _focusedTarget;

	[SerializeField]
	private SphereCollider _detectionCollider;

	[SerializeField]
	private float _detectionForwardMultiplier = 6.5f;

	[SerializeField]
	private Vector3 _detectionAdjust = Vector3.zero;

	[SerializeField]
	private Transform _eyeballTransform;

	[SerializeField]
	private Transform _eyeballBaseTransform;

	[SerializeField]
	private RotateObject _eyeBaseRotateObject;

	[SerializeField]
	private RotateObject _collarRotateObject;

	[SerializeField]
	private float _lookAtTargetAdjust = -15.5f;

	private readonly List<Player> _playersWithinRadius = new List<Player>();

	[Space]
	[Header("Beam Parameters")]
	[SerializeField]
	private Vector3 _beamHitPoint = Vector3.zero;

	[SerializeField]
	private CombatCollider _beamHitBox;

	[SerializeField]
	private ParticleSystem _alertEffect;

	[SerializeField]
	private ParticleSystem _beamInitEffect;

	[SerializeField]
	private ParticleSystem _beamGroundCollideEffect;

	[SerializeField]
	private LineRenderer _beamLineRender;

	[SerializeField]
	private float _timeBeforeBeamInit = 0.98f;

	[SerializeField]
	private float _preBeamRotationSpeed = 18.5f;

	[SerializeField]
	private float _beamRotationSpeed = 7.45f;

	[SyncVar]
	public bool _isShootingBeam;

	private float _currentRotationSpeed;

	private bool _beginInitBeam;

	[Header("Audio")]
	[SerializeField]
	private AudioSource _aSrc_proneLoop;

	[SerializeField]
	private AudioSource _aSrc_beamLoop;

	[SerializeField]
	private AudioSource _aSrc_alert;

	protected NetworkBehaviourSyncVar ____focusedTargetNetId;

	public Player Network_focusedTarget
	{
		get
		{
			return GetSyncVarNetworkBehaviour(____focusedTargetNetId, ref _focusedTarget);
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter_NetworkBehaviour(value, ref _focusedTarget, 1uL, On_FocusedTargetChange, ref ____focusedTargetNetId);
		}
	}

	public bool Network_isShootingBeam
	{
		get
		{
			return _isShootingBeam;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _isShootingBeam, 2uL, null);
		}
	}

	private void On_FocusedTargetChange(Player _old, Player _new)
	{
		if (_new != null)
		{
			_alertEffect.Play();
			_aSrc_alert.Play();
		}
	}

	private void Update()
	{
		if (!_isShootingBeam)
		{
			_currentRotationSpeed = _preBeamRotationSpeed;
		}
		else
		{
			_currentRotationSpeed = _beamRotationSpeed;
		}
		if ((bool)Player._mainPlayer)
		{
			bool mute = Player._mainPlayer._bufferingStatus || (Player._mainPlayer._isHostPlayer && Player._mainPlayer.gameObject.scene != base.gameObject.scene);
			_aSrc_proneLoop.mute = mute;
			_aSrc_beamLoop.mute = mute;
			_aSrc_alert.mute = mute;
		}
		Vector3 vector = base.transform.InverseTransformDirection(_eyeballTransform.forward);
		_detectionCollider.center = Vector3.zero + vector * _detectionForwardMultiplier + _detectionAdjust;
		Handle_FocusedTargetBehavior();
		Handle_BeamBehavior();
	}

	private void Handle_FocusedTargetBehavior()
	{
		_eyeBaseRotateObject.enabled = !Network_focusedTarget;
		if (!Network_focusedTarget)
		{
			if (base.isServer)
			{
				StopAllCoroutines();
				Network_isShootingBeam = false;
			}
			_eyeballTransform.localRotation = Quaternion.Lerp(_eyeballTransform.localRotation, _eyeballBaseTransform.localRotation, 18.5f * Time.deltaTime);
			_beginInitBeam = false;
			_beamGroundCollideEffect.Stop();
			_aSrc_proneLoop.volume = Mathf.Lerp(_aSrc_proneLoop.volume, 0.35f, 1.25f * Time.deltaTime);
			return;
		}
		if (base.isServer)
		{
			Handle_FocusedTargetPriority();
			if (!Network_focusedTarget)
			{
				return;
			}
		}
		_aSrc_proneLoop.volume = Mathf.Lerp(_aSrc_proneLoop.volume, 0f, 3.25f * Time.deltaTime);
		Vector3 forward = Network_focusedTarget.transform.position - base.transform.position;
		forward.y = 0f;
		Vector3 forward2 = Network_focusedTarget.transform.position - base.transform.position;
		forward2.y -= _lookAtTargetAdjust;
		if (base.isServer)
		{
			_eyeballBaseTransform.rotation = Quaternion.Lerp(_eyeballBaseTransform.rotation, Quaternion.LookRotation(forward), _preBeamRotationSpeed * Time.deltaTime);
		}
		_eyeballTransform.rotation = Quaternion.Lerp(_eyeballTransform.rotation, Quaternion.LookRotation(forward2), _currentRotationSpeed * Time.deltaTime);
		if (!_beginInitBeam && base.isServer)
		{
			StartCoroutine(Init_BeamBehavior());
		}
		void Handle_FocusedTargetPriority()
		{
			if ((bool)Network_focusedTarget)
			{
				if (Network_focusedTarget._currentPlayerCondition != PlayerCondition.ACTIVE)
				{
					Init_RemoveFocusedTarget();
				}
				else if (Network_focusedTarget.GetComponent<PlayerVisual>()._forceHidden)
				{
					Init_RemoveFocusedTarget();
				}
			}
		}
		void Init_RemoveFocusedTarget()
		{
			if (_playersWithinRadius.Contains(Network_focusedTarget))
			{
				_playersWithinRadius.Remove(Network_focusedTarget);
			}
			if (_playersWithinRadius.Count > 0)
			{
				Network_focusedTarget = _playersWithinRadius[0];
			}
			else
			{
				Network_focusedTarget = null;
			}
		}
	}

	[IteratorStateMachine(typeof(_003CInit_BeamBehavior_003Ed__28))]
	[ServerCallback]
	private IEnumerator Init_BeamBehavior()
	{
		if (!NetworkServer.active)
		{
			return null;
		}
		return new _003CInit_BeamBehavior_003Ed__28(0)
		{
			_003C_003E4__this = this
		};
	}

	private void Handle_BeamBehavior()
	{
		_beamHitBox.gameObject.SetActive(_isShootingBeam);
		_beamLineRender.enabled = _isShootingBeam;
		if (!_isShootingBeam)
		{
			_beamHitPoint = _eyeballTransform.transform.position;
			_aSrc_beamLoop.Stop();
			_beamInitEffect.Stop();
			_beamLineRender.startWidth = 0f;
			_collarRotateObject.endRotY = 4f;
			return;
		}
		if (!_beamInitEffect.isPlaying)
		{
			_beamInitEffect.Play();
		}
		_collarRotateObject.endRotY = -45.5f;
		if (!_aSrc_beamLoop.isPlaying)
		{
			_aSrc_beamLoop.Play();
		}
		PhysicsScene physicsScene = base.gameObject.scene.GetPhysicsScene();
		LayerMask layerMask = LayerMask.GetMask("Ground", "Player_TriggerCollider");
		if (physicsScene.Raycast(_eyeballTransform.position, _eyeballTransform.transform.forward, out var hitInfo, float.PositiveInfinity, layerMask))
		{
			_beamHitPoint = hitInfo.point;
		}
		_beamLineRender.startWidth = Mathf.Lerp(_beamLineRender.startWidth, 1.5f, 4.5f * Time.deltaTime);
		_beamLineRender.SetPosition(0, _eyeballTransform.transform.position);
		_beamLineRender.SetPosition(1, _beamHitPoint);
		_beamHitBox.transform.position = _beamHitPoint;
		_beamGroundCollideEffect.gameObject.transform.position = _beamHitPoint + Vector3.up * 0.65f;
		if (!_beamGroundCollideEffect.isPlaying)
		{
			_beamGroundCollideEffect.Play();
		}
		Handle_BeamServerParams();
	}

	[ServerCallback]
	private void Handle_BeamServerParams()
	{
		if (NetworkServer.active && (bool)Network_focusedTarget)
		{
			_beamHitBox.transform.rotation = _eyeballTransform.rotation;
		}
	}

	[ServerCallback]
	private void OnTriggerStay(Collider other)
	{
		if (!NetworkServer.active || other.gameObject.scene != base.gameObject.scene || !other.gameObject.GetComponentInParent<Player>())
		{
			return;
		}
		Player componentInParent = other.gameObject.GetComponentInParent<Player>();
		if (componentInParent._currentPlayerCondition != PlayerCondition.ACTIVE || componentInParent.GetComponent<PlayerVisual>()._forceHidden)
		{
			return;
		}
		PhysicsScene physicsScene = base.gameObject.scene.GetPhysicsScene();
		LayerMask layerMask = LayerMask.GetMask("Ground", "Player_TriggerCollider");
		Vector3 direction = other.gameObject.transform.position - base.transform.position;
		if (!physicsScene.Raycast(base.transform.position, direction, out var hitInfo, float.PositiveInfinity, layerMask) || (bool)hitInfo.collider.gameObject.GetComponentInParent<Player>())
		{
			if (!_playersWithinRadius.Contains(componentInParent))
			{
				_playersWithinRadius.Add(componentInParent);
			}
			if (!Network_focusedTarget)
			{
				Network_focusedTarget = componentInParent;
			}
		}
	}

	[ServerCallback]
	private void OnTriggerExit(Collider other)
	{
		if (!NetworkServer.active || other.gameObject.scene != base.gameObject.scene || !other.gameObject.GetComponentInParent<Player>())
		{
			return;
		}
		Player componentInParent = other.gameObject.GetComponentInParent<Player>();
		if (_playersWithinRadius.Contains(componentInParent))
		{
			_playersWithinRadius.Remove(componentInParent);
		}
		if (_playersWithinRadius.Count > 0)
		{
			if (Network_focusedTarget != _playersWithinRadius[0])
			{
				_beginInitBeam = false;
				Network_isShootingBeam = false;
				Network_focusedTarget = _playersWithinRadius[0];
			}
		}
		else
		{
			_beginInitBeam = false;
			Network_isShootingBeam = false;
			Network_focusedTarget = null;
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
			writer.WriteNetworkBehaviour(Network_focusedTarget);
			writer.WriteBool(_isShootingBeam);
			return;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteNetworkBehaviour(Network_focusedTarget);
		}
		if ((base.syncVarDirtyBits & 2L) != 0L)
		{
			writer.WriteBool(_isShootingBeam);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize_NetworkBehaviour(ref _focusedTarget, On_FocusedTargetChange, reader, ref ____focusedTargetNetId);
			GeneratedSyncVarDeserialize(ref _isShootingBeam, null, reader.ReadBool());
			return;
		}
		long num = (long)reader.ReadULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize_NetworkBehaviour(ref _focusedTarget, On_FocusedTargetChange, reader, ref ____focusedTargetNetId);
		}
		if ((num & 2L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _isShootingBeam, null, reader.ReadBool());
		}
	}
}
