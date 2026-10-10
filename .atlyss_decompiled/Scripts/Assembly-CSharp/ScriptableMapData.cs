using Mirror;
using UnityEngine;

[CreateAssetMenu(fileName = "wayPoint_", menuName = "ScriptableObjects/Waypoint", order = 0)]
public class ScriptableMapData : ScriptableObject
{
	[Header("Scriptable Map Data / Waypoint")]
	public string _mapCaptionTitle = "";

	public string _spawnPointTag = "startPoint";

	public string _mapLockID = "";

	public int _GUI_index;

	[Space]
	[Scene]
	public string _subScene = string.Empty;

	[Space]
	public Sprite _mapSelectionIcon;

	public Vector2 _mapIconPosition = Vector2.zero;

	[Space]
	public Sprite _backdropSprite;

	public Vector2 _backdropPosition = Vector2.zero;

	public Vector2 _backdropScaling = Vector2.one;
}
