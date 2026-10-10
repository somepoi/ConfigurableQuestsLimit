using UnityEngine;

[CreateAssetMenu(fileName = "helm_", menuName = "ScriptableObjects/Item/Equipment/Armor/Helm", order = 0)]
public class ScriptableHelm : ScriptableArmor
{
	[Header("Helm Params")]
	public ScriptableArmorRender _helmRender;

	public Mesh _helmOverrideMesh;

	[Range(0f, 100f)]
	public float _blendHelmValue;

	public bool _useHelmHairDisplay;

	public bool _hideEars;

	public bool _hideMisc;

	public bool _isFullHelm;

	public override void OnValidate()
	{
		base.OnValidate();
		_equipType = ItemEquipType.ARMOR;
		_itemSortTag = SortTag.GEAR_1;
	}
}
