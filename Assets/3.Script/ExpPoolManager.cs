using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpPoolManager : MonoBehaviour
{
    public static ExpPoolManager Instance;

    [SerializeField] private GameObject smallPrefab;
    [SerializeField] private GameObject mediumPrefab;
    [SerializeField] private GameObject largePrefab;

    private Dictionary<ExpDropType, Queue<GameObject>> poolDict = new();

    void Awake()
    {
        Instance = this;

        poolDict[ExpDropType.Small] = new Queue<GameObject>();
        poolDict[ExpDropType.Medium] = new Queue<GameObject>();
        poolDict[ExpDropType.Large] = new Queue<GameObject>();

        Preload(smallPrefab, ExpDropType.Small, 30);
        Preload(mediumPrefab, ExpDropType.Medium, 20);
        Preload(largePrefab, ExpDropType.Large, 10);
    }

    void Preload(GameObject prefab, ExpDropType type, int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject obj = Instantiate(prefab, transform); // 부모는 EXPPool
            obj.SetActive(false);
            poolDict[type].Enqueue(obj);
        }
    }

    public GameObject GetExp(ExpDropType type, Vector3 pos)
    {
        if (!poolDict.ContainsKey(type)) return null;

        GameObject obj = poolDict[type].Count > 0
            ? poolDict[type].Dequeue()
            : Instantiate(GetPrefab(type), transform);

        obj.transform.position = pos;
        obj.SetActive(true);
        return obj;
    }

    public void ReturnToPool(ExpDropType type, GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.SetParent(transform);
        poolDict[type].Enqueue(obj);
    }

    GameObject GetPrefab(ExpDropType type) => type switch
    {
        ExpDropType.Small => smallPrefab,
        ExpDropType.Medium => mediumPrefab,
        ExpDropType.Large => largePrefab,
        _ => null
    };
}

