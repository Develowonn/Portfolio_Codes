// # System
using System.Collections;
using System.Collections.Generic;

// # Unity
using UnityEngine;
using UnityEngine.LightTransport;
using UnityEngine.SocialPlatforms;
using static BlockConstants;

[System.Serializable]
public class Chunk
{
	// #  청크 공유를 위한 클래스
	//public ChunkSync chunkSync;

	// # 청크 좌표 
	private Vector2Int		coord;

	// # 청크 타입
	private ChunkType		chunkType;

	// # 메쉬 오브젝트 관련
	public  GameObject		chunkObject;
	private MeshRenderer		meshRenderer;
	private MeshFilter		meshFilter;
	private MeshCollider		meshCollider;
	private Material		material;

	// # 메쉬 데이터 관련
	private int			vertexIndex		  = 0;
	private List<Vector3>		vertices		  = new List<Vector3>();
	private List<int>	 	triangles		  = new List<int>();
	private List<Vector2>		uvs		          = new List<Vector2>();

	private Map			map		          = null;
	private MapSettingManager	mapSettingManager         = null;

	private BlockData[,,]		blockInChunk              = new BlockData[ChunkData.ChunkWidthValue, ChunkData.ChunkHeightValue, ChunkData.ChunkLengthValue];
	private int[,]		        blockHeight		  = new int[ChunkData.ChunkWidthValue, ChunkData.ChunkLengthValue];

	#region 프로퍼티 ( Get 기능 )
	public Vector2Int	        Coord                     => coord;
	public BlockData[,,]            BlockInChunk              => blockInChunk;
	#endregion

	public Chunk(Vector2Int coord, Map map, MapSettingManager mapSettingManager, ChunkType chunkType)
	{
		this.coord			   = coord;

		this.map			   = map;
		this.mapSettingManager = mapSettingManager;

		this.chunkObject	 = mapSettingManager.InstantiateChunk();
		this.meshFilter		 = chunkObject.GetComponent<MeshFilter>();
		this.meshRenderer	 = chunkObject.GetComponent<MeshRenderer>();
		this.meshCollider	 = chunkObject.GetComponent<MeshCollider>();

		this.chunkType		 = chunkType;
		this.chunkObject.tag = "Ground";

		chunkObject.transform.localPosition = new Vector3(coord.x * ChunkData.ChunkWidthValue, 0.0f, coord.y * ChunkData.ChunkLengthValue);
		System.Text.StringBuilder sb		= new System.Text.StringBuilder();
		sb.Append("Chunk "); sb.Append(coord.x); sb.Append(","); sb.Append(coord.y);
		chunkObject.name			        = sb.ToString();

		switch (chunkType)
		{
			case ChunkType.Ground:
				chunkObject.transform.SetParent(mapSettingManager.GroundChunkParent);

				meshRenderer.material = mapSettingManager.MapGroundMaterial;
				material		      = mapSettingManager.MapGroundMaterial;
				break;
			case ChunkType.Water:
				chunkObject.transform.SetParent(mapSettingManager.WaterChunkParent);

				meshRenderer.material = mapSettingManager.MapWaterMaterial;
				material		      = mapSettingManager.MapWaterMaterial;
				break;
		}

		PopulateBlockHeight();
		PopulateChunkBlock();
		UpdateChunk();
	}

	///<summary>현재 청크의 월드 공간 위치를 가져옵니다.</summary>
	public Vector3 Position
	{
		get { return chunkObject.transform.localPosition; }
	}

	public bool IsActive
	{
		get => chunkObject.activeSelf;
		set => chunkObject.SetActive(value);
	}

	private Vector3 ToWorldPos(in Vector3 pos)      => Position + pos;
    private Vector3 ToWorldPos(int x, int y, int z) => Position + new Vector3(x, y, z);

	private void PopulateBlockHeight()
	{
		for(int x = 0; x < ChunkData.ChunkWidthValue; x++)
		{
			for(int z = 0; z < ChunkData.ChunkLengthValue; z++)
			{
				blockHeight[x, z] = map.CalculateBlcokHeight(ToWorldPos(x, 0, z));
            }
		}
	}

	private void PopulateChunkBlock()
	{
        for (int y = 0; y < ChunkData.ChunkHeightValue; y++)
        {
            for (int x = 0; x < ChunkData.ChunkWidthValue; x++)
            {
                for (int z = 0; z < ChunkData.ChunkLengthValue; z++)
                {
                    blockInChunk[x, y, z] = map.CalculateBlockData(ToWorldPos(x, y, z), blockHeight[x, z]);
                }
            }
        }
    }

