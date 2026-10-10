using Mirror;
using UnityEngine;

public class HC_InstanceListEntry : ListDataEntry
{
	[Header("Host Console Instance List Entry")]
	public NetworkIdentity _netId;

	public MapInstance _mapInstance;

	private PatternInstanceManager _patternInstance;

	public void Apply_MapInstanceData()
	{
		if ((bool)_netId)
		{
			if (!_mapInstance)
			{
				_mapInstance = _netId.GetComponent<MapInstance>();
			}
			_dataID = (int)_mapInstance.netId;
			if ((bool)_mapInstance._patternInstance)
			{
				_patternInstance = _mapInstance._patternInstance;
				_dataNameText.text = $"{_mapInstance._mapName} [Pattern: {_mapInstance._patternInstance._instancePatternID}]";
			}
			else
			{
				_dataNameText.text = _mapInstance._mapName ?? "";
			}
		}
	}
}
