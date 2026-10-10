using UnityEngine;

public class VisualManager : MonoBehaviour
{
	public void Apply_FilteringMode(FilterMode _filterMode)
	{
		Texture[] array = Resources.LoadAll<Texture>("_GRAPHIC/");
		foreach (Texture obj in array)
		{
			obj.anisoLevel = 16;
			obj.filterMode = _filterMode;
		}
	}
}
