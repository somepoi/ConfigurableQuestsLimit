using System.Runtime.InteropServices;
using Mirror;
using UnityEngine;

public class ItemDropEntity_Chest : ItemDropEntity
{
	[Header("Chest Visuals / Colliders")]
	[SerializeField]
	private Animator _animator;

	[SerializeField]
	private AudioSource _chestOpenASrc;

	[SerializeField]
	private AudioSource _chestAppearASrc;

	[SerializeField]
	private ParticleSystem _chestOpenEffect;

	[SerializeField]
	private ParticleSystem _chestAppearEffect;

	[SerializeField]
	private Collider _interactableCollider;

	[SerializeField]
	private Collider _chestPhysicalCollider;

	[SerializeField]
	private LerpScale _visualScaler;

	[Header("Chest Parameters")]
	[SerializeField]
	[Range(0f, 99.9f)]
	private float _chestResetTimer;

	[SyncVar(hook = "On_ChestHidden")]
	public bool _isChestHidden = true;

	[SyncVar(hook = "On_ChestOpen")]
	public bool _isChestOpen;

	private float _currentResetTimer;

	private float _chestOpenToResetTimer = 2.5f;

	public bool Network_isChestHidden
	{
		get
		{
			return _isChestHidden;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _isChestHidden, 1uL, On_ChestHidden);
		}
	}

	public bool Network_isChestOpen
	{
		get
		{
			return _isChestOpen;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _isChestOpen, 2uL, On_ChestOpen);
		}
	}

	private void On_ChestHidden(bool _oldBool, bool _newBool)
	{
		if ((!Player._mainPlayer._isHostPlayer || !(Player._mainPlayer.gameObject.scene != base.gameObject.scene)) && !_newBool)
		{
			_chestAppearEffect.Play();
			_chestAppearASrc.Play();
		}
	}

	private void On_ChestOpen(bool _oldBool, bool _newBool)
	{
		if (!Player._mainPlayer._isHostPlayer || !(Player._mainPlayer.gameObject.scene != base.gameObject.scene))
		{
			if (_newBool)
			{
				_animator.CrossFade("Chest_Open", 0f, 0);
				_chestOpenEffect.Play();
				_chestOpenASrc.Play();
			}
			else
			{
				_animator.CrossFade("Chest_Closed", 0.2f, 0);
			}
		}
	}

	protected override void OnValidate()
	{
		base.OnValidate();
		if (_chestResetTimer < 0f)
		{
			_chestResetTimer = 0f;
		}
	}

	[ServerCallback]
	public void Appear_Chest()
	{
		if (NetworkServer.active)
		{
			Network_isChestHidden = false;
		}
	}

	private void Update()
	{
		_interactableCollider.enabled = !_isChestHidden && !_isChestOpen;
		_chestPhysicalCollider.enabled = !_isChestHidden && !_isChestOpen;
		if (_isChestHidden)
		{
			_visualScaler.setScale = new Vector3(0f, 2.5f, 0f);
		}
		else
		{
			_visualScaler.setScale = Vector3.one;
			_currentResetTimer = 0f;
		}
		Server_HandleChestParams();
	}

	[ServerCallback]
	private void Server_HandleChestParams()
	{
		if (!NetworkServer.active)
		{
			return;
		}
		if (_isChestHidden)
		{
			if (_chestResetTimer > 0f && _currentResetTimer != _chestResetTimer)
			{
				_currentResetTimer += Time.deltaTime;
			}
			if (_chestResetTimer > 0f && _currentResetTimer >= _chestResetTimer)
			{
				Network_isChestHidden = false;
				Network_isChestOpen = false;
			}
		}
		else if (_isChestOpen)
		{
			if (_chestOpenToResetTimer > 0f)
			{
				_chestOpenToResetTimer -= Time.deltaTime;
				return;
			}
			_chestOpenToResetTimer = 2.5f;
			Network_isChestHidden = true;
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
			writer.WriteBool(_isChestHidden);
			writer.WriteBool(_isChestOpen);
			return;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteBool(_isChestHidden);
		}
		if ((base.syncVarDirtyBits & 2L) != 0L)
		{
			writer.WriteBool(_isChestOpen);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize(ref _isChestHidden, On_ChestHidden, reader.ReadBool());
			GeneratedSyncVarDeserialize(ref _isChestOpen, On_ChestOpen, reader.ReadBool());
			return;
		}
		long num = (long)reader.ReadULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _isChestHidden, On_ChestHidden, reader.ReadBool());
		}
		if ((num & 2L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _isChestOpen, On_ChestOpen, reader.ReadBool());
		}
	}
}
