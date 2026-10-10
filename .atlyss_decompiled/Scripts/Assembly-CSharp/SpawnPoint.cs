using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
	[Header("Map Instance Spawn Point")]
	public string _spawnPointTag = "spawnPoint";

	[SerializeField]
	private MapInstance _mapInstance;

	private void OnValidate()
	{
		base.gameObject.name = "Spawnpoint (" + _spawnPointTag + ")";
	}

	private void OnEnable()
	{
		if ((bool)_mapInstance)
		{
			_mapInstance._instanceSpawnPoints.Add(this);
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawIcon(base.transform.position + Vector3.up * 1.5f, "_spawnPointIco", allowScaling: false);
		Gizmos.DrawRay(new Ray(base.transform.position, base.transform.forward));
	}
}
