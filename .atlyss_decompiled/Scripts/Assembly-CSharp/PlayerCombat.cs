using System.Collections;
using System.Runtime.InteropServices;
using Mirror;
using Mirror.RemoteCalls;
using UnityEngine;

public class PlayerCombat : NetworkBehaviour
{
	private Player _player;

	private PlayerMove _pMove;

	private PlayerCasting _pCasting;

	private PlayerEquipment _pEquipment;

	private PlayerVisual _pVisual;

	private PlayerSound _pSound;

	private PlayerStats _pStats;

	private StatusEntity _statusEntity;

	[Header("Weapon Sheathing")]
	[SyncVar(hook = "OnWeaponSheatheChange")]
	public WeaponSheatheCondition _currentSheathCondition;

	[SerializeField]
	private float _commandBufferTimer = 0.65f;

	[SerializeField]
	private float _quickSwapBufferTimer = 0.25f;

	public float _currentCommandTimer;

	public bool _weaponSwapQueued;

	public bool _lockSheatheWeaponControl;

	private float _switchWeaponBuffer;

	private bool _commandBuffer;

	[Header("Player Combat Params")]
	public ScriptableWeaponType _currentScriptableWeaponType;

	public ScriptableWeaponType _unarmedScriptableWeaponType;

	public WeaponTypeCombatParams _setWepTypeCombatParams;

	public float _weaponTypeDamageBonus;

	[Space]
	public int _currentSwingState;

	public float _focusWindowBuffer;

	public float _lockControlBuffer;

	public float _attackControlBuffer;

	public float _groundAttackControlBuffer;

	private float _emptyCombatStateBuffer;

	private bool _withinAttackQueue;

	private float _actionBuffer;

	private int _maxSwingState;

	[Header("Equipped Weaponry")]
	public ScriptableWeapon _equippedWeapon;

	public ScriptableShield _equippedShield;

	[SyncVar]
	public bool _isUsingAltWeapon;

	[SyncVar]
	public bool _lockWeapon;

	public bool _isDisarmed;

	private bool _isUnarmed = true;

	[Space]
	[HideInInspector]
	public EquipData _foundWeaponEquip;

	[HideInInspector]
	public EquipData _foundAltWeaponEquip;

	[HideInInspector]
	public EquipData _foundShieldEquip;

	[Header("Hitbox Params")]
	[SyncVar]
	public DamageType _damageTypeOverride;

	[SyncVar]
	public bool _useDamageTypeOverride;

	[Space]
	public BoxCollider _meleeHitBox;

	public BoxCollider _meleeJumpHitBox;

	public BoxCollider _meleeJumpLandHitBox;

	public BoxCollider _meleeChargeHitBox;

	[Space]
	public CombatCollider _meleeDamageCollider;

	public CombatCollider _meleeJumpDamageCollider;

	public CombatCollider _meleeJumpLandDamageCollider;

	public CombatCollider _meleeChargeCollider;

	[Header("Ranged Weapon Params")]
	public Transform _rangeWeaponBulletPos;

	[Header("Jump Attacking")]
	public Transform _airAttackCastPos;

	public float _airAttackLock;

	public bool _isAirAttacked;

	public bool _localAirAttackInput;

	private bool _detectGroundFromAirAttack;

	private bool _hittedWall;

	private float _jumpAttackBuffer;

	[Header("Charge Attacking")]
	public float _maxChargeInputBuffer = 0.08f;

	public float _chargeAttackCooldown;

	public bool _isChargingWeapon;

	private float _currentChargeBuffer;

	private bool _isChargedWeapon;

	[Header("Blocking")]
	[SyncVar(hook = "OnBlockingChange")]
	public bool _isBlocking;

	[SyncVar]
	public bool _inParryWindow;

	public bool _localBlockingInput;

	public float _blockControlBuffer;

	public float _attackBlockBuffer;

	public float _parryChanceBuffer;

	public float _parryChanceModifier;

	private float _staminaReductionRate;

	[Header("Creep Combat Queue")]
	public CreepQueue _setCreepQueue = new CreepQueue();

	private bool IsQuickSwapInput
	{
		get
		{
			if (!Input.GetKeyDown(InputControlManager.current._quickSwapWeapon))
			{
				return GamepadInput.GetButtonDown(GamepadButton.Button_North);
			}
			return true;
		}
	}

	public bool IsChargeAttackInput
	{
		get
		{
			if (!Input.GetKey(InputControlManager.current._chargeAttack))
			{
				if (GamepadInput.GetButton(GamepadButton.RightTrigger))
				{
					return GamepadInput.GetButton(GamepadButton.LeftShoulder);
				}
				return false;
			}
			return true;
		}
	}

	public bool IsBlockInput
	{
		get
		{
			if (!Input.GetKey(InputControlManager.current._block))
			{
				return GamepadInput.GetButton(GamepadButton.LeftTrigger);
			}
			return true;
		}
	}

