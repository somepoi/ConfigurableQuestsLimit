using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "lootTable_", menuName = "ScriptableObjects/Item/Loot Table", order = 5)]
public class ScriptableLootTable : ScriptableObject
{
	public ItemDrop[] _itemDrops = new ItemDrop[0];

	public int _currencyAmount;

	private void OnValidate()
	{
		if (_currencyAmount < 0)
		{
			_currencyAmount = 0;
		}
		for (int i = 0; i < _itemDrops.Length; i++)
		{
			if (_itemDrops[i]._dropQuantity <= 0)
			{
				_itemDrops[i]._dropQuantity = 1;
			}
			if (_itemDrops[i]._dropChance <= 0f)
			{
				_itemDrops[i]._dropChance = 0.001f;
			}
			if (_itemDrops[i]._item == null)
			{
				_itemDrops[i]._itemTag = "Empty Item Drop Slot";
			}
			else
			{
				_itemDrops[i]._itemTag = $"{_itemDrops[i]._item._itemName} ({_itemDrops[i]._dropChance * 100f}%)";
			}
		}
	}

	public ScriptableItem Roll_ForItem(bool _guaranteeItem)
	{
		List<ItemDrop> list = new List<ItemDrop>();
		list.AddRange(_itemDrops);
		GameManager._current.Shuffle_List(list);
		for (int i = 0; i < list.Count; i++)
		{
			if (Random.Range(0f, 1f) < list[i]._dropChance)
			{
				return list[i]._item;
			}
		}
		if (_guaranteeItem)
		{
			return list[Random.Range(0, list.Count)]._item;
		}
		return null;
	}
}
