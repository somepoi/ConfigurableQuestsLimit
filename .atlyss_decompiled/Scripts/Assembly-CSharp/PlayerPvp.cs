using System.Runtime.InteropServices;
using Mirror;
using Mirror.RemoteCalls;
using UnityEngine;

public class PlayerPvp : NetworkBehaviour
{
	private Player _player;

	private PlayerSound _pSound;

	private ChatBehaviour _cB;

	[Header("PvP / PvE Flagging")]
	[SyncVar(hook = "OnVersusConditionChange")]
	public VersusFlag _currentVersusFlag;

	[SerializeField]
	private SpriteRenderer _pvpIcon;

	[SerializeField]
	private float _maxPvpToggleBuffer = 1.5f;

	private float _pvpToggleBuffer;

	[Header("Arena Flagging Conditions")]
	[SerializeField]
	private int _arena_setAutoFlagTimer = 20;

	[SyncVar]
	public int _arena_autoFlagTimer;

	[Space]
	[SerializeField]
	private float _arena_setAutoFlagBuffer = 1.25f;

	private float _arena_autoFlagBuffer;

	public VersusFlag Network_currentVersusFlag
	{
		get
		{
			return _currentVersusFlag;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _currentVersusFlag, 1uL, OnVersusConditionChange);
		}
	}

	public int Network_arena_autoFlagTimer
	{
		get
		{
			return _arena_autoFlagTimer;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _arena_autoFlagTimer, 2uL, null);
		}
	}

	private void OnVersusConditionChange(VersusFlag _oldCondition, VersusFlag _newCondition)
	{
		switch (_newCondition)
		{
		case VersusFlag.PVE:
			_cB.Init_GameLogicMessage("Unflagged for PvP.");
			if ((bool)Player._mainPlayer && !Player._mainPlayer._bufferingStatus)
			{
				_pSound._aSrcGeneral.PlayOneShot(_pSound._pvpOffSound, 0.35f);
			}
			break;
		case VersusFlag.PVP:
			_cB.Init_GameLogicMessage("Flagged for PvP.");
			if ((bool)Player._mainPlayer && !Player._mainPlayer._bufferingStatus)
			{
				_pSound._aSrcGeneral.PlayOneShot(_pSound._pvpFlagSound, 0.35f);
			}
			break;
		}
	}

	private void Awake()
	{
		_player = GetComponent<Player>();
		_pSound = GetComponent<PlayerSound>();
		_cB = GetComponent<ChatBehaviour>();
	}

	private void Update()
	{
		Handle_LocalPvPControl();
		Server_HandleConditions();
	}

	[ServerCallback]
	private void Server_HandleConditions()
	{
		if (!NetworkServer.active)
		{
			return;
		}
		if (_player._currentPlayerCondition == PlayerCondition.DEAD || _player._playerZoneType == ZoneType.Safe)
		{
			Network_arena_autoFlagTimer = _arena_setAutoFlagTimer;
			_arena_autoFlagBuffer = _arena_setAutoFlagBuffer;
			Network_currentVersusFlag = VersusFlag.PVE;
		}
		if (_player._bufferingStatus || _player._currentPlayerCondition == PlayerCondition.DEAD || _player._playerZoneType != ZoneType.Arena)
		{
			return;
		}
		if (_arena_autoFlagTimer > 0)
		{
			if (_arena_autoFlagBuffer > 0f)
			{
				_arena_autoFlagBuffer -= Time.deltaTime;
				return;
			}
			Network_arena_autoFlagTimer = _arena_autoFlagTimer - 1;
			_arena_autoFlagBuffer = _arena_setAutoFlagBuffer;
		}
		else
		{
			Network_arena_autoFlagTimer = 0;
			Network_currentVersusFlag = VersusFlag.PVP;
		}
	}

	[Command]
	public void Cmd_FlagPvp(bool _isTrue)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteBool(_isTrue);
		SendCommandInternal("System.Void PlayerPvp::Cmd_FlagPvp(System.Boolean)", 767885484, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientCallback]
	private void Handle_LocalPvPControl()
	{
		if (!NetworkClient.active)
		{
			return;
		}
		Handle_PvPIconDisplay();
		if (!base.isLocalPlayer || _player._inUI || _player._inChat)
		{
			return;
		}
		if (_pvpToggleBuffer > 0f)
		{
			_pvpToggleBuffer -= Time.deltaTime;
		}
		else
		{
			_pvpToggleBuffer = 0f;
		}
		if (!Input.GetKeyDown(InputControlManager.current._flagPvp) || !(_pvpToggleBuffer <= 0f))
		{
			return;
		}
		if (AtlyssNetworkManager._current._soloMode)
		{
			ErrorPromptTextManager.current.Init_ErrorPrompt("Cannot PvP in Singleplayer Mode");
			return;
		}
		if (_player._playerZoneType == ZoneType.Safe)
		{
			ErrorPromptTextManager.current.Init_ErrorPrompt("Cannot PvP Here");
			return;
		}
		if (_player._playerZoneType == ZoneType.Arena)
		{
			ErrorPromptTextManager.current.Init_ErrorPrompt("Cannot toggle PvP in Arena");
			return;
		}
		switch (_currentVersusFlag)
		{
		case VersusFlag.PVE:
			Cmd_FlagPvp(_isTrue: true);
			break;
		case VersusFlag.PVP:
			Cmd_FlagPvp(_isTrue: false);
			break;
		}
		_pvpToggleBuffer = _maxPvpToggleBuffer;
		void Handle_PvPIconDisplay()
		{
			if ((bool)Player._mainPlayer)
			{
				if (Player._mainPlayer.Network_playerMapInstance != _player.Network_playerMapInstance)
				{
					_pvpIcon.enabled = false;
				}
				else
				{
					_pvpIcon.enabled = _currentVersusFlag == VersusFlag.PVP && !_player._bufferingStatus && !base.isLocalPlayer;
				}
			}
		}
	}

	public override bool Weaved()
	{
		return true;
	}

	protected void UserCode_Cmd_FlagPvp__Boolean(bool _isTrue)
	{
		if (_player._playerZoneType != ZoneType.Arena)
		{
			if (_isTrue)
			{
				Network_currentVersusFlag = VersusFlag.PVP;
			}
			else
			{
				Network_currentVersusFlag = VersusFlag.PVE;
			}
		}
	}

	protected static void InvokeUserCode_Cmd_FlagPvp__Boolean(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command Cmd_FlagPvp called on client.");
		}
		else
		{
			((PlayerPvp)obj).UserCode_Cmd_FlagPvp__Boolean(reader.ReadBool());
		}
	}

	static PlayerPvp()
	{
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerPvp), "System.Void PlayerPvp::Cmd_FlagPvp(System.Boolean)", InvokeUserCode_Cmd_FlagPvp__Boolean, requiresAuthority: true);
	}

	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			GeneratedNetworkCode._Write_VersusFlag(writer, _currentVersusFlag);
			writer.WriteInt(_arena_autoFlagTimer);
			return;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 1L) != 0L)
		{
			GeneratedNetworkCode._Write_VersusFlag(writer, _currentVersusFlag);
		}
		if ((base.syncVarDirtyBits & 2L) != 0L)
		{
			writer.WriteInt(_arena_autoFlagTimer);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize(ref _currentVersusFlag, OnVersusConditionChange, GeneratedNetworkCode._Read_VersusFlag(reader));
			GeneratedSyncVarDeserialize(ref _arena_autoFlagTimer, null, reader.ReadInt());
			return;
		}
		long num = (long)reader.ReadULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _currentVersusFlag, OnVersusConditionChange, GeneratedNetworkCode._Read_VersusFlag(reader));
		}
		if ((num & 2L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _arena_autoFlagTimer, null, reader.ReadInt());
		}
	}
}
