using UnityEngine;

public class TimerAnimatorObject : MonoBehaviour
{
	[Header("Timer Animator Object")]
	[SerializeField]
	private Animator _animator;

	[SerializeField]
	private float _repeatRate = 1f;

	[SerializeField]
	private float _intializeTimer = 0.4f;

	private bool _isInit;

	[Header("Visual / Audio")]
	[SerializeField]
	private GameObject[] _gameObjectCache;

	[SerializeField]
	private AudioSource[] _aSrcCache;

	[SerializeField]
	private bool _hardDisableASrc;

	private void Awake()
	{
		_aSrcCache = GetComponentsInChildren<AudioSource>(includeInactive: true);
	}

	private void Start()
	{
		InvokeRepeating("Init_Object", _intializeTimer, _repeatRate);
	}

	private void Init_Object()
	{
		if ((bool)_animator)
		{
			if (!_isInit)
			{
				_animator.SetTrigger("_on");
				_isInit = true;
			}
			else
			{
				_animator.SetTrigger("_off");
				_isInit = false;
			}
		}
	}

	private void Update()
	{
		if (!Player._mainPlayer)
		{
			return;
		}
		if (TabMenu._current._optionsCell._isDisconnecting || Player._mainPlayer._bufferingStatus)
		{
			for (int i = 0; i < _aSrcCache.Length; i++)
			{
				_aSrcCache[i].mute = !_hardDisableASrc;
			}
			for (int j = 0; j < _gameObjectCache.Length; j++)
			{
				_gameObjectCache[j].SetActive(value: false);
			}
		}
		else if (Player._mainPlayer._isHostPlayer)
		{
			for (int k = 0; k < _aSrcCache.Length; k++)
			{
				_aSrcCache[k].mute = Player._mainPlayer.gameObject.scene != base.gameObject.scene && !_hardDisableASrc;
			}
			for (int l = 0; l < _gameObjectCache.Length; l++)
			{
				_gameObjectCache[l].SetActive(Player._mainPlayer.gameObject.scene == base.gameObject.scene);
			}
		}
		else
		{
			for (int m = 0; m < _gameObjectCache.Length; m++)
			{
				_gameObjectCache[m].SetActive(value: true);
			}
			for (int n = 0; n < _aSrcCache.Length; n++)
			{
				_aSrcCache[n].mute = _hardDisableASrc;
			}
		}
	}
}
