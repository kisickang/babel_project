using UnityEngine;

[CreateAssetMenu(fileName = "PlayerExpData", menuName = "GameData/PlayerExpData")]
public class PlayerExpData : ScriptableObject
{
    public LevelExp[] levelExps;
}

[System.Serializable]
public class LevelExp
{
    public int Level;
    public int RequiredExp; // 여기 "대문자 R"로 반드시 수정
}


