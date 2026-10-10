using UnityEngine;
using UnityEngine.UI;

public class QuickSwapWeaponGUI : MonoBehaviour
{
	[Header("Weapon Quick Swapper")]
	[SerializeField]
	private MenuElement _weaponQuickSwapSlot;

	[SerializeField]
	private MenuElement _altWeaponQuickSwapSlot;

	[SerializeField]
	private MenuElement _swapQueueElement;

	[SerializeField]
	private Image _weaponQuickSwapIcon;

	[SerializeField]
	private Image _altWeaponQuickSwapIcon;

	[SerializeField]
	private Image _quickSwapGamepadIcon;

	[SerializeField]
	private Text _quickSwapHotkeyText;

	[Space]
	[SerializeField]
	private Image _lockWeapon_ico;

	[Space]
	[SerializeField]
	private Button _mainNumeralDisplay;

	[SerializeField]
	private Button _altNumeralDisplay;

	private void Update()
	{
		Handle_WeaponQuickSwapDisplay();
	}

	private void Handle_WeaponQuickSwapDisplay()
	{
		if (!Player._mainPlayer)
		{
			return;
		}
		PlayerCombat pCombat = Player._mainPlayer._pCombat;
		if (InputControlManager.current._setGamepadType != GamepadType.Keyboard)
		{
			_quickSwapHotkeyText.text = string.Empty;
		}
		else
		{
			_quickSwapHotkeyText.text = InputControlManager.current.Convert_KeyCodeName(InputControlManager.current._quickSwapWeapon.ToString());
		}
		_quickSwapGamepadIcon.enabled = InputControlManager.current._setGamepadType != GamepadType.Keyboard;
		_mainNumeralDisplay.interactable = !pCombat._isUsingAltWeapon;
		_altNumeralDisplay.interactable = pCombat._isUsingAltWeapon;
		if (!GamepadInput.GetButton(GamepadButton.LeftShoulder) && !GamepadInput.GetButton(GamepadButton.RightShoulder))
		{
			_quickSwapGamepadIcon.color = Color.white;
		}
		else
		{
			_quickSwapGamepadIcon.color = Color.grey;
		}
		bool num = pCombat.CanQuickSwapWeapon(_hasInput: false);
		_swapQueueElement.isEnabled = pCombat._weaponSwapQueued;
		_lockWeapon_ico.enabled = pCombat._lockWeapon;
		if (!num)
		{
			_weaponQuickSwapIcon.color = Color.gray;
			_altWeaponQuickSwapIcon.color = Color.gray;
		}
		else
		{
			_weaponQuickSwapIcon.color = Color.white;
			_altWeaponQuickSwapIcon.color = Color.white;
		}
		if (pCombat._lockWeapon)
		{
			_weaponQuickSwapSlot.transform.SetSiblingIndex(1);
			_altWeaponQuickSwapSlot.isEnabled = false;
			_weaponQuickSwapSlot.isEnabled = true;
		}
		else
		{
			_altWeaponQuickSwapSlot.isEnabled = pCombat._isUsingAltWeapon;
			_weaponQuickSwapSlot.isEnabled = !pCombat._isUsingAltWeapon;
			if (_weaponQuickSwapSlot.isEnabled)
			{
				_weaponQuickSwapSlot.transform.SetSiblingIndex(1);
			}
			if (_altWeaponQuickSwapSlot.isEnabled)
			{
				_altWeaponQuickSwapSlot.transform.SetSiblingIndex(1);
			}
		}
		if ((bool)pCombat._foundWeaponEquip._scriptableEquip)
		{
			_weaponQuickSwapIcon.enabled = true;
			_weaponQuickSwapIcon.sprite = pCombat._foundWeaponEquip._scriptableEquip._itemIcon;
		}
		else
		{
			_weaponQuickSwapIcon.enabled = false;
		}
		if ((bool)pCombat._foundAltWeaponEquip._scriptableEquip)
		{
			_altWeaponQuickSwapIcon.enabled = true;
			_altWeaponQuickSwapIcon.sprite = pCombat._foundAltWeaponEquip._scriptableEquip._itemIcon;
		}
		else
		{
			_altWeaponQuickSwapIcon.enabled = false;
		}
	}
}
