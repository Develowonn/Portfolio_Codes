// # Unity
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using static BlockConstants;

public class Map
{
	private MapSettingManager mapSettingManager;

	private Chunk[,] groundChunks;
	private Chunk[,] waterChunks;

	// 이전 프레임에 활성화 되었던 청크 목록
	private List<Chunk> prevActiveChunkList    = new List<Chunk>();
	// 현재 프레임에 활성화된 청크 목록
	private List<Chunk> currentActiveChunkList = new List<Chunk>();

	// 플레이어의 이전 프레임 위치
	private Vector2  prevPlayerCoord;
	// 플레이어의 현재 프레임 위치
	private Vector2  currentPlayerCoord;

	public Map(MapSettingManager mapSettingManager)
	{
		this.mapSettingManager = mapSettingManager;

		groundChunks = new Chunk[mapSettingManager.MapSizeInChunks, mapSettingManager.MapSizeInChunks];
		waterChunks  = new Chunk[mapSettingManager.MapSizeInChunks, mapSettingManager.MapSizeInChunks];

		GenerateMap();
	}

    /// <summary> 주어진 청크 좌표에서 해당하는 청크를 업데이트합니다. </summary>
    public void DrawMap(int x, int z)
	{
		GetChunkFromChunkCoord(x, z, ChunkType.Ground).UpdateChunk();
        GetChunkFromChunkCoord(x, z, ChunkType.Water).UpdateChunk();
    }

    /// <summary> 맵을 생성하고, 청크를 생성하여 표시합니다. </summary>
    public void GenerateMap()
	{
        int center =  mapSettingManager.MapSizeInChunks / 2;
        int viewMin = center - mapSettingManager.ViewDistanceInChunks;
        int viewMax = center + mapSettingManager.ViewDistanceInChunks;

        for (int x = viewMin; x < viewMax; x++)
        {
            for (int z = viewMin; z < viewMax; z++)
            {
                GenerateChunk(x, z);
            }
        }

        for (int x = viewMin; x < viewMax; x++)
        {
            for (int z = viewMin; z < viewMax; z++)
            {
                DrawMap(x, z);
            }
        }
    }

    /// <summary> 주어진 좌표에서 청크를 생성합니다. </summary>
    public void GenerateChunk(int x, int z)
	{
		Chunk groundTempChunk = new Chunk(new Vector2Int(x, z), this, mapSettingManager, ChunkType.Ground);
        Chunk waterTempChunk  = new Chunk(new Vector2Int(x, z), this, mapSettingManager, ChunkType.Water);

		JsonManager.Instance.Save(groundTempChunk); //새로 생성된 청크 저장

        groundChunks[x, z]   = groundTempChunk;
		waterChunks[x, z]	 = waterTempChunk;
	}

    /// <summary> 주어진 위치의 청크를 업데이트합니다. </summary>
    public void UpdateChunk(Vector3 pos)
	{
		if (IsVoxelInMap(pos))
		{
			GetChunkFromPosition(pos, ChunkType.Ground).UpdateChunk();
		}
		else
		{
			Debug.Log("청크가 존재하지 않아 UpdateChunk() 실행 불가");
		}
	}

    /// <summary> 플레이어의 위치를 확인하고, 청크 업데이트를 체크합니다. </summary>
    public void CheckAndUpdateChunks()
	{
		currentPlayerCoord = GetChunkCoordFromPosition(mapSettingManager.Player.position);

		if (!prevPlayerCoord.Equals(currentPlayerCoord))
			UpdateChunkInViewRange();

		prevPlayerCoord = currentPlayerCoord;
	}

    /// <summary> 스폰 위치를 초기화합니다. </summary>
    public void InitializeSpawnPosition()
	{
		Vector3 spawnPosition = new Vector3(
				ChunkData.ChunkWidthValue  * mapSettingManager.MapSizeInChunks * 0.5f,
				ChunkData.ChunkHeightValue,
				ChunkData.ChunkLengthValue * mapSettingManager.MapSizeInChunks * 0.5f
		);
		mapSettingManager.Player.position = spawnPosition;

		prevPlayerCoord    = new Vector2Int(-1, -1);
		currentPlayerCoord = GetChunkCoordFromPosition(mapSettingManager.Player.position);
	}

    /// <summary> 현재 뷰 범위 내에서 청크를 활성화/비활성화합니다. </summary>
    private void UpdateChunkInViewRange()
	{
		Vector2 chunkCoord = GetChunkCoordFromPosition(mapSettingManager.Player.position);
		int viewDist = mapSettingManager.ViewDistanceInChunks;

		(int x, int z) viewMin = ((int)chunkCoord.x - viewDist, (int)chunkCoord.y - viewDist);
		(int x, int z) viewMax = ((int)chunkCoord.x + viewDist, (int)chunkCoord.y + viewDist);

		// 활성 목록 : 현재 -> 이전으로 이동
		prevActiveChunkList = currentActiveChunkList;
		currentActiveChunkList = new List<Chunk>();

		for (int x = viewMin.x; x < viewMax.x; x++)
		{
			for (int z = viewMin.z; z < viewMax.z; z++)
			{
				if (IsChunkInMap(x, z) == false)
					continue;

				Chunk currentChunk = groundChunks[x, z];

				if (groundChunks[x, z] == null)
				{
					GenerateChunk(x, z);
					currentChunk = groundChunks[x, z];
				}
				else if (groundChunks[x, z].IsActive == false)
				{
					groundChunks[x, z].IsActive = true;
					waterChunks[x, z].IsActive = true;
				}

				currentActiveChunkList.Add(currentChunk);

				if (prevActiveChunkList.Contains(currentChunk))
					prevActiveChunkList.Remove(currentChunk);
			}
		}

		// 차집합으로 남은 청크들 비활성화
		foreach (var chunk in prevActiveChunkList)
		{
			Vector2Int coord = chunk.Coord;
			chunk.IsActive = false;
			waterChunks[coord.x, coord.y].IsActive = false;
		}
	}

