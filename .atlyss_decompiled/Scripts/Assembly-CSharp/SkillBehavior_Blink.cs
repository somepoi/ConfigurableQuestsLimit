using System.Collections;
using UnityEngine;

public class SkillBehavior_Blink : SkillObjectBehavior
{
	[Header("Skill Behavior - Blink")]
	[SerializeField]
	private float _blinkSpeed = 48.5f;

	private bool _executeMovement;

	private Vector3 _directionalInput;

	private int _dashPresses;

	public override void Init_SkillBehavior()
	{
	}

	public override void End_SkillBehavior()
	{
	}

	private void BlinkAction(Player _p)
	{
		if (_p.isLocalPlayer && _p.isOwned)
		{
			float horiz_input = InputControlManager.current._horiz_input;
			float vert_input = InputControlManager.current._vert_input;
			Vector3 vector = new Vector3(horiz_input, 0f, vert_input);
			_p._pCasting.Cmd_SetPassiveRuntimeObject(_skillObject._scriptSkill._skillName, 0, _isTrue: true);
			_p._pVisual.Send_CrossFadeAnim("spell_castOmni", 0f, 9, LatencyCheck.IGNORE_LATENCY);
			_p._pVisual.Cmd_HideWeapon(0.6f);
			if (_p._currentPlayerAction == PlayerAction.ATTACKING || _p._pCombat._isChargingWeapon)
			{
				_p._pCombat.Cmd_InterruptCombat(_resetAnimation: true);
				_p._pCombat._chargeAttackCooldown = 0.4f;
			}
			if ((bool)_p._pCasting._currentCastSkill)
			{
				_p._pCasting.Cmd_InitCastInterrupt();
			}
			_directionalInput = GameManager._current.CameraRelativeFlatten(vector, Vector3.up);
			if (vector == Vector3.zero)
			{
				_directionalInput = _p.transform.forward;
			}
			StartCoroutine(Init_MovementExecution(_p));
		}
	}

	private IEnumerator Init_MovementExecution(Player _p)
	{
		_p.GetComponent<PlayerClimbing>()._climbAttachBuffer = 0.92f;
		_executeMovement = true;
		yield return new WaitForSeconds(0.1f);
		_executeMovement = false;
	}

	public override void Update_SkillBehavior()
	{
		PlayerMove _pMove;
		PlayerCombat _pCombat;
		if ((bool)_skillObject._parentPlayer && _skillObject._parentPlayer.isLocalPlayer)
		{
			_pMove = Player._mainPlayer._pMove;
			_pCombat = Player._mainPlayer._pCombat;
			if (_executeMovement)
			{
				_pMove._playerController.Move(_blinkSpeed * Time.deltaTime * _directionalInput);
			}
			Handle_BlinkControl();
		}
		void Handle_BlinkControl()
		{
			if (_pMove._dashCallbackTimer <= 0f)
			{
				_dashPresses = 0;
			}
			else if (_pCombat._isAirAttacked || _pCombat._localAirAttackInput)
			{
				_dashPresses = 0;
			}
			else
			{
				switch (_pMove._currentMovementAction)
				{
				case MovementAction.LEDGEGRAB:
					_dashPresses = 0;
					break;
				case MovementAction.CLIMB:
					_dashPresses = 0;
					break;
				default:
					if (!_executeMovement)
					{
						if (InputControlManager.current.IsDashInputDown)
						{
							_dashPresses++;
						}
					}
					else
					{
						_dashPresses = 0;
					}
					if (InputControlManager.current.IsDashInputDown && _dashPresses >= 2)
					{
						BlinkAction(_skillObject._parentPlayer);
					}
					break;
				}
			}
		}
	}
}
