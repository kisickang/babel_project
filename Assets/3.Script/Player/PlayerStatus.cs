using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    [Header("HP Settings")]
    [SerializeField] private float maxHP = 100f;
    [Range(0, 100)] public float currentHP = 100f;

    [Header("MP Settings")]
    [SerializeField] private float maxMP = 100f;
    [Range(0, 100)] public float currentMP = 100f;

    public float MaxHP => maxHP;
    public float CurrentHP => Mathf.Clamp(currentHP, 0, maxHP);
    public float MaxMP => maxMP;
    public float CurrentMP => Mathf.Clamp(currentMP, 0, maxMP);

    [Header("Damage Popup")]
    [SerializeField] private GameObject playerDamageUIPrefab;
    [SerializeField] private Transform spriteGroup;
    [SerializeField] private float popupYOffset = 0.3f; // 오프셋 조절 가능

    private void OnValidate()
    {
        currentHP = Mathf.Clamp(currentHP, 0f, maxHP);
        currentMP = Mathf.Clamp(currentMP, 0f, maxMP);
    }

    public void TakeDamage(float damage)
    {
        currentHP = Mathf.Max(currentHP - damage, 0f);
        ShowDamagePopup((int)damage);
    }

    private void ShowDamagePopup(int damage)
    {
        if (playerDamageUIPrefab == null)
        {
            Debug.LogWarning("PlayerDamageUIPrefab이 비어 있음!");
            return;
        }

        Vector3 popupPos = GetPopupPosition();
        GameObject popup = Instantiate(playerDamageUIPrefab, popupPos, Quaternion.identity);
        var popupScript = popup.GetComponent<DamagePopup>();
        if (popupScript == null)
            Debug.LogWarning("DamagePopup 스크립트가 프리팹에 없음!");
        else
            popupScript.Show(damage);
    }

    private Vector3 GetPopupPosition()
    {
        if (spriteGroup == null)
        {
            Debug.LogWarning("spriteGroup이 비어 있음");
            return transform.position + Vector3.up * 1.5f;
        }

        SpriteRenderer[] renderers = spriteGroup.GetComponentsInChildren<SpriteRenderer>();
        if (renderers.Length > 0)
        {
            float topY = float.MinValue;
            Vector3 centerX = transform.position;

            foreach (var sr in renderers)
            {
                if (sr.bounds.max.y > topY)
                {
                    topY = sr.bounds.max.y;
                    centerX = sr.bounds.center;
                }
            }

            return new Vector3(centerX.x, topY + popupYOffset, 0f);
        }

        return transform.position + Vector3.up * 1.5f;
    }

    [ContextMenu("Take 10 Damage")]
    public void DebugDamage() => TakeDamage(10f);

    [ContextMenu("Recover 10 HP")]
    public void DebugHeal() => currentHP = Mathf.Min(currentHP + 10f, maxHP);
}
