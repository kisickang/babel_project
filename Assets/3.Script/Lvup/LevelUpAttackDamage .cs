using UnityEngine;

public class LevelUpAttackDamage : LevelUpPopup
{
    protected override void ApplyEffect()
    {
        var player = FindObjectOfType<PlayerControll>();
        player.IncreaseAttackDamageByPercent(0.1f);
    }
}
