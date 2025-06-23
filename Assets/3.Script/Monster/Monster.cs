using UnityEngine;

public class Monster : MonoBehaviour
{
    [Header("몬스터 데이터")]
    [SerializeField] private MonsterData data;

    [Header("그래픽 그룹")]
    [SerializeField] private Transform spriteGroup;

    private Transform player;
    private Rigidbody2D rb;
    private SpriteRenderer[] spriteRenderers;
    private int currentHP;

    private Canvas overlayCanvas;
    [SerializeField] private GameObject damageUIPrefab;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        if (spriteGroup == null)
            Debug.LogWarning("[Monster] spriteGroup이 비어 있습니다.");

        if (data == null)
        {
            Debug.LogError("[Monster] MonsterData 연결 안됨");
            enabled = false;
            return;
        }
    }

    void OnEnable()
    {
        currentHP = data.maxHealth;

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }

        if (overlayCanvas == null)
        {
            overlayCanvas = FindObjectOfType<Canvas>();
            if (overlayCanvas == null || overlayCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
                Debug.LogWarning("Screen Space - Overlay Canvas를 찾을 수 없습니다.");
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"플레이어에게 {data.damage} 데미지 입힘");
        }
    }



public void TakeDamage(int damage)
{
    currentHP -= damage;

    Vector3 popupPos = GetPopupPosition(); // 머리 위 위치 계산
    GameObject popup = Instantiate(damageUIPrefab, popupPos, Quaternion.identity);
    popup.GetComponent<DamagePopup>()?.Show(damage); // 애니메이션 재생

    if (currentHP <= 0)
        Die();
}


    private void Die()
    {
        Debug.Log($"[{data.monsterName}] 사망");

        if (data.dropItems != null)
        {
            foreach (var item in data.dropItems)
            {
                if (item != null)
                    Instantiate(item, transform.position, Quaternion.identity);
            }
        }

        gameObject.SetActive(false); // 풀로 복귀
    }
    private Vector3 GetPopupPosition()
    {
        SpriteRenderer sr = spriteGroup.GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
        {
            Bounds bounds = sr.bounds;
            return new Vector3(bounds.center.x, bounds.max.y + 0.2f, 0f); // 머리 위
        }

        return transform.position + Vector3.up * 1.5f; // fallback
    }
    private Vector3 GetPopupLocalOffset()
    {
        SpriteRenderer sr = spriteGroup.GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
        {
            Bounds bounds = sr.bounds;
            Vector3 localOffset = spriteGroup.InverseTransformPoint(new Vector3(bounds.center.x, bounds.max.y + 0.2f, 0f));
            return localOffset;
        }

        return new Vector3(0f, 1.5f, 0f); // fallback
    }
}
