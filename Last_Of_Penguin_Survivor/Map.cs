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

	// ���� �����ӿ� Ȱ��ȭ �Ǿ��� ûũ ���
	private List<Chunk> prevActiveChunkList    = new List<Chunk>();
	// ���� �����ӿ� Ȱ��ȭ�� ûũ ���
	private List<Chunk> currentActiveChunkList = new List<Chunk>();

	// �÷��̾��� ���� ������ ��ġ
	private Vector2  prevPlayerCoord;
	// �÷��̾��� ���� ������ ��ġ
	private Vector2  currentPlayerCoord;

	public Map(MapSettingManager mapSettingManager)
	{
		this.mapSettingManager = mapSettingManager;

		groundChunks = new Chunk[mapSettingManager.MapSizeInChunks, mapSettingManager.MapSizeInChunks];
		waterChunks  = new Chunk[mapSettingManager.MapSizeInChunks, mapSettingManager.MapSizeInChunks];

		GenerateMap();
	}

    /// <summary> �־��� ûũ ��ǥ���� �ش��ϴ� ûũ�� ������Ʈ�մϴ�. </summary>
    public void DrawMap(int x, int z)
	{
		GetChunkFromChunkCoord(x, z, ChunkType.Ground).UpdateChunk();
        GetChunkFromChunkCoord(x, z, ChunkType.Water).UpdateChunk();
    }

    /// <summary> ���� �����ϰ�, ûũ�� �����Ͽ� ǥ���մϴ�. </summary>
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

    /// <summary> �־��� ��ǥ���� ûũ�� �����մϴ�. </summary>
    public void GenerateChunk(int x, int z)
	{
		Chunk groundTempChunk = new Chunk(new Vector2Int(x, z), this, mapSettingManager, ChunkType.Ground);
        Chunk waterTempChunk  = new Chunk(new Vector2Int(x, z), this, mapSettingManager, ChunkType.Water);

		JsonManager.Instance.Save(groundTempChunk); //���� ������ ûũ ����

        groundChunks[x, z]   = groundTempChunk;
		waterChunks[x, z]	 = waterTempChunk;
	}

    /// <summary> �־��� ��ġ�� ûũ�� ������Ʈ�մϴ�. </summary>
    public void UpdateChunk(Vector3 pos)
	{
		if (IsVoxelInMap(pos))
		{
			GetChunkFromPosition(pos, ChunkType.Ground).UpdateChunk();
		}
		else
		{
			Debug.Log("ûũ�� �������� �ʾ� UpdateChunk() ���� �Ұ�");
		}
	}

    /// <summary> �÷��̾��� ��ġ�� Ȯ���ϰ�, ûũ ������Ʈ�� üũ�մϴ�. </summary>
    public void CheckAndUpdateChunks()
	{
		currentPlayerCoord = GetChunkCoordFromPosition(mapSettingManager.Player.position);

		if (!prevPlayerCoord.Equals(currentPlayerCoord))
			UpdateChunkInViewRange();

		prevPlayerCoord = currentPlayerCoord;
	}

    /// <summary> ���� ��ġ�� �ʱ�ȭ�մϴ�. </summary>
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

    /// <summary> ���� �� ���� ������ ûũ�� Ȱ��ȭ/��Ȱ��ȭ�մϴ�. </summary>
    private void UpdateChunkInViewRange()
	{
		Vector2 chunkCoord = GetChunkCoordFromPosition(mapSettingManager.Player.position);
		int viewDist = mapSettingManager.ViewDistanceInChunks;

		(int x, int z) viewMin = ((int)chunkCoord.x - viewDist, (int)chunkCoord.y - viewDist);
		(int x, int z) viewMax = ((int)chunkCoord.x + viewDist, (int)chunkCoord.y + viewDist);

		// Ȱ�� ��� : ���� -> �������� �̵�
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

		// ���������� ���� ûũ�� ��Ȱ��ȭ
		foreach (var chunk in prevActiveChunkList)
		{
			Vector2Int coord = chunk.Coord;
			chunk.IsActive = false;
			waterChunks[coord.x, coord.y].IsActive = false;
		}
	}

    #region ���� ������, �� ������ ���� 
    /// <summary> �־��� ��ġ���� ��� ���̸� ����Ͽ� ��ȯ�մϴ�. </summary>
    public int CalculateBlcokHeight(Vector3 pos)
	{
		return PerlinNoise.GetHeightFromNoise(new Vector2(pos.x, pos.z), mapSettingManager.Scale, mapSettingManager.Seed);
	}

    /// <summary> �־��� ��ġ�� ��� ���̿� ���� �ش� ����� �����͸� ����Ͽ� ��ȯ�մϴ�. </summary>
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

    #region ûũ ��ȸ �� ���� ���� �˻� 
    /// <summary> �־��� ��ġ�� ûũ Ÿ�Կ� �´� ûũ�� ��ȯ�մϴ�. </summary>
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

    /// <summary> �־��� ��ǥ�� ûũ Ÿ�Կ� �´� ûũ�� ��ȯ�մϴ�. </summary>
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

    /// <summary> �־��� ûũ ��ǥ�� ûũ Ÿ�Կ� �´� ûũ�� ��ȯ�մϴ�. </summary>	
    public Chunk GetChunkFromChunkCoord(int x, int z, ChunkType chunkType)
	{
        switch (chunkType)
        {
            case ChunkType.Ground:
				return groundChunks[x, z];
			case ChunkType.Water:
				return waterChunks[x, z];
		}
        Debug.Log($"{x} : {z} ��ǥ�� �ִ� ûũ�� �����ϴ�");
		return null;
	}

    /// <summary> �־��� ��ġ���� ûũ ��ǥ�� ����Ͽ� ��ȯ�մϴ�. </summary>
    public Vector2 GetChunkCoordFromPosition(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x / ChunkData.ChunkWidthValue);
        int z = Mathf.FloorToInt(pos.z / ChunkData.ChunkLengthValue);

        return new Vector2(x, z);
    }

    /// <summary> �־��� ���� ��ġ�� �� ���� �ִ��� Ȯ���Ͽ� ��ȯ�մϴ�. </summary>
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

    /// <summary> �־��� ûũ�� �� ���� �ִ��� Ȯ���Ͽ� ��ȯ�մϴ�. </summary>
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