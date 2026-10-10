using UnityEngine;

public class SkillBehavior_lifetap : SkillObjectBehavior
{
	[Header("Life Tap Behavior")]
	public int _manaRecovery;

	private bool _isInitialized;

	public override void End_SkillBehavior()
	{
		Object.Destroy(base.gameObject);
	}

	public override void Update_SkillBehavior()
	{
	}

	public override void Init_SkillBehavior()
	{
		if ((bool)_skillObject._parentPlayer && !_isInitialized)
		{
			StatusEntity statusEntity = _skillObject._parentPlayer._statusEntity;
			_ = _skillObject._parentPlayer._pStats;
			_ = _skillObject._parentPlayer._pCasting;
			_isInitialized = true;
			if (!statusEntity.isServer)
			{
				Invoke("End_SkillBehavior", 2.5f);
				return;
			}
			statusEntity.Network_currentMana = statusEntity._currentMana + _manaRecovery;
			Invoke("End_SkillBehavior", 2.5f);
		}
	}
}
