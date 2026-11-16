using UnityEngine;

// Числа баланса босса.
[CreateAssetMenu(menuName = "Gem Runner/Boss Stats")]
public class BossStats : ScriptableObject
{
    public int maxHp = 5;
    public float moveSpeed = 2f;
    public float moveRange = 3f;

    [Header("Стрельба")]
    public float windUpTime = 0.4f;          // красная вспышка перед выстрелом
    public float fireballSpeed = 6f;
    public float fireballLife = 4f;
    public float shootDelay = 2f;            // пауза между выстрелами
    public float enragedShootDelay = 1f;     // вторая фаза: ниже половины HP
}
