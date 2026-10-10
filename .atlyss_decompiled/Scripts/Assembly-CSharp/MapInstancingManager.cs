using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapInstancingManager : MonoBehaviour
{
	[CompilerGenerated]
	private sealed class _003CSendPeer_ToScene_003Ed__18 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public Player _player;

		public MapInstancingManager _003C_003E4__this;

		public string _newScene;

		public ZoneDifficulty _difficultyValue;

		public string _spawnTag;

		public string _oldScene;

		private MapInstance _003C_setInstance_003E5__2;

		private int _003Ci_003E5__3;

		private bool _003C_alreadyLoadedInstance_003E5__4;

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
		public _003CSendPeer_ToScene_003Ed__18(int _003C_003E1__state)
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
			MapInstancingManager mapInstancingManager = _003C_003E4__this;
			SceneMessage message;
			switch (num)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				_003C_setInstance_003E5__2 = null;
				if (!_player)
				{
					return false;
				}
				if (!_player._isHostPlayer)
				{
					mapInstancingManager._bufferLoad = true;
				}
				_player.Network_currentGameCondition = GameCondition.LOADING_GAME;
				_003C_003E2__current = new WaitForSeconds(1.25f);
				_003C_003E1__state = 1;
				return true;
			case 1:
				_003C_003E1__state = -1;
				if (_player._isHostPlayer && mapInstancingManager._bufferLoad)
				{
					_003C_003E2__current = new WaitForSeconds(1f);
					_003C_003E1__state = 2;
					return true;
				}
				goto IL_00d4;
			case 2:
				_003C_003E1__state = -1;
				goto IL_00d4;
			case 3:
				_003C_003E1__state = -1;
				goto IL_0258;
			case 4:
				_003C_003E1__state = -1;
				goto IL_02e8;
			case 5:
				_003C_003E1__state = -1;
				goto IL_0357;
			case 6:
				_003C_003E1__state = -1;
				goto IL_0357;
			case 7:
				_003C_003E1__state = -1;
				goto IL_054f;
			case 8:
				_003C_003E1__state = -1;
				if (!_003C_setInstance_003E5__2._patternInstance)
				{
					break;
				}
				goto IL_05dc;
			case 9:
				_003C_003E1__state = -1;
				if (!_003C_setInstance_003E5__2._patternInstance._generatingPattern)
				{
					string spawnPointTag = _003C_setInstance_003E5__2._patternInstance._instancePatterns[_003C_setInstance_003E5__2._patternInstance._instancePatternID]._setSpawnPoint._spawnPointTag;
					_003C_setInstance_003E5__2.SendPeer_ToSpawnPoint(_player.gameObject, spawnPointTag);
					_003C_003E2__current = new WaitForSeconds(3.5f);
					_003C_003E1__state = 10;
					return true;
				}
				goto IL_05dc;
			case 10:
				{
					_003C_003E1__state = -1;
					break;
				}
				IL_05dc:
				_003C_003E2__current = null;
				_003C_003E1__state = 9;
				return true;
				IL_0357:
				if (_003C_setInstance_003E5__2 == null)
				{
					_003C_setInstance_003E5__2 = mapInstancingManager.Find_LoadedInstance(_newScene, _player);
					_003C_003E2__current = new WaitForSeconds(0.25f);
					_003C_003E1__state = 6;
					return true;
				}
				if (!_player)
				{
					return false;
				}
				SceneManager.MoveGameObjectToScene(_player.gameObject, _003C_setInstance_003E5__2._loadedScene);
				_player.gameObject.transform.SetAsLastSibling();
				_003C_setInstance_003E5__2._peersInInstance.Add(_player);
				if ((bool)_003C_setInstance_003E5__2._patternInstance)
				{
					PatternInstanceManager patternInstance = _003C_setInstance_003E5__2._patternInstance;
					if (!patternInstance.Network_instanceHostPlayer)
					{
						patternInstance.Network_instanceHostPlayer = _player;
					}
					if ((bool)_player.Network_partyObject && _player.Network_partyObject.Network_partyLeaderPlayer == _player)
					{
						patternInstance.Network_setDungeonDifficulty = _difficultyValue;
					}
					if (!_player.Network_partyObject)
					{
						patternInstance.Network_setDungeonDifficulty = _difficultyValue;
					}
					if (!patternInstance._setInstancePattern._patternContainerObject)
					{
						GameWorldManager._current.Create_NewDungeonQueue(_003C_setInstance_003E5__2);
						patternInstance.Network_instancePatternID = GameWorldManager._current.Retrieve_NextDungeonQueue(_003C_setInstance_003E5__2);
					}
				}
				else
				{
					_003C_setInstance_003E5__2.SendPeer_ToSpawnPoint(_player.gameObject, _spawnTag);
				}
				switch (_003C_setInstance_003E5__2._zoneType)
				{
				case ZoneType.Safe:
					_player._statusEntity.Replenish_All();
					break;
				case ZoneType.Dungeon:
					_player._statusEntity.Replenish_All();
					break;
				case ZoneType.Arena:
					_player._statusEntity.Replenish_All();
					break;
				}
				message = new SceneMessage
				{
					sceneName = _newScene,
					sceneOperation = SceneOperation.LoadAdditive
				};
				_player.connectionToClient.Send(message);
				goto IL_054f;
				IL_02d2:
				if (_003Ci_003E5__3 < mapInstancingManager._loadedInstances.Count)
				{
					_003C_alreadyLoadedInstance_003E5__4 = mapInstancingManager._loadedInstances[_003Ci_003E5__3]._loadedScene.path == _newScene;
					if (!_003C_alreadyLoadedInstance_003E5__4 || !mapInstancingManager._loadedInstances[_003Ci_003E5__3]._patternInstance || !(mapInstancingManager._loadedInstances[_003Ci_003E5__3]._patternInstance.gameObject.scene.path == _newScene))
					{
						goto IL_0258;
					}
					if (!mapInstancingManager._loadedInstances[_003Ci_003E5__3]._patternInstance.Network_instanceParty || !(_player.Network_partyObject == mapInstancingManager._loadedInstances[_003Ci_003E5__3]._patternInstance.Network_instanceParty))
					{
						mapInstancingManager._instanceQueue.Add(_newScene);
						_003C_003E2__current = mapInstancingManager.Server_LoadScene(_newScene);
						_003C_003E1__state = 3;
						return true;
					}
				}
				goto IL_02e8;
				IL_054f:
				if (!_player.connectionToClient.isReady)
				{
					_003C_003E2__current = null;
					_003C_003E1__state = 7;
					return true;
				}
				if (_oldScene != string.Empty)
				{
					SceneMessage message2 = new SceneMessage
					{
						sceneName = _oldScene,
						sceneOperation = SceneOperation.UnloadAdditive
					};
					_player.connectionToClient.Send(message2);
				}
				_003C_003E2__current = new WaitForSeconds(0.85f);
				_003C_003E1__state = 8;
				return true;
				IL_00d4:
				mapInstancingManager._bufferLoad = false;
				if (_player._isHostPlayer)
				{
					for (int i = 0; i < mapInstancingManager._loadedInstances.Count; i++)
					{
						if (mapInstancingManager._loadedInstances[i]._loadedScene == _player.gameObject.scene)
						{
							mapInstancingManager._loadedInstances[i].Destroy_HostCameraColliders();
						}
					}
				}
				_003Ci_003E5__3 = 0;
				goto IL_02d2;
				IL_0258:
				if (!_003C_alreadyLoadedInstance_003E5__4)
				{
					if (_003Ci_003E5__3 >= mapInstancingManager._loadedInstances.Count - 1 && !mapInstancingManager._instanceQueue.Contains(_newScene))
					{
						mapInstancingManager._instanceQueue.Add(_newScene);
						_003C_003E2__current = mapInstancingManager.Server_LoadScene(_newScene);
						_003C_003E1__state = 4;
						return true;
					}
					_003Ci_003E5__3++;
					goto IL_02d2;
				}
				goto IL_02e8;
				IL_02e8:
				SceneManager.MoveGameObjectToScene(_player.gameObject, SceneManager.GetActiveScene());
				_003C_003E2__current = new WaitForSeconds(0.45f);
				_003C_003E1__state = 5;
				return true;
			}
			if ((bool)Player._mainPlayer && _player == Player._mainPlayer && Player._mainPlayer._isHostPlayer)
			{
				_003C_setInstance_003E5__2.Spawn_HostCameraColliders();
			}
			_player.Network_playerZoneType = _003C_setInstance_003E5__2._zoneType;
			_player.Network_currentGameCondition = GameCondition.IN_GAME;
			_player.Network_currentPlayerCondition = PlayerCondition.ACTIVE;
			_player._lastSpawnPoint = _spawnTag;
			if (mapInstancingManager._instanceQueue.Contains(_newScene))
			{
				mapInstancingManager._instanceQueue.Remove(_newScene);
			}
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
	private sealed class _003CServer_LoadScene_003Ed__11 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public string _sceneName;

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
		public _003CServer_LoadScene_003Ed__11(int _003C_003E1__state)
		{
			this._003C_003E1__state = _003C_003E1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
		}

		private bool MoveNext()
		{
			switch (_003C_003E1__state)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				_003C_003E2__current = SceneManager.LoadSceneAsync(_sceneName, new LoadSceneParameters
				{
					loadSceneMode = LoadSceneMode.Additive,
					localPhysicsMode = LocalPhysicsMode.Physics3D
				});
				_003C_003E1__state = 1;
				return true;
			case 1:
				_003C_003E1__state = -1;
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

	public static MapInstancingManager _current;

	[Header("Maps / Instancing")]
	[Scene]
	public string _hubMap;

	public string _hubMainSpawnPointTag = "startPoint";

	public string _hubRespawnPointTag = "respawnPoint";

	public List<MapInstance> _loadedInstances = new List<MapInstance>();

	public List<string> _instanceQueue = new List<string>();

	private bool _bufferLoad;

	private void Awake()
	{
		_current = this;
	}

	private void Update()
	{
		Iterate_LoadedInstances();
	}

	[ServerCallback]
	private void Iterate_LoadedInstances()
	{
		if (!NetworkServer.active)
		{
			return;
		}
		for (int i = 0; i < _loadedInstances.Count; i++)
		{
			if (_loadedInstances[i] == null)
			{
				_loadedInstances.RemoveAt(i);
				break;
			}
		}
	}

	[ServerCallback]
	public void Init_LoadSubScene(string _sceneName)
	{
		if (NetworkServer.active)
		{
			StartCoroutine(Server_LoadScene(_sceneName));
		}
	}

	[IteratorStateMachine(typeof(_003CServer_LoadScene_003Ed__11))]
	[ServerCallback]
	private IEnumerator Server_LoadScene(string _sceneName)
	{
		if (!NetworkServer.active)
		{
			return null;
		}
		return new _003CServer_LoadScene_003Ed__11(0)
		{
			_sceneName = _sceneName
		};
	}

	public void Init_UnloadSubScene(Scene _scene)
	{
		StartCoroutine(Server_UnloadSubScene(_scene));
	}

	public void Init_UnloadAllSubScenes()
	{
		StartCoroutine(Server_UnloadAllSubScenes());
	}

	private IEnumerator Server_UnloadSubScene(Scene _scene)
	{
		AsyncOperation _aSync = SceneManager.UnloadSceneAsync(_scene);
		while (!_aSync.isDone)
		{
			yield return null;
		}
	}

	private IEnumerator Server_UnloadAllSubScenes()
	{
		for (int index = 0; index < _loadedInstances.Count; index++)
		{
			if (_loadedInstances[index]._loadedScene.IsValid())
			{
				_loadedInstances[index].Destroy_HostCameraColliders();
				yield return SceneManager.UnloadSceneAsync(_loadedInstances[index]._loadedScene);
			}
		}
		_loadedInstances.Clear();
		yield return Resources.UnloadUnusedAssets();
	}

	private MapInstance Find_LoadedInstance(string _newScene, Player _p)
	{
		for (int i = 0; i < _loadedInstances.Count; i++)
		{
			if (_loadedInstances[i]._loadedScene.path != _newScene)
			{
				continue;
			}
			if (!_p)
			{
				return null;
			}
			if ((bool)_loadedInstances[i]._patternInstance)
			{
				if ((bool)_loadedInstances[i]._patternInstance.Network_instanceParty)
				{
					if (_p.Network_partyObject == _loadedInstances[i]._patternInstance.Network_instanceParty)
					{
						return _loadedInstances[i];
					}
				}
				else if (_loadedInstances[i]._peersInInstance.Count <= 0)
				{
					return _loadedInstances[i];
				}
				continue;
			}
			return _loadedInstances[i];
		}
		return null;
	}

	[ServerCallback]
	public void Init_SendPeer_ToScene(Player _player, string _old, string _new, string _spawnTag, ZoneDifficulty _difficulty)
	{
		if (NetworkServer.active)
		{
			StartCoroutine(SendPeer_ToScene(_player, _old, _new, _spawnTag, _difficulty));
		}
	}

	[IteratorStateMachine(typeof(_003CSendPeer_ToScene_003Ed__18))]
	[ServerCallback]
	public IEnumerator SendPeer_ToScene(Player _player, string _oldScene, string _newScene, string _spawnTag, ZoneDifficulty _difficultyValue)
	{
		if (!NetworkServer.active)
		{
			return null;
		}
		return new _003CSendPeer_ToScene_003Ed__18(0)
		{
			_003C_003E4__this = this,
			_player = _player,
			_oldScene = _oldScene,
			_newScene = _newScene,
			_spawnTag = _spawnTag,
			_difficultyValue = _difficultyValue
		};
	}
}
