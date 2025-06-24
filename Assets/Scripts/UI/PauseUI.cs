using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{
    public GameObject pauseMenuUI;
    private bool isPaused = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
               
            }
            else
            {
                Pause();
            }
        }

        if (isPaused)
        {
            AudioListener.pause = true;
        }
        else
        {
            AudioListener.pause = false;
        }

    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        // Nascondi il cursore del mouse
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        pauseMenuUI.SetActive(true);
        // rendi visibile il cursore del mouse
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        isPaused = true;
    }

    public void Menu()
    {
        FindObjectOfType<AudioManager>().Play("ButtonUI");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 3);
    }

    public void QuitGame()
    {
        FindObjectOfType<AudioManager>().Play("ButtonUI");
        Application.Quit();
    }

}
