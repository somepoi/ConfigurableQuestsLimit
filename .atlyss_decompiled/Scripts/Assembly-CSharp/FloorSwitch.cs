using System.Collections.Generic;
using System.Runtime.InteropServices;
using Mirror;
using UnityEngine;

public class FloorSwitch : NetworkBehaviour
{
	private enum FloorSwitchType : byte
	{
		PLAYER_STEP = 0,
		PUSHBLOCK_STEP = 1,
		ANY_STEP = 2
	}

	[Header("Dungeon Floor Switch")]
	[SerializeField]
	private NetTrigger _netTrigger;

	[SerializeField]
	private FloorSwitchType _floorSwitchType;

	[SerializeField]
	private Animator _floorSwitchAnim;

	[SerializeField]
	private ParticleSystem _switchParticle;

	[Space]
	[SerializeField]
	private float _setPlayerStepBuffer = 0.6f;

	[SerializeField]
	private float _resetTimer;

	[SyncVar(hook = "OnFloorSwitchPressed")]
	public bool _isPressed;

	[SerializeField]
	private bool _unpressOnTriggerExit = true;

	private readonly List<Player> _peersOnSwitch = new List<Player>();

	private float _playerStepBuffer;

	[Header("Audio")]
	[SerializeField]
	private AudioSource _aSrc_unpressed;

	[SerializeField]
	private AudioSource _aSrc_pressed;

	public bool Network_isPressed
	{
		get
		{
			return _isPressed;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _isPressed, 1uL, OnFloorSwitchPressed);
		}
	}

	private void OnFloorSwitchPressed(bool _old, bool _new)
	{
		if ((!Player._mainPlayer || !Player._mainPlayer._bufferingStatus) && (!Player._mainPlayer || !Player._mainPlayer._isHostPlayer || !(Player._mainPlayer.gameObject.scene != base.gameObject.scene)))
		{
			if (_new)
			{
				_aSrc_pressed.Play();
				_switchParticle.Play();
			}
			else
			{
				_aSrc_unpressed.Play();
				_switchParticle.Play();
			}
		}
	}

	private void Update()
	{
		_floorSwitchAnim.SetBool("_isPressed", _isPressed);
		if ((bool)_netTrigger)
		{
			if (_isPressed && !_netTrigger._triggerEnabled)
			{
				_netTrigger.Init_EnableTrigger();
			}
			if (!_isPressed && _netTrigger._triggerEnabled)
			{
				_netTrigger.Init_DisableTrigger();
			}
		}
		if (_peersOnSwitch.Count <= 0 && _isPressed && base.isServer && _floorSwitchType == FloorSwitchType.PLAYER_STEP)
		{
			Network_isPressed = false;
		}
	}

	[ServerCallback]
	private void OnTriggerStay(Collider other)
	{
		if (!NetworkServer.active || _isPressed || (!other.gameObject.GetComponentInParent<Player>() && _floorSwitchType == FloorSwitchType.PLAYER_STEP) || (!other.gameObject.GetComponent<PushBlock>() && _floorSwitchType == FloorSwitchType.PUSHBLOCK_STEP) || other.gameObject.scene != base.gameObject.scene)
		{
			return;
		}
		if ((bool)other.gameObject.GetComponentInParent<Player>() && _floorSwitchType == FloorSwitchType.PLAYER_STEP)
		{
			if (!_peersOnSwitch.Contains(other.gameObject.GetComponentInParent<Player>()))
			{
				_peersOnSwitch.Add(other.gameObject.GetComponentInParent<Player>());
			}
			if (_playerStepBuffer < _setPlayerStepBuffer)
			{
				_playerStepBuffer += Time.deltaTime;
			}
			else
			{
				Network_isPressed = true;
			}
		}
		if ((bool)other.gameObject.GetComponentInParent<PushBlock>() && _floorSwitchType == FloorSwitchType.PUSHBLOCK_STEP)
		{
			Network_isPressed = true;
		}
	}

	[ServerCallback]
	private void OnTriggerExit(Collider other)
	{
		if (NetworkServer.active && _isPressed && _unpressOnTriggerExit && ((bool)other.gameObject.GetComponentInParent<Player>() || _floorSwitchType != FloorSwitchType.PLAYER_STEP) && ((bool)other.gameObject.GetComponent<PushBlock>() || _floorSwitchType != FloorSwitchType.PUSHBLOCK_STEP) && !(other.gameObject.scene != base.gameObject.scene))
		{
			if ((bool)other.gameObject.GetComponentInParent<Player>() && _floorSwitchType == FloorSwitchType.PLAYER_STEP && _peersOnSwitch.Contains(other.gameObject.GetComponentInParent<Player>()))
			{
				_peersOnSwitch.Remove(other.gameObject.GetComponentInParent<Player>());
			}
			if ((bool)other.gameObject.GetComponentInParent<PushBlock>() && _floorSwitchType == FloorSwitchType.PUSHBLOCK_STEP)
			{
				Network_isPressed = false;
			}
			_playerStepBuffer = 0f;
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
			writer.WriteBool(_isPressed);
			return;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteBool(_isPressed);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize(ref _isPressed, OnFloorSwitchPressed, reader.ReadBool());
			return;
		}
		long num = (long)reader.ReadULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _isPressed, OnFloorSwitchPressed, reader.ReadBool());
		}
	}
}
