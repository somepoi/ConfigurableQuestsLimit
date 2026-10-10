using System;

[Serializable]
public class ServerHostSettings_Profile
{
	public string _globalNickname;

	public string _serverName;

	public string _serverPassword;

	public string _serverMotd;

	public int _maxAllowedConnections;

	public bool _streamMode;

	public BannedClientParameter[] _banList;

	public BannedClientParameter[] _mutedList;

	public string[] _hiddenLobbies;
}
