using UnityEngine;
using UnityEngine.UI;

public class QuestTrackElement : MonoBehaviour
{
	[Header("Quest Track Element")]
	public ScriptableQuest _scriptQuest;

	public Text _trackQuestNameText;

	public Text _trackElementText;

	[Space]
	public int _questIndex;

	public bool _isHidden;

	public void Update_QuestTrackElement()
	{
		if (!_scriptQuest || !Player._mainPlayer || SettingsManager._current._hideQuestTracker.isOn)
		{
			return;
		}
		PlayerQuesting pQuest = Player._mainPlayer._pQuest;
		if (pQuest._questProgressData.Count <= 0)
		{
			return;
		}
		_isHidden = pQuest._questProgressData[_questIndex]._hideQuestTrack;
		_trackQuestNameText.text = _scriptQuest._questName ?? "";
		_trackElementText.text = string.Empty;
		if (_scriptQuest._questSubType == QuestSubType.MAIN_QUEST)
		{
			_trackQuestNameText.text = "<color=cyan>" + _scriptQuest._questName + "</color>";
		}
		string text = "<color=lime>";
		string text2 = "<color=white>";
		for (int i = 0; i < _scriptQuest._questObjective._questItemRequirements.Length; i++)
		{
			QuestItemRequirement questItemRequirement = _scriptQuest._questObjective._questItemRequirements[i];
			string text3 = ((pQuest._questProgressData[_questIndex]._itemProgressValues[i] < questItemRequirement._itemsNeeded) ? text2 : text);
			if (!string.IsNullOrEmpty(_trackElementText.text))
			{
				_trackElementText.text += "\n";
			}
			_trackElementText.text += $"{text3}{questItemRequirement._questItem._itemName}: ({pQuest._questProgressData[_questIndex]._itemProgressValues[i]} / {questItemRequirement._itemsNeeded})</color>";
		}
		for (int j = 0; j < _scriptQuest._questObjective._questCreepRequirements.Length; j++)
		{
			QuestCreepRequirement questCreepRequirement = _scriptQuest._questObjective._questCreepRequirements[j];
			string text3 = ((pQuest._questProgressData[_questIndex]._creepKillProgressValues[j] < questCreepRequirement._creepsKilled) ? text2 : text);
			string text4 = questCreepRequirement._questCreep._creepName + "s slain: ";
			string text5 = questCreepRequirement._questCreep._creepName + " slain: ";
			string empty = string.Empty;
			empty = ((questCreepRequirement._creepsKilled <= 1) ? text5 : text4);
			if (!string.IsNullOrEmpty(_trackElementText.text))
			{
				_trackElementText.text += "\n";
			}
			_trackElementText.text += $"{text3}{empty} ({pQuest._questProgressData[_questIndex]._creepKillProgressValues[j]} / {questCreepRequirement._creepsKilled})</color>";
		}
		for (int k = 0; k < _scriptQuest._questObjective._questTriggerRequirements.Length; k++)
		{
			QuestTriggerRequirement questTriggerRequirement = _scriptQuest._questObjective._questTriggerRequirements[k];
			string text3 = ((pQuest._questProgressData[_questIndex]._triggerProgressValues[k] < questTriggerRequirement._triggerEmitsNeeded) ? text2 : text);
			if (!string.IsNullOrEmpty(_trackElementText.text))
			{
				_trackElementText.text += "\n";
			}
			_trackElementText.text += $"{text3}{questTriggerRequirement._prefix} {questTriggerRequirement._suffix}: ({pQuest._questProgressData[_questIndex]._triggerProgressValues[k]} / {questTriggerRequirement._triggerEmitsNeeded})</color>";
		}
		if (pQuest._questProgressData[_questIndex]._questComplete)
		{
			_trackElementText.color = Color.green;
			Text trackElementText = _trackElementText;
			trackElementText.text = trackElementText.text + "\n" + _scriptQuest._questCompleteReturnMessage;
		}
		else
		{
			_trackElementText.color = Color.white;
		}
	}
}
