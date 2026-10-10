using UnityEngine;

public class CreepLineOfSight : MonoBehaviour
{
	private Creep _baseCreep;

	[Header("Creep Line Of Sight / Aggro Collider")]
	public CreepAwareness _creepAwareness;

	public CapsuleCollider _aggroCapsule;

	private void Awake()
	{
		_baseCreep = GetComponentInParent<Creep>();
	}

	private void Update()
	{
		if (!_baseCreep.isServer)
		{
			base.enabled = false;
			return;
		}
		float lineOfSightRadius = GameManager._current._statLogics._lineOfSightRadius;
		float aggroLength = GameManager._current._statLogics._aggroLength;
		Vector3 aggroCapsuleCenter = GameManager._current._statLogics._aggroCapsuleCenter;
		_aggroCapsule.enabled = !_baseCreep.Network_aggroedEntity && _baseCreep._scriptCreep._canAggro && _baseCreep._currentCreepCondition == CreepCondition.ACTIVE;
		_aggroCapsule.radius = lineOfSightRadius;
		_aggroCapsule.height = aggroLength;
		_aggroCapsule.center = aggroCapsuleCenter;
	}

	private void OnTriggerStay(Collider other)
	{
		if (!(other.gameObject.scene != base.gameObject.scene) && !(_creepAwareness._interestToAggroBuffer > 0f) && !_baseCreep.Network_aggroedEntity && !_baseCreep._disableAggro && other.gameObject.transform.root.TryGetComponent<Player>(out var component) && component._currentPlayerCondition == PlayerCondition.ACTIVE && !component._bufferingStatus && !component._isHidden && _baseCreep.CanDetectAggroTarget(other.gameObject))
		{
			Handle_DetectEntityBehavior(other.gameObject.transform);
		}
	}

	private void Handle_DetectEntityBehavior(Transform _objectTransform)
	{
		if (!(_creepAwareness._interestToAggroBuffer > 0f) && !_baseCreep.Network_aggroedEntity && !_baseCreep._disableAggro)
		{
			PhysicsScene physicsScene = base.gameObject.scene.GetPhysicsScene();
			Vector3 direction = _objectTransform.position - _baseCreep.transform.position;
			Vector3 origin = _baseCreep.transform.position + Vector3.up * 3.5f;
			if (physicsScene.Raycast(origin, direction, out var hitInfo, _aggroCapsule.height, CastLayers.PathfinderMask, QueryTriggerInteraction.Collide) && hitInfo.collider.gameObject.transform.root.TryGetComponent<Player>(out var component) && component._currentPlayerCondition == PlayerCondition.ACTIVE && !component._bufferingStatus && !component._isHidden && (!_baseCreep._scriptCreep._ignoreHighLevelPlayer || component._pStats._currentLevel - _baseCreep._creepLevel < 6))
			{
				StatusEntity statusEntity = component._statusEntity;
				_baseCreep.Server_AggroIcon(1);
				_baseCreep._pathFindCtrl._blocked = false;
				_baseCreep.Network_aggroedEntity = statusEntity;
			}
		}
	}
}
