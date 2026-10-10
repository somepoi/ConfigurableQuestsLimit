using System.Collections;
using System.Collections.Generic;
using Steamworks;
using UnityEngine;
using UnityEngine.UI;

public class WhoMenuCell : TabMenuCell
{
	[Header("Who Menu Cell")]
	[SerializeField]
	private ScrollRect _scrollRectList;

	[SerializeField]
	private RectTransform _scrollRectContent;

	[Space]
	[SerializeField]
	private Text _serverNameText;

	[SerializeField]
	private Text _foundPlayerCounterText;

	[SerializeField]
	private Text _localPingCounterText;

	[SerializeField]
	private Text _refreshProgressText;

	[SerializeField]
	private RotateObject _refreshButtonRotateIco;

	[Header("Tab Option Elements")]
	[SerializeField]
	private GameObject _whoTab_selectionDollyElement;

	[SerializeField]
	private Button _refreshListButton;

	[SerializeField]
	private Button _mutePeerButton;

	[SerializeField]
	private Button _inviteToPartyButton;

	[Header("Party Option Elements")]
	[SerializeField]
	private GameObject _whoTab_partySelectionDollyElement;

	[SerializeField]
	private Button _giveLeaderButton;

	[SerializeField]
	private Button _leavePartyButton;

	[SerializeField]
	private Button _teleportToLeaderButton;

	[SerializeField]
	private Text _teleportToLeaderCooldownCounter;

	[Header("Steamworks Elements")]
	[SerializeField]
	private GameObject _lobbyRealmCellObject;

	[SerializeField]
	private Text _lobbyRealmText;

	[SerializeField]
	private Image _lobbyRealmIcon;

	[SerializeField]
	private Sprite[] _lobbyRealmIcons;

	[Header("Runtime")]
	public WhoListDataEntry _selectedDataEntry;

	private float _refreshListButtonBuffer;

	private bool _locatingPeers;

	[Header("Gamepad")]
	[SerializeField]
	private RectTransform _gamepad_tabCursor;

	[SerializeField]
	private Image _gamepad_whoTabListHighlight;

	[SerializeField]
	private bool _gamepad_selectingTabs = true;

	[Space]
	[SerializeField]
	private MenuElement _gamepad_contextPrompt_southButton;

	[SerializeField]
	private MenuElement _gamepad_contextPrompt_eastButton;

	private readonly List<SettingsCell> _gamepad_entrySettingCells = new List<SettingsCell>();

	private int _gamepad_currentTabSelection;

	private int _gamepad_currentEntryCell;

	public int Gamepad_CurrentTabSelection
	{
		get
		{
			return _gamepad_currentTabSelection;
		}
		set
		{
			if (value < 0)
			{
				value = 0;
			}
			if (value > 5)
			{
				value = 5;
			}
			if (value > 2 && !Player._mainPlayer.Network_partyObject)
			{
				value = 2;
			}
			if (value == 3 && (bool)Player._mainPlayer.Network_partyObject && Player._mainPlayer.Network_partyObject.Network_partyLeaderPlayer != Player._mainPlayer)
			{
				if (_gamepad_currentTabSelection < value)
				{
					value++;
				}
				else if (_gamepad_currentTabSelection > value)
				{
					value--;
				}
			}
			if (_whoTab_selectionDollyElement.activeInHierarchy && _gamepad_currentTabSelection != value)
			{
				ToolTipManager._current.Disable_AllTooltips();
				if (IsInvoking("Init_GamepadToolTip"))
				{
					CancelInvoke("Init_GamepadToolTip");
				}
				Invoke("Init_GamepadToolTip", 0.65f);
			}
			_gamepad_currentTabSelection = value;
		}
	}

	public int Gamepad_CurrentEntryCell
	{
		get
		{
			return _gamepad_currentEntryCell;
		}
		set
		{
			if (value < 0)
			{
				value = _gamepad_entrySettingCells.Count - 1;
			}
			if (value > _gamepad_entrySettingCells.Count - 1)
			{
				value = 0;
			}
			_gamepad_currentEntryCell = value;
		}
	}

	public override void Cell_OnAwake()
	{
		Init_SetButtons();
	}

