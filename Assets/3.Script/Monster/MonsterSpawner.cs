using System.Collections;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [Header("Hierarchy 정리용 부모")]
    [SerializeField] private Transform monsterParent;

    [System.Serializable]
    public class SpawnWaveData
    {
        public string poolTag;
        public int countPerWave;
        public float interval;
        public float waveStartTime;
    }

    public SpawnWaveData[] waves;
    public Transform player;
    public float spawnDistance = 10f;

    void Start()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
            else
                Debug.LogError("[MonsterSpawner] Player를 찾을 수 없습니다. 태그 확인!");
        }

        foreach (var wave in waves)
        {
            StartCoroutine(SpawnWaveRoutine(wave));
        }
    }

    IEnumerator SpawnWaveRoutine(SpawnWaveData wave)
    {
        yield return new WaitForSeconds(wave.waveStartTime);

        while (true)
        {
            for (int i = 0; i < wave.countPerWave; i++)
            {
                Vector3 spawnPos = GetRandomSpawnPosition();
                GameObject monster = MonsterPoolManager.Instance.SpawnFromPool(wave.poolTag, spawnPos, Quaternion.identity);

                if (monster == null)
                {
                    Debug.LogWarning($"[MonsterSpawner] '{wave.poolTag}' 풀에서 몬스터를 가져오지 못했습니다.");
                    continue;
                }

                if (monsterParent != null)
                {
                    monster.transform.SetParent(monsterParent, true); // 월드 좌표 유지
                }
                else
                {
                    Debug.LogWarning("[MonsterSpawner] monsterParent가 비어 있습니다.");
                }
            }

            yield return new WaitForSeconds(wave.interval);
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        Vector2 dir = Random.insideUnitCircle.normalized;
        return player.position + new Vector3(dir.x, dir.y, 0f) * spawnDistance;
    }
}
