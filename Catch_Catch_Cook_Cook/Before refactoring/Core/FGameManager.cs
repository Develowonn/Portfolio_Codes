using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class FGameManager : MonoBehaviour
{
    public static FGameManager instance { get; private set; }

    [SerializeField] private FoodData[] foodDatas;

    [Header("[ Game Data ] - No Data Modify!!")]
    [Space(5)]
    public Food currentFood;
    [SerializeField] private Food[] foods;

    [Space(5)]
    public int foodIndex = 0;
    public int materialIndex = 0;
    public MainPlate mainPlate;


    private void Awake()
    {
        instance = this;

        // Foods Array Size Init
        foods = new Food[foodDatas.Length];
    }

    private void Start()
    {
        for (int index = 0; index < foodDatas.Length; index++)
            C_FoodInit(index);

        FoodInit();
    }

    private void Update()
    {
        if (GameManager.instance.isPause) return;
        Timer();
    }

    #region # Array Init Functions
    private void C_FoodInit(int index)
    {
        foods[index] = new Food();
        foods[index].foodName = foodDatas[index].foodName;
        foods[index].foodSprite = foodDatas[index].foodSprite;
        foods[index].foodComplete = foodDatas[index].foodComplete;
        foods[index].timer = foodDatas[index].timer;

        MaterialInit(index);
    }

    private void MaterialInit(int index)
    {
        foods[index].foodMaterials = new Material[foodDatas[index].foodMaterials.Length];

        for (int index2 = 0; index2 < foods[index].foodMaterials.Length; index2++)
        {
            foods[index].foodMaterials[index2] = new Material();
            foods[index].foodMaterials[index2].materialName = foodDatas[index].foodMaterials[index2].materialName;
            foods[index].foodMaterials[index2].materialSprite = foodDatas[index].foodMaterials[index2].materialSprite;
            foods[index].foodMaterials[index2].materialCount = foodDatas[index].foodMaterials[index2].materialCount;
        }
    }
    #endregion

    #region Cook Init Function
    public void FoodInit()
    {
        if (GameManager.instance.isGameEnd) return;

        currentFood = foods[foodIndex];

        MainPlate _mainPlate = mainPlate.GetComponent<MainPlate>();
        _mainPlate.Init(currentFood.foodName, currentFood.foodSprite, currentFood.foodComplete, currentFood.timer, currentFood.foodMaterials[materialIndex]);
    }

    public void MaterialInit()
    {
        // 재료가 남아있으면 재료설정
        if (materialIndex < currentFood.foodMaterials.Length - 1)
        {
            materialIndex++;
            MainPlate _mainPlate = mainPlate.GetComponent<MainPlate>();
            _mainPlate.MaterialInit(currentFood.foodMaterials[materialIndex]);
        }
        // 재료가 끝났으면 음식설정
        else
        {
            GameManager.instance.completeFoodCount++;
            NextRecipeInit();
        }
    }

    public void NextRecipeInit()
    {
        materialIndex = 0;
        foodIndex++;
        if (foodIndex >= foods.Length)
        {
            StartCoroutine(GameEnd());
            return;
        }
        if (GameManager.instance.isGameEnd) return;
        foreach (var item in GameManager.instance.plateSpawnParent.GetComponentsInChildren<SubPlate>())
        {
            Destroy(item.gameObject);
        }
        StartCoroutine(GameManager.instance.WaitFoodGame());
    }

    public IEnumerator GameEnd()
    {
        GameManager.instance.Face();
        GameManager.instance.isGameEnd = true;
        GameManager.instance.plateSpawnParent.gameObject.SetActive(false);

        yield return new WaitForSeconds(1.5f); 
        // 스테이지 클리어 조건
        if (GameManager.instance.completeFoodCount >= Mathf.FloorToInt(currentFood.foodMaterials.Length / 2))
        {
            // 만든 스테이지 수를 넘어가면 리턴시킴
            if (StageManager.instance.clickStageIndex + 1 < StageManager.instance.stageButtons.Length)
            {
                StageManager.instance.StageClear(StageManager.instance.clickStageIndex + 1);
                GameUiManager.instance.nextStageText.gameObject.SetActive(true);
            }
        }
        GameUiManager.instance.gameEndPanel.SetActive(true);
    }
    #endregion

    private void Timer()
    {
        if (GameManager.instance.isGameEnd) return;
        if (GameManager.instance.isTimeOver) return;

        float timer = Mathf.Max(Mathf.FloorToInt(currentFood.timer -= Time.deltaTime), 0);
        GameManager.instance.timerText.text = timer.ToString();
        if (currentFood.timer <= 0)
        {
            GameManager.instance.isTimeOver = true;
            NextRecipeInit();
        }
    }
}
