using UnityEngine;

namespace cakeslice;

[RequireComponent(typeof(Renderer))]
public class Outline : MonoBehaviour
{
	public int color;

	public bool eraseRenderer;

	private bool visible;

	private Material[] _SharedMaterials;

	public Renderer Renderer { get; private set; }

	public SpriteRenderer SpriteRenderer { get; private set; }

	public SkinnedMeshRenderer SkinnedMeshRenderer { get; private set; }

	public MeshFilter MeshFilter { get; private set; }

	public bool IsVisible => visible;

	public Material[] SharedMaterials
	{
		get
		{
			if (_SharedMaterials == null)
			{
				_SharedMaterials = Renderer.sharedMaterials;
			}
			return _SharedMaterials;
		}
	}

	private void Awake()
	{
		Renderer = GetComponent<Renderer>();
		SkinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
		SpriteRenderer = GetComponent<SpriteRenderer>();
		MeshFilter = GetComponent<MeshFilter>();
	}

	private void OnEnable()
	{
		OutlineEffect.Instance?.AddOutline(this);
	}

	private void OnDisable()
	{
		OutlineEffect.Instance?.RemoveOutline(this);
	}

	private void OnBecameVisible()
	{
		visible = true;
	}

	private void OnBecameInvisible()
	{
		visible = false;
	}
}
