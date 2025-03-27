// # System
using System.Collections;
using System.Collections.Generic;

// # Unity
using UnityEngine;

public class TankCrushCheck : MonoBehaviour
{
    private TankMovement tankMovement;

    private void Start()
    {
        tankMovement = GetComponentInParent<TankMovement>();
    }

    private void OnTriggerStay(Collider collision)
    {
        if (collision.gameObject.CompareTag("Environment"))
        {
            tankMovement.CollideStructures();
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.CompareTag("Environment"))
        {
            tankMovement.AvoidStructures();
        }
    }
}
