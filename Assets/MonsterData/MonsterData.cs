using UnityEngine;

public enum ExpDropType { None, Small, Medium, Large }

[CreateAssetMenu(fileName = "MonsterData", menuName = "GameData/MonsterData", order = 1)]
public class MonsterData : ScriptableObject
{
    public int id;
    public string monsterName;
    public int maxHealth;
    public float moveSpeed;
    public int damage;
    public ExpDropType dropExpType;
}
