using System;
using UnityEngine;

[Serializable]
public class ShopkeepItem
{
	public string _itemNameTag = string.Empty;

	public ScriptableItem _scriptItem;

	[Header("Costs / Value")]
	public int _dedicatedItemValue;

	public bool _useDedicatedItemValue = true;

	public StatModifierCost _specialStoreCost = new StatModifierCost();

	[Header("Equipment Modifier Params")]
	public ScriptableStatModifier _setStatModifier;

	public bool _randomizeModifier;

	[Header("Gamble Params")]
	public ScriptableLootTable _gambleLootTable;

	public int _gambleValue = 100;

	public bool _isGambleSlot;
}
