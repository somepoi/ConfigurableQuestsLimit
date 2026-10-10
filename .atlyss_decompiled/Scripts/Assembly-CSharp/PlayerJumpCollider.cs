using UnityEngine;

public class PlayerJumpCollider : MonoBehaviour
{
	[SerializeField]
	private float _upwardsForce = 10f;

	[SerializeField]
	private float _forwardForce;

	[SerializeField]
	private float _gravityMod = 50f;

	[SerializeField]
	private bool _onlyWorkMidAir = true;

	[SerializeField]
	private float _colliderBufferTime = 0.25f;

	private float _currentBuffer;

	private void Update()
	{
		if (_currentBuffer > 0f)
		{
			_currentBuffer -= Time.deltaTime;
		}
		else
		{
			_currentBuffer = 0f;
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if (!other.GetComponentInParent<Player>() || _currentBuffer > 0f)
		{
			return;
		}
		Player componentInParent = other.GetComponentInParent<Player>();
		if (componentInParent.isLocalPlayer && !componentInParent._bufferingStatus && (!componentInParent._isHostPlayer || !(componentInParent.gameObject.scene != base.gameObject.scene)))
		{
			PlayerMove pMove = componentInParent._pMove;
			if (!_onlyWorkMidAir || !pMove.RayGroundCheck())
			{
				pMove.Init_Jump(_upwardsForce, _forwardForce, 0f, _gravityMod, _useAnim: false);
				pMove._lockControlMidair = true;
				_currentBuffer = _colliderBufferTime;
			}
		}
	}
}
