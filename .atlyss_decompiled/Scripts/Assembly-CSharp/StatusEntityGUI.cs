using System.Collections.Generic;
using Mirror;
using Mirror.RemoteCalls;
using UnityEngine;
using UnityEngine.UI;

public class StatusEntityGUI : NetworkBehaviour
{
	private StatusEntity _statusEntity;

	private Player _isPlayer;

	private PlayerStats _pStats;

	private PlayerCombat _pCombat;

	private PlayerVisual _pVisual;

	private Creep _isCreep;

	[Header("Status Entity UI")]
	[SerializeField]
	private GameObject _healthBarObject;

	[SerializeField]
	private Image _healthBarFill;

	private float _healthFillAmount;

	[Space]
	[SerializeField]
	private GameObject _castBarObject;

	[SerializeField]
	private Image _castBarFill;

	private float _castFillAmount;

	[Space]
	[SerializeField]
	private CanvasGroup _stamWheelGroup;

	[SerializeField]
	private Image _stamWheelFill;

	[SerializeField]
	private Image _stamWheelGhostFill;

	[SerializeField]
	private Image _stamWheelBackdrop;

	private float _stamFillAmount;

	private float _stamGhostFillAmount;

	[Space]
	public List<DamageNumberSlot> _floatNumberSlots = new List<DamageNumberSlot>();

	private void Awake()
	{
		_statusEntity = GetComponent<StatusEntity>();
		_healthBarObject.SetActive(value: false);
	}

	private void Update()
	{
		if (!GameManager._current || !Player._mainPlayer)
		{
			return;
		}
		_isPlayer = _statusEntity._isPlayer;
		_isCreep = _statusEntity._isCreep;
		if ((bool)_isPlayer)
		{
			if (!_pStats)
			{
				_pStats = _isPlayer._pStats;
			}
			if (!_pCombat)
			{
				_pCombat = _isPlayer._pCombat;
			}
			if (!_pVisual)
			{
				_pVisual = _isPlayer._pVisual;
			}
		}
		Client_HealthbarDisplay();
		Client_StaminaWheelDisplay();
		Client_Iterate_FloatNumberSlots();
	}

	[ClientCallback]
	private void Client_Iterate_FloatNumberSlots()
	{
		if (!NetworkClient.active || _floatNumberSlots.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < _floatNumberSlots.Count; i++)
		{
			if (_floatNumberSlots[i] == null)
			{
				_floatNumberSlots.RemoveAt(i);
				break;
			}
			if (!_floatNumberSlots[i]._entityObject)
			{
				_floatNumberSlots.RemoveAt(i);
				break;
			}
			if (_floatNumberSlots[i]._floatValue._displayTimer <= 0f)
			{
				_floatNumberSlots.RemoveAt(i);
				break;
			}
		}
	}

