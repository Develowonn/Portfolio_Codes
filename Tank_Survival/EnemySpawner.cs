// # System
using System.Collections;
using System.Collections.Generic;

// # Unity
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private Transform   tankTransform;
    [SerializeField]
    private GameObject  enemyPrefab;

    [Header("Spawn Settings")]
    [SerializeField]
    private float       spawnCycle;
    [SerializeField]
    private Transform[] spawnPositions;

    private void Start()
    {
        StartCoroutine(Spawn());
    }

    int number = 0;
    private IEnumerator Spawn()
    {
        while (true)
        {
            Transform spawnPosition = spawnPositions[Random.Range(0, spawnPositions.Length)];

            GameObject enemy = Instantiate(enemyPrefab, spawnPosition.position, Quaternion.identity);

            enemy.name = "Enemy" + number;
            enemy.GetComponent<EnemyBase>().Initlize(tankTransform);

            number++;

            yield return new WaitForSeconds(spawnCycle);
        }
    }
}
