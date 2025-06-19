using UnityEngine;

public class AnimationRelay : MonoBehaviour
{
    [SerializeField] private PlayerControll controller;

    public void ApplyAttack()  // ✅ 애니메이션 이벤트에서 선택 가능
    {
        controller?.ApplyAttack();
    }

    public void BasicAttack_SFX()  // ✅ 이것도 이벤트에 연결 가능
    {
        controller?.BasicAttack_SFX();
    }
}
