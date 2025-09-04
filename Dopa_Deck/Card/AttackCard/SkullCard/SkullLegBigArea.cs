using UnityEngine;

public class SkullLegBigArea : MonoBehaviour
{
    private void Update()
    {
        transform.rotation = Quaternion.Euler(-90, 0 , 0);
    }

    private void OnTriggerEnter(Collider other)
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
}
