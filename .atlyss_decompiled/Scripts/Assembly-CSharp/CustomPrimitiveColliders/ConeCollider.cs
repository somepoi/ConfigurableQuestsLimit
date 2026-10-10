using System;
using UnityEngine;

namespace CustomPrimitiveColliders;

[AddComponentMenu("CustomPrimitiveColliders/3D/Cone Collider")]
[RequireComponent(typeof(MeshCollider))]
public class ConeCollider : BaseCustomCollider
{
	[SerializeField]
	private float m_radius = 0.5f;

	[SerializeField]
	private float m_length = 1f;

	[SerializeField]
	private bool m_useOpenAngle;

	[SerializeField]
	[Range(1f, 179f)]
	private int m_openAngle = 45;

	[SerializeField]
	private int m_numVertices = 32;

	private void Awake()
	{
		ReCreate(m_radius, m_length, m_useOpenAngle, m_openAngle, m_numVertices);
	}

	public void ReCreate(float radius, float length, bool useOpenAngle = false, int openAngle = 45, int numVertices = 32)
	{
		Mesh sharedMesh = CreateMesh(radius, length, useOpenAngle, openAngle, numVertices);
		if (base.meshCollider.sharedMesh != null)
		{
			base.meshCollider.sharedMesh.Clear();
			if (Application.isPlaying)
			{
				UnityEngine.Object.Destroy(base.meshCollider.sharedMesh);
			}
			else
			{
				UnityEngine.Object.DestroyImmediate(base.meshCollider.sharedMesh);
			}
		}
		base.meshCollider.sharedMesh = sharedMesh;
	}

	private Mesh CreateMesh(float radius, float length, bool useOpenAngle, int openAngle, int numVertices)
	{
		if (radius <= 0f)
		{
			radius = 0.01f;
		}
		if (length <= 0f)
		{
			length = 0.01f;
		}
		openAngle = Mathf.Clamp(openAngle, 1, 179);
		if (useOpenAngle)
		{
			radius = length * Mathf.Tan((float)openAngle * (MathF.PI / 180f) / 2f);
		}
		if (numVertices < 4)
		{
			numVertices = 4;
		}
		m_radius = radius;
		m_length = length;
		m_useOpenAngle = useOpenAngle;
		m_openAngle = openAngle;
		m_numVertices = numVertices;
		Mesh mesh = new Mesh();
		Vector3[] array = new Vector3[numVertices * 3];
		Vector3[] array2 = new Vector3[numVertices * 3];
		Vector2[] array3 = new Vector2[numVertices * 3];
		int[] array4 = new int[numVertices * 6];
		float f = Mathf.Atan(radius / length);
		float num = Mathf.Sin(f);
		float num2 = Mathf.Cos(f);
		int num3 = 0;
		for (int i = 0; i < numVertices; i++)
		{
			float f2 = MathF.PI * 2f * (float)i / (float)numVertices;
			float num4 = Mathf.Sin(f2);
			float num5 = Mathf.Cos(f2);
			float f3 = MathF.PI * 2f * ((float)i + 0.5f) / (float)numVertices;
			float num6 = Mathf.Sin(f3);
			float num7 = Mathf.Cos(f3);
			array[i] = Vector3.zero;
			array[i + numVertices] = new Vector3(radius * num5, radius * num4, length);
			array[i + numVertices * 2] = new Vector3(0f, 0f, length);
			array2[i] = new Vector3(num7 * num2, num6 * num2, 0f - num);
			array2[i + numVertices] = new Vector3(num7 * num2, num6 * num2, 0f - num);
			array2[i + numVertices * 2] = Vector3.forward;
			array3[i] = new Vector2((float)i / (float)numVertices, 1f);
			array3[i + numVertices] = new Vector2((float)i / (float)numVertices, 0f);
			array3[i + numVertices * 2] = new Vector2(0.5f, 0.5f);
			array4[num3++] = i + numVertices;
			array4[num3++] = i;
			array4[num3++] = ((i == numVertices - 1) ? numVertices : (i + numVertices + 1));
			array4[num3++] = i + numVertices;
			array4[num3++] = ((i == numVertices - 1) ? numVertices : (i + numVertices + 1));
			array4[num3++] = i + numVertices * 2;
		}
		mesh.vertices = array;
		mesh.normals = array2;
		mesh.uv = array3;
		mesh.triangles = array4;
		return mesh;
	}
}
