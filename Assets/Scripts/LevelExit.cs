using UnityEngine;

public class LevelExit : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // цель уровня (камни) и сохранение проверяет LevelManager
        if (LevelManager.Instance != null) LevelManager.Instance.TryExit();
        else SceneLoader.LoadNext();
    }
}