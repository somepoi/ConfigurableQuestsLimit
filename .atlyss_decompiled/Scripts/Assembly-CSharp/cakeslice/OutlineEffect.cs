using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR;

namespace cakeslice;

[DisallowMultipleComponent]
[RequireComponent(typeof(Camera))]
public class OutlineEffect : MonoBehaviour
{
	private readonly LinkedSet<Outline> outlines = new LinkedSet<Outline>();

	[Range(1f, 6f)]
	public float lineThickness = 1.25f;

	[Range(0f, 10f)]
	public float lineIntensity = 0.5f;

	[Range(0f, 1f)]
	public float fillAmount = 0.2f;

	public Color lineColor0 = Color.red;

	public Color lineColor1 = Color.green;

	public Color lineColor2 = Color.blue;

	public bool additiveRendering;

	public bool backfaceCulling = true;

	public Texture2D fillTexture;

	public Vector2 fillTextureScale = Vector2.one;

	public bool useFillColor;

	public Color fillColor = Color.blue;

	[Header("These settings can affect performance!")]
	public bool cornerOutlines;

	public bool addLinesBetweenColors;

	[Header("Advanced settings")]
	public bool scaleWithScreenSize = true;

	[Range(0f, 1f)]
	public float alphaCutoff = 0.5f;

	public bool flipY;

	public Camera sourceCamera;

	public bool autoEnableOutlines;

	[HideInInspector]
	public Camera outlineCamera;

	private Material outline1Material;

	private Material outline2Material;

	private Material outline3Material;

	private Material outlineEraseMaterial;

	private Shader outlineShader;

	private Shader outlineBufferShader;

	[HideInInspector]
	public Material outlineShaderMaterial;

	[HideInInspector]
	public RenderTexture renderTexture;

	[HideInInspector]
	public RenderTexture extraRenderTexture;

	private CommandBuffer commandBuffer;

	private List<Material> materialBuffer = new List<Material>();

	private bool RenderTheNextFrame;

	public static OutlineEffect Instance { get; private set; }

	private Material GetMaterialFromID(int ID)
	{
		return ID switch
		{
			0 => outline1Material, 
			1 => outline2Material, 
			2 => outline3Material, 
			_ => outline1Material, 
		};
	}

	private Material CreateMaterial(Color emissionColor)
	{
		Material material = new Material(outlineBufferShader);
		material.SetColor("_Color", emissionColor);
		material.SetInt("_SrcBlend", 5);
		material.SetInt("_DstBlend", 10);
		material.SetInt("_ZWrite", 0);
		material.DisableKeyword("_ALPHATEST_ON");
		material.EnableKeyword("_ALPHABLEND_ON");
		material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
		material.renderQueue = 3000;
		return material;
	}

	private void Awake()
	{
		if (Instance != null)
		{
			UnityEngine.Object.Destroy(this);
			throw new Exception("you can only have one outline camera in the scene");
		}
		Instance = this;
	}

	private void Start()
	{
		CreateMaterialsIfNeeded();
		UpdateMaterialsPublicProperties();
		if (sourceCamera == null)
		{
			sourceCamera = GetComponent<Camera>();
			if (sourceCamera == null)
			{
				sourceCamera = Camera.main;
			}
		}
		if (outlineCamera == null)
		{
			Camera[] componentsInChildren = GetComponentsInChildren<Camera>();
			foreach (Camera camera in componentsInChildren)
			{
				if (camera.name == "Outline Camera")
				{
					outlineCamera = camera;
					camera.enabled = false;
					break;
				}
			}
			if (outlineCamera == null)
			{
				GameObject gameObject = new GameObject("Outline Camera");
				gameObject.transform.parent = sourceCamera.transform;
				outlineCamera = gameObject.AddComponent<Camera>();
				outlineCamera.enabled = false;
			}
		}
		if (renderTexture != null)
		{
			renderTexture.Release();
		}
		if (extraRenderTexture != null)
		{
			renderTexture.Release();
		}
		renderTexture = new RenderTexture(sourceCamera.pixelWidth, sourceCamera.pixelHeight, 16, RenderTextureFormat.Default);
		extraRenderTexture = new RenderTexture(sourceCamera.pixelWidth, sourceCamera.pixelHeight, 16, RenderTextureFormat.Default);
		UpdateOutlineCameraFromSource();
		commandBuffer = new CommandBuffer();
		outlineCamera.AddCommandBuffer(CameraEvent.BeforeImageEffects, commandBuffer);
	}

