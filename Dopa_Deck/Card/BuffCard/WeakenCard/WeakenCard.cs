// # System
using System;

// # Unity
using UnityEngine;

// # ETC
using Cysharp.Threading.Tasks;

public class WeakenCard : Card
{
    private GameObject  aura;

    [SerializeField]
    private float       debuffDuration;
    [SerializeField, Range(0.0f, 1.0f)]
    private float       slowPercent;
    [SerializeField, Range(0.0f, 1.0f)]
    private float       attackDebuffPercent;

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

        await Debuff();
        Destroy(aura);
    }

    private void SpawnAura()
    {
        aura = Instantiate(auraVFX, player.transform.position + auraOffset, Quaternion.Euler(-75, 0, 0));
        aura.transform.SetParent(player);
    }

    private async UniTask Debuff()
    {
        float defaultMovementSpeed = playerStat.GetMovementSpeedStat();
        float defaultAttackPower   = playerStat.GetAttackPowerStat();

        playerStat.ReduceMoveSpeedByPercent(slowPercent);
        playerStat.ReduceAttackPowerByPercent(attackDebuffPercent);

        await UniTask.Delay(TimeSpan.FromSeconds(debuffDuration));

        playerStat.SetMovementSpeed(defaultMovementSpeed);
        playerStat.SetAttackPower(defaultAttackPower);
    }
}
