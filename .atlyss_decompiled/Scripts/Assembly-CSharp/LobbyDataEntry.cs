using Steamworks;
using UnityEngine;
using UnityEngine.UI;

public class LobbyDataEntry : MonoBehaviour
{
	[Header("Lobby Data")]
	public CSteamID lobbyID;

	public AtlyssSteamLobbyTag _atlyssSteamLobbyTag;

	[Space]
	public string _lobbyName;

	public string _lobbyVersion;

	public string _lobbyPlayerCount;

	public string _lobbyMaxPlayers;

	public string _lobbyPing;

	public bool _moddedLobby;

	public bool passwordLobby;

	[Header("Lobby UI Elements")]
	public Image _lobbyTagIcon;

	public GameObject _moddedLobbyTag;

	public Sprite[] _lobbyTagSprites = new Sprite[0];

	[Space]
	public Button _joinLobbyButton;

	public Text _joinLobbyButtonText;

	public Button _blacklistLobbyButton;

	public Text _lobbyNameText;

	public Text _lobbyPlayerCountText;

	public Text _lobbyPingText;

	private void Awake()
	{
		_joinLobbyButton.onClick.AddListener(Init_JoinLobby);
		_blacklistLobbyButton.onClick.AddListener(Init_BlacklistLobbyID);
	}

	private void Update()
	{
		if (_lobbyName.Length > 20)
		{
			_lobbyName = "Null";
		}
		_lobbyNameText.text = _lobbyName;
	}

	public void SetLobbyData()
	{
		if (_lobbyName == string.Empty)
		{
			_lobbyNameText.text = "Untitled Lobby";
		}
		else
		{
			_lobbyNameText.text = _lobbyName;
		}
		_lobbyPlayerCountText.text = "Players: " + _lobbyPlayerCount + " / " + _lobbyMaxPlayers;
		int.TryParse(_lobbyPing, out var result);
		int num = result + 30;
		_lobbyPingText.text = $"Ping: {num}ms";
		_lobbyTagIcon.sprite = _lobbyTagSprites[(uint)_atlyssSteamLobbyTag];
		_moddedLobbyTag.SetActive(_moddedLobby);
	}

	public void Init_JoinLobby()
	{
		if (passwordLobby)
		{
			LobbyListManager._current._gamepad_inputBuffer = 0.2f;
			LobbyListManager._current._lobbyPasswordInput.text = string.Empty;
			LobbyListManager._current._heldCSteamID = lobbyID;
			LobbyListManager._current._isPasswordInput = true;
		}
		else
		{
			LobbyListManager._current._selectedLobby = true;
			LobbyListManager._current.lobbyListWindow_element.isEnabled = false;
			MainMenuManager._current._menuAcceptAsrc.Play();
			MainMenuManager._current._menuInputBuffer = 0.2f;
			SteamLobby._current.JoinLobby(lobbyID);
		}
	}

	public void Init_BlacklistLobbyID()
	{
		HostConsole._current.Init_NetworkStatusMessage("Lobby hidden. Refreshing query...");
		LobbyListManager._current._lobbyBlacklist.Add(_lobbyName);
		ProfileDataManager._current.Save_HostSettingsData();
		LobbyListManager._current.Init_RefreshLobbyList();
	}
}
