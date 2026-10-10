using System.Collections;
using UnityEngine;

public class SkillBehavior_bounce : SkillObjectBehavior
{
	private PlayerMove _pMove;

	private PlayerCombat _pCombat;

	private PlayerCasting _pCasting;

	private PlayerVisual _pVisual;

	private PlayerTargeting _pTargeting;

	private StatusEntity _pStatus;

	[Header("Bounce Skill Function")]
	[SerializeField]
	private float _initBounceJumpForce = 35.5f;

	[SerializeField]
	private float _initBounceJumpForwardForce;

	[SerializeField]
	private float _initBounceJumpGravityMod = 35.5f;

	[SerializeField]
	private float _lockControlBuffer = 0.03f;

	private bool _isBouncing;

	private bool _initAimBounce;

	private bool _initAirLock;

	private float _inputHoldBuffer;

	private float _hitwallCooldownBuffer;

	public override void End_SkillBehavior()
	{
	}

	public override void Init_SkillBehavior()
	{
	}

	public override void Update_SkillBehavior()
	{
		if (!_skillObject._parentPlayer)
		{
			return;
		}
		if (!_pMove)
		{
			_pMove = _skillObject._parentPlayer._pMove;
		}
		if (!_pCombat)
		{
			_pCombat = _skillObject._parentPlayer._pCombat;
		}
		if (!_pCasting)
		{
			_pCasting = _skillObject._parentPlayer._pCasting;
		}
		if (!_pVisual)
		{
			_pVisual = _skillObject._parentPlayer._pVisual;
		}
		if (!_pTargeting)
		{
			_pTargeting = _skillObject._parentPlayer._pTargeting;
		}
		if (!_pStatus)
		{
			_pStatus = _skillObject._parentPlayer._statusEntity;
		}
		if (!_skillObject._parentPlayer.isLocalPlayer || _skillObject._parentPlayer._inUI || _skillObject._parentPlayer._bufferingStatus || _skillObject._parentPlayer._inChat)
		{
			return;
		}
		if (InputControlManager.current.IsDashInput)
		{
			_inputHoldBuffer += Time.deltaTime;
		}
		else
		{
			_inputHoldBuffer = 0f;
		}
		if (_hitwallCooldownBuffer > 0f)
		{
			_hitwallCooldownBuffer -= Time.deltaTime;
		}
		else
		{
			_hitwallCooldownBuffer = 0f;
		}
		if (_skillObject._parentPlayer._isHurt || _pStatus._isDamageBuffer)
		{
			_inputHoldBuffer = 0f;
			return;
		}
		switch (_pMove._currentMovementAction)
		{
		case MovementAction.LEDGEGRAB:
			_inputHoldBuffer = 0f;
			return;
		case MovementAction.CLIMB:
			_inputHoldBuffer = 0f;
			return;
		}
		if (_skillObject._parentPlayer._currentPlayerAction != PlayerAction.IDLE)
		{
			_inputHoldBuffer = 0f;
			return;
		}
		if (_skillObject._parentPlayer._currentPlayerCondition != PlayerCondition.ACTIVE)
		{
			_inputHoldBuffer = 0f;
			return;
		}
		if (_inputHoldBuffer >= 0.6f && _hitwallCooldownBuffer <= 0f && !_isBouncing)
		{
			StartCoroutine(Init_BounceFunction());
		}
		if (_isBouncing)
		{
			if (!_pMove.RayGroundCheck() && _initAirLock)
			{
				LayerMask layerMask = CastLayers.GroundMask;
				if (base.gameObject.scene.GetPhysicsScene().Raycast(_pMove.transform.position, _pMove.transform.forward, 3.25f, layerMask))
				{
					_pMove._ctrlJumpForce = 0f;
					_pMove._disableDoubleJumpAnim = true;
					_pVisual.Send_CrossFadeAnim("JumpAtk_HitWall", 0.01f, 3, LatencyCheck.IGNORE_LATENCY);
					_pVisual.Send_CrossFadeAnim("Fall", 0.01f, 0, LatencyCheck.IGNORE_LATENCY);
					_hitwallCooldownBuffer = 0.8f;
					_pMove.Init_Jump(25.5f, -25.5f, 0f, 45.5f, _useAnim: true);
					_isBouncing = false;
					_initAirLock = false;
					StopAllCoroutines();
				}
				_pMove._lockControlBuffer = _lockControlBuffer;
				_pCasting._lockControlBuffer = _lockControlBuffer;
			}
			if (_skillObject._parentPlayer._isHurt)
			{
				StopAllCoroutines();
				_pVisual.Send_CrossFadeAnim("Empty", 0f, 9, LatencyCheck.IGNORE_LATENCY);
				_pMove._lockControlBuffer = 0.03f;
				_pCombat._lockControlBuffer = 0.03f;
				_pCasting._lockControlBuffer = 0.03f;
				_isBouncing = false;
				_initAirLock = false;
			}
		}
		if (_initAimBounce && !_pTargeting._targetedEntity)
		{
			_pMove.transform.rotation = Quaternion.Euler(0f, CameraFunction._current._RotY, 0f);
		}
	}

	private IEnumerator Init_BounceFunction()
	{
		if (!_isBouncing)
		{
			_isBouncing = true;
			_initAimBounce = true;
			_pMove._ctrlJumpForce = 0f;
			_pMove.Init_Jump(_initBounceJumpForce, _initBounceJumpForwardForce, 0f, _initBounceJumpGravityMod, _useAnim: false);
			_pMove._lockControlMidair = true;
			_pMove._worldSpaceInput = Vector3.zero;
			_pMove.inputIn = Vector3.zero;
			_pMove._lockControlBuffer = 1.5f;
			_pCombat._lockControlBuffer = 1.5f;
			_pCasting._lockControlBuffer = 1.5f;
			_pVisual.Send_CrossFadeAnim("spell_chargeBounce", 0.01f, 9, LatencyCheck.IGNORE_LATENCY);
			while (!_pMove.RayGroundCheck())
			{
				yield return null;
			}
			_pVisual.Send_CrossFadeAnim("spell_castBounce", 0.2f, 9, LatencyCheck.IGNORE_LATENCY);
			_pCasting.Cmd_SetPassiveRuntimeObject(_skillObject._scriptSkill._skillName, 0, _isTrue: true);
			yield return new WaitForSeconds(0.3f);
			_pCombat._lockControlBuffer = _lockControlBuffer;
			_initAirLock = true;
			_initAimBounce = false;
			yield return new WaitForSeconds(0.7f);
			while (!_pMove.RayGroundCheck())
			{
				yield return null;
			}
			_initAirLock = false;
			yield return new WaitForSeconds(0.2f);
			_isBouncing = false;
		}
	}
}
