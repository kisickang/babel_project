using UnityEngine;

public class LevelUpMoveSpeed : LevelUpPopup
{
    protected override void ApplyEffect()
    {
        var player = FindObjectOfType<PlayerControll>();
        player.IncreaseMoveSpeedByPercent(0.1f);
    }
}
