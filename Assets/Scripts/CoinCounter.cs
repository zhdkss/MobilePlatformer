using TMPro;
using UnityEngine;

public class CoinCounter : MonoBehaviour
{
    public static CoinCounter Instance;   // чтобы камень мог найти счётчик
    public TMP_Text label;
    int coins;

    void Awake() { Instance = this; label.text = "x 0"; }

    public void Add()
    {
        coins++;
        label.text = "x " + coins;
    }
}