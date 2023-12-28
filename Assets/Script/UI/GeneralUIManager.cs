using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GeneralUIManager : MonoBehaviour
{
    public static GeneralUIManager main;
    private void Awake()
    {
        main = this;
        VictoryWindow.gameObject.SetActive(false);
        DefeatWindow.gameObject.SetActive(false);
        PauseMenu.gameObject.SetActive(false);
    }
    private void OnApplicationPause(bool pause)
    {
        ShowPauseWindow();
    }
    public GraphicRaycaster Raycaster;
    public GameObject PauseMenu;
    public GameObject VictoryWindow;
    public GameObject DefeatWindow;
    public void ShowPauseWindow()
    {
        PauseMenu.gameObject.SetActive(true);
    }
    public void ShowVictoryWindow()
    {
        VictoryWindow.gameObject.SetActive(true);
    }
    public void ShowDefeatWindow()
    {
        DefeatWindow.gameObject.SetActive(true);
    }

}
