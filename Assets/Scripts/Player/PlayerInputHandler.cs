using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInputHandler : MonoBehaviour
{
    public Controls.GameplayActions controls;

    void Awake()
    {
        controls = new Controls().Gameplay;
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
