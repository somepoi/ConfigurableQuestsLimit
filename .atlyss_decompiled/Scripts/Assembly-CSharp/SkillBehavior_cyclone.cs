using UnityEngine;

public class SkillBehavior_cyclone : SkillObjectBehavior
{
	[Header("Weapon Kick Skill")]
	[SerializeField]
	private RaceModelEquipDisplay _pRME;

	[SerializeField]
	private PlayerCombat _pCombat;

	private bool _isComplete;

	private void OnEnable()
	{
		_isComplete = false;
	}

	public override void Init_SkillBehavior()
	{
	}

	public override void Update_SkillBehavior()
	{
		_pCombat = _skillObject._parentPlayer._pCombat;
		_pRME = _skillObject._parentPlayer._pVisual._raceModelED;
		if ((bool)_pRME && (bool)_pCombat && !_isComplete)
		{
			_pRME._hideTakenOutWeaponBuffer = 0.1f;
			_pRME._hidePutAwayWeaponBuffer = 0.1f;
			_pCombat._isDisarmed = true;
			if (_skillObject._parentPlayer._bufferingStatus || _skillObject._parentPlayer._currentPlayerCondition != PlayerCondition.ACTIVE)
			{
				End_SkillBehavior();
			}
		}
	}

	public override void End_SkillBehavior()
	{
		_isComplete = true;
		_pCombat._isDisarmed = false;
	}
}
