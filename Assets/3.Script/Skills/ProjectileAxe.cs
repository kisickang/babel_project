using UnityEngine;

public class ProjectileAxe : MonoBehaviour
{
    [SerializeField] private Transform spriteGroup; // ← Sprite 그룹만 회전
    [SerializeField] private Transform trailFX;

    private Vector2 direction;
    private float speed;
    private float damage;
    private float lifetime = 1.5f;
    private float rotateSpeed = 720f;
    private Rigidbody2D rb;

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

        if (trailFX != null)
        {
            trailFX.position = transform.position;

            // 🔥 방향 회전 적용 (Z축 회전)
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
            trailFX.position = spriteGroup.position; // 따라붙게
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        Monster monster = collision.GetComponent<Monster>();
        if (monster != null)
        {
            monster.TakeDamage((int)damage);
            DisableSelf();
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
    }
}