    ///<summary>지정된 위치의 복셀 데이터를 새로운 데이터로 변경합니다.</summary>
    public void EditVoxel(Vector3 pos, BlockData newBlockData)
    {
		Debug.Log($"블럭파괴! : {pos}, {newBlockData}");

        int globalX = Mathf.FloorToInt(pos.x);
        int globalY = Mathf.FloorToInt(pos.y);
        int globalZ = Mathf.FloorToInt(pos.z);

        int localX = globalX - Mathf.FloorToInt(chunkObject.transform.position.x);
        int localZ = globalZ - Mathf.FloorToInt(chunkObject.transform.position.z);

        blockInChunk[localX, globalY, localZ] = newBlockData;

        UpdateChunk();
        UpdateSurroundingVoxels(localX, globalY, localZ);

        //chunkSync?.EditChunk(coord, this);
    }

    #region 청크 메시 생성 및 갱신
    ///<summary>주변 청크의 메시를 갱신합니다.</summary>
    private void UpdateSurroundingVoxels(int x, int y, int z)
    {
        Vector3 thisVoxel = new Vector3(x, y, z);

        for (int p = 0; p < 6; p++)
        {
            Vector3 currentVoxel = thisVoxel + VoxelData.FaceChecks[p];

            if (!IsVoxelInChunk((int)currentVoxel.x, (int)currentVoxel.y, (int)currentVoxel.z))
            {
                Vector3 pos = currentVoxel + Position;
                if (!map.IsVoxelInMap(pos)) return;

                Chunk tempChunk = map.GetChunkFromPosition(currentVoxel + Position, ChunkType.Ground);
                tempChunk.UpdateChunk();
            }
        }
    }

    ///<summary>청크 데이터를 갱신하고 메시를 생성합니다.</summary>
    public void UpdateChunk()
	{
		ClearMeshData();

		switch (chunkType)
		{
			case ChunkType.Ground:
				for (int y = 0; y < ChunkData.ChunkHeightValue; y++)
				{
					for (int x = 0; x < ChunkData.ChunkWidthValue; x++)
					{
						for (int z = 0; z < ChunkData.ChunkLengthValue; z++)
						{
							if (blockInChunk[x, y, z].isSoild)
							{
								UpdateMeshData(new Vector3(x, y, z));
							}
						}
					}
				}
				break;
			case ChunkType.Water:
				for (int y = 0; y < ChunkData.ChunkHeightValue; y++)
				{
					for (int x = 0; x < ChunkData.ChunkWidthValue; x++)
					{
						for (int z = 0; z < ChunkData.ChunkLengthValue; z++)
						{
							if (blockInChunk[x, y, z].id == BlockConstants.Water)
							{
								UpdateMeshData(new Vector3(x, y, z));
							}
						}
					}
				}
				break;
		}

		CreateMesh();
	}

	///<summary>메시에 필요한 데이터를 초기화합니다.</summary>
	public void ClearMeshData()
	{
		vertexIndex = 0;

		vertices.Clear();
		triangles.Clear();
		uvs.Clear();
	}

	///<summary>현재 청크의 메시를 생성하고 충돌 메시를 설정합니다.</summary>
	public void CreateMesh()
	{
		Mesh mesh = new Mesh
		{
			vertices = vertices.ToArray(),
			triangles = triangles.ToArray(),
			uv = uvs.ToArray()
		};

		mesh.RecalculateNormals();
		mesh.Optimize();
		meshFilter.mesh = mesh;

		// 충돌 메시 설정
		meshCollider.sharedMesh = null;
		Mesh collisionMesh = new Mesh
		{
			vertices = vertices.ToArray(),
			triangles = triangles.ToArray()
		};

		collisionMesh.RecalculateNormals();
		meshCollider.sharedMesh = collisionMesh;
	}

	///<summary>지정된 위치의 복셀 데이터를 메시에 추가합니다.</summary>
	private void UpdateMeshData(Vector3 pos)
	{
		for (int p = 0; p < VoxelData.FaceCount; p++)
		{
			if (IsVoxelSolid(pos + VoxelData.FaceChecks[p]))
				continue;

			for (int i = 0; i < VoxelData.VerticesCount; i++)
			{
				vertices.Add(pos + VoxelData.VoxelVerts[VoxelData.VoxelTris[p, i]]);
			}

			AddTexture(blockInChunk[(int)pos.x, (int)pos.y, (int)pos.z].GetBlockTexutreID(p), blockInChunk[(int)pos.x, (int)pos.y, (int)pos.z].rotation);

            // 복셀 면을 그리기 위한 삼각형 정보를 저장
            triangles.Add(vertexIndex + 0);
			triangles.Add(vertexIndex + 1);
			triangles.Add(vertexIndex + 2);

			triangles.Add(vertexIndex + 2);
			triangles.Add(vertexIndex + 1);
			triangles.Add(vertexIndex + 3);

            // 1면이 추가될 때마다 4개의 정점이 필요하기 때문에 4를 더함
            vertexIndex += 4;
		}
	}

