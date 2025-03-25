using System.Collections;

// # Unity
using UnityEngine;

public static class VoxelData
{
	// # 텍스쳐 관련 
	public const int TextureAtlasSizeInBlocksX = 12; // X 방향 블록 수
	public const int TextureAtlasSizeInBlocksY = 14; // Y 방향 블록 수

	// # 메쉬 관련
	public const int FaceCount     = 6;
	public const int VerticesCount = 4;

	public static float NormalizedBlockTextureSizeX
	{
		get { return 1f / (float)TextureAtlasSizeInBlocksX; }
	}

	public static float NormalizedBlockTextureSizeY
	{
		get { return 1f / (float)TextureAtlasSizeInBlocksY; }
	}

	// Y값을 -2로 조정한 VoxelVerts
	public static Vector3[] VoxelVerts = new Vector3[8]
	{
		new Vector3( 0.0f, 0.0f, 0.0f ),
		new Vector3( 1.0f, 0.0f, 0.0f ),
		new Vector3( 1.0f, 1.0f, 0.0f ),
		new Vector3( 0.0f, 1.0f, 0.0f ),
		new Vector3( 0.0f, 0.0f, 1.0f ),
		new Vector3( 1.0f, 0.0f, 1.0f ),
		new Vector3( 1.0f, 1.0f, 1.0f ),
		new Vector3( 0.0f, 1.0f, 1.0f )
	};

	public static int[,] VoxelTris = new int[6, 4]
	{
        // [ Block face order ]
        // Back -> Front -> Top -> Bottom -> Left -> Right

        { 0, 3, 1, 2 },     // Back   Face
        { 5, 6, 4, 7 },     // Front  Face
        { 3, 7, 2, 6 },     // Top    Face
        { 1, 5, 0, 4 },     // Bottom Face
        { 4, 7 ,0, 3 },     // Left   Face
        { 1, 2, 5, 6 }      // Right  Face
    };

	public static readonly Vector2[] VoxelUvs = new Vector2[4]
	{
		new Vector2( 0.0f, 0.0f ),
		new Vector2( 0.0f, 1.0f ),
		new Vector2( 1.0f, 0.0f ),
		new Vector2( 1.0f, 1.0f )
	};

	public static readonly Vector3[] FaceChecks = new Vector3[6]
	{
		new Vector3( 0.0f,  0.0f, -1.0f ),  // Back   Face
        new Vector3( 0.0f,  0.0f,  1.0f ),  // Front  Face
        new Vector3( 0.0f,  1.0f,  0.0f ),  // Top    Face (Relative to -2, now it's -1)
        new Vector3( 0.0f, -1.0f,  0.0f ),  // Bottom Face (Relative to -2, now it's -3)
        new Vector3(-1.0f,  0.0f,  0.0f ),  // Left   Face
        new Vector3( 1.0f,  0.0f,  0.0f )   // Right  Face
    };
}