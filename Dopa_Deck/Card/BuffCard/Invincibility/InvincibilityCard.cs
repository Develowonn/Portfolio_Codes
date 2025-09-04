// # System
using System;

// # Unity
using UnityEngine;

// # ETC
using Cysharp.Threading.Tasks;

public class InvincibilityCard : Card
{
    private GameObject  aura;

    [SerializeField]
    private float       invincibilityDuration;

    [Header("Offset")]
    [SerializeField]
    private Vector3     auraOffset;

    [Header("VFX")]
    [SerializeField]
    private GameObject  auraVFX;

	private bool        isDone;
	private Transform   player;
    private PlayerStat  playerStat;

    public override void Execute()
    {
        if(isDone) return;

        player      = InGameManager.Instance.GetPlayerObject().transform;
        playerStat  = player.GetComponent<PlayerStat>();
        isDone      = true;

        ExecuteSkill().Forget();
    }

    private async UniTask ExecuteSkill()
    {
        SpawnAura();
        await ActivateInvincibility();

        Destroy(aura);
    }

    private void SpawnAura()
    {
        aura = Instantiate(auraVFX, player.transform.position + auraOffset, Quaternion.Euler(-90, 0, 0));
    }

    private async UniTask ActivateInvincibility()
    {
        playerStat.SetInvincibility(true);
        await UniTask.Delay(TimeSpan.FromSeconds(invincibilityDuration));
        playerStat.SetInvincibility(false);
    }
}