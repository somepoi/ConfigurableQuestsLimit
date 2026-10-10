using UnityEngine;
using UnityEngine.UI;

public class QuickItemActionBarGUI : MonoBehaviour
{
	public static QuickItemActionBarGUI _current;

	public QuickItemSlot[] _quickItemSlots = new QuickItemSlot[5];

	[Header("Gamepad")]
	public GameObject _gamepad_quickItemSlotObject;

	[SerializeField]
	private Image _gamepad_ico_00;

	[SerializeField]
	private Image _gamepad_ico_01;

	private void Awake()
	{
		_current = this;
	}

	private void Start()
	{
		Invoke("Load_SavedQuickSlotItems", 0.65f);
	}

	public void Load_SavedQuickSlotItems()
	{
		if (ProfileDataManager._current._characterFile._quickItemSlotProfile == null || ProfileDataManager._current._characterFile._quickItemSlotProfile.Length == 0)
		{
			return;
		}
		for (int i = 0; i < _quickItemSlots.Length; i++)
		{
			ScriptableItem scriptableItem = GameManager._current.Locate_Item(ProfileDataManager._current._characterFile._quickItemSlotProfile[i]);
			if ((bool)scriptableItem)
			{
				_quickItemSlots[i].Set_ItemToSlot(scriptableItem);
			}
		}
	}

	private void Update()
	{
		if (!Player._mainPlayer || Player._mainPlayer._currentPlayerCondition != PlayerCondition.ACTIVE || Player._mainPlayer._bufferingStatus || Player._mainPlayer._inChat)
		{
			return;
		}
		bool num = InputControlManager.current._setGamepadType != GamepadType.Keyboard;
		if (GamepadInput.GetButton(GamepadButton.RightTrigger))
		{
			_gamepad_ico_00.color = Color.white;
		}
		else
		{
			_gamepad_ico_00.color = Color.grey;
		}
		if (GamepadInput.GetButton(GamepadButton.Button_South))
		{
			_gamepad_ico_01.color = Color.white;
		}
		else
		{
			_gamepad_ico_01.color = Color.grey;
		}
		if (!num)
		{
			if (Input.GetKeyDown(InputControlManager.current._consumableSlot_0))
			{
				_quickItemSlots[0].Use_QuickSlotItem();
			}
			if (Input.GetKeyDown(InputControlManager.current._consumableSlot_1))
			{
				_quickItemSlots[1].Use_QuickSlotItem();
			}
			if (Input.GetKeyDown(InputControlManager.current._consumableSlot_2))
			{
				_quickItemSlots[2].Use_QuickSlotItem();
			}
			if (Input.GetKeyDown(InputControlManager.current._consumableSlot_3))
			{
				_quickItemSlots[3].Use_QuickSlotItem();
			}
		}
		else if (GamepadInput.GetButton(GamepadButton.LeftShoulder) || GamepadInput.GetButton(GamepadButton.RightShoulder))
		{
			if (InputControlManager.current.DpadLeft(DpadHoldType.OnDown))
			{
				_quickItemSlots[0].Use_QuickSlotItem();
			}
			if (InputControlManager.current.DpadRight(DpadHoldType.OnDown))
			{
				_quickItemSlots[1].Use_QuickSlotItem();
			}
			if (InputControlManager.current.DpadUp(DpadHoldType.OnDown))
			{
				_quickItemSlots[2].Use_QuickSlotItem();
			}
			if (InputControlManager.current.DpadDown(DpadHoldType.OnDown))
			{
				_quickItemSlots[3].Use_QuickSlotItem();
			}
		}
		for (int i = 0; i < _quickItemSlots.Length; i++)
		{
			switch (i)
			{
			case 0:
				_quickItemSlots[i]._hotkeyTag.text = InputControlManager.current.Convert_KeyCodeName(InputControlManager.current._consumableSlot_0.ToString()) ?? "";
				break;
			case 1:
				_quickItemSlots[i]._hotkeyTag.text = InputControlManager.current.Convert_KeyCodeName(InputControlManager.current._consumableSlot_1.ToString()) ?? "";
				break;
			case 2:
				_quickItemSlots[i]._hotkeyTag.text = InputControlManager.current.Convert_KeyCodeName(InputControlManager.current._consumableSlot_2.ToString()) ?? "";
				break;
			case 3:
				_quickItemSlots[i]._hotkeyTag.text = InputControlManager.current.Convert_KeyCodeName(InputControlManager.current._consumableSlot_3.ToString()) ?? "";
				break;
			}
		}
	}
}
