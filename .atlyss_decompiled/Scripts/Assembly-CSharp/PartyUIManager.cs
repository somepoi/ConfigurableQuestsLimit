using UnityEngine;
using UnityEngine.UI;

public class PartyUIManager : MonoBehaviour
{
	public static PartyUIManager _current;

	[Header("Party UI Management")]
	[SerializeField]
	private CanvasGroup _partyPanelGroup;

	public MenuElement _partyInviteElement;

	public Text _invitedByPrompt;

	public Button _acceptInviteButton;

	public Button _declineInviteButton;

	[Space]
	public PartyObjectBehavior _partyObject;

	public PartyUIDataEntry[] _partyDataEntries = new PartyUIDataEntry[5];

	[Space]
	public Transform _partyDataEntryContainer;

	public Transform _emptyEntryContainer;

	[Header("Gamepad")]
	[SerializeField]
	private Image _gamepad_joinPartyIcon;

	[SerializeField]
	private Image _gamepad_declinePartyIcon;

	[Header("Audio")]
	[SerializeField]
	private AudioSource _aSrc;

	[SerializeField]
	private AudioClip _partyFormSfx;

	[SerializeField]
	private AudioClip _partyJoinSfx;

	[SerializeField]
	private AudioClip _partyInviteSfx;

	private void Awake()
	{
		_current = this;
		_acceptInviteButton.onClick.AddListener(AcceptPartyInvite);
		_declineInviteButton.onClick.AddListener(DeclinePartyInvite);
	}

	private void AcceptPartyInvite()
	{
		Player._mainPlayer.Cmd_SetPartyInviteCondition(PartyInviteStatus.ACCEPTED);
	}

	private void DeclinePartyInvite()
	{
		Player._mainPlayer.Cmd_SetPartyInviteCondition(PartyInviteStatus.DECLINED);
	}

	public void Init_PartyInviteWindow()
	{
		_aSrc.PlayOneShot(_partyInviteSfx, 1.25f);
		_partyInviteElement.isEnabled = true;
	}

	private void Update()
	{
		Handle_PartyUIDisplay();
	}

	private void Handle_PartyUIDisplay()
	{
		if (AtlyssNetworkManager._current._soloMode)
		{
			base.enabled = false;
		}
		else
		{
			if (!Player._mainPlayer)
			{
				return;
			}
			if (!_partyObject && (bool)Player._mainPlayer.Network_partyObject)
			{
				_aSrc.PlayOneShot(_partyFormSfx, 0.45f);
			}
			_partyObject = Player._mainPlayer.Network_partyObject;
			if (!_partyObject)
			{
				_partyPanelGroup.alpha = 0f;
			}
			else
			{
				_partyPanelGroup.alpha = 1f;
			}
			_partyPanelGroup.interactable = _partyObject;
			_partyPanelGroup.blocksRaycasts = _partyObject;
			_gamepad_joinPartyIcon.enabled = InputControlManager.current._setGamepadType != GamepadType.Keyboard;
			_gamepad_declinePartyIcon.enabled = InputControlManager.current._setGamepadType != GamepadType.Keyboard;
			if (Player._mainPlayer._partyInviteStatus == PartyInviteStatus.INVITED)
			{
				if (GamepadInput.GetButtonDown(GamepadButton.Start))
				{
					AcceptPartyInvite();
				}
				if (GamepadInput.GetButtonDown(GamepadButton.Back))
				{
					DeclinePartyInvite();
				}
			}
			for (int i = 0; i < _partyDataEntries.Length; i++)
			{
				if ((bool)_partyDataEntries[i]._player && !_partyDataEntries[i]._player.Network_partyObject)
				{
					Clear_PartyDataEntry(_partyDataEntries[i]);
					break;
				}
				if ((bool)_partyDataEntries[i]._player && _partyDataEntries[i]._player.Network_partyObject != Player._mainPlayer.Network_partyObject)
				{
					Clear_PartyDataEntry(_partyDataEntries[i]);
					break;
				}
				if (!_partyDataEntries[i]._player)
				{
					Clear_PartyDataEntry(_partyDataEntries[i]);
				}
			}
			if (!_partyObject)
			{
				for (int j = 0; j < _partyDataEntries.Length; j++)
				{
					if ((bool)_partyDataEntries[j]._player)
					{
						Clear_PartyDataEntry(_partyDataEntries[j]);
					}
				}
				return;
			}
			for (int k = 0; k < _partyObject._syncPartyPeers.Count; k++)
			{
				Player player = _partyObject._syncPartyPeers[k];
				if (player == Player._mainPlayer)
				{
					continue;
				}
				for (int l = 0; l < _partyDataEntries.Length; l++)
				{
					if ((bool)_partyDataEntries[l]._player)
					{
						continue;
					}
					bool flag = false;
					for (int m = 0; m < _partyDataEntries.Length; m++)
					{
						if (_partyDataEntries[m]._player == player)
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						_partyDataEntries[l]._player = player;
						_partyDataEntries[l]._netID = player.netIdentity;
						_partyDataEntries[l].transform.SetParent(_partyDataEntryContainer);
					}
				}
			}
		}
	}

	private void Clear_PartyDataEntry(PartyUIDataEntry _entry)
	{
		_entry.transform.SetParent(_emptyEntryContainer);
		_entry.transform.localPosition = Vector3.zero;
		_entry.transform.localScale = Vector3.zero;
		_entry._player = null;
		_entry._netID = null;
	}
}
