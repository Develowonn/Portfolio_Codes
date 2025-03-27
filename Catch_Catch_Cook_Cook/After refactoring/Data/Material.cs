// # System
using System.Collections;
using System.Collections.Generic;

// # Unity
using UnityEngine;

[System.Serializable]
public class Material
{
	public string	materialName;
	public Sprite   materialSprite;
	public int		materialMaxCount;
	public int		materialCount;

	[Header("# Offset")]
	public Vector3  positionOffset;
	public Vector3  rotationOffset;
	public Vector3  scale;
}
