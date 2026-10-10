using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Mirror;
using Mirror.RemoteCalls;
using UnityEngine;

public class Creep : NetworkBehaviour
{
	[HideInInspector]
	public StatusEntity _statusEntity;

	[HideInInspector]
	public ItemDropEntity _itemDropEntity;

	private CreepAwareness _creepAwareness;

	[Header("Base Creep Parameters")]
	[SyncVar]
	public string _creepDisplayName = string.Empty;

	[SyncVar]
	public string _creepNameID = string.Empty;

	[SyncVar(hook = "OnChangeStatModifier")]
	public int _statModifierID;

	[SyncVar]
	public bool _isTargetable = true;

	[SyncVar]
	public bool _disableTriggerColliders;

	[Space]
	public ScriptableCreep _scriptCreep;

	public CreepBehavior _creepBehavior;

	public ScriptableStatModifier _scriptStatModifier;

	public GameObject _creepMinimapIcon;

	[Space]
	public GameObject _creepObject;

	public CreepSpawner _parentCreepSpawner;

	public Animator _creepAnim;

	public Material _glowMaterial;

	private SkinnedMeshRenderer[] _meshRends;

	private Renderer[] _otherMeshRends;

	private readonly List<int> _glowMaterialIndexes = new List<int>();

	[Space]
	public CapsuleCollider _creepPushCollider;

	public CapsuleCollider _creepRigidbodyCollider;

	[Header("Base Creep Spawning Conditions")]
	[SyncVar]
	public CreepCondition _currentCreepCondition = CreepCondition.SPAWNING;

	public float _setRespawnBuffer;

	public float _respawnBuffer;

	public float _spawnInBuffer;

	public float _repositionBuffer;

	[Header("Creep Stats")]
	[SyncVar]
	public int _creepLevel;

	[SyncVar(hook = "OnChangeElementID")]
	public int _combatElementID;

	public ScriptableCombatElement _combatElement;

	public DamageType _creepAttribute;

	[SyncVar]
	public StatStruct _statStruct;

	[Header("Targeting / Aggro")]
	[SyncVar(hook = "OnChangeAggroTarget")]
	public StatusEntity _aggroedEntity;

	[SyncVar]
	public bool _ignoreAggroedTarget;

	public bool _disableAggro;

	[Space]
	public Player _playerTagged;

	public List<CreepThreatSlot> _creepThreatSlots = new List<CreepThreatSlot>();

	[Header("Runtime Conditions")]
	public float _parryBuffer;

	public float _knockbackBuffer;

	public float _attackActionBuffer;

	public float _aggroIconBuffer;

	private float _creepQueueBuffer;

	public bool _isKnockedBack;

	public bool _isCreepMugged;

	[Space]
	[SyncVar]
	public bool _isMoving;

	[SyncVar]
	public bool _isParryStaggered;

	public bool _isActionLocked;

	[Header("Pathfinding / Rigidbody")]
	public Rigidbody _creepRB;

	public RaycastPathFinder _pathFindCtrl;

	public WanderBehavior _wanderBehavior;

	public bool _suppressWandererBehavior;

	private PhysicsScene _physicsScene;

	[Header("Ledge / Height Jumping")]
	[SerializeField]
	private Transform _ledgeJumpcaster;

	[SerializeField]
	private Transform _heightJumpCheckerPos;

	[SerializeField]
	private float _heightJumpDetection = 6f;

	private float _heightJumpBuffer;

	[Header("Base Creep Visuals")]
	[SerializeField]
	private GameObject _shadowProjectionObject;

	[SerializeField]
	private Transform _modifierEmblemEffect;

	[SerializeField]
	private MeshRenderer[] _modifierEmblemMeshes;

	private float _miscEffectBuffer;

	private RenderModify _rendModify;

	[Space]
	[SerializeField]
	private ParticleSystem _spawnInEffect;

	[SerializeField]
	private ParticleSystem _bloodEffect;

	[SerializeField]
	private ParticleSystem _gibEffect;

	[SerializeField]
	private ParticleSystem _parryStaggerEffect;

	public GameObject _dashEffect;

	[Space]
	[SerializeField]
	private Transform _aggroIconPos;

	[SerializeField]
	private Animator _aggroIconAnim;

	[Space]
	[SerializeField]
	private Transform _minimapIcon;

	[SerializeField]
	private SpriteRenderer _minimapIconRender;

	[SerializeField]
	private Color _minimapIco_standardCreep = Color.magenta;

	[SerializeField]
	private Color _minimapIco_eliteCreep = Color.yellow;

	[Header("Creep Audio")]
	public AudioSource _creepDynamicASrc;

	public AudioSource _creepAggroASrc;

	public AudioSource _creepJumpAsrc;

	public AudioSource _creepParryStaggerAsrc;

	public AudioSource _creepGibASrc;

	public AudioSource _creepSpawnASrc;

	public AudioSource _creepSkillChargeASrc;

	private float _hurtRpcBuffer;

	private AudioSource[] _aSrcCache;

	private bool _isCulled;

	protected NetworkBehaviourSyncVar ____aggroedEntityNetId;

