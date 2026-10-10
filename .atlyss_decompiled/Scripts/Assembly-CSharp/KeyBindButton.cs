using System;
using UnityEngine.UI;

[Serializable]
public class KeyBindButton
{
	public string _inputReference = string.Empty;

	public string _keyBind = string.Empty;

	public Button _button;

	public Text _buttonLabel;

	public bool waitForKeyPress;
}
