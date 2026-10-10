using UnityEngine;

public class CreepPushCollision : MonoBehaviour
{
	private Creep _parentCreep;

	public Collider _physicalCollider;

	[SerializeField]
	private float _pushJumpBuffer;

	private void Awake()
	{
		_parentCreep = GetComponentInParent<Creep>();
	}

	private void FixedUpdate()
	{
		if (_pushJumpBuffer > 0f)
		{
			_pushJumpBuffer -= Time.deltaTime;
			return;
		}
		_pushJumpBuffer = 0f;
		if (_parentCreep._currentCreepCondition == CreepCondition.DEAD)
		{
			return;
		}
		PhysicsScene physicsScene = base.gameObject.scene.GetPhysicsScene();
		Collider[] array = new Collider[3];
		int num = physicsScene.OverlapSphere(base.transform.position, 4.5f, array, CastLayers.CreepPushMask, QueryTriggerInteraction.Ignore);
		for (int i = 0; i < num; i++)
		{
			Collider collider = array[i];
			if ((bool)collider)
			{
				Init_CollisionCheck(collider);
			}
		}
	}

	private void Init_CollisionCheck(Collider _collider)
	{
		if (!(_collider.transform.root == base.transform) && (bool)_collider.gameObject.GetComponentInParent<Creep>() && _parentCreep._currentCreepCondition != CreepCondition.DEAD && !(_pushJumpBuffer > 0f))
		{
			Creep componentInParent = _collider.gameObject.GetComponentInParent<Creep>();
			if ((bool)componentInParent && componentInParent._currentCreepCondition != CreepCondition.DEAD && !(componentInParent.transform.position.y > base.transform.position.y) && !(componentInParent == _parentCreep) && componentInParent.isServer)
			{
				componentInParent._creepRB.AddRelativeForce(0f, 15.5f, 0f, ForceMode.Impulse);
				componentInParent._creepRB.AddForce(componentInParent.transform.forward * 8.5f, ForceMode.VelocityChange);
				_pushJumpBuffer = 0.8f;
			}
		}
	}
}
