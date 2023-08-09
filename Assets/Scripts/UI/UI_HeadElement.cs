using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_HeadElement : MonoBehaviour
{
    [Header("Provide if Head has sub items")]
    public UI_SubElement[] items;

    [Header("If head should be frame, then what frame?")]
    public List<UI_PanelController> frame;

    public float swapTime = .5f;

    private void Awake()
    {
        if (frame.Count > 0)
        {
            foreach(UI_PanelController u in frame)
            {
                u.heads.Add(this);
            }
            
        }
    }
}
