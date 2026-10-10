using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopListDataEntry : ListDataEntry, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerDownHandler
{
	[Header("ShopList Data Entry")]
	public Image _itemCellBackdrop;

	public Image _itemIcon;

	public Image _selectionHighlight;

	public Image _priceIcon;

	public Sprite _currencyPriceIconSprite;

	[Space]
	public TextMeshProUGUI _priceText;

	public Text _itemQuantity;

	[Space]
	public Sprite _gambleIcon_gear;

	public Sprite _gambleIcon_consumable;

	public Sprite _gambleIcon_trade;

	public Color _gambleColor_low = Color.white;

	public Color _gambleColor_mid = Color.white;

	public Color _gambleColor_high = Color.white;

	[Space]
	[SerializeField]
	private GameObject _skillIconObject;

	[SerializeField]
	private Image _skillIco;

	[Header("Runtime")]
	public NetNPC _shopkeepNPC;

	public ScriptableItem _scriptableItem;

	public string _keyID = string.Empty;

	public int _gamepadIndex;

	public bool _isbuybackItem;

	private float _commandBuffer;

	[Space]
	public StatModifierCost _specialPriceCost = new StatModifierCost();

	public int _dedicatedValue;

	public bool _useDedicatedValue;

	[Space]
	public bool _isGambleItem;

	public int _gambleValue;

	[Space]
	public ItemData _itemData;

	public ShopkeepItemStruct _shopStruct;

	public void OnPointerEnter(PointerEventData eventData)
	{
		On_HighlightEntry();
	}

	public void On_HighlightEntry()
	{
		_isHighlighted = true;
		bool flag = InputControlManager.current._setGamepadType != GamepadType.Keyboard && InputControlManager.current._gamepadMouseCursorBuffer <= 0f;
		bool flag2 = _scriptableItem._itemType == ItemType.GEAR;
		int vendorValue = (_isGambleItem ? _gambleValue : ((!_useDedicatedValue) ? _scriptableItem._vendorCost : _dedicatedValue));
		if ((bool)_specialPriceCost._scriptItem)
		{
			vendorValue = _specialPriceCost._scriptItemQuantity;
			if (flag2)
			{
				ToolTipManager._current._equipToolTip._specialCurrencyItem = _specialPriceCost._scriptItem;
			}
			else
			{
				ToolTipManager._current._itemToolTip._specialCurrencyItem = _specialPriceCost._scriptItem;
			}
		}
		if (flag2)
		{
			if (flag)
			{
				ToolTipManager._current._equipToolTip._anchorReference = base.transform;
			}
			ToolTipManager._current.Apply_EquipDataToolTip((ScriptableEquipment)_scriptableItem, _isGambleItem);
			ToolTipManager._current._equipToolTip._setItemData = _itemData;
			ToolTipManager._current._equipToolTip._vendorValue = vendorValue;
			ToolTipManager._current._equipToolTip.Set_TooltipAnchorPos(new Vector3(215f, 0f, 0f));
			ToolTipManager._current._equipToolTip.Enable_ToolTip();
			return;
		}
		if (flag)
		{
			ToolTipManager._current._itemToolTip._anchorReference = base.transform;
		}
		ToolTipManager._current.Apply_ItemDataToolTip(_scriptableItem, _isGambleItem);
		ToolTipManager._current._itemToolTip._vendorValue = vendorValue;
		ToolTipManager._current._itemToolTip.Set_TooltipAnchorPos(new Vector3(185f, 0f, 0f));
		ToolTipManager._current._itemToolTip.Enable_ToolTip();
		if (_scriptableItem.GetType() == typeof(ScriptableSkillScroll) && !_isGambleItem)
		{
			ScriptableSkillScroll scriptableSkillScroll = (ScriptableSkillScroll)_scriptableItem;
			float num = 175f;
			ToolTipManager._current._skillToolTip._skillStruct = new SkillStruct
			{
				_skillUnlocked = false,
				_skillName = scriptableSkillScroll._scriptableSkill._skillName
			};
			if (flag)
			{
				ToolTipManager._current._skillToolTip._anchorReference = ToolTipManager._current._itemToolTip.transform;
				ToolTipManager._current._skillToolTip.Set_TooltipAnchorPos(new Vector3(num * 1.85f, 0f, 0f));
			}
			else
			{
				ToolTipManager._current._skillToolTip.Set_TooltipAnchorPos(new Vector3(num * 2.85f, 0f, 0f));
			}
			ToolTipManager._current._skillToolTip._scriptSkill = scriptableSkillScroll._scriptableSkill;
			ToolTipManager._current._skillToolTip.Enable_ToolTip();
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		_isHighlighted = false;
		StopAllCoroutines();
		ToolTipManager._current.Disable_AllTooltips();
		EventSystem.current.SetSelectedGameObject(null);
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Left)
		{
			if (Input.GetKey(KeyCode.LeftShift))
			{
				ShopkeepManager._current.Init_PurchaseItem(_scriptableItem, 1, _keyID, this);
			}
		}
		else
		{
			ToolTipManager._current._itemToolTip.Disable_ToolTip();
			ToolTipManager._current._equipToolTip.Disable_ToolTip();
			ShopkeepManager._current.Init_PurchasePromptWindow(this);
			ShopkeepManager._current.Cancel_SellPrompt();
		}
	}

	private void Update()
	{
		if (!_shopkeepNPC || !_scriptableItem || !ShopkeepManager._current._isOpen)
		{
			return;
		}
		if (_commandBuffer > 0f)
		{
			_commandBuffer -= Time.deltaTime;
		}
		else
		{
			_commandBuffer = 0f;
		}
		if (_isbuybackItem)
		{
			if (!_shopkeepNPC._syncBuybackItems.ContainsKey(_keyID))
			{
				Object.Destroy(base.gameObject);
				return;
			}
			_shopStruct = _shopkeepNPC._syncBuybackItems[_keyID];
		}
		else
		{
			if (!_shopkeepNPC._vendorItems.ContainsKey(_keyID))
			{
				Object.Destroy(base.gameObject);
				return;
			}
			_shopStruct = _shopkeepNPC._vendorItems[_keyID];
		}
		if (_isGambleItem)
		{
			switch (_scriptableItem._itemType)
			{
			case ItemType.GEAR:
				_itemIcon.sprite = _gambleIcon_gear;
				break;
			case ItemType.CONSUMABLE:
				_itemIcon.sprite = _gambleIcon_consumable;
				break;
			case ItemType.TRADE:
				_itemIcon.sprite = _gambleIcon_trade;
				break;
			}
		}
		else
		{
			_itemIcon.sprite = _scriptableItem._itemIcon;
		}
		_selectionHighlight.enabled = ShopkeepManager._current._selectedPurchaseListEntry == this;
		int num = _scriptableItem._vendorCost;
		_useDedicatedValue = _shopStruct._useDedicatedValue;
		if (_useDedicatedValue)
		{
			num = (_dedicatedValue = _shopStruct._dedicatedValue);
		}
		_priceText.text = $"{num}";
		if (num <= 0)
		{
			_priceText.text = "FREE";
		}
		if (_shopStruct._isbuybackItem)
		{
			_itemQuantity.text = $"{_shopStruct._stockQuantity}";
		}
		else
		{
			_itemQuantity.text = string.Empty;
		}
		if ((bool)_specialPriceCost._scriptItem && _specialPriceCost._scriptItemQuantity > 0)
		{
			_priceText.text = $"{_specialPriceCost._scriptItemQuantity}";
			_priceIcon.sprite = _specialPriceCost._scriptItem._itemIcon;
		}
		else
		{
			_priceIcon.sprite = _currencyPriceIconSprite;
		}
		_itemCellBackdrop.color = GameManager._current._statLogics._itemRarityColors[(uint)_scriptableItem._itemRarity];
		if (_itemData._modifierID > 0 && _scriptableItem._itemType == ItemType.GEAR)
		{
			ScriptableEquipment scriptableEquipment = (ScriptableEquipment)_scriptableItem;
			_itemCellBackdrop.color = GameManager._current._statLogics._itemRarityColors[(uint)(scriptableEquipment._itemRarity + 1)];
		}
		if (_isGambleItem)
		{
			if (_gambleValue == 100)
			{
				_itemCellBackdrop.color = _gambleColor_low;
			}
			else if (_gambleValue == 1000)
			{
				_itemCellBackdrop.color = _gambleColor_mid;
			}
			else if (_gambleValue == 10000)
			{
				_itemCellBackdrop.color = _gambleColor_high;
			}
			_priceText.text = $"{_gambleValue}";
		}
		if (_shopStruct._stockQuantity <= 0)
		{
			_itemCellBackdrop.color = Color.red;
		}
		if (_scriptableItem._itemType == ItemType.CONSUMABLE)
		{
			ScriptableConsumable scriptableConsumable = (ScriptableConsumable)_scriptableItem;
			bool flag = scriptableConsumable.GetType() == typeof(ScriptableSkillScroll);
			_skillIconObject.SetActive(!_isGambleItem & flag);
			if (flag)
			{
				ScriptableSkillScroll scriptableSkillScroll = (ScriptableSkillScroll)scriptableConsumable;
				_skillIco.sprite = scriptableSkillScroll._scriptableSkill._skillIcon;
			}
		}
		else
		{
			_skillIconObject.SetActive(value: false);
		}
	}
}
