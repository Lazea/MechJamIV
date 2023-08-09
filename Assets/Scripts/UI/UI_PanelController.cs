using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_PanelController : MonoBehaviour
{
    public List<UI_HeadElement> heads;

    public GameObject panel;

    public bool holdOpen;


    // Update is called once per frame
    void Update()
    {
        if(heads.Count > 0)
        {
            panel.SetActive(checkOn());
        }
    }

    bool checkOn()
    {
        bool test = false;

        //if ANY heads are active, then activate
        foreach(UI_HeadElement h in heads)
        {
            if (h.gameObject.activeSelf)
                test = true;
        }

        return test || holdOpen;
    }
}
