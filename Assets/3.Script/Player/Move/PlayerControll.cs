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
    [SerializeField] private GameObject attackEffectPrefab;
    [SerializeField] private Transform attackEffectSpawnPoint;

    [Header("Animation Target")]
    [Tooltip("실제 애니메이션이 재생되는 Sprite의 Animator")]
    [SerializeField] private Animator spriteAnimator; // <- 여기!

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

        // Animator가 null이면 자식에서 자동으로 찾도록 fallback
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

    // Sprite 오브젝트에서 호출되는 함수니까 [MessageReceiver] 역할
    public void ApplyAttack()
    {
        Debug.Log($"공격 발생! 데미지: {attackDamage}");

        if (attackEffectPrefab && attackEffectSpawnPoint)
        {
            // 현재 방향 기준 (scale.x로 판단)
            float facingDirection = Mathf.Sign(spriteTransform.localScale.x);

            // 회전 방향 적용 (Y축 반전으로 Flip)
            Quaternion effectRotation = (facingDirection > 0)
                ? Quaternion.identity // 오른쪽: 기본 회전
                : Quaternion.Euler(0, 180f, 0); // 왼쪽: Y축 반전

            Instantiate(attackEffectPrefab, attackEffectSpawnPoint.position, effectRotation);
        }
    }


    public void BasicAttack_SFX()
    {
        Debug.Log("공격 사운드 실행");
        // AudioManager.Instance.Play("Attack"); 등으로 연동 가능
    }
}
