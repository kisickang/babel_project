using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    [Header("Follow Target")]
    [SerializeField] private Transform target;

    [Header("Offset & Smoothing")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f);
    [SerializeField] private float smoothTime = 0f;

    [Header("Bounds (optional)")]
    [SerializeField] private Vector2 minPosition;
    [SerializeField] private Vector2 maxPosition;
    [SerializeField] private bool useBounds = false;

    private Vector3 velocity = Vector3.zero;

    private void FixedUpdate()
    {
        if (target == null) return;

        // 목표 위치 계산 (offset 포함)
        Vector3 desiredPosition = target.position + offset;
        // 부드럽게 이동
        Vector3 smoothed = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime);

        // 영역 제한
        if (useBounds)
        {
            smoothed.x = Mathf.Clamp(smoothed.x, minPosition.x, maxPosition.x);
            smoothed.y = Mathf.Clamp(smoothed.y, minPosition.y, maxPosition.y);
        }

        // Z축 고정 (절대값)
        smoothed.z = offset.z;

        transform.position = smoothed;
    }

    // 인스펙터에서 자동으로 할당하고 싶다면 Awake에 넣어도 됩니다.
    private void Awake()
    {
        // Main Camera가 붙어 있으면 자동으로 타겟 할당 시도
        if (Camera.main != null && Camera.main.transform.parent == null)
            Debug.LogWarning("Target이 비어있습니다! Inspector에서 할당해주세요.");
    }
}
