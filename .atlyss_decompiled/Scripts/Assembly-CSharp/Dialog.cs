using System;
using UnityEngine;

[Serializable]
public class Dialog
{
	[HideInInspector]
	public string _dialogKey = string.Empty;

	[TextArea(3, 3)]
	public string _dialogInput;

	public string[] _altInputs = new string[0];

	public Sprite facepic;

	public string _npcAnimationTag = string.Empty;

	public DialogUIPrompt _dialogUI;

	[Space]
	public DialogSelection[] _dialogSelections;
}
