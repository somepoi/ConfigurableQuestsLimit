using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	[Header("Item Slot UI")]
	public Button _slotButton;

	public bool _isStorageSlot;

	public bool[] _filledSlotTab;

	private void Awake()
	{
		_filledSlotTab = new bool[Enum.GetValues(typeof(ItemType)).Length];
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (_isStorageSlot)
		{
			ItemStorageManager._current._currentHighlightedItemSlot = this;
		}
		else
		{
			TabMenu._current._itemCell._currentHighlightedItemSlot = this;
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (_isStorageSlot)
		{
			ItemStorageManager._current._currentHighlightedItemSlot = null;
		}
		else
		{
			TabMenu._current._itemCell._currentHighlightedItemSlot = null;
		}
	}
}
