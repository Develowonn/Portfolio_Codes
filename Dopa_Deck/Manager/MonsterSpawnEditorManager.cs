// # System
using System.IO;
using System.Collections.Generic;

// # Unity
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// # Etc
using TMPro;

public class MonsterSpawnEditorManager : MonoBehaviour
{
	[SerializeField]
	private int						gameMaxTime;

	[Header("Data")]
	[SerializeField]
	private EditorMonsterData[]		monsterData;
	private List<GameObject>        monsterObjectLists;
	private List<MonsterSpawnData>  monsterSpawnDatas;

	[Header("UI")]
	[SerializeField]
	private Slider					timerSliderBar;
	[SerializeField]
	private TMP_Text				currentGameTimeText;

	[Space(10), SerializeField]
	private TMP_InputField			gameTimeInputfield;
	[SerializeField]
	private Button					gameTimeSaveButton;

	[Space(10), SerializeField]
	private TMP_Dropdown			monsterDropdown;
	[SerializeField]
	private TMP_Dropdown			stageDropdown;

	[Space(10), SerializeField]
	private Button					saveButton;
	[SerializeField]
	private Button					resetButton;

	private void Start()
	{
		monsterSpawnDatas  = new List<MonsterSpawnData>();
		monsterObjectLists = new List<GameObject>();

		InitializeUI();
		InitializeData();
	}

	private void Update()
	{
		// 포인터가 UI 위에 있지 않으면 
		if(Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
		{
			SpawnMonster();
		}
	}

	private void InitializeData()
	{
		foreach (var monster in monsterObjectLists)
		{
			Destroy(monster);
		}
		monsterObjectLists.Clear();
		monsterSpawnDatas.Clear();

		string fileName							  = $"{stageDropdown.options[stageDropdown.value].text}";
		MonsterSpawnDataList monsterSpawnDataList = EnemySpawnEditorJsonManager.Instance.LoadData(fileName);
		
		if(monsterSpawnDataList == null) return;

		monsterSpawnDatas = monsterSpawnDataList.monsterSpawnDatas;
		UpdateActiveMonstersByTime();
	}

	private void InitializeUI()
	{
		// 드랍다운 설정
		stageDropdown.onValueChanged.AddListener(delegate { OnStageDropDownValueChanged(); });

		// 슬라이더 바 설정 
		timerSliderBar.maxValue = gameMaxTime;
		timerSliderBar.onValueChanged.AddListener(delegate { OnTimerValueChanged(); });

		// 게임 타임 설정 
		gameTimeSaveButton.onClick.AddListener(() => OnClickGameTimeSave());

		// 버튼 설정
		saveButton.onClick.AddListener(() => OnClickSaveButton());
		resetButton.onClick.AddListener(() => OnClickResetButton());
	}	

	private void SpawnMonster()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue))
		{
			GameObject monsterObj = monsterData[monsterDropdown.value].monsterObject;	

			Vector3 spawnPos	  = hit.point;
			spawnPos.y            = 0.0f;

		    EditorMonster monster = Instantiate(monsterObj, spawnPos, Quaternion.identity)
				.GetComponent<EditorMonster>();

			monsterObjectLists.Add(monster.gameObject);

			AddMonsterSpawnData(monster.GetMonsterName(), (int)timerSliderBar.value, spawnPos);
		}
	}

	private void AddMonsterSpawnData(string monsterName, int spawnTime, Vector3 spawnPos)
	{
		monsterSpawnDatas.Add(new MonsterSpawnData(monsterName, spawnTime, spawnPos));
	}

	private void UpdateActiveMonstersByTime()
	{
		foreach(var monster in monsterObjectLists)
		{
			Destroy(monster);
		}
		monsterObjectLists.Clear();

		foreach(var monster in monsterSpawnDatas)
		{
			if((int)timerSliderBar.value == monster.spawnTime)
			{
				GameObject monsterObj = monsterData[(int)GetMonsterType(monster.monsterName)].monsterObject;

				GameObject temp       = Instantiate(monsterObj, monster.spawnPosition, Quaternion.identity);
				monsterObjectLists.Add(temp.gameObject);
			}
		}
	}

	private MonsterType GetMonsterType(string monsterName)
	{
		return monsterName switch
		{
			Constants.MonsterName.Mushroom => MonsterType.Mushroom,
			Constants.MonsterName.Radish   => MonsterType.Radish,
			Constants.MonsterName.Golem    => MonsterType.Golem,
			Constants.MonsterName.Cat	   => MonsterType.Cat,
			_                              => MonsterType.Cat
		};
	}

	private void OnTimerValueChanged()
	{
		currentGameTimeText.text = $"{(int)timerSliderBar.value}초";
		UpdateActiveMonstersByTime();
	}

	private void OnStageDropDownValueChanged()
	{
		InitializeData();
	}

	private void OnClickGameTimeSave()
	{
		int.TryParse(gameTimeInputfield.text, out int gameTime);

		timerSliderBar.maxValue  = gameTime;
		gameTimeInputfield.text  = default;
	}

	private void OnClickSaveButton()
	{
		string fileName = $"{stageDropdown.options[stageDropdown.value].text}.json";
		EnemySpawnEditorJsonManager.Instance.SaveData(monsterSpawnDatas, fileName);
	}

	private void OnClickResetButton()
	{
		string fileName = $"{stageDropdown.options[stageDropdown.value].text}.json";

		foreach (var monster in monsterObjectLists)
		{
			Destroy(monster);
		}

		monsterObjectLists.Clear();
		monsterSpawnDatas.Clear();
		EnemySpawnEditorJsonManager.Instance.ResetData(fileName);
	}
}