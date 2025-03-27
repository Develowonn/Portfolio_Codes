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

        // Ž���� ������ ���� ���, ĳ���� �ʱ� ��ġ�� ȸ�� ��Ŵ 
        if ( unitDetector.DetectedUnit == null)
        {
            CheckSpinActive(cannonTransform.rotation, Quaternion.Euler(0.0f, 0.0f, 0.0f), 5.0f);

            cannonTransform.localRotation = Quaternion.Slerp(
                cannonTransform.localRotation,             // ���� ȸ��             
                Quaternion.Euler(0.0f, 0.0f, 0.0f),        // �ʱ� ȸ��
                spinSpeed * 0.5f * Time.deltaTime          // ȸ�� �ӵ� 
            );

            return;
        }

        // Ÿ���� ������ ���, ĳ���� Ÿ�� �������� ȸ�� ��Ŵ
        Vector3 currentTarget = unitDetector.DetectedUnit.transform.position;
        Vector3 direction     = currentTarget - cannonTransform.position;

        // ���̸� 0���� ���߸鼭 �¿�θ� ȸ����ų �� �ְ� �� 
        direction.y = 0;

        // ��ǥ ȸ���� ��� (Y�� ȸ����)
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        // �� ������ ���� ���̰� 10�� �̻��̸� ���� �� 
        CheckSpinActive(cannonTransform.rotation, targetRotation, 10.0f);

        // ���� ȸ���� ��ǥ ȸ�� ���̸� �������ϰ� ȸ��
        cannonTransform.rotation = Quaternion.Slerp(
            cannonTransform.rotation,       // ���� ȸ��
            targetRotation,                 // ��ǥ ȸ�� (Y�ุ)
            spinSpeed * Time.deltaTime      // ȸ�� �ӵ�
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
