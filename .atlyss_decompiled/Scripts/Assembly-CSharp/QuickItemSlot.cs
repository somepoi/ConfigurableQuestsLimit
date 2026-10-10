using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class QuickItemSlot : MonoBehaviour, IPointerDownHandler, IEventSystemHandler
{
	[Header("Quick Item Slot")]
	public Text _quantityCounter;

	public Text _hotkeyTag;

	public Image _itemIcon;

	public Image _itemCooldownFill;

	public Image _hotKeyGamepadButtonIco;

	public string _setItemName = string.Empty;

	public int _quantityFound;

	private ScriptableItem _setScriptItem;

	public ItemListDataEntry _originParentIDLE;

	private void Update()
	{
		if ((bool)Player._mainPlayer)
		{
			Handle_InputDisplay();
			Iterate_ItemHandleInfo();
		}
	}

	private void Handle_InputDisplay()
	{
		_hotkeyTag.enabled = InputControlManager.current._setGamepadType == GamepadType.Keyboard;
		_hotKeyGamepadButtonIco.enabled = (!TabMenu._current._isOpen && InputControlManager.current._setGamepadType != GamepadType.Keyboard && (GamepadInput.GetButton(GamepadButton.LeftShoulder) || GamepadInput.GetButton(GamepadButton.RightShoulder))) || (TabMenu._current._isOpen && TabMenu._current._selectedMenuCell == TabMenu._current._itemCell && GamepadInput.GetButton(GamepadButton.RightTrigger) && GamepadInput.GetButton(GamepadButton.Button_South));
	}

	public void Set_ItemToSlot(ScriptableItem _scriptItem)
	{
		_setItemName = _scriptItem._itemName;
		_setScriptItem = _scriptItem;
	}

	private void Iterate_ItemHandleInfo()
	{
		if (string.IsNullOrWhiteSpace(_setItemName))
		{
			_itemIcon.enabled = false;
			_quantityCounter.enabled = false;
			_quantityCounter.enabled = false;
			_itemCooldownFill.enabled = false;
			_quantityCounter.text = string.Empty;
			_originParentIDLE = null;
			_setScriptItem = null;
			return;
		}
		for (int i = 0; i < Player._mainPlayer._pInventory._consumableBuffers.Count; i++)
		{
			if (!(Player._mainPlayer._pInventory._consumableBuffers[i]._scriptableConsumable._itemName != _setItemName))
			{
				_itemCooldownFill.enabled = true;
				_itemCooldownFill.fillAmount = Player._mainPlayer._pInventory._consumableBuffers[i]._bufferTimer / Player._mainPlayer._pInventory._consumableBuffers[i]._scriptableConsumable._consumableCooldown;
			}
		}
		_itemIcon.enabled = true;
		_itemIcon.sprite = _setScriptItem._itemIcon;
		_quantityFound = Player._mainPlayer._pInventory.Check_ItemQuantity(_setScriptItem);
		_quantityCounter.enabled = _setScriptItem._itemSortTag != SortTag.TOOL;
		if (_quantityCounter.enabled)
		{
			_quantityCounter.text = $"{_quantityFound}";
		}
		if (_quantityFound <= 0)
		{
			_itemCooldownFill.enabled = false;
		}
		bool flag = false;
		if (_setScriptItem._itemType == ItemType.CONSUMABLE)
		{
			ScriptableConsumable scriptableConsumable = (ScriptableConsumable)_setScriptItem;
			for (int j = 0; j < Player._mainPlayer._pInventory._consumableBuffers.Count; j++)
			{
				if (Player._mainPlayer._pInventory._consumableBuffers[j]._scriptableConsumable == scriptableConsumable)
				{
					flag = true;
					break;
				}
			}
			if (!scriptableConsumable.CanUseConsumable(Player._mainPlayer._statusEntity, _isButtonPrompt: false))
			{
				flag = true;
			}
		}
		if ((_quantityFound <= 0) | flag)
		{
			_itemIcon.color = Color.grey;
		}
		else
		{
			_itemIcon.color = Color.white;
		}
	}

	public void Use_QuickSlotItem()
	{
		if (!Player._mainPlayer || Player._mainPlayer._currentPlayerCondition != PlayerCondition.ACTIVE || Player._mainPlayer._bufferingStatus || Player._mainPlayer._inUI)
		{
			return;
		}
		base.transform.localScale = Vector2.one * 0.5f;
		if (_setItemName == string.Empty)
		{
			ErrorPromptTextManager.current.Init_ErrorPrompt("No Item Assigned");
			return;
		}
		if (_quantityFound <= 0)
		{
			ErrorPromptTextManager.current.Init_ErrorPrompt("Out of Item");
			return;
		}
		for (int i = 0; i < TabMenu._current._itemCell._listDataEntries.Count; i++)
		{
			ItemListDataEntry itemListDataEntry = (ItemListDataEntry)TabMenu._current._itemCell._listDataEntries[i];
			if (_setScriptItem._itemType == ItemType.CONSUMABLE && !(_setItemName != itemListDataEntry._itemData._itemName))
			{
				ScriptableConsumable scriptConsumable = (ScriptableConsumable)_setScriptItem;
				Player._mainPlayer._pInventory.Init_UseConsumable(scriptConsumable, itemListDataEntry._itemData);
				break;
			}
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Right)
		{
			Clear_ItemData();
		}
	}

	public void Clear_ItemData()
	{
		_setItemName = string.Empty;
		ProfileDataManager._current.Save_ProfileData();
	}
}
