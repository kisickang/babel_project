using System.Collections.Generic;
using UnityEngine;

public class AxePoolManager : MonoBehaviour
{
    public static AxePoolManager Instance;

    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private int poolSize = 10;
    [SerializeField] private Transform poolParent;

    private Queue<GameObject> pool = new Queue<GameObject>();

    void Awake()
    {
        Instance = this;
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(projectilePrefab, poolParent);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    public GameObject Get()
    {
        if (pool.Count == 0)
        {
            GameObject obj = Instantiate(projectilePrefab, poolParent);
            obj.SetActive(false);
            return obj;
        }

        GameObject pooled = pool.Dequeue();
        pool.Enqueue(pooled);
        return pooled;
    }
}
