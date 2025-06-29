using UnityEngine;

public class EXP : MonoBehaviour
{
    public ExpDropType type; // Small, Medium, Large
    public int expValue = 1;

    private Transform player;
    private bool isAttracted = false;
    private bool isCollected = false;

    [SerializeField] private float attractRange = 2f;     // 흡수 시작 거리
    [SerializeField] private float moveSpeed = 5f;        // 흡수 이동 속도

    void OnEnable()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        isAttracted = false;
        isCollected = false;
    }

    void Update()
    {
        if (player == null || isCollected) return;

        float dist = Vector2.Distance(transform.position, player.position);

        if (!isAttracted && dist <= attractRange)
        {
            isAttracted = true;
        }

        if (isAttracted)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);

            if (dist < 0.1f)
            {
                isCollected = true;

                PlayerStatus status = player.GetComponent<PlayerStatus>();
                if (status != null)
                    status.AddExp(expValue);

                ExpPoolManager.Instance.ReturnToPool(type, gameObject);
            }
        }
    }
}
