using System.Runtime.InteropServices;
using Mirror;
using UnityEngine;

public class ResourceEntity : NetworkBehaviour
{
	[Header("Resource Entity")]
	public string _resourceNodeTag = "New Node";

	public ScriptableProfession _scriptableProfession;

	public ScriptableLootTable _resourceEntityLoot;

	[Space]
	public GameObject _resourceVisualContainer;

	public Animator _resourceAnimator;

	[Space]
	public int _resourceEntityLevel;

	[Header("Effects / Visuals")]
	[SerializeField]
	private ParticleSystem _init_ActiveEffect;

	[SerializeField]
	private ParticleSystem _init_inactiveEffect;

	[Space]
	[SerializeField]
	private AudioSource _aSrc_init_active;

	[SerializeField]
	private AudioSource _aSrc_init_inactive;

	[Header("Runtime")]
	public int _currentResourceCount;

	[SerializeField]
	private float _currentRechargeTime;

	private float _setRechargeTime;

	[SyncVar(hook = "On_isActive")]
	public bool _isActive;

	public bool Network_isActive
	{
		get
		{
			return _isActive;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _isActive, 1uL, On_isActive);
		}
	}

	public void On_isActive(bool _old, bool _new)
	{
		if ((bool)Player._mainPlayer && (!Player._mainPlayer._isHostPlayer || !(Player._mainPlayer.gameObject.scene != base.gameObject.scene)))
		{
			if (_new)
			{
				_aSrc_init_active.Play();
				_init_ActiveEffect.Play();
			}
			else
			{
				_aSrc_init_inactive.Play();
				_init_inactiveEffect.Play();
			}
		}
	}

	public override void OnStartServer()
	{
		base.OnStartServer();
		_currentRechargeTime = 0f;
		_setRechargeTime = Random.Range(12.5f, 85.5f);
		_currentResourceCount = Random.Range(1, 4);
		Network_isActive = true;
	}

	private void Update()
	{
		_resourceAnimator.SetBool("_isActive", _isActive);
		Server_ResourceEntityParams();
	}

	[ServerCallback]
	private void Server_ResourceEntityParams()
	{
		if (!NetworkServer.active)
		{
			return;
		}
		if ((bool)_scriptableProfession && _scriptableProfession._professionName != "Fishing")
		{
			Network_isActive = _currentResourceCount > 0;
		}
		if (!_isActive)
		{
			_currentRechargeTime += Time.deltaTime;
			if (_currentRechargeTime >= _setRechargeTime)
			{
				_currentResourceCount = Random.Range(1, 7);
				_setRechargeTime = Random.Range(160.5f, 385.5f);
			}
		}
		else
		{
			_currentRechargeTime = 0f;
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if (other.gameObject.transform.root.TryGetComponent<Player>(out var component) && !(component != Player._mainPlayer) && component.isLocalPlayer)
		{
			component._pProfession._setResourceEntity = this;
			string tutorialTag = "tut_resource";
			string message = "This is a Profession Resource Node. You will need a tool consumable item to use this node, which can be purchased from vendors.";
			TutorialManager._current.Init_TutorialMessage(message, tutorialTag);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.transform.root.TryGetComponent<Player>(out var component) && !(component != Player._mainPlayer) && component.isLocalPlayer)
		{
			component._pProfession._setResourceEntity = null;
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
			writer.WriteBool(_isActive);
			return;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteBool(_isActive);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize(ref _isActive, On_isActive, reader.ReadBool());
			return;
		}
		long num = (long)reader.ReadULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _isActive, On_isActive, reader.ReadBool());
		}
	}
}
