using UnityEngine;

[CreateAssetMenu(fileName = "emoteList_", menuName = "ScriptableObjects/Game Logic/Emote List", order = 0)]
public class ScriptableEmoteList : ScriptableObject
{
	public EmoteCommand[] _emoteCommandList = new EmoteCommand[0];
}
