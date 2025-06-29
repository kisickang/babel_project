using UnityEngine;

public class LevelUpAttackSpeed : LevelUpPopup
{
    protected override void ApplyEffect()
    {
        var player = FindObjectOfType<PlayerControll>();
        player.IncreaseAttackSpeedByPercent(0.1f);
    }
}
