using UnityEngine;

public class GroundType : MonoBehaviour
{
	public GroundMaterial groundMaterial;

	public bool _canLedgeClimb;

	public bool _noForwardForceOnLedgeJump;

	[ContextMenu("Apply To Environment Object")]
	public void Editor_ApplyToEnvObj()
	{
		MapInstance mapInstance = Object.FindObjectOfType<MapInstance>();
		base.transform.SetParent(mapInstance._environmentGroup.transform);
		if (groundMaterial == GroundMaterial.Water)
		{
			base.gameObject.layer = LayerMask.NameToLayer("Water");
		}
		else
		{
			base.gameObject.layer = LayerMask.NameToLayer("Ground");
		}
	}

	private void Awake()
	{
		if (groundMaterial == GroundMaterial.Water)
		{
			base.gameObject.layer = LayerMask.NameToLayer("Water");
		}
		else
		{
			base.gameObject.layer = LayerMask.NameToLayer("Ground");
		}
	}
}