	private void Init_SetButtons()
	{
		_inviteToPartyButton.onClick.AddListener(Init_InviteToParty);
		_leavePartyButton.onClick.AddListener(Init_LeaveParty);
		_teleportToLeaderButton.onClick.AddListener(Init_TeleportToPartyLeader);
		_giveLeaderButton.onClick.AddListener(Init_GivePartyLeader);
		_mutePeerButton.onClick.AddListener(Init_MutePeer);
		_refreshListButton.onClick.AddListener(Refresh_PlayerList);
	}

	public override void Handle_CellUpdate()
	{
		if (_refreshListButtonBuffer > 0f)
		{
			_refreshListButtonBuffer -= Time.deltaTime;
		}
		else
		{
			_refreshListButtonBuffer = 0f;
		}
		if (!TabMenu._current._isOpen)
		{
			_refreshListButtonBuffer = 0f;
			_locatingPeers = false;
			Clear_ListData();
		}
		else
		{
			if (!Player._mainPlayer)
			{
				return;
			}
			_lobbyRealmCellObject.SetActive(AtlyssNetworkManager._current._steamworksMode && !AtlyssNetworkManager._current._soloMode);
			_whoTab_selectionDollyElement.SetActive(AtlyssNetworkManager._current._steamworksMode && !AtlyssNetworkManager._current._soloMode);
			_whoTab_partySelectionDollyElement.SetActive(Player._mainPlayer.Network_partyObject);
			_refreshListButton.interactable = _refreshListButtonBuffer == 0f;
			_mutePeerButton.interactable = _selectedDataEntry;
			_inviteToPartyButton.interactable = (bool)_selectedDataEntry && !_bufferCommandRequest;
			_leavePartyButton.interactable = (bool)Player._mainPlayer.Network_partyObject && !_bufferCommandRequest;
			_giveLeaderButton.gameObject.SetActive((bool)Player._mainPlayer.Network_partyObject && Player._mainPlayer.Network_partyObject.Network_partyLeaderPlayer == Player._mainPlayer && !_bufferCommandRequest);
			_teleportToLeaderCooldownCounter.enabled = Player._mainPlayer._partyLeaderTeleportBuffer > 0f && !Player._mainPlayer._canTeleportToLeader;
			_teleportToLeaderCooldownCounter.text = Player._mainPlayer._partyLeaderTeleportBuffer.ToString("F1");
			if (AtlyssNetworkManager._current._soloMode)
			{
				_refreshProgressText.text = "Network not Initialized";
				_serverNameText.text = "-";
				_foundPlayerCounterText.text = "-";
				_localPingCounterText.text = "-";
				_refreshListButton.interactable = false;
				_gamepad_whoTabListHighlight.enabled = false;
				return;
			}
			if ((bool)ServerInfoObject._current)
			{
				_serverNameText.text = ServerInfoObject._current._serverName ?? "";
				_foundPlayerCounterText.text = $"{_listDataEntries.Count + 1} / {ServerInfoObject._current._maxConnections} Players";
			}
			Handle_LocalLatencyDisplay();
			Handle_RealmInfoDisplay();
			Handle_GamepadControl();
		}
		void Handle_LocalLatencyDisplay()
		{
			if (Player._mainPlayer._isHostPlayer)
			{
				_localPingCounterText.text = "Lobby Host";
			}
			else
			{
				int latency = Player._mainPlayer._latency;
				_localPingCounterText.text = $"Ping: {latency}ms";
				if (Player._mainPlayer._latency < 100)
				{
					_localPingCounterText.color = Color.green;
				}
				else if (Player._mainPlayer._latency > 100)
				{
					_localPingCounterText.color = Color.yellow;
				}
				else if (Player._mainPlayer._latency > 190)
				{
					_localPingCounterText.color = new Color(1f, 0.6f, 0f);
				}
				else if (Player._mainPlayer._latency > 230)
				{
					_localPingCounterText.color = Color.red;
				}
			}
		}
		void Handle_RealmInfoDisplay()
		{
			if (AtlyssNetworkManager._current._steamworksMode)
			{
				switch (ServerInfoObject._current._steamLobbyRealm)
				{
				case AtlyssSteamLobbyTag.GAIA:
					_lobbyRealmText.text = $"{ServerInfoObject._current._steamLobbyRealm} (PvE)";
					break;
				case AtlyssSteamLobbyTag.DUALOS:
					_lobbyRealmText.text = $"{ServerInfoObject._current._steamLobbyRealm} (PvP)";
					break;
				case AtlyssSteamLobbyTag.NOTH:
					_lobbyRealmText.text = $"{ServerInfoObject._current._steamLobbyRealm} (Social)";
					break;
				case AtlyssSteamLobbyTag.LOODIA:
					_lobbyRealmText.text = $"{ServerInfoObject._current._steamLobbyRealm} (RP)";
					break;
				}
				_lobbyRealmIcon.sprite = _lobbyRealmIcons[(uint)ServerInfoObject._current._steamLobbyRealm];
			}
		}
	}

