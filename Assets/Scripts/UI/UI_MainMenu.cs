using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_MainMenu : MonoBehaviour
{
    UI_Aesthetics uA;
    UI_MenuDirector director;

    public GameObject titleMain;
    public GameObject titleSecond;
    public UI_SubElement startPrompt;
    public bool menuActive;


    // Start is called before the first frame update
    void Start()
    {
        uA = FindObjectOfType<UI_Aesthetics>();
        director = GetComponent<UI_MenuDirector>();
        titleSecond.SetActive(false);

        uA.StartCoroutine(uA.SwapPanel(director, director.UIMajor[0]));
        uA.StartCoroutine(uA.Blink(startPrompt));
    }

    // Update is called once per frame
    void Update()
    {
        if (!uA.swappingMenu && Input.anyKeyDown && !menuActive)
        {
            menuActive = true;

            titleMain.SetActive(false);
            titleSecond.SetActive(true);

            uA.StartCoroutine(uA.SwapPanel(director, director.UIMajor[1]));
        }
    }
}
