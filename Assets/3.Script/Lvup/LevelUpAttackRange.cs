using UnityEngine;

public class LevelUpAttackRange : LevelUpPopup
{
    protected override void ApplyEffect()
    {
        var player = FindObjectOfType<PlayerControll>();
        player.IncreaseAttackRangeByPercent(0.1f);
    }
}
