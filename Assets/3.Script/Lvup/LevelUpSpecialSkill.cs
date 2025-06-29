using UnityEngine;

public class LevelUpSpecialSkill : LevelUpPopup
{
    protected override void ApplyEffect()
    {
        var player = FindObjectOfType<PlayerControll>();
        player.IncreaseSpecialSkillPowerByPercent(0.1f);
    }
}
