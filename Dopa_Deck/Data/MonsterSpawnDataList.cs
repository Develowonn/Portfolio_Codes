using System.Collections.Generic;

[System.Serializable]
public class MonsterSpawnDataList 
{
	public MonsterSpawnDataList(List<MonsterSpawnData> monsterSpawnDatas)
	{
		this.monsterSpawnDatas = monsterSpawnDatas;
	}

	public List<MonsterSpawnData> monsterSpawnDatas; 
}
