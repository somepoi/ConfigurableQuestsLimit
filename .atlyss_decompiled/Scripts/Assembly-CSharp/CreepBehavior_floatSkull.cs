using System.Collections;
using UnityEngine;

public class CreepBehavior_floatSkull : CreepBehavior
{
	[Header("Float Skull Behavior")]
	[SerializeField]
	private float _timeBeforeChargeAttack = 0.6f;

	[SerializeField]
	private float _chargeAttackCooldown = 1.25f;

	[SerializeField]
	private float _nonBubbleRandomVelocity = 0.35f;

	[SerializeField]
	private Color _floatSkullNonBubbleTint = Color.white;

	[SerializeField]
	private ParticleSystem _deathParticleEffect;

	private bool _isChargingAtTarget;

	[Header("Bubble Condition")]
	[SerializeField]
	private ParticleSystem _bubbleParticleEffect;

	[SerializeField]
	private ParticleSystem _chargeBubbleParticleEffect;

	[SerializeField]
	private float _regainBubbleBuffer = 3.5f;

	[SerializeField]
	[Range(0f, 1f)]
	private float _chanceToLooseBubble = 0.1f;

	[SerializeField]
	private float _chargeBubbleBufferSet = 2.5f;

	private bool _bubbleDisabled;

	private float _currentBubbleBuffer;

	private float _chargeBubbleBuffer;

	public override void Behavior_OnServerFixedUpdate()
	{
	}

	public override void Behavior_OnHurt()
	{
		if (_baseCreep.isServer && Random.Range(0f, 1f) <= _chanceToLooseBubble && !_bubbleDisabled)
		{
			Deactivate_Bubble();
			StopAllCoroutines();
			StartCoroutine(RandomizeDirectionsInterval());
		}
	}

	public override void Server_Behavior_OnSpawn()
	{
		base.Server_Behavior_OnSpawn();
		Reset_Behavior();
		Invoke("Activate_Bubble", 0.085f);
		_baseCreep.Server_CrossFadeAnim("Idle", 0f, 0);
	}

	public override void Behavior_OnServerUpdate()
	{
		if (_bubbleDisabled)
		{
			Handle_DisabledBubbleBehavior();
		}
		else
		{
			Handle_BubbleBehavior();
		}
		if (_chargeBubbleBuffer > 0f)
		{
			_chargeBubbleBuffer -= Time.deltaTime;
		}
		else
		{
			_chargeBubbleBuffer = 0f;
		}
	}

	private void Handle_DisabledBubbleBehavior()
	{
		if (_currentBubbleBuffer < _regainBubbleBuffer)
		{
			_currentBubbleBuffer += Time.deltaTime;
		}
		else
		{
			Activate_Bubble();
		}
		_chargeBubbleParticleEffect.Stop();
		_baseCreep._pathFindCtrl._disablePathing = true;
		_isChargingAtTarget = false;
		if (_isChargingAtTarget)
		{
			_baseCreep._attackActionBuffer = 0.85f;
		}
	}

	private void Handle_BubbleBehavior()
	{
		if (_baseCreep.isServer && _baseCreep._pathFindCtrl._isStoppedAndNearTarget && (bool)_baseCreep.Network_aggroedEntity && !_onAggroedEntityBuffer && !_isChargingAtTarget)
		{
			StartCoroutine(ChargeAtTarget());
		}
	}

	private IEnumerator ChargeAtTarget()
	{
		if (_baseCreep.isServer && !_isChargingAtTarget && !(_chargeBubbleBuffer > 0f) && !_onAggroedEntityBuffer)
		{
			_isChargingAtTarget = true;
			yield return new WaitForSeconds(_timeBeforeChargeAttack);
			_baseCreep._creepPushCollider.gameObject.SetActive(value: false);
			_baseCreep._pathFindCtrl._disablePathing = true;
			_baseCreep.Server_CrossFadeAnim("Attack01", 0f, 1);
			yield return new WaitForSeconds(1.25f);
			_baseCreep._creepPushCollider.gameObject.SetActive(value: true);
			_baseCreep._pathFindCtrl._disablePathing = false;
			_chargeBubbleParticleEffect.Stop();
			yield return new WaitForSeconds(_chargeAttackCooldown);
			_isChargingAtTarget = false;
			_chargeBubbleBuffer = _chargeBubbleBufferSet;
		}
	}

	private IEnumerator RandomizeDirectionsInterval()
	{
		if (!_baseCreep.isServer)
		{
			yield break;
		}
		_baseCreep._pathFindCtrl.Network_dontLookAtTarget = true;
		_baseCreep.Server_CrossFadeAnim("alt_action", 0f, 1);
		do
		{
			if (_baseCreep._currentCreepCondition != CreepCondition.ACTIVE || _statusEntity._currentHealth <= 0)
			{
				_baseCreep.Server_CrossFadeAnim("Empty", 0f, 1);
				yield break;
			}
			yield return new WaitForSeconds(0.65f);
			_baseCreep.Server_CrossFadeAnim("alt_action", 0f, 1);
			_baseCreep.transform.rotation = Quaternion.Euler(0f, Random.Range(-360f, 360f), 0f);
			Vector3 velocity = _baseCreep.transform.forward * _nonBubbleRandomVelocity;
			velocity.y = _baseCreep._creepRB.velocity.y;
			_baseCreep._creepRB.velocity = velocity;
			_baseCreep.Rpc_TintRender(_floatSkullNonBubbleTint);
			yield return new WaitForSeconds(0.1f);
		}
		while (_bubbleDisabled);
		_baseCreep.Server_CrossFadeAnim("Empty", 0f, 1);
		Activate_Bubble();
	}

	public override void Reset_Behavior()
	{
		if (_baseCreep.isServer && !_bubbleDisabled)
		{
			StopAllCoroutines();
			_baseCreep.Server_CrossFadeAnim("Empty", 0f, 1);
			_chargeBubbleParticleEffect.Stop();
			_baseCreep._creepPushCollider.gameObject.SetActive(value: true);
			_isChargingAtTarget = false;
			_chargeBubbleBuffer = _chargeBubbleBufferSet;
			_baseCreep._pathFindCtrl._disablePathing = false;
		}
	}

	private void Activate_Bubble()
	{
		if (_baseCreep.isServer)
		{
			_bubbleDisabled = false;
			_currentBubbleBuffer = 0f;
			_baseCreep._pathFindCtrl.Network_dontLookAtTarget = false;
			_baseCreep._pathFindCtrl._disablePathing = false;
		}
	}

	private void Deactivate_Bubble()
	{
		if (_baseCreep.isServer)
		{
			_bubbleDisabled = true;
			_chargeBubbleParticleEffect.Stop();
		}
	}

	public void Play_BubbleAttackEffect()
	{
		_chargeBubbleParticleEffect.Play();
	}

	public void BubbleEffect_Play()
	{
		_bubbleParticleEffect.Play();
	}

	public void BubbleEffect_Stop()
	{
		_bubbleParticleEffect.Stop();
	}

	public void Play_DeathEffect()
	{
		_bubbleParticleEffect.Stop();
		_deathParticleEffect.Play();
	}
}
