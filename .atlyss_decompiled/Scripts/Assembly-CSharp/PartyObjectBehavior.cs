using System.Runtime.InteropServices;
using Mirror;
using UnityEngine;

public class PartyObjectBehavior : NetworkBehaviour
{
	[Header("Party Object")]
	[SyncVar]
	public Player _partyLeaderPlayer;

	[SyncVar]
	public int _maxPartyLimit = 5;

	[SyncVar]
	public bool _inPatternInstance;

	[Space]
	public readonly SyncList<Player> _syncPartyPeers = new SyncList<Player>();

	private bool _beginListIteration;

	private float _iterationBuffer = 1.25f;

	protected NetworkBehaviourSyncVar ____partyLeaderPlayerNetId;

	public Player Network_partyLeaderPlayer
	{
		get
		{
			return GetSyncVarNetworkBehaviour(____partyLeaderPlayerNetId, ref _partyLeaderPlayer);
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter_NetworkBehaviour(value, ref _partyLeaderPlayer, 1uL, null, ref ____partyLeaderPlayerNetId);
		}
	}

	public int Network_maxPartyLimit
	{
		get
		{
			return _maxPartyLimit;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _maxPartyLimit, 2uL, null);
		}
	}

	public bool Network_inPatternInstance
	{
		get
		{
			return _inPatternInstance;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _inPatternInstance, 4uL, null);
		}
	}

	private void Start()
	{
		_syncPartyPeers.Callback += OnPeersUpdated;
		for (int i = 0; i < _syncPartyPeers.Count; i++)
		{
			OnPeersUpdated(SyncList<Player>.Operation.OP_ADD, i, (Player)null, _syncPartyPeers[i]);
		}
	}

	private void OnPeersUpdated(SyncList<Player>.Operation op, int _i, Player _oldPlayer, Player _newPlayer)
	{
		switch (op)
		{
		}
	}

	private void Update()
	{
		Server_PartyObjectUpdate();
	}

	[ServerCallback]
	private void Server_PartyObjectUpdate()
	{
		if (!NetworkServer.active)
		{
			return;
		}
		if ((bool)Network_partyLeaderPlayer && (bool)Network_partyLeaderPlayer.Network_playerMapInstance)
		{
			Network_inPatternInstance = Network_partyLeaderPlayer.Network_playerMapInstance._patternInstance;
		}
		else
		{
			Network_inPatternInstance = false;
		}
		_beginListIteration = _iterationBuffer <= 0f;
		if (_iterationBuffer > 0f)
		{
			_iterationBuffer -= Time.deltaTime;
		}
		else
		{
			_iterationBuffer = 0f;
		}
		if (!_beginListIteration)
		{
			return;
		}
		if (Network_partyLeaderPlayer == null && _syncPartyPeers.Count > 0)
		{
			Network_partyLeaderPlayer = _syncPartyPeers[Random.Range(0, _syncPartyPeers.Count)];
		}
		if (_syncPartyPeers.Count <= 1)
		{
			NetworkServer.UnSpawn(base.gameObject);
			Object.Destroy(base.gameObject);
			return;
		}
		for (int i = 0; i < _syncPartyPeers.Count; i++)
		{
			if (_syncPartyPeers[i] == null)
			{
				_syncPartyPeers.RemoveAt(i);
				break;
			}
			if (_syncPartyPeers[i].Network_partyObject != this)
			{
				if (_syncPartyPeers[i] == Network_partyLeaderPlayer)
				{
					Network_partyLeaderPlayer = null;
				}
				_syncPartyPeers.RemoveAt(i);
				break;
			}
		}
	}

	public PartyObjectBehavior()
	{
		InitSyncObject(_syncPartyPeers);
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
			writer.WriteNetworkBehaviour(Network_partyLeaderPlayer);
			writer.WriteInt(_maxPartyLimit);
			writer.WriteBool(_inPatternInstance);
			return;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteNetworkBehaviour(Network_partyLeaderPlayer);
		}
		if ((base.syncVarDirtyBits & 2L) != 0L)
		{
			writer.WriteInt(_maxPartyLimit);
		}
		if ((base.syncVarDirtyBits & 4L) != 0L)
		{
			writer.WriteBool(_inPatternInstance);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize_NetworkBehaviour(ref _partyLeaderPlayer, null, reader, ref ____partyLeaderPlayerNetId);
			GeneratedSyncVarDeserialize(ref _maxPartyLimit, null, reader.ReadInt());
			GeneratedSyncVarDeserialize(ref _inPatternInstance, null, reader.ReadBool());
			return;
		}
		long num = (long)reader.ReadULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize_NetworkBehaviour(ref _partyLeaderPlayer, null, reader, ref ____partyLeaderPlayerNetId);
		}
		if ((num & 2L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _maxPartyLimit, null, reader.ReadInt());
		}
		if ((num & 4L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _inPatternInstance, null, reader.ReadBool());
		}
	}
}
