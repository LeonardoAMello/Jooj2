using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseControl : MonoBehaviour
{
    public static bool gameIsPaused = false;
    public PlayerController player;
    public GameObject pauseScreen;
    public Animator transicao;

    private void Start()
    {
        transicao.SetTrigger("Finish");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            if (gameIsPaused)
                Resume();
            else
                Pause();
    }

    public void Resume()
    {
        gameIsPaused = false;
        Time.timeScale = 1f;
        pauseScreen.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void Pause()
    {
        gameIsPaused = true;
        Time.timeScale = 0f;
        pauseScreen.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Menu()
    {
        Resume();

        Spawner.enemyCount = 0;

        PlayerPrefs.SetInt("gameStarted", 1);
        transicao.SetTrigger("Start");

        Invoke("LoadMenu", 1f);
    }

    private void LoadMenu()
    {
        StartCoroutine(new MenuController().LoadAsynchronously(0));
    }
}
