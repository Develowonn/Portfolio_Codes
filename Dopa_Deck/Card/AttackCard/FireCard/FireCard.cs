// # System
using System;

// # Unity
using UnityEngine;

// # ETC
using Cysharp.Threading.Tasks;

public class FireCard : Card
{
    private GameObject  fire;
    private GameObject  explosion;
    private GameObject  fireAreaObject;
    private FireArea    fireArea;

    [SerializeField]
    private int         attackCount;
    [SerializeField]
    private float       warningAreaDuration;
    [SerializeField]
    private float       fireDuration;

    [Header("VFX")]
    [SerializeField]
    private GameObject  fireAreaVFX;
    [SerializeField]
    private GameObject  fireVFX;
    [SerializeField]
    private GameObject  explosionVFX;

    private bool        isDone;
    private Transform   player;

    public override void Execute()
    {
        if(isDone) return;

        player = InGameManager.Instance.GetPlayerObject().transform;

        isDone = true;

        ExecuteSkill().Forget();
    }

    private async UniTask ExecuteSkill()
	{
        SpawnFireArea();
        await UniTask.Delay(TimeSpan.FromSeconds(1.0f));
        fireArea.SetKillerArea(false);

        await UniTask.Delay(TimeSpan.FromSeconds(2.0f));
        await ShowWarningAndAttackArea();

        await UniTask.Delay(TimeSpan.FromSeconds(1.5f));
        Destroy(fireAreaObject);
	}

    private void SpawnFireArea()
    {
        fireAreaObject = Instantiate(fireAreaVFX, player.position, Quaternion.Euler(0, 45, 0));
        fireAreaObject.transform.position = new Vector3(player.position.x, 0, player.position.z);

        fireArea = fireAreaObject.GetComponent<FireArea>();
    }

    private void SpawnFire(Vector3 spawnPos)
    {
        fire = Instantiate(fireVFX, spawnPos, Quaternion.Euler(0, 45, 0));
    }

    private void SpawnFireExplosion(Vector3 spawnPos)
    {
        explosion = Instantiate(explosionVFX, spawnPos, Quaternion.Euler(0, 45, 0));
    }

    private async UniTask ShowWarningAndAttackArea()
    {
        for (int i = 0; i < attackCount; i++)
        {
            WarningArea warningArea = fireArea.GetRandomWarningArea();
            GameObject  area        = fireArea.GetWarningArea(warningArea);
            area.SetActive(true);

            await UniTask.Delay(TimeSpan.FromSeconds(warningAreaDuration));
            area.SetActive(false);

            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
            SpawnFire(area.transform.position);
            await UniTask.Delay(TimeSpan.FromSeconds(fireDuration));

            Destroy(fire);
            SpawnFireExplosion(area.transform.position);
        }
    }
}