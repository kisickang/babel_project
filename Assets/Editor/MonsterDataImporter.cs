using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class MonsterDataImporter
{
    [MenuItem("Tools/Import Monster CSV")]
    public static void ImportMonsterCSV()
    {
        string csvPath = Application.dataPath + "/MonsterData/monster_data.csv"; // 필요시 경로 수정
        string assetFolder = "Assets/MonsterDataAssets";

        if (!Directory.Exists(assetFolder))
            Directory.CreateDirectory(assetFolder);

        string[] lines;

        // 파일 공유 허용해서 Excel 열려 있어도 읽기 가능하게 처리
        using (var fs = new FileStream(csvPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        using (var sr = new StreamReader(fs))
        {
            var list = new List<string>();
            while (!sr.EndOfStream)
                list.Add(sr.ReadLine());
            lines = list.ToArray();
        }

        for (int i = 1; i < lines.Length; i++) // 첫 줄은 헤더
        {
            string[] cols = lines[i].Split(',');

            if (cols.Length < 7)
            {
                Debug.LogWarning($"[{i}] 줄 데이터가 누락됨: {lines[i]}");
                continue;
            }

            int id = int.Parse(cols[0]);
            string monsterName = cols[1].Trim();
            int maxHealth = int.Parse(cols[2]);
            float moveSpeed = float.Parse(cols[3]);
            int damage = int.Parse(cols[4]);

            // dropExpType은 CSV의 7번째 열 (index 6)
            string expTypeStr = cols[6].Trim();

            ExpDropType expType = expTypeStr switch
            {
                "Small" => ExpDropType.Small,
                "Medium" => ExpDropType.Medium,
                "Large" => ExpDropType.Large,
                _ => ExpDropType.None
            };

            string assetPath = $"{assetFolder}/{monsterName}.asset";

            // 기존 에셋 삭제 (덮어쓰기)
            if (File.Exists(assetPath))
            {
                AssetDatabase.DeleteAsset(assetPath);
            }

            var data = ScriptableObject.CreateInstance<MonsterData>();
            data.id = id;
            data.monsterName = monsterName;
            data.maxHealth = maxHealth;
            data.moveSpeed = moveSpeed;
            data.damage = damage;
            data.dropExpType = expType;

            AssetDatabase.CreateAsset(data, assetPath);

            Debug.Log($"✅ {monsterName} 데이터 생성됨 (Exp: {expType})");
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("🎉 몬스터 CSV → ScriptableObject 변환 완료!");
    }
}
