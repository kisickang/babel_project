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

        runtimeMat.SetFloat("_Progress", 0f);  // ← 여기서 초기화 확정!
    }


    void OnEnable()
    {
        runtimeMat.SetFloat("_Progress", 0f); // ✅ 꼭 필요!
        runtimeMat.SetFloat("_BandSize", radius);

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

    void PlayEffect()
    {
        //Debug.Log("[FanMesh] PlayEffect 시작");

        runtimeMat.SetFloat("_Progress", 0f); // ← 다시한번 강제
        tween = DOTween.To(
            () => 0f,
            x =>
            {
                runtimeMat.SetFloat("_Progress", x);
                //Debug.Log($"[FanMesh] Progress: {x}");
            },
            1f,
            duration
        ).SetEase(Ease.Linear)
         .SetId(this)
         .OnComplete(() =>
         {
             //Debug.Log("[FanMesh] DOTween Complete → SetActive(false)");
             gameObject.SetActive(false);
         });
    }

    void OnDisable()
    {
        DOTween.Kill(this);
    }


}
