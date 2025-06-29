using UnityEngine;

namespace PlayerAnimation
{
    [RequireComponent(typeof(Animator))]
    public class PlayerAnimationController : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D rb;
        private Animator anim;

        private static readonly int RunHash = Animator.StringToHash("Run");
        private static readonly int AttackTriggerHash = Animator.StringToHash("Attack");

        [Header("Auto Attack Settings")]
        [SerializeField] private float attackInterval = 1f; // 공격 간격

        private float attackTimer;

        void Awake()
        {
            anim = GetComponent<Animator>();
            if (rb == null)
                rb = GetComponentInParent<Rigidbody2D>();
        }

        void Update()
        {
            float moveSpeed = rb.velocity.magnitude;

            // 1. Run 애니메이션 여부
            bool isRunning = moveSpeed > 0.01f;
            anim.SetBool(RunHash, isRunning);

            // 2. 이동 속도를 애니메이션 Speed_Move 파라미터에 전달
            anim.SetFloat("Speed_Move", moveSpeed);

            // 3. 자동 공격
            attackTimer += Time.deltaTime;
            if (attackTimer >= attackInterval)
            {
                TriggerAttackAnimation();
                attackTimer = 0f;
            }
        }


        public void TriggerAttackAnimation()
        {
            anim.SetTrigger(AttackTriggerHash);
        }
    }
}
