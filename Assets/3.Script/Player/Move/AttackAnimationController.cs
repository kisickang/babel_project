using UnityEngine;
public class AttackAnimationController : MonoBehaviour
{
    private Animator anim;
    private static readonly int ThrowHash = Animator.StringToHash("ThrowTrigger");

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            anim.SetTrigger(ThrowHash);
        }
    }
}
