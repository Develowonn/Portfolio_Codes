using UnityEngine;

public class FireExplosion : MonoBehaviour
{
    [SerializeField, Range(0.0f, 1.0f)]
    private float playerHealthReductionRate;

    private void AttackToPlayer(Collider other)
    {
		// 플레이어 체력 퍼센트에 반영해 감소
		PlayerController playerController = other.GetComponent<PlayerController>();

        if (playerController != null)
        {
			playerController.TakeDamagePercent
				(playerHealthReductionRate);
        }
    }

    private void AttackToMonster(Collider other) 
    {
        Monster     monster     = other.GetComponent<Monster>();
		BossMonster bossMonster = other.GetComponent<BossMonster>();

		if (bossMonster != null)
		{
			bossMonster.TakeDamage(bossMonster.GetCurrentHp() / 2);
			return;
		}
		else if (monster != null)
        {
            monster.TakeDamage(Constants.maxDamage);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Constants.Tag.Player))
        {
            AttackToPlayer(other);
        }
        else if (other.CompareTag(Constants.Tag.Monster))
        {
            AttackToMonster(other);
        }
    }
}
