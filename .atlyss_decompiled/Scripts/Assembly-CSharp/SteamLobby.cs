using System.Collections;
using System.Collections.Generic;
using Mirror;
using Steamworks;
using UnityEngine;

public class SteamLobby : MonoBehaviour
{
	public static SteamLobby _current;

	protected Callback<LobbyCreated_t> _lobbyCreated;

	protected Callback<GameLobbyJoinRequested_t> _gameLobbyJoinRequested;

	protected Callback<GameRichPresenceJoinRequested_t> _gameRichLobbyJoinQuested;

	protected Callback<LobbyEnter_t> _lobbyEntered;

	protected Callback<LobbyKicked_t> _lobbyKicked;

	protected Callback<LobbyMatchList_t> _LobbyList;

	protected Callback<LobbyDataUpdate_t> _LobbyDataUpdated;

	public List<CSteamID> _lobbyIds = new List<CSteamID>();

	public ulong _currentLobbyID;

	private const string _hostAddressKey = "HostAddress";

	public CSteamID _cachedSteamID;

	public string GetFriendlyName(CSteamID id)
	{
		return SteamFriends.GetFriendPersonaName(id);
	}

	public Texture2D GetSteamImageAsTexture2D(int iImage)
	{
		Texture2D texture2D = null;
		if (SteamUtils.GetImageSize(iImage, out var pnWidth, out var pnHeight))
		{
			byte[] array = new byte[pnWidth * pnHeight * 4];
			if (SteamUtils.GetImageRGBA(iImage, array, (int)(pnWidth * pnHeight * 4)))
			{
				texture2D = new Texture2D((int)pnWidth, (int)pnHeight, TextureFormat.RGBA32, mipChain: false, linear: true);
				texture2D.LoadRawTextureData(array);
				texture2D.Apply();
			}
		}
		return texture2D;
	}

	private void Awake()
	{
		if (_current == null)
		{
			_current = this;
		}
	}

	private void Start()
	{
		Init_LobbyCallbacks();
	}