	private void Handle_GamepadControl()
	{
		bool flag = InputControlManager.current._setGamepadType != GamepadType.Keyboard;
		bool flag2 = GamepadInput.GetButtonDown(GamepadButton.RightStick_Up) || InputControlManager.current.DpadUp(DpadHoldType.OnDown);
		bool flag3 = GamepadInput.GetButtonDown(GamepadButton.RightStick_Down) || InputControlManager.current.DpadDown(DpadHoldType.OnDown);
		bool flag4 = GamepadInput.GetButtonDown(GamepadButton.RightStick_Left) || InputControlManager.current.DpadLeft(DpadHoldType.OnDown);
		bool flag5 = GamepadInput.GetButtonDown(GamepadButton.RightStick_Right) || InputControlManager.current.DpadRight(DpadHoldType.OnDown);
		_gamepad_tabCursor.gameObject.SetActive(((TabMenu._current._selectedMenuCell == TabMenu._current._whoCell && !AtlyssNetworkManager._current._soloMode) & flag) && _gamepad_selectingTabs);
		_gamepad_whoTabListHighlight.enabled = (!AtlyssNetworkManager._current._soloMode & flag) && !_gamepad_selectingTabs;
		_gamepad_contextPrompt_southButton.isEnabled = (!_gamepad_selectingTabs && (bool)_selectedDataEntry) & flag;
		_gamepad_contextPrompt_eastButton.isEnabled = (bool)_selectedDataEntry & flag;
		for (int i = 0; i < _gamepad_entrySettingCells.Count; i++)
		{
			if (_gamepad_entrySettingCells == null)
			{
				_gamepad_entrySettingCells.RemoveAt(i);
				break;
			}
			if (!_gamepad_selectingTabs && (bool)_gamepad_entrySettingCells[i] && _gamepad_entrySettingCells[i]._isSelected)
			{
				_selectedDataEntry = _gamepad_entrySettingCells[i].GetComponent<WhoListDataEntry>();
			}
			if ((bool)_gamepad_entrySettingCells[i])
			{
				_gamepad_entrySettingCells[i]._isSelected = flag && !_gamepad_selectingTabs && i == _gamepad_currentEntryCell;
			}
			if (_gamepad_entrySettingCells.Count > 3 && _gamepad_entrySettingCells[i]._isSelected && InputControlManager.current._gamepadMouseCursorBuffer <= 0f)
			{
				Canvas.ForceUpdateCanvases();
				Vector2 b = (Vector2)_scrollRectList.transform.InverseTransformPoint(_scrollRectContent.position) - (Vector2)_scrollRectList.transform.InverseTransformPoint(_gamepad_entrySettingCells[i].transform.position);
				_scrollRectContent.anchoredPosition = Vector2.Lerp(_scrollRectContent.anchoredPosition, b, 12.5f * Time.deltaTime);
			}
		}
		if (!flag || TabMenu._current._selectedMenuCell != TabMenu._current._whoCell)
		{
			return;
		}
		if (_gamepad_currentTabSelection == 3 && (bool)Player._mainPlayer.Network_partyObject && Player._mainPlayer.Network_partyObject.Network_partyLeaderPlayer != Player._mainPlayer)
		{
			_gamepad_currentTabSelection = 4;
		}
		if (!Player._mainPlayer.Network_partyObject && _gamepad_currentTabSelection > 2)
		{
			_gamepad_currentTabSelection = 0;
		}
		if (GamepadInput.GetButtonDown(GamepadButton.Button_East) && (bool)_selectedDataEntry)
		{
			_gamepad_selectingTabs = true;
			_gamepad_currentEntryCell = 0;
			_selectedDataEntry = null;
			TabMenu._current._gamepadSelectAsrc.Play();
		}
		if (!_gamepad_selectingTabs)
		{
			if (flag2)
			{
				Gamepad_CurrentEntryCell--;
				TabMenu._current._gamepadSelectAsrc.Play();
			}
			if (flag3)
			{
				Gamepad_CurrentEntryCell++;
				TabMenu._current._gamepadSelectAsrc.Play();
			}
			if (flag4)
			{
				_gamepad_selectingTabs = true;
				_gamepad_currentEntryCell = 0;
				_selectedDataEntry = null;
				TabMenu._current._gamepadSelectAsrc.Play();
			}
			if (GamepadInput.GetButtonDown(GamepadButton.Button_South))
			{
				_gamepad_selectingTabs = true;
				TabMenu._current._gamepadSelectAsrc.Play();
			}
			if ((bool)_selectedDataEntry)
			{
				if (GamepadInput.GetButtonDown(GamepadButton.Button_South))
				{
					_gamepad_selectingTabs = true;
					TabMenu._current._gamepadSelectAsrc.Play();
				}
				if (GamepadInput.GetButtonDown(GamepadButton.Button_West))
				{
					_selectedDataEntry._steamProfileButton.onClick.Invoke();
					TabMenu._current._gamepadSelectAsrc.Play();
				}
			}
			return;
		}
		switch (_gamepad_currentTabSelection)
		{
		case 0:
			_gamepad_tabCursor.transform.position = _refreshListButton.transform.position;
			if (GamepadInput.GetButtonDown(GamepadButton.Button_South) && _refreshListButton.IsInteractable())
			{
				_refreshListButton.onClick.Invoke();
				TabMenu._current._gamepadSelectAsrc.Play();
				ToolTipManager._current.Disable_AllTooltips();
			}
			break;
		case 1:
			_gamepad_tabCursor.transform.position = _mutePeerButton.transform.position;
			if (GamepadInput.GetButtonDown(GamepadButton.Button_South) && _mutePeerButton.IsInteractable())
			{
				_mutePeerButton.onClick.Invoke();
				TabMenu._current._gamepadSelectAsrc.Play();
				ToolTipManager._current.Disable_AllTooltips();
			}
			break;
		case 2:
			_gamepad_tabCursor.transform.position = _inviteToPartyButton.transform.position;
			if (GamepadInput.GetButtonDown(GamepadButton.Button_South) && _inviteToPartyButton.IsInteractable())
			{
				_inviteToPartyButton.onClick.Invoke();
				TabMenu._current._gamepadSelectAsrc.Play();
				ToolTipManager._current.Disable_AllTooltips();
			}
			break;
		case 3:
			_gamepad_tabCursor.transform.position = _giveLeaderButton.transform.position;
			if (GamepadInput.GetButtonDown(GamepadButton.Button_South) && _giveLeaderButton.IsInteractable())
			{
				_giveLeaderButton.onClick.Invoke();
				TabMenu._current._gamepadSelectAsrc.Play();
				ToolTipManager._current.Disable_AllTooltips();
			}
			break;
		case 4:
			_gamepad_tabCursor.transform.position = _teleportToLeaderButton.transform.position;
			if (GamepadInput.GetButtonDown(GamepadButton.Button_South) && _teleportToLeaderButton.IsInteractable())
			{
				_teleportToLeaderButton.onClick.Invoke();
				TabMenu._current._gamepadSelectAsrc.Play();
				ToolTipManager._current.Disable_AllTooltips();
			}
			break;
		case 5:
			_gamepad_tabCursor.transform.position = _leavePartyButton.transform.position;
			if (GamepadInput.GetButtonDown(GamepadButton.Button_South) && _leavePartyButton.IsInteractable())
			{
				_leavePartyButton.onClick.Invoke();
				TabMenu._current._gamepadSelectAsrc.Play();
				ToolTipManager._current.Disable_AllTooltips();
			}
			break;
		}
		if (flag5)
		{
			_gamepad_selectingTabs = false;
			_gamepad_currentTabSelection = 0;
			TabMenu._current._gamepadSelectAsrc.Play();
			ToolTipManager._current.Disable_AllTooltips();
		}
		if (flag2)
		{
			Gamepad_CurrentTabSelection--;
			TabMenu._current._gamepadSelectAsrc.Play();
		}
		if (flag3)
		{
			Gamepad_CurrentTabSelection++;
			TabMenu._current._gamepadSelectAsrc.Play();
		}
	}

