using System.Runtime.InteropServices;
using Mirror;
using UnityEngine;

public class DungeonKey : NetworkBehaviour
{
	private NetTrigger _interactTrigger;

	private PatternInstanceManager _dungeonInstance;

	[Header("Dungeon Key")]
	public GameObject _keyVisualMesh;

	public ParticleSystem _keyPickupEffect;

	[SyncVar(hook = "OnPickupKey")]
	public bool _isPickedUp;

	public bool Network_isPickedUp
	{
		get
		{
			return _isPickedUp;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _isPickedUp, 1uL, OnPickupKey);
		}
	}

	private void OnPickupKey(bool _oldBool, bool _newBool)
	{
		if (_newBool)
		{
			_keyPickupEffect.Play();
			_keyVisualMesh.SetActive(value: false);
		}
		else
		{
			_keyVisualMesh.SetActive(value: true);
		}
	}

	private void Awake()
	{
		_interactTrigger = GetComponent<NetTrigger>();
		PatternInstanceManager[] array = Object.FindObjectsOfType<PatternInstanceManager>();
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].gameObject.scene == base.gameObject.scene)
			{
				_dungeonInstance = array[i];
				break;
			}
		}
	}

	[ServerCallback]
	public void Init_PickupKey()
	{
		if (NetworkServer.active && !_isPickedUp)
		{
			PatternInstanceManager dungeonInstance = _dungeonInstance;
			dungeonInstance.Network_dungeonKeysAquired = dungeonInstance._dungeonKeysAquired + 1;
			Network_isPickedUp = true;
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
			writer.WriteBool(_isPickedUp);
			return;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteBool(_isPickedUp);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize(ref _isPickedUp, OnPickupKey, reader.ReadBool());
			return;
		}
		long num = (long)reader.ReadULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _isPickedUp, OnPickupKey, reader.ReadBool());
		}
	}
}
