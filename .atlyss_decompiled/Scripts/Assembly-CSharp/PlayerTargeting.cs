using System.Collections;
using System.Runtime.InteropServices;
using Mirror;
using Mirror.RemoteCalls;
using UnityEngine;

public class PlayerTargeting : NetworkBehaviour
{
	private Player _player;

	private PlayerMove _pMove;

	private PlayerCombat _pCombat;

	private PlayerSound _pSound;

	private StatusEntity _playerStatus;

	private CameraFunction camFunc;

	[Header("Entity Targeting")]
	[SerializeField]
	private float _targeterCastLength = 125.5f;

	[Header("Camera Parameters")]
	[SerializeField]
	private float _clampCameraTargeting_low = -35.5f;

	[SerializeField]
	private float _clampCameraTargeting_high = 50.5f;

	[SerializeField]
	private float _cameraTargetingSmooth = 6.85f;

	[SerializeField]
	private float _aimAdjust = -1.25f;

	private Vector3 _hitPoint = Vector3.zero;

	[Header("Targeted Entity Params")]
	[SyncVar]
	public NetworkIdentity _syncSelectedEntity;

	public StatusEntity _foundEntity;

	public StatusEntity _targetedEntity;

	public bool _isTargetingEntity;

	private float _targetDistance;

	private bool tabTargetBuffer;

	[Header("Lockon Reticle")]
	[SerializeField]
	private Transform _lockonReticleTransform;

	[SerializeField]
	private Transform _lockonArrowTransform;

	protected uint ____syncSelectedEntityNetId;

	public NetworkIdentity Network_syncSelectedEntity
	{
		get
		{
			return GetSyncVarNetworkIdentity(____syncSelectedEntityNetId, ref _syncSelectedEntity);
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter_NetworkIdentity(value, ref _syncSelectedEntity, 1uL, null, ref ____syncSelectedEntityNetId);
		}
	}

	private void Awake()
	{
		_player = GetComponent<Player>();
		_pMove = GetComponent<PlayerMove>();
		_pCombat = GetComponent<PlayerCombat>();
		_pSound = GetComponent<PlayerSound>();
		_playerStatus = GetComponent<StatusEntity>();
		_lockonReticleTransform.localScale = Vector3.one * 125.5f;
		_lockonReticleTransform.gameObject.SetActive(value: false);
		_lockonArrowTransform.gameObject.SetActive(value: false);
	}

	private void Start()
	{
		camFunc = CameraFunction._current;
	}

	private void Update()
	{
		Client_EntityTargeting();
	}

	private void FixedUpdate()
	{
		Client_ProjectileAimRotation();
	}

	[ClientCallback]
	private void Client_ProjectileAimRotation()
	{
		if (NetworkClient.active && base.isLocalPlayer)
		{
			if (_player._currentGameCondition == GameCondition.LOADING_GAME || _player._currentPlayerCondition != PlayerCondition.ACTIVE)
			{
				Disable_CrosshairDisplay();
			}
			else if (!Network_syncSelectedEntity)
			{
				Aim_StrafeLookRotation();
			}
			else
			{
				Aim_TargetLookRotation();
			}
		}
		void Aim_StrafeLookRotation()
		{
			InGameUI._current._crosshairImage.enabled = _pMove._strafeToggle;
			if (CameraFunction._current._unlockedCamera)
			{
				_pCombat._rangeWeaponBulletPos.localRotation = Quaternion.identity;
			}
			else if (_pMove._strafeToggle)
			{
				PhysicsScene physicsScene = base.gameObject.scene.GetPhysicsScene();
				Vector2 vector = new Vector2(Screen.width / 2, Screen.height / 2);
				Ray ray = CameraFunction._current._mainCamera.ScreenPointToRay(vector);
				if (physicsScene.Raycast(ray.origin, ray.direction, out var hitInfo, _aimAdjust, CastLayers.GroundMask))
				{
					_hitPoint = hitInfo.point;
				}
				else
				{
					_hitPoint = ray.origin + _aimAdjust * ray.direction;
				}
				if (Vector3.Distance(hitInfo.point, base.transform.position) >= 12.5f)
				{
					_pCombat._rangeWeaponBulletPos.LookAt(_hitPoint);
				}
				else if (_hitPoint.y > base.transform.position.y)
				{
					_pCombat._rangeWeaponBulletPos.localRotation = Quaternion.identity;
				}
			}
			else
			{
				_pCombat._rangeWeaponBulletPos.localRotation = Quaternion.identity;
			}
		}
		void Aim_TargetLookRotation()
		{
			float num = Vector3.Distance(Network_syncSelectedEntity.transform.position, base.transform.position);
			InGameUI._current._crosshairImage.enabled = false;
			if (num > 10.5f)
			{
				Vector3 position = Network_syncSelectedEntity.transform.position;
				float num2 = 3.5f;
				if ((bool)Network_syncSelectedEntity.GetComponent<Creep>())
				{
					num2 = Network_syncSelectedEntity.GetComponent<Creep>()._scriptCreep._generalCreepVisual._targeterOffset;
				}
				position.y = Network_syncSelectedEntity.transform.position.y + num2;
				_pCombat._rangeWeaponBulletPos.LookAt(position);
			}
			else
			{
				_pCombat._rangeWeaponBulletPos.localRotation = Quaternion.identity;
			}
		}
		void Disable_CrosshairDisplay()
		{
			InGameUI._current._crosshairImage.enabled = false;
			_pCombat._rangeWeaponBulletPos.localRotation = Quaternion.identity;
		}
	}

