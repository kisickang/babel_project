using TMPro;
using UnityEngine;
using System.Collections;

public class DamagePopup : MonoBehaviour
{
    [SerializeField] private TextMeshPro text;
    [SerializeField] private float moveY = 0.5f;      // 인스펙터에서 조절 가능
    [SerializeField] private float duration = 0.5f;   // 짧은 시간 동안

    public void Show(int damage)
    {
        text.text = damage.ToString();
        StartCoroutine(PopupRoutine());
    }

    private IEnumerator PopupRoutine()
    {
        float elapsed = 0f;
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + new Vector3(0, moveY, 0);

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(startPos, endPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}
