using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    private Vector3     offset;
    [SerializeField]
    private Transform   target;
    [SerializeField]
    private float       smoothTime;

    private Vector3     currentVelocity;

    private void Start()
    {
        currentVelocity = Vector3.zero;
    }

    private void LateUpdate()
    {
        Vector3 targetPosition = target.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, smoothTime);
    }
}
