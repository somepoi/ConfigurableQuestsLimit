using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipUISlot : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerDownHandler
{
	public string _equipSlot_tag = string.Empty;

	public string _equipSlot_altTag = string.Empty;

	public bool _inVanityMode;

	private float _commandBuffer;

	[Space]
	public Image _slot_image;

	public Image _equipSlot_Icon;

	public Image _equipSlot_emptyIcon;

	public Image _equipSlot_rarityOverlay;

	public Image _errorIcon;

	[Space]
	public Button _hideArmorToggleButton;

	public Sprite _hideArmor_visibleIco;

	public Sprite _hideArmor_invisibleIco;

	[Space]
	public ItemListDataEntry _setEntry;

	[SerializeField]
	private Vector3 _promptOffsetPosition = Vector3.zero;

	private void Awake()
	{
		if ((bool)_hideArmorToggleButton)
		{
			_hideArmorToggleButton.onClick.AddListener(PromptCmd_HideArmor);
		}
	}

	private void Update()
	{
		if (!Player._mainPlayer || !TabMenu._current._isOpen || TabMenu._current._selectedMenuCell != TabMenu._current._itemCell)
		{
			return;
		}
		_inVanityMode = TabMenu._current._itemCell._currentEquipCellTab == EquipCellTab.VANITY_PANEL;
		if (_inVanityMode)
		{
			Handle_VanityEquipDisplay();
		}
		else
		{
			Handle_StandardEquipDisplay();
		}
		if (!_setEntry)
		{
			_equipSlot_rarityOverlay.color = Color.clear;
		}
		else if (!_setEntry._itemData._isEquipped)
		{
			_setEntry = null;
		}
		if (_commandBuffer > 0f)
		{
			_commandBuffer -= Time.deltaTime;
		}
		else
		{
			_commandBuffer = 0f;
		}
		if (!_inVanityMode)
		{
			switch (_equipSlot_tag)
			{
			case "Helm":
				if (Player._mainPlayer._pVisual._playerAppearanceStruct._hideHelm)
				{
					_hideArmorToggleButton.image.sprite = _hideArmor_invisibleIco;
				}
				else
				{
					_hideArmorToggleButton.image.sprite = _hideArmor_visibleIco;
				}
				break;
			case "Cape":
				if (Player._mainPlayer._pVisual._playerAppearanceStruct._hideCape)
				{
					_hideArmorToggleButton.image.sprite = _hideArmor_invisibleIco;
				}
				else
				{
					_hideArmorToggleButton.image.sprite = _hideArmor_visibleIco;
				}
				break;
			case "Chest":
				if (Player._mainPlayer._pVisual._playerAppearanceStruct._hideChest)
				{
					_hideArmorToggleButton.image.sprite = _hideArmor_invisibleIco;
				}
				else
				{
					_hideArmorToggleButton.image.sprite = _hideArmor_visibleIco;
				}
				break;
			case "Leggings":
				if (Player._mainPlayer._pVisual._playerAppearanceStruct._hideLeggings)
				{
					_hideArmorToggleButton.image.sprite = _hideArmor_invisibleIco;
				}
				else
				{
					_hideArmorToggleButton.image.sprite = _hideArmor_visibleIco;
				}
				break;
			}
			return;
		}
		switch (_equipSlot_tag)
		{
		case "Helm":
			if (Player._mainPlayer._pVisual._playerAppearanceStruct._hideVanityHelm)
			{
				_hideArmorToggleButton.image.sprite = _hideArmor_invisibleIco;
			}
			else
			{
				_hideArmorToggleButton.image.sprite = _hideArmor_visibleIco;
			}
			break;
		case "Cape":
			if (Player._mainPlayer._pVisual._playerAppearanceStruct._hideVanityCape)
			{
				_hideArmorToggleButton.image.sprite = _hideArmor_invisibleIco;
			}
			else
			{
				_hideArmorToggleButton.image.sprite = _hideArmor_visibleIco;
			}
			break;
		case "Chest":
			if (Player._mainPlayer._pVisual._playerAppearanceStruct._hideVanityChest)
			{
				_hideArmorToggleButton.image.sprite = _hideArmor_invisibleIco;
			}
			else
			{
				_hideArmorToggleButton.image.sprite = _hideArmor_visibleIco;
			}
			break;
		case "Leggings":
			if (Player._mainPlayer._pVisual._playerAppearanceStruct._hideVanityLeggings)
			{
				_hideArmorToggleButton.image.sprite = _hideArmor_invisibleIco;
			}
			else
			{
				_hideArmorToggleButton.image.sprite = _hideArmor_visibleIco;
			}
			break;
		}
	}

	private void Handle_StandardEquipDisplay()
	{
		_slot_image.color = Color.white;
		_equipSlot_Icon.color = Color.white;
		ItemListDataEntry itemListDataEntry = null;
		if ((bool)TabMenu._current._itemCell._heldItemListDataEntry)
		{
			itemListDataEntry = TabMenu._current._itemCell._heldItemListDataEntry;
		}
		if ((bool)itemListDataEntry)
		{
			ScriptableItem scriptableItem = itemListDataEntry._scriptableItem;
			if (scriptableItem._itemType == ItemType.GEAR)
			{
				if (scriptableItem.GetType() == typeof(ScriptableHelm))
				{
					if (_equipSlot_tag == "Helm")
					{
						_slot_image.color = Color.white;
						_equipSlot_Icon.color = Color.white;
					}
					else
					{
						_slot_image.color = Color.grey;
						_equipSlot_Icon.color = Color.grey;
					}
				}
				else if (scriptableItem.GetType() == typeof(ScriptableChestpiece))
				{
					if (_equipSlot_tag == "Chest")
					{
						_slot_image.color = Color.white;
						_equipSlot_Icon.color = Color.white;
					}
					else
					{
						_slot_image.color = Color.grey;
						_equipSlot_Icon.color = Color.grey;
					}
				}
				else if (scriptableItem.GetType() == typeof(ScriptableLeggings))
				{
					if (_equipSlot_tag == "Leggings")
					{
						_slot_image.color = Color.white;
						_equipSlot_Icon.color = Color.white;
					}
					else
					{
						_slot_image.color = Color.grey;
						_equipSlot_Icon.color = Color.grey;
					}
				}
				else if (scriptableItem.GetType() == typeof(ScriptableCape))
				{
					if (_equipSlot_tag == "Cape")
					{
						_slot_image.color = Color.white;
						_equipSlot_Icon.color = Color.white;
					}
					else
					{
						_slot_image.color = Color.grey;
						_equipSlot_Icon.color = Color.grey;
					}
				}
				else if (scriptableItem.GetType() == typeof(ScriptableRing))
				{
					if (_equipSlot_tag == "Ring")
					{
						_slot_image.color = Color.white;
						_equipSlot_Icon.color = Color.white;
					}
					else
					{
						_slot_image.color = Color.grey;
						_equipSlot_Icon.color = Color.grey;
					}
				}
				else if (scriptableItem.GetType() == typeof(ScriptableShield))
				{
					if (_equipSlot_tag == "Shield")
					{
						_slot_image.color = Color.white;
						_equipSlot_Icon.color = Color.white;
					}
					else
					{
						_slot_image.color = Color.grey;
						_equipSlot_Icon.color = Color.grey;
					}
				}
				else if (scriptableItem.GetType() == typeof(ScriptableWeapon))
				{
					if (_equipSlot_tag == "Weapon" || _equipSlot_tag == "Alt Weapon")
					{
						_slot_image.color = Color.white;
						_equipSlot_Icon.color = Color.white;
					}
					else
					{
						_slot_image.color = Color.grey;
						_equipSlot_Icon.color = Color.grey;
					}
				}
			}
		}
		else
		{
			_slot_image.color = Color.white;
			_equipSlot_Icon.color = Color.white;
		}
		if (_equipSlot_tag != "Shield")
		{
			_errorIcon.enabled = false;
		}
		if ((bool)_setEntry && (bool)_setEntry._scriptableItem)
		{
			_equipSlot_rarityOverlay.color = GameManager._current._statLogics._itemRarityColors[(uint)_setEntry._scriptableItem._itemRarity];
			if (_setEntry._itemData._modifierID > 0 && _setEntry._itemData._isEquipped)
			{
				ScriptableEquipment scriptableEquipment = (ScriptableEquipment)_setEntry._scriptableItem;
				_equipSlot_rarityOverlay.color = GameManager._current._statLogics._itemRarityColors[(uint)(scriptableEquipment._itemRarity + 1)];
			}
		}
	}

	private void Handle_VanityEquipDisplay()
	{
		if (!Player._mainPlayer._pEquipment || !Player._mainPlayer._pEquipment._equips.ContainsKey(_equipSlot_tag))
		{
			return;
		}
		_slot_image.color = Color.magenta;
		_equipSlot_Icon.color = Color.white;
		ItemListDataEntry itemListDataEntry = null;
		if ((bool)TabMenu._current._itemCell._heldItemListDataEntry)
		{
			itemListDataEntry = TabMenu._current._itemCell._heldItemListDataEntry;
		}
		if ((bool)itemListDataEntry)
		{
			ScriptableItem scriptableItem = itemListDataEntry._scriptableItem;
			if (scriptableItem._itemType == ItemType.GEAR)
			{
				if (scriptableItem.GetType() == typeof(ScriptableHelm))
				{
					if (_equipSlot_tag == "Helm")
					{
						_slot_image.color = Color.magenta;
						_equipSlot_Icon.color = Color.white;
					}
					else
					{
						_slot_image.color = Color.grey;
						_equipSlot_Icon.color = Color.grey;
					}
				}
				else if (scriptableItem.GetType() == typeof(ScriptableChestpiece))
				{
					if (_equipSlot_tag == "Chest")
					{
						_slot_image.color = Color.magenta;
						_equipSlot_Icon.color = Color.white;
					}
					else
					{
						_slot_image.color = Color.grey;
						_equipSlot_Icon.color = Color.grey;
					}
				}
				else if (scriptableItem.GetType() == typeof(ScriptableLeggings))
				{
					if (_equipSlot_tag == "Leggings")
					{
						_slot_image.color = Color.magenta;
						_equipSlot_Icon.color = Color.white;
					}
					else
					{
						_slot_image.color = Color.grey;
						_equipSlot_Icon.color = Color.grey;
					}
				}
				else if (scriptableItem.GetType() == typeof(ScriptableCape))
				{
					if (_equipSlot_tag == "Cape")
					{
						_slot_image.color = Color.magenta;
						_equipSlot_Icon.color = Color.white;
					}
					else
					{
						_slot_image.color = Color.grey;
						_equipSlot_Icon.color = Color.grey;
					}
				}
				else if (scriptableItem.GetType() == typeof(ScriptableRing))
				{
					if (_equipSlot_tag == "Ring")
					{
						_slot_image.color = Color.magenta;
						_equipSlot_Icon.color = Color.white;
					}
					else
					{
						_slot_image.color = Color.grey;
						_equipSlot_Icon.color = Color.grey;
					}
				}
				else if (scriptableItem.GetType() == typeof(ScriptableShield))
				{
					if (_equipSlot_tag == "Shield")
					{
						_slot_image.color = Color.magenta;
						_equipSlot_Icon.color = Color.white;
					}
					else
					{
						_slot_image.color = Color.grey;
						_equipSlot_Icon.color = Color.grey;
					}
				}
				else if (scriptableItem.GetType() == typeof(ScriptableWeapon))
				{
					if (_equipSlot_tag == "Weapon" || _equipSlot_tag == "Alt Weapon")
					{
						_slot_image.color = Color.magenta;
						_equipSlot_Icon.color = Color.white;
					}
					else
					{
						_slot_image.color = Color.grey;
						_equipSlot_Icon.color = Color.grey;
					}
				}
			}
			else
			{
				_slot_image.color = Color.magenta;
				_equipSlot_Icon.color = Color.white;
			}
		}
		if (!Player._mainPlayer._pEquipment._equips[_equipSlot_tag]._vanityEquip)
		{
			_equipSlot_rarityOverlay.color = Color.clear;
		}
		else
		{
			_equipSlot_rarityOverlay.color = GameManager._current._statLogics._itemRarityColors[(uint)Player._mainPlayer._pEquipment._equips[_equipSlot_tag]._vanityEquip._itemRarity];
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		switch (eventData.button)
		{
		case PointerEventData.InputButton.Left:
			On_LeftClick();
			break;
		case PointerEventData.InputButton.Right:
			On_RightClick();
			break;
		}
		void On_LeftClick()
		{
			if ((bool)Player._mainPlayer._pEquipment._equips[_equipSlot_tag]._scriptableEquip && (bool)_setEntry && Input.GetKey(KeyCode.LeftShift))
			{
				if (_inVanityMode)
				{
					Init_RemoveVanity();
				}
				else
				{
					Player._mainPlayer._pEquipment.Init_UnEquipItem(Player._mainPlayer._pEquipment._equips[_equipSlot_tag]._heldItem);
					ToolTipManager._current.Disable_AllTooltips();
					TabMenu._current._itemCell._currentHighlightedDataEntry = null;
					TabMenu._current._itemCell.Init_InventoryTab(1);
					TabMenu._current._itemCell.Init_InventoryTab(0);
				}
			}
		}
		void On_RightClick()
		{
			if (_inVanityMode)
			{
				Init_RemoveVanity();
			}
			else if ((bool)_setEntry)
			{
				TabMenu._current._itemCell.Init_ItemPromptWindow(_setEntry);
				Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z) + _promptOffsetPosition;
				TabMenu._current._itemCell._itemPromptWindowElement.transform.position = position;
				ToolTipManager._current.Disable_AllTooltips();
			}
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if ((bool)Player._mainPlayer._pEquipment)
		{
			Init_EquipSlotTooltip();
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		ToolTipManager._current._genericToolTip.Disable_ToolTip();
		ToolTipManager._current._equipToolTip.Disable_ToolTip();
	}

	public bool HasVanity()
	{
		if ((bool)Player._mainPlayer._pEquipment._equips[_equipSlot_tag]._vanityEquip)
		{
			return true;
		}
		return false;
	}

	public void Init_RemoveVanity()
	{
		if (HasVanity())
		{
			Player._mainPlayer._pEquipment.Cmd_RemoveVanity(_equipSlot_tag);
			Player._mainPlayer._pEquipment._equips[_equipSlot_tag]._vanityEquip = null;
			TabMenu._current._itemCell.Refresh_EquipCells();
			ToolTipManager._current.Disable_AllTooltips();
		}
	}

	public void Init_EquipOnEntryDrop(ItemListDataEntry _ILDE)
	{
		if (!_ILDE._scriptableItem)
		{
			return;
		}
		if (_ILDE._scriptableItem._itemType != ItemType.GEAR)
		{
			_ILDE.Relocate_ToOriginSlot();
			return;
		}
		if (_ILDE._itemData._isEquipped)
		{
			_ILDE.Relocate_ToOriginSlot();
			return;
		}
		bool _isHelmItem = _ILDE._scriptableItem.GetType() == typeof(ScriptableHelm);
		bool _isChestItem = _ILDE._scriptableItem.GetType() == typeof(ScriptableChestpiece);
		bool _isLeggingItem = _ILDE._scriptableItem.GetType() == typeof(ScriptableLeggings);
		bool _isCapeItem = _ILDE._scriptableItem.GetType() == typeof(ScriptableCape);
		bool _isRingItem = _ILDE._scriptableItem.GetType() == typeof(ScriptableRing);
		bool flag = _ILDE._scriptableItem.GetType() == typeof(ScriptableWeapon);
		bool _isShieldItem = _ILDE._scriptableItem.GetType() == typeof(ScriptableShield);
		_ILDE._setEquipOriginSlotNum = TabMenu._current._itemCell._heldItemListDataEntry._originSlotNumber;
		_ILDE.Relocate_ToOriginSlot();
		if (IsApplicableArmorItem())
		{
			if (_inVanityMode)
			{
				TabMenu._current._itemCell.PromptCmd_TransmogEquip(_ILDE);
			}
			else
			{
				TabMenu._current._itemCell.PromptCmd_EquipItem(_ILDE);
			}
		}
		else if (flag)
		{
			TabMenu._current._itemCell.PromptCmd_EquipItem(_ILDE);
		}
		bool IsApplicableArmorItem()
		{
			if (_isHelmItem && _equipSlot_tag == "Helm")
			{
				return true;
			}
			if (_isChestItem && _equipSlot_tag == "Chest")
			{
				return true;
			}
			if (_isLeggingItem && _equipSlot_tag == "Leggings")
			{
				return true;
			}
			if (_isCapeItem && _equipSlot_tag == "Cape")
			{
				return true;
			}
			if (_isShieldItem && _equipSlot_tag == "Shield")
			{
				return true;
			}
			if (_isRingItem && _equipSlot_tag == "Ring")
			{
				return true;
			}
			return false;
		}
	}

	public void Init_EquipSlotTooltip()
	{
		bool flag = InputControlManager.current._setGamepadType != GamepadType.Keyboard;
		string text = _equipSlot_tag;
		if (!string.IsNullOrWhiteSpace(_equipSlot_altTag))
		{
			text = _equipSlot_altTag;
		}
		if (flag)
		{
			ToolTipManager._current._genericToolTip._anchorReference = base.transform;
			ToolTipManager._current._equipToolTip._anchorReference = base.transform;
		}
		if (!_inVanityMode)
		{
			if (!_setEntry || !_setEntry._scriptableItem)
			{
				ToolTipManager._current.Apply_GenericToolTip(text ?? "");
				if (flag)
				{
					ToolTipManager._current._genericToolTip._anchorReference = base.transform;
				}
				ToolTipManager._current._genericToolTip.Set_TooltipAnchorPos(new Vector3(-55f, 35f, 0f));
				ToolTipManager._current._genericToolTip.Enable_ToolTip();
			}
			else
			{
				ToolTipManager._current.Apply_EquipDataToolTip((ScriptableEquipment)_setEntry._scriptableItem, _isGambleItem: false);
				ToolTipManager._current._equipToolTip._setItemData = _setEntry._itemData;
				ToolTipManager._current._equipToolTip.Set_TooltipAnchorPos(new Vector3(-215f, 0f, 0f));
				ToolTipManager._current._equipToolTip.Enable_ToolTip();
			}
		}
		else if (!Player._mainPlayer._pEquipment._equips[_equipSlot_tag]._vanityEquip)
		{
			ToolTipManager._current.Apply_GenericToolTip(text ?? "");
			ToolTipManager._current._genericToolTip.Set_TooltipAnchorPos(new Vector3(-55f, 35f, 0f));
			ToolTipManager._current._genericToolTip.Enable_ToolTip();
		}
		else
		{
			ToolTipManager._current.Apply_GenericToolTip("Illusion: " + Player._mainPlayer._pEquipment._equips[_equipSlot_tag]._vanityEquip._itemName);
			ToolTipManager._current._genericToolTip.Set_TooltipAnchorPos(new Vector3(-55f, 35f, 0f));
			ToolTipManager._current._genericToolTip.Enable_ToolTip();
		}
	}

	private bool Check_IfCanHideArmorRender()
	{
		if (Player._mainPlayer._currentPlayerCondition != PlayerCondition.ACTIVE)
		{
			return false;
		}
		if (Player._mainPlayer._currentPlayerAction != PlayerAction.IDLE)
		{
			return false;
		}
		if (Player._mainPlayer._bufferingStatus)
		{
			return false;
		}
		if (_commandBuffer > 0f)
		{
			return false;
		}
		Player._mainPlayer._pSound._aSrcGeneral.PlayOneShot(Player._mainPlayer._pSound._equipSound, 0.65f);
		return true;
	}

	public void PromptCmd_HideArmor()
	{
		if (Check_IfCanHideArmorRender())
		{
			switch (_equipSlot_tag)
			{
			case "Helm":
				Player._mainPlayer._pVisual.Cmd_ToggleArmorRender(0, TabMenu._current._itemCell._currentEquipCellTab);
				break;
			case "Cape":
				Player._mainPlayer._pVisual.Cmd_ToggleArmorRender(1, TabMenu._current._itemCell._currentEquipCellTab);
				break;
			case "Chest":
				Player._mainPlayer._pVisual.Cmd_ToggleArmorRender(2, TabMenu._current._itemCell._currentEquipCellTab);
				break;
			case "Leggings":
				Player._mainPlayer._pVisual.Cmd_ToggleArmorRender(3, TabMenu._current._itemCell._currentEquipCellTab);
				break;
			}
			_commandBuffer = 0.8f;
		}
	}
}
