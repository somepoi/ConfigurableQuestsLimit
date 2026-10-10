using UnityEngine;

public class LadderEndCap : MonoBehaviour
{
	[Header("Ladder End Cap")]
	public bool _bottomEndcap;

	[SerializeField]
	private float _topEndCapJumpForce = 45.5f;

	private void Awake()
	{
		base.gameObject.SetActive(value: false);
	}

	private void OnTriggerStay(Collider other)
	{
		if (!Player._mainPlayer || !other.gameObject.GetComponent<Player>() || !other.gameObject.GetComponent<Player>())
		{
			return;
		}
		Player component = other.gameObject.GetComponent<Player>();
		PlayerClimbing component2 = other.gameObject.GetComponent<PlayerClimbing>();
		if (!(component != Player._mainPlayer) && component.isLocalPlayer && component2._isClimbing && (!_bottomEndcap || component2._isGoingDown) && (_bottomEndcap || !component2._isGoingDown))
		{
			if (!_bottomEndcap)
			{
				component2.Init_JumpOffRope(_topEndCapJumpForce, 0f);
			}
			else
			{
				component2.Init_JumpOffRope(15.5f, -3.5f);
			}
		}
	}
}
