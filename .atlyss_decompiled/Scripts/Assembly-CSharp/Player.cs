using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Mirror;
using Mirror.RemoteCalls;
using Steamworks;
using TMPro;
using UnityEngine;

public class Player : NetworkBehaviour
{
	public static Player _mainPlayer;

	[Header("Reference")]
	public PlayerMove _pMove;

	public PlayerStats _pStats;

	public PlayerPvp _pPvp;

	public PlayerCombat _pCombat;

	public PlayerCasting _pCasting;

	public PlayerInventory _pInventory;

	public PlayerEquipment _pEquipment;

	public PlayerVisual _pVisual;

	public PlayerTargeting _pTargeting;

	public PlayerQuesting _pQuest;

	public PlayerProfession _pProfession;

	public PlayerInteract _pInteract;

	public PlayerSound _pSound;

	public StatusEntity _statusEntity;

	public ChatBehaviour _chatBehaviour;

	public NetworkTransformUnreliable _netTransform;

	[Space]
	[SerializeField]
	private GameObject[] _NonLocalPlayerDisableObjects;

	public GameObject _interactColliderObject;

	[Header("Player General Params")]
	[SyncVar(hook = "OnGameConditionChange")]
	public GameCondition _currentGameCondition;

	[SyncVar(hook = "OnPlayerConditionChange")]
	public PlayerCondition _currentPlayerCondition;

	public PlayerAction _currentPlayerAction;

	[Space]
	[SyncVar]
	public string _nickname = "null";

	[SyncVar]
	public string _globalNickname = "";

	[SerializeField]
	private TextMeshPro _nicknameTextMesh;

	[SerializeField]
	private TextMeshPro _globalNicknameTextMesh;

	[SyncVar]
	public int _latency;

	[Header("Steamworks")]
	[SyncVar]
	public string _steamID = string.Empty;

	[Header("Player Party Params")]
	[SyncVar]
	public PartyInviteStatus _partyInviteStatus;

	[SyncVar(hook = "OnPartyObjectChange")]
	public PartyObjectBehavior _partyObject;

	[SyncVar]
	public Player _invitedByPlayer;

	public SpriteRenderer _partyIndicatorIcon;

	public SpriteRenderer _partyLeaderIcon;

	private Player _cachedInvitedPlayer;

	private float _partyDeclineBuffer;

	[Header("Player Map Instancing")]
	[SyncVar]
	public ZoneType _playerZoneType;

	[SyncVar(hook = "OnPlayerMapInstanceChange")]
	public MapInstance _playerMapInstance;

	[SyncVar]
	public string _mapName = string.Empty;

	public List<string> _waypointAttunements = new List<string>();

	[Space]
	[SerializeField]
	private Transform _minimapIcon;

	[SerializeField]
	private SpriteRenderer _minimapIconRender;

	[Header("Recall Data")]
	[Scene]
	public string _recalledMapInstance = string.Empty;

	public string _recalledMapInstanceName = string.Empty;

	[SyncVar]
	public string _recallSpawnPoint = string.Empty;

	public string _lastSpawnPoint = string.Empty;

	[Header("Sync Flags")]
	[Tooltip("This is set to true when the player is playing and hosting the lobby / server.")]
	[SyncVar]
	public bool _isHostPlayer;

	[Tooltip("This is set to true when the player is away. This is set after a certain time threshold.")]
	[SyncVar(hook = "OnPlayerAfkChange")]
	public bool _isAfk;

	[Tooltip("This is set to true when the player is hurt from touching a hitbox and recieving damage.")]
	[SyncVar(hook = "OnPlayerHurt")]
	public bool _isHurt;

	[Tooltip("This is set to true when the player has invincibility frames. (example: Dashing)")]
	[SyncVar(hook = "OnPlayerIFrame")]
	public bool _inIFrame;

	[Header("Global Flags")]
	[Tooltip("This is set to true when the player is loading into a map or connecting to the server.")]
	public bool _bufferingStatus;

	[Tooltip("This is set to true when the player is looking in the Tab menu or on the general game menu.")]
	public bool _inUI;

	[Tooltip("This is set to true when the player is currently in the chat input field box.")]
	public bool _inChat;

	[Tooltip("This is set to true when the player is sitting down for resting.")]
	public bool _isSitting;

	[Tooltip("This is set to true when the player is hidden (stealth).")]
	public bool _isHidden;

	[Header("Runtime Buffers")]
	public float _spawnBuffer;

	public float _currentHurtBuffer;

	public float _knockbackBuffer;

	public float _afkBufferTimer;

	private float _currentIFrameBuffer;

	private float _noLatencyCheckBuffer;

	private float _steamIDCheckBuffer;

	private float _sitBuffer;

	private float _packetBuffer;

	private int _sentPackets;

	[SyncVar]
	public bool _canTeleportToLeader;

	public float _partyLeaderTeleportBuffer;

	protected NetworkBehaviourSyncVar ____partyObjectNetId;

	protected NetworkBehaviourSyncVar ____invitedByPlayerNetId;

	protected NetworkBehaviourSyncVar ____playerMapInstanceNetId;

