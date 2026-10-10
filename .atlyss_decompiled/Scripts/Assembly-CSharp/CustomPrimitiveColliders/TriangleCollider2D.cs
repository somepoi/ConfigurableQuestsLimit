using System;
using UnityEngine;

namespace CustomPrimitiveColliders;

[AddComponentMenu("CustomPrimitiveColliders/2D/Triangle Collider 2D")]
[RequireComponent(typeof(PolygonCollider2D))]
public class TriangleCollider2D : BaseCustomCollider
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

	private void Awake()
	{
		ReCreate(m_radius, m_length, m_useOpenAngle, m_openAngle);
	}

	public void ReCreate(float radius, float length, bool useOpenAngle = false, int openAngle = 45)
	{
		Vector2[] points = CreatePoints(radius, length, useOpenAngle, openAngle);
		base.polygonCollider2d.points = null;
		base.polygonCollider2d.points = points;
	}

	private Vector2[] CreatePoints(float radius, float length, bool useOpenAngle, int openAngle)
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
		m_radius = radius;
		m_length = length;
		m_useOpenAngle = useOpenAngle;
		m_openAngle = openAngle;
		return new Vector2[4]
		{
			Vector2.zero,
			new Vector2(radius, length),
			new Vector2(0f - radius, length),
			Vector2.zero
		};
	}
}
