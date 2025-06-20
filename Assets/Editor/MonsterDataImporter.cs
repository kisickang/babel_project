using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class MonsterDataImporter
{
    [MenuItem("Tools/Import Monster CSV")]
    public static void ImportMonsterCSV()
    {
        string csvPath = Application.dataPath + "/MonsterData/monster_data.csv"; // 경로 수정 가능
        string assetFolder = "Assets/MonsterDataAssets";

        if (!Directory.Exists(assetFolder))
            Directory.CreateDirectory(assetFolder);

        string[] lines = File.ReadAllLines(csvPath);

        for (int i = 1; i < lines.Length; i++) // 첫 줄은 헤더
        {
            string[] cols = lines[i].Split(',');

            if (cols.Length < 6)
            {
                Debug.LogWarning($"[{i}] 줄 데이터가 누락되었습니다: {lines[i]}");
                continue;
            }

            string name = cols[1];
            int maxHealth = int.Parse(cols[2]);
            float moveSpeed = float.Parse(cols[3]);
            int damage = int.Parse(cols[4]);
            string dropPathStr = cols[5];

            GameObject[] dropItems = null;
            if (!string.IsNullOrWhiteSpace(dropPathStr))
            {
                string[] dropPaths = dropPathStr.Split('|');
                List<GameObject> drops = new();
                foreach (string path in dropPaths)
                {
                    GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(path.Trim());
                    if (obj != null) drops.Add(obj);
                    else Debug.LogWarning($"드랍 아이템 경로가 잘못됨: {path}");
                }
                dropItems = drops.ToArray();
            }

            // Create ScriptableObject
            var data = ScriptableObject.CreateInstance<MonsterData>();
            data.monsterName = name;
            data.maxHealth = maxHealth;
            data.moveSpeed = moveSpeed;
            data.damage = damage;
            data.dropItems = dropItems;

            string assetPath = $"{assetFolder}/{name}.asset";
            AssetDatabase.CreateAsset(data, assetPath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("✅ 몬스터 데이터 임포트 완료!");
    }
}
