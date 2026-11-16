using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Главное меню: выбор уровня по сохранению, громкость, сброс прогресса с подтверждением.
public class MainMenuController : MonoBehaviour
{
    public LevelData[] levels;
    public Button[] levelButtons;        // кнопка i запускает levels[i]
    public TMP_Text[] levelLabels;
    public TMP_Text bestScoreLabel;
    public Slider volumeSlider;
    public GameObject levelSelectPanel;
    public GameObject confirmPanel;

    void Start()
    {
        SaveSystem.ApplySettings();
        volumeSlider.SetValueWithoutNotify(SaveSystem.Data.volume);
        levelSelectPanel.SetActive(false);
        confirmPanel.SetActive(false);
        Refresh();
    }

    // Закрытые уровни серые и не нажимаются
    void Refresh()
    {
        for (int i = 0; i < levels.Length; i++)
        {
            bool open = SaveSystem.IsUnlocked(levels[i].levelNumber);
            levelButtons[i].interactable = open;
            levelLabels[i].text = open ? levels[i].levelName : "LOCKED";
        }
        bestScoreLabel.text = "BEST SCORE: " + SaveSystem.Data.bestScore;
    }

    public void OpenLevelSelect() { Refresh(); levelSelectPanel.SetActive(true); }
    public void CloseLevelSelect() { levelSelectPanel.SetActive(false); }

    public void PlayLevel(int index)
    {
        if (!SaveSystem.IsUnlocked(levels[index].levelNumber)) return;
        Time.timeScale = 1f;
        SceneManager.LoadScene(levels[index].sceneName);
    }

    public void OnVolumeChanged(float value) { SaveSystem.SetVolume(value); }

    public void AskReset() { confirmPanel.SetActive(true); }
    public void CancelReset() { confirmPanel.SetActive(false); }

    public void ConfirmReset()
    {
        SaveSystem.ResetProgress();
        confirmPanel.SetActive(false);
        Refresh();
    }

    void OnApplicationPause(bool paused)
    {
        if (paused) SaveSystem.Save();
    }
}
