using UnityEngine;

public class WorldPortalWaypoint : MonoBehaviour
{
	[Header("World Portal Waypoint")]
	public ScriptableMapData _scriptMapData;

	public bool _isAttuned;

	[Space]
	[SerializeField]
	private LerpScale _worldPortalLerpScale;

	[SerializeField]
	private DialogTrigger _worldPortalDialogTrigger;

	[Header("Visual")]
	[SerializeField]
	private GameObject _unattunedVisualObject;

	[SerializeField]
	private ParticleSystem _initAttuneEffect;

	[SerializeField]
	private ParticleSystem _attunedEffect;

	[SerializeField]
	private ParticleSystem[] _attunedTorches;

	[Space]
	[SerializeField]
	private AudioSource _attunedAsrc;

	[SerializeField]
	private AudioSource _unattunedAsrc;

	[SerializeField]
	private AudioSource _initAttunementAsrc;

	private void Start()
	{
		if (!Player._mainPlayer)
		{
			return;
		}
		if (!_scriptMapData)
		{
			base.enabled = false;
			return;
		}
		for (int i = 0; i < Player._mainPlayer._waypointAttunements.Count; i++)
		{
			if (!(Player._mainPlayer._waypointAttunements[i] != _scriptMapData._mapLockID))
			{
				_isAttuned = true;
				break;
			}
		}
		if (_isAttuned)
		{
			Init_AttuneEffect();
		}
	}

	private void Update()
	{
		_unattunedVisualObject.SetActive(!_isAttuned);
		if (_isAttuned)
		{
			_worldPortalLerpScale.setScale = Vector3.one * 5f;
		}
		else
		{
			_worldPortalLerpScale.setScale = Vector3.zero;
		}
		_worldPortalLerpScale.gameObject.SetActive(_isAttuned);
		_worldPortalDialogTrigger.enabled = _isAttuned;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!_isAttuned && (bool)Player._mainPlayer && !Player._mainPlayer._bufferingStatus && (bool)other.GetComponentInParent<Player>() && other.GetComponentInParent<Player>().isLocalPlayer)
		{
			_isAttuned = true;
			Player._mainPlayer._waypointAttunements.Add(_scriptMapData._mapLockID);
			ProfileDataManager._current.Save_ProfileData();
			Player._mainPlayer._chatBehaviour.Init_GameLogicMessage("Attuned World Portal Waypoint.");
			Player._mainPlayer._chatBehaviour.New_ChatMessage("<color=yellow>Attuned the " + _scriptMapData._mapCaptionTitle + " Waypoint!</color>");
			Init_AttuneEffect();
		}
	}

	private void Init_AttuneEffect()
	{
		ParticleSystem[] attunedTorches = _attunedTorches;
		foreach (ParticleSystem particleSystem in attunedTorches)
		{
			if ((bool)particleSystem)
			{
				particleSystem.Play();
			}
		}
		_initAttuneEffect.Play();
		_attunedEffect.Play();
		if (!Player._mainPlayer._bufferingStatus)
		{
			_initAttunementAsrc.Play();
		}
		_attunedAsrc.Play();
		_unattunedAsrc.Stop();
	}
}
