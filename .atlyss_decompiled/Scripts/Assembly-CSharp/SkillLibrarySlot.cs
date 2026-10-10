using System;

[Serializable]
public class SkillLibrarySlot
{
	public string _skillTag = string.Empty;

	public ScriptableSkill _scriptSkill;

	public int _skillStructIndex;

	public SkillTier _skillTier;
}
