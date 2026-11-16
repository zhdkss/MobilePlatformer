using UnityEngine;

// Все числа баланса игрока. Меняются в ассете, а не в коде.
[CreateAssetMenu(menuName = "Gem Runner/Player Stats")]
public class PlayerStats : ScriptableObject
{
    [Header("Движение")]
    public float moveSpeed = 6f;
    public float jumpForce = 13f;

    [Header("Удобство управления")]
    public float stickDeadZone = 0.25f;     // слабый наклон стика не считается
    public float coyoteTime = 0.12f;        // можно прыгнуть чуть позже края платформы
    public float jumpBufferTime = 0.15f;    // нажатие чуть раньше приземления запоминается
    public float jumpCutMultiplier = 0.5f;  // отпустил кнопку рано = низкий прыжок

    [Header("Здоровье")]
    public int maxHp = 3;
    public float invincibleTime = 1.5f;

    [Header("Стрельба")]
    public float bulletSpeed = 12f;
    public float bulletLife = 2f;
}
