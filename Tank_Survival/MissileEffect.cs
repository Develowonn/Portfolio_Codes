// # System
using System.Collections;
using System.Collections.Generic;

// # Unity
using UnityEngine;

public class MissileEffect : MonoBehaviour
{
    private HashSet<GameObject> hitObjects = new HashSet<GameObject>();
    private float damage = 0;

    public void Initialize(float damage)
    {
        this.damage = damage;
    }

    private void OnParticleCollision(GameObject other)
    {
        // OnParticleCollision 은 충돌이 일어나는 동안 Update()처럼 계속 호출한다
        // 이를 방지하기 위해 데미지를 받은 오브젝트를 Hashset에 추가해서 
        // Hashset에 포함되어 있으면 리턴시켜 계속 데미지가 받는 현상을 방지했다.

        if (other.CompareTag(TagType.Enemy))
        {
            if (hitObjects.Contains(other)) return;

            EnemyBase enemyBase = other.GetComponent<EnemyBase>();
            enemyBase.TakeDamage(damage);

            hitObjects.Add(other);
        }
    }
}
