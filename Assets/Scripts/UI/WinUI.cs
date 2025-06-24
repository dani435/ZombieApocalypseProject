using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinUI : MonoBehaviour
{
    
    public void RestartButton()
    {
        FindObjectOfType<AudioManager>().Play("ButtonUI");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
    }
    public void BackMenu()
    {
        FindObjectOfType<AudioManager>().Play("ButtonUI");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
    public void QuitGame()
    {
        FindObjectOfType<AudioManager>().Play("ButtonUI");
        Debug.Log("QUIT");
        Application.Quit();

    }
}
