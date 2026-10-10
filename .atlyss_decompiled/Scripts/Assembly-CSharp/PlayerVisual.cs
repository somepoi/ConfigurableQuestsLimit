using System.Collections;
using System.Runtime.InteropServices;
using Mirror;
using Mirror.RemoteCalls;
using UnityEngine;

public class PlayerVisual : NetworkBehaviour
{
	private enum HideRenderType : byte
	{
		HELM = 0,
		CAPE = 1,
		CHEST = 2,
		LEGGINGS = 3
	}

	private Player _player;

	private PlayerMove _pMove;

	private PlayerSound _pSound;

	private PlayerWater _pWater;

	private PlayerCombat _pCombat;

	private PlayerCasting _pCasting;

	private PlayerStats _pStats;

	private static readonly int _HUE;

	private static readonly int _SATURATION;

	private static readonly int _BRIGHTNESS;

	private static readonly int _CONTRAST;

	private static readonly int _TINT;

	[Header("Player Visual Parameters")]
	[SyncVar(hook = "OnAppearanceStructChange")]
	public PlayerAppearanceStruct _playerAppearanceStruct;

	[Header("Player Model")]
	[SerializeField]
	private Transform _raceModelContainer;

	public RotateObject _raceModelRotater;

	public bool _preventRotationLerp;

	private Transform[] _rendObjects;

	public GameObject _raceModelObject;

	public PlayerRaceModel _playerRaceModel;

	public RaceModelEquipDisplay _raceModelED;

	public RenderModify _raceModelRenderMod;

	public GameObject _shadowProjectionObject;

	public Color _stealthColor = Color.grey;

	public Vector3 _headPos;

	private bool _isCulledDisplay;

	[SyncVar]
	public bool _forceHidden;

	[Header("Player Animator")]
	public Animator _visualAnimator;

	public PlayerModelAnimManager _raceModelAnimManager;

	[SyncVar]
	public float _movementAnimSpeedMod;

	public float _animSpeedModify = 0.04f;

	[Header("Effects")]
	public LerpScale _visualContainerLerpScale;

	public LerpScale _blockOrbEffect;

	public MeshRenderer _blockOrbRender;

	[Space]
	public ParticleSystem _vanity_sparkleEffect;

	public ParticleSystem _poofEffect;

	[Space]
	public ParticleSystem _teleportEffect;

	public ParticleSystem _blockShatterEffect;

	public ParticleSystem _hurtCloudEffect;

	public ParticleSystem _attackForceCloudEffect;

	public ParticleSystem _coinSellEffect;

	public ParticleSystem _deathSoulEffect;

	public ParticleSystem _parryWindowEffect;

	[Space]
	public ParticleSystem _landEffect;

	public ParticleSystem _dashEffect;

	public ParticleSystem _doubleJumpRingEffect;

	public ParticleSystem _spinJumpEffect;

	public ParticleSystem[] _footStepEffects;

	[Space]
	public ParticleSystem _waterEnterEffect;

	public ParticleSystem _waterExitEffect;

	public ParticleSystem _waterRippleEffect;

	public ParticleSystem _waterRippleEffectFoot;

	public float _wateRippleDetector = 3.5f;

	public float _wateRippleFootDetector = 3.5f;

	[Space]
	public ParticleSystem _attributeCircleEffect;

	public ParticleSystem[] _attributeEffects;

	private bool _isCulled;

