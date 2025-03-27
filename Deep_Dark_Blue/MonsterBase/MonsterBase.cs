using DG.Tweening;
using System.Collections.Generic;
using System.Collections;

using UnityEngine;

public abstract class MonsterBase : MonoBehaviour
{
    [SerializeField]
    protected MonsterState  state;
    [SerializeField]
    protected MonsterType   type;
    [SerializeField]
    protected int           maxHp;
    protected int           currentHp;
    [SerializeField]    
    protected int           attackPower;
    [SerializeField]
    protected float         moveSpeed;
    [SerializeField]
    protected float         noDotDamageTime = 1;

    protected bool          isDead;
    protected bool          isNoDotDamage;

    protected List<SpriteRenderer> spriteRendererList;
    protected Collider2D Collider { get; private set; }

    public MonsterType MonsterType => type;

    protected virtual void Awake()
    {
        Collider = GetComponent<Collider2D>();
        spriteRendererList = new List<SpriteRenderer>();
        GetComponentsInChildren(spriteRendererList);
    }

    protected virtual void Update()
    {
        if (isDead) return;

        switch (state)
        {
            case MonsterState.Idle:
                OnStateIdle();
                break;

            case MonsterState.Move:
                OnStateMove();
                break;

            case MonsterState.Attack:
                OnStateAttack();
                break;

            case MonsterState.Die:
                OnStateDie();
                break;
        }
    }

    protected abstract void Initilalize();
    protected abstract void OnStateIdle();
    protected abstract void OnStateMove();
    protected abstract void OnStateAttack();

    public virtual void TakeDamage(int damage)
    {
        currentHp -= damage;
        currentHp = Mathf.Clamp(currentHp, 0, maxHp);

        if (currentHp == 0)
        {
            state = MonsterState.Die;
        }
        AudioManager.Instance.PlaySound(EAudioName.Hit, transform, transform.position);
        UIManager.Instance.CreateDamageText(damage, 7, transform.position + (Vector3)Random.insideUnitCircle * 0.2f);
    }

    public virtual void TakeDotDamage(int damage)
    {
        if (isNoDotDamage) return;
        isNoDotDamage = true;

        currentHp -= damage;
        currentHp = Mathf.Clamp(currentHp, 0, maxHp);

        if (currentHp <= 0)
        {
            Collider.enabled = false;
            state = MonsterState.Die;
        }

        StartCoroutine(InvincibleInDotDamage(noDotDamageTime));
        AudioManager.Instance.PlaySound(EAudioName.Hit, transform, transform.position);
        UIManager.Instance.CreateDamageText(damage, 7, transform.position + (Vector3)Random.insideUnitCircle * 0.2f);
    }

    private IEnumerator InvincibleInDotDamage(float time)
    {
        yield return new WaitForSeconds(time);
        isNoDotDamage = false;
    }

    public virtual Tween Fade(float alpha, float time)
    {
        Tween fadeTween = null;
        spriteRendererList.ForEach(renderer =>
        {
            renderer.DOKill();
            if (fadeTween == null)
                fadeTween = renderer.DOColor(new Color(1, 1, 1, alpha), time);
            else
                renderer.DOColor(new Color(1, 1, 1, alpha), time);
        });

        return fadeTween;
    }
    protected abstract void OnStateDie();
}
