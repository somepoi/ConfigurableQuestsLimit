using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderModify : MonoBehaviour
{
	public GameObject _objModel;

	[Header("Render Meshes")]
	public SkinnedMeshRenderer[] _skinnedObjRenders = new SkinnedMeshRenderer[0];

	public MeshRenderer[] _objRenders = new MeshRenderer[0];

	public List<MeshRenderer> _dynamicObjRenders;

	public bool _findRendersOnStart;

	[Header("Scaling")]
	public float _setVisualScale = 1f;

	public bool _RefreshScale;

	public float _scaleBuffer;

	[Header("Shader")]
	public bool _RefreshShader;

	[Header("Tint Conditions")]
	public float _smoothTintTime = 2.5f;

	public float _shineSmoothTime = 4.6f;

	private static readonly int _HUE = Shader.PropertyToID("_Hue");

	private static readonly int _SATURATION = Shader.PropertyToID("_Saturation");

	private static readonly int _BRIGHTNESS = Shader.PropertyToID("_Brightness");

	private static readonly int _CONTRAST = Shader.PropertyToID("_Contrast");

	private static readonly int _TINT = Shader.PropertyToID("_ColorTint");

	private static readonly int _SHINE = Shader.PropertyToID("_ShineValue");

	public Color _defaultColor = Color.white;

	private void Start()
	{
		if (_findRendersOnStart)
		{
			FindRenders();
		}
	}

	private void Update()
	{
		Handle_RefreshShader();
		Handle_RefreshScale();
	}

	public void FindRenders()
	{
		_skinnedObjRenders = base.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>(includeInactive: true);
		_objRenders = base.gameObject.GetComponentsInChildren<MeshRenderer>(includeInactive: true);
	}

	public void Init_Time_RefreshShader(float _time)
	{
		StopAllCoroutines();
		StartCoroutine(Time_RefreshShader(_time));
	}

	private IEnumerator Time_RefreshShader(float _time)
	{
		_RefreshShader = true;
		yield return new WaitForSeconds(_time);
		_RefreshShader = false;
	}

	private void Handle_RefreshScale()
	{
		_RefreshScale = _scaleBuffer > 0f;
		if (_RefreshScale && (bool)_objModel)
		{
			_objModel.transform.localScale = Vector3.Lerp(_objModel.transform.localScale, Vector3.one * _setVisualScale, 7.65f * Time.deltaTime);
			_scaleBuffer -= Time.deltaTime;
		}
	}

	public void Reset_Color()
	{
		_defaultColor = Color.white;
		SkinnedMeshRenderer[] skinnedObjRenders = _skinnedObjRenders;
		foreach (SkinnedMeshRenderer skinnedMeshRenderer in skinnedObjRenders)
		{
			if (skinnedMeshRenderer == null)
			{
				continue;
			}
			Material[] materials = skinnedMeshRenderer.materials;
			for (int j = 0; j < materials.Length; j++)
			{
				if (materials[j].HasProperty(_TINT))
				{
					materials[j].SetColor(_TINT, _defaultColor);
				}
			}
			skinnedMeshRenderer.materials = materials;
		}
		MeshRenderer[] objRenders = _objRenders;
		foreach (MeshRenderer meshRenderer in objRenders)
		{
			if (meshRenderer == null)
			{
				continue;
			}
			Material[] materials2 = meshRenderer.materials;
			for (int k = 0; k < materials2.Length; k++)
			{
				if (materials2[k].HasProperty(_TINT))
				{
					materials2[k].SetColor(_TINT, _defaultColor);
				}
			}
			meshRenderer.materials = materials2;
		}
		foreach (MeshRenderer dynamicObjRender in _dynamicObjRenders)
		{
			if (dynamicObjRender == null)
			{
				continue;
			}
			Material[] materials3 = dynamicObjRender.materials;
			for (int l = 0; l < materials3.Length; l++)
			{
				if (materials3[l].HasProperty(_TINT))
				{
					materials3[l].SetColor(_TINT, _defaultColor);
				}
			}
			dynamicObjRender.materials = materials3;
		}
		Init_Time_RefreshShader(5f);
	}

	private void Handle_RefreshShader()
	{
		if (!_RefreshShader || !_objModel)
		{
			return;
		}
		SkinnedMeshRenderer[] skinnedObjRenders = _skinnedObjRenders;
		foreach (SkinnedMeshRenderer skinnedMeshRenderer in skinnedObjRenders)
		{
			if (skinnedMeshRenderer == null)
			{
				continue;
			}
			Material[] materials = skinnedMeshRenderer.materials;
			for (int j = 0; j < materials.Length; j++)
			{
				if (materials[j].HasProperty(_TINT))
				{
					materials[j].SetColor(_TINT, Color.Lerp(materials[j].GetColor(_TINT), _defaultColor, _smoothTintTime * Time.deltaTime));
				}
				if (materials[j].HasProperty(_SHINE))
				{
					materials[j].SetFloat(_SHINE, Mathf.Lerp(materials[j].GetFloat(_SHINE), 0f, _shineSmoothTime * Time.deltaTime));
				}
			}
			skinnedMeshRenderer.materials = materials;
		}
		MeshRenderer[] objRenders = _objRenders;
		foreach (MeshRenderer meshRenderer in objRenders)
		{
			if (meshRenderer == null)
			{
				continue;
			}
			Material[] materials2 = meshRenderer.materials;
			for (int k = 0; k < materials2.Length; k++)
			{
				if (materials2[k].HasProperty(_TINT))
				{
					materials2[k].SetColor(_TINT, Color.Lerp(materials2[k].GetColor(_TINT), _defaultColor, _smoothTintTime * Time.deltaTime));
				}
				if (materials2[k].HasProperty(_SHINE))
				{
					materials2[k].SetFloat(_SHINE, Mathf.Lerp(materials2[k].GetFloat(_SHINE), 0f, _shineSmoothTime * Time.deltaTime));
				}
			}
			meshRenderer.materials = materials2;
		}
		foreach (MeshRenderer dynamicObjRender in _dynamicObjRenders)
		{
			if (dynamicObjRender == null)
			{
				continue;
			}
			Material[] materials3 = dynamicObjRender.materials;
			for (int l = 0; l < materials3.Length; l++)
			{
				if (materials3[l].HasProperty(_TINT))
				{
					materials3[l].SetColor(_TINT, Color.Lerp(materials3[l].GetColor(_TINT), _defaultColor, _smoothTintTime * Time.deltaTime));
				}
				if (materials3[l].HasProperty(_SHINE))
				{
					materials3[l].SetFloat(_SHINE, Mathf.Lerp(materials3[l].GetFloat(_SHINE), 0f, _shineSmoothTime * Time.deltaTime));
				}
			}
			dynamicObjRender.materials = materials3;
		}
	}

	public void Apply_ColorAdjustShaderParams(float _h, float _s, float _b, float _c)
	{
		SkinnedMeshRenderer[] skinnedObjRenders = _skinnedObjRenders;
		foreach (SkinnedMeshRenderer skinnedMeshRenderer in skinnedObjRenders)
		{
			if (!(skinnedMeshRenderer == null))
			{
				Material[] materials = skinnedMeshRenderer.materials;
				for (int j = 0; j < materials.Length; j++)
				{
					materials[j].SetFloat(_HUE, _h);
					materials[j].SetFloat(_SATURATION, _s);
					materials[j].SetFloat(_BRIGHTNESS, _b);
					materials[j].SetFloat(_CONTRAST, _c);
				}
				skinnedMeshRenderer.materials = materials;
			}
		}
		MeshRenderer[] objRenders = _objRenders;
		foreach (MeshRenderer meshRenderer in objRenders)
		{
			if (!(meshRenderer == null))
			{
				Material[] materials2 = meshRenderer.materials;
				for (int k = 0; k < materials2.Length; k++)
				{
					materials2[k].SetFloat(_HUE, _h);
					materials2[k].SetFloat(_SATURATION, _s);
					materials2[k].SetFloat(_BRIGHTNESS, _b);
					materials2[k].SetFloat(_CONTRAST, _c);
				}
				meshRenderer.materials = materials2;
			}
		}
		foreach (MeshRenderer dynamicObjRender in _dynamicObjRenders)
		{
			if (!(dynamicObjRender == null))
			{
				Material[] materials3 = dynamicObjRender.materials;
				for (int l = 0; l < materials3.Length; l++)
				{
					materials3[l].SetFloat(_HUE, _h);
					materials3[l].SetFloat(_SATURATION, _s);
					materials3[l].SetFloat(_BRIGHTNESS, _b);
					materials3[l].SetFloat(_CONTRAST, _c);
				}
				dynamicObjRender.materials = materials3;
			}
		}
	}

	public void Tint_Shader(Color _color)
	{
		if (!_objModel)
		{
			return;
		}
		SkinnedMeshRenderer[] skinnedObjRenders = _skinnedObjRenders;
		foreach (SkinnedMeshRenderer skinnedMeshRenderer in skinnedObjRenders)
		{
			if (!(skinnedMeshRenderer == null))
			{
				Material[] materials = skinnedMeshRenderer.materials;
				for (int j = 0; j < materials.Length; j++)
				{
					materials[j].SetColor(_TINT, _color);
				}
				skinnedMeshRenderer.materials = materials;
			}
		}
		MeshRenderer[] objRenders = _objRenders;
		foreach (MeshRenderer meshRenderer in objRenders)
		{
			if (!(meshRenderer == null))
			{
				Material[] materials2 = meshRenderer.materials;
				for (int k = 0; k < materials2.Length; k++)
				{
					materials2[k].SetColor(_TINT, _color);
				}
				meshRenderer.materials = materials2;
			}
		}
		foreach (MeshRenderer dynamicObjRender in _dynamicObjRenders)
		{
			if (!(dynamicObjRender == null))
			{
				Material[] materials3 = dynamicObjRender.materials;
				for (int l = 0; l < materials3.Length; l++)
				{
					materials3[l].SetColor(_TINT, _color);
				}
				dynamicObjRender.materials = materials3;
			}
		}
		Init_Time_RefreshShader(5f);
	}

	public void Shine_Shader(Color _color, float _value)
	{
		if (!_objModel)
		{
			return;
		}
		SkinnedMeshRenderer[] skinnedObjRenders = _skinnedObjRenders;
		foreach (SkinnedMeshRenderer skinnedMeshRenderer in skinnedObjRenders)
		{
			if (!(skinnedMeshRenderer == null))
			{
				Material[] materials = skinnedMeshRenderer.materials;
				for (int j = 0; j < materials.Length; j++)
				{
					materials[j].SetColor("_ColorTint2", _color);
					materials[j].SetFloat(_SHINE, _value);
				}
				skinnedMeshRenderer.materials = materials;
			}
		}
		MeshRenderer[] objRenders = _objRenders;
		foreach (MeshRenderer meshRenderer in objRenders)
		{
			if (!(meshRenderer == null))
			{
				Material[] materials2 = meshRenderer.materials;
				for (int k = 0; k < materials2.Length; k++)
				{
					materials2[k].SetColor("_ColorTint2", _color);
					materials2[k].SetFloat(_SHINE, _value);
				}
				meshRenderer.materials = materials2;
			}
		}
		foreach (MeshRenderer dynamicObjRender in _dynamicObjRenders)
		{
			if (!(dynamicObjRender == null))
			{
				Material[] materials3 = dynamicObjRender.materials;
				for (int l = 0; l < materials3.Length; l++)
				{
					materials3[l].SetColor("_ColorTint2", _color);
					materials3[l].SetFloat(_SHINE, _value);
				}
				dynamicObjRender.materials = materials3;
			}
		}
		Init_Time_RefreshShader(1.25f);
	}
}
