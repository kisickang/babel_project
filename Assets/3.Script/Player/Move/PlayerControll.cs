using System.Collections;
using UnityEngine;

public class PlayerControll : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;

    [Header("Attack Settings")]
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float attackInterval = 1.5f;
    [SerializeField] private GameObject attackEffectInstance;
    [SerializeField] private Transform attackEffectSpawnPoint;

    [Header("Attack Area (Fan Shape)")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackAngle = 60f;

    [Header("Animation Target")]
    [SerializeField] private Animator spriteAnimator;

    [Header("Mouse Flip")]
    [SerializeField] private Transform spriteTransform;
    [SerializeField] private Camera targetCamera;

    private bool isAttacking = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (targetCamera == null)
            targetCamera = Camera.main;

        if (spriteTransform == null)
            spriteTransform = transform;

        if (spriteAnimator == null)
            spriteAnimator = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        StartCoroutine(AutoAttackRoutine());
    }

    void Update()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        Vector3 mouseWorld = targetCamera.ScreenToWorldPoint(Input.mousePosition);
        float direction = mouseWorld.x < transform.position.x ? 1f : -1f;
        spriteTransform.localScale = new Vector3(direction, spriteTransform.localScale.y, spriteTransform.localScale.z);
    }

    void FixedUpdate()
    {
        rb.velocity = moveInput.normalized * moveSpeed;
    }

    private IEnumerator AutoAttackRoutine()
    {
        isAttacking = true;

        while (isAttacking)
        {
            if (spriteAnimator != null)
                spriteAnimator.SetTrigger("Attack");

            yield return new WaitForSeconds(attackInterval);
        }
    }

    public void ApplyAttack()
    {
        Debug.Log($"공격 발생! 데미지: {attackDamage}");

        if (attackEffectInstance && attackEffectSpawnPoint)
        {
            float facingDirection = Mathf.Sign(spriteTransform.localScale.x);
            Quaternion effectRotation = (facingDirection > 0)
                ? Quaternion.identity
                : Quaternion.Euler(0, 180f, 0);

            attackEffectInstance.transform.SetPositionAndRotation(attackEffectSpawnPoint.position, effectRotation);
            attackEffectInstance.SetActive(true);
            StartCoroutine(DisableEffectAfterSeconds(0.5f));
        }

        PerformFanShapedAttack(); // ✅ 공격 판정 호출
    }

    private void PerformFanShapedAttack()
    {
        Vector2 origin = attackPoint.position;
        Vector2 forward = attackPoint.right;

        Collider2D[] hits = Physics2D.OverlapCircleAll(origin, attackRange);

        foreach (var hit in hits)
        {
            // 부모에 Monster 스크립트가 있는지 확인
            Monster monster = hit.GetComponentInParent<Monster>();
            if (monster != null)
            {
                Vector2 toTarget = (Vector2)hit.transform.position - origin;
                float angle = Vector2.Angle(forward, toTarget);

                if (angle <= attackAngle / 2f)
                {
                    Debug.Log($"[히트] {hit.name} 몬스터가 부채꼴 범위에 감지됨!");
                    monster.TakeDamage(attackDamage);
                }
            }
        }
    }


    private IEnumerator DisableEffectAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if (attackEffectInstance != null)
            attackEffectInstance.SetActive(false);
    }

    public void BasicAttack_SFX()
    {
        Debug.Log("공격 사운드 실행");
        // AudioManager.Instance.Play("Attack"); 등으로 연동 가능
    }
}
