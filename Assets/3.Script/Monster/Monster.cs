using UnityEngine;
using System.Collections.Generic;

public class Monster : MonoBehaviour
{
    [Header("몬스터 데이터")]
    [SerializeField] private MonsterData data;

    [Header("그래픽 그룹")]
    [Tooltip("Group_Sprite Transform을 할당하세요.")]
    [SerializeField] private Transform spriteGroup;

    private Transform player;
    private Rigidbody2D rb;

    // ↓ 스프라이트 정렬 관련
    private SpriteRenderer[] spriteRenderers;
    private Dictionary<SpriteRenderer, int> baseOrderOffsets = new();

    private int currentHealth;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // SpriteRenderer 추적
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        foreach (var sr in spriteRenderers)
        {
            baseOrderOffsets[sr] = sr.sortingOrder;
        }

        if (spriteGroup == null)
            Debug.LogWarning("[Monster] spriteGroup이 비어 있습니다. Group_Sprite를 연결해주세요.");

        if (data == null)
        {
            Debug.LogError("[Monster] MonsterData가 연결되지 않았습니다.");
            enabled = false;
            return;
        }

        currentHealth = data.maxHealth;
    }

    void OnEnable()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
            else
                Debug.LogError("[Monster] Player를 찾을 수 없습니다. 태그를 'Player'로 지정해주세요.");
        }
    }

    void Update()
    {
        if (player == null) return;

        // 플레이어 방향으로 이동
        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = direction * data.moveSpeed;

        // 좌우 반전 (왼쪽이 기본 방향)
        if (spriteGroup != null)
        {
            float scaleX = (player.position.x > transform.position.x) ? -1f : 1f;
            spriteGroup.localScale = new Vector3(scaleX, 1f, 1f);
        }
    }

    void LateUpdate()
    {
        // y 좌표 기준으로 order 정렬
        int baseOrder = -(int)(transform.position.y * 100);
        foreach (var sr in spriteRenderers)
        {
            sr.sortingOrder = baseOrder + baseOrderOffsets[sr];
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"플레이어에게 {data.damage} 데미지 입힘");

            // 데미지 로직 추가 시:
            // other.GetComponent<PlayerHealth>()?.TakeDamage(data.damage);
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"[{data.monsterName}] 사망");

        // 드랍 아이템 생성
        if (data.dropItems != null)
        {
            foreach (var item in data.dropItems)
            {
                if (item != null)
                    Instantiate(item, transform.position, Quaternion.identity);
            }
        }

        // 오브젝트 풀로 되돌리거나 파괴
        gameObject.SetActive(false);
        // 또는 Destroy(gameObject);
    }
}
