// # System
using System.Collections;
using System.Collections.Generic;

// # Unity
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyBase : MonoBehaviour
{
    [SerializeField]
    private float               defaultHp;
    private float               currentHp;

    [SerializeField]
    private float               movementSpeed;

    [SerializeField]
    private Transform           target;

    private new Rigidbody       rigidbody;
    private NavMeshAgent        navMeshAgent;

    private void Start()
    {
        rigidbody    = GetComponent<Rigidbody>();
        navMeshAgent = GetComponent<NavMeshAgent>();

        navMeshAgent.speed = movementSpeed;
    }

    private void Update()
    {
        if(target != null)
        {
            MoveToTarget();
        }
    }

    public void Initlize(Transform target)
    {
        this.target = target;

        currentHp = defaultHp;
    }

    private void MoveToTarget()
    {
        navMeshAgent.SetDestination(target.position);
    }

    public void TakeDamage(float damage)
    {
        Debug.Log("데미지받음");
        currentHp -= damage;
        Die();
    }

    public void Die()
    {
        if(currentHp <= 0)
        {
            Destroy(gameObject);
        }
    }
}