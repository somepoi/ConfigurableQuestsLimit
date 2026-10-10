using System.Runtime.InteropServices;
using Mirror;
using UnityEngine;

public class PlayerSound : NetworkBehaviour
{
	private Player _player;

	private AudioSource[] _cachedAudioSources;

	[Header("General Audio")]
	public AudioSource _aSrcGeneral;

	public AudioSource _aSrcJump;

	public AudioSource _aSrcCombat;

	public AudioSource _aSrcDash;

	[Space]
	public AudioClip _teleport;

	public AudioClip _warp;

	public AudioClip _attributeGlyph;

	public AudioClip _equipSound;

	public AudioClip _unEquipSound;

	public AudioClip _purchaseItemSound;

	public AudioClip _pickupKeySound;

	public AudioClip[] _generalSounds;

	[Header("Footstep Audio")]
	public AudioSource _aSrcFootstep;

	public AudioClip _waterWalkStep;

	public AudioClip[] _footstepSounds;

	public AudioClip[] _landSounds;

	[Header("Water Enter/Exit Audio")]
	public AudioClip _waterEnter;

	public AudioClip _waterExit;

	[Header("Combat Audio")]
	public AudioClip _airAttackInitSound;

	public AudioClip _weaponQuickSwitchSfx;

	public AudioClip _knockbackSlapSfx;

	public AudioClip _lowHealthPulseSfx;

	public AudioClip _lowHealthWarning;

	public AudioClip _pvpFlagSound;

	public AudioClip _pvpOffSound;

	[Header("Targeting Audio")]
	public AudioSource _aSrcTargeting;

	public AudioClip _lockonSound;

	public AudioClip _lockoutSound;

	[Header("Character Voice Audio")]
	[SyncVar]
	public float _voicePitch = 1f;

	[Header("Lexicon Audio")]
	public AudioSource _aSrcLexicon;

	public AudioClip _lexiconBell;

	public AudioClip _lexiconOpen;

	public AudioClip _lexiconClose;

	[Header("Interaction Audio")]
	public AudioSource _aSrcItemPickup;

	public AudioSource _aSrcInteractBlip;

	public AudioClip _portalInteract;

	[Header("Fishing Audio")]
	public AudioClip _fishReelSfx;

	[Header("Quest Audio")]
	public AudioClip _questAcceptSfx;

	public AudioClip _questTurnInSfx;

	public AudioClip _questAbandonSfx;

	public AudioClip _questProgressTick;

	public AudioClip _questProgressComplete;

	[Header("Dynamic UI")]
	public AudioSource _aSrcPlayerUI;

	public float Network_voicePitch
	{
		get
		{
			return _voicePitch;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref _voicePitch, 1uL, null);
		}
	}

	private void Awake()
	{
		_player = GetComponent<Player>();
	}

	private void Start()
	{
		ReCache_AudioSources();
	}

	public void ReCache_AudioSources()
	{
		_cachedAudioSources = GetComponentsInChildren<AudioSource>(includeInactive: true);
	}

	private void Update()
	{
		Handle_AudioSourceCheck();
	}

	private void Handle_AudioSourceCheck()
	{
		if (!Player._mainPlayer || base.isLocalPlayer)
		{
			return;
		}
		bool flag = _player.Network_playerMapInstance == Player._mainPlayer.Network_playerMapInstance && !Player._mainPlayer._bufferingStatus;
		for (int i = 0; i < _cachedAudioSources.Length; i++)
		{
			if ((bool)_cachedAudioSources[i])
			{
				_cachedAudioSources[i].mute = !flag;
			}
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
			writer.WriteFloat(_voicePitch);
			return;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteFloat(_voicePitch);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize(ref _voicePitch, null, reader.ReadFloat());
			return;
		}
		long num = (long)reader.ReadULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref _voicePitch, null, reader.ReadFloat());
		}
	}
}
