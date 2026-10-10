using Mirror;
using UnityEngine;

[CreateAssetMenu(fileName = "quest_", menuName = "ScriptableObjects/Game Logic/Quest", order = 0)]
public class ScriptableQuest : ScriptableObject
{
	[Header("Scriptable Quest")]
	public string _questName = "new quest";

	public QuestType _questType = QuestType.REPEATABLE;

	public QuestSubType _questSubType;

	public Sprite _questIco;

	[TextArea(25, 25)]
	public string _questDescription;

	public string _questCompleteReturnMessage = "Return to (NPC) in (MAP).";

	[Header("Quest Requirements")]
	public int _questLevel = 1;

	[Space]
	public ScriptablePlayerRace _raceRequirement;

	public ScriptablePlayerBaseClass _baseClassRequirement;

	public ScriptableSkill _skillToHide;

	public bool _requireNoviceClass;

	public bool _autoFinishQuest;

	[Space]
	public ScriptableQuest[] _preQuestRequirements;

	[Header("Quest Objective")]
	public QuestItemReward _questObjectiveItem;

	[Space]
	[Scene]
	public string _scenePath;

	public QuestObjective _questObjective;

	[Header("Quest Rewards")]
	[Range(0f, 9.9f)]
	public float _questExperiencePercentage = 0.15f;

	public int _questCurrencyReward = 50;

	public QuestItemReward[] _questItemRewards;

	private void OnValidate()
	{
		if (_questLevel < 1)
		{
			_questLevel = 1;
		}
		if (_questCurrencyReward < 0)
		{
			_questCurrencyReward = 0;
		}
		if (_questObjective._questItemRequirements != null)
		{
			int num = 0;
			for (int i = 0; i < _questObjective._questItemRequirements.Length; i++)
			{
				if (_questObjective._questItemRequirements[i]._itemsNeeded < 1)
				{
					_questObjective._questItemRequirements[i]._itemsNeeded = 1;
				}
				num += _questObjective._questItemRequirements[i]._itemsNeeded;
			}
		}
		if (_questObjective._questCreepRequirements == null)
		{
			return;
		}
		int num2 = 0;
		for (int j = 0; j < _questObjective._questCreepRequirements.Length; j++)
		{
			if (_questObjective._questCreepRequirements[j]._creepsKilled < 1)
			{
				_questObjective._questCreepRequirements[j]._creepsKilled = 1;
			}
			num2 += _questObjective._questCreepRequirements[j]._creepsKilled;
		}
	}
}
