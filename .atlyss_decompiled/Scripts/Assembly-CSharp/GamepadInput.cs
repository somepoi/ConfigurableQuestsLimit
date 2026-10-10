using System;
using System.Collections.Generic;
using UnityEngine;

public class GamepadInput : MonoBehaviour
{
	[Serializable]
	private class PlatformMappings
	{
		public string name;

		public GamepadMapping[] data;

		public PlatformMappings(string name)
		{
			this.name = name;
		}
	}

	[Serializable]
	private class GamepadMapping
	{
		public string name;

		public int[] axisIndexes;

		public Vector2[] axisRange;

		public int[] buttonIndexes;

		public GamepadMapping(string name, int[] axisIndexes, Vector2[] axisRange, int[] buttonIndexes)
		{
			this.name = name;
			this.axisIndexes = axisIndexes;
			this.axisRange = axisRange;
			this.buttonIndexes = buttonIndexes;
		}

		private bool HasButton(GamepadButton button)
		{
			return (int)button < buttonIndexes.Length;
		}

		public int GetButtonIndex(GamepadButton button)
		{
			if (HasButton(button))
			{
				return buttonIndexes[(int)button];
			}
			return -1;
		}

		private bool HasAxis(GamepadAxis axis)
		{
			return (int)axis < axisIndexes.Length;
		}

		public int GetAxisIndex(GamepadAxis axis)
		{
			if (HasAxis(axis))
			{
				return axisIndexes[(int)axis];
			}
			return -1;
		}

		public Vector2 GetAxisRange(GamepadAxis axis)
		{
			if (HasAxis(axis))
			{
				return axisRange[(int)axis];
			}
			return new Vector2(0f, 1f);
		}
	}

	[Serializable]
	private class GamepadRawData
	{
		public int id;

		public bool[] buttons;

		public float[] axes;
	}

	public static GamepadInput _current;

	public GamepadPlatform currentPlatform;

	[SerializeField]
	private PlatformMappings[] mappings;

	public TextAsset mappingsData;

	private GamepadRawData[] gamepadRawData;

	private GamepadRawData[] oldGamepadRawData;

	private static bool updatedThisFrame = false;

	private const float axisPressThreshold = 0.3f;

	private const int MIN_GAMEPAD_ID = 1;

	private const int MAX_GAMEPAD_ID = 4;

	private const int NUM_GAMEPADS = 5;

	private const int MIN_RAW_AXIS_ID = 1;

	private const int MAX_RAW_AXIS_ID = 12;

	private const int NUM_RAW_AXES = 13;

	private const int MIN_RAW_BUTTON_ID = 0;

	private const int MAX_RAW_BUTTON_ID = 19;

	private const int NUM_RAW_BUTTONS = 20;

	private const int JOYSTICK_BUTTON_BASE = 350;

	private const int JOYSTICK_BUTTON_DELTA = 20;

	private const int NUM_GAMEPAD_TYPES = 12;

	private const int NUM_PLATFORMS = 3;

	private const int NUM_STICKS = 3;

	private const int NUM_AXES = 8;

	private const int NUM_BUTTONS = 26;

	private const int NUM_MAPPED_AXES = 8;

	private const int NUM_MAPPED_BUTTONS = 18;

	private static readonly Vector2 V_10 = new Vector2(-1f, 0f);

	private static readonly Vector2 V_11 = new Vector2(-1f, 1f);

	private static readonly Vector2 V0_1 = new Vector2(0f, -1f);

	private static readonly Vector2 V01 = new Vector2(0f, 1f);

	private static readonly Vector2 V1_1 = new Vector2(1f, -1f);

	private static readonly Vector2 V10 = new Vector2(1f, 0f);

	private static readonly Vector2 V_invalid = new Vector2(0f, 1f);

	private static GamepadInput instance;

	private static GamepadPlatform CurrentPlatform => Instance.currentPlatform;

	public static GamepadInput Instance
	{
		get
		{
			if (instance == null)
			{
				instance = UnityEngine.Object.FindObjectOfType<GamepadInput>();
			}
			return instance;
		}
	}

	private void Awake()
	{
		_current = this;
		oldGamepadRawData = ReadAllRawData();
		gamepadRawData = ReadAllRawData();
	}

	private void Start()
	{
		if (Application.platform == RuntimePlatform.WindowsPlayer)
		{
			currentPlatform = GamepadPlatform.Windows;
		}
		if (Application.platform == RuntimePlatform.LinuxPlayer)
		{
			currentPlatform = GamepadPlatform.Linux;
		}
	}

	private void PreUpdate()
	{
		updatedThisFrame = true;
		oldGamepadRawData = gamepadRawData;
		gamepadRawData = ReadAllRawData();
	}

	private void LateUpdate()
	{
		updatedThisFrame = false;
	}

