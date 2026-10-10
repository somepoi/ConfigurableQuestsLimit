using UnityEngine;

public class CreepAwareness : MonoBehaviour
{
	private Creep _baseCreep;

	[Header("Creep Awareness Behavior")]
	public SphereCollider _awarenessSphere;

	private float _awarenessChance;

	[Header("Runtime")]
	public StatusEntity _aggroTargetBeforeDeath;

	public float _interestToAggroBuffer;

	public float _assistAggroBuffer = 0.6f;

	public float _onDeathBuffer;

	private float _generalInterestBuffer;

	private bool _isInterested;

	private Vector3 _pointOfInterestSpot;

	private Player _lastInterestedPlayer;

	private float _lastInterestBuffer;

	private void Awake()
	{
		_baseCreep = GetComponentInParent<Creep>();
	}

	private void Update()
	{
		if (!_baseCreep.isServer)
		{
			base.enabled = false;
		}
		else if (_baseCreep._scriptCreep._canAggro && !_baseCreep._disableAggro)
		{
			float awarenessRadius = GameManager._current._statLogics._awarenessRadius;
			_awarenessSphere.radius = awarenessRadius;
			Handle_BufferParams();
			if (_baseCreep._currentCreepCondition != CreepCondition.ACTIVE)
			{
				_awarenessSphere.enabled = _onDeathBuffer > 0f;
				_generalInterestBuffer = 1f;
				_lastInterestBuffer = 0f;
				_isInterested = false;
			}
			else
			{
				_awarenessSphere.enabled = true;
				_aggroTargetBeforeDeath = null;
				Handle_InterestCondition();
			}
		}
	}

	private void Handle_BufferParams()
	{
		_awarenessChance = GameManager._current._statLogics._awarenessChance;
		if (_generalInterestBuffer > 0f)
		{
			_generalInterestBuffer -= Time.deltaTime;
		}
		else
		{
			_generalInterestBuffer = 0f;
		}
		if (_onDeathBuffer > 0f)
		{
			_onDeathBuffer -= Time.deltaTime;
		}
		else
		{
			_onDeathBuffer = 0f;
		}
		if (_assistAggroBuffer > 0f)
		{
			_assistAggroBuffer -= Time.deltaTime;
		}
		else
		{
			_assistAggroBuffer = 0f;
		}
		if (_interestToAggroBuffer > 0f)
		{
			_interestToAggroBuffer -= Time.deltaTime;
		}
		else
		{
			_interestToAggroBuffer = 0f;
		}
		if (_lastInterestBuffer > 0f)
		{
			_lastInterestBuffer -= Time.deltaTime;
			return;
		}
		_lastInterestBuffer = 0f;
		_lastInterestedPlayer = null;
	}

	public void Init_InterestPoint(Vector3 _position)
	{
		if (_baseCreep._currentCreepCondition == CreepCondition.ACTIVE && !_baseCreep.Network_aggroedEntity)
		{
			_baseCreep.Server_AggroIcon(0);
			_interestToAggroBuffer = GameManager._current._statLogics._awareToAggroTime;
			_generalInterestBuffer = 3.5f;
			_isInterested = true;
			_pointOfInterestSpot = _position;
		}
	}

	private void Handle_InterestCondition()
	{
		if (!_baseCreep.Network_aggroedEntity)
		{
			_isInterested = _generalInterestBuffer > 0f;
			if (_isInterested)
			{
				Vector3 forward = _pointOfInterestSpot - _baseCreep.transform.position;
				forward.y = 0f;
				_baseCreep._pathFindCtrl._target = null;
				_baseCreep._pathFindCtrl.Disable_Pathfinding(0.65f);
				_baseCreep.transform.rotation = Quaternion.Lerp(_baseCreep.transform.rotation, Quaternion.LookRotation(forward), 9.5f * Time.deltaTime);
			}
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if (!(other.gameObject.scene != base.gameObject.scene))
		{
			if (other.gameObject.transform.root.TryGetComponent<Player>(out var component))
			{
				OnStay_PlayerObject(component);
			}
			if (other.gameObject.transform.root.TryGetComponent<CreepBehavior>(out var component2))
			{
				OnStay_CreepObject(component2);
			}
			if (other.gameObject.transform.root.TryGetComponent<ProjectileObject>(out var component3))
			{
				OnStay_ProjectileObject(component3);
			}
		}
		void OnStay_CreepObject(CreepBehavior _cB)
		{
			if (!(_cB == _baseCreep._creepBehavior) && !(_assistAggroBuffer > 0f))
			{
				StatusEntity statusEntity = null;
				if (_baseCreep._parentCreepSpawner._playersWithinSpawnerRadius.Count > 0 || !_baseCreep._parentCreepSpawner._resetAggroedCreepsIfEmptyRadius)
				{
					if ((bool)_baseCreep.Network_aggroedEntity && _baseCreep._currentCreepCondition == CreepCondition.ACTIVE)
					{
						statusEntity = _baseCreep.Network_aggroedEntity;
					}
					if ((bool)_aggroTargetBeforeDeath && _baseCreep._currentCreepCondition == CreepCondition.DEAD)
					{
						statusEntity = _aggroTargetBeforeDeath;
					}
					if ((bool)statusEntity && !(_cB == null) && (bool)_cB._baseCreep)
					{
						Creep baseCreep = _cB._baseCreep;
						if (baseCreep._currentCreepCondition == CreepCondition.ACTIVE && baseCreep._parentCreepSpawner._playersWithinSpawnerRadius.Count > 0 && !baseCreep.Network_aggroedEntity && (baseCreep._scriptCreep._creepAssistType != CreepAssistType.ASSIST_SAME_CREEP || !(_baseCreep._scriptCreep != baseCreep._scriptCreep)) && baseCreep._scriptCreep._creepAssistType != CreepAssistType.NO_ASSISTANCE)
						{
							baseCreep.Network_aggroedEntity = statusEntity;
							baseCreep.Server_AddThreat(statusEntity, 5f);
							baseCreep.Server_AggroIcon(1);
							_assistAggroBuffer = 0.35f;
						}
					}
				}
			}
		}
		void OnStay_PlayerObject(Player _p)
		{
			if (_p._currentPlayerCondition == PlayerCondition.ACTIVE && !_p._bufferingStatus && !_baseCreep.Network_aggroedEntity && _baseCreep.CanDetectAggroTarget(other.gameObject) && !_isInterested && !(_generalInterestBuffer > 0f) && (!(_lastInterestedPlayer == _p) || !(_lastInterestBuffer > 0f)))
			{
				if (Random.Range(0f, 1f) <= _awarenessChance)
				{
					Vector3 position = _p.transform.position;
					position.y = 0f;
					Init_InterestPoint(position);
				}
				else
				{
					_generalInterestBuffer = 3.5f;
				}
				_lastInterestedPlayer = _p;
				_lastInterestBuffer = 12.5f;
			}
		}
		void OnStay_ProjectileObject(ProjectileObject _projObj)
		{
			if (!_baseCreep.Network_aggroedEntity && (bool)_projObj && (bool)_projObj._parentTransform && (bool)_projObj._parentTransform.GetComponent<Player>() && !_isInterested && !(_generalInterestBuffer > 0f))
			{
				if ((float)Random.Range(0, 101) <= _awarenessChance)
				{
					Vector3 position = _projObj.transform.position;
					position.y = 0f;
					Init_InterestPoint(position);
				}
				else
				{
					_generalInterestBuffer = 3.5f;
				}
			}
		}
	}
}
