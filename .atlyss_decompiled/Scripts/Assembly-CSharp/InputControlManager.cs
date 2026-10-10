using System;
using System.IO;
using UnityEngine;

public class InputControlManager : MonoBehaviour
{
	public static InputControlManager current;

	[Header("Input Control Management")]
	[SerializeField]
	private string dataPath = string.Empty;

	public GamepadType _setGamepadType = GamepadType.Keyboard;

	public GamepadInfo _setGamepadInfo;

	[Space]
	public ControlInputProfile _inputProfile;

	[Header("Movement Inputs")]
	public KeyCode _up;

	public KeyCode _down;

	public KeyCode _left;

	public KeyCode _right;

	public KeyCode _jump;

	public KeyCode _dash;

	public KeyCode _lockDirection;

	public bool isLockDirectionToggled;

	[Header("Action Inputs")]
	public KeyCode _attack;

	public KeyCode _chargeAttack;

	public KeyCode _block;

	public KeyCode _target;

	public KeyCode _interact;

	public KeyCode _flagPvp;

	public KeyCode _resetCamera;

	public KeyCode actionBar_0;

	public KeyCode actionBar_1;

	public KeyCode actionBar_2;

	public KeyCode actionBar_3;

	public KeyCode actionBar_4;

	public KeyCode actionBar_5;

	public KeyCode _recall;

	public KeyCode _quickSwapWeapon;

	public KeyCode _sheatheWeapon;

	public KeyCode _sit;

	[Header("Items")]
	public KeyCode _consumableSlot_0;

	public KeyCode _consumableSlot_1;

	public KeyCode _consumableSlot_2;

	public KeyCode _consumableSlot_3;

	[Header("UI")]
	public KeyCode _tabMenu;

	public KeyCode _itemMenuHotkey;

	public KeyCode _statMenuHotkey;

	public KeyCode _skillMenuHotkey;

	public KeyCode _questMenuHotkey;

	public KeyCode _whoMenuHotkey;

	public KeyCode _hideUI;

	public KeyCode _hostConsole;

	[Header("Input Axis")]
	public float _horiz_input;

	public float _vert_input;

	[Space]
	public float altAxis_Sensitivity = 1f;

	public float _altHoriz_input;

	public float _altVert_input;

	[Space]
	public bool _invert_altHoriz;

	public bool _invert_vertHoriz;

	[Header("Gamepad Elements")]
	public float _gamepadMouseCursorBuffer;

	public float _analogAltAxisModifier = 2.5f;

	[Space]
	private float _linux_horizDpad;

	private bool _linux_horizDpad_down;

	private float _linux_vertDpad;

	private bool _linux_vertDpad_down;

	public bool IsDashInput
	{
		get
		{
			if (!Input.GetKey(_dash))
			{
				if (!GamepadInput.GetButton(GamepadButton.LeftShoulder) && !GamepadInput.GetButton(GamepadButton.RightShoulder))
				{
					return GamepadInput.GetButton(GamepadButton.Button_East);
				}
				return false;
			}
			return true;
		}
	}

	public bool IsDashInputDown
	{
		get
		{
			if (!Input.GetKeyDown(_dash))
			{
				if (!GamepadInput.GetButton(GamepadButton.LeftShoulder) && !GamepadInput.GetButton(GamepadButton.RightShoulder))
				{
					return GamepadInput.GetButtonDown(GamepadButton.Button_East);
				}
				return false;
			}
			return true;
		}
	}

	public bool IsInteractionInput
	{
		get
		{
			if (!Input.GetKey(_interact))
			{
				return GamepadInput.GetButton(GamepadButton.Button_West);
			}
			return true;
		}
	}

	public bool IsInteractionInputDown
	{
		get
		{
			if (!Input.GetKeyDown(_interact))
			{
				return GamepadInput.GetButtonDown(GamepadButton.Button_West);
			}
			return true;
		}
	}

	public bool DpadLeft(DpadHoldType _holdType)
	{
		if (_holdType == DpadHoldType.OnDown && _linux_horizDpad_down)
		{
			return false;
		}
		if (_linux_horizDpad < 0f)
		{
			_linux_horizDpad_down = true;
			return true;
		}
		if (_holdType == DpadHoldType.OnDown && GamepadInput.GetButtonDown(GamepadButton.DPad_Left))
		{
			return true;
		}
		if (_holdType == DpadHoldType.OnHold && GamepadInput.GetButton(GamepadButton.DPad_Left))
		{
			return true;
		}
		if (!Player._mainPlayer && Input.GetKeyDown(KeyCode.LeftArrow))
		{
			return true;
		}
		_linux_horizDpad_down = false;
		return false;
	}

