using UnityEngine;

public class ToolTipManager : MonoBehaviour
{
	public static ToolTipManager _current;

	[Header("Tooltip Management")]
	public TooltipElement _genericToolTip;

	public SkillToolTip _skillToolTip;

	public ItemToolTip _itemToolTip;

	public EquipToolTip _equipToolTip;

	private void Awake()
	{
		if (_current == null)
		{
			_current = this;
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void Update()
	{
		if ((bool)Player._mainPlayer)
		{
			if ((bool)TabMenu._current._itemCell._heldItemListDataEntry)
			{
				Disable_AllTooltips();
			}
			if (TabMenu._current._skillsCell._isSkillDragging)
			{
				Disable_AllTooltips();
			}
			if (!Application.isFocused)
			{
				Disable_AllTooltips();
			}
			if (Cursor.lockState == CursorLockMode.Locked && InputControlManager.current._setGamepadType == GamepadType.Keyboard)
			{
				_genericToolTip._tooltipGroup.alpha = 0f;
				_skillToolTip._tooltipGroup.alpha = 0f;
				_itemToolTip._tooltipGroup.alpha = 0f;
				_equipToolTip._tooltipGroup.alpha = 0f;
			}
			if (InputControlManager.current._setGamepadType != GamepadType.Keyboard && !TabMenu._current._isOpen)
			{
				Disable_AllTooltips();
			}
		}
		else if (Cursor.lockState == CursorLockMode.Locked)
		{
			_genericToolTip._tooltipGroup.alpha = 0f;
			_skillToolTip._tooltipGroup.alpha = 0f;
			_itemToolTip._tooltipGroup.alpha = 0f;
			_equipToolTip._tooltipGroup.alpha = 0f;
		}
	}

	public void Disable_AllTooltips()
	{
		_genericToolTip.Disable_ToolTip();
		_skillToolTip.Disable_ToolTip();
		_itemToolTip.Disable_ToolTip();
		_equipToolTip.Disable_ToolTip();
	}

	public void Apply_GenericToolTip(string _message)
	{
		if (!Player._mainPlayer || !Player._mainPlayer._bufferingStatus)
		{
			_genericToolTip._toolTipDescription.text = _message ?? "";
		}
	}

	public void Apply_SkillDataToolTip(ScriptableSkill _scriptSkill, SkillStruct _skillStruct)
	{
		if (!Player._mainPlayer || !Player._mainPlayer._bufferingStatus)
		{
			_skillToolTip._scriptSkill = _scriptSkill;
			_skillToolTip._skillStruct = _skillStruct;
		}
	}

	public void Apply_ItemDataToolTip(ScriptableItem _scriptItem, bool _isGambleItem)
	{
		if (!Player._mainPlayer || !Player._mainPlayer._bufferingStatus)
		{
			_itemToolTip._scriptItem = _scriptItem;
			_itemToolTip._isGambleItem = _isGambleItem;
		}
	}

	public void Apply_EquipDataToolTip(ScriptableEquipment _scriptEquip, bool _isGambleItem)
	{
		if (!Player._mainPlayer || !Player._mainPlayer._bufferingStatus)
		{
			_equipToolTip._setScriptEquip = _scriptEquip;
			_equipToolTip._isGambleItem = _isGambleItem;
		}
	}
}