    #region 높이 데이터, 블럭 데이터 설정 
    /// <summary> 주어진 위치에서 블록 높이를 계산하여 반환합니다. </summary>
    public int CalculateBlcokHeight(Vector3 pos)
	{
		return PerlinNoise.GetHeightFromNoise(new Vector2(pos.x, pos.z), mapSettingManager.Scale, mapSettingManager.Seed);
	}

    /// <summary> 주어진 위치와 블록 높이에 따라 해당 블록의 데이터를 계산하여 반환합니다. </summary>
    public BlockData CalculateBlockData(Vector3 pos, int blockHeight)
	{
		if(blockHeight <= mapSettingManager.WaterHeight && pos.y < mapSettingManager.WaterHeight)
		{
			return mapSettingManager.FindBlockType(Water);
		}
		else if(pos.y < blockHeight)
		{
			return mapSettingManager.GetBlockTypeWithNoise(new Vector2(pos.x, pos.z));
        }
		else
		{
			return mapSettingManager.FindBlockType(Air);
        }
	}
    #endregion

    #region 청크 조회 및 월드 범위 검사 
    /// <summary> 주어진 위치와 청크 타입에 맞는 청크를 반환합니다. </summary>
    public Chunk GetChunkFromPosition(Vector3 pos, ChunkType chunkType)
	{
		int x = Mathf.FloorToInt(pos.x / ChunkData.ChunkWidthValue);
		int z = Mathf.FloorToInt(pos.z / ChunkData.ChunkLengthValue);

		switch (chunkType) 
		{
			case ChunkType.Ground:
				return groundChunks[x, z];
            case ChunkType.Water:
				return waterChunks[x, z];
		}
		return null;
    }

    /// <summary> 주어진 좌표와 청크 타입에 맞는 청크를 반환합니다. </summary>
    public Chunk GetChunkFromPosition(float x, float z, ChunkType chunkType)
    {
        int coordX = Mathf.FloorToInt(x / ChunkData.ChunkWidthValue);
        int coordZ = Mathf.FloorToInt(z / ChunkData.ChunkLengthValue);

        switch (chunkType)
        {
            case ChunkType.Ground:
				return groundChunks[coordX, coordZ];
			case ChunkType.Water:
				return waterChunks[coordX, coordZ];
        }

        return null;
    }

    /// <summary> 주어진 청크 좌표와 청크 타입에 맞는 청크를 반환합니다. </summary>	
    public Chunk GetChunkFromChunkCoord(int x, int z, ChunkType chunkType)
	{
        switch (chunkType)
        {
            case ChunkType.Ground:
				return groundChunks[x, z];
			case ChunkType.Water:
				return waterChunks[x, z];
		}
        Debug.Log($"{x} : {z} 좌표에 있는 청크가 없습니다");
		return null;
	}

    /// <summary> 주어진 위치에서 청크 좌표를 계산하여 반환합니다. </summary>
    public Vector2 GetChunkCoordFromPosition(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x / ChunkData.ChunkWidthValue);
        int z = Mathf.FloorToInt(pos.z / ChunkData.ChunkLengthValue);

        return new Vector2(x, z);
    }

    /// <summary> 주어진 복셀 위치가 맵 내에 있는지 확인하여 반환합니다. </summary>
    public bool IsVoxelInMap(Vector3 pos)
	{
		int x = Mathf.FloorToInt(pos.x);
		int y = Mathf.FloorToInt(pos.y);
		int z = Mathf.FloorToInt(pos.z);

		if (x < 0 || x >= (mapSettingManager.MapSizeInChunks * ChunkData.ChunkWidthValue) ||
			z < 0 || z >= (mapSettingManager.MapSizeInChunks * ChunkData.ChunkLengthValue) ||
			y < 0 || y >= ChunkData.ChunkHeightValue)
		{
			return false;
		}
		else
		{
			return true;
		}
	}

    /// <summary> 주어진 청크가 맵 내에 있는지 확인하여 반환합니다. </summary>
    public bool IsChunkInMap(int x, int z)
	{
		if (x >= 0 && x < mapSettingManager.MapSizeInChunks &&
			z >= 0 && z < mapSettingManager.MapSizeInChunks)
		{
			return true;
		}
		else
		{
			return false;
		}
    }

	//public string GetVoxelType(Vector3 pos)
	//{
	//	int x = Mathf.FloorToInt(pos.x);
	//	int y = Mathf.FloorToInt(pos.y);
	//	int z = Mathf.FloorToInt(pos.z);

	//	y += -((int)mapSettingManager.ParentChunkYPos);

	//	if (x < 0 || x >= (mapSettingManager.MapWidthChunkValue * ChunkData.ChunkWidthValue) ||
	//		z < 0 || z >= (mapSettingManager.MapLengthChunkValue * ChunkData.ChunkLengthValue) ||
	//		y <= mapSettingManager.ParentChunkYPos || y >= ChunkData.ChunkHeightValue)
	//	{
	//		return Air;
	//	}
	//	return mapBlock[x, y, z].id;
	//}
	#endregion
}