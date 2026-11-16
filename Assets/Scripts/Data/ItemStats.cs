using UnityEngine;

// Что даёт предмет: очки (камень) или здоровье (сердце).
[CreateAssetMenu(menuName = "Gem Runner/Item Stats")]
public class ItemStats : ScriptableObject
{
    public int scoreValue = 10;
    public int healAmount = 0;
}
