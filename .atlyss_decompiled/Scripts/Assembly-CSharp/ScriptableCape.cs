using UnityEngine;

[CreateAssetMenu(fileName = "cape_", menuName = "ScriptableObjects/Item/Equipment/Armor/Cape", order = 3)]
public class ScriptableCape : ScriptableArmor
{
	[Header("Cape Params")]
	public Mesh _capeMesh;

	public override void OnValidate()
	{
		base.OnValidate();
		_equipType = ItemEquipType.ARMOR;
		_itemSortTag = SortTag.GEAR_2;
		_canDyeArmor = false;
	}
}
