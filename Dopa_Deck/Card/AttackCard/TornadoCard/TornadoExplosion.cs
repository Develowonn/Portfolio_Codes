// # System
using System;

// # Unity
using UnityEngine;

// # ETC
using Cysharp.Threading.Tasks;


public class TornadoExplosion : MonoBehaviour
{
    [SerializeField]
    private float explosionDamageDelay;

	private async UniTaskVoid Explode(BossMonster monster)
	{
		await UniTask.Delay(TimeSpan.FromSeconds(explosionDamageDelay));

		if (monster != null)
		{
			monster.TakeDamage(monster.GetCurrentHp() / 2);
		}
	}


	private async UniTaskVoid Explode(Monster monster)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(explosionDamageDelay));

        if (monster != null)
        {
            monster.TakeDamage(Constants.maxDamage);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Monster     monster		= other.gameObject.GetComponent<Monster>();
		BossMonster bossMonster = other.GetComponent<BossMonster>();

		if (bossMonster != null)
		{
			Explode(bossMonster).Forget();
			return;
		}
		else if(monster != null)
		{
			Explode(monster).Forget();
		}
	}
}
