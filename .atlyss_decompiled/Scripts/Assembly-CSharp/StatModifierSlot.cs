using System;
using UnityEngine;

[Serializable]
public class StatModifierSlot
{
	[HideInInspector]
	public string _equipModifierTag = string.Empty;

	public ScriptableStatModifier _equipModifier;

	[Range(0f, 1f)]
	public float _modifierChance = 0.05f;
}
