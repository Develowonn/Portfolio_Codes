// # System
using System.Collections;

// # Unity
using UnityEngine;

using DG.Tweening;

public class GoblinShark : MonsterBase
{
    [Header("공격 감지 범위 설정")]
    [SerializeField]
    private float       attackDetectionRange;
    [SerializeField]
    private Transform   attackDetectionPosition;
    [SerializeField]
    private float       attackCooltime;

    [Header("감지 범위 값 설정")]
    [SerializeField]
    private float       glowFishDetectRange;
    [SerializeField]
    private LayerMask   attackLayerMask;
    [SerializeField]
    private LayerMask   glowFishLayerMask;

    [Header("대쉬 값 설정")]
    [SerializeField]
    private float       dashCoolTime;
    [SerializeField]
    private float       dashSpeedMultiple;

    private Vector3     defaultPosition;
    private Collider2D  glowFish;
    private Coroutine   attackCoroutine;
    private Coroutine   homeCoroutine;

    private Animator    animator;
    private int         attackHash = Animator.StringToHash("OnAttack");
    private int         dashHash   = Animator.StringToHash("OnDash");
    private int         moveHash   = Animator.StringToHash("IsMove");

    private bool        isAttackDetect;
    private bool        isAttackCooltime;
    private bool        isDash;
    private bool        isFindGlowFish;

    private void Start()
    {
        Initilalize();
    }
    protected override void Initilalize()
    {
        currentHp       = maxHp;
        defaultPosition = transform.position;

        animator = GetComponentInChildren<Animator>();
        StartCoroutine(WaitDashCooltimeCoroutine(5.0f));
    }

    protected override void Update()
    {
        base.Update();
    }

    private void FixedUpdate()
    {
        Collider2D[] temp     = Physics2D.OverlapCircleAll(transform.position, glowFishDetectRange, glowFishLayerMask);

        if(!isFindGlowFish && temp != null)
        {
            foreach(var fish in temp)
            {
                if (fish.GetComponentInChildren<GlowFish>() != null && fish.GetComponentInChildren<GlowFish>().IsNoticed)
                {
                    glowFish       = fish;
                    isFindGlowFish = true;
                    break;
                }
            }
        }
    }

    protected override void OnStateIdle()
    {
        if (homeCoroutine != null) return;

        if(glowFish != null && isFindGlowFish)
        {
            animator.SetBool(moveHash, true);
            state = MonsterState.Move;
        }
    }

    protected override void OnStateMove()
    {
        if (glowFish == null || glowFish.transform == null)
        {
            glowFish       = null;
            isFindGlowFish = false;
            homeCoroutine  = StartCoroutine(nameof(GoHomeCoroutine));

            animator.SetBool(moveHash, false);
            state = MonsterState.Idle;
            return;
        }

        isAttackDetect = Physics2D.OverlapCircle(attackDetectionPosition.position, attackDetectionRange, attackLayerMask);
        if(!isAttackCooltime && isAttackDetect! && !animator.GetBool(attackHash))
        {
            state = MonsterState.Attack;
        }

        Vector3 direction = (glowFish.transform.position - transform.position).normalized;
        if (isDash)
        {
            animator.SetTrigger(dashHash);
            StartCoroutine(DashCoroutine());
            StartCoroutine(WaitDashCooltimeCoroutine(dashCoolTime));
            isDash = false;
        }

        transform.position += direction * moveSpeed * Time.deltaTime;

        // 회전 시키기
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float clampedAngle = Mathf.Clamp(angle, -70f, 70f);

        Quaternion targetRotation = Quaternion.Euler(0, (direction.x > 0 ? 180 : 0), -clampedAngle);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 200f * Time.deltaTime);
    }

    protected override void OnStateAttack()
    {
        isAttackDetect = Physics2D.OverlapCircle(attackDetectionPosition.position, attackDetectionRange, attackLayerMask);

        if (attackCoroutine == null && glowFish != null)
        {
            isAttackCooltime = true;
            attackCoroutine  = StartCoroutine(nameof(AttackCoroutine));
        }
    }

    private IEnumerator DashCoroutine()
    {
        float defaultMoveSpeed = moveSpeed;
        moveSpeed             *= dashSpeedMultiple;
        yield return new WaitForSeconds(0.5f);
        moveSpeed              = defaultMoveSpeed;
    }

    private IEnumerator WaitDashCooltimeCoroutine(float time)
    {
		yield return CoroutineHelper.WaitForSeconds(time);
		isDash = true;
    }
    private IEnumerator WaitAttackCooltimeCoroutine(float time)
    {
		yield return CoroutineHelper.WaitForSeconds(time);
		isAttackCooltime = false;
    }


    private IEnumerator AttackCoroutine()
    {
        float y = (GameManager.Instance.Player.transform.position.x < transform.position.x) ? 0 : 180;
        transform.rotation = Quaternion.Euler(0, y, transform.rotation.eulerAngles.z);

        animator.SetTrigger(attackHash);

        float currentAnimationTime = animator.GetCurrentAnimatorStateInfo(0).length;
		yield return CoroutineHelper.WaitForSeconds(currentAnimationTime + 1.0f);

		Collider2D[] objects = Physics2D.OverlapCircleAll(attackDetectionPosition.position, attackDetectionRange, attackLayerMask);
        if (objects != null)
        {
            foreach(var obj in objects)
            {
                if(obj.CompareTag("Monster"))
                {
                    AudioManager.Instance.PlaySound(EAudioName.Bite, transform, transform.position);
                    MonsterBase monster = obj.GetComponent<MonsterBase>();

                    if(monster != null)
                    {
                        monster.TakeDamage(attackPower);
                    }
                    else
                    {
                        obj.GetComponentInChildren<MonsterBase>().TakeDamage(attackPower);
                    }
                }
                
                if(obj.CompareTag("Player"))
                {
                    GameManager.Instance.Player.GetComponent<Player>().TakeDamage(attackPower);
                }
            }
        }

        attackCoroutine = null;
        StartCoroutine(WaitAttackCooltimeCoroutine(attackCooltime));
        state           = MonsterState.Move;
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

    private IEnumerator GoHomeCoroutine()
    {
        float moveSpeed = this.moveSpeed * 0.5f;

        while (Vector3.Distance(transform.position, defaultPosition) >= 0.1f)
        {
            Vector3 direction = (defaultPosition - transform.position).normalized;

            // 회전 시키기
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            float clampedAngle = Mathf.Clamp(angle, -70f, 70f);
            Quaternion targetRotation = Quaternion.Euler(0, (direction.x > 0 ? 180 : 0), -clampedAngle);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 200f * Time.deltaTime);

            // 움직임
            transform.position += direction * moveSpeed * Time.deltaTime;
            yield return null;
        }

        transform.rotation = Quaternion.Euler(0, 0, 0);
        homeCoroutine = null;
    }

    public override void TakeDamage(int damage)
    {
        currentHp -= damage;

        if(currentHp <= 0)
        {
            state = MonsterState.Die;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, glowFishDetectRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackDetectionPosition.position, attackDetectionRange);
    }
}
