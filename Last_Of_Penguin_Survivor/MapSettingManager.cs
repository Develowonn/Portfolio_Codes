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

	//public ChunkSync chunkSync;             // ûũ ������ ���� Ŭ����

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
	private Material    mapGroundMaterial;			  // �� �ؽ��� ��Ʋ�� 
	[SerializeField]
	private Material    mapWaterMaterial;             // �� �ؽ��� ��Ʋ�� 

	[Header("[# Abuot Map Size ]")]
	[SerializeField]
	private int		    mapSizeInChunks;

	[Header("[# About Setting Noise ]")]
	[SerializeField]
	private float		scale;                      //  �޸� �������� ũ��(scale)      
	[SerializeField, Range(0, 2000)]
	private int			seed;                       //  ���� �õ�(seed) ��

	[Header("[# About Data ]")]
	[SerializeField]
	private	BlockDataConfig[]		blockDataConfig;          // ���� Ÿ���� �� �����͸� ��� ������, �ؽ��ĸ� �ٸ� 
	[SerializeField]
	private	BlockTextureData[]  blockTextureDataList;     // ���� Ÿ���� �� �����͸� ��� ������, �ؽ��ĸ� �ٸ� 
	[SerializeField]
	private BlockWeightData[]		blockWeightConfig;

	[Header("[# About Water Setting]")]
	[SerializeField]
	private float	  waterChunkObjectYScale;          //���� �ؼ��� ����
    [SerializeField]
    private int		  waterHeight;                     // �� ����

	[Header("[# ETC]")]
	[SerializeField]
	private Transform player;
	[SerializeField]
	private int		  viewDistanceInChunks;

    #region ������Ƽ ( Get ��� )
    // # ������Ƽ ( Get ��� )
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

		// �� ������ ID ���� 
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
    /// ûũ �������� ������ ��ȯ�մϴ�.
    /// </summary>
    public GameObject InstantiateChunk()
	{
		return Instantiate(chunkPrefab);
	}

    /// <summary>
    /// �� ûũ�� ��ġ�� �� ûũ�� ��ġ �� �������� �ʱ�ȭ�մϴ�.
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
	/// �־��� ��ǥ���� ������ ���� ������� �� �����͸� ��ȯ�մϴ�.
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
    /// �־��� �� ������ �迭���� ����ġ�� ������� ������ ���� ��ȯ�մϴ�.
    /// </summary>
    private BlockData GetBlockTextureWeightRandom(BlockData[] blockDatas)
	{
		if (blockDatas.Length == 0)
		{
			return null;
		}

		// ��� ����ġ�� ���Ͽ� ���� ���ϱ�
		float totalWeight = 0.0f;
		foreach (BlockData blockData in blockDatas)
		{
			totalWeight += blockData.weight;
		}

		if (totalWeight == 0.0f)
		{
			return blockDatas[0]; // ù ��° �����͸� ��ȯ
		}

		// 0���� ���� ������ ���� ���� ����
		float randomValue = UnityEngine.Random.value * totalWeight;

		// ���� ���� ��� ������ ���ϴ��� Ȯ���Ͽ� ������ ����
		foreach (var weightedBlockData in blockDatas)
		{
			randomValue -= weightedBlockData.weight;

			if (randomValue < 0.0f)
			{
				return weightedBlockData;
			}
		}

		// �ش���� ���� ��� ������ ������ ��ȯ
		return blockDatas[blockDatas.Length - 1];
	}

	/// <summary>
	/// �� ID�� �ش��ϴ� �� �����͸� �˻��� ��ȯ�մϴ�.
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
    /// �־��� ��� ID�� �ش��ϴ� �ؽ�ó�� ã��, �־��� �ε����� �ش��ϴ� �ؽ�ó�� ��ȯ�մϴ�.
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
