using System.Collections;
using UnityEngine;

public class SkillBehavior_agileTechnique : SkillObjectBehavior
{
	private PlayerMove _pMove;

	private PlayerVisual _pVisual;

	private PlayerCasting _pCasting;

	private PlayerTargeting _pTargeting;

	private PlayerCombat _pCombat;

	[Header("Agile Technique Skill")]
	public int _maxJumpModifier = 3;

	[Space]
	[SerializeField]
	private float _dashJumpForce = 25.5f;

	[SerializeField]
	private float _dashJumpForwardForce = 35.5f;

	[SerializeField]
	private float _dashJumpGravityMod = 45.5f;

	[SerializeField]
	private float _timeBeforeDashLongJump = 0.4f;

	[SerializeField]
	private bool _enableDashLongjump;

	private bool _isDashJumping;

	private int _dashPresses;

	public override void Init_SkillBehavior()
	{
	}

	public override void End_SkillBehavior()
	{
	}

	public void OnDestroy()
	{
		if ((bool)_skillObject._parentPlayer && _skillObject._parentPlayer.isLocalPlayer && (bool)_pMove)
		{
			_pMove._maxJumps = 2;
		}
	}

	public override void Update_SkillBehavior()
	{
		if ((bool)_skillObject._parentPlayer && _skillObject._parentPlayer.isLocalPlayer)
		{
			if (!_pMove)
			{
				_pMove = _skillObject._parentPlayer._pMove;
			}
			if (!_pVisual)
			{
				_pVisual = _skillObject._parentPlayer._pVisual;
			}
			if (!_pCasting)
			{
				_pCasting = _skillObject._parentPlayer._pCasting;
			}
			if (!_pCombat)
			{
				_pCombat = _skillObject._parentPlayer._pCombat;
			}
			if (!_pTargeting)
			{
				_pTargeting = _skillObject._parentPlayer._pTargeting;
			}
			if ((bool)_pMove)
			{
				_pMove._maxJumps = _maxJumpModifier;
			}
			Handle_DashJumpControl();
		}
		bool Check_ToResetDashPresses()
		{
			if (_pMove._dashCallbackTimer <= 0f)
			{
				return true;
			}
			if (_pCombat._isAirAttacked || _pCombat._localAirAttackInput)
			{
				return true;
			}
			return _pMove._currentMovementAction switch
			{
				MovementAction.LEDGEGRAB => true, 
				MovementAction.CLIMB => true, 
				_ => false, 
			};
		}
		void Handle_DashJumpControl()
		{
			if (_enableDashLongjump)
			{
				if (Check_ToResetDashPresses())
				{
					_dashPresses = 0;
				}
				else
				{
					if (!_isDashJumping)
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
						StartCoroutine(Init_DashLongJump());
					}
				}
			}
		}
	}

	private IEnumerator Init_DashLongJump()
	{
		if (_isDashJumping)
		{
			yield break;
		}
		float horiz = InputControlManager.current._horiz_input;
		float vert = InputControlManager.current._vert_input;
		Vector3 inputIn = new Vector3(horiz, 0f, vert);
		if (inputIn == Vector3.zero)
		{
			yield break;
		}
		_isDashJumping = true;
		yield return new WaitForSeconds(_timeBeforeDashLongJump);
		if (_pCombat._isAirAttacked || _pCombat._localAirAttackInput)
		{
			_isDashJumping = false;
			yield break;
		}
		if (_pMove._currentMovementAction == MovementAction.LEDGEGRAB)
		{
			_isDashJumping = false;
			yield break;
		}
		if (_pMove._currentMovementAction == MovementAction.CLIMB)
		{
			_isDashJumping = false;
			yield break;
		}
		_pMove._lockControlMidair = true;
		_pMove._airTime = 0f;
		_pMove._currentJumps = 1;
		_pMove.Init_Jump(_dashJumpForce, 0f, 0f, _dashJumpGravityMod, _useAnim: false);
		_pMove._cachedLastInput = GameManager._current.CameraRelativeFlatten(inputIn, Vector3.up);
		_pMove._ctrlJumpForce = _dashJumpForwardForce;
		_pCasting.Cmd_SetPassiveRuntimeObject(_skillObject._scriptSkill._skillName, 0, _isTrue: true);
		if (_pMove._strafeToggle || (bool)_pTargeting._targetedEntity)
		{
			if (vert > 0f)
			{
				_pVisual.Send_CrossFadeAnim("DashJump_forward", 0.01f, 10, LatencyCheck.IGNORE_LATENCY);
			}
			else if (vert < 0f)
			{
				_pVisual.Send_CrossFadeAnim("DashJump_back", 0.01f, 10, LatencyCheck.IGNORE_LATENCY);
			}
			else if (horiz > 0f)
			{
				_pVisual.Send_CrossFadeAnim("DashJump_right", 0.01f, 10, LatencyCheck.IGNORE_LATENCY);
			}
			else if (horiz < 0f)
			{
				_pVisual.Send_CrossFadeAnim("DashJump_left", 0.01f, 10, LatencyCheck.IGNORE_LATENCY);
			}
		}
		else
		{
			_pVisual.Send_CrossFadeAnim("DashJump_forward", 0.01f, 10, LatencyCheck.IGNORE_LATENCY);
		}
		yield return new WaitForSeconds(0.2f);
		_pVisual.Send_CrossFadeAnim("Fall", 0.01f, 0, LatencyCheck.IGNORE_LATENCY);
		while (!_pMove.RayGroundCheck() && _skillObject._parentPlayer._currentPlayerAction == PlayerAction.IDLE)
		{
			if (base.gameObject.scene.GetPhysicsScene().Raycast(_pMove.transform.position, _pMove.transform.forward, 3.25f, CastLayers.GroundMask))
			{
				_pMove._ctrlJumpForce = 0f;
				_pMove._disableDoubleJumpAnim = true;
				if (_pCombat._isChargingWeapon)
				{
					_pCombat.Cmd_InterruptCombat(_resetAnimation: false);
				}
				_pVisual.Send_CrossFadeAnim("JumpAtk_HitWall", 0.01f, 3, LatencyCheck.IGNORE_LATENCY);
				_pVisual.Send_CrossFadeAnim("Fall", 0.01f, 0, LatencyCheck.IGNORE_LATENCY);
				_pMove.Init_Jump(25.5f, -25.5f, 0f, 45.5f, _useAnim: false);
				_isDashJumping = false;
				yield break;
			}
			yield return null;
		}
		_isDashJumping = false;
	}
}
