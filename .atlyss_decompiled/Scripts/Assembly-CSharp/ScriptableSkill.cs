using UnityEngine;

[CreateAssetMenu(fileName = "skill_", menuName = "ScriptableObjects/Skill/Scriptable Skill", order = 0)]
public class ScriptableSkill : ScriptableObject
{
	public string _skillName = "New Skill";

	public Sprite _skillIcon;

	[Header("Scriptable Skill")]
	public SkillControlType _skillControlType;

	public DamageType _skillDamageType = DamageType.Mind;

	public CombatColliderType _skillUtilityType;

	public bool _allowAsBonusSkill;

	[Space]
	[TextArea(5, 10)]
	public string _skillDescription = "New Skill Description";

	[Header("Skill Weapon")]
	public Skill_WeaponRequirement[] _weaponTypeRequirements;

	public SkillToolTipRequirement _toolTipRequirement;

	public bool _requireShield;

	[Header("Skill Params")]
	public SkillParameters _skillRankParams;

	[Header("Skill Movement")]
	public float _movementSpeedDecrease = -8.65f;

	public bool _strafeDirection;

	[Space]
	[Range(0f, 2.5f)]
	public float _airTimeLockBuffer;

	public bool _enableAirTimeMod;

	[Space]
	public float _lockMoveControlTime;

	public float _lockCombatControlTime = 0.35f;

	public float _timeBeforeAttackForce;

	public float _attackForceApply;

	public float _attackForceDampen = 120f;

	public bool _attackForceInputInfluence;

	[Header("Skill Effects")]
	public CastEffectCollection _castEffectCollection;

	public bool _useGlyphEffect = true;

	[Header("Skill Animation")]
	public string _skillChargeAnimation = "Direct";

	public string _skillCastAnimation = "Direct";

	public float _animChargeSmoothing = 0.1f;

	public float _animCastSmoothing;

	[Space]
	public WeaponSheatheCondition _setSheatheCondition;

	public bool _applySheatheCondition;

	public bool _hideWeapon = true;

	private void OnValidate()
	{
		if (_skillRankParams._baseSkillPower < 0)
		{
			_skillRankParams._baseSkillPower = 0;
		}
		if (_skillRankParams._baseCooldown < 0.12f)
		{
			_skillRankParams._baseCooldown = 0.12f;
		}
		if (_weaponTypeRequirements == null || _weaponTypeRequirements.Length == 0)
		{
			return;
		}
		for (int i = 0; i < _weaponTypeRequirements.Length; i++)
		{
			if ((bool)_weaponTypeRequirements[i]._wepType)
			{
				_weaponTypeRequirements[i]._weaponTypeTag = _weaponTypeRequirements[i]._wepType._weaponTypeClassTag;
			}
		}
	}

