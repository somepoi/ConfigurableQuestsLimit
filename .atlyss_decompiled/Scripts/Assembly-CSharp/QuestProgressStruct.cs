using System;

[Serializable]
public class QuestProgressStruct
{
	public string _questTag;

	public int[] _itemProgressValues;

	public int[] _creepKillProgressValues;

	public int[] _triggerProgressValues;

	public bool _questComplete;

	public bool _hideQuestTrack;
}