	public string Network_creepDisplayName
	{
		get
		{
			return _creepDisplayName;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _creepDisplayName, 1uL, null);
		}
	}

	public string Network_creepNameID
	{
		get
		{
			return _creepNameID;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _creepNameID, 2uL, null);
		}
	}

	public int Network_statModifierID
	{
		get
		{
			return _statModifierID;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _statModifierID, 4uL, OnChangeStatModifier);
		}
	}

	public bool Network_isTargetable
	{
		get
		{
			return _isTargetable;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _isTargetable, 8uL, null);
		}
	}

	public bool Network_disableTriggerColliders
	{
		get
		{
			return _disableTriggerColliders;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _disableTriggerColliders, 16uL, null);
		}
	}

	public CreepCondition Network_currentCreepCondition
	{
		get
		{
			return _currentCreepCondition;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _currentCreepCondition, 32uL, null);
		}
	}

	public int Network_creepLevel
	{
		get
		{
			return _creepLevel;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _creepLevel, 64uL, null);
		}
	}

	public int Network_combatElementID
	{
		get
		{
			return _combatElementID;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _combatElementID, 128uL, OnChangeElementID);
		}
	}

	public StatStruct Network_statStruct
	{
		get
		{
			return _statStruct;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _statStruct, 256uL, null);
		}
	}

	public StatusEntity Network_aggroedEntity
	{
		get
		{
			return GetSyncVarNetworkBehaviour(____aggroedEntityNetId, ref _aggroedEntity);
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter_NetworkBehaviour(value, ref _aggroedEntity, 512uL, OnChangeAggroTarget, ref ____aggroedEntityNetId);
		}
	}

	public bool Network_ignoreAggroedTarget
	{
		get
		{
			return _ignoreAggroedTarget;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _ignoreAggroedTarget, 1024uL, null);
		}
	}

	public bool Network_isMoving
	{
		get
		{
			return _isMoving;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _isMoving, 2048uL, null);
		}
	}

	public bool Network_isParryStaggered
	{
		get
		{
			return _isParryStaggered;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _isParryStaggered, 4096uL, null);
		}
	}

	private void OnChangeStatModifier(int _old, int _new)
	{
		if (_meshRends == null || _meshRends.Length == 0)
		{
			return;
		}
		if (_new > 0)
		{
			ScriptableStatModifier scriptStatModifier = GameManager._current.Locate_StatModifier(_new);
			_scriptStatModifier = scriptStatModifier;
			for (int i = 0; i < _meshRends.Length; i++)
			{
				if (!(_meshRends[i] == null) && !(_meshRends[i].materials[_glowMaterialIndexes[i]] == null))
				{
					_meshRends[i].materials[_glowMaterialIndexes[i]].SetColor("_Color", _scriptStatModifier._glowTintColor);
				}
			}
			for (int j = 0; j < _otherMeshRends.Length; j++)
			{
				if (!(_otherMeshRends[j] == null) && !(_otherMeshRends[j].material == null))
				{
					_otherMeshRends[j].material.SetColor("_Color", _scriptStatModifier._glowTintColor);
				}
			}
			return;
		}
		_scriptStatModifier = null;
		for (int k = 0; k < _meshRends.Length; k++)
		{
			if (!(_meshRends[k] == null) && !(_meshRends[k].materials[_glowMaterialIndexes[k]] == null))
			{
				_meshRends[k].materials[_glowMaterialIndexes[k]].SetColor("_Color", Color.clear);
			}
		}
		for (int l = 0; l < _otherMeshRends.Length; l++)
		{
			if (!(_otherMeshRends[l] == null) && !(_otherMeshRends[l].material == null))
			{
				_otherMeshRends[l].material.SetColor("_Color", Color.clear);
			}
		}
	}

	private void OnChangeAggroTarget(StatusEntity _old, StatusEntity _new)
	{
		if (!(_aggroIconBuffer > 0f) && !(_new == null) && _new != _old)
		{
			Init_AggroIcon(1);
			_aggroIconBuffer = 0.2f;
		}
	}

	private void OnChangeElementID(int _old, int _new)
	{
		ScriptableCombatElement scriptableCombatElement = GameManager._current.Locate_CombatElement(_new);
		if (!scriptableCombatElement)
		{
			_combatElement = GameManager._current._statLogics._defaultCombatElement;
		}
		else
		{
			_combatElement = scriptableCombatElement;
		}
	}

	private void Awake()
	{
		_statusEntity = GetComponent<StatusEntity>();
		_itemDropEntity = GetComponent<ItemDropEntity>();
		_rendModify = GetComponent<RenderModify>();
		_creepAwareness = GetComponentInChildren<CreepAwareness>();
		_creepRB = GetComponent<Rigidbody>();
		_pathFindCtrl = GetComponent<RaycastPathFinder>();
		_wanderBehavior = GetComponent<WanderBehavior>();
		_minimapIcon.transform.localScale = Vector3.one * 250f;
		_minimapIcon.transform.localPosition = Vector3.up * 80f;
	}

	public override void OnStartServer()
	{
		base.OnStartServer();
		Apply_BaseCreepData();
		_physicsScene = base.gameObject.scene.GetPhysicsScene();
	}

	public override void OnStartClient()
	{
		base.OnStartClient();
		_scriptCreep = GameManager._current.Locate_Creep(_creepNameID ?? "");
		if (base.isClientOnly)
		{
			Apply_BaseCreepData();
		}
	}

	private void OnDestroy()
	{
		Vector3 pos = base.transform.position + Vector3.up * _scriptCreep._generalCreepVisual._targeterOffset;
		Init_UnspawnEffect(pos);
		if ((bool)_creepObject)
		{
			Object.Destroy(_creepObject);
		}
	}

	public void Apply_BaseCreepData()
	{
		if (!_scriptCreep)
		{
			Debug.LogError("Creep not found: (" + _creepNameID + "). Is it missing or a typo?");
			return;
		}
		if (base.isServer)
		{
			string creepName = _scriptCreep._creepName;
			Network_creepDisplayName = creepName;
		}
		Server_SetPathFinderParameters();
		Spawn_CreepObject();
	}

	[ServerCallback]
	private void Server_SetPathFinderParameters()
	{
		if (NetworkServer.active)
		{
			_wanderBehavior.enabled = _scriptCreep._pathfindParams._wanderingPathfinder;
			_pathFindCtrl._targetStoppingDistance = _scriptCreep._pathfindParams._stoppingDistance;
			_pathFindCtrl._baseMovSpeed = _scriptCreep._pathfindParams._movementSpeed;
			_pathFindCtrl._currentMovSpeed = _pathFindCtrl._baseMovSpeed;
		}
	}

	private void Spawn_CreepObject()
	{
		if ((bool)_creepObject)
		{
			return;
		}
		if (!_scriptCreep._creepObject)
		{
			Debug.LogError("Creep Object not found in: (" + _creepNameID + "). Is it missing?");
			return;
		}
		_creepObject = Object.Instantiate(_scriptCreep._creepObject, base.transform);
		_creepObject.transform.localPosition = Vector3.zero;
		base.gameObject.name = $"[netID: {base.netId}] _creepBase({_scriptCreep._creepName})";
		_creepObject.name = $"[netID: {base.netId}] _creepObject({_scriptCreep._creepName})";
		_rendModify._objModel = _creepObject;
		_rendModify._skinnedObjRenders = _creepObject.GetComponentsInChildren<SkinnedMeshRenderer>(includeInactive: true);
		_rendModify._objRenders = _creepObject.GetComponentsInChildren<MeshRenderer>(includeInactive: true);
		_rendModify._setVisualScale = _scriptCreep._generalCreepVisual._creepVisualScale;
		_rendModify._RefreshScale = true;
		if ((bool)_creepObject.GetComponentInChildren<Animator>(includeInactive: true))
		{
			_creepAnim = _creepObject.GetComponentInChildren<Animator>(includeInactive: true);
		}
		_creepBehavior = _creepObject.GetComponent<CreepBehavior>();
		_aSrcCache = GetComponentsInChildren<AudioSource>(includeInactive: true);
		_meshRends = _creepObject.GetComponentsInChildren<SkinnedMeshRenderer>(includeInactive: true);
		_otherMeshRends = _creepObject.GetComponentsInChildren<Renderer>(includeInactive: true);
		for (int i = 0; i < _meshRends.Length; i++)
		{
			Material material = new Material(_glowMaterial);
			material.SetColor("_Color", Color.clear);
			List<Material> list = new List<Material>();
			Material[] materials = _meshRends[i].materials;
			list.AddRange(materials);
			list.Add(material);
			_glowMaterialIndexes.Add(_meshRends[i].materials.Length);
			_meshRends[i].materials = list.ToArray();
		}
		base.transform.SetAsLastSibling();
		_creepObject.transform.SetAsLastSibling();
		_ledgeJumpcaster.SetParent(_creepObject.transform);
		_ledgeJumpcaster.localPosition = Vector3.zero;
		_heightJumpCheckerPos.SetParent(_creepObject.transform);
		_heightJumpCheckerPos.localPosition = new Vector3(0f, 2f, -2f);
		if (_scriptCreep._isElite)
		{
			_minimapIconRender.color = _minimapIco_eliteCreep;
		}
		else
		{
			_minimapIconRender.color = _minimapIco_standardCreep;
		}
		Apply_CreepBloodColors();
		void Apply_CreepBloodColors()
		{
			GeneralCreepVisual generalCreepVisual = _scriptCreep._generalCreepVisual;
			ParticleSystem.MainModule main = _bloodEffect.main;
			ParticleSystem.MainModule main2 = _bloodEffect.subEmitters.GetSubEmitterSystem(0).main;
			main.startColor = generalCreepVisual._creepBloodColor;
			main2.startColor = generalCreepVisual._creepBloodSubColor;
			ParticleSystem.MainModule main3 = _gibEffect.main;
			ParticleSystem.MainModule main4 = _gibEffect.subEmitters.GetSubEmitterSystem(0).main;
			main3.startColor = generalCreepVisual._creepGibColor;
			main4.startColor = generalCreepVisual._creepGibSubColor;
		}
	}

	private void Update()
	{
		if (base.isClientOnly && !_creepRB.isKinematic)
		{
			_creepRB.isKinematic = true;
		}
		int currencyDrop = (int)GameManager._current._statLogics._currencyDropCurve.Evaluate(_creepLevel - 1) + _scriptCreep._currencyDropBonus;
		_itemDropEntity._currencyDrop = currencyDrop;
		if (_combatElementID == 0)
		{
			_combatElement = GameManager._current._statLogics._defaultCombatElement;
		}
		Server_HandleBufferParams();
		Server_HandleAggroParams();
		Update_CreepAnimation();
		Update_CreepObject();
		Handle_AudioCache();
		if ((bool)_scriptCreep)
		{
			_creepPushCollider.radius = _scriptCreep._pathfindParams._creepPushColliderRadius;
		}
		if ((bool)Player._mainPlayer)
		{
			_creepMinimapIcon.SetActive(_currentCreepCondition != CreepCondition.DEAD && Player._mainPlayer.gameObject.scene == base.gameObject.scene);
		}
	}

	[ServerCallback]
	private void Server_HandleBufferParams()
	{
		if (!NetworkServer.active)
		{
			return;
		}
		if ((bool)_scriptStatModifier)
		{
			Network_creepDisplayName = _scriptStatModifier._modifierTag + " " + _scriptCreep._creepName;
		}
		else
		{
			Network_creepDisplayName = _scriptCreep._creepName ?? "";
			Network_statModifierID = 0;
		}
		if (!_creepAnim.GetCurrentAnimatorStateInfo(1).IsName("Empty"))
		{
			if (_creepQueueBuffer <= 0f && (bool)_scriptCreep && !_scriptCreep._isElite && (bool)Network_aggroedEntity && (bool)Network_aggroedEntity._isPlayer)
			{
				Player isPlayer = Network_aggroedEntity._isPlayer;
				if (!isPlayer._pCombat._setCreepQueue._creepAttacker)
				{
					isPlayer._pCombat.Init_CreepAttackQueue(this);
					_creepQueueBuffer = 0.6f;
				}
			}
			_attackActionBuffer = 0.6f;
		}
		if (_suppressWandererBehavior)
		{
			_wanderBehavior.enabled = false;
		}
		if (_scriptCreep._unkillable)
		{
			_statusEntity.Network_currentHealth = _statStruct._maxHealth - 1;
		}
		if (_respawnBuffer > 0f)
		{
			_respawnBuffer -= Time.deltaTime;
		}
		else
		{
			_respawnBuffer = 0f;
		}
		if (_spawnInBuffer > 0f)
		{
			_spawnInBuffer -= Time.deltaTime;
		}
		else
		{
			_spawnInBuffer = 0f;
		}
		if (_attackActionBuffer > 0f)
		{
			_attackActionBuffer -= Time.deltaTime;
		}
		else
		{
			_attackActionBuffer = 0f;
		}
		if (_aggroIconBuffer > 0f)
		{
			_aggroIconBuffer -= Time.deltaTime;
		}
		else
		{
			_aggroIconBuffer = 0f;
		}
		if (_hurtRpcBuffer > 0f)
		{
			_hurtRpcBuffer -= Time.deltaTime;
		}
		else
		{
			_hurtRpcBuffer = 0f;
		}
		if (_creepQueueBuffer > 0f)
		{
			_creepQueueBuffer -= Time.deltaTime;
		}
		else
		{
			_creepQueueBuffer = 0f;
		}
		if (_currentCreepCondition == CreepCondition.ACTIVE)
		{
			Handle_ActiveBufferParams();
		}
		else
		{
			Handle_InactiveBufferParams();
		}
		if ((bool)_parentCreepSpawner && _setRespawnBuffer > 0f && _respawnBuffer == 0f && _currentCreepCondition == CreepCondition.DEAD)
		{
			_parentCreepSpawner.Callback_RespawnCreep(this);
		}
		void Handle_ActiveBufferParams()
		{
			if (_knockbackBuffer > 0f)
			{
				_knockbackBuffer -= Time.deltaTime;
				_isKnockedBack = true;
			}
			if (_isKnockedBack && _knockbackBuffer <= 0f)
			{
				Rpc_CrossFadeAnim("Empty", 0.085f, 2);
				_knockbackBuffer = 0f;
				_isKnockedBack = false;
			}
			if (_parryBuffer > 0f)
			{
				_parryBuffer -= Time.deltaTime;
			}
			Network_isParryStaggered = _parryBuffer > 0f;
		}
		void Handle_InactiveBufferParams()
		{
			_parryBuffer = 0f;
			_knockbackBuffer = 0f;
			_isKnockedBack = false;
			Network_isParryStaggered = false;
			Network_isMoving = false;
		}
	}

	[ServerCallback]
	private void FixedUpdate()
	{
		if (NetworkServer.active)
		{
			_pathFindCtrl.gravityScale = GameManager._current._statLogics._creepGravityScale;
			if (!_physicsScene.Raycast(base.transform.position + Vector3.up * 3.5f, Vector3.down, float.PositiveInfinity, CastLayers.GroundMask))
			{
				Reset_Creep();
			}
			if (_currentCreepCondition == CreepCondition.ACTIVE)
			{
				Server_CreepPathfindJumping();
				Network_isMoving = !_pathFindCtrl._isStoppedAndNearTarget && !_pathFindCtrl._disablePathing && _creepRB.velocity.magnitude > 1.25f && !_isParryStaggered;
			}
			else
			{
				_knockbackBuffer = 0f;
				Network_isMoving = false;
			}
		}
	}

	[ServerCallback]
	private void Server_CreepPathfindJumping()
	{
		if (!NetworkServer.active)
		{
			return;
		}
		if (_heightJumpBuffer > 0f)
		{
			_heightJumpBuffer -= Time.deltaTime;
			return;
		}
		_heightJumpBuffer = 0f;
		if (CanPathFindJump())
		{
			bool flag = _physicsScene.Raycast(_heightJumpCheckerPos.position, _creepObject.transform.forward, _heightJumpDetection, CastLayers.GroundMask);
			bool num = _physicsScene.Raycast(_ledgeJumpcaster.position, _creepObject.transform.forward, 2.35f, CastLayers.GroundMask);
			if (flag)
			{
				Init_HeightJump();
			}
			if (num)
			{
				Init_LedgeJump();
			}
			Vector3 vector = _pathFindCtrl._target.position - base.transform.position;
			vector.y = 0f;
		}
		bool CanPathFindJump()
		{
			if (_currentCreepCondition != CreepCondition.ACTIVE)
			{
				return false;
			}
			if (_statusEntity._currentHealth <= 0)
			{
				return false;
			}
			if (_isParryStaggered)
			{
				return false;
			}
			if (_ignoreAggroedTarget)
			{
				return false;
			}
			if (_creepRB.isKinematic)
			{
				return false;
			}
			if (_attackActionBuffer > 0f)
			{
				return false;
			}
			if (_pathFindCtrl._disablePathing)
			{
				return false;
			}
			if (!_pathFindCtrl._target)
			{
				return false;
			}
			if (!_isMoving)
			{
				return false;
			}
			return true;
		}
		void Init_HeightJump()
		{
			if (!_scriptCreep._pathfindParams._disableHeightJump)
			{
				_creepRB.AddRelativeForce(0f, GameManager._current._statLogics._creepHeightJumpForce, 0f, ForceMode.Impulse);
				_creepRB.AddForce(_creepObject.transform.forward * GameManager._current._statLogics._creepHeightJumpForwardForce, ForceMode.VelocityChange);
				Rpc_PlayCreepJumpEffect();
				float num2 = 0f;
				if ((bool)Network_aggroedEntity)
				{
					num2 = Network_aggroedEntity.transform.position.y - base.transform.position.y;
				}
				if ((bool)Network_aggroedEntity && num2 >= 25.5f)
				{
					_heightJumpBuffer = 0.38f;
					_pathFindCtrl._gravityBuffer = 0.1f;
				}
				else
				{
					_heightJumpBuffer = 0.6f;
					_pathFindCtrl._gravityBuffer = 0.1f;
				}
			}
		}
		void Init_LedgeJump()
		{
			if (!_scriptCreep._pathfindParams._disableLedgeJump)
			{
				float creepLedgeJumpForce = GameManager._current._statLogics._creepLedgeJumpForce;
				float creepLedgeJumpForwardForce = GameManager._current._statLogics._creepLedgeJumpForwardForce;
				_creepRB.AddRelativeForce(0f, creepLedgeJumpForce, 0f, ForceMode.Impulse);
				_creepRB.AddForce(_creepObject.transform.forward * creepLedgeJumpForwardForce, ForceMode.VelocityChange);
			}
		}
	}

	[ClientRpc]
	private void Rpc_PlayCreepJumpEffect()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendRPCInternal("System.Void Creep::Rpc_PlayCreepJumpEffect()", 500620726, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	public bool CanDetectAggroTarget(GameObject _targetableObject)
	{
		if (!_scriptCreep._canAggro)
		{
			return false;
		}
		if (_disableAggro)
		{
			return false;
		}
		if ((bool)Network_aggroedEntity)
		{
			return false;
		}
		if (_targetableObject.scene != base.gameObject.scene)
		{
			return false;
		}
		if (_currentCreepCondition != CreepCondition.ACTIVE)
		{
			return false;
		}
		return true;
	}

	[ServerCallback]
	public void Server_AggroIcon(int _value)
	{
		if (NetworkServer.active && _scriptCreep._canAggro && !_disableAggro)
		{
			Rpc_PlayAggroIcon(_value);
		}
	}

	[ClientRpc]
	private void Rpc_PlayAggroIcon(int _value)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteInt(_value);
		SendRPCInternal("System.Void Creep::Rpc_PlayAggroIcon(System.Int32)", 342701790, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	[ClientCallback]
	private void Init_AggroIcon(int _value)
	{
		if (!NetworkClient.active || !Player._mainPlayer || Player._mainPlayer.gameObject.scene != base.gameObject.scene || Vector3.Distance(Player._mainPlayer.transform.position, base.transform.position) >= 125f)
		{
			return;
		}
		switch (_value)
		{
		case 0:
			_aggroIconAnim.CrossFade("_aggroIconAnim_interested", 0f, 0);
			break;
		case 1:
			_aggroIconAnim.CrossFade("_aggroIconAnim_notice", 0f, 0);
			if (!_creepAggroASrc.isPlaying)
			{
				_creepAggroASrc.Play();
			}
			break;
		}
	}

	[ServerCallback]
	public void Server_AddThreat(StatusEntity _targetEntity, float _value)
	{
		if (!NetworkServer.active || _targetEntity.gameObject.scene != base.gameObject.scene || !_scriptCreep._canAggro || _value <= 0f || !_targetEntity.TryGetComponent<Player>(out var component) || component._currentGameCondition != GameCondition.IN_GAME || component._currentPlayerCondition == PlayerCondition.DEAD || component._spawnBuffer > 0f || component._bufferingStatus)
		{
			return;
		}
		bool flag = false;
		if (_creepThreatSlots.Count <= 0)
		{
			CreepThreatSlot item = new CreepThreatSlot
			{
				_statusEntity = _targetEntity,
				_player = component,
				_currentThreat = _value
			};
			_creepThreatSlots.Add(item);
			return;
		}
		for (int i = 0; i < _creepThreatSlots.Count; i++)
		{
			if (_creepThreatSlots[i]._statusEntity == _targetEntity)
			{
				_creepThreatSlots[i]._currentThreat += _value;
				flag = true;
			}
			else
			{
				_creepThreatSlots[i]._currentThreat -= _value;
			}
			if (i >= _creepThreatSlots.Count - 1 && !flag)
			{
				CreepThreatSlot item2 = new CreepThreatSlot
				{
					_statusEntity = _targetEntity,
					_player = component,
					_currentThreat = _value
				};
				_creepThreatSlots.Add(item2);
			}
		}
	}

	[ServerCallback]
	private void Server_HandleAggroParams()
	{
		if (NetworkServer.active && _scriptCreep._canAggro && !_disableAggro)
		{
			_pathFindCtrl._canPatrolNodes = !Network_aggroedEntity;
			Iterate_DespawnDistance();
			Iterate_HealthRefreshCondition();
			Iterate_AutoAggroConditions();
			Iterate_ThreatList();
			Handle_AggroedNetObj();
		}
		void Handle_AggroedNetObj()
		{
			if (_scriptCreep._playMapInstanceActionMusic && (bool)_parentCreepSpawner._mapInstance._actionMusic)
			{
				_parentCreepSpawner._mapInstance._playActionMusic = Network_aggroedEntity;
			}
			if ((bool)Network_aggroedEntity)
			{
				if ((bool)_parentCreepSpawner && _parentCreepSpawner._resetAggroedCreepsIfEmptyRadius && _parentCreepSpawner._playersWithinSpawnerRadius.Count <= 0)
				{
					_creepAwareness._assistAggroBuffer = 1.5f;
					_creepAwareness._interestToAggroBuffer = 1.5f;
					Reset_Creep();
				}
				else
				{
					if (!_ignoreAggroedTarget)
					{
						_pathFindCtrl._target = Network_aggroedEntity.transform;
					}
					_pathFindCtrl._canPatrolNodes = false;
					if (_pathFindCtrl._isStoppedAndNearTarget || _pathFindCtrl._disablePathing)
					{
						LookAt_TargetedNetObj();
					}
					if (Network_aggroedEntity.TryGetComponent<Player>(out var component) && (component._currentPlayerCondition != PlayerCondition.ACTIVE || component._isHidden || component._bufferingStatus))
					{
						if (_playerTagged == component)
						{
							_playerTagged = null;
						}
						_pathFindCtrl._target = null;
						Network_aggroedEntity = null;
					}
				}
			}
		}
		void Iterate_AutoAggroConditions()
		{
			if (_currentCreepCondition == CreepCondition.ACTIVE && _statusEntity._currentHealth > 0 && _parentCreepSpawner._aggroWithinRadius && _parentCreepSpawner._playersWithinSpawnerRadius.Count > 0 && !Network_aggroedEntity)
			{
				Player player = _parentCreepSpawner._playersWithinSpawnerRadius[Random.Range(0, _parentCreepSpawner._playersWithinSpawnerRadius.Count)];
				bool flag = !player._bufferingStatus && !player._isHidden && player._currentPlayerCondition == PlayerCondition.ACTIVE && player._spawnBuffer <= 0f;
				Vector3 direction = player.transform.position - base.transform.position;
				Vector3 origin = base.transform.position + Vector3.up * 3.5f;
				if ((!_physicsScene.Raycast(origin, direction, out var hitInfo, float.PositiveInfinity, CastLayers.PathfinderMask, QueryTriggerInteraction.Collide) || ((bool)hitInfo.collider.gameObject.GetComponentInParent<Player>() && !(hitInfo.collider.gameObject.GetComponentInParent<Player>() != player))) && flag)
				{
					StatusEntity targetEntity = (Network_aggroedEntity = player._statusEntity);
					Server_AddThreat(targetEntity, 5f);
					Rpc_PlayAggroIcon(1);
				}
			}
		}
		void Iterate_DespawnDistance()
		{
			if (_currentCreepCondition == CreepCondition.ACTIVE && _statusEntity._currentHealth > 0 && (bool)_parentCreepSpawner && (!_parentCreepSpawner._mapInstance || _parentCreepSpawner._mapInstance._zoneType != ZoneType.Dungeon))
			{
				if (_parentCreepSpawner._playersWithinSpawnerRadius.Count <= 0 && Vector3.Distance(base.transform.position, _parentCreepSpawner.transform.position) >= 1200f)
				{
					Reset_Creep();
				}
				if ((bool)Network_aggroedEntity && Vector3.Distance(base.transform.position, Network_aggroedEntity.transform.position) >= 180f && !_scriptCreep._isElite)
				{
					_ = (bool)Network_aggroedEntity._isPlayer;
					Reset_Creep();
				}
			}
		}
		void Iterate_HealthRefreshCondition()
		{
			if (_currentCreepCondition == CreepCondition.ACTIVE && _statusEntity._currentHealth < _statStruct._maxHealth && _parentCreepSpawner._playersWithinSpawnerRadius.Count <= 0 && !Network_aggroedEntity)
			{
				StatusEntity statusEntity = _statusEntity;
				statusEntity.Network_currentHealth = statusEntity._currentHealth + 1;
			}
		}
		void Iterate_ThreatList()
		{
			if (_currentCreepCondition == CreepCondition.ACTIVE && _creepThreatSlots.Count > 0)
			{
				for (int i = 0; i < _creepThreatSlots.Count; i++)
				{
					if (!_creepThreatSlots[i]._statusEntity || !_creepThreatSlots[i]._player)
					{
						_creepThreatSlots.RemoveAt(i);
					}
					else if (_creepThreatSlots[i]._player._currentPlayerCondition != PlayerCondition.ACTIVE)
					{
						_creepThreatSlots.RemoveAt(i);
					}
					else if (_creepThreatSlots[i]._player._bufferingStatus)
					{
						_creepThreatSlots.RemoveAt(i);
					}
					else
					{
						if (_creepThreatSlots[i]._currentThreat > 150f)
						{
							_creepThreatSlots[i]._currentThreat = 150f;
						}
						if (_creepThreatSlots[i]._player._isHidden)
						{
							_creepBehavior.Reset_Behavior();
							_pathFindCtrl._target = null;
							Network_aggroedEntity = null;
							_creepAwareness.Init_InterestPoint(_creepThreatSlots[i]._player.transform.position);
							_creepThreatSlots.RemoveAt(i);
						}
						else if (!(_attackActionBuffer > 0f) && Network_aggroedEntity != _creepThreatSlots[i]._statusEntity)
						{
							if (_creepThreatSlots[i]._currentThreat >= 140f)
							{
								Network_aggroedEntity = _creepThreatSlots[i]._statusEntity;
								break;
							}
							if (_creepThreatSlots[i]._currentThreat == _creepThreatSlots.Max((CreepThreatSlot x) => x._currentThreat))
							{
								Network_aggroedEntity = _creepThreatSlots[i]._statusEntity;
							}
						}
					}
				}
			}
		}
		void LookAt_TargetedNetObj()
		{
			if (_currentCreepCondition == CreepCondition.ACTIVE && !_pathFindCtrl._dontLookAtTarget && !_ignoreAggroedTarget && !_isParryStaggered)
			{
				Vector3 forward = Network_aggroedEntity.transform.position - base.transform.position;
				forward.y = 0f;
				base.transform.rotation = Quaternion.Lerp(base.transform.rotation, Quaternion.LookRotation(forward), 35.5f * Time.deltaTime);
			}
		}
	}

	[ServerCallback]
	public void Reset_AggroParameters()
	{
		if (NetworkServer.active)
		{
			if (_creepThreatSlots.Count > 0)
			{
				_creepThreatSlots.Clear();
			}
			Network_aggroedEntity = null;
			_pathFindCtrl._target = null;
			_pathFindCtrl._canPatrolNodes = true;
			_wanderBehavior._disableWanderBehavior = true;
		}
	}

	[ServerCallback]
	public void Reset_Creep()
	{
		if (NetworkServer.active)
		{
			Vector3 pos = base.transform.position + Vector3.up * _scriptCreep._generalCreepVisual._targeterOffset;
			Rpc_InitUnspawnEffect(pos, 0f);
			Reset_AggroParameters();
			if ((bool)_parentCreepSpawner)
			{
				_parentCreepSpawner.Init_CreepSpawn(this);
			}
			_wanderBehavior._disableWanderBehavior = false;
			_statusEntity._syncConditions.Clear();
		}
	}

	public void BaseCreep_OnSpawn()
	{
		Server_ApplySpawnConditions();
	}

	[ServerCallback]
	private void Server_ApplySpawnConditions()
	{
		if (!NetworkServer.active)
		{
			return;
		}
		if (!_scriptCreep._pathfindParams._alwaysKinematic)
		{
			_creepRB.isKinematic = false;
		}
		if (_scriptCreep._pathfindParams._wanderingPathfinder)
		{
			_wanderBehavior._disableWanderBehavior = false;
		}
		if (!(_spawnInBuffer > 0f))
		{
			_creepRigidbodyCollider.center = GameManager._current._statLogics._creepRigidbodyCenter;
			_creepBehavior.Server_Behavior_OnSpawn();
			Server_ApplyCreepStats();
			_playerTagged = null;
			Network_aggroedEntity = null;
			Network_ignoreAggroedTarget = false;
			_creepThreatSlots.Clear();
			_pathFindCtrl._target = null;
			_pathFindCtrl._pushCollider.enabled = false;
			_isCreepMugged = false;
			Network_isParryStaggered = false;
			Network_isMoving = false;
			_pathFindCtrl.Disable_Pathfinding(Random.Range(0.85f, 1.56f));
			if (!_scriptCreep._pathfindParams._ignoreGravity)
			{
				_pathFindCtrl._disableGravity = false;
			}
			else
			{
				_pathFindCtrl._disableGravity = true;
			}
			Network_currentCreepCondition = CreepCondition.ACTIVE;
			Server_CrossFadeAnim("Empty", 0f, 3);
			_spawnInBuffer = 1.25f;
			if (base.isServerOnly)
			{
				StartCoroutine(Init_SpawnInRpc());
			}
			Rpc_OnCreepSpawn();
		}
	}

	[ServerCallback]
	public void Server_ApplyCreepStats()
	{
		if (!NetworkServer.active)
		{
			return;
		}
		StatLogics statLogics = GameManager._current._statLogics;
		Network_creepLevel = _scriptCreep._creepLevel;
		StatStruct statStruct = default;
		ScriptableCombatElement scriptableCombatElement = _scriptCreep._combatElement;
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		int num4 = (int)statLogics._maxHealthCurve.Evaluate(_creepLevel - 1) + _scriptCreep._creepStatStruct._maxHealth;
		int num5 = (int)statLogics._maxManaCurve.Evaluate(_creepLevel - 1) + _scriptCreep._creepStatStruct._maxMana;
		int num6 = (int)statLogics._maxStaminaCurve.Evaluate(_creepLevel - 1) + _scriptCreep._creepStatStruct._maxStamina;
		int num7 = _scriptCreep._creepStatStruct._experience;
		if ((bool)_scriptStatModifier)
		{
			if ((bool)_scriptStatModifier._modifierElement)
			{
				scriptableCombatElement = _scriptStatModifier._modifierElement;
			}
			statStruct = _scriptStatModifier._modifierStatStruct;
			num = (float)_creepLevel * _scriptStatModifier._creepStatModifierPercentage;
			num2 = (float)_creepLevel * _scriptStatModifier._experiencePercentage;
			num3 = (float)_creepLevel * _scriptStatModifier._creepMaxHealthBoostPercentage;
			Network_statModifierID = _scriptStatModifier._modifierID;
			if (_scriptStatModifier._overrideDamageType)
			{
				_creepAttribute = _scriptStatModifier._damageType;
			}
			else
			{
				_creepAttribute = _scriptCreep._creepAttribute;
			}
		}
		else
		{
			_creepAttribute = _scriptCreep._creepAttribute;
			Network_statModifierID = 0;
		}
		int num8 = ((!_parentCreepSpawner._creepSpawnerReigon) ? _parentCreepSpawner._playersWithinSpawnerRadius.Count : _parentCreepSpawner._creepSpawnerReigon._playersInReigon.Count);
		if ((bool)_parentCreepSpawner._creepArena && (bool)_parentCreepSpawner._creepArena._patternInstanceManager && (bool)_parentCreepSpawner._creepArena._patternInstanceManager.Network_instanceParty)
		{
			num8 = _parentCreepSpawner._creepArena._patternInstanceManager.Network_instanceParty._syncPartyPeers.Count;
		}
		if (num8 > 1)
		{
			if (_creepLevel >= 5)
			{
				int num9 = (int)((float)num4 * GameManager._current._statLogics._creepMaxHealthModifier) * num8;
				num4 += num9;
			}
			int num10 = (int)((float)num7 * GameManager._current._statLogics._creepExperienceModifier) * num8;
			num7 += num10;
		}
		if (num4 <= 0)
		{
			num4 = 1;
		}
		if (num5 <= 0)
		{
			num5 = 1;
		}
		if (num6 <= 0)
		{
			num6 = 1;
		}
		StatStruct network_statStruct = new StatStruct
		{
			_maxHealth = num4 + (int)((float)statStruct._maxHealth * num) + (int)((float)num4 * num3),
			_maxMana = num5 + (int)((float)statStruct._maxMana * num),
			_maxStamina = num6 + (int)((float)statStruct._maxStamina * num),
			_experience = num7 + (int)((float)statStruct._experience * num2),
			_attackPower = _scriptCreep._creepStatStruct._attackPower + (int)((float)statStruct._attackPower * num),
			_magicPower = _scriptCreep._creepStatStruct._magicPower + (int)((float)statStruct._magicPower * num),
			_dexPower = _scriptCreep._creepStatStruct._dexPower + (int)((float)statStruct._dexPower * num),
			_defense = _scriptCreep._creepStatStruct._defense + (int)((float)statStruct._defense * num),
			_magicDefense = _scriptCreep._creepStatStruct._magicDefense + (int)((float)statStruct._magicDefense * num),
			_criticalRate = _scriptCreep._creepStatStruct._criticalRate + statStruct._criticalRate * num,
			_magicCriticalRate = _scriptCreep._creepStatStruct._magicCriticalRate + statStruct._magicCriticalRate * num,
			_evasion = statLogics._baseEvasion + _scriptCreep._creepStatStruct._evasion + statStruct._evasion * num,
			_fireResist = _scriptCreep._creepStatStruct._fireResist + (int)((float)statStruct._fireResist * num),
			_waterResist = _scriptCreep._creepStatStruct._waterResist + (int)((float)statStruct._waterResist * num),
			_natureResist = _scriptCreep._creepStatStruct._natureResist + (int)((float)statStruct._natureResist * num),
			_earthResist = _scriptCreep._creepStatStruct._earthResist + (int)((float)statStruct._earthResist * num),
			_holyResist = _scriptCreep._creepStatStruct._holyResist + (int)((float)statStruct._holyResist * num),
			_shadowResist = _scriptCreep._creepStatStruct._shadowResist + (int)((float)statStruct._shadowResist * num)
		};
		float creepResistModifierPercent = GameManager._current._statLogics._creepResistModifierPercent;
		if (!scriptableCombatElement)
		{
			scriptableCombatElement = GameManager._current._statLogics._defaultCombatElement;
		}
		switch (scriptableCombatElement._elementID)
		{
		case 1:
			network_statStruct._fireResist += (int)((float)_creepLevel * creepResistModifierPercent);
			network_statStruct._waterResist -= (int)((float)_creepLevel * creepResistModifierPercent);
			break;
		case 2:
			network_statStruct._waterResist += (int)((float)_creepLevel * creepResistModifierPercent);
			network_statStruct._earthResist -= (int)((float)_creepLevel * creepResistModifierPercent);
			network_statStruct._natureResist -= (int)((float)_creepLevel * creepResistModifierPercent);
			break;
		case 3:
			network_statStruct._natureResist += (int)((float)_creepLevel * creepResistModifierPercent);
			network_statStruct._fireResist -= (int)((float)_creepLevel * creepResistModifierPercent);
			break;
		case 4:
			network_statStruct._earthResist += (int)((float)_creepLevel * creepResistModifierPercent);
			network_statStruct._natureResist -= (int)((float)_creepLevel * creepResistModifierPercent);
			break;
		case 5:
			network_statStruct._holyResist += (int)((float)_creepLevel * creepResistModifierPercent);
			network_statStruct._shadowResist -= (int)((float)_creepLevel * creepResistModifierPercent);
			break;
		case 6:
			network_statStruct._shadowResist += (int)((float)_creepLevel * creepResistModifierPercent);
			network_statStruct._holyResist -= (int)((float)_creepLevel * creepResistModifierPercent);
			break;
		}
		Network_statStruct = network_statStruct;
		Network_combatElementID = scriptableCombatElement._elementID;
		_statusEntity.Network_currentHealth = _statStruct._maxHealth;
		_statusEntity.Network_currentMana = _statStruct._maxMana;
	}

	[ClientRpc]
	private void Rpc_OnCreepSpawn()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendRPCInternal("System.Void Creep::Rpc_OnCreepSpawn()", 902541009, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	private IEnumerator Init_SpawnInRpc()
	{
		if ((bool)_creepObject)
		{
			_creepObject.SetActive(value: true);
			_rendModify._scaleBuffer = 0f;
			_creepObject.transform.localScale = new Vector3(0f, 8.85f, 0f);
			_shadowProjectionObject.transform.localScale = Vector3.zero;
			_creepAnim.CrossFade("Idle", 0f);
			_creepAnim.CrossFade("Empty", 0f, 2);
			_rendModify._defaultColor = Color.white;
			_rendModify.Tint_Shader(Color.white);
			yield return new WaitForSeconds(0.32f);
			Init_SpawnSound();
			yield return new WaitForSeconds(0.65f);
			_rendModify._scaleBuffer = 2.5f;
			_miscEffectBuffer = 1.2f;
		}
		void Init_SpawnSound()
		{
			if ((bool)Player._mainPlayer && !(Player._mainPlayer.gameObject.scene != base.gameObject.scene))
			{
				_spawnInEffect.gameObject.SetActive(value: true);
				_spawnInEffect.Play();
				_creepSpawnASrc.mute = Player._mainPlayer._bufferingStatus;
				_creepSpawnASrc.pitch = Random.Range(0.9854382f, 1.0843763f);
				_creepSpawnASrc.Play();
			}
		}
	}

	[ServerCallback]
	public void Server_BaseCreep_OnHurt()
	{
		if (NetworkServer.active)
		{
			_creepBehavior.Behavior_OnHurt();
			if (!(_hurtRpcBuffer > 0f))
			{
				Rpc_PlayCreepHurtEffect();
				_hurtRpcBuffer = 0.12f;
			}
		}
	}

	[ClientRpc]
	public void Rpc_PlayCreepHurtEffect()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendRPCInternal("System.Void Creep::Rpc_PlayCreepHurtEffect()", 620004503, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	[ServerCallback]
	public virtual void BaseCreep_OnDeath()
	{
		if (NetworkServer.active && _currentCreepCondition != CreepCondition.DEAD && !_scriptCreep._unkillable)
		{
			bool isGibbed = _statusEntity._currentHealth <= -10;
			Server_OnDeathCondition();
			Rpc_InitCreepDeathParams(isGibbed);
		}
	}

	[ServerCallback]
	public void Server_OnDeathCondition()
	{
		if (NetworkServer.active)
		{
			if (_setRespawnBuffer > 0f)
			{
				_respawnBuffer = _setRespawnBuffer + Random.Range(0f, 2.5f);
			}
			_creepAwareness._onDeathBuffer = 0.26f;
			Network_currentCreepCondition = CreepCondition.DEAD;
			Server_CrossFadeAnim("Death", 0f, 3);
			_creepBehavior.Reset_Behavior();
			_creepPushCollider.enabled = false;
			if (_scriptCreep._pathfindParams._makeKinematicOnDeath)
			{
				_creepRB.isKinematic = true;
			}
			else
			{
				_creepRB.velocity = new Vector3(0f, _creepRB.velocity.y, 0f);
			}
			_creepAwareness._aggroTargetBeforeDeath = Network_aggroedEntity;
			Reset_AggroParameters();
			if ((bool)_parentCreepSpawner._deathCounterTrigger)
			{
				_parentCreepSpawner._deathCounterTrigger.Add_Counter();
			}
			if ((bool)_parentCreepSpawner._creepArena)
			{
				_parentCreepSpawner._creepArena._killedCreepsInWave++;
			}
			_parentCreepSpawner.CurrentKillCount++;
			OnDeath_QueryData_ToPlayer();
		}
		void Apply_PartyDistribution()
		{
			PartyObjectBehavior network_partyObject = _playerTagged.Network_partyObject;
			for (int i = 0; i < network_partyObject._syncPartyPeers.Count; i++)
			{
				if ((!_parentCreepSpawner || !(network_partyObject._syncPartyPeers[i].Network_playerMapInstance != _parentCreepSpawner._mapInstance)) && network_partyObject._syncPartyPeers[i]._currentPlayerCondition != PlayerCondition.DEAD && !network_partyObject._syncPartyPeers[i]._bufferingStatus)
				{
					float num = Vector3.Distance(_playerTagged.transform.position, network_partyObject._syncPartyPeers[i].transform.position);
					if (num <= GameManager._current._statLogics._partyExperienceProximity || _parentCreepSpawner._playersWithinSpawnerRadius.Contains(network_partyObject._syncPartyPeers[i]))
					{
						float percentage = 1f;
						for (int j = 0; j < _creepThreatSlots.Count && !(_creepThreatSlots[j]._player == network_partyObject._syncPartyPeers[i]); j++)
						{
							if (j >= _creepThreatSlots.Count && _creepThreatSlots[j]._player != network_partyObject._syncPartyPeers[i])
							{
								percentage = 0.06f;
							}
						}
						if (network_partyObject._syncPartyPeers.Count == 1)
						{
							percentage = 1f;
						}
						else if (network_partyObject._syncPartyPeers.Count == 2)
						{
							percentage = 0.95f;
						}
						else if (network_partyObject._syncPartyPeers.Count == 3)
						{
							percentage = 0.85f;
						}
						else if (network_partyObject._syncPartyPeers.Count == 4)
						{
							percentage = 0.65f;
						}
						else if (network_partyObject._syncPartyPeers.Count == 5)
						{
							percentage = 0.55f;
						}
						Init_SendExp(network_partyObject._syncPartyPeers[i], percentage);
					}
					if (num <= GameManager._current._statLogics._partyQuestPointProximity || _parentCreepSpawner._playersWithinSpawnerRadius.Contains(network_partyObject._syncPartyPeers[i]))
					{
						network_partyObject._syncPartyPeers[i]._pQuest.Target_Query_CreepKillProgress(_scriptCreep._creepName);
					}
				}
			}
		}
		void Init_SendExp(Player _p, float _percentage)
		{
			if ((bool)_p)
			{
				PlayerStats pStats = _p._pStats;
				int num = (int)GameManager._current._statLogics._experienceGainCurve.Evaluate(_creepLevel) + _scriptCreep._creepStatStruct._experience;
				num = (int)((float)num * _percentage);
				if ((bool)_scriptStatModifier)
				{
					num += (int)((float)num * _scriptStatModifier._experiencePercentage);
				}
				pStats.GainExp(num, _creepLevel);
			}
		}
		void OnDeath_QueryData_ToPlayer()
		{
			if ((bool)_playerTagged && !_playerTagged.Network_partyObject)
			{
				_playerTagged._pQuest.Target_Query_CreepKillProgress(_scriptCreep._creepName);
				Init_SendExp(_playerTagged, 1f);
			}
			if ((bool)_playerTagged && (bool)_playerTagged.Network_partyObject)
			{
				Apply_PartyDistribution();
			}
		}
	}

	[ClientRpc]
	private void Rpc_InitCreepDeathParams(bool _isGibbed)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteBool(_isGibbed);
		SendRPCInternal("System.Void Creep::Rpc_InitCreepDeathParams(System.Boolean)", -1499682856, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	private void Update_CreepObject()
	{
		if (!_creepObject)
		{
			return;
		}
		if (!Player._mainPlayer && !base.isServerOnly)
		{
			Toggle_ModelRend(_isTrue: false);
			return;
		}
		if (TabMenu._current._optionsCell._isDisconnecting)
		{
			Toggle_ModelRend(_isTrue: false);
			return;
		}
		if (_creepObject.transform.parent != null)
		{
			_creepObject.transform.SetParent(null);
		}
		Apply_CreepObjectVisualParams();
		Handle_ActiveCreepObject();
		Handle_DeadCreepObject();
		void Apply_CreepObjectVisualParams()
		{
			GeneralCreepVisual _cVisual = _scriptCreep._generalCreepVisual;
			if ((bool)Player._mainPlayer && Player._mainPlayer._bufferingStatus)
			{
				_rendModify._scaleBuffer = 1.2f;
				_miscEffectBuffer = 1.2f;
			}
			if (_miscEffectBuffer > 0f)
			{
				Handle_StatModifierEffect();
				Handle_ShadowProjectionEffect();
				float num = _cVisual._creepVisualScale;
				if ((bool)_scriptStatModifier)
				{
					num += _scriptStatModifier._creepScaleAdd;
				}
				_rendModify._setVisualScale = num;
				_bloodEffect.transform.localScale = _cVisual._bloodEffectSize * Vector3.one;
				_gibEffect.transform.localPosition = _cVisual._targeterOffset * Vector3.up;
				_gibEffect.transform.localScale = _cVisual._gibEffectSize * Vector3.one;
				_aggroIconPos.localPosition = Vector3.up * _cVisual._aggroiconYPosition;
				_spawnInEffect.transform.localPosition = Vector3.up * _cVisual._targeterOffset;
				_parryStaggerEffect.transform.localPosition = new Vector3(0f, _cVisual._lockonArrowOffset, 0f);
				_creepDynamicASrc.pitch = _scriptCreep._generalCreepAudio._dynamicAudioPitch;
			}
			if (_isParryStaggered)
			{
				if (!_creepParryStaggerAsrc.isPlaying)
				{
					_creepParryStaggerAsrc.Play();
				}
				if (!_parryStaggerEffect.isEmitting)
				{
					_parryStaggerEffect.Play();
				}
				_rendModify._scaleBuffer = 1.2f;
				_miscEffectBuffer = 1.2f;
			}
			else
			{
				_parryStaggerEffect.Stop();
			}
			void Handle_ShadowProjectionEffect()
			{
				if (_currentCreepCondition == CreepCondition.ACTIVE)
				{
					_shadowProjectionObject.transform.localScale = Vector3.Lerp(_shadowProjectionObject.transform.localScale, Vector3.one * _cVisual._shadowProjectionScale, 8.5f * Time.deltaTime);
				}
				else if (_shadowProjectionObject.transform.localScale != Vector3.zero)
				{
					_shadowProjectionObject.transform.localScale = Vector3.zero;
				}
			}
			void Handle_StatModifierEffect()
			{
				if ((bool)Player._mainPlayer)
				{
					if ((bool)_parentCreepSpawner && _parentCreepSpawner._mapInstance != Player._mainPlayer.Network_playerMapInstance)
					{
						_modifierEmblemEffect.gameObject.SetActive(value: false);
					}
					else
					{
						_modifierEmblemEffect.gameObject.SetActive((bool)_scriptStatModifier && (bool)_scriptStatModifier._modifierEmblemTexture);
					}
				}
				else
				{
					_modifierEmblemEffect.gameObject.SetActive(value: false);
				}
				if ((bool)_scriptStatModifier)
				{
					Vector3 b = Vector3.one * 12.5f * (_cVisual._creepVisualScale * 0.65f);
					_modifierEmblemEffect.localPosition = Vector3.up * _cVisual._modifierEmblemTexturePos;
					if (_currentCreepCondition == CreepCondition.ACTIVE)
					{
						_modifierEmblemEffect.localScale = Vector3.Lerp(_modifierEmblemEffect.localScale, b, 7.5f * Time.deltaTime);
					}
					else
					{
						_modifierEmblemEffect.localScale = Vector3.Lerp(_modifierEmblemEffect.localScale, Vector3.zero, 7.5f * Time.deltaTime);
					}
					if ((bool)_scriptStatModifier._modifierEmblemTexture)
					{
						for (int i = 0; i < _modifierEmblemMeshes.Length; i++)
						{
							_modifierEmblemMeshes[i].material.mainTexture = _scriptStatModifier._modifierEmblemTexture;
							_modifierEmblemMeshes[i].material.SetColor("_TintColor", _scriptStatModifier._glowTintColor);
							_modifierEmblemMeshes[i].enabled = true;
						}
					}
				}
			}
		}
		void Handle_ActiveCreepObject()
		{
			if (_currentCreepCondition == CreepCondition.ACTIVE && _statusEntity._currentHealth > 0)
			{
				Toggle_ModelRend(Player._mainPlayer.gameObject.scene == base.gameObject.scene);
				if (_isParryStaggered)
				{
					_creepObject.transform.SetPositionAndRotation(Vector3.Lerp(_creepObject.transform.position, base.transform.position, 20.5f * Time.deltaTime), _creepObject.transform.rotation);
				}
				else if (Snap_ToBaseCreepRotation())
				{
					_creepObject.transform.SetPositionAndRotation(Vector3.Lerp(_creepObject.transform.position, base.transform.position, 20.5f * Time.deltaTime), base.transform.rotation);
				}
				else
				{
					Vector3 forward = Network_aggroedEntity.transform.position - base.transform.position;
					forward.y = 0f;
					_creepObject.transform.SetPositionAndRotation(Vector3.Lerp(_creepObject.transform.position, base.transform.position, 20.5f * Time.deltaTime), Quaternion.Lerp(_creepObject.transform.rotation, Quaternion.LookRotation(forward), 35.5f * Time.deltaTime));
				}
			}
		}
		void Handle_DeadCreepObject()
		{
			if (_currentCreepCondition != CreepCondition.ACTIVE)
			{
				if ((bool)Player._mainPlayer && Player._mainPlayer._bufferingStatus)
				{
					Toggle_ModelRend(_isTrue: false);
				}
				_creepObject.transform.SetPositionAndRotation(base.transform.position, base.transform.rotation);
			}
		}
		bool Snap_ToBaseCreepRotation()
		{
			if (!Network_aggroedEntity)
			{
				return true;
			}
			if (_ignoreAggroedTarget)
			{
				return true;
			}
			if (_pathFindCtrl._dontLookAtTarget)
			{
				return true;
			}
			return false;
		}
	}

	[ClientRpc]
	public void Rpc_TintRender(Color _colorTint)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteColor(_colorTint);
		SendRPCInternal("System.Void Creep::Rpc_TintRender(UnityEngine.Color)", 670686333, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	[ClientCallback]
	public void Toggle_ModelRend(bool _isTrue)
	{
		if (!NetworkClient.active)
		{
			return;
		}
		if (!Player._mainPlayer)
		{
			_isTrue = false;
		}
		if (!_creepObject || ((bool)Player._mainPlayer && !Player._mainPlayer._bufferingStatus && ((_isCulled && !_isTrue) || (!_isCulled & _isTrue))))
		{
			return;
		}
		if ((bool)_shadowProjectionObject)
		{
			GameObject gameObject = _shadowProjectionObject.transform.GetChild(0).gameObject;
			if ((bool)Player._mainPlayer && base.gameObject.scene == Player._mainPlayer.gameObject.scene)
			{
				if (_isTrue)
				{
					gameObject.layer = 0;
				}
				else
				{
					gameObject.layer = 9;
				}
			}
			else
			{
				gameObject.layer = 9;
			}
		}
		for (int i = 0; i < _meshRends.Length; i++)
		{
			_meshRends[i].enabled = _isTrue;
		}
		for (int j = 0; j < _otherMeshRends.Length; j++)
		{
			_otherMeshRends[j].enabled = _isTrue;
		}
		_isCulled = !_isTrue;
	}

	[ClientCallback]
	private void Handle_AudioCache()
	{
		if (!NetworkClient.active || _aSrcCache.Length == 0)
		{
			return;
		}
		for (int i = 0; i < _aSrcCache.Length; i++)
		{
			if (!Player._mainPlayer)
			{
				_aSrcCache[i].mute = true;
			}
			else
			{
				_aSrcCache[i].mute = base.gameObject.scene != Player._mainPlayer.gameObject.scene;
			}
		}
	}

	public void Update_CreepAnimation()
	{
		_creepAnim.SetBool("_isMoving", _isMoving);
		_creepAnim.SetBool("_parryStaggered", _isParryStaggered);
	}

	[ClientRpc]
	public void Rpc_InitSkillChargeEffect(string _skillName, float _castTimeModifier)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteString(_skillName);
		writer.WriteFloat(_castTimeModifier);
		SendRPCInternal("System.Void Creep::Rpc_InitSkillChargeEffect(System.String,System.Single)", -908834377, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	public void Rpc_InitSkillCastEffect(string _skillName)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteString(_skillName);
		SendRPCInternal("System.Void Creep::Rpc_InitSkillCastEffect(System.String)", 1481599015, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	[ServerCallback]
	public void Server_CrossFadeAnim(string _animName, float _animFadeTime, int _animLayer)
	{
		if (NetworkServer.active && !(_creepAnim == null) && !(_creepAnim.gameObject == null))
		{
			_creepAnim.CrossFade(_animName, _animFadeTime, _animLayer);
			Rpc_CrossFadeAnim(_animName, _animFadeTime, _animLayer);
		}
	}

	[ClientRpc]
	public void Rpc_CrossFadeAnim(string _animName, float _animFadeTime, int _animLayer)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteString(_animName);
		writer.WriteFloat(_animFadeTime);
		writer.WriteInt(_animLayer);
		SendRPCInternal("System.Void Creep::Rpc_CrossFadeAnim(System.String,System.Single,System.Int32)", 298863783, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	[ServerCallback]
	public void Server_CrossFadeAnimTimed(string _animName, float _animFadeTime, int _animLayer, float _delay)
	{
		if (NetworkServer.active && !(_creepAnim == null) && !(_creepAnim.gameObject == null))
		{
			StartCoroutine(CrossFadeAnim_Timed(_delay, _animName, _animFadeTime, _animLayer));
			Rpc_CrossFadeAnim_Timed(_animName, _animFadeTime, _animLayer, _delay);
		}
	}

	[ClientRpc]
	public void Rpc_CrossFadeAnim_Timed(string _animName, float _animFadeTime, int _animLayer, float _delay)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteString(_animName);
		writer.WriteFloat(_animFadeTime);
		writer.WriteInt(_animLayer);
		writer.WriteFloat(_delay);
		SendRPCInternal("System.Void Creep::Rpc_CrossFadeAnim_Timed(System.String,System.Single,System.Int32,System.Single)", 1552179428, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	private IEnumerator CrossFadeAnim_Timed(float _time, string _animName, float _animFadeTime, int _animLayer)
	{
		yield return new WaitForSeconds(_time);
		_creepAnim.CrossFade(_animName, _animFadeTime, _animLayer);
		Animation_Callback(_animName);
	}

	[ClientCallback]
	private void Animation_Callback(string _animName)
	{
		if (NetworkClient.active && _animName == "Hurt_Heavy")
		{
			Vector3 vector = Vector3.one * _scriptCreep._generalCreepVisual._creepVisualScale;
			_creepObject.transform.localScale = new Vector3(vector.x * 1.08f, vector.y * 2f, vector.z * 1.08f);
		}
	}

	[ClientRpc]
	public void Rpc_InitUnspawnEffect(Vector3 _pos, float _time)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteVector3(_pos);
		writer.WriteFloat(_time);
		SendRPCInternal("System.Void Creep::Rpc_InitUnspawnEffect(UnityEngine.Vector3,System.Single)", -335341999, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	private IEnumerator Init_UnspawnEffect_Timed(Vector3 _pos, float _time)
	{
		yield return new WaitForSeconds(_time);
		Init_UnspawnEffect(_pos);
		Toggle_ModelRend(_isTrue: false);
	}

	[ClientCallback]
	public void Init_UnspawnEffect(Vector3 _pos)
	{
		if (NetworkClient.active && (bool)Player._mainPlayer && (!Player._mainPlayer._isHostPlayer || !(base.gameObject.scene != Player._mainPlayer.gameObject.scene)))
		{
			GameObject obj = PoolManager._current.GenerateObject(null, "deAggro", 1.25f);
			ParticleSystem component = obj.GetComponent<ParticleSystem>();
			AudioSource componentInChildren = obj.GetComponentInChildren<AudioSource>();
			obj.transform.position = _pos;
			component.Play();
			componentInChildren.Play();
		}
	}

	public override bool Weaved()
	{
		return true;
	}

	protected void UserCode_Rpc_PlayCreepJumpEffect()
	{
		if ((bool)Player._mainPlayer && !(base.gameObject.scene != Player._mainPlayer.gameObject.scene))
		{
			Vector3 vector = Vector3.one * _scriptCreep._generalCreepVisual._creepVisualScale;
			_creepObject.transform.localScale = new Vector3(vector.x * 0.85f, vector.y * 2.35f, vector.z * 0.85f);
			_rendModify._scaleBuffer = 2.3f;
			_miscEffectBuffer = 2.3f;
			_creepJumpAsrc.Stop();
			_creepJumpAsrc.Play();
		}
	}

	protected static void InvokeUserCode_Rpc_PlayCreepJumpEffect(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC Rpc_PlayCreepJumpEffect called on server.");
		}
		else
		{
			((Creep)obj).UserCode_Rpc_PlayCreepJumpEffect();
		}
	}

	protected void UserCode_Rpc_PlayAggroIcon__Int32(int _value)
	{
		Init_AggroIcon(_value);
	}

	protected static void InvokeUserCode_Rpc_PlayAggroIcon__Int32(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC Rpc_PlayAggroIcon called on server.");
		}
		else
		{
			((Creep)obj).UserCode_Rpc_PlayAggroIcon__Int32(reader.ReadInt());
		}
	}

	protected void UserCode_Rpc_OnCreepSpawn()
	{
		StartCoroutine(Init_SpawnInRpc());
	}

	protected static void InvokeUserCode_Rpc_OnCreepSpawn(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC Rpc_OnCreepSpawn called on server.");
		}
		else
		{
			((Creep)obj).UserCode_Rpc_OnCreepSpawn();
		}
	}

	protected void UserCode_Rpc_PlayCreepHurtEffect()
	{
		if ((bool)Player._mainPlayer && !(Player._mainPlayer.gameObject.scene != base.gameObject.scene))
		{
			if ((bool)_scriptCreep._generalCreepAudio._painSfx)
			{
				_creepDynamicASrc.PlayOneShot(_scriptCreep._generalCreepAudio._painSfx, 0.42f);
			}
			Vector3 vector = Vector3.one * _scriptCreep._generalCreepVisual._creepVisualScale;
			_creepObject.transform.localScale = new Vector3(vector.x * 1.08f, vector.y * 1.15f, vector.z * 1.08f);
			_rendModify._scaleBuffer = 1.2f;
			_miscEffectBuffer = 1.2f;
			_rendModify.Tint_Shader(Color.red);
			_bloodEffect.Play();
		}
	}

	protected static void InvokeUserCode_Rpc_PlayCreepHurtEffect(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC Rpc_PlayCreepHurtEffect called on server.");
		}
		else
		{
			((Creep)obj).UserCode_Rpc_PlayCreepHurtEffect();
		}
	}

	protected void UserCode_Rpc_InitCreepDeathParams__Boolean(bool _isGibbed)
	{
		if ((bool)Player._mainPlayer && !Player._mainPlayer._bufferingStatus)
		{
			if ((bool)_scriptCreep._generalCreepAudio._deathSfx)
			{
				_creepDynamicASrc.PlayOneShot(_scriptCreep._generalCreepAudio._deathSfx, 0.65f);
			}
			if (_isGibbed && _scriptCreep._generalCreepVisual._canGib && !SettingsManager._current._disableGibsToggle.isOn)
			{
				Init_GibEffect();
			}
			else
			{
				Init_StandardDeathEffect();
			}
			Init_DropItems();
			_miscEffectBuffer = 1.2f;
			Vector3 pos = base.transform.position + Vector3.up * _scriptCreep._generalCreepVisual._targeterOffset;
			StartCoroutine(Init_UnspawnEffect_Timed(pos, _scriptCreep._generalCreepVisual._deathAnimDespawnTime));
		}
		bool Drop_ForPlayer()
		{
			if (Player._mainPlayer._isHostPlayer && Player._mainPlayer.gameObject.scene != base.gameObject.scene)
			{
				return false;
			}
			if (Vector3.Distance(Player._mainPlayer.transform.position, base.transform.position) >= 125f)
			{
				return false;
			}
			return true;
		}
		void Init_CreepLootTable()
		{
			List<ItemDrop> list = new List<ItemDrop>();
			list.AddRange(_scriptCreep._itemDrops);
			_itemDropEntity._itemDrops = list.ToArray();
		}
		void Init_DropItems()
		{
			Init_CreepLootTable();
			if (Drop_ForPlayer())
			{
				NetworkIdentity netIDTag = Player._mainPlayer.netIdentity;
				float currencyPercent = 1f;
				if ((bool)_scriptStatModifier)
				{
					currencyPercent = _scriptStatModifier._currencyDropPercentage;
				}
				_itemDropEntity.Client_DropCurrency(_itemDropEntity._currencyRate, currencyPercent);
				_itemDropEntity.Client_DropItems(netIDTag);
			}
		}
		void Init_GibEffect()
		{
			if (!(Player._mainPlayer.gameObject.scene != base.gameObject.scene))
			{
				_gibEffect.Play();
				if (_creepGibASrc.enabled)
				{
					_creepGibASrc.Play();
				}
				if ((bool)Player._mainPlayer && Vector3.Distance(Player._mainPlayer.transform.position, base.transform.position) <= 38.5f)
				{
					CameraFunction._current.MyShaker.Shake(CameraFunction._current.ShakePreset);
				}
				_rendModify._RefreshScale = false;
				Toggle_ModelRend(_isTrue: false);
			}
		}
		void Init_StandardDeathEffect()
		{
			if (!(Player._mainPlayer.gameObject.scene != base.gameObject.scene))
			{
				_rendModify._RefreshScale = false;
				_bloodEffect.Play();
				float num = _scriptCreep._generalCreepVisual._creepVisualScale;
				if ((bool)_scriptStatModifier)
				{
					num += _scriptStatModifier._creepScaleAdd;
				}
				_creepObject.transform.localScale = Vector3.one * num;
			}
		}
	}

	protected static void InvokeUserCode_Rpc_InitCreepDeathParams__Boolean(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC Rpc_InitCreepDeathParams called on server.");
		}
		else
		{
			((Creep)obj).UserCode_Rpc_InitCreepDeathParams__Boolean(reader.ReadBool());
		}
	}

	protected void UserCode_Rpc_TintRender__Color(Color _colorTint)
	{
		if ((bool)_creepObject)
		{
			_rendModify.Tint_Shader(_colorTint);
		}
	}

	protected static void InvokeUserCode_Rpc_TintRender__Color(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC Rpc_TintRender called on server.");
		}
		else
		{
			((Creep)obj).UserCode_Rpc_TintRender__Color(reader.ReadColor());
		}
	}

	protected void UserCode_Rpc_InitSkillChargeEffect__String__Single(string _skillName, float _castTimeModifier)
	{
		if (string.IsNullOrEmpty(_skillName) || !_creepBehavior || !Player._mainPlayer || (Player._mainPlayer._isHostPlayer && Player._mainPlayer.gameObject.scene != base.gameObject.scene))
		{
			return;
		}
		ScriptableSkill scriptableSkill = GameManager._current.Locate_Skill(_skillName);
		if ((bool)scriptableSkill && _creepBehavior.GetType() == typeof(CreepBehavior_StandardCreep))
		{
			CreepBehavior_StandardCreep creepBehavior_StandardCreep = (CreepBehavior_StandardCreep)_creepBehavior;
			creepBehavior_StandardCreep._currentCastSkill = scriptableSkill;
			creepBehavior_StandardCreep._setCastTimeModifier = _castTimeModifier;
			for (int i = 0; i < creepBehavior_StandardCreep._creepCastEffectPositions.Length; i++)
			{
				GameObject obj = Object.Instantiate(scriptableSkill._castEffectCollection._chargeSkillEffect, creepBehavior_StandardCreep._creepCastEffectPositions[i]);
				obj.transform.localScale = Vector3.one * (_scriptCreep._generalCreepVisual._creepVisualScale + 2.5f);
				_creepSkillChargeASrc.Stop();
				_creepSkillChargeASrc.clip = scriptableSkill._castEffectCollection._channelSound;
				_creepSkillChargeASrc.Play();
				ParticleSystem component = obj.GetComponent<ParticleSystem>();
				creepBehavior_StandardCreep._cachedSkillChargeEffects.Add(component);
			}
		}
	}

	protected static void InvokeUserCode_Rpc_InitSkillChargeEffect__String__Single(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC Rpc_InitSkillChargeEffect called on server.");
		}
		else
		{
			((Creep)obj).UserCode_Rpc_InitSkillChargeEffect__String__Single(reader.ReadString(), reader.ReadFloat());
		}
	}

	protected void UserCode_Rpc_InitSkillCastEffect__String(string _skillName)
	{
		if (!base.isServer && (bool)_creepBehavior && (bool)Player._mainPlayer && !string.IsNullOrEmpty(_skillName))
		{
			ScriptableSkill scriptableSkill = GameManager._current.Locate_Skill(_skillName);
			if ((bool)scriptableSkill && _creepBehavior.GetType() == typeof(CreepBehavior_StandardCreep))
			{
				CreepBehavior_StandardCreep obj = (CreepBehavior_StandardCreep)_creepBehavior;
				scriptableSkill.Init_SkillBehavior(_statusEntity);
				obj._currentCastSkill = null;
			}
		}
	}

	protected static void InvokeUserCode_Rpc_InitSkillCastEffect__String(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC Rpc_InitSkillCastEffect called on server.");
		}
		else
		{
			((Creep)obj).UserCode_Rpc_InitSkillCastEffect__String(reader.ReadString());
		}
	}

	protected void UserCode_Rpc_CrossFadeAnim__String__Single__Int32(string _animName, float _animFadeTime, int _animLayer)
	{
		if (!(_creepAnim == null) && !(_creepAnim.gameObject == null))
		{
			_creepAnim.CrossFade(_animName, _animFadeTime, _animLayer);
			Animation_Callback(_animName);
			_rendModify._scaleBuffer = 1.2f;
			_miscEffectBuffer = 1.2f;
		}
	}

	protected static void InvokeUserCode_Rpc_CrossFadeAnim__String__Single__Int32(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC Rpc_CrossFadeAnim called on server.");
		}
		else
		{
			((Creep)obj).UserCode_Rpc_CrossFadeAnim__String__Single__Int32(reader.ReadString(), reader.ReadFloat(), reader.ReadInt());
		}
	}

	protected void UserCode_Rpc_CrossFadeAnim_Timed__String__Single__Int32__Single(string _animName, float _animFadeTime, int _animLayer, float _delay)
	{
		if (!(_creepAnim == null) && !(_creepAnim.gameObject == null) && !base.isServer)
		{
			StartCoroutine(CrossFadeAnim_Timed(_delay, _animName, _animFadeTime, _animLayer));
			_rendModify._scaleBuffer = 1.2f;
			_miscEffectBuffer = 1.2f;
		}
	}

	protected static void InvokeUserCode_Rpc_CrossFadeAnim_Timed__String__Single__Int32__Single(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC Rpc_CrossFadeAnim_Timed called on server.");
		}
		else
		{
			((Creep)obj).UserCode_Rpc_CrossFadeAnim_Timed__String__Single__Int32__Single(reader.ReadString(), reader.ReadFloat(), reader.ReadInt(), reader.ReadFloat());
		}
	}

	protected void UserCode_Rpc_InitUnspawnEffect__Vector3__Single(Vector3 _pos, float _time)
	{
		if ((!Player._mainPlayer._isHostPlayer || !(base.gameObject.scene != Player._mainPlayer.gameObject.scene)) && (bool)Player._mainPlayer)
		{
			StartCoroutine(Init_UnspawnEffect_Timed(_pos, _time));
		}
	}

	protected static void InvokeUserCode_Rpc_InitUnspawnEffect__Vector3__Single(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC Rpc_InitUnspawnEffect called on server.");
		}
		else
		{
			((Creep)obj).UserCode_Rpc_InitUnspawnEffect__Vector3__Single(reader.ReadVector3(), reader.ReadFloat());
		}
	}

	static Creep()
	{
		RemoteProcedureCalls.RegisterRpc(typeof(Creep), "System.Void Creep::Rpc_PlayCreepJumpEffect()", InvokeUserCode_Rpc_PlayCreepJumpEffect);
		RemoteProcedureCalls.RegisterRpc(typeof(Creep), "System.Void Creep::Rpc_PlayAggroIcon(System.Int32)", InvokeUserCode_Rpc_PlayAggroIcon__Int32);
		RemoteProcedureCalls.RegisterRpc(typeof(Creep), "System.Void Creep::Rpc_OnCreepSpawn()", InvokeUserCode_Rpc_OnCreepSpawn);
		RemoteProcedureCalls.RegisterRpc(typeof(Creep), "System.Void Creep::Rpc_PlayCreepHurtEffect()", InvokeUserCode_Rpc_PlayCreepHurtEffect);
		RemoteProcedureCalls.RegisterRpc(typeof(Creep), "System.Void Creep::Rpc_InitCreepDeathParams(System.Boolean)", InvokeUserCode_Rpc_InitCreepDeathParams__Boolean);
		RemoteProcedureCalls.RegisterRpc(typeof(Creep), "System.Void Creep::Rpc_TintRender(UnityEngine.Color)", InvokeUserCode_Rpc_TintRender__Color);
		RemoteProcedureCalls.RegisterRpc(typeof(Creep), "System.Void Creep::Rpc_InitSkillChargeEffect(System.String,System.Single)", InvokeUserCode_Rpc_InitSkillChargeEffect__String__Single);
		RemoteProcedureCalls.RegisterRpc(typeof(Creep), "System.Void Creep::Rpc_InitSkillCastEffect(System.String)", InvokeUserCode_Rpc_InitSkillCastEffect__String);
		RemoteProcedureCalls.RegisterRpc(typeof(Creep), "System.Void Creep::Rpc_CrossFadeAnim(System.String,System.Single,System.Int32)", InvokeUserCode_Rpc_CrossFadeAnim__String__Single__Int32);
		RemoteProcedureCalls.RegisterRpc(typeof(Creep), "System.Void Creep::Rpc_CrossFadeAnim_Timed(System.String,System.Single,System.Int32,System.Single)", InvokeUserCode_Rpc_CrossFadeAnim_Timed__String__Single__Int32__Single);
		RemoteProcedureCalls.RegisterRpc(typeof(Creep), "System.Void Creep::Rpc_InitUnspawnEffect(UnityEngine.Vector3,System.Single)", InvokeUserCode_Rpc_InitUnspawnEffect__Vector3__Single);
	}

	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteString(_creepDisplayName);
			writer.WriteString(_creepNameID);
			writer.WriteInt(_statModifierID);
			writer.WriteBool(_isTargetable);
			writer.WriteBool(_disableTriggerColliders);
			GeneratedNetworkCode._Write_CreepCondition(writer, _currentCreepCondition);
			writer.WriteInt(_creepLevel);
			writer.WriteInt(_combatElementID);
			GeneratedNetworkCode._Write_StatStruct(writer, _statStruct);
			writer.WriteNetworkBehaviour(Network_aggroedEntity);
			writer.WriteBool(_ignoreAggroedTarget);
			writer.WriteBool(_isMoving);
			writer.WriteBool(_isParryStaggered);
			return;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteString(_creepDisplayName);
		}
		if ((base.syncVarDirtyBits & 2L) != 0L)
		{
			writer.WriteString(_creepNameID);
		}
		if ((base.syncVarDirtyBits & 4L) != 0L)
		{
			writer.WriteInt(_statModifierID);
		}
		if ((base.syncVarDirtyBits & 8L) != 0L)
		{
			writer.WriteBool(_isTargetable);
		}
		if ((base.syncVarDirtyBits & 0x10L) != 0L)
		{
			writer.WriteBool(_disableTriggerColliders);
		}
		if ((base.syncVarDirtyBits & 0x20L) != 0L)
		{
			GeneratedNetworkCode._Write_CreepCondition(writer, _currentCreepCondition);
		}
		if ((base.syncVarDirtyBits & 0x40L) != 0L)
		{
			writer.WriteInt(_creepLevel);
		}
		if ((base.syncVarDirtyBits & 0x80L) != 0L)
		{
			writer.WriteInt(_combatElementID);
		}
		if ((base.syncVarDirtyBits & 0x100L) != 0L)
		{
			GeneratedNetworkCode._Write_StatStruct(writer, _statStruct);
		}
		if ((base.syncVarDirtyBits & 0x200L) != 0L)
		{
			writer.WriteNetworkBehaviour(Network_aggroedEntity);
		}
		if ((base.syncVarDirtyBits & 0x400L) != 0L)
		{
			writer.WriteBool(_ignoreAggroedTarget);
		}
		if ((base.syncVarDirtyBits & 0x800L) != 0L)
		{
			writer.WriteBool(_isMoving);
		}
		if ((base.syncVarDirtyBits & 0x1000L) != 0L)
		{
			writer.WriteBool(_isParryStaggered);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize(ref _creepDisplayName, null, reader.ReadString());
			GeneratedSyncVarDeserialize(ref _creepNameID, null, reader.ReadString());
			GeneratedSyncVarDeserialize(ref _statModifierID, OnChangeStatModifier, reader.ReadInt());
			GeneratedSyncVarDeserialize(ref _isTargetable, null, reader.ReadBool());
			GeneratedSyncVarDeserialize(ref _disableTriggerColliders, null, reader.ReadBool());
			GeneratedSyncVarDeserialize(ref _currentCreepCondition, null, GeneratedNetworkCode._Read_CreepCondition(reader));
			GeneratedSyncVarDeserialize(ref _creepLevel, null, reader.ReadInt());
			GeneratedSyncVarDeserialize(ref _combatElementID, OnChangeElementID, reader.ReadInt());
			GeneratedSyncVarDeserialize(ref _statStruct, null, GeneratedNetworkCode._Read_StatStruct(reader));
			GeneratedSyncVarDeserialize_NetworkBehaviour(ref _aggroedEntity, OnChangeAggroTarget, reader, ref ____aggroedEntityNetId);
			GeneratedSyncVarDeserialize(ref _ignoreAggroedTarget, null, reader.ReadBool());
			GeneratedSyncVarDeserialize(ref _isMoving, null, reader.ReadBool());
			GeneratedSyncVarDeserialize(ref _isParryStaggered, null, reader.ReadBool());
			return;
		}
		long num = (long)reader.ReadULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _creepDisplayName, null, reader.ReadString());
		}
		if ((num & 2L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _creepNameID, null, reader.ReadString());
		}
		if ((num & 4L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _statModifierID, OnChangeStatModifier, reader.ReadInt());
		}
		if ((num & 8L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _isTargetable, null, reader.ReadBool());
		}
		if ((num & 0x10L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _disableTriggerColliders, null, reader.ReadBool());
		}
		if ((num & 0x20L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _currentCreepCondition, null, GeneratedNetworkCode._Read_CreepCondition(reader));
		}
		if ((num & 0x40L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _creepLevel, null, reader.ReadInt());
		}
		if ((num & 0x80L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _combatElementID, OnChangeElementID, reader.ReadInt());
		}
		if ((num & 0x100L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _statStruct, null, GeneratedNetworkCode._Read_StatStruct(reader));
		}
		if ((num & 0x200L) != 0L)
		{
			GeneratedSyncVarDeserialize_NetworkBehaviour(ref _aggroedEntity, OnChangeAggroTarget, reader, ref ____aggroedEntityNetId);
		}
		if ((num & 0x400L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _ignoreAggroedTarget, null, reader.ReadBool());
		}
		if ((num & 0x800L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _isMoving, null, reader.ReadBool());
		}
		if ((num & 0x1000L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _isParryStaggered, null, reader.ReadBool());
		}
	}
}
