using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class HC_PeerListEntry : ListDataEntry
{
	[Header("Host Console Peer List Entry")]
	public NetworkIdentity _netId;

	public Player _peerPlayer;

	public Button _entryButton;

	public Image _highLightSelect;

	private void Awake()
	{
		_entryButton.onClick.AddListener(Select_Entry);
	}

	private void Update()
	{
		_highLightSelect.enabled = HostConsole._current._selectedPeerEntry == this;
	}

	private void Select_Entry()
	{
		HostConsole._current._selectedPeerEntry = this;
	}

	public void Apply_PeerDataEntry()
	{
		string arg = "Connecting";
		string empty = string.Empty;
		if ((bool)_peerPlayer)
		{
			arg = $"{_peerPlayer._nickname} [{_peerPlayer._latency}ms]";
		}
		_dataNameText.text = $"[connID: {_dataID}] {arg} {empty}";
	}
}
