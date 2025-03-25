// # Unity
using UnityEngine;

public static class PerlinNoise
{
	// # Perlin Noise For Height
	public static int GetHeightFromNoise(Vector2 coord, float scale, int seed)
	{
		// # Seed
		System.Random prng = new System.Random(seed);
		float offsetX = prng.Next(0, 100000);
		float offsetZ = prng.Next(0, 100000);

		scale = Mathf.Max(scale, 0.001f);

		float xCoord = coord.x / scale + offsetX;
		float zCoord = coord.y / scale + offsetZ;

		// �޸� ����� ���ϴ� ���̿��� - 1 �����ؼ� + 1�� ���ؼ� ���ϴ� ���̱��� ������ ����
		int height = Mathf.RoundToInt(Mathf.PerlinNoise(xCoord, zCoord) * (ChunkData.ChunkInitHeightValue + 1));

		return Mathf.Max(1, height);
	}

	// # Perlin Noise For Block Type
	public static float GetBlockFromNoise(Vector2 coord, float amplitude, float scale, int seed)
	{
		// # Seed
		System.Random prng = new System.Random(seed);
		float offsetX = prng.Next(-100000, 100000);
		float offsetZ = prng.Next(-100000, 100000);

		scale = Mathf.Max(scale, 0.001f);

		float xCoord = coord.x / scale + offsetX;
		float zCoord = coord.y / scale + offsetZ;

		return Mathf.PerlinNoise(xCoord, zCoord) * (amplitude + 0.1f);
	}
}
