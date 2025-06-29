using UnityEngine;

public class LevelUpEffectRelay : MonoBehaviour
{
    public void ShowLevelUpWindow()
    {
        Debug.Log($"[Relay] ShowLevelUpWindow 호출: {Time.frameCount}");
        LevelUpManager mgr = FindObjectOfType<LevelUpManager>();
        if (mgr != null)
            mgr.ShowRandomPopups();
    }
}
