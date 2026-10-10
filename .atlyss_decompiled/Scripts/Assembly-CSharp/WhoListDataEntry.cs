using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class WhoListDataEntry : ListDataEntry
{
	[Header("Who List Data Entry Runtime")]
	public Player _player;

	public NetworkIdentity _netId;

	private PlayerStats _pStats;

	[Header("Who Data Entry Buttons")]
	public Button _dataEntryButton;

	public Button _steamProfileButton;

	[Header("Who Data Entry Icons")]
	public Image _highlightImage;

	public Image _classIcon;

	[SerializeField]
	private Image _partyCheckmarkIco;

	[SerializeField]
	private Image _partyLeaderIco;

	[SerializeField]
	private Image _mutedErrorSymbol;

	[SerializeField]
	private Image _afkSymbol;

	[Header("Who Data Entry Text")]
	[SerializeField]
	private Text _nicknameText;

	[SerializeField]
	private Text _globalNameText;

	[SerializeField]
	private Text _locationText;

	[SerializeField]
	private Text _latencyText;

	private void Awake()
	{
		_dataEntryButton.onClick.AddListener(Select_Entry);
		_steamProfileButton.onClick.AddListener(Select_Entry_SteamProfile);
	}

	private void Select_Entry()
	{
		TabMenu._current._whoCell._selectedDataEntry = this;
	}

	private void Select_Entry_SteamProfile()
	{
		TabMenu._current._whoCell._selectedDataEntry = this;
		TabMenu._current._whoCell.Init_OpenSteamProfile();
	}

	private void Update()
	{
		Handle_WhoDataEntry();
	}

	private void Handle_WhoDataEntry()
	{
		_steamProfileButton.gameObject.SetActive(SteamManager.Initialized);
		if (!_player || !_netId)
		{
			return;
		}
		if (!_pStats)
		{
			_pStats = _player._pStats;
		}
		if (!string.IsNullOrWhiteSpace(_player._globalNickname))
		{
			_globalNameText.color = Color.cyan;
			_nicknameText.text = $"{_player._nickname} <color=yellow>(lv-{_pStats._currentLevel})</color>";
			_globalNameText.text = "@" + _player._globalNickname;
		}
		else
		{
			_nicknameText.color = Color.white;
			_nicknameText.text = $"{_player._nickname} <color=yellow>(lv-{_pStats._currentLevel})</color>";
			_globalNameText.text = string.Empty;
		}
		bool flag = _player.Network_partyObject;
		bool flag2 = flag && _player.Network_partyObject.Network_partyLeaderPlayer == _player;
		_partyCheckmarkIco.enabled = flag;
		_partyLeaderIco.enabled = flag2;
		if (string.IsNullOrWhiteSpace(_player._mapName))
		{
			_locationText.text = "...";
		}
		else
		{
			_locationText.text = _player._mapName ?? "";
		}
		if (_player._latency <= 0)
		{
			_latencyText.text = "Host";
		}
		else
		{
			_latencyText.text = $"{_player._latency}ms";
		}
		_highlightImage.enabled = TabMenu._current._whoCell._selectedDataEntry == this;
		_afkSymbol.enabled = _player._isAfk;
		_mutedErrorSymbol.enabled = false;
		if ((bool)_player._pStats._class)
		{
			if (_player._pStats._syncClassTier > 0)
			{
				_classIcon.sprite = _player._pStats._class._playerClassTiers[_player._pStats._syncClassTier - 1]._classTierIcon;
			}
			else
			{
				_classIcon.sprite = _player._pStats._class._classIcon;
			}
		}
		else
		{
			_classIcon.sprite = GameManager._current._statLogics._emptyClassEmblem;
		}
		for (int i = 0; i < AtlyssNetworkManager._current._mutedClientList.Count; i++)
		{
			if (_player._steamID == AtlyssNetworkManager._current._mutedClientList[i]._steamID)
			{
				_mutedErrorSymbol.enabled = true;
				break;
			}
		}
	}
}
