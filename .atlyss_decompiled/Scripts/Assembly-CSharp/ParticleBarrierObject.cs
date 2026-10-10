using UnityEngine;

public class ParticleBarrierObject : MonoBehaviour
{
	[Header("Particle Barrier Object")]
	[SerializeField]
	private ParticleSystem _barrierParticleSystem;

	[SerializeField]
	private Collider _barrierCollider;

	[SerializeField]
	private Teleporter _barrierTeleporter;

	[Space]
	[SerializeField]
	private CreepArena _setCreepArena;

	[SerializeField]
	private CreepSpawner _setCreepSpawner;

	private void Update()
	{
		if (!_setCreepArena && !_setCreepSpawner)
		{
			Debug.LogError("No Creep Arena or Spawner assigned to this barrier. (" + base.gameObject.name + ").");
			return;
		}
		bool flag = ((bool)_setCreepArena && _setCreepArena._arenaEnabled) || ((bool)_setCreepSpawner && !_setCreepSpawner._disableSpawner);
		if (flag && !_barrierParticleSystem.isEmitting)
		{
			_barrierParticleSystem.Play();
		}
		else if (!flag)
		{
			_barrierParticleSystem.Stop();
		}
		_barrierCollider.enabled = flag;
		if (flag && !_barrierTeleporter._isEnabled)
		{
			_barrierTeleporter.Enable_Teleporter();
		}
		else if (!flag)
		{
			_barrierTeleporter._isEnabled = false;
		}
	}
}
