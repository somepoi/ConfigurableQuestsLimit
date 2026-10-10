using TMPro;
using UnityEngine;

public class FloatingValue : MonoBehaviour
{
	[Header("Float Value Parameters")]
	public int _value;

	public float _returnToPoolTime;

	public float _displayTimer;

	private float _floatTextGravity;

	[Space]
	[SerializeField]
	private float _returnToPoolTimeSet = 1.25f;

	[SerializeField]
	private float _displayTimerSet = 0.45f;

	public float _speedTimeSet = 0.35f;

	public float _standardFontSize = 9f;

	public float _missFontSize = 3.5f;

	[Space]
	public ScriptableCombatElement _scriptableCombatElement;

	[Space]
	public ScriptableConditionGroup _conditionGroup;

	public float _conditionFontSize = 6f;

	[Header("Transforms")]
	[SerializeField]
	private Transform _setNumberContainer;

	public Transform _setFollowObject;

	public LerpScale _lerpScaler;

	[Header("Speed Values")]
	public float _floatTextSpeed = 4.5f;

	public float _speedTime;

	[Header("Float Value Display")]
	public TextMeshPro _textMesh;

	public SpriteRenderer _floatValueSpriteRender;

	public SpriteRenderer _floatValueCritSpriteRender;

	public Sprite _defenseShieldIco;

	public Sprite _magicDefenseShieldIco;

	public Sprite _parrySparkleIco;

	public Sprite _shatterIco;

	public Sprite _healIco;

	public Sprite _manaIco;

	public TMP_ColorGradient[] _floatGradients = new TMP_ColorGradient[3];

	[Header("Sine Rotation")]
	[SerializeField]
	private float _sineRotationRadius = 2.5f;

	[SerializeField]
	private float _sineOriginRadius = 0.45f;

	[SerializeField]
	private float _sineRotationSpeed = 0.95f;

	private float _currentSineRotationSpeed;

	[Header("Audio")]
	[SerializeField]
	private AudioSource _aSrc_expPickup;

	public int _dirNum = 1;

	[SerializeField]
	private float _upwardsSpeed = 15.65f;

	private bool _playExpGainSound;

	private void Awake()
	{
		_lerpScaler = GetComponentInChildren<LerpScale>();
		_textMesh = GetComponentInChildren<TextMeshPro>();
	}

	private void OnEnable()
	{
		Reset_FloatValueParams();
	}

	public void Reset_FloatValueParams()
	{
		_speedTime = _speedTimeSet;
		_floatTextGravity = 0f;
		_playExpGainSound = false;
		_returnToPoolTime = _returnToPoolTimeSet;
		_displayTimer = _displayTimerSet;
		_setNumberContainer.transform.localPosition = Vector3.one;
		_currentSineRotationSpeed = _sineRotationSpeed;
		_lerpScaler.setScale = Vector3.one * 4.15f;
		_sineRotationRadius = _sineOriginRadius;
		_sineRotationRadius *= Random.Range(_dirNum, 1.75f * (float)_dirNum);
	}

	private void Update()
	{
		Handle_ValueDisplay();
		Handle_SineRotation();
		Handle_FollowObject();
		Handle_FloatValueTimers();
	}

