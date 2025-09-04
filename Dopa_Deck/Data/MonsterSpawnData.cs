using UnityEngine;

[System.Serializable]
public class MonsterSpawnData
{
	public MonsterSpawnData(string monsterName, int spawnTime, Vector3 spawnPosition)
	{
		this.monsterName   = monsterName;
		this.spawnTime     = spawnTime;	
		this.spawnPosition = spawnPosition;
	}

	public string      monsterName;	
	public int		   spawnTime;
	public Vector3	   spawnPosition;
}