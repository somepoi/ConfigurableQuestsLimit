using System;
using UnityEngine;

[Serializable]
public class ItemDrop
{
	[HideInInspector]
	public string _itemTag = string.Empty;

	public ScriptableItem _item;

	[Range(0f, 1f)]
	public float _dropChance = 1f;

	[Range(1f, 20f)]
	public int _dropQuantity = 1;

	[Space]
	public ScriptableQuest _questRequirement;

	public bool _checkIfPlayerHasItem;

	[Header("Item Drop Event Month")]
	public int _eventMonth;

	public bool _useEventMonth;
}
