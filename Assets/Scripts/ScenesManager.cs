using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : Singleton<ScenesManager>
{
    public int GetSceneID()
    {
        return SceneManager.GetActiveScene().buildIndex;
    }

    public void ReloadScene()
    {
        LoadScene(GetSceneID());
    }

    public void LoadMainMenu()
    {
        LoadScene(0);
    }

    public void LoadNextScene()
    {
        int i = GetSceneID();
        LoadScene(i++);
    }

    public void LoadPreviousScene()
    {
        int i = GetSceneID();
        LoadScene(i--);
    }

    public void LoadScene(int i)
    {
        SceneManager.LoadScene(i);
    }
}
