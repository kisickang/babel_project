using UnityEngine;

public class Monster : MonoBehaviour
{
    [Header("데이터 설정")]
    [SerializeField] private MonsterData data; // 직접 연결

    [Header("그래픽")]
    [SerializeField] private Transform spriteGroup;

    [Header("드롭 프리팹")]
    [SerializeField] private GameObject expSmallPrefab;
    [SerializeField] private GameObject expMediumPrefab;
    [SerializeField] private GameObject expLargePrefab;

    [SerializeField] private GameObject damageUIPrefab;

    private Transform player;
    private Rigidbody2D rb;
    private int currentHP;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (spriteGroup == null)
            Debug.LogWarning("[Monster] spriteGroup 미지정");

        if (data == null)
        {
            Debug.LogError("[Monster] MonsterData가 연결되지 않았습니다.");
            enabled = false;
            return;
        }
    }

    void OnEnable()
    {
        currentHP = data.maxHealth;

        if (player == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p) player = p.transform;
        }
    }

    void Update()
    {
        if (player == null) return;

        Vector2 dir = (player.position - transform.position).normalized;
        rb.velocity = dir * data.moveSpeed;

        if (spriteGroup != null)
        {
            float scaleX = (player.position.x > transform.position.x) ? -1f : 1f;
            spriteGroup.localScale = new Vector3(scaleX, 1f, 1f);
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStatus playerStatus = other.GetComponent<PlayerStatus>();
            if (playerStatus != null)
            {
                playerStatus.TakeDamage(data.damage); // ✅ 이제 data.damage 사용
                Debug.Log($"[Monster] 플레이어에게 {data.damage} 데미지 줌!");
            }
        }
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;

        Vector3 popupPos = GetPopupPosition();
        GameObject popup = Instantiate(damageUIPrefab, popupPos, Quaternion.identity);
        popup.GetComponent<DamagePopup>()?.Show(damage);

        if (currentHP <= 0)
            Die();
    }

    private void Die()
    {
        DropExp();
        gameObject.SetActive(false); // 오브젝트 풀 복귀
    }

    private void DropExp()
    {
        GameObject prefab = data.dropExpType switch
        {
            ExpDropType.Small => expSmallPrefab,
            ExpDropType.Medium => expMediumPrefab,
            ExpDropType.Large => expLargePrefab,
            _ => null
        };

        if (prefab != null)
            Instantiate(prefab, transform.position, Quaternion.identity);
    }

    private Vector3 GetPopupPosition()
    {
        SpriteRenderer sr = spriteGroup.GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
        {
            Bounds bounds = sr.bounds;
            return new Vector3(bounds.center.x, bounds.max.y + 0.2f, 0f);
        }
        return transform.position + Vector3.up * 1.5f;
    }
}
