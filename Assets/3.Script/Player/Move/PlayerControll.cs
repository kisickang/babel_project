using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControll : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float baseMoveSpeed = 5f;
    private float moveSpeedMultiplier = 1f;
    private Rigidbody2D rb;
    private Vector2 moveInput;

    [Header("Attack Settings")]
    [SerializeField] private int baseAttackDamage = 100;
    private float attackDamageMultiplier = 1f;

    [SerializeField] private float baseAttackInterval = 3f;
    private float attackIntervalMultiplier = 1f;

    [SerializeField] private GameObject attackEffectInstance;
    [SerializeField] private Transform attackEffectSpawnPoint;

    [Header("Attack Area (Fan Shape)")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float baseAttackRange = 5f;
    private float attackRangeMultiplier = 1f;
    [SerializeField] private float attackAngle = 60f;
    [SerializeField] private GameObject fanVisualPrefab;

    [Header("Animation Target")]
    [SerializeField] private Animator spriteAnimator;

    [Header("Mouse Flip")]
    [SerializeField] private Transform spriteTransform;
    [SerializeField] private Camera targetCamera;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform projectileParent;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private GameObject axeThrowFXObject;
    [SerializeField] private float axeThrowFXDuration = 0.4f;
    [SerializeField] private float projectileSpeed = 10f;

    [SerializeField] private PlayerStatus playerStatus;

    public float specialSkillMultiplier = 1f;

    private bool isAttacking = false;
    private bool isFanEffectRunning = false;
    private Coroutine attackRoutine;

    private int CurrentAttackDamage => Mathf.RoundToInt(baseAttackDamage * attackDamageMultiplier);
    private float CurrentMoveSpeed => baseMoveSpeed * moveSpeedMultiplier;
    private float CurrentAttackInterval => baseAttackInterval * attackIntervalMultiplier;
    private float CurrentAttackRange => baseAttackRange * attackRangeMultiplier;

    void Start()
    {
        if (attackRoutine == null)
            attackRoutine = StartCoroutine(AutoAttackRoutine());
    }

    void OnDisable()
    {
        if (attackRoutine != null)
        {
            StopCoroutine(attackRoutine);
            attackRoutine = null;
        }
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (targetCamera == null) targetCamera = Camera.main;
        if (spriteTransform == null) spriteTransform = transform;
        if (spriteAnimator == null) spriteAnimator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        Vector3 mouseWorld = targetCamera.ScreenToWorldPoint(Input.mousePosition);
        float direction = mouseWorld.x < transform.position.x ? 1f : -1f;
        spriteTransform.localScale = new Vector3(direction, spriteTransform.localScale.y, spriteTransform.localScale.z);

        if (Input.GetMouseButtonDown(1))
            TryUseSkill();
    }

    void FixedUpdate()
    {
        rb.velocity = moveInput.normalized * CurrentMoveSpeed;
    }

    private IEnumerator AutoAttackRoutine()
    {
        isAttacking = true;
        while (isAttacking)
        {
            if (spriteAnimator != null)
                spriteAnimator.SetTrigger("Attack");

            yield return new WaitForSeconds(CurrentAttackInterval);
        }
    }

    public void ApplyAttack()
    {
        //Debug.Log($"[AttackLog] 공격 발생! 데미지: {CurrentAttackDamage}");
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
        Vector2 mouseWorld = targetCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 forward = (mouseWorld - origin).normalized;

        Collider2D[] hits = Physics2D.OverlapCircleAll(origin, CurrentAttackRange);
        HashSet<Monster> damagedMonsters = new HashSet<Monster>();

        foreach (var hit in hits)
        {
            Monster monster = hit.GetComponentInParent<Monster>();
            if (monster != null && !damagedMonsters.Contains(monster))
            {
                Vector2 toTarget = (Vector2)hit.transform.position - origin;
                float angle = Vector2.Angle(forward, toTarget);

                if (angle <= attackAngle / 2f)
                {
                    damagedMonsters.Add(monster);
                    monster.TakeDamage(CurrentAttackDamage);
                    // Debug.Log($"[Hit] {monster.name} 데미지 적용");
                }
            }
        }
    }

    private IEnumerator PlayFanVisual()
    {
        if (isFanEffectRunning) yield break;
        isFanEffectRunning = true;

        if (fanVisualPrefab == null || attackPoint == null) yield break;

        Vector3 mouseWorld = targetCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = (mouseWorld - attackPoint.position).normalized;
        float angleZ = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        GameObject visual = Instantiate(fanVisualPrefab, attackPoint.position, Quaternion.Euler(0, 0, angleZ));
        FanMesh mesh = visual.GetComponent<FanMesh>();
        if (mesh != null)
        {
            mesh.radius = CurrentAttackRange;
            mesh.angle = attackAngle;
            mesh.duration = 0.25f;
            mesh.GenerateFan();
        }

        yield return new WaitForSeconds(0.3f);
        isFanEffectRunning = false;
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

    public void TryUseSkill()
    {
        if (playerStatus.CurrentMP >= 30f)
        {
            playerStatus.currentMP -= 30f;

            Vector3 spawnPos = projectileSpawnPoint.position;
            Vector3 mousePos = targetCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector2 baseDir = (mousePos - spawnPos).normalized;

            float spreadAngle = 30f; // 전체 부채꼴 각도
            float angleStep = (axeCount > 1) ? spreadAngle / (axeCount - 1) : 0f;
            float startAngle = -spreadAngle / 2f;

            for (int i = 0; i < axeCount; i++)
            {
                float angle = startAngle + angleStep * i;
                Vector2 rotatedDir = Quaternion.Euler(0, 0, angle) * baseDir;

                GameObject proj = AxePoolManager.Instance.Get();
                proj.transform.SetPositionAndRotation(spawnPos, Quaternion.identity);
                proj.SetActive(true);

                proj.GetComponent<ProjectileAxe>().Initialize(rotatedDir, projectileSpeed, CurrentAttackDamage * specialSkillMultiplier);
            }

            PlayAxeThrowFX(spawnPos, mousePos);
        }
        else
        {
            Debug.Log("마나 부족!");
        }
    }


    private IEnumerator DisableAxeThrowFXAfterDelay()
    {
        yield return new WaitForSeconds(axeThrowFXDuration);
        if (axeThrowFXObject != null)
            axeThrowFXObject.SetActive(false);
    }

    private void PlayAxeThrowFX(Vector3 spawnPos, Vector3 mouseWorld)
    {
        if (axeThrowFXObject == null) return;

        Vector2 dir = (mouseWorld - spawnPos).normalized;
        float angleZ = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        axeThrowFXObject.transform.rotation = Quaternion.Euler(0, 0, angleZ);
        Vector3 offset = axeThrowFXObject.transform.right * 1.5f;
        Vector3 finalPos = spawnPos + offset;

        axeThrowFXObject.transform.position = finalPos;

        axeThrowFXObject.SetActive(false);
        axeThrowFXObject.SetActive(true);

        StartCoroutine(DisableAxeThrowFXAfterDelay());
    }

    public void IncreaseAttackDamageByPercent(float percent)
    {
        Debug.Log($"[레벨업] 호출됨 - base: {baseAttackDamage}, multiplier: {attackDamageMultiplier}, percent: {percent}");
        attackDamageMultiplier += percent;
        Debug.Log($"[레벨업] 증가 후 → 데미지: {CurrentAttackDamage}, multiplier: {attackDamageMultiplier}");
    }
    public void IncreaseMoveSpeedByPercent(float percent)
    {
        moveSpeedMultiplier += percent;
        Debug.Log($"[레벨업] 이동속도 증가됨 → {CurrentMoveSpeed}");
    }

    public void IncreaseAttackSpeedByPercent(float percent)
    {
        attackIntervalMultiplier *= (1f - percent);
        Debug.Log($"[레벨업] 공격속도 증가됨 (interval: {CurrentAttackInterval})");
    }

    public void IncreaseAttackRangeByPercent(float percent)
    {
        attackRangeMultiplier += percent;
        Debug.Log($"[레벨업] 공격 범위 증가됨 → {CurrentAttackRange}");
    }


    public void IncreaseSpecialSkillPowerByPercent(float percent)
    {
        specialSkillMultiplier += percent;
        Debug.Log($"[레벨업] 특수스킬 배율 증가됨 → {specialSkillMultiplier}");
    }
    public float GetSpecialSkillDamage(float baseDamage)
    {
        return baseDamage * specialSkillMultiplier;
    }
    [SerializeField] private int axeCount = 1; // 레벨업 시 증가
    private const int MaxAxeCount = 10;

    public void IncreaseAxeCount(int amount)
    {
        axeCount = Mathf.Min(axeCount + amount, MaxAxeCount);
        Debug.Log($"[레벨업] 도끼 개수 증가됨 → {axeCount}");
    }
}
