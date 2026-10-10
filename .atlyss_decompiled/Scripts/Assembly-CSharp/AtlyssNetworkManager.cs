using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Mirror;
using Steamworks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AtlyssNetworkManager : NetworkManager
{
	public static AtlyssNetworkManager _current;

	[HideInInspector]
	public MapInstancingManager _mapInstanceManager;

	[HideInInspector]
	public ProfileDataSender _profileDataSender;

	[Header("Server Information")]
	public string _serverName = "ATLYSS Server";

	public string _serverPassword = string.Empty;

	public string _sentPassword = string.Empty;

	public string _serverMotd = string.Empty;

	public AtlyssSteamLobbyTag _steamLobbyRealm;

	private ServerInfoObject _cachedInfoObject;

	public List<BannedClientParameter> _bannedClientList = new List<BannedClientParameter>();

	public List<BannedClientParameter> _mutedClientList = new List<BannedClientParameter>();

	[Space]
	public bool _soloMode;

	public bool _serverMode;

	public bool _steamworksMode;

	[Header("Headless Console")]
	private string _consoleInputBuffer = "";

	private bool _allowConsoleInput;

	[Header("Network Object Registry")]
	public NetworkObjectLibrary _networkObjectLibrary;

	public override void Awake()
	{
		base.Awake();
		_current = this;
		_mapInstanceManager = GetComponent<MapInstancingManager>();
		_profileDataSender = GetComponent<ProfileDataSender>();
		Invoke("Clear_ConsoleInit", 1.35f);
	}

	public override void Update()
	{
		base.Update();
		Handle_NetworkManagerControl();
		Console_GetInput();
	}

	private void Handle_NetworkManagerControl()
	{
		if (!NetworkServer.dontListen && NetworkServer.active)
		{
			_soloMode = false;
		}
		if (NetworkServer.active && !NetworkClient.active && _serverMode && AudioListener.volume != 0f)
		{
			AudioListener.volume = 0f;
		}
		Handle_DisconnectHotkey();
		void Handle_DisconnectHotkey()
		{
			if ((SettingsManager._current._persistDisconnectHotkey.isOn || LoadSceneManager._current._isLoadingScreen) && !_soloMode)
			{
				bool flag = Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.F12);
				if (InputControlManager.current._setGamepadType != GamepadType.Keyboard && LoadSceneManager._current._isLoadingScreen)
				{
					flag = GamepadInput.GetButtonDown(GamepadButton.Back);
				}
				if (flag)
				{
					if (NetworkServer.active && NetworkClient.isConnected)
					{
						if (_steamworksMode)
						{
							SteamMatchmaking.LeaveLobby((CSteamID)SteamLobby._current._currentLobbyID);
						}
						StopHost();
					}
					else if (NetworkClient.isConnected)
					{
						if (_steamworksMode)
						{
							SteamMatchmaking.LeaveLobby((CSteamID)SteamLobby._current._currentLobbyID);
						}
						StopClient();
					}
				}
			}
		}
	}

	private void Clear_ConsoleInit()
	{
		if (NetworkServer.active)
		{
			Console.Clear();
			HostConsole._current.New_InitalizeServerMessage();
			_allowConsoleInput = true;
		}
	}

	private void ProcessInput()
	{
		HostConsole._current.Init_ServerMessage(_consoleInputBuffer);
	}

	private void Console_GetInput()
	{
		if (!Console.KeyAvailable || !_allowConsoleInput)
		{
			return;
		}
		AudioListener.volume = 0f;
		ConsoleKeyInfo consoleKeyInfo = Console.ReadKey();
		switch (consoleKeyInfo.Key)
		{
		case ConsoleKey.Backspace:
			if (_consoleInputBuffer.Length > 0)
			{
				Console.Write("\b \b");
				_consoleInputBuffer = _consoleInputBuffer.Substring(0, _consoleInputBuffer.Length - 1);
			}
			break;
		case ConsoleKey.Enter:
			if (_consoleInputBuffer.Length > 0)
			{
				Console.WriteLine();
				_consoleInputBuffer = _consoleInputBuffer.Trim();
				ProcessInput();
				_consoleInputBuffer = "";
			}
			break;
		default:
			if (Regex.IsMatch(consoleKeyInfo.KeyChar.ToString(), "[ -~]"))
			{
				Console.Write(consoleKeyInfo.KeyChar);
				_consoleInputBuffer += consoleKeyInfo.KeyChar;
			}
			break;
		}
	}

	public override void OnStartServer()
	{
		base.OnStartServer();
		_profileDataSender.Init_RegisterProfileMessage();
		HostConsole._current.New_InitalizeServerMessage();
		Init_MultiplayerHostParams();
		void Init_MultiplayerHostParams()
		{
			if (!_soloMode)
			{
				HostConsole._current.Init_NetworkStatusMessage(_serverName + " successfully hosted!");
				GameObject obj = UnityEngine.Object.Instantiate(_networkObjectLibrary._serverInfoObject);
				obj.transform.position = Vector3.zero;
				obj.transform.localScale = Vector3.one;
				ServerInfoObject component = obj.GetComponent<ServerInfoObject>();
				component.Network_serverName = _serverName;
				component.Network_maxConnections = maxConnections;
				component.Network_serverMOTD = _serverMotd;
				component.Network_steamLobbyRealm = _steamLobbyRealm;
				_cachedInfoObject = component;
				NetworkServer.Spawn(obj);
			}
		}
	}

	public override void OnStopServer()
	{
		base.OnStopServer();
		if (!_soloMode)
		{
			HostConsole._current.Init_NetworkStatusMessage("Stopped hosting " + _serverName + ".");
			if (_steamworksMode)
			{
				SteamMatchmaking.LeaveLobby(SteamUser.GetSteamID());
			}
			if ((bool)_cachedInfoObject)
			{
				NetworkServer.Destroy(_cachedInfoObject.gameObject);
				_cachedInfoObject = null;
			}
		}
		_mapInstanceManager.Init_UnloadAllSubScenes();
		MainMenuManager._current.Reset_MainMenuConditions();
		AudioListener.volume = 1f;
		HostConsole._current.Clear_Log();
		NetworkServer.ClearHandlers();
	}

	public override void OnStartClient()
	{
		base.OnStartClient();
		Register_NetworkLibrary();
		void Register_NetworkLibrary()
		{
			NetworkObjectLibrary networkObjectLibrary = _networkObjectLibrary;
			NetworkClient.RegisterPrefab(networkObjectLibrary._networkItemObject);
			NetworkClient.RegisterPrefab(networkObjectLibrary._creepObject);
			NetworkClient.RegisterPrefab(networkObjectLibrary._partyObject);
			NetworkClient.RegisterPrefab(networkObjectLibrary._serverInfoObject);
		}
	}

	public override void OnStopClient()
	{
		base.OnStopClient();
		if (!_soloMode)
		{
			HostConsole._current.Init_NetworkStatusMessage("Disconnected.");
			if (_steamworksMode)
			{
				SteamMatchmaking.LeaveLobby((CSteamID)SteamLobby._current._currentLobbyID);
			}
		}
		GameManager._current.Refresh_StaticBonusSkills();
		MainMenuManager._current.Reset_MainMenuConditions();
		LobbyListManager._current._selectedLobby = false;
		NetworkClient.ClearSpawners();
		NetworkClient.DestroyAllClientObjects();
		SceneManager.LoadScene(1);
	}

	public override void OnServerConnect(NetworkConnectionToClient _conn)
	{
		base.OnServerConnect(_conn);
	}

	public override void OnServerDisconnect(NetworkConnectionToClient _conn)
	{
		base.OnServerDisconnect(_conn);
		HostConsole._current.Destroy_PeerListEntry(_conn.connectionId);
	}
}
