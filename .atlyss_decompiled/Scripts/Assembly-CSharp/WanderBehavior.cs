using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(RaycastPathFinder))]
public class WanderBehavior : NetworkBehaviour
{
	private RaycastPathFinder _pathFindCtrl;

	private PhysicsScene _physicsScene;

	private Creep _isCreep;

	[Header("Wandering Behavior")]
	public float _wanderResetFloor = 2.5f;

	public float _wanderResetCeiling = 12.85f;

	private float _wanderResetTimer;

	private float _currentWanderResetTimer;

	private Transform _wanderPositionTarget;

	[Space]
	public bool _disableWanderBehavior;

	private float _giveUpWallTime;

	private void Start()
	{
		if (base.isClientOnly)
		{
			base.enabled = false;
		}
		_physicsScene = base.gameObject.scene.GetPhysicsScene();
		_isCreep = GetComponent<Creep>();
		_pathFindCtrl = GetComponent<RaycastPathFinder>();
	}

	private void Update()
	{
		Handle_WanderingBehavior();
	}

	[ServerCallback]
	private void Handle_WanderingBehavior()
	{
		if (!NetworkServer.active)
		{
			return;
		}
		if (_wanderPositionTarget == null)
		{
			GameObject gameObject = new GameObject($"[netID: {base.netIdentity.netId}] _wanderPositioner");
			SceneManager.MoveGameObjectToScene(gameObject, base.gameObject.scene);
			gameObject.transform.position = base.transform.position;
			_wanderPositionTarget = gameObject.transform;
			_pathFindCtrl._target = null;
			_currentWanderResetTimer = 0f;
		}
		if (_disableWanderBehavior || ((bool)_isCreep && (bool)_isCreep.Network_aggroedEntity))
		{
			return;
		}
		if (_pathFindCtrl._pathFindNodes.Length == 0)
		{
			_currentWanderResetTimer += Time.deltaTime;
			if (_currentWanderResetTimer >= _wanderResetTimer && (bool)_wanderPositionTarget)
			{
				New_WanderPosition();
				_wanderResetTimer = Random.Range(_wanderResetFloor, _wanderResetCeiling);
				_currentWanderResetTimer = 0f;
				_giveUpWallTime = 0f;
			}
		}
		else if ((bool)_isCreep && (bool)_isCreep._parentCreepSpawner && _isCreep._parentCreepSpawner._pathFindNodes.Length != 0 && _pathFindCtrl._target == null)
		{
			_pathFindCtrl.CurrentNodeIndex = 0;
			_pathFindCtrl._currentTimeBetweenNodes = 0f;
			_pathFindCtrl._target = _isCreep._parentCreepSpawner._pathFindNodes[0];
		}
		if ((bool)_wanderPositionTarget && _pathFindCtrl._target == _wanderPositionTarget)
		{
			if (_physicsScene.Raycast(base.transform.position, base.transform.forward, 12.5f, CastLayers.GroundMask))
			{
				_giveUpWallTime += Time.deltaTime;
			}
			if (_giveUpWallTime >= 1.25f)
			{
				_wanderPositionTarget.transform.position = base.transform.position;
			}
		}
	}

	[ServerCallback]
	public void New_WanderPosition()
	{
		if (NetworkServer.active && (bool)_isCreep && (bool)_isCreep._parentCreepSpawner && (bool)_isCreep._parentCreepSpawner._mainCollider)
		{
			Vector3 vector = RandomNavSphere(_isCreep._parentCreepSpawner.transform.position, _isCreep._parentCreepSpawner._mainCollider.bounds.extents.magnitude * 0.46f, CastLayers.GroundMask);
			if (!(vector == Vector3.zero))
			{
				_pathFindCtrl._target = _wanderPositionTarget;
				_wanderPositionTarget.position = vector;
			}
		}
	}

	[ServerCallback]
	private Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
	{
		if (!NetworkServer.active)
		{
			return default;
		}
		Vector3 origin2 = origin + Random.insideUnitSphere * dist;
		origin2.y = base.transform.position.y;
		origin2 += Vector3.up * 35.85f;
		if (_physicsScene.Raycast(origin2, Vector3.down, out var hitInfo, float.PositiveInfinity, layermask))
		{
			return hitInfo.point;
		}
		return Vector3.zero;
	}

	private void OnDestroy()
	{
		if (_wanderPositionTarget != null)
		{
			Object.Destroy(_wanderPositionTarget.gameObject);
		}
	}

	public override bool Weaved()
	{
		return true;
	}
}
