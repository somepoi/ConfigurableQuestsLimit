using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Mirror;
using Mirror.RemoteCalls;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreepSpawner : NetworkBehaviour
{
	[CompilerGenerated]
	private sealed class _003CCreepSpawn_003Ed__62 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public CreepSpawner _003C_003E4__this;

		public Creep _baseCreepObj;

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
		public _003CCreepSpawn_003Ed__62(int _003C_003E1__state)
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
			CreepSpawner creepSpawner = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
			{
				_003C_003E1__state = -1;
				if (creepSpawner._spawnedCreeps.Count <= 0)
				{
					return false;
				}
				if (!_baseCreepObj)
				{
					return false;
				}
				_baseCreepObj._scriptStatModifier = null;
				if ((bool)creepSpawner._spawnerStatModifierTable && (bool)creepSpawner._creepArena && creepSpawner._creepArena._setArenaSlot != null && creepSpawner._creepArena._setArenaSlot._zoneDifficulty != ZoneDifficulty.EASY)
				{
					for (int j = 0; j < creepSpawner._spawnerStatModifierTable._statModifierSlots.Length; j++)
					{
						if (UnityEngine.Random.Range(0f, 1f) <= creepSpawner._spawnerStatModifierTable._statModifierSlots[j]._modifierChance && (!creepSpawner._spawnerStatModifierTable._statModifierSlots[j]._equipModifier._modifierElement || !_baseCreepObj._scriptCreep._combatElement))
						{
							_baseCreepObj._scriptStatModifier = creepSpawner._spawnerStatModifierTable._statModifierSlots[j]._equipModifier;
							break;
						}
					}
				}
				Vector3 position = creepSpawner.transform.position + Vector3.up * 5f;
				Quaternion rotation = creepSpawner.transform.rotation;
				if (creepSpawner._physicsScene.Raycast(creepSpawner.transform.position + Vector3.up * 5f, Vector3.down, out var hitInfo, float.PositiveInfinity, CastLayers.GroundMask))
				{
					position = hitInfo.point + Vector3.up * 5f;
				}
				if (creepSpawner._spawnPositions != null && creepSpawner._spawnPositions.Length != 0)
				{
					position = creepSpawner._spawnPositions[creepSpawner._currentSpawnIndex].position + UnityEngine.Random.insideUnitSphere * 8.5f;
					position.y = creepSpawner.transform.position.y;
					position += Vector3.up * 5f;
					rotation = creepSpawner._spawnPositions[creepSpawner._currentSpawnIndex].rotation;
					creepSpawner.CurrentSpawnIndex++;
				}
				_baseCreepObj.transform.SetPositionAndRotation(position, rotation);
				_baseCreepObj.BaseCreep_OnSpawn();
				_003C_003E2__current = new WaitForSeconds(0.4f);
				_003C_003E1__state = 1;
				return true;
			}
			case 1:
				_003C_003E1__state = -1;
				if (creepSpawner._aggroOnSpawn)
				{
					if (!_baseCreepObj)
					{
						return false;
					}
					if ((bool)_baseCreepObj.Network_aggroedEntity)
					{
						return false;
					}
					if (creepSpawner._playersWithinSpawnerRadius.Count > 0)
					{
						for (int i = 0; i < creepSpawner._playersWithinSpawnerRadius.Count; i++)
						{
							if (!(creepSpawner._playersWithinSpawnerRadius[i] == null) && creepSpawner._playersWithinSpawnerRadius[i]._currentPlayerCondition == PlayerCondition.ACTIVE && !creepSpawner._playersWithinSpawnerRadius[i]._isHidden)
							{
								_baseCreepObj.Network_aggroedEntity = creepSpawner._playersWithinSpawnerRadius[i]._statusEntity;
								_baseCreepObj.Server_AddThreat(creepSpawner._playersWithinSpawnerRadius[i]._statusEntity, 5f);
								_baseCreepObj.Server_AggroIcon(1);
								break;
							}
						}
					}
				}
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

	[CompilerGenerated]
	private sealed class _003CDestroy_AllSpawnedCreeps_003Ed__64 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public CreepSpawner _003C_003E4__this;

		public float _time;

		private List<Creep> _003C_aggroedCreepList_003E5__2;

		private int _003Ci_003E5__3;

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
		public _003CDestroy_AllSpawnedCreeps_003Ed__64(int _003C_003E1__state)
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
			CreepSpawner creepSpawner = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				if (creepSpawner._spawnedCreeps.Count <= 0)
				{
					return false;
				}
				if (creepSpawner._isDestroyingCreeps)
				{
					return false;
				}
				_003C_aggroedCreepList_003E5__2 = new List<Creep>();
				creepSpawner._isDestroyingCreeps = true;
				_003C_003E2__current = new WaitForSeconds(_time);
				_003C_003E1__state = 1;
				return true;
			case 1:
			{
				_003C_003E1__state = -1;
				for (int i = 0; i < creepSpawner._spawnedCreeps.Count; i++)
				{
					if (!(creepSpawner._spawnedCreeps[i] == null))
					{
						if ((bool)creepSpawner._spawnedCreeps[i].Network_aggroedEntity && !creepSpawner._forceDeleteAggroedCreeps && !creepSpawner._creepArena)
						{
							_003C_aggroedCreepList_003E5__2.Add(creepSpawner._spawnedCreeps[i]);
						}
						else
						{
							_003CDestroy_AllSpawnedCreeps_003Eg__Destroy_SingleCreep_007C64_0(creepSpawner._spawnedCreeps[i].gameObject);
						}
					}
				}
				goto IL_01d5;
			}
			case 2:
				_003C_003E1__state = -1;
				_003CDestroy_AllSpawnedCreeps_003Eg__Destroy_SingleCreep_007C64_0(_003C_aggroedCreepList_003E5__2[_003Ci_003E5__3].gameObject);
				goto IL_0198;
			case 3:
				{
					_003C_003E1__state = -1;
					goto IL_01d5;
				}
				IL_01d5:
				if (_003C_aggroedCreepList_003E5__2.Count <= 0 || creepSpawner._forceDeleteAggroedCreeps)
				{
					break;
				}
				_003Ci_003E5__3 = 0;
				goto IL_01a8;
				IL_0198:
				_003Ci_003E5__3++;
				goto IL_01a8;
				IL_01a8:
				if (_003Ci_003E5__3 < _003C_aggroedCreepList_003E5__2.Count)
				{
					if (_003C_aggroedCreepList_003E5__2[_003Ci_003E5__3] == null)
					{
						_003C_aggroedCreepList_003E5__2.RemoveAt(_003Ci_003E5__3);
					}
					else if (!_003C_aggroedCreepList_003E5__2[_003Ci_003E5__3].Network_aggroedEntity)
					{
						_003C_003E2__current = new WaitForSeconds(0.8f);
						_003C_003E1__state = 2;
						return true;
					}
					goto IL_0198;
				}
				_003C_003E2__current = null;
				_003C_003E1__state = 3;
				return true;
			}
			creepSpawner.CurrentSpawnIndex = 0;
			creepSpawner._spawnedCreeps.Clear();
			creepSpawner._isDestroyingCreeps = false;
			return false;
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

	[CompilerGenerated]
	private sealed class _003CInit_SpawnedCreeps_003Ed__59 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public CreepSpawner _003C_003E4__this;

		public float _timeBeforeSpawnInit;

		private int _003C_setCreepCount_003E5__2;

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
		public _003CInit_SpawnedCreeps_003Ed__59(int _003C_003E1__state)
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
			CreepSpawner creepSpawner = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				if (creepSpawner._spawnerWorldTime != WorldTime.NULL && GameWorldManager._current._worldTime != creepSpawner._spawnerWorldTime)
				{
					return false;
				}
				if (creepSpawner._disableSpawner)
				{
					return false;
				}
				creepSpawner._isSpawningCreeps = true;
				goto IL_0063;
			case 1:
				_003C_003E1__state = -1;
				if (creepSpawner._isDestroyingCreeps)
				{
					goto IL_0063;
				}
				goto IL_0082;
			case 2:
				_003C_003E1__state = -1;
				if (!creepSpawner._creepSpawnerReigon || creepSpawner._creepSpawnerReigon._playersInReigon.Count > 0)
				{
					if (creepSpawner._spawnerWorldTime != WorldTime.NULL && GameWorldManager._current._worldTime != creepSpawner._spawnerWorldTime)
					{
						creepSpawner._isSpawningCreeps = false;
						return false;
					}
					_003C_setCreepCount_003E5__2 = creepSpawner._creepCount;
					if ((bool)creepSpawner._creepArena && creepSpawner._creepArena._setArenaSlot != null)
					{
						_003C_setCreepCount_003E5__2 = creepSpawner._creepArena._setArenaSlot._creepArenaWaves[creepSpawner._creepArena._currentWave]._creepPool.Length;
					}
					_003C_003E2__current = new WaitForSeconds(_timeBeforeSpawnInit);
					_003C_003E1__state = 3;
					return true;
				}
				goto IL_0082;
			case 3:
				_003C_003E1__state = -1;
				creepSpawner.CurrentSpawnIndex = UnityEngine.Random.Range(0, creepSpawner._spawnPositions.Length);
				goto IL_0161;
			case 4:
				_003C_003E1__state = -1;
				goto IL_01d0;
			case 5:
				{
					_003C_003E1__state = -1;
					if (_003C_setCreepCount_003E5__2 <= 0)
					{
						creepSpawner._isSpawningCreeps = false;
						return false;
					}
					goto IL_0161;
				}
				IL_0082:
				_003C_003E2__current = null;
				_003C_003E1__state = 2;
				return true;
				IL_0161:
				if (creepSpawner._isSpecialSpawn)
				{
					Vector3 position = creepSpawner._spawnPositions[creepSpawner._currentSpawnIndex].position;
					position.y = creepSpawner.transform.position.y;
					position += Vector3.up * 5f;
					creepSpawner.Rpc_InitSpecialSpawnEffect(position);
					_003C_003E2__current = new WaitForSeconds(1.95f);
					_003C_003E1__state = 4;
					return true;
				}
				goto IL_01d0;
				IL_01d0:
				creepSpawner.Server_NewCreepObject();
				_003C_setCreepCount_003E5__2--;
				_003C_003E2__current = new WaitForSeconds(0.8f);
				_003C_003E1__state = 5;
				return true;
				IL_0063:
				_003C_003E2__current = null;
				_003C_003E1__state = 1;
				return true;
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

	public MapInstance _mapInstance;

	[HideInInspector]
	public CreepArena _creepArena;

	[HideInInspector]
	public CreepSpawnerReigon _creepSpawnerReigon;

	public Collider _mainCollider;

	private PhysicsScene _physicsScene;

	[Header("Creep Spawner")]
	[SerializeField]
	private WorldTime _spawnerWorldTime = WorldTime.NULL;

	public ScriptableCreep _creepToSpawn;

	[Tooltip("If applied, a spawned creep in a normal+ Difficulty instance will have a chance to have a stat modifier assigned to them.")]
	public ScriptableStatModifierTable _spawnerStatModifierTable;

	[Tooltip("The amount of creeps to spawn.")]
	public int _creepCount = 1;

	[Space]
	[Tooltip("The time before the spawner begins spawning creeps as soon as it's enabled.")]
	[SerializeField]
	private float _spawnInitializeTime = 0.8f;

	[Tooltip("The time before the spawner begins destroying creeps as soon as it's disabled.")]
	[SerializeField]
	private float _destroyInitializeTime = 1.25f;

	[Space]
	[Tooltip("The general time it takes to call for a respawn on the creep spawner for dead creeps. Chance % is provided on the spawners when the buffer hits 0.0f.")]
	public float _generalRespawnTime = 8.65f;

	[Range(0f, 1f)]
	public float _generalRespawnChance = 0.23f;

	[Space]
	public CreepSpawner[] _linkedSpawners = new CreepSpawner[0];

	[Header("Creep Spawner Flags")]
	[Tooltip("Shuts off respawning for this creep spawner, until otherwise re-activated by a net trigger object.")]
	public bool _preventRespawning;

	[Tooltip("Prevents the spawner from increasing the respawn chance in response to the overall kill count accumulation by players killing creeps.")]
	public bool _preventChanceGrowth;

	private float _originChanceToRespawn;

	private float _chanceToRespawn = 0.6f;

	[Space]
	[Tooltip("Any spawning creep will aggro any player within the spawners radius. Does not affect creeps that have already been spawned. (Always enabled if a creep arena is tied to it.)")]
	public bool _aggroOnSpawn;

	[Tooltip("Any currently spawned creep will aggro any player within the spawners radius. (Always enabled if a creep arena is tied to it.)")]
	public bool _aggroWithinRadius;

	[Tooltip("Resets all aggroed creeps if the spawner radius player count is <= 0. This will teleport them back and reset all their stat values.")]
	public bool _resetAggroedCreepsIfEmptyRadius;

	[Tooltip("Force deletion of aggroed creeps while the spawner deletes the spawned creep pool rather than waiting for them to die.")]
	public bool _forceDeleteAggroedCreeps;

	[Space]
	[Tooltip("Turns off any wandering behavior on creeps, regardless if they are set to wander by their scriptable object data.")]
	public bool _suppressWandererBehavior;

	[Tooltip("Any creep who leaves the spawner radius will be re-positioned in the center of the spawner. (Always enabled if a creep arena is tied to it.)")]
	public bool _repositionIfCreepLeavesRadius;

	[Space]
	[Tooltip("Enable this if the spawner has a special spawned creep in the instance. This will enable the special spawn effect before spawning in the creep.")]
	public bool _isSpecialSpawn;

	[Header("Network Trigger")]
	public NetTrigger _deathCounterTrigger;

	[Header("Navigation")]
	public Transform[] _pathFindNodes = new Transform[0];

	public float _timeBetweenNodes = 7.5f;

	public bool _ignoreNodesAtStart;

	[Header("Runtime")]
	[SyncVar]
	public bool _disableSpawner;

	private int _currentKillCount;

	[Space]
	public List<Creep> _spawnedCreeps = new List<Creep>();

	public List<Player> _playersWithinSpawnerRadius = new List<Player>();

	public List<Creep> _creepsWithinSpawnerRadius = new List<Creep>();

	private bool _isSpawningCreeps;

	private bool _isDestroyingCreeps;

	[Header("Editor - Spawn Positions")]
	[SerializeField]
	private Transform[] _spawnPositions = new Transform[0];

	private int _currentSpawnIndex;

	public int CurrentKillCount
	{
		get
		{
			return _currentKillCount;
		}
		set
		{
			if (value < 0)
			{
				value = 0;
			}
			else if (value >= 1200)
			{
				value = 1200;
			}
			_currentKillCount = value;
		}
	}

	public int CurrentSpawnIndex
	{
		get
		{
			return _currentSpawnIndex;
		}
		set
		{
			if (value < 0)
			{
				value = _spawnPositions.Length - 1;
			}
			else if (value >= _spawnPositions.Length)
			{
				value = 0;
			}
			_currentSpawnIndex = value;
		}
	}

	public bool Network_disableSpawner
	{
		get
		{
			return _disableSpawner;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _disableSpawner, 1uL, null);
		}
	}

	protected override void OnValidate()
	{
		base.OnValidate();
		if (_creepCount <= 0)
		{
			_creepCount = 1;
		}
		if ((bool)_creepToSpawn)
		{
			base.gameObject.name = $"_entity_creepSpawner ({_creepToSpawn._creepName}) ({_spawnerWorldTime})";
		}
		else
		{
			base.gameObject.name = $"_entity_creepSpawner (No Creep Assigned) ({_spawnerWorldTime})";
		}
		if ((bool)_creepArena)
		{
			base.gameObject.name = "_entity_creepSpawner (Creep Arena)";
			Apply_ArenaParameters();
		}
	}

	[ContextMenu("Generate Spawn Locations")]
	private void Generate_SpawnLocations()
	{
		for (int i = 0; i < _spawnPositions.Length; i++)
		{
			UnityEngine.Object.DestroyImmediate(_spawnPositions[i].gameObject);
		}
		_spawnPositions = new Transform[0];
		StartCoroutine(Init_SpawnLocations());
	}

	private IEnumerator Init_SpawnLocations()
	{
		int _setGenerationCount = 6;
		do
		{
			Vector3 origin = base.transform.position + UnityEngine.Random.insideUnitSphere * 120f;
			origin.y = base.transform.position.y + 25.5f;
			if (Physics.Raycast(origin, Vector3.down, out var hitInfo, float.PositiveInfinity, LayerMask.GetMask("Ground")))
			{
				origin = hitInfo.point;
				GameObject obj = new GameObject();
				obj.name = $"Spawn Positioner #{_setGenerationCount}";
				obj.AddComponent<ChildGizmo>();
				obj.transform.SetParent(base.transform);
				obj.transform.SetPositionAndRotation(origin + Vector3.up * 4.5f, Quaternion.Euler(Vector3.up * UnityEngine.Random.Range(0, 360)));
				_setGenerationCount--;
			}
			yield return null;
		}
		while (_setGenerationCount > 0);
		Allocate_SpawnLocations();
	}

	[ContextMenu("Allocate Spawn Locations")]
	private void Allocate_SpawnLocations()
	{
		Transform[] componentsInChildren = GetComponentsInChildren<Transform>();
		List<Transform> list = new List<Transform>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (!(componentsInChildren[i] == base.transform) && (bool)componentsInChildren[i].GetComponent<ChildGizmo>())
			{
				list.Add(componentsInChildren[i]);
			}
		}
		_spawnPositions = list.ToArray();
	}

	[ContextMenu("Randomize Spawn Rotations")]
	private void Randomize_SpawnRotations()
	{
		for (int i = 0; i < _spawnPositions.Length; i++)
		{
			_spawnPositions[i].localRotation = Quaternion.Euler(Vector3.up * UnityEngine.Random.Range(0f, 360f));
		}
	}

	[ContextMenu("Reset Spawn Rotations")]
	private void Reset_SpawnRotations()
	{
		for (int i = 0; i < _spawnPositions.Length; i++)
		{
			_spawnPositions[i].localRotation = Quaternion.identity;
		}
	}

	private void Apply_ArenaParameters()
	{
		if ((bool)_creepArena)
		{
			_spawnerWorldTime = WorldTime.NULL;
			Network_disableSpawner = true;
			_creepToSpawn = null;
			_aggroOnSpawn = true;
			_aggroWithinRadius = true;
			_resetAggroedCreepsIfEmptyRadius = true;
			_repositionIfCreepLeavesRadius = true;
		}
	}

	private void Awake()
	{
		if ((bool)GameManager._current && (bool)GameManager._current._statLogics)
		{
			_physicsScene = base.gameObject.scene.GetPhysicsScene();
			_mainCollider = GetComponent<Collider>();
			_chanceToRespawn = _generalRespawnChance;
			_originChanceToRespawn = _generalRespawnChance;
			if ((bool)GetComponentInChildren<CreepArena>())
			{
				_creepArena = GetComponentInChildren<CreepArena>();
			}
			if ((bool)GetComponentInChildren<CreepSpawnerReigon>())
			{
				_creepSpawnerReigon = GetComponentInChildren<CreepSpawnerReigon>();
			}
			Allocate_SpawnLocations();
			Apply_ArenaParameters();
		}
	}

	private void OnTriggerStay(Collider other)
	{
		Creep component2;
		if (other.transform.root.TryGetComponent<Player>(out var component))
		{
			if ((!component._isHostPlayer || !(component.gameObject.scene != base.gameObject.scene)) && component._currentPlayerCondition == PlayerCondition.ACTIVE && !component._bufferingStatus && !_playersWithinSpawnerRadius.Contains(component))
			{
				_playersWithinSpawnerRadius.Add(component);
			}
		}
		else if (other.TryGetComponent<Creep>(out component2) && !(component2.gameObject.scene != base.gameObject.scene) && !(component2._parentCreepSpawner != this) && !_creepsWithinSpawnerRadius.Contains(component2))
		{
			_creepsWithinSpawnerRadius.Add(component2);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		Creep component2;
		if (other.transform.root.TryGetComponent<Player>(out var component))
		{
			if ((!component._isHostPlayer || !(component.gameObject.scene != base.gameObject.scene)) && _playersWithinSpawnerRadius.Contains(component))
			{
				_playersWithinSpawnerRadius.Remove(component);
			}
		}
		else if (other.TryGetComponent<Creep>(out component2) && !(component2.gameObject.scene != base.gameObject.scene) && !(component2._parentCreepSpawner != this) && _creepsWithinSpawnerRadius.Contains(component2))
		{
			_creepsWithinSpawnerRadius.Remove(component2);
		}
	}

	[ServerCallback]
	public void Enable_Spawner()
	{
		if (NetworkServer.active)
		{
			Network_disableSpawner = false;
			Start_EnemySpawns();
		}
	}

	[ServerCallback]
	public void Disable_Spawner()
	{
		if (NetworkServer.active && !_disableSpawner)
		{
			Network_disableSpawner = true;
			Init_DestroyAllSpawnedCreeps();
		}
	}

	private void Update()
	{
		Server_CreepSpawnerUpdate();
	}

	[ServerCallback]
	private void Server_CreepSpawnerUpdate()
	{
		if (NetworkServer.active)
		{
			Handle_SpawningBehavior();
			Handle_SpawnChanceRate();
			Iterate_SpawnedCreeps();
			Iterate_SpawnerRadiusPlayerList();
		}
		void Handle_SpawnChanceRate()
		{
			float num = _originChanceToRespawn + (float)_currentKillCount * 0.00052f;
			if (num > 1f)
			{
				num = 1f;
			}
			_chanceToRespawn = num;
		}
		void Handle_SpawningBehavior()
		{
			if (!_disableSpawner && !_creepArena && (bool)_creepToSpawn)
			{
				if (_spawnerWorldTime == WorldTime.NULL)
				{
					if (_spawnedCreeps.Count < _creepCount)
					{
						Start_EnemySpawns();
					}
				}
				else
				{
					if (_spawnerWorldTime != GameWorldManager._current._worldTime && _spawnedCreeps.Count > 0 && !_isDestroyingCreeps)
					{
						Init_DestroyAllSpawnedCreeps();
					}
					if (_spawnerWorldTime == GameWorldManager._current._worldTime && _spawnedCreeps.Count <= 0 && !_isSpawningCreeps)
					{
						StartCoroutine(Init_SpawnedCreeps(3.25f));
					}
				}
			}
		}
		void Iterate_SpawnedCreeps()
		{
			for (int i = 0; i < _spawnedCreeps.Count; i++)
			{
				if (_spawnedCreeps[i] == null)
				{
					_spawnedCreeps.RemoveAt(i);
					break;
				}
				if (_repositionIfCreepLeavesRadius && !_creepsWithinSpawnerRadius.Contains(_spawnedCreeps[i]) && _spawnedCreeps[i]._spawnInBuffer <= 0f)
				{
					if (_spawnedCreeps[i]._repositionBuffer >= 1.2f)
					{
						_spawnedCreeps[i].transform.position = _spawnPositions[UnityEngine.Random.Range(0, _spawnPositions.Length)].position;
						_spawnedCreeps[i]._repositionBuffer = 0f;
					}
					else
					{
						_spawnedCreeps[i]._repositionBuffer += Time.deltaTime;
					}
				}
			}
			for (int j = 0; j < _creepsWithinSpawnerRadius.Count; j++)
			{
				if (_creepsWithinSpawnerRadius[j] == null)
				{
					_creepsWithinSpawnerRadius.RemoveAt(j);
					break;
				}
			}
		}
		void Iterate_SpawnerRadiusPlayerList()
		{
			if ((bool)_creepSpawnerReigon && _creepSpawnerReigon._playersInReigon.Count <= 0 && _spawnedCreeps.Count > 0)
			{
				Init_DestroyAllSpawnedCreeps();
			}
			if (_playersWithinSpawnerRadius.Count <= 0)
			{
				_currentKillCount = 0;
			}
			else
			{
				for (int i = 0; i < _playersWithinSpawnerRadius.Count; i++)
				{
					if (_playersWithinSpawnerRadius[i] == null)
					{
						_playersWithinSpawnerRadius.RemoveAt(i);
						break;
					}
					if (_playersWithinSpawnerRadius[i]._bufferingStatus)
					{
						_playersWithinSpawnerRadius.RemoveAt(i);
						break;
					}
					if (_playersWithinSpawnerRadius[i]._currentPlayerCondition != PlayerCondition.ACTIVE)
					{
						_playersWithinSpawnerRadius.RemoveAt(i);
						break;
					}
				}
			}
		}
	}

	[ServerCallback]
	public void Start_EnemySpawns()
	{
		if (NetworkServer.active && _spawnerWorldTime == WorldTime.NULL && !_isSpawningCreeps && !_disableSpawner)
		{
			StartCoroutine(Init_SpawnedCreeps(_spawnInitializeTime));
		}
	}

	[IteratorStateMachine(typeof(_003CInit_SpawnedCreeps_003Ed__59))]
	[ServerCallback]
	private IEnumerator Init_SpawnedCreeps(float _timeBeforeSpawnInit)
	{
		if (!NetworkServer.active)
		{
			return null;
		}
		return new _003CInit_SpawnedCreeps_003Ed__59(0)
		{
			_003C_003E4__this = this,
			_timeBeforeSpawnInit = _timeBeforeSpawnInit
		};
	}

	[ServerCallback]
	private void Server_NewCreepObject()
	{
		if (NetworkServer.active && _spawnedCreeps.Count < _creepCount)
		{
			ScriptableCreep scriptableCreep = _creepToSpawn;
			if ((bool)_creepArena && _creepArena._setArenaSlot != null)
			{
				scriptableCreep = _creepArena._setArenaSlot._creepArenaWaves[_creepArena._currentWave]._creepPool[_spawnedCreeps.Count];
			}
			GameObject obj = UnityEngine.Object.Instantiate(AtlyssNetworkManager._current._networkObjectLibrary._creepObject);
			Creep component = obj.GetComponent<Creep>();
			RaycastPathFinder pathFindCtrl = component._pathFindCtrl;
			component.Network_currentCreepCondition = CreepCondition.SPAWNING;
			component._scriptCreep = scriptableCreep;
			component.Network_creepNameID = scriptableCreep._creepName;
			component._suppressWandererBehavior = _suppressWandererBehavior;
			component._parentCreepSpawner = this;
			if (!_preventRespawning)
			{
				component._setRespawnBuffer = _generalRespawnTime;
			}
			else
			{
				component._setRespawnBuffer = 0f;
			}
			if (_pathFindNodes.Length != 0)
			{
				pathFindCtrl._pathFindNodes = _pathFindNodes;
				pathFindCtrl._timeBetweenNodes = _timeBetweenNodes;
			}
			pathFindCtrl.marginCollision = 2.85f;
			_spawnedCreeps.Add(component);
			SceneManager.MoveGameObjectToScene(obj, base.gameObject.scene);
			NetworkServer.Spawn(obj);
			Init_CreepSpawn(component);
		}
	}

	[ServerCallback]
	public void Init_CreepSpawn(Creep _baseCreepObj)
	{
		if (NetworkServer.active)
		{
			StartCoroutine(CreepSpawn(_baseCreepObj));
		}
	}

	[IteratorStateMachine(typeof(_003CCreepSpawn_003Ed__62))]
	[ServerCallback]
	private IEnumerator CreepSpawn(Creep _baseCreepObj)
	{
		if (!NetworkServer.active)
		{
			return null;
		}
		return new _003CCreepSpawn_003Ed__62(0)
		{
			_003C_003E4__this = this,
			_baseCreepObj = _baseCreepObj
		};
	}

	[ServerCallback]
	public void Init_DestroyAllSpawnedCreeps()
	{
		if (NetworkServer.active && _spawnedCreeps.Count > 0 && !_isDestroyingCreeps)
		{
			StartCoroutine(Destroy_AllSpawnedCreeps(_destroyInitializeTime));
		}
	}

	[IteratorStateMachine(typeof(_003CDestroy_AllSpawnedCreeps_003Ed__64))]
	[ServerCallback]
	private IEnumerator Destroy_AllSpawnedCreeps(float _time)
	{
		if (!NetworkServer.active)
		{
			return null;
		}
		return new _003CDestroy_AllSpawnedCreeps_003Ed__64(0)
		{
			_003C_003E4__this = this,
			_time = _time
		};
	}

	[ServerCallback]
	public void Callback_RespawnCreep(Creep _creepObject)
	{
		if (NetworkServer.active && !(_creepObject._setRespawnBuffer <= 0f) && !(_creepObject._respawnBuffer > 0f) && !_creepArena)
		{
			if (UnityEngine.Random.Range(0f, 1f) <= _chanceToRespawn)
			{
				Init_CreepSpawn(_creepObject);
				_creepObject._respawnBuffer = _creepObject._setRespawnBuffer;
			}
			else
			{
				_creepObject._respawnBuffer = _creepObject._setRespawnBuffer;
			}
		}
	}

	[ClientRpc]
	private void Rpc_InitSpecialSpawnEffect(Vector3 _position)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteVector3(_position);
		SendRPCInternal("System.Void CreepSpawner::Rpc_InitSpecialSpawnEffect(UnityEngine.Vector3)", 557847696, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	[CompilerGenerated]
	internal static void _003CDestroy_AllSpawnedCreeps_003Eg__Destroy_SingleCreep_007C64_0(GameObject _creepObject)
	{
		if (!(_creepObject == null))
		{
			NetworkServer.UnSpawn(_creepObject);
			UnityEngine.Object.Destroy(_creepObject);
		}
	}

	public override bool Weaved()
	{
		return true;
	}

	protected void UserCode_Rpc_InitSpecialSpawnEffect__Vector3(Vector3 _position)
	{
		if ((bool)Player._mainPlayer && !(Player._mainPlayer.Network_playerMapInstance != _mapInstance))
		{
			PoolManager._current.GenerateObject(null, "SpecialSpawn_Effect", 2.5f).transform.position = _position;
		}
	}

	protected static void InvokeUserCode_Rpc_InitSpecialSpawnEffect__Vector3(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			UnityEngine.Debug.LogError("RPC Rpc_InitSpecialSpawnEffect called on server.");
		}
		else
		{
			((CreepSpawner)obj).UserCode_Rpc_InitSpecialSpawnEffect__Vector3(reader.ReadVector3());
		}
	}

	static CreepSpawner()
	{
		RemoteProcedureCalls.RegisterRpc(typeof(CreepSpawner), "System.Void CreepSpawner::Rpc_InitSpecialSpawnEffect(UnityEngine.Vector3)", InvokeUserCode_Rpc_InitSpecialSpawnEffect__Vector3);
	}

	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteBool(_disableSpawner);
			return;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteBool(_disableSpawner);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize(ref _disableSpawner, null, reader.ReadBool());
			return;
		}
		long num = (long)reader.ReadULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _disableSpawner, null, reader.ReadBool());
		}
	}
}
