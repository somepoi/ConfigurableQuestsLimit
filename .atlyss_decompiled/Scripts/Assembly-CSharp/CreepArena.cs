using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Mirror;
using UnityEngine;

public class CreepArena : NetworkBehaviour
{
	[HideInInspector]
	public CreepSpawner _creepSpawnerObject;

	[HideInInspector]
	public PatternInstanceManager _patternInstanceManager;

	[Header("Creep Arena")]
	[SerializeField]
	private CreepArenaSlot[] _creepArenaSlots = new CreepArenaSlot[0];

	public CreepArenaSlot _setArenaSlot;

	[Space]
	[SyncVar]
	public bool _arenaReady = true;

	[SyncVar(hook = "OnArenaEnabled")]
	public bool _arenaEnabled;

	public bool _oneTimeArena;

	public bool _arenaCompleted;

	private bool _startingNextWave;

	[Header("Net Trigger Events")]
	[SerializeField]
	private TriggerMessage _triggerMessage;

	[SerializeField]
	private NetTrigger _counterTrigger;

	[SerializeField]
	private NetTrigger[] _triggersToActivate = new NetTrigger[0];

	[Header("Runtime")]
	[SyncVar]
	public int _currentWave;

	[SerializeField]
	private float _arenaResetTime = 45.6f;

	[HideInInspector]
	public int _killedCreepsInWave;

	public float _currentResetBuffer;

	private int _maxCreepsToKill;

	[Header("Chests")]
	[SerializeField]
	private ItemDropEntity_Chest[] _creepArenaChests = new ItemDropEntity_Chest[0];

	[SerializeField]
	private bool _randomize_activateChests;

	[Header("Colliders")]
	[SerializeField]
	private LerpPosition[] _wallDisplayLerpPositions = new LerpPosition[0];

	[SerializeField]
	private Teleporter[] _arenaJoinTeleports = new Teleporter[0];

	private float _colliderBuffer;

	[Header("Visuals / Audio")]
	[SerializeField]
	private AudioSource _arenaInitAsrc;

	[SerializeField]
	private ParticleSystem _creepArenaActiveEffect;

	[SerializeField]
	private ParticleSystem _initCreepArenaEffect;

