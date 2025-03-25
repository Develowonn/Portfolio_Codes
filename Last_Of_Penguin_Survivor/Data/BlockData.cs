using Newtonsoft.Json;

// # Unity
using UnityEngine;

[System.Serializable]
public class BlockData
{
	public BlockData(BlockData blockData)
	{
		id           = blockData.id;
		weight       = blockData.weight;
		isSoild      = blockData.isSoild;
		isDestroy    = blockData.isDestroy;
		isDisposable = blockData.isDisposable;

		backFaceTexture   = blockData.backFaceTexture;
		frontFaceTexture  = blockData.frontFaceTexture;
		topFaceTexture    = blockData.topFaceTexture;
		bottomFaceTexture = blockData.bottomFaceTexture;
		leftFaceTexture   = blockData.leftFaceTexture;
		rightFaceTexture  = blockData.rightFaceTexture;
		rotation          = blockData.rotation;

		level             = blockData.level;
	}


	[HideInInspector]
	public string id;                 // ���� ���� ����
	public float weight;             // ����ġ

	[Header("Bool Setting")]
	public bool isSoild;
	public bool isDestroy;            // �� �ı� ���� ���� 
	public bool isGgongGgongActivate; // �ǲ�����ġ Ȱ��ȭ ����
	public bool isDisposable;         // ��ȸ�� ����

	[Header("Texture Values")]
	public string backFaceTexture;
	public string frontFaceTexture;
	public string topFaceTexture;
	public string bottomFaceTexture;
	public string leftFaceTexture;
	public string rightFaceTexture;
	public int rotation;

	public int level;

	public int GetBlockTexutreID(int id)
	{
		switch (id)
		{
			case 0:
				return MapSettingManager.Instance.FindTexture(backFaceTexture, level);
			case 1:
				return MapSettingManager.Instance.FindTexture(frontFaceTexture, level);
			case 2:
				return MapSettingManager.Instance.FindTexture(topFaceTexture, level);
			case 3:
				return MapSettingManager.Instance.FindTexture(bottomFaceTexture, level);
			case 4:
				return MapSettingManager.Instance.FindTexture(leftFaceTexture, level);
			case 5:
				return MapSettingManager.Instance.FindTexture(rightFaceTexture, level);
			default:
				return 0;
		}
	}

	public void SetBlockTextureID(BlockSurfaceType blockSurfaceType, string id)
	{
		switch (blockSurfaceType)
		{
			case BlockSurfaceType.Front:
				frontFaceTexture = id;
				break;
			case BlockSurfaceType.Back:
				backFaceTexture = id;
				break;
			case BlockSurfaceType.Top:
				topFaceTexture = id;
				break;
			case BlockSurfaceType.Bottom:
				bottomFaceTexture = id;
				break;
			case BlockSurfaceType.Left:
				leftFaceTexture = id;
				break;
			case BlockSurfaceType.Right:
				rightFaceTexture = id;
				break;
			default:
				Debug.LogError("SetBlockTextureID Function Error");
				break;
		}
	}
}