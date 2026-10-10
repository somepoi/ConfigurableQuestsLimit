using System;
using UnityEngine;

[Serializable]
public class PlayerClassTier
{
	public string _classTierName = string.Empty;

	public Sprite _classTierIcon;

	public Color _blockTierEmissionColor = Color.grey;

	public ScriptableSkill[] _classTierSkills;
}
