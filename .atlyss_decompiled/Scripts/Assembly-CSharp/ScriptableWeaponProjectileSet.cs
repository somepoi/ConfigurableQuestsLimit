using UnityEngine;

[CreateAssetMenu(fileName = "weaponProjectileSet_", menuName = "ScriptableObjects/Item/Equipment/WeaponProjectileSet", order = 5)]
public class ScriptableWeaponProjectileSet : ScriptableObject
{
	[Header("Weapon Projectile Set")]
	public GameObject[] _projectileObjects;
}