	public bool DpadRight(DpadHoldType _holdType)
	{
		if (_holdType == DpadHoldType.OnDown && _linux_horizDpad_down)
		{
			return false;
		}
		if (_linux_horizDpad > 0f)
		{
			_linux_horizDpad_down = true;
			return true;
		}
		if (_holdType == DpadHoldType.OnDown && GamepadInput.GetButtonDown(GamepadButton.DPad_Right))
		{
			return true;
		}
		if (_holdType == DpadHoldType.OnHold && GamepadInput.GetButton(GamepadButton.DPad_Right))
		{
			return true;
		}
		if (!Player._mainPlayer && Input.GetKeyDown(KeyCode.RightArrow))
		{
			return true;
		}
		_linux_horizDpad_down = false;
		return false;
	}

	public bool DpadUp(DpadHoldType _holdType)
	{
		if (_holdType == DpadHoldType.OnDown && _linux_vertDpad_down)
		{
			return false;
		}
		if (_linux_vertDpad < 0f)
		{
			_linux_vertDpad_down = true;
			return true;
		}
		if (_holdType == DpadHoldType.OnDown && GamepadInput.GetButtonDown(GamepadButton.DPad_Up))
		{
			return true;
		}
		if (_holdType == DpadHoldType.OnHold && GamepadInput.GetButton(GamepadButton.DPad_Up))
		{
			return true;
		}
		if (!Player._mainPlayer && Input.GetKeyDown(KeyCode.UpArrow))
		{
			return true;
		}
		_linux_vertDpad_down = false;
		return false;
	}

	public bool DpadDown(DpadHoldType _holdType)
	{
		if (_holdType == DpadHoldType.OnDown && _linux_vertDpad_down)
		{
			return false;
		}
		if (_linux_vertDpad > 0f)
		{
			_linux_vertDpad_down = true;
			return true;
		}
		if (_holdType == DpadHoldType.OnDown && GamepadInput.GetButtonDown(GamepadButton.DPad_Down))
		{
			return true;
		}
		if (_holdType == DpadHoldType.OnHold && GamepadInput.GetButton(GamepadButton.DPad_Down))
		{
			return true;
		}
		if (!Player._mainPlayer && Input.GetKeyDown(KeyCode.DownArrow))
		{
			return true;
		}
		_linux_vertDpad_down = false;
		return false;
	}

