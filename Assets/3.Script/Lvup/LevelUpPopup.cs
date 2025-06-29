using UnityEngine;
using UnityEngine.UI;

public abstract class LevelUpPopup : MonoBehaviour
{
    protected virtual void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        ApplyEffect();                         // 자식 클래스에서 효과 적용
        LevelUpManager.InvokePopupSelected();  // ✅ 안전한 방식으로 이벤트 호출
    }

    protected abstract void ApplyEffect();     // 각 스킬 팝업이 구현할 메서드
}
