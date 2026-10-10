using System.Collections;
using Steamworks;
using UnityEngine;
using UnityEngine.UI;

public class SettingsCell : MonoBehaviour
{
	[Header("Settings Gamepad Select Cell")]
	public SettingsMenuSelection _cellTab;

	public Vector2 _menuPosition = Vector2.zero;

	public Image[] _gamepadNavIcons = new Image[0];

	public bool _isSelected;

	[Header("Standard References")]
	public Toggle _setToggle;

	public Dropdown _setDropdown;

	public InputField _setInputField;

	[Space]
	public Slider _setSlider;

	public float _sliderDragModifier = 1f;

	public float _resetValue;

	public bool _enableResetValue = true;

	public bool _onButtonDownSlider;

	[Header("Buttons")]
	public Button _southButtonOption_button;

	public Button _leftOptionButton;

	public Button _rightOptionButton;

	[Header("Character Creation Slider Refs")]
	public bool _isChestReaction;

	public bool _isBottomReaction;

	public bool _isArmReaction;

	public bool _isVoicePitch;

	[Header("Runtime")]
	private bool _isInputLeft;

	private bool _isInputRight;

	private bool _isInputLeftDown;

	private bool _isInputRightDown;

	private bool _isResettingReaction;

	private void Update()
	{
		Handle_IfSelected();
		if (_gamepadNavIcons != null && _gamepadNavIcons.Length != 0)
		{
			for (int i = 0; i < _gamepadNavIcons.Length; i++)
			{
				_gamepadNavIcons[i].enabled = _isSelected && InputControlManager.current._setGamepadType != GamepadType.Keyboard;
			}
		}
	}

