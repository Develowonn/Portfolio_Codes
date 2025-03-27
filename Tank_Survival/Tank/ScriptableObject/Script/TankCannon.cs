// # System
using System.Collections;
using System.Collections.Generic;

// # Unity
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "TankCannon", menuName = "Tank/Cannon")]
public class TankCannon : ScriptableObject
{
    [Header("Tank Cannon Stat")]
    public int          hp;
    public int          damage;

    [Header("Tank Cannon Object")]
    public GameObject   tankCannon;
    public Vector3      tankCannonScale;
    public Vector3      positionOffset;

    [Header("Tank Cannon Attack")]
    public GameObject   missileAttackEffect;
}
