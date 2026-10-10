using System.Collections;
using System.Runtime.InteropServices;
using Mirror;
using UnityEngine;

public class DoorTrigger : NetworkBehaviour
{
	[Header("Door Trigger Parameters")]
	[SerializeField]
	private LerpPosition _doorLerpPosition;

	[SerializeField]
	private float _timeBeforeIndexSet = 0.8f;

	[SyncVar]
	public bool _isOpen;

	private bool _initIndexSwitch;

	public bool Network_isOpen
	{
		get
		{
			return _isOpen;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _isOpen, 1uL, null);
		}
	}

	[ServerCallback]
	public void Server_OpenDoor()
	{
		if (NetworkServer.active)
		{
			Network_isOpen = true;
		}
	}

	[ServerCallback]
	public void Server_CloseDoor()
	{
		if (NetworkServer.active)
		{
			Network_isOpen = false;
		}
	}

	protected override void OnValidate()
	{
		base.OnValidate();
		if ((bool)_doorLerpPosition)
		{
			base.name = "_doorTrigger (" + _doorLerpPosition.gameObject.name + ")";
		}
		else
		{
			base.name = "_doorTrigger (No Lerp Position Object Assigned)";
		}
	}

	private void Update()
	{
		Handle_DoorLerpPosition();
	}

	private void Handle_DoorLerpPosition()
	{
		if (!_doorLerpPosition)
		{
			return;
		}
		if (_isOpen)
		{
			if (_doorLerpPosition.CurrentIndex != 1)
			{
				StartCoroutine(Init_DoorIndex(1));
			}
		}
		else if (_doorLerpPosition.CurrentIndex != 0)
		{
			StartCoroutine(Init_DoorIndex(0));
		}
	}

	private IEnumerator Init_DoorIndex(int _i)
	{
		if (!_initIndexSwitch)
		{
			_initIndexSwitch = true;
			yield return new WaitForSeconds(_timeBeforeIndexSet);
			_doorLerpPosition.Apply_PositionIndex(_i);
			_initIndexSwitch = false;
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
			writer.WriteBool(_isOpen);
			return;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteBool(_isOpen);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize(ref _isOpen, null, reader.ReadBool());
			return;
		}
		long num = (long)reader.ReadULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _isOpen, null, reader.ReadBool());
		}
	}
}