	[ClientCallback]
	private void Client_HealthbarDisplay()
	{
		if (!NetworkClient.active)
		{
			return;
		}
		int _maxHealth;
		if ((bool)_healthBarObject)
		{
			bool flag = true;
			float num = Vector3.Distance(base.transform.position, Player._mainPlayer.transform.position);
			if ((bool)_isCreep && (bool)Player._mainPlayer && Player._mainPlayer._pTargeting._targetedEntity == _isCreep._statusEntity)
			{
				flag = false;
			}
			if (flag && num >= 80f)
			{
				_healthBarObject.SetActive(value: false);
				return;
			}
			_maxHealth = 0;
			Handle_PlayerHealthBar();
			Handle_PlayerCastBar();
			Handle_CreepHealthBar();
			Handle_CreepCastBar();
			_healthFillAmount = (float)_statusEntity._currentHealth / (float)_maxHealth;
			_healthBarFill.fillAmount = _healthFillAmount;
		}
		void Handle_CreepCastBar()
		{
			if ((bool)_isCreep)
			{
				if (_isCreep._creepBehavior.GetType() == typeof(CreepBehavior_StandardCreep))
				{
					CreepBehavior_StandardCreep creepBehavior_StandardCreep = (CreepBehavior_StandardCreep)_isCreep._creepBehavior;
					_castBarObject.SetActive(creepBehavior_StandardCreep._currentCastSkill);
					if ((bool)creepBehavior_StandardCreep._currentCastSkill)
					{
						_castFillAmount = creepBehavior_StandardCreep._currentCastSkill._skillRankParams._baseCastTime + creepBehavior_StandardCreep._setCastTimeModifier;
						_castBarFill.fillAmount = creepBehavior_StandardCreep._castTime / _castFillAmount;
					}
				}
				else
				{
					_castBarObject.SetActive(value: false);
				}
			}
		}
		void Handle_CreepHealthBar()
		{
			if ((bool)_isCreep && (bool)_isCreep._scriptCreep && (bool)Player._mainPlayer)
			{
				if ((bool)_isCreep._parentCreepSpawner && _isCreep._parentCreepSpawner._mapInstance != Player._mainPlayer.Network_playerMapInstance)
				{
					_healthBarObject.SetActive(value: false);
				}
				else
				{
					GeneralCreepVisual generalCreepVisual = _isCreep._scriptCreep._generalCreepVisual;
					_maxHealth = _isCreep._statStruct._maxHealth;
					if (!generalCreepVisual._showHealthBar || _statusEntity._currentHealth <= 0 || !_isCreep._isTargetable)
					{
						_healthBarObject.SetActive(value: false);
					}
					else
					{
						Transform transform = _healthBarObject.transform;
						if ((bool)transform.parent)
						{
							transform.SetParent(null);
						}
						Vector3 vector = new Vector3(_isCreep._creepBehavior.transform.position.x, _isCreep._creepBehavior.transform.position.y + generalCreepVisual._healthBarYPosition, _isCreep._creepBehavior.transform.position.z);
						if (transform.position != vector)
						{
							transform.position = vector;
						}
						if (transform.localScale != generalCreepVisual._healthBarSize)
						{
							transform.localScale = generalCreepVisual._healthBarSize;
						}
						if (_statusEntity._currentHealth >= _maxHealth || _statusEntity._currentHealth <= 0)
						{
							_healthBarObject.SetActive(value: false);
						}
						else
						{
							_healthBarObject.SetActive(value: true);
						}
					}
				}
			}
		}
		void Handle_PlayerCastBar()
		{
			if ((bool)_isPlayer)
			{
				if (base.isLocalPlayer || ((bool)Player._mainPlayer && Player._mainPlayer.Network_playerMapInstance != _isPlayer.Network_playerMapInstance) || _isPlayer._pVisual._forceHidden)
				{
					_castBarObject.SetActive(value: false);
				}
				else
				{
					_castBarObject.SetActive(_isPlayer._pCasting._currentCastSkill);
					if ((bool)_isPlayer._pCasting._currentCastSkill)
					{
						_castFillAmount = _isPlayer._pCasting._currentCastSkill._skillRankParams._baseCastTime;
						_castBarFill.fillAmount = _isPlayer._pCasting._castTimer / _castFillAmount;
					}
				}
			}
		}
		void Handle_PlayerHealthBar()
		{
			if ((bool)_isPlayer)
			{
				if (base.isLocalPlayer || ((bool)Player._mainPlayer && Player._mainPlayer.Network_playerMapInstance != _isPlayer.Network_playerMapInstance) || _isPlayer._pVisual._forceHidden)
				{
					_healthBarObject.SetActive(value: false);
				}
				else
				{
					_maxHealth = _pStats._statStruct._maxHealth;
					if (_statusEntity._currentHealth >= _maxHealth || _statusEntity._currentHealth <= 0)
					{
						_healthBarObject.SetActive(value: false);
					}
					else
					{
						_healthBarObject.SetActive(value: true);
					}
				}
			}
		}
	}

	[ClientCallback]
	private void Client_StaminaWheelDisplay()
	{
		if (!NetworkClient.active || !_isPlayer || !_stamWheelGroup)
		{
			return;
		}
		if (!base.isLocalPlayer)
		{
			_stamWheelGroup.alpha = 0f;
			return;
		}
		if (base.gameObject.scene != Player._mainPlayer.gameObject.scene || !base.isLocalPlayer)
		{
			_stamWheelGroup.alpha = 0f;
			return;
		}
		if (_isPlayer._currentPlayerCondition != PlayerCondition.ACTIVE)
		{
			_stamWheelGroup.alpha = 0f;
			return;
		}
		int maxStamina = _pStats._statStruct._maxStamina;
		if (_statusEntity._regenStamBuffer < 0f)
		{
			_stamWheelBackdrop.color = Color.Lerp(_stamWheelBackdrop.color, Color.red, 6.5f * Time.deltaTime);
		}
		else
		{
			_stamWheelBackdrop.color = Color.Lerp(_stamWheelBackdrop.color, Color.grey, 6.5f * Time.deltaTime);
		}
		if (_pCombat._isBlocking)
		{
			_stamWheelGroup.alpha = Mathf.Lerp(_stamWheelGroup.alpha, 1f, 8.5f * Time.deltaTime);
		}
		else if (_statusEntity._currentStamina >= maxStamina)
		{
			_stamWheelGroup.alpha = Mathf.Lerp(_stamWheelGroup.alpha, 0f, 8.5f * Time.deltaTime);
			_stamFillAmount = 1f;
			_stamGhostFillAmount = 1f;
		}
		else
		{
			_stamWheelGroup.alpha = Mathf.Lerp(_stamWheelGroup.alpha, 1f, 8.5f * Time.deltaTime);
		}
		_stamFillAmount += ((float)_statusEntity._currentStamina / (float)maxStamina - _stamFillAmount) * 0.2f;
		if (float.IsNaN(_stamFillAmount))
		{
			_stamFillAmount = 0f;
		}
		_stamWheelFill.fillAmount = _stamFillAmount;
		_stamGhostFillAmount += ((float)_statusEntity._currentStamina / (float)maxStamina - _stamGhostFillAmount) * 0.045f;
		if (float.IsNaN(_stamGhostFillAmount))
		{
			_stamGhostFillAmount = 0f;
		}
		_stamWheelGhostFill.fillAmount = _stamGhostFillAmount;
	}

