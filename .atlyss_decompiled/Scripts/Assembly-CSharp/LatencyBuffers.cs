using System;
using UnityEngine;

[Serializable]
public class LatencyBuffers
{
	[HideInInspector]
	public string _latencyBufferTag = string.Empty;

	public Vector2Int _latencyThreshold;

	public float _bufferValue;
}
