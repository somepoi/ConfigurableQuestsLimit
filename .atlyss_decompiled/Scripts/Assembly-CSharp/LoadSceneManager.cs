using System.Collections;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class LoadSceneManager : MonoBehaviour
{
	public static LoadSceneManager _current;

	[Header("Load Scene Management")]
	[SerializeField]
	private Camera _renderLoadSceneCamera;

	[SerializeField]
	private GameObject _loadConeEffectObject;

	[SerializeField]
	private ParticleSystem _loadSwirlEffect;

	[SerializeField]
	private ParticleSystem _loadIcoEffect;

	[SerializeField]
	private ParticleSystem _loadCursorEffect;

	[SerializeField]
	private Rigidbody _loadCursorRb;

	[SerializeField]
	private float _loadCursorSpeed = 35.65f;

	[SerializeField]
	private CanvasGroup _fadeGroup;

	[SerializeField]
	private AudioSource _aSrc;

	[Header("Fade In / Out")]
	public bool _isLoadingScreen;

	public bool _loadInteractionBuffer;

	private bool _fadeIn;

	[Header("Introduction Assets")]
	[SerializeField]
	private CanvasGroup _newCharacterMessageGroup;

	[SerializeField]
	private Text _newCharacterMessageText;

	[SerializeField]
	private ParticleSystem _introEffect;

	[SerializeField]
	private AudioSource _aSrc_introNoise;

	[SerializeField]
	private AudioClip _introNoise_00;

	[SerializeField]
	private AudioClip _introNoise_01;

	[SerializeField]
	private AudioClip _introNoise_02;

	private bool _timeoutPrediction;

	private bool _isIntroDialog;

	private void Awake()
	{
		_current = this;
		_fadeGroup.alpha = 0f;
		_newCharacterMessageGroup.alpha = 0f;
		Disable_LoadScreen();
	}

	private void Update()
	{
		Handle_FadeEffect();
		if (!NetworkClient.active)
		{
			_isLoadingScreen = false;
			_loadConeEffectObject.SetActive(value: false);
		}
		if (!AtlyssNetworkManager._current._soloMode && _isLoadingScreen && !_timeoutPrediction)
		{
			StartCoroutine(Buffer_TimeoutPrediction());
		}
		if (!_isLoadingScreen)
		{
			StopAllCoroutines();
			_timeoutPrediction = false;
		}
	}

	private IEnumerator Buffer_TimeoutPrediction()
	{
		_timeoutPrediction = true;
		yield return new WaitForSeconds(12.65f);
		if (!Player._mainPlayer)
		{
			AtlyssNetworkManager._current.StopClient();
			HostConsole._current.Init_NetworkStatusMessage("Connection Timed Out.");
			_timeoutPrediction = false;
		}
	}

	private void FixedUpdate()
	{
		if (_isLoadingScreen)
		{
			if (_loadInteractionBuffer)
			{
				Vector3 force = new Vector3(InputControlManager.current._horiz_input, InputControlManager.current._vert_input, 0f) * _loadCursorSpeed;
				_loadCursorRb.AddForce(force);
				_aSrc.volume = Mathf.Lerp(_aSrc.volume, 0.65f, 1.5f * Time.deltaTime);
			}
			_loadConeEffectObject.SetActive(value: true);
		}
		else
		{
			_loadCursorRb.position = new Vector3(0f, 0f, 8.75f);
			if (!_loadCursorRb.isKinematic)
			{
				_loadCursorRb.velocity = Vector3.zero;
			}
			_aSrc.volume = Mathf.Lerp(_aSrc.volume, 0f, 3.5f * Time.deltaTime);
			_loadConeEffectObject.SetActive(value: false);
		}
		_loadCursorRb.isKinematic = !_isLoadingScreen;
	}

	private void Handle_FadeEffect()
	{
		if (!_isLoadingScreen)
		{
			_fadeGroup.alpha -= 3.65f * Time.deltaTime;
		}
		if (_fadeIn)
		{
			_fadeGroup.alpha += 3.65f * Time.deltaTime;
		}
	}

	public void Init_LoadScreen()
	{
		_fadeIn = true;
		_fadeGroup.alpha = 0f;
		_isLoadingScreen = true;
		Invoke("LoadScreen_Effect", 0.45f);
		Invoke("AllowCursorMove", 1.25f);
		if (ProfileDataManager._current._characterFile._isNewCharacter)
		{
			StartCoroutine(Init_IntroDialog());
		}
	}

	private void LoadScreen_Effect()
	{
		_fadeIn = false;
		_fadeGroup.alpha = 0f;
		_renderLoadSceneCamera.enabled = true;
		_loadSwirlEffect.Play();
		_loadCursorEffect.Play();
		_loadIcoEffect.Play();
		GetComponent<Animator>().CrossFade("_animCtrl_loadScreenInit", 0f, 0);
		_loadCursorRb.velocity = Vector3.zero;
	}

	private void AllowCursorMove()
	{
		_loadInteractionBuffer = true;
	}

	private IEnumerator Init_IntroDialog()
	{
		if (!_isIntroDialog)
		{
			_isIntroDialog = true;
			_introEffect.Play();
			yield return new WaitForSeconds(1.5f);
			_aSrc_introNoise.PlayOneShot(_introNoise_00, 0.75f);
			_newCharacterMessageText.text = "Your time is not over yet, little one...";
			while (_newCharacterMessageGroup.alpha < 0.98f)
			{
				_newCharacterMessageGroup.alpha = Mathf.Lerp(_newCharacterMessageGroup.alpha, 1f, 2.5f * Time.deltaTime);
				yield return null;
			}
			yield return new WaitForSeconds(1.5f);
			_aSrc_introNoise.PlayOneShot(_introNoise_01, 0.75f);
			while (_newCharacterMessageGroup.alpha > 0.08f)
			{
				_newCharacterMessageGroup.alpha = Mathf.Lerp(_newCharacterMessageGroup.alpha, 0f, 2.5f * Time.deltaTime);
				yield return null;
			}
			_newCharacterMessageGroup.alpha = 0f;
			_newCharacterMessageText.text = "This world is broken, in need of your help.";
			while (_newCharacterMessageGroup.alpha < 0.98f)
			{
				_newCharacterMessageGroup.alpha = Mathf.Lerp(_newCharacterMessageGroup.alpha, 1f, 2.5f * Time.deltaTime);
				yield return null;
			}
			yield return new WaitForSeconds(1.5f);
			_aSrc_introNoise.PlayOneShot(_introNoise_02, 0.75f);
			while (_newCharacterMessageGroup.alpha > 0.08f)
			{
				_newCharacterMessageGroup.alpha = Mathf.Lerp(_newCharacterMessageGroup.alpha, 0f, 2.5f * Time.deltaTime);
				yield return null;
			}
			_newCharacterMessageText.text = "I plea for your soul, and your strength...";
			while (_newCharacterMessageGroup.alpha < 0.98f)
			{
				_newCharacterMessageGroup.alpha = Mathf.Lerp(_newCharacterMessageGroup.alpha, 1f, 2.5f * Time.deltaTime);
				yield return null;
			}
			yield return new WaitForSeconds(1.5f);
			while (_newCharacterMessageGroup.alpha > 0.08f)
			{
				_newCharacterMessageGroup.alpha = Mathf.Lerp(_newCharacterMessageGroup.alpha, 0f, 2.5f * Time.deltaTime);
				yield return null;
			}
			_newCharacterMessageGroup.alpha = 0f;
			_introEffect.Stop();
			yield return new WaitForSeconds(1.5f);
			_newCharacterMessageGroup.alpha = 0f;
			Init_LoadScreenDisable();
			_isIntroDialog = false;
		}
	}

	public void Disable_LoadScreen()
	{
		Init_LoadScreenDisable();
	}

	private void Init_LoadScreenDisable()
	{
		_loadSwirlEffect.Stop();
		_loadCursorEffect.Stop();
		_loadIcoEffect.Stop();
		_renderLoadSceneCamera.enabled = false;
		_isLoadingScreen = false;
		_loadInteractionBuffer = false;
		_fadeGroup.alpha = 1f;
		_newCharacterMessageGroup.alpha = 0f;
		_introEffect.Stop();
	}
}
