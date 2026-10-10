using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConditionListDataEntry : MonoBehaviour
{
	[Header("Condition Cell ListData")]
	public ConditionData _conditionData;

	[SerializeField]
	private Image _conditionIcon;

	[SerializeField]
	private Image _conditionIconOverlay;

	[SerializeField]
	private Image _conditionDurationFill;

	[SerializeField]
	private TextMeshProUGUI _conditionDurationCounter;

	[SerializeField]
	private CanvasGroup _canvasGroup;

	private bool _isHoveredOver;

	private void Update()
	{
		Handle_ConditionData();
	}

	private void Handle_ConditionData()
	{
		if (!_conditionData._scriptableCondition)
		{
			return;
		}
		_conditionIcon.enabled = _conditionData._scriptableCondition._conditionIcon;
		if ((bool)_conditionData._scriptableCondition._conditionIcon)
		{
			_conditionIcon.sprite = _conditionData._scriptableCondition._conditionIcon;
		}
		if (_conditionData._setDuration <= 0 && (bool)_conditionDurationCounter)
		{
			_conditionDurationCounter.enabled = false;
			return;
		}
		if ((bool)_conditionDurationCounter)
		{
			_conditionDurationCounter.enabled = true;
			int num = Mathf.FloorToInt(_conditionData._currentDuration / 60f);
			int num2 = Mathf.FloorToInt(_conditionData._currentDuration % 60f);
			if (num <= 0)
			{
				num = 0;
			}
			if (num2 <= 0)
			{
				num2 = 0;
			}
			string text = $"{num}m";
			string text2 = $"{num2}s";
			if (num <= 0)
			{
				_conditionDurationCounter.text = text2;
			}
			else
			{
				_conditionDurationCounter.text = text;
			}
		}
		if ((bool)_conditionDurationFill)
		{
			_conditionDurationFill.fillAmount = _conditionData._currentDuration / (float)_conditionData._setDuration;
		}
		if (_conditionData._currentDuration <= 5f && (float)_conditionData._setDuration > 0f)
		{
			if (!IsInvoking("FlashIcon"))
			{
				InvokeRepeating("FlashIcon", 0f, 1f);
			}
			_canvasGroup.alpha = Mathf.Lerp(_canvasGroup.alpha, 1f, 4.5f * Time.deltaTime);
		}
		else
		{
			CancelInvoke("FlashIcon");
			_canvasGroup.alpha = 1f;
		}
		_conditionIconOverlay.color = _conditionData._scriptableCondition._conditionGroup._floatValueColor;
	}

	private void FlashIcon()
	{
		_canvasGroup.alpha = 0f;
	}

	public void ToolTip_DisplayConditionInfo()
	{
		ToolTipManager._current.Apply_GenericToolTip(string.Empty);
		string text = string.Empty;
		if ((bool)_conditionData._scriptableCondition._conditionGroup)
		{
			text = " (" + _conditionData._scriptableCondition._conditionGroup._conditionGroupTag + ")";
		}
		string text2 = "<b>" + _conditionData._scriptableCondition._conditionName + "</b>" + text;
		StatStruct statStruct = _conditionData._statStruct;
		DamageType attribute = _conditionData._attribute;
		int setConditionPower = _conditionData._setConditionPower;
		int setDuration = _conditionData._setDuration;
		float setConditionPercent = _conditionData._setConditionPercent;
		float setRepeatRate = _conditionData._setRepeatRate;
		text2 = text2 + "\n" + _conditionData._scriptableCondition.Generate_ConditionDescriptor(statStruct, attribute, setConditionPower, setDuration, setConditionPercent, setRepeatRate);
		ToolTipManager._current._genericToolTip.Set_TooltipAnchorPos(new Vector2(0f, 35f));
		ToolTipManager._current.Apply_GenericToolTip(text2);
		ToolTipManager._current._genericToolTip.Enable_ToolTip();
		_isHoveredOver = true;
	}

	private void OnDestroy()
	{
		if (_isHoveredOver)
		{
			ToolTipManager._current._genericToolTip.Disable_ToolTip();
		}
	}

	public void Disable_Tooltip()
	{
		ToolTipManager._current._genericToolTip.Disable_ToolTip();
		_isHoveredOver = false;
	}
}
