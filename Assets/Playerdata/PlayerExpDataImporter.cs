using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class PlayerExpDataImporter
{
    [MenuItem("Tools/Import Player EXP CSV")]
    public static void ImportPlayerExpCSV()
    {
        string csvPath = Application.dataPath + "/Playerdata/player_expdata.csv";
        string assetPath = "Assets/Playerdata/PlayerExpData.asset";

        if (!File.Exists(csvPath))
        {
            Debug.LogError("CSV 파일이 없습니다: " + csvPath);
            return;
        }

        string[] lines = File.ReadAllLines(csvPath);
        List<LevelExp> expList = new();

        for (int i = 1; i < lines.Length; i++) // 헤더 제외
        {
            string[] cols = lines[i].Split(',');

            if (cols.Length < 2) continue;

            int level = int.Parse(cols[0].Trim());
            int exp = int.Parse(cols[1].Trim());

            expList.Add(new LevelExp { Level = level, RequiredExp = exp });
        }

        var expSO = ScriptableObject.CreateInstance<PlayerExpData>();
        expSO.levelExps = expList.ToArray();

        AssetDatabase.DeleteAsset(assetPath); // 기존 제거
        AssetDatabase.CreateAsset(expSO, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("✅ PlayerExpData.asset 생성 완료");
    }
}