	public PlayerAppearanceStruct Network_playerAppearanceStruct
	{
		get
		{
			return _playerAppearanceStruct;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _playerAppearanceStruct, 1uL, OnAppearanceStructChange);
		}
	}

	public bool Network_forceHidden
	{
		get
		{
			return _forceHidden;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _forceHidden, 2uL, null);
		}
	}

	public float Network_movementAnimSpeedMod
	{
		get
		{
			return _movementAnimSpeedMod;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _movementAnimSpeedMod, 4uL, null);
		}
	}

	private void OnAppearanceStructChange(PlayerAppearanceStruct _old, PlayerAppearanceStruct _new)
	{
		Apply_NetworkedCharacterDisplay();
	}

	[Command]
	public void Cmd_SendNew_PlayerAppearanceStruct(PlayerAppearanceStruct _newStruct)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		GeneratedNetworkCode._Write_PlayerAppearanceStruct(writer, _newStruct);
		SendCommandInternal("System.Void PlayerVisual::Cmd_SendNew_PlayerAppearanceStruct(PlayerAppearanceStruct)", 62916306, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void Awake()
	{
		_player = GetComponent<Player>();
		_pMove = GetComponent<PlayerMove>();
		_pSound = GetComponent<PlayerSound>();
		_pCombat = GetComponent<PlayerCombat>();
		_pStats = GetComponent<PlayerStats>();
		_pCasting = GetComponent<PlayerCasting>();
		_pWater = GetComponentInChildren<PlayerWater>();
	}

	private void Start()
	{
		StartCoroutine(Create_PlayerRaceModel());
	}

	private IEnumerator Create_PlayerRaceModel()
	{
		do
		{
			yield return null;
		}
		while (_playerAppearanceStruct._setRaceTag == string.Empty);
		if ((bool)_raceModelObject)
		{
			yield break;
		}
		ScriptablePlayerRace scriptablePlayerRace = GameManager._current.Locate_PlayerRace(_playerAppearanceStruct._setRaceTag);
		if ((bool)scriptablePlayerRace)
		{
			GameObject raceModelObject = Object.Instantiate(scriptablePlayerRace._raceModel);
			_raceModelObject = raceModelObject;
			_raceModelObject.transform.SetParent(_raceModelContainer);
			_raceModelRenderMod = _raceModelObject.GetComponentInChildren<RenderModify>();
			_visualAnimator = _raceModelObject.GetComponentInChildren<Animator>();
			_playerRaceModel = _raceModelObject.GetComponent<PlayerRaceModel>();
			_raceModelED = _raceModelObject.GetComponent<RaceModelEquipDisplay>();
			_raceModelObject.transform.localPosition = Vector3.zero;
			_raceModelObject.transform.rotation = base.transform.rotation;
			_raceModelED.Remove_ChestPieceDisplay();
			_raceModelED.Remove_HelmDisplay();
			_raceModelED.Remove_LeggingsDisplay();
			_raceModelED.Remove_WeaponDisplay();
			_raceModelAnimManager = _playerRaceModel.GetComponentInChildren<PlayerModelAnimManager>();
			_rendObjects = _raceModelRotater.gameObject.GetComponentsInChildren<Transform>(includeInactive: true);
			RenderPlayerModel(_isTrue: false);
			_pSound.ReCache_AudioSources();
			yield return new WaitForSeconds(0.45f);
			if (base.isLocalPlayer)
			{
				_headPos = _playerRaceModel._headBoneTransform.position;
				_headPos.x = 0f;
				_headPos.z = 0f;
				_headPos.y += 0.45f;
				CameraFunction._current.positionAdjust = _headPos;
			}
			Apply_NetworkedCharacterDisplay();
		}
	}

	public void Apply_NetworkedCharacterDisplay()
	{
		if ((bool)_playerRaceModel)
		{
			_playerRaceModel.SetTextureIndex = _playerAppearanceStruct._textureID;
			_playerRaceModel._displayBoobs = _playerAppearanceStruct._displayBoobs;
			ColorAdjustShader_Profile setSkinColorProfile = new ColorAdjustShader_Profile
			{
				_hue = _playerAppearanceStruct._bodyColorHue,
				_brightness = _playerAppearanceStruct._bodyColorBrightness,
				_contrast = _playerAppearanceStruct._bodyColorContrast,
				_saturation = _playerAppearanceStruct._bodyColorSaturation
			};
			ColorAdjustShader_Profile hairColorProfile = new ColorAdjustShader_Profile
			{
				_hue = _playerAppearanceStruct._hairStyleHue,
				_brightness = _playerAppearanceStruct._hairStyleBrightness,
				_contrast = _playerAppearanceStruct._hairStyleContrast,
				_saturation = _playerAppearanceStruct._hairStyleSaturation
			};
			ColorAdjustShader_Profile miscColorProfile = new ColorAdjustShader_Profile
			{
				_hue = _playerAppearanceStruct._miscHue,
				_brightness = _playerAppearanceStruct._miscBrightness,
				_contrast = _playerAppearanceStruct._miscContrast,
				_saturation = _playerAppearanceStruct._miscSaturation
			};
			PlayerAppearance_Profile aP = new PlayerAppearance_Profile
			{
				_armWeight = _playerAppearanceStruct._armWeight,
				_bellyWeight = _playerAppearanceStruct._bellyWeight,
				_displayBoobs = _playerAppearanceStruct._displayBoobs,
				_boobWeight = _playerAppearanceStruct._boobWeight,
				_bottomWeight = _playerAppearanceStruct._bottomWeight,
				_headWidth = _playerAppearanceStruct._headWidth,
				_heightWeight = _playerAppearanceStruct._heightWeight,
				_muzzleWeight = _playerAppearanceStruct._muzzleWeight,
				_torsoWeight = _playerAppearanceStruct._torsoWeight,
				_widthWeight = _playerAppearanceStruct._widthWeight,
				_hairColorProfile = hairColorProfile,
				_miscColorProfile = miscColorProfile,
				_setSkinColorProfile = setSkinColorProfile
			};
			aP = _playerRaceModel._scriptablePlayerRace.Init_ParamsCheck(aP);
			_playerRaceModel._headBoneTransform.localScale = new Vector3(aP._headWidth, 1f, 1f);
			_playerRaceModel._armatureTransform.localScale = new Vector3(aP._widthWeight, aP._heightWeight, aP._widthWeight);
			_playerRaceModel._bellyWeight = aP._bellyWeight;
			_playerRaceModel._boobWeight = aP._boobWeight;
			_playerRaceModel._bottomWeight = aP._bottomWeight;
			_playerRaceModel._torsoWeight = aP._torsoWeight;
			_playerRaceModel._muzzleWeight = aP._muzzleWeight;
			_playerRaceModel._armWeight = aP._armWeight;
			_playerRaceModel.SetEarIndex = _playerAppearanceStruct._earID;
			_playerRaceModel.SetHairIndex = _playerAppearanceStruct._hairStyleID;
			_playerRaceModel.SetMiscIndex = _playerAppearanceStruct._miscID;
			_playerRaceModel.SetEyeIndex = _playerAppearanceStruct._eyeID;
			_playerRaceModel.SetMouthIndex = _playerAppearanceStruct._mouthID;
			_playerRaceModel.SetTailIndex = _playerAppearanceStruct._tailID;
			_playerRaceModel._setSkinColorProfile = aP._setSkinColorProfile;
			_playerRaceModel._setHairColorProfile = aP._hairColorProfile;
			_playerRaceModel._setMiscColorProfile = aP._miscColorProfile;
			_playerRaceModel._hairHueIsBodyHue = _playerAppearanceStruct._hairIsBodyColor;
			_raceModelED.DyeIndex = _playerAppearanceStruct._dyeIndex;
			_raceModelED._hideHelmVisual = _playerAppearanceStruct._hideHelm;
			_raceModelED._hideCapeVisual = _playerAppearanceStruct._hideCape;
			_raceModelED._hideChestVisual = _playerAppearanceStruct._hideChest;
			_raceModelED._hideLeggingsVisual = _playerAppearanceStruct._hideLeggings;
			_raceModelED._hideVanityHelm = _playerAppearanceStruct._hideVanityHelm;
			_raceModelED._hideVanityCape = _playerAppearanceStruct._hideVanityCape;
			_raceModelED._hideVanityChest = _playerAppearanceStruct._hideVanityChest;
			_raceModelED._hideVanityLeggings = _playerAppearanceStruct._hideVanityLeggings;
			_raceModelED._displayBuffer = 20.5f;
			_playerRaceModel.Apply_CharacterDisplay();
			_raceModelRenderMod.FindRenders();
		}
	}

	private void Update()
	{
		OnUpdate_AnimationCondition();
		OnUpdate_EffectCondition();
		Handle_PeerCullDisplay();
	}

	[ClientCallback]
	private void OnUpdate_EffectCondition()
	{
		if (NetworkClient.active && (bool)Player._mainPlayer)
		{
			if ((bool)_raceModelRenderMod && (Player._mainPlayer._bufferingStatus || _player._bufferingStatus))
			{
				_raceModelRenderMod._scaleBuffer = 2.5f;
			}
			_blockOrbEffect.gameObject.SetActive((bool)Player._mainPlayer && Player._mainPlayer.Network_playerMapInstance == _player.Network_playerMapInstance);
			_hurtCloudEffect.gameObject.SetActive((bool)Player._mainPlayer && Player._mainPlayer.Network_playerMapInstance == _player.Network_playerMapInstance);
			Handle_BlockOrbEffects();
			Handle_HurtShaderTintDisplay();
			Handle_WaterRipplingEffect();
		}
	}

	[ClientCallback]
	private void Handle_BlockOrbEffects()
	{
		if (NetworkClient.active)
		{
			Handle_BlockColor();
			if (_pCombat._inParryWindow && !_parryWindowEffect.isEmitting)
			{
				_parryWindowEffect.Play();
			}
			if (!_pCombat._isBlocking || !_pCombat._inParryWindow)
			{
				_parryWindowEffect.Stop();
			}
		}
		void Handle_BlockColor()
		{
			Color color = ((_pStats._class == null) ? Color.grey : ((_pStats._syncClassTier > 0) ? _pStats._class._playerClassTiers[_pStats._syncClassTier - 1]._blockTierEmissionColor : _pStats._class._blockEmissionColor));
			if (_blockOrbRender.material.GetColor("_EmissionColor") != color)
			{
				_blockOrbRender.material.SetColor("_EmissionColor", color);
			}
		}
	}

	[ClientCallback]
	private void Handle_WaterRipplingEffect()
	{
		if (!NetworkClient.active || !Player._mainPlayer || Player._mainPlayer.Network_playerMapInstance != _player.Network_playerMapInstance)
		{
			return;
		}
		PhysicsScene physicsScene = base.gameObject.scene.GetPhysicsScene();
		if (physicsScene.Raycast(base.transform.position + Vector3.up * 9.5f, Vector3.down, out var hitInfo, _wateRippleDetector, CastLayers.WaterMask) && _pWater._isInWater)
		{
			_waterRippleEffect.transform.position = hitInfo.point;
			if (!_waterRippleEffect.isEmitting)
			{
				_waterRippleEffect.Play();
			}
		}
		else
		{
			_waterRippleEffect.Stop();
		}
		Vector3 position = base.transform.position;
		position.y = base.transform.position.y + 3.5f;
		if (physicsScene.SphereCast(position, 0.3f, Vector3.down, out var hitInfo2, _wateRippleFootDetector, CastLayers.WaterMask) && !_pWater._isInWater)
		{
			_waterRippleEffectFoot.transform.position = hitInfo2.point;
			if (!_waterRippleEffectFoot.isEmitting)
			{
				_waterRippleEffectFoot.Play();
			}
		}
		else if (_waterRippleEffectFoot.isEmitting)
		{
			_waterRippleEffectFoot.Stop();
		}
	}

	[ClientCallback]
	public void Play_FootstepEffect(int _index)
	{
		if (NetworkClient.active)
		{
			_footStepEffects[_index].Play();
		}
	}

	[Command]
	public void Cmd_PlayTeleportEffect()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void PlayerVisual::Cmd_PlayTeleportEffect()", 1550370011, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	public void Rpc_PlayTeleportEffect()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendRPCInternal("System.Void PlayerVisual::Rpc_PlayTeleportEffect()", 2022651152, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	[ClientCallback]
	public void Init_TeleportEffect()
	{
		if (NetworkClient.active && (bool)Player._mainPlayer && !(Player._mainPlayer.Network_playerMapInstance != _player.Network_playerMapInstance) && !Player._mainPlayer._bufferingStatus && !_player._bufferingStatus)
		{
			_teleportEffect.Play();
			_pSound._aSrcGeneral.PlayOneShot(_pSound._teleport, 1f);
			_pSound._aSrcGeneral.PlayOneShot(_pSound._warp, 0.25f);
			_visualContainerLerpScale.originScale = new Vector3(0f, 2.5f, 0f);
			_visualContainerLerpScale.Apply_OriginScale();
		}
	}

	[Command]
	public void Cmd_VanitySparkleEffect()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void PlayerVisual::Cmd_VanitySparkleEffect()", -490587725, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	public void Rpc_VanitySparkleEffect()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendRPCInternal("System.Void PlayerVisual::Rpc_VanitySparkleEffect()", 1265225758, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	[Command]
	public void Cmd_PoofSmokeEffect()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void PlayerVisual::Cmd_PoofSmokeEffect()", 1376653761, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	public void Rpc_PoofSmokeEffect()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendRPCInternal("System.Void PlayerVisual::Rpc_PoofSmokeEffect()", -130050644, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	[Command]
	public void Cmd_JumpAttackEffect()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void PlayerVisual::Cmd_JumpAttackEffect()", -1466830350, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc(includeOwner = false)]
	public void Rpc_JumpAttackEffect()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendRPCInternal("System.Void PlayerVisual::Rpc_JumpAttackEffect()", -930026649, writer, 0, includeOwner: false);
		NetworkWriterPool.Return(writer);
	}

	[ClientCallback]
	public void Init_JumpAttackEffect()
	{
		if (NetworkClient.active && !(Player._mainPlayer.Network_playerMapInstance != _player.Network_playerMapInstance) && !Player._mainPlayer._bufferingStatus && !_player._bufferingStatus && _pSound._aSrcGeneral.enabled)
		{
			_pSound._aSrcGeneral.PlayOneShot(_pSound._airAttackInitSound, 0.45f);
			_spinJumpEffect.Play();
		}
	}

	public void Send_CrossFadeAnim(string _animName, float _animFadeTime, int _animLayer, LatencyCheck _latencyCheck)
	{
		if (base.isClientOnly && base.isLocalPlayer)
		{
			Cmd_CrossFadeAnim(_animName, _animFadeTime, _animLayer);
		}
		if (base.isServer)
		{
			Rpc_CrossFadeAnim(_animName, _animFadeTime, _animLayer);
		}
		if (_latencyCheck == LatencyCheck.CHECK_LATENCY)
		{
			LatencyBuffers[] latencyBuffers = GameManager._current._latencyBuffers;
			float num = 0f;
			for (int i = 0; i < latencyBuffers.Length; i++)
			{
				if (_player._latency >= latencyBuffers[i]._latencyThreshold.x && _player._latency <= latencyBuffers[i]._latencyThreshold.y)
				{
					num = latencyBuffers[i]._bufferValue;
				}
			}
			if (num <= 0f)
			{
				Local_CrossFadeAnim(_animName, _animFadeTime, _animLayer);
			}
			else
			{
				StartCoroutine(Emit_AnimTime(_animName, _animFadeTime, _animLayer, num));
			}
		}
		else
		{
			Local_CrossFadeAnim(_animName, _animFadeTime, _animLayer);
		}
	}

	private IEnumerator Emit_AnimTime(string _animName, float _animFadeTime, int _animLayer, float _time)
	{
		yield return new WaitForSeconds(_time);
		Local_CrossFadeAnim(_animName, _animFadeTime, _animLayer);
	}

	public void Local_CrossFadeAnim(string _animName, float _animFadeTime, int _animLayer)
	{
		if ((bool)_visualAnimator && (_pMove.RayGroundCheck() || _animLayer != 11))
		{
			Iterate_AnimationCallback(_animName, _animLayer);
			_visualAnimator.CrossFade(_animName, _animFadeTime, _animLayer);
		}
	}

	[Command]
	public void Cmd_CrossFadeAnim(string _animName, float _animFadeTime, int _animLayer)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteString(_animName);
		writer.WriteFloat(_animFadeTime);
		writer.WriteInt(_animLayer);
		SendCommandInternal("System.Void PlayerVisual::Cmd_CrossFadeAnim(System.String,System.Single,System.Int32)", -431029900, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc(includeOwner = false)]
	private void Rpc_CrossFadeAnim(string _animName, float _animFadeTime, int _animLayer)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteString(_animName);
		writer.WriteFloat(_animFadeTime);
		writer.WriteInt(_animLayer);
		SendRPCInternal("System.Void PlayerVisual::Rpc_CrossFadeAnim(System.String,System.Single,System.Int32)", 1646869791, writer, 0, includeOwner: false);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	public void IncludeRpc_Crossfade(string _animName, float _animFadeTime, int _animLayer)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteString(_animName);
		writer.WriteFloat(_animFadeTime);
		writer.WriteInt(_animLayer);
		SendRPCInternal("System.Void PlayerVisual::IncludeRpc_Crossfade(System.String,System.Single,System.Int32)", -510476432, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	[Command]
	public void Cmd_ShowItemEmote(string _tag)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteString(_tag);
		SendCommandInternal("System.Void PlayerVisual::Cmd_ShowItemEmote(System.String)", 394860515, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	private void Rpc_ShowItemEmote(string _tag)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteString(_tag);
		SendRPCInternal("System.Void PlayerVisual::Rpc_ShowItemEmote(System.String)", 1349157656, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	private void Iterate_AnimationCallback(string _animName, int _animLayer)
	{
		switch (_animLayer)
		{
		case 0:
			_visualAnimator.SetBool("_inMovement", _animName != "Idle");
			_visualAnimator.SetBool("_inAltMovement", _animName == "Alt_Move");
			if (_animName != "Idle" && _animName != "Idle_02")
			{
				_visualAnimator.CrossFade("Empty", 0f, 11);
			}
			switch (_animName)
			{
			case "LedgeHold":
				_visualAnimator.CrossFade("Empty", 0f, 10);
				_visualAnimator.CrossFade("Empty", 0f, 9);
				_visualContainerLerpScale.originScale = new Vector3(0.9f, 1.5f, 0.9f);
				_visualContainerLerpScale.Apply_OriginScale();
				break;
			case "Jump":
				_visualContainerLerpScale.originScale = new Vector3(0.9f, 2.4f, 0.9f);
				_visualContainerLerpScale.Apply_OriginScale();
				break;
			case "DoubleJump":
				_visualContainerLerpScale.originScale = new Vector3(0.9f, 1.5f, 0.9f);
				_visualContainerLerpScale.Apply_OriginScale();
				if (_pCombat._isBlocking)
				{
					_visualAnimator.CrossFade("Jump", 0f, 0);
				}
				break;
			case "TripleJump":
				_visualContainerLerpScale.originScale = new Vector3(0.9f, 1.5f, 0.9f);
				_visualContainerLerpScale.Apply_OriginScale();
				if (_pCombat._isBlocking)
				{
					_visualAnimator.CrossFade("Jump", 0f, 0);
				}
				break;
			case "SwimJump":
				_visualContainerLerpScale.originScale = new Vector3(0.9f, 1.5f, 0.9f);
				_visualContainerLerpScale.Apply_OriginScale();
				break;
			}
			break;
		case 1:
			if (_animName != "Empty")
			{
				_visualAnimator.CrossFade("Empty", 0f, 11);
			}
			if (_animName == "hurt_big")
			{
				if (_pSound._aSrcGeneral.isActiveAndEnabled)
				{
					_pSound._aSrcGeneral.PlayOneShot(_pSound._knockbackSlapSfx, 0.25f);
				}
				_visualAnimator.CrossFade("Empty", 0f, 10);
				_player._knockbackBuffer = 0.4f;
				if ((bool)Player._mainPlayer && Player._mainPlayer.Network_playerMapInstance == _player.Network_playerMapInstance)
				{
					GameObject obj = PoolManager._current.GenerateObject(null, "knockbackCloudEffect", 1.5f);
					obj.transform.SetParent(_player.transform);
					obj.transform.localPosition = Vector3.zero;
				}
			}
			break;
		case 2:
			if (_animName != "Hold_None")
			{
				if (base.isServer)
				{
					Rpc_AltMovementBool(_isTrue: false);
				}
				_visualAnimator.CrossFade("Empty", 0f, 11);
				_visualAnimator.SetTrigger("_resetIdle2Anim");
			}
			break;
		case 3:
			if (base.isServer)
			{
				Rpc_AltMovementBool(_isTrue: false);
				if (_animName == "Block_NoShield")
				{
					_pCombat.Server_DisableHitboxObjects();
				}
				else if (_animName == "Block_Shield")
				{
					_pCombat.Server_DisableHitboxObjects();
				}
				else if (_animName != "Empty" && _animName != "JumpAtk_HitWall" && _animName != "bell_jumpAtk")
				{
					_player._currentPlayerAction = PlayerAction.ATTACKING;
				}
			}
			_visualAnimator.SetTrigger("_resetIdle2Anim");
			if (_animName != "Empty")
			{
				_visualAnimator.CrossFade("Empty", 0f, 11);
				_visualAnimator.CrossFade("Empty", 0f, 9);
			}
			else
			{
				switch (_animName)
				{
				case "Block_NoShield_hit":
					Local_ResetSpinPlayerModel();
					_player._currentPlayerAction = PlayerAction.IDLE;
					break;
				case "Block_Shield_hit":
					Local_ResetSpinPlayerModel();
					_player._currentPlayerAction = PlayerAction.IDLE;
					break;
				case "Block_Shattered":
					Local_ResetSpinPlayerModel();
					_player._currentPlayerAction = PlayerAction.IDLE;
					break;
				}
			}
			if (_animName == "JumpAtk_HitWall" && (bool)Player._mainPlayer && Player._mainPlayer.Network_playerMapInstance == _player.Network_playerMapInstance)
			{
				PoolManager._current.GenerateObject(null, "wallHit", 0.8f).transform.position = base.transform.position;
			}
			if (_animName == (_pCombat._setWepTypeCombatParams._weaponTypeJumpAttackClipHandle ?? "") && _visualAnimator.GetCurrentAnimatorStateInfo(10).IsName("Dash"))
			{
				_visualAnimator.CrossFade("Empty", 0f, 10);
			}
			if (!base.isLocalPlayer && _player._currentPlayerAction == PlayerAction.CASTING)
			{
				_player._currentPlayerAction = PlayerAction.IDLE;
			}
			_raceModelED._hideTakenOutWeaponBuffer = 0f;
			break;
		case 7:
			if (_animName != "Empty")
			{
				_visualAnimator.CrossFade("Empty", 0f, 11);
			}
			_visualAnimator.SetTrigger("_resetIdle2Anim");
			_visualAnimator.CrossFade("Empty", 0f, 10);
			break;
		case 8:
			if (_animName != "Empty")
			{
				_visualAnimator.CrossFade("Idle", 0f, 0);
				_visualAnimator.CrossFade("empty", 0f, 1);
				_visualAnimator.CrossFade("Empty", 0f, 3);
				_visualAnimator.CrossFade("Empty", 0f, 7);
				_visualAnimator.CrossFade("Empty", 0f, 9);
				_visualAnimator.CrossFade("Empty", 0f, 10);
				_visualAnimator.CrossFade("Empty", 0f, 11);
				_visualAnimator.CrossFade("Empty", 0f, 12);
				_raceModelAnimManager._weaponChargeEffect.Stop();
				_deathSoulEffect.Play();
			}
			else
			{
				_deathSoulEffect.Stop();
			}
			break;
		case 9:
			if (base.isServer)
			{
				Rpc_AltMovementBool(_isTrue: false);
			}
			_visualAnimator.SetTrigger("_resetIdle2Anim");
			if (_animName != "Empty")
			{
				if (!_pCombat._isBlocking)
				{
					_visualAnimator.CrossFade("Empty", 0f, 3);
				}
				_visualAnimator.CrossFade("Empty", 0f, 7);
				_visualAnimator.CrossFade("Empty", 0f, 10);
				_visualAnimator.CrossFade("Empty", 0f, 11);
				if (!base.isLocalPlayer)
				{
					_player._currentPlayerAction = PlayerAction.CASTING;
				}
			}
			else
			{
				if (!base.isLocalPlayer)
				{
					_player._currentPlayerAction = PlayerAction.IDLE;
				}
				_raceModelED._hideTakenOutWeaponBuffer = 0f;
			}
			break;
		case 10:
			if (_animName == "Dash")
			{
				if (!base.isLocalPlayer && _player._currentPlayerAction != PlayerAction.CASTING)
				{
					_player._currentPlayerAction = PlayerAction.IDLE;
				}
				if (!_visualAnimator.GetBool("_isBlocking") && !_pCombat._isChargingWeapon && _player._currentPlayerAction != PlayerAction.CASTING)
				{
					_visualAnimator.CrossFade("Empty", 0f, 3);
					_pCombat.CombatInterrupt();
				}
				if ((bool)Player._mainPlayer && Player._mainPlayer.Network_playerMapInstance == _player.Network_playerMapInstance)
				{
					_dashEffect.Play();
					_pSound._aSrcDash.Play();
				}
				_visualAnimator.CrossFade("empty", 0f, 1);
				_visualAnimator.CrossFade("Empty", 0f, 7);
				_visualAnimator.CrossFade("Empty", 0f, 11);
				Local_ResetSpinPlayerModel();
			}
			break;
		case 11:
			if (!(_animName != "Empty"))
			{
				break;
			}
			_visualAnimator.CrossFade("Idle", 0f, 0);
			_visualAnimator.CrossFade("Empty", 0f, 3);
			_visualAnimator.CrossFade("Empty", 0f, 9);
			_visualAnimator.CrossFade("Empty", 0f, 10);
			if (base.isServer)
			{
				if ((bool)_pCasting._currentCastSkill)
				{
					_pCasting.Server_InterruptCast();
				}
				_pCombat.Network_currentSheathCondition = WeaponSheatheCondition.PUT_AWAY;
				_pCombat.Server_InterruptCombat(_resetAnimation: false);
			}
			break;
		case 12:
			_visualAnimator.SetBool("_isClimbingRope", _animName == "Climb");
			if (_animName != "Empty")
			{
				_visualAnimator.CrossFade("Idle", 0f, 0);
				_visualAnimator.CrossFade("Empty", 0f, 3);
				_visualAnimator.CrossFade("Empty", 0f, 9);
				_visualAnimator.CrossFade("Empty", 0f, 10);
			}
			break;
		case 4:
		case 5:
		case 6:
			break;
		}
	}

	[Command]
	public void Cmd_ChangeClimbAnimationSpeed(float _value)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteFloat(_value);
		SendCommandInternal("System.Void PlayerVisual::Cmd_ChangeClimbAnimationSpeed(System.Single)", -1228218420, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	private void Rpc_SetPlaybackSpeed(float _value)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteFloat(_value);
		SendRPCInternal("System.Void PlayerVisual::Rpc_SetPlaybackSpeed(System.Single)", 436510433, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	[ClientCallback]
	private void OnUpdate_AnimationCondition()
	{
		if (!NetworkClient.active || !_playerRaceModel || !_visualAnimator)
		{
			return;
		}
		if (_player._bufferingStatus)
		{
			_visualAnimator.CrossFade("Empty", 0f, 3);
		}
		if (!_preventRotationLerp)
		{
			_raceModelRotater.gameObject.transform.localRotation = Quaternion.Lerp(_raceModelRotater.gameObject.transform.localRotation, Quaternion.identity, 35.5f * Time.deltaTime);
		}
		Handle_AnimatorParams();
		if (_playerAppearanceStruct._isLeftHanded)
		{
			Vector3 vector = new Vector3(-1f, 1f, 1f);
			if (_raceModelContainer.localScale != vector)
			{
				_raceModelContainer.localScale = vector;
			}
		}
		else if (_raceModelContainer.localScale != Vector3.one)
		{
			_raceModelContainer.localScale = Vector3.one;
		}
		if (_player._currentPlayerCondition == PlayerCondition.DEAD)
		{
			if ((bool)Player._mainPlayer && Player._mainPlayer._bufferingStatus)
			{
				_visualAnimator.CrossFade("Death", 0f, 8);
			}
			_raceModelRotater.Alter_RotationValues(Vector3.zero);
			_raceModelRotater.gameObject.transform.localRotation = Quaternion.identity;
			_raceModelRotater.Alter_RotationValues(Vector3.zero);
			_preventRotationLerp = false;
		}
		else
		{
			Local_CrossFadeAnim("Empty", 0f, 8);
		}
		void Handle_AnimatorParams()
		{
			float num = (_pMove._movSpeed + _pMove._currentRevUpSpeed) / GameManager._current._statLogics._baseMoveSpeed;
			Network_movementAnimSpeedMod = num * _animSpeedModify;
			bool canAltIdleAnim = _visualAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle") && _visualAnimator.GetCurrentAnimatorStateInfo(3).IsName("Empty") && _visualAnimator.GetCurrentAnimatorStateInfo(9).IsName("Empty");
			_playerRaceModel._canAltIdleAnim = canAltIdleAnim;
			if (!_visualAnimator.GetCurrentAnimatorStateInfo(8).IsName("Empty"))
			{
				_visualAnimator.CrossFade("empty", 0f, 1);
				_visualAnimator.CrossFade("Empty", 0f, 3);
				_visualAnimator.CrossFade("Empty", 0f, 7);
				_visualAnimator.CrossFade("Empty", 0f, 9);
				_visualAnimator.CrossFade("Empty", 0f, 10);
				_visualAnimator.CrossFade("Empty", 0f, 11);
				_visualAnimator.CrossFade("Empty", 0f, 12);
			}
			_visualAnimator.SetBool("_isGrounded", _pMove.RayGroundCheck());
			_visualAnimator.SetBool("_isBlocking", _pCombat._isBlocking);
			_visualAnimator.SetBool("_isDead", _player._currentPlayerCondition == PlayerCondition.DEAD);
			_visualAnimator.SetFloat("_movAnimSpd", _playerRaceModel._baseMovementMultiplier * _movementAnimSpeedMod);
			if (base.isLocalPlayer)
			{
				Handle_LocalAnimatorParams();
			}
			else if (_pCombat._currentSheathCondition == WeaponSheatheCondition.PUT_AWAY)
			{
				_visualAnimator.CrossFade("Hold_None", 0f, 2);
			}
		}
		void Handle_LocalAnimatorParams()
		{
			if (_playerRaceModel._hasAltMovementAnim)
			{
				if (_pMove._currentRevUpSpeed >= GameManager._current._statLogics._maxRevUpMoveSpeed && _pCombat._currentSheathCondition == WeaponSheatheCondition.PUT_AWAY)
				{
					if (!_visualAnimator.GetBool("_inAltMovement") && _visualAnimator.GetBool("_inMovement"))
					{
						Cmd_AltMovementAnimBool(_isTrue: true);
					}
				}
				else if (_visualAnimator.GetBool("_inAltMovement"))
				{
					Cmd_AltMovementAnimBool(_isTrue: false);
				}
			}
		}
	}

	[Command]
	private void Cmd_AltMovementAnimBool(bool _isTrue)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteBool(_isTrue);
		SendCommandInternal("System.Void PlayerVisual::Cmd_AltMovementAnimBool(System.Boolean)", -1221984063, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	private void Rpc_AltMovementBool(bool _isTrue)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteBool(_isTrue);
		SendRPCInternal("System.Void PlayerVisual::Rpc_AltMovementBool(System.Boolean)", -906202949, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	private void Handle_PeerCullDisplay()
	{
		if (!Player._mainPlayer || _player._currentGameCondition != GameCondition.IN_GAME)
		{
			return;
		}
		if ((!base.isLocalPlayer && _player.Network_playerMapInstance != Player._mainPlayer.Network_playerMapInstance) || _forceHidden)
		{
			if (!_isCulledDisplay && !_forceHidden)
			{
				_teleportEffect.Play();
			}
			Emit_RenderDisplay(_isTrue: false);
			return;
		}
		if (!Player._mainPlayer && _raceModelRotater.gameObject.layer != CastLayers.DefaultLayer)
		{
			Emit_RenderDisplay(_isTrue: true);
			_teleportEffect.Play();
			return;
		}
		if (base.gameObject.scene != Player._mainPlayer.gameObject.scene && !_isCulledDisplay)
		{
			_teleportEffect.Play();
			_pSound._aSrcGeneral.PlayOneShot(_pSound._teleport, 1.05f);
			Emit_RenderDisplay(_isTrue: false);
		}
		if (base.gameObject.scene == Player._mainPlayer.gameObject.scene && _isCulledDisplay && (!base.isLocalPlayer || !CameraCollision._current._unhidePlayerModel))
		{
			_teleportEffect.Play();
			if (_pSound._aSrcGeneral.enabled)
			{
				_pSound._aSrcGeneral.PlayOneShot(_pSound._teleport, 1.05f);
			}
			Emit_RenderDisplay(_isTrue: true);
		}
	}

	[Command]
	public void Cmd_ToggleArmorRender(int _value, EquipCellTab _equipType)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteInt(_value);
		GeneratedNetworkCode._Write_EquipCellTab(writer, _equipType);
		SendCommandInternal("System.Void PlayerVisual::Cmd_ToggleArmorRender(System.Int32,EquipCellTab)", 1290836410, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[Command]
	public void Cmd_HideWeapon(float _time)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteFloat(_time);
		SendCommandInternal("System.Void PlayerVisual::Cmd_HideWeapon(System.Single)", -2086708254, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	private void Rpc_HideWeapon(float _time)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteFloat(_time);
		SendRPCInternal("System.Void PlayerVisual::Rpc_HideWeapon(System.Single)", -2005941043, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	public void RenderPlayerModel(bool _isTrue)
	{
		if ((bool)_playerRaceModel)
		{
			Emit_RenderDisplay(_isTrue);
		}
	}

	private void Emit_RenderDisplay(bool _isTrue)
	{
		if (!_playerRaceModel || !_raceModelRenderMod || (!_isTrue && _isCulled) || (_isTrue && !_isCulled))
		{
			return;
		}
		LayerMask layerMask = CastLayers.DefaultLayer;
		LayerMask layerMask2 = CastLayers.CullLayer;
		LayerMask layerMask3 = 0;
		if (_isTrue)
		{
			layerMask3 = layerMask;
			_isCulledDisplay = false;
			_raceModelRenderMod.Reset_Color();
			_raceModelRenderMod.Init_Time_RefreshShader(2.5f);
		}
		else
		{
			layerMask3 = layerMask2;
			_isCulledDisplay = true;
		}
		if (base.isLocalPlayer && _forceHidden && !_player._bufferingStatus)
		{
			_raceModelRenderMod._defaultColor = _stealthColor;
			_raceModelRenderMod._RefreshShader = true;
		}
		else
		{
			for (int i = 0; i < _rendObjects.Length; i++)
			{
				_rendObjects[i].gameObject.layer = layerMask3;
			}
			_shadowProjectionObject.layer = layerMask3;
		}
		_isCulled = !_isTrue;
	}

	[ClientCallback]
	private void Handle_HurtShaderTintDisplay()
	{
		if (!NetworkClient.active || !Player._mainPlayer || _player._bufferingStatus || Player._mainPlayer.Network_playerMapInstance != _player.Network_playerMapInstance)
		{
			return;
		}
		if (_player._statusEntity._generalHitBuffer > 0f && (bool)_raceModelRenderMod)
		{
			_raceModelRenderMod.Tint_Shader(Color.red);
		}
		if (_player._statusEntity._currentHealth <= (int)((float)_player._pStats._statStruct._maxHealth * 0.3f) && _player._currentPlayerCondition == PlayerCondition.ACTIVE)
		{
			if (!IsInvoking("Flash_PlayerLowHealth"))
			{
				InvokeRepeating("Flash_PlayerLowHealth", 0f, 2.5f);
				_pSound._aSrcGeneral.PlayOneShot(_pSound._lowHealthWarning, 0.75f);
			}
		}
		else
		{
			CancelInvoke("Flash_PlayerLowHealth");
		}
	}

	[ClientCallback]
	private void Flash_PlayerLowHealth()
	{
		if (NetworkClient.active && !_player._bufferingStatus && (bool)_raceModelRenderMod)
		{
			_raceModelRenderMod.Tint_Shader(Color.red);
			_playerRaceModel.Set_EyeCondition(EyeCondition.Hurt, 1.25f);
			if (_pSound._aSrcGeneral.enabled)
			{
				_pSound._aSrcGeneral.PlayOneShot(_pSound._lowHealthPulseSfx, 0.75f);
			}
		}
	}

	[ClientCallback]
	public void Flash_PlayerRecieveHealth()
	{
		if (NetworkClient.active && !_player._bufferingStatus && (bool)_raceModelRenderMod)
		{
			_raceModelRenderMod.Tint_Shader(Color.green);
		}
	}

	[Command]
	public void Cmd_ResetSpinPlayerModel()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void PlayerVisual::Cmd_ResetSpinPlayerModel()", -858721918, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	public void Rpc_RandomSpinPlayerModel(float _value)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteFloat(_value);
		SendRPCInternal("System.Void PlayerVisual::Rpc_RandomSpinPlayerModel(System.Single)", -530900698, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	public void Rpc_ResetSpinPlayerModel()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendRPCInternal("System.Void PlayerVisual::Rpc_ResetSpinPlayerModel()", 2031888503, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	public void Local_ResetSpinPlayerModel()
	{
		_raceModelRotater.Alter_RotationValues(Vector3.zero);
		_raceModelRotater.gameObject.transform.localRotation = Quaternion.identity;
		_raceModelRotater.Alter_RotationValues(Vector3.zero);
		_preventRotationLerp = false;
	}

	[ClientRpc]
	public void Rpc_PreventRotationLerp(float _time)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteFloat(_time);
		SendRPCInternal("System.Void PlayerVisual::Rpc_PreventRotationLerp(System.Single)", -1067476100, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	public void Prevent_RotationLerp(float _time)
	{
		StartCoroutine(PreventRotationLerp_Reset(_time));
	}

	public void HardReset_RotationLerpInit(float _time)
	{
		StartCoroutine(HardReset_RotationLerp(_time));
	}

	private IEnumerator HardReset_RotationLerp(float _time)
	{
		yield return new WaitForSeconds(_time);
		_preventRotationLerp = false;
		Local_ResetSpinPlayerModel();
	}

	private IEnumerator PreventRotationLerp_Reset(float _time)
	{
		yield return new WaitForSeconds(_time);
		_preventRotationLerp = false;
	}

	static PlayerVisual()
	{
		_HUE = Shader.PropertyToID("_Hue");
		_SATURATION = Shader.PropertyToID("_Saturation");
		_BRIGHTNESS = Shader.PropertyToID("_Brightness");
		_CONTRAST = Shader.PropertyToID("_Contrast");
		_TINT = Shader.PropertyToID("_ColorTint");
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerVisual), "System.Void PlayerVisual::Cmd_SendNew_PlayerAppearanceStruct(PlayerAppearanceStruct)", InvokeUserCode_Cmd_SendNew_PlayerAppearanceStruct__PlayerAppearanceStruct, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerVisual), "System.Void PlayerVisual::Cmd_PlayTeleportEffect()", InvokeUserCode_Cmd_PlayTeleportEffect, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerVisual), "System.Void PlayerVisual::Cmd_VanitySparkleEffect()", InvokeUserCode_Cmd_VanitySparkleEffect, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerVisual), "System.Void PlayerVisual::Cmd_PoofSmokeEffect()", InvokeUserCode_Cmd_PoofSmokeEffect, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerVisual), "System.Void PlayerVisual::Cmd_JumpAttackEffect()", InvokeUserCode_Cmd_JumpAttackEffect, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerVisual), "System.Void PlayerVisual::Cmd_CrossFadeAnim(System.String,System.Single,System.Int32)", InvokeUserCode_Cmd_CrossFadeAnim__String__Single__Int32, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerVisual), "System.Void PlayerVisual::Cmd_ShowItemEmote(System.String)", InvokeUserCode_Cmd_ShowItemEmote__String, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerVisual), "System.Void PlayerVisual::Cmd_ChangeClimbAnimationSpeed(System.Single)", InvokeUserCode_Cmd_ChangeClimbAnimationSpeed__Single, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerVisual), "System.Void PlayerVisual::Cmd_AltMovementAnimBool(System.Boolean)", InvokeUserCode_Cmd_AltMovementAnimBool__Boolean, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerVisual), "System.Void PlayerVisual::Cmd_ToggleArmorRender(System.Int32,EquipCellTab)", InvokeUserCode_Cmd_ToggleArmorRender__Int32__EquipCellTab, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerVisual), "System.Void PlayerVisual::Cmd_HideWeapon(System.Single)", InvokeUserCode_Cmd_HideWeapon__Single, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerVisual), "System.Void PlayerVisual::Cmd_ResetSpinPlayerModel()", InvokeUserCode_Cmd_ResetSpinPlayerModel, requiresAuthority: true);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerVisual), "System.Void PlayerVisual::Rpc_PlayTeleportEffect()", InvokeUserCode_Rpc_PlayTeleportEffect);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerVisual), "System.Void PlayerVisual::Rpc_VanitySparkleEffect()", InvokeUserCode_Rpc_VanitySparkleEffect);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerVisual), "System.Void PlayerVisual::Rpc_PoofSmokeEffect()", InvokeUserCode_Rpc_PoofSmokeEffect);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerVisual), "System.Void PlayerVisual::Rpc_JumpAttackEffect()", InvokeUserCode_Rpc_JumpAttackEffect);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerVisual), "System.Void PlayerVisual::Rpc_CrossFadeAnim(System.String,System.Single,System.Int32)", InvokeUserCode_Rpc_CrossFadeAnim__String__Single__Int32);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerVisual), "System.Void PlayerVisual::IncludeRpc_Crossfade(System.String,System.Single,System.Int32)", InvokeUserCode_IncludeRpc_Crossfade__String__Single__Int32);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerVisual), "System.Void PlayerVisual::Rpc_ShowItemEmote(System.String)", InvokeUserCode_Rpc_ShowItemEmote__String);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerVisual), "System.Void PlayerVisual::Rpc_SetPlaybackSpeed(System.Single)", InvokeUserCode_Rpc_SetPlaybackSpeed__Single);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerVisual), "System.Void PlayerVisual::Rpc_AltMovementBool(System.Boolean)", InvokeUserCode_Rpc_AltMovementBool__Boolean);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerVisual), "System.Void PlayerVisual::Rpc_HideWeapon(System.Single)", InvokeUserCode_Rpc_HideWeapon__Single);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerVisual), "System.Void PlayerVisual::Rpc_RandomSpinPlayerModel(System.Single)", InvokeUserCode_Rpc_RandomSpinPlayerModel__Single);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerVisual), "System.Void PlayerVisual::Rpc_ResetSpinPlayerModel()", InvokeUserCode_Rpc_ResetSpinPlayerModel);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerVisual), "System.Void PlayerVisual::Rpc_PreventRotationLerp(System.Single)", InvokeUserCode_Rpc_PreventRotationLerp__Single);
	}

	public override bool Weaved()
	{
		return true;
	}

	protected void UserCode_Cmd_SendNew_PlayerAppearanceStruct__PlayerAppearanceStruct(PlayerAppearanceStruct _newStruct)
	{
		_pSound.Network_voicePitch = _newStruct._voicePitch;
		Network_playerAppearanceStruct = _newStruct;
		Rpc_VanitySparkleEffect();
	}

	protected static void InvokeUserCode_Cmd_SendNew_PlayerAppearanceStruct__PlayerAppearanceStruct(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command Cmd_SendNew_PlayerAppearanceStruct called on client.");
		}
		else
		{
			((PlayerVisual)obj).UserCode_Cmd_SendNew_PlayerAppearanceStruct__PlayerAppearanceStruct(GeneratedNetworkCode._Read_PlayerAppearanceStruct(reader));
		}
	}

	protected void UserCode_Cmd_PlayTeleportEffect()
	{
		_player.Add_PacketCount();
		Rpc_PlayTeleportEffect();
	}

	protected static void InvokeUserCode_Cmd_PlayTeleportEffect(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command Cmd_PlayTeleportEffect called on client.");
		}
		else
		{
			((PlayerVisual)obj).UserCode_Cmd_PlayTeleportEffect();
		}
	}

	protected void UserCode_Rpc_PlayTeleportEffect()
	{
		Init_TeleportEffect();
	}

	protected static void InvokeUserCode_Rpc_PlayTeleportEffect(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC Rpc_PlayTeleportEffect called on server.");
		}
		else
		{
			((PlayerVisual)obj).UserCode_Rpc_PlayTeleportEffect();
		}
	}

	protected void UserCode_Cmd_VanitySparkleEffect()
	{
		_player.Add_PacketCount();
		Rpc_VanitySparkleEffect();
	}

	protected static void InvokeUserCode_Cmd_VanitySparkleEffect(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command Cmd_VanitySparkleEffect called on client.");
		}
		else
		{
			((PlayerVisual)obj).UserCode_Cmd_VanitySparkleEffect();
		}
	}

	protected void UserCode_Rpc_VanitySparkleEffect()
	{
		if ((bool)Player._mainPlayer && !(Player._mainPlayer.Network_playerMapInstance != _player.Network_playerMapInstance) && !Player._mainPlayer._bufferingStatus && !_player._bufferingStatus)
		{
			_vanity_sparkleEffect.Play();
			_vanity_sparkleEffect.GetComponent<AudioSource>().Play();
		}
	}

	protected static void InvokeUserCode_Rpc_VanitySparkleEffect(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC Rpc_VanitySparkleEffect called on server.");
		}
		else
		{
			((PlayerVisual)obj).UserCode_Rpc_VanitySparkleEffect();
		}
	}

	protected void UserCode_Cmd_PoofSmokeEffect()
	{
		_player.Add_PacketCount();
		Rpc_PoofSmokeEffect();
	}

	protected static void InvokeUserCode_Cmd_PoofSmokeEffect(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command Cmd_PoofSmokeEffect called on client.");
		}
		else
		{
			((PlayerVisual)obj).UserCode_Cmd_PoofSmokeEffect();
		}
	}

	protected void UserCode_Rpc_PoofSmokeEffect()
	{
		if ((bool)Player._mainPlayer && !(Player._mainPlayer.Network_playerMapInstance != _player.Network_playerMapInstance) && !Player._mainPlayer._bufferingStatus && !_player._bufferingStatus)
		{
			_poofEffect.Play();
			_poofEffect.GetComponent<AudioSource>().Play();
		}
	}

	protected static void InvokeUserCode_Rpc_PoofSmokeEffect(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC Rpc_PoofSmokeEffect called on server.");
		}
		else
		{
			((PlayerVisual)obj).UserCode_Rpc_PoofSmokeEffect();
		}
	}

	protected void UserCode_Cmd_JumpAttackEffect()
	{
		_player.Add_PacketCount();
		Rpc_JumpAttackEffect();
	}

	protected static void InvokeUserCode_Cmd_JumpAttackEffect(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command Cmd_JumpAttackEffect called on client.");
		}
		else
		{
			((PlayerVisual)obj).UserCode_Cmd_JumpAttackEffect();
		}
	}

	protected void UserCode_Rpc_JumpAttackEffect()
	{
		Init_JumpAttackEffect();
	}

	protected static void InvokeUserCode_Rpc_JumpAttackEffect(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC Rpc_JumpAttackEffect called on server.");
		}
		else
		{
			((PlayerVisual)obj).UserCode_Rpc_JumpAttackEffect();
		}
	}

	protected void UserCode_Cmd_CrossFadeAnim__String__Single__Int32(string _animName, float _animFadeTime, int _animLayer)
	{
		if ((bool)_visualAnimator && (_pMove.RayGroundCheck() || _animLayer != 11))
		{
			if (!_player._isHostPlayer)
			{
				Local_CrossFadeAnim(_animName, _animFadeTime, _animLayer);
			}
			Rpc_CrossFadeAnim(_animName, _animFadeTime, _animLayer);
		}
	}

	protected static void InvokeUserCode_Cmd_CrossFadeAnim__String__Single__Int32(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command Cmd_CrossFadeAnim called on client.");
		}
		else
		{
			((PlayerVisual)obj).UserCode_Cmd_CrossFadeAnim__String__Single__Int32(reader.ReadString(), reader.ReadFloat(), reader.ReadInt());
		}
	}

	protected void UserCode_Rpc_CrossFadeAnim__String__Single__Int32(string _animName, float _animFadeTime, int _animLayer)
	{
		if ((_pMove.RayGroundCheck() || _animLayer != 11) && (bool)_visualAnimator)
		{
			Iterate_AnimationCallback(_animName, _animLayer);
			Local_CrossFadeAnim(_animName, _animFadeTime, _animLayer);
		}
	}

	protected static void InvokeUserCode_Rpc_CrossFadeAnim__String__Single__Int32(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC Rpc_CrossFadeAnim called on server.");
		}
		else
		{
			((PlayerVisual)obj).UserCode_Rpc_CrossFadeAnim__String__Single__Int32(reader.ReadString(), reader.ReadFloat(), reader.ReadInt());
		}
	}

	protected void UserCode_IncludeRpc_Crossfade__String__Single__Int32(string _animName, float _animFadeTime, int _animLayer)
	{
		if ((_pMove.RayGroundCheck() || _animLayer != 11) && (bool)_visualAnimator)
		{
			Iterate_AnimationCallback(_animName, _animLayer);
			Local_CrossFadeAnim(_animName, _animFadeTime, _animLayer);
		}
	}

	protected static void InvokeUserCode_IncludeRpc_Crossfade__String__Single__Int32(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC IncludeRpc_Crossfade called on server.");
		}
		else
		{
			((PlayerVisual)obj).UserCode_IncludeRpc_Crossfade__String__Single__Int32(reader.ReadString(), reader.ReadFloat(), reader.ReadInt());
		}
	}

	protected void UserCode_Cmd_ShowItemEmote__String(string _tag)
	{
		Rpc_ShowItemEmote(_tag);
	}

	protected static void InvokeUserCode_Cmd_ShowItemEmote__String(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command Cmd_ShowItemEmote called on client.");
		}
		else
		{
			((PlayerVisual)obj).UserCode_Cmd_ShowItemEmote__String(reader.ReadString());
		}
	}

	protected void UserCode_Rpc_ShowItemEmote__String(string _tag)
	{
		if ((bool)_playerRaceModel && (bool)_raceModelAnimManager)
		{
			ScriptableItem scriptableItem = GameManager._current.Locate_Item(_tag);
			if ((bool)scriptableItem)
			{
				_raceModelAnimManager._showItemRender.sprite = scriptableItem._itemIcon;
				Local_CrossFadeAnim("showItem", 0f, 11);
			}
		}
	}

	protected static void InvokeUserCode_Rpc_ShowItemEmote__String(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC Rpc_ShowItemEmote called on server.");
		}
		else
		{
			((PlayerVisual)obj).UserCode_Rpc_ShowItemEmote__String(reader.ReadString());
		}
	}

	protected void UserCode_Cmd_ChangeClimbAnimationSpeed__Single(float _value)
	{
		if ((bool)_visualAnimator)
		{
			_visualAnimator.SetFloat("_climbAnimSpd", _value);
			Rpc_SetPlaybackSpeed(_value);
		}
	}

	protected static void InvokeUserCode_Cmd_ChangeClimbAnimationSpeed__Single(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command Cmd_ChangeClimbAnimationSpeed called on client.");
		}
		else
		{
			((PlayerVisual)obj).UserCode_Cmd_ChangeClimbAnimationSpeed__Single(reader.ReadFloat());
		}
	}

	protected void UserCode_Rpc_SetPlaybackSpeed__Single(float _value)
	{
		if ((bool)_visualAnimator)
		{
			_visualAnimator.SetFloat("_climbAnimSpd", _value);
		}
	}

	protected static void InvokeUserCode_Rpc_SetPlaybackSpeed__Single(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC Rpc_SetPlaybackSpeed called on server.");
		}
		else
		{
			((PlayerVisual)obj).UserCode_Rpc_SetPlaybackSpeed__Single(reader.ReadFloat());
		}
	}

	protected void UserCode_Cmd_AltMovementAnimBool__Boolean(bool _isTrue)
	{
		if ((bool)_playerRaceModel && _playerRaceModel._hasAltMovementAnim)
		{
			_visualAnimator.SetBool("_inAltMovement", _isTrue);
			Rpc_AltMovementBool(_isTrue);
		}
	}

	protected static void InvokeUserCode_Cmd_AltMovementAnimBool__Boolean(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command Cmd_AltMovementAnimBool called on client.");
		}
		else
		{
			((PlayerVisual)obj).UserCode_Cmd_AltMovementAnimBool__Boolean(reader.ReadBool());
		}
	}

	protected void UserCode_Rpc_AltMovementBool__Boolean(bool _isTrue)
	{
		if ((bool)_playerRaceModel && _playerRaceModel._hasAltMovementAnim)
		{
			_visualAnimator.SetBool("_inAltMovement", _isTrue);
		}
	}

	protected static void InvokeUserCode_Rpc_AltMovementBool__Boolean(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC Rpc_AltMovementBool called on server.");
		}
		else
		{
			((PlayerVisual)obj).UserCode_Rpc_AltMovementBool__Boolean(reader.ReadBool());
		}
	}

	protected void UserCode_Cmd_ToggleArmorRender__Int32__EquipCellTab(int _value, EquipCellTab _equipType)
	{
		PlayerAppearanceStruct playerAppearanceStruct = _playerAppearanceStruct;
		switch ((HideRenderType)(byte)_value)
		{
		case HideRenderType.HELM:
			if (_equipType == EquipCellTab.EQUIPMENT_PANEL)
			{
				playerAppearanceStruct._hideHelm = !playerAppearanceStruct._hideHelm;
			}
			else
			{
				playerAppearanceStruct._hideVanityHelm = !playerAppearanceStruct._hideVanityHelm;
			}
			break;
		case HideRenderType.CAPE:
			if (_equipType == EquipCellTab.EQUIPMENT_PANEL)
			{
				playerAppearanceStruct._hideCape = !playerAppearanceStruct._hideCape;
			}
			else
			{
				playerAppearanceStruct._hideVanityCape = !playerAppearanceStruct._hideVanityCape;
			}
			break;
		case HideRenderType.CHEST:
			if (_equipType == EquipCellTab.EQUIPMENT_PANEL)
			{
				playerAppearanceStruct._hideChest = !playerAppearanceStruct._hideChest;
			}
			else
			{
				playerAppearanceStruct._hideVanityChest = !playerAppearanceStruct._hideVanityChest;
			}
			break;
		case HideRenderType.LEGGINGS:
			if (_equipType == EquipCellTab.EQUIPMENT_PANEL)
			{
				playerAppearanceStruct._hideLeggings = !playerAppearanceStruct._hideLeggings;
			}
			else
			{
				playerAppearanceStruct._hideVanityLeggings = !playerAppearanceStruct._hideVanityLeggings;
			}
			break;
		}
		Network_playerAppearanceStruct = playerAppearanceStruct;
	}

	protected static void InvokeUserCode_Cmd_ToggleArmorRender__Int32__EquipCellTab(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command Cmd_ToggleArmorRender called on client.");
		}
		else
		{
			((PlayerVisual)obj).UserCode_Cmd_ToggleArmorRender__Int32__EquipCellTab(reader.ReadInt(), GeneratedNetworkCode._Read_EquipCellTab(reader));
		}
	}

	protected void UserCode_Cmd_HideWeapon__Single(float _time)
	{
		Rpc_HideWeapon(_time);
	}

	protected static void InvokeUserCode_Cmd_HideWeapon__Single(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command Cmd_HideWeapon called on client.");
		}
		else
		{
			((PlayerVisual)obj).UserCode_Cmd_HideWeapon__Single(reader.ReadFloat());
		}
	}

	protected void UserCode_Rpc_HideWeapon__Single(float _time)
	{
		if ((bool)_raceModelED)
		{
			_raceModelED._hideTakenOutWeaponBuffer = _time;
		}
	}

	protected static void InvokeUserCode_Rpc_HideWeapon__Single(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC Rpc_HideWeapon called on server.");
		}
		else
		{
			((PlayerVisual)obj).UserCode_Rpc_HideWeapon__Single(reader.ReadFloat());
		}
	}

	protected void UserCode_Cmd_ResetSpinPlayerModel()
	{
		_raceModelRotater.Alter_RotationValues(Vector3.zero);
		_raceModelRotater.gameObject.transform.localRotation = Quaternion.identity;
		_raceModelRotater.Alter_RotationValues(Vector3.zero);
		_preventRotationLerp = false;
		Rpc_ResetSpinPlayerModel();
	}

	protected static void InvokeUserCode_Cmd_ResetSpinPlayerModel(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command Cmd_ResetSpinPlayerModel called on client.");
		}
		else
		{
			((PlayerVisual)obj).UserCode_Cmd_ResetSpinPlayerModel();
		}
	}

	protected void UserCode_Rpc_RandomSpinPlayerModel__Single(float _value)
	{
		float x = Random.Range(0f - _value, _value);
		float y = Random.Range(0f - _value, _value);
		float z = Random.Range(0f - _value, _value);
		_raceModelRotater.Alter_RotationValues(new Vector3(x, y, z));
		_visualAnimator.CrossFade("Empty", 0f, 10);
	}

	protected static void InvokeUserCode_Rpc_RandomSpinPlayerModel__Single(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC Rpc_RandomSpinPlayerModel called on server.");
		}
		else
		{
			((PlayerVisual)obj).UserCode_Rpc_RandomSpinPlayerModel__Single(reader.ReadFloat());
		}
	}

	protected void UserCode_Rpc_ResetSpinPlayerModel()
	{
		Local_ResetSpinPlayerModel();
	}

	protected static void InvokeUserCode_Rpc_ResetSpinPlayerModel(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC Rpc_ResetSpinPlayerModel called on server.");
		}
		else
		{
			((PlayerVisual)obj).UserCode_Rpc_ResetSpinPlayerModel();
		}
	}

	protected void UserCode_Rpc_PreventRotationLerp__Single(float _time)
	{
		_preventRotationLerp = true;
		StartCoroutine(PreventRotationLerp_Reset(_time));
	}

	protected static void InvokeUserCode_Rpc_PreventRotationLerp__Single(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC Rpc_PreventRotationLerp called on server.");
		}
		else
		{
			((PlayerVisual)obj).UserCode_Rpc_PreventRotationLerp__Single(reader.ReadFloat());
		}
	}

	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			GeneratedNetworkCode._Write_PlayerAppearanceStruct(writer, _playerAppearanceStruct);
			writer.WriteBool(_forceHidden);
			writer.WriteFloat(_movementAnimSpeedMod);
			return;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 1L) != 0L)
		{
			GeneratedNetworkCode._Write_PlayerAppearanceStruct(writer, _playerAppearanceStruct);
		}
		if ((base.syncVarDirtyBits & 2L) != 0L)
		{
			writer.WriteBool(_forceHidden);
		}
		if ((base.syncVarDirtyBits & 4L) != 0L)
		{
			writer.WriteFloat(_movementAnimSpeedMod);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize(ref _playerAppearanceStruct, OnAppearanceStructChange, GeneratedNetworkCode._Read_PlayerAppearanceStruct(reader));
			GeneratedSyncVarDeserialize(ref _forceHidden, null, reader.ReadBool());
			GeneratedSyncVarDeserialize(ref _movementAnimSpeedMod, null, reader.ReadFloat());
			return;
		}
		long num = (long)reader.ReadULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _playerAppearanceStruct, OnAppearanceStructChange, GeneratedNetworkCode._Read_PlayerAppearanceStruct(reader));
		}
		if ((num & 2L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _forceHidden, null, reader.ReadBool());
		}
		if ((num & 4L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _movementAnimSpeedMod, null, reader.ReadFloat());
		}
	}
}
