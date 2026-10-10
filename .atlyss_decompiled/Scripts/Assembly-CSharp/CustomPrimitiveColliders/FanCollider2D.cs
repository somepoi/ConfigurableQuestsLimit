using UnityEngine;

namespace CustomPrimitiveColliders;

[AddComponentMenu("CustomPrimitiveColliders/2D/Fan Collider 2D")]
[RequireComponent(typeof(PolygonCollider2D))]
public class FanCollider2D : BaseCustomCollider
{
	[SerializeField]
	private float m_radius = 1f;

	[SerializeField]
	[Range(1f, 360f)]
	private int m_fanAngle = 135;

	[SerializeField]
	private int m_numVertices = 32;

	private void Awake()
	{
		ReCreate(m_radius, m_fanAngle, m_numVertices);
	}

	public void ReCreate(float radius, int fanAngle, int numVertices = 32)
	{
		Vector2[] points = CreatePoints(radius, fanAngle, numVertices);
		base.polygonCollider2d.points = null;
		base.polygonCollider2d.points = points;
	}

	private Vector2[] CreatePoints(float radius, int fanAngle, int numVertices)
	{
		if (radius <= 0f)
		{
			radius = 0.01f;
		}
		fanAngle = Mathf.Clamp(fanAngle, 1, 360);
		if (numVertices < 4)
		{
			numVertices = 4;
		}
		m_radius = radius;
		m_fanAngle = fanAngle;
		m_numVertices = numVertices;
		Vector2[] array = new Vector2[numVertices + ((fanAngle != 360) ? 1 : 2)];
		Quaternion quaternion = Quaternion.Euler(0f, 0f, (float)fanAngle / (float)((fanAngle == 360) ? m_numVertices : (numVertices - 1)));
		array[0] = Vector2.zero;
		for (int i = 1; i <= numVertices; i++)
		{
			if (i == 1)
			{
				array[i] = new Vector2(radius, 0f);
			}
			else
			{
				array[i] = quaternion * array[i - 1];
			}
		}
		if (fanAngle == 360)
		{
			array[^1] = array[1];
		}
		int num = Mathf.FloorToInt(numVertices / 2);
		Vector2 vector = ((numVertices % 2 != 0) ? (array[num + 1] - array[0]) : (array[num] - array[0] + (array[num + 1] - array[0])));
		Quaternion quaternion2 = Quaternion.FromToRotation(vector, Vector3.up);
		for (int j = 0; j < array.Length; j++)
		{
			array[j] = quaternion2 * array[j];
		}
		return array;
	}
}