	public void OnPreRender()
	{
		if (commandBuffer == null)
		{
			return;
		}
		if (outlines.Count == 0)
		{
			if (!RenderTheNextFrame)
			{
				return;
			}
			RenderTheNextFrame = false;
		}
		else
		{
			RenderTheNextFrame = true;
		}
		CreateMaterialsIfNeeded();
		if (renderTexture == null || renderTexture.width != sourceCamera.pixelWidth || renderTexture.height != sourceCamera.pixelHeight)
		{
			if (renderTexture != null)
			{
				renderTexture.Release();
			}
			if (extraRenderTexture != null)
			{
				renderTexture.Release();
			}
			renderTexture = new RenderTexture(sourceCamera.pixelWidth, sourceCamera.pixelHeight, 16, RenderTextureFormat.Default);
			extraRenderTexture = new RenderTexture(sourceCamera.pixelWidth, sourceCamera.pixelHeight, 16, RenderTextureFormat.Default);
			outlineCamera.targetTexture = renderTexture;
		}
		UpdateMaterialsPublicProperties();
		UpdateOutlineCameraFromSource();
		outlineCamera.targetTexture = renderTexture;
		commandBuffer.SetRenderTarget(renderTexture);
		commandBuffer.Clear();
		foreach (Outline outline in outlines)
		{
			if (!outline.IsVisible)
			{
				continue;
			}
			LayerMask layerMask = sourceCamera.cullingMask;
			if (!(outline != null) || (int)layerMask != ((int)layerMask | (1 << outline.gameObject.layer)))
			{
				continue;
			}
			for (int i = 0; i < outline.SharedMaterials.Length; i++)
			{
				Material material = null;
				if (!(outline.SharedMaterials[i] != null) || !outline.SharedMaterials[i].HasProperty("_MainTex") || !(outline.SharedMaterials[i].mainTexture != null))
				{
					material = ((!outline.eraseRenderer) ? GetMaterialFromID(outline.color) : outlineEraseMaterial);
				}
				else
				{
					foreach (Material item in materialBuffer)
					{
						if (item.mainTexture == outline.SharedMaterials[i].mainTexture)
						{
							if (outline.eraseRenderer && item.color == outlineEraseMaterial.color)
							{
								material = item;
							}
							else if (!outline.eraseRenderer && item.color == GetMaterialFromID(outline.color).color)
							{
								material = item;
							}
						}
					}
					if (material == null)
					{
						material = ((!outline.eraseRenderer) ? new Material(GetMaterialFromID(outline.color)) : new Material(outlineEraseMaterial));
						material.mainTexture = outline.SharedMaterials[i].mainTexture;
						materialBuffer.Add(material);
					}
				}
				if (backfaceCulling)
				{
					material.SetInt("_Culling", 2);
				}
				else
				{
					material.SetInt("_Culling", 0);
				}
				MeshFilter meshFilter = outline.MeshFilter;
				SkinnedMeshRenderer skinnedMeshRenderer = outline.SkinnedMeshRenderer;
				SpriteRenderer spriteRenderer = outline.SpriteRenderer;
				if ((bool)meshFilter)
				{
					if (meshFilter.sharedMesh != null && i < meshFilter.sharedMesh.subMeshCount)
					{
						commandBuffer.DrawRenderer(outline.Renderer, material, i, 0);
					}
				}
				else if ((bool)skinnedMeshRenderer)
				{
					if (skinnedMeshRenderer.sharedMesh != null && i < skinnedMeshRenderer.sharedMesh.subMeshCount)
					{
						commandBuffer.DrawRenderer(outline.Renderer, material, i, 0);
					}
				}
				else if ((bool)spriteRenderer)
				{
					commandBuffer.DrawRenderer(outline.Renderer, material, i, 0);
				}
			}
		}
		outlineCamera.Render();
	}

