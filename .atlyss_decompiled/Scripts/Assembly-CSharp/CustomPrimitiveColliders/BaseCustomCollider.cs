using UnityEngine;

namespace CustomPrimitiveColliders;

public abstract class BaseCustomCollider : MonoBehaviour
{
	private MeshCollider m_meshCollider;

	private PolygonCollider2D m_polygonCollider2d;

	protected MeshCollider meshCollider
	{
		get
		{
			if (m_meshCollider == null)
			{
				m_meshCollider = GetComponent<MeshCollider>();
				if (m_meshCollider == null)
				{
					m_meshCollider = base.gameObject.AddComponent<MeshCollider>();
				}
			}
			return m_meshCollider;
		}
	}

	protected PolygonCollider2D polygonCollider2d
	{
		get
		{
			if (m_polygonCollider2d == null)
			{
				m_polygonCollider2d = GetComponent<PolygonCollider2D>();
				if (m_polygonCollider2d == null)
				{
					m_polygonCollider2d = base.gameObject.AddComponent<PolygonCollider2D>();
				}
			}
			return m_polygonCollider2d;
		}
	}
}
