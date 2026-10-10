using System.Collections;
using UnityEngine;

public class PlayerModelAnimManager : MonoBehaviour
{
	private Player _player;

	private PlayerMove _pMove;

	private PlayerCombat _pCombat;

	private PlayerVisual _pVisual;

	private PlayerSound _pSound;

	private PlayerWater _pWater;

	private PlayerRaceModel _raceModel;

	private RaceModelEquipDisplay _raceModelED;

	public GroundMaterial _walkGroundMaterial;

	[Space]
	public ParticleSystem _weaponChargeEffect;

	public ParticleSystem[] _weaponTrailEffects = new ParticleSystem[0];

	[Space]
	public SpriteRenderer _showItemRender;

	private float _jumpForce;

	private float _forwardJumpForce;

	private float _gravityModForce;

	private bool _isRandomRotated;

	private bool Validate_PlayerCondition()
	{
		if (!_player)
		{
			return false;
		}
		if (_player._bufferingStatus)
		{
			return false;
		}
		if (!Player._mainPlayer)
		{
			return false;
		}
		if (Player._mainPlayer.Network_playerMapInstance != _player.Network_playerMapInstance)
		{
			return false;
		}
		return true;
	}

	private void Awake()
	{
		_raceModel = GetComponentInParent<PlayerRaceModel>();
		_raceModelED = GetComponentInParent<RaceModelEquipDisplay>();
	}

	public void Set_TailMovementCondition()
	{
		if (_raceModel._tailDisplays.Length != 0 && !(_raceModel._tailDisplays[_raceModel.SetTailIndex].transform.parent.gameObject == null) && (bool)_raceModel._tailDisplays[_raceModel.SetTailIndex].transform.parent.gameObject.GetComponent<Animator>())
		{
			Animator component = _raceModel._tailDisplays[_raceModel.SetTailIndex].transform.parent.gameObject.GetComponent<Animator>();
			if (!component.GetBool("_capeTailMode"))
			{
				component.CrossFade("Tail_Move", 0.5f, 0);
			}
		}
	}

	private void Update()
	{
		if ((bool)_player)
		{
			_pMove = _player._pMove;
			_pCombat = _player._pCombat;
			_pVisual = _player._pVisual;
			_pSound = _player._pSound;
			if (!_pWater)
			{
				_pWater = _player.GetComponentInChildren<PlayerWater>();
			}
			if (Validate_PlayerCondition())
			{
				Detect_GroundMaterialType();
				Set_CombatEffectColors();
			}
		}
		else
		{
			_player = GetComponentInParent<Player>();
		}
	}

	private void Detect_GroundMaterialType()
	{
		if (!Validate_PlayerCondition())
		{
			return;
		}
		PhysicsScene physicsScene = base.gameObject.scene.GetPhysicsScene();
		Vector3 position = base.transform.position;
		position.y = base.transform.position.y + 3.5f;
		physicsScene.SphereCast(position, 0.3f, Vector3.down, out var hitInfo, 6.5f, CastLayers.FootstepMask);
		_pSound._aSrcFootstep.pitch = _raceModel._scriptablePlayerRace._footstepPitch + Random.Range(-0.0946982f, 0.0945692f);
		if ((bool)hitInfo.collider)
		{
			TerrainCollider component2;
			if (hitInfo.collider.gameObject.TryGetComponent<GroundType>(out var component))
			{
				_walkGroundMaterial = component.groundMaterial;
			}
			else if (hitInfo.collider.gameObject.TryGetComponent<TerrainCollider>(out component2) && (bool)_player.Network_playerMapInstance && _player.Network_playerMapInstance._terrainManager._terrainManagerDatas.Count > 0)
			{
				int terrainAtPosition = _player.Network_playerMapInstance._terrainManager.GetTerrainAtPosition(hitInfo.point, component2);
				_walkGroundMaterial = _player.Network_playerMapInstance._terrainManager.groundMaterials[terrainAtPosition];
			}
			else
			{
				_walkGroundMaterial = GroundMaterial.Basic;
			}
		}
	}

