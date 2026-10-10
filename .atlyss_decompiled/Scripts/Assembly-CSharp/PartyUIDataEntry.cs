using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class PartyUIDataEntry : MonoBehaviour
{
	[Header("Party UI Data Entry")]
	public Player _player;

	public NetworkIdentity _netID;

	public Color _partySlotColor = Color.white;

	[Space]
	[SerializeField]
	private Text _playerNameText;

	[SerializeField]
	private Text _playerLevelCounterText;

	[SerializeField]
	private Text _playerMapLocationText;

	[SerializeField]
	private GameObject _playerVersusTagObject;

	[SerializeField]
	private GameObject _partyLeaderCrownObject;

	[SerializeField]
	private Image _raceAvatar;

	[SerializeField]
	private Image _partyIndicatorSlotIcon;

	[SerializeField]
	private Image _healthbarFill;

	[SerializeField]
	private Image _manabarFill;

	[SerializeField]
	private Image _classIcon;

	private void Update()
	{
		if (AtlyssNetworkManager._current._soloMode)
		{
			base.enabled = false;
		}
		else
		{
			if (!Player._mainPlayer || !Player._mainPlayer.Network_partyObject || !_player || !_netID)
			{
				return;
			}
			if (!_player.Network_partyObject)
			{
				_player = null;
				_netID = null;
				return;
			}
			_playerNameText.text = _player._nickname ?? "";
			_playerLevelCounterText.text = $"{_player._pStats._currentLevel}";
			_partyIndicatorSlotIcon.color = _partySlotColor;
			_player._partyIndicatorIcon.color = _partySlotColor;
			if (!string.IsNullOrWhiteSpace(_player._mapName))
			{
				_playerMapLocationText.text = _player._mapName ?? "";
			}
			else
			{
				_playerMapLocationText.text = "...";
			}
			_healthbarFill.fillAmount = (float)_player._statusEntity._currentHealth / (float)_player._pStats._statStruct._maxHealth;
			_manabarFill.fillAmount = (float)_player._statusEntity._currentMana / (float)_player._pStats._statStruct._maxMana;
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
			if ((bool)_player._pVisual._playerRaceModel)
			{
				_raceAvatar.sprite = _player._pVisual._playerRaceModel._scriptablePlayerRace._raceAvatar;
			}
			_playerVersusTagObject.SetActive(_player._pPvp._currentVersusFlag == VersusFlag.PVP);
			_partyLeaderCrownObject.SetActive(_player.Network_partyObject.Network_partyLeaderPlayer == _player);
		}
	}
}
