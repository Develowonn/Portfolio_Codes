// # System
using System.IO;
using System.Collections.Generic;

// # Unity
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class EnemySpawnEditorJsonManager : MonoBehaviour
{
	public static EnemySpawnEditorJsonManager Instance;

	private void Awake()
	{
		Instance = this;
	}

	public void SaveData(List<MonsterSpawnData> dataArray, string fileName)
	{
		MonsterSpawnDataList dataList = new MonsterSpawnDataList(dataArray);
		string savePath               = Path.Combine(Application.dataPath, Constants.Channel.JsonChannel, fileName);
		string json                   = JsonUtility.ToJson(dataList, true);
		
		File.WriteAllText(savePath, json);

		#if UNITY_EDITOR
		AssetDatabase.Refresh();
		#endif
	}

	public MonsterSpawnDataList LoadData(string fileName)
	{
		TextAsset jsonText = Resources.Load<TextAsset>("Datas/" + fileName);

		if (jsonText == null)
		{
			Debug.Log($"{fileName} File이 존재하지 않습니다.");
			return null;
		}

		MonsterSpawnDataList result = JsonUtility.FromJson<MonsterSpawnDataList>(jsonText.text);

		return result;
	}

	public void ResetData(string fileName)
	{
		string savePath = Path.Combine(Application.dataPath, Constants.Channel.JsonChannel, fileName);
		File.WriteAllText(savePath, string.Empty);

		#if UNITY_EDITOR
		AssetDatabase.Refresh();
		#endif
	}
}
