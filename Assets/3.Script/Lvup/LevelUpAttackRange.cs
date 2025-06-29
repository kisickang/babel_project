using UnityEngine;

public class LevelUpAttackRange : LevelUpPopup
{
    [Header("공격 범위 증가 비율 (예: 0.1 = 10%)")]
    [Range(0f, 1f)]
    public float damagePercent = 0.1f;

    // 🔄 public override로 변경
    public override void ApplyEffect()
    {
        var player = FindObjectOfType<PlayerControll>();
        if (player != null)
            player.IncreaseAttackRangeByPercent(damagePercent);
    }
}
