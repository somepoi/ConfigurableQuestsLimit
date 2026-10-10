using System;
using UnityEngine;

[Serializable]
public class SkillLoadOutSlot
{
	public string _loadoutTag = string.Empty;

	public CombatColliderType _utilityType;

	public int _skillStructIndex;

	[Space]
	public ScriptableSkill _scriptSkill;

	public ScriptableSkill _altScriptSkill;
}