	public bool Cannot_CastSkill(StatusEntity _obj)
	{
		if (_skillRankParams._castOnNoConditionOnly && _obj._syncConditions.Count > 0)
		{
			for (int i = 0; i < _obj._syncConditions.Count; i++)
			{
				if ((bool)_skillRankParams._skillObjectCondition._scriptableCondition && _obj._syncConditions[i]._conditionID == _skillRankParams._skillObjectCondition._scriptableCondition._conditionID)
				{
					return true;
				}
				if ((bool)_skillRankParams._selfConditionOutput._scriptableCondition && _obj._syncConditions[i]._conditionID == _skillRankParams._selfConditionOutput._scriptableCondition._conditionID)
				{
					return true;
				}
			}
		}
		bool flag = false;
		if ((bool)_obj._isPlayer)
		{
			PlayerStats pStats = _obj._isPlayer._pStats;
			flag = _obj._isPlayer._pCasting._ignoreCost;
			if (_obj.isLocalPlayer && (bool)_skillRankParams._baseRequiredItem && _skillRankParams._basedItemCost > 0)
			{
				ScriptableItem baseRequiredItem = _skillRankParams._baseRequiredItem;
				PlayerInventory pInventory = _obj._isPlayer._pInventory;
				for (int j = 0; j < pInventory._heldItems.Count; j++)
				{
					if (pInventory._heldItems[j]._itemName == baseRequiredItem._itemName)
					{
						if (pInventory._heldItems[j]._quantity >= _skillRankParams._basedItemCost)
						{
							break;
						}
						return true;
					}
					if (j >= pInventory._heldItems.Count - 1)
					{
						return true;
					}
				}
			}
			if (_skillRankParams._checkFullHealth && _obj._currentHealth >= pStats._statStruct._maxHealth)
			{
				return true;
			}
			if (_skillRankParams._checkFullMana && _obj._currentMana >= pStats._statStruct._maxMana)
			{
				return true;
			}
			if (_skillRankParams._checkFullStamina && _obj._currentStamina >= pStats._statStruct._maxStamina)
			{
				return true;
			}
		}
		if (flag)
		{
			return false;
		}
		if (_skillRankParams._manaCost > 0 && _obj._currentMana < _skillRankParams._manaCost)
		{
			return true;
		}
		if ((bool)_obj._isPlayer)
		{
			if (_skillRankParams._healthCost > 0 && _obj._currentHealth < _skillRankParams._healthCost)
			{
				return true;
			}
			if (_skillRankParams._staminaCost > 0 && _obj._currentStamina < _skillRankParams._staminaCost)
			{
				return true;
			}
		}
		return false;
	}

	public void Init_SkillInput(StatusEntity _obj)
	{
		if (!Cannot_CastSkill(_obj))
		{
			Init_PlayerSkillInput();
		}
		void Init_PlayerSkillInput()
		{
			if ((bool)_obj._isPlayer)
			{
				PlayerCombat pCombat = _obj._isPlayer._pCombat;
				PlayerCasting pCasting = _obj._isPlayer._pCasting;
				PlayerVisual pVisual = _obj._isPlayer._pVisual;
				string text = _skillChargeAnimation;
				if (_weaponTypeRequirements != null)
				{
					for (int i = 0; i < _weaponTypeRequirements.Length; i++)
					{
						if (_weaponTypeRequirements[i]._wepType == pCombat._currentScriptableWeaponType)
						{
							text = _weaponTypeRequirements[i]._animOverride_charge;
							break;
						}
						if (i >= _weaponTypeRequirements.Length - 1 && pCombat._currentScriptableWeaponType != _weaponTypeRequirements[i]._wepType)
						{
							return;
						}
					}
				}
				if (!_requireShield || ((bool)pCombat._equippedShield && pCombat._currentScriptableWeaponType._weaponHandedness != WeaponHandedness.Two_Handed))
				{
					_obj._isPlayer._currentPlayerAction = PlayerAction.CASTING;
					if (!string.IsNullOrEmpty(text) && _obj._isPlayer.isLocalPlayer && _obj._isPlayer.isClient)
					{
						pVisual.Send_CrossFadeAnim("spell_charge" + text, _animChargeSmoothing, 9, LatencyCheck.CHECK_LATENCY);
					}
					pCasting._currentCastSkill = this;
					pCasting._isCasting = true;
				}
			}
		}
	}

	public void Init_SkillBehavior(StatusEntity _obj)
	{
		if (Cannot_CastSkill(_obj))
		{
			return;
		}
		Init_PlayerSkillBehavior();
		Init_ServerParams();
		_obj.ScriptSkill_CancelConditionCallback(CancelConditionOnCastType.ANY);
		if ((bool)_skillRankParams._selfConditionOutput._scriptableCondition)
		{
			int num = _skillRankParams._selfConditionOutput._conditionDuration;
			int conditionPower = _skillRankParams._selfConditionOutput._conditionPower;
			float num2 = _skillRankParams._selfConditionOutput._powerPercent;
			float conditionRepeatRate = _skillRankParams._selfConditionOutput._conditionRepeatRate;
			if ((bool)_obj._isPlayer)
			{
				num2 += _obj._isPlayer._pStats.Retrieve_SkillConditionBonusPower(this);
				num += _obj._isPlayer._pStats.Retrieve_SkillConditionBonusDuration(this);
			}
			_skillRankParams._selfConditionOutput._scriptableCondition.Apply_ConditionToEntity(_obj, _obj, _skillDamageType, conditionPower, num, num2, conditionRepeatRate);
		}
		if (_skillRankParams._skillObject_onCast != null)
		{
			Init_NewSkillObject(_skillRankParams._skillObject_onCast, _obj);
		}
		void Init_PlayerSkillBehavior()
		{
			if ((bool)_obj._isPlayer)
			{
				PlayerInventory pInventory = _obj._isPlayer._pInventory;
				PlayerCombat pCombat = _obj._isPlayer._pCombat;
				PlayerVisual pVisual = _obj._isPlayer._pVisual;
				PlayerMove pMove = _obj._isPlayer._pMove;
				ChatBehaviour chatBehaviour = _obj._isPlayer._chatBehaviour;
				string text = _skillCastAnimation;
				if (_weaponTypeRequirements != null)
				{
					for (int i = 0; i < _weaponTypeRequirements.Length; i++)
					{
						if (_weaponTypeRequirements[i]._wepType == pCombat._currentScriptableWeaponType)
						{
							text = _weaponTypeRequirements[i]._animOverride_cast;
							break;
						}
						if (i >= _weaponTypeRequirements.Length - 1 && pCombat._currentScriptableWeaponType != _weaponTypeRequirements[i]._wepType)
						{
							return;
						}
					}
				}
				if (!_requireShield || ((bool)pCombat._equippedShield && pCombat._currentScriptableWeaponType._weaponHandedness != WeaponHandedness.Two_Handed))
				{
					if (_obj.isLocalPlayer)
					{
						if ((bool)_skillRankParams._baseRequiredItem && _skillRankParams._basedItemCost > 0)
						{
							for (int j = 0; j < pInventory._heldItems.Count; j++)
							{
								if (pInventory._heldItems[j]._itemName == _skillRankParams._baseRequiredItem._itemName)
								{
									if (pInventory._heldItems[j]._quantity >= _skillRankParams._basedItemCost)
									{
										pInventory.Remove_Item(pInventory._heldItems[j], _skillRankParams._basedItemCost);
										break;
									}
									return;
								}
								if (j >= pInventory._heldItems.Count - 1)
								{
									return;
								}
							}
						}
						chatBehaviour.Init_GameLogicMessage("Casted Skill: " + _skillName + ".");
					}
					if (!string.IsNullOrEmpty(text))
					{
						pVisual.Local_CrossFadeAnim("spell_cast" + text, _animCastSmoothing, 9);
					}
					Init_SkillMovementForce(pMove, pCombat);
				}
			}
		}
		void Init_ServerParams()
		{
			if (_obj.isServer)
			{
				Init_StandardCost();
			}
		}
		void Init_StandardCost()
		{
			if (!_obj._isPlayer || !_obj._isPlayer._pCasting._ignoreCost)
			{
				if (_skillRankParams._manaCost > 0)
				{
					_obj.Subtract_Mana(_skillRankParams._manaCost);
				}
				if (_skillRankParams._healthCost > 0)
				{
					_obj.Subtract_Health(_skillRankParams._healthCost);
				}
				if (_skillRankParams._staminaCost > 0)
				{
					_obj.Subtract_Stamina(_skillRankParams._staminaCost);
				}
			}
		}
	}

