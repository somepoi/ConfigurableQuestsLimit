using UnityEngine;

[CreateAssetMenu(fileName = "dyeConsumable_", menuName = "ScriptableObjects/Item/Consumables/Armor Dye", order = 1)]
public class ScriptableArmorDye : ScriptableConsumable
{
	[Header("Scriptable Armor Dye")]
	public ColorAdjustShader_Profile _dyeParams = new ColorAdjustShader_Profile
	{
		_hue = 0f,
		_saturation = 1f,
		_brightness = 0f,
		_contrast = 1f
	};

	public Color _particleColoration = Color.white;

	public override bool CanUseConsumable(StatusEntity _statusEntity, bool _isButtonPrompt)
	{
		PlayerAppearanceStruct playerAppearanceStruct = _statusEntity._isPlayer._pVisual._playerAppearanceStruct;
		for (int i = 0; i < GameManager._current._statLogics._armorDyes.Length; i++)
		{
			if (GameManager._current._statLogics._armorDyes[i] == this && ((playerAppearanceStruct._dyeIndex == i) & _isButtonPrompt))
			{
				ErrorPromptTextManager.current.Init_ErrorPrompt("Already Using This Dye");
				return false;
			}
		}
		return true;
	}

	public override void Use_Consumable(StatusEntity _statusEntity)
	{
		if (!_statusEntity.isServer)
		{
			return;
		}
		PlayerVisual pVisual = _statusEntity._isPlayer._pVisual;
		PlayerAppearanceStruct playerAppearanceStruct = pVisual._playerAppearanceStruct;
		for (int i = 0; i < GameManager._current._statLogics._armorDyes.Length; i++)
		{
			if (GameManager._current._statLogics._armorDyes[i] == this)
			{
				playerAppearanceStruct._dyeIndex = i;
				break;
			}
		}
		pVisual.Network_playerAppearanceStruct = playerAppearanceStruct;
	}

	public override void OnValidate()
	{
		base.OnValidate();
		_itemSortTag = SortTag.DYE;
	}
}
