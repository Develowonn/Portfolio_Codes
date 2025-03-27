using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Food
{
    public string foodName;
    public Sprite foodSprite;
    public int foodComplete;
    public float timer;

    [Space(10)] public Material[] foodMaterials;
}
