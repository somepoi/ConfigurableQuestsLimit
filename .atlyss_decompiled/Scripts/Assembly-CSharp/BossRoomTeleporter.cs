using System.Runtime.InteropServices;
using Mirror;
using UnityEngine;

public class BossRoomTeleporter : NetworkBehaviour
{
	[Header("Boss Room Teleporter")]
	public PatternInstanceManager _patternInstanceManager;

	[SerializeField]
	private GameObject _visualContainer;

	[SerializeField]
	private Transform[] _teleportPoints;

	[SerializeField]
	private Teleporter _shortcutTeleporter;

	[SyncVar]
	public bool _isEnabled = true;

	private bool _wentThroughTeleporter;

	private bool _teleportBuffer;

	public bool Network_isEnabled
	{
		get
		{
			return _isEnabled;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _isEnabled, 1uL, null);
		}
	}

	private void Update()
	{
		_visualContainer.SetActive(_isEnabled);
		if ((bool)_shortcutTeleporter)
		{
			_shortcutTeleporter._isEnabled = _wentThroughTeleporter;
		}
		Server_BossTeleporter();
	}

	[ServerCallback]
	private void Server_BossTeleporter()
	{
		if (NetworkServer.active && (bool)_patternInstanceManager)
		{
			Network_isEnabled = _patternInstanceManager._setDungeonDifficulty != ZoneDifficulty.EASY;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if ((bool)_patternInstanceManager && _isEnabled && (bool)other.gameObject.GetComponentInParent<Player>())
		{
			Player componentInParent = other.gameObject.GetComponentInParent<Player>();
			if (Check_PlayerCondition(componentInParent))
			{
				Init_Teleport();
			}
		}
	}

	private bool Check_PlayerCondition(Player _p)
	{
		if (!_p)
		{
			return false;
		}
		if (!_p.isLocalPlayer)
		{
			return false;
		}
		if (_p._currentPlayerCondition != PlayerCondition.ACTIVE)
		{
			return false;
		}
		if ((bool)_p.Network_playerMapInstance && _p.Network_playerMapInstance != _patternInstanceManager._mapInstance)
		{
			return false;
		}
		if (_p._bufferingStatus)
		{
			return false;
		}
		return true;
	}

	private void Reset_TeleportBuffer()
	{
		_teleportBuffer = false;
	}

	private void Init_Teleport()
	{
		if (!_patternInstanceManager)
		{
			return;
		}
		Player mainPlayer = Player._mainPlayer;
		PlayerVisual pVisual = mainPlayer._pVisual;
		CharacterController playerController = mainPlayer._pMove._playerController;
		if (mainPlayer._currentPlayerCondition == PlayerCondition.ACTIVE && !mainPlayer._bufferingStatus && !_teleportBuffer)
		{
			pVisual.Init_TeleportEffect();
			if (mainPlayer.isLocalPlayer)
			{
				playerController.enabled = false;
				Transform transform = _teleportPoints[Random.Range(0, _teleportPoints.Length)];
				mainPlayer.transform.SetPositionAndRotation(transform.position, transform.rotation);
				CameraFunction._current.CameraReset();
				playerController.enabled = true;
				_teleportBuffer = true;
				Invoke("Reset_TeleportBuffer", 1.25f);
				_wentThroughTeleporter = true;
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
			writer.WriteBool(_isEnabled);
			return;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteBool(_isEnabled);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize(ref _isEnabled, null, reader.ReadBool());
			return;
		}
		long num = (long)reader.ReadULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _isEnabled, null, reader.ReadBool());
		}
	}
}
