// # System
using System.Collections;
using System.Collections.Generic;

// # Unity
using UnityEngine;

[System.Serializable]
public class Food
{
	public string		foodName;
	public int			foodCompleteness = 100;
	public float		cookingTimeLimit;
	public Sprite		foodSprite;
	public Material[]	materials;
}