	private void Init_LobbyCallbacks()
	{
		if (SteamManager.Initialized)
		{
			_lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
			_gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequest);
			_gameRichLobbyJoinQuested = Callback<GameRichPresenceJoinRequested_t>.Create(OnJoinRichRequest);
			_lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
			_lobbyKicked = Callback<LobbyKicked_t>.Create(OnLobbyKicked);
			_LobbyList = Callback<LobbyMatchList_t>.Create(OnGetLobbyList);
			_LobbyDataUpdated = Callback<LobbyDataUpdate_t>.Create(OnGetLobbyData);
		}
	}

	public void HostLobby(ELobbyType lobbyType)
	{
		SteamMatchmaking.CreateLobby(lobbyType, AtlyssNetworkManager._current.maxConnections);
	}

	private void OnLobbyCreated(LobbyCreated_t _callback)
	{
		if (_callback.m_eResult != EResult.k_EResultOK)
		{
			HostConsole._current.Init_NetworkStatusMessage($"Lobby creation failed - {_callback.m_eResult}");
			return;
		}
		AtlyssNetworkManager._current.StartHost();
		bool flag = false;
		string text = AtlyssNetworkManager._current._serverName;
		string value = LobbyListManager._current._lobbyPasswordInput.text;
		if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
		{
			value = string.Empty;
		}
		if (LobbyListManager._current._lobbyTypeDropdown.value != 0)
		{
			value = string.Empty;
		}
		if (!string.IsNullOrWhiteSpace(value))
		{
			flag = true;
		}
		CSteamID cSteamID = new CSteamID(_callback.m_ulSteamIDLobby);
		if (text.Length > 20)
		{
			text = "ATLYSS Lobby";
		}
		SteamMatchmaking.SetLobbyMemberLimit(cSteamID, AtlyssNetworkManager._current.maxConnections);
		SteamMatchmaking.SetLobbyData(cSteamID, "HostAddress", SteamUser.GetSteamID().ToString());
		SteamMatchmaking.SetLobbyData(cSteamID, "name", text);
		string text2 = string.Empty;
		if (flag)
		{
			text2 = "1";
		}
		int num = GameManager._current.HasExternalTools();
		SteamMatchmaking.SetLobbyData(cSteamID, "password", text2 ?? "");
		SteamMatchmaking.SetLobbyData(cSteamID, "ver", Application.version ?? "");
		SteamMatchmaking.SetLobbyData(cSteamID, "lobbyType", $"{LobbyListManager._current._lobbyTypeDropdown.value}");
		SteamMatchmaking.SetLobbyData(cSteamID, "mod", $"{num}");
		AtlyssSteamLobbyTag atlyssSteamLobbyTag = LobbyListManager._current._hostLobbyRealm;
		string text3 = text;
		for (int i = 0; i < LobbyListManager._current._parseGaiaKeywords.Length; i++)
		{
			if (text3.Contains(LobbyListManager._current._parseGaiaKeywords[i]))
			{
				atlyssSteamLobbyTag = AtlyssSteamLobbyTag.GAIA;
			}
		}
		for (int j = 0; j < LobbyListManager._current._parseDualosKeywords.Length; j++)
		{
			if (text3.Contains(LobbyListManager._current._parseDualosKeywords[j]))
			{
				atlyssSteamLobbyTag = AtlyssSteamLobbyTag.DUALOS;
			}
		}
		for (int k = 0; k < LobbyListManager._current._parseNothKeywords.Length; k++)
		{
			if (text3.Contains(LobbyListManager._current._parseNothKeywords[k]))
			{
				atlyssSteamLobbyTag = AtlyssSteamLobbyTag.NOTH;
			}
		}
		for (int l = 0; l < LobbyListManager._current._parseLoodiaKeywords.Length; l++)
		{
			if (text3.Contains(LobbyListManager._current._parseLoodiaKeywords[l]))
			{
				atlyssSteamLobbyTag = AtlyssSteamLobbyTag.LOODIA;
			}
		}
		AtlyssNetworkManager._current._steamLobbyRealm = atlyssSteamLobbyTag;
		SteamMatchmaking.SetLobbyData(cSteamID, "lobbyFocus", $"{(int)atlyssSteamLobbyTag}");
		StartCoroutine(SetPingLocation());
		StartCoroutine(Set_LobbyReady(cSteamID));
		_cachedSteamID = cSteamID;
	}

	private IEnumerator SetPingLocation()
	{
		string pszBuf;
		do
		{
			SteamNetworkingUtils.GetLocalPingLocation(out var result);
			SteamNetworkingUtils.ConvertPingLocationToString(ref result, out pszBuf, 1024);
			yield return new WaitForSeconds(4f);
		}
		while (!(pszBuf != ""));
		SteamMatchmaking.SetLobbyData(new CSteamID(_currentLobbyID), "pingLocation", pszBuf);
	}

	private IEnumerator Set_LobbyReady(CSteamID _setSteamID)
	{
		yield return new WaitForSeconds(12.5f);
		SteamMatchmaking.SetLobbyData(_setSteamID, "isReady", "1");
	}

	public void JoinLobby(CSteamID _lobbyID)
	{
		StartCoroutine(Connect_ToLobby(_lobbyID));
		LobbyListManager._current.lobbyListWindow_element.isEnabled = false;
	}

	private void OnJoinRequest(GameLobbyJoinRequested_t _callback)
	{
		Init_LobbyJoinRequest(_callback.m_steamIDLobby);
	}

	private void OnJoinRichRequest(GameRichPresenceJoinRequested_t _callback)
	{
		Init_LobbyJoinRequest(_callback.m_steamIDFriend);
	}

	private void Init_LobbyJoinRequest(CSteamID _steamID)
	{
		if (MainMenuManager._current._mainMenuCondition == MainMenuCondition.In_Game)
		{
			HostConsole._current.Init_NetworkStatusMessage("Currently in-game. Restart to the Main Menu to join a lobby.");
			return;
		}
		HostConsole._current.Init_NetworkStatusMessage("Lobby queued... Select a character to join!");
		_cachedSteamID = _steamID;
		MainMenuManager._current._characterSelectManager._cachedSteamLobby = true;
		MainMenuManager._current.Set_MenuCondition(1);
	}

	private IEnumerator Connect_ToLobby(CSteamID _lobbyID)
	{
		string[] array = new string[12]
		{
			"Connecting...", "Attempting to commune...", "Waiting for a signal...", "Preaching... or was it pre-caching?", "Finding a way in...", "Fingers crossed...", "Anticipating...", "Holding out...", "Praying...", "Awaiting...",
			"Hoping...", "Managing..."
		};
		if (InputControlManager.current._setGamepadType == GamepadType.Keyboard)
		{
			HostConsole._current.Init_NetworkStatusMessage(array[Random.Range(0, array.Length)] + " (CTRL + F12 Cancels)");
		}
		else
		{
			HostConsole._current.Init_NetworkStatusMessage(array[Random.Range(0, array.Length)] + " (Back Button Cancels)");
		}
		yield return new WaitForSeconds(0.65f);
		SteamMatchmaking.JoinLobby(_lobbyID);
	}

	private void OnLobbyEntered(LobbyEnter_t _callback)
	{
		switch (_callback.m_EChatRoomEnterResponse)
		{
		case 1u:
			if (InputControlManager.current._setGamepadType == GamepadType.Keyboard)
			{
				HostConsole._current.Init_NetworkStatusMessage("Connection Successful! (CTRL + F12 Cancels)");
			}
			else
			{
				HostConsole._current.Init_NetworkStatusMessage("Connection Successful! (Back Button Cancels)");
			}
			break;
		case 2u:
			Reset_LobbyQueueParams("Cannot join - Lobby no longer exists.");
			return;
		case 3u:
			Reset_LobbyQueueParams("Cannot join - Denied.");
			return;
		case 4u:
			Reset_LobbyQueueParams("Cannot join - Lobby is full.");
			return;
		case 5u:
			Reset_LobbyQueueParams("Cannot join - Unexpected error.");
			return;
		case 6u:
			Reset_LobbyQueueParams("Cannot join - Denied.");
			return;
		case 7u:
			Reset_LobbyQueueParams("Cannot join - Denied.");
			return;
		case 8u:
			Reset_LobbyQueueParams("Cannot join - Denied.");
			return;
		case 9u:
			Reset_LobbyQueueParams("Cannot join - Denied.");
			return;
		case 10u:
			Reset_LobbyQueueParams("Cannot join - Denied.");
			return;
		case 11u:
			Reset_LobbyQueueParams("Cannot join - Denied.");
			return;
		case 15u:
			Reset_LobbyQueueParams("Cannot join - too many join attempts.");
			return;
		}
		string lobbyData = SteamMatchmaking.GetLobbyData(new CSteamID(_callback.m_ulSteamIDLobby), "HostAddress");
		_currentLobbyID = _callback.m_ulSteamIDLobby;
		if (!NetworkServer.active)
		{
			AtlyssNetworkManager._current.networkAddress = lobbyData;
			AtlyssNetworkManager._current.StartClient();
			MainMenuManager._current._characterSelectManager.Send_CharacterFile();
			LobbyListManager._current.lobbyListWindow_element.isEnabled = false;
			LobbyListManager._current._joinLobbyPasswordInput.text = string.Empty;
		}
	}

	private void Reset_LobbyQueueParams(string _errorMessage)
	{
		if (_errorMessage != string.Empty)
		{
			HostConsole._current.Init_NetworkStatusMessage(_errorMessage);
		}
		MainMenuManager._current.Set_MenuCondition(0);
		_cachedSteamID = default;
	}

	private void OnLobbyKicked(LobbyKicked_t _callback)
	{
		HostConsole._current.Init_NetworkStatusMessage("Disconnected.");
	}

	public void GetLobbiesList()
	{
		if (_lobbyIds.Count > 0)
		{
			_lobbyIds.Clear();
		}
		SteamMatchmaking.AddRequestLobbyListFilterSlotsAvailable(0);
		SteamMatchmaking.AddRequestLobbyListResultCountFilter(60);
		SteamMatchmaking.RequestLobbyList();
	}

	private void OnGetLobbyData(LobbyDataUpdate_t _result)
	{
		LobbyListManager._current.Init_SteamLobbyIteration(_lobbyIds, _result);
	}

	private void OnGetLobbyList(LobbyMatchList_t _result)
	{
		if (LobbyListManager._current._lobbyListEntries.Count > 0)
		{
			LobbyListManager._current.Clear_Lobbylist();
		}
		for (int i = 0; i < _result.m_nLobbiesMatching; i++)
		{
			CSteamID lobbyByIndex = SteamMatchmaking.GetLobbyByIndex(i);
			_lobbyIds.Add(lobbyByIndex);
			SteamMatchmaking.RequestLobbyData(lobbyByIndex);
		}
	}
}
