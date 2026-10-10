using UnityEngine;

[CreateAssetMenu(fileName = "leggings_", menuName = "ScriptableObjects/Item/Equipment/Armor/Leggings", order = 2)]
public class ScriptableLeggings : ScriptableArmor
{
	[Header("Leggings Params")]
	public ScriptableArmorRender _legPieceRender_01;

	public Texture _legPieceTexture_01;

	[Space]
	public ScriptableArmorRender _legPieceRender_02;

	public Texture _legPieceTexture_02;

	[Space]
	public ScriptableArmorRender _legPieceRender_03;

	public Texture _legPieceTexture_03;

	[Space]
	public ScriptableArmorRender _legPieceRender_04;

	public Texture _legPieceTexture_04;

	[Space]
	public bool _textureUpperLegOnly;

	public bool _textureFeet = true;

	public override void OnValidate()
	{
		base.OnValidate();
		_equipType = ItemEquipType.ARMOR;
		_itemSortTag = SortTag.GEAR_4;
		if (_legPieceRender_01 == null || !_legPieceTexture_01 || !_legPieceTexture_01)
		{
			_legPieceTexture_01 = _armorTexture;
		}
		if (_legPieceRender_02 == null || !_legPieceTexture_02 || !_legPieceTexture_02)
		{
			_legPieceTexture_02 = _armorTexture;
		}
		if (_legPieceRender_03 == null || !_legPieceTexture_03 || !_legPieceTexture_03)
		{
			_legPieceTexture_03 = _armorTexture;
		}
		if (_legPieceRender_04 == null || !_legPieceTexture_04 || !_legPieceTexture_04)
		{
			_legPieceTexture_04 = _armorTexture;
		}
	}
}
