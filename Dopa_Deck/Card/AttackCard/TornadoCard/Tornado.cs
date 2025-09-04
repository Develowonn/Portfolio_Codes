using UnityEngine;

public class Tornado : MonoBehaviour
{
    private void OnParticleCollision(GameObject other)
    {
        Monster     monster     = other.GetComponent<Monster>();
		BossMonster bossMonster = other.GetComponent<BossMonster>();

        if(bossMonster != null)
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
