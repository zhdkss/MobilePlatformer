using System;
using System.Collections.Generic;

// То, что лежит в save.json. Только простые типы и List: JsonUtility не умеет Dictionary.
[Serializable]
public class SaveData
{
    public int version = 2;
    public int bestScore = 0;
    public List<int> unlockedLevels = new List<int> { 1 };
    public float musicVolume = 0.8f;   // 0..1 на ползунке, в миксер уходит в децибелах
    public float sfxVolume = 0.8f;
}
