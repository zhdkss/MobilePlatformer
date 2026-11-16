using TMPro;
using UnityEngine;

public class CoinCounter : MonoBehaviour
{
    public static CoinCounter Instance;   // чтобы камень мог найти счётчик
    public TMP_Text label;

    public int Gems { get; private set; }    // сколько камней собрано (цель уровня)
    public int Score { get; private set; }   // очки: идут в рекорд в сохранении

    void Awake() { Instance = this; label.text = "0"; }

    // value приходит из ItemStats камня
    public void Add(int value)
    {
        Gems++;
        Score += value;
        label.text = Score.ToString();
    }
}
