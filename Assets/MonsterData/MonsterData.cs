using UnityEngine;

[CreateAssetMenu(fileName = "MonsterData", menuName = "GameData/MonsterData", order = 1)]
public class MonsterData : ScriptableObject
{
    public string monsterName;
    public int maxHealth;
    public float moveSpeed;
    public int damage;
    public GameObject[] dropItems;
}
