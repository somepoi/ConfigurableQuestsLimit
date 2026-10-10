using UnityEngine;

public class SkillBehavior_bulwark : SkillObjectBehavior
{
	private StatusEntity _statusEntity;

	private PlayerCombat _pCombat;

	[Header("Bulwark Skill Object")]
	[SerializeField]
	private GameObject _bulwarkEffect;

	[SerializeField]
	[Range(0f, 1f)]
	private float _staminaPercentDecrease = 0.03f;

	[SerializeField]
	[Range(0f, 1f)]
	private float _ignoreStaminaBlockCostChance = 0.3f;

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
			if (!_statusEntity)
			{
				_statusEntity = _skillObject._parentPlayer._statusEntity;
			}
			if (!_pCombat)
			{
				_pCombat = _skillObject._parentPlayer._pCombat;
			}
			_bulwarkEffect.SetActive(_pCombat._isBlocking);
			base.transform.localPosition = Vector3.zero;
			if (_skillObject._parentPlayer.isServer)
			{
				_statusEntity._staminaUseOnBlockPercent = 1f - _staminaPercentDecrease;
				_statusEntity._chanceToIgnoreBlockStaminaCost = _ignoreStaminaBlockCostChance;
				_statusEntity._dampenKnockback = true;
			}
		}
	}

	private void OnDestroy()
	{
		if ((bool)_skillObject._parentPlayer)
		{
			_skillObject._parentPlayer._statusEntity._staminaUseOnBlockPercent = 1f;
			_skillObject._parentPlayer._statusEntity._chanceToIgnoreBlockStaminaCost = 0f;
			_statusEntity._dampenKnockback = false;
		}
	}
}
