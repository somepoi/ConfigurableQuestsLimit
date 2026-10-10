using UnityEngine;

namespace SuperSystems.ImageEffects;

[ExecuteInEditMode]
public class WireframeImageEffect : MonoBehaviour
{
	public enum WireframeType
	{
		None = 0,
		Solid = 1,
		ShadedUnlit = 2,
		Transparent = 3,
		TransparentCulled = 4,
		COUNT = 5
	}

	[Header("Replacement Shader")]
	public WireframeType wireframeType;

	public string replacementTag = "RenderType";

	public bool cameraBackgroundMatchesBaseColor = true;

	[Header("Wireframe Shader Properties")]
	[Range(0f, 800f)]
	public float wireThickness = 600f;

	[Range(0f, 20f)]
	public float wireSmoothness = 3f;

	public Color wireColor = Color.green;

	public Color baseColor = Color.black;

	public float maxTriSize = 25f;

	private Color initialClearColor;

	private CameraClearFlags initialClearFlag;

	private Camera cam;

	private WireframeType lastWireframeType;

	protected void OnEnable()
	{
		cam = GetComponent<Camera>();
		initialClearFlag = cam.clearFlags;
		initialClearColor = cam.backgroundColor;
	}

	protected void OnDisable()
	{
		ResetCamera();
		lastWireframeType = WireframeType.None;
	}

	protected void Update()
	{
		Shader.SetGlobalFloat("_WireThickness", wireThickness);
		Shader.SetGlobalFloat("_WireSmoothness", wireSmoothness);
		Shader.SetGlobalColor("_WireColor", wireColor);
		Shader.SetGlobalColor("_BaseColor", baseColor);
		Shader.SetGlobalFloat("_MaxTriSize", maxTriSize);
		if (wireframeType != WireframeType.None && cameraBackgroundMatchesBaseColor)
		{
			cam.backgroundColor = baseColor;
			cam.clearFlags = CameraClearFlags.Color;
		}
		ApplyShader();
	}

	private void ApplyShader()
	{
		if (wireframeType != lastWireframeType)
		{
			lastWireframeType = wireframeType;
			switch (wireframeType)
			{
			case WireframeType.Solid:
				cam.SetReplacementShader(Shader.Find("hidden/SuperSystems/Wireframe-Global"), replacementTag);
				break;
			case WireframeType.ShadedUnlit:
				cam.SetReplacementShader(Shader.Find("hidden/SuperSystems/Wireframe-Shaded-Unlit-Global"), replacementTag);
				break;
			case WireframeType.Transparent:
				cam.SetReplacementShader(Shader.Find("hidden/SuperSystems/Wireframe-Transparent-Global"), replacementTag);
				break;
			case WireframeType.TransparentCulled:
				cam.SetReplacementShader(Shader.Find("hidden/SuperSystems/Wireframe-Transparent-Culled-Global"), replacementTag);
				break;
			default:
				ResetCamera();
				break;
			}
		}
	}

	private void ResetCamera()
	{
		cam.ResetReplacementShader();
		cam.backgroundColor = initialClearColor;
		cam.clearFlags = initialClearFlag;
	}
}
