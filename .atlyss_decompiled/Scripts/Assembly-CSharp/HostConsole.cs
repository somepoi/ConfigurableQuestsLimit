using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HostConsole : MonoBehaviour
{
	public static HostConsole _current;

	private ConsoleCommandManager _cmdManager;

	private AtlyssNetworkManager _nM;

	[Header("Host Console")]
	public MenuElement _hostConsoleElement;

	[SerializeField]
	private Text _serverNameText;

	[SerializeField]
	private Text _serverPasswordText;

	[SerializeField]
	private Text _serverPortText;

	[SerializeField]
	private Text _serverConnectionsText;

	public bool _isOpen;

	public bool _focusedInInput;

	[Header("Network Status Message")]
	[SerializeField]
	private CanvasGroup _networkStatusGroup;

	[SerializeField]
	private Text _networkStatusMessage;

	[SerializeField]
	private float _networkStatusTimer = 3.5f;

	private float _currentNetworkStatusTimer;

	[Header("Console Log")]
	[SerializeField]
	private Text _consoleTextField;

	[SerializeField]
	private InputField _consoleInputField;

	[SerializeField]
	private ScrollRect _consoleLogContent;

	[SerializeField]
	private Button _shutdownHostButton;

	[SerializeField]
	private Button _consoleClearFieldButton;

	[SerializeField]
	private Button _closeConsoleButton;

	[SerializeField]
	private Button _banPeerButton;

	[SerializeField]
	private Button _kickPeerButton;

	private string _dataPath = string.Empty;

	[Header("Data Entry Lists")]
	[SerializeField]
	private GameObject _peerListDataEntryPrefab;

	[SerializeField]
	private GameObject _instanceListDataEntryPrefab;

	[SerializeField]
	private Transform _peerListContent;

	[SerializeField]
	private Transform _instanceListContent;

	public HC_PeerListEntry _selectedPeerEntry;

	public List<HC_PeerListEntry> _peerListEntries = new List<HC_PeerListEntry>();

	public List<HC_InstanceListEntry> _instanceListEntries = new List<HC_InstanceListEntry>();

	private void Awake()
	{
		if (_current == null)
		{
			_current = this;
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			_dataPath = Application.dataPath + "/AuditLogs/";
			_cmdManager = GetComponent<ConsoleCommandManager>();
			_closeConsoleButton.onClick.AddListener(Close_HostConsole);
			_shutdownHostButton.onClick.AddListener(Init_Shutdown);
			_kickPeerButton.onClick.AddListener(Kick_Peer);
			_banPeerButton.onClick.AddListener(Ban_Peer);
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void ScrollToBottom()
	{
		_consoleLogContent.verticalScrollbar.value = 0f;
	}

	private void Update()
	{
		_nM = AtlyssNetworkManager._current;
		if ((bool)_nM)
		{
			Handle_NetworkStatusMessenger();
			if (!_hostConsoleElement.isEnabled)
			{
				_selectedPeerEntry = null;
			}
			bool flag = NetworkClient.active && !_nM._soloMode && (bool)Player._mainPlayer && Player._mainPlayer._isHostPlayer;
			_hostConsoleElement.gameObject.SetActive(flag);
			if (flag)
			{
				Iterate_DataListEntries();
				Handle_HostConsoleControl();
				Handle_ServerInfo();
			}
		}
	}

	private void Handle_NetworkStatusMessenger()
	{
		if (_currentNetworkStatusTimer > 0f)
		{
			_networkStatusGroup.alpha = 1f;
			_currentNetworkStatusTimer -= Time.deltaTime;
		}
		else
		{
			_currentNetworkStatusTimer = 0f;
			_networkStatusGroup.alpha = Mathf.Lerp(_networkStatusGroup.alpha, 0f, 12.5f * Time.deltaTime);
		}
	}

	public void Init_NetworkStatusMessage(string _msg)
	{
		_networkStatusMessage.text = _msg;
		_currentNetworkStatusTimer = _networkStatusTimer;
	}

	public void Kick_Peer()
	{
		if ((bool)_selectedPeerEntry && NetworkServer.active)
		{
			if (_selectedPeerEntry._peerPlayer._isHostPlayer)
			{
				Init_NetworkStatusMessage("Cannot kick the host!");
				return;
			}
			_cmdManager.Init_KickClient(_selectedPeerEntry, _selectedPeerEntry._peerPlayer.connectionToClient.connectionId);
			_selectedPeerEntry = null;
		}
	}

	public void Ban_Peer()
	{
		if ((bool)_selectedPeerEntry && NetworkServer.active)
		{
			if (_selectedPeerEntry._peerPlayer._isHostPlayer)
			{
				Init_NetworkStatusMessage("Cannot ban the host!");
				return;
			}
			_cmdManager.Init_BanClient(_selectedPeerEntry, _selectedPeerEntry._peerPlayer.connectionToClient.connectionId);
			_selectedPeerEntry = null;
		}
	}

	private void Handle_HostConsoleControl()
	{
		_isOpen = _hostConsoleElement.isEnabled;
		_focusedInInput = _consoleInputField.isFocused && _isOpen;
		_consoleClearFieldButton.interactable = _hostConsoleElement.isEnabled;
		_closeConsoleButton.interactable = _hostConsoleElement.isEnabled;
		_consoleInputField.interactable = _isOpen;
		_banPeerButton.interactable = _selectedPeerEntry;
		_kickPeerButton.interactable = _selectedPeerEntry;
		if (_consoleTextField.text.Length >= 1300)
		{
			Clear_Log();
		}
		if (!NetworkServer.active)
		{
			_hostConsoleElement.isEnabled = false;
			_consoleTextField.text = string.Empty;
			return;
		}
		if (_isOpen)
		{
			if (Input.GetKeyDown(KeyCode.Return) && !_consoleInputField.isFocused)
			{
				_consoleInputField.ActivateInputField();
			}
		}
		else
		{
			_consoleInputField.text = string.Empty;
		}
		if (_nM._serverMode)
		{
			_hostConsoleElement.isEnabled = true;
			return;
		}
		if (Input.GetKeyDown(InputControlManager.current._hostConsole) && !Player._mainPlayer._inChat && !SettingsManager._current._isOpen)
		{
			_hostConsoleElement.isEnabled = !_hostConsoleElement.isEnabled;
		}
		if (Input.GetKeyDown(KeyCode.Escape) && _isOpen && CameraFunction._current._unlockedCamera)
		{
			Close_HostConsole();
		}
		if (!_hostConsoleElement.isEnabled)
		{
			_consoleLogContent.verticalScrollbar.value = 0f;
		}
	}

	private void Handle_ServerInfo()
	{
		_serverNameText.text = _nM._serverName ?? "";
		if (!string.IsNullOrWhiteSpace(_nM._serverPassword))
		{
			_serverPasswordText.text = "Password: " + _nM._serverPassword;
		}
		else
		{
			_serverPasswordText.text = "No password";
		}
		if (LobbyListManager._current._lobbyTypeDropdown.value == 1)
		{
			_serverPasswordText.text = "Friend Lobby (Invite Only)";
		}
		if (AtlyssNetworkManager._current._steamworksMode)
		{
			_serverPortText.text = "Steamworks Lobby";
		}
		else
		{
			_serverPortText.text = $"Port: {_nM.GetComponent<TelepathyTransport>().port} (TCP)";
		}
		_serverConnectionsText.text = $"Connections: {NetworkServer.connections.Count} / {NetworkServer.maxConnections}";
	}

	private void Init_Shutdown()
	{
		if (NetworkServer.active)
		{
			_cmdManager.Init_ButtonShutdown();
		}
	}

	public void Init_ServerDisconnect()
	{
		if (NetworkServer.active)
		{
			Clear_DataEntryLists();
			if (NetworkClient.isConnected)
			{
				_nM.StopHost();
			}
			else
			{
				_nM.StopServer();
			}
		}
	}

	public void Clear_Log()
	{
		_consoleTextField.text = string.Empty;
	}

	public void New_InitalizeServerMessage()
	{
		if (AtlyssNetworkManager._current._steamworksMode)
		{
			New_LogMessage("Steam Lobby (" + _nM._serverName + ") Initialized...");
		}
		New_LogMessage("Type '/help' for listing of console commands.");
	}

	public void New_LogMessage(string _message)
	{
		bool num = _consoleLogContent.verticalScrollbar.value <= 0.15f;
		string text = $"[{DateTime.Now.Hour}:{DateTime.Now.Minute}] " + _message + "\n";
		_consoleTextField.text += text;
		Console.ForegroundColor = ConsoleColor.Yellow;
		Console.WriteLine("{0}", text);
		Console.ResetColor();
		if (num)
		{
			if (IsInvoking("ScrollToBottom"))
			{
				CancelInvoke("ScrollToBottom");
			}
			Invoke("ScrollToBottom", 0.085f);
		}
	}

	public void Close_HostConsole()
	{
		_hostConsoleElement.isEnabled = false;
		_isOpen = false;
	}

	public void Send_ServerMessage(string _message)
	{
		if (!Input.GetKeyDown(KeyCode.Return) || string.IsNullOrWhiteSpace(_message))
		{
			return;
		}
		EventSystem.current.SetSelectedGameObject(null);
		if (_message.Contains('<') || _message.Contains('>'))
		{
			New_LogMessage("'<' and '>' are not allowed in server messages.");
		}
		else
		{
			string[] array = _message.Split(' ');
			if (_message[0] == '/')
			{
				New_LogMessage(array[0] + ((array.Length > 1) ? (" " + array[1]) : string.Empty));
				_cmdManager.Init_ConsoleCommand(array[0].TrimStart('/'), (array.Length > 1) ? array[1] : string.Empty);
			}
			else
			{
				Init_ServerMessage(_message);
			}
		}
		_consoleInputField.text = string.Empty;
	}

	public void Init_ServerMessage(string _message)
	{
		if (!string.IsNullOrWhiteSpace(_message))
		{
			NetworkServer.SendToAll(new ServerMessage
			{
				servMsg = _message
			});
			New_LogMessage(_message);
		}
	}

	private void Iterate_DataListEntries()
	{
		if (!NetworkServer.active)
		{
			return;
		}
		for (int i = 0; i < _instanceListEntries.Count; i++)
		{
			if (_instanceListEntries[i] == null || _instanceListEntries[i]._netId == null)
			{
				UnityEngine.Object.Destroy(_instanceListEntries[i].gameObject);
				_instanceListEntries.RemoveAt(i);
			}
		}
	}

	public void Clear_DataEntryLists()
	{
		for (int i = 0; i < _peerListEntries.Count; i++)
		{
			UnityEngine.Object.Destroy(_peerListEntries[i].gameObject);
		}
		for (int j = 0; j < _instanceListEntries.Count; j++)
		{
			UnityEngine.Object.Destroy(_instanceListEntries[j].gameObject);
		}
		_peerListEntries.Clear();
		_instanceListEntries.Clear();
	}

	public void Create_PeerListEntry(int _connID, Player _playerObject)
	{
		if (!AtlyssNetworkManager._current._soloMode)
		{
			GameObject obj = UnityEngine.Object.Instantiate(_peerListDataEntryPrefab);
			HC_PeerListEntry component = obj.GetComponent<HC_PeerListEntry>();
			obj.transform.SetParent(_peerListContent);
			obj.transform.position = Vector3.zero;
			obj.transform.localScale = Vector3.one;
			component._dataID = _connID;
			component._peerPlayer = _playerObject;
			component._netId = _playerObject.netIdentity;
			component.Apply_PeerDataEntry();
			_peerListEntries.Add(component);
		}
	}

	public void Destroy_PeerListEntry(int _connID)
	{
		for (int i = 0; i < _peerListEntries.Count; i++)
		{
			if (_peerListEntries[i]._dataID == _connID)
			{
				_current.Init_ServerMessage(_peerListEntries[i]._peerPlayer._nickname + " disconnected from the server.");
				UnityEngine.Object.Destroy(_peerListEntries[i].gameObject);
				_peerListEntries.RemoveAt(i);
			}
		}
	}

	public void Create_InstanceListEntry(NetworkIdentity _netId)
	{
		if ((bool)_netId)
		{
			GameObject obj = UnityEngine.Object.Instantiate(_instanceListDataEntryPrefab);
			obj.transform.SetParent(_instanceListContent);
			obj.transform.position = Vector3.zero;
			obj.transform.localScale = Vector3.one;
			HC_InstanceListEntry component = obj.GetComponent<HC_InstanceListEntry>();
			component._netId = _netId;
			component.Apply_MapInstanceData();
			_instanceListEntries.Add(component);
		}
	}
}