	private void Set_CombatEffectColors()
	{
		if ((bool)_pCombat._equippedWeapon)
		{
			ParticleSystem component = _weaponChargeEffect.transform.GetChild(0).GetComponent<ParticleSystem>();
			ParticleSystem.MainModule main = _weaponChargeEffect.main;
			main.startColor = _pCombat._equippedWeapon.trailColors[0];
			ParticleSystem.MainModule main2 = component.main;
			main2.startColor = _pCombat._equippedWeapon.trailColors[0];
			for (int i = 0; i < _weaponTrailEffects.Length; i++)
			{
				ParticleSystem.MainModule main3 = _weaponTrailEffects[i].main;
				main3.startColor = _pCombat._equippedWeapon.trailColors[0];
			}
		}
	}

	public void Set_AttackControlLockBuffer(float _value)
	{
		_pCombat._attackControlBuffer = _value;
	}

	public void Init_AttackMoveForce()
	{
		if (_player.isLocalPlayer)
		{
			_pMove._isChargeAttackForced = false;
			if (_pMove.RayGroundCheck())
			{
				StartCoroutine(Init_ApplyAttackForce(_pCombat._currentScriptableWeaponType._weaponData._attackMoveForceDampen, _pCombat._currentScriptableWeaponType._weaponData._attackMoveForce));
			}
		}
	}

	public void Init_ChargeAttackMoveForce()
	{
		if (_player.isLocalPlayer && (_pCombat._setWepTypeCombatParams._applyChargeAttackForceOnNoInput || InputControlManager.current._horiz_input != 0f || InputControlManager.current._vert_input != 0f))
		{
			_pMove._isChargeAttackForced = true;
			StartCoroutine(Init_ApplyAttackForce(_pCombat._setWepTypeCombatParams._chargeAttackMoveForceDampen, _pCombat._setWepTypeCombatParams._chargeAttackMoveForce));
		}
	}

	private IEnumerator Init_ApplyAttackForce(float _dampen, float _force)
	{
		yield return new WaitForSeconds(0.04f);
		if (_pMove.RayGroundCheck() && _force >= 85f)
		{
			_pVisual._attackForceCloudEffect.Play();
		}
		_pMove._attackForceDampen = _dampen;
		_pMove._attackForce = _force;
	}

	public void Init_RangedWeaponAttack(int _swingState)
	{
		_pCombat.Init_ShootProjectile(_swingState);
	}

	public void Init_HitboxCollider()
	{
		_pCombat._meleeJumpDamageCollider._noHitstunChance = false;
		_pCombat._meleeDamageCollider.Initalize_ColliderData();
		_pCombat._meleeHitBox.enabled = true;
		StopCoroutine("Reset_Hitboxes");
		StartCoroutine(Reset_Hitboxes());
	}

	public void Init_HitboxColliderPercent(float _percentage)
	{
		if (!(_percentage < 0f))
		{
			_pCombat._meleeJumpDamageCollider._noHitstunChance = _percentage < 1f;
			_pCombat._meleeDamageCollider._damagePercent = _percentage;
			_pCombat._meleeDamageCollider.Initalize_ColliderData();
			_pCombat._meleeHitBox.enabled = true;
			StopCoroutine("Reset_Hitboxes");
			StartCoroutine(Reset_Hitboxes());
		}
	}

	public void Init_JumpHitboxCollider()
	{
		if (!_pCombat._isAirAttacked && !_pCombat._isBlocking)
		{
			_pCombat._meleeJumpDamageCollider.Initalize_ColliderData();
			if (!_pCombat._meleeJumpHitBox.enabled)
			{
				_pCombat._meleeJumpHitBox.enabled = true;
			}
			_pVisual._playerRaceModel.Set_EyeCondition(EyeCondition.Pissed, 2.75f);
			_pCombat._isAirAttacked = true;
		}
	}

	public void Init_JumpLandHitboxCollider()
	{
		if (!_pCombat._isBlocking)
		{
			_pCombat._meleeJumpLandDamageCollider.Initalize_ColliderData();
			_pCombat._meleeJumpLandHitBox.enabled = true;
			_pCombat._meleeJumpHitBox.enabled = false;
			StopCoroutine("Reset_Hitboxes");
			StartCoroutine(Reset_Hitboxes());
		}
	}

	public void Init_ChargeHitboxCollider()
	{
		_pCombat._meleeChargeCollider.Initalize_ColliderData();
		if (!_pCombat._meleeChargeHitBox.enabled)
		{
			_pCombat._meleeChargeHitBox.enabled = true;
		}
		_pVisual._playerRaceModel.Set_EyeCondition(EyeCondition.Pissed, 2.75f);
		StopCoroutine("Reset_Hitboxes");
		StartCoroutine(Reset_Hitboxes());
	}

