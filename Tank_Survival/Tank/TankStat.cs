// # System
using System.Collections;
using System.Collections.Generic;

// # Unity
using UnityEngine;

public class TankStat : MonoBehaviour
{
    [SerializeField]
    private int     hp;
    [SerializeField]
    private int     cannonDamage;
    [SerializeField]
    private int     movementSpeed;

    public int      HP => hp;
    public int      CannonDamage => cannonDamage;
    public int      MovementSpeed => movementSpeed;


    public void InitilaizeStat(int hp, int cannonDamage, int movementSpeed)
    {
        this.hp             = hp;
        this.cannonDamage   = cannonDamage;
        this.movementSpeed  = movementSpeed;
    }
}