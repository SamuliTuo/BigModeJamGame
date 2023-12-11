using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu instance { get; private set; }
    private void Awake() { if (instance != null && instance != this) Destroy(this); else instance = this; }

    public bool paused = false;


    private Transform pausePanel;

    private void Start()
    {
        pausePanel = transform.GetChild(0);
    }

    public void GamePaused()
    {
        paused = true;
        pausePanel.gameObject.SetActive(true);
    }

    public void GameUnPaused()
    {
        paused = false;
        pausePanel.gameObject.SetActive(false);
    }

    public void ExitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif

        Application.Quit();
    }

    public void OnBGMValueChanged(float newValue)
    {
        print(newValue);
    }
    public void OnSFXValueChanged(float newValue)
    {
        print(newValue);
    }
    public void OnCamSpeedValueChanged(float newValue)
    {
        print(newValue);
    }

    public void OnContinue()
    {

    }
    public void OnExitGame()
    {

    }
}