	public static bool GetButton(GamepadButton button)
	{
		if (InputControlManager.current._setGamepadType == GamepadType.Keyboard)
		{
			return false;
		}
		GamepadInfo setGamepadInfo = InputControlManager.current._setGamepadInfo;
		if (!IsValidGamepadId(setGamepadInfo.id))
		{
			return false;
		}
		CheckPreUpdate();
		GamepadMapping mapping = GetMapping(setGamepadInfo, CurrentPlatform);
		GamepadRawData rawData = GetRawData(setGamepadInfo);
		int buttonIndex = mapping.GetButtonIndex(button);
		if (0 <= buttonIndex && buttonIndex < rawData.buttons.Length)
		{
			return rawData.buttons[buttonIndex];
		}
		return TryObtainButtonFromAxisValue(button, mapping, rawData);
	}

	public static bool GetButtonDown(GamepadButton button)
	{
		if ((bool)Player._mainPlayer && InputControlManager.current._setGamepadType == GamepadType.Keyboard)
		{
			return false;
		}
		GamepadInfo setGamepadInfo = InputControlManager.current._setGamepadInfo;
		if (!IsValidGamepadId(setGamepadInfo.id))
		{
			return false;
		}
		CheckPreUpdate();
		GamepadMapping mapping = GetMapping(setGamepadInfo, CurrentPlatform);
		GamepadRawData rawData = GetRawData(setGamepadInfo);
		GamepadRawData oldRawData = GetOldRawData(setGamepadInfo);
		int buttonIndex = mapping.GetButtonIndex(button);
		if (0 <= buttonIndex && buttonIndex < rawData.buttons.Length)
		{
			bool num = oldRawData.buttons[buttonIndex];
			bool flag = rawData.buttons[buttonIndex];
			return !num & flag;
		}
		bool num2 = TryObtainButtonFromAxisValue(button, mapping, oldRawData);
		bool flag2 = TryObtainButtonFromAxisValue(button, mapping, rawData);
		return !num2 & flag2;
	}

	public static bool GetButtonUp(GamepadButton button)
	{
		if (InputControlManager.current._setGamepadType == GamepadType.Keyboard)
		{
			return false;
		}
		GamepadInfo setGamepadInfo = InputControlManager.current._setGamepadInfo;
		if (!IsValidGamepadId(setGamepadInfo.id))
		{
			return false;
		}
		CheckPreUpdate();
		GamepadMapping mapping = GetMapping(setGamepadInfo, CurrentPlatform);
		GamepadRawData rawData = GetRawData(setGamepadInfo);
		GamepadRawData oldRawData = GetOldRawData(setGamepadInfo);
		int buttonIndex = mapping.GetButtonIndex(button);
		if (0 <= buttonIndex && buttonIndex < rawData.buttons.Length)
		{
			bool num = oldRawData.buttons[buttonIndex];
			bool flag = rawData.buttons[buttonIndex];
			if (num)
			{
				return !flag;
			}
			return false;
		}
		bool num2 = TryObtainButtonFromAxisValue(button, mapping, oldRawData);
		bool flag2 = TryObtainButtonFromAxisValue(button, mapping, rawData);
		if (num2)
		{
			return !flag2;
		}
		return false;
	}

	public static float GetAxis(GamepadAxis axis)
	{
		GamepadInfo setGamepadInfo = InputControlManager.current._setGamepadInfo;
		if (!IsValidGamepadId(setGamepadInfo.id))
		{
			return 0f;
		}
		CheckPreUpdate();
		GamepadMapping mapping = GetMapping(setGamepadInfo, CurrentPlatform);
		GamepadRawData rawData = GetRawData(setGamepadInfo);
		int axisIndex = mapping.GetAxisIndex(axis);
		if (0 <= axisIndex && axisIndex < rawData.axes.Length)
		{
			float value = rawData.axes[axisIndex];
			Vector2 axisRange = mapping.GetAxisRange(axis);
			return Remap(value, axisRange, GetOutputRange(axis));
		}
		return TryObtainAxisValueFromButtons(axis, mapping, rawData);
	}

	public static Vector2 GetStick(GamepadStick stick)
	{
		if (!IsValidGamepadId(InputControlManager.current._setGamepadInfo.id))
		{
			return Vector2.zero;
		}
		CheckPreUpdate();
		float axis = GetAxis((GamepadAxis)((int)stick * 2));
		float axis2 = GetAxis((GamepadAxis)((int)stick * 2 + 1));
		return new Vector2(axis, axis2);
	}

	private static void CheckPreUpdate()
	{
		if (!updatedThisFrame)
		{
			Instance.PreUpdate();
		}
	}

	[ContextMenu("Apply Mappings")]
	private void ApplyMappings()
	{
		mappings = ParseMappings(mappingsData);
	}

