// # System
using System;

// # Unity
using UnityEngine;

// # ETC
using Cysharp.Threading.Tasks;


public class CooltimeReductionCard : Card
{
    private GameObject  aura;

    [SerializeField]
    private float       cooolTimeReductionDuration;
    // ��Ÿ�� ������
    [SerializeField, Range(0f, 1f)]
    private float       coolTimeReductionPercent; 

    [Header("Offset")]
    [SerializeField]
    private Vector3     auraOffset;

    [Header("VFX")]
    [SerializeField]
    private GameObject  auraVFX;

	private bool        isDone;
	private Transform   player;

    public override void Execute()
    {
		if (isDone) return;

		player = InGameManager.Instance.GetPlayerObject().transform;
		isDone = true;

		ExecuteSkill().Forget();
    }

    private async UniTask ExecuteSkill()
    {
        SpawnAura();

        await ReduceSkillCooltime();
        Destroy(aura);
    }

    private void SpawnAura()
    {
        aura = Instantiate(auraVFX, player.transform.position + auraOffset, Quaternion.Euler(-90, 0, 0));
    }

    private async UniTask ReduceSkillCooltime()
    {
        // ��Ÿ�� ������ ���� O
        PlayerSkillManager p = player.GetComponent<PlayerSkillManager>();
        p.SkillCoolTimeBuffTrigger(coolTimeReductionPercent, true);

        await UniTask.Delay(TimeSpan.FromSeconds(cooolTimeReductionDuration));

		// ��Ÿ�� ������ ���� X 
		p.SkillCoolTimeBuffTrigger(coolTimeReductionPercent, false);
    }
}