	public WeaponSheatheCondition Network_currentSheathCondition
	{
		get
		{
			return _currentSheathCondition;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _currentSheathCondition, 1uL, OnWeaponSheatheChange);
		}
	}

	public bool Network_isUsingAltWeapon
	{
		get
		{
			return _isUsingAltWeapon;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _isUsingAltWeapon, 2uL, null);
		}
	}

	public bool Network_lockWeapon
	{
		get
		{
			return _lockWeapon;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _lockWeapon, 4uL, null);
		}
	}

	public DamageType Network_damageTypeOverride
	{
		get
		{
			return _damageTypeOverride;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _damageTypeOverride, 8uL, null);
		}
	}

	public bool Network_useDamageTypeOverride
	{
		get
		{
			return _useDamageTypeOverride;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _useDamageTypeOverride, 16uL, null);
		}
	}

	public bool Network_isBlocking
	{
		get
		{
			return _isBlocking;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _isBlocking, 32uL, OnBlockingChange);
		}
	}

	public bool Network_inParryWindow
	{
		get
		{
			return _inParryWindow;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _inParryWindow, 64uL, null);
		}
	}

	private void OnApplicationFocus(bool focus)
	{
		if (focus)
		{
			_focusWindowBuffer = 0.35f;
		}
	}

	private void Awake()
	{
		_player = GetComponent<Player>();
		_pMove = GetComponent<PlayerMove>();
		_pCasting = GetComponent<PlayerCasting>();
		_pEquipment = GetComponent<PlayerEquipment>();
		_pVisual = GetComponent<PlayerVisual>();
		_pSound = GetComponent<PlayerSound>();
		_pStats = GetComponent<PlayerStats>();
		_statusEntity = GetComponent<StatusEntity>();
		_meleeHitBox.enabled = false;
		_meleeJumpHitBox.enabled = false;
		_meleeChargeHitBox.enabled = false;
		_meleeJumpLandHitBox.enabled = false;
	}

	private void Update()
	{
		Global_Handle_EquippedWeaponry();
		Server_Handle_BlockCondition();
		Client_Handle_BufferingParams();
		Server_Handle_BufferingParams();
		if (_isBlocking)
		{
			_isAirAttacked = false;
		}
		if (_player._currentPlayerCondition != PlayerCondition.ACTIVE)
		{
			Server_DisableHitboxObjects();
		}
		if ((bool)_player._pVisual._visualAnimator && _player._pVisual._visualAnimator.GetCurrentAnimatorStateInfo(3).IsName("Empty"))
		{
			if (_emptyCombatStateBuffer >= 0.4f)
			{
				Server_DisableHitboxObjects();
			}
			else
			{
				_emptyCombatStateBuffer += Time.deltaTime;
			}
		}
		else
		{
			_emptyCombatStateBuffer = 0f;
		}
		if (base.isLocalPlayer && !_player._bufferingStatus && _player._currentPlayerCondition == PlayerCondition.ACTIVE)
		{
			Client_Handle_WeaponSheatheControl();
			Client_Handle_QuickSwapWeaponControl();
			Client_Handle_CombatControl();
		}
	}

	private void Global_Handle_EquippedWeaponry()
	{
		_foundWeaponEquip = _pEquipment._equips["Weapon"];
		_foundAltWeaponEquip = _pEquipment._equips["Alt Weapon"];
		_foundShieldEquip = _pEquipment._equips["Shield"];
		bool _hasEquippedWeapon;
		if (!_lockWeapon)
		{
			if (_isUsingAltWeapon)
			{
				_hasEquippedWeapon = _foundAltWeaponEquip._scriptableEquip != null && !_isDisarmed;
			}
			else
			{
				_hasEquippedWeapon = _foundWeaponEquip._scriptableEquip != null && !_isDisarmed;
			}
		}
		else
		{
			_hasEquippedWeapon = _foundWeaponEquip._scriptableEquip != null && !_isDisarmed;
		}
		bool flag = _foundShieldEquip._scriptableEquip != null;
		if (flag)
		{
			_equippedShield = (ScriptableShield)_foundShieldEquip._scriptableEquip;
		}
		else
		{
			_equippedShield = null;
		}
		Apply_EquippedWeaponProperties();
		Apply_UnarmedProperties();
		_weaponTypeDamageBonus = _pStats.Retrieve_WeaponTypeBonusPower();
		if ((bool)_pVisual._visualAnimator && (bool)_currentScriptableWeaponType)
		{
			_pVisual._visualAnimator.SetBool("_hasShield", flag && _currentScriptableWeaponType._weaponHandedness == WeaponHandedness.One_Handed);
		}
		void Apply_EquippedWeaponProperties()
		{
			if (_hasEquippedWeapon)
			{
				if (!_lockWeapon)
				{
					if (_isUsingAltWeapon && (bool)_foundAltWeaponEquip._scriptableEquip)
					{
						_equippedWeapon = (ScriptableWeapon)_foundAltWeaponEquip._scriptableEquip;
						Network_useDamageTypeOverride = _pEquipment._equips["Alt Weapon"]._heldItem._useDamageTypeOverride;
						Network_damageTypeOverride = _pEquipment._equips["Alt Weapon"]._heldItem._damageTypeOverride;
					}
					else if ((bool)_foundWeaponEquip._scriptableEquip)
					{
						_equippedWeapon = (ScriptableWeapon)_foundWeaponEquip._scriptableEquip;
						Network_useDamageTypeOverride = _pEquipment._equips["Weapon"]._heldItem._useDamageTypeOverride;
						Network_damageTypeOverride = _pEquipment._equips["Weapon"]._heldItem._damageTypeOverride;
					}
				}
				else if ((bool)_foundWeaponEquip._scriptableEquip)
				{
					_equippedWeapon = (ScriptableWeapon)_foundWeaponEquip._scriptableEquip;
					Network_useDamageTypeOverride = _pEquipment._equips["Weapon"]._heldItem._useDamageTypeOverride;
					Network_damageTypeOverride = _pEquipment._equips["Weapon"]._heldItem._damageTypeOverride;
				}
				if ((bool)_equippedWeapon)
				{
					_currentScriptableWeaponType = _equippedWeapon.weaponType;
					_setWepTypeCombatParams = _equippedWeapon.weaponType._weaponData._weaponTypeCombatParams[0];
					if ((bool)_pVisual._visualAnimator)
					{
						_pVisual._visualAnimator.SetBool("_holdingKatars", _currentScriptableWeaponType._weaponDisplayMode == WeaponRenderDisplaymode.DUAL);
					}
					if (_pCasting._passiveSkillObjects.Count > 0)
					{
						for (int i = 0; i < _pCasting._passiveSkillObjects.Count; i++)
						{
							SkillObjectBehavior skillObjectBehavior = _pCasting._passiveSkillObjects[i];
							if (skillObjectBehavior.GetType() == typeof(SkillBehavior_WeaponMastery))
							{
								SkillBehavior_WeaponMastery skillBehavior_WeaponMastery = (SkillBehavior_WeaponMastery)skillObjectBehavior;
								for (int j = 0; j < skillBehavior_WeaponMastery._setWeaponTypes.Length; j++)
								{
									if (skillBehavior_WeaponMastery._setWeaponTypes[j] == _currentScriptableWeaponType)
									{
										_setWepTypeCombatParams = _equippedWeapon.weaponType._weaponData._weaponTypeCombatParams[skillBehavior_WeaponMastery._setParamIndex];
									}
								}
							}
						}
					}
					_maxSwingState = _currentScriptableWeaponType._weaponData._maxSwingStates;
					_maxChargeInputBuffer = _setWepTypeCombatParams._chargeInputBuffer;
					_meleeHitBox.size = _currentScriptableWeaponType._weaponData._hitboxScale;
					_meleeHitBox.center = _currentScriptableWeaponType._weaponData._hitboxCenter;
					_meleeDamageCollider._scriptWeapon = _equippedWeapon;
					_meleeDamageCollider._dmgWeight = _currentScriptableWeaponType._damageWeight;
					_meleeDamageCollider._isPersistantDamage = _currentScriptableWeaponType._weaponData._hitboxPersistantTime > 0f;
					_meleeDamageCollider._outputBuffer = _currentScriptableWeaponType._weaponData._hitboxPersistantTime;
					_meleeDamageCollider._percentAdjustEachHit = _currentScriptableWeaponType._weaponData._percentAdjustEachHit;
					if ((bool)_equippedWeapon._combatElement)
					{
						_meleeDamageCollider._combatElement = _equippedWeapon._combatElement;
					}
					else
					{
						_meleeDamageCollider._combatElement = GameManager._current._statLogics._defaultCombatElement;
					}
					_meleeJumpHitBox.size = _setWepTypeCombatParams._jumpAttackColliderScale;
					_meleeJumpHitBox.center = _setWepTypeCombatParams._jumpAttackColliderCenter;
					_meleeJumpDamageCollider._scriptWeapon = _equippedWeapon;
					_meleeJumpDamageCollider._dmgWeight = _currentScriptableWeaponType._damageWeight;
					_meleeJumpDamageCollider._damagePercent = _setWepTypeCombatParams._jumpDamagePercent;
					if ((bool)_equippedWeapon._combatElement)
					{
						_meleeJumpDamageCollider._combatElement = _equippedWeapon._combatElement;
					}
					else
					{
						_meleeJumpDamageCollider._combatElement = GameManager._current._statLogics._defaultCombatElement;
					}
					_meleeJumpDamageCollider._isPersistantDamage = _setWepTypeCombatParams._persistantDamageRate > 0f;
					_meleeJumpDamageCollider._outputBuffer = _setWepTypeCombatParams._persistantDamageRate;
					_meleeJumpLandHitBox.size = _setWepTypeCombatParams._jumpAttackLandColliderScale;
					_meleeJumpLandHitBox.center = _setWepTypeCombatParams._jumpAttackLandColliderCenter;
					_meleeJumpLandDamageCollider._scriptWeapon = _equippedWeapon;
					_meleeJumpLandDamageCollider._dmgWeight = _currentScriptableWeaponType._damageWeight;
					_meleeJumpLandDamageCollider._damageForce = _setWepTypeCombatParams._jumpLandForce;
					_meleeJumpLandDamageCollider._damagePercent = _setWepTypeCombatParams._jumpLandDamagePercent;
					if ((bool)_equippedWeapon._combatElement)
					{
						_meleeJumpLandDamageCollider._combatElement = _equippedWeapon._combatElement;
					}
					else
					{
						_meleeJumpLandDamageCollider._combatElement = GameManager._current._statLogics._defaultCombatElement;
					}
					_meleeJumpLandDamageCollider._isPersistantDamage = _setWepTypeCombatParams._persistantDamageRate > 0f;
					_meleeJumpLandDamageCollider._outputBuffer = _setWepTypeCombatParams._jumpAtkLandPersistantDamageRate;
					_meleeChargeHitBox.size = _setWepTypeCombatParams._chargeAttackColliderScale;
					_meleeChargeHitBox.center = _setWepTypeCombatParams._chargeAttackColliderCenter;
					_meleeChargeCollider._scriptWeapon = _equippedWeapon;
					_meleeChargeCollider._dmgWeight = _currentScriptableWeaponType._damageWeight;
					_meleeChargeCollider._damageForce = _setWepTypeCombatParams._chargeAttackForce;
					_meleeChargeCollider._damagePercent = _setWepTypeCombatParams._chargeAtkDamagePercent;
					if ((bool)_equippedWeapon._combatElement)
					{
						_meleeChargeCollider._combatElement = _equippedWeapon._combatElement;
					}
					else
					{
						_meleeChargeCollider._combatElement = GameManager._current._statLogics._defaultCombatElement;
					}
					_meleeChargeCollider._isPersistantDamage = _setWepTypeCombatParams._persistantDamageRate > 0f;
					_meleeChargeCollider._outputBuffer = _setWepTypeCombatParams._chargeAtkPersistantDamageRate;
					ItemData itemData = (_lockWeapon ? _foundWeaponEquip._heldItem : ((!_isUsingAltWeapon) ? _foundWeaponEquip._heldItem : _foundAltWeaponEquip._heldItem));
					if (itemData._modifierID > 0)
					{
						ScriptableStatModifier scriptableStatModifier = GameManager._current.Locate_StatModifier(itemData._modifierID);
						if ((bool)scriptableStatModifier && (bool)scriptableStatModifier._modifierElement)
						{
							_meleeDamageCollider._combatElement = scriptableStatModifier._modifierElement;
							_meleeJumpDamageCollider._combatElement = scriptableStatModifier._modifierElement;
							_meleeChargeCollider._combatElement = scriptableStatModifier._modifierElement;
						}
					}
					_isUnarmed = false;
				}
			}
		}
		void Apply_UnarmedProperties()
		{
			if (!_hasEquippedWeapon)
			{
				_currentScriptableWeaponType = _unarmedScriptableWeaponType;
				if ((bool)_pVisual._raceModelED)
				{
					_pVisual._raceModelED.Remove_WeaponDisplay();
				}
				_maxSwingState = _currentScriptableWeaponType._weaponData._maxSwingStates;
				_maxChargeInputBuffer = _setWepTypeCombatParams._chargeInputBuffer;
				_meleeDamageCollider._combatElement = GameManager._current._statLogics._defaultCombatElement;
				_meleeJumpDamageCollider._combatElement = GameManager._current._statLogics._defaultCombatElement;
				_meleeDamageCollider._dmgType = _currentScriptableWeaponType._combatType;
				_meleeDamageCollider._dmgWeight = _currentScriptableWeaponType._damageWeight;
				_meleeDamageCollider._scriptWeapon = null;
				_meleeJumpDamageCollider._scriptWeapon = null;
				_setWepTypeCombatParams = _currentScriptableWeaponType._weaponData._weaponTypeCombatParams[0];
				_pVisual.Local_CrossFadeAnim("Hold_None", 0f, 2);
				_meleeHitBox.size = _currentScriptableWeaponType._weaponData._hitboxScale;
				_meleeDamageCollider._scriptableConditionSlot = new ConditionSlot();
				_meleeJumpDamageCollider._scriptableConditionSlot = new ConditionSlot();
				_meleeChargeCollider._scriptableConditionSlot = new ConditionSlot();
				_equippedWeapon = null;
				_isUnarmed = true;
				if (base.isServer)
				{
					Network_damageTypeOverride = DamageType.Strength;
					Network_useDamageTypeOverride = false;
				}
			}
		}
	}

	[ServerCallback]
	private void Server_Handle_BufferingParams()
	{
		if (NetworkServer.active)
		{
			Network_inParryWindow = _parryChanceBuffer > 0f;
			Handle_CreepAttackerQueue();
			Iterate_HitboxParams();
		}
		void Handle_CreepAttackerQueue()
		{
			if (!_setCreepQueue._creepAttacker)
			{
				_setCreepQueue._bufferTime = 0f;
			}
			if (_setCreepQueue._bufferTime > 0f)
			{
				_setCreepQueue._bufferTime -= Time.deltaTime;
			}
			else
			{
				_setCreepQueue._bufferTime = 0f;
			}
			if (_setCreepQueue._bufferTime <= 0f)
			{
				_setCreepQueue._creepAttacker = null;
			}
		}
		void Iterate_HitboxParams()
		{
			if (_player._bufferingStatus || _isBlocking)
			{
				Server_DisableHitboxObjects();
			}
		}
	}

	[Command]
	public void Cmd_ResetHitboxes()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void PlayerCombat::Cmd_ResetHitboxes()", 305637848, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ServerCallback]
	public void Server_DisableHitboxObjects()
	{
		if (NetworkServer.active)
		{
			_meleeHitBox.enabled = false;
			_meleeJumpHitBox.enabled = false;
			_meleeChargeHitBox.enabled = false;
			_meleeJumpLandHitBox.enabled = false;
		}
	}

	[ClientCallback]
	private void Init_ActionBuffer(float _buffer)
	{
		if (NetworkClient.active)
		{
			_player._currentPlayerAction = PlayerAction.ATTACKING;
			_actionBuffer = _buffer;
		}
	}

	[ClientCallback]
	private void Client_Handle_BufferingParams()
	{
		if (!NetworkClient.active || !base.isLocalPlayer)
		{
			return;
		}
		if (_focusWindowBuffer > 0f)
		{
			_focusWindowBuffer -= Time.deltaTime;
		}
		else
		{
			_focusWindowBuffer = 0f;
		}
		if (_chargeAttackCooldown > 0f)
		{
			_chargeAttackCooldown -= Time.deltaTime;
		}
		else
		{
			_chargeAttackCooldown = 0f;
		}
		if (_jumpAttackBuffer > 0f)
		{
			_jumpAttackBuffer -= Time.deltaTime;
		}
		else
		{
			_jumpAttackBuffer = 0f;
		}
		if (_lockControlBuffer > 0f)
		{
			_lockControlBuffer -= Time.deltaTime;
		}
		else
		{
			_lockControlBuffer = 0f;
		}
		if (_attackControlBuffer > 0f)
		{
			_attackControlBuffer -= Time.deltaTime;
		}
		else
		{
			_attackControlBuffer = 0f;
		}
		if (_groundAttackControlBuffer > 0f)
		{
			_groundAttackControlBuffer -= Time.deltaTime;
		}
		else
		{
			_groundAttackControlBuffer = 0f;
		}
		if (_airAttackLock > 0f)
		{
			_airAttackLock -= Time.deltaTime;
		}
		else
		{
			_airAttackLock = 0f;
		}
		if (_switchWeaponBuffer > 0f)
		{
			_switchWeaponBuffer -= Time.deltaTime;
		}
		else
		{
			_switchWeaponBuffer = 0f;
		}
		if (_attackBlockBuffer > 0f)
		{
			_attackBlockBuffer -= Time.deltaTime;
		}
		else
		{
			_attackBlockBuffer = 0f;
		}
		if (_parryChanceBuffer < 0f)
		{
			_parryChanceBuffer = 0f;
		}
		if ((bool)_pCasting._currentCastSkill || _player._currentPlayerAction == PlayerAction.CASTING)
		{
			_localBlockingInput = false;
		}
		if (_player._currentPlayerCondition == PlayerCondition.DEAD)
		{
			_blockControlBuffer = 0f;
			_withinAttackQueue = false;
		}
		else if (_player._currentPlayerAction == PlayerAction.ATTACKING || _isChargingWeapon)
		{
			float attackMovementSpeedModifier = ((!_isChargingWeapon) ? _currentScriptableWeaponType._weaponData._movSpeedDecrease : _setWepTypeCombatParams._chargeMovSpeedDecrease);
			_pMove._attackMovementSpeedModifier = attackMovementSpeedModifier;
			_pMove._currentRevUpSpeed = 0f;
		}
		else
		{
			_pMove._attackMovementSpeedModifier = 0f;
		}
		_commandBuffer = _currentCommandTimer > 0f;
		if (_currentCommandTimer > 0f)
		{
			_currentCommandTimer -= 0.1f * Time.deltaTime;
		}
		else
		{
			_currentCommandTimer = 0f;
		}
		if (_actionBuffer > 0f)
		{
			_actionBuffer -= Time.deltaTime;
		}
		else
		{
			_actionBuffer = 0f;
			if (!_isAirAttacked && (_player._currentPlayerAction == PlayerAction.ATTACKING || _player._currentPlayerAction == PlayerAction.BLOCKING))
			{
				_player._currentPlayerAction = PlayerAction.IDLE;
			}
		}
		if (_pMove._airTime <= 0f)
		{
			_hittedWall = false;
		}
	}

	[ClientCallback]
	private void Client_Handle_QuickSwapWeaponControl()
	{
		if (NetworkClient.active && !_weaponSwapQueued && !GamepadInput.GetButton(GamepadButton.LeftShoulder) && !GamepadInput.GetButton(GamepadButton.RightShoulder) && IsQuickSwapInput)
		{
			Init_QuickSwapWeapon();
		}
	}

	public bool CanQuickSwapWeapon(bool _hasInput)
	{
		if (DialogManager._current._isDialogEnabled && DialogManager._current._currentDialogUIPrompt == DialogUIPrompt.VANITY_MIRROR)
		{
			return false;
		}
		if (SettingsManager._current._isOpen)
		{
			return false;
		}
		if (SettingsManager._current._gamepadInputBuffer > 0f)
		{
			return false;
		}
		if (_player._bufferingStatus)
		{
			return false;
		}
		if (_player._inChat)
		{
			return false;
		}
		if (_commandBuffer)
		{
			return false;
		}
		if (_pCasting._castEndBuffer > 0f)
		{
			return false;
		}
		if (_localAirAttackInput || _isAirAttacked)
		{
			return false;
		}
		if (_isDisarmed)
		{
			return false;
		}
		if (_player._currentPlayerCondition == PlayerCondition.DEAD)
		{
			if (_hasInput)
			{
				ErrorPromptTextManager.current.Init_ErrorPrompt("You're Dead");
			}
			return false;
		}
		if (_pMove._movementLockType != MovementLockType.NONE)
		{
			if (_hasInput)
			{
				ErrorPromptTextManager.current.Init_ErrorPrompt("Not Ready");
			}
			return false;
		}
		if (_pCasting._isCasting || _player._currentPlayerAction == PlayerAction.CASTING || (bool)_pCasting._currentCastSkill)
		{
			if (_hasInput)
			{
				ErrorPromptTextManager.current.Init_ErrorPrompt("Not Ready");
			}
			return false;
		}
		if (_blockControlBuffer > 0f)
		{
			if (_hasInput)
			{
				ErrorPromptTextManager.current.Init_ErrorPrompt("Not Ready");
			}
			return false;
		}
		if (_lockControlBuffer > 0f)
		{
			if (_hasInput)
			{
				ErrorPromptTextManager.current.Init_ErrorPrompt("Not Ready");
			}
			return false;
		}
		return true;
	}

	[ClientCallback]
	public void Init_QuickSwapWeapon()
	{
		if (NetworkClient.active && CanQuickSwapWeapon(_hasInput: true))
		{
			StartCoroutine(Queue_WeaponSwap());
		}
	}

	private IEnumerator Queue_WeaponSwap()
	{
		_weaponSwapQueued = true;
		_lockControlBuffer = 0.1f;
		yield return new WaitForSeconds(0.02f);
		while (_player._currentPlayerAction == PlayerAction.ATTACKING)
		{
			yield return null;
		}
		while (_localAirAttackInput)
		{
			yield return null;
		}
		while (_isChargingWeapon)
		{
			yield return null;
		}
		while (_actionBuffer > 0f)
		{
			yield return null;
		}
		yield return new WaitForSeconds(0.04f);
		Cmd_QuickSwapWeapon();
		_lockControlBuffer = 0.3f;
		_currentSwingState = 0;
		_pSound._aSrcPlayerUI.PlayOneShot(_pSound._weaponQuickSwitchSfx, 0.15f);
		ToolTipManager._current.Disable_AllTooltips();
		ActionBarManager._current.Init_RowScaleEffect();
		yield return new WaitForSeconds(0.04f);
		if (!_localBlockingInput && !_isBlocking)
		{
			_pVisual.Send_CrossFadeAnim("Empty", 0f, 3, LatencyCheck.IGNORE_LATENCY);
		}
		if (_player._latency > 50)
		{
			_switchWeaponBuffer = 0.25f;
		}
		_currentCommandTimer = _quickSwapBufferTimer;
		_weaponSwapQueued = false;
		_actionBuffer += 0.3f;
	}

	private bool Check_CanCommandSwap()
	{
		if (_player._currentPlayerCondition == PlayerCondition.DEAD)
		{
			return false;
		}
		if (_pCasting._isCasting || (bool)_pCasting._currentCastSkill)
		{
			return false;
		}
		if (_player._bufferingStatus)
		{
			return false;
		}
		if (_isChargingWeapon)
		{
			return false;
		}
		return true;
	}

	[Command]
	private void Cmd_QuickSwapWeapon()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void PlayerCombat::Cmd_QuickSwapWeapon()", 1155479679, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[Command]
	public void Cmd_LockWeapon()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void PlayerCombat::Cmd_LockWeapon()", -1969178106, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[Command]
	public void Cmd_SheatheWeapon(WeaponSheatheCondition _sheatheType)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		GeneratedNetworkCode._Write_WeaponSheatheCondition(writer, _sheatheType);
		SendCommandInternal("System.Void PlayerCombat::Cmd_SheatheWeapon(WeaponSheatheCondition)", -2123711192, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	public void OnWeaponSheatheChange(WeaponSheatheCondition _oldValue, WeaponSheatheCondition _newValue)
	{
		StartCoroutine(Queue_WeaponSheatheCondition());
	}

	private IEnumerator Queue_WeaponSheatheCondition()
	{
		do
		{
			yield return null;
		}
		while (!_pVisual._playerRaceModel);
		switch (_currentSheathCondition)
		{
		case WeaponSheatheCondition.TAKEN_OUT:
			if (!_isUnarmed)
			{
				string weaponHoldClip = _currentScriptableWeaponType._weaponData._weaponHoldClip;
				_pVisual.Local_CrossFadeAnim(weaponHoldClip ?? "", 0f, 2);
			}
			if ((bool)_pVisual._playerRaceModel)
			{
				_pVisual._playerRaceModel.Set_EyeCondition(EyeCondition.Pissed, 0.95f);
			}
			break;
		case WeaponSheatheCondition.PUT_AWAY:
			if (_pSound._aSrcGeneral.enabled)
			{
				_pSound._aSrcGeneral.PlayOneShot(_pSound._generalSounds[6], 0.45f);
			}
			break;
		}
	}

	[ClientCallback]
	private void Client_Handle_WeaponSheatheControl()
	{
		if (!NetworkClient.active || !_canSheatheInput())
		{
			return;
		}
		bool flag = Input.GetKeyDown(InputControlManager.current._sheatheWeapon) || GamepadInput.GetButton(GamepadButton.LeftStick);
		bool flag2 = (Input.GetKey(InputControlManager.current._block) || GamepadInput.GetButton(GamepadButton.LeftTrigger)) && _statusEntity._currentStamina > 0 && !_player._inUI;
		bool flag3 = (Input.GetKey(InputControlManager.current._attack) || (Input.GetKey(InputControlManager.current._chargeAttack) && _setWepTypeCombatParams._allowChargeAttack)) && !_player._inUI;
		bool flag4 = GamepadInput.GetButton(GamepadButton.RightTrigger) && !_player._inUI;
		if (_currentSheathCondition == WeaponSheatheCondition.PUT_AWAY && (flag3 | flag | flag2 | flag4))
		{
			if (_pMove._currentMovementAction != MovementAction.CLIMB && _pMove._currentMovementAction != MovementAction.LEDGEGRAB)
			{
				Cmd_SheatheWeapon(WeaponSheatheCondition.TAKEN_OUT);
				_currentCommandTimer = _commandBufferTimer;
				if (flag3 && (!Input.GetKey(InputControlManager.current._chargeAttack) || !_setWepTypeCombatParams._allowChargeAttack))
				{
					StartCoroutine(Init_AttackQueue());
				}
			}
		}
		else if ((_currentSheathCondition == WeaponSheatheCondition.TAKEN_OUT) & flag)
		{
			Cmd_SheatheWeapon(WeaponSheatheCondition.PUT_AWAY);
			_currentCommandTimer = _commandBufferTimer;
		}
		bool _canSheatheInput()
		{
			if (_focusWindowBuffer > 0f)
			{
				return false;
			}
			if (_isChargingWeapon)
			{
				return false;
			}
			if (_isBlocking || _localBlockingInput)
			{
				return false;
			}
			if (_commandBuffer)
			{
				return false;
			}
			if (_player._currentPlayerAction != PlayerAction.IDLE)
			{
				return false;
			}
			if (_pMove._movementLockType != MovementLockType.NONE)
			{
				return false;
			}
			if (_player._inChat)
			{
				return false;
			}
			if (_isUnarmed && !_equippedShield)
			{
				return false;
			}
			if (_lockControlBuffer > 0f)
			{
				return false;
			}
			if (_lockSheatheWeaponControl)
			{
				return false;
			}
			return true;
		}
	}

	[Command]
	public void Cmd_InterruptCombat(bool _resetAnimation)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteBool(_resetAnimation);
		SendCommandInternal("System.Void PlayerCombat::Cmd_InterruptCombat(System.Boolean)", -821965423, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ServerCallback]
	public void Server_InterruptCombat(bool _resetAnimation)
	{
		if (NetworkServer.active)
		{
			Rpc_WeaponChargeDisplay(_isTrue: false);
			Init_CombatInterruption(_resetAnimation);
			Rpc_InterruptCombat(_resetAnimation);
		}
	}

	[ClientRpc]
	private void Rpc_InterruptCombat(bool _resetAnimation)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteBool(_resetAnimation);
		SendRPCInternal("System.Void PlayerCombat::Rpc_InterruptCombat(System.Boolean)", 255629884, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	private void Init_CombatInterruption(bool _resetAnimation)
	{
		if (_resetAnimation)
		{
			_pVisual.Send_CrossFadeAnim("Empty", 0f, 3, LatencyCheck.IGNORE_LATENCY);
		}
		CombatInterrupt();
	}

	public void CombatInterrupt()
	{
		_player._currentPlayerAction = PlayerAction.IDLE;
		Server_DisableHitboxObjects();
		_isAirAttacked = false;
		_localAirAttackInput = false;
		_isChargingWeapon = false;
		_currentSwingState = 0;
		_currentChargeBuffer = 0f;
		_pMove._attackForce = 0f;
	}

	[ClientCallback]
	private void Client_Handle_CombatControl()
	{
		if (NetworkClient.active)
		{
			if (_isChargingWeapon && (_player._inUI || _player._inChat))
			{
				Release_WeaponCharge();
			}
			if (_localBlockingInput && _isBlocking && (_player._inUI || _player._inChat))
			{
				Cmd_ApplyBlockCondition(_isTrue: false);
				_blockControlBuffer = 0f;
				_localBlockingInput = false;
			}
			if (_player._currentPlayerCondition != PlayerCondition.ACTIVE)
			{
				_blockControlBuffer = 0f;
				_localBlockingInput = false;
			}
			if (!_canCombatControl())
			{
				_currentChargeBuffer = 0f;
				return;
			}
			Handle_ChargeAttackControl();
			Handle_BlockControl();
			Handle_AttackControl();
		}
		void Handle_AttackControl()
		{
			bool flag = InputControlManager.current._setGamepadType != GamepadType.Keyboard && InputControlManager.current._gamepadMouseCursorBuffer <= 0f;
			bool num = ((Input.GetKey(InputControlManager.current._attack) && !flag) || (flag && GamepadInput.GetButton(GamepadButton.RightTrigger))) && !CanJumpAttack();
			bool flag2 = ((Input.GetKeyDown(InputControlManager.current._attack) && !flag) || (flag && GamepadInput.GetButtonDown(GamepadButton.RightTrigger))) && CanJumpAttack();
			if (num)
			{
				StartCoroutine(Init_AttackQueue());
			}
			else if (flag2)
			{
				StartCoroutine(Init_AttackQueue());
			}
		}
		void Handle_BlockControl()
		{
			if (!_player._isHurt && !(_currentCommandTimer > 0.08f))
			{
				int currentStamina = _statusEntity._currentStamina;
				if (_blockControlBuffer <= 0f)
				{
					_blockControlBuffer = 0f;
					if (_localBlockingInput)
					{
						_player._currentPlayerAction = PlayerAction.BLOCKING;
					}
					if (IsBlockInput)
					{
						if (currentStamina <= 0)
						{
							ErrorPromptTextManager.current.Init_ErrorPrompt("Fatigued");
						}
						else if (!(_blockControlBuffer > 0f) && !_localBlockingInput && !_isBlocking)
						{
							Cmd_ApplyBlockCondition(_isTrue: true);
							Init_BlockBufferParams();
							_localBlockingInput = true;
						}
					}
					else if (_localBlockingInput && !_statusEntity._isDamageBuffer)
					{
						Cmd_ApplyBlockCondition(_isTrue: false);
						Init_BlockBufferParams();
						_localBlockingInput = false;
					}
				}
				else
				{
					_blockControlBuffer -= Time.deltaTime;
				}
			}
		}
		void Handle_ChargeAttackControl()
		{
			if (!(_actionBuffer > 0f) && !(_lockControlBuffer > 0f) && !(_chargeAttackCooldown > 0f) && _pMove._movementLockType == MovementLockType.NONE && !_localBlockingInput && !_isBlocking)
			{
				if (_isDisarmed)
				{
					_currentChargeBuffer = 0f;
				}
				else
				{
					string weaponTypeChargeHandle = _currentScriptableWeaponType._weaponData._weaponTypeChargeHandle;
					float smoothBetweenClips = _currentScriptableWeaponType._weaponData._smoothBetweenClips;
					if (IsChargeAttackInput)
					{
						if (!_isChargedWeapon)
						{
							if (_setWepTypeCombatParams._allowChargeAttack)
							{
								_currentChargeBuffer += Time.deltaTime;
							}
							else
							{
								_currentChargeBuffer = 0f;
							}
							if (_currentChargeBuffer >= _setWepTypeCombatParams._chargeTime)
							{
								_currentChargeBuffer = 0f;
								_isChargedWeapon = true;
								Cmd_WeaponChargeDisplay(_isTrue: true);
							}
						}
						else
						{
							_currentChargeBuffer = 0f;
						}
						if (!_isChargingWeapon)
						{
							_pVisual.Send_CrossFadeAnim(weaponTypeChargeHandle ?? "", smoothBetweenClips, 3, LatencyCheck.CHECK_LATENCY);
							_isChargedWeapon = false;
							_isChargingWeapon = true;
						}
					}
					else if (_isChargingWeapon)
					{
						Release_WeaponCharge();
					}
				}
			}
		}
		void Init_BlockBufferParams()
		{
			_blockControlBuffer = 0.12f;
			_attackBlockBuffer = 0.08f;
		}
		bool _canCombatControl()
		{
			if (_player._currentPlayerAction == PlayerAction.CASTING || (bool)_pCasting._currentCastSkill)
			{
				return false;
			}
			if (_player._inChat || _player._inUI || _player._bufferingStatus)
			{
				return false;
			}
			if (_focusWindowBuffer > 0f || _lockControlBuffer > 0f)
			{
				return false;
			}
			if (Input.GetKey(InputControlManager.current._sit))
			{
				return false;
			}
			switch (_pMove._currentMovementAction)
			{
			case MovementAction.CLIMB:
				return false;
			case MovementAction.LEDGEGRAB:
				return false;
			default:
				if (_player._latency >= 200)
				{
					if (_pMove._currentMovementAction == MovementAction.DASH)
					{
						return false;
					}
					if (_pMove._currentMovementAction == MovementAction.JUMPDASH)
					{
						return false;
					}
					if (_commandBuffer)
					{
						return false;
					}
				}
				else if (_currentSheathCondition != WeaponSheatheCondition.TAKEN_OUT && !_isUnarmed)
				{
					return false;
				}
				if (_switchWeaponBuffer > 0f)
				{
					return false;
				}
				return true;
			}
		}
	}

	private IEnumerator Init_AttackQueue()
	{
		if (!_withinAttackQueue)
		{
			_withinAttackQueue = true;
			while ((bool)_equippedWeapon && _currentSheathCondition == WeaponSheatheCondition.PUT_AWAY)
			{
				yield return null;
			}
			Init_Attack();
			_withinAttackQueue = false;
		}
	}

	private void OnBlockingChange(bool _oldBool, bool _newBool)
	{
		if (!Player._mainPlayer || Player._mainPlayer._bufferingStatus)
		{
			return;
		}
		if (_newBool)
		{
			_pSound._aSrcGeneral.PlayOneShot(_pSound._generalSounds[12], 0.35f);
			if ((bool)_equippedShield && _currentScriptableWeaponType._weaponHandedness == WeaponHandedness.One_Handed)
			{
				_pVisual.Local_CrossFadeAnim("Block_Shield", 0f, 3);
			}
			else
			{
				_pVisual.Local_CrossFadeAnim(_currentScriptableWeaponType._weaponData._weaponBlockAnimation ?? "", 0f, 3);
			}
			_pVisual._blockOrbEffect.transform.localScale = Vector3.one * 12.5f;
			_pVisual._blockOrbEffect.setScale = new Vector3(6.5f, 6.5f, 6.5f);
			_pVisual._preventRotationLerp = false;
			if (base.isLocalPlayer)
			{
				_pVisual.Cmd_ResetSpinPlayerModel();
			}
		}
		else
		{
			_pVisual.Local_CrossFadeAnim("Empty", 0f, 3);
			_pVisual._blockOrbEffect.setScale = Vector3.zero;
		}
	}

	[ServerCallback]
	private void Server_Handle_BlockCondition()
	{
		if (!NetworkServer.active)
		{
			return;
		}
		if (_parryChanceBuffer > 0f)
		{
			_parryChanceBuffer -= Time.deltaTime;
		}
		if (_player._bufferingStatus || _pMove._currentMovementAction == MovementAction.LEDGEGRAB)
		{
			_staminaReductionRate = 0f;
			Network_isBlocking = false;
			return;
		}
		if (!_isBlocking)
		{
			_staminaReductionRate = 0f;
			return;
		}
		Server_DisableHitboxObjects();
		if ((bool)_pCasting._currentCastSkill || _player._currentPlayerAction == PlayerAction.CASTING)
		{
			_staminaReductionRate = 0f;
			Network_isBlocking = false;
			return;
		}
		if (_staminaReductionRate < GameManager._current._statLogics._blockStaminaReductionRate)
		{
			_staminaReductionRate += Time.deltaTime;
		}
		else if (_staminaReductionRate >= GameManager._current._statLogics._blockStaminaReductionRate)
		{
			StatusEntity statusEntity = _statusEntity;
			statusEntity.Network_currentStamina = statusEntity._currentStamina - 1;
			_staminaReductionRate = 0f;
		}
		if (_statusEntity._currentStamina <= 0)
		{
			Target_CancelBlock();
			Network_isBlocking = false;
		}
		if (base.isServerOnly)
		{
			if ((bool)_equippedShield && _currentScriptableWeaponType._weaponHandedness == WeaponHandedness.One_Handed)
			{
				_pVisual.Send_CrossFadeAnim("Block_Shield", 0f, 3, LatencyCheck.IGNORE_LATENCY);
			}
			else
			{
				_pVisual.Send_CrossFadeAnim("Block_NoShield", 0f, 3, LatencyCheck.IGNORE_LATENCY);
			}
		}
	}

	[Command]
	private void Cmd_ApplyBlockCondition(bool _isTrue)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteBool(_isTrue);
		SendCommandInternal("System.Void PlayerCombat::Cmd_ApplyBlockCondition(System.Boolean)", 1683590518, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	public void Target_CancelBlock()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendTargetRPCInternal(null, "System.Void PlayerCombat::Target_CancelBlock()", -1289386945, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientCallback]
	private bool CanJumpAttack()
	{
		if (!NetworkClient.active)
		{
			return default;
		}
		if (!_setWepTypeCombatParams._allowJumpAttack)
		{
			return false;
		}
		if (_weaponSwapQueued)
		{
			return false;
		}
		if (_hittedWall)
		{
			return false;
		}
		switch (_pMove._currentMovementAction)
		{
		case MovementAction.JUMP:
			return true;
		case MovementAction.FALL:
			return true;
		case MovementAction.JUMPDASH:
			return true;
		default:
			if (_pMove._airTime >= 1.5f)
			{
				return true;
			}
			return false;
		}
	}

	[ClientCallback]
	private void Init_Attack()
	{
		if (NetworkClient.active && AbleToInitAttack())
		{
			if (CanJumpAttack())
			{
				Init_JumpAttack();
			}
			else
			{
				Init_GroundAttack();
			}
		}
		bool AbleToInitAttack()
		{
			if (_player._isHurt || _isBlocking || Input.GetKey(InputControlManager.current._block))
			{
				return false;
			}
			if (_player._inChat || _player._inUI)
			{
				return false;
			}
			if (_actionBuffer > 0f)
			{
				return false;
			}
			if (_attackControlBuffer > 0f)
			{
				return false;
			}
			if (_chargeAttackCooldown > 0f)
			{
				return false;
			}
			if (_statusEntity._isDamageBuffer)
			{
				return false;
			}
			if (_pCasting._isQueuedSkill)
			{
				return false;
			}
			if (IsChargeAttackInput)
			{
				return false;
			}
			if (_isAirAttacked)
			{
				return false;
			}
			return true;
		}
		void Init_GroundAttack()
		{
			if ((_pMove.RayGroundCheck() || !_setWepTypeCombatParams._allowJumpAttack) && !(_groundAttackControlBuffer > 0f) && !(_attackBlockBuffer > 0f))
			{
				string text = $"{_currentScriptableWeaponType._weaponData._weaponTypeAttackClipHandle}{_currentSwingState + 1}";
				float attackActionBuffer = _currentScriptableWeaponType._weaponData._attackActionBuffer;
				float smoothBetweenClips = _currentScriptableWeaponType._weaponData._smoothBetweenClips;
				_pVisual.Send_CrossFadeAnim("Empty", 0f, 10, LatencyCheck.IGNORE_LATENCY);
				_pVisual.Send_CrossFadeAnim(text ?? "", smoothBetweenClips, 3, LatencyCheck.CHECK_LATENCY);
				_currentSwingState++;
				if (_currentSwingState > _maxSwingState)
				{
					_currentSwingState = 0;
				}
				Init_ActionBuffer(attackActionBuffer);
			}
		}
		void Init_JumpAttack()
		{
			if (CanJumpAttack() && !_isAirAttacked && !_localAirAttackInput && !_localBlockingInput)
			{
				_pMove._lockControlMidair = true;
				_pMove._movementLockType = MovementLockType.SOFT_LOCK;
				if (_setWepTypeCombatParams._persistantDamageRate > 0f)
				{
					_pVisual.Cmd_JumpAttackEffect();
					_pVisual.Init_JumpAttackEffect();
				}
				Invoke("Init_PlayerJumpAttack", _setWepTypeCombatParams._timeBeforeJumpAttack);
				StartCoroutine(Init_AirAttack());
				_localAirAttackInput = true;
				_jumpAttackBuffer = 0.2f;
			}
		}
	}

	[ClientCallback]
	private void Init_PlayerJumpAttack()
	{
		if (NetworkClient.active)
		{
			_pMove.Init_Jump(_setWepTypeCombatParams._JumpAttackJumpForce, _setWepTypeCombatParams._JumpAttackFowardForce, 0f, _setWepTypeCombatParams._JumpAttackJumpGravityMultiply, _useAnim: true);
			_pVisual.Send_CrossFadeAnim("Jump", 0f, 0, LatencyCheck.IGNORE_LATENCY);
		}
	}

	private IEnumerator Init_AirAttack()
	{
		if (_hittedWall)
		{
			yield break;
		}
		_detectGroundFromAirAttack = false;
		_pMove._ctrlJumpForce = 0f;
		_blockControlBuffer = 0.2f;
		yield return new WaitForSeconds(0.06f);
		_pVisual.Send_CrossFadeAnim("Empty", 0f, 10, LatencyCheck.IGNORE_LATENCY);
		_pVisual.Send_CrossFadeAnim(_setWepTypeCombatParams._weaponTypeJumpAttackClipHandle ?? "", 0f, 3, LatencyCheck.IGNORE_LATENCY);
		yield return new WaitForSeconds(0.12f);
		_detectGroundFromAirAttack = true;
		while (!_pMove.RayGroundCheck() && _detectGroundFromAirAttack)
		{
			Handle_JumpAttack_WallDetection();
			_pMove._ctrlJumpForce = 0f;
			_pMove._currentJumps = _pMove._maxJumps;
			if (_localBlockingInput || _player._knockbackBuffer > 0f)
			{
				if (_localBlockingInput && !base.gameObject.scene.GetPhysicsScene().Raycast(base.transform.position, -base.transform.up, 80.5f, CastLayers.GroundMask))
				{
					int value = (int)((float)_pStats._statStruct._maxStamina * 0.4f);
					_statusEntity.Cmd_SubtractStamina(value);
				}
				End_AirAttack();
				_detectGroundFromAirAttack = false;
				yield break;
			}
			yield return null;
		}
		while (!_pMove.RayGroundCheck() && _hittedWall)
		{
			yield return null;
		}
		if (_detectGroundFromAirAttack)
		{
			End_AirAttack();
		}
		yield return new WaitForSeconds(0.1f);
		_detectGroundFromAirAttack = false;
		_hittedWall = false;
		void Handle_JumpAttack_WallDetection()
		{
			if (base.gameObject.scene.GetPhysicsScene().Raycast(_airAttackCastPos.position, base.transform.forward, 4.25f, CastLayers.GroundMask) && _currentScriptableWeaponType._weaponData._canWallKnockback)
			{
				_pMove._disableDoubleJumpAnim = true;
				if (IsInvoking("Init_PlayerJumpAttack"))
				{
					CancelInvoke("Init_PlayerJumpAttack");
				}
				_pVisual.Send_CrossFadeAnim("JumpAtk_HitWall", 0.01f, 3, LatencyCheck.IGNORE_LATENCY);
				_pVisual.Send_CrossFadeAnim("Fall", 0.01f, 0, LatencyCheck.IGNORE_LATENCY);
				_pMove._attackForce = 0f;
				_pMove._jumpForwardForce = 0f;
				_pMove.Init_Jump(25.5f, -20.5f, 0f, 45.5f, _useAnim: false);
				Init_ActionBuffer(_setWepTypeCombatParams._jumpAttackActionBuffer);
				_pMove._lockControlMidair = true;
				_isAirAttacked = false;
				_localAirAttackInput = false;
				_hittedWall = true;
				Cmd_InterruptCombat(_resetAnimation: false);
				_detectGroundFromAirAttack = false;
			}
		}
	}

	[ClientCallback]
	public void End_AirAttack()
	{
		if (!NetworkClient.active)
		{
			return;
		}
		if (_setWepTypeCombatParams._weaponTypeJumpLandClipHandle != "Empty" && _player._knockbackBuffer <= 0f)
		{
			if (!_localBlockingInput && !_isBlocking && !string.IsNullOrWhiteSpace(_setWepTypeCombatParams._weaponTypeJumpLandClipHandle))
			{
				_pMove.Apply_MovementLockCondition(MovementLockType.SOFT_LOCK, _setWepTypeCombatParams._jumpLandDelayTime);
				_pVisual.Send_CrossFadeAnim(_setWepTypeCombatParams._weaponTypeJumpLandClipHandle ?? "", 0f, 3, LatencyCheck.IGNORE_LATENCY);
			}
			else if (!_isBlocking)
			{
				_pVisual.Send_CrossFadeAnim("Empty", 0f, 3, LatencyCheck.IGNORE_LATENCY);
			}
		}
		Init_ActionBuffer(_setWepTypeCombatParams._jumpAttackActionBuffer);
		_localAirAttackInput = false;
		_isAirAttacked = false;
	}

	[Command]
	public void Cmd_WeaponChargeDisplay(bool _isTrue)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteBool(_isTrue);
		SendCommandInternal("System.Void PlayerCombat::Cmd_WeaponChargeDisplay(System.Boolean)", 1108601356, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	private void Rpc_WeaponChargeDisplay(bool _isTrue)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteBool(_isTrue);
		SendRPCInternal("System.Void PlayerCombat::Rpc_WeaponChargeDisplay(System.Boolean)", 426928439, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	[ClientCallback]
	private void Init_ChargeAttackRotationLock()
	{
		if (NetworkClient.active)
		{
			_pMove._disableStrafeBuffer = _setWepTypeCombatParams._chargeAttackStrafeRotationLock;
		}
	}

	[ClientCallback]
	private void Release_WeaponCharge()
	{
		if (!NetworkClient.active)
		{
			return;
		}
		string text = $"{_currentScriptableWeaponType._weaponData._weaponTypeAttackClipHandle}{_currentSwingState + 1}";
		float attackActionBuffer = _currentScriptableWeaponType._weaponData._attackActionBuffer;
		float smoothBetweenClips = _currentScriptableWeaponType._weaponData._smoothBetweenClips;
		if (!_isChargedWeapon)
		{
			_pVisual.Send_CrossFadeAnim(text ?? "", smoothBetweenClips, 3, LatencyCheck.CHECK_LATENCY);
			_currentSwingState++;
			if (_currentSwingState > _maxSwingState)
			{
				_currentSwingState = 0;
			}
			Init_ActionBuffer(attackActionBuffer);
		}
		else
		{
			text = _currentScriptableWeaponType._weaponData._weaponTypeChargeAttackHandle ?? "";
			attackActionBuffer = _setWepTypeCombatParams._chargeAttackActionBuffer;
			Invoke("Init_ChargeAttackRotationLock", _setWepTypeCombatParams._timeBeforeChargeAttackStrafeRotationLock);
			_pVisual.Send_CrossFadeAnim("Empty", 0f, 10, LatencyCheck.CHECK_LATENCY);
			_pVisual.Send_CrossFadeAnim(text, smoothBetweenClips, 3, LatencyCheck.CHECK_LATENCY);
			Init_ActionBuffer(attackActionBuffer);
			Cmd_WeaponChargeDisplay(_isTrue: false);
		}
		_chargeAttackCooldown = 0.8f;
		_currentChargeBuffer = 0f;
		_isChargingWeapon = false;
	}

	public void Init_ShootProjectile(int _swingState)
	{
		if (!_equippedWeapon || !_equippedWeapon._weaponProjectileSet || _player._currentPlayerCondition != PlayerCondition.ACTIVE || _statusEntity._currentHealth <= 0 || (base.isClientOnly && !base.isLocalPlayer && (Player._mainPlayer.Network_playerMapInstance != _player.Network_playerMapInstance || _player.Network_playerMapInstance == null)))
		{
			return;
		}
		ScriptableWeaponProjectileSet weaponProjectileSet = _equippedWeapon._weaponProjectileSet;
		if (weaponProjectileSet._projectileObjects[_swingState] == null)
		{
			Debug.LogError("No weapon projectile object found on the scriptable weapon. Check it to make sure it does.");
			return;
		}
		GameObject gameObject = PoolManager._current.GenerateObject(weaponProjectileSet._projectileObjects[_swingState], string.Empty, 0f);
		ProjectileObject component = gameObject.GetComponent<ProjectileObject>();
		component._parentTransform = base.transform;
		gameObject.transform.SetParent(base.gameObject.transform);
		gameObject.transform.SetParent(null);
		if (component._applyRotationToParent)
		{
			Vector3 eulerAngles = base.transform.rotation.eulerAngles;
			eulerAngles.x = 0f;
			Quaternion rotation = Quaternion.Euler(eulerAngles);
			if (component._ignoreYAxisRotation)
			{
				gameObject.transform.SetPositionAndRotation(_rangeWeaponBulletPos.position + component._offsetPosition, rotation);
			}
			else
			{
				gameObject.transform.SetPositionAndRotation(_rangeWeaponBulletPos.position + component._offsetPosition, _rangeWeaponBulletPos.rotation);
			}
		}
		else
		{
			gameObject.transform.SetPositionAndRotation(_rangeWeaponBulletPos.position, _rangeWeaponBulletPos.rotation);
		}
		CombatCollider componentInChildren = gameObject.GetComponentInChildren<CombatCollider>();
		if ((bool)_equippedWeapon._combatElement)
		{
			componentInChildren._combatElement = _equippedWeapon._combatElement;
		}
		componentInChildren._parentObject = base.gameObject;
		componentInChildren._scriptWeapon = _equippedWeapon;
		componentInChildren._dmgWeight = _equippedWeapon.weaponType._damageWeight;
		if (base.isServer)
		{
			_statusEntity.Init_OnAttackCallback();
		}
	}

	[ServerCallback]
	public void Init_CreepAttackQueue(Creep _creep)
	{
		if (NetworkServer.active)
		{
			_setCreepQueue._creepAttacker = _creep;
			_setCreepQueue._bufferTime = Random.Range(0.008592f, 0.285f);
		}
	}

	public override bool Weaved()
	{
		return true;
	}

	protected void UserCode_Cmd_ResetHitboxes()
	{
		Server_DisableHitboxObjects();
	}

	protected static void InvokeUserCode_Cmd_ResetHitboxes(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command Cmd_ResetHitboxes called on client.");
		}
		else
		{
			((PlayerCombat)obj).UserCode_Cmd_ResetHitboxes();
		}
	}

	protected void UserCode_Cmd_QuickSwapWeapon()
	{
		if (Check_CanCommandSwap())
		{
			_player.Add_PacketCount();
			Network_isUsingAltWeapon = !_isUsingAltWeapon;
			_pStats.Apply_StatCalculations();
		}
	}

	protected static void InvokeUserCode_Cmd_QuickSwapWeapon(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command Cmd_QuickSwapWeapon called on client.");
		}
		else
		{
			((PlayerCombat)obj).UserCode_Cmd_QuickSwapWeapon();
		}
	}

	protected void UserCode_Cmd_LockWeapon()
	{
		if (Check_CanCommandSwap())
		{
			_player.Add_PacketCount();
			Network_lockWeapon = !_lockWeapon;
			_pStats.Apply_StatCalculations();
		}
	}

	protected static void InvokeUserCode_Cmd_LockWeapon(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command Cmd_LockWeapon called on client.");
		}
		else
		{
			((PlayerCombat)obj).UserCode_Cmd_LockWeapon();
		}
	}

	protected void UserCode_Cmd_SheatheWeapon__WeaponSheatheCondition(WeaponSheatheCondition _sheatheType)
	{
		Network_currentSheathCondition = _sheatheType;
	}

	protected static void InvokeUserCode_Cmd_SheatheWeapon__WeaponSheatheCondition(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command Cmd_SheatheWeapon called on client.");
		}
		else
		{
			((PlayerCombat)obj).UserCode_Cmd_SheatheWeapon__WeaponSheatheCondition(GeneratedNetworkCode._Read_WeaponSheatheCondition(reader));
		}
	}

	protected void UserCode_Cmd_InterruptCombat__Boolean(bool _resetAnimation)
	{
		Server_InterruptCombat(_resetAnimation);
	}

	protected static void InvokeUserCode_Cmd_InterruptCombat__Boolean(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command Cmd_InterruptCombat called on client.");
		}
		else
		{
			((PlayerCombat)obj).UserCode_Cmd_InterruptCombat__Boolean(reader.ReadBool());
		}
	}

	protected void UserCode_Rpc_InterruptCombat__Boolean(bool _resetAnimation)
	{
		Init_CombatInterruption(_resetAnimation);
	}

	protected static void InvokeUserCode_Rpc_InterruptCombat__Boolean(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC Rpc_InterruptCombat called on server.");
		}
		else
		{
			((PlayerCombat)obj).UserCode_Rpc_InterruptCombat__Boolean(reader.ReadBool());
		}
	}

	protected void UserCode_Cmd_ApplyBlockCondition__Boolean(bool _isTrue)
	{
		if (_isTrue)
		{
			Server_InterruptCombat(_resetAnimation: false);
			Server_DisableHitboxObjects();
			_parryChanceBuffer = GameManager._current._statLogics._baseParryChanceBuffer + _parryChanceModifier;
		}
		else
		{
			Server_DisableHitboxObjects();
		}
		Network_isBlocking = _isTrue;
	}

	protected static void InvokeUserCode_Cmd_ApplyBlockCondition__Boolean(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command Cmd_ApplyBlockCondition called on client.");
		}
		else
		{
			((PlayerCombat)obj).UserCode_Cmd_ApplyBlockCondition__Boolean(reader.ReadBool());
		}
	}

	protected void UserCode_Target_CancelBlock()
	{
		_localBlockingInput = false;
		_blockControlBuffer = 0.2f;
	}

	protected static void InvokeUserCode_Target_CancelBlock(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC Target_CancelBlock called on server.");
		}
		else
		{
			((PlayerCombat)obj).UserCode_Target_CancelBlock();
		}
	}

	protected void UserCode_Cmd_WeaponChargeDisplay__Boolean(bool _isTrue)
	{
		_isChargingWeapon = _isTrue;
		_pVisual._raceModelAnimManager.Init_ChargeEffect(_isTrue);
		Rpc_WeaponChargeDisplay(_isTrue);
	}

	protected static void InvokeUserCode_Cmd_WeaponChargeDisplay__Boolean(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command Cmd_WeaponChargeDisplay called on client.");
		}
		else
		{
			((PlayerCombat)obj).UserCode_Cmd_WeaponChargeDisplay__Boolean(reader.ReadBool());
		}
	}

	protected void UserCode_Rpc_WeaponChargeDisplay__Boolean(bool _isTrue)
	{
		PlayerModelAnimManager componentInChildren = GetComponentInChildren<PlayerModelAnimManager>();
		if ((bool)componentInChildren && (bool)_pSound && (bool)_equippedWeapon && (bool)_equippedWeapon.weaponType)
		{
			if ((bool)componentInChildren)
			{
				componentInChildren.Init_ChargeEffect(_isTrue);
			}
			if (!base.isLocalPlayer)
			{
				_isChargingWeapon = _isTrue;
			}
			if (_pSound._aSrcGeneral.isActiveAndEnabled && (bool)_currentScriptableWeaponType && _isTrue)
			{
				_pSound._aSrcGeneral.PlayOneShot(_currentScriptableWeaponType._chargeWeaponSfx[0], 0.45f);
			}
		}
	}

	protected static void InvokeUserCode_Rpc_WeaponChargeDisplay__Boolean(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC Rpc_WeaponChargeDisplay called on server.");
		}
		else
		{
			((PlayerCombat)obj).UserCode_Rpc_WeaponChargeDisplay__Boolean(reader.ReadBool());
		}
	}

	static PlayerCombat()
	{
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerCombat), "System.Void PlayerCombat::Cmd_ResetHitboxes()", InvokeUserCode_Cmd_ResetHitboxes, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerCombat), "System.Void PlayerCombat::Cmd_QuickSwapWeapon()", InvokeUserCode_Cmd_QuickSwapWeapon, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerCombat), "System.Void PlayerCombat::Cmd_LockWeapon()", InvokeUserCode_Cmd_LockWeapon, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerCombat), "System.Void PlayerCombat::Cmd_SheatheWeapon(WeaponSheatheCondition)", InvokeUserCode_Cmd_SheatheWeapon__WeaponSheatheCondition, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerCombat), "System.Void PlayerCombat::Cmd_InterruptCombat(System.Boolean)", InvokeUserCode_Cmd_InterruptCombat__Boolean, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerCombat), "System.Void PlayerCombat::Cmd_ApplyBlockCondition(System.Boolean)", InvokeUserCode_Cmd_ApplyBlockCondition__Boolean, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerCombat), "System.Void PlayerCombat::Cmd_WeaponChargeDisplay(System.Boolean)", InvokeUserCode_Cmd_WeaponChargeDisplay__Boolean, requiresAuthority: true);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerCombat), "System.Void PlayerCombat::Rpc_InterruptCombat(System.Boolean)", InvokeUserCode_Rpc_InterruptCombat__Boolean);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerCombat), "System.Void PlayerCombat::Rpc_WeaponChargeDisplay(System.Boolean)", InvokeUserCode_Rpc_WeaponChargeDisplay__Boolean);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerCombat), "System.Void PlayerCombat::Target_CancelBlock()", InvokeUserCode_Target_CancelBlock);
	}

	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			GeneratedNetworkCode._Write_WeaponSheatheCondition(writer, _currentSheathCondition);
			writer.WriteBool(_isUsingAltWeapon);
			writer.WriteBool(_lockWeapon);
			GeneratedNetworkCode._Write_DamageType(writer, _damageTypeOverride);
			writer.WriteBool(_useDamageTypeOverride);
			writer.WriteBool(_isBlocking);
			writer.WriteBool(_inParryWindow);
			return;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 1L) != 0L)
		{
			GeneratedNetworkCode._Write_WeaponSheatheCondition(writer, _currentSheathCondition);
		}
		if ((base.syncVarDirtyBits & 2L) != 0L)
		{
			writer.WriteBool(_isUsingAltWeapon);
		}
		if ((base.syncVarDirtyBits & 4L) != 0L)
		{
			writer.WriteBool(_lockWeapon);
		}
		if ((base.syncVarDirtyBits & 8L) != 0L)
		{
			GeneratedNetworkCode._Write_DamageType(writer, _damageTypeOverride);
		}
		if ((base.syncVarDirtyBits & 0x10L) != 0L)
		{
			writer.WriteBool(_useDamageTypeOverride);
		}
		if ((base.syncVarDirtyBits & 0x20L) != 0L)
		{
			writer.WriteBool(_isBlocking);
		}
		if ((base.syncVarDirtyBits & 0x40L) != 0L)
		{
			writer.WriteBool(_inParryWindow);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize(ref _currentSheathCondition, OnWeaponSheatheChange, GeneratedNetworkCode._Read_WeaponSheatheCondition(reader));
			GeneratedSyncVarDeserialize(ref _isUsingAltWeapon, null, reader.ReadBool());
			GeneratedSyncVarDeserialize(ref _lockWeapon, null, reader.ReadBool());
			GeneratedSyncVarDeserialize(ref _damageTypeOverride, null, GeneratedNetworkCode._Read_DamageType(reader));
			GeneratedSyncVarDeserialize(ref _useDamageTypeOverride, null, reader.ReadBool());
			GeneratedSyncVarDeserialize(ref _isBlocking, OnBlockingChange, reader.ReadBool());
			GeneratedSyncVarDeserialize(ref _inParryWindow, null, reader.ReadBool());
			return;
		}
		long num = (long)reader.ReadULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _currentSheathCondition, OnWeaponSheatheChange, GeneratedNetworkCode._Read_WeaponSheatheCondition(reader));
		}
		if ((num & 2L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _isUsingAltWeapon, null, reader.ReadBool());
		}
		if ((num & 4L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _lockWeapon, null, reader.ReadBool());
		}
		if ((num & 8L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _damageTypeOverride, null, GeneratedNetworkCode._Read_DamageType(reader));
		}
		if ((num & 0x10L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _useDamageTypeOverride, null, reader.ReadBool());
		}
		if ((num & 0x20L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _isBlocking, OnBlockingChange, reader.ReadBool());
		}
		if ((num & 0x40L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _inParryWindow, null, reader.ReadBool());
		}
	}
}
