using UnityEngine;

public class SkillBehavior_manaflow : SkillObjectBehavior
{
	private StatusEntity _statusEntity;

	[Header("Manaflow Skill Object")]
	[SerializeField]
	[Range(0f, 0.8f)]
	private float _manaRegenerationAdjust = 0.05f;

	private void OnDestroy()
	{
		if ((bool)_skillObject._parentPlayer && _skillObject._parentPlayer.isServer)
		{
			_statusEntity.Network_manaRegenRate = GameManager._current._statLogics._baseManaRegenRate;
		}
	}

	public override void Init_SkillBehavior()
	{
	}

	public override void End_SkillBehavior()
	{
	}

	public override void Update_SkillBehavior()
	{
		if ((bool)_skillObject._parentPlayer && _skillObject._parentPlayer.isServer)
		{
			_statusEntity = _skillObject._parentPlayer._statusEntity;
			_statusEntity.Network_manaRegenRate = GameManager._current._statLogics._baseManaRegenRate - _manaRegenerationAdjust;
		}
	}
}
