using UnityEngine;
using UnityEngine.UI;

public class ProfessionCellSlot : MonoBehaviour
{
	public ScriptableProfession _scriptableProfession;

	public ProfessionStruct _professionStruct;

	public int _professionID;

	[Space]
	public Text _professionNameText;

	public Text _professionLevelText;

	public Text _professionExperienceCounter;

	[Space]
	public Image _professionIcon;

	public Image _professionExpFill;

	private float _expFillAmount;

	[Space]
	[SerializeField]
	private bool _isInGameUICell;

	private void Update()
	{
		if (!Player._mainPlayer || (!_isInGameUICell && !TabMenu._current._isOpen) || (!_isInGameUICell && TabMenu._current._selectedMenuCell != TabMenu._current._statsCell))
		{
			return;
		}
		if (!_scriptableProfession)
		{
			if (!_isInGameUICell)
			{
				base.gameObject.SetActive(value: false);
			}
			return;
		}
		PlayerStats pStats = Player._mainPlayer._pStats;
		_professionStruct = pStats._playerProfessions[_professionID];
		_professionNameText.text = _scriptableProfession._professionName ?? "";
		_professionLevelText.text = $"Lv-{_professionStruct._professionLvl}";
		if (_professionIcon.sprite != _scriptableProfession._professionIcon)
		{
			_professionIcon.sprite = _scriptableProfession._professionIcon;
		}
		Handle_ExperienceBar();
		void Handle_ExperienceBar()
		{
			if (_professionStruct._professionLvl < _scriptableProfession._maxProfessionLevel)
			{
				int num = (int)_scriptableProfession._professionCurve.Evaluate(_professionStruct._professionLvl);
				_professionExperienceCounter.text = $"{_professionStruct._professionXP} / {num}";
				_expFillAmount += ((float)_professionStruct._professionXP / (float)num - _expFillAmount) * 0.2f;
				if (float.IsNaN(_expFillAmount))
				{
					_expFillAmount = 0f;
				}
				_professionExpFill.fillAmount = _expFillAmount;
			}
			else
			{
				_professionExperienceCounter.text = "MAX";
				_professionExpFill.fillAmount = 1f;
			}
		}
	}
}
