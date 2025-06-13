using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    [Header("HP Settings")]
    [Tooltip("최대 HP")]
    [SerializeField] private float maxHP = 100f;
    [Tooltip("현재 HP (인스펙터에서 조절 가능)")]
    [Range(0, 100)]
    public float currentHP = 100f;

    [Header("MP Settings")]
    [Tooltip("최대 MP")]
    [SerializeField] private float maxMP = 100f;
    [Tooltip("현재 MP (인스펙터에서 조절 가능)")]
    [Range(0, 100)]
    public float currentMP = 100f;

    public float MaxHP => maxHP;
    public float CurrentHP => Mathf.Clamp(currentHP, 0, maxHP);

    public float MaxMP => maxMP;
    public float CurrentMP => Mathf.Clamp(currentMP, 0, maxMP);

    private void OnValidate()
    {
        // 인스펙터에서 값 변경 시 실시간으로 클램프
        currentHP = Mathf.Clamp(currentHP, 0f, maxHP);
        currentMP = Mathf.Clamp(currentMP, 0f, maxMP);
    }

    // 예시용 메서드: 데미지/회복
    [ContextMenu("Take 10 Damage")]
    public void DebugDamage() => currentHP = Mathf.Max(currentHP - 10f, 0f);

    [ContextMenu("Recover 10 HP")]
    public void DebugHeal() => currentHP = Mathf.Min(currentHP + 10f, maxHP);
}
