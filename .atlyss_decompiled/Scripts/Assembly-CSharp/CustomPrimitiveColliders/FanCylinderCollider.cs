using UnityEngine;

namespace CustomPrimitiveColliders;

[AddComponentMenu("CustomPrimitiveColliders/3D/Fan Cylinder Collider")]
[RequireComponent(typeof(MeshCollider))]
public class FanCylinderCollider : BaseCustomCollider
{
	[SerializeField]
	private float m_radius = 1f;

	[SerializeField]
	private float m_height = 1f;

	[SerializeField]
	[Range(1f, 360f)]
	private int m_fanAngle = 135;

	[SerializeField]
	private int m_numVertices = 32;

	private void Awake()
	{
		ReCreate(m_radius, m_height, m_fanAngle, m_numVertices);
	}

	public void ReCreate(float radius, float height, int fanAngle, int numVertices = 32)
	{
		Mesh sharedMesh = CreateMesh(radius, height, fanAngle, numVertices);
		if (base.meshCollider.sharedMesh != null)
		{
			base.meshCollider.sharedMesh.Clear();
			if (Application.isPlaying)
			{
				Object.Destroy(base.meshCollider.sharedMesh);
			}
			else
			{
				Object.DestroyImmediate(base.meshCollider.sharedMesh);
			}
		}
		base.meshCollider.sharedMesh = sharedMesh;
	}

	private Mesh CreateMesh(float radius, float height, int fanAngle, int numVertices)
	{
		if (radius <= 0f)
		{
			radius = 0.01f;
		}
		if (height <= 0f)
		{
			height = 0.01f;
		}
		fanAngle = Mathf.Clamp(fanAngle, 1, 360);
		if (numVertices < 4)
		{
			numVertices = 4;
		}
		m_radius = radius;
		m_height = height;
		m_fanAngle = fanAngle;
		m_numVertices = numVertices;
		Mesh mesh = new Mesh();
		Vector3[] array = new Vector3[numVertices * 2 + 2];
		Vector3[] array2 = new Vector3[array.Length];
		Vector2[] array3 = new Vector2[array.Length];
		int[] array4 = new int[numVertices * 4 * 3];
		float num = height / 2f;
		Vector3 zero = Vector3.zero;
		Quaternion quaternion = Quaternion.Euler(0f, (float)fanAngle / (float)((fanAngle == 360) ? m_numVertices : (numVertices - 1)), 0f);
		array[0] = new Vector3(0f, 0f - num, 0f);
		array[^1] = new Vector3(0f, num, 0f);
		array2[0] = Vector3.down;
		array2[^1] = Vector3.up;
		array3[0] = new Vector2(0.5f, 0.5f);
		array3[^1] = array3[0];
		int num2 = 0;
		for (int i = 1; i <= numVertices; i++)
		{
			if (i == 1)
			{
				array[i] = new Vector3(radius, 0f - num, 0f);
				array[i + numVertices] = new Vector3(radius, num, 0f);
			}
			else
			{
				array[i] = quaternion * array[i - 1];
				array[i + numVertices] = quaternion * array[i + numVertices - 1];
			}
			if (i == numVertices)
			{
				if (fanAngle >= 360)
				{
					array4[num2++] = 1;
					array4[num2++] = i;
					array4[num2++] = 0;
					array4[num2++] = numVertices * 2;
					array4[num2++] = 1 + numVertices;
					array4[num2++] = array.Length - 1;
					array4[num2++] = 1;
					array4[num2++] = 1 + numVertices;
					array4[num2++] = numVertices;
					array4[num2++] = 1 + numVertices;
					array4[num2++] = numVertices * 2;
					array4[num2++] = numVertices;
				}
				else
				{
					array4[num2++] = 1;
					array4[num2++] = 1 + numVertices;
					array4[num2++] = 0;
					array4[num2++] = 1 + numVertices;
					array4[num2++] = array.Length - 1;
					array4[num2++] = 0;
					array4[num2++] = 0;
					array4[num2++] = array.Length - 1;
					array4[num2++] = numVertices;
					array4[num2++] = array.Length - 1;
					array4[num2++] = numVertices * 2;
					array4[num2++] = numVertices;
				}
			}
			else
			{
				array4[num2++] = i + 1;
				array4[num2++] = i;
				array4[num2++] = 0;
				array4[num2++] = i + numVertices;
				array4[num2++] = i + numVertices + 1;
				array4[num2++] = array.Length - 1;
				array4[num2++] = i + 1;
				array4[num2++] = i + numVertices + 1;
				array4[num2++] = i;
				array4[num2++] = i + numVertices + 1;
				array4[num2++] = i + numVertices;
				array4[num2++] = i;
			}
		}
		int num3 = Mathf.FloorToInt(numVertices / 2);
		Vector3 fromDirection = ((numVertices % 2 != 0) ? (array[num3 + 1] - array[0]) : (array[num3] - array[0] + (array[num3 + 1] - array[0])));
		Quaternion quaternion2 = Quaternion.FromToRotation(fromDirection, Vector3.forward);
		for (int j = 0; j < array.Length; j++)
		{
			float y = array[j].y;
			array[j] = quaternion2 * array[j];
			array[j].y = y;
		}
		for (int k = 1; k <= numVertices; k++)
		{
			array2[k] = array[k] - zero;
			array2[k + numVertices] = array[k + numVertices] - zero;
			array3[k] = new Vector2(0.5f + array[k].x / (2f * radius), 0.5f + array[k].z / (2f * radius));
			array3[k + numVertices] = new Vector2(0.5f + array[k + numVertices].x / (2f * radius), 0.5f + array[k + numVertices].z / (2f * radius));
		}
		mesh.vertices = array;
		mesh.normals = array2;
		mesh.uv = array3;
		mesh.triangles = array4;
		return mesh;
	}
}
