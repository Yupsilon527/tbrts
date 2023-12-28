using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGameButton : MonoBehaviour
{
    public string MainScene;
    public string[] SecondaryScenes;
    public void ChangeScene()
    {
        SceneManager.LoadScene(MainScene,LoadSceneMode.Single);
        foreach (string scene in SecondaryScenes)
        {
            SceneManager.LoadScene(scene, LoadSceneMode.Additive);
        }
    }
}
