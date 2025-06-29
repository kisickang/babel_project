using UnityEngine;
using UnityEngine.UI;

public class LevelUpPopup : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(ApplyEffect);
    }

    public void ApplyEffect()
    {
        // 공격력 10% 증가
        var player = FindObjectOfType<PlayerControll>();
        player.IncreaseAttackDamageByPercent(0.1f);

        // 팝업 닫기
        transform.parent.gameObject.SetActive(false);
        Time.timeScale = 1f;
    }
}
