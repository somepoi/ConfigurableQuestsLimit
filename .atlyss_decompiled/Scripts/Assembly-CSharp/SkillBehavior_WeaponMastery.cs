using UnityEngine;

public class SkillBehavior_WeaponMastery : SkillObjectBehavior
{
	[Header("Weapon Mastery Passive Behavior")]
	public ScriptableWeaponType[] _setWeaponTypes;

	public int _setParamIndex;

	public float _damageBonusIncrease;

	public override void End_SkillBehavior()
	{
	}

	public override void Init_SkillBehavior()
	{
	}

	public override void Update_SkillBehavior()
	{
	}
}
