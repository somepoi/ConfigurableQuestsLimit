using UnityEngine;

public abstract class ScriptableItem : ScriptableObject
{
	[HideInInspector]
	public ItemType _itemType = ItemType.TRADE;

	[Header("Scriptable Item")]
	public string _itemName = "New Item";

	public Sprite _itemIcon;

	public ItemRarity _itemRarity;

	public SortTag _itemSortTag;

	[TextArea(3, 3)]
	public string _itemDescription = string.Empty;

	[Space]
	public ScriptableQuest _scriptableQuest;

	public string _questTriggerTag = string.Empty;

	public bool _abandonQuestWhenDropped;

	[Space]
	public int _vendorCost = 10;

	public int _maxStackAmount = 1;

	public int _professionExperience;

	public bool _destroyOnDrop;

	private void OnValidate()
	{
		if (_vendorCost < 0)
		{
			_vendorCost = 0;
		}
	}

	public bool IsTransmoggedItem()
	{
		if (Player._mainPlayer._pEquipment._vanitySyncStruct._helmTag == _itemName)
		{
			return true;
		}
		if (Player._mainPlayer._pEquipment._vanitySyncStruct._chestTag == _itemName)
		{
			return true;
		}
		if (Player._mainPlayer._pEquipment._vanitySyncStruct._leggingTag == _itemName)
		{
			return true;
		}
		if (Player._mainPlayer._pEquipment._vanitySyncStruct._capeTag == _itemName)
		{
			return true;
		}
		if (Player._mainPlayer._pEquipment._vanitySyncStruct._shieldTag == _itemName)
		{
			return true;
		}
		return false;
	}
}
