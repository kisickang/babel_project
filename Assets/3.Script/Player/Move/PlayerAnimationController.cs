using UnityEngine;

namespace PlayerAnimation
{
    [RequireComponent(typeof(Animator))]
    public class PlayerAnimationController : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D rb;
        private Animator anim;

        private static readonly int WalkHash = Animator.StringToHash("Walk");
        private static readonly int AttackTriggerHash = Animator.StringToHash("Attack");

        [Header("Auto Attack Settings")]
        [SerializeField] private float attackInterval = 1f; // 공격 간격 (초)

        private float attackTimer;

        void Awake()
        {
            anim = GetComponent<Animator>();
            if (rb == null)
                rb = GetComponentInParent<Rigidbody2D>();
        }

        void Update()
        {
            // 걷기 애니메이션 처리
            bool isWalking = rb.velocity.sqrMagnitude > 0.01f;
            anim.SetBool(WalkHash, isWalking);

            // 자동 공격 타이머
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
