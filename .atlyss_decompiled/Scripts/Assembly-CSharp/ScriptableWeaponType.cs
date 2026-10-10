using UnityEngine;

[CreateAssetMenu(fileName = "weaponType_", menuName = "ScriptableObjects/Game Logic/Weapon Type", order = 0)]
public class ScriptableWeaponType : ScriptableObject
{
	[Header("Scriptable Weapon Type")]
	public string _weaponTypeClassTag = "New Weapon Class";

	public DamageType _combatType;

	public DamageWeight _damageWeight = DamageWeight.Medium;

	public WeaponHandedness _weaponHandedness;

	public WeaponAnimationSlot _weaponData;

	[Header("Weapon Mesh Display")]
	public WeaponRenderDisplaymode _weaponDisplayMode;

	public int _backWeaponIndex;

	[Tooltip("Effectively hides the weapon mesh if the player does not have the weapon taken out.")]
	public bool _hidePutAwayWeapon;

	[Tooltip("Put thine boobs together on the character if they hold the weapon. (Bows, Polearms, etc).")]
	public bool _lockBoobOnTakeOut;

	[Space]
	public Vector3 _weaponChargeEffectPosition = Vector3.zero;

	public Vector3 _weaponTrailBasePosition = Vector3.zero;

	public Vector3 _weaponTrailEndPosition = Vector3.zero;

	[Header("Weapon Type Audio")]
	public AudioClip[] _takeOutWeaponSfx = new AudioClip[1];

	public AudioClip[] _chargeWeaponSfx = new AudioClip[1];

	public AudioClip[] _weaponSwingSfx = new AudioClip[1];
}
