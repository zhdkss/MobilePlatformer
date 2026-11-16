using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Чтение и запись save.json в Application.persistentDataPath.
// Если файла нет или он сломан — игра берёт значения по умолчанию и не падает.
public static class SaveSystem
{
    const string FileName = "save.json";
    static SaveData data;

    public static string FilePath => Path.Combine(Application.persistentDataPath, FileName);

    // Первое обращение само загружает файл
    public static SaveData Data
    {
        get
        {
            if (data == null) Load();
            return data;
        }
    }

    public static void Load()
    {
        data = null;
        try
        {
            if (File.Exists(FilePath))
                data = JsonUtility.FromJson<SaveData>(File.ReadAllText(FilePath));
        }
        catch (Exception e)
        {
            Debug.LogWarning("Save file is broken, using defaults: " + e.Message);
            data = null;
        }

        if (data == null) data = new SaveData();   // файла нет, он пустой или сломан
        Validate();
    }

    public static void Save()
    {
        try
        {
            File.WriteAllText(FilePath, JsonUtility.ToJson(Data, true));
        }
        catch (Exception e)
        {
            Debug.LogWarning("Could not write save file: " + e.Message);
        }
    }

    // Даже правильный JSON может содержать мусор: чиним значения
    static void Validate()
    {
        if (data.unlockedLevels == null) data.unlockedLevels = new List<int>();
        if (!data.unlockedLevels.Contains(1)) data.unlockedLevels.Add(1);   // первый уровень открыт всегда
        data.volume = Mathf.Clamp01(data.volume);
        data.bestScore = Mathf.Max(0, data.bestScore);
    }

    public static bool IsUnlocked(int levelNumber)
    {
        return Data.unlockedLevels.Contains(levelNumber);
    }

    public static void UnlockLevel(int levelNumber)
    {
        if (!IsUnlocked(levelNumber)) Data.unlockedLevels.Add(levelNumber);
    }

    public static void SubmitScore(int score)
    {
        if (score > Data.bestScore) Data.bestScore = score;
    }

    public static void SetVolume(float value)
    {
        Data.volume = Mathf.Clamp01(value);
        ApplySettings();
        Save();                                    // настройки сохраняем сразу
    }

    public static void ApplySettings()
    {
        AudioListener.volume = Data.volume;
    }

    // Сброс прогресса: настройки (громкость) остаются
    public static void ResetProgress()
    {
        float volume = Data.volume;
        data = new SaveData();
        data.volume = volume;
        Save();
    }
}
