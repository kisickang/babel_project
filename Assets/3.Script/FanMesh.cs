using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class FanMesh : MonoBehaviour
{
    [Range(0f, 360f)] public float angle = 60f;
    public float radius = 2f;
    public int segments = 20;

    [Header("Effect Settings")]
    public float duration = 0.25f;

    private Mesh mesh;
    private Material runtimeMat;
    private Tween tween;
    private MeshRenderer meshRenderer;

    void Awake()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        meshRenderer = GetComponent<MeshRenderer>();
        runtimeMat = new Material(meshRenderer.sharedMaterial);
        meshRenderer.material = runtimeMat;
    }

    void OnEnable()
    {
        // 다시 켜질 때 효과 재시작
        runtimeMat.SetFloat("_Progress", 0f);
        DOTween.Kill(this);

        GenerateFan();
        PlayEffect();
    }

    public void GenerateFan()
    {
        mesh.Clear();

        int vertCount = segments + 2;
        Vector3[] vertices = new Vector3[vertCount];
        int[] triangles = new int[segments * 3];

        vertices[0] = Vector3.zero;

        float angleStep = angle / segments;
        for (int i = 0; i <= segments; i++)
        {
            float currentAngle = -angle / 2f + angleStep * i;
            float rad = currentAngle * Mathf.Deg2Rad;
            vertices[i + 1] = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f) * radius;
        }

        for (int i = 0; i < segments; i++)
        {
            int idx = i * 3;
            triangles[idx] = 0;
            triangles[idx + 1] = i + 1;
            triangles[idx + 2] = i + 2;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        runtimeMat.SetFloat("_BandSize", radius);
    }

    private void PlayEffect()
    {
        tween = DOTween.To(
            () => 0f,
            x => runtimeMat.SetFloat("_Progress", x),
            1f,
            duration
        ).SetEase(Ease.Linear)
         .SetId(this)
         .OnComplete(() =>
         {
             gameObject.SetActive(false); // ✅ 삭제 대신 비활성화로 변경
         });
    }

    void OnDisable()
    {
        DOTween.Kill(this);
    }
}
