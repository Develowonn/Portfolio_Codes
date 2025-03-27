using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPlate : MonoBehaviour
{
    [Header("Plate Info")]
    public string foodName;
    public int foodComplete;
    public float timer;

    [Header("Plate Cur Material Info")]
    public Material foodMaterial;

    public SpriteRenderer spriteRdr;

    private void Awake()
    {
        spriteRdr = GetComponent<SpriteRenderer>();
    }

    public void Init(string foodName, Sprite foodImg, int foodComplete, float timer, Material material)
    {
        this.foodName = foodName;
        this.spriteRdr.sprite = foodImg;
        this.foodComplete = foodComplete;
        this.timer = timer;
        this.foodMaterial = material;

        // 처음 투명도 세팅
        float maxAlpha = 1.0f; // 최대 투명도
        float minAlpha = 0.2f; // 최소 투명도
        float increment = (maxAlpha - minAlpha) / (FGameManager.instance.currentFood.foodMaterials.Length - 1); // 재료를 획득할 때마다 증가할 투명도 값

        float alpha = minAlpha + increment * FGameManager.instance.materialIndex;
        alpha = Mathf.Clamp(alpha, minAlpha, maxAlpha); // 투명도가 minAlpha와 maxAlpha 사이에 머무르도록 클램핑
        spriteRdr.color = new Color(1f, 1f, 1f, alpha);
    }

    public void MaterialInit(Material material)
    {
        this.foodMaterial = material;
    }
}
