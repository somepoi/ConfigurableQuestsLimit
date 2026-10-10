using System.Runtime.InteropServices;
using Mirror;
using UnityEngine;

public class Portal : NetworkBehaviour
{
	[Header("Portal Parameters")]
	public ScenePortalData _scenePortal;

	[Space]
	[SyncVar]
	public bool _netDisablePortal;

	public bool _isPortalOpen = true;

	public bool _disableIfSoloMode;

	[Header("Visual")]
	public GameObject _portalVisualContainer;

	public ParticleSystem _portalEffect;

	private AudioSource[] _aSrcCache;

	[Space]
	public SpriteRenderer _minimapIcon;

	public Color _activeColor_minimap = Color.blue;

	public Color _inactiveColor_minimap = Color.grey;

	[Header("Quest Attunement")]
	public ScriptableQuest _scriptQuestOverride;

	public bool _openPortalIfQuestComplete = true;

	public bool _openPortalIfOnQuest = true;

	public bool Network_netDisablePortal
	{
		get
		{
			return _netDisablePortal;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _netDisablePortal, 1uL, null);
		}
	}

	protected override void OnValidate()
	{
		base.OnValidate();
		switch (_scenePortal._portalType)
		{
		case PortalType.NORMAL:
			if (_scenePortal._subScene != string.Empty)
			{
				base.gameObject.name = "_entity_portal (" + _scenePortal._subScene + " -> " + _scenePortal._spawnPointTag + ")";
			}
			else
			{
				base.gameObject.name = "_entity_portal (No Scene Attached)";
			}
			break;
		case PortalType.DUNGEON:
			if (_scenePortal._subScene != string.Empty)
			{
				base.gameObject.name = "_entity_dungeonPortal (" + _scenePortal._subScene + " -> " + _scenePortal._spawnPointTag + ")";
			}
			else
			{
				base.gameObject.name = "_entity_dungeonPortal (No Scene Attached)";
			}
			break;
		case PortalType.ARENA:
			if (_scenePortal._subScene != string.Empty)
			{
				base.gameObject.name = "_entity_arenaPortal (" + _scenePortal._subScene + " -> " + _scenePortal._spawnPointTag + ")";
			}
			else
			{
				base.gameObject.name = "_entity_arenaPortal (No Scene Attached)";
			}
			break;
		}
		if (!_scriptQuestOverride)
		{
			_openPortalIfOnQuest = false;
			_openPortalIfQuestComplete = false;
		}
		if (_isPortalOpen)
		{
			_portalEffect.Play();
		}
		else
		{
			_portalEffect.Stop();
		}
	}

	private void Awake()
	{
		_aSrcCache = GetComponentsInChildren<AudioSource>(includeInactive: true);
	}

	public virtual void OnDrawGizmos()
	{
		Gizmos.DrawIcon(base.transform.position + Vector3.up * 4.5f, "_portalIco", allowScaling: false);
	}

	public virtual void Update()
	{
		Client_PortalRuntime();
	}

	[ClientCallback]
	private void Client_PortalRuntime()
	{
		if (!NetworkClient.active || !Player._mainPlayer || !Player._mainPlayer.isLocalPlayer)
		{
			return;
		}
		if ((bool)_minimapIcon)
		{
			if (_isPortalOpen)
			{
				_minimapIcon.color = _activeColor_minimap;
			}
			else
			{
				_minimapIcon.color = _inactiveColor_minimap;
			}
		}
		if ((bool)_scenePortal._portalDialogTrigger)
		{
			_scenePortal._portalDialogTrigger.gameObject.SetActive(_isPortalOpen);
		}
		if (!_netDisablePortal && _isPortalOpen && !_portalEffect.isEmitting)
		{
			_portalEffect.Play();
		}
		if (_netDisablePortal || !_isPortalOpen)
		{
			_portalEffect.Stop();
		}
		if (Player._mainPlayer._isHostPlayer)
		{
			for (int i = 0; i < _aSrcCache.Length; i++)
			{
				_aSrcCache[i].enabled = base.gameObject.scene == Player._mainPlayer.gameObject.scene && _isPortalOpen;
			}
		}
		if (_disableIfSoloMode && AtlyssNetworkManager._current._soloMode)
		{
			_isPortalOpen = false;
		}
		else if (!_netDisablePortal)
		{
			Handle_QuestOverrideConditions(Player._mainPlayer);
		}
	}

	private void Handle_QuestOverrideConditions(Player _p)
	{
		PlayerQuesting pQuest = _p._pQuest;
		if (!pQuest)
		{
			return;
		}
		if (_openPortalIfQuestComplete)
		{
			if (pQuest._finishedQuests.Count <= 0)
			{
				_isPortalOpen = false;
				return;
			}
			if (pQuest._finishedQuests.ContainsKey(_scriptQuestOverride._questName))
			{
				_isPortalOpen = true;
				return;
			}
			_isPortalOpen = false;
		}
		if ((bool)_scriptQuestOverride && _openPortalIfOnQuest)
		{
			_isPortalOpen = pQuest._scriptableQuests.Contains(_scriptQuestOverride);
		}
	}

	[ServerCallback]
	public void Toggle_NetDisabler(bool _isTrue)
	{
		if (NetworkServer.active)
		{
			Network_netDisablePortal = _isTrue;
		}
	}

	[ServerCallback]
	public void Server_InteractPortal(NetworkIdentity _netID, ZoneDifficulty _setDifficulty)
	{
		if (NetworkServer.active)
		{
			MapInstancingManager mapInstanceManager = AtlyssNetworkManager._current._mapInstanceManager;
			Player component = _netID.GetComponent<Player>();
			if (_scenePortal._subScene != string.Empty)
			{
				mapInstanceManager.Init_SendPeer_ToScene(component, component.gameObject.scene.path, _scenePortal._subScene, _scenePortal._spawnPointTag, _setDifficulty);
			}
			else
			{
				Debug.LogError("No subscene applied to this portal.");
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
			writer.WriteBool(_netDisablePortal);
			return;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteBool(_netDisablePortal);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize(ref _netDisablePortal, null, reader.ReadBool());
			return;
		}
		long num = (long)reader.ReadULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _netDisablePortal, null, reader.ReadBool());
		}
	}
}
