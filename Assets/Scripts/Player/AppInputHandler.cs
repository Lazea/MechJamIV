using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AppInputHandler : MonoBehaviour
{
    public Controls.AppActions appControls;

    public UnityEvent onPause;

    void Awake()
    {
        appControls = new Controls().App;

        appControls.Pause.started += ctx => { onPause.Invoke(); };
    }

    private void OnEnable()
    {
        appControls.Enable();
    }

    private void OnDisable()
    {
        appControls.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
