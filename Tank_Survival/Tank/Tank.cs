// # System
using System.Collections;
using System.Collections.Generic;

// # Unity
using UnityEngine;

public class Tank : MonoBehaviour
{
    [SerializeField]
    private GameObject      crashCheckColliderObject;

    [Header("Tank Attack Value")]
    [SerializeField]
    private float           defaultAttackCooltime;

    [Header("Tank Missile Settings")]
    [SerializeField]
    private GameObject      missileAttackEffect;
    [SerializeField]
    private GameObject      missileFlashEffect;

    [Header("Tank Part Data")]
    [SerializeField]
    private TankBase        tankBase;
    [SerializeField]
    private TankCannon      tankCannon;

    private bool            isAttack;

    private GameObject      tankBaseObject;
    private GameObject      tankCannonObject;

    private Transform       missileFlashSpawnPos;

    private TankStat        tankStat;
    private TankSpin        tankSpin;
    private UnitDetector    unitDetector;

    private WaitForSeconds  attackCooltime;

    private void Start()
    {
        attackCooltime = new WaitForSeconds(defaultAttackCooltime);

        isAttack     = true;

        tankStat     = GetComponent<TankStat>();
        tankSpin     = GetComponent<TankSpin>();
        unitDetector = GetComponent<UnitDetector>();

        UpdateTank();
    }

    private void Update()
    {
        AttackDetectedUnit();
    }

    #region 탱크 파츠 관련 함수 
    public void InitializePartDatas(TankBase baseData, TankCannon cannonData)
    {
        this.tankBase   = baseData;
        this.tankCannon = cannonData;
    }

    public void UpdateTank()
    {
        // 탱크 파츠 데이터에 맞게 오브젝트 업데이트 
        UpdatePart();

        tankStat.InitilaizeStat(tankBase.hp, tankCannon.damage, tankBase.movementSpeed);
        tankSpin.InitilizeCannon(tankCannonObject.transform);

        missileFlashSpawnPos = GameObject.FindWithTag("FlashSpawnPos").transform;
    }

    public void UpdatePart()
    {   
        // Tank Base Generate
        if(tankBaseObject != null) Destroy(tankBaseObject);

        GameObject baseObject = Instantiate(tankBase.tankBase, Vector3.zero, Quaternion.identity);

        tankBaseObject = baseObject;
        baseObject.gameObject.name = "base";
        baseObject.transform.position += tankBase.positionOffset;
        baseObject.transform.localScale = tankBase.tankBaseScale;

        baseObject.transform.SetParent(this.transform, false);

        // Tank Cannon Generate
        if (tankCannonObject != null) Destroy(tankCannonObject);

        GameObject cannonObject = Instantiate(tankCannon.tankCannon, Vector3.zero, Quaternion.identity);

        tankCannonObject = cannonObject;
        cannonObject.gameObject.name = "cannon";
        cannonObject.transform.position += tankCannon.positionOffset;
        cannonObject.transform.localScale = tankCannon.tankCannonScale;

        cannonObject.transform.SetParent(this.transform, false);

        // Crash Check Collider Init
        crashCheckColliderObject.transform.localPosition = tankBase.crashCheckColliderPosition;
        crashCheckColliderObject.GetComponent<BoxCollider>().size = tankBase.crashCheckColliderSize;

        // Cannon Attack Object Init
        missileAttackEffect = tankCannon.missileAttackEffect;
    }
    #endregion

    private void AttackDetectedUnit()
    {
        if (!isAttack || tankSpin.IsSpin) return;

        isAttack = false;
        StartCoroutine(UpdateAttackCooltimeCoroutine());

        // 미사일 이펙트 스폰 위치
        if (unitDetector.DetectedUnit == null) return;

        Vector3 spawnPos = unitDetector.DetectedUnit.transform.position;
        spawnPos.y = 0.4f;

        // 미사일 생성
        GameObject flashEffectClone = Instantiate(missileFlashEffect, missileFlashSpawnPos.position, Quaternion.identity);
        flashEffectClone.transform.SetParent(this.transform);

        GameObject missileClone = Instantiate(missileAttackEffect, spawnPos, Quaternion.identity);
        missileClone.GetComponent<MissileEffect>().Initialize(tankStat.CannonDamage);
    }

    private IEnumerator UpdateAttackCooltimeCoroutine()
    {
        yield return attackCooltime;
        isAttack = true;
    }
}
