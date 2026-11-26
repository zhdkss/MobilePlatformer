using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject pausePanel;      // панель паузы, выключена на старте

    public void Pause()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0f;           // время стоит
        GameAudio.SetPaused(true);     // музыка тише и глуше
    }

    public void Resume()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f;           // время идёт дальше
        GameAudio.SetPaused(false);
    }

    public void Restart()
    {
        Time.timeScale = 1f;           // иначе новая сцена загрузится замороженной
        GameAudio.SetPaused(false);
        SceneLoader.Restart();
    }
}