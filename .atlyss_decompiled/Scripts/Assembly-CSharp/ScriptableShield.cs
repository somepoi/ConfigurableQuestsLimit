using UnityEngine;

[CreateAssetMenu(fileName = "shield_", menuName = "ScriptableObjects/Item/Equipment/Shield", order = 5)]
public class ScriptableShield : ScriptableEquipment
{
	[Header("Shield Visuals")]
	public Mesh _shieldMesh;

	public Texture _shieldTexture;

	public ColorAdjustShader_Profile _textureColorAdjust = new ColorAdjustShader_Profile
	{
		_hue = 1f,
		_brightness = 0f,
		_contrast = 1f,
		_saturation = 1f
	};

	public override void OnValidate()
	{
		base.OnValidate();
		_equipType = ItemEquipType.OFFHAND;
		_itemSortTag = SortTag.GEAR_5;
	}
}
