using UnityEngine;
using UnityEngine.UI;

public class AttributeListDataEntry : ListDataEntry
{
	private GameManager _gm;

	private PlayerStats _pStats;

	[Header("Stats Cell ListData")]
	[SerializeField]
	private Image _attributeIcon;

	[SerializeField]
	private Text _attributeValueText;

	[SerializeField]
	private GameObject _gamepadSelectionCursor;

	[SerializeField]
	private Image _gamepadLeftTriggerIco;

	[SerializeField]
	private Image _gamepadRightTriggerIco;

	public Button _attributeButton_singlePoint;

	public Button _attributeButton_removePoint;

	public Animator _attributeAnimator;

	public bool _isApplyingAttributePoints;

	public int _heldAttributeValue;

	public int _pointsPutIn;

	[SerializeField]
	private Vector2 _tooltipAnchorPos = Vector2.zero;

	private ScriptableStatAttribute _scriptableAttribute;

	private float _inputBuffer;

	private bool _heldUpPointButton;

	private bool _heldDownPointButton;

	[Header("Audio")]
	[SerializeField]
	private AudioSource _aSrc_upAttribute;

	[SerializeField]
	private AudioSource _aSrc_downAttribute;

	private void Update()
	{
		Handle_AttributeData();
	}

	public void Hold_UpPointButton(bool _isTrue)
	{
		_heldUpPointButton = _isTrue;
	}

	public void Hold_DownPointButton(bool _isTrue)
	{
		_heldDownPointButton = _isTrue;
	}

	private void Handle_AttributeData()
	{
		if (!GameManager._current || !Player._mainPlayer)
		{
			return;
		}
		if (_inputBuffer > 0f)
		{
			_inputBuffer -= Time.deltaTime;
		}
		if (_pStats == null)
		{
			_pStats = Player._mainPlayer._pStats;
		}
		if (_pStats._playerAttributes[_dataID]._attributeName == string.Empty)
		{
			return;
		}
		_gm = GameManager._current;
		_dataNameText.text = _gm._statLogics._statAttributes[_dataID]._attributeName;
		_attributeIcon.sprite = _gm._statLogics._statAttributes[_dataID]._attributeIcon;
		if (!_isApplyingAttributePoints)
		{
			_attributeButton_singlePoint.interactable = TabMenu._current._statsCell._phantomAtbPoints >= 1;
			_attributeButton_removePoint.interactable = false;
			_attributeValueText.text = $"{_pStats._playerAttributes[_dataID]._attributeValue}";
			_heldAttributeValue = _pStats._playerAttributes[_dataID]._attributeValue;
			_attributeValueText.color = Color.white;
			_pointsPutIn = 0;
		}
		else
		{
			_attributeValueText.text = $"{_heldAttributeValue}";
			if (_pointsPutIn > 0)
			{
				_attributeValueText.color = Color.green;
			}
			_attributeButton_singlePoint.interactable = TabMenu._current._statsCell._phantomAtbPoints > 0;
			_attributeButton_removePoint.interactable = _pointsPutIn > 0;
		}
		if (_heldUpPointButton)
		{
			Init_ApplyAttributePoint();
		}
		if (_heldDownPointButton)
		{
			Init_RemoveAttributePoint();
		}
		_gamepadSelectionCursor.SetActive(InputControlManager.current._setGamepadType != GamepadType.Keyboard && _isHighlighted && TabMenu._current._isOpen);
		_gamepadLeftTriggerIco.enabled = InputControlManager.current._setGamepadType != GamepadType.Keyboard && _isHighlighted && _pStats._currentAttributePoints > 0;
		_gamepadRightTriggerIco.enabled = InputControlManager.current._setGamepadType != GamepadType.Keyboard && _isHighlighted && _pStats._currentAttributePoints > 0;
		if (GamepadInput.GetButton(GamepadButton.LeftTrigger))
		{
			_gamepadLeftTriggerIco.color = Color.white;
		}
		else
		{
			_gamepadLeftTriggerIco.color = Color.grey;
		}
		if (GamepadInput.GetButton(GamepadButton.RightTrigger))
		{
			_gamepadRightTriggerIco.color = Color.white;
		}
		else
		{
			_gamepadRightTriggerIco.color = Color.grey;
		}
		if (InputControlManager.current._setGamepadType != GamepadType.Keyboard && TabMenu._current.CurrentCellSelection == 1)
		{
			if (!_isHighlighted)
			{
				_heldUpPointButton = false;
				_heldDownPointButton = false;
			}
			else if (TabMenu._current._isOpen)
			{
				Hold_UpPointButton(GamepadInput.GetButton(GamepadButton.RightTrigger));
				Hold_DownPointButton(GamepadInput.GetButton(GamepadButton.LeftTrigger));
			}
		}
	}

	public void Init_ApplyAttributePoint()
	{
		if (_pStats._currentAttributePoints > 0 && !(_inputBuffer > 0f) && TabMenu._current._statsCell._phantomAtbPoints > 0)
		{
			_isApplyingAttributePoints = true;
			_heldAttributeValue++;
			_pointsPutIn++;
			TabMenu._current._statsCell._phantomAtbPoints--;
			TabMenu._current._statsCell.Project_PhantomAttributeStats();
			_aSrc_upAttribute.Stop();
			_aSrc_upAttribute.Play();
			_inputBuffer = 0.16f;
		}
	}

	public void Init_RemoveAttributePoint()
	{
		if (!_isApplyingAttributePoints || _pStats._currentAttributePoints <= 0)
		{
			return;
		}
		if (_pointsPutIn <= 0)
		{
			_isApplyingAttributePoints = false;
		}
		else if (!(_inputBuffer > 0f))
		{
			_heldAttributeValue--;
			_pointsPutIn--;
			TabMenu._current._statsCell._phantomAtbPoints++;
			TabMenu._current._statsCell.Project_PhantomAttributeStats();
			if (_pointsPutIn == 0)
			{
				_isApplyingAttributePoints = false;
			}
			_aSrc_downAttribute.Stop();
			_aSrc_downAttribute.Play();
			_inputBuffer = 0.16f;
		}
	}

	public void Init_TooltipInfo()
	{
		for (int i = 0; i < GameManager._current._statLogics._statAttributes.Length; i++)
		{
			if (_pStats._playerAttributes[_dataID]._attributeName == GameManager._current._statLogics._statAttributes[i]._attributeName)
			{
				_scriptableAttribute = GameManager._current._statLogics._statAttributes[i];
				break;
			}
		}
		ToolTipManager._current.Apply_GenericToolTip(_scriptableAttribute._attributeDescriptor ?? "");
		ToolTipManager._current._genericToolTip.Set_TooltipAnchorPos(_tooltipAnchorPos);
		ToolTipManager._current._genericToolTip.Enable_ToolTip();
	}

	public void Disable_TooltipInfo()
	{
		ToolTipManager._current._genericToolTip.Disable_ToolTip();
	}
}
