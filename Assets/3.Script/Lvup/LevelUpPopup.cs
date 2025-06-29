using UnityEngine;
using UnityEngine.UI;

public abstract class LevelUpPopup : MonoBehaviour
{
    protected virtual void Awake()
    {
        var button = GetComponent<Button>();
        button.onClick.RemoveAllListeners();  // ✅ 중복 클릭 방지
        button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        Debug.Log($"[OnClick] {gameObject.name} 클릭됨! (Frame: {Time.frameCount})");
        ApplyEffect();                         // ✅ 자식 클래스에서 효과 적용
        LevelUpManager.InvokePopupSelected();  // ✅ 안전한 방식으로 이벤트 호출
    }

    // 🔄 public으로 선언하여 인스펙터에서 연결 가능
    public abstract void ApplyEffect();
}
