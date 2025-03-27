using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubPlate : MonoBehaviour
{
    [Header("[ Plate Info ]")]
    public string materialName;
    [Space(5)]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float originMoveSpeed;

    public Transform particleSpawnPos;
    public GameObject particle;

    private Vector3 dirVec;

    private SpriteRenderer materialSpriteRdr;
    private Rigidbody2D rigid;

    private void Awake()
    {
        materialSpriteRdr = GetComponentsInChildren<SpriteRenderer>()[1];
        rigid = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        originMoveSpeed = moveSpeed;
    }

    private void Update()
    {
        if (materialName != FGameManager.instance.currentFood.foodMaterials[FGameManager.instance.materialIndex].materialName)
            Destroy(particle);
    }

    private void FixedUpdate()
    {
        if (GameManager.instance.isPause) moveSpeed = 0;
        else moveSpeed = originMoveSpeed;

        rigid.velocity = dirVec * moveSpeed * Time.fixedDeltaTime;
    }

    public void Init(string name, Sprite sprite)
    {
        this.materialName = name;
        materialSpriteRdr.sprite = sprite;
    }

    public void MoveDirVec(Vector3 dirVec)
    {
        this.dirVec = dirVec;
    }

    // Temp Code
    private void OnTriggerEnter2D(Collider2D trigger)
    {
        FGameManager fGameManager = FGameManager.instance;
        GameManager gameManager = GameManager.instance;

        if (trigger.CompareTag("MainPlate"))
        {
            if (fGameManager.mainPlate.foodMaterial.materialName == materialName)
            {
                fGameManager.mainPlate.foodMaterial.materialCount--;
                if (fGameManager.mainPlate.foodMaterial.materialCount <= 0)
                {
                    // 요리 완성도
                    float maxAlpha = 1.0f; // 최대 투명도
                    float minAlpha = 0.2f; // 최소 투명도
                    float increment = (maxAlpha - minAlpha) / (fGameManager.currentFood.foodMaterials.Length - 1); // 재료를 획득할 때마다 증가할 투명도 값

                    float alpha = minAlpha + increment * fGameManager.materialIndex;
                    alpha = Mathf.Clamp(alpha, minAlpha, maxAlpha); // 투명도가 minAlpha와 maxAlpha 사이에 머무르도록 클램핑
                    fGameManager.mainPlate.spriteRdr.color = new Color(1f, 1f, 1f, alpha);

                    fGameManager.MaterialInit();
                }

                // Particle Instantiate
                GameObject clone = Instantiate(gameManager.goodParticle, gameManager.particleSpawnPos.position, Quaternion.identity);
                clone.transform.SetParent(gameManager.particleSpawnPos);
            }
            else
            {
                GameObject clone = Instantiate(gameManager.badParticle, gameManager.particleSpawnPos.position, Quaternion.identity);
                clone.transform.SetParent(gameManager.particleSpawnPos);
                fGameManager.mainPlate.foodComplete--;
            }
            Destroy(gameObject);
        }
    }
}
