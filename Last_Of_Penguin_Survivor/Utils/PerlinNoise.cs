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

		// 펄린 노이즈가 원하는 높이에서 - 1 부족해서 + 1을 더해서 원하는 높이까지 나오게 해줌
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
