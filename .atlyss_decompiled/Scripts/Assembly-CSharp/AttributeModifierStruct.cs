using System;
using UnityEngine;

[Serializable]
public class AttributeModifierStruct
{
	public StatStruct _statStruct;

	[Space]
	public float _statBaseMultiplier = 0.05f;

	public float _statIncreasePercentage;

	public float _statDampen;
}
