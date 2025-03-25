// # System
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


// # Unity
using UnityEngine;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.Rendering;
using UnityEngine.UIElements;



// # Static
using static BlockConstants;

public class MapSettingManager : MonoBehaviour
{
	public static MapSettingManager Instance { get; private set; }

	//public ChunkSync chunkSync;             // 청크 공유를 위한 클래스

	private Map map;

	[Header("[# Abuot Chunk Parents ]")]
	[SerializeField]
	private Transform	waterChunkParent;
	[SerializeField]
	private Transform	groundChunkParent;
	[SerializeField]
	public float		parentChunkYPos = -1f;

	[Header("[# Abuot Creating Map]")]
	[SerializeField]
	private GameObject  chunkPrefab;
	[SerializeField]
	private Material    mapGroundMaterial;			  // 맵 텍스쳐 아틀라스 
	[SerializeField]
	private Material    mapWaterMaterial;             // 맵 텍스쳐 아틀라스 

	[Header("[# Abuot Map Size ]")]
	[SerializeField]
	private int		    mapSizeInChunks;

	[Header("[# About Setting Noise ]")]
	[SerializeField]
	private float		scale;                      //  펄린 노이즈의 크기(scale)      
	[SerializeField, Range(0, 2000)]
	private int			seed;                       //  랜덤 시드(seed) 값

	[Header("[# About Data ]")]
	[SerializeField]
	private	BlockDataConfig[]		blockDataConfig;          // 같은 타입의 블럭 데이터를 담고 있지만, 텍스쳐만 다름 
	[SerializeField]
	private	BlockTextureData[]  blockTextureDataList;     // 같은 타입의 블럭 데이터를 담고 있지만, 텍스쳐만 다름 
	[SerializeField]
	private BlockWeightData[]		blockWeightConfig;

	[Header("[# About Water Setting]")]
	[SerializeField]
	private float	  waterChunkObjectYScale;          //물의 해수면 높이
    [SerializeField]
    private int		  waterHeight;                     // 물 높이

	[Header("[# ETC]")]
	[SerializeField]
	private Transform player;
	[SerializeField]
	private int		  viewDistanceInChunks;

    #region 프로퍼티 ( Get 기능 )
    // # 프로퍼티 ( Get 기능 )
    public Map		 Map => map;

	public Transform WaterChunkParent => waterChunkParent;
	public Transform GroundChunkParent => groundChunkParent;

	public Material  MapGroundMaterial => mapGroundMaterial;
	public Material  MapWaterMaterial => mapWaterMaterial;

	public int		 MapSizeInChunks => mapSizeInChunks;

	public int		 WaterHeight => waterHeight;

	public float	 Scale => scale;
	public int		 Seed => seed;

	public float	 ParentChunkYPos        => parentChunkYPos;
	public float	 WaterChunkObjectYScale => waterChunkObjectYScale;
	public int		 ViewDistanceInChunks   => viewDistanceInChunks;

	public Transform Player					=> player;


    public BlockDataConfig[] BlockDataConfig => blockDataConfig;
    #endregion

    private void Awake()
	{
		Instance = this;

		// 블럭 데이터 ID 설정 
		foreach (BlockDataConfig blockDataList in blockDataConfig)
		{
			for (int index = 0; index < blockDataList.blockDatas.Length; index++)
			{
				blockDataList.blockDatas[index].id = blockDataList.id;
			}
		}

		map = new Map(this);
	}

	private void Start()
	{
		map.InitializeSpawnPosition();
		InitializeChunk();
	}

    private void Update()
    {
		map.CheckAndUpdateChunks();
	}

    /// <summary>
    /// 청크 프리팹을 생성해 반환합니다.
    /// </summary>
    public GameObject InstantiateChunk()
	{
		return Instantiate(chunkPrefab);
	}

    /// <summary>
    /// 땅 청크의 위치와 물 청크의 위치 및 스케일을 초기화합니다.
    /// </summary>
    private void InitializeChunk()
	{
        Vector3 mapPos = Vector3.up * parentChunkYPos;
        GroundChunkParent.transform.position = mapPos;

        Vector3 mapPos1 = Vector3.up * parentChunkYPos;
        WaterChunkParent.transform.position = mapPos1;
        WaterChunkParent.transform.localScale = new Vector3(1, WaterChunkObjectYScale, 1);
    }

	/// <summary>
	/// 주어진 좌표에서 노이즈 값을 기반으로 블럭 데이터를 반환합니다.
	/// </summary>
	public BlockData GetBlockTypeWithNoise(Vector2 coord)
	{
		int x = Mathf.FloorToInt(coord.x);
		int z = Mathf.FloorToInt(coord.y);

        float maxAmplitude = blockWeightConfig.Max(config => config.threshold) + 1;
        float noiseValue   = PerlinNoise.GetBlockFromNoise(new Vector2(x, z), maxAmplitude, scale, seed);

        foreach (var config in blockWeightConfig)
        {
            if (noiseValue < config.threshold)
            {
                return FindBlockType(config.id);
            }
        }

		return FindBlockType(Snow);
    }

    /// <summary>
    /// 주어진 블럭 데이터 배열에서 가중치를 기반으로 랜덤한 블럭을 반환합니다.
    /// </summary>
    private BlockData GetBlockTextureWeightRandom(BlockData[] blockDatas)
	{
		if (blockDatas.Length == 0)
		{
			return null;
		}

		// 모든 가중치를 더하여 총합 구하기
		float totalWeight = 0.0f;
		foreach (BlockData blockData in blockDatas)
		{
			totalWeight += blockData.weight;
		}

		if (totalWeight == 0.0f)
		{
			return blockDatas[0]; // 첫 번째 데이터를 반환
		}

		// 0부터 총합 사이의 랜덤 값을 생성
		float randomValue = UnityEngine.Random.value * totalWeight;

		// 랜덤 값이 어느 범위에 속하는지 확인하여 데이터 선택
		foreach (var weightedBlockData in blockDatas)
		{
			randomValue -= weightedBlockData.weight;

			if (randomValue < 0.0f)
			{
				return weightedBlockData;
			}
		}

		// 해당되지 않은 경우 마지막 데이터 반환
		return blockDatas[blockDatas.Length - 1];
	}

	/// <summary>
	/// 블럭 ID에 해당하는 블럭 데이터를 검색해 반환합니다.
	/// </summary>
	public BlockData FindBlockType(string blockID)
	{
		foreach (var blockDataInList in blockDataConfig)
		{
			if (blockDataInList.id == blockID)
			{
				BlockData selecetedBlockData = new BlockData(GetBlockTextureWeightRandom(blockDataInList.blockDatas));

				return selecetedBlockData;
			}
		}
		return new BlockData(FindBlockType(Air));
	}

    /// <summary>
    /// 주어진 블록 ID에 해당하는 텍스처를 찾고, 주어진 인덱스에 해당하는 텍스처를 반환합니다.
    /// </summary>
    public int FindTexture(string blockID, int index = 0)
	{
		foreach (var textureinlist in blockTextureDataList)
		{
			if (textureinlist.id == blockID)
			{
				return textureinlist.blockTextures[Mathf.Min(index, textureinlist.blockTextures.Length - 1)];
			}
		}
		return 0;
	}
}
