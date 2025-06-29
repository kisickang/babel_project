using UnityEngine;

public class LevelUpMoveSpeed : LevelUpPopup
{
    [Header("증가 비율 (예: 0.1 = 10%)")]
    [Range(0f, 1f)] public float movePercent = 0.1f;
    public override void ApplyEffect()
    {
        var player = FindObjectOfType<PlayerControll>();
        player.IncreaseMoveSpeedByPercent(movePercent);
    }
}
