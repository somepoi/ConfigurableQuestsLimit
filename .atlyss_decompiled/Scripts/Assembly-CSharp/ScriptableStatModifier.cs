using UnityEngine;

[CreateAssetMenu(fileName = "statModifier_", menuName = "ScriptableObjects/Item/Equipment/StatModifier", order = 0)]
public class ScriptableStatModifier : ScriptableObject
{
	[Header("Stat Modifier")]
	public string _modifierTag = "New Modifier";

	public int _modifierID;

	[Space]
	public ScriptableCombatElement _modifierElement;

	public Color _glowTintColor = Color.white;

	[Space]
	public StatStruct _modifierStatStruct;

	[Range(0f, 1f)]
	public float _modifierPercentage = 0.45f;

	[Header("Creep Stat Modifier Params")]
	public Texture _modifierEmblemTexture;

	public float _creepScaleAdd;

	public bool _useCreepGroundGlyph;

	[Range(0f, 1f)]
	public float _creepMaxHealthBoostPercentage = 0.08f;

	[Range(0f, 1f)]
	public float _creepStatModifierPercentage = 0.45f;

	[Range(0f, 1f)]
	public float _experiencePercentage = 0.25f;

	[Range(0f, 5f)]
	public float _currencyDropPercentage = 1.5f;

	[Space]
	public DamageType _damageType;

	public bool _overrideDamageType;
}