	private void Handle_ValueDisplay()
	{
		_floatValueSpriteRender.enabled = ((bool)_conditionGroup && (bool)_conditionGroup._conditionGroupIcon && _value > 0) || ((bool)_scriptableCombatElement && (bool)_scriptableCombatElement._floatValueIcon);
		_textMesh.fontSize = _standardFontSize;
		_textMesh.enableVertexGradient = true;
		_textMesh.color = Color.white;
		if ((bool)_scriptableCombatElement && (bool)_scriptableCombatElement._floatValueIcon)
		{
			_floatValueSpriteRender.sprite = _scriptableCombatElement._floatValueIcon;
			_textMesh.enableVertexGradient = false;
			_textMesh.color = _scriptableCombatElement._floatValueColor;
		}
		if ((bool)_conditionGroup && (bool)_conditionGroup._conditionGroupIcon && _value > 0)
		{
			_floatValueSpriteRender.sprite = _conditionGroup._conditionGroupIcon;
			_textMesh.fontSize = _conditionFontSize;
			_textMesh.enableVertexGradient = false;
			_textMesh.color = _conditionGroup._floatValueColor;
		}
		if (_textMesh.colorGradientPreset == _floatGradients[3])
		{
			_floatValueCritSpriteRender.enabled = true;
			_textMesh.enableVertexGradient = true;
			_textMesh.color = Color.white;
		}
		else
		{
			_floatValueCritSpriteRender.enabled = false;
		}
		_textMesh.text = $"{_value}";
		if (SettingsManager._current._hideDamageValueIcons.isOn)
		{
			_floatValueSpriteRender.enabled = false;
		}
		switch (_value)
		{
		case 0:
			_textMesh.text = "Miss";
			Reset_FloatValueParams();
			break;
		case -1:
			_textMesh.text = "     Shattered";
			Reset_FloatValueParams();
			_floatValueSpriteRender.enabled = true;
			_floatValueSpriteRender.sprite = _shatterIco;
			break;
		case -2:
			_textMesh.text = "   Block";
			Reset_FloatValueParams();
			_floatValueSpriteRender.enabled = true;
			_floatValueSpriteRender.sprite = _defenseShieldIco;
			break;
		case -3:
			_textMesh.text = "Critical!";
			_textMesh.fontSize = 13.5f;
			break;
		case -4:
			_textMesh.text = "Miss";
			Reset_FloatValueParams();
			_textMesh.fontSize = _missFontSize;
			break;
		case -5:
			_textMesh.text = "Absorb";
			Reset_FloatValueParams();
			break;
		case -6:
			_textMesh.text = "Parry";
			Reset_FloatValueParams();
			_floatValueSpriteRender.enabled = true;
			_floatValueSpriteRender.sprite = _parrySparkleIco;
			break;
		case -7:
			_textMesh.text = "Immune";
			Reset_FloatValueParams();
			break;
		case -8:
			_textMesh.text = "   Block";
			Reset_FloatValueParams();
			_floatValueSpriteRender.enabled = true;
			_floatValueSpriteRender.sprite = _magicDefenseShieldIco;
			break;
		}
		if (_textMesh.colorGradientPreset == _floatGradients[4])
		{
			if (!_playExpGainSound)
			{
				_aSrc_expPickup.Play();
				_playExpGainSound = true;
			}
			_textMesh.text = $"               +{_value} EXP";
			_sineRotationSpeed = 0f;
			_floatTextGravity = 0f;
			_textMesh.fontSize = 4.2f;
			_floatValueSpriteRender.enabled = false;
			_floatValueCritSpriteRender.enabled = false;
			_textMesh.enableVertexGradient = true;
			_textMesh.color = Color.white;
		}
		else
		{
			_sineRotationSpeed = 16f;
		}
		if (_textMesh.colorGradientPreset == _floatGradients[5])
		{
			_floatValueSpriteRender.enabled = true;
			_floatValueSpriteRender.sprite = _healIco;
		}
		else if (_textMesh.colorGradientPreset == _floatGradients[8])
		{
			_floatValueSpriteRender.enabled = true;
			_floatValueSpriteRender.sprite = _manaIco;
		}
		else if (_textMesh.colorGradientPreset == _floatGradients[3])
		{
			_textMesh.fontSize = 9.5f;
		}
		void Reset_FloatValueParams()
		{
			_textMesh.fontSize = _standardFontSize;
			_floatValueSpriteRender.enabled = false;
			_floatValueCritSpriteRender.enabled = false;
			_textMesh.enableVertexGradient = true;
			_textMesh.color = Color.white;
		}
	}

	private void Handle_SineRotation()
	{
		if (_currentSineRotationSpeed > 0f)
		{
			_currentSineRotationSpeed -= Time.deltaTime;
			_setNumberContainer.Translate(_sineRotationRadius * _sineRotationSpeed * Time.deltaTime * Vector3.left, Space.Self);
		}
	}

	private void Handle_FollowObject()
	{
		if ((bool)_setFollowObject)
		{
			Vector3 position = _setFollowObject.position;
			position.y = _setFollowObject.position.y + 0.85f;
			base.transform.position = Vector3.Lerp(base.transform.position, position, 7.95f * Time.deltaTime);
		}
	}

	private void Handle_FloatValueTimers()
	{
		if (_returnToPoolTime > 0f)
		{
			_returnToPoolTime -= Time.deltaTime;
		}
		else
		{
			PoolManager._current.Init_ReturnObjectToContainer(base.gameObject);
		}
		if (_displayTimer <= 0f)
		{
			_lerpScaler.setScale = new Vector3(0f, 12.5f, 12.5f);
		}
		else
		{
			_displayTimer -= Time.deltaTime;
		}
		if (_value != -2 && _value != -7)
		{
			if (_speedTime > 0f)
			{
				_setNumberContainer.Translate(_speedTime * _upwardsSpeed * Time.deltaTime * Vector3.up, Space.Self);
				_speedTime -= Time.deltaTime;
			}
			else
			{
				_floatTextGravity += Time.deltaTime * 4.65f;
			}
			_floatTextGravity += Time.deltaTime * 7.65f;
			_setNumberContainer.Translate(_floatTextGravity * Time.deltaTime * Vector3.down, Space.Self);
		}
	}
}
