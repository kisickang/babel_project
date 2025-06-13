using UnityEngine;

namespace PlayerAnimation
{
    /// <summary>
    /// 마우스 방향에 따라 transform.localScale.x 를 ±1 로 설정해주는 유틸 클래스
    /// </summary>
    public static class FlipWithMouse
    {
        /// <summary>
        /// 넘겨받은 Transform 을 마우스 방향에 따라 좌우 뒤집기
        /// </summary>
        public static void ApplyFlip(Transform t)
        {
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float x = (mouseWorld.x < t.position.x) ? 1f : -1f;
            t.localScale = new Vector3(x, t.localScale.y, t.localScale.z);
        }
    }

    [RequireComponent(typeof(Animator))]
    public class PlayerAnimationController : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("이동 속도를 읽어올 Rigidbody2D")]
        [SerializeField] private Rigidbody2D rb;

        private Animator anim;
        private static readonly int WalkHash = Animator.StringToHash("Walk");

        void Awake()
        {
            anim = GetComponent<Animator>();
            if (rb == null)
                rb = GetComponentInParent<Rigidbody2D>();
        }

        void Update()
        {
            // 1) 걷기 애니메이션 전환
            bool isWalking = rb.velocity.sqrMagnitude > 0.01f;
            anim.SetBool(WalkHash, isWalking);

            // 2) FlipWithMouse 클래스의 ApplyFlip 메서드 호출
            FlipWithMouse.ApplyFlip(transform);
        }
    }
}
