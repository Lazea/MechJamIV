using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLocker : MonoBehaviour
{
    public bool debugKeepCursorUnlocked;

    // Start is called before the first frame update
    void Start()
    {
        int buildIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        switch(buildIndex)
        {
            case 3:
                if(!debugKeepCursorUnlocked)
                    LockMouse();
                break;
            default:
                UnlockMouse();
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown("m"))
            ToggleMouse();
#endif
    }

    public void LockMouse()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void UnlockMouse()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ToggleMouse()
    {
        Cursor.visible = !Cursor.visible;
        Cursor.lockState = (Cursor.visible) ? CursorLockMode.None : CursorLockMode.Locked;
    }
}
