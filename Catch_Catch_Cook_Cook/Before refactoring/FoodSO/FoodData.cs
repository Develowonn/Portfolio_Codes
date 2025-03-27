using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FoodData", menuName = "Scriptable Object/FoodData")]
public class FoodData : ScriptableObject
{
    public string foodName;
    public Sprite foodSprite;
    public int foodComplete;
    public float timer;

    [Space(10)] public Material[] foodMaterials;
}
