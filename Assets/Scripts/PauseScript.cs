using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseScript : MonoBehaviour
{
    private bool isPaused = false;
    public bool IsPaused { get { return isPaused; } }

    [SerializeField]
    private GameObject pauseMenuUI;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            //Could turn them into two functions for more readability, but will leave them this
            //way for now
            isPaused = !isPaused;
            pauseMenuUI.SetActive(isPaused);
            Time.timeScale = isPaused ? 0f : 1f;
            //Make sure to keep the state of the other components so you cannot access these
            //while the game is paused
            ScriptManagers.Instance.equipSystem.enabled = !ScriptManagers.Instance.equipSystem.enabled;
            ScriptManagers.Instance.inventorySystem.enabled = !ScriptManagers.Instance.inventorySystem.enabled;
            ScriptManagers.Instance.craftingSystem.enabled = !ScriptManagers.Instance.craftingSystem.enabled;
        }
    }
}
