using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankMovement : MonoBehaviour
{
    private const float forwardRotationMultiplier  = -1.0f;  // 전진 시 바퀴 회전 방향
    private const float backwardRotationMultiplier =  1.0f;  // 후진 시 바퀴 회전 방향

    [Header("Control Settings")]
    [SerializeField]
    private float         movementSpeed;
    [SerializeField]
    private float         rotationSpeed;
    [SerializeField]
    private RotateAngle   rotateAngle;

    [Header("Wheel Settings")]
    [SerializeField]
    private GameObject[]  wheels;
    [SerializeField]
    private float         wheelRotationSpeed;

    private float         movementInput;
    private float         rotationInput;

    private bool          isCrash;

    private new Rigidbody rigidbody;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        movementInput = Input.GetAxis("Vertical");
        rotationInput = Input.GetAxis("Horizontal");

        RotateWheel(movementInput, rotationInput);
    }

    private void FixedUpdate()
    {
        if (movementInput == 0 && rotationInput == 0) return;

        MoveTank(movementInput);
        RotateTank(rotationInput);
    }

    private void MoveTank(float movementInput)
    {
        if(isCrash && movementInput >= 0.0f)
        {
            rigidbody.velocity = Vector3.zero;
            return;
        }

        Vector3 moveDirection = transform.forward * 
            movementSpeed * movementInput;

        moveDirection.y = rigidbody.velocity.y;

        //rigidbody.MovePosition(rigidbody.position + moveDirection);
        rigidbody.velocity = moveDirection;
    }

    private void RotateTank(float rotationInput)
    {
        if(isCrash && movementInput >= 0.0f && rotationInput != 0.0f)
        {
            rigidbody.velocity = Vector3.zero;
            return;
        }

        float multiply          = movementInput >= 0.0f ? 1.0f : -1.0f;
        float rotation          = rotationInput * rotationSpeed * Time.fixedDeltaTime;
        Quaternion turnRotation = Quaternion.Euler(0.0f, rotation * multiply, 0.0f);

        rigidbody.MoveRotation(rigidbody.rotation * turnRotation);
    }

    private void RotateWheel(float movementInput, float rotationInput)
    {
        foreach (GameObject wheel in wheels)
        { 
            if (wheel != null)
            {
                float multiplier    =  movementInput > 0.0f ? forwardRotationMultiplier : backwardRotationMultiplier;
                float wheelRotation = (movementInput * rotationSpeed * Time.deltaTime) * multiplier;

                switch (rotateAngle)
                {
                    case RotateAngle.X:
                        wheel.transform.Rotate(wheelRotation, 0.0f, 0.0f);
                        break;

                    case RotateAngle.Y:
                        wheel.transform.Rotate(0.0f, wheelRotation, 0.0f);
                        break;

                    case RotateAngle.Z:
                        wheel.transform.Rotate(0.0f, 0.0f, wheelRotation);
                        break;
                }
            }
        }
    }

    public void CollideStructures()
    {
        isCrash            = true;
        rigidbody.velocity = Vector3.zero;
    }

    public void AvoidStructures()
    {
        isCrash            = false;
        rigidbody.velocity = Vector3.zero;
    }
}   