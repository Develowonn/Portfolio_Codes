// # System
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

// # Unity
using UnityEngine;

public class Anglerfish : MonsterBase
{
    [Space(10)]
    [SerializeField]
    private float      attackDetectionRange;
    [SerializeField]
    private Transform  attackDetectionPosition;

    [Space(10)]
    [SerializeField]
    private float      moveDetectionRange;

    [Space(10)]
    [SerializeField]
    private Transform  itemTag;
    [SerializeField]
    private GameObject item;
    [SerializeField]
    private LayerMask  layerMask;
    [SerializeField]
    private float      turnCooltime;

    private Vector3    defaultPosition;
    private Collider2D targetCollider;
    private Coroutine  attackCoroutine;
    private Coroutine  homeCoroutine;

    private bool       isDetect;
    private bool       isAttackDetect;
    private bool       isMoveTect;

    private Animator   animator;

    #region Animator Hashcode
    private int attackHash = Animator.StringToHash("OnAttack");
    private int moveHash   = Animator.StringToHash("IsMove");
    #endregion

    private void Start()
    {
        Initilalize();
    }

    protected override void Update()
    {
        base.Update();
    }

    private void FixedUpdate()
    {
        targetCollider = Physics2D.OverlapCircle(attackDetectionPosition.position, attackDetectionRange, layerMask);
    }

    protected override void Initilalize()
    {
        currentHp       = maxHp;
        defaultPosition = transform.position;

        animator        = GetComponent<Animator>();
    }

    protected override void OnStateIdle()
    {
        if (homeCoroutine != null) return;
             
        isMoveTect     = Physics2D.OverlapCircle(transform.position, moveDetectionRange, layerMask);
        isAttackDetect = Physics2D.OverlapCircle(attackDetectionPosition.position, attackDetectionRange, layerMask);

        if (isMoveTect && !animator.GetBool(moveHash))
        {
            animator.SetBool(moveHash, true);
            state = MonsterState.Move;
        }
        else if (isAttackDetect && !animator.GetBool(attackHash))
        {
            state = MonsterState.Attack;
        }
        else if (!isMoveTect && !isAttackDetect)
        {
            homeCoroutine = null;

            animator.SetBool(moveHash, false);
            StartCoroutine(GoHomeCoroutine());

            state = MonsterState.Idle;
        }
    }

    protected override void OnStateMove()
    {
        isAttackDetect = Physics2D.OverlapCircle(attackDetectionPosition.position, attackDetectionRange, layerMask);
        isMoveTect     = Physics2D.OverlapCircle(transform.position, moveDetectionRange, layerMask);

        if (isAttackDetect && !animator.GetBool(attackHash))
        {
            state = MonsterState.Attack;
            return;
        }
        else if (!isMoveTect && animator.GetBool(moveHash))
        {
            animator.SetBool(moveHash, false);
            StartCoroutine(GoHomeCoroutine());

            state = MonsterState.Idle;
            return;
        }

        Vector3 direction = (GameManager.Instance.Player.transform.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;

        float angle        = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float clampedAngle = Mathf.Clamp(angle, -70f, 70f);

        Quaternion targetRotation = Quaternion.Euler(0, (direction.x > 0 ? 180 : 0), -clampedAngle);
        transform.rotation        = Quaternion.RotateTowards(transform.rotation, targetRotation, 200f * Time.deltaTime);
    }

    protected override void OnStateAttack()
    {
        isAttackDetect = Physics2D.OverlapCircle(attackDetectionPosition.position, attackDetectionRange, layerMask);
        isMoveTect     = Physics2D.OverlapCircle(transform.position, moveDetectionRange, layerMask);

        if (attackCoroutine == null && targetCollider != null)
        {
            attackCoroutine = StartCoroutine(AttackCoroutine());
        }
    }

    private IEnumerator GoHomeCoroutine()
    {
        float moveSpeed = this.moveSpeed * 0.1f;

        while (Vector3.Distance(transform.position, defaultPosition) >= 0.1f)
        {
            Vector3 direction = (defaultPosition - transform.position).normalized;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            float clampedAngle = Mathf.Clamp(angle, -70f, 70f);
            Quaternion targetRotation = Quaternion.Euler(0, (direction.x > 0 ? 180 : 0), -clampedAngle);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 200f * Time.deltaTime);

            transform.position += direction * moveSpeed * Time.deltaTime;
            yield return null;
        }

        transform.rotation = Quaternion.Euler(0, 0, 0);
        homeCoroutine      = null;
    }

    private IEnumerator AttackCoroutine()
    {
        float y = (transform.rotation.y > 0.1f) ? 180 : 0;
        transform.rotation = Quaternion.Euler(0, y, transform.rotation.eulerAngles.z);

        animator.SetTrigger(attackHash);

        float currentAnimationTime = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return CoroutineHelper.WaitForSeconds(currentAnimationTime + 1.0f);

        if (targetCollider != null)
        {
            AudioManager.Instance.PlaySound(EAudioName.BigBite, transform, transform.position);
            GameManager.Instance.Player.GetComponent<Player>().TakeDamage(attackPower);
        }

        state           = MonsterState.Move;
        attackCoroutine = null;
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (attackDetectionPosition != null) 
            Gizmos.DrawWireSphere(attackDetectionPosition.position, attackDetectionRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, moveDetectionRange);
    }
}
