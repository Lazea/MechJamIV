using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInputHandler : MonoBehaviour
{
    public Controls.GameplayActions controls;

    [Header("Movement Events")]
    public UnityEvent<Vector2> onLook;
    public UnityEvent<Vector2> onMove;
    public UnityEvent onJump;
    public UnityEvent onDash;

    [Header("Action Events")]
    public UnityEvent onShoot;
    public UnityEvent onMelee;
    public UnityEvent onInteractHold;
    public UnityEvent onAbilityA;
    public UnityEvent onAbilityAHold;
    public UnityEvent onAbilityB;
    public UnityEvent onAbilityBHold;
    public UnityEvent onAbilityC;
    public UnityEvent onAbilityCHold;

    void Awake()
    {
        controls = new Controls().Gameplay;

        controls.Look.performed += ctx => { onLook.Invoke(ctx.ReadValue<Vector2>()); };
        controls.Look.canceled += ctx => { onLook.Invoke(Vector2.zero); };
        controls.Movement.performed += ctx => { onMove.Invoke(ctx.ReadValue<Vector2>()); };
        controls.Movement.canceled += ctx => { onMove.Invoke(Vector2.zero); };

        controls.Jump.started += ctx => { onJump.Invoke(); };
        controls.Dash.started += ctx => { onDash.Invoke(); };

        controls.Shoot.started += ctx => { onShoot.Invoke(); };
        controls.Melee.started += ctx => { onMelee.Invoke(); };
        controls.Interact.started += ctx => { onInteractHold.Invoke(); };
        controls.Interact.performed += ctx => { onInteractHold.Invoke(); };

        controls.AbilityA.started += ctx => { onAbilityA.Invoke(); onAbilityAHold.Invoke(); };
        controls.AbilityA.performed += ctx => { onAbilityAHold.Invoke(); };
        controls.AbilityB.started += ctx => { onAbilityB.Invoke(); onAbilityBHold.Invoke(); };
        controls.AbilityB.performed += ctx => { onAbilityBHold.Invoke(); };
        controls.AbilityC.started += ctx => { onAbilityC.Invoke(); onAbilityCHold.Invoke(); };
        controls.AbilityC.performed += ctx => { onAbilityCHold.Invoke(); };
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
