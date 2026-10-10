using UnityEngine;

[CreateAssetMenu(fileName = "statModifierTable_", menuName = "ScriptableObjects/Item/Stat Modifier Table", order = 6)]
public class ScriptableStatModifierTable : ScriptableObject
{
	public StatModifierSlot[] _statModifierSlots;
}
