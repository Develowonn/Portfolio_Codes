using UnityEngine;

public class FireWall : MonoBehaviour
{
    [SerializeField, Range(0.0f, 1.0f)]
    private float       playerHealthReductionRate;

    [Header("VFX")]
    [SerializeField]
    private GameObject  fireVfx;
    [SerializeField]    
    private float       fireVfxDestroyTime;

    private void OnPlayerTouchWall(Collision collider)
    {

    }

    private void OnMonsterTouchWall(Collision collider)
    {
        Monster     monster     = collider.gameObject.GetComponent<Monster>();
		BossMonster bossMonster = collider.gameObject.GetComponent<BossMonster>();

		if (bossMonster != null)
		{
			GameObject fireEffect = Instantiate(fireVfx, collider.transform.position, Quaternion.Euler(-90, 0, 0));
			Destroy(fireEffect, fireVfxDestroyTime);

			bossMonster.TakeDamage(bossMonster.GetCurrentHp() / 2);
            return;
		}
		else if (monster != null)
        {
            GameObject fireEffect = Instantiate(fireVfx, collider.transform.position, Quaternion.Euler(-90, 0, 0));
            Destroy(fireEffect, fireVfxDestroyTime);

            monster.TakeDamage(Constants.maxDamage);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(Constants.Tag.Player))
        {
            OnPlayerTouchWall(collision);
        }
        else if (collision.gameObject.CompareTag(Constants.Tag.Monster))
        {
            OnMonsterTouchWall(collision);
        }
    }
}
