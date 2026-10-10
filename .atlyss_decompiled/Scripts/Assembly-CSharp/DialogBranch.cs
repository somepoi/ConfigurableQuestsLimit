using System;
using UnityEngine;

[Serializable]
public class DialogBranch
{
	[HideInInspector]
	public string _dialogIndexTag = string.Empty;

	public Dialog[] dialogs = new Dialog[1];
}
