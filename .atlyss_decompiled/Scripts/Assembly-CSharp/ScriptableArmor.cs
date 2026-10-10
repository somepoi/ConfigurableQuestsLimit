using UnityEngine;

public class ScriptableArmor : ScriptableEquipment
{
	[Header("Scriptable Armor")]
	public Texture _armorTexture;

	public ColorAdjustShader_Profile _colorAdjustParams = new ColorAdjustShader_Profile
	{
		_hue = 0f,
		_saturation = 1f,
		_brightness = 0f,
		_contrast = 1f
	};

	public bool _canDyeArmor = true;

	public override void OnValidate()
	{
		base.OnValidate();
		_itemType = ItemType.GEAR;
	}
}
