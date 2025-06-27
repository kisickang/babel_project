using UnityEngine;

public class EXP : MonoBehaviour
{
    public ExpDropType type; // Small, Medium, Large
    public int expValue = 1; // Inspector에서 설정

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStatus player = other.GetComponent<PlayerStatus>();
            if (player != null)
                player.AddExp(expValue); // 플레이어 경험치 증가

            ExpPoolManager.Instance.ReturnToPool(type, gameObject); // 풀에 반환
        }
    }
}
