using UnityEngine;

public class LevelUpAttackDamage : LevelUpPopup
{
    [Header("공격력 증가 비율 (예: 0.1 = 10%)")]
    [Range(0f, 1f)] public float damagePercent = 0.1f;

    public override void ApplyEffect()
    {
        var player = FindObjectOfType<PlayerControll>();
        player.IncreaseAttackDamageByPercent(damagePercent);
    }
}
