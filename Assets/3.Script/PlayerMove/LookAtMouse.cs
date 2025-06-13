using UnityEngine;

public class OrbitLookAtMouse : MonoBehaviour
{
    [Header("Orbit Settings")]
    [Tooltip("플레이어 오브젝트 Transform을 드래그해서 할당하세요.")]
    [SerializeField] private Transform AxisTransform;
    [Tooltip("플레이어를 기준으로 얼만큼 떨어진 위치에서 회전시킬지 반지름을 설정하세요.")]
    [SerializeField] private float orbitRadius = 1f;

    [Header("Camera (optional)")]
    [Tooltip("월드 좌표 변환에 사용할 카메라. 비워두면 Camera.main 사용")]
    [SerializeField] private Camera targetCamera;

    private void Awake()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;
        if (AxisTransform == null)
            Debug.LogError("OrbitLookAtMouse: Player Transform이 할당되지 않았습니다!");
    }

    private void Update()
    {
        // 1) 마우스 스크린 좌표 → 월드 좌표
        Vector3 mouseWorld = targetCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = AxisTransform.position.z;

        // 2) 플레이어 → 마우스 방향 벡터
        Vector2 dir = (mouseWorld - AxisTransform.position).normalized;

        // 3) 궤도 상 위치 계산 (원둘레 상)
        Vector3 orbitPos = (Vector3)dir * orbitRadius + AxisTransform.position;
        orbitPos.z = transform.position.z; // Z-depth 고정

        transform.position = orbitPos;

        // 4) 화살표(혹은 HUD)의 회전 각도 계산
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
