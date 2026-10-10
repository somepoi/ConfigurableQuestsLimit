using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Mirror;
using Mirror.RemoteCalls;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ChatBehaviour : NetworkBehaviour
{
	public enum ChatChannel : byte
	{
		GLOBAL = 0,
		PARTY = 1,
		ZONE = 2
	}

	public static ChatBehaviour _current;

	private Player _player;

	[Header("Debug")]
	private bool _debugModeEnabled;

	[Header("Chat Behavior Params")]
	[SerializeField]
	private TextMeshPro _chatTextMesh;

	public bool _focusedInChat;

	[Space]
	[SerializeField]
	private GameObject _chatBubbleObject;

	[SyncVar]
	private bool _chatBubbleEnabled;

	[Space]
	[SyncVar]
	public bool _joinChatRoomBuffer;

	private float _currentJoinChatRoomBuffer;

	[SyncVar]
	public bool _messageSendBuffer;

	private float _currentMessageSendBuffer;

	[Header("Chat Channel Dock")]
	public ChatChannel _setChatChannel;

	[SerializeField]
	private bool _inGlobalChat = true;

	[SerializeField]
	private bool _inPartyChat = true;

	[SerializeField]
	private bool _inZoneChat = true;

	[Header("UI Assets")]
	public ChatBehaviourAssets _chatAssets;

	private float _triggerMessageBuffer;

	public Color[] _chatChannelColors;

	[Header("Chat Line Thresholds")]
	[SerializeField]
	private int _maxChatboxLines = 6;

	public List<string> _chatMessages = new List<string>();

	private string _lastMessage = string.Empty;

	[Space]
	public float _textFadeTimer;

	public float _logicFadeTimer;

	private float _inputBuffer;

	[Header("Game Logic Feed")]
	[SerializeField]
	private int _maxGameLogicLines = 6;

	public List<string> _gameLogicMessages = new List<string>();

	[Header("Player Emotes")]
	[SerializeField]
	private ScriptableEmoteList _scriptableEmoteList;

	private float _emoteBuffer;

	private int _setEmoteIndex;

	[Header("Audio")]
	public AudioSource aSrc;

	public AudioSource _pVoiceASrc;

	[SerializeField]
	private AudioClip _focusInputSfx;

	[SerializeField]
	private AudioClip _triggerMessageSfx;

	private bool IsChatEnterInput
	{
		get
		{
			if (!Input.GetKeyDown(KeyCode.Return))
			{
				if (GamepadInput.GetButtonDown(GamepadButton.Back))
				{
					return Player._mainPlayer._partyInviteStatus != PartyInviteStatus.INVITED;
				}
				return false;
			}
			return true;
		}
	}

	public bool Network_chatBubbleEnabled
	{
		get
		{
			return _chatBubbleEnabled;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _chatBubbleEnabled, 1uL, null);
		}
	}

	public bool Network_joinChatRoomBuffer
	{
		get
		{
			return _joinChatRoomBuffer;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _joinChatRoomBuffer, 2uL, null);
		}
	}

	public bool Network_messageSendBuffer
	{
		get
		{
			return _messageSendBuffer;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _messageSendBuffer, 4uL, null);
		}
	}

	private static event Action<string> OnChatMessage;

	private void Awake()
	{
		_player = GetComponent<Player>();
		_chatTextMesh.text = string.Empty;
		Display_Chat(_isTrue: false);
	}

	public override void OnStartAuthority()
	{
		NetworkClient.RegisterHandler<ServerMessage>(OnServerMessage);
		OnChatMessage += New_ChatMessage;
		_current = this;
	}

	public void OnClick_GlobalChannel()
	{
		if (_inGlobalChat)
		{
			New_ChatMessage("<color=yellow>Disabled #Global Chat Channel.</color>");
		}
		else
		{
			New_ChatMessage("<color=yellow>Enabled #Global Chat Channel.</color>");
		}
		_chatAssets._chatInput.ActivateInputField();
		_inGlobalChat = !_inGlobalChat;
	}

	public void OnClick_PartyChannel()
	{
		if (_inPartyChat)
		{
			New_ChatMessage("<color=#B2EC5D>Disabled #Party Chat Channel.</color>");
		}
		else
		{
			New_ChatMessage("<color=#B2EC5D>Enabled #Party Chat Channel.</color>");
		}
		_chatAssets._chatInput.ActivateInputField();
		_inPartyChat = !_inPartyChat;
	}

	public void OnClick_ZoneChannel()
	{
		if (_inZoneChat)
		{
			New_ChatMessage("<color=#FF8A90>Disabled #Zone Chat Channel.</color>");
		}
		else
		{
			New_ChatMessage("<color=#FF8A90>Enabled #Zone Chat Channel.</color>");
		}
		_chatAssets._chatInput.ActivateInputField();
		_inZoneChat = !_inZoneChat;
	}

	[ClientCallback]
	public void Display_Chat(bool _isTrue)
	{
		if (NetworkClient.active && (bool)_chatAssets)
		{
			float alpha = 1f;
			if (!_isTrue)
			{
				alpha = 0f;
			}
			for (int i = 0; i < _chatAssets._chatTextGroups.Length; i++)
			{
				_chatAssets._chatTextGroups[i].alpha = alpha;
			}
		}
	}

	private void Update()
	{
		if (_emoteBuffer > 0f)
		{
			_emoteBuffer -= Time.deltaTime;
		}
		else
		{
			_emoteBuffer = 0f;
		}
		Server_BufferParams();
		Client_HandleChannelDockDisplay();
		Client_HandleChatboxUpdate();
		if ((bool)Player._mainPlayer)
		{
			_chatBubbleObject.SetActive(_chatBubbleEnabled && Player._mainPlayer.Network_playerMapInstance == _player.Network_playerMapInstance);
		}
		else
		{
			_chatBubbleObject.SetActive(value: false);
		}
	}

	[ServerCallback]
	private void Server_BufferParams()
	{
		if (NetworkServer.active)
		{
			if (_currentMessageSendBuffer > 0f)
			{
				_currentMessageSendBuffer -= Time.deltaTime;
			}
			else
			{
				_currentMessageSendBuffer = 0f;
			}
			if (_currentJoinChatRoomBuffer > 0f)
			{
				_currentJoinChatRoomBuffer -= Time.deltaTime;
			}
			else
			{
				_currentJoinChatRoomBuffer = 0f;
			}
			Network_messageSendBuffer = _currentMessageSendBuffer > 0f;
			Network_joinChatRoomBuffer = _currentJoinChatRoomBuffer > 0f;
		}
	}

	[ClientCallback]
	private void Client_HandleChannelDockDisplay()
	{
		if (NetworkClient.active && base.isLocalPlayer && (bool)_chatAssets)
		{
			_chatAssets._chatChannelDockObject.SetActive(_focusedInChat);
			_chatAssets._globalButton_highlight.enabled = !_inGlobalChat;
			_chatAssets._partyButton_highlight.enabled = !_inPartyChat;
			_chatAssets._roomButton_highlight.enabled = !_inZoneChat;
		}
	}

	[Command]
	private void Cmd_ToggleChatBubble(bool _isTrue)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteBool(_isTrue);
		SendCommandInternal("System.Void ChatBehaviour::Cmd_ToggleChatBubble(System.Boolean)", -681793834, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientCallback]
	private void Client_HandleChatboxUpdate()
	{
		if (NetworkClient.active && base.isLocalPlayer)
		{
			Handle_ChatFade();
			Handle_ChatBoxControls();
		}
		void Handle_ChatBoxControls()
		{
			if (_inputBuffer > 0f)
			{
				_inputBuffer -= Time.deltaTime;
			}
			else
			{
				_inputBuffer = 0f;
			}
			if (!_chatAssets)
			{
				Display_Chat(_isTrue: false);
			}
			else
			{
				if (_player._currentPlayerAction == PlayerAction.IDLE && _player._pMove._currentMovementAction == MovementAction.IDLE)
				{
					bool flag = GamepadInput.GetButton(GamepadButton.LeftShoulder) || GamepadInput.GetButton(GamepadButton.RightShoulder);
					bool num = Input.GetKeyDown(InputControlManager.current._sit) || (InputControlManager.current.DpadRight(DpadHoldType.OnDown) && !flag);
					bool flag2 = _inputBuffer <= 0f && !_player._inChat && !TabMenu._current._isOpen && !SettingsManager._current._isOpen && !DialogManager._current._isDialogEnabled;
					if (num & flag2)
					{
						Cmd_SendChatMessage("/sit", _setChatChannel);
						_inputBuffer = 0.85f;
					}
				}
				if (_triggerMessageBuffer <= 0f)
				{
					_chatAssets._triggerMessageText.text = string.Empty;
				}
				else
				{
					_triggerMessageBuffer -= Time.deltaTime;
				}
				if (_player._bufferingStatus || HostConsole._current._isOpen || DialogManager._current._isDialogEnabled)
				{
					Display_Chat(_isTrue: false);
				}
				else
				{
					Display_Chat(_focusedInChat);
					if (!_focusedInChat)
					{
						_chatAssets.inputPlaceHolderText.text = "Enter text... (/g, /p, /z)";
						_chatAssets._chatInput.image.color = Color.grey;
						if (_chatBubbleEnabled)
						{
							Cmd_ToggleChatBubble(_isTrue: false);
						}
					}
					else
					{
						_textFadeTimer = 7.5f;
						if (!_chatBubbleEnabled)
						{
							Cmd_ToggleChatBubble(_isTrue: true);
						}
						switch (_chatAssets._chatInput.text)
						{
						case "/g ":
							_setChatChannel = ChatChannel.GLOBAL;
							_chatAssets._chatInput.text = string.Empty;
							_chatAssets._chatInput.ActivateInputField();
							break;
						case "/p ":
							_setChatChannel = ChatChannel.PARTY;
							_chatAssets._chatInput.text = string.Empty;
							_chatAssets._chatInput.ActivateInputField();
							break;
						case "/z ":
							_setChatChannel = ChatChannel.ZONE;
							_chatAssets._chatInput.text = string.Empty;
							_chatAssets._chatInput.ActivateInputField();
							break;
						}
						switch (_setChatChannel)
						{
						case ChatChannel.GLOBAL:
							_chatAssets.inputPlaceHolderText.text = "#Global Chat... (>/g, /p, /z)";
							_chatAssets._chatInput.caretColor = Color.yellow;
							_chatAssets._chatInput.selectionColor = Color.yellow;
							_chatAssets._chatInput.textComponent.color = Color.yellow;
							break;
						case ChatChannel.PARTY:
							_chatAssets.inputPlaceHolderText.text = "#Party Chat... (/g, >/p, /z)";
							_chatAssets._chatInput.caretColor = _chatChannelColors[1];
							_chatAssets._chatInput.selectionColor = _chatChannelColors[1];
							_chatAssets._chatInput.textComponent.color = _chatChannelColors[1];
							break;
						case ChatChannel.ZONE:
							_chatAssets.inputPlaceHolderText.text = "#Zone Chat... (/g, /p, >/z)";
							_chatAssets._chatInput.caretColor = _chatChannelColors[3];
							_chatAssets._chatInput.selectionColor = _chatChannelColors[3];
							_chatAssets._chatInput.textComponent.color = _chatChannelColors[3];
							break;
						}
						if (!_chatAssets._chatInput.IsActive())
						{
							_chatAssets._chatInput.image.color = Color.grey;
						}
						_chatAssets._chatboxTextGroup.alpha = 1f;
						if (_inputBuffer <= 0f)
						{
							bool num2 = InputControlManager.current._setGamepadType != GamepadType.Keyboard && GamepadInput.GetButton(GamepadButton.RightStick_Up);
							if (InputControlManager.current._setGamepadType != GamepadType.Keyboard && GamepadInput.GetButton(GamepadButton.RightStick_Down))
							{
								_chatAssets._chatContent.anchoredPosition += Vector2.up * 4f;
							}
							if (num2)
							{
								_chatAssets._chatContent.anchoredPosition -= Vector2.up * 4f;
							}
							if (Input.GetKeyDown(KeyCode.UpArrow) || (InputControlManager.current._setGamepadType != GamepadType.Keyboard && InputControlManager.current.DpadUp(DpadHoldType.OnDown)))
							{
								_chatAssets._chatInput.ActivateInputField();
								_chatAssets._chatInput.text = _lastMessage;
							}
							if (InputControlManager.current._setGamepadType != GamepadType.Keyboard && GamepadInput.GetButtonDown(GamepadButton.Start))
							{
								_chatAssets._chatInput.ActivateInputField();
								if (SteamManager.Initialized && (SteamUtils.IsSteamRunningOnSteamDeck() || SteamUtils.IsSteamInBigPictureMode()))
								{
									SteamUtils.ShowFloatingGamepadTextInput(EFloatingGamepadTextInputMode.k_EFloatingGamepadTextInputModeModeSingleLine, 0, 0, 500, 200);
								}
							}
							if (Input.GetKeyDown(KeyCode.Escape))
							{
								_focusedInChat = false;
								CameraFunction._current._unlockedCamera = false;
							}
						}
					}
					if (IsChatEnterInput)
					{
						if (!_focusedInChat && _inputBuffer <= 0f)
						{
							if ((bool)_focusInputSfx)
							{
								aSrc.PlayOneShot(_focusInputSfx, 0.55f);
							}
							_chatAssets._chatInput.ActivateInputField();
							_focusedInChat = true;
							_inputBuffer = 0.12f;
						}
						if (_focusedInChat && _inputBuffer <= 0f)
						{
							_focusedInChat = false;
							CameraFunction._current._unlockedCamera = false;
							_inputBuffer = 0.12f;
						}
					}
				}
			}
		}
		void Handle_ChatFade()
		{
			if ((bool)_chatAssets)
			{
				if (Player._mainPlayer._bufferingStatus)
				{
					_chatAssets._chatboxTextGroup.alpha = Mathf.Lerp(_chatAssets._chatboxTextGroup.alpha, 0f, 13.5f * Time.deltaTime);
					_chatAssets._gameLogicGroup.alpha = Mathf.Lerp(_chatAssets._gameLogicGroup.alpha, 0f, 13.5f * Time.deltaTime);
				}
				else
				{
					if (!SettingsManager._current._fadeChatText.isOn)
					{
						_chatAssets._chatboxTextGroup.alpha = Mathf.Lerp(_chatAssets._chatboxTextGroup.alpha, 1f, 13.5f * Time.deltaTime);
						_textFadeTimer = 1f;
					}
					if (!SettingsManager._current._fadeGameFeed.isOn)
					{
						_chatAssets._gameLogicGroup.alpha = Mathf.Lerp(_chatAssets._gameLogicGroup.alpha, 1f, 13.5f * Time.deltaTime);
						_logicFadeTimer = 1f;
					}
					if (_logicFadeTimer > 0f)
					{
						_chatAssets._gameLogicGroup.alpha = 1f;
						if (SettingsManager._current._fadeGameFeed.isOn)
						{
							_logicFadeTimer -= Time.deltaTime;
						}
					}
					else
					{
						_logicFadeTimer = 0f;
						_chatAssets._gameLogicGroup.alpha = Mathf.Lerp(_chatAssets._gameLogicGroup.alpha, 0f, 13.5f * Time.deltaTime);
					}
					if (_textFadeTimer > 0f)
					{
						if (SettingsManager._current._fadeChatText.isOn)
						{
							_textFadeTimer -= Time.deltaTime;
						}
					}
					else if (!_focusedInChat)
					{
						_chatAssets._chatboxTextGroup.alpha = Mathf.Lerp(_chatAssets._chatboxTextGroup.alpha, 0f, 13.5f * Time.deltaTime);
						_textFadeTimer = 0f;
					}
				}
			}
		}
	}

	private void OnServerMessage(ServerMessage _message)
	{
		string servMsg = _message.servMsg;
		if ((!servMsg.Contains('<') || !servMsg.Contains('>')) && servMsg.Length <= 125)
		{
			string text = "<color=#00ffff>" + servMsg + "</color>";
			New_ChatMessage(text ?? "");
		}
	}

	[TargetRpc]
	public void Target_RecieveMessage(string _message)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteString(_message);
		SendTargetRPCInternal(null, "System.Void ChatBehaviour::Target_RecieveMessage(System.String)", -1326188344, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	public void Target_GameLogicMessage(string _message)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteString(_message);
		SendTargetRPCInternal(null, "System.Void ChatBehaviour::Target_GameLogicMessage(System.String)", -958150023, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	public void Target_RecieveTriggerMessage(string _msg)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteString(_msg);
		SendTargetRPCInternal(null, "System.Void ChatBehaviour::Target_RecieveTriggerMessage(System.String)", -573862716, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientCallback]
	public void Client_RecieveTriggerMessage(string _msg)
	{
		if (NetworkClient.active && !(InGameUI._current._mapInstanceTitleGroup.alpha > 0.1f))
		{
			_chatAssets._triggerMessageText.text = _msg ?? "";
			aSrc.PlayOneShot(_triggerMessageSfx, 0.25f);
			_triggerMessageBuffer = 3.85f;
		}
	}

	public void New_ChatMessage(string _message)
	{
		if (!_chatAssets || _chatAssets._chatText == null)
		{
			return;
		}
		if (_chatMessages.Count == _maxChatboxLines)
		{
			_chatMessages.RemoveAt(0);
		}
		_chatMessages.Add(_message);
		_chatAssets._chatText.text = string.Empty;
		TextMeshProUGUI chatText = _chatAssets._chatText;
		chatText.text = chatText.text + "\n<color=#a7fc00>Welcome to ATLYSS (version: " + Application.version + ")</color>";
		if (!AtlyssNetworkManager._current._soloMode && ServerInfoObject._current._serverMOTD != string.Empty)
		{
			TextMeshProUGUI chatText2 = _chatAssets._chatText;
			chatText2.text = chatText2.text + "\n<color=#a7fc00>" + ServerInfoObject._current._serverMOTD + "</color>";
		}
		foreach (string chatMessage in _chatMessages)
		{
			TextMeshProUGUI chatText3 = _chatAssets._chatText;
			chatText3.text = chatText3.text + "\n" + chatMessage;
		}
		if (!InGameUI._current._hideUIHotkeyToggle && !DialogManager._current._isDialogEnabled)
		{
			_chatAssets._chatboxTextGroup.alpha = 1f;
			_textFadeTimer = 7.5f;
		}
	}

	[ClientCallback]
	public void Init_GameLogicMessage(string _message)
	{
		if (!NetworkClient.active || !base.isLocalPlayer || !_chatAssets || _player._bufferingStatus)
		{
			return;
		}
		if (_gameLogicMessages.Count >= _maxGameLogicLines)
		{
			_gameLogicMessages.RemoveAt(0);
		}
		_gameLogicMessages.Add(_message);
		_chatAssets._gameLogicText.text = string.Empty;
		foreach (string gameLogicMessage in _gameLogicMessages)
		{
			TextMeshProUGUI gameLogicText = _chatAssets._gameLogicText;
			gameLogicText.text = gameLogicText.text + "\n" + gameLogicMessage;
		}
		if (!InGameUI._current._hideUIHotkeyToggle && !DialogManager._current._isDialogEnabled)
		{
			_logicFadeTimer = 7.5f;
		}
	}

	[ClientCallback]
	public void Send_ChatMessage(string _message)
	{
		if (!NetworkClient.active || !Input.GetKeyDown(KeyCode.Return) || string.IsNullOrWhiteSpace(_message))
		{
			return;
		}
		_focusedInChat = false;
		if (Iterate_EmoteCommands(_message) && base.isLocalPlayer && _player._pProfession._isProfessionAction)
		{
			_player._pProfession.Cancel_ProfessionAction();
		}
		if (!TabMenu._current._isOpen)
		{
			CameraFunction._current._unlockedCamera = false;
		}
		EventSystem.current.SetSelectedGameObject(null);
		_chatAssets._chatInput.DeactivateInputField();
		_lastMessage = _message;
		bool flag = false;
		if (_message == "/clear")
		{
			_chatMessages.Clear();
			_chatAssets._chatText.text = "";
			flag = true;
		}
		if (_message == "/emotes")
		{
			_chatMessages.Clear();
			for (int i = 0; i < _scriptableEmoteList._emoteCommandList.Length; i++)
			{
				string item = "\n" + _scriptableEmoteList._emoteCommandList[i]._emoteChatCommand;
				TextMeshProUGUI chatText = _chatAssets._chatText;
				chatText.text = chatText.text + "\n" + _scriptableEmoteList._emoteCommandList[i]._emoteChatCommand;
				_chatMessages.Add(item);
			}
			flag = true;
		}
		if ((_message.Contains('<') && _message.Contains('>')) | flag)
		{
			_message = string.Empty;
		}
		if (_message.Length > 125)
		{
			_message = string.Empty;
		}
		_chatAssets._chatInput.text = string.Empty;
		_inputBuffer = 0.25f;
		switch (_setChatChannel)
		{
		case ChatChannel.GLOBAL:
			if (!_inGlobalChat)
			{
				New_ChatMessage("Global chat is disabled.");
				return;
			}
			break;
		case ChatChannel.PARTY:
			if (!_inPartyChat)
			{
				New_ChatMessage("Party chat is disabled.");
				return;
			}
			break;
		case ChatChannel.ZONE:
			if (!_inZoneChat)
			{
				New_ChatMessage("Zone chat is disabled.");
				return;
			}
			break;
		}
		if (_message != string.Empty)
		{
			Cmd_SendChatMessage(_message, _setChatChannel);
		}
	}

	[Command]
	public void Cmd_SendChatMessage(string _message, ChatChannel _chatChannel)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteString(_message);
		GeneratedNetworkCode._Write_ChatBehaviour_002FChatChannel(writer, _chatChannel);
		SendCommandInternal("System.Void ChatBehaviour::Cmd_SendChatMessage(System.String,ChatBehaviour/ChatChannel)", -272093685, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private bool Iterate_EmoteCommands(string _commandName)
	{
		for (int i = 0; i < _scriptableEmoteList._emoteCommandList.Length; i++)
		{
			if (_scriptableEmoteList._emoteCommandList[i]._emoteChatCommand == _commandName)
			{
				_setEmoteIndex = i;
				return true;
			}
		}
		return false;
	}

	[ClientRpc]
	private void Rpc_RecieveChatMessage(string message, bool _isEmoteMessage, ChatChannel _chatChannel)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteString(message);
		writer.WriteBool(_isEmoteMessage);
		GeneratedNetworkCode._Write_ChatBehaviour_002FChatChannel(writer, _chatChannel);
		SendRPCInternal("System.Void ChatBehaviour::Rpc_RecieveChatMessage(System.String,System.Boolean,ChatBehaviour/ChatChannel)", -351496298, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	private void Clear_Textmesh()
	{
		_chatTextMesh.text = string.Empty;
	}

	[ClientCallback]
	private void OnDestroy()
	{
		if (NetworkClient.active && base.isOwned)
		{
			OnChatMessage -= New_ChatMessage;
		}
	}

	public override bool Weaved()
	{
		return true;
	}

	protected void UserCode_Cmd_ToggleChatBubble__Boolean(bool _isTrue)
	{
		Network_chatBubbleEnabled = _isTrue;
	}

	protected static void InvokeUserCode_Cmd_ToggleChatBubble__Boolean(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command Cmd_ToggleChatBubble called on client.");
		}
		else
		{
			((ChatBehaviour)obj).UserCode_Cmd_ToggleChatBubble__Boolean(reader.ReadBool());
		}
	}

	protected void UserCode_Target_RecieveMessage__String(string _message)
	{
		New_ChatMessage("<color=yellow>" + _message + "</color>");
	}

	protected static void InvokeUserCode_Target_RecieveMessage__String(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC Target_RecieveMessage called on server.");
		}
		else
		{
			((ChatBehaviour)obj).UserCode_Target_RecieveMessage__String(reader.ReadString());
		}
	}

	protected void UserCode_Target_GameLogicMessage__String(string _message)
	{
		Init_GameLogicMessage(_message);
	}

	protected static void InvokeUserCode_Target_GameLogicMessage__String(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC Target_GameLogicMessage called on server.");
		}
		else
		{
			((ChatBehaviour)obj).UserCode_Target_GameLogicMessage__String(reader.ReadString());
		}
	}

	protected void UserCode_Target_RecieveTriggerMessage__String(string _msg)
	{
		Client_RecieveTriggerMessage(_msg);
	}

	protected static void InvokeUserCode_Target_RecieveTriggerMessage__String(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC Target_RecieveTriggerMessage called on server.");
		}
		else
		{
			((ChatBehaviour)obj).UserCode_Target_RecieveTriggerMessage__String(reader.ReadString());
		}
	}

	protected void UserCode_Cmd_SendChatMessage__String__ChatChannel(string _message, ChatChannel _chatChannel)
	{
		if (GameManager._current.ContainsUnicodeCharacter(_message))
		{
			_message = string.Empty;
		}
		if (_message.Contains('<') && _message.Contains('>'))
		{
			_message = string.Empty;
		}
		if (_message.Contains('\0'))
		{
			_message = string.Empty;
		}
		if (_message.Length > 125)
		{
			_message = string.Empty;
		}
		if (_messageSendBuffer)
		{
			_message = string.Empty;
		}
		if (_message == string.Empty)
		{
			return;
		}
		if (Iterate_EmoteCommands(_message))
		{
			if (_player._currentPlayerCondition == PlayerCondition.ACTIVE && !(_emoteBuffer > 0f))
			{
				_player._pVisual.IncludeRpc_Crossfade(_scriptableEmoteList._emoteCommandList[_setEmoteIndex]._emoteAnimationTag, 0.1f, 11);
				_emoteBuffer = 0.85f;
				if (_scriptableEmoteList._emoteCommandList[_setEmoteIndex]._emoteChatMessage != string.Empty)
				{
					string message = "<color=yellow>" + _player._nickname + " " + _scriptableEmoteList._emoteCommandList[_setEmoteIndex]._emoteChatMessage + "</color>";
					Rpc_RecieveChatMessage(message, _isEmoteMessage: true, _chatChannel);
				}
			}
		}
		else if (_message == "/afk" && !_player._isAfk)
		{
			_player.Network_isAfk = true;
			_player._afkBufferTimer = 0f;
		}
		else if (_chatChannel == ChatChannel.PARTY && !_player.Network_partyObject)
		{
			Target_RecieveMessage("You must be in a party to send party chat messages.");
		}
		else if (ProfileDataManager._current._hostSettingsProfile._streamMode && !AtlyssNetworkManager._current._soloMode && !_player._isHostPlayer)
		{
			Target_RecieveMessage("Stream Mode is enabled in this lobby.");
		}
		else
		{
			string message2 = _message ?? "";
			Rpc_RecieveChatMessage(message2, _isEmoteMessage: false, _chatChannel);
			_currentMessageSendBuffer = 0.6f;
		}
	}

	protected static void InvokeUserCode_Cmd_SendChatMessage__String__ChatChannel(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command Cmd_SendChatMessage called on client.");
		}
		else
		{
			((ChatBehaviour)obj).UserCode_Cmd_SendChatMessage__String__ChatChannel(reader.ReadString(), GeneratedNetworkCode._Read_ChatBehaviour_002FChatChannel(reader));
		}
	}

	protected void UserCode_Rpc_RecieveChatMessage__String__Boolean__ChatChannel(string message, bool _isEmoteMessage, ChatChannel _chatChannel)
	{
		bool flag = false;
		for (int i = 0; i < AtlyssNetworkManager._current._mutedClientList.Count; i++)
		{
			if (_player._steamID == AtlyssNetworkManager._current._mutedClientList[i]._steamID)
			{
				flag = true;
				break;
			}
		}
		switch (_chatChannel)
		{
		case ChatChannel.GLOBAL:
			if ((bool)Player._mainPlayer && !Player._mainPlayer._chatBehaviour._inGlobalChat)
			{
				return;
			}
			break;
		case ChatChannel.PARTY:
			if (!_player.Network_partyObject || ((bool)Player._mainPlayer && (!Player._mainPlayer._chatBehaviour._inPartyChat || !Player._mainPlayer.Network_partyObject || ((bool)Player._mainPlayer.Network_partyObject && Player._mainPlayer.Network_partyObject != _player.Network_partyObject))))
			{
				return;
			}
			break;
		case ChatChannel.ZONE:
			if (((bool)Player._mainPlayer && !Player._mainPlayer._chatBehaviour._inZoneChat) || Player._mainPlayer.Network_playerMapInstance != _player.Network_playerMapInstance)
			{
				return;
			}
			break;
		}
		if (_isEmoteMessage)
		{
			if (base.isLocalPlayer || !Player._mainPlayer || !(Player._mainPlayer.Network_playerMapInstance != _player.Network_playerMapInstance))
			{
				OnChatMessage?.Invoke(message ?? "");
			}
		}
		else if (!flag)
		{
			string text = "";
			switch (_chatChannel)
			{
			case ChatChannel.GLOBAL:
				text = "<color=#afeeee>[" + _player._nickname + "]</color>: <color=yellow>" + message + "</color>";
				break;
			case ChatChannel.PARTY:
				text = "<color=#afeeee>[" + _player._nickname + "]</color>: <color=#B2EC5D>" + message + "</color>";
				break;
			case ChatChannel.ZONE:
				text = "<color=#afeeee>[" + _player._nickname + "]</color>: <color=#FF8A90>" + message + "</color>";
				break;
			}
			OnChatMessage?.Invoke(text ?? "");
			Init_CharacterTalkEffect();
		}
		void Init_CharacterTalkEffect()
		{
			if (base.isLocalPlayer || !Player._mainPlayer || !(Player._mainPlayer.Network_playerMapInstance != _player.Network_playerMapInstance))
			{
				if (InGameUI._current._displayUI)
				{
					_chatTextMesh.color = _chatChannelColors[(uint)_chatChannel];
					_chatTextMesh.text = message;
				}
				if (IsInvoking("Clear_Textmesh"))
				{
					CancelInvoke("Clear_Textmesh");
				}
				Invoke("Clear_Textmesh", 3.5f);
				if (_player._statusEntity._currentHealth > 0 && (bool)_player._pVisual._playerRaceModel)
				{
					ScriptablePlayerRace scriptablePlayerRace = _player._pVisual._playerRaceModel._scriptablePlayerRace;
					if ((bool)scriptablePlayerRace)
					{
						_pVoiceASrc.pitch = _player._pSound._voicePitch;
						_pVoiceASrc.pitch = UnityEngine.Random.Range(_pVoiceASrc.pitch - 0.059684f, _pVoiceASrc.pitch + 0.059473f);
						if (_pVoiceASrc.isActiveAndEnabled)
						{
							_pVoiceASrc.PlayOneShot(scriptablePlayerRace._chatVoiceClips[UnityEngine.Random.Range(0, scriptablePlayerRace._chatVoiceClips.Length)]);
						}
						_player._pVisual._playerRaceModel.Set_MouthCondition(MouthCondition.Open, 0.45f);
					}
				}
			}
		}
	}

	protected static void InvokeUserCode_Rpc_RecieveChatMessage__String__Boolean__ChatChannel(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC Rpc_RecieveChatMessage called on server.");
		}
		else
		{
			((ChatBehaviour)obj).UserCode_Rpc_RecieveChatMessage__String__Boolean__ChatChannel(reader.ReadString(), reader.ReadBool(), GeneratedNetworkCode._Read_ChatBehaviour_002FChatChannel(reader));
		}
	}

	static ChatBehaviour()
	{
		RemoteProcedureCalls.RegisterCommand(typeof(ChatBehaviour), "System.Void ChatBehaviour::Cmd_ToggleChatBubble(System.Boolean)", InvokeUserCode_Cmd_ToggleChatBubble__Boolean, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(ChatBehaviour), "System.Void ChatBehaviour::Cmd_SendChatMessage(System.String,ChatBehaviour/ChatChannel)", InvokeUserCode_Cmd_SendChatMessage__String__ChatChannel, requiresAuthority: true);
		RemoteProcedureCalls.RegisterRpc(typeof(ChatBehaviour), "System.Void ChatBehaviour::Rpc_RecieveChatMessage(System.String,System.Boolean,ChatBehaviour/ChatChannel)", InvokeUserCode_Rpc_RecieveChatMessage__String__Boolean__ChatChannel);
		RemoteProcedureCalls.RegisterRpc(typeof(ChatBehaviour), "System.Void ChatBehaviour::Target_RecieveMessage(System.String)", InvokeUserCode_Target_RecieveMessage__String);
		RemoteProcedureCalls.RegisterRpc(typeof(ChatBehaviour), "System.Void ChatBehaviour::Target_GameLogicMessage(System.String)", InvokeUserCode_Target_GameLogicMessage__String);
		RemoteProcedureCalls.RegisterRpc(typeof(ChatBehaviour), "System.Void ChatBehaviour::Target_RecieveTriggerMessage(System.String)", InvokeUserCode_Target_RecieveTriggerMessage__String);
	}

	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteBool(_chatBubbleEnabled);
			writer.WriteBool(_joinChatRoomBuffer);
			writer.WriteBool(_messageSendBuffer);
			return;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteBool(_chatBubbleEnabled);
		}
		if ((base.syncVarDirtyBits & 2L) != 0L)
		{
			writer.WriteBool(_joinChatRoomBuffer);
		}
		if ((base.syncVarDirtyBits & 4L) != 0L)
		{
			writer.WriteBool(_messageSendBuffer);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize(ref _chatBubbleEnabled, null, reader.ReadBool());
			GeneratedSyncVarDeserialize(ref _joinChatRoomBuffer, null, reader.ReadBool());
			GeneratedSyncVarDeserialize(ref _messageSendBuffer, null, reader.ReadBool());
			return;
		}
		long num = (long)reader.ReadULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _chatBubbleEnabled, null, reader.ReadBool());
		}
		if ((num & 2L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _joinChatRoomBuffer, null, reader.ReadBool());
		}
		if ((num & 4L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _messageSendBuffer, null, reader.ReadBool());
		}
	}
}
