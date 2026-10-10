using UnityEngine;

[CreateAssetMenu(fileName = "class_", menuName = "ScriptableObjects/Game Logic/Playable Class", order = 0)]
public class ScriptablePlayerBaseClass : ScriptableObject
{
	[Header("Class Parameters")]
	public string _className = string.Empty;

	public Sprite _classIcon;

	public Color _blockEmissionColor = Color.grey;

	[Space]
	public ScriptableSkill[] _classSkills;

	[Space]
	public PlayerClassTier[] _playerClassTiers;
}
