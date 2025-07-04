using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    [SerializeField] private float popupYOffset = 0.3f;

    [Header("Recovery Settings")]
    [SerializeField] private float hpRecoverAmount = 5f;
    [SerializeField] private float hpRecoverInterval = 3f;
    [SerializeField] private ParticleSystem hpEffect;

    [SerializeField] private float mpRecoverAmount = 10f;
    [SerializeField] private float mpRecoverInterval = 1f;

    [Header("EXP System")]
    [SerializeField] private PlayerExpData expData;
    [SerializeField] private Image expBar;
    [SerializeField] private TextMeshProUGUI levelText;

    [Header("FX")]
    [SerializeField] private GameObject levelUpEffect;
    [SerializeField] private float levelUpEffectDuration = 1.5f;

    [Header("UI")]
    [SerializeField] private GameObject levelUpPopupPanel;
    [SerializeField] private Image hpBar; // ✅ 추가: HP 바 이미지

    private int level = 1;
    private int currentExp = 0;

    void Start()
    {
        StartCoroutine(HpRecoverRoutine());
        StartCoroutine(MpRecoverRoutine());
        UpdateExpUI();
        UpdateHPUI(); // ✅ 시작 시 HP UI 초기화
    }

    private IEnumerator HpRecoverRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(hpRecoverInterval);
        while (true)
        {
            yield return wait;

            if (currentHP < maxHP)
            {
                currentHP = Mathf.Min(currentHP + hpRecoverAmount, maxHP);
                UpdateHPUI(); // ✅ 회복 시 HP UI 갱신

                if (hpEffect != null)
                {
                    if (!hpEffect.gameObject.activeSelf)
                        hpEffect.gameObject.SetActive(true);
                    hpEffect.Play();
                }
            }
        }
    }

    private IEnumerator MpRecoverRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(mpRecoverInterval);
        while (true)
        {
            yield return wait;

            if (currentMP < maxMP)
            {
                currentMP = Mathf.Min(currentMP + mpRecoverAmount, maxMP);
                // MP는 이펙트 없음
            }
        }
    }

    public void TakeDamage(float damage)
    {
        Debug.Log($"[PlayerStatus] 데미지 {damage} 받음! (현재 HP: {currentHP})");

        currentHP = Mathf.Max(currentHP - damage, 0f);
        ShowDamagePopup((int)damage);
        UpdateHPUI();
    }

    private void UpdateHPUI() // ✅ 추가된 함수
    {
        if (hpBar != null)
            hpBar.fillAmount = currentHP / maxHP;
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
        if (popupScript != null)
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

    public void AddExp(int amount)
    {
        currentExp += amount;

        while (level < expData.levelExps.Length &&
               currentExp >= expData.levelExps[level - 1].RequiredExp)
        {
            currentExp -= expData.levelExps[level - 1].RequiredExp;
            level++;
            Debug.Log($"레벨업! ▶ 현재 레벨: {level}");

            if (levelUpEffect != null)
            {
                levelUpEffect.SetActive(false);
                levelUpEffect.SetActive(true);
                Invoke(nameof(DisableLevelUpEffect), levelUpEffectDuration);
            }

            if (levelUpPopupPanel != null)
            {
                Time.timeScale = 0f;
                levelUpPopupPanel.SetActive(true);
            }
        }

        UpdateExpUI();
    }

    private void DisableLevelUpEffect()
    {
        if (levelUpEffect != null)
            levelUpEffect.SetActive(false);
    }

    private void UpdateExpUI()
    {
        int maxExp = expData.levelExps[Mathf.Clamp(level - 1, 0, expData.levelExps.Length - 1)].RequiredExp;
        expBar.fillAmount = (float)currentExp / maxExp;
        levelText.text = level.ToString();
    }

    private void OnValidate()
    {
        currentHP = Mathf.Clamp(currentHP, 0f, maxHP);
        currentMP = Mathf.Clamp(currentMP, 0f, maxMP);
    }

    [ContextMenu("Take 10 Damage")]
    public void DebugDamage() => TakeDamage(10f);

    [ContextMenu("Recover 10 HP")]
    public void DebugHeal()
    {
        currentHP = Mathf.Min(currentHP + 10f, maxHP);
        UpdateHPUI(); // 디버그 회복 시에도 갱신
    }
}
