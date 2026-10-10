using UnityEngine;

[CreateAssetMenu(fileName = "ring_", menuName = "ScriptableObjects/Item/Equipment/Armor/Ring", order = 4)]
public class ScriptableRing : ScriptableArmor
{
	public override void OnValidate()
	{
		base.OnValidate();
		_equipType = ItemEquipType.ARMOR;
		_itemSortTag = SortTag.GEAR_6;
		_canDyeArmor = false;
	}
}
