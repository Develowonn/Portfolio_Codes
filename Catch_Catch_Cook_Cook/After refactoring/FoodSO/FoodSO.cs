// # System
using System.Collections;
using System.Collections.Generic;

// # Unity
using UnityEngine;

[CreateAssetMenu(fileName = "FoodSO", menuName = "SO/FoodSO")]
public class FoodSO : ScriptableObject
{
	public string	  foodName;
	public float	  cookingTimeLimit;
	public Sprite	  foodSprite;
	public Material[] materials;
}