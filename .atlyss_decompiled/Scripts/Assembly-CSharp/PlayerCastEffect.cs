using UnityEngine;

public class PlayerCastEffect : MonoBehaviour
{
	private Player _player;

	private ScriptableSkill _currentSkill;

	[Header("Spell Charging")]
	[SerializeField]
	private Light _groundGlyphObject;

	[SerializeField]
	private RotateObject _groundGlyphRotate;

	[SerializeField]
	private ParticleSystem _interruptCastEffect;

	[SerializeField]
	private AudioSource _chargeAsrc;

	private GameObject _leftSpellCharge;

	private GameObject _rightSpellCharge;

	private ParticleSystem _leftChargeEffect;

	private ParticleSystem _rightChargeEffect;

	private bool _fadeChannelASrc;

	private bool _isOff = true;

	[Header("Spell Casting")]
	[SerializeField]
	private AudioSource _castAsrc;

	[SerializeField]
	private AudioSource _castInterruptAsrc;

	private GameObject _leftSpellCast;

	private GameObject _rightSpellCast;

	private void Awake()
	{
		_player = GetComponentInParent<Player>();
	}

	private void Start()
	{
		_groundGlyphObject.intensity = 0f;
		_groundGlyphObject.enabled = false;
		_groundGlyphRotate.enabled = false;
		_chargeAsrc.volume = 0f;
		_fadeChannelASrc = true;
		_isOff = true;
	}

	private void Update()
	{
		Handle_CastEffectConditions();
	}

	private void Handle_CastEffectConditions()
	{
		if (_player.isServerOnly || !Player._mainPlayer)
		{
			return;
		}
		if (!_fadeChannelASrc)
		{
			_chargeAsrc.volume = Mathf.Lerp(_chargeAsrc.volume, 0.55f, 2.5f * Time.deltaTime);
		}
		else
		{
			_chargeAsrc.volume = Mathf.Lerp(_chargeAsrc.volume, 0f, 2.5f * Time.deltaTime);
		}
		PlayerRaceModel playerRaceModel = _player._pVisual._playerRaceModel;
		if (!playerRaceModel)
		{
			return;
		}
		if ((bool)_leftSpellCast)
		{
			_leftSpellCast.transform.position = playerRaceModel._leftHandBone.position;
		}
		if ((bool)_rightSpellCast)
		{
			_rightSpellCast.transform.position = playerRaceModel._rightHandBone.position;
		}
		if (_player.Network_playerMapInstance == null || _player._bufferingStatus || _player._currentPlayerCondition != PlayerCondition.ACTIVE || Player._mainPlayer.Network_playerMapInstance != _player.Network_playerMapInstance)
		{
			if ((bool)_leftChargeEffect)
			{
				_leftChargeEffect.Stop();
			}
			if ((bool)_rightChargeEffect)
			{
				_rightChargeEffect.Stop();
			}
			_groundGlyphObject.intensity = 0f;
			_groundGlyphObject.enabled = false;
			_groundGlyphRotate.enabled = false;
			_fadeChannelASrc = true;
			_isOff = true;
		}
		else if (_isOff)
		{
			if ((bool)_leftChargeEffect)
			{
				_leftChargeEffect.Stop();
			}
			if ((bool)_rightChargeEffect)
			{
				_rightChargeEffect.Stop();
			}
			_groundGlyphObject.intensity = Mathf.Lerp(_groundGlyphObject.intensity, 0f, 28.5f * Time.deltaTime);
			if (_groundGlyphObject.intensity <= 0.1f)
			{
				_groundGlyphObject.enabled = false;
			}
			_groundGlyphRotate.enabled = false;
		}
		else
		{
			CastEffectCollection castEffectCollection = _currentSkill._castEffectCollection;
			if ((bool)castEffectCollection._channelGroundGlyphTexture && _currentSkill._useGlyphEffect)
			{
				_groundGlyphObject.enabled = true;
				_groundGlyphObject.spotAngle = Mathf.Lerp(_groundGlyphObject.spotAngle, castEffectCollection._groundGlyphSpotAngleSize, 14.5f * Time.deltaTime);
				_groundGlyphObject.intensity = Mathf.Lerp(_groundGlyphObject.intensity, castEffectCollection._groundGlyphIntensity, 4.5f * Time.deltaTime);
			}
			_leftSpellCharge.transform.position = playerRaceModel._leftHandBone.position;
			_rightSpellCharge.transform.position = playerRaceModel._rightHandBone.position;
			_groundGlyphRotate.enabled = true;
		}
	}