	///<summary>지정된 텍스처 ID와 회전을 기반으로 UV 좌표를 추가합니다.</summary>
	private void AddTexture(int textureID, int rotation = 0)
	{
		const float uvXBeginOffset = 0.003f;
		const float uvXEndOffset = 0.003f;
		const float uvYBeginOffset = 0.003f;
		const float uvYEndOffset = 0.003f;


        // textureID에 따라 텍스처의 위치를 계산합니다.
        float y = textureID / VoxelData.TextureAtlasSizeInBlocksX;
		float x = textureID % VoxelData.TextureAtlasSizeInBlocksX;

		x *= VoxelData.NormalizedBlockTextureSizeX;
		y *= VoxelData.NormalizedBlockTextureSizeY;

		y = 1f - y - VoxelData.NormalizedBlockTextureSizeY;

        // 텍스처의 4개의 꼭짓점에 대한 UV 좌표를 계산합니다.
        Vector2[] unprocessedUVs = new Vector2[]
		{
		new(x + uvXBeginOffset, y + uvYBeginOffset),
		new(x + uvXBeginOffset, y + VoxelData.NormalizedBlockTextureSizeY - uvYEndOffset),
		new(x + VoxelData.NormalizedBlockTextureSizeX - uvXEndOffset, y + uvYBeginOffset),
		new(x + VoxelData.NormalizedBlockTextureSizeX - uvXEndOffset, y + VoxelData.NormalizedBlockTextureSizeY - uvYEndOffset)
		};

		Vector2[] finalUVs = (Vector2[])unprocessedUVs.Clone();


        // 회전 값에 따라 UV 좌표를 회전시킵니다.
        switch (rotation)
		{
			case 1:
				finalUVs[0] = unprocessedUVs[1];
				finalUVs[1] = unprocessedUVs[3];
				finalUVs[2] = unprocessedUVs[0];
				finalUVs[3] = unprocessedUVs[2];
				break;
			case 2:
				finalUVs[0] = unprocessedUVs[3];
				finalUVs[1] = unprocessedUVs[2];
				finalUVs[2] = unprocessedUVs[1];
				finalUVs[3] = unprocessedUVs[0];
				break;
			case 3:
				finalUVs[0] = unprocessedUVs[2];
				finalUVs[1] = unprocessedUVs[0];
				finalUVs[2] = unprocessedUVs[3];
				finalUVs[3] = unprocessedUVs[1];
				break;
		}

		foreach (Vector2 uv in finalUVs)
		{
			uvs.Add(uv);
		}
	}
    #endregion

    #region 조건에 적합한지 검사하는 함수
    /// <summary>
    /// 복셀이 청크 내에 있는지 확인합니다.
    /// </summary>
    private bool IsVoxelInChunk(int x, int y, int z)
	{
		if (x < 0 || x > ChunkData.ChunkWidthValue - 1
		 || y < 0 || y > ChunkData.ChunkHeightValue - 1
		 || z < 0 || z > ChunkData.ChunkLengthValue - 1)
			return false;
		else
			return true;
	}

	/// <summary>
	/// 지정된 위치의 복셀이 청크 내에 있는지 확인하고, 해당 복셀이 솔리드한지 검사합니다.
	/// </summary>
	private bool IsVoxelSolid(Vector3 pos)
	{
		int x = Mathf.FloorToInt(pos.x);
		int y = Mathf.FloorToInt(pos.y);
		int z = Mathf.FloorToInt(pos.z);

		if (chunkType == ChunkType.Ground)
		{
			// 청크 내에 있는지 확인
			if (!IsVoxelInChunk(x, y, z))
			{
				Vector3 globalPos = new Vector3(x, y, z) + Position;

				if (!map.IsVoxelInMap(globalPos))
					return false;

                int localX  = Mathf.FloorToInt(globalPos.x) % ChunkData.ChunkWidthValue;
                int localY  = Mathf.FloorToInt(globalPos.y);
                int localZ  = Mathf.FloorToInt(globalPos.z) % ChunkData.ChunkLengthValue;
				Chunk chunk = map.GetChunkFromPosition(globalPos, ChunkType.Ground);

				if (chunk == null) return false;
                return chunk.BlockInChunk[localX, localY, localZ].isSoild;
			}

			return blockInChunk[x, y, z].id != BlockConstants.Air && blockInChunk[x, y, z].isSoild;
		}
		else
		{
			// 청크 내에 있는지 확인
			if (!IsVoxelInChunk(x, y, z))
			{
				Vector3 globalPos = new Vector3(x, y, z) + Position;

				if (!map.IsVoxelInMap(globalPos))
                    return false;

                int localX  = Mathf.FloorToInt(globalPos.x) % ChunkData.ChunkWidthValue;
                int localY  = Mathf.FloorToInt(globalPos.y);
                int localZ  = Mathf.FloorToInt(globalPos.z) % ChunkData.ChunkLengthValue;
                Chunk chunk = map.GetChunkFromPosition(globalPos, ChunkType.Ground);

				if (chunk == null) return false;

                return map.GetChunkFromPosition(globalPos, ChunkType.Ground).BlockInChunk[localX, localY, localZ].isSoild;
            }
            return blockInChunk[x, y, z].id != BlockConstants.Air && !blockInChunk[x, y, z].isSoild;
        }
    }
    #endregion
}
