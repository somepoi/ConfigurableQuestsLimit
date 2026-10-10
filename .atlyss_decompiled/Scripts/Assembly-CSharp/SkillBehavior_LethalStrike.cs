using System.Collections;
using UnityEngine;

public class SkillBehavior_LethalStrike : SkillObjectBehavior
{
	private PlayerMove _pMove;

	private PlayerCombat _pCombat;

	private PlayerCasting _pCasting;

	private PlayerVisual _pVisual;

	private StatusEntity _pStatus;

	[Header("Lethal Strike Skill Behavior")]
	[SerializeField]
	private ScriptableSkill _lethalStrikeSkill;

	[SerializeField]
	private float _chargeUpTime = 0.6f;

	[SerializeField]
	private float _cooldownTime = 0.8f;

	public bool _isStriking;

	private float _inputHoldBuffer;

	private bool _isInput;

	private bool _onCooldown;

	public override void Init_SkillBehavior()
	{
	}

	public override void End_SkillBehavior()
	{
	}

	public override void Update_SkillBehavior()
	{
		if (!_skillObject._parentPlayer || !_skillObject._parentPlayer.isLocalPlayer)
		{
			return;
		}
		Player _p = _skillObject._parentPlayer;
		if (!_p)
		{
			return;
		}
		if (!_pMove)
		{
			_pMove = _p._pMove;
		}
		if (!_pCombat)
		{
			_pCombat = _p._pCombat;
		}
		if (!_pCasting)
		{
			_pCasting = _p._pCasting;
		}
		if (!_pVisual)
		{
			_pVisual = _p._pVisual;
		}
		if (!_pStatus)
		{
			_pStatus = _p._statusEntity;
		}
		_isInput = InputControlManager.current.IsDashInput;
		if (CancelChargeCheck() && _inputHoldBuffer > 0f)
		{
			if (!_onCooldown && _isStriking)
			{
				StartCoroutine(Init_Cooldown());
			}
			Reset_LethalStrike(_useInterruptEffect: true);
			return;
		}
		if (!_onCooldown)
		{
			if (_isInput)
			{
				_inputHoldBuffer += Time.deltaTime;
			}
			else
			{
				_inputHoldBuffer = 0f;
			}
		}
		else
		{
			_inputHoldBuffer = 0f;
		}
		if (_inputHoldBuffer >= _chargeUpTime && !_isStriking)
		{
			StartCoroutine(Init_LethalStrike());
		}
		bool CancelChargeCheck()
		{
			if (_p._inUI || TabMenu._current._isOpen || SettingsManager._current._isOpen || _p._chatBehaviour._focusedInChat)
			{
				return true;
			}
			if (_p._currentPlayerCondition != PlayerCondition.ACTIVE)
			{
				return true;
			}
			if (_p._bufferingStatus)
			{
				return true;
			}
			switch (_pMove._currentMovementAction)
			{
			case MovementAction.LEDGEGRAB:
				return true;
			case MovementAction.CLIMB:
				return true;
			default:
				if (GamepadInput.GetButton(GamepadButton.LeftShoulder) || GamepadInput.GetButton(GamepadButton.RightShoulder))
				{
					return true;
				}
				if (!_pVisual._visualAnimator.GetCurrentAnimatorStateInfo(11).IsName("Empty"))
				{
					return true;
				}
				if (_p._currentPlayerAction != PlayerAction.IDLE)
				{
					return true;
				}
				if (_p._knockbackBuffer > 0f)
				{
					return true;
				}
				if (_pCombat._localBlockingInput || _pCombat._isBlocking)
				{
					return true;
				}
				if (_pCombat._isChargingWeapon)
				{
					return true;
				}
				return false;
			}
		}
	}

	private void Reset_LethalStrike(bool _useInterruptEffect)
	{
		if (_isStriking)
		{
			_pVisual.Send_CrossFadeAnim("Empty", 0f, 9, LatencyCheck.IGNORE_LATENCY);
			if (_useInterruptEffect)
			{
				_pCasting.Cmd_InterruptEffect();
			}
			_pCasting.Cmd_SetPassiveRuntimeObject(_skillObject._scriptSkill._skillName, 0, _isTrue: false);
			_pCasting.Cmd_SetPassiveRuntimeObject(_skillObject._scriptSkill._skillName, 2, _isTrue: false);
			_pCasting.Cmd_SetPassiveRuntimeObject(_skillObject._scriptSkill._skillName, 3, _isTrue: false);
			_pCombat._lockSheatheWeaponControl = false;
		}
		StopCoroutine("Init_LethalStrike");
		_inputHoldBuffer = 0f;
		_isStriking = false;
	}

	private IEnumerator Init_LethalStrike()
	{
		if (_isStriking)
		{
			yield break;
		}
		_pCasting.Cmd_SetPassiveRuntimeObject(_skillObject._scriptSkill._skillName, 3, _isTrue: true);
		_isStriking = true;
		if (_pCombat._currentSheathCondition != WeaponSheatheCondition.TAKEN_OUT)
		{
			_pCombat.Cmd_SheatheWeapon(WeaponSheatheCondition.TAKEN_OUT);
		}
		while (_pCombat._currentSheathCondition == WeaponSheatheCondition.PUT_AWAY)
		{
			_pCombat._lockSheatheWeaponControl = true;
			yield return null;
		}
		yield return new WaitForSeconds(0.32f);
		if (!_isInput)
		{
			Reset_LethalStrike(_useInterruptEffect: true);
		}
		else if (_isStriking)
		{
			_pVisual.Send_CrossFadeAnim("spell_chargeMelee_Strike", 0.01f, 9, LatencyCheck.IGNORE_LATENCY);
			_pCasting.Cmd_SetPassiveRuntimeObject(_skillObject._scriptSkill._skillName, 0, _isTrue: true);
			_pMove._isChargeAttackForced = true;
			while (_inputHoldBuffer > 0f)
			{
				_pMove.Apply_CastStrafeLock();
				_pCombat._lockSheatheWeaponControl = true;
				yield return null;
			}
			if (_isStriking)
			{
				_pVisual.Send_CrossFadeAnim("spell_castMelee_Strike", 0.2f, 9, LatencyCheck.IGNORE_LATENCY);
				_pMove._currentRevUpSpeed = 0f;
				_lethalStrikeSkill.Init_SkillMovementForce(_pMove, _pCombat);
				_pCasting.Cmd_SetPassiveRuntimeObject(_skillObject._scriptSkill._skillName, 0, _isTrue: false);
				_pCasting.Cmd_SetPassiveRuntimeObject(_skillObject._scriptSkill._skillName, 1, _isTrue: true);
				_pCasting.Cmd_SetPassiveRuntimeObject(_skillObject._scriptSkill._skillName, 2, _isTrue: true);
				StartCoroutine(Init_Cooldown());
				yield return new WaitForSeconds(0.6f);
				Reset_LethalStrike(_useInterruptEffect: false);
			}
		}
	}

	private IEnumerator Init_Cooldown()
	{
		if (!_onCooldown)
		{
			_onCooldown = true;
			yield return new WaitForSeconds(_cooldownTime);
			_onCooldown = false;
		}
	}
}
