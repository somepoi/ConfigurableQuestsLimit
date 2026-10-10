using System.Runtime.InteropServices;
using Mirror;
using UnityEngine;

public class ServerInfoObject : NetworkBehaviour
{
	public static ServerInfoObject _current;

	[Header("Server Info Object")]
	[SyncVar]
	public string _serverName = string.Empty;

	[SyncVar]
	public string _serverMOTD = string.Empty;

	[SyncVar]
	public int _maxConnections = 2;

	[Space]
	[SyncVar]
	public AtlyssSteamLobbyTag _steamLobbyRealm;

	public string Network_serverName
	{
		get
		{
			return _serverName;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _serverName, 1uL, null);
		}
	}

	public string Network_serverMOTD
	{
		get
		{
			return _serverMOTD;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _serverMOTD, 2uL, null);
		}
	}

	public int Network_maxConnections
	{
		get
		{
			return _maxConnections;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _maxConnections, 4uL, null);
		}
	}

	public AtlyssSteamLobbyTag Network_steamLobbyRealm
	{
		get
		{
			return _steamLobbyRealm;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _steamLobbyRealm, 8uL, null);
		}
	}

	private void Awake()
	{
		_current = this;
	}

	private void Update()
	{
		if (NetworkServer.active && AtlyssNetworkManager._current._steamworksMode)
		{
			Network_steamLobbyRealm = AtlyssNetworkManager._current._steamLobbyRealm;
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
			writer.WriteString(_serverName);
			writer.WriteString(_serverMOTD);
			writer.WriteInt(_maxConnections);
			GeneratedNetworkCode._Write_AtlyssSteamLobbyTag(writer, _steamLobbyRealm);
			return;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteString(_serverName);
		}
		if ((base.syncVarDirtyBits & 2L) != 0L)
		{
			writer.WriteString(_serverMOTD);
		}
		if ((base.syncVarDirtyBits & 4L) != 0L)
		{
			writer.WriteInt(_maxConnections);
		}
		if ((base.syncVarDirtyBits & 8L) != 0L)
		{
			GeneratedNetworkCode._Write_AtlyssSteamLobbyTag(writer, _steamLobbyRealm);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize(ref _serverName, null, reader.ReadString());
			GeneratedSyncVarDeserialize(ref _serverMOTD, null, reader.ReadString());
			GeneratedSyncVarDeserialize(ref _maxConnections, null, reader.ReadInt());
			GeneratedSyncVarDeserialize(ref _steamLobbyRealm, null, GeneratedNetworkCode._Read_AtlyssSteamLobbyTag(reader));
			return;
		}
		long num = (long)reader.ReadULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _serverName, null, reader.ReadString());
		}
		if ((num & 2L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _serverMOTD, null, reader.ReadString());
		}
		if ((num & 4L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _maxConnections, null, reader.ReadInt());
		}
		if ((num & 8L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _steamLobbyRealm, null, GeneratedNetworkCode._Read_AtlyssSteamLobbyTag(reader));
		}
	}
}
