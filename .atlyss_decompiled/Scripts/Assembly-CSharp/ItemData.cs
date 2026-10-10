using System;
using UnityEngine;

[Serializable]
public class ItemData
{
	public string _itemName;

	public int _quantity = 1;

	public int _maxQuantity = 1;

	public int _slotNumber;

	[Space]
	public int _modifierID;

	public DamageType _damageTypeOverride;

	public bool _useDamageTypeOverride;

	[Space]
	public bool _isEquipped;

	public bool _isAltWeapon;
}
