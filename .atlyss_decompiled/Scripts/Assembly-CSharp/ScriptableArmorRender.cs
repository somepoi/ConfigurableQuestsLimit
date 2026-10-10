using UnityEngine;

[CreateAssetMenu(fileName = "armorRender_", menuName = "ScriptableObjects/Item/Equipment/RenderDisplayData", order = 0)]
public class ScriptableArmorRender : ScriptableObject
{
	[Header("Armor Render Display")]
	public RaceArmorRender[] _raceArmorRenders;

	public bool _useThighBlendshape;

	private void OnValidate()
	{
		for (int i = 0; i < _raceArmorRenders.Length; i++)
		{
			if ((bool)_raceArmorRenders[i]._scriptableRace)
			{
				_raceArmorRenders[i]._raceNameTag = _raceArmorRenders[i]._scriptableRace._raceName;
			}
		}
	}
}