	private void Init_MutePeer()
	{
		for (int i = 0; i < AtlyssNetworkManager._current._mutedClientList.Count; i++)
		{
			if (AtlyssNetworkManager._current._mutedClientList[i]._steamID == _selectedDataEntry._player._steamID)
			{
				AtlyssNetworkManager._current._mutedClientList.RemoveAt(i);
				Player._mainPlayer._chatBehaviour.New_ChatMessage("Unmuted " + _selectedDataEntry._player._nickname + ".");
				ProfileDataManager._current.Save_HostSettingsData();
				return;
			}
		}
		BannedClientParameter item = new BannedClientParameter
		{
			_characterNickname = _selectedDataEntry._player._nickname,
			_address = "",
			_steamID = _selectedDataEntry._player._steamID
		};
		AtlyssNetworkManager._current._mutedClientList.Add(item);
		Player._mainPlayer._chatBehaviour.New_ChatMessage("Muted " + _selectedDataEntry._player._nickname + ".");
		ProfileDataManager._current.Save_HostSettingsData();
	}

	private void Init_InviteToParty()
	{
		if ((bool)_selectedDataEntry)
		{
			if ((bool)Player._mainPlayer.Network_partyObject && Player._mainPlayer.Network_partyObject.Network_partyLeaderPlayer != Player._mainPlayer)
			{
				ErrorPromptTextManager.current.Init_ErrorPrompt("Not the Party Leader");
			}
			else if (_selectedDataEntry._player == null)
			{
				ErrorPromptTextManager.current.Init_ErrorPrompt("Player not found");
			}
			else if ((bool)_selectedDataEntry._player.Network_partyObject)
			{
				ErrorPromptTextManager.current.Init_ErrorPrompt("Player already in a Party");
			}
			else
			{
				Player._mainPlayer.Cmd_InviteToParty(_selectedDataEntry._player);
			}
		}
	}

