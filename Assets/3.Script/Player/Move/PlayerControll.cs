using UnityEngine;

public class PlayerControll : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;

    [Header("Mouse Flip")]
    [Tooltip("좌우 반전을 적용할 기준 Transform (보통 Sprite)")]
    [SerializeField] private Transform spriteTransform;

    [Tooltip("카메라가 null이면 Camera.main 사용")]
    [SerializeField] private Camera targetCamera;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (targetCamera == null)
            targetCamera = Camera.main;

        if (spriteTransform == null)
            spriteTransform = transform; // 기본값으로 자기 자신
    }

    void Update()
    {
        // 입력 처리
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        // 마우스 좌우 방향에 따라 Flip
        Vector3 mouseWorld = targetCamera.ScreenToWorldPoint(Input.mousePosition);
        float direction = mouseWorld.x < transform.position.x ? 1f : -1f;

        spriteTransform.localScale = new Vector3(direction, spriteTransform.localScale.y, spriteTransform.localScale.z);
    }

    void FixedUpdate()
    {
        rb.velocity = moveInput.normalized * moveSpeed;
    }
}