	public void Play_InterruptEffect()
	{
		if (!_player.isServerOnly)
		{
			_interruptCastEffect.Play();
			_castInterruptAsrc.pitch = Random.Range(1f, 1.105f);
			if (_castInterruptAsrc.isActiveAndEnabled)
			{
				_castInterruptAsrc.Play();
			}
			_groundGlyphRotate.enabled = false;
		}
	}

	public void Play_ChannelEffect(ScriptableSkill _skill)
	{
		if (_player.isServerOnly || !_skill._castEffectCollection)
		{
			return;
		}
		CastEffectCollection castEffectCollection = _skill._castEffectCollection;
		_currentSkill = _skill;
		_isOff = false;
		if ((bool)castEffectCollection._channelGroundGlyphTexture && _currentSkill._useGlyphEffect)
		{
			_groundGlyphObject.spotAngle = 179f;
			_groundGlyphRotate.Alter_RotationValues(new Vector3(0f, 0f, castEffectCollection._channelGroundGlyphRotationSpeed));
			_groundGlyphObject.cookie = castEffectCollection._channelGroundGlyphTexture;
			_groundGlyphObject.color = castEffectCollection._groundGlyphColor;
		}
		if ((bool)_leftChargeEffect)
		{
			_leftChargeEffect.Stop();
		}
		if ((bool)_rightChargeEffect)
		{
			_rightChargeEffect.Stop();
		}
		_leftSpellCharge = Object.Instantiate(_skill._castEffectCollection._chargeSkillEffect, base.transform.position, Quaternion.identity, base.transform);
		_rightSpellCharge = Object.Instantiate(_skill._castEffectCollection._chargeSkillEffect, base.transform.position, Quaternion.identity, base.transform);
		_leftChargeEffect = _leftSpellCharge.GetComponent<ParticleSystem>();
		_rightChargeEffect = _rightSpellCharge.GetComponent<ParticleSystem>();
		if ((bool)castEffectCollection._channelSound)
		{
			_chargeAsrc.clip = castEffectCollection._channelSound;
			_chargeAsrc.pitch = Random.Range(castEffectCollection._channelSoundPitch, castEffectCollection._channelSoundPitch + 0.0105f);
			if (_chargeAsrc.isActiveAndEnabled)
			{
				_chargeAsrc.Play();
			}
			_fadeChannelASrc = false;
		}
	}

	public void Stop_ChannelEffect()
	{
		if (!_player.isServerOnly)
		{
			if ((bool)_leftChargeEffect)
			{
				_leftChargeEffect.Stop();
			}
			if ((bool)_rightChargeEffect)
			{
				_rightChargeEffect.Stop();
			}
			_currentSkill = null;
			_fadeChannelASrc = true;
			_isOff = true;
		}
	}

	public void Play_CastEffect()
	{
		if (_player.isServerOnly)
		{
			return;
		}
		if ((bool)_leftChargeEffect)
		{
			_leftChargeEffect.Stop();
		}
		if ((bool)_rightChargeEffect)
		{
			_rightChargeEffect.Stop();
		}
		if (!_currentSkill || !_currentSkill._castEffectCollection || Player._mainPlayer.Network_playerMapInstance != _player.Network_playerMapInstance)
		{
			return;
		}
		CastEffectCollection castEffectCollection = _currentSkill._castEffectCollection;
		_leftSpellCast = Object.Instantiate(castEffectCollection._castSkillEffect, base.transform.position, Quaternion.identity, base.transform);
		_rightSpellCast = Object.Instantiate(castEffectCollection._castSkillEffect, base.transform.position, Quaternion.identity, base.transform);
		if ((bool)castEffectCollection._castSound)
		{
			_castAsrc.pitch = Random.Range(castEffectCollection._castSoundPitch, castEffectCollection._castSoundPitch + 0.0105f);
			if (_castAsrc.enabled)
			{
				_castAsrc.PlayOneShot(castEffectCollection._castSound);
			}
		}
		Stop_ChannelEffect();
	}
}
