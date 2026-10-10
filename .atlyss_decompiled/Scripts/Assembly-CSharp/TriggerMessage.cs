using System;

[Serializable]
public class TriggerMessage
{
	public string _singleMessage = "";

	public string _incompleteMessage = "";

	public string[] _triggerMessageArray = new string[0];

	public bool _sentMsg;
}
