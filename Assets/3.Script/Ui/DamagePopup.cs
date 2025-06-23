using TMPro;
using UnityEngine;
using System.Collections;

public class DamagePopup : MonoBehaviour
{
    [SerializeField] private TextMeshPro text;
    [SerializeField] private float moveY = 1.5f;
    [SerializeField] private float duration = 1f;

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
