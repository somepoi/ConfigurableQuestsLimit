using UnityEngine;

public class MapReigon : MonoBehaviour
{
	[Header("Map Instance Reigon")]
	public string _reigonTitle = "New Reigon";

	public Collider _reigon_collider;

	public bool _mainPlayerInRegion;

	[Space]
	public AudioClip _reigon_dayMusic;

	public AudioClip _reigon_nightMusic;

	private void OnValidate()
	{
		if ((bool)_reigon_collider)
		{
			_reigon_collider.isTrigger = true;
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if ((bool)other.GetComponentInParent<Player>())
		{
			Player componentInParent = other.GetComponentInParent<Player>();
			if (!componentInParent._bufferingStatus && !(componentInParent.gameObject.scene != base.gameObject.scene) && !(componentInParent != Player._mainPlayer))
			{
				_mainPlayerInRegion = true;
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if ((bool)other.GetComponentInParent<Player>() && !(other.GetComponentInParent<Player>() != Player._mainPlayer))
		{
			_mainPlayerInRegion = false;
		}
	}
}
