using System;
using UnityEngine;

[Serializable]
public class PlayerStats_Profile
{
	public int _currentLevel;

	public string _classID;

	public int _classTier;

	public int _currentExp;

	public int _skillPoints;

	public int _attributePoints;

	[Space]
	public AttributeStruct[] _attributes;

	public ProfessionStruct[] _professions;
}
