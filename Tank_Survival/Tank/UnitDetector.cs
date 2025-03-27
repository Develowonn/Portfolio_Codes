// # System
using System.Collections;
using System.Collections.Generic;

// # Unity
using UnityEngine;

public class UnitDetector : MonoBehaviour
{
    [Header("Detector Settings")]
    [SerializeField]
    private float               detectionRange;
    [SerializeField]
    private LayerMask           detectionUnitLayer;
    [SerializeField]
    private float               coolTime;

    private bool                isDetected;
    private Collider            detectedUnit; 
    public  Collider            DetectedUnit { get => detectedUnit; }

    private void Start()
    {
        isDetected = true;
    }

    private void Update()
    {
        DetectUnitWithinRange();
    }

    /// <summary>
    /// 사거리 안에 있는 유닛을 탐지합니다.
    /// </summary>
    private void DetectUnitWithinRange()
    {
        Collider[] detectedMonsters = Physics.OverlapSphere(transform.position, detectionRange, detectionUnitLayer);

        if(detectedMonsters.Length == 0)
        {
            detectedUnit = null;
            return;
        }

        if (!isDetected) return;
        detectedUnit = FindNearUnit(detectedMonsters);
    }
    
    /// <summary>
    /// 사거리 안에 있는 유닛들 중 제일 가까운 유닛을 반환시킵니다.
    /// </summary>
    private Collider FindNearUnit(Collider[] detectedMonsters)
    {
        isDetected = false;
        StartCoroutine(DetectCoolTimeCoroutine());

        Collider nearUnit         = null;
        float    distanceFromUnit = float.MaxValue;

        foreach(Collider detectedMonster in detectedMonsters)
        {
            if(Vector3.Distance(transform.position, detectedMonster.transform.position) < distanceFromUnit)
            {
                nearUnit         = detectedMonster;
                distanceFromUnit = Vector3.Distance(transform.position, nearUnit.transform.position);
            }
        }

        return nearUnit;
    }

    private IEnumerator DetectCoolTimeCoroutine()
    {
        yield return new WaitForSeconds(coolTime);

        isDetected = true;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
