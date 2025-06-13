using UnityEngine;
using UnityEngine.UI;

public class StatusUIManager : MonoBehaviour
{
    [Header("Player Reference")]
    [Tooltip("플레이어의 HP/MP를 관리하는 스크립트(예: PlayerStatus)")]
    [SerializeField] private PlayerStatus playerStatus;

    [Header("UI References")]
    [Tooltip("HP 바로 사용할 Image (Image Type을 Filled로, Fill Method는 Horizontal으로 설정하세요)")]
    [SerializeField] private Image hpBar;
    [Tooltip("MP 바로 사용할 Image (Image Type을 Filled로, Fill Method는 Horizontal으로 설정하세요)")]
    [SerializeField] private Image mpBar;

    [Header("Animation Settings")]
    [Tooltip("바가 따라갈 속도 (1초에 얼마나 빨리 채워질지)")]
    [SerializeField] private float smoothSpeed = 2f;

    public float targetHpFill;
    public float targetMpFill;

    private void Awake()
    {
        if (playerStatus == null) Debug.LogError("StatusUIManager: PlayerStatus 할당 안됨!");
        if (hpBar == null || mpBar == null) Debug.LogError("StatusUIManager: HP/MP Image 할당 안됨!");
    }

    private void Update()
    {
        // 1) 플레이어 상태에서 퍼센트 계산
        targetHpFill = playerStatus.CurrentHP / playerStatus.MaxHP;
        targetMpFill = playerStatus.CurrentMP / playerStatus.MaxMP;

        // 2) 부드럽게 보간해서 fillAmount에 반영
        hpBar.fillAmount = Mathf.MoveTowards(hpBar.fillAmount, targetHpFill, smoothSpeed * Time.deltaTime);
        mpBar.fillAmount = Mathf.MoveTowards(mpBar.fillAmount, targetMpFill, smoothSpeed * Time.deltaTime);
    }
}
