using System.Collections;
using UnityEngine;

public class PlayerControll : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;

    [Header("Attack Settings")]
    [SerializeField] private int attackDamage = 100;
    [SerializeField] private float attackInterval = 3f;
    [SerializeField] private GameObject attackEffectInstance;
    [SerializeField] private Transform attackEffectSpawnPoint;

    [Header("Attack Area (Fan Shape)")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange = 5f;
    [SerializeField] private float attackAngle = 60f;
    [SerializeField] private GameObject fanVisualPrefab; // <-- 수정됨

    [Header("Animation Target")]
    [SerializeField] private Animator spriteAnimator;

    [Header("Mouse Flip")]
    [SerializeField] private Transform spriteTransform;
    [SerializeField] private Camera targetCamera;

    private bool isAttacking = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (targetCamera == null) targetCamera = Camera.main;
        if (spriteTransform == null) spriteTransform = transform;
        if (spriteAnimator == null) spriteAnimator = GetComponentInChildren<Animator>();
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

            ApplyAttack();
            yield return new WaitForSeconds(attackInterval);
        }
    }

    public void ApplyAttack()
    {
        Debug.Log($"[AttackLog] 공격 발생! 데미지: {attackDamage}");
        PerformFanShapedAttack();
        StartCoroutine(PlayFanVisual());

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
    }

    private void PerformFanShapedAttack()
    {
        if (attackPoint == null) return;

        Vector2 origin = attackPoint.position;
        Vector2 forward = attackPoint.right;
        Collider2D[] hits = Physics2D.OverlapCircleAll(origin, attackRange);

        foreach (var hit in hits)
        {
            Monster monster = hit.GetComponentInParent<Monster>();
            if (monster != null)
            {
                Vector2 toTarget = (Vector2)hit.transform.position - origin;
                float angle = Vector2.Angle(forward, toTarget);

                if (angle <= attackAngle / 2f)
                {
                    Debug.Log($"[Hit] {hit.name} 몬스터가 부채꼴 범위에 감지됨!");
                    monster.TakeDamage(attackDamage);
                }
            }
        }
    }

    private IEnumerator PlayFanVisual()
    {
        if (fanVisualPrefab == null || attackPoint == null) yield break;

        Vector3 mouseWorld = targetCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = (mouseWorld - attackPoint.position).normalized;
        float angleZ = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        GameObject visual = Instantiate(fanVisualPrefab, attackPoint.position, Quaternion.Euler(0, 0, angleZ));
        FanMesh mesh = visual.GetComponent<FanMesh>();
        if (mesh != null)
        {
            mesh.radius = attackRange; // 자동 반영됨
            mesh.angle = attackAngle;
            mesh.duration = 0.25f;
            mesh.GenerateFan();
        }

        yield return null;
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
    }
}