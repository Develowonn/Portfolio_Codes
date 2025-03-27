// # System
using System.Collections;
using System.Collections.Generic;

// # Unity
using UnityEngine;

public class PlateSpawner : MonoBehaviour
{
	[SerializeField]
	private Transform    plateLeftSpawnPoint;
	[SerializeField]
	private Transform	 plateRightSpawnPoint;
	[SerializeField]
	private float	     plateMovementSpeed;
	[SerializeField]
	private float		 spawnTime;

	private void Start()
	{
		StartCoroutine(SpawnPlate());
	}

	private IEnumerator SpawnPlate()
	{
		WaitForSeconds spawnTime = new WaitForSeconds(this.spawnTime);

		while(true)
		{
			// 스폰 위치 설정
			bool		   isLeftSpawn = Random.Range(0, 2) == 0 ? true : false;
			Vector3		   spawnPoint  = isLeftSpawn == true ? plateLeftSpawnPoint.position 
														  : plateRightSpawnPoint.position;

			// 접시 소환
			GameObject     clone       = MaterialPlatePoolManager.Instance.GetMaterialPlate();
			Vector3		   movementDir = isLeftSpawn == true ? Vector3.right : Vector3.left;

			// 접시 세팅
			Material material = GameManager.Instance.GetRandomMaterial();
			clone.GetComponent<MaterialPlate>().Initialize(spawnPoint, plateMovementSpeed, movementDir, material);

			yield return spawnTime;
		}
	}
}