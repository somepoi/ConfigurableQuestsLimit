using UnityEngine;

[CreateAssetMenu(fileName = "profession_", menuName = "ScriptableObjects/Game Logic/Profession", order = 0)]
public class ScriptableProfession : ScriptableObject
{
	[Header("Scriptable Profession")]
	public string _professionName = "New Profession";

	public Sprite _professionIcon;

	[Space]
	public int _maxProfessionLevel = 5;

	public AnimationCurve _professionCurve;

	[Space]
	public string _interactionTag = "INTERACT";

	public ScriptableProfessionTool _professionTool;
}
