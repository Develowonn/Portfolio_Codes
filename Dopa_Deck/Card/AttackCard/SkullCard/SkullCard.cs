// # System
using System;

// # Unity
using UnityEngine;

// # ETC
using Cysharp.Threading.Tasks;

public class SkullCard : Card
{
	private GameObject			skull;
	private GameObject			skullAura;
	private GameObject			skullLegSmallAura;
	private GameObject			skullLegBigAura;
	private GameObject			skullLegAttackAura;

	[SerializeField]
	private Vector2				attackRange;
	[SerializeField]
	private float				attackCoolTime;
	[SerializeField]
	private float				skullLegCount;

	[Header("Camera")]
	[SerializeField]
	private Vector3				zoomOutOffset;
	[SerializeField]
	private float				completionTime;
	private CameraFollowTarget	camaraFollowTarget;

	[Header("Offset")]
	[SerializeField]
	private Vector3				skullOffset;
	[SerializeField]
	private Vector3				skullAuraOffset;
	[SerializeField]
	private Vector3				skullExplosionOffset;
	[SerializeField]
	private Vector3				skullLegBigAuraOffset;
	[SerializeField]
	private Vector3				skullLegAttackAuraOffset;

	[Header("VFX")]
	[SerializeField]
	private GameObject			skullVFX;
	[SerializeField]
	private GameObject			skullAuraVFX;
	[SerializeField]
	private GameObject			skullAttackVFX;
	[SerializeField]
	private GameObject			skullExplosionVFX;
	[SerializeField]
	private GameObject			skullLegSmallAreaVFX;
	[SerializeField]
	private GameObject			skullLegBigAreaVFX;
	[SerializeField]
	private GameObject			skullLegAttackAreaVFX;

	private bool				isDone;
	private Transform			player;

	public override void Execute()
	{
		if(isDone) return;

		player				= InGameManager.Instance.GetPlayerObject().transform;
		camaraFollowTarget	= Camera.main.GetComponent<CameraFollowTarget>();
		isDone				= true;

		ExecuteSkill().Forget();
	}

	private async UniTask ExecuteSkill()
	{
		SpawnSkullAura();
		await UniTask.Delay(TimeSpan.FromSeconds(1.0f));

		SpawnSkull();
		await UniTask.Delay(TimeSpan.FromSeconds(0.5f));

		await SpawnSkullLeg();
		await SpawnSkullLegArea();
	}

	private void SpawnSkullExplosion()
	{
		Instantiate(skullExplosionVFX, player.position + skullExplosionOffset, Quaternion.Euler(-90, 0, 0));
	}

	private void SpawnSkull()
	{
		skull = Instantiate(skullVFX, player.position + skullOffset, Quaternion.Euler(-105, 0, 0));
		skull.transform.SetParent(player.transform);
	}

	private void SpawnSkullAura()
	{
		skullAura = Instantiate(skullAuraVFX, player.position + skullAuraOffset, Quaternion.Euler(-105, 0, 0));
		skullAura.transform.SetParent(player.transform);
	}

	private async UniTask SpawnSkullLeg()
	{
		for(int i = 0; i < skullLegCount; i++)
		{
			float x = UnityEngine.Random.Range(player.position.x - attackRange.x, player.position.x + attackRange.x);
			float z = UnityEngine.Random.Range(player.position.z - attackRange.y, player.position.z + attackRange.y);
			Vector3 spawnPosition = new Vector3(x, 0, z);

			Instantiate(skullAttackVFX, spawnPosition, Quaternion.Euler(-90, 0, 0));
			await UniTask.Delay(TimeSpan.FromSeconds(attackCoolTime));
		}
	}

	private async UniTask SpawnSkullLegArea()
	{
		Destroy(skull);
		Destroy(skullAura);
		SpawnSkullExplosion();

		skullLegSmallAura = Instantiate(skullLegSmallAreaVFX, player.position, Quaternion.Euler(-90, 0, 0));
		skullLegSmallAura.transform.SetParent(player.transform);

		await UniTask.Delay(TimeSpan.FromSeconds(1.5f));
		// 플레이어 움직임 제약
		player.GetComponent<PlayerController>().SetMovable(false);

		skullLegBigAura = Instantiate(skullLegBigAreaVFX, player.position + skullLegBigAuraOffset, Quaternion.Euler(-90, 0, 0));
		skullLegBigAura.transform.SetParent(player.transform);

		Vector3 origionCameraPosOffset      = camaraFollowTarget.GetPositionOffset();
		Vector3 origionCameraRotationOffset = camaraFollowTarget.GetRotationOffset();

		camaraFollowTarget.SetOffset(zoomOutOffset);
		camaraFollowTarget.SetRotation(new Vector3(63, 0, 0 ));

		await UniTask.Delay(TimeSpan.FromSeconds(1.0f));
		skullLegAttackAura = Instantiate(skullLegAttackAreaVFX, player.position + skullLegAttackAuraOffset, Quaternion.Euler(-90, 0, 0));

		await UniTask.Delay(TimeSpan.FromSeconds(2.0f));
		player.GetComponent<PlayerController>().SetMovable(true);
		camaraFollowTarget.SetOffset(origionCameraPosOffset);
		camaraFollowTarget.SetRotation(origionCameraRotationOffset);
	}
}