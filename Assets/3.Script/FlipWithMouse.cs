using UnityEngine;

public class FlipWithMouse : MonoBehaviour
{
    void Update()
    {
        // 마우스 월드좌표
        Vector3 m = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // 왼쪽이면 scale.x = 1, 오른쪽이면 -1
        float x = (m.x < transform.position.x) ? 1f : -1f;
        transform.localScale = new Vector3(x, 1f, 1f);
    }
}
