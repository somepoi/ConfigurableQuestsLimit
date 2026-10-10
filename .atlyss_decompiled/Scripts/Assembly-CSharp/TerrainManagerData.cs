using System;
using UnityEngine;

[Serializable]
public class TerrainManagerData
{
	public TerrainData _terrainData;

	public Vector3 _terrainPos;

	public int alphamapWidth;

	public int alphamapHeight;

	public int mNumTextures;

	public float[,,] mSplatmapData;
}
