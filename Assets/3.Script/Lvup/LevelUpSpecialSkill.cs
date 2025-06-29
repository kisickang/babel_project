using UnityEngine;

public class LevelUpSpecialSkill : LevelUpPopup
{
    [Header("증가 비율 (예: 0.1 = 10%)")]
    [Range(0f, 1f)] public float Percent = 0.1f;
    public override void ApplyEffect()
    {
        var player = FindObjectOfType<PlayerControll>();
        player.IncreaseSpecialSkillPowerByPercent(Percent);
    }
}
