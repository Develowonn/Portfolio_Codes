using UnityEngine;

using DG.Tweening;

public class JellyFish : MonsterBase
{
    private Rigidbody2D rigid;

    protected override void Awake()
    {
        base.Update();
        rigid = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Initilalize();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void Initilalize()
    {
        currentHp = maxHp;
        Debug.Log("Init");
    }

    protected override void OnStateIdle()
    {
        if(Physics2D.OverlapCircle(transform.position, 5f))
        {
            state = MonsterState.Move;
        }
    }

    protected override void OnStateMove()
    {
        Debug.Log("Move");
    }

    protected override void OnStateAttack()
    {
        Debug.Log("Attack");
    }

    protected override void OnStateDie()
    {
        if (isDead) return;

        Fade(0, 0.6f).OnComplete(() =>
        {
            Destroy(gameObject);
        });
        isDead = true;
    }
}
