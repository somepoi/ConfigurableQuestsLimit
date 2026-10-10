using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Mirror;
using UnityEngine;

public class PatternInstanceManager : NetworkBehaviour
{
	[CompilerGenerated]
	private sealed class _003CServer_StartupPatterns_003Ed__44 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public PatternInstanceManager _003C_003E4__this;

		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		[DebuggerHidden]
		public _003CServer_StartupPatterns_003Ed__44(int _003C_003E1__state)
		{
			this._003C_003E1__state = _003C_003E1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
		}

		private bool MoveNext()
		{
			int num = _003C_003E1__state;
			PatternInstanceManager patternInstanceManager = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
			{
				_003C_003E1__state = -1;
				for (int i = 0; i < patternInstanceManager._instancePatterns.Length; i++)
				{
					patternInstanceManager._003CServer_StartupPatterns_003Eg__Iterate_StartupInstancePattern_007C44_0(patternInstanceManager._instancePatterns[i]);
				}
				_003C_003E2__current = new WaitForSeconds(1.25f);
				_003C_003E1__state = 1;
				return true;
			}
			case 1:
				_003C_003E1__state = -1;
				patternInstanceManager.StartCoroutine(patternInstanceManager.Init_SetPattern());
				return false;
			}
		}

		bool IEnumerator.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			return this.MoveNext();
		}

		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}
	}

	[Header("Pattern Parameters")]
	public MapInstance _mapInstance;

	[SyncVar]
	public int _instancePatternID;

	[Space]
	public InstancePattern[] _instancePatterns;

	public ScriptableLootTable[] _difficultyChestLootTables;

	public ScriptableLootTable[] _difficultyBreakableLootTables;

	[Header("Instance Boss")]
	public GameObject _normalBossLightOverride;

	public GameObject _hardBossLightOverride;

	[Space]
	public CreepSpawner _bossSpawner;

	public CreepSpawner _normalBossSpawner;

	public CreepSpawner _hardBossSpawner;

	[Space]
	public ItemDropEntity_Chest _bossLootChest;

	public ScriptableLootTable _normalBossLootTable;

	public ScriptableLootTable _hardBossLootTable;

	[Header("Runtime")]
	[SyncVar]
	public Player _instanceHostPlayer;

	[SyncVar]
	public PartyObjectBehavior _instanceParty;

	[SyncVar]
	public ZoneDifficulty _setDungeonDifficulty;

	[SyncVar(hook = "On_DungeonKeyChange")]
	public int _dungeonKeysAquired;

	[Space]
	[SyncVar]
	public bool _isBossEngaged;

	[SyncVar]
	public bool _isBossDefeated;

	[SyncVar]
	public bool _allArenasBeaten;

	[Space]
	public BossRoomTeleporter _bossRoomTeleporter;

	public InstancePattern _setInstancePattern;

	public List<CreepArena> _setInstanceCreepArenas = new List<CreepArena>();

	private Renderer[] _cachedLocalRenderers;

	private AudioSource[] _cachedPatternASources;

	[Space]
	[SyncVar]
	public bool _generatingPattern;

	public bool _isSettingPattern;

	[Header("Interface")]
	[SerializeField]
	private GameObject _minimapCameraObject;

	[SerializeField]
	private GameObject _minimapDeathSpotIcon;

	[Header("Instance Music")]
	public AudioSource _muAmbienceSrc;

	public AudioSource _muActionSrc;

	public AudioSource _muBossSrc;

	public AudioClip _clearMusic;

	public AudioClip _hardBossTheme;

	protected NetworkBehaviourSyncVar ____instanceHostPlayerNetId;

	protected NetworkBehaviourSyncVar ____instancePartyNetId;

	public int Network_instancePatternID
	{
		get
		{
			return _instancePatternID;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _instancePatternID, 1uL, null);
		}
	}

	public Player Network_instanceHostPlayer
	{
		get
		{
			return GetSyncVarNetworkBehaviour(____instanceHostPlayerNetId, ref _instanceHostPlayer);
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter_NetworkBehaviour(value, ref _instanceHostPlayer, 2uL, null, ref ____instanceHostPlayerNetId);
		}
	}

	public PartyObjectBehavior Network_instanceParty
	{
		get
		{
			return GetSyncVarNetworkBehaviour(____instancePartyNetId, ref _instanceParty);
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter_NetworkBehaviour(value, ref _instanceParty, 4uL, null, ref ____instancePartyNetId);
		}
	}

	public ZoneDifficulty Network_setDungeonDifficulty
	{
		get
		{
			return _setDungeonDifficulty;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _setDungeonDifficulty, 8uL, null);
		}
	}

	public int Network_dungeonKeysAquired
	{
		get
		{
			return _dungeonKeysAquired;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _dungeonKeysAquired, 16uL, On_DungeonKeyChange);
		}
	}

	public bool Network_isBossEngaged
	{
		get
		{
			return _isBossEngaged;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _isBossEngaged, 32uL, null);
		}
	}

	public bool Network_isBossDefeated
	{
		get
		{
			return _isBossDefeated;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _isBossDefeated, 64uL, null);
		}
	}

	public bool Network_allArenasBeaten
	{
		get
		{
			return _allArenasBeaten;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _allArenasBeaten, 128uL, null);
		}
	}

	public bool Network_generatingPattern
	{
		get
		{
			return _generatingPattern;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _generatingPattern, 256uL, null);
		}
	}

	private void Awake()
	{
		Allocate_PatternData();
	}

	protected override void OnValidate()
	{
		base.OnValidate();
		_mapInstance = GetComponent<MapInstance>();
		if ((bool)_muAmbienceSrc)
		{
			_muAmbienceSrc.volume = 0f;
		}
		if ((bool)_muActionSrc)
		{
			_muActionSrc.volume = 0f;
		}
	}

	[ContextMenu("Allocate Pattern Data")]
	public void Allocate_PatternData()
	{
		_mapInstance = GetComponent<MapInstance>();
		for (int i = 0; i < _instancePatterns.Length; i++)
		{
			if ((bool)_instancePatterns[i]._patternContainerObject)
			{
				_instancePatterns[i]._patternTag = $"INSTANCE PATTERN #{i}";
				_instancePatterns[i]._patternContainerObject.name = $"PATTERN CONTAINER #{i}";
				GameObject patternContainerObject = _instancePatterns[i]._patternContainerObject;
				NetworkIdentity[] componentsInChildren = patternContainerObject.GetComponentsInChildren<NetworkIdentity>(includeInactive: true);
				PatternInstanceObject[] componentsInChildren2 = patternContainerObject.GetComponentsInChildren<PatternInstanceObject>(includeInactive: true);
				Teleporter[] componentsInChildren3 = patternContainerObject.GetComponentsInChildren<Teleporter>(includeInactive: true);
				_instancePatterns[i]._netObjects = componentsInChildren;
				_instancePatterns[i]._localObjects = componentsInChildren2;
				_instancePatterns[i]._teleporters = componentsInChildren3;
			}
		}
	}

	private void On_DungeonKeyChange(int _old, int _new)
	{
		if ((bool)Player._mainPlayer && !Player._mainPlayer._bufferingStatus && (!Player._mainPlayer._isHostPlayer || !(Player._mainPlayer.gameObject.scene != base.gameObject.scene)))
		{
			PlayerSound pSound = Player._mainPlayer._pSound;
			ChatBehaviour chatBehaviour = Player._mainPlayer._chatBehaviour;
			if (_new > _old)
			{
				pSound._aSrcGeneral.PlayOneShot(pSound._pickupKeySound, 0.45f);
				chatBehaviour.New_ChatMessage("You received a dungeon key.");
			}
			if (_new < _old)
			{
				chatBehaviour.New_ChatMessage("A dungeon key dissipates...");
			}
		}
	}

	public override void OnStartServer()
	{
		base.OnStartServer();
		StartCoroutine(Server_StartupPatterns());
	}

	public override void OnStartClient()
	{
		base.OnStartClient();
		_minimapDeathSpotIcon.SetActive(value: false);
		if (base.isClientOnly)
		{
			StartCoroutine(Init_SetPattern());
		}
	}

	private void Update()
	{
		Server_IterateInstanceHostPlayer();
		Server_IterateHostObjects();
		if ((bool)_muActionSrc && (bool)_muAmbienceSrc && base.isServer && (bool)Player._mainPlayer)
		{
			_muAmbienceSrc.mute = Player._mainPlayer.gameObject.scene != base.gameObject.scene;
			_muActionSrc.mute = Player._mainPlayer.gameObject.scene != base.gameObject.scene;
			_muBossSrc.mute = Player._mainPlayer.gameObject.scene != base.gameObject.scene;
		}
		if ((bool)Player._mainPlayer)
		{
			if (Player._mainPlayer.Network_playerMapInstance != _mapInstance)
			{
				return;
			}
			if (Player._mainPlayer._currentPlayerCondition == PlayerCondition.DEAD)
			{
				_minimapDeathSpotIcon.transform.position = new Vector3(Player._mainPlayer.transform.position.x, _minimapDeathSpotIcon.transform.position.y, Player._mainPlayer.transform.position.z);
				_minimapDeathSpotIcon.SetActive(value: true);
			}
		}
		Client_HandleInstanceMusic();
		if ((bool)_normalBossLightOverride)
		{
			_normalBossLightOverride.SetActive(_setDungeonDifficulty != ZoneDifficulty.HARD);
		}
		if ((bool)_hardBossLightOverride)
		{
			_hardBossLightOverride.SetActive(_setDungeonDifficulty == ZoneDifficulty.HARD);
		}
		if ((bool)_bossLootChest)
		{
			switch (_setDungeonDifficulty)
			{
			case ZoneDifficulty.NORMAL:
				_bossLootChest._lootTableOverride = _normalBossLootTable;
				break;
			case ZoneDifficulty.HARD:
				_bossLootChest._lootTableOverride = _hardBossLootTable;
				break;
			}
		}
	}

	[ClientCallback]
	private void Client_HandleInstanceMusic()
	{
		if (!NetworkClient.active || !_muAmbienceSrc || !_muActionSrc)
		{
			return;
		}
		if (_setDungeonDifficulty == ZoneDifficulty.HARD && (bool)_hardBossTheme && !_isBossDefeated)
		{
			_muBossSrc.clip = _hardBossTheme;
		}
		bool flag = false;
		for (int i = 0; i < _setInstanceCreepArenas.Count; i++)
		{
			if (!(_setInstanceCreepArenas[i] == null))
			{
				if (_setInstanceCreepArenas[i]._arenaEnabled && _setInstanceCreepArenas[i]._creepSpawnerObject._playersWithinSpawnerRadius.Contains(Player._mainPlayer))
				{
					flag = true;
					break;
				}
				if (i >= _setInstancePattern._netObjects.Length - 1 && !_setInstanceCreepArenas[i]._arenaEnabled)
				{
					flag = false;
					break;
				}
			}
		}
		if (_isBossEngaged && _bossSpawner._playersWithinSpawnerRadius.Contains(Player._mainPlayer))
		{
			flag = true;
		}
		if (!Player._mainPlayer._bufferingStatus)
		{
			if (_isBossDefeated && (bool)_muBossSrc)
			{
				if (_muBossSrc.clip != _clearMusic)
				{
					_muBossSrc.volume = Mathf.Lerp(_muBossSrc.volume, 0f, 0.8f * Time.deltaTime);
					if (_muBossSrc.volume <= 0.008f)
					{
						_muBossSrc.Stop();
						_muBossSrc.clip = _clearMusic;
						_muBossSrc.Play();
					}
				}
				else
				{
					_muBossSrc.volume = Mathf.Lerp(_muBossSrc.volume, 1f, 0.8f * Time.deltaTime);
				}
				_muAmbienceSrc.volume = Mathf.Lerp(_muAmbienceSrc.volume, 0f, 0.8f * Time.deltaTime);
				_muActionSrc.volume = Mathf.Lerp(_muActionSrc.volume, 0f, 0.8f * Time.deltaTime);
			}
			else if (flag && Player._mainPlayer._currentPlayerCondition == PlayerCondition.ACTIVE)
			{
				_muAmbienceSrc.volume = Mathf.Lerp(_muAmbienceSrc.volume, 0f, 0.8f * Time.deltaTime);
				if (_isBossEngaged)
				{
					if (!_muBossSrc.isPlaying)
					{
						_muBossSrc.Play();
					}
					_muBossSrc.volume = Mathf.Lerp(_muBossSrc.volume, 1f, 0.6f * Time.deltaTime);
				}
				else
				{
					_muActionSrc.volume = Mathf.Lerp(_muActionSrc.volume, 1f, 0.6f * Time.deltaTime);
				}
			}
			else
			{
				_muAmbienceSrc.volume = Mathf.Lerp(_muAmbienceSrc.volume, 1f, 0.8f * Time.deltaTime);
				_muActionSrc.volume = Mathf.Lerp(_muActionSrc.volume, 0f, 0.6f * Time.deltaTime);
				_muBossSrc.volume = Mathf.Lerp(_muBossSrc.volume, 0f, 0.6f * Time.deltaTime);
			}
		}
		else
		{
			_muAmbienceSrc.volume = Mathf.Lerp(_muAmbienceSrc.volume, 0f, 0.8f * Time.deltaTime);
			_muActionSrc.volume = Mathf.Lerp(_muActionSrc.volume, 0f, 0.8f * Time.deltaTime);
			_muBossSrc.volume = Mathf.Lerp(_muBossSrc.volume, 0f, 0.8f * Time.deltaTime);
		}
	}

	[ServerCallback]
	private void Server_IterateInstanceHostPlayer()
	{
		if (!NetworkServer.active)
		{
			return;
		}
		if ((bool)Network_instanceHostPlayer)
		{
			if (Network_instanceHostPlayer.Network_playerMapInstance != _mapInstance)
			{
				Network_instanceHostPlayer = null;
			}
			if ((bool)Network_instanceHostPlayer)
			{
				Network_instanceParty = Network_instanceHostPlayer.Network_partyObject;
			}
		}
		else
		{
			Network_instanceParty = null;
			if (_mapInstance._peersInInstance.Count > 0)
			{
				Network_instanceHostPlayer = _mapInstance._peersInInstance[UnityEngine.Random.Range(0, _mapInstance._peersInInstance.Count)];
			}
		}
	}

	[ServerCallback]
	private void Server_IterateHostObjects()
	{
		if (!NetworkServer.active)
		{
			return;
		}
		Handle_BossSpawnerElements();
		for (int i = 0; i < _setInstanceCreepArenas.Count; i++)
		{
			if (!(_setInstanceCreepArenas[i] == null))
			{
				if (!_setInstanceCreepArenas[i]._arenaCompleted)
				{
					Network_allArenasBeaten = false;
					break;
				}
				if (i >= _setInstanceCreepArenas.Count - 1 && _setInstanceCreepArenas[i]._arenaCompleted)
				{
					Network_allArenasBeaten = true;
				}
			}
		}
		if (!Player._mainPlayer)
		{
			return;
		}
		_minimapCameraObject.SetActive(Player._mainPlayer.Network_playerMapInstance == _mapInstance);
		if (_cachedLocalRenderers != null && _cachedLocalRenderers.Length != 0)
		{
			Renderer[] cachedLocalRenderers = _cachedLocalRenderers;
			foreach (Renderer renderer in cachedLocalRenderers)
			{
				if (!(renderer == null))
				{
					renderer.enabled = Player._mainPlayer.Network_playerMapInstance == _mapInstance;
				}
			}
		}
		if (_cachedPatternASources != null && _cachedPatternASources.Length != 0)
		{
			AudioSource[] cachedPatternASources = _cachedPatternASources;
			foreach (AudioSource audioSource in cachedPatternASources)
			{
				if (!(audioSource == null))
				{
					audioSource.mute = Player._mainPlayer.Network_playerMapInstance != _mapInstance;
				}
			}
		}
		Teleporter[] teleporters = _setInstancePattern._teleporters;
		foreach (Teleporter teleporter in teleporters)
		{
			if (!(teleporter == null))
			{
				teleporter.gameObject.SetActive(Player._mainPlayer.Network_playerMapInstance == _mapInstance);
			}
		}
		void Handle_BossSpawnerElements()
		{
			if ((bool)_bossSpawner)
			{
				switch (_setDungeonDifficulty)
				{
				case ZoneDifficulty.NORMAL:
					if (!_normalBossSpawner)
					{
						return;
					}
					_bossSpawner._linkedSpawners = _normalBossSpawner._linkedSpawners;
					_bossSpawner._creepToSpawn = _normalBossSpawner._creepToSpawn;
					break;
				case ZoneDifficulty.HARD:
					if (!_hardBossSpawner)
					{
						return;
					}
					_bossSpawner._linkedSpawners = _hardBossSpawner._linkedSpawners;
					_bossSpawner._creepToSpawn = _hardBossSpawner._creepToSpawn;
					break;
				}
				Network_isBossEngaged = !_bossSpawner._disableSpawner && _bossSpawner._playersWithinSpawnerRadius.Count > 0 && _bossSpawner._spawnedCreeps.Count > 0 && (bool)_bossSpawner._spawnedCreeps[0].Network_aggroedEntity;
				if (_bossSpawner._spawnedCreeps.Count > 0)
				{
					Network_isBossDefeated = _bossSpawner._spawnedCreeps[0]._currentCreepCondition == CreepCondition.DEAD;
				}
			}
		}
	}

	[IteratorStateMachine(typeof(_003CServer_StartupPatterns_003Ed__44))]
	[ServerCallback]
	private IEnumerator Server_StartupPatterns()
	{
		if (!NetworkServer.active)
		{
			return null;
		}
		return new _003CServer_StartupPatterns_003Ed__44(0)
		{
			_003C_003E4__this = this
		};
	}

	private IEnumerator Init_SetPattern()
	{
		if (_isSettingPattern)
		{
			yield break;
		}
		_isSettingPattern = true;
		if (_generatingPattern && base.isClientOnly)
		{
			yield return null;
		}
		yield return new WaitForSeconds(1.2f);
		if (base.isClientOnly)
		{
			for (int i = 0; i < _instancePatterns.Length; i++)
			{
				Client_IteratePatterns(_instancePatterns[i]);
			}
		}
		if (base.isServer && _generatingPattern)
		{
			for (int j = 0; j < _instancePatterns.Length; j++)
			{
				Server_IteratePatterns(_instancePatterns[j]);
			}
		}
		yield return new WaitForSeconds(0.8f);
		_setInstancePattern = _instancePatterns[_instancePatternID];
		bool _inSameMapInstance = (bool)Player._mainPlayer && Player._mainPlayer.Network_playerMapInstance == _mapInstance;
		yield return new WaitForSeconds(0.8f);
		for (int k = 0; k < _instancePatterns.Length; k++)
		{
			Iterate_ClearPatternObjects(_instancePatterns[k]);
		}
		PatternInstanceObject[] localObjects = _setInstancePattern._localObjects;
		foreach (PatternInstanceObject patternInstanceObject in localObjects)
		{
			if (!(patternInstanceObject == null))
			{
				if (patternInstanceObject._hardDifficultyOnly)
				{
					patternInstanceObject.gameObject.SetActive(_setDungeonDifficulty == ZoneDifficulty.HARD);
				}
				else if (!patternInstanceObject.gameObject.activeInHierarchy)
				{
					patternInstanceObject.gameObject.SetActive(value: true);
				}
				if (patternInstanceObject.gameObject.activeInHierarchy && !patternInstanceObject.GetComponent<NetworkIdentity>())
				{
					patternInstanceObject.transform.SetParent(_mapInstance._environmentGroup.transform);
				}
			}
		}
		Teleporter[] teleporters = _setInstancePattern._teleporters;
		foreach (Teleporter teleporter in teleporters)
		{
			if (!(teleporter == null))
			{
				teleporter.transform.SetParent(_mapInstance._environmentGroup.transform);
				if (base.isClientOnly)
				{
					teleporter.gameObject.SetActive(_inSameMapInstance);
					teleporter.Enable_Teleporter();
				}
			}
		}
		NetworkIdentity[] netObjects = _setInstancePattern._netObjects;
		foreach (NetworkIdentity networkIdentity in netObjects)
		{
			if (networkIdentity == null)
			{
				continue;
			}
			if (base.isServer && _generatingPattern)
			{
				if (networkIdentity.TryGetComponent<CreepArena>(out var component))
				{
					_setInstanceCreepArenas.Add(component);
					component._patternInstanceManager = this;
				}
				networkIdentity.transform.SetParent(null);
				networkIdentity.transform.SetAsLastSibling();
				NetworkServer.Spawn(networkIdentity.gameObject);
			}
			if (networkIdentity.TryGetComponent<BossRoomTeleporter>(out var component2))
			{
				_bossRoomTeleporter = component2;
				component2._patternInstanceManager = this;
			}
			if (networkIdentity.TryGetComponent<ItemDropEntity_Chest>(out var component3))
			{
				component3._lootTableOverride = _difficultyChestLootTables[(uint)_setDungeonDifficulty];
			}
			if (networkIdentity.TryGetComponent<BreakableObject>(out var component4))
			{
				component4._itemDropEntity._lootTableOverride = _difficultyBreakableLootTables[(uint)_setDungeonDifficulty];
			}
		}
		if (base.isServer && _generatingPattern)
		{
			for (int m = 0; m < _instancePatterns.Length; m++)
			{
				if (m != _instancePatternID)
				{
					for (int n = 0; n < _instancePatterns[m]._netObjects.Length; n++)
					{
						NetworkServer.Destroy(_instancePatterns[m]._netObjects[n].gameObject);
					}
					for (int num = 0; num < _instancePatterns[m]._teleporters.Length; num++)
					{
						UnityEngine.Object.Destroy(_instancePatterns[m]._teleporters[num].gameObject);
					}
					_instancePatterns[m]._localObjects = new PatternInstanceObject[0];
					_instancePatterns[m]._netObjects = new NetworkIdentity[0];
					_instancePatterns[m]._teleporters = new Teleporter[0];
				}
			}
			Network_generatingPattern = false;
		}
		List<Renderer> list = new List<Renderer>();
		List<AudioSource> list2 = new List<AudioSource>();
		localObjects = _setInstancePattern._localObjects;
		foreach (PatternInstanceObject patternInstanceObject2 in localObjects)
		{
			if (!(patternInstanceObject2 == null))
			{
				if (_cachedLocalRenderers != null && _cachedLocalRenderers.Length != 0)
				{
					break;
				}
				Renderer[] componentsInChildren = patternInstanceObject2.gameObject.GetComponentsInChildren<Renderer>();
				list.AddRange(componentsInChildren);
				AudioSource[] componentsInChildren2 = patternInstanceObject2.gameObject.GetComponentsInChildren<AudioSource>();
				list2.AddRange(componentsInChildren2);
			}
		}
		teleporters = _setInstancePattern._teleporters;
		foreach (Teleporter teleporter2 in teleporters)
		{
			if (!(teleporter2 == null))
			{
				if (_cachedPatternASources != null && _cachedPatternASources.Length != 0)
				{
					break;
				}
				AudioSource[] componentsInChildren3 = teleporter2.gameObject.GetComponentsInChildren<AudioSource>();
				list2.AddRange(componentsInChildren3);
			}
		}
		netObjects = _setInstancePattern._netObjects;
		foreach (NetworkIdentity networkIdentity2 in netObjects)
		{
			if (!(networkIdentity2 == null))
			{
				if (_cachedPatternASources != null && _cachedPatternASources.Length != 0)
				{
					break;
				}
				AudioSource[] componentsInChildren4 = networkIdentity2.gameObject.GetComponentsInChildren<AudioSource>();
				list2.AddRange(componentsInChildren4);
			}
		}
		if (list.Count > 0)
		{
			_cachedLocalRenderers = list.ToArray();
		}
		if (list2.Count > 0)
		{
			_cachedPatternASources = list2.ToArray();
		}
		for (int num2 = 0; num2 < _instancePatterns.Length; num2++)
		{
			if (num2 != _instancePatternID)
			{
				_instancePatterns[num2]._patternContainerObject.SetActive(value: false);
			}
		}
		_isSettingPattern = false;
		void Client_IteratePatterns(InstancePattern _setPattern)
		{
			PatternInstanceObject[] localObjects2 = _setPattern._localObjects;
			foreach (PatternInstanceObject patternInstanceObject3 in localObjects2)
			{
				if (!(patternInstanceObject3 == null) && !patternInstanceObject3.GetComponent<NetworkIdentity>())
				{
					patternInstanceObject3.gameObject.SetActive(value: false);
					patternInstanceObject3.transform.SetParent(_mapInstance._environmentGroup.transform);
				}
			}
			Teleporter[] teleporters2 = _setPattern._teleporters;
			foreach (Teleporter teleporter3 in teleporters2)
			{
				if (!(teleporter3 == null))
				{
					teleporter3.Disable_Teleporter();
					teleporter3.transform.SetParent(_mapInstance._environmentGroup.transform);
					teleporter3.gameObject.SetActive(value: false);
				}
			}
		}
		void Iterate_ClearPatternObjects(InstancePattern _iteratedPattern)
		{
			if (_iteratedPattern != _setInstancePattern)
			{
				PatternInstanceObject[] localObjects2 = _iteratedPattern._localObjects;
				foreach (PatternInstanceObject patternInstanceObject3 in localObjects2)
				{
					if (!(patternInstanceObject3 == null))
					{
						UnityEngine.Object.Destroy(patternInstanceObject3.gameObject);
					}
				}
			}
		}
		static void Server_IteratePatterns(InstancePattern _setPattern)
		{
			NetworkIdentity[] netObjects2 = _setPattern._netObjects;
			foreach (NetworkIdentity networkIdentity3 in netObjects2)
			{
				if (!(networkIdentity3 == null))
				{
					NetworkServer.UnSpawn(networkIdentity3.gameObject);
				}
			}
		}
	}

	[CompilerGenerated]
	private void _003CServer_StartupPatterns_003Eg__Iterate_StartupInstancePattern_007C44_0(InstancePattern _setPattern)
	{
		for (int i = 0; i < _setPattern._netObjects.Length; i++)
		{
			if (_setPattern._netObjects[i].transform.parent != null)
			{
				_setPattern._netObjects[i].transform.SetParent(null);
				_setPattern._netObjects[i].transform.SetAsLastSibling();
			}
		}
		Teleporter[] teleporters = _setPattern._teleporters;
		foreach (Teleporter teleporter in teleporters)
		{
			if (!(teleporter == null))
			{
				if (teleporter.transform.parent != _mapInstance._environmentGroup.transform)
				{
					teleporter.transform.SetParent(_mapInstance._environmentGroup.transform);
				}
				teleporter.gameObject.SetActive(value: false);
			}
		}
		PatternInstanceObject[] localObjects = _setPattern._localObjects;
		foreach (PatternInstanceObject patternInstanceObject in localObjects)
		{
			if (!(patternInstanceObject == null))
			{
				if ((bool)patternInstanceObject.GetComponent<NetworkIdentity>())
				{
					patternInstanceObject.transform.SetParent(null);
					NetworkServer.UnSpawn(patternInstanceObject.gameObject);
				}
				else if (patternInstanceObject.transform.parent != _mapInstance._environmentGroup.transform)
				{
					patternInstanceObject.transform.SetParent(_mapInstance._environmentGroup.transform);
					patternInstanceObject.gameObject.SetActive(value: false);
				}
			}
		}
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
			writer.WriteInt(_instancePatternID);
			writer.WriteNetworkBehaviour(Network_instanceHostPlayer);
			writer.WriteNetworkBehaviour(Network_instanceParty);
			GeneratedNetworkCode._Write_ZoneDifficulty(writer, _setDungeonDifficulty);
			writer.WriteInt(_dungeonKeysAquired);
			writer.WriteBool(_isBossEngaged);
			writer.WriteBool(_isBossDefeated);
			writer.WriteBool(_allArenasBeaten);
			writer.WriteBool(_generatingPattern);
			return;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteInt(_instancePatternID);
		}
		if ((base.syncVarDirtyBits & 2L) != 0L)
		{
			writer.WriteNetworkBehaviour(Network_instanceHostPlayer);
		}
		if ((base.syncVarDirtyBits & 4L) != 0L)
		{
			writer.WriteNetworkBehaviour(Network_instanceParty);
		}
		if ((base.syncVarDirtyBits & 8L) != 0L)
		{
			GeneratedNetworkCode._Write_ZoneDifficulty(writer, _setDungeonDifficulty);
		}
		if ((base.syncVarDirtyBits & 0x10L) != 0L)
		{
			writer.WriteInt(_dungeonKeysAquired);
		}
		if ((base.syncVarDirtyBits & 0x20L) != 0L)
		{
			writer.WriteBool(_isBossEngaged);
		}
		if ((base.syncVarDirtyBits & 0x40L) != 0L)
		{
			writer.WriteBool(_isBossDefeated);
		}
		if ((base.syncVarDirtyBits & 0x80L) != 0L)
		{
			writer.WriteBool(_allArenasBeaten);
		}
		if ((base.syncVarDirtyBits & 0x100L) != 0L)
		{
			writer.WriteBool(_generatingPattern);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize(ref _instancePatternID, null, reader.ReadInt());
			GeneratedSyncVarDeserialize_NetworkBehaviour(ref _instanceHostPlayer, null, reader, ref ____instanceHostPlayerNetId);
			GeneratedSyncVarDeserialize_NetworkBehaviour(ref _instanceParty, null, reader, ref ____instancePartyNetId);
			GeneratedSyncVarDeserialize(ref _setDungeonDifficulty, null, GeneratedNetworkCode._Read_ZoneDifficulty(reader));
			GeneratedSyncVarDeserialize(ref _dungeonKeysAquired, On_DungeonKeyChange, reader.ReadInt());
			GeneratedSyncVarDeserialize(ref _isBossEngaged, null, reader.ReadBool());
			GeneratedSyncVarDeserialize(ref _isBossDefeated, null, reader.ReadBool());
			GeneratedSyncVarDeserialize(ref _allArenasBeaten, null, reader.ReadBool());
			GeneratedSyncVarDeserialize(ref _generatingPattern, null, reader.ReadBool());
			return;
		}
		long num = (long)reader.ReadULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _instancePatternID, null, reader.ReadInt());
		}
		if ((num & 2L) != 0L)
		{
			GeneratedSyncVarDeserialize_NetworkBehaviour(ref _instanceHostPlayer, null, reader, ref ____instanceHostPlayerNetId);
		}
		if ((num & 4L) != 0L)
		{
			GeneratedSyncVarDeserialize_NetworkBehaviour(ref _instanceParty, null, reader, ref ____instancePartyNetId);
		}
		if ((num & 8L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _setDungeonDifficulty, null, GeneratedNetworkCode._Read_ZoneDifficulty(reader));
		}
		if ((num & 0x10L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _dungeonKeysAquired, On_DungeonKeyChange, reader.ReadInt());
		}
		if ((num & 0x20L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _isBossEngaged, null, reader.ReadBool());
		}
		if ((num & 0x40L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _isBossDefeated, null, reader.ReadBool());
		}
		if ((num & 0x80L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _allArenasBeaten, null, reader.ReadBool());
		}
		if ((num & 0x100L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _generatingPattern, null, reader.ReadBool());
		}
	}
}
