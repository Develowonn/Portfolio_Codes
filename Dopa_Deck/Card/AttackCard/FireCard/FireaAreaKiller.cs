using UnityEngine;

public class FireaAreaKiller : MonoBehaviour
{
    [Header("VFX")]
    [SerializeField]
    private GameObject fireVfx;
    [SerializeField]
    private float      fireVfxDestroyTime;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Constants.Tag.Monster))
        {
            Monster     monster     = other.GetComponent<Monster>();
			BossMonster bossMonster = other.GetComponent<BossMonster>();

			if (bossMonster != null)
			{
				return;
			}
			else if (monster != null)
            {
                GameObject fireEffect = Instantiate(fireVfx, other.transform.position, Quaternion.Euler(-90, 0, 0));
                Destroy(fireEffect, fireVfxDestroyTime);

                monster.TakeDamage(Constants.maxDamage);
            }
        }
    }
}