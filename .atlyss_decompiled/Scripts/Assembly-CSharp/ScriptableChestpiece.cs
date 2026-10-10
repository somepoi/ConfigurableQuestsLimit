using UnityEngine;

[CreateAssetMenu(fileName = "chestPiece_", menuName = "ScriptableObjects/Item/Equipment/Armor/Chestpiece", order = 1)]
public class ScriptableChestpiece : ScriptableArmor
{
	public enum ShoulderPadDisplayType : byte
	{
		None = 0,
		Both = 1,
		Left = 2,
		Right = 3
	}

	[Header("Chestpiece Params")]
	public Texture _legOverlayTexture;

	public Texture _boobOverrideTexture;

	public bool _lockBoobs;

	public bool _disableJiggleBoobBones;

	[Header("Collar Display")]
	public Mesh _neckCollarMesh;

	public Texture _neckCollarTexture;

	[Header("Chestpiece Render Display")]
	public ScriptableArmorRender _chestRenderDisplay;

	public Texture _chestRenderTexture;

	public Texture _chestRenderBoobTexture;

	[Header("Robeskirt Display")]
	public ScriptableArmorRender _robeSkirtRender;

	public Texture _robeSkirtTexture;

	[Header("Hip Mesh Display")]
	public ScriptableArmorRender _hipMeshDisplay;

	public Texture _hipMeshTexture;

	[Header("Arms Display")]
	public ScriptableArmorRender _armCuffRender;

	public bool _textureArms = true;

	[Header("Shoulderpad Display")]
	public ShoulderPadDisplayType _shoulderPadDisplayType;

	public Mesh _shoulderpadMesh;

	public Texture _shoulderpadTexture;

	public override void OnValidate()
	{
		base.OnValidate();
		_equipType = ItemEquipType.ARMOR;
		_itemSortTag = SortTag.GEAR_3;
		if (!_neckCollarMesh)
		{
			_neckCollarTexture = null;
		}
		if (!_chestRenderDisplay)
		{
			_chestRenderTexture = null;
		}
		if (!_robeSkirtRender)
		{
			_robeSkirtTexture = null;
		}
		if (!_shoulderpadMesh)
		{
			_shoulderpadTexture = null;
		}
	}
}