	public GameCondition Network_currentGameCondition
	{
		get
		{
			return _currentGameCondition;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _currentGameCondition, 1uL, OnGameConditionChange);
		}
	}

	public PlayerCondition Network_currentPlayerCondition
	{
		get
		{
			return _currentPlayerCondition;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _currentPlayerCondition, 2uL, OnPlayerConditionChange);
		}
	}

	public string Network_nickname
	{
		get
		{
			return _nickname;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _nickname, 4uL, null);
		}
	}

	public string Network_globalNickname
	{
		get
		{
			return _globalNickname;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _globalNickname, 8uL, null);
		}
	}

	public int Network_latency
	{
		get
		{
			return _latency;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _latency, 16uL, null);
		}
	}

	public string Network_steamID
	{
		get
		{
			return _steamID;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _steamID, 32uL, null);
		}
	}

	public PartyInviteStatus Network_partyInviteStatus
	{
		get
		{
			return _partyInviteStatus;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _partyInviteStatus, 64uL, null);
		}
	}

	public PartyObjectBehavior Network_partyObject
	{
		get
		{
			return GetSyncVarNetworkBehaviour(____partyObjectNetId, ref _partyObject);
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter_NetworkBehaviour(value, ref _partyObject, 128uL, OnPartyObjectChange, ref ____partyObjectNetId);
		}
	}

	public Player Network_invitedByPlayer
	{
		get
		{
			return GetSyncVarNetworkBehaviour(____invitedByPlayerNetId, ref _invitedByPlayer);
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter_NetworkBehaviour(value, ref _invitedByPlayer, 256uL, null, ref ____invitedByPlayerNetId);
		}
	}

	public ZoneType Network_playerZoneType
	{
		get
		{
			return _playerZoneType;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _playerZoneType, 512uL, null);
		}
	}

	public MapInstance Network_playerMapInstance
	{
		get
		{
			return GetSyncVarNetworkBehaviour(____playerMapInstanceNetId, ref _playerMapInstance);
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter_NetworkBehaviour(value, ref _playerMapInstance, 1024uL, OnPlayerMapInstanceChange, ref ____playerMapInstanceNetId);
		}
	}

	public string Network_mapName
	{
		get
		{
			return _mapName;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _mapName, 2048uL, null);
		}
	}

	public string Network_recallSpawnPoint
	{
		get
		{
			return _recallSpawnPoint;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _recallSpawnPoint, 4096uL, null);
		}
	}

	public bool Network_isHostPlayer
	{
		get
		{
			return _isHostPlayer;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _isHostPlayer, 8192uL, null);
		}
	}

	public bool Network_isAfk
	{
		get
		{
			return _isAfk;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _isAfk, 16384uL, OnPlayerAfkChange);
		}
	}

	public bool Network_isHurt
	{
		get
		{
			return _isHurt;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _isHurt, 32768uL, OnPlayerHurt);
		}
	}

	public bool Network_inIFrame
	{
		get
		{
			return _inIFrame;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _inIFrame, 65536uL, OnPlayerIFrame);
		}
	}

	public bool Network_canTeleportToLeader
	{
		get
		{
			return _canTeleportToLeader;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _canTeleportToLeader, 131072uL, null);
		}
	}

	private void OnPlayerAfkChange(bool _oldBool, bool _newBool)
	{
		if (base.isLocalPlayer && !AtlyssNetworkManager._current._soloMode)
		{
			if (_newBool)
			{
				_chatBehaviour.Init_GameLogicMessage("You are now AFK.");
			}
			else
			{
				_chatBehaviour.Init_GameLogicMessage("You are no longer AFK.");
			}
		}
		if (_currentPlayerCondition == PlayerCondition.ACTIVE)
		{
			if (_newBool)
			{
				_pVisual.Local_CrossFadeAnim("sit_Init", 0f, 11);
			}
			else
			{
				_pVisual.Local_CrossFadeAnim("Empty", 0.06f, 11);
			}
		}
	}

	private void OnPlayerHurt(bool _oldBool, bool _newBool)
	{
		if (base.isLocalPlayer && base.isOwned)
		{
			_pCombat._airAttackLock = 0.85f;
		}
		Init_PlayerHurtDisplay();
		void Init_PlayerHurtDisplay()
		{
			if ((bool)_pVisual._playerRaceModel)
			{
				if (_newBool)
				{
					_pVisual._playerRaceModel.Set_EyeCondition(EyeCondition.Hurt, 0f);
					_pVisual._raceModelRenderMod.Tint_Shader(Color.red);
					_pVisual._hurtCloudEffect.Stop();
					_pVisual._hurtCloudEffect.Play();
				}
				else
				{
					_pVisual._playerRaceModel.Set_EyeCondition(EyeCondition.Hurt, 2.25f);
				}
			}
		}
	}

	private void OnPlayerIFrame(bool _oldBool, bool _newBool)
	{
		if (_newBool && (bool)_pVisual._playerRaceModel)
		{
			_pVisual._raceModelRenderMod.Shine_Shader(Color.white, 1f);
		}
	}

	private void OnGameConditionChange(GameCondition _oldCondition, GameCondition _newCondition)
	{
		if (base.isLocalPlayer)
		{
			Handle_LocalGameCondition();
		}
		if (_newCondition == GameCondition.LOADING_GAME)
		{
			_pVisual.RenderPlayerModel(_isTrue: false);
			_pVisual._teleportEffect.Play();
		}
		void Handle_LocalGameCondition()
		{
			switch (_newCondition)
			{
			case GameCondition.LOADING_GAME:
				LoadSceneManager._current.Init_LoadScreen();
				break;
			case GameCondition.IN_GAME:
				SettingsManager._current.Set_TextureFilterToggle();
				if (SettingsManager._current._networkFlagPvPOnMapEnter.isOn && _playerZoneType != ZoneType.Safe && _playerZoneType != ZoneType.Arena && !AtlyssNetworkManager._current._soloMode)
				{
					_pPvp.Cmd_FlagPvp(_isTrue: true);
				}
				LoadSceneManager._current.Disable_LoadScreen();
				CameraFunction._current._unlockedCamera = false;
				_pMove.Set_MovementAction(MovementAction.IDLE);
				_pMove._playerController.enabled = true;
				_pCasting.Init_QuerySkillStructs();
				Steamworks_SetRichPresence();
				break;
			}
		}
	}

	private void OnPlayerConditionChange(PlayerCondition _oldCondition, PlayerCondition _newCondition)
	{
		switch (_newCondition)
		{
		case PlayerCondition.ACTIVE:
			_pStats.Apply_StatCalculations();
			_pVisual.Local_CrossFadeAnim("Empty", 0f, 8);
			if (base.isLocalPlayer)
			{
				CameraFunction._current._unlockedCamera = false;
			}
			break;
		case PlayerCondition.DEAD:
			_pSound._aSrcGeneral.PlayOneShot(_pSound._generalSounds[24], 0.65f);
			Player_OnDeath();
			break;
		}
	}

	private void OnPlayerMapInstanceChange(MapInstance _old, MapInstance _new)
	{
		if (!(_new == null) && base.isLocalPlayer)
		{
			InGameUI._current.Init_MapTitleDisplay("");
		}
	}

	private void OnPartyObjectChange(PartyObjectBehavior _old, PartyObjectBehavior _new)
	{
		if (base.isLocalPlayer && _new != null)
		{
			SteamAchievementManager.current.Init_EarnAchievement("ATLYSS_ACHIEVEMENT_12");
		}
	}

	private void Awake()
	{
		_minimapIcon.transform.localScale = Vector3.one * 250f;
		_minimapIcon.transform.localPosition = Vector3.up * 100f;
	}

	public override void OnStartAuthority()
	{
		base.OnStartAuthority();
		LoadSceneManager._current.Init_LoadScreen();
		Network_isHostPlayer = base.isServer && base.isClient;
		_mainPlayer = this;
		if (!AtlyssNetworkManager._current._soloMode && !_isHostPlayer)
		{
			InvokeRepeating("Cache_Latency", 0f, 6f);
		}
		Init_SetWaypointData();
		void Init_SetWaypointData()
		{
			string[] waypointAttunementProfile = ProfileDataManager._current._characterFile._waypointAttunementProfile;
			if (_waypointAttunements.Count <= 0 && waypointAttunementProfile != null)
			{
				for (int i = 0; i < waypointAttunementProfile.Length; i++)
				{
					if (!_waypointAttunements.Contains(waypointAttunementProfile[i]))
					{
						_waypointAttunements.Add(waypointAttunementProfile[i]);
					}
				}
			}
			if (!_waypointAttunements.Contains("sanctum"))
			{
				_waypointAttunements.Add("sanctum");
			}
		}
	}

	private void Update()
	{
		Handle_GlobalParameters();
		Handle_ClientParameters();
		Handle_ServerParameters();
	}

	private void Handle_GlobalParameters()
	{
		if (_partyLeaderTeleportBuffer > 0f)
		{
			_partyLeaderTeleportBuffer -= Time.deltaTime;
		}
		else
		{
			_partyLeaderTeleportBuffer = 0f;
		}
		Handle_BufferingStatusCondition();
		_interactColliderObject.SetActive(_currentPlayerCondition == PlayerCondition.DEAD && !_bufferingStatus);
		if (!AtlyssNetworkManager._current._soloMode && !_isHostPlayer && !_bufferingStatus && _latency <= 0)
		{
			if (_noLatencyCheckBuffer < 4.5f)
			{
				_noLatencyCheckBuffer += Time.deltaTime;
			}
			else
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
		void Handle_BufferingStatusCondition()
		{
			_bufferingStatus = _currentPlayerCondition == PlayerCondition.CONNECTING || _currentGameCondition == GameCondition.LOADING_GAME;
			if (_bufferingStatus)
			{
				for (int i = 0; i < _NonLocalPlayerDisableObjects.Length; i++)
				{
					_NonLocalPlayerDisableObjects[i].SetActive(base.isLocalPlayer);
				}
			}
		}
	}

	[Command]
	private void Cmd_SetLatency(int _ping)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteInt(_ping);
		SendCommandInternal("System.Void Player::Cmd_SetLatency(System.Int32)", -1615410088, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[Command]
	private void Cmd_InitAfkCondition(bool _isTrue)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteBool(_isTrue);
		SendCommandInternal("System.Void Player::Cmd_InitAfkCondition(System.Boolean)", 488273671, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientCallback]
	private void Cache_Latency()
	{
		if (NetworkClient.active && !AtlyssNetworkManager._current._soloMode && NetworkClient.active && base.isLocalPlayer)
		{
			int ping = (int)Math.Round(NetworkTime.rtt * 1000.0);
			Cmd_SetLatency(ping);
		}
	}

	public void Player_OnDeath()
	{
		if (base.isServer)
		{
			_pCombat.Server_InterruptCombat(_resetAnimation: true);
		}
		_pVisual.Local_CrossFadeAnim("Idle", 0f, 0);
		_pVisual.Local_CrossFadeAnim("PreDeath", 0f, 8);
	}

	[ClientCallback]
	private void Handle_ClientParameters()
	{
		if (NetworkClient.active && (bool)_mainPlayer)
		{
			Handle_NameTagDisplay();
			Handle_MinimapIcon();
			Handle_LocalParams();
			Handle_PartyInviteStatus();
			Handle_PartyIndicatorDisplay();
		}
		void Handle_AFKCondition()
		{
			if (_isAfk && _afkBufferTimer < 0.2f)
			{
				_afkBufferTimer += Time.deltaTime;
			}
			if (!Input.anyKey)
			{
				if (_afkBufferTimer < 126f)
				{
					_afkBufferTimer += Time.deltaTime;
				}
			}
			else
			{
				if (_isAfk && _afkBufferTimer >= 0.2f)
				{
					Cmd_InitAfkCondition(_isTrue: false);
				}
				_afkBufferTimer = 0f;
			}
			if (_afkBufferTimer >= 125f && _afkBufferTimer < 126f)
			{
				_afkBufferTimer = 126f;
				Cmd_InitAfkCondition(_isTrue: true);
				_afkBufferTimer = 0f;
			}
		}
		void Handle_LocalParams()
		{
			if (base.isLocalPlayer && base.isOwned)
			{
				_inUI = TabMenu._current._isOpen || SettingsManager._current._isOpen || HostConsole._current._isOpen || (CameraFunction._current._unlockedCamera && !_pTargeting._isTargetingEntity) || DialogManager._current._currentDialogTrigger != null;
				_inChat = _chatBehaviour._focusedInChat || HostConsole._current._focusedInInput;
				if (_knockbackBuffer > 0f)
				{
					_knockbackBuffer -= Time.deltaTime;
				}
				else
				{
					_knockbackBuffer = 0f;
				}
				Handle_AFKCondition();
			}
		}
		void Handle_MinimapIcon()
		{
			bool flag = (bool)_mainPlayer && !SettingsManager._current._hideDungeonMinimap.isOn && _mapName == _mainPlayer._mapName;
			_minimapIcon.gameObject.SetActive(flag);
			if (flag)
			{
				if ((bool)Network_partyObject && (bool)_mainPlayer && _mainPlayer.Network_partyObject == Network_partyObject)
				{
					_minimapIconRender.color = Color.cyan;
				}
				else
				{
					_minimapIconRender.color = Color.grey;
				}
				if (_mainPlayer == this)
				{
					_minimapIconRender.color = Color.white;
				}
			}
		}
		void Handle_NameTagDisplay()
		{
			PhysicsScene physicsScene = base.gameObject.scene.GetPhysicsScene();
			Vector3 direction = _mainPlayer.transform.position - base.transform.position;
			string text = "";
			string text2 = "";
			if (_isHostPlayer && !AtlyssNetworkManager._current._soloMode)
			{
				text = "[HOST] ";
			}
			if (base.isClientOnly && base.gameObject.name != "_player(" + _nickname + ")")
			{
				base.gameObject.name = "_player(" + _nickname + ")";
			}
			if (_isAfk)
			{
				text2 = " (AFK)";
			}
			float num = Vector3.Distance(_mainPlayer.transform.position, base.transform.position);
			bool flag = false;
			if (physicsScene.Raycast(base.transform.position + Vector3.up * 2.5f, direction, out var hitInfo, float.PositiveInfinity, CastLayers.PathfinderMask))
			{
				flag = hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("Ground");
			}
			bool flag2 = num <= 20.5f && !flag;
			if (_mainPlayer == this)
			{
				_nicknameTextMesh.enabled = (InGameUI._current._displayUI && SettingsManager._current._networkDisplayLocalNameTag.isOn && !_bufferingStatus) & flag2;
				_globalNicknameTextMesh.enabled = (InGameUI._current._displayUI && !string.IsNullOrWhiteSpace(_globalNickname) && SettingsManager._current._networkDisplayLocalNameTag.isOn && !AtlyssNetworkManager._current._soloMode && !_bufferingStatus) & flag2;
			}
			else
			{
				_nicknameTextMesh.enabled = (InGameUI._current._displayUI && _mainPlayer.Network_playerMapInstance == Network_playerMapInstance && !_pVisual._forceHidden && !_bufferingStatus) & flag2;
				_globalNicknameTextMesh.enabled = (InGameUI._current._displayUI && !string.IsNullOrWhiteSpace(_globalNickname) && _mainPlayer.Network_playerMapInstance == Network_playerMapInstance && !AtlyssNetworkManager._current._soloMode && !_pVisual._forceHidden && !_bufferingStatus) & flag2;
			}
			if (_nicknameTextMesh.enabled && _nicknameTextMesh.text != $"{text}{_nickname} <color=yellow>(Lv-{_pStats._currentLevel})</color>{text2}")
			{
				_nicknameTextMesh.text = $"{text}{_nickname} <color=yellow>(Lv-{_pStats._currentLevel})</color>{text2}";
			}
			if (_globalNicknameTextMesh.enabled && _globalNicknameTextMesh.text != "@" + _globalNickname)
			{
				_globalNicknameTextMesh.text = "@" + _globalNickname;
			}
		}
	}

	[Command]
	public void Cmd_SetInactive()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void Player::Cmd_SetInactive()", 1466610852, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ServerCallback]
	private void Init_SitHeal()
	{
		if (NetworkServer.active)
		{
			_statusEntity.Add_Health((int)((float)_pStats._statStruct._maxHealth * GameManager._current._statLogics._sitRestHealPercent));
		}
	}

	[ServerCallback]
	public void Add_PacketCount()
	{
		if (NetworkServer.active && !AtlyssNetworkManager._current._soloMode && !_isHostPlayer)
		{
			_packetBuffer = 0.2f;
			_sentPackets++;
		}
	}

	[ServerCallback]
	private void Handle_ServerParameters()
	{
		if (!NetworkServer.active)
		{
			return;
		}
		if (base.netIdentity.connectionToClient == null)
		{
			NetworkServer.Destroy(base.gameObject);
			return;
		}
		Network_isHostPlayer = base.isLocalPlayer && base.isOwned;
		base.gameObject.name = $"[connID: {base.connectionToClient.connectionId}] _player({_nickname})";
		if ((bool)Network_playerMapInstance)
		{
			Network_mapName = Network_playerMapInstance._mapName;
		}
		Handle_NicknameParams();
		Handle_ServerBufferParams();
		Handle_RestFunctionParams();
		Handle_PartyParameters();
		void Handle_NicknameParams()
		{
			if (_nickname.Length > 18 || _nickname.Contains('<') || _nickname.Contains('>') || _nickname.Contains('/'))
			{
				Network_nickname = GameManager._current._statLogics._defaultNames[UnityEngine.Random.Range(0, GameManager._current._statLogics._defaultNames.Length)];
			}
		}
		void Handle_PartyParameters()
		{
			if (!AtlyssNetworkManager._current._soloMode)
			{
				if (_partyInviteStatus == PartyInviteStatus.INVITING)
				{
					Pend_PartyInvite();
				}
				else if (_partyInviteStatus == PartyInviteStatus.INVITED)
				{
					if (_partyDeclineBuffer < 20.45f)
					{
						_partyDeclineBuffer += Time.deltaTime;
					}
					else
					{
						Network_partyInviteStatus = PartyInviteStatus.DECLINED;
						_partyDeclineBuffer = 0f;
					}
				}
				else if (_partyInviteStatus == PartyInviteStatus.DECLINED)
				{
					if (_partyDeclineBuffer < 0.6f)
					{
						_partyDeclineBuffer += Time.deltaTime;
					}
					else
					{
						Network_partyInviteStatus = PartyInviteStatus.NULL;
						_partyDeclineBuffer = 0f;
					}
				}
				else
				{
					_partyDeclineBuffer = 0f;
				}
				if (_bufferingStatus)
				{
					Network_partyInviteStatus = PartyInviteStatus.NULL;
					_partyDeclineBuffer = 0f;
				}
			}
		}
		void Handle_RestFunctionParams()
		{
			if (_bufferingStatus || _currentPlayerCondition != PlayerCondition.ACTIVE)
			{
				Reset_SitRecoveryInvoke();
			}
			else
			{
				_isSitting = _pVisual._visualAnimator.GetCurrentAnimatorStateInfo(11).IsName("sit_Loop") || _pVisual._visualAnimator.GetCurrentAnimatorStateInfo(11).IsName("sit_Loop02");
				if (!_isSitting)
				{
					Reset_SitRecoveryInvoke();
				}
				else if (_sitBuffer < 10f)
				{
					_sitBuffer += Time.deltaTime;
				}
				if (_sitBuffer >= 10f && !IsInvoking("Init_SitHeal"))
				{
					_chatBehaviour.Target_GameLogicMessage("You're feeling rested...");
					InvokeRepeating("Init_SitHeal", 0f, 5f);
				}
			}
		}
		void Handle_ServerBufferParams()
		{
			Network_inIFrame = _currentIFrameBuffer > 0f;
			Network_isHurt = _currentHurtBuffer > 0f;
			_isHidden = _pVisual._forceHidden;
			if (_currentIFrameBuffer > 0f)
			{
				_currentIFrameBuffer -= Time.deltaTime;
			}
			else
			{
				_currentIFrameBuffer = 0f;
			}
			if (_spawnBuffer > 0f)
			{
				_spawnBuffer -= Time.deltaTime;
			}
			else
			{
				_spawnBuffer = 0f;
			}
			if (_currentHurtBuffer > 0f)
			{
				_currentHurtBuffer -= Time.deltaTime;
			}
			else
			{
				_currentHurtBuffer = 0f;
			}
			if (_packetBuffer > 0f)
			{
				_packetBuffer -= Time.deltaTime;
			}
			else
			{
				_packetBuffer = 0f;
				_sentPackets = 0;
			}
			if (_sentPackets >= 20)
			{
				base.connectionToClient.Disconnect();
			}
			Network_canTeleportToLeader = _partyLeaderTeleportBuffer <= 0f;
			if (!AtlyssNetworkManager._current._soloMode && AtlyssNetworkManager._current._steamworksMode && SteamManager.Initialized)
			{
				if (_steamIDCheckBuffer < 6.5f)
				{
					_steamIDCheckBuffer += Time.deltaTime;
				}
				else if (_steamIDCheckBuffer >= 6.5f && _steamID == string.Empty)
				{
					base.connectionToClient.Disconnect();
				}
			}
		}
		void Reset_SitRecoveryInvoke()
		{
			if (IsInvoking("Init_SitHeal"))
			{
				_chatBehaviour.Target_GameLogicMessage("You're no longer rested.");
				CancelInvoke("Init_SitHeal");
			}
			_sitBuffer = 0f;
			_isSitting = false;
		}
	}

	[ServerCallback]
	public void Set_IFrame(float _time)
	{
		if (NetworkServer.active)
		{
			_currentIFrameBuffer = _time;
			Network_inIFrame = true;
		}
	}

	[TargetRpc]
	public void Target_ResetCameraPositioning()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendTargetRPCInternal(null, "System.Void Player::Target_ResetCameraPositioning()", 1844751887, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void Init_CameraResetParams()
	{
		CameraFunction._current.CameraReset();
		_pMove._attackForce = 0f;
		_pMove._jumpForwardForce = 0f;
		_pMove._ctrlJumpForce = 0f;
		_pMove._playerController.enabled = true;
	}

	[Command]
	public void Cmd_Respawn()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void Player::Cmd_Respawn()", 1763004191, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ServerCallback]
	private void Init_RespawnParams()
	{
		if (NetworkServer.active)
		{
			_statusEntity.Replenish_All();
			Target_ResetCameraPositioning();
		}
	}

	[Command]
	public void Cmd_ReturnToRecalledInstance()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void Player::Cmd_ReturnToRecalledInstance()", -468714789, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[Command]
	public void Cmd_SceneTransport(string _subScene, string _spawnTag, ZoneDifficulty _difficultyValue)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteString(_subScene);
		writer.WriteString(_spawnTag);
		GeneratedNetworkCode._Write_ZoneDifficulty(writer, _difficultyValue);
		SendCommandInternal("System.Void Player::Cmd_SceneTransport(System.String,System.String,ZoneDifficulty)", -1717696887, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ServerCallback]
	public void Server_SceneTransport(string _subScene, string _spawnTag, ZoneDifficulty _difficultyValue)
	{
		if (!NetworkServer.active)
		{
			return;
		}
		MapInstancingManager mapInstanceManager = AtlyssNetworkManager._current._mapInstanceManager;
		_pCombat.Network_currentSheathCondition = WeaponSheatheCondition.PUT_AWAY;
		Add_PacketCount();
		Set_IFrame(2.5f);
		if (_subScene == base.gameObject.scene.path)
		{
			Transform transform = null;
			for (int i = 0; i < Network_playerMapInstance._instanceSpawnPoints.Count; i++)
			{
				if (Network_playerMapInstance._instanceSpawnPoints[i]._spawnPointTag == _spawnTag)
				{
					transform = Network_playerMapInstance._instanceSpawnPoints[i].transform;
					break;
				}
			}
			_netTransform.RpcTeleport(transform.position, transform.rotation);
			_spawnBuffer = 1.25f;
			_pVisual.Rpc_PlayTeleportEffect();
			Init_RespawnParams();
		}
		else
		{
			mapInstanceManager.Init_SendPeer_ToScene(this, base.gameObject.scene.path, _subScene, _spawnTag, _difficultyValue);
		}
	}

	[Command]
	public void Cmd_TeleportToPartyLeader()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void Player::Cmd_TeleportToPartyLeader()", 1358367712, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[Command]
	public void Cmd_InviteToParty(Player _targetPlayer)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteNetworkBehaviour(_targetPlayer);
		SendCommandInternal("System.Void Player::Cmd_InviteToParty(Player)", -132919816, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[Command]
	public void Cmd_LeaveParty()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void Player::Cmd_LeaveParty()", 1132628218, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[Command]
	public void Cmd_SetPartyInviteCondition(PartyInviteStatus _status)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		GeneratedNetworkCode._Write_PartyInviteStatus(writer, _status);
		SendCommandInternal("System.Void Player::Cmd_SetPartyInviteCondition(PartyInviteStatus)", -1859378836, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[Command]
	public void Cmd_GiveLeadership(Player _p)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteNetworkBehaviour(_p);
		SendCommandInternal("System.Void Player::Cmd_GiveLeadership(Player)", 1334343584, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientCallback]
	private void Handle_PartyIndicatorDisplay()
	{
		if (NetworkClient.active && (bool)_mainPlayer)
		{
			if (_mainPlayer.Network_playerMapInstance != Network_playerMapInstance)
			{
				_partyIndicatorIcon.enabled = false;
				_partyLeaderIcon.enabled = false;
			}
			else
			{
				_partyIndicatorIcon.enabled = (bool)Network_partyObject && _mainPlayer.Network_partyObject == Network_partyObject && !_bufferingStatus && !base.isLocalPlayer;
				_partyLeaderIcon.enabled = _partyIndicatorIcon.enabled && Network_partyObject.Network_partyLeaderPlayer == this;
			}
		}
	}

	[ClientCallback]
	private void Handle_PartyInviteStatus()
	{
		if (!NetworkClient.active || AtlyssNetworkManager._current._soloMode || !base.isLocalPlayer)
		{
			return;
		}
		if (_partyInviteStatus == PartyInviteStatus.INVITED)
		{
			if (!PartyUIManager._current._partyInviteElement.isEnabled)
			{
				PartyUIManager._current.Init_PartyInviteWindow();
			}
		}
		else
		{
			PartyUIManager._current._partyInviteElement.isEnabled = false;
		}
		if (_partyInviteStatus == PartyInviteStatus.INVITED)
		{
			PartyUIManager._current._invitedByPrompt.text = Network_invitedByPlayer._nickname + " invited you to a party.";
		}
	}

	[ServerCallback]
	public void Pend_PartyInvite()
	{
		if (!NetworkServer.active)
		{
			return;
		}
		if (_cachedInvitedPlayer == null)
		{
			_chatBehaviour.Target_RecieveMessage("Lost connection with invited player.");
			Network_partyInviteStatus = PartyInviteStatus.NULL;
			_cachedInvitedPlayer = null;
		}
		else
		{
			if (_partyInviteStatus != PartyInviteStatus.INVITING || !_cachedInvitedPlayer)
			{
				return;
			}
			if (_cachedInvitedPlayer._partyInviteStatus == PartyInviteStatus.DECLINED)
			{
				_chatBehaviour.Target_RecieveMessage(_cachedInvitedPlayer._nickname + " declined your invite.");
				_cachedInvitedPlayer.Network_partyInviteStatus = PartyInviteStatus.NULL;
				_cachedInvitedPlayer.Network_invitedByPlayer = null;
				Network_partyInviteStatus = PartyInviteStatus.NULL;
			}
			else if (_cachedInvitedPlayer._partyInviteStatus == PartyInviteStatus.ACCEPTED)
			{
				if (!Network_partyObject)
				{
					GameObject obj = UnityEngine.Object.Instantiate(AtlyssNetworkManager._current._networkObjectLibrary._partyObject);
					PartyObjectBehavior component = obj.GetComponent<PartyObjectBehavior>();
					NetworkServer.Spawn(obj);
					component._syncPartyPeers.Add(this);
					component.Network_partyLeaderPlayer = this;
					Network_partyObject = component;
				}
				else if (Network_partyObject.Network_partyLeaderPlayer != this)
				{
					return;
				}
				if (Network_partyObject._syncPartyPeers.Count < Network_partyObject._maxPartyLimit)
				{
					_cachedInvitedPlayer.Network_partyObject = Network_partyObject;
					Network_partyObject._syncPartyPeers.Add(_cachedInvitedPlayer);
					Network_partyInviteStatus = PartyInviteStatus.NULL;
					_cachedInvitedPlayer.Network_partyInviteStatus = PartyInviteStatus.NULL;
					_cachedInvitedPlayer = null;
				}
			}
		}
	}

	public void Steamworks_SetRichPresence()
	{
		if (SteamManager.Initialized)
		{
			if ((bool)Network_playerMapInstance)
			{
				SteamFriends.SetRichPresence("MAPINSTANCE", Network_playerMapInstance._mapName ?? "");
			}
			else
			{
				SteamFriends.SetRichPresence("MAPINSTANCE", "");
			}
			SteamFriends.SetRichPresence("LEVEL", $"{_pStats._currentLevel}");
			if (!_pStats._class)
			{
				SteamFriends.SetRichPresence("CLASS", GameManager._current._statLogics._emptyClassName ?? "");
			}
			else if (_pStats._syncClassTier <= 0)
			{
				SteamFriends.SetRichPresence("CLASS", _pStats._class._className ?? "");
			}
			else
			{
				SteamFriends.SetRichPresence("CLASS", _pStats._class._playerClassTiers[_pStats._syncClassTier - 1]._classTierName ?? "");
			}
			SteamFriends.SetRichPresence("steam_display", "#Status_GameStatus");
		}
	}

	public override bool Weaved()
	{
		return true;
	}

	protected void UserCode_Cmd_SetLatency__Int32(int _ping)
	{
		Network_latency = _ping;
	}

	protected static void InvokeUserCode_Cmd_SetLatency__Int32(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command Cmd_SetLatency called on client.");
		}
		else
		{
			((Player)obj).UserCode_Cmd_SetLatency__Int32(reader.ReadInt());
		}
	}

	protected void UserCode_Cmd_InitAfkCondition__Boolean(bool _isTrue)
	{
		Network_isAfk = _isTrue;
	}

	protected static void InvokeUserCode_Cmd_InitAfkCondition__Boolean(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command Cmd_InitAfkCondition called on client.");
		}
		else
		{
			((Player)obj).UserCode_Cmd_InitAfkCondition__Boolean(reader.ReadBool());
		}
	}

	protected void UserCode_Cmd_SetInactive()
	{
		Network_currentPlayerCondition = PlayerCondition.CONNECTING;
	}

	protected static void InvokeUserCode_Cmd_SetInactive(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command Cmd_SetInactive called on client.");
		}
		else
		{
			((Player)obj).UserCode_Cmd_SetInactive();
		}
	}

	protected void UserCode_Target_ResetCameraPositioning()
	{
		Invoke("Init_CameraResetParams", 0.05f);
	}

	protected static void InvokeUserCode_Target_ResetCameraPositioning(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC Target_ResetCameraPositioning called on server.");
		}
		else
		{
			((Player)obj).UserCode_Target_ResetCameraPositioning();
		}
	}

	protected void UserCode_Cmd_Respawn()
	{
		if (_currentPlayerCondition != PlayerCondition.ACTIVE && _statusEntity._currentHealth <= 0)
		{
			MapInstancingManager mapInstanceManager = AtlyssNetworkManager._current._mapInstanceManager;
			switch (_playerZoneType)
			{
			case ZoneType.Safe:
				_netTransform.RpcTeleport(Network_playerMapInstance._instanceSpawnPoints[0].transform.position);
				break;
			case ZoneType.Field:
				Server_SceneTransport(mapInstanceManager._hubMap, mapInstanceManager._hubRespawnPointTag, ZoneDifficulty.NORMAL);
				break;
			case ZoneType.Dungeon:
			{
				SpawnPoint setRespawnPoint = Network_playerMapInstance._patternInstance._setInstancePattern._setRespawnPoint;
				_netTransform.RpcTeleport(setRespawnPoint.transform.position, setRespawnPoint.transform.rotation);
				break;
			}
			case ZoneType.Arena:
				_netTransform.RpcTeleport(Network_playerMapInstance._instanceSpawnPoints[UnityEngine.Random.Range(0, Network_playerMapInstance._instanceSpawnPoints.Count)].transform.position);
				break;
			}
			_spawnBuffer = 1.25f;
			Add_PacketCount();
			_pVisual.Rpc_PlayTeleportEffect();
			if (_playerZoneType == ZoneType.Field)
			{
				Invoke("Init_RespawnParams", 0.6f);
			}
			else
			{
				Init_RespawnParams();
			}
		}
	}

	protected static void InvokeUserCode_Cmd_Respawn(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command Cmd_Respawn called on client.");
		}
		else
		{
			((Player)obj).UserCode_Cmd_Respawn();
		}
	}

	protected void UserCode_Cmd_ReturnToRecalledInstance()
	{
		if (!string.IsNullOrEmpty(_recalledMapInstance))
		{
			Server_SceneTransport(_recalledMapInstance, _recallSpawnPoint, ZoneDifficulty.NORMAL);
			Network_recallSpawnPoint = string.Empty;
		}
	}

	protected static void InvokeUserCode_Cmd_ReturnToRecalledInstance(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command Cmd_ReturnToRecalledInstance called on client.");
		}
		else
		{
			((Player)obj).UserCode_Cmd_ReturnToRecalledInstance();
		}
	}

	protected void UserCode_Cmd_SceneTransport__String__String__ZoneDifficulty(string _subScene, string _spawnTag, ZoneDifficulty _difficultyValue)
	{
		Server_SceneTransport(_subScene, _spawnTag, _difficultyValue);
	}

	protected static void InvokeUserCode_Cmd_SceneTransport__String__String__ZoneDifficulty(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command Cmd_SceneTransport called on client.");
		}
		else
		{
			((Player)obj).UserCode_Cmd_SceneTransport__String__String__ZoneDifficulty(reader.ReadString(), reader.ReadString(), GeneratedNetworkCode._Read_ZoneDifficulty(reader));
		}
	}

	protected void UserCode_Cmd_TeleportToPartyLeader()
	{
		if ((bool)Network_partyObject && !(Network_partyObject.Network_partyLeaderPlayer == null) && _canTeleportToLeader && !Network_partyObject.Network_partyLeaderPlayer._bufferingStatus && (bool)Network_partyObject.Network_partyLeaderPlayer.Network_playerMapInstance)
		{
			Add_PacketCount();
			_partyLeaderTeleportBuffer = GameManager._current._statLogics._teleportToLeaderCooldown;
			Player network_partyLeaderPlayer = Network_partyObject.Network_partyLeaderPlayer;
			ZoneDifficulty difficultyValue = ZoneDifficulty.NORMAL;
			if ((bool)network_partyLeaderPlayer.Network_playerMapInstance._patternInstance)
			{
				difficultyValue = network_partyLeaderPlayer.Network_playerMapInstance._patternInstance._setDungeonDifficulty;
			}
			Server_SceneTransport(network_partyLeaderPlayer.gameObject.scene.path, network_partyLeaderPlayer._lastSpawnPoint, difficultyValue);
		}
	}

	protected static void InvokeUserCode_Cmd_TeleportToPartyLeader(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command Cmd_TeleportToPartyLeader called on client.");
		}
		else
		{
			((Player)obj).UserCode_Cmd_TeleportToPartyLeader();
		}
	}

	protected void UserCode_Cmd_InviteToParty__Player(Player _targetPlayer)
	{
		if (!_targetPlayer)
		{
			_chatBehaviour.Target_RecieveMessage("Player not found.");
			return;
		}
		if ((bool)_targetPlayer.Network_partyObject)
		{
			_chatBehaviour.Target_RecieveMessage(_targetPlayer._nickname + " is already in a party.");
			return;
		}
		if (_targetPlayer._bufferingStatus)
		{
			_chatBehaviour.Target_RecieveMessage(_targetPlayer._nickname + " is currently busy.");
			return;
		}
		if (_targetPlayer._partyInviteStatus != PartyInviteStatus.NULL)
		{
			_chatBehaviour.Target_RecieveMessage(_targetPlayer._nickname + " is currently busy.");
			return;
		}
		Network_partyInviteStatus = PartyInviteStatus.INVITING;
		_targetPlayer.Network_partyInviteStatus = PartyInviteStatus.INVITED;
		_chatBehaviour.Target_RecieveMessage("Inviting " + _targetPlayer._nickname + "...");
		_cachedInvitedPlayer = _targetPlayer;
		_cachedInvitedPlayer.Network_invitedByPlayer = this;
	}

	protected static void InvokeUserCode_Cmd_InviteToParty__Player(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command Cmd_InviteToParty called on client.");
		}
		else
		{
			((Player)obj).UserCode_Cmd_InviteToParty__Player(reader.ReadNetworkBehaviour<Player>());
		}
	}

	protected void UserCode_Cmd_LeaveParty()
	{
		Network_partyObject = null;
	}

	protected static void InvokeUserCode_Cmd_LeaveParty(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command Cmd_LeaveParty called on client.");
		}
		else
		{
			((Player)obj).UserCode_Cmd_LeaveParty();
		}
	}

	protected void UserCode_Cmd_SetPartyInviteCondition__PartyInviteStatus(PartyInviteStatus _status)
	{
		Network_partyInviteStatus = _status;
	}

	protected static void InvokeUserCode_Cmd_SetPartyInviteCondition__PartyInviteStatus(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command Cmd_SetPartyInviteCondition called on client.");
		}
		else
		{
			((Player)obj).UserCode_Cmd_SetPartyInviteCondition__PartyInviteStatus(GeneratedNetworkCode._Read_PartyInviteStatus(reader));
		}
	}

	protected void UserCode_Cmd_GiveLeadership__Player(Player _p)
	{
		if (!(_p == null) && (bool)Network_partyObject && !(Network_partyObject.Network_partyLeaderPlayer != this) && (bool)_p.Network_partyObject && !(_p.Network_partyObject != Network_partyObject))
		{
			Network_partyObject.Network_partyLeaderPlayer = _p;
			_chatBehaviour.Target_RecieveTriggerMessage("You gave " + _p._nickname + " party leadership.");
			_p._chatBehaviour.Target_RecieveTriggerMessage("You are now the party leader.");
		}
	}

	protected static void InvokeUserCode_Cmd_GiveLeadership__Player(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command Cmd_GiveLeadership called on client.");
		}
		else
		{
			((Player)obj).UserCode_Cmd_GiveLeadership__Player(reader.ReadNetworkBehaviour<Player>());
		}
	}

	static Player()
	{
		RemoteProcedureCalls.RegisterCommand(typeof(Player), "System.Void Player::Cmd_SetLatency(System.Int32)", InvokeUserCode_Cmd_SetLatency__Int32, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(Player), "System.Void Player::Cmd_InitAfkCondition(System.Boolean)", InvokeUserCode_Cmd_InitAfkCondition__Boolean, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(Player), "System.Void Player::Cmd_SetInactive()", InvokeUserCode_Cmd_SetInactive, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(Player), "System.Void Player::Cmd_Respawn()", InvokeUserCode_Cmd_Respawn, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(Player), "System.Void Player::Cmd_ReturnToRecalledInstance()", InvokeUserCode_Cmd_ReturnToRecalledInstance, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(Player), "System.Void Player::Cmd_SceneTransport(System.String,System.String,ZoneDifficulty)", InvokeUserCode_Cmd_SceneTransport__String__String__ZoneDifficulty, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(Player), "System.Void Player::Cmd_TeleportToPartyLeader()", InvokeUserCode_Cmd_TeleportToPartyLeader, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(Player), "System.Void Player::Cmd_InviteToParty(Player)", InvokeUserCode_Cmd_InviteToParty__Player, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(Player), "System.Void Player::Cmd_LeaveParty()", InvokeUserCode_Cmd_LeaveParty, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(Player), "System.Void Player::Cmd_SetPartyInviteCondition(PartyInviteStatus)", InvokeUserCode_Cmd_SetPartyInviteCondition__PartyInviteStatus, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(Player), "System.Void Player::Cmd_GiveLeadership(Player)", InvokeUserCode_Cmd_GiveLeadership__Player, requiresAuthority: true);
		RemoteProcedureCalls.RegisterRpc(typeof(Player), "System.Void Player::Target_ResetCameraPositioning()", InvokeUserCode_Target_ResetCameraPositioning);
	}

	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			GeneratedNetworkCode._Write_GameCondition(writer, _currentGameCondition);
			GeneratedNetworkCode._Write_PlayerCondition(writer, _currentPlayerCondition);
			writer.WriteString(_nickname);
			writer.WriteString(_globalNickname);
			writer.WriteInt(_latency);
			writer.WriteString(_steamID);
			GeneratedNetworkCode._Write_PartyInviteStatus(writer, _partyInviteStatus);
			writer.WriteNetworkBehaviour(Network_partyObject);
			writer.WriteNetworkBehaviour(Network_invitedByPlayer);
			GeneratedNetworkCode._Write_ZoneType(writer, _playerZoneType);
			writer.WriteNetworkBehaviour(Network_playerMapInstance);
			writer.WriteString(_mapName);
			writer.WriteString(_recallSpawnPoint);
			writer.WriteBool(_isHostPlayer);
			writer.WriteBool(_isAfk);
			writer.WriteBool(_isHurt);
			writer.WriteBool(_inIFrame);
			writer.WriteBool(_canTeleportToLeader);
			return;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 1L) != 0L)
		{
			GeneratedNetworkCode._Write_GameCondition(writer, _currentGameCondition);
		}
		if ((base.syncVarDirtyBits & 2L) != 0L)
		{
			GeneratedNetworkCode._Write_PlayerCondition(writer, _currentPlayerCondition);
		}
		if ((base.syncVarDirtyBits & 4L) != 0L)
		{
			writer.WriteString(_nickname);
		}
		if ((base.syncVarDirtyBits & 8L) != 0L)
		{
			writer.WriteString(_globalNickname);
		}
		if ((base.syncVarDirtyBits & 0x10L) != 0L)
		{
			writer.WriteInt(_latency);
		}
		if ((base.syncVarDirtyBits & 0x20L) != 0L)
		{
			writer.WriteString(_steamID);
		}
		if ((base.syncVarDirtyBits & 0x40L) != 0L)
		{
			GeneratedNetworkCode._Write_PartyInviteStatus(writer, _partyInviteStatus);
		}
		if ((base.syncVarDirtyBits & 0x80L) != 0L)
		{
			writer.WriteNetworkBehaviour(Network_partyObject);
		}
		if ((base.syncVarDirtyBits & 0x100L) != 0L)
		{
			writer.WriteNetworkBehaviour(Network_invitedByPlayer);
		}
		if ((base.syncVarDirtyBits & 0x200L) != 0L)
		{
			GeneratedNetworkCode._Write_ZoneType(writer, _playerZoneType);
		}
		if ((base.syncVarDirtyBits & 0x400L) != 0L)
		{
			writer.WriteNetworkBehaviour(Network_playerMapInstance);
		}
		if ((base.syncVarDirtyBits & 0x800L) != 0L)
		{
			writer.WriteString(_mapName);
		}
		if ((base.syncVarDirtyBits & 0x1000L) != 0L)
		{
			writer.WriteString(_recallSpawnPoint);
		}
		if ((base.syncVarDirtyBits & 0x2000L) != 0L)
		{
			writer.WriteBool(_isHostPlayer);
		}
		if ((base.syncVarDirtyBits & 0x4000L) != 0L)
		{
			writer.WriteBool(_isAfk);
		}
		if ((base.syncVarDirtyBits & 0x8000L) != 0L)
		{
			writer.WriteBool(_isHurt);
		}
		if ((base.syncVarDirtyBits & 0x10000L) != 0L)
		{
			writer.WriteBool(_inIFrame);
		}
		if ((base.syncVarDirtyBits & 0x20000L) != 0L)
		{
			writer.WriteBool(_canTeleportToLeader);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize(ref _currentGameCondition, OnGameConditionChange, GeneratedNetworkCode._Read_GameCondition(reader));
			GeneratedSyncVarDeserialize(ref _currentPlayerCondition, OnPlayerConditionChange, GeneratedNetworkCode._Read_PlayerCondition(reader));
			GeneratedSyncVarDeserialize(ref _nickname, null, reader.ReadString());
			GeneratedSyncVarDeserialize(ref _globalNickname, null, reader.ReadString());
			GeneratedSyncVarDeserialize(ref _latency, null, reader.ReadInt());
			GeneratedSyncVarDeserialize(ref _steamID, null, reader.ReadString());
			GeneratedSyncVarDeserialize(ref _partyInviteStatus, null, GeneratedNetworkCode._Read_PartyInviteStatus(reader));
			GeneratedSyncVarDeserialize_NetworkBehaviour(ref _partyObject, OnPartyObjectChange, reader, ref ____partyObjectNetId);
			GeneratedSyncVarDeserialize_NetworkBehaviour(ref _invitedByPlayer, null, reader, ref ____invitedByPlayerNetId);
			GeneratedSyncVarDeserialize(ref _playerZoneType, null, GeneratedNetworkCode._Read_ZoneType(reader));
			GeneratedSyncVarDeserialize_NetworkBehaviour(ref _playerMapInstance, OnPlayerMapInstanceChange, reader, ref ____playerMapInstanceNetId);
			GeneratedSyncVarDeserialize(ref _mapName, null, reader.ReadString());
			GeneratedSyncVarDeserialize(ref _recallSpawnPoint, null, reader.ReadString());
			GeneratedSyncVarDeserialize(ref _isHostPlayer, null, reader.ReadBool());
			GeneratedSyncVarDeserialize(ref _isAfk, OnPlayerAfkChange, reader.ReadBool());
			GeneratedSyncVarDeserialize(ref _isHurt, OnPlayerHurt, reader.ReadBool());
			GeneratedSyncVarDeserialize(ref _inIFrame, OnPlayerIFrame, reader.ReadBool());
			GeneratedSyncVarDeserialize(ref _canTeleportToLeader, null, reader.ReadBool());
			return;
		}
		long num = (long)reader.ReadULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _currentGameCondition, OnGameConditionChange, GeneratedNetworkCode._Read_GameCondition(reader));
		}
		if ((num & 2L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _currentPlayerCondition, OnPlayerConditionChange, GeneratedNetworkCode._Read_PlayerCondition(reader));
		}
		if ((num & 4L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _nickname, null, reader.ReadString());
		}
		if ((num & 8L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _globalNickname, null, reader.ReadString());
		}
		if ((num & 0x10L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _latency, null, reader.ReadInt());
		}
		if ((num & 0x20L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _steamID, null, reader.ReadString());
		}
		if ((num & 0x40L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _partyInviteStatus, null, GeneratedNetworkCode._Read_PartyInviteStatus(reader));
		}
		if ((num & 0x80L) != 0L)
		{
			GeneratedSyncVarDeserialize_NetworkBehaviour(ref _partyObject, OnPartyObjectChange, reader, ref ____partyObjectNetId);
		}
		if ((num & 0x100L) != 0L)
		{
			GeneratedSyncVarDeserialize_NetworkBehaviour(ref _invitedByPlayer, null, reader, ref ____invitedByPlayerNetId);
		}
		if ((num & 0x200L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _playerZoneType, null, GeneratedNetworkCode._Read_ZoneType(reader));
		}
		if ((num & 0x400L) != 0L)
		{
			GeneratedSyncVarDeserialize_NetworkBehaviour(ref _playerMapInstance, OnPlayerMapInstanceChange, reader, ref ____playerMapInstanceNetId);
		}
		if ((num & 0x800L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _mapName, null, reader.ReadString());
		}
		if ((num & 0x1000L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _recallSpawnPoint, null, reader.ReadString());
		}
		if ((num & 0x2000L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _isHostPlayer, null, reader.ReadBool());
		}
		if ((num & 0x4000L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _isAfk, OnPlayerAfkChange, reader.ReadBool());
		}
		if ((num & 0x8000L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _isHurt, OnPlayerHurt, reader.ReadBool());
		}
		if ((num & 0x10000L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _inIFrame, OnPlayerIFrame, reader.ReadBool());
		}
		if ((num & 0x20000L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _canTeleportToLeader, null, reader.ReadBool());
		}
	}
}
