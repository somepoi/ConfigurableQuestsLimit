using Mirror;
using UnityEngine;

public class WorldPortal : NetworkBehaviour
{
	[Header("World Portal Entity")]
	public ScriptableMapData[] _worldPortalSlots;

	private AudioSource[] _cachedAudioSources;

	[SerializeField]
	private Collider _worldPortalCollider;

	public int _selectedPortalEntry;

	public int SelectedPortalEntry
	{
		get
		{
			return _selectedPortalEntry;
		}
		set
		{
			if (value < 0)
			{
				value = _worldPortalSlots.Length - 1;
			}
			if (value > _worldPortalSlots.Length - 1)
			{
				value = 0;
			}
			_selectedPortalEntry = value;
		}
	}

	[ContextMenu("Allocate World Portal Data")]
	public void Allocate_PortalData()
	{
		ScriptableMapData[] worldPortalSlots = Resources.LoadAll<ScriptableMapData>("_PREFAB/_ENTITY/_PORTAL/_WORLDPORTAL/");
		_worldPortalSlots = worldPortalSlots;
	}

	private void Awake()
	{
		_cachedAudioSources = GetComponentsInChildren<AudioSource>();
	}

	private void Update()
	{
		_worldPortalCollider.enabled = Player._mainPlayer._waypointAttunements.Count > 0;
		if ((bool)Player._mainPlayer && Player._mainPlayer._isHostPlayer)
		{
			for (int i = 0; i < _cachedAudioSources.Length; i++)
			{
				_cachedAudioSources[i].mute = Player._mainPlayer.gameObject.scene != base.gameObject.scene;
			}
		}
	}

	public override bool Weaved()
	{
		return true;
	}
}