	public void Init_SkillMovementForce(PlayerMove _pMove, PlayerCombat _pCombat)
	{
		if (_pMove.isLocalPlayer)
		{
			if (_strafeDirection)
			{
				_pMove.Apply_CastStrafeLock();
			}
			if (_lockCombatControlTime > 0f)
			{
				_pCombat._lockControlBuffer = _lockCombatControlTime;
			}
			if (_lockMoveControlTime > 0f)
			{
				_pMove._freezeMovementBuffer = _lockMoveControlTime;
			}
			if (_enableAirTimeMod)
			{
				_pMove._airTimeLockBuffer = _airTimeLockBuffer;
			}
			_pMove._currentRevUpSpeed = 0f;
			_pMove.Init_AttackForceApply(_attackForceApply, _attackForceDampen, _timeBeforeAttackForce);
		}
	}

	public void Init_NewSkillObject(GameObject _skObj, StatusEntity _obj)
	{
		if ((bool)Player._mainPlayer && !Player._mainPlayer._isHostPlayer && (bool)_obj._isPlayer && Player._mainPlayer.Network_playerMapInstance != _obj._isPlayer.Network_playerMapInstance && _skillControlType != SkillControlType.Passive)
		{
			return;
		}
		PlayerStats _pStats = null;
		PlayerCasting _pCasting = null;
		GameObject _newSkillObj = PoolManager._current.GenerateObject(_skObj, string.Empty, 0f);
		SkillObject _skillObj = _newSkillObj.GetComponent<SkillObject>();
		if (_skillRankParams._isSummonSkill)
		{
			_obj._summonSkillObject = _skillObj;
			if (_obj.isServer)
			{
				_obj.Network_summonSkillTag = _skillName;
			}
		}
		if ((bool)_skillObj && (bool)_skillObj.GetComponent<SkillObjectBehavior>())
		{
			_skillObj._skillObjBehavior = _skillObj.GetComponent<SkillObjectBehavior>();
		}
		if ((bool)_obj._isPlayer)
		{
			if ((bool)_skillObj)
			{
				_skillObj._parentPlayer = _obj._isPlayer;
			}
			_pStats = _obj._isPlayer._pStats;
			_pCasting = _obj._isPlayer._pCasting;
		}
		if ((bool)_obj._isCreep)
		{
			_skillObj._parentCreep = _obj._isCreep;
		}
		if ((bool)_skillObj)
		{
			_skillObj._scriptSkill = this;
		}
		_newSkillObj.transform.SetParent(_obj.transform);
		_newSkillObj.transform.SetParent(null);
		Check_ForDamageCollider();
		Check_ForProjectileObject();
		Check_ForPassiveSkillObject();
		void Check_ForDamageCollider()
		{
			CombatCollider componentInChildren = _newSkillObj.GetComponentInChildren<CombatCollider>(includeInactive: true);
			if ((bool)componentInChildren)
			{
				StatStruct statStruct = default;
				if ((bool)_obj._isPlayer)
				{
					componentInChildren._parentObject = _obj._isPlayer.gameObject;
					statStruct = _pStats._statStruct;
					if (_skillRankParams._useWeaponPower)
					{
						componentInChildren._scriptWeapon = _obj._isPlayer._pCombat._equippedWeapon;
					}
				}
				else if ((bool)_obj._isCreep)
				{
					componentInChildren._parentObject = _obj._isCreep.gameObject;
					statStruct = _obj._isCreep._statStruct;
				}
				ScriptableCombatElement scriptableCombatElement = _skillRankParams._skillElement;
				if (!scriptableCombatElement)
				{
					scriptableCombatElement = GameManager._current._statLogics._defaultCombatElement;
				}
				ConditionSlot skillObjectCondition = _skillRankParams._skillObjectCondition;
				if ((bool)_obj._isPlayer && (bool)_pStats)
				{
					skillObjectCondition._powerPercent += _pStats.Retrieve_SkillConditionBonusPower(this);
					skillObjectCondition._conditionDuration += _pStats.Retrieve_SkillConditionBonusDuration(this);
				}
				componentInChildren._combatElement = scriptableCombatElement;
				componentInChildren._scriptableConditionSlot = skillObjectCondition;
				componentInChildren._resetCritChanceOnEnable = false;
				componentInChildren._setPhysCritChance = statStruct._criticalRate;
				componentInChildren._setMagicCritChance = statStruct._magicCriticalRate;
				switch (componentInChildren._colliderType)
				{
				case CombatColliderType.OFFENSIVE:
					_obj.ScriptSkill_CancelConditionCallback(CancelConditionOnCastType.OFFENSIVE);
					break;
				case CombatColliderType.SUPPORT:
					_obj.ScriptSkill_CancelConditionCallback(CancelConditionOnCastType.SUPPORT);
					break;
				}
				componentInChildren._dmgType = _skillDamageType;
				componentInChildren.Get_ParentObjectInfo();
			}
		}
		void Check_ForPassiveSkillObject()
		{
			if (_skillControlType == SkillControlType.Passive)
			{
				if ((bool)_pCasting)
				{
					_pCasting._passiveSkillObjects.Add(_skillObj._skillObjBehavior);
				}
				_newSkillObj.transform.SetParent(_obj.transform);
				_newSkillObj.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
			}
		}
		void Check_ForProjectileObject()
		{
			ProjectileObject _projectileObj = _newSkillObj.GetComponentInChildren<ProjectileObject>(includeInactive: true);
			Vector3 _projectileBulletPos = _obj.transform.position;
			Vector3 _projectileBulletPosForward = _obj.transform.forward;
			Quaternion _projectileBulletRot = _obj.transform.rotation;
			if ((bool)_obj._isPlayer)
			{
				Init_PlayerProjectile();
			}
			if ((bool)_obj._isCreep)
			{
				Init_CreepProjectile();
			}
			if (!_projectileObj)
			{
				_newSkillObj.transform.position = _projectileBulletPos;
			}
			else
			{
				if (_projectileObj._applyRotationToParent)
				{
					Vector3 eulerAngles = _obj.transform.rotation.eulerAngles;
					eulerAngles.x = 0f;
					Quaternion rotation = Quaternion.Euler(eulerAngles);
					if (_projectileObj._ignoreYAxisRotation)
					{
						_newSkillObj.transform.SetPositionAndRotation(_projectileBulletPos + _projectileObj._offsetPosition, rotation);
					}
					else
					{
						_newSkillObj.transform.SetPositionAndRotation(_projectileBulletPos + _projectileObj._offsetPosition, _projectileBulletRot);
					}
				}
				else
				{
					Vector3 offsetPosition = _projectileObj._offsetPosition;
					offsetPosition.z = 0f;
					offsetPosition.x = 0f;
					_projectileObj.transform.position = _projectileBulletPos + offsetPosition;
					_projectileObj.transform.position += _projectileBulletPosForward * _projectileObj._offsetPosition.z;
				}
				_projectileObj._parentTransform = _obj.transform;
			}
			void Init_CreepProjectile()
			{
				CreepBehavior creepBehavior = _obj._isCreep._creepBehavior;
				if (creepBehavior.GetType() == typeof(CreepBehavior_StandardCreep))
				{
					CreepBehavior_StandardCreep creepBehavior_StandardCreep = (CreepBehavior_StandardCreep)creepBehavior;
					if ((bool)creepBehavior_StandardCreep._projectileAimer)
					{
						creepBehavior_StandardCreep._projectileAimer.GetPositionAndRotation(out _projectileBulletPos, out _projectileBulletRot);
						_projectileBulletPosForward = creepBehavior_StandardCreep._projectileAimer.forward;
					}
				}
			}
			void Init_PlayerProjectile()
			{
				PlayerCombat pCombat = _obj._isPlayer._pCombat;
				pCombat._rangeWeaponBulletPos.GetPositionAndRotation(out _projectileBulletPos, out _projectileBulletRot);
				_projectileBulletPosForward = pCombat._rangeWeaponBulletPos.forward;
				if ((bool)_projectileObj)
				{
					_pCasting._cachedProjectileObj = _projectileObj;
				}
			}
		}
	}
}
