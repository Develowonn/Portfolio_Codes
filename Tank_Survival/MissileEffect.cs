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
        // OnParticleCollision �� �浹�� �Ͼ�� ���� Update()ó�� ��� ȣ���Ѵ�
        // �̸� �����ϱ� ���� �������� ���� ������Ʈ�� Hashset�� �߰��ؼ� 
        // Hashset�� ���ԵǾ� ������ ���Ͻ��� ��� �������� �޴� ������ �����ߴ�.

        if (other.CompareTag(TagType.Enemy))
        {
            if (hitObjects.Contains(other)) return;

            EnemyBase enemyBase = other.GetComponent<EnemyBase>();
            enemyBase.TakeDamage(damage);

            hitObjects.Add(other);
        }
    }
}
