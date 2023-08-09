using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildHideUI : MonoBehaviour
{
    public bool hideInstead = true;
    public bool destroyOnStandalone = false;
    public bool destroyOnEditor = false;
    public bool destroyOnWebGL = true;

    // Start is called before the first frame update
    void Awake()
    {
#if UNITY_STANDALONE
        if (destroyOnStandalone)
            if(hideInstead)
                gameObject.SetActive(false);
            else
                Destroy(gameObject);
#elif UNITY_EDITOR
        if (destroyOnEditor)
            if (hideInstead)
                gameObject.SetActive(false);
            else
                Destroy(gameObject);
#elif UNITY_WEBGL
        if(destroyOnWebGL)
            if(hideInstead)
                gameObject.SetActive(false);
            else
                Destroy(gameObject);
#endif
    }
}
