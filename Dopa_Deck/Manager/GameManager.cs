using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField]
    private int     maxEnergy;
	[SerializeField]
	private bool    isEnergtInfinity;

	private string  playerName;
    private int     playerGold;
    private int     currentEnergy;

    public string  GetPlayerName()    { return playerName; }
    public int     GetPlayerGold()    { return playerGold; }
    public int     GetMaxEnergy()     { return maxEnergy; }
    public int     GetCurrentEnergy() { return currentEnergy; }
    public bool    IsEnergtInfinity() { return isEnergtInfinity; }


	private void Start()
	{
		currentEnergy = maxEnergy;
        playerName    = string.Empty;
	}

	public void SetPlayerName(string value)
    {
        playerName = value;
    }

    public void AddPlayerGold(int gold)
    {
        playerGold += gold;
    }
}