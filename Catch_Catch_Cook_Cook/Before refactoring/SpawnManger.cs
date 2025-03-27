using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManger : MonoBehaviour
{
    // Index 0 : Left | Index 1 : Right
    [SerializeField] private Transform[] spawnPoint;
    [SerializeField] private GameObject spawnPrefab;
    [SerializeField] private float spawnTime;

    [Space(5)]
    [SerializeField] private int definiteSpawnCount = 0; // 필요한 재료 확정 소환 숫자
    [SerializeField] private int curDefiniteSpawnCount = 0; // 필요한 재료 확정 소환 숫자

    private WaitForSeconds waitForSeconds;

    private void Awake()
    {
        waitForSeconds = new WaitForSeconds(spawnTime);
        definiteSpawnCount = Random.Range(2, 4);
    }

    private void Start()
    {
        StartCoroutine(Spawner());
    }

    private IEnumerator Spawner()
    {
        while (!GameManager.instance.isGameEnd) 
        {
            if(!GameManager.instance.isFoodComplete)
            {
                if(!GameManager.instance.isPause)
                {
                    int rSpawnIndex = Random.Range(0, spawnPoint.Length);
                    GameObject clone = Instantiate(spawnPrefab, spawnPoint[rSpawnIndex].position, Quaternion.identity);
                    clone.transform.SetParent(GameManager.instance.plateSpawnParent);
                    SubPlate subPlate = clone.GetComponent<SubPlate>();

                    // Move DirVec Init
                    if (rSpawnIndex == 0) subPlate.MoveDirVec(Vector3.right);
                    else subPlate.MoveDirVec(Vector3.left);

                    // Plate Info Init
                    if (curDefiniteSpawnCount >= definiteSpawnCount)
                    {
                        curDefiniteSpawnCount = 0;
                        definiteSpawnCount = Random.Range(2, 4);
                        var material = FGameManager.instance.currentFood.foodMaterials[FGameManager.instance.materialIndex];
                        subPlate.Init(material.materialName, material.materialSprite);
                    }
                    else
                    {
                        curDefiniteSpawnCount++;
                        int ranMaterial = Random.Range(0, FGameManager.instance.currentFood.foodMaterials.Length);
                        var material = FGameManager.instance.currentFood.foodMaterials[ranMaterial];

                        // 콩나물이 등장하면 천장 초기화
                        if (material.materialName == FGameManager.instance.currentFood.foodMaterials[FGameManager.instance.materialIndex].materialName)
                            curDefiniteSpawnCount = 0;
                        subPlate.Init(material.materialName, material.materialSprite);
                    }

                    // Particle 
                    if (subPlate.materialName == FGameManager.instance.currentFood.foodMaterials[FGameManager.instance.materialIndex].materialName)
                    {
                        GameObject cloneParticle = Instantiate(GameManager.instance.sparkleParticle, subPlate.particleSpawnPos.position, Quaternion.identity);
                        cloneParticle.transform.SetParent(clone.transform);
                        subPlate.particle = cloneParticle;
                    }
                }
            }

            yield return waitForSeconds;
        }
    }
}
