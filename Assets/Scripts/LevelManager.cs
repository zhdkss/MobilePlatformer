using TMPro;
using UnityEngine;

// Один и тот же код во всех уровнях: читает LevelData,
// расставляет врагов, ведёт таймер, проверяет цель и сохраняет прогресс.
public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    public LevelData data;
    public TMP_Text timerLabel;
    public float messageTime = 2f;       // сколько секунд висит подсказка вместо таймера

    float timeLeft;
    float messageTimer;
    bool finished;

    void Awake() { Instance = this; }

    void Start()
    {
        SaveSystem.ApplySettings();
        timeLeft = data.timeLimit;

        // Таймер каждый кадр переписывает свой текст, поэтому он не должен быть надписью кнопки
        if (timerLabel != null && timerLabel.GetComponentInParent<UnityEngine.UI.Button>() != null)
        {
            Debug.LogWarning("Timer Label is a button text (" + timerLabel.name + "). Run Tools > Gem Runner > Data 3 to create a real timer text.");
            timerLabel = null;
        }

        foreach (EnemySpawn spawn in data.spawns)
            if (spawn.prefab != null)
                Instantiate(spawn.prefab, spawn.position, Quaternion.identity);
    }

    void Update()
    {
        if (finished) return;

        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0f)
        {
            finished = true;
            GameAudio.Play(Sfx.Lose);
            SceneLoader.Restart();              // время вышло
            return;
        }

        if (messageTimer > 0f) messageTimer -= Time.deltaTime;
        else if (timerLabel) timerLabel.text = Mathf.CeilToInt(timeLeft).ToString();
    }

    // Вызывает LevelExit, когда игрок дошёл до выхода
    public void TryExit()
    {
        int missing = data.gemsToExit - CoinCounter.Instance.Gems;
        if (missing > 0)
        {
            if (timerLabel) timerLabel.text = "NEED " + missing + " GEMS";
            messageTimer = messageTime;
            return;
        }
        Complete();
    }

    // Уровень пройден: рекорд, следующий уровень, запись на диск
    public void Complete()
    {
        if (finished) return;
        finished = true;

        SaveSystem.SubmitScore(CoinCounter.Instance.Score);
        if (data.nextLevel != null) SaveSystem.UnlockLevel(data.nextLevel.levelNumber);
        SaveSystem.Save();

        GameAudio.Play(Sfx.Win);
        SceneLoader.LoadNext();
    }

    // Игру свернули или закрывают — сохраняем
    void OnApplicationPause(bool paused)
    {
        if (paused) SaveSystem.Save();
    }
}
