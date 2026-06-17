using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private string menuSceneName = "MainMenu";

    public bool IsPaused { get; private set; }

    private void Start()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    public void Pause()
    {
        IsPaused = true;
        if (pausePanel != null)
            pausePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        IsPaused = false;
        if (pausePanel != null)
            pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void Quit()
    {
        Time.timeScale = 1f; // a próxima cena precisa começar com o tempo rodando
        SceneManager.LoadScene(menuSceneName);
    }
}
