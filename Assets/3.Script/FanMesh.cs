using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class FanMesh : MonoBehaviour
{
    [Range(0f, 360f)]
    public float angle = 60f;
    public float radius = 2f;
    public int segments = 20;

    private Mesh mesh;

    void Awake()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
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
    }
}