	private void Init_LeaveParty()
	{
		if (!Player._mainPlayer.Network_partyObject)
		{
			ErrorPromptTextManager.current.Init_ErrorPrompt("Not in a Party");
		}
		else
		{
			Player._mainPlayer.Cmd_LeaveParty();
		}
	}

	private void Init_GivePartyLeader()
	{
		if (!Player._mainPlayer.Network_partyObject)
		{
			ErrorPromptTextManager.current.Init_ErrorPrompt("Not in a Party");
			return;
		}
		if (Player._mainPlayer.Network_partyObject.Network_partyLeaderPlayer != Player._mainPlayer)
		{
			ErrorPromptTextManager.current.Init_ErrorPrompt("Not the party leader");
			return;
		}
		if (!_selectedDataEntry)
		{
			ErrorPromptTextManager.current.Init_ErrorPrompt("No player selected");
			return;
		}
		Player player = _selectedDataEntry._player;
		if (player == null)
		{
			ErrorPromptTextManager.current.Init_ErrorPrompt("Player not found");
		}
		else if (player.Network_partyObject != Player._mainPlayer.Network_partyObject)
		{
			ErrorPromptTextManager.current.Init_ErrorPrompt("Player is not in your party");
		}
		else
		{
			Player._mainPlayer.Cmd_GiveLeadership(_selectedDataEntry._player);
		}
	}

