using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelUpManager : MonoBehaviour
{
    [Header("UI 프리팹 리스트")]
    [SerializeField] private List<GameObject> popupPrefabs;

    [Header("왼쪽/오른쪽 슬롯")]
    [SerializeField] private Transform leftSlot;
    [SerializeField] private Transform rightSlot;

    [Header("패널 오브젝트")]
    [SerializeField] private GameObject levelUpPanel;

    private GameObject leftPopupInstance;
    private GameObject rightPopupInstance;

    // ✅ 외부에서 구독 가능한 이벤트
    public static event Action OnPopupSelected;

    // ✅ 외부에서 안전하게 호출하는 메서드
    public static void InvokePopupSelected()
    {
        OnPopupSelected?.Invoke();
    }

    public void ShowRandomPopups()
    {
        //      Debug.Log("[LevelUpManager] ShowRandomPopups 호출");
        Debug.Log($"[LevelUpManager] ShowRandomPopups 호출: {Time.frameCount}");
        HidePopups();

        List<int> selectedIndexes = new List<int>();
        while (selectedIndexes.Count < 2)
        {
            int index = UnityEngine.Random.Range(0, popupPrefabs.Count);
            if (!selectedIndexes.Contains(index))
                selectedIndexes.Add(index);
        }

        leftPopupInstance = Instantiate(popupPrefabs[selectedIndexes[0]], leftSlot);
        Debug.Log($"LEFT 팝업 생성: {popupPrefabs[selectedIndexes[0]].name}");
        rightPopupInstance = Instantiate(popupPrefabs[selectedIndexes[1]], rightSlot);
        Debug.Log($"RIGHT 팝업 생성: {popupPrefabs[selectedIndexes[1]].name}");

        levelUpPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void HidePopups()
    {
        if (leftPopupInstance != null) Destroy(leftPopupInstance);
        if (rightPopupInstance != null) Destroy(rightPopupInstance);

        levelUpPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    private void OnEnable()
    {
        //        Debug.Log("[LevelUpManager] OnEnable 호출");
        Debug.LogWarning($"[LevelUpManager] HidePopups 호출: {Time.frameCount}");
        OnPopupSelected -= HidePopups; // ✅ 중복 구독 방지
        OnPopupSelected += HidePopups;
    }

    private void OnDisable()
    {
        OnPopupSelected -= HidePopups; // ✅ 구독 해제
    }
}
