using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectListDataEntry : ListDataEntry
{
	[Header("Character Select Data Entry")]
	public CharacterFile _characterFileData;

	public Button _characterFileSelectButton;

	[SerializeField]
	private TextMeshProUGUI _characterNicknameText;

	[SerializeField]
	private TextMeshProUGUI _characterInfoText;

	[SerializeField]
	private Image _selectedImage;

	[SerializeField]
	private Image _raceEmblemIcon;

	[SerializeField]
	private Image _classEmblemIcon;

	[SerializeField]
	private GameObject _gamepadIconObject;

	public Text _fileNumberText;

	private void Start()
	{
		_characterFileSelectButton.onClick.AddListener(Select_CharacterData);
	}

	public void Select_CharacterData()
	{
		ProfileDataManager._current.Set_FileIndex(_dataID);
	}

	private void Update()
	{
		_characterFileSelectButton.interactable = _characterFileData != ProfileDataManager._current._characterFile && !MainMenuManager._current._characterSelectManager._deleteCharacterPrompt;
		_selectedImage.gameObject.SetActive(_characterFileData == ProfileDataManager._current._characterFile);
		_gamepadIconObject.SetActive(InputControlManager.current._setGamepadType != GamepadType.Keyboard);
		_raceEmblemIcon.gameObject.SetActive(!_characterFileData._isEmptySlot);
		_classEmblemIcon.gameObject.SetActive(!_characterFileData._isEmptySlot);
		if (_characterFileData._isEmptySlot)
		{
			Handle_EmptySlotDisplay();
		}
		else
		{
			Handle_FilledSlotDisplay();
		}
		void Handle_EmptySlotDisplay()
		{
			_characterNicknameText.text = "Empty Slot";
			_characterInfoText.text = "";
		}
		void Handle_FilledSlotDisplay()
		{
			ScriptablePlayerRace scriptablePlayerRace = GameManager._current.Locate_PlayerRace(_characterFileData._appearanceProfile._setRaceTag);
			_characterFileData = ProfileDataManager._current._characterFiles[_dataID];
			if ((bool)scriptablePlayerRace)
			{
				_raceEmblemIcon.sprite = scriptablePlayerRace._raceEmblemIcon;
			}
			_characterNicknameText.text = _characterFileData._nickName ?? "";
			if (_characterFileData._statsProfile._classID == string.Empty)
			{
				_characterInfoText.text = $"Lv-{_characterFileData._statsProfile._currentLevel} {_characterFileData._appearanceProfile._setRaceTag} {GameManager._current._statLogics._emptyClassName}";
			}
			else
			{
				ScriptablePlayerBaseClass scriptablePlayerBaseClass = GameManager._current.Locate_PlayerClass(_characterFileData._statsProfile._classID);
				if (_characterFileData._statsProfile._classTier > 0)
				{
					_characterInfoText.text = $"Lv-{_characterFileData._statsProfile._currentLevel} {_characterFileData._appearanceProfile._setRaceTag} {scriptablePlayerBaseClass._playerClassTiers[_characterFileData._statsProfile._classTier - 1]._classTierName}";
					_classEmblemIcon.sprite = scriptablePlayerBaseClass._playerClassTiers[_characterFileData._statsProfile._classTier - 1]._classTierIcon;
				}
				else
				{
					_characterInfoText.text = $"Lv-{_characterFileData._statsProfile._currentLevel} {_characterFileData._appearanceProfile._setRaceTag} {scriptablePlayerBaseClass._className}";
					_classEmblemIcon.sprite = scriptablePlayerBaseClass._classIcon;
				}
			}
			if (string.IsNullOrEmpty(_characterFileData._statsProfile._classID))
			{
				_classEmblemIcon.sprite = GameManager._current._statLogics._emptyClassEmblem;
			}
		}
	}
}
