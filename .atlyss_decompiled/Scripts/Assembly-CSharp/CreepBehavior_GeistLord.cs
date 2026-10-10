using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreepBehavior_GeistLord : CreepBehavior
{
	private enum GeistLordAction
	{
		IDLE = 0,
		TELEPORT = 1,
		MELEE_SWIPE = 2,
		SHOOT_DARKORBS = 3,
		LASERBEAM = 4,
		MAGIC_SCYTHES = 5,
		SUMMON_MINIONS = 6
	}

	[Header("Geist Lord AI Behavior")]
	[SerializeField]
	private Collider _triggerCollider;

	[SerializeField]
	private GeistLordAction _currentAction;

	private GeistLordAction _lastAction;

	private bool _summonedMinions;

	private bool _beginIntroAnim;

	private bool _finishedIntroAnim;

	[SerializeField]
	private float _timeBeforeNextAction = 1.2f;

	private float _currentActionBuffer;

	private float _nextActionBuffer;

	[Space]
	[Range(0f, 1f)]
	[SerializeField]
	private float _teleportChance = 0.6f;

	[Range(0f, 1f)]
	[SerializeField]
	private float _teleportChanceOnHurt = 0.6f;

	[SerializeField]
	private float _backAwayForceBeforeTeleport = 155.5f;

	[SerializeField]
	private float _timeBeforeTeleport = 3.5f;

	[SerializeField]
	private float _teleportTimeFloor = 1f;

	[SerializeField]
	private float _teleportTimeCeiling = 2f;

	[SerializeField]
	private float _teleportBufferTime = 6.5f;

	private Transform _lastTeleportTransform;

	private float _teleportBuffer;

	[Space]
	[SerializeField]
	private float _preemptiveMeleeSwipeForce = 125.5f;

	[SerializeField]
	private float _meleeSwipeForwardForce = 200f;

	[SerializeField]
	private CombatCollider _meleeSwipeCollider;

	[SerializeField]
	private float _distanceToMeleeSwipe = 30f;

	[SerializeField]
	private float _timeBeforeMeleeSwipe = 0.6f;

	[SerializeField]
	private float _meleeSwipeCooldown = 6.5f;

	private bool _onMeleeSwipeCooldown;

	[Space]
	[Range(0f, 1f)]
	[SerializeField]
	private float _chanceToDarkOrb = 0.5f;

	[SerializeField]
	private Transform _darkOrbSpawnPosition;

	[SerializeField]
	private GameObject _darkOrbProjectile;

	[SerializeField]
	private float _timeBeforeDarkOrbs = 0.8f;

	[SerializeField]
	private float _darkOrbCooldown = 1.2f;

	[Space]
	[Range(0f, 1f)]
	[SerializeField]
	private float _chanceToMagicScythe = 0.5f;

	[SerializeField]
	private Transform _magicScytheSpawnPosition;

	[SerializeField]
	private GameObject _magicScytheProjectile;

	[SerializeField]
	private float _timeBeforeMagicScythe = 0.8f;

	[SerializeField]
	private float _magicScytheCooldown = 1.2f;

	[SerializeField]
	private float _distanceToScythe = 60f;

	[Space]
	[Range(0f, 1f)]
	[SerializeField]
	private float _chanceToLaserBeam = 0.5f;

	[SerializeField]
	private Transform _laserBeamSpawnPosition;

	[SerializeField]
	private GameObject _laserBeamProjectile;

	[SerializeField]
	private float _timeBeforeLaserBeam = 0.9f;

	[SerializeField]
	private float _laserBeamCooldown = 3.5f;

	public override void Behavior_OnServerFixedUpdate()
	{
	}

	public override void Behavior_OnHurt()
	{
		if (_currentAction == GeistLordAction.IDLE && Random.Range(0f, 1f) <= _teleportChanceOnHurt)
		{
			StartCoroutine(GeistLord_Teleport());
		}
	}

	public override void Update()
	{
		base.Update();
		if (_baseCreep.isClientOnly)
		{
			_triggerCollider.enabled = _baseCreep._isTargetable;
		}
		if (!_beginIntroAnim)
		{
			StartCoroutine(Init_AggroAnimation());
		}
		if (_finishedIntroAnim && _baseCreep.isServer && !_baseCreep.Network_aggroedEntity && _baseCreep._creepThreatSlots.Count <= 0)
		{
			Reset_Behavior();
		}
	}

	private IEnumerator Init_AggroAnimation()
	{
		_beginIntroAnim = true;
		if (_baseCreep.isServer)
		{
			_baseCreep.Network_isTargetable = false;
			_baseCreep._pathFindCtrl._disablePathing = true;
			_baseCreep._pathFindCtrl.Network_dontLookAtTarget = true;
			_baseCreep.Network_ignoreAggroedTarget = true;
			_baseCreep.Server_CrossFadeAnim("Aggro_Init", 0f, 1);
		}
		yield return new WaitForSeconds(1.5f);
		_finishedIntroAnim = true;
	}

	public override void Behavior_OnServerUpdate()
	{
		if (!_baseCreep.isServer)
		{
			return;
		}
		if (_currentActionBuffer > 0f)
		{
			_currentActionBuffer -= Time.deltaTime;
		}
		else if (_currentActionBuffer < 0f)
		{
			_currentActionBuffer = 0f;
		}
		if (!_finishedIntroAnim)
		{
			return;
		}
		if (_baseCreep._currentCreepCondition == CreepCondition.DEAD)
		{
			for (int i = 0; i < _baseCreep._parentCreepSpawner._linkedSpawners.Length; i++)
			{
				for (int j = 0; j < _baseCreep._parentCreepSpawner._linkedSpawners[i]._spawnedCreeps.Count; j++)
				{
					_baseCreep._parentCreepSpawner._linkedSpawners[i]._spawnedCreeps[j]._statusEntity.Network_currentHealth = 0;
				}
			}
		}
		if (_teleportBuffer > 0f)
		{
			_teleportBuffer -= Time.deltaTime;
		}
		else
		{
			_teleportBuffer = 0f;
		}
		_triggerCollider.enabled = _currentAction != GeistLordAction.TELEPORT;
		if (_currentAction == GeistLordAction.IDLE)
		{
			if (_nextActionBuffer < _timeBeforeNextAction)
			{
				_nextActionBuffer += Time.deltaTime;
				return;
			}
			Init_GeistLordAction();
			_nextActionBuffer = 0f;
		}
		else
		{
			_nextActionBuffer = 0f;
		}
	}

	public override void Reset_Behavior()
	{
		if (_baseCreep.isServer && (_baseCreep._parentCreepSpawner._playersWithinSpawnerRadius.Count <= 0 || _baseCreep._statusEntity._currentHealth <= 0) && _baseCreep._parentCreepSpawner._spawnedCreeps.Count > 0 && _summonedMinions)
		{
			for (int i = 0; i < _baseCreep._parentCreepSpawner._linkedSpawners.Length; i++)
			{
				_baseCreep._parentCreepSpawner._linkedSpawners[i].Disable_Spawner();
			}
			_summonedMinions = false;
		}
		StopAllCoroutines();
		if (_currentAction != GeistLordAction.IDLE)
		{
			_baseCreep.Server_CrossFadeAnim("Empty", 0.095f, 1);
		}
		_currentAction = GeistLordAction.IDLE;
		_currentActionBuffer = 1f;
		_baseCreep._pathFindCtrl._disablePathing = false;
		_baseCreep._pathFindCtrl.Network_dontLookAtTarget = false;
		_baseCreep.Network_ignoreAggroedTarget = false;
		_baseCreep.Network_isTargetable = true;
		_onMeleeSwipeCooldown = false;
	}

	private void Init_GeistLordAction()
	{
		if (_baseCreep._currentCreepCondition != CreepCondition.ACTIVE || _currentAction != GeistLordAction.IDLE || _currentActionBuffer != 0f || !_baseCreep.Network_aggroedEntity || !_baseCreep.isServer)
		{
			return;
		}
		float num = Random.Range(0f, 1.1f);
		if ((float)_baseCreep._statusEntity._currentHealth <= (float)_baseCreep._statStruct._maxHealth * 0.5f && !_summonedMinions)
		{
			StartCoroutine(GeistLord_Teleport());
			return;
		}
		if ((float)_baseCreep._statusEntity._currentHealth <= (float)_baseCreep._statStruct._maxHealth * 0.3f)
		{
			num -= 0.3f;
		}
		if (num <= _teleportChance && _lastAction != GeistLordAction.TELEPORT && _teleportBuffer <= 0f)
		{
			StartCoroutine(GeistLord_Teleport());
			return;
		}
		if (Random.Range(0f, 1.1f) <= _chanceToDarkOrb)
		{
			StartCoroutine(GeistLord_DarkOrbs());
			return;
		}
		float num2 = Random.Range(0f, 1.1f);
		if ((bool)_baseCreep.Network_aggroedEntity && _chanceToMagicScythe <= num2 && Vector3.Distance(base.transform.position, _baseCreep.Network_aggroedEntity.transform.position) <= _distanceToScythe)
		{
			StartCoroutine(GeistLord_MagicScythes());
		}
		else if (Vector3.Distance(_baseCreep.Network_aggroedEntity.transform.position, base.transform.position) <= _distanceToMeleeSwipe && !_onMeleeSwipeCooldown)
		{
			StartCoroutine(GeistLord_MeleeSwipe());
		}
	}

	private IEnumerator GeistLord_Teleport()
	{
		if (_currentAction != GeistLordAction.IDLE || _baseCreep._pathFindCtrl._pathFindNodes.Length == 0)
		{
			yield break;
		}
		_currentAction = GeistLordAction.TELEPORT;
		_lastAction = _currentAction;
		_baseCreep._pathFindCtrl._disablePathing = true;
		_baseCreep._creepRB.AddForce(-base.transform.forward * _backAwayForceBeforeTeleport, ForceMode.Impulse);
		yield return new WaitForSeconds(0.85f);
		_baseCreep.Server_CrossFadeAnim("specialAction_00", 0f, 1);
		_baseCreep._statusEntity.Clear_AllConditions();
		_baseCreep.Network_isTargetable = false;
		if ((float)_baseCreep._statusEntity._currentHealth <= (float)_baseCreep._statStruct._maxHealth * 0.5f && !_summonedMinions)
		{
			for (int i = 0; i < _baseCreep._parentCreepSpawner._linkedSpawners.Length; i++)
			{
				_baseCreep._parentCreepSpawner._linkedSpawners[i].Enable_Spawner();
			}
			_summonedMinions = true;
			yield return new WaitForSeconds(8.5f);
		}
		yield return new WaitForSeconds(_timeBeforeTeleport);
		List<Transform> list = new List<Transform>();
		for (int j = 0; j < _baseCreep._pathFindCtrl._pathFindNodes.Length; j++)
		{
			if (!(_lastTeleportTransform == _baseCreep._pathFindCtrl._pathFindNodes[j]))
			{
				list.Add(_baseCreep._pathFindCtrl._pathFindNodes[j]);
			}
		}
		_lastTeleportTransform = list[Random.Range(0, list.Count)];
		_baseCreep.transform.position = _lastTeleportTransform.position;
		_baseCreep._pathFindCtrl._disablePathing = false;
		yield return new WaitForSeconds(Random.Range(_teleportTimeFloor, _teleportTimeCeiling));
		_baseCreep.Server_CrossFadeAnim("Empty", 0.095f, 1);
		_baseCreep.Network_isTargetable = true;
		_currentAction = GeistLordAction.IDLE;
		_currentActionBuffer = 1.3f;
		float num = Random.Range(0f, 1f);
		if (_chanceToLaserBeam <= num || (float)_baseCreep._statusEntity._currentHealth <= (float)_baseCreep._statStruct._maxHealth * 0.3f)
		{
			StartCoroutine(GeistLord_LaserBeam());
		}
		_teleportBuffer = _teleportBufferTime;
	}

	public void Melee_SwipeForce()
	{
		_baseCreep._creepRB.AddForce(base.transform.forward * _meleeSwipeForwardForce, ForceMode.Impulse);
	}

	private IEnumerator GeistLord_MeleeSwipe()
	{
		if (_currentAction == GeistLordAction.IDLE)
		{
			_currentAction = GeistLordAction.MELEE_SWIPE;
			_lastAction = _currentAction;
			_meleeSwipeCollider._dmgType = DamageType.Strength;
			_onMeleeSwipeCooldown = true;
			_baseCreep._creepRB.AddForce(base.transform.forward * _preemptiveMeleeSwipeForce, ForceMode.Impulse);
			yield return new WaitForSeconds(_timeBeforeMeleeSwipe);
			_baseCreep.Server_CrossFadeAnim("Attack01", 0f, 1);
			_baseCreep._attackActionBuffer = 1.35f;
			yield return new WaitForSeconds(_meleeSwipeCooldown);
			_currentAction = GeistLordAction.IDLE;
			_currentActionBuffer = 1.3f;
			_onMeleeSwipeCooldown = false;
		}
	}

	private IEnumerator GeistLord_DarkOrbs()
	{
		if (_currentAction == GeistLordAction.IDLE)
		{
			_currentAction = GeistLordAction.SHOOT_DARKORBS;
			_lastAction = _currentAction;
			_baseCreep._pathFindCtrl._disablePathing = true;
			_baseCreep._creepRB.AddForce(-base.transform.forward * _backAwayForceBeforeTeleport, ForceMode.Impulse);
			yield return new WaitForSeconds(_timeBeforeDarkOrbs);
			_baseCreep.Server_CrossFadeAnim("Attack02", 0f, 1);
			yield return new WaitForSeconds(_darkOrbCooldown);
			_currentAction = GeistLordAction.IDLE;
			_baseCreep._pathFindCtrl._disablePathing = false;
			_currentActionBuffer = 1.3f;
		}
	}

	public void Init_DarkOrbExecution()
	{
		GameObject gameObject = PoolManager._current.GenerateObject(_darkOrbProjectile, string.Empty, 0f);
		gameObject.GetComponentInChildren<CombatCollider>()._parentObject = base.gameObject;
		gameObject.transform.SetParent(base.transform);
		gameObject.transform.SetParent(null);
		gameObject.transform.SetAsLastSibling();
		gameObject.transform.SetPositionAndRotation(_darkOrbSpawnPosition.position, Quaternion.Euler(0f, base.transform.eulerAngles.y, 0f));
		if ((bool)_baseCreep.Network_aggroedEntity)
		{
			gameObject.GetComponent<ProjectileObject>()._targetTransform = _baseCreep.Network_aggroedEntity.transform;
		}
	}

	private IEnumerator GeistLord_MagicScythes()
	{
		if (_currentAction == GeistLordAction.IDLE)
		{
			_currentAction = GeistLordAction.MAGIC_SCYTHES;
			_lastAction = _currentAction;
			_baseCreep._pathFindCtrl._disablePathing = true;
			yield return new WaitForSeconds(_timeBeforeMagicScythe);
			_baseCreep.Server_CrossFadeAnim("Attack03", 0f, 1);
			yield return new WaitForSeconds(_magicScytheCooldown);
			_currentAction = GeistLordAction.IDLE;
			_baseCreep._pathFindCtrl._disablePathing = false;
			_currentActionBuffer = 1.3f;
		}
	}

	public void Init_MagicScytheExecution()
	{
		GameObject obj = PoolManager._current.GenerateObject(_magicScytheProjectile, string.Empty, 0f);
		obj.GetComponent<ProjectileObject>()._parentTransform = base.transform;
		obj.GetComponentInChildren<CombatCollider>()._parentObject = base.gameObject;
		obj.transform.SetParent(base.transform);
		obj.transform.SetParent(null);
		obj.transform.SetAsLastSibling();
		obj.transform.SetPositionAndRotation(_magicScytheSpawnPosition.position, Quaternion.Euler(0f, base.transform.eulerAngles.y, 0f));
	}

	private IEnumerator GeistLord_LaserBeam()
	{
		if (_currentAction == GeistLordAction.IDLE)
		{
			_currentAction = GeistLordAction.LASERBEAM;
			_lastAction = _currentAction;
			_baseCreep._pathFindCtrl._disablePathing = true;
			yield return new WaitForSeconds(_timeBeforeLaserBeam);
			_baseCreep.Network_ignoreAggroedTarget = true;
			_baseCreep._pathFindCtrl.Network_dontLookAtTarget = true;
			_baseCreep.Server_CrossFadeAnim("specialAction_01", 0f, 1);
			yield return new WaitForSeconds(_laserBeamCooldown);
			_baseCreep.Server_CrossFadeAnim("Empty", 0.095f, 1);
			_currentAction = GeistLordAction.IDLE;
			_baseCreep._pathFindCtrl._disablePathing = false;
			_currentActionBuffer = 1.3f;
			_baseCreep.Network_ignoreAggroedTarget = false;
			_baseCreep._pathFindCtrl.Network_dontLookAtTarget = false;
		}
	}

	public void Init_LaserBeamExecution()
	{
		GameObject obj = PoolManager._current.GenerateObject(_laserBeamProjectile, string.Empty, 0f);
		obj.GetComponentInChildren<CombatCollider>()._parentObject = base.gameObject;
		obj.transform.SetParent(base.transform);
		obj.transform.SetParent(null);
		obj.transform.SetAsLastSibling();
		obj.transform.SetPositionAndRotation(_laserBeamSpawnPosition.position, Quaternion.Euler(0f, base.transform.eulerAngles.y, 0f));
	}
}