	private void Init_TeleportToPartyLeader()
	{
		if (Can_TeleportToPartyLeader())
		{
			if (!Player._mainPlayer.Network_partyObject)
			{
				ErrorPromptTextManager.current.Init_ErrorPrompt("Not in a Party");
				return;
			}
			if (!Player._mainPlayer._canTeleportToLeader)
			{
				ErrorPromptTextManager.current.Init_ErrorPrompt("On cooldown");
				return;
			}
			Player._mainPlayer._partyLeaderTeleportBuffer = GameManager._current._statLogics._teleportToLeaderCooldown;
			Player._mainPlayer._pSound._aSrcGeneral.PlayOneShot(Player._mainPlayer._pSound._portalInteract, 0.85f);
			Player._mainPlayer._pVisual.Send_CrossFadeAnim("Idle", 0f, 0, LatencyCheck.IGNORE_LATENCY);
			Player._mainPlayer._pMove._freezeMovementBuffer = 1.25f;
			Player._mainPlayer.Cmd_TeleportToPartyLeader();
		}
		bool Can_TeleportToPartyLeader()
		{
			if (_bufferCommandRequest)
			{
				return false;
			}
			if (AtlyssNetworkManager._current._soloMode)
			{
				return false;
			}
			if (!Player._mainPlayer)
			{
				return false;
			}
			if (!Player._mainPlayer.Network_partyObject)
			{
				ErrorPromptTextManager.current.Init_ErrorPrompt("Not in a party");
				return false;
			}
			if (!Player._mainPlayer._canTeleportToLeader)
			{
				ErrorPromptTextManager.current.Init_ErrorPrompt("On cooldown");
				return false;
			}
			if (Player._mainPlayer._playerZoneType == ZoneType.Dungeon)
			{
				ErrorPromptTextManager.current.Init_ErrorPrompt("Currently in a Dungeon");
				return false;
			}
			PartyObjectBehavior network_partyObject = Player._mainPlayer.Network_partyObject;
			if (!network_partyObject.Network_partyLeaderPlayer)
			{
				ErrorPromptTextManager.current.Init_ErrorPrompt("No party leader found");
				return false;
			}
			if (network_partyObject.Network_partyLeaderPlayer == Player._mainPlayer)
			{
				ErrorPromptTextManager.current.Init_ErrorPrompt("You're the party leader");
				return false;
			}
			if (network_partyObject.Network_partyLeaderPlayer.Network_playerMapInstance == Player._mainPlayer.Network_playerMapInstance)
			{
				ErrorPromptTextManager.current.Init_ErrorPrompt("In same map instance");
				return false;
			}
			if (network_partyObject.Network_partyLeaderPlayer._bufferingStatus)
			{
				ErrorPromptTextManager.current.Init_ErrorPrompt("Leader is loading");
				return false;
			}
			PlayerQuesting pQuest = Player._mainPlayer._pQuest;
			ScriptableQuest partyLeaderTeleportRequirement = GameManager._current._statLogics._partyLeaderTeleportRequirement;
			if (!pQuest._finishedQuests.ContainsKey(partyLeaderTeleportRequirement._questName ?? "") && !pQuest._scriptableQuests.Contains(partyLeaderTeleportRequirement))
			{
				ErrorPromptTextManager.current.Init_ErrorPrompt("Requires introduction quest");
				return false;
			}
			return true;
		}
	}

	public void Refresh_PlayerList()
	{
		if (!(_refreshListButtonBuffer > 0f))
		{
			Create_PlayerDataEntries();
			_selectedDataEntry = null;
			_refreshListButtonBuffer = 2.5f;
		}
	}

	private void Create_PlayerDataEntries()
	{
		if (!AtlyssNetworkManager._current._soloMode && !(_refreshListButtonBuffer > 0f) && (bool)Player._mainPlayer)
		{
			_selectedDataEntry = null;
			Clear_ListData();
			StopAllCoroutines();
			StartCoroutine(Process_FindEntries());
		}
	}

