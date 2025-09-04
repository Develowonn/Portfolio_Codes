// # System
using System;

// # Unity
using UnityEngine;

// # ETC
using Cysharp.Threading.Tasks;

public class TornadoCard : Card
{
	private GameObject[]    tornados;
	private GameObject		aura;
	private GameObject		particle;
	private GameObject		explosion;

	[SerializeField]
	private int				tornadoCount;
	[SerializeField]
	private float			tornadoSpawnTime;
	[SerializeField]
	private float			radius;
	[SerializeField]
	private float			explosionTime;

	[Header("Offset")]
	[SerializeField]
	private Vector3			auraOffset;
	[SerializeField]
	private Vector3			particleOffset;

	[Header("VFX")]
	[SerializeField]
	private GameObject		auraVfx;
	[SerializeField]
	private GameObject      paritcleVfx;
	[SerializeField]
	private GameObject		tornadoVfx;
	[SerializeField]
	private GameObject		ExplosionVfx;

	private bool			isDone;

	public override void Execute()
	{
		if(isDone) return;

		isDone = true;

		ExecuteSkill().Forget();
	}

	private async UniTask ExecuteSkill()
	{
		SpawnAura();
		await UniTask.Delay(TimeSpan.FromSeconds(0.1f));

		SpawnParticle();
		await UniTask.Delay(TimeSpan.FromSeconds(0.5f));

		SpawnTornado();
		await ActivateTornado();
		await UniTask.Delay(TimeSpan.FromSeconds(explosionTime));

		SpawnExplosion();
		DestroyObject();
	}

	private void SpawnAura()
	{
		aura = Instantiate(auraVfx, InGameManager.Instance.GetPlayerObject().transform.position + auraOffset, Quaternion.Euler(-90, 0, 0));
		aura.transform.SetParent(InGameManager.Instance.GetPlayerObject().transform);
	}

	private void SpawnParticle()
	{
		particle = Instantiate(paritcleVfx, InGameManager.Instance.GetPlayerObject().transform.position + particleOffset, Quaternion.Euler(-90, 0, 0));
		particle.transform.SetParent(InGameManager.Instance.GetPlayerObject().transform);
	}

	private void SpawnExplosion()
	{
		explosion = Instantiate(ExplosionVfx, InGameManager.Instance.GetPlayerObject().transform.position + auraOffset, Quaternion.Euler(-90, 0, 0));
		explosion.transform.SetParent(InGameManager.Instance.GetPlayerObject().transform);
	}

	private void SpawnTornado()
	{
		tornados = new GameObject[tornadoCount];

		for (int i = 0; i < tornadoCount; i++)
		{
			float angle = i * Mathf.PI * 2f / tornadoCount;
			Vector3 offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
			Vector3 spawnPos = InGameManager.Instance.GetPlayerObject().transform.position + offset;

			tornados[i]		 = Instantiate(tornadoVfx, spawnPos, Quaternion.Euler(-90, 0, 0));
			tornados[i].SetActive(false);

			// 토네이도의 움직임 방향 정하기
			Vector3 direction = (spawnPos - InGameManager.Instance.GetPlayerObject().transform.position).normalized;
			tornados[i].GetComponent<MovementRigidbody>().MoveTo(direction);
		}
	}

	private async UniTask ActivateTornado()
	{
		for(int i = 0; i < tornadoCount; i++)
		{
			tornados[i].SetActive(true);
			await UniTask.Delay(TimeSpan.FromSeconds(tornadoSpawnTime));
		}
	}

	private void DestroyObject()
	{
		foreach(GameObject obj in tornados)
		{
			Destroy(obj);
		}

		Destroy(aura);
		Destroy(particle);
	}
}