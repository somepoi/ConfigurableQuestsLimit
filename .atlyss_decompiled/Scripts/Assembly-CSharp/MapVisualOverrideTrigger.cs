using UnityEngine;

public class MapVisualOverrideTrigger : MonoBehaviour
{
	[SerializeField]
	private MapInstance _setMapInstance;

	public string _reigonName = "";

	[SerializeField]
	private ZoneType _reigonZoneType = ZoneType.Field;

	[Space]
	public bool _useOverrideVisuals = true;

	[SerializeField]
	private MapInstanceVisuals _overrideVisuals;

	private void OnTriggerStay(Collider other)
	{
		Creep component2;
		if (other.gameObject.transform.root.TryGetComponent<Player>(out var component))
		{
			if (component.Network_playerMapInstance != _setMapInstance)
			{
				return;
			}
			if (component.isServer)
			{
				component.Network_playerZoneType = _reigonZoneType;
			}
		}
		else if (other.gameObject.transform.root.TryGetComponent<Creep>(out component2))
		{
			if (!component2.isServer)
			{
				return;
			}
			if (_reigonZoneType == ZoneType.Safe)
			{
				component2.Reset_Creep();
			}
		}
		if ((bool)Player._mainPlayer && !(Player._mainPlayer.Network_playerMapInstance != _setMapInstance) && (!Player._mainPlayer._bufferingStatus || _setMapInstance._zoneType == ZoneType.Dungeon) && other.gameObject.transform.root.TryGetComponent<Player>(out var component3) && !(component3 != Player._mainPlayer) && !(other.gameObject == Player._mainPlayer.gameObject))
		{
			if (_useOverrideVisuals)
			{
				_setMapInstance._setOverrideVisuals = _overrideVisuals;
				_setMapInstance._useOverrideVisuals = true;
			}
			if (!string.IsNullOrWhiteSpace(_reigonName))
			{
				InGameUI._current.Init_MapTitleDisplay(_reigonName);
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.transform.root.TryGetComponent<Player>(out var component))
		{
			if (component.Network_playerMapInstance != _setMapInstance)
			{
				return;
			}
			if (component.isServer)
			{
				component.Network_playerZoneType = _setMapInstance._zoneType;
			}
		}
		if ((bool)Player._mainPlayer && !(Player._mainPlayer.Network_playerMapInstance != _setMapInstance) && (!Player._mainPlayer._bufferingStatus || _setMapInstance._zoneType == ZoneType.Dungeon) && other.gameObject.transform.root.TryGetComponent<Player>(out var component2) && !(component2 != Player._mainPlayer) && !(other.gameObject == Player._mainPlayer.gameObject))
		{
			if (_useOverrideVisuals)
			{
				_setMapInstance._setOverrideVisuals = new MapInstanceVisuals();
				_setMapInstance._useOverrideVisuals = false;
			}
			if (!string.IsNullOrWhiteSpace(_reigonName))
			{
				InGameUI._current.Init_MapTitleDisplay("");
			}
		}
	}
}
