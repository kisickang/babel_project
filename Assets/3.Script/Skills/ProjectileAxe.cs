using System.Collections.Generic;
using UnityEngine;

public class ProjectileAxe : MonoBehaviour
{
    [SerializeField] private Transform spriteGroup;
    [SerializeField] private Transform trailFX;

    private Vector2 direction;
    private float speed;
    private float damage;
    private float lifetime = 1.5f;
    private float rotateSpeed = 720f;
    private Rigidbody2D rb;

    private HashSet<Monster> damagedMonsters = new();  // 💥 중복 히트 방지

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Initialize(Vector2 dir, float moveSpeed, float dmg)
    {
        direction = dir.normalized;
        speed = moveSpeed;
        damage = dmg;

        transform.position = transform.position;

        damagedMonsters.Clear(); // 💥 재사용 시 초기화

        if (trailFX != null)
        {
            trailFX.position = transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            trailFX.rotation = Quaternion.Euler(0f, 0f, angle);

            ParticleSystem ps = trailFX.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                ps.Play();
            }
        }

        rb.velocity = direction * speed;
        Invoke(nameof(DisableSelf), lifetime);
    }

    void Update()
    {
        if (spriteGroup != null)
            spriteGroup.Rotate(Vector3.forward, -rotateSpeed * Time.deltaTime);

        if (trailFX != null)
            trailFX.position = spriteGroup.position;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Monster monster = collision.GetComponentInParent<Monster>();
        if (monster != null && !damagedMonsters.Contains(monster))
        {
            float finalDamage = FindObjectOfType<PlayerControll>().GetSpecialSkillDamage(damage);
            monster.TakeDamage(Mathf.RoundToInt(finalDamage));
            damagedMonsters.Add(monster); // 💥 같은 몬스터 중복 방지
            Debug.Log($"[도끼] {monster.name}에게 {finalDamage} 데미지 관통 적용됨");
        }
    }

    void DisableSelf()
    {
        rb.velocity = Vector2.zero;
        gameObject.SetActive(false);
    }

    void OnDisable()
    {
        CancelInvoke();
        damagedMonsters.Clear(); // 풀링 재사용 시 초기화
    }
}
