using System.Collections;
using UnityEngine;

public class ConditionObject : MonoBehaviour
{
	[Header("Runtime")]
	public StatusEntity _statusEntity;

	public ScriptableCondition _scriptCondition;

	[Space]
	public Renderer[] _miscRenderCache;

	public MeshRenderer[] _meshRenderCache;

	public AudioSource[] _aSrcCache;

	[Header("Display")]
	public GameObject _visualContainer;

	[SerializeField]
	private float _setReturnToPoolTimer;

	[SerializeField]
	private bool _returnToPoolEarly;

	[SerializeField]
	private float _playSoundInit = 0.05f;

	private float _returnToPoolTimer;

	private void Awake()
	{
		_miscRenderCache = GetComponentsInChildren<Renderer>(includeInactive: true);
		_meshRenderCache = GetComponentsInChildren<MeshRenderer>(includeInactive: true);
		_aSrcCache = GetComponentsInChildren<AudioSource>(includeInactive: true);
		Init_ConditionObject();
	}

	private void OnEnable()
	{
		Init_ConditionObject();
	}

	public virtual void Init_ConditionObject()
	{
		for (int i = 0; i < _aSrcCache.Length; i++)
		{
			_aSrcCache[i].playOnAwake = false;
			_aSrcCache[i].dopplerLevel = 0f;
			_aSrcCache[i].spatialBlend = 1f;
		}
		_returnToPoolTimer = 0f;
		StartCoroutine(Init_ConditionSound());
	}

	private IEnumerator Init_ConditionSound()
	{
		yield return new WaitForSeconds(_playSoundInit);
		PlaySound();
		void PlaySound()
		{
			if ((bool)Player._mainPlayer && !Player._mainPlayer._bufferingStatus && (bool)_statusEntity && (!_statusEntity._isPlayer || (!_statusEntity._isPlayer._bufferingStatus && !(_statusEntity._isPlayer.Network_playerMapInstance != Player._mainPlayer.Network_playerMapInstance))))
			{
				for (int i = 0; i < _aSrcCache.Length; i++)
				{
					if (_aSrcCache[i].gameObject.activeInHierarchy)
					{
						_aSrcCache[i].Stop();
						_aSrcCache[i].Play();
					}
				}
			}
		}
	}

	private void Update()
	{
		ConditionObject_OnUpdate();
	}

	public virtual void ConditionObject_OnUpdate()
	{
		if (_returnToPoolEarly)
		{
			_returnToPoolTimer += Time.deltaTime;
			if (_returnToPoolTimer >= _setReturnToPoolTimer)
			{
				PoolManager._current.Init_ReturnObjectToContainer(base.gameObject);
				base.gameObject.SetActive(value: false);
			}
		}
		base.transform.rotation = _statusEntity.transform.rotation;
		Handle_ConditionObjectVisuals();
	}

	public virtual void ConditionObject_UpdateBehavior()
	{
		if ((bool)_statusEntity && (bool)_statusEntity._isPlayer)
		{
			_ = _statusEntity._isPlayer._bufferingStatus;
		}
	}

	private void Handle_ConditionObjectVisuals()
	{
		if ((bool)Player._mainPlayer)
		{
			for (int i = 0; i < _miscRenderCache.Length; i++)
			{
				_miscRenderCache[i].enabled = Player._mainPlayer.gameObject.scene == base.gameObject.scene;
			}
			for (int j = 0; j < _meshRenderCache.Length; j++)
			{
				_meshRenderCache[j].enabled = Player._mainPlayer.gameObject.scene == base.gameObject.scene;
			}
			for (int k = 0; k < _aSrcCache.Length; k++)
			{
				_aSrcCache[k].mute = Player._mainPlayer.gameObject.scene != base.gameObject.scene;
			}
			if ((bool)_statusEntity._isPlayer)
			{
				Cull_ConditionObject(Player._mainPlayer.Network_playerMapInstance == _statusEntity._isPlayer.Network_playerMapInstance);
			}
			else if ((bool)_statusEntity._isCreep && (bool)_statusEntity._isCreep._parentCreepSpawner)
			{
				Cull_ConditionObject(Player._mainPlayer.Network_playerMapInstance == _statusEntity._isCreep._parentCreepSpawner._mapInstance);
				base.transform.localPosition = new Vector3(base.transform.localPosition.x, _statusEntity._isCreep._scriptCreep._generalCreepVisual._targeterOffset, base.transform.localPosition.z);
			}
		}
		void Cull_ConditionObject(bool _isTrue)
		{
			_visualContainer.SetActive(_isTrue);
			for (int l = 0; l < _aSrcCache.Length; l++)
			{
				if (!_isTrue)
				{
					_aSrcCache[l].Stop();
				}
				else if (!_aSrcCache[l].isPlaying && _aSrcCache[l].loop)
				{
					_aSrcCache[l].Play();
				}
				_aSrcCache[l].mute = !_isTrue;
			}
		}
	}
}