	private void OnEnable()
	{
		Outline[] array = UnityEngine.Object.FindObjectsOfType<Outline>();
		Outline[] array2;
		if (autoEnableOutlines)
		{
			array2 = array;
			foreach (Outline obj in array2)
			{
				obj.enabled = false;
				obj.enabled = true;
			}
			return;
		}
		array2 = array;
		foreach (Outline outline in array2)
		{
			if (outline.enabled && !outlines.Contains(outline))
			{
				outlines.Add(outline);
			}
		}
	}

	private void OnDestroy()
	{
		if (renderTexture != null)
		{
			renderTexture.Release();
		}
		if (extraRenderTexture != null)
		{
			extraRenderTexture.Release();
		}
		DestroyMaterials();
	}

	[ImageEffectOpaque]
	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (outlineShaderMaterial != null)
		{
			outlineShaderMaterial.SetTexture("_OutlineSource", renderTexture);
			if (addLinesBetweenColors)
			{
				Graphics.Blit(source, extraRenderTexture, outlineShaderMaterial, 0);
				outlineShaderMaterial.SetTexture("_OutlineSource", extraRenderTexture);
			}
			Graphics.Blit(source, destination, outlineShaderMaterial, 1);
		}
	}

	private void CreateMaterialsIfNeeded()
	{
		if (outlineShader == null)
		{
			outlineShader = Resources.Load<Shader>("OutlineShader");
		}
		if (outlineBufferShader == null)
		{
			outlineBufferShader = Resources.Load<Shader>("OutlineBufferShader");
		}
		if (outlineShaderMaterial == null)
		{
			outlineShaderMaterial = new Material(outlineShader);
			if (fillTexture != null)
			{
				outlineShaderMaterial.SetTexture("_FillTexture", fillTexture);
			}
			outlineShaderMaterial.hideFlags = HideFlags.HideAndDontSave;
			UpdateMaterialsPublicProperties();
		}
		if (outlineEraseMaterial == null)
		{
			outlineEraseMaterial = CreateMaterial(new Color(0f, 0f, 0f, 0f));
		}
		if (outline1Material == null)
		{
			outline1Material = CreateMaterial(new Color(1f, 0f, 0f, 0f));
		}
		if (outline2Material == null)
		{
			outline2Material = CreateMaterial(new Color(0f, 1f, 0f, 0f));
		}
		if (outline3Material == null)
		{
			outline3Material = CreateMaterial(new Color(0f, 0f, 1f, 0f));
		}
	}

	private void DestroyMaterials()
	{
		foreach (Material item in materialBuffer)
		{
			UnityEngine.Object.DestroyImmediate(item);
		}
		materialBuffer.Clear();
		UnityEngine.Object.DestroyImmediate(outlineShaderMaterial);
		UnityEngine.Object.DestroyImmediate(outlineEraseMaterial);
		UnityEngine.Object.DestroyImmediate(outline1Material);
		UnityEngine.Object.DestroyImmediate(outline2Material);
		UnityEngine.Object.DestroyImmediate(outline3Material);
		outlineShader = null;
		outlineBufferShader = null;
		outlineShaderMaterial = null;
		outlineEraseMaterial = null;
		outline1Material = null;
		outline2Material = null;
		outline3Material = null;
	}

	public void UpdateMaterialsPublicProperties()
	{
		if (!outlineShaderMaterial)
		{
			return;
		}
		float num = 1f;
		if (scaleWithScreenSize)
		{
			num = (float)Screen.height / 360f;
		}
		if (scaleWithScreenSize && num < 1f)
		{
			if (XRSettings.isDeviceActive && sourceCamera.stereoTargetEye != StereoTargetEyeMask.None)
			{
				outlineShaderMaterial.SetFloat("_LineThicknessX", 0.001f * (1f / (float)XRSettings.eyeTextureWidth) * 1000f);
				outlineShaderMaterial.SetFloat("_LineThicknessY", 0.001f * (1f / (float)XRSettings.eyeTextureHeight) * 1000f);
			}
			else
			{
				outlineShaderMaterial.SetFloat("_LineThicknessX", 0.001f * (1f / (float)Screen.width) * 1000f);
				outlineShaderMaterial.SetFloat("_LineThicknessY", 0.001f * (1f / (float)Screen.height) * 1000f);
			}
		}
		else if (XRSettings.isDeviceActive && sourceCamera.stereoTargetEye != StereoTargetEyeMask.None)
		{
			outlineShaderMaterial.SetFloat("_LineThicknessX", num * (lineThickness / 1000f) * (1f / (float)XRSettings.eyeTextureWidth) * 1000f);
			outlineShaderMaterial.SetFloat("_LineThicknessY", num * (lineThickness / 1000f) * (1f / (float)XRSettings.eyeTextureHeight) * 1000f);
		}
		else
		{
			outlineShaderMaterial.SetFloat("_LineThicknessX", num * (lineThickness / 1000f) * (1f / (float)Screen.width) * 1000f);
			outlineShaderMaterial.SetFloat("_LineThicknessY", num * (lineThickness / 1000f) * (1f / (float)Screen.height) * 1000f);
		}
		outlineShaderMaterial.SetFloat("_LineIntensity", lineIntensity);
		outlineShaderMaterial.SetFloat("_FillAmount", fillAmount);
		outlineShaderMaterial.SetColor("_FillColor", fillColor);
		outlineShaderMaterial.SetFloat("_UseFillColor", useFillColor ? 1 : 0);
		outlineShaderMaterial.SetFloat("_UseFillTexture", (fillTexture != null) ? 1 : 0);
		outlineShaderMaterial.SetFloat("_FillTextureScaleX", fillTextureScale.x);
		outlineShaderMaterial.SetFloat("_FillTextureScaleY", fillTextureScale.y);
		outlineShaderMaterial.SetColor("_LineColor1", lineColor0 * lineColor0);
		outlineShaderMaterial.SetColor("_LineColor2", lineColor1 * lineColor1);
		outlineShaderMaterial.SetColor("_LineColor3", lineColor2 * lineColor2);
		if (flipY)
		{
			outlineShaderMaterial.SetInt("_FlipY", 1);
		}
		else
		{
			outlineShaderMaterial.SetInt("_FlipY", 0);
		}
		if (!additiveRendering)
		{
			outlineShaderMaterial.SetInt("_Dark", 1);
		}
		else
		{
			outlineShaderMaterial.SetInt("_Dark", 0);
		}
		if (cornerOutlines)
		{
			outlineShaderMaterial.SetInt("_CornerOutlines", 1);
		}
		else
		{
			outlineShaderMaterial.SetInt("_CornerOutlines", 0);
		}
		Shader.SetGlobalFloat("_OutlineAlphaCutoff", alphaCutoff);
	}

	private void UpdateOutlineCameraFromSource()
	{
		outlineCamera.CopyFrom(sourceCamera);
		outlineCamera.renderingPath = RenderingPath.Forward;
		outlineCamera.backgroundColor = new Color(0f, 0f, 0f, 0f);
		outlineCamera.clearFlags = CameraClearFlags.Color;
		outlineCamera.rect = new Rect(0f, 0f, 1f, 1f);
		outlineCamera.cullingMask = 0;
		outlineCamera.targetTexture = renderTexture;
		outlineCamera.enabled = false;
		outlineCamera.allowHDR = false;
	}

	public void AddOutline(Outline outline)
	{
		outlines.Add(outline);
	}

	public void RemoveOutline(Outline outline)
	{
		outlines.Remove(outline);
	}
}
