using UnityEngine;

[CreateAssetMenu(fileName = "weapon_", menuName = "ScriptableObjects/Item/Equipment/Weapon", order = 4)]
public class ScriptableWeapon : ScriptableEquipment
{
	[Header("Weapon Parameters")]
	public ScriptableWeaponType weaponType;

	public ScriptableCombatElement _combatElement;

	public ScriptableWeaponProjectileSet _weaponProjectileSet;

	[Space]
	public int _weaponDamage = 1;

	public int _damageBonus;

	public Vector2Int _readonlyDamageRange = Vector2Int.zero;

	[Header("Weapon Visuals")]
	public Mesh weaponMesh;

	public Texture weaponTexture;

	public ColorAdjustShader_Profile _textureColorAdjust = new ColorAdjustShader_Profile
	{
		_hue = 1f,
		_brightness = 0f,
		_contrast = 1f,
		_saturation = 1f
	};

	public Color[] trailColors = new Color[2]
	{
		Color.white,
		Color.red
	};

	[Header("Weapon Audio")]
	public int _weaponTakeOutSfxIndex;

	public int _weaponSwingSfxIndex;

	[Space]
	public AudioClip _setWeaponTakeOutSfx;

	public AudioClip _setWeaponSwingSfx;

	public override void OnValidate()
	{
		base.OnValidate();
		_equipType = ItemEquipType.WEAPON;
		_itemSortTag = SortTag.GEAR_0;
		if (_weaponDamage <= 0)
		{
			_weaponDamage = 1;
		}
		Set_WeaponTypeExternals();
		void Set_WeaponTypeExternals()
		{
			if ((bool)weaponType)
			{
				if (_damageBonus < 0)
				{
					_damageBonus = 0;
				}
				_readonlyDamageRange.x = _weaponDamage;
				_readonlyDamageRange.y = _weaponDamage + _damageBonus + 3;
				if (_weaponTakeOutSfxIndex < 0)
				{
					_weaponTakeOutSfxIndex = 0;
				}
				if (_weaponSwingSfxIndex < 0)
				{
					_weaponSwingSfxIndex = 0;
				}
				if (_weaponTakeOutSfxIndex > weaponType._takeOutWeaponSfx.Length - 1)
				{
					_weaponTakeOutSfxIndex = weaponType._takeOutWeaponSfx.Length - 1;
				}
				if (_weaponSwingSfxIndex > weaponType._weaponSwingSfx.Length - 1)
				{
					_weaponSwingSfxIndex = weaponType._weaponSwingSfx.Length - 1;
				}
				if ((bool)weaponType._takeOutWeaponSfx[_weaponTakeOutSfxIndex])
				{
					_setWeaponTakeOutSfx = weaponType._takeOutWeaponSfx[_weaponTakeOutSfxIndex];
				}
				else
				{
					_setWeaponTakeOutSfx = null;
				}
				if ((bool)weaponType._weaponSwingSfx[_weaponSwingSfxIndex])
				{
					_setWeaponSwingSfx = weaponType._weaponSwingSfx[_weaponSwingSfxIndex];
				}
				else
				{
					_setWeaponSwingSfx = null;
				}
			}
		}
	}

	public int Get_BaseCeilingDamage()
	{
		return _weaponDamage + _damageBonus + 3;
	}

	public int Scale_FloorDamage(StatStruct _statStruct, DamageType _setDmgType, float _bonusPercentage)
	{
		float num = 0f;
		switch (_setDmgType)
		{
		case DamageType.Strength:
			num = (float)_statStruct._attackPower * GameManager._current._statLogics._scale_weaponFloor;
			break;
		case DamageType.Dexterity:
			num = (float)_statStruct._dexPower * GameManager._current._statLogics._scale_weaponFloor;
			break;
		case DamageType.Mind:
			num = (float)_statStruct._magicPower * GameManager._current._statLogics._scale_weaponFloor;
			break;
		}
		return (int)((float)_weaponDamage * _bonusPercentage + num);
	}

	public int Scale_CeilingDamage(StatStruct _statStruct, DamageType _setDmgType, float _bonusPercentage)
	{
		float num = 0f;
		switch (_setDmgType)
		{
		case DamageType.Strength:
			num = (float)_statStruct._attackPower * GameManager._current._statLogics._scale_weaponCeiling;
			break;
		case DamageType.Dexterity:
			num = (float)_statStruct._dexPower * GameManager._current._statLogics._scale_weaponCeiling;
			break;
		case DamageType.Mind:
			num = (float)_statStruct._magicPower * GameManager._current._statLogics._scale_weaponCeiling;
			break;
		}
		return (int)((float)Get_BaseCeilingDamage() * _bonusPercentage + num);
	}
}
