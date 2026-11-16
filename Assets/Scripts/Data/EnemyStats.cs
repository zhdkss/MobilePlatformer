using UnityEngine;

// Числа баланса одного типа врага. У каждого типа свой ассет:
// Patroller, Shooter, Charger и обычный враг. Не все поля нужны каждому типу.
[CreateAssetMenu(menuName = "Gem Runner/Enemy Stats")]
public class EnemyStats : ScriptableObject
{
    [Header("Зрение")]
    public float viewRange = 5f;                       // как далеко видит
    [Range(10f, 360f)] public float viewAngle = 120f;  // угол обзора перед собой
    public float heightTolerance = 1.2f;               // таран: игрок должен быть на той же высоте

    [Header("Движение")]
    public float patrolSpeed = 2f;
    public float waitAtPoint = 0.5f;
    public float chaseSpeed = 3.5f;
    public float loseSightTime = 1.5f;   // сколько секунд ищет, потеряв игрока
    public float chaseLeash = 2f;        // насколько можно выйти за крайние точки

    [Header("Атака")]
    public float attackRange = 1.6f;     // с какого расстояния начинает замах
    public float windUpTime = 0.5f;      // замах перед атакой
    public float attackSpeed = 9f;       // скорость рывка / тарана / снаряда
    public float attackDistance = 2.25f; // длина рывка / тарана
    public float cooldown = 1f;          // отдых после атаки
    public float projectileLife = 4f;    // сколько живёт снаряд

    [Header("Касание")]
    public int contactDamage = 1;
    public float stompBounce = 10f;      // отскок игрока, когда он прыгнул на врага
}
