using UnityEngine;

public class SkillBehavior_Reflect : SkillObjectBehavior
{
	[Header("Reflect Skill")]
	[SerializeField]
	private GameObject _reflectEffect;

	public float _reflectPercentage = 0.4f;

	[SerializeField]
	[Range(0f, 3f)]
	private float _parryStaggerModifier;

	[SerializeField]
	private float _parryTimeWindowModifier;

	private void OnDestroy()
	{
		StatusEntity statusEntity = _skillObject._parentPlayer._statusEntity;
		PlayerCombat pCombat = _skillObject._parentPlayer._pCombat;
		if ((bool)_skillObject._parentPlayer && _skillObject._parentPlayer.isServer)
		{
			statusEntity._reflectPercent = 0f;
			pCombat._parryChanceModifier = 0f;
		}
	}

	public override void End_SkillBehavior()
	{
	}

	public override void Init_SkillBehavior()
	{
	}

	public override void Update_SkillBehavior()
	{
		if ((bool)_skillObject._parentPlayer)
		{
			StatusEntity statusEntity = _skillObject._parentPlayer._statusEntity;
			PlayerCombat pCombat = _skillObject._parentPlayer._pCombat;
			_reflectEffect.SetActive(pCombat._isBlocking && pCombat._inParryWindow);
			base.transform.localPosition = Vector3.zero;
			if (_skillObject._parentPlayer.isServer)
			{
				bool network_reflectDamage = (pCombat._parryChanceBuffer > 0f && pCombat._isBlocking) || statusEntity._autoParry;
				pCombat._parryChanceModifier = _parryTimeWindowModifier;
				statusEntity._parryBufferApply = GameManager._current._statLogics._baseParryApplyBuffer + _parryStaggerModifier;
				statusEntity.Network_reflectDamage = network_reflectDamage;
				statusEntity._reflectPercent = _reflectPercentage;
			}
		}
	}
}
