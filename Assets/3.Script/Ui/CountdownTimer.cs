using UnityEngine;
using TMPro;

public class CountdownTimer : MonoBehaviour
{
    [Header("Timer Settings")]
    [Tooltip("시작 시간을 초 단위로 설정 (기본 30분 = 1800초)")]
    [SerializeField] private float startTimeInSeconds = 1800f;

    [Header("UI References")]
    [Tooltip("카운트다운 시간을 표시할 TextMeshProUGUI 컴포넌트")]
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Color Settings")]
    [Tooltip("남은 시간이 경고 구간(<=120초)일 때 변경될 색상")]
    [SerializeField] private Color warningColor = Color.red;
    [Tooltip("평상시 표시될 색상")]
    [SerializeField] private Color normalColor = Color.white;

    private float remainingTime;
    private bool isRunning;

    private void Start()
    {
        ResetTimer();
    }

    private void Update()
    {
        if (!isRunning) return;

        remainingTime -= Time.deltaTime;
        if (remainingTime <= 0f)
        {
            remainingTime = 0f;
            isRunning = false;
            // 타이머 종료 시 추가 동작이 필요하면 여기 호출
        }

        UpdateTimerDisplay();
    }

    private void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(remainingTime / 60f);
        int seconds = Mathf.FloorToInt(remainingTime % 60f);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        // 120초 이하일 때 warningColor, 그 외에는 normalColor
        timerText.color = (remainingTime <= 120f) ? warningColor : normalColor;
    }

    /// <summary>
    /// 타이머를 시작 값으로 리셋하고 자동으로 실행
    /// </summary>
    public void ResetTimer()
    {
        remainingTime = startTimeInSeconds;
        isRunning = true;
        UpdateTimerDisplay();
    }

    /// <summary>
    /// 외부에서 일시정지하거나 재시작할 때 호출
    /// </summary>
    public void SetRunning(bool running)
    {
        isRunning = running;
    }
}
