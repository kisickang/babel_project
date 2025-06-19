using System.Collections.Generic;
using UnityEngine;

public class MonsterPoolManager : MonoBehaviour
{
    public static MonsterPoolManager Instance;

    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    public List<Pool> pools;
    private Dictionary<string, Queue<GameObject>> poolDictionary;

    public Transform poolParent; // Inspector에서 빈 오브젝트 할당

void Awake()
{
    Instance = this;
    poolDictionary = new Dictionary<string, Queue<GameObject>>();

    foreach (var pool in pools)
    {
        Queue<GameObject> objectPool = new Queue<GameObject>();
        for (int i = 0; i < pool.size; i++)
        {
            GameObject obj = Instantiate(pool.prefab);
            obj.SetActive(false);

            // ✅ 생성 시 정리용 부모에 넣기
            if (poolParent != null)
                obj.transform.SetParent(poolParent);

            objectPool.Enqueue(obj);
        }
        poolDictionary.Add(pool.tag, objectPool);
    }
}


    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag)) return null;

        GameObject obj = poolDictionary[tag].Dequeue();
        obj.SetActive(true);
        obj.transform.position = position;
        obj.transform.rotation = rotation;

        poolDictionary[tag].Enqueue(obj);
        return obj;
    }
}
