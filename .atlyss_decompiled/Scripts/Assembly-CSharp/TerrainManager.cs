using System.Collections.Generic;
using UnityEngine;

public class TerrainManager : MonoBehaviour
{
	public List<TerrainManagerData> _terrainManagerDatas = new List<TerrainManagerData>();

	public GroundMaterial[] groundMaterials;

	private Terrain[] foundTerrains;

	private void Start()
	{
		GetTerrainProps();
	}

	public void GetTerrainProps()
	{
		foundTerrains = Terrain.activeTerrains;
		for (int i = 0; i < foundTerrains.Length; i++)
		{
			TerrainData terrainData = foundTerrains[i].terrainData;
			TerrainManagerData item = new TerrainManagerData
			{
				_terrainData = terrainData,
				_terrainPos = foundTerrains[i].transform.position,
				alphamapWidth = terrainData.alphamapWidth,
				alphamapHeight = terrainData.alphamapHeight,
				mSplatmapData = terrainData.GetAlphamaps(0, 0, terrainData.alphamapWidth, terrainData.alphamapHeight),
				mNumTextures = terrainData.GetAlphamaps(0, 0, terrainData.alphamapWidth, terrainData.alphamapHeight).Length / (terrainData.alphamapWidth * terrainData.alphamapHeight)
			};
			_terrainManagerDatas.Add(item);
		}
	}

	private Vector3 ConvertToSplatMapCoordinate(Vector3 playerPos, TerrainData _terrainData)
	{
		Vector3 result = default;
		for (int i = 0; i < _terrainManagerDatas.Count; i++)
		{
			if (!(_terrainManagerDatas[i]._terrainData != _terrainData))
			{
				result.x = (playerPos.x - _terrainManagerDatas[i]._terrainPos.x) / _terrainData.size.x * (float)_terrainData.alphamapWidth;
				result.z = (playerPos.z - _terrainManagerDatas[i]._terrainPos.z) / _terrainData.size.z * (float)_terrainData.alphamapHeight;
			}
		}
		return result;
	}

	private int GetActiveTerrainTextureIdx(Vector3 pos, TerrainData _terrainData)
	{
		int result = 0;
		float num = 0f;
		for (int i = 0; i < _terrainManagerDatas.Count; i++)
		{
			if (_terrainManagerDatas[i]._terrainData != _terrainData)
			{
				continue;
			}
			Vector3 vector = ConvertToSplatMapCoordinate(pos, _terrainManagerDatas[i]._terrainData);
			for (int j = 0; j < _terrainManagerDatas[i].mNumTextures; j++)
			{
				if (_terrainManagerDatas[i].mSplatmapData.Length >= j && num < _terrainManagerDatas[i].mSplatmapData[(int)vector.z, (int)vector.x, j])
				{
					result = j;
				}
			}
		}
		return result;
	}

	public int GetTerrainAtPosition(Vector3 pos, TerrainCollider _terrainCollider)
	{
		TerrainData terrainData = _terrainCollider.terrainData;
		return GetActiveTerrainTextureIdx(pos, terrainData);
	}
}
