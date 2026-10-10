using UnityEngine;

[CreateAssetMenu(fileName = "shopKeep_", menuName = "ScriptableObjects/Game Logic/Shopkeep", order = 0)]
public class ScriptableShopkeep : ScriptableObject
{
	[Header("Scriptable Shopkeep")]
	public string _shopName = "New Shop";

	public ShopkeepItemTable[] _shopkeepItemTables;

	public bool _canBuyFromPlayer = true;

	public bool _isGambler;

	private void OnValidate()
	{
		for (int i = 0; i < _shopkeepItemTables.Length; i++)
		{
			if (_shopkeepItemTables[i]._levelRequirement < 1)
			{
				_shopkeepItemTables[i]._levelRequirement = 1;
			}
			string shopkeepTableTag = $"Shopkeep Table #{i} (LV-{_shopkeepItemTables[i]._levelRequirement})";
			_shopkeepItemTables[i]._shopkeepTableTag = shopkeepTableTag;
			for (int j = 0; j < _shopkeepItemTables[i]._shopkeepItems.Length; j++)
			{
				if (_shopkeepItemTables[i]._shopkeepItems[j]._isGambleSlot)
				{
					_shopkeepItemTables[i]._shopkeepItems[j]._scriptItem = null;
					_shopkeepItemTables[i]._shopkeepItems[j]._itemNameTag = $"Gamble Slot (Value: {_shopkeepItemTables[i]._shopkeepItems[j]._gambleValue}, Items: {_shopkeepItemTables[i]._shopkeepItems[j]._gambleLootTable._itemDrops.Length})";
					_isGambler = true;
				}
				else if ((bool)_shopkeepItemTables[i]._shopkeepItems[j]._scriptItem)
				{
					int num = ((!_shopkeepItemTables[i]._shopkeepItems[j]._useDedicatedItemValue) ? _shopkeepItemTables[i]._shopkeepItems[j]._scriptItem._vendorCost : _shopkeepItemTables[i]._shopkeepItems[j]._dedicatedItemValue);
					if ((bool)_shopkeepItemTables[i]._shopkeepItems[j]._specialStoreCost._scriptItem)
					{
						num = _shopkeepItemTables[i]._shopkeepItems[j]._specialStoreCost._scriptItemQuantity;
						_shopkeepItemTables[i]._shopkeepItems[j]._itemNameTag = $"{_shopkeepItemTables[i]._shopkeepItems[j]._scriptItem._itemName} (Value: {num} {_shopkeepItemTables[i]._shopkeepItems[j]._specialStoreCost._scriptItem._itemName}s)";
					}
					else
					{
						_shopkeepItemTables[i]._shopkeepItems[j]._itemNameTag = $"{_shopkeepItemTables[i]._shopkeepItems[j]._scriptItem._itemName} (Value: {num})";
					}
				}
			}
		}
	}
}