	private void Awake()
	{
		if (current == null)
		{
			UnityEngine.Object.DontDestroyOnLoad(this);
			current = this;
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void Start()
	{
		dataPath = Application.dataPath + "/Settings/";
		if (!Directory.Exists(dataPath))
		{
			Directory.CreateDirectory(dataPath);
			InputProfile_NewProfile();
			SaveInputData();
			LoadInputData();
		}
		else
		{
			LoadInputData();
		}
	}

	[ContextMenu("Reset Input Profile")]
	public void InputProfile_NewProfile()
	{
		InputProfile_SetDefaults();
		_inputProfile.isLockDirectionToggled = false;
		_inputProfile.altAxisSensitivity = 1f;
	}

	public void InputProfile_SetDefaults()
	{
		_inputProfile._gamepadType = GamepadType.Keyboard;
		_inputProfile.altAxisSensitivity = 1f;
		_inputProfile._jumpInput = $"{KeyCode.Space}";
		_inputProfile._forwardInput = $"{KeyCode.W}";
		_inputProfile._backwardInput = $"{KeyCode.S}";
		_inputProfile._leftInput = $"{KeyCode.A}";
		_inputProfile._rightInput = $"{KeyCode.D}";
		_inputProfile._dashInput = $"{KeyCode.E}";
		_inputProfile._lockDirection = $"{KeyCode.LeftShift}";
		_inputProfile._flagPvpInput = $"{KeyCode.V}";
		_inputProfile._resetCameraInput = $"{KeyCode.H}";
		_inputProfile._interactInput = $"{KeyCode.F}";
		_inputProfile._attackInput = $"{KeyCode.Mouse0}";
		_inputProfile._chargeAttackInput = $"{KeyCode.R}";
		_inputProfile._blockInput = $"{KeyCode.Mouse1}";
		_inputProfile._targetInput = $"{KeyCode.Mouse2}";
		_inputProfile.actionBarInput_0 = $"{KeyCode.Alpha1}";
		_inputProfile.actionBarInput_1 = $"{KeyCode.Alpha2}";
		_inputProfile.actionBarInput_2 = $"{KeyCode.Alpha3}";
		_inputProfile.actionBarInput_3 = $"{KeyCode.Alpha4}";
		_inputProfile.actionBarInput_4 = $"{KeyCode.Alpha5}";
		_inputProfile.actionBarInput_5 = $"{KeyCode.Alpha6}";
		_inputProfile._recallInput = $"{KeyCode.G}";
		_inputProfile._quickSwapWeaponInput = $"{KeyCode.Q}";
		_inputProfile._sheatheWeaponInput = $"{KeyCode.Z}";
		_inputProfile._sitInput = $"{KeyCode.X}";
		_inputProfile._consumableSlot_0 = $"{KeyCode.F1}";
		_inputProfile._consumableSlot_1 = $"{KeyCode.F2}";
		_inputProfile._consumableSlot_2 = $"{KeyCode.F3}";
		_inputProfile._consumableSlot_3 = $"{KeyCode.F4}";
		_inputProfile._consumableSlot_4 = $"{KeyCode.F5}";
		_inputProfile._tabMenuInput = $"{KeyCode.Tab}";
		_inputProfile._lexiconHotkeyInput = $"{KeyCode.T}";
		_inputProfile._itemMenuHotkeyInput = $"{KeyCode.I}";
		_inputProfile._skillMenuHotkeyInput = $"{KeyCode.K}";
		_inputProfile._statMenuHotkeyInput = $"{KeyCode.P}";
		_inputProfile._questMenuHotkeyInput = $"{KeyCode.L}";
		_inputProfile._whoMenuHotkeyInput = $"{KeyCode.O}";
		_inputProfile._hideUIInput = $"{KeyCode.Backslash}";
		_inputProfile._hostConsoleInput = $"{KeyCode.BackQuote}";
		SaveInputData();
		LoadInputData();
	}

	public void SaveInputData()
	{
		if (!Directory.Exists(dataPath))
		{
			Directory.CreateDirectory(dataPath);
		}
		string contents = JsonUtility.ToJson(_inputProfile, prettyPrint: true);
		File.WriteAllText(dataPath + "/inputProfile.json", contents);
	}

	public void LoadInputData()
	{
		if (!Directory.Exists(dataPath))
		{
			return;
		}
		string json = File.ReadAllText(dataPath + "/inputProfile.json");
		_inputProfile = JsonUtility.FromJson<ControlInputProfile>(json);
		_setGamepadType = _inputProfile._gamepadType;
		altAxis_Sensitivity = _inputProfile.altAxisSensitivity;
		foreach (KeyCode value in Enum.GetValues(typeof(KeyCode)))
		{
			string text = value.ToString();
			if (text == _inputProfile._forwardInput)
			{
				_up = value;
			}
			if (text == _inputProfile._backwardInput)
			{
				_down = value;
			}
			if (text == _inputProfile._leftInput)
			{
				_left = value;
			}
			if (text == _inputProfile._rightInput)
			{
				_right = value;
			}
			if (text == _inputProfile._jumpInput)
			{
				_jump = value;
			}
			if (text == _inputProfile._dashInput)
			{
				_dash = value;
			}
			if (text == _inputProfile._lockDirection)
			{
				_lockDirection = value;
			}
			if (text == _inputProfile._attackInput)
			{
				_attack = value;
			}
			if (text == _inputProfile._chargeAttackInput)
			{
				_chargeAttack = value;
			}
			if (text == _inputProfile._blockInput)
			{
				_block = value;
			}
			if (text == _inputProfile._targetInput)
			{
				_target = value;
			}
			if (text == _inputProfile._interactInput)
			{
				_interact = value;
			}
			if (text == _inputProfile._flagPvpInput)
			{
				_flagPvp = value;
			}
			if (text == _inputProfile._resetCameraInput)
			{
				_resetCamera = value;
			}
			if (text == _inputProfile.actionBarInput_0)
			{
				actionBar_0 = value;
			}
			if (text == _inputProfile.actionBarInput_1)
			{
				actionBar_1 = value;
			}
			if (text == _inputProfile.actionBarInput_2)
			{
				actionBar_2 = value;
			}
			if (text == _inputProfile.actionBarInput_3)
			{
				actionBar_3 = value;
			}
			if (text == _inputProfile.actionBarInput_4)
			{
				actionBar_4 = value;
			}
			if (text == _inputProfile.actionBarInput_5)
			{
				actionBar_5 = value;
			}
			if (text == _inputProfile._recallInput)
			{
				_recall = value;
			}
			if (text == _inputProfile._quickSwapWeaponInput)
			{
				_quickSwapWeapon = value;
			}
			if (text == _inputProfile._sheatheWeaponInput)
			{
				_sheatheWeapon = value;
			}
			if (text == _inputProfile._sitInput)
			{
				_sit = value;
			}
			if (text == _inputProfile._consumableSlot_0)
			{
				_consumableSlot_0 = value;
			}
			if (text == _inputProfile._consumableSlot_1)
			{
				_consumableSlot_1 = value;
			}
			if (text == _inputProfile._consumableSlot_2)
			{
				_consumableSlot_2 = value;
			}
			if (text == _inputProfile._consumableSlot_3)
			{
				_consumableSlot_3 = value;
			}
			if (text == _inputProfile._tabMenuInput)
			{
				_tabMenu = value;
			}
			if (text == _inputProfile._statMenuHotkeyInput)
			{
				_statMenuHotkey = value;
			}
			if (text == _inputProfile._skillMenuHotkeyInput)
			{
				_skillMenuHotkey = value;
			}
			if (text == _inputProfile._itemMenuHotkeyInput)
			{
				_itemMenuHotkey = value;
			}
			if (text == _inputProfile._questMenuHotkeyInput)
			{
				_questMenuHotkey = value;
			}
			if (text == _inputProfile._whoMenuHotkeyInput)
			{
				_whoMenuHotkey = value;
			}
			if (text == _inputProfile._hideUIInput)
			{
				_hideUI = value;
			}
			if (text == _inputProfile._hostConsoleInput)
			{
				_hostConsole = value;
			}
		}
	}

	private void Update()
	{
		Handle_ControlInput();
	}

	private void Handle_ControlInput()
	{
		_vert_input = (Input.GetKey(_up) ? 1 : 0) - (Input.GetKey(_down) ? 1 : 0);
		_horiz_input = (Input.GetKey(_right) ? 1 : 0) - (Input.GetKey(_left) ? 1 : 0);
		_altHoriz_input = Input.GetAxisRaw("Mouse X") * altAxis_Sensitivity;
		_altVert_input = Input.GetAxisRaw("Mouse Y") * altAxis_Sensitivity;
		float _gamePadHoriz = GamepadInput.GetAxis(GamepadAxis.LeftX);
		float _gamePadVert = GamepadInput.GetAxis(GamepadAxis.LeftY);
		float _gamePadAltHoriz = GamepadInput.GetAxis(GamepadAxis.RightX) * _analogAltAxisModifier;
		float _gamePadAltVert = GamepadInput.GetAxis(GamepadAxis.RightY) * _analogAltAxisModifier * -1f;
		if (SettingsManager._current._analogDirectionMode.isOn && _setGamepadType == GamepadType.Keyboard)
		{
			if (_gamePadHoriz != 0f)
			{
				_horiz_input = _gamePadHoriz;
			}
			if (_gamePadVert != 0f)
			{
				_vert_input = _gamePadVert;
			}
		}
		Handle_GamepadInput();
		if (_invert_altHoriz)
		{
			_altHoriz_input *= -1f;
		}
		if (_invert_vertHoriz)
		{
			_altVert_input *= -1f;
		}
		void Handle_GamepadInput()
		{
			GamepadInfo setGamepadInfo = new GamepadInfo(1, GamepadType.Xbox360);
			_setGamepadInfo = setGamepadInfo;
			if (_gamepadMouseCursorBuffer > 0f)
			{
				if ((bool)TabMenu._current)
				{
					if (!TabMenu._current._itemCell._heldItemListDataEntry)
					{
						_gamepadMouseCursorBuffer -= Time.deltaTime;
					}
				}
				else
				{
					_gamepadMouseCursorBuffer -= Time.deltaTime;
				}
			}
			else
			{
				_gamepadMouseCursorBuffer = 0f;
			}
			if (_setGamepadType == GamepadType.Keyboard)
			{
				_gamepadMouseCursorBuffer = 0f;
			}
			else
			{
				float axisRaw = Input.GetAxisRaw("Mouse X");
				float axisRaw2 = Input.GetAxisRaw("Mouse Y");
				if (axisRaw != 0f || axisRaw2 != 0f)
				{
					if (_gamepadMouseCursorBuffer == 0f && (bool)ToolTipManager._current)
					{
						ToolTipManager._current.Disable_AllTooltips();
					}
					if ((bool)TabMenu._current)
					{
						if (!TabMenu._current._itemCell._heldItemListDataEntry)
						{
							_gamepadMouseCursorBuffer = 2f;
						}
					}
					else
					{
						_gamepadMouseCursorBuffer = 2f;
					}
				}
				if (_gamePadHoriz != 0f)
				{
					_horiz_input = _gamePadHoriz;
				}
				if (_gamePadVert != 0f)
				{
					_vert_input = _gamePadVert;
				}
				if (_gamePadAltHoriz != 0f)
				{
					_altHoriz_input = _gamePadAltHoriz;
				}
				if (_gamePadAltVert != 0f)
				{
					_altVert_input = _gamePadAltVert;
				}
				if (GamepadInput._current.currentPlatform == GamepadPlatform.Linux)
				{
					_linux_horizDpad = Input.GetAxisRaw("Xbox_DpadX");
					_linux_vertDpad = Input.GetAxisRaw("Xbox_DpadY");
					if (_linux_horizDpad == 0f && _linux_vertDpad == 0f)
					{
						_linux_horizDpad_down = false;
						_linux_vertDpad_down = false;
					}
				}
			}
		}
	}

	public string Convert_KeyCodeName(string _string)
	{
		return _string switch
		{
			"None" => "Unbound", 
			"Alpha1" => "1", 
			"Alpha2" => "2", 
			"Alpha3" => "3", 
			"Alpha4" => "4", 
			"Alpha5" => "5", 
			"Alpha6" => "6", 
			"Alpha7" => "7", 
			"Alpha8" => "8", 
			"Alpha9" => "9", 
			"Alpha0" => "0", 
			"LeftShift" => "LSh", 
			"RightShift" => "RSh", 
			"LeftControl" => "LCtrl", 
			"RightControl" => "RCtrl", 
			"UpArrow" => "Up", 
			"DownArrow" => "Down", 
			"LeftArrow" => "Left", 
			"RightArrow" => "Right", 
			"Mouse0" => "M1", 
			"Mouse1" => "M2", 
			"Mouse2" => "M3", 
			"Mouse3" => "M4", 
			"Mouse4" => "M5", 
			"Mouse5" => "M6", 
			"Mouse6" => "M7", 
			"Keypad1" => "KP1", 
			"Keypad2" => "KP2", 
			"Keypad3" => "KP3", 
			"Keypad4" => "KP4", 
			"Keypad5" => "KP5", 
			"Keypad6" => "KP6", 
			"Keypad7" => "KP7", 
			"Keypad8" => "KP8", 
			"Keypad9" => "KP9", 
			"Keypad0" => "KP0", 
			"BackQuote" => "Tilde", 
			"Backslash" => "\\", 
			"Slash" => "/", 
			"LeftBracket" => "[", 
			"RightBracket" => "]", 
			"Minus" => "-", 
			"Plus" => "+", 
			"Equals" => "=", 
			"Period" => ".", 
			"Semicolon" => ";", 
			"Quote" => "'", 
			"KeypadMinus" => "KP-", 
			"KeypadMultiply" => "KP*", 
			"KeypadDivide" => "KP/", 
			"KeypadPeriod" => "KP.", 
			"KeypadEnter" => "KPEn", 
			"Joystick1Button0" => "(A)", 
			"Joystick1Button1" => "(B)", 
			"Joystick1Button2" => "(X)", 
			"Joystick1Button3" => "(Y)", 
			"Joystick1Button4" => "(LB)", 
			"Joystick1Button5" => "(RB)", 
			"Joystick1Button6" => "Select", 
			"Joystick1Button7" => "Start", 
			"Joystick1Button8" => "LPress", 
			"Joystick1Button9" => "RPress", 
			_ => _string, 
		};
	}
}
