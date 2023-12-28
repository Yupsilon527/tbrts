using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartLevelButton : MonoBehaviour
{
    
    public void RestartLevel()
    {
        List<int> scIndex = new List<int>();
        for (int i = 0; i< SceneManager.sceneCount; i++)        
        {
            scIndex.Add( SceneManager.GetSceneAt(i).buildIndex);
        }

        bool first = true;
        foreach (int scene in scIndex)
        {
            SceneManager.LoadScene(scene, first ? LoadSceneMode.Single : LoadSceneMode.Additive);
            first = false;
        }
    }
}
