using UnityEngine;

[CreateAssetMenu(fileName = "conditionGroup_", menuName = "ScriptableObjects/Buffs and Conditions/Cache/Condition Group", order = 0)]
public class ScriptableConditionGroup : ScriptableObject
{
	[Header("Condition Group")]
	public string _conditionGroupTag = "New Condition Group";

	public int _conditionGroupID;

	[Space]
	public Sprite _conditionGroupIcon;

	public Color _floatValueColor = Color.white;

	public ConditionType _conditionType;
}
