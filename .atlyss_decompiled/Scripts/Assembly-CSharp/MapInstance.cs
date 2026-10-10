using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Mirror;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class MapInstance : NetworkBehaviour
{
	[HideInInspector]
	public Scene _loadedScene;

	[HideInInspector]
	public bool _availableToUnload;

	[Header("Scene Map Info")]
	public string _mapName = "New Map";

	public ZoneType _zoneType = ZoneType.Field;

	[Space]
	public PatternInstanceManager _patternInstance;

	public TerrainManager _terrainManager;

	[Header("Environmental Objects")]
	public GameObject _environmentGroup;

	public float _setEnvironmentScale = 1f;

	private readonly List<GameObject> _environmentHostColliders = new List<GameObject>();

	private Renderer[] _environmentRenders;

	private Light[] _environmentLights;

	[Header("Map Visuals")]
	[SerializeField]
	private Camera _skyboxCamera;

	[SerializeField]
	private CameraFogDensity _skyboxCamFogParams;

	[SerializeField]
	private Light _directionalLightObj;

	[Space]
	[SerializeField]
	private MapInstanceVisuals _mapVisuals;

	public bool _useOverrideVisuals;

	[Space]
	[SerializeField]
	private ParticleSystem[] _dayTimeParticles;

	[SerializeField]
	private ParticleSystem[] _nightTimeParticles;

	[Header("Map Audio")]
	[SerializeField]
	private AudioSource _weatherASrc;

	private AudioSource[] _cachedAudioSources;

	private AudioReverbZone[] _cachedAudioReverbZones;

	[Header("Music")]
	[SerializeField]
	private AudioSource _nullMusic;

	[SerializeField]
	private AudioSource _daytimeMusic;

	[SerializeField]
	private AudioSource _nightMusic;

	[SerializeField]
	private float _timeBeforeMusicStart = 12.5f;

	[Space]
	public AudioSource _actionMusic;

	public bool _playActionMusic;

	[Header("Runtime")]
	[SyncVar(hook = "OnWorldTimeChange")]
	public WorldTime _instanceWorldTime;

	[SyncVar]
	public ClockSetting _instanceClockSetting;

	[SyncVar]
	public int _instanceTime;

	[SyncVar]
	public bool _isWeatherEnabled;

	[HideInInspector]
	public MapInstanceVisuals _setOverrideVisuals;

	[Space]
	[SyncVar]
	public int _peercount;

	public List<SpawnPoint> _instanceSpawnPoints;

	public List<Player> _peersInInstance = new List<Player>();

	[Space]
	private Vector3 _originDirectionalLightRotation = Vector3.zero;

	private float _currentWeatherIntervalBuffer;

	private Terrain[] _terrainDatas;

	private float _unloadBufferTime;

	private bool _previousHostInScene = true;

	private float _musicBeginBuffer;

	private bool _musicStarted;

	private float _dayNightCycleSmooth;

	private bool _resettingDayNightCycleSmoothing;

	public WorldTime Network_instanceWorldTime
	{
		get
		{
			return _instanceWorldTime;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _instanceWorldTime, 1uL, OnWorldTimeChange);
		}
	}

	public ClockSetting Network_instanceClockSetting
	{
		get
		{
			return _instanceClockSetting;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _instanceClockSetting, 2uL, null);
		}
	}

	public int Network_instanceTime
	{
		get
		{
			return _instanceTime;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _instanceTime, 4uL, null);
		}
	}

	public bool Network_isWeatherEnabled
	{
		get
		{
			return _isWeatherEnabled;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _isWeatherEnabled, 8uL, null);
		}
	}

	public int Network_peercount
	{
		get
		{
			return _peercount;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _peercount, 16uL, null);
		}
	}

	private void OnWorldTimeChange(WorldTime _oldTime, WorldTime _newTime)
	{
		if ((bool)Player._mainPlayer && !Player._mainPlayer._bufferingStatus && !(Player._mainPlayer.Network_playerMapInstance != this) && !_mapVisuals._isInteriorInstance)
		{
			switch (_newTime)
			{
			case WorldTime.DAY:
				GameWorldManager._current._dayNightASrc.PlayOneShot(GameWorldManager._current._rooster, 0.45f);
				break;
			case WorldTime.NIGHT:
				GameWorldManager._current._dayNightASrc.PlayOneShot(GameWorldManager._current._wolf, 0.45f);
				break;
			}
			if (!_nullMusic)
			{
				_musicBeginBuffer = 0f;
				_musicStarted = false;
			}
		}
	}

	protected override void OnValidate()
	{
		base.OnValidate();
		base.name = "_MapInstance (" + _mapName + ")";
		if (!_patternInstance && (bool)GetComponent<PatternInstanceManager>())
		{
			_patternInstance = GetComponent<PatternInstanceManager>();
		}
		if (!_terrainManager && (bool)GetComponent<TerrainManager>())
		{
			_terrainManager = GetComponent<TerrainManager>();
		}
		if ((bool)_directionalLightObj)
		{
			_directionalLightObj.color = _mapVisuals._directionalLightColor;
		}
	}

	private void Awake()
	{
		if ((bool)_nullMusic)
		{
			_nullMusic.volume = 0f;
		}
		if ((bool)_daytimeMusic)
		{
			_daytimeMusic.volume = 0f;
		}
		if ((bool)_nightMusic)
		{
			_nightMusic.volume = 0f;
		}
		_environmentGroup.transform.localScale = Vector3.one * _setEnvironmentScale;
	}

	private void Start()
	{
		if ((bool)_directionalLightObj)
		{
			_originDirectionalLightRotation = _directionalLightObj.transform.rotation.eulerAngles;
		}
		Allocate_AudioSources();
	}

	private void Allocate_AudioSources()
	{
		_cachedAudioSources = _environmentGroup.GetComponentsInChildren<AudioSource>();
		_cachedAudioReverbZones = _environmentGroup.GetComponentsInChildren<AudioReverbZone>();
	}

	public override void OnStartServer()
	{
		base.OnStartServer();
		Apply_InstanceData();
	}

	[ServerCallback]
	private void Apply_InstanceData()
	{
		if (NetworkServer.active)
		{
			AtlyssNetworkManager current = AtlyssNetworkManager._current;
			HostConsole._current.Create_InstanceListEntry(base.netIdentity);
			HostConsole._current.New_LogMessage("Instance " + _mapName + " loaded.");
			_loadedScene = base.gameObject.scene;
			current._mapInstanceManager._loadedInstances.Add(this);
			Invoke("Enable_UnloadAvailability", 10.35f);
			int num = Random.Range(0, 2);
			if ((bool)_mapVisuals._weatherParticleSystem && num == 1)
			{
				Network_isWeatherEnabled = true;
			}
			_environmentLights = _environmentGroup.GetComponentsInChildren<Light>();
			_environmentRenders = _environmentGroup.GetComponentsInChildren<Renderer>();
			_terrainDatas = _environmentGroup.GetComponentsInChildren<Terrain>();
		}
	}

	private void Update()
	{
		Handle_InstanceRuntime();
		Handle_VisualSettings();
		Handle_AudioSettings();
	}

	[ServerCallback]
	private void Enable_UnloadAvailability()
	{
		if (NetworkServer.active)
		{
			_availableToUnload = true;
		}
	}

	[ServerCallback]
	public void SendPeer_ToSpawnPoint(GameObject _peerObject, string _spawnTag)
	{
		if (!NetworkServer.active || string.IsNullOrEmpty(_spawnTag))
		{
			return;
		}
		NetworkTransformUnreliable netTransform = _peerObject.GetComponent<Player>()._netTransform;
		Transform transform = null;
		Vector3 euler = Vector3.zero;
		for (int i = 0; i < _instanceSpawnPoints.Count; i++)
		{
			if (!(_instanceSpawnPoints[i]._spawnPointTag != _spawnTag))
			{
				transform = _instanceSpawnPoints[i].transform;
				euler = transform.rotation.eulerAngles;
				euler.x = 0f;
				euler.z = 0f;
				break;
			}
		}
		if (transform == null)
		{
			Debug.LogError("No spawn point with Tag " + _spawnTag + " found in the Map Instance Array.");
		}
		else
		{
			netTransform.RpcTeleport(transform.position, Quaternion.Euler(euler));
		}
	}

	[ServerCallback]
	private void Handle_InstanceRuntime()
	{
		if (NetworkServer.active)
		{
			Network_instanceWorldTime = GameWorldManager._current._worldTime;
			Network_instanceClockSetting = GameWorldManager._current._clockSetting;
			Network_instanceTime = GameWorldManager._current._timeDisplay;
			Handle_IteratePopulationList();
			Handle_AudioSources();
			Handle_WeatherRuntime();
		}
		void Handle_AudioSources()
		{
			if (!base.isClientOnly && _cachedAudioSources.Length != 0 && (bool)Player._mainPlayer)
			{
				for (int i = 0; i < _cachedAudioSources.Length; i++)
				{
					if ((bool)_cachedAudioSources[i])
					{
						_cachedAudioSources[i].mute = Player._mainPlayer.Network_playerMapInstance != this;
					}
				}
				for (int j = 0; j < _cachedAudioReverbZones.Length; j++)
				{
					_cachedAudioReverbZones[j].enabled = Player._mainPlayer.Network_playerMapInstance == this;
				}
			}
		}
		void Handle_IteratePopulationList()
		{
			Network_peercount = _peersInInstance.Count;
			for (int i = 0; i < _peersInInstance.Count; i++)
			{
				if (_peersInInstance[i] == null)
				{
					_peersInInstance.RemoveAt(i);
					break;
				}
				if (_peersInInstance[i].gameObject.scene != _loadedScene)
				{
					_peersInInstance.RemoveAt(i);
				}
				else
				{
					_peersInInstance[i].Network_playerMapInstance = this;
				}
			}
			if (_peersInInstance.Count <= 0)
			{
				if (_availableToUnload && !MapInstancingManager._current._instanceQueue.Contains(base.gameObject.scene.path))
				{
					if (_unloadBufferTime < 4.5f)
					{
						_unloadBufferTime += Time.deltaTime;
					}
					else
					{
						Destroy_HostCameraColliders();
						HostConsole._current.New_LogMessage(_mapName + " is empty. Unloading instance.");
						AtlyssNetworkManager._current._mapInstanceManager.Init_UnloadSubScene(_loadedScene);
						_availableToUnload = false;
					}
				}
			}
			else
			{
				_unloadBufferTime = 0f;
			}
		}
		void Handle_WeatherRuntime()
		{
			if ((bool)_mapVisuals._weatherParticleSystem && _mapVisuals._chanceForWeather > 0)
			{
				if (_currentWeatherIntervalBuffer < _mapVisuals._weatherIntervalBuffer)
				{
					_currentWeatherIntervalBuffer += Time.deltaTime;
				}
				else
				{
					_currentWeatherIntervalBuffer = 0f;
					if (Random.Range(0, 100) <= _mapVisuals._chanceForWeather)
					{
						Network_isWeatherEnabled = !_isWeatherEnabled;
					}
				}
			}
		}
	}

	[ServerCallback]
	public void Handle_CheckHostCameraColliders()
	{
		if (!NetworkServer.active || !Player._mainPlayer || !Player._mainPlayer._isHostPlayer)
		{
			return;
		}
		if (Player._mainPlayer.gameObject.scene == base.gameObject.scene)
		{
			if (_environmentHostColliders.Count <= 0)
			{
				Spawn_HostCameraColliders();
			}
		}
		else if (_environmentHostColliders.Count > 0)
		{
			_environmentHostColliders.Clear();
		}
	}

	[ServerCallback]
	public void Spawn_HostCameraColliders()
	{
		if (!NetworkServer.active || !Player._mainPlayer || Player._mainPlayer.gameObject.scene != base.gameObject.scene)
		{
			return;
		}
		Collider[] componentsInChildren = _environmentGroup.GetComponentsInChildren<Collider>();
		List<Collider> list = new List<Collider>();
		list.AddRange(componentsInChildren);
		PushBlock[] array = Object.FindObjectsOfType<PushBlock>();
		for (int i = 0; i < array.Length; i++)
		{
			list.Add(array[i]._pushblockCube.GetComponent<Collider>());
		}
		for (int j = 0; j < list.Count; j++)
		{
			if (list[j].gameObject.layer != LayerMask.NameToLayer("Ground"))
			{
				continue;
			}
			GameObject gameObject = null;
			if (list[j].gameObject.TryGetComponent<BoxCollider>(out var component))
			{
				if ((bool)component.GetComponentInParent<ParticleBarrierObject>())
				{
					continue;
				}
				if (!component.isTrigger)
				{
					gameObject = new GameObject($"_hostCameraCollider_{j}");
					BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
					boxCollider.size = component.size;
					boxCollider.center = component.center;
				}
			}
			if (list[j].gameObject.TryGetComponent<MeshCollider>(out var component2) && !component2.isTrigger)
			{
				gameObject = new GameObject($"_hostCameraCollider_{j}");
				gameObject.AddComponent<MeshCollider>().sharedMesh = component2.sharedMesh;
			}
			if (list[j].gameObject.TryGetComponent<SphereCollider>(out var component3) && !component3.isTrigger)
			{
				gameObject = new GameObject($"_hostCameraCollider_{j}");
				gameObject.AddComponent<SphereCollider>().radius = component3.radius;
			}
			if (!(gameObject == null))
			{
				gameObject.transform.SetPositionAndRotation(list[j].gameObject.transform.position, list[j].gameObject.transform.rotation);
				gameObject.transform.localScale = list[j].gameObject.transform.lossyScale;
				if ((bool)list[j].gameObject.GetComponentInParent<PushBlock>() || (bool)list[j].gameObject.GetComponentInParent<LerpPosition>())
				{
					SceneManager.MoveGameObjectToScene(gameObject, base.gameObject.scene);
					ParentConstraint parentConstraint = gameObject.AddComponent<ParentConstraint>();
					ConstraintSource source = new ConstraintSource
					{
						sourceTransform = list[j].gameObject.transform,
						weight = 1f
					};
					parentConstraint.AddSource(source);
					parentConstraint.constraintActive = true;
					SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetActiveScene());
				}
				gameObject.layer = LayerMask.NameToLayer("Ground");
				_environmentHostColliders.Add(gameObject);
			}
		}
		for (int k = 0; k < _terrainDatas.Length; k++)
		{
			GameObject gameObject2 = new GameObject("_hostCameraCollider_terrain");
			gameObject2.AddComponent<TerrainCollider>().terrainData = _terrainDatas[k].terrainData;
			SceneManager.MoveGameObjectToScene(gameObject2, SceneManager.GetActiveScene());
			gameObject2.layer = LayerMask.NameToLayer("Ground");
			gameObject2.transform.position = _terrainDatas[k].gameObject.transform.position;
			_environmentHostColliders.Add(gameObject2);
		}
	}

	[ServerCallback]
	public void Destroy_HostCameraColliders()
	{
		if (NetworkServer.active && _environmentHostColliders.Count > 0)
		{
			for (int i = 0; i < _environmentHostColliders.Count; i++)
			{
				Object.Destroy(_environmentHostColliders[i]);
			}
			_environmentHostColliders.Clear();
		}
	}

	[ClientCallback]
	private void Handle_AudioSettings()
	{
		if (!NetworkClient.active || !GameWorldManager._current || !CameraFunction._current || !Player._mainPlayer)
		{
			return;
		}
		bool flag = base.isServer && base.isClient;
		bool flag2 = flag && Player._mainPlayer.gameObject.scene == base.gameObject.scene;
		if (Player._mainPlayer._bufferingStatus)
		{
			if ((bool)_daytimeMusic)
			{
				_daytimeMusic.volume = Mathf.Lerp(_daytimeMusic.volume, 0f, 0.85f * Time.deltaTime);
			}
			if ((bool)_nightMusic)
			{
				_nightMusic.volume = Mathf.Lerp(_nightMusic.volume, 0f, 0.85f * Time.deltaTime);
			}
			if ((bool)_nullMusic)
			{
				_nullMusic.volume = Mathf.Lerp(_nullMusic.volume, 0f, 0.85f * Time.deltaTime);
			}
			if ((bool)_actionMusic)
			{
				_actionMusic.volume = Mathf.Lerp(_actionMusic.volume, 0f, 0.85f * Time.deltaTime);
			}
			_musicBeginBuffer = 0f;
			return;
		}
		if (_musicBeginBuffer < _timeBeforeMusicStart && !_musicStarted)
		{
			_musicBeginBuffer += Time.deltaTime;
		}
		if (!flag2 & flag)
		{
			_musicStarted = false;
			_musicBeginBuffer = 0f;
			if ((bool)_nullMusic)
			{
				_nullMusic.Stop();
			}
			if ((bool)_daytimeMusic)
			{
				_daytimeMusic.Stop();
			}
			if ((bool)_nightMusic)
			{
				_nightMusic.Stop();
			}
			if ((bool)_actionMusic)
			{
				_actionMusic.Stop();
			}
			return;
		}
		if (_musicBeginBuffer >= _timeBeforeMusicStart && (bool)_nullMusic && !_musicStarted)
		{
			_nullMusic.Play();
			_musicStarted = true;
		}
		if ((bool)_actionMusic)
		{
			if (_playActionMusic)
			{
				if (!_actionMusic.isPlaying)
				{
					_actionMusic.Play();
				}
				if ((bool)_daytimeMusic)
				{
					_daytimeMusic.volume = Mathf.Lerp(_daytimeMusic.volume, 0f, 0.3f * Time.deltaTime);
				}
				if ((bool)_nightMusic)
				{
					_nightMusic.volume = Mathf.Lerp(_nightMusic.volume, 0f, 0.3f * Time.deltaTime);
				}
				if ((bool)_nullMusic)
				{
					_nullMusic.volume = Mathf.Lerp(_nullMusic.volume, 0f, 0.3f * Time.deltaTime);
				}
				_actionMusic.volume = Mathf.Lerp(_actionMusic.volume, 0.75f, 0.3f * Time.deltaTime);
				return;
			}
			_actionMusic.volume = Mathf.Lerp(_actionMusic.volume, 0f, 0.8f * Time.deltaTime);
		}
		if ((bool)_nullMusic)
		{
			if (_musicStarted && !_playActionMusic)
			{
				_nullMusic.volume = Mathf.Lerp(_nullMusic.volume, 0.75f, 1.5f * Time.deltaTime);
			}
			return;
		}
		switch (_instanceWorldTime)
		{
		case WorldTime.DAY:
			if ((bool)_nightMusic)
			{
				_nightMusic.volume = Mathf.Lerp(_nightMusic.volume, 0f, 0.85f * Time.deltaTime);
			}
			if ((bool)_daytimeMusic)
			{
				_daytimeMusic.volume = 0.75f;
			}
			if (_musicBeginBuffer >= _timeBeforeMusicStart && (bool)_daytimeMusic && !_musicStarted)
			{
				_daytimeMusic.Play();
				_musicStarted = true;
			}
			break;
		case WorldTime.NIGHT:
			if ((bool)_daytimeMusic)
			{
				_daytimeMusic.volume = Mathf.Lerp(_daytimeMusic.volume, 0f, 1.5f * Time.deltaTime);
			}
			if ((bool)_nightMusic)
			{
				_nightMusic.volume = 0.75f;
			}
			if (_musicBeginBuffer >= _timeBeforeMusicStart && (bool)_nightMusic && !_musicStarted)
			{
				_nightMusic.Play();
				_musicStarted = true;
			}
			break;
		}
	}

	private IEnumerator Reset_DayNightSmoothing()
	{
		_resettingDayNightCycleSmoothing = true;
		yield return new WaitForSeconds(0.8f);
		_dayNightCycleSmooth = GameWorldManager._current._dayNightCycleSmooth;
		_resettingDayNightCycleSmoothing = false;
	}

	[ClientCallback]
	private void Handle_VisualSettings()
	{
		if (!NetworkClient.active)
		{
			return;
		}
		if (!GameWorldManager._current || !CameraFunction._current || !Player._mainPlayer)
		{
			return;
		}
		bool _isHost = base.isServer && base.isClient;
		bool _hostInScene = _isHost && Player._mainPlayer.gameObject.scene == base.gameObject.scene;
		if (Player._mainPlayer._bufferingStatus)
		{
			_useOverrideVisuals = false;
		}
		MapInstanceVisuals _runTimeVisuals;
		if (!_useOverrideVisuals)
		{
			if (_dayNightCycleSmooth != GameWorldManager._current._dayNightCycleSmooth && !_resettingDayNightCycleSmoothing)
			{
				StartCoroutine(Reset_DayNightSmoothing());
			}
			if (Player._mainPlayer._bufferingStatus)
			{
				_dayNightCycleSmooth = GameWorldManager._current._dayNightCycleSmooth;
			}
			_runTimeVisuals = _mapVisuals;
		}
		else
		{
			_runTimeVisuals = _setOverrideVisuals;
			_dayNightCycleSmooth = 1.2f;
		}
		Host_VisualParams();
		Assign_DayNightColoration();
		Assign_Fog();
		Assign_AmbientLighting();
		Assign_Weather();
		void Assign_AmbientLighting()
		{
			if (!(!_hostInScene & _isHost))
			{
				RenderSettings.ambientMode = AmbientMode.Flat;
			}
		}
		void Assign_DayNightColoration()
		{
			if (!(!_hostInScene & _isHost))
			{
				if (_runTimeVisuals._isInteriorInstance)
				{
					_runTimeVisuals._ambientNightColor = _runTimeVisuals._ambientColor;
					_runTimeVisuals._skySolidNightColor = _runTimeVisuals._skySolidColor;
					_runTimeVisuals._directionalLightNightColor = _runTimeVisuals._directionalLightColor;
					_runTimeVisuals._directionalLightNightRotation = _originDirectionalLightRotation;
				}
				if (LoadSceneManager._current._loadInteractionBuffer)
				{
					switch (_instanceWorldTime)
					{
					case WorldTime.DAY:
						AutoColor_Day();
						break;
					case WorldTime.NIGHT:
						AutoColor_Night();
						break;
					}
				}
				else
				{
					switch (_instanceWorldTime)
					{
					case WorldTime.DAY:
						if (_dayTimeParticles != null)
						{
							for (int k = 0; k < _dayTimeParticles.Length; k++)
							{
								if (!_dayTimeParticles[k].isPlaying)
								{
									_dayTimeParticles[k].Play();
								}
							}
						}
						if (_nightTimeParticles != null)
						{
							for (int l = 0; l < _nightTimeParticles.Length; l++)
							{
								if (_nightTimeParticles[l].isPlaying)
								{
									_nightTimeParticles[l].Stop();
								}
							}
						}
						if (_runTimeVisuals._useForceSkyColor)
						{
							_skyboxCamera.backgroundColor = Color.Lerp(_skyboxCamera.backgroundColor, _runTimeVisuals._forceSkyColor, _dayNightCycleSmooth * Time.deltaTime);
						}
						else
						{
							_skyboxCamera.backgroundColor = Color.Lerp(_skyboxCamera.backgroundColor, _runTimeVisuals._skySolidColor, _dayNightCycleSmooth * Time.deltaTime);
						}
						_directionalLightObj.transform.rotation = Quaternion.Lerp(_directionalLightObj.transform.rotation, Quaternion.Euler(_originDirectionalLightRotation), _dayNightCycleSmooth * Time.deltaTime);
						_directionalLightObj.color = Color.Lerp(_directionalLightObj.color, _runTimeVisuals._directionalLightColor, _dayNightCycleSmooth * Time.deltaTime);
						RenderSettings.fogColor = Color.Lerp(RenderSettings.fogColor, _runTimeVisuals._skySolidColor, _dayNightCycleSmooth * Time.deltaTime);
						RenderSettings.ambientSkyColor = Color.Lerp(RenderSettings.ambientSkyColor, _runTimeVisuals._ambientColor, _dayNightCycleSmooth * Time.deltaTime);
						break;
					case WorldTime.NIGHT:
						if (_dayTimeParticles != null)
						{
							for (int i = 0; i < _dayTimeParticles.Length; i++)
							{
								if (_dayTimeParticles[i].isPlaying)
								{
									_dayTimeParticles[i].Stop();
								}
							}
						}
						if (_nightTimeParticles != null)
						{
							for (int j = 0; j < _nightTimeParticles.Length; j++)
							{
								if (!_nightTimeParticles[j].isPlaying)
								{
									_nightTimeParticles[j].Play();
								}
							}
						}
						if (_runTimeVisuals._useForceSkyColor)
						{
							_skyboxCamera.backgroundColor = Color.Lerp(_skyboxCamera.backgroundColor, _runTimeVisuals._forceSkyColor, _dayNightCycleSmooth * Time.deltaTime);
						}
						else
						{
							_skyboxCamera.backgroundColor = Color.Lerp(_skyboxCamera.backgroundColor, _runTimeVisuals._skySolidNightColor, _dayNightCycleSmooth * Time.deltaTime);
						}
						_directionalLightObj.transform.rotation = Quaternion.Lerp(_directionalLightObj.transform.rotation, Quaternion.Euler(_runTimeVisuals._directionalLightNightRotation), _dayNightCycleSmooth * Time.deltaTime);
						_directionalLightObj.color = Color.Lerp(_directionalLightObj.color, _runTimeVisuals._directionalLightNightColor, _dayNightCycleSmooth * Time.deltaTime);
						RenderSettings.fogColor = Color.Lerp(RenderSettings.fogColor, _runTimeVisuals._skySolidNightColor, _dayNightCycleSmooth * Time.deltaTime);
						RenderSettings.ambientSkyColor = Color.Lerp(RenderSettings.ambientSkyColor, _runTimeVisuals._ambientNightColor, _dayNightCycleSmooth * Time.deltaTime);
						break;
					}
				}
			}
		}
		void Assign_Fog()
		{
			if (_runTimeVisuals._enableFog && !(!_hostInScene & _isHost))
			{
				RenderSettings.fog = true;
				RenderSettings.fogMode = _runTimeVisuals._fogMode;
				RenderSettings.fogDensity = _runTimeVisuals._skybox_fogDensity;
				if ((bool)_skyboxCamFogParams)
				{
					_skyboxCamFogParams.fogDensity = _runTimeVisuals._mainCam_fogDensity;
				}
			}
		}
		void Assign_Weather()
		{
			if ((bool)_runTimeVisuals._weatherParticleSystem)
			{
				if (!_hostInScene & _isHost)
				{
					_runTimeVisuals._weatherParticleSystem.Stop();
				}
				else if (_isWeatherEnabled)
				{
					Vector3 position = Player._mainPlayer.gameObject.transform.position;
					position.y = 320.5f;
					_runTimeVisuals._weatherParticleSystem.gameObject.transform.position = position;
					if (_runTimeVisuals._isInteriorInstance)
					{
						_runTimeVisuals._weatherParticleSystem.Stop();
						_weatherASrc.volume = Mathf.Lerp(_weatherASrc.volume, 0f, 0.3f * Time.deltaTime);
					}
					else
					{
						if (!_runTimeVisuals._weatherParticleSystem.isEmitting)
						{
							_runTimeVisuals._weatherParticleSystem.Play();
						}
						_weatherASrc.volume = Mathf.Lerp(_weatherASrc.volume, 0.45f, 0.07f * Time.deltaTime);
					}
				}
				else
				{
					_runTimeVisuals._weatherParticleSystem.Stop();
					_weatherASrc.volume = Mathf.Lerp(_weatherASrc.volume, 0f, 0.3f * Time.deltaTime);
				}
			}
		}
		void AutoColor_Day()
		{
			_skyboxCamera.backgroundColor = _runTimeVisuals._skySolidColor;
			_directionalLightObj.transform.rotation = Quaternion.Euler(_originDirectionalLightRotation);
			_directionalLightObj.color = _runTimeVisuals._directionalLightColor;
			RenderSettings.fogColor = _runTimeVisuals._skySolidColor;
			RenderSettings.ambientSkyColor = _runTimeVisuals._ambientColor;
		}
		void AutoColor_Night()
		{
			_skyboxCamera.backgroundColor = _runTimeVisuals._skySolidNightColor;
			_directionalLightObj.transform.rotation = Quaternion.Euler(_runTimeVisuals._directionalLightNightRotation);
			_directionalLightObj.color = _runTimeVisuals._directionalLightNightColor;
			RenderSettings.fogColor = _runTimeVisuals._skySolidNightColor;
			RenderSettings.ambientSkyColor = _runTimeVisuals._ambientNightColor;
		}
		void Host_VisualParams()
		{
			if (_isHost && _previousHostInScene != _hostInScene)
			{
				_previousHostInScene = _hostInScene;
				_skyboxCamera.enabled = _hostInScene;
				Light[] environmentLights = _environmentLights;
				for (int i = 0; i < environmentLights.Length; i++)
				{
					environmentLights[i].enabled = _hostInScene;
				}
				for (int j = 0; j < _environmentRenders.Length; j++)
				{
					_environmentRenders[j].enabled = _hostInScene;
				}
				if (_terrainDatas != null && _terrainDatas.Length != 0)
				{
					for (int k = 0; k < _terrainDatas.Length; k++)
					{
						if (!(_terrainDatas[k] == null))
						{
							_terrainDatas[k].drawHeightmap = _hostInScene;
							_terrainDatas[k].drawTreesAndFoliage = _hostInScene;
						}
					}
				}
			}
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawIcon(base.transform.position + Vector3.up * 1.5f, "_mapInstanceIco", allowScaling: false);
	}

	public override bool Weaved()
	{
		return true;
	}

	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			GeneratedNetworkCode._Write_WorldTime(writer, _instanceWorldTime);
			GeneratedNetworkCode._Write_ClockSetting(writer, _instanceClockSetting);
			writer.WriteInt(_instanceTime);
			writer.WriteBool(_isWeatherEnabled);
			writer.WriteInt(_peercount);
			return;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 1L) != 0L)
		{
			GeneratedNetworkCode._Write_WorldTime(writer, _instanceWorldTime);
		}
		if ((base.syncVarDirtyBits & 2L) != 0L)
		{
			GeneratedNetworkCode._Write_ClockSetting(writer, _instanceClockSetting);
		}
		if ((base.syncVarDirtyBits & 4L) != 0L)
		{
			writer.WriteInt(_instanceTime);
		}
		if ((base.syncVarDirtyBits & 8L) != 0L)
		{
			writer.WriteBool(_isWeatherEnabled);
		}
		if ((base.syncVarDirtyBits & 0x10L) != 0L)
		{
			writer.WriteInt(_peercount);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize(ref _instanceWorldTime, OnWorldTimeChange, GeneratedNetworkCode._Read_WorldTime(reader));
			GeneratedSyncVarDeserialize(ref _instanceClockSetting, null, GeneratedNetworkCode._Read_ClockSetting(reader));
			GeneratedSyncVarDeserialize(ref _instanceTime, null, reader.ReadInt());
			GeneratedSyncVarDeserialize(ref _isWeatherEnabled, null, reader.ReadBool());
			GeneratedSyncVarDeserialize(ref _peercount, null, reader.ReadInt());
			return;
		}
		long num = (long)reader.ReadULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _instanceWorldTime, OnWorldTimeChange, GeneratedNetworkCode._Read_WorldTime(reader));
		}
		if ((num & 2L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _instanceClockSetting, null, GeneratedNetworkCode._Read_ClockSetting(reader));
		}
		if ((num & 4L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _instanceTime, null, reader.ReadInt());
		}
		if ((num & 8L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _isWeatherEnabled, null, reader.ReadBool());
		}
		if ((num & 0x10L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _peercount, null, reader.ReadInt());
		}
	}
}
