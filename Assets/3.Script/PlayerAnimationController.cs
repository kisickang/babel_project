using UnityEngine;

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
        // 속도가 거의 0인지로 Idle/Walk 판단
        bool isWalking = rb.velocity.sqrMagnitude > 0.01f;
        anim.SetBool(WalkHash, isWalking);
    }
}
