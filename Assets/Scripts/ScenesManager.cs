using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : Singleton<ScenesManager>
{
    public int GetSceneID()
    {
        int i = SceneManager.GetActiveScene().buildIndex;
        Debug.Log("Getting Scene ID: " + i);
        return i;
    }

    public void ReloadScene()
    {
        Debug.Log("Reloading Scene");
        LoadScene(GetSceneID());
    }

    public void LoadMainMenu()
    {
        Debug.Log("Loading Main Menu Scene");
        LoadScene(0);
    }

    public void LoadNextScene()
    {
        Debug.Log("Loading Next Scene");
        int i = GetSceneID() + 1;
        LoadScene(i);
    }

    public void LoadPreviousScene()
    {
        Debug.Log("Loading Previous Scene");
        int i = GetSceneID() - 1;
        LoadScene(i);
    }

    public void LoadScene(int i)
    {
        Debug.Log("Loading Scene: " + i);
        Time.timeScale = 1f;
        SceneManager.LoadScene(i);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game");
        Time.timeScale = 1f;
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