	private IEnumerator Process_FindEntries()
	{
		if (_locatingPeers)
		{
			yield break;
		}
		_locatingPeers = true;
		_refreshProgressText.text = "Searching...";
		_refreshButtonRotateIco.Alter_RotationValues(Vector3.back * 250f);
		_gamepad_currentEntryCell = 0;
		_gamepad_entrySettingCells.Clear();
		yield return new WaitForSeconds(0.65f);
		Player[] array = Object.FindObjectsOfType<Player>();
		for (int i = 0; i < array.Length; i++)
		{
			if (!(array[i] == Player._mainPlayer) && !(array[i] == null))
			{
				GameObject obj = Object.Instantiate(_listDataObject_prefab);
				obj.transform.SetParent(_cell_ScrollViewContent);
				obj.transform.position = Vector3.zero;
				obj.transform.localScale = Vector3.one;
				WhoListDataEntry component = obj.GetComponent<WhoListDataEntry>();
				component._player = array[i];
				component._netId = array[i].netIdentity;
				_listDataEntries.Add(component);
			}
		}
		if (_listDataEntries.Count <= 0)
		{
			_refreshProgressText.text = "No players found.";
		}
		else
		{
			_refreshProgressText.text = string.Empty;
		}
		_gamepad_entrySettingCells.AddRange(_cell_ScrollViewContent.GetComponentsInChildren<SettingsCell>());
		_locatingPeers = false;
		Canvas.ForceUpdateCanvases();
		if (_listDataEntries.Count > 0)
		{
			if (_listDataEntries.Count < 3)
			{
				_scrollRectContent.pivot = new Vector2(0.47f, 1f);
			}
			else
			{
				_scrollRectContent.pivot = new Vector2(0.47f, 0.6f);
			}
			Vector2 anchoredPosition = (Vector2)_scrollRectList.transform.InverseTransformPoint(_scrollRectContent.position) - (Vector2)_scrollRectList.transform.InverseTransformPoint(_gamepad_entrySettingCells[0].transform.position);
			_scrollRectContent.anchoredPosition = anchoredPosition;
		}
	}

	public void Init_OpenSteamOverlay()
	{
		SteamFriends.ActivateGameOverlay("friends");
	}

	public void Init_OpenSteamProfile()
	{
		if ((bool)_selectedDataEntry && !(_selectedDataEntry._player == null))
		{
			ulong.TryParse(_selectedDataEntry._player._steamID, out var result);
			CSteamID steamID = new CSteamID(result);
			SteamFriends.ActivateGameOverlayToUser("steamid", steamID);
		}
	}

	private void Init_GamepadToolTip()
	{
		if (!Cursor.visible && _gamepad_selectingTabs && !(TabMenu._current._selectedMenuCell != TabMenu._current._whoCell))
		{
			ToolTipManager._current.Disable_AllTooltips();
			switch (_gamepad_currentTabSelection)
			{
			case 0:
				ToolTipManager._current._genericToolTip._anchorReference = _refreshListButton.transform;
				Init_StringGenericToolTip("Refresh List");
				break;
			case 1:
				ToolTipManager._current._genericToolTip._anchorReference = _mutePeerButton.transform;
				Init_StringGenericToolTip("Mute / Unmute Player");
				break;
			case 2:
				ToolTipManager._current._genericToolTip._anchorReference = _inviteToPartyButton.transform;
				Init_StringGenericToolTip("Party Invite");
				break;
			case 3:
				ToolTipManager._current._genericToolTip._anchorReference = _giveLeaderButton.transform;
				Init_StringGenericToolTip("Give Leadership");
				break;
			case 4:
				ToolTipManager._current._genericToolTip._anchorReference = _teleportToLeaderButton.transform;
				Init_StringGenericToolTip("Teleport to Leader");
				break;
			case 5:
				ToolTipManager._current._genericToolTip._anchorReference = _leavePartyButton.transform;
				Init_StringGenericToolTip("Leave Party");
				break;
			}
		}
	}

	public void Disable_ToolTip()
	{
		ToolTipManager._current._genericToolTip.Disable_ToolTip();
	}

	public void Init_StringGenericToolTip(string _string)
	{
		ToolTipManager._current.Apply_GenericToolTip(_string ?? "");
		ToolTipManager._current._genericToolTip.Set_TooltipAnchorPos(new Vector2(-125f, 0f));
		ToolTipManager._current._genericToolTip.Enable_ToolTip();
	}
}
