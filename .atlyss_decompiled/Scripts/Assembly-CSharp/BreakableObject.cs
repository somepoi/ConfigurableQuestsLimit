using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Mirror;
using Mirror.RemoteCalls;
using UnityEngine;

public class BreakableObject : NetworkBehaviour
{
	[CompilerGenerated]
	private sealed class _003CInit_RemoveBreakableObject_003Ed__11 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public BreakableObject _003C_003E4__this;

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
		public _003CInit_RemoveBreakableObject_003Ed__11(int _003C_003E1__state)
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
			BreakableObject breakableObject = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				_003C_003E2__current = new WaitForSeconds(5f);
				_003C_003E1__state = 1;
				return true;
			case 1:
				_003C_003E1__state = -1;
				NetworkServer.Destroy(breakableObject.gameObject);
				return false;
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

	public ItemDropEntity _itemDropEntity;

	[Header("Breakable Object")]
	[SerializeField]
	private ScriptableLootTable _scriptLootTable;

	[SerializeField]
	private Collider _breakableCollider;

	[SyncVar]
	public bool _isBroken;

	[Space]
	[SerializeField]
	private GameObject _objectVisualObject;

	[SerializeField]
	private ParticleSystem _breakEffect;

	private AudioSource _aSrc;

	public bool Network_isBroken
	{
		get
		{
			return _isBroken;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _isBroken, 1uL, null);
		}
	}

	private void Awake()
	{
		_itemDropEntity = GetComponent<ItemDropEntity>();
		_aSrc = GetComponent<AudioSource>();
	}

	private void Update()
	{
		_breakableCollider.enabled = !_isBroken;
		_objectVisualObject.SetActive(!_isBroken);
	}

	[ServerCallback]
	private void OnTriggerEnter(Collider other)
	{
		if (NetworkServer.active)
		{
			if (IsValidCollider() && !_isBroken && (!other.GetComponentInParent<CombatCollider>() || other.GetComponentInParent<CombatCollider>()._colliderType != CombatColliderType.SUPPORT))
			{
				Rpc_Break(other.gameObject.transform.forward);
				StartCoroutine(Init_RemoveBreakableObject());
				Network_isBroken = true;
			}
		}
		bool IsValidCollider()
		{
			if (other.gameObject.scene != base.gameObject.scene)
			{
				return false;
			}
			if ((bool)other.GetComponentInParent<CreepBehavior>())
			{
				return true;
			}
			if ((bool)other.GetComponentInParent<CombatCollider>())
			{
				return true;
			}
			return false;
		}
	}

	[ClientRpc]
	private void Rpc_Break(Vector3 _originPos)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteVector3(_originPos);
		SendRPCInternal("System.Void BreakableObject::Rpc_Break(UnityEngine.Vector3)", 416269415, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	[IteratorStateMachine(typeof(_003CInit_RemoveBreakableObject_003Ed__11))]
	[ServerCallback]
	private IEnumerator Init_RemoveBreakableObject()
	{
		if (!NetworkServer.active)
		{
			return null;
		}
		return new _003CInit_RemoveBreakableObject_003Ed__11(0)
		{
			_003C_003E4__this = this
		};
	}

	public override bool Weaved()
	{
		return true;
	}

	protected void UserCode_Rpc_Break__Vector3(Vector3 _originPos)
	{
		if (!Player._mainPlayer || Player._mainPlayer._bufferingStatus || (Player._mainPlayer._isHostPlayer && Player._mainPlayer.gameObject.scene != base.gameObject.scene))
		{
			return;
		}
		_breakEffect.transform.localRotation = Quaternion.LookRotation(_originPos);
		_breakEffect.Play();
		if ((bool)_aSrc)
		{
			_aSrc.pitch = UnityEngine.Random.Range(0.9825f, 1.095f);
			_aSrc.Play();
		}
		if ((bool)_itemDropEntity && (bool)_scriptLootTable)
		{
			_itemDropEntity._lootTableOverride = _scriptLootTable;
			_itemDropEntity._currencyDrop = _scriptLootTable._currencyAmount;
			if (UnityEngine.Random.Range(0, 2) == 1)
			{
				_itemDropEntity.Client_DropCurrency(1, 1f);
			}
			_itemDropEntity.Client_DropItems(Player._mainPlayer.netIdentity);
		}
	}

	protected static void InvokeUserCode_Rpc_Break__Vector3(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			UnityEngine.Debug.LogError("RPC Rpc_Break called on server.");
		}
		else
		{
			((BreakableObject)obj).UserCode_Rpc_Break__Vector3(reader.ReadVector3());
		}
	}

	static BreakableObject()
	{
		RemoteProcedureCalls.RegisterRpc(typeof(BreakableObject), "System.Void BreakableObject::Rpc_Break(UnityEngine.Vector3)", InvokeUserCode_Rpc_Break__Vector3);
	}

	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteBool(_isBroken);
			return;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteBool(_isBroken);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize(ref _isBroken, null, reader.ReadBool());
			return;
		}
		long num = (long)reader.ReadULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _isBroken, null, reader.ReadBool());
		}
	}
}
