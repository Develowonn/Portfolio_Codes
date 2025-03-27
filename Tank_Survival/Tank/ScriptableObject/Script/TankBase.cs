// # System
using System.Collections;
using System.Collections.Generic;

// # Unity
using UnityEngine;

[CreateAssetMenu(fileName = "TankBase", menuName = "Tank/Base")]
public class TankBase : ScriptableObject
{
    public TankCannon[] tankCannons;

    [Header("Tank Base Stat")]
    public int          hp;
    public int          movementSpeed;

    [Header("Tank Base Object")]
    public GameObject   tankBase;
    public Vector3      tankBaseScale;
    public Vector3      positionOffset;

    [Header("Crash Check Collider")]
    public Vector3      crashCheckColliderPosition;
    public Vector3      crashCheckColliderSize;

    [Header("Cannon Installation Area")]
    public Transform[]  mainCannonInstallationArea;     // 메인 캐논 설치 위치 
    public Transform[]  subCannonInstallationArea;      // 서브 캐논 설치 위치 
}