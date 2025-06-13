using UnityEngine;

public class LevelUpHandler : MonoBehaviour
{
    // AnimationEvent와 시그니처(리턴타입, 매개변수)가 정확히 일치해야 합니다.
    public void ShowLevelUpWindow()
    {
        // 여기에 레벨업 창을 띄우는 로직 작성
        Debug.Log("ShowLevelUpWindow 이벤트 수신!");
    }
}
