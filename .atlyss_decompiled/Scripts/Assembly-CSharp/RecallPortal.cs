using Mirror;
using UnityEngine;

public class RecallPortal : MonoBehaviour, IInteractable
{
	[Header("Recall Portal")]
	public ScenePortalData _scenePortal;

	[SerializeField]
	private Collider _triggerCollider;

	[SerializeField]
	private ParticleSystem[] _portalParticleEffects = new ParticleSystem[2];

	[SerializeField]
	private AudioSource[] _portalSounds;

	public bool _isPortalOpen;

	private float _commandBuffer;

	private void Update()
	{
		Handle_RecallPortal();
	}

	private void Handle_RecallPortal()
	{
		if ((bool)Player._mainPlayer)
		{
			Player mainPlayer = Player._mainPlayer;
			if (_commandBuffer > 0f)
			{
				_commandBuffer -= Time.deltaTime;
			}
			else
			{
				_commandBuffer = 0f;
			}
			if (Player._mainPlayer._isHostPlayer && Player._mainPlayer.gameObject.scene != base.gameObject.scene)
			{
				_isPortalOpen = false;
				return;
			}
			_isPortalOpen = mainPlayer._recalledMapInstance != string.Empty;
			_triggerCollider.enabled = _isPortalOpen;
			_scenePortal._portalCaptionTitle = mainPlayer._recalledMapInstanceName ?? "";
			Handle_PortalDisplay();
		}
		void Handle_PortalDisplay()
		{
			if (!_isPortalOpen)
			{
				for (int i = 0; i < _portalParticleEffects.Length; i++)
				{
					_portalParticleEffects[i].Stop();
				}
				for (int j = 0; j < _portalSounds.Length; j++)
				{
					_portalSounds[j].Stop();
				}
			}
			else
			{
				for (int k = 0; k < _portalParticleEffects.Length; k++)
				{
					if (!_portalParticleEffects[k].isEmitting)
					{
						_portalParticleEffects[k].Play();
					}
				}
				for (int l = 0; l < _portalSounds.Length; l++)
				{
					if (!_portalSounds[l].isPlaying)
					{
						_portalSounds[l].Play();
					}
				}
			}
		}
	}

	public void Init_PickupItem(NetworkIdentity _netID)
	{
		if (_isPortalOpen)
		{
			Player component = _netID.GetComponent<Player>();
			component.Cmd_ReturnToRecalledInstance();
			component._recalledMapInstance = string.Empty;
			component._recalledMapInstanceName = string.Empty;
			_commandBuffer = 1f;
		}
	}
}
