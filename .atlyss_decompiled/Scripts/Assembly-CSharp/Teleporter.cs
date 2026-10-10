using UnityEngine;

public class Teleporter : MonoBehaviour
{
	[Header("Local Teleporter Entity")]
	[SerializeField]
	private Transform _teleportPoint;

	[SerializeField]
	private GameObject _teleporterVisualContainer;

	[SerializeField]
	private ParticleSystem _teleporterEffect;

	[SerializeField]
	private bool _allowCreeps;

	public bool _isEnabled;

	private bool _teleportBuffer;

	private AudioSource _aSrc;

	private Renderer[] _cachedRenders;

	private void Awake()
	{
		_aSrc = GetComponentInChildren<AudioSource>();
		_cachedRenders = GetComponentsInChildren<Renderer>();
	}

	private void Start()
	{
		if (!_isEnabled)
		{
			Disable_Teleporter();
		}
		else
		{
			Enable_Teleporter();
		}
	}

	private void Update()
	{
		if (!Player._mainPlayer)
		{
			return;
		}
		if (Player._mainPlayer._isHostPlayer)
		{
			_aSrc.mute = (bool)Player._mainPlayer && Player._mainPlayer._isHostPlayer && Player._mainPlayer.gameObject.scene != base.gameObject.scene;
			if (_isEnabled)
			{
				_teleporterVisualContainer.SetActive((bool)Player._mainPlayer && Player._mainPlayer._isHostPlayer && Player._mainPlayer.gameObject.scene == base.gameObject.scene);
			}
			else
			{
				_teleporterVisualContainer.SetActive(value: false);
			}
		}
		else
		{
			_teleporterVisualContainer.SetActive(_isEnabled);
		}
	}

	[ContextMenu("Enable Teleporter")]
	public void Enable_Teleporter()
	{
		if ((bool)_teleporterVisualContainer)
		{
			_teleporterVisualContainer.SetActive(value: true);
		}
		if ((bool)_teleporterEffect && !_teleporterEffect.isPlaying)
		{
			_teleporterEffect.Play();
		}
		if ((bool)_aSrc && _aSrc.isActiveAndEnabled && !_aSrc.isPlaying)
		{
			_aSrc.Play();
		}
		_isEnabled = true;
		_teleportBuffer = true;
		for (int i = 0; i < _cachedRenders.Length; i++)
		{
			_cachedRenders[i].enabled = true;
		}
		Invoke("Reset_TeleportBuffer", 1.25f);
	}

	[ContextMenu("Disable Teleporter")]
	public void Disable_Teleporter()
	{
		if ((bool)_teleporterVisualContainer)
		{
			_teleporterVisualContainer.SetActive(value: false);
		}
		if ((bool)_teleporterEffect)
		{
			_teleporterEffect.Stop();
		}
		if ((bool)_aSrc)
		{
			_aSrc.Stop();
		}
		_isEnabled = false;
		_teleportBuffer = true;
		Invoke("Reset_TeleportBuffer", 1.25f);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!_isEnabled || _teleportBuffer)
		{
			return;
		}
		if ((bool)other.gameObject.GetComponent<Player>())
		{
			Player component = other.gameObject.GetComponent<Player>();
			PlayerVisual pVisual = component._pVisual;
			CharacterController playerController = component._pMove._playerController;
			if (component._currentPlayerCondition != PlayerCondition.ACTIVE || component._bufferingStatus)
			{
				return;
			}
			component._spawnBuffer = 0.45f;
			pVisual.Init_TeleportEffect();
			if (!component.isLocalPlayer)
			{
				return;
			}
			playerController.enabled = false;
			component.transform.SetPositionAndRotation(_teleportPoint.position, _teleportPoint.rotation);
			CameraFunction._current.CameraReset();
			playerController.enabled = true;
			_teleportBuffer = true;
			Invoke("Reset_TeleportBuffer", 1.25f);
		}
		if ((bool)other.gameObject.GetComponent<Creep>() && _allowCreeps)
		{
			Creep component2 = other.gameObject.GetComponent<Creep>();
			if (component2._currentCreepCondition == CreepCondition.ACTIVE && component2.isServer)
			{
				component2.transform.SetPositionAndRotation(_teleportPoint.position, _teleportPoint.rotation);
				_teleportBuffer = true;
				Invoke("Reset_TeleportBuffer", 1.25f);
			}
		}
	}

	private void Reset_TeleportBuffer()
	{
		_teleportBuffer = false;
	}
}
