using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void LoadRandom()
    {
        Debug.Log("Loading Random!");
        SceneManager.LoadScene(1);
    }
}