	private void Handle_IfSelected()
	{
		if (InputControlManager.current._setGamepadType == GamepadType.Keyboard)
		{
			_isSelected = false;
		}
		else if (!_isSelected)
		{
			if ((bool)_setInputField)
			{
				_setInputField.DeactivateInputField();
			}
		}
		else
		{
			if (SettingsManager._current._isOpen && _cellTab != SettingsManager._current._currentSettingsMenuSelection)
			{
				return;
			}
			_isInputLeft = GamepadInput.GetButton(GamepadButton.RightStick_Left) || InputControlManager.current.DpadLeft(DpadHoldType.OnHold);
			_isInputRight = GamepadInput.GetButton(GamepadButton.RightStick_Right) || InputControlManager.current.DpadRight(DpadHoldType.OnHold);
			_isInputLeftDown = GamepadInput.GetButtonDown(GamepadButton.RightStick_Left) || InputControlManager.current.DpadLeft(DpadHoldType.OnDown);
			_isInputRightDown = GamepadInput.GetButtonDown(GamepadButton.RightStick_Right) || InputControlManager.current.DpadRight(DpadHoldType.OnDown);
			if (!SettingsManager._current._isOpen)
			{
				_isInputLeft = GamepadInput.GetButton(GamepadButton.LeftStick_Left) || InputControlManager.current.DpadLeft(DpadHoldType.OnHold);
				_isInputRight = GamepadInput.GetButton(GamepadButton.LeftStick_Right) || InputControlManager.current.DpadRight(DpadHoldType.OnHold);
				_isInputLeftDown = GamepadInput.GetButtonDown(GamepadButton.LeftStick_Left) || InputControlManager.current.DpadLeft(DpadHoldType.OnDown);
				_isInputRightDown = GamepadInput.GetButtonDown(GamepadButton.LeftStick_Right) || InputControlManager.current.DpadRight(DpadHoldType.OnDown);
			}
			if (!Player._mainPlayer)
			{
				_isInputLeft = GamepadInput.GetButton(GamepadButton.LeftStick_Left) || GamepadInput.GetButton(GamepadButton.RightStick_Left) || InputControlManager.current.DpadLeft(DpadHoldType.OnHold);
				_isInputRight = GamepadInput.GetButton(GamepadButton.LeftStick_Right) || GamepadInput.GetButton(GamepadButton.RightStick_Right) || InputControlManager.current.DpadRight(DpadHoldType.OnHold);
				_isInputLeftDown = GamepadInput.GetButtonDown(GamepadButton.LeftStick_Left) || GamepadInput.GetButtonDown(GamepadButton.RightStick_Left) || InputControlManager.current.DpadLeft(DpadHoldType.OnDown);
				_isInputRightDown = GamepadInput.GetButtonDown(GamepadButton.LeftStick_Right) || GamepadInput.GetButtonDown(GamepadButton.RightStick_Right) || InputControlManager.current.DpadRight(DpadHoldType.OnDown);
			}
			if (GamepadInput.GetButtonDown(GamepadButton.Button_South) && MainMenuManager._current._menuInputBuffer <= 0f)
			{
				if ((bool)_setToggle)
				{
					_setToggle.isOn = !_setToggle.isOn;
				}
				if ((bool)_setInputField)
				{
					_setInputField.ActivateInputField();
					if (SteamManager.Initialized && (SteamUtils.IsSteamRunningOnSteamDeck() || SteamUtils.IsSteamInBigPictureMode()))
					{
						SteamUtils.ShowFloatingGamepadTextInput(EFloatingGamepadTextInputMode.k_EFloatingGamepadTextInputModeModeSingleLine, 0, 0, 500, 200);
					}
				}
				if ((bool)_southButtonOption_button && _southButtonOption_button.interactable)
				{
					_southButtonOption_button.onClick.Invoke();
					if ((bool)_southButtonOption_button.GetComponent<ButtonSound>())
					{
						_southButtonOption_button.GetComponent<ButtonSound>().Play_ClickSound();
					}
				}
			}
			if (GamepadInput.GetButtonDown(GamepadButton.RightStick) && (bool)_setSlider && _enableResetValue)
			{
				_setSlider.value = _resetValue;
			}
			if (_isInputLeft)
			{
				if ((bool)_setSlider && !_onButtonDownSlider)
				{
					_setSlider.value -= _sliderDragModifier;
				}
				if (!Player._mainPlayer && _isChestReaction && !_isResettingReaction)
				{
					MainMenuManager._current._characterCreationManager.BoobSurpriseEffect();
					MainMenuManager._current._characterCreationManager.OnBoobSliderHold();
					StartCoroutine(Reset_CCReactionInput());
				}
				if (!Player._mainPlayer && _isBottomReaction && !_isResettingReaction)
				{
					MainMenuManager._current._characterCreationManager.BottomSurpriseEffect();
					MainMenuManager._current._characterCreationManager.OnBottomSliderHold();
					StartCoroutine(Reset_CCReactionInput());
				}
				if (!Player._mainPlayer && _isArmReaction && !_isResettingReaction)
				{
					MainMenuManager._current._characterCreationManager.OnArmSliderHold();
					StartCoroutine(Reset_CCReactionInput());
				}
				if (!Player._mainPlayer && _isVoicePitch && !_isResettingReaction)
				{
					StartCoroutine(Reset_CCReactionInput());
				}
			}
			if (_isInputRight)
			{
				if ((bool)_setSlider && !_onButtonDownSlider)
				{
					_setSlider.value += _sliderDragModifier;
				}
				if (!Player._mainPlayer && _isChestReaction && !_isResettingReaction)
				{
					MainMenuManager._current._characterCreationManager.BoobSurpriseEffect();
					MainMenuManager._current._characterCreationManager.OnBoobSliderHold();
					StartCoroutine(Reset_CCReactionInput());
				}
				if (!Player._mainPlayer && _isBottomReaction && !_isResettingReaction)
				{
					MainMenuManager._current._characterCreationManager.BottomSurpriseEffect();
					MainMenuManager._current._characterCreationManager.OnBottomSliderHold();
					StartCoroutine(Reset_CCReactionInput());
				}
				if (!Player._mainPlayer && _isArmReaction && !_isResettingReaction)
				{
					MainMenuManager._current._characterCreationManager.OnArmSliderHold();
					StartCoroutine(Reset_CCReactionInput());
				}
				if (_isVoicePitch && !_isResettingReaction)
				{
					StartCoroutine(Reset_CCReactionInput());
				}
			}
			if (_isInputLeftDown)
			{
				if ((bool)_setDropdown)
				{
					_setDropdown.value--;
				}
				if ((bool)_setSlider && _onButtonDownSlider)
				{
					_setSlider.value -= _sliderDragModifier;
				}
				if ((bool)_leftOptionButton && _leftOptionButton.interactable)
				{
					_leftOptionButton.onClick.Invoke();
					if ((bool)_leftOptionButton.GetComponent<ButtonSound>())
					{
						_leftOptionButton.GetComponent<ButtonSound>().Play_ClickSound();
					}
				}
			}
			if (!_isInputRightDown)
			{
				return;
			}
			if ((bool)_setDropdown)
			{
				_setDropdown.value++;
			}
			if ((bool)_setSlider && _onButtonDownSlider)
			{
				_setSlider.value += _sliderDragModifier;
			}
			if ((bool)_rightOptionButton && _rightOptionButton.interactable)
			{
				_rightOptionButton.onClick.Invoke();
				if ((bool)_rightOptionButton.GetComponent<ButtonSound>())
				{
					_rightOptionButton.GetComponent<ButtonSound>().Play_ClickSound();
				}
			}
		}
	}

	private IEnumerator Reset_CCReactionInput()
	{
		if (!_isResettingReaction && !Player._mainPlayer)
		{
			_isResettingReaction = true;
			while (_isInputLeft || _isInputRight)
			{
				yield return null;
			}
			yield return new WaitForSeconds(0.2f);
			MainMenuManager._current._characterCreationManager.OnBoobSliderRelease();
			MainMenuManager._current._characterCreationManager.OnArmSliderRelease();
			MainMenuManager._current._characterCreationManager.OnBottomSliderRelease();
			if (_isVoicePitch)
			{
				MainMenuManager._current._characterCreationManager.OnVoiceSliderRelease();
			}
			_isResettingReaction = false;
		}
	}
}
