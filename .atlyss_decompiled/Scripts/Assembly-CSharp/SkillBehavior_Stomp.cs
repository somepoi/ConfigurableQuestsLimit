using System.Collections;
using UnityEngine;

public class SkillBehavior_Stomp : SkillObjectBehavior
{
	private PlayerVisual _pVisual;

	private PlayerCombat _pCombat;

	private PlayerCasting _pCasting;

	private PlayerMove _pMove;

	private bool _isLocal;

	[Header("Fighter Stomp Skill")]
	[SerializeField]
	private GameObject _stompObject;

	[SerializeField]
	private float _downwardsVelocity = 12.5f;

	[SerializeField]
	private float _gravityModifier = 12.5f;

	public override void Init_SkillBehavior()
	{
		if ((bool)_skillObject._parentPlayer)
		{
			_isLocal = _skillObject._parentPlayer.isLocalPlayer;
			_pVisual = _skillObject._parentPlayer._pVisual;
			_pCombat = _skillObject._parentPlayer._pCombat;
			_pCasting = _skillObject._parentPlayer._pCasting;
			_pMove = _skillObject._parentPlayer._pMove;
			StartCoroutine(Init_StompAction());
		}
	}

	private IEnumerator Init_StompAction()
	{
		if (!_pMove.RayGroundCheck() && _isLocal)
		{
			_pMove.Init_Jump(0f - _downwardsVelocity, 0f, 0f, _gravityModifier, _useAnim: false);
		}
		while (!_pMove.RayGroundCheck())
		{
			if (_isLocal)
			{
				_pCombat._lockControlBuffer = 0.2f;
				_pCasting._lockControlBuffer = 0.2f;
				_pMove._lockControlBuffer = 0.2f;
			}
			yield return null;
		}
		_stompObject.transform.position = _skillObject._parentPlayer.transform.position + Vector3.up * -1.5f;
		_pVisual.Local_CrossFadeAnim("spell_castStomp", 0f, 9);
		_stompObject.SetActive(value: true);
	}

	public override void Update_SkillBehavior()
	{
	}

	public override void End_SkillBehavior()
	{
	}
}