	private IEnumerator Reset_Hitboxes()
	{
		float seconds = _pCombat._currentScriptableWeaponType._weaponData._hitboxTime;
		if (_pCombat._meleeChargeHitBox.enabled)
		{
			seconds = _pCombat._setWepTypeCombatParams._chargeAttackHitboxTime;
		}
		if (_pCombat._meleeJumpLandHitBox.enabled)
		{
			seconds = _pCombat._setWepTypeCombatParams._jumpAtkLandHitboxTime;
		}
		yield return new WaitForSeconds(seconds);
		Init_ResetAllHitboxes();
	}

	public void Init_ResetAllHitboxes()
	{
		_pCombat.Server_DisableHitboxObjects();
		_pCombat._isAirAttacked = false;
		_isRandomRotated = false;
	}

	public void Init_IFrameTime()
	{
		_player.Set_IFrame(GameManager._current._statLogics._dashIFrameTime);
	}

	public void Init_ChargeEffect(bool _isTrue)
	{
		if ((bool)Player._mainPlayer && (bool)_pCombat && (bool)_pCombat._currentScriptableWeaponType && (bool)_pCombat._equippedWeapon)
		{
			ParticleSystemRenderer component = _weaponChargeEffect.GetComponent<ParticleSystemRenderer>();
			if (Player._mainPlayer._isHostPlayer)
			{
				component.enabled = Player._mainPlayer.gameObject.scene == base.gameObject.scene;
			}
			_weaponChargeEffect.gameObject.SetActive(value: true);
			switch (_pCombat._currentScriptableWeaponType._weaponDisplayMode)
			{
			case WeaponRenderDisplaymode.RIGHT:
				_weaponChargeEffect.transform.SetParent(_raceModelED._rightHandWeaponRender.transform);
				break;
			case WeaponRenderDisplaymode.LEFT:
				_weaponChargeEffect.transform.SetParent(_raceModelED._leftHandWeaponRender.transform);
				break;
			case WeaponRenderDisplaymode.DUAL:
				_weaponChargeEffect.transform.SetParent(base.transform);
				break;
			}
			_weaponChargeEffect.transform.localScale = Vector3.one;
			_weaponChargeEffect.transform.localPosition = _pCombat._currentScriptableWeaponType._weaponChargeEffectPosition;
			if (_isTrue)
			{
				_weaponChargeEffect.Play();
			}
			else
			{
				_weaponChargeEffect.Stop();
			}
		}
	}

	public void Set_JumpForce(float _value)
	{
		_jumpForce = _value;
	}

	public void Set_ForwardForce(float _value)
	{
		_forwardJumpForce = _value;
	}

	public void Set_GravityForce(float _value)
	{
		_gravityModForce = _value;
	}

	public void Init_AirLockCondition()
	{
		_pMove._lockControlMidair = true;
	}

	public void Init_Jump()
	{
		_pMove._currentJumps = 0;
		_pMove.Init_Jump(_jumpForce, _forwardJumpForce, 0f, _gravityModForce, _useAnim: true);
	}

	public void Init_JumpGroundOnly()
	{
		if (_pMove.RayGroundCheck())
		{
			_pMove._currentJumps = _pMove._maxJumps;
			_pMove.Init_Jump(_jumpForce, _forwardJumpForce, 0f, _gravityModForce, _useAnim: true);
		}
	}

	public void Init_JumpAirOnly()
	{
		if (!_pMove.RayGroundCheck())
		{
			_pMove._currentJumps = _pMove._maxJumps;
			_pMove.Init_Jump(_jumpForce, _forwardJumpForce, 0f, _gravityModForce, _useAnim: false);
		}
	}

	public void Init_VerticalSpin(float _value)
	{
		_pVisual._raceModelRotater.Alter_RotationValues(new Vector3(_value, 0f, 0f));
		_pVisual._preventRotationLerp = true;
		_pVisual.HardReset_RotationLerpInit(0.45f);
	}

