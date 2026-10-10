using System;
using UnityEngine;

[Serializable]
public struct ShopkeepItemStruct
{
	public string _itemName;

	public ItemData _itemData;

	[Space]
	public int _stockQuantity;

	public bool _removeAtEmptyStock;

	public int _equipModifierID;

	public int _dedicatedValue;

	public bool _useDedicatedValue;

	[Space]
	public string _specialItemCostName;

	public int _specialItemCostQuantity;

	public bool _isbuybackItem;

	public bool _isGambleItem;

	public int _gambleValue;
}
