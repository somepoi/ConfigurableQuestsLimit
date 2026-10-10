using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatBehaviourAssets : MonoBehaviour
{
	public static ChatBehaviourAssets _current;

	public ChatBehaviour _setChatBehaviour;

	[Header("Chat Canvas Groups")]
	public CanvasGroup _generalCanvasGroup;

	public CanvasGroup _chatboxTextGroup;

	public CanvasGroup _gameLogicGroup;

	public CanvasGroup[] _chatTextGroups;

	[Header("Chat Channel Dock")]
	public GameObject _chatChannelDockObject;

	public Button _channelButton_global;

	public Button _channelButton_party;

	public Button _channelButton_room;

	public Text _channelButton_roomText;

	public Image _globalButton_highlight;

	public Image _partyButton_highlight;

	public Image _roomButton_highlight;

	[Header("UI Elements")]
	public RectTransform _chatContent;

	public InputField _chatInput;

	public TextMeshProUGUI _chatText;

	public Text inputPlaceHolderText;

	[Space]
	public TextMeshProUGUI _gameLogicText;

	public TextMeshProUGUI _triggerMessageText;

	[Header("Gamepad")]
	public Image _gamepad_inputIco;

	private void Awake()
	{
		_current = this;
	}

	private void Start()
	{
		_chatText.text = string.Empty;
		_gameLogicText.text = string.Empty;
		_triggerMessageText.text = string.Empty;
		for (int i = 0; i < _chatTextGroups.Length; i++)
		{
			_chatTextGroups[i].alpha = 0f;
		}
	}

	private void Update()
	{
		if ((bool)Player._mainPlayer)
		{
			if (!_setChatBehaviour)
			{
				_setChatBehaviour = Player._mainPlayer._chatBehaviour;
			}
			else if (!_setChatBehaviour._chatAssets && !Player._mainPlayer._bufferingStatus)
			{
				_setChatBehaviour._chatAssets = this;
				_channelButton_global.onClick.AddListener(_setChatBehaviour.OnClick_GlobalChannel);
				_channelButton_party.onClick.AddListener(_setChatBehaviour.OnClick_PartyChannel);
				_channelButton_room.onClick.AddListener(_setChatBehaviour.OnClick_ZoneChannel);
				_chatInput.onSubmit.AddListener(_setChatBehaviour.Send_ChatMessage);
				TextMeshProUGUI chatText = _chatText;
				chatText.text = chatText.text + "\n<color=#a7fc00>Welcome to ATLYSS (version: " + Application.version + ")</color>";
			}
			if ((bool)_setChatBehaviour)
			{
				_generalCanvasGroup.blocksRaycasts = Player._mainPlayer._inChat;
			}
			if (InGameUI._current._hideUIHotkeyToggle)
			{
				_triggerMessageText.text = string.Empty;
			}
			_gamepad_inputIco.enabled = InputControlManager.current._setGamepadType != GamepadType.Keyboard;
		}
	}
}