	public void Init_HoriztonalSpin(float _value)
	{
		_pVisual._raceModelRotater.Alter_RotationValues(new Vector3(0f, _value, 0f));
		_pVisual._preventRotationLerp = true;
		_pVisual.HardReset_RotationLerpInit(0.45f);
	}

	public void Set_ZRotation(float _value)
	{
		if (!_isRandomRotated)
		{
			if (Random.Range(1, 3) == 2)
			{
				_value *= -1f;
			}
			_pVisual._raceModelRotater.gameObject.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, _value));
			_pVisual._preventRotationLerp = true;
			_pVisual.Prevent_RotationLerp(0.45f);
		}
		_isRandomRotated = true;
	}

	public void Reset_Spin()
	{
		_pVisual.HardReset_RotationLerpInit(0f);
	}

	public void Shake_Camera(float _distance)
	{
		if (Validate_PlayerCondition() && !(Vector3.Distance(Player._mainPlayer.transform.position, base.transform.position) > _distance))
		{
			CameraFunction._current.MyShaker.Shake(CameraFunction._current._verticalShakePreset);
		}
	}

	public void PlayDashEffect()
	{
		if (Validate_PlayerCondition() && !_pWater._isInWater)
		{
			_pVisual._dashEffect.gameObject.SetActive(value: true);
		}
	}

	public void PlaySound(int _index)
	{
		if (Validate_PlayerCondition() && _pSound._aSrcGeneral.enabled)
		{
			_pSound._aSrcGeneral.PlayOneShot(_pSound._generalSounds[_index], 0.65f);
		}
	}

	public void PlayJumpSound()
	{
		if (Validate_PlayerCondition() && _pSound._aSrcJump.enabled && !_pSound._aSrcJump.isPlaying)
		{
			_pSound._aSrcJump.Play();
		}
	}

	public void PlayWeaponSwingSound()
	{
		if (!Validate_PlayerCondition() || !_pSound._aSrcGeneral.enabled)
		{
			return;
		}
		ScriptableWeapon equippedWeapon = _pCombat._equippedWeapon;
		ScriptableWeaponType currentScriptableWeaponType = _pCombat._currentScriptableWeaponType;
		_pSound._aSrcCombat.pitch = Random.Range(0.98715f, 1.03847f);
		if ((bool)equippedWeapon)
		{
			if ((bool)equippedWeapon._setWeaponSwingSfx)
			{
				_pSound._aSrcCombat.PlayOneShot(equippedWeapon._setWeaponSwingSfx, 0.45f);
			}
		}
		else if ((bool)currentScriptableWeaponType._weaponSwingSfx[0])
		{
			_pSound._aSrcCombat.PlayOneShot(currentScriptableWeaponType._weaponSwingSfx[0], 0.45f);
		}
	}

	public void PlayWeaponTakeOutSound()
	{
		if (Validate_PlayerCondition() && _pSound._aSrcGeneral.enabled)
		{
			ScriptableWeapon equippedWeapon = _pCombat._equippedWeapon;
			_pSound._aSrcCombat.pitch = Random.Range(0.98715f, 1.03847f);
			if ((bool)equippedWeapon && (bool)equippedWeapon._setWeaponTakeOutSfx)
			{
				_pSound._aSrcCombat.PlayOneShot(equippedWeapon._setWeaponTakeOutSfx, 0.45f);
			}
		}
	}

	public void PlayFootstep()
	{
		if (Validate_PlayerCondition() && _pSound._aSrcFootstep.enabled && _pMove._movementLockType == MovementLockType.NONE && _pMove.RayGroundCheck())
		{
			if (_pWater._isInWater)
			{
				_pSound._aSrcFootstep.PlayOneShot(_pSound._waterWalkStep, 1f);
				return;
			}
			_pSound._aSrcFootstep.PlayOneShot(_pSound._footstepSounds[(int)_walkGroundMaterial], 1f);
			_pVisual.Play_FootstepEffect((int)_walkGroundMaterial);
		}
	}

	public void Init_EyeCondition(int _index)
	{
		if (!_raceModel._eyeConditionIsInfinite)
		{
			_raceModel.Set_EyeCondition((EyeCondition)_index, 1.25f);
		}
	}

	public void Init_MouthCondition(int _index)
	{
		if (!_raceModel._mouthConditionIsInfinite)
		{
			_raceModel.Set_MouthCondition((MouthCondition)_index, 1.25f);
		}
	}
}
