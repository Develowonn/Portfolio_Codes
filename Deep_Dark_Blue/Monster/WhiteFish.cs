// # System
using System.Collections;

// # Unity
using UnityEngine;

using DG.Tweening;

public class WhiteFish : MonsterBase
{
    [SerializeField]
    private string   lightTag = "Light";
    [SerializeField]
    private float    stunDuration;
    [SerializeField]
    private float    lifeTime;
    [SerializeField]
    private int[]    angle = { 15, 130, 223, 330 };

    private bool     isStun;

    private Animator animator;
    private int      stunHash = Animator.StringToHash("IsStun");

    private void Start()
    {
        Initilalize();
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void TakeDamage(int damage)
    {
        currentHp -= damage;

        if(currentHp <= 0)
        {
            state = MonsterState.Die; 
        }
    }

    protected override void Initilalize()
    {
        currentHp          = maxHp;
        transform.rotation = Quaternion.Euler(0, 0, angle[Random.Range(0, angle.Length)]);

        animator           = GetComponentInChildren<Animator>();
    }
    protected override void OnStateIdle()
    {
        if(isStun)
        {
            animator.SetBool(stunHash, true);
            StartCoroutine(GetHitByLightCoroutine());
        }
    }

    protected override void OnStateMove()
    {
        transform.Translate(Vector3.right * moveSpeed * Time.deltaTime, Space.Self);
    }

    protected override void OnStateAttack()
    {
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

    private IEnumerator GetHitByLightCoroutine()
    {
        isStun = false;
        yield return CoroutineHelper.WaitForSeconds(stunDuration);

        Destroy(this, lifeTime);
        animator.SetBool(stunHash, false);
        state = MonsterState.Move;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag(lightTag))
        {
            isStun = true;
        }
    }
}