	private static GamepadRawData[] ReadAllRawData()
	{
		GamepadRawData[] array = new GamepadRawData[5];
		for (int i = 1; i <= 4; i++)
		{
			array[i] = ReadRawdata(i);
		}
		return array;
	}

	private static GamepadRawData ReadRawdata(int gamepadId)
	{
		GamepadRawData gamepadRawData = new GamepadRawData();
		gamepadRawData.id = gamepadId;
		gamepadRawData.buttons = new bool[20];
		for (int i = 0; i <= 19; i++)
		{
			KeyCode key = (KeyCode)(350 + 20 * (gamepadId - 1) + i);
			gamepadRawData.buttons[i] = Input.GetKey(key);
		}
		gamepadRawData.axes = new float[13];
		for (int j = 1; j <= 12; j++)
		{
			gamepadRawData.axes[j] = Input.GetAxisRaw($"Joystick{gamepadId}_Axis{j}");
		}
		return gamepadRawData;
	}

	private static GamepadRawData GetRawData(GamepadInfo gamepadInfo)
	{
		return Instance.gamepadRawData[gamepadInfo.id];
	}

	private static GamepadRawData GetOldRawData(GamepadInfo gamepadInfo)
	{
		return Instance.oldGamepadRawData[gamepadInfo.id];
	}

	private static GamepadMapping GetMapping(GamepadInfo gamepadInfo, GamepadPlatform platform)
	{
		return Instance.mappings[(int)platform].data[(int)gamepadInfo.type];
	}

	private static PlatformMappings[] ParseMappings(TextAsset textAsset)
	{
		string[] array = textAsset.text.Split(new char[2] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
		string[][] array2 = new string[array.Length][];
		for (int i = 0; i < array.Length; i++)
		{
			array2[i] = array[i].Split(new char[1] { '\t' }, StringSplitOptions.None);
		}
		Dictionary<string, Vector2> dictionary = new Dictionary<string, Vector2>
		{
			{ "-1_0", V_10 },
			{ "-1_1", V_11 },
			{ "0_-1", V0_1 },
			{ "0_1", V01 },
			{ "1_-1", V1_1 },
			{ "1_0", V10 }
		};
		PlatformMappings[] array3 = new PlatformMappings[3];
		for (int j = 0; j < 3; j++)
		{
			GamepadPlatform gamepadPlatform = (GamepadPlatform)j;
			string text = gamepadPlatform.ToString();
			array3[j] = new PlatformMappings(text);
			array3[j].data = new GamepadMapping[12];
			for (int k = 0; k < 12; k++)
			{
				GamepadType gamepadType = (GamepadType)k;
				string text2 = gamepadType.ToString();
				int[] array4 = new int[8];
				Vector2[] array5 = new Vector2[8];
				int[] array6 = new int[18];
				int num = k * 3 + j;
				for (int l = 0; l < 8; l++)
				{
					if (int.TryParse(array2[l][num], out var result))
					{
						array4[l] = result;
					}
					else
					{
						array4[l] = -1;
					}
				}
				for (int m = 0; m < 8; m++)
				{
					string text3 = array2[m + 8][num];
					text3 = text3.Trim();
					if (dictionary.ContainsKey(text3))
					{
						array5[m] = dictionary[text3];
					}
					else
					{
						array5[m] = V_invalid;
					}
				}
				for (int n = 0; n < 18; n++)
				{
					if (int.TryParse(array2[n + 16][num], out var result2))
					{
						array6[n] = result2;
					}
					else
					{
						array6[n] = -1;
					}
				}
				array3[j].data[k] = new GamepadMapping(text2, array4, array5, array6);
			}
		}
		return array3;
	}

	private static float Remap(float value, Vector2 from, Vector2 to)
	{
		return Remap(value, new Vector4(from.x, from.y, to.x, to.y));
	}

	private static float Remap(float value, Vector4 range)
	{
		float x = range.x;
		float y = range.y;
		float z = range.z;
		float w = range.w;
		return z + (w - z) * ((value - x) / (y - x));
	}

	private static Vector2 GetOutputRange(GamepadAxis axis)
	{
		if (axis < GamepadAxis.LeftTrigger)
		{
			return new Vector2(-1f, 1f);
		}
		return new Vector2(0f, 1f);
	}

	private static bool TryObtainButtonFromAxisValue(GamepadButton button, GamepadMapping mapping, GamepadRawData data)
	{
		GamepadAxis axis = AxisFromButton(button);
		int axisIndex = mapping.GetAxisIndex(axis);
		if (0 <= axisIndex && axisIndex < data.axes.Length)
		{
			float value = data.axes[axisIndex];
			Vector2 axisRange = mapping.GetAxisRange(axis);
			float num = Remap(value, axisRange, GetOutputRange(axis));
			int num2 = SignFromButton(button);
			return num * (float)num2 >= 0.3f;
		}
		return false;
	}

	private static float TryObtainAxisValueFromButtons(GamepadAxis axis, GamepadMapping mapping, GamepadRawData data)
	{
		GamepadButton button = NegativeButtonFromAxis(axis);
		GamepadButton button2 = PositiveButtonFromAxis(axis);
		bool flag = false;
		int buttonIndex = mapping.GetButtonIndex(button);
		if (0 <= buttonIndex && buttonIndex < data.buttons.Length)
		{
			flag = data.buttons[buttonIndex];
		}
		bool flag2 = false;
		int buttonIndex2 = mapping.GetButtonIndex(button2);
		if (0 <= buttonIndex2 && buttonIndex2 < data.buttons.Length)
		{
			flag2 = data.buttons[buttonIndex2];
		}
		float num = 0f;
		if (flag)
		{
			num--;
		}
		if (flag2)
		{
			num++;
		}
		return num;
	}

	private static GamepadAxis AxisFromButton(GamepadButton button)
	{
		return button switch
		{
			GamepadButton.LeftTrigger => GamepadAxis.LeftTrigger, 
			GamepadButton.RightTrigger => GamepadAxis.RightTrigger, 
			GamepadButton.DPad_Up => GamepadAxis.DPadY, 
			GamepadButton.DPad_Down => GamepadAxis.DPadY, 
			GamepadButton.DPad_Left => GamepadAxis.DPadX, 
			GamepadButton.DPad_Right => GamepadAxis.DPadX, 
			GamepadButton.LeftStick_Up => GamepadAxis.LeftY, 
			GamepadButton.LeftStick_Down => GamepadAxis.LeftY, 
			GamepadButton.LeftStick_Left => GamepadAxis.LeftX, 
			GamepadButton.LeftStick_Right => GamepadAxis.LeftX, 
			GamepadButton.RightStick_Up => GamepadAxis.RightY, 
			GamepadButton.RightStick_Down => GamepadAxis.RightY, 
			GamepadButton.RightStick_Left => GamepadAxis.RightX, 
			GamepadButton.RightStick_Right => GamepadAxis.RightX, 
			_ => GamepadAxis.INVALID, 
		};
	}

	private static int SignFromButton(GamepadButton button)
	{
		return button switch
		{
			GamepadButton.LeftTrigger => 1, 
			GamepadButton.RightTrigger => 1, 
			GamepadButton.DPad_Up => 1, 
			GamepadButton.DPad_Down => -1, 
			GamepadButton.DPad_Left => -1, 
			GamepadButton.DPad_Right => 1, 
			GamepadButton.LeftStick_Up => 1, 
			GamepadButton.LeftStick_Down => -1, 
			GamepadButton.LeftStick_Left => -1, 
			GamepadButton.LeftStick_Right => 1, 
			GamepadButton.RightStick_Up => 1, 
			GamepadButton.RightStick_Down => -1, 
			GamepadButton.RightStick_Left => -1, 
			GamepadButton.RightStick_Right => 1, 
			_ => 0, 
		};
	}

	private static GamepadButton NegativeButtonFromAxis(GamepadAxis axis)
	{
		return axis switch
		{
			GamepadAxis.LeftTrigger => GamepadButton.INVALID, 
			GamepadAxis.RightTrigger => GamepadButton.INVALID, 
			GamepadAxis.DPadX => GamepadButton.DPad_Left, 
			GamepadAxis.DPadY => GamepadButton.DPad_Down, 
			GamepadAxis.LeftX => GamepadButton.LeftStick_Left, 
			GamepadAxis.LeftY => GamepadButton.LeftStick_Down, 
			GamepadAxis.RightX => GamepadButton.RightStick_Left, 
			GamepadAxis.RightY => GamepadButton.RightStick_Down, 
			_ => GamepadButton.INVALID, 
		};
	}

	private static GamepadButton PositiveButtonFromAxis(GamepadAxis axis)
	{
		return axis switch
		{
			GamepadAxis.LeftTrigger => GamepadButton.LeftTrigger, 
			GamepadAxis.RightTrigger => GamepadButton.RightTrigger, 
			GamepadAxis.DPadX => GamepadButton.DPad_Right, 
			GamepadAxis.DPadY => GamepadButton.DPad_Up, 
			GamepadAxis.LeftX => GamepadButton.LeftStick_Right, 
			GamepadAxis.LeftY => GamepadButton.LeftStick_Up, 
			GamepadAxis.RightX => GamepadButton.RightStick_Right, 
			GamepadAxis.RightY => GamepadButton.RightStick_Up, 
			_ => GamepadButton.INVALID, 
		};
	}

	private static bool IsValidGamepadId(int id)
	{
		if (1 <= id)
		{
			return id <= 4;
		}
		return false;
	}
}