	public bool Network_arenaReady
	{
		get
		{
			return _arenaReady;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _arenaReady, 1uL, null);
		}
	}

	public bool Network_arenaEnabled
	{
		get
		{
			return _arenaEnabled;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _arenaEnabled, 2uL, OnArenaEnabled);
		}
	}

	public int Network_currentWave
	{
		get
		{
			return _currentWave;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _currentWave, 4uL, null);
		}
	}

	protected override void OnValidate()
	{
		base.OnValidate();
		for (int i = 0; i < _creepArenaSlots.Length; i++)
		{
			_creepArenaSlots[i]._arenaSlotTag = $"Arena Slot #{i} ({_creepArenaSlots[i]._zoneDifficulty})";
			for (int j = 0; j < _creepArenaSlots[i]._creepArenaWaves.Length; j++)
			{
				_creepArenaSlots[i]._creepArenaWaves[j]._arenaWaveTag = $"Arena Slot #{i} / Wave #{j}";
			}
		}
	}

	private void Awake()
	{
		_creepSpawnerObject = GetComponent<CreepSpawner>();
		_setArenaSlot = null;
	}

	public override void OnStartServer()
	{
		base.OnStartServer();
		for (int i = 0; i < _creepArenaChests.Length; i++)
		{
			_creepArenaChests[i].Network_isChestHidden = true;
		}
	}

	private void OnArenaEnabled(bool _oldBool, bool _newBool)
	{
		if ((!Player._mainPlayer._isHostPlayer || !(Player._mainPlayer.gameObject.scene != base.gameObject.scene)) && _newBool)
		{
			_colliderBuffer = 0.6f;
			_initCreepArenaEffect.Play();
			_arenaInitAsrc.Play();
		}
	}

	private void Update()
	{
		Client_HandleCreepArenaParams();
		Server_HandleCreepArenaParams();
	}

	[ClientCallback]
	private void Client_HandleCreepArenaParams()
	{
		if (!NetworkClient.active || !Player._mainPlayer)
		{
			return;
		}
		if (Player._mainPlayer._isHostPlayer && Player._mainPlayer.gameObject.scene != base.gameObject.scene)
		{
			_creepArenaActiveEffect.Stop();
			return;
		}
		if (_colliderBuffer > 0f)
		{
			_colliderBuffer -= Time.deltaTime;
		}
		else
		{
			_colliderBuffer = 0f;
		}
		for (int i = 0; i < _wallDisplayLerpPositions.Length; i++)
		{
			int index = (_arenaEnabled ? 1 : 0);
			_wallDisplayLerpPositions[i].Apply_PositionIndex(index);
		}
		for (int j = 0; j < _arenaJoinTeleports.Length; j++)
		{
			if (_arenaEnabled && !_arenaJoinTeleports[j]._isEnabled)
			{
				_arenaJoinTeleports[j].Enable_Teleporter();
			}
			if (!_arenaEnabled && _arenaJoinTeleports[j]._isEnabled)
			{
				_arenaJoinTeleports[j].Disable_Teleporter();
			}
		}
		bool flag = Vector3.Distance(Player._mainPlayer.transform.position, base.transform.position) <= 120f;
		if ((_arenaReady && !_creepArenaActiveEffect.isPlaying) & flag)
		{
			_creepArenaActiveEffect.Play();
		}
		if ((!_arenaReady && _creepArenaActiveEffect.isPlaying) || !flag)
		{
			_creepArenaActiveEffect.Stop();
		}
	}

	[ServerCallback]
	private void Server_HandleCreepArenaParams()
	{
		if (!NetworkServer.active)
		{
			return;
		}
		if (_creepSpawnerObject._disableSpawner)
		{
			_setArenaSlot = null;
		}
		Network_arenaReady = _currentResetBuffer <= 0f;
		if (!_arenaEnabled)
		{
			if (_currentResetBuffer > 0f)
			{
				if (!_oneTimeArena)
				{
					_currentResetBuffer -= Time.deltaTime;
				}
				return;
			}
			for (int i = 0; i < _creepArenaChests.Length; i++)
			{
				_creepArenaChests[i].Network_isChestHidden = true;
			}
			_currentResetBuffer = 0f;
		}
		else
		{
			Handle_CurrentArenaSlot();
		}
		void Handle_CurrentArenaSlot()
		{
			if (_setArenaSlot != null)
			{
				_creepSpawnerObject._creepCount = _setArenaSlot._creepArenaWaves[_currentWave]._creepPool.Length;
				_maxCreepsToKill = _setArenaSlot._creepArenaWaves[_currentWave]._creepPool.Length;
				if (_currentWave >= _setArenaSlot._creepArenaWaves.Length - 1 && _killedCreepsInWave >= _maxCreepsToKill)
				{
					Server_EndCreepArenaSession();
				}
				else if (_killedCreepsInWave >= _setArenaSlot._creepArenaWaves[_currentWave]._creepPool.Length && !_startingNextWave)
				{
					StartCoroutine(Init_NextWave());
				}
			}
		}
	}

	private IEnumerator Init_NextWave()
	{
		_startingNextWave = true;
		_creepSpawnerObject.Init_DestroyAllSpawnedCreeps();
		_killedCreepsInWave = 0;
		Network_currentWave = _currentWave + 1;
		yield return new WaitForSeconds(1f);
		_creepSpawnerObject.Start_EnemySpawns();
		_startingNextWave = false;
	}

	[ServerCallback]
	public void Server_InitArenaSession()
	{
		if (!NetworkServer.active || (_oneTimeArena && _arenaCompleted))
		{
			return;
		}
		List<CreepArenaSlot> list = new List<CreepArenaSlot>();
		for (int i = 0; i < _creepArenaSlots.Length; i++)
		{
			if ((!_patternInstanceManager || _patternInstanceManager._setDungeonDifficulty == _creepArenaSlots[i]._zoneDifficulty) && (_creepArenaSlots[i]._arenaTime == WorldTime.NULL || GameWorldManager._current._worldTime == _creepArenaSlots[i]._arenaTime))
			{
				list.Add(_creepArenaSlots[i]);
			}
		}
		if (list.Count <= 0)
		{
			Debug.LogWarning(base.gameObject.name + " has no arena slots.");
			return;
		}
		_setArenaSlot = list[Random.Range(0, list.Count)];
		_creepSpawnerObject.Enable_Spawner();
		_currentResetBuffer = _arenaResetTime;
		_killedCreepsInWave = 0;
		Network_arenaEnabled = true;
		Init_SendTriggerMessage();
	}

	[ServerCallback]
	public void Server_ResetCreepArena()
	{
		if (NetworkServer.active && _arenaEnabled)
		{
			_maxCreepsToKill = 0;
			_killedCreepsInWave = 0;
			Network_currentWave = 0;
			Network_arenaEnabled = false;
			_setArenaSlot = null;
			Network_arenaReady = true;
		}
	}

	[ServerCallback]
	private void Server_EndCreepArenaSession()
	{
		if (!NetworkServer.active || !_arenaEnabled || _creepSpawnerObject._disableSpawner)
		{
			return;
		}
		_maxCreepsToKill = 0;
		_killedCreepsInWave = 0;
		Network_currentWave = 0;
		Network_arenaEnabled = false;
		_setArenaSlot = null;
		if (_oneTimeArena)
		{
			_arenaCompleted = true;
		}
		_creepSpawnerObject.Disable_Spawner();
		int num = 0;
		for (int i = 0; i < _creepArenaChests.Length; i++)
		{
			if (_randomize_activateChests)
			{
				if (Random.Range(0, 2) <= 0)
				{
					_creepArenaChests[i].Network_isChestHidden = false;
					_creepArenaChests[i].Network_isChestOpen = false;
					num++;
				}
			}
			else
			{
				_creepArenaChests[i].Network_isChestHidden = false;
				_creepArenaChests[i].Network_isChestOpen = false;
			}
		}
		if (num <= 0 && _creepArenaChests != null && _creepArenaChests.Length != 0 && _randomize_activateChests)
		{
			_creepArenaChests[Random.Range(0, _creepArenaChests.Length)].Network_isChestHidden = false;
			_creepArenaChests[Random.Range(0, _creepArenaChests.Length)].Network_isChestOpen = false;
		}
		for (int j = 0; j < _triggersToActivate.Length; j++)
		{
			_triggersToActivate[j].Network_deactivateTrigger = false;
		}
		if ((bool)_counterTrigger)
		{
			_counterTrigger.Add_Counter();
		}
	}

	[ServerCallback]
	private void Init_SendTriggerMessage()
	{
		if (!NetworkServer.active)
		{
			return;
		}
		string text = _triggerMessage._singleMessage;
		if (_triggerMessage._triggerMessageArray.Length != 0)
		{
			text = _triggerMessage._triggerMessageArray[Random.Range(0, _triggerMessage._triggerMessageArray.Length)];
		}
		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		ChatBehaviour[] array = Object.FindObjectsOfType<ChatBehaviour>();
		foreach (ChatBehaviour chatBehaviour in array)
		{
			if (!(chatBehaviour == null) && !(chatBehaviour.gameObject.scene != base.gameObject.scene))
			{
				if (_triggerMessage._sentMsg)
				{
					break;
				}
				chatBehaviour.Target_RecieveTriggerMessage(text);
			}
		}
		_triggerMessage._sentMsg = true;
	}

	[ServerCallback]
	public void Refresh_Arena()
	{
		if (NetworkServer.active)
		{
			_currentResetBuffer = 0f;
			_arenaCompleted = false;
			Server_ResetCreepArena();
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
			writer.WriteBool(_arenaReady);
			writer.WriteBool(_arenaEnabled);
			writer.WriteInt(_currentWave);
			return;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteBool(_arenaReady);
		}
		if ((base.syncVarDirtyBits & 2L) != 0L)
		{
			writer.WriteBool(_arenaEnabled);
		}
		if ((base.syncVarDirtyBits & 4L) != 0L)
		{
			writer.WriteInt(_currentWave);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize(ref _arenaReady, null, reader.ReadBool());
			GeneratedSyncVarDeserialize(ref _arenaEnabled, OnArenaEnabled, reader.ReadBool());
			GeneratedSyncVarDeserialize(ref _currentWave, null, reader.ReadInt());
			return;
		}
		long num = (long)reader.ReadULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _arenaReady, null, reader.ReadBool());
		}
		if ((num & 2L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _arenaEnabled, OnArenaEnabled, reader.ReadBool());
		}
		if ((num & 4L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _currentWave, null, reader.ReadInt());
		}
	}
}
