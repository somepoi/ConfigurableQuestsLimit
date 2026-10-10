using System.Collections.Generic;
using UnityEngine;

public class CreepSpawnerReigon : MonoBehaviour
{
	[Header("Creep Spawner Reigon")]
	public Collider _reigon_collider;

	public List<Player> _playersInReigon = new List<Player>();

	private void OnValidate()
	{
		if ((bool)_reigon_collider)
		{
			_reigon_collider.isTrigger = true;
		}
	}

	private void Update()
	{
		Iterate_PlayersInRadius();
	}

	private void Iterate_PlayersInRadius()
	{
		for (int i = 0; i < _playersInReigon.Count; i++)
		{
			if (_playersInReigon[i] == null)
			{
				_playersInReigon.RemoveAt(i);
				break;
			}
			if (_playersInReigon[i].gameObject.scene != base.gameObject.scene)
			{
				_playersInReigon.RemoveAt(i);
				break;
			}
			if (_playersInReigon[i]._bufferingStatus)
			{
				_playersInReigon.RemoveAt(i);
				break;
			}
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if ((bool)other.GetComponentInParent<Player>())
		{
			Player componentInParent = other.GetComponentInParent<Player>();
			if (!componentInParent._bufferingStatus && !(componentInParent.gameObject.scene != base.gameObject.scene) && !_playersInReigon.Contains(componentInParent))
			{
				_playersInReigon.Add(componentInParent);
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if ((bool)other.GetComponentInParent<Player>())
		{
			Player componentInParent = other.GetComponentInParent<Player>();
			if (_playersInReigon.Contains(componentInParent))
			{
				_playersInReigon.Remove(componentInParent);
			}
		}
	}
}
