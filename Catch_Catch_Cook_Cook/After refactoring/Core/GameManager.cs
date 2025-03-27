// # System
using System.Collections;
using System.Collections.Generic;

// # Unity
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
	[SerializeField]
	private FoodPlate   foodPlate;

	[Header("# Food ScriptableObject")]
	[SerializeField]
	private FoodSO[]	foodScriptableObjects;

	[Header("# Effect Prefab")]
	[SerializeField]
	private GameObject  requiredMaterialEffect;
	[SerializeField]
	private GameObject  successEffect;
	[SerializeField]
	private GameObject  failEffect;

	private Food[]		foods;

	private int			currentFoodIndex;
	private int			currentMaterialIndex;

	public override void Awake()
	{
		base.Awake();

		SetFoodData();
	}

	/// <summary>
	/// 스크립터블 오브젝트에 있는 데이터를 가져와 "foods"에 초기화합니다
	/// </summary>
	private void SetFoodData()
	{
		// 배열 공간 생성
		foods = new Food[foodScriptableObjects.Length];
		
		// 초기화 
		for(int index = 0 ; index < foods.Length; index++)
		{
			foods[index] = new Food();

			foods[index].foodName			= foodScriptableObjects[index].foodName;
			foods[index].cookingTimeLimit   = foodScriptableObjects[index].cookingTimeLimit;
			foods[index].foodSprite			= foodScriptableObjects[index].foodSprite;

			// 재료 초기화
			int materialsArrayLength		= foodScriptableObjects[index].materials.Length;
			foods[index].materials			= new Material[materialsArrayLength];

			for(int index2 = 0; index2 < foods[index].materials.Length; index2++)
			{
				foods[index].materials[index2] = new Material();

				foods[index].materials[index2].materialName      = foodScriptableObjects[index].materials[index2].materialName;
				foods[index].materials[index2].materialSprite    = foodScriptableObjects[index].materials[index2].materialSprite;
				foods[index].materials[index2].materialMaxCount  = foodScriptableObjects[index].materials[index2].materialMaxCount;
				foods[index].materials[index2].materialCount     = foodScriptableObjects[index].materials[index2].materialMaxCount;

				foods[index].materials[index2].positionOffset    = foodScriptableObjects[index].materials[index2].positionOffset;
				foods[index].materials[index2].rotationOffset    = foodScriptableObjects[index].materials[index2].rotationOffset;
				foods[index].materials[index2].scale             = foodScriptableObjects[index].materials[index2].scale;
			}
		}
	}

	/// <summary>
	/// 현재 음식 데이터를 반환합니다
	/// </summary>
	public Food	GetCurrentFood()
	{
		return foods[currentFoodIndex];
	}

	/// <summary>
	/// 현재 음식 재료 중 랜덤으로 재료를 반환합니다
	/// </summary>
	public Material GetRandomMaterial()
	{
		int	randomIndex = Random.Range(0, foods[currentFoodIndex].materials.Length);
		return foods[currentFoodIndex].materials[randomIndex];
	}

	/// <summary>
	/// 현재 음식에 필요한 재료 데이터를 반환합니다
	/// </summary>
	public Material GetCurrentMaterial()
	{
		return foods[currentFoodIndex].materials[currentMaterialIndex];
	}

	/// <summary>
	/// 현재 음식에 필요한 재료의 이름을 반환합니다
	/// </summary>
	public string GetCurrentMaterialName()
	{
		return foods[currentFoodIndex].materials[currentMaterialIndex].materialName;
	}

	/// <summary>
	/// 현재 요리에 필요한 총 재료 개수를 반환합니다
	/// </summary>
	public int GetCurrentFoodTotalMaterialCount()
	{
		int  result		 = 0;
		Food currentFood = foods[currentFoodIndex];

		for(int i = 0 ; i < currentFood.materials.Length; i++)
		{
			result += currentFood.materials[i].materialCount;
		}

		return result;
	}

	/// <summary>
	/// 현재 음식에 필요한 재료를 얻으면 감소시킨다
	/// </summary>
	public void ReduceMaterialCount()
	{
		Food	 currentFood     = foods[currentFoodIndex];
		Material currentMaterial = currentFood.materials[currentMaterialIndex];

		// 재료 감소 
		currentMaterial.materialCount--;

		// 현재 얻어야 할 재료가 "0"일 때
		if(currentMaterial.materialCount <= 0)
		{
			// 더 이상 얻어야 할 재료가 없다면
			if (currentMaterialIndex == currentFood.materials.Length - 1)
			{
				MoveNextFood();
				return;
			}

			currentMaterialIndex++;
		}
	}

	/// <summary>
	/// 다음 요리로 이동한다
	/// </summary>
	public void MoveNextFood()
	{
		MaterialPlatePoolManager.Instance.ReturnAllPlate();

		if(currentFoodIndex == foods.Length - 1)
		{
			// 스테이지 클리어
			return;
		}

		currentFoodIndex++;
		currentMaterialIndex = 0;

		foodPlate.Initilalize();
	}

	#region 이펙트 소환 관련 함수
	/// <summary>
	/// 원하는 위치에 Effect를 생성합니다
	/// </summary>
	public GameObject SpawnEffect(EffectType effectType, Vector3 position)
	{
		GameObject effect;

		switch(effectType)
		{
			case EffectType.SuccessEffect:
				effect = Instantiate(successEffect, position, Quaternion.identity);
				return effect;

			case EffectType.FailEffect:
				effect = Instantiate(failEffect, position, Quaternion.identity);
				return effect;

			case EffectType.RequiredMaterialEffect:
				effect = Instantiate(requiredMaterialEffect, position, Quaternion.identity);
				return effect;

			default:
				Debug.Log("Can't Find Effect");
				return null;
		}
	}

	/// <summary>
	/// 원하는 위치에 Effect를 생성하고 지속시간을 설정합니다
	/// </summary>
	public GameObject SpawnEffect(EffectType effectType, Vector3 position, float duration)
	{
		GameObject effect;

		switch (effectType)
		{
			case EffectType.SuccessEffect:
				effect = Instantiate(successEffect, position, Quaternion.identity);
				Destroy(effect, duration);
				return effect;

			case EffectType.FailEffect:
				effect = Instantiate(failEffect, position, Quaternion.identity);
				Destroy(effect, duration);
				return effect;


			case EffectType.RequiredMaterialEffect:
				effect = Instantiate(requiredMaterialEffect, position, Quaternion.identity);
				Destroy(effect, duration);
				return effect;

			default:
				Debug.Log("Can't Find Effect");
				return null;
		}

	}
	#endregion
}
