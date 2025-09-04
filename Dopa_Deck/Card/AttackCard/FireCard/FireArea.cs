
using UnityEngine;

public class FireArea : MonoBehaviour
{
    [SerializeField]
    private GameObject[] warningAreas;
    [SerializeField]
    private GameObject[] monsterKillerAreas;

    private int          previousIndex = -1;

    public int GetWarningAreasLength() { return warningAreas.Length; }

    public WarningArea GetRandomWarningArea()
    {
        int i = 0;

        // 이전 경고 영역이랑 중복되지 않도록
        do
        {
            i = Random.Range(0, warningAreas.Length);
        } while (i == previousIndex);
        previousIndex = i;

        return i switch
        {
            0 => WarningArea.Center,
            1 => WarningArea.LeftUP,
            2 => WarningArea.RightUP,
            3 => WarningArea.LeftDown,
            4 => WarningArea.RightDown,
            _ => WarningArea.Center
        };
    }

    public GameObject GetWarningArea(WarningArea warningArea)
    {
        return warningArea switch
        {
            WarningArea.Center => warningAreas[(int)WarningArea.Center],
            WarningArea.LeftUP => warningAreas[(int)WarningArea.LeftUP],
            WarningArea.RightUP => warningAreas[(int)WarningArea.RightUP],
            WarningArea.LeftDown => warningAreas[(int)WarningArea.LeftDown],
            WarningArea.RightDown => warningAreas[(int)WarningArea.RightDown],
            _ => null
        };
    }

    public void SetKillerArea(bool value)
    {
        for(int i = 0; i < monsterKillerAreas.Length; i++)
        {
            monsterKillerAreas[i].SetActive(value);
        }
    }
}
