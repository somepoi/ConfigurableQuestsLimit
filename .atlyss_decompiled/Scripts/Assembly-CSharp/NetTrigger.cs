using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

public class NetTrigger : NetworkBehaviour, IInteractable
{
	[CompilerGenerated]
	private sealed class _003CTriggerTimer_003Ed__43 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public NetTrigger _003C_003E4__this;

		public int _timer;

		private int _003C_currentTimer_003E5__2;

		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		[DebuggerHidden]
		public _003CTriggerTimer_003Ed__43(int _003C_003E1__state)
		{
			this._003C_003E1__state = _003C_003E1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
		}

		private bool MoveNext()
		{
			int num = _003C_003E1__state;
			NetTrigger netTrigger = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				if (netTrigger._timerBuffer)
				{
					return false;
				}
				_003C_currentTimer_003E5__2 = _timer;
				netTrigger._timerBuffer = true;
				goto IL_0046;
			case 1:
				_003C_003E1__state = -1;
				if (_003C_currentTimer_003E5__2 <= 0)
				{
					netTrigger._OffTriggerEvent.Invoke();
					netTrigger.Network_triggerEnabled = false;
					_003C_003E2__current = new WaitForSeconds(1.25f);
					_003C_003E1__state = 2;
					return true;
				}
				goto IL_0046;
			case 2:
				{
					_003C_003E1__state = -1;
					netTrigger.Reset_TimerBuffer();
					return false;
				}
				IL_0046:
				_003C_currentTimer_003E5__2--;
				_003C_003E2__current = new WaitForSeconds(1f);
				_003C_003E1__state = 1;
				return true;
			}
		}

		bool IEnumerator.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			return this.MoveNext();
		}

		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}
	}

	[Header("Trigger Events")]
	[SerializeField]
	private UnityEvent _OnTriggerEvent;

	[SerializeField]
	private UnityEvent _OffTriggerEvent;

	[Header("Trigger Runtime Conditions")]
	[SerializeField]
	private Collider _triggerCollider;

	[SerializeField]
	[Range(0.01f, 1f)]
	private float _chanceForTrigger = 1f;

	[SerializeField]
	private float _triggerIntroBuffer;

	[SyncVar(hook = "OnTriggerEnable")]
	public bool _triggerEnabled;

	[Space]
	public NetworkIdentity _lastNetID;

	public bool _wasInteractedWith;

	[Space]
	[SyncVar]
	public bool _requireInteractionKey;

	[SerializeField]
	private bool _oneTimeOnly;

	[SerializeField]
	private bool _playerOnlyTrigger = true;

	private float _currentTriggerIntroBuffer;

	private float _triggerBuffer;

	[Header("Trigger Deactive Conditions")]
	public GameObject _deactiveVisualContainer;

	public AudioSource _activateTriggerAsrc;

	public ParticleSystem _activateTriggerEffect;

	[SyncVar]
	public bool _deactivateTrigger;

	[Header("Message")]
	public TriggerMessage _triggerMessage;

	[Header("Trigger Timer")]
	[SerializeField]
	[Range(0f, 250f)]
	private int _resetTimer;

	[SerializeField]
	[Range(0f, 5f)]
	private float _timeBeforeEventInvoke;

	[SerializeField]
	private bool _resetInvokeOnExit;

	private bool _timerBuffer;

	[Header("Trigger Counter")]
	[SerializeField]
	private int _maxCounter;

	[SerializeField]
	private bool _resetCounterAtMax;

	private int _currentCounter;

	[Header("Dungeon Key Trigger")]
	public bool _requireDungeonKey;

	public int CurrentCounter
	{
		get
		{
			return _currentCounter;
		}
		set
		{
			if (value < 0)
			{
				value = 0;
			}
			if (value > _maxCounter)
			{
				value = _maxCounter;
			}
			if (value == _maxCounter)
			{
				if (!_triggerCollider)
				{
					Init_EnableTrigger();
				}
				if (_resetCounterAtMax)
				{
					Network_triggerEnabled = false;
					_triggerMessage._sentMsg = false;
					value = 0;
				}
			}
			_currentCounter = value;
		}
	}

	public bool Network_triggerEnabled
	{
		get
		{
			return _triggerEnabled;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _triggerEnabled, 1uL, OnTriggerEnable);
		}
	}

	public bool Network_requireInteractionKey
	{
		get
		{
			return _requireInteractionKey;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _requireInteractionKey, 2uL, null);
		}
	}

	public bool Network_deactivateTrigger
	{
		get
		{
			return _deactivateTrigger;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _deactivateTrigger, 4uL, null);
		}
	}

	protected override void OnValidate()
	{
		base.OnValidate();
		if ((bool)_deactiveVisualContainer)
		{
			_deactiveVisualContainer.SetActive(_deactivateTrigger);
		}
	}

	private void Awake()
	{
		Network_triggerEnabled = false;
		if (_requireInteractionKey)
		{
			base.gameObject.layer = LayerMask.NameToLayer("Interactable");
		}
		else
		{
			base.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
		}
		if ((bool)_triggerCollider)
		{
			_triggerCollider.isTrigger = true;
		}
		_currentTriggerIntroBuffer = _triggerIntroBuffer;
	}

	private void Update()
	{
		bool mute = (bool)Player._mainPlayer && (Player._mainPlayer._bufferingStatus || (Player._mainPlayer.isServer && Player._mainPlayer.gameObject.scene != base.gameObject.scene));
		if ((bool)_activateTriggerAsrc)
		{
			_activateTriggerAsrc.mute = mute;
		}
		if ((bool)_deactiveVisualContainer)
		{
			if (!_deactivateTrigger && _deactiveVisualContainer.activeInHierarchy)
			{
				_activateTriggerAsrc.Play();
				_activateTriggerEffect.Play();
				_deactiveVisualContainer.SetActive(value: false);
			}
			_deactiveVisualContainer.SetActive(_deactivateTrigger);
		}
		if ((bool)_triggerCollider)
		{
			_triggerCollider.enabled = !_deactivateTrigger && !_wasInteractedWith;
		}
		if (_triggerBuffer > 0f)
		{
			_triggerBuffer -= Time.deltaTime;
		}
		if (_currentTriggerIntroBuffer > 0f)
		{
			_currentTriggerIntroBuffer -= Time.deltaTime;
		}
	}

	[ServerCallback]
	public void Init_PickupItem(NetworkIdentity _netID)
	{
		if (NetworkServer.active)
		{
			Init_EnableTrigger();
		}
	}

	private void Init_OnTriggerEvent()
	{
		_OnTriggerEvent.Invoke();
	}

	private void Init_OffTriggerEvent()
	{
		_OffTriggerEvent.Invoke();
	}

	private void OnTriggerEnable(bool _oldBool, bool _newBool)
	{
		if (base.isServer)
		{
			return;
		}
		if (_newBool)
		{
			Invoke("Init_OnTriggerEvent", _timeBeforeEventInvoke);
			if (_oneTimeOnly)
			{
				_wasInteractedWith = true;
				if ((bool)_triggerCollider)
				{
					_triggerCollider.enabled = false;
				}
			}
		}
		else
		{
			if (_oneTimeOnly)
			{
				return;
			}
			Invoke("Init_OffTriggerEvent", _timeBeforeEventInvoke);
		}
		_triggerBuffer = 1.25f;
	}

	[ServerCallback]
	public void Init_EnableTrigger()
	{
		if (!NetworkServer.active || _deactivateTrigger || _triggerBuffer > 0f || _currentTriggerIntroBuffer > 0f || UnityEngine.Random.Range(0f, 1f) > _chanceForTrigger)
		{
			return;
		}
		Init_SendTriggerMessage();
		if (!_triggerEnabled)
		{
			Invoke("Init_OnTriggerEvent", _timeBeforeEventInvoke);
			Network_triggerEnabled = true;
			if (_oneTimeOnly)
			{
				_wasInteractedWith = true;
				if ((bool)_triggerCollider)
				{
					_triggerCollider.enabled = false;
				}
			}
			return;
		}
		if (_triggerEnabled)
		{
			if (_oneTimeOnly)
			{
				return;
			}
			Invoke("Init_OffTriggerEvent", _timeBeforeEventInvoke);
			Network_triggerEnabled = false;
		}
		_triggerBuffer = 1.25f;
	}

	[ServerCallback]
	public void Init_DisableTrigger()
	{
		if (NetworkServer.active && !_deactivateTrigger && !(_triggerBuffer > 0f) && !(_currentTriggerIntroBuffer > 0f) && !(UnityEngine.Random.Range(0f, 1f) > _chanceForTrigger))
		{
			Invoke("Init_OffTriggerEvent", _timeBeforeEventInvoke);
			Network_triggerEnabled = false;
			_triggerBuffer = 1.25f;
		}
	}

	public void Add_Counter()
	{
		if (!_deactivateTrigger)
		{
			CurrentCounter++;
		}
	}

	[ServerCallback]
	private void OnTriggerStay(Collider other)
	{
		if (!NetworkServer.active || _deactivateTrigger || _requireInteractionKey || _currentCounter != _maxCounter || _timerBuffer || _currentTriggerIntroBuffer > 0f)
		{
			return;
		}
		bool flag = ((!_playerOnlyTrigger) ? (other.gameObject.layer == LayerMask.NameToLayer("Player_TriggerCollider") || other.gameObject.layer == LayerMask.NameToLayer("CreepObject")) : (other.gameObject.layer == LayerMask.NameToLayer("Player_TriggerCollider")));
		if (!flag || (_oneTimeOnly && _wasInteractedWith))
		{
			return;
		}
		if (_requireDungeonKey)
		{
			Player componentInParent = other.gameObject.GetComponentInParent<Player>();
			ChatBehaviour chatBehaviour = componentInParent._chatBehaviour;
			if (componentInParent.Network_playerMapInstance._patternInstance._dungeonKeysAquired <= 0)
			{
				chatBehaviour.Target_RecieveMessage("The door is locked. It requires a Dungeon Key.");
				return;
			}
		}
		if (!_triggerEnabled)
		{
			_lastNetID = other.gameObject.GetComponentInParent<NetworkIdentity>();
			Invoke("Init_OnTriggerEvent", _timeBeforeEventInvoke);
			Init_SendTriggerMessage();
			Network_triggerEnabled = true;
			_wasInteractedWith = true;
		}
		if (_resetTimer > 0)
		{
			StartCoroutine(TriggerTimer(_resetTimer));
		}
	}

	[ServerCallback]
	private void OnTriggerExit(Collider other)
	{
		if (!NetworkServer.active || _deactivateTrigger || _requireInteractionKey || _currentCounter != _maxCounter || _timerBuffer || _currentTriggerIntroBuffer > 0f)
		{
			return;
		}
		bool flag = false;
		flag = ((!_playerOnlyTrigger) ? (other.gameObject.layer == LayerMask.NameToLayer("Player_TriggerCollider") || other.gameObject.layer == LayerMask.NameToLayer("CreepObject")) : (other.gameObject.layer == LayerMask.NameToLayer("Player_TriggerCollider")));
		if (flag && !_oneTimeOnly && _triggerEnabled)
		{
			if (_resetInvokeOnExit)
			{
				CancelInvoke("Init_OnTriggerEvent");
			}
			Invoke("Init_OffTriggerEvent", _timeBeforeEventInvoke);
			Network_triggerEnabled = false;
		}
	}

	[ServerCallback]
	private void Reset_TimerBuffer()
	{
		if (NetworkServer.active)
		{
			_timerBuffer = false;
		}
	}

	[ServerCallback]
	public void Disable_Trigger()
	{
		if (NetworkServer.active)
		{
			Network_triggerEnabled = false;
		}
	}

	[IteratorStateMachine(typeof(_003CTriggerTimer_003Ed__43))]
	[ServerCallback]
	private IEnumerator TriggerTimer(int _timer)
	{
		if (!NetworkServer.active)
		{
			return null;
		}
		return new _003CTriggerTimer_003Ed__43(0)
		{
			_003C_003E4__this = this,
			_timer = _timer
		};
	}

	[ServerCallback]
	private void Init_SendTriggerMessage()
	{
		if (!NetworkServer.active || (_oneTimeOnly && _wasInteractedWith) || _currentTriggerIntroBuffer > 0f)
		{
			return;
		}
		string text = _triggerMessage._singleMessage;
		if (_triggerMessage._triggerMessageArray.Length != 0)
		{
			text = _triggerMessage._triggerMessageArray[UnityEngine.Random.Range(0, _triggerMessage._triggerMessageArray.Length)];
		}
		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		ChatBehaviour[] array = UnityEngine.Object.FindObjectsOfType<ChatBehaviour>();
		foreach (ChatBehaviour chatBehaviour in array)
		{
			if (!(chatBehaviour == null) && !(chatBehaviour.gameObject.scene != base.gameObject.scene))
			{
				if (_triggerMessage._sentMsg)
				{
					break;
				}
				chatBehaviour.Target_RecieveTriggerMessage(text);
			}
		}
		_triggerMessage._sentMsg = true;
	}

	[ServerCallback]
	public void Clear_LastInteractionParams()
	{
		if (NetworkServer.active)
		{
			_lastNetID = null;
			_wasInteractedWith = false;
			Network_triggerEnabled = false;
		}
	}

	public override bool Weaved()
	{
		return true;
	}

	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteBool(_triggerEnabled);
			writer.WriteBool(_requireInteractionKey);
			writer.WriteBool(_deactivateTrigger);
			return;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteBool(_triggerEnabled);
		}
		if ((base.syncVarDirtyBits & 2L) != 0L)
		{
			writer.WriteBool(_requireInteractionKey);
		}
		if ((base.syncVarDirtyBits & 4L) != 0L)
		{
			writer.WriteBool(_deactivateTrigger);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize(ref _triggerEnabled, OnTriggerEnable, reader.ReadBool());
			GeneratedSyncVarDeserialize(ref _requireInteractionKey, null, reader.ReadBool());
			GeneratedSyncVarDeserialize(ref _deactivateTrigger, null, reader.ReadBool());
			return;
		}
		long num = (long)reader.ReadULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _triggerEnabled, OnTriggerEnable, reader.ReadBool());
		}
		if ((num & 2L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _requireInteractionKey, null, reader.ReadBool());
		}
		if ((num & 4L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _deactivateTrigger, null, reader.ReadBool());
		}
	}
}