	[Command]
	private void Cmd_TargetSyncCreep(NetworkIdentity nId)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteNetworkIdentity(nId);
		SendCommandInternal("System.Void PlayerTargeting::Cmd_TargetSyncCreep(Mirror.NetworkIdentity)", -1278361272, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientCallback]
	private void Client_EntityTargeting()
	{
		if (!NetworkClient.active || !base.isLocalPlayer)
		{
			return;
		}
		if (_player._currentGameCondition != GameCondition.IN_GAME || _player._currentPlayerCondition != PlayerCondition.ACTIVE)
		{
			_lockonArrowTransform.gameObject.SetActive(value: false);
			Client_EndTarget();
			return;
		}
		if (_isTargetingEntity)
		{
			Handle_TargetedEntity();
		}
		else
		{
			Handle_FindTargetableEntities();
			Handle_TargetingInitControl();
		}
		if (!_targetedEntity && _isTargetingEntity)
		{
			Client_EndTarget();
		}
		void Apply_LockonReticle()
		{
			_lockonArrowTransform.gameObject.SetActive(value: false);
			_lockonReticleTransform.gameObject.SetActive(value: true);
			_lockonReticleTransform.localScale = Vector3.Lerp(_lockonReticleTransform.localScale, Vector3.one * 4.5f, 15.5f * Time.deltaTime);
			_lockonReticleTransform.SetParent(null);
			float num = 2.5f;
			if ((bool)_targetedEntity._isCreep)
			{
				num = _targetedEntity._isCreep._scriptCreep._generalCreepVisual._targeterOffset;
			}
			Vector3 position = _targetedEntity.transform.position;
			position.y = _targetedEntity.transform.position.y + num;
			_lockonReticleTransform.position = Vector3.Lerp(_lockonReticleTransform.position, position, 14.5f * Time.deltaTime);
		}
		void Check_StatusEntity(StatusEntity _singledEntity)
		{
			if ((bool)_singledEntity._isCreep && !_singledEntity._isCreep._isTargetable)
			{
				_foundEntity = null;
				TargetInfoDisplayManager._current._setStatusEntityTarget = null;
			}
			else if (_singledEntity._currentHealth > 0)
			{
				_foundEntity = _singledEntity;
				TargetInfoDisplayManager._current._setStatusEntityTarget = _foundEntity;
			}
			else
			{
				_foundEntity = null;
				TargetInfoDisplayManager._current._setStatusEntityTarget = null;
			}
		}
		void Handle_FindTargetableEntities()
		{
			_lockonReticleTransform.position = base.transform.position;
			_lockonReticleTransform.gameObject.SetActive(value: false);
			if (_player._inChat || _player._inUI || _player._bufferingStatus)
			{
				TargetInfoDisplayManager._current._setStatusEntityTarget = null;
				_lockonArrowTransform.gameObject.SetActive(value: false);
				_foundEntity = null;
			}
			else
			{
				PhysicsScene physicsScene = base.gameObject.scene.GetPhysicsScene();
				Vector2 vector = new Vector2(Screen.width / 2, Screen.height / 2);
				Ray ray = camFunc._mainCamera.ScreenPointToRay(vector);
				if ((bool)_foundEntity && _foundEntity._currentHealth > 0)
				{
					float num = 8.5f;
					Transform transform = _foundEntity.transform;
					if ((bool)_foundEntity._isCreep)
					{
						if (!_foundEntity._isCreep._isTargetable)
						{
							TargetInfoDisplayManager._current._setStatusEntityTarget = null;
							_lockonArrowTransform.gameObject.SetActive(value: false);
							_foundEntity = null;
							return;
						}
						num = _foundEntity._isCreep._scriptCreep._generalCreepVisual._lockonArrowOffset;
						transform = _foundEntity._isCreep._creepBehavior.transform;
					}
					_lockonArrowTransform.gameObject.SetActive(value: true);
					Vector3 position = transform.position;
					position.y = transform.position.y + num;
					_lockonArrowTransform.position = position;
				}
				else
				{
					_lockonArrowTransform.gameObject.SetActive(value: false);
				}
				physicsScene.Raycast(ray.origin, ray.direction, out var hitInfo, _targeterCastLength, CastLayers.TargetableMask);
				if ((bool)hitInfo.collider)
				{
					if ((bool)hitInfo.collider.GetComponentInParent<CreepBehavior>())
					{
						CreepBehavior componentInParent = hitInfo.collider.GetComponentInParent<CreepBehavior>();
						if ((bool)componentInParent && (bool)componentInParent._baseCreep)
						{
							StatusEntity statusEntity = componentInParent._baseCreep._statusEntity;
							Check_StatusEntity(statusEntity);
						}
					}
					else
					{
						_foundEntity = null;
						TargetInfoDisplayManager._current._setStatusEntityTarget = null;
					}
				}
				else
				{
					_foundEntity = null;
					TargetInfoDisplayManager._current._setStatusEntityTarget = null;
				}
			}
		}
		void Handle_TargetedEntity()
		{
			Creep _creepObj;
			Vector3 _targetPos;
			Vector3 _targetLock;
			if (!_targetedEntity)
			{
				Client_EndTarget();
			}
			else if (_targetedEntity.TryGetComponent<Creep>(out _creepObj) && _creepObj._currentCreepCondition != CreepCondition.ACTIVE)
			{
				Client_EndTarget();
			}
			else if (_playerStatus._currentHealth <= 0 || !_targetedEntity || _targetedEntity._currentHealth <= 0)
			{
				Client_EndTarget();
			}
			else if (_player._inUI && _isTargetingEntity)
			{
				Client_EndTarget();
			}
			else
			{
				_targetDistance = Vector3.Distance(base.transform.position, _targetedEntity.transform.position);
				TargetInfoDisplayManager._current._setStatusEntityTarget = _targetedEntity;
				float num = ((!(_targetDistance > 6.5f)) ? _clampCameraTargeting_low : 0f);
				_targetPos = new Vector3(_lockonReticleTransform.position.x, _lockonReticleTransform.position.y + num, _lockonReticleTransform.position.z);
				_targetLock = _targetedEntity.transform.position;
				_targetLock.y = base.transform.position.y;
				Player_LookAtTarget();
				Camera_LookAtTarget();
				Apply_LockonReticle();
				if (!tabTargetBuffer && EndTarget())
				{
					Client_EndTarget();
				}
			}
			void Camera_LookAtTarget()
			{
				Vector3 eulerAngles = Quaternion.LookRotation(_targetPos - camFunc._mainCamera.transform.position).eulerAngles;
				if (eulerAngles.x > 180f)
				{
					eulerAngles.x -= 360f;
				}
				eulerAngles.x = Mathf.Clamp(eulerAngles.x, _clampCameraTargeting_low, _clampCameraTargeting_high);
				camFunc.transform.rotation = Quaternion.Lerp(camFunc.transform.rotation, Quaternion.Euler(eulerAngles), _cameraTargetingSmooth * Time.deltaTime);
			}
			bool EndTarget()
			{
				if (Input.GetKeyDown(InputControlManager.current._target))
				{
					return true;
				}
				if (_targetDistance > _targeterCastLength)
				{
					return true;
				}
				if (Input.GetKeyDown(KeyCode.Escape))
				{
					return true;
				}
				if (GamepadInput.GetButtonDown(GamepadButton.RightStick))
				{
					return true;
				}
				if ((bool)_creepObj && !_creepObj._isTargetable)
				{
					return true;
				}
				return false;
			}
			void Player_LookAtTarget()
			{
				if (!_player._bufferingStatus && _player._currentPlayerCondition == PlayerCondition.ACTIVE && _pMove._movementLockType == MovementLockType.NONE && (!_pMove._lockControlMidair || !(_pMove._jumpForwardForce > 0f)) && !(_pMove._lockControlBuffer > 0f) && !(_pMove._attackForce > 0f))
				{
					base.transform.LookAt(_targetLock);
				}
			}
		}
		void Handle_TargetingInitControl()
		{
			if (!_player._inChat && !_player._inUI && (bool)_foundEntity && (Input.GetKeyDown(InputControlManager.current._target) || GamepadInput.GetButtonDown(GamepadButton.RightStick)))
			{
				Init_TargetAction();
			}
		}
		void Init_TargetAction()
		{
			if (!tabTargetBuffer)
			{
				_pSound._aSrcTargeting.PlayOneShot(_pSound._lockonSound, 0.65f);
				_targetedEntity = _foundEntity;
				_isTargetingEntity = true;
				Cmd_TargetSyncCreep(_targetedEntity.netIdentity);
				StartCoroutine(InputBuffer());
			}
		}
	}

	[ClientCallback]
	private void Client_EndTarget()
	{
		if (NetworkClient.active && base.isLocalPlayer && _isTargetingEntity)
		{
			_pSound._aSrcTargeting.PlayOneShot(_pSound._lockoutSound, 0.65f);
			Cmd_TargetSyncCreep(null);
			_isTargetingEntity = false;
			_targetedEntity = null;
			_foundEntity = null;
			_targetDistance = 0f;
			_lockonReticleTransform.SetParent(base.transform);
			_lockonReticleTransform.localPosition = Vector3.zero;
			_lockonReticleTransform.localScale = Vector3.one * 125.5f;
			_lockonReticleTransform.gameObject.SetActive(value: false);
			if (camFunc.transform.eulerAngles.x > 80f)
			{
				camFunc._RotX = base.transform.eulerAngles.x;
			}
			else
			{
				camFunc._RotX = camFunc.transform.eulerAngles.x;
			}
			camFunc._RotY = camFunc.transform.eulerAngles.y;
			camFunc.positionAdjust = GetComponent<PlayerVisual>()._headPos;
			StartCoroutine(InputBuffer());
			if (!_player._inUI && !_player._inChat)
			{
				camFunc._unlockedCamera = false;
			}
		}
	}

	private IEnumerator InputBuffer()
	{
		tabTargetBuffer = true;
		yield return new WaitForSeconds(0.25f);
		tabTargetBuffer = false;
	}

	public override bool Weaved()
	{
		return true;
	}

	protected void UserCode_Cmd_TargetSyncCreep__NetworkIdentity(NetworkIdentity nId)
	{
		Network_syncSelectedEntity = nId;
	}

	protected static void InvokeUserCode_Cmd_TargetSyncCreep__NetworkIdentity(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command Cmd_TargetSyncCreep called on client.");
		}
		else
		{
			((PlayerTargeting)obj).UserCode_Cmd_TargetSyncCreep__NetworkIdentity(reader.ReadNetworkIdentity());
		}
	}

	static PlayerTargeting()
	{
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerTargeting), "System.Void PlayerTargeting::Cmd_TargetSyncCreep(Mirror.NetworkIdentity)", InvokeUserCode_Cmd_TargetSyncCreep__NetworkIdentity, requiresAuthority: true);
	}

	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteNetworkIdentity(Network_syncSelectedEntity);
			return;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteNetworkIdentity(Network_syncSelectedEntity);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize_NetworkIdentity(ref _syncSelectedEntity, null, reader, ref ____syncSelectedEntityNetId);
			return;
		}
		long num = (long)reader.ReadULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize_NetworkIdentity(ref _syncSelectedEntity, null, reader, ref ____syncSelectedEntityNetId);
		}
	}
}
