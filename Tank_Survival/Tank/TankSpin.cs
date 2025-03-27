// # System
using System.Collections;
using System.Collections.Generic;

// # Unity
using UnityEngine;

public class TankSpin : MonoBehaviour
{
    [SerializeField]
    private UnitDetector unitDetector;

    [Header("Cannon Settings")]
    [SerializeField]
    private Transform    cannonTransform;
    [SerializeField]
    private float        spinSpeed;

    private bool         isSpin;
    
    public bool          IsSpin => isSpin;

    public void InitilizeCannon(Transform canonTransform)
    {
        this.cannonTransform = canonTransform;
    }

    private void Update()
    {
        if (cannonTransform == null)
        {
            Debug.Log("Cannon Transform is null!!");
            return;
        }

        // 탐지된 유닛이 없는 경우, 캐논을 초기 위치로 회전 시킴 
        if ( unitDetector.DetectedUnit == null)
        {
            CheckSpinActive(cannonTransform.rotation, Quaternion.Euler(0.0f, 0.0f, 0.0f), 5.0f);

            cannonTransform.localRotation = Quaternion.Slerp(
                cannonTransform.localRotation,             // 현재 회전             
                Quaternion.Euler(0.0f, 0.0f, 0.0f),        // 초기 회전
                spinSpeed * 0.5f * Time.deltaTime          // 회전 속도 
            );

            return;
        }

        // 타겟이 감지된 경우, 캐논을 타겟 방향으로 회전 시킴
        Vector3 currentTarget = unitDetector.DetectedUnit.transform.position;
        Vector3 direction     = currentTarget - cannonTransform.position;

        // 높이를 0으로 맞추면서 좌우로만 회전시킬 수 있게 함 
        direction.y = 0;

        // 목표 회전을 계산 (Y축 회전만)
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        // 두 사이의 각도 차이가 10도 이상이면 스핀 중 
        CheckSpinActive(cannonTransform.rotation, targetRotation, 10.0f);

        // 현재 회전과 목표 회전 사이를 스무스하게 회전
        cannonTransform.rotation = Quaternion.Slerp(
            cannonTransform.rotation,       // 현재 회전
            targetRotation,                 // 목표 회전 (Y축만)
            spinSpeed * Time.deltaTime      // 회전 속도
        );
    }

    private bool CheckSpinActive(Quaternion a, Quaternion b, float angleDifference)
    {
        if(Quaternion.Angle(a, b) > angleDifference)
        {
            isSpin = true;
        }
        else
        {
            isSpin = false;
        }

        return isSpin;
    }
}
