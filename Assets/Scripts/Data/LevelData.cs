using UnityEngine;

// Один враг уровня: какой префаб и где появляется.
[System.Serializable]
public class EnemySpawn
{
    public GameObject prefab;
    public Vector2 position;
}

// Описание уровня. LevelManager в сцене читает этот ассет:
// расставляет врагов, запускает таймер, проверяет цель.
[CreateAssetMenu(menuName = "Gem Runner/Level Data")]
public class LevelData : ScriptableObject
{
    public int levelNumber = 1;
    public string levelName = "LEVEL 1";
    public string sceneName = "Level1";

    [Header("Правила")]
    public float timeLimit = 120f;       // секунд на уровень
    public int gemsToExit = 0;           // сколько камней нужно, чтобы выход открылся

    [Header("Враги")]
    public EnemySpawn[] spawns = new EnemySpawn[0];

    [Header("Что открывается после победы")]
    public LevelData nextLevel;          // пусто у последнего уровня
}