	[TargetRpc]
	public void Target_Display_FloatingNumber(StatusEntity _followObj, FloatTextColor _floatTextColor, int _value, int _combatElementID, int _conditionGroupID)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteNetworkBehaviour(_followObj);
		GeneratedNetworkCode._Write_FloatTextColor(writer, _floatTextColor);
		writer.WriteInt(_value);
		writer.WriteInt(_combatElementID);
		writer.WriteInt(_conditionGroupID);
		SendTargetRPCInternal(null, "System.Void StatusEntityGUI::Target_Display_FloatingNumber(StatusEntity,FloatTextColor,System.Int32,System.Int32,System.Int32)", 2012522415, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void OnDestroy()
	{
		Object.Destroy(_healthBarObject);
	}

	public override bool Weaved()
	{
		return true;
	}

	protected void UserCode_Target_Display_FloatingNumber__StatusEntity__FloatTextColor__Int32__Int32__Int32(StatusEntity _followObj, FloatTextColor _floatTextColor, int _value, int _combatElementID, int _conditionGroupID)
	{
		if (!Player._mainPlayer || Player._mainPlayer._bufferingStatus || _followObj == null || _followObj.gameObject.scene != Player._mainPlayer.gameObject.scene)
		{
			return;
		}
		DamageNumberSlot damageNumberSlot = new DamageNumberSlot
		{
			_entityObject = _followObj,
			_floatValue = GameManager._current.Spawn_FloatingText(_value, _followObj, _floatTextColor, 9f, 65.5f)
		};
		if (_conditionGroupID > 0)
		{
			ScriptableConditionGroup scriptableConditionGroup = GameManager._current.Locate_ConditionGroup(_conditionGroupID);
			if ((bool)scriptableConditionGroup)
			{
				damageNumberSlot._floatValue._conditionGroup = scriptableConditionGroup;
			}
			else
			{
				damageNumberSlot._floatValue._conditionGroup = null;
			}
		}
		else if ((bool)damageNumberSlot._floatValue)
		{
			damageNumberSlot._floatValue._conditionGroup = null;
		}
		if (_combatElementID > 0)
		{
			ScriptableCombatElement scriptableCombatElement = GameManager._current.Locate_CombatElement(_combatElementID);
			if ((bool)scriptableCombatElement)
			{
				damageNumberSlot._floatValue._scriptableCombatElement = scriptableCombatElement;
			}
			else
			{
				damageNumberSlot._floatValue._scriptableCombatElement = null;
			}
		}
		else if ((bool)damageNumberSlot._floatValue)
		{
			damageNumberSlot._floatValue._scriptableCombatElement = null;
		}
		_floatNumberSlots.Add(damageNumberSlot);
	}

	protected static void InvokeUserCode_Target_Display_FloatingNumber__StatusEntity__FloatTextColor__Int32__Int32__Int32(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC Target_Display_FloatingNumber called on server.");
		}
		else
		{
			((StatusEntityGUI)obj).UserCode_Target_Display_FloatingNumber__StatusEntity__FloatTextColor__Int32__Int32__Int32(reader.ReadNetworkBehaviour<StatusEntity>(), GeneratedNetworkCode._Read_FloatTextColor(reader), reader.ReadInt(), reader.ReadInt(), reader.ReadInt());
		}
	}

	static StatusEntityGUI()
	{
		RemoteProcedureCalls.RegisterRpc(typeof(StatusEntityGUI), "System.Void StatusEntityGUI::Target_Display_FloatingNumber(StatusEntity,FloatTextColor,System.Int32,System.Int32,System.Int32)", InvokeUserCode_Target_Display_FloatingNumber__StatusEntity__FloatTextColor__Int32__Int32__Int32);
	}
}
