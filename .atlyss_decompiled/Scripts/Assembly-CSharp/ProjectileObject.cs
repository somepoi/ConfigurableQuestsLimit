using System.Collections;
using MilkShake;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ProjectileObject : MonoBehaviour
{
	private Rigidbody _rb;

	private CombatCollider[] _dmgColliders;

	private float _clientside_stopBuffer;

	[Header("Spawn Conditions")]
	public DelaySpawn[] _delaySpawns;

	[SerializeField]
	private bool _delaySpawnsOnAwake = true;

	[SerializeField]
	private bool _delaySpawnsOnStop;

	[SerializeField]
	private bool _delaySpawnsOnGroundCollide;

	[SerializeField]
	private bool _endSpawnsOnStop;

	public bool _delaySpawnsCallback;

	[Header("Projectile Transform")]
	public Transform _parentTransform;

	public Transform _targetTransform;

	public Vector3 _offsetPosition = Vector3.zero;

	public float _forwardPosMod;

	public bool _applyPositionToParent;

	public bool _applyRotationToParent = true;

	public bool _ignoreYAxisRotation;

	[Header("Projectile Speed")]
	[SerializeField]
	private float _projectileForwardSpeed = 0.65f;

	[SerializeField]
	private float _forwardSpeedModifyOnCollide;

	[SerializeField]
	private float _timeBeforeMovement;

	[SerializeField]
	private bool _stackSpeedModify;

	[SerializeField]
	private bool _alwaysKinematic;

	public float _currentProjectileForwardSpeed;

	private bool _speedModfied;

	private bool _isStopped;

	private bool _initMovement;

	[Header("Projectile Gravity")]
	[SerializeField]
	private float _gravity;

	[SerializeField]
	private float _projectileUpwardsForce;

	[Header("Projectile Bounce")]
	[SerializeField]
	private float _bounceVelocity;

	[Header("Projectile Lifetime")]
	[SerializeField]
	private float _projectileLifetime = 4.5f;

	[SerializeField]
	private float _lifeTimeModifyOnCollide;

	[SerializeField]
	private float _returnToObjectPoolTime = 1.5f;

	[SerializeField]
	private float _targetTransformTime;

	[SerializeField]
	private bool _stackLifeTimeModify = true;

	[SerializeField]
	private bool _destroyOnStop;

	private float _currentLifetimeValue;

	private float _currentTargetTransformBuffer;

	private bool _lifeTimeModified;

	[Header("Raycast Directory")]
	[SerializeField]
	private bool _useRaycastPrediction;

	[SerializeField]
	private float _raycastLength = 25.65f;

	[SerializeField]
	private Vector3 _rayHitPoint = Vector3.zero;

	[Header("Boomerang Effect")]
	[SerializeField]
	private bool _returnToParent;

	[SerializeField]
	private float _timeBeforeReturnToParent = 1.5f;

	[SerializeField]
	private float _returnSpeed = 95.5f;

	[SerializeField]
	private float _returnDistDetection = 12.65f;

	[Header("Projectile Collision Callback")]
	[SerializeField]
	private bool _stopOnGroundCollide = true;

	[SerializeField]
	private bool _stopOnEntityCollide = true;

	[SerializeField]
	private bool _disableDamageColliderOnStop;

	[SerializeField]
	private bool _stopIfOnSeperateScene;

	[SerializeField]
	private bool _stopVelocityOnEntityCollide;

	[Header("Projectile Visual")]
	[SerializeField]
	private GameObject _projectileVisual;

	private Transform[] _cachedVisualObjects;

	[Space]
	[SerializeField]
	private MeshFilter _meshFilter;

	[SerializeField]
	private MeshRenderer _meshRenderer;

	[SerializeField]
	private bool _useWeaponMesh;

	[Space]
	[SerializeField]
	private Animator _projectileAnim;

	[SerializeField]
	private ParticleSystem _projectileOnStopEffect;

	[SerializeField]
	private bool _collisionAnimation = true;

	[Space]
	[SerializeField]
	private GameObject[] _visualsToHideOnStopped;

	private ParticleSystem[] _particleCache;

	[Header("Camera Shake")]
	[SerializeField]
	private float _distanceToShakeCamera;

	[SerializeField]
	private ShakePreset _shakePreset;

	[Header("Projectile Audio")]
	[SerializeField]
	private AudioSource _aSrcOnStop;

	[SerializeField]
	private AudioSource _aSrcOnGroundCollide;

	[SerializeField]
	private AudioSource _aSrcOnEntityCollide;

	private AudioSource[] _aSrcCache;

	private bool _isReturningToPool;

	private float _returnToParentTimer;

	private float _forceReturnParentTimer;

	private void OnValidate()
	{
		if (_delaySpawnsOnAwake)
		{
			_delaySpawnsOnStop = false;
		}
		if (_delaySpawnsOnStop)
		{
			_delaySpawnsOnAwake = false;
			_delaySpawnsOnGroundCollide = false;
		}
		if (_endSpawnsOnStop)
		{
			_delaySpawnsOnStop = false;
		}
		if (_timeBeforeMovement < 0f)
		{
			_timeBeforeMovement = 0f;
		}
		if (_bounceVelocity < 0f)
		{
			_bounceVelocity = 0f;
		}
		if (_useRaycastPrediction)
		{
			_alwaysKinematic = true;
		}
	}

	public virtual void Awake()
	{
		_rb = GetComponent<Rigidbody>();
		_aSrcCache = GetComponentsInChildren<AudioSource>(includeInactive: true);
		_particleCache = GetComponentsInChildren<ParticleSystem>(includeInactive: true);
		_dmgColliders = GetComponentsInChildren<CombatCollider>(includeInactive: true);
		if ((bool)_projectileVisual)
		{
			_cachedVisualObjects = _projectileVisual.GetComponentsInChildren<Transform>(includeInactive: true);
		}
	}

	public virtual void OnEnable()
	{
		Init_OnEnableFunction();
	}

	private void Init_OnEnableFunction()
	{
		if ((bool)Player._mainPlayer)
		{
			if (_visualsToHideOnStopped.Length != 0)
			{
				for (int i = 0; i < _visualsToHideOnStopped.Length; i++)
				{
					if ((bool)_visualsToHideOnStopped[i].GetComponent<ParticleSystem>())
					{
						_visualsToHideOnStopped[i].GetComponent<ParticleSystem>().Play();
					}
					else
					{
						_visualsToHideOnStopped[i].SetActive(value: true);
					}
				}
			}
			if (Player._mainPlayer._isHostPlayer)
			{
				for (int j = 0; j < _particleCache.Length; j++)
				{
					if (_particleCache[j].main.playOnAwake && Player._mainPlayer.gameObject.scene != base.gameObject.scene)
					{
						_particleCache[j].Stop();
					}
				}
				if ((bool)_projectileVisual)
				{
					_projectileVisual.SetActive(Player._mainPlayer.gameObject.scene == base.gameObject.scene);
				}
			}
		}
		_targetTransform = null;
		_currentProjectileForwardSpeed = _projectileForwardSpeed;
		_currentLifetimeValue = _projectileLifetime;
		_clientside_stopBuffer = 0f;
		_returnToParentTimer = 0f;
		_forceReturnParentTimer = 0f;
		_rayHitPoint = Vector3.zero;
		_speedModfied = false;
		_lifeTimeModified = false;
		if (_dmgColliders != null && _dmgColliders.Length != 0)
		{
			for (int k = 0; k < _dmgColliders.Length; k++)
			{
				_dmgColliders[k]._damageBufferPool.Clear();
				_dmgColliders[k]._disableCollider = false;
			}
		}
		if (!_rb.isKinematic)
		{
			_rb.velocity = Vector3.zero;
		}
		base.transform.position += _offsetPosition;
		if ((bool)GetComponent<Collider>())
		{
			GetComponent<Collider>().enabled = true;
		}
		_isStopped = false;
		_initMovement = false;
		if (!_alwaysKinematic)
		{
			_rb.isKinematic = false;
		}
		if (_delaySpawnsOnAwake)
		{
			Init_DelaySpawns();
		}
		if ((bool)_projectileAnim)
		{
			_projectileAnim.CrossFade("Spawn", 0f, 0);
		}
		Assign_ParentParameters();
		for (int l = 0; l < _aSrcCache.Length; l++)
		{
			_aSrcCache[l].Stop();
		}
		if (_targetTransformTime > 0f)
		{
			_currentTargetTransformBuffer = _targetTransformTime;
		}
		Invoke("Begin_ProjectileEffects", 0.04f);
		Invoke("Begin_ShakeEffect", 0.04f);
	}

	public void Assign_ParentParameters()
	{
		CombatCollider[] componentsInChildren = GetComponentsInChildren<CombatCollider>(includeInactive: true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (!(componentsInChildren[i] == null))
			{
				if (!_parentTransform)
				{
					break;
				}
				componentsInChildren[i]._parentObject = _parentTransform.gameObject;
				componentsInChildren[i].Get_ParentObjectInfo();
			}
		}
	}

	private void Begin_ProjectileEffects()
	{
		if (!Player._mainPlayer || (Player._mainPlayer._isHostPlayer && base.gameObject.scene != Player._mainPlayer.gameObject.scene))
		{
			return;
		}
		if ((bool)_meshFilter && _useWeaponMesh && (bool)_parentTransform.GetComponentInChildren<RaceModelEquipDisplay>())
		{
			RaceModelEquipDisplay componentInChildren = _parentTransform.GetComponentInChildren<RaceModelEquipDisplay>();
			_meshFilter.sharedMesh = componentInChildren._rightHandWeaponRender.GetComponent<MeshFilter>().sharedMesh;
			_meshRenderer.material = componentInChildren._rightHandWeaponRender.material;
		}
		for (int i = 0; i < _aSrcCache.Length; i++)
		{
			if (_aSrcCache[i].enabled && _aSrcCache[i].playOnAwake)
			{
				_aSrcCache[i].mute = false;
				if (_aSrcCache[i].enabled)
				{
					_aSrcCache[i].Play();
				}
			}
		}
	}

	private void Begin_ShakeEffect()
	{
		if ((bool)Player._mainPlayer && (!Player._mainPlayer._isHostPlayer || !(base.gameObject.scene != Player._mainPlayer.gameObject.scene)) && !(_distanceToShakeCamera <= 0f))
		{
			ShakePreset shakePreset = CameraFunction._current.ShakePreset;
			if ((bool)_shakePreset)
			{
				shakePreset = _shakePreset;
			}
			if (Vector3.Distance(base.transform.position, Player._mainPlayer.transform.position) <= _distanceToShakeCamera)
			{
				CameraFunction._current.MyShaker.Shake(shakePreset);
			}
		}
	}

	public void Init_DelaySpawns()
	{
		if (base.gameObject.activeInHierarchy)
		{
			for (int i = 0; i < _delaySpawns.Length; i++)
			{
				StartCoroutine(SpawnDelay_Object(i));
			}
		}
	}

	private IEnumerator SpawnDelay_Object(int _index)
	{
		yield return new WaitForSeconds(_delaySpawns[_index]._timeDelay);
		GameObject gameObject = ((!_delaySpawns[_index]._useObjectPool) ? Object.Instantiate(_delaySpawns[_index]._objectToSpawn) : PoolManager._current.GenerateObject(_delaySpawns[_index]._objectToSpawn, string.Empty, 0f));
		ProjectileObject componentInChildren = gameObject.GetComponentInChildren<ProjectileObject>(includeInactive: true);
		CombatCollider[] componentsInChildren = gameObject.GetComponentsInChildren<CombatCollider>(includeInactive: true);
		if ((bool)componentInChildren)
		{
			componentInChildren._parentTransform = _parentTransform;
		}
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (!(componentsInChildren[i] == null))
			{
				if (!_parentTransform)
				{
					break;
				}
				componentsInChildren[i]._parentObject = _parentTransform.gameObject;
				if ((bool)GetComponentInChildren<CombatCollider>(includeInactive: true))
				{
					componentsInChildren[i]._scriptWeapon = GetComponentInChildren<CombatCollider>(includeInactive: true)._scriptWeapon;
					componentsInChildren[i]._combatElement = GetComponentInChildren<CombatCollider>(includeInactive: true)._combatElement;
					componentsInChildren[i]._skillObject = GetComponentInChildren<CombatCollider>(includeInactive: true)._skillObject;
				}
			}
		}
		SkillObject component = GetComponent<SkillObject>();
		SkillObject component2 = gameObject.GetComponent<SkillObject>();
		if ((bool)component2 && (bool)component)
		{
			component2._scriptSkill = component._scriptSkill;
			if ((bool)component2._parentPlayer)
			{
				component2._parentPlayer = component._parentPlayer;
			}
			if ((bool)component2._parentCreep)
			{
				component2._parentCreep = component._parentCreep;
			}
			for (int j = 0; j < componentsInChildren.Length; j++)
			{
				if (!(componentsInChildren[j] == null))
				{
					if (!_parentTransform)
					{
						break;
					}
					componentsInChildren[j]._scriptableConditionSlot = component2._scriptSkill._skillRankParams._skillObjectCondition;
				}
			}
		}
		gameObject.transform.SetParent(base.transform);
		gameObject.transform.SetParent(null);
		if (_delaySpawns[_index]._copyParentRotation)
		{
			gameObject.transform.SetPositionAndRotation(base.gameObject.transform.position, base.transform.rotation);
			gameObject.transform.rotation = Quaternion.Euler(gameObject.transform.eulerAngles + _delaySpawns[_index]._rotationAdjust);
		}
		else
		{
			gameObject.transform.SetPositionAndRotation(base.gameObject.transform.position, Quaternion.Euler(Vector3.zero + _delaySpawns[_index]._rotationAdjust));
		}
		gameObject.transform.localScale = Vector3.one;
	}

	public virtual void Update()
	{
		Handle_ProjectileLifetime();
		Handle_ProjectileHostDisplay();
		if ((bool)Player._mainPlayer && Player._mainPlayer.isClientOnly && _clientside_stopBuffer < 0.12f)
		{
			_clientside_stopBuffer += Time.deltaTime;
		}
		if ((bool)_parentTransform)
		{
			if (_applyPositionToParent)
			{
				_rb.isKinematic = true;
				base.transform.position = _parentTransform.position + _offsetPosition;
			}
			if (_forwardPosMod > 0f)
			{
				_rb.isKinematic = true;
				base.transform.position = _parentTransform.position + _parentTransform.forward * _forwardPosMod;
			}
		}
		if ((bool)_targetTransform)
		{
			base.transform.LookAt(_targetTransform.position + Vector3.up * 3.5f);
			if (_currentTargetTransformBuffer > 0f)
			{
				_currentTargetTransformBuffer -= Time.deltaTime;
			}
			else
			{
				_currentTargetTransformBuffer = 0f;
			}
			if (_targetTransformTime > 0f && _currentTargetTransformBuffer <= 0f)
			{
				_targetTransform = null;
			}
		}
		if (_useRaycastPrediction)
		{
			if (base.gameObject.scene.GetPhysicsScene().Raycast(base.transform.position + -base.transform.forward * 2.5f, base.transform.forward, out var hitInfo, _raycastLength, CastLayers.GroundMask))
			{
				if (_rayHitPoint == Vector3.zero)
				{
					_rayHitPoint = hitInfo.point;
				}
			}
			else if (_rayHitPoint == Vector3.zero)
			{
				_rayHitPoint = base.transform.position + _raycastLength * base.transform.forward;
			}
			if (_rayHitPoint != Vector3.zero)
			{
				base.transform.LookAt(_rayHitPoint);
				if (Vector3.Distance(base.transform.position, _rayHitPoint) <= 6.5f)
				{
					Init_StopProjectile();
				}
				else if (!_isStopped)
				{
					base.transform.Translate(_currentProjectileForwardSpeed * 85f * Time.deltaTime * Vector3.forward);
				}
			}
		}
		if (_dmgColliders != null && _dmgColliders.Length != 0)
		{
			for (int i = 0; i < _dmgColliders.Length; i++)
			{
				if (_dmgColliders[i]._disableCollider)
				{
					Init_StopProjectile();
				}
			}
		}
		if ((bool)_parentTransform && _parentTransform.gameObject.scene != base.gameObject.scene && _stopIfOnSeperateScene)
		{
			Init_StopProjectile();
		}
	}

	private void Handle_ProjectileLifetime()
	{
		if (_returnToParent)
		{
			if (_returnToParentTimer < _timeBeforeReturnToParent)
			{
				_returnToParentTimer += Time.deltaTime;
			}
			else
			{
				base.transform.LookAt(_parentTransform);
				_rb.AddRelativeForce(Vector3.forward * _returnSpeed, ForceMode.Force);
				GetComponent<Collider>().enabled = false;
				if (Vector3.Distance(base.transform.position, _parentTransform.position) <= _returnDistDetection)
				{
					Init_StopProjectile();
				}
				_forceReturnParentTimer += Time.deltaTime;
				if (_forceReturnParentTimer >= 1.2f)
				{
					Init_StopProjectile();
				}
			}
		}
		if (!(_projectileLifetime <= 0f) && !_isStopped)
		{
			if (_currentLifetimeValue > 0f)
			{
				_currentLifetimeValue -= Time.deltaTime;
			}
			else
			{
				Init_StopProjectile();
			}
		}
	}

	private void Handle_ProjectileHostDisplay()
	{
		if (!Player._mainPlayer || !Player._mainPlayer._isHostPlayer || _cachedVisualObjects == null || _cachedVisualObjects.Length == 0)
		{
			return;
		}
		if (base.gameObject.scene != Player._mainPlayer.gameObject.scene)
		{
			for (int i = 0; i < _cachedVisualObjects.Length; i++)
			{
				if (_cachedVisualObjects[i].gameObject.layer != CastLayers.CullLayer)
				{
					_cachedVisualObjects[i].gameObject.layer = CastLayers.CullLayer;
				}
			}
			for (int j = 0; j < _aSrcCache.Length; j++)
			{
				_aSrcCache[j].mute = true;
			}
			return;
		}
		for (int k = 0; k < _cachedVisualObjects.Length; k++)
		{
			if (_cachedVisualObjects[k].gameObject.layer != CastLayers.DefaultLayer)
			{
				_cachedVisualObjects[k].gameObject.layer = CastLayers.DefaultLayer;
			}
		}
		for (int l = 0; l < _aSrcCache.Length; l++)
		{
			_aSrcCache[l].mute = false;
		}
	}

	public virtual void FixedUpdate()
	{
		if (_isStopped)
		{
			return;
		}
		PhysicsScene physicsScene = base.gameObject.scene.GetPhysicsScene();
		if (_returnToParentTimer >= _timeBeforeReturnToParent)
		{
			base.transform.LookAt(_parentTransform);
			_rb.AddRelativeForce(Vector3.forward * _returnSpeed, ForceMode.Impulse);
			GetComponent<Collider>().enabled = false;
			if (Vector3.Distance(base.transform.position, _parentTransform.position) <= _returnDistDetection)
			{
				Init_StopProjectile();
			}
		}
		if (_stopOnGroundCollide)
		{
			if (!_useRaycastPrediction && physicsScene.Raycast(base.transform.position + -base.transform.forward * 2.5f, base.transform.forward, 6.5f, CastLayers.GroundMask))
			{
				Init_StopProjectile();
			}
			if (physicsScene.Raycast(base.transform.position, Vector3.down, 1.5f, CastLayers.GroundMask) && _gravity > 0f)
			{
				if (_delaySpawnsOnGroundCollide)
				{
					Init_DelaySpawns();
				}
				Init_StopProjectile();
			}
		}
		if (_currentProjectileForwardSpeed < 0f)
		{
			_currentProjectileForwardSpeed = 0f;
		}
		if (!_initMovement && !_alwaysKinematic && !IsInvoking("Init_Movement"))
		{
			Invoke("Init_Movement", _timeBeforeMovement);
		}
		if (_initMovement && _gravity != 0f)
		{
			_rb.AddForce(_gravity * Time.deltaTime * Vector3.down, ForceMode.Impulse);
		}
	}

	public void Init_Movement()
	{
		Vector3 force = _currentProjectileForwardSpeed * 85f * Vector3.forward;
		Vector3 force2 = _projectileUpwardsForce * 85f * Vector3.up;
		if (!_rb.isKinematic)
		{
			_rb.velocity = Vector3.zero;
		}
		_rb.AddRelativeForce(force, ForceMode.Impulse);
		_rb.AddRelativeForce(force2, ForceMode.Impulse);
		_initMovement = true;
	}

	public virtual void OnTriggerEnter(Collider _other)
	{
		if (_isStopped)
		{
			return;
		}
		if ((bool)_other.GetComponentInParent<Player>())
		{
			Player componentInParent = _other.GetComponentInParent<Player>();
			if (componentInParent._statusEntity._deathBuffer >= 1.2f || componentInParent._pPvp._currentVersusFlag == VersusFlag.PVE || componentInParent._bufferingStatus)
			{
				return;
			}
			if (_dmgColliders != null && _dmgColliders.Length != 0)
			{
				for (int i = 0; i < _dmgColliders.Length; i++)
				{
					if (componentInParent.gameObject == _dmgColliders[i]._parentObject)
					{
						return;
					}
				}
			}
		}
		else if ((bool)_other.GetComponentInParent<CreepBehavior>())
		{
			Creep baseCreep = _other.GetComponentInParent<CreepBehavior>()._baseCreep;
			if (baseCreep._statusEntity._deathBuffer >= 1.2f)
			{
				return;
			}
			if (_dmgColliders != null && _dmgColliders.Length != 0)
			{
				for (int j = 0; j < _dmgColliders.Length; j++)
				{
					if (baseCreep.gameObject == _dmgColliders[j]._parentObject)
					{
						return;
					}
				}
			}
		}
		else if (_other.gameObject.layer == LayerMask.NameToLayer("Ground"))
		{
			if (_delaySpawnsOnGroundCollide)
			{
				Init_DelaySpawns();
			}
			if ((bool)_aSrcOnGroundCollide)
			{
				_aSrcOnGroundCollide.Play();
			}
			if (_stopOnGroundCollide)
			{
				Init_StopProjectile();
			}
			else if (_bounceVelocity > 0f)
			{
				_rb.velocity = new Vector3(_rb.velocity.x, _bounceVelocity, _rb.velocity.z);
			}
		}
		if (_stopOnEntityCollide && _dmgColliders == null)
		{
			Init_StopProjectile();
		}
		if ((bool)_projectileAnim && _collisionAnimation)
		{
			_projectileAnim.CrossFade("Collide", 0f, 0);
		}
	}

	public void Projectile_EntityCollideCallback(Transform _hitTransform)
	{
		if (_hitTransform == _parentTransform || ((bool)Player._mainPlayer && Player._mainPlayer.isClientOnly && (bool)_parentTransform && !_parentTransform.GetComponent<Player>() && _clientside_stopBuffer < 0.06f))
		{
			return;
		}
		if ((bool)_hitTransform.GetComponentInParent<Player>())
		{
			Player componentInParent = _hitTransform.GetComponentInParent<Player>();
			if (componentInParent._statusEntity._deathBuffer >= 1.2f || componentInParent._bufferingStatus || ((bool)_parentTransform && (bool)_parentTransform.GetComponent<Player>() && componentInParent._pPvp._currentVersusFlag == VersusFlag.PVE))
			{
				return;
			}
		}
		if (_stopOnEntityCollide && !_isStopped)
		{
			Init_StopProjectile();
		}
		if (_stopVelocityOnEntityCollide)
		{
			_rb.isKinematic = true;
		}
		if ((bool)_aSrcOnEntityCollide && !_aSrcOnEntityCollide.isPlaying)
		{
			_aSrcOnEntityCollide.Play();
		}
		Modify_ProjectileLifeTime();
		Modify_ProjectileSpeed();
	}

	private void Modify_ProjectileLifeTime()
	{
		if (_lifeTimeModifyOnCollide != 0f && (_stackLifeTimeModify || !_lifeTimeModified))
		{
			_currentLifetimeValue += _lifeTimeModifyOnCollide;
			_lifeTimeModified = true;
		}
	}

	private void Modify_ProjectileSpeed()
	{
		if (_forwardSpeedModifyOnCollide != 0f && (_stackSpeedModify || !_speedModfied))
		{
			_currentProjectileForwardSpeed += _forwardSpeedModifyOnCollide;
			_speedModfied = true;
		}
	}

	public void Init_StopProjectile()
	{
		if (_isStopped)
		{
			return;
		}
		if (_visualsToHideOnStopped.Length != 0)
		{
			for (int i = 0; i < _visualsToHideOnStopped.Length; i++)
			{
				if ((bool)_visualsToHideOnStopped[i].GetComponent<ParticleSystem>())
				{
					_visualsToHideOnStopped[i].GetComponent<ParticleSystem>().Stop();
				}
				else
				{
					_visualsToHideOnStopped[i].SetActive(value: false);
				}
			}
			if ((bool)_projectileOnStopEffect && (bool)Player._mainPlayer && Player._mainPlayer.gameObject.scene == base.gameObject.scene)
			{
				_projectileOnStopEffect.Play();
			}
		}
		if ((bool)_rb)
		{
			if (!_alwaysKinematic)
			{
				_rb.isKinematic = false;
			}
			else
			{
				_rb.isKinematic = true;
			}
			if (!_rb.isKinematic)
			{
				_rb.velocity = Vector3.zero;
			}
		}
		_isStopped = true;
		if ((bool)GetComponentInParent<SkillObjectBehavior>())
		{
			GetComponentInParent<SkillObjectBehavior>().End_SkillBehavior();
		}
		if (_dmgColliders != null)
		{
			for (int j = 0; j < _dmgColliders.Length; j++)
			{
				if (_disableDamageColliderOnStop)
				{
					_dmgColliders[j]._disableCollider = true;
				}
			}
		}
		if (_delaySpawnsOnStop)
		{
			Init_DelaySpawns();
		}
		if ((bool)_aSrcOnStop && _aSrcOnStop.enabled && (bool)Player._mainPlayer && Player._mainPlayer.gameObject.scene == base.gameObject.scene)
		{
			_aSrcOnStop.pitch = Random.Range(0.9874612f, 1.156439f);
			_aSrcOnStop.Play();
		}
		if (_endSpawnsOnStop)
		{
			StopAllCoroutines();
		}
		if (_destroyOnStop)
		{
			Object.Destroy(base.transform.root.gameObject, _returnToObjectPoolTime);
		}
		else if (!_isReturningToPool && base.gameObject.activeInHierarchy)
		{
			StartCoroutine(ReturnObjectToPool());
		}
	}

	private IEnumerator ReturnObjectToPool()
	{
		_isReturningToPool = true;
		yield return new WaitForSeconds(_returnToObjectPoolTime);
		_parentTransform = null;
		SkillObject component = GetComponent<SkillObject>();
		if ((bool)component)
		{
			component._parentPlayer = null;
			component._parentCreep = null;
		}
		PoolManager._current.Init_ReturnObjectToContainer(base.gameObject);
		_isReturningToPool = false;
		_isStopped = false;
	}
}
