using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // Вызывается из кнопки: пишем имя сцены в поле On Click
    public void LoadByName(string sceneName)
    {
        Time.timeScale = 1f;               // на случай, если игра была на паузе
        SceneManager.LoadScene(sceneName);
    }

    public static void LoadNext()
    {
        int next = SceneManager.GetActiveScene().buildIndex + 1;
        SceneManager.LoadScene(next);       // Level1 → Level2 → Level3_Boss → Win
    }

    public static void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Quit()
    {
        Application.Quit();                 // в редакторе ничего не делает, на телефоне закрывает
    }
}