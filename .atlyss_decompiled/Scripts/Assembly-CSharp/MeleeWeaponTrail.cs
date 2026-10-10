using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MeleeWeaponTrail : MonoBehaviour
{
	[Serializable]
	public class Point
	{
		public float timeCreated;

		public Vector3 basePosition;

		public Vector3 tipPosition;
	}

	private Player _parentPlayer;

	private GameObject _trailObject;

	private Mesh _trailMesh;

	private Vector3 _lastPosition;

	[SerializeField]
	private bool _emit = true;

	[SerializeField]
	private float _emitTime;

	[SerializeField]
	private Material _material;

	public float _lifeTime = 1f;

	public Color[] _colors = new Color[0];

	public float[] _sizes = new float[0];

	[SerializeField]
	private float _minVertexDistance = 0.1f;

	[SerializeField]
	private float _maxVertexDistance = 10f;

	private float _minVertexDistanceSqr;

	private float _maxVertexDistanceSqr;

	[SerializeField]
	private float _maxAngle = 3f;

	private bool _trailObjectInitalized;

	[SerializeField]
	private int subdivisions = 4;

	public Transform _base;

	public Transform _tip;

	private List<Point> _points = new List<Point>();

	private List<Point> _smoothedPoints = new List<Point>();

	public bool Emit
	{
		set
		{
			_emit = value;
		}
	}

	private void Start()
	{
		StartCoroutine(Init_TrailObject());
	}

	private IEnumerator Init_TrailObject()
	{
		_parentPlayer = GetComponentInParent<Player>();
		if ((bool)_parentPlayer)
		{
			if ((bool)Player._mainPlayer && _parentPlayer == Player._mainPlayer)
			{
				yield return new WaitForSeconds(0.02f);
			}
			if ((bool)Player._mainPlayer && _parentPlayer != Player._mainPlayer)
			{
				yield return new WaitForSeconds(0.25f);
			}
			_lastPosition = base.transform.position;
			_trailObject = new GameObject();
			_trailObject.transform.SetParent(base.transform);
			_trailObject.transform.SetParent(null);
			_trailObject.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
			_trailObject.transform.localScale = Vector3.one;
			_trailObject.AddComponent(typeof(MeshFilter));
			_trailObject.AddComponent(typeof(MeshRenderer));
			_trailObject.GetComponent<Renderer>().material = _material;
			_trailMesh = new Mesh();
			_trailMesh.indexFormat = IndexFormat.UInt32;
			_trailMesh.name = base.name + "TrailMesh";
			_trailObject.GetComponent<MeshFilter>().mesh = _trailMesh;
			_minVertexDistanceSqr = _minVertexDistance * _minVertexDistance;
			_maxVertexDistanceSqr = _maxVertexDistance * _maxVertexDistance;
			_trailObject.name = "_wepTrailMesh_(" + _parentPlayer._nickname + ")";
			_trailObjectInitalized = true;
		}
	}

	private void OnDestroy()
	{
		if ((bool)_trailMesh)
		{
			UnityEngine.Object.Destroy(_trailMesh);
		}
		if ((bool)_trailObject)
		{
			UnityEngine.Object.Destroy(_trailObject);
		}
	}

	private void Clear_Trail()
	{
		RemoveOldPoints(_points);
		RemoveOldPoints(_smoothedPoints);
		if (_smoothedPoints.Count == 0 && (bool)_trailMesh)
		{
			_trailMesh.Clear();
		}
	}

	private void Update()
	{
		if (!Player._mainPlayer || !_trailObjectInitalized)
		{
			return;
		}
		if (!_parentPlayer)
		{
			if ((bool)_trailObject)
			{
				UnityEngine.Object.Destroy(_trailMesh);
			}
			if ((bool)_trailMesh)
			{
				Clear_Trail();
				UnityEngine.Object.Destroy(_trailMesh);
			}
			return;
		}
		if ((bool)_parentPlayer._pCasting._currentCastSkill || _parentPlayer._currentPlayerAction == PlayerAction.CASTING)
		{
			if ((bool)_trailMesh)
			{
				Clear_Trail();
			}
			return;
		}
		if (!_trailObject && !_trailMesh)
		{
			StopAllCoroutines();
			StartCoroutine(Init_TrailObject());
			_trailObjectInitalized = false;
		}
		if (Player._mainPlayer._bufferingStatus || !Player._mainPlayer.Network_playerMapInstance)
		{
			Clear_Trail();
			return;
		}
		if (Player._mainPlayer._isHostPlayer && Player._mainPlayer.gameObject.scene != base.gameObject.scene)
		{
			Clear_Trail();
			return;
		}
		if (!_parentPlayer.isLocalPlayer && _parentPlayer.Network_playerMapInstance != Player._mainPlayer.Network_playerMapInstance)
		{
			Clear_Trail();
			return;
		}
		if (_emit && _emitTime != 0f)
		{
			_emitTime -= Time.deltaTime;
			if (_emitTime == 0f)
			{
				_emitTime = -1f;
			}
			if (_emitTime < 0f)
			{
				_emit = false;
			}
		}
		float sqrMagnitude = (_lastPosition - base.transform.position).sqrMagnitude;
		if (_emit)
		{
			if (sqrMagnitude > _minVertexDistanceSqr)
			{
				bool flag = false;
				if (_points.Count < 3)
				{
					flag = true;
				}
				else
				{
					List<Point> points = _points;
					Vector3 tipPosition = points[points.Count - 2].tipPosition;
					List<Point> points2 = _points;
					Vector3 vector = tipPosition - points2[points2.Count - 3].tipPosition;
					List<Point> points3 = _points;
					Vector3 tipPosition2 = points3[points3.Count - 1].tipPosition;
					List<Point> points4 = _points;
					Vector3 to = tipPosition2 - points4[points4.Count - 2].tipPosition;
					if (Vector3.Angle(vector, to) > _maxAngle || sqrMagnitude > _maxVertexDistanceSqr)
					{
						flag = true;
					}
				}
				if (flag)
				{
					Point point = new Point();
					point.basePosition = _base.position;
					point.tipPosition = _tip.position;
					point.timeCreated = Time.time;
					_points.Add(point);
					_lastPosition = base.transform.position;
					if (_points.Count == 1)
					{
						_smoothedPoints.Add(point);
					}
					else if (_points.Count > 1)
					{
						for (int i = 0; i < 1 + subdivisions; i++)
						{
							_smoothedPoints.Add(point);
						}
					}
					if (_points.Count >= 4)
					{
						Vector3[] array = new Vector3[4];
						List<Point> points5 = _points;
						array[0] = points5[points5.Count - 4].tipPosition;
						List<Point> points6 = _points;
						array[1] = points6[points6.Count - 3].tipPosition;
						List<Point> points7 = _points;
						array[2] = points7[points7.Count - 2].tipPosition;
						List<Point> points8 = _points;
						array[3] = points8[points8.Count - 1].tipPosition;
						IEnumerable<Vector3> collection = Interpolate.NewCatmullRom(array, subdivisions, loop: false);
						Vector3[] array2 = new Vector3[4];
						List<Point> points9 = _points;
						array2[0] = points9[points9.Count - 4].basePosition;
						List<Point> points10 = _points;
						array2[1] = points10[points10.Count - 3].basePosition;
						List<Point> points11 = _points;
						array2[2] = points11[points11.Count - 2].basePosition;
						List<Point> points12 = _points;
						array2[3] = points12[points12.Count - 1].basePosition;
						IEnumerable<Vector3> collection2 = Interpolate.NewCatmullRom(array2, subdivisions, loop: false);
						List<Vector3> list = new List<Vector3>(collection);
						List<Vector3> list2 = new List<Vector3>(collection2);
						float timeCreated = _points[_points.Count - 4].timeCreated;
						float timeCreated2 = _points[_points.Count - 1].timeCreated;
						for (int j = 0; j < list.Count; j++)
						{
							int num = _smoothedPoints.Count - (list.Count - j);
							if (num > -1 && num < _smoothedPoints.Count)
							{
								Point point2 = new Point();
								point2.basePosition = list2[j];
								point2.tipPosition = list[j];
								point2.timeCreated = Mathf.Lerp(timeCreated, timeCreated2, (float)j / (float)list.Count);
								_smoothedPoints[num] = point2;
							}
						}
					}
				}
				else
				{
					List<Point> points13 = _points;
					points13[points13.Count - 1].basePosition = _base.position;
					List<Point> points14 = _points;
					points14[points14.Count - 1].tipPosition = _tip.position;
					List<Point> smoothedPoints = _smoothedPoints;
					smoothedPoints[smoothedPoints.Count - 1].basePosition = _base.position;
					List<Point> smoothedPoints2 = _smoothedPoints;
					smoothedPoints2[smoothedPoints2.Count - 1].tipPosition = _tip.position;
				}
			}
			else
			{
				if (_points.Count > 0)
				{
					List<Point> points15 = _points;
					points15[points15.Count - 1].basePosition = _base.position;
					List<Point> points16 = _points;
					points16[points16.Count - 1].tipPosition = _tip.position;
				}
				if (_smoothedPoints.Count > 0)
				{
					List<Point> smoothedPoints3 = _smoothedPoints;
					smoothedPoints3[smoothedPoints3.Count - 1].basePosition = _base.position;
					List<Point> smoothedPoints4 = _smoothedPoints;
					smoothedPoints4[smoothedPoints4.Count - 1].tipPosition = _tip.position;
				}
			}
		}
		else if ((bool)_trailMesh)
		{
			_trailMesh.Clear();
		}
		RemoveOldPoints(_points);
		if (_points.Count == 0 && (bool)_trailMesh)
		{
			_trailMesh.Clear();
		}
		RemoveOldPoints(_smoothedPoints);
		if (_smoothedPoints.Count == 0 && (bool)_trailMesh)
		{
			_trailMesh.Clear();
		}
		List<Point> smoothedPoints5 = _smoothedPoints;
		if (smoothedPoints5.Count <= 1)
		{
			return;
		}
		Vector3[] array3 = new Vector3[smoothedPoints5.Count * 2];
		Vector2[] array4 = new Vector2[smoothedPoints5.Count * 2];
		int[] array5 = new int[(smoothedPoints5.Count - 1) * 6];
		Color[] array6 = new Color[smoothedPoints5.Count * 2];
		for (int k = 0; k < smoothedPoints5.Count; k++)
		{
			Point point3 = smoothedPoints5[k];
			float num2 = (Time.time - point3.timeCreated) / _lifeTime;
			Color color = Color.Lerp(Color.white, Color.clear, num2);
			if (_colors != null && _colors.Length != 0)
			{
				float num3 = num2 * (float)(_colors.Length - 1);
				float num4 = Mathf.Floor(num3);
				float num5 = Mathf.Clamp(Mathf.Ceil(num3), 1f, _colors.Length - 1);
				float t = Mathf.InverseLerp(num4, num5, num3);
				if (num4 >= (float)_colors.Length)
				{
					num4 = _colors.Length - 1;
				}
				if (num4 < 0f)
				{
					num4 = 0f;
				}
				if (num5 >= (float)_colors.Length)
				{
					num5 = _colors.Length - 1;
				}
				if (num5 < 0f)
				{
					num5 = 0f;
				}
				color = Color.Lerp(_colors[(int)num4], _colors[(int)num5], t);
			}
			float num6 = 0f;
			if (_sizes != null && _sizes.Length != 0)
			{
				float num7 = num2 * (float)(_sizes.Length - 1);
				float num8 = Mathf.Floor(num7);
				float num9 = Mathf.Clamp(Mathf.Ceil(num7), 1f, _sizes.Length - 1);
				float t2 = Mathf.InverseLerp(num8, num9, num7);
				if (num8 >= (float)_sizes.Length)
				{
					num8 = _sizes.Length - 1;
				}
				if (num8 < 0f)
				{
					num8 = 0f;
				}
				if (num9 >= (float)_sizes.Length)
				{
					num9 = _sizes.Length - 1;
				}
				if (num9 < 0f)
				{
					num9 = 0f;
				}
				num6 = Mathf.Lerp(_sizes[(int)num8], _sizes[(int)num9], t2);
			}
			Vector3 vector2 = point3.tipPosition - point3.basePosition;
			array3[k * 2] = point3.basePosition - vector2 * (num6 * 0.5f);
			array3[k * 2 + 1] = point3.tipPosition + vector2 * (num6 * 0.5f);
			array6[k * 2] = (array6[k * 2 + 1] = color);
			float x = (float)k / (float)smoothedPoints5.Count;
			array4[k * 2] = new Vector2(x, 0f);
			array4[k * 2 + 1] = new Vector2(x, 1f);
			if (k > 0)
			{
				array5[(k - 1) * 6] = k * 2 - 2;
				array5[(k - 1) * 6 + 1] = k * 2 - 1;
				array5[(k - 1) * 6 + 2] = k * 2;
				array5[(k - 1) * 6 + 3] = k * 2 + 1;
				array5[(k - 1) * 6 + 4] = k * 2;
				array5[(k - 1) * 6 + 5] = k * 2 - 1;
			}
		}
		if ((bool)_trailMesh)
		{
			_trailMesh.Clear();
			_trailMesh.vertices = array3;
			_trailMesh.colors = array6;
			_trailMesh.uv = array4;
			_trailMesh.triangles = array5;
		}
	}

	private void RemoveOldPoints(List<Point> pointList)
	{
		List<Point> list = new List<Point>();
		foreach (Point point in pointList)
		{
			if (Time.time - point.timeCreated > _lifeTime)
			{
				list.Add(point);
			}
		}
		foreach (Point item in list)
		{
			pointList.Remove(item);
		}
	}
}
