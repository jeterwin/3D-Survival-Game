using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseScript : MonoBehaviour
{
    public static PauseScript Instance;
    private bool isPaused = false;
    public bool IsPaused { get { return isPaused; } }

    [SerializeField]
    private GameObject pauseMenuUI;

    private void Awake()
    {
        Instance = this;
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            //Could turn them into two functions for more readability, but will leave them this
            //way for now
            isPaused = !isPaused;
            pauseMenuUI.SetActive(isPaused);
            Time.timeScale = isPaused ? 0f : 1f;
        }
    }
}
