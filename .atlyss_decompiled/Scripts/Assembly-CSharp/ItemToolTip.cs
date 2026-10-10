using UnityEngine;
using UnityEngine.UI;

public class ItemToolTip : TooltipElement
{
	[Header("Item Tooltip")]
	public ScriptableItem _scriptItem;

	[Space]
	[SerializeField]
	private Text _itemToolTipType;

	[SerializeField]
	private Image _itemIconBackground;

	[SerializeField]
	private Sprite _gambleIco_Consumable;

	[SerializeField]
	private Sprite _gambleIco_Trade;

	[Space]
	[SerializeField]
	private GameObject _consumableUseTab;

	[SerializeField]
	private Text _consumableDescription;

	[Space]
	[SerializeField]
	private GameObject _vendorValueTab;

	[SerializeField]
	private Text _vendorValueCounter;

	public bool _isGambleItem;

	public int _setItemQuantity = 1;

	public int _vendorValue;

	public Image _currencyIcon;

	public ScriptableItem _specialCurrencyItem;

	public override void Update()
	{
		base.Update();
		Apply_ItemStats();
	}

	public override void Disable_ToolTip()
	{
		base.Disable_ToolTip();
		_setItemQuantity = 1;
		_scriptItem = null;
		_specialCurrencyItem = null;
	}

	public void Apply_ItemStats()
	{
		if (!_scriptItem)
		{
			Disable_ToolTip();
			return;
		}
		_vendorValueTab.SetActive(TabMenu._current._itemTradeMode);
		if (TabMenu._current._itemTradeMode)
		{
			if ((bool)_specialCurrencyItem)
			{
				int num = Player._mainPlayer._pInventory.Check_ItemQuantity(_specialCurrencyItem);
				string text = "<color=lime>";
				if (num < _vendorValue)
				{
					text = "<color=#e69138>";
				}
				_vendorValueCounter.text = $"{text}{_specialCurrencyItem._itemName} ({num} / {_vendorValue})</color>";
				_currencyIcon.sprite = _specialCurrencyItem._itemIcon;
			}
			else
			{
				if (_setItemQuantity <= 1)
				{
					_vendorValueCounter.text = $"{_vendorValue} {GameManager._current._statLogics._currencyName}";
				}
				else
				{
					_vendorValueCounter.text = $"{_vendorValue * _setItemQuantity} {GameManager._current._statLogics._currencyName} <color=grey>({_vendorValue} each)</color>";
				}
				_currencyIcon.sprite = GameManager._current._statLogics._currencyIcon;
			}
		}
		if (_isGambleItem)
		{
			_toolTipName.text = "Mystery Item";
			_toolTipSubName.text = "[Unknown]";
			switch (_scriptItem._itemType)
			{
			case ItemType.CONSUMABLE:
				_toolTipIcon.sprite = _gambleIco_Consumable;
				break;
			case ItemType.TRADE:
				_toolTipIcon.sprite = _gambleIco_Trade;
				break;
			}
			_toolTipName.color = Color.white;
			_toolTipSubName.color = Color.white;
			_itemIconBackground.color = new Color(176f, 194f, 74f);
			_consumableUseTab.SetActive(value: false);
			_toolTipDescription.gameObject.SetActive(value: true);
			_toolTipDescription.text = "You can't really see what this is until you buy it.";
			return;
		}
		_toolTipName.text = _scriptItem._itemName ?? "";
		if (string.IsNullOrEmpty(_scriptItem._itemDescription))
		{
			_toolTipDescription.gameObject.SetActive(value: false);
		}
		else
		{
			_toolTipDescription.gameObject.SetActive(value: true);
			_toolTipDescription.text = _scriptItem._itemDescription ?? "";
			if (_scriptItem.GetType() == typeof(ScriptableStatusConsumable))
			{
				ScriptableStatusConsumable scriptableStatusConsumable = (ScriptableStatusConsumable)_scriptItem;
				int healthApply = scriptableStatusConsumable._healthApply;
				int manaApply = scriptableStatusConsumable._manaApply;
				int staminaApply = scriptableStatusConsumable._staminaApply;
				int expGain = scriptableStatusConsumable._expGain;
				_consumableDescription.text = "<color=lime>";
				bool flag = false;
				if (healthApply > 0)
				{
					_consumableDescription.text += $"Recovers {scriptableStatusConsumable._healthApply} Health.";
					flag = true;
				}
				if (manaApply > 0)
				{
					if (flag)
					{
						_consumableDescription.text += "\n";
					}
					_consumableDescription.text += $"Recovers {scriptableStatusConsumable._manaApply} Mana.";
					flag = true;
				}
				if (staminaApply > 0)
				{
					if (flag)
					{
						_consumableDescription.text += "\n";
					}
					_consumableDescription.text += $"Recovers {scriptableStatusConsumable._staminaApply} Stamina.";
					flag = true;
				}
				if (expGain > 0)
				{
					if (flag)
					{
						_consumableDescription.text += "\n";
					}
					_consumableDescription.text += $"Gain {scriptableStatusConsumable._expGain} Experience on use.";
					flag = true;
				}
				if (scriptableStatusConsumable._consumableCondition != null && (bool)scriptableStatusConsumable._consumableCondition._scriptableCondition)
				{
					ConditionSlot consumableCondition = scriptableStatusConsumable._consumableCondition;
					ScriptableCondition scriptableCondition = consumableCondition._scriptableCondition;
					StatStruct parentStatStruct = default;
					if (flag)
					{
						_consumableDescription.text += "\n";
					}
					_consumableDescription.text += scriptableCondition.Generate_ConditionDescriptor(parentStatStruct, DamageType.Pure, consumableCondition._conditionPower, consumableCondition._conditionDuration, consumableCondition._powerPercent, consumableCondition._conditionRepeatRate);
					flag = true;
				}
				_consumableDescription.text += "</color>";
				_consumableUseTab.SetActive(flag);
			}
			else
			{
				_consumableUseTab.SetActive(value: false);
			}
		}
		_toolTipName.color = GameManager._current._statLogics._itemRarityColors[(uint)_scriptItem._itemRarity];
		_toolTipSubName.color = GameManager._current._statLogics._itemRarityColors[(uint)_scriptItem._itemRarity];
		_itemIconBackground.color = GameManager._current._statLogics._itemRarityColors[(uint)_scriptItem._itemRarity];
		_toolTipSubName.text = $"[{_scriptItem._itemRarity}]";
		_toolTipIcon.sprite = _scriptItem._itemIcon;
		switch (_scriptItem._itemType)
		{
		case ItemType.CONSUMABLE:
			_itemToolTipType.text = "Consumable";
			break;
		case ItemType.TRADE:
			_itemToolTipType.text = "Trade Item";
			break;
		}
	}
}
