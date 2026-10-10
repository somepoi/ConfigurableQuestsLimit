using UnityEngine;

[CreateAssetMenu(fileName = "SKILLSCROLL_", menuName = "ScriptableObjects/Item/Consumables/Skill Scroll", order = 0)]
public class ScriptableSkillScroll : ScriptableConsumable
{
	[Header("Skill Scroll Consumable")]
	public ScriptableSkill _scriptableSkill;

	public override bool CanUseConsumable(StatusEntity _statusEntity, bool _isButtonPrompt)
	{
		PlayerCasting pCasting = _statusEntity._isPlayer._pCasting;
		for (int i = 0; i < pCasting._playerSkillStructs.Count; i++)
		{
			if (pCasting._playerSkillStructs[i]._skillName == _scriptableSkill._skillName)
			{
				if (_isButtonPrompt)
				{
					ErrorPromptTextManager.current.Init_ErrorPrompt("Already Learned Skill");
				}
				return false;
			}
		}
		for (int j = 0; j < pCasting._bonusSkillSlots.Length; j++)
		{
			if (pCasting._bonusSkillSlots[j]._bonusSkillTag == _scriptableSkill._skillName && pCasting._bonusSkillSlots[j]._isLearned)
			{
				if (_isButtonPrompt)
				{
					ErrorPromptTextManager.current.Init_ErrorPrompt("Already Learned Skill");
				}
				return false;
			}
		}
		return true;
	}

	public override void Use_Consumable(StatusEntity _statusEntity)
	{
		Apply_SkillScroll(_statusEntity);
	}

	private void Apply_SkillScroll(StatusEntity _statusEntity)
	{
		if (!_statusEntity.isLocalPlayer || !_statusEntity._isPlayer)
		{
			return;
		}
		PlayerCasting pCasting = _statusEntity._isPlayer._pCasting;
		ChatBehaviour chatBehaviour = _statusEntity._isPlayer._chatBehaviour;
		int num = 0;
		for (int i = 0; i < pCasting._bonusSkillSlots.Length; i++)
		{
			if (pCasting._bonusSkillSlots[i]._isLearned)
			{
				num++;
			}
			if (pCasting._bonusSkillSlots[i]._bonusSkillTag == _scriptableSkill._skillName)
			{
				pCasting._bonusSkillSlots[i]._isLearned = true;
			}
		}
		if (num >= 12)
		{
			SteamAchievementManager.current.Init_EarnAchievement("ATLYSS_ACHIEVEMENT_16");
		}
		ProfileDataManager._current.Save_ProfileData();
		chatBehaviour.Init_GameLogicMessage("Learned Skill: " + _scriptableSkill._skillName);
		pCasting.Init_SkillLibrary();
		QuestTrigger_OnUse();
	}

	public override void OnValidate()
	{
		base.OnValidate();
		if ((bool)_scriptableSkill)
		{
			_itemName = "Skill Scroll (" + _scriptableSkill._skillName + ")";
			_itemDescription = "Teaches the user " + _scriptableSkill._skillName + ", applying it to the general skillbook.";
			_itemSortTag = SortTag.SKILLSCROLL;
		}
	}
}
