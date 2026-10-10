using UnityEngine;

[CreateAssetMenu(fileName = "comElement_", menuName = "ScriptableObjects/Game Logic/Combat Element", order = 0)]
public class ScriptableCombatElement : ScriptableObject
{
	[Header("Combat Element")]
	public string _elementName = "New Element";

	public int _elementID;

	[Space]
	public Sprite _elementIcon;

	public GameObject _elementHitEffect;

	[Space]
	public Sprite _floatValueIcon;

	public Color _floatValueColor = Color.white;

	[Space]
	public AudioClip _lightHitSfx;

	public AudioClip _mediumHitSfx;

	public AudioClip _heavyHitSfx;
}
