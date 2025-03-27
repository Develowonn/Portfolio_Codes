// # System
using System.Collections;
using System.Collections.Generic;

// # Unity
using UnityEngine;

public class MaterialPlate : MonoBehaviour
{
	[SerializeField]
	private GameObject	material;
	private string		materialName;		
	private GameObject  effectObject;

	private float		movementSpeed;
	private Vector3		movementDirection;

	private void OnDisable()
	{
		if(effectObject != null)
		{
			Destroy(effectObject);
		}
	}

	private void Update()
	{
		if(effectObject != null && materialName != GameManager.Instance.GetCurrentMaterialName())
		{
			Destroy(effectObject);
		}	
		
		if(effectObject == null && materialName == GameManager.Instance.GetCurrentMaterialName())
		{
			effectObject = GameManager.Instance.SpawnEffect(EffectType.RequiredMaterialEffect, transform.position);
			effectObject.transform.SetParent(transform);
		}

		MoveTo();
	}

	public void Initialize(Vector3 spawnPoint, float movementSpeed, Vector3 movementDirection, Material material)
	{
		transform.position     = spawnPoint;
		this.movementSpeed     = movementSpeed;
		this.movementDirection = movementDirection;

		// 재료 설정 
		this.materialName	   = material.materialName;
		this.material.GetComponent<SpriteRenderer>().sprite = material.materialSprite;
		
		// 재료 오프셋 설정
		this.material.transform.position  += material.positionOffset;
		this.material.transform.rotation   = Quaternion.Euler(this.material.transform.rotation.eulerAngles + 
													material.rotationOffset);
		//this.material.transform.localScale = material.scale == Vector3.zero ? Vector3.one : material.scale; 

		// 메인 요리에 필요한 재료일 때 파티클 생성
		if (materialName == GameManager.Instance.GetCurrentMaterialName())
		{
			effectObject = GameManager.Instance.SpawnEffect(EffectType.RequiredMaterialEffect, transform.position);
			effectObject.transform.SetParent(transform);
		}
	}

	private void MoveTo()
	{
		transform.position += movementSpeed * movementDirection * Time.deltaTime;
	}

	private void OnTriggerEnter2D(Collider2D trigger)
	{
		if (trigger.CompareTag(Constants.MainPlateTag))
		{
			// 요리에 필요한 재료일 때
			if(materialName == GameManager.Instance.GetCurrentMaterialName())
			{
				GameManager.Instance.ReduceMaterialCount();

				// 이펙트 처리
				GameManager.Instance.SpawnEffect(EffectType.SuccessEffect, trigger.transform.position, 1.0f);

				// 메인 요리 투명도 처리
				trigger.GetComponent<FoodPlate>().UpdateAlpha();

				// 얻은 재료 개수 추가
				trigger.GetComponent<FoodPlate>().AddMaterialCount();

			}
			// 요리에 필요한 재료가 아닐 때
			else
			{
				// 이펙트 처리
				GameManager.Instance.SpawnEffect(EffectType.FailEffect, trigger.transform.position, 1.0f);
			}

			MaterialPlatePoolManager.Instance.ReturnMaterialPlate(gameObject);
		}
	}
}
