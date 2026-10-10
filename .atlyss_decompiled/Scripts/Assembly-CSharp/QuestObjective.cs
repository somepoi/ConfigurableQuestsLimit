using System;

[Serializable]
public class QuestObjective
{
	public QuestItemRequirement[] _questItemRequirements;

	public QuestCreepRequirement[] _questCreepRequirements;

	public QuestTriggerRequirement[] _questTriggerRequirements;
}
