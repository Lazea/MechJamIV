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
    public UnityEvent onVerticalThrustHold;
    public UnityEvent onVerticalThrustRelease;
    public UnityEvent onLateralThrust;

    [Header("Action Events")]
    public UnityEvent onShoot;
    public UnityEvent onMelee;
    public UnityEvent onInteractHold;
    public UnityEvent onInteractRelease;
    public UnityEvent onAbilityA;
    public UnityEvent onAbilityAHold;
    public UnityEvent onAbilityARelease;
    public UnityEvent onAbilityB;
    public UnityEvent onAbilityBHold;
    public UnityEvent onAbilityBRelease;
    public UnityEvent onAbilityC;
    public UnityEvent onAbilityCHold;
    public UnityEvent onAbilityCRelease;

    void Awake()
    {
        controls = new Controls().Gameplay;

        controls.Look.performed += ctx => { onLook.Invoke(ctx.ReadValue<Vector2>()); };
        controls.Look.canceled += ctx => { onLook.Invoke(Vector2.zero); };
        controls.Movement.performed += ctx => { onMove.Invoke(ctx.ReadValue<Vector2>()); };
        controls.Movement.canceled += ctx => { onMove.Invoke(Vector2.zero); };

        controls.VerticalThrust.started += ctx => { onVerticalThrustHold.Invoke(); };
        controls.VerticalThrust.canceled += ctx => { onVerticalThrustRelease.Invoke(); };
        controls.LateralThrust.started += ctx => { onLateralThrust.Invoke(); };

        controls.Shoot.started += ctx => { onShoot.Invoke(); };
        controls.Melee.started += ctx => { onMelee.Invoke(); };
        controls.Interact.started += ctx => { onInteractHold.Invoke(); };
        controls.Interact.performed += ctx => { onInteractHold.Invoke(); };
        controls.Interact.canceled += ctx => { onInteractRelease.Invoke(); };

        controls.AbilityA.started += ctx => { onAbilityA.Invoke(); };
        controls.AbilityA.performed += ctx => { onAbilityAHold.Invoke(); };
        controls.AbilityA.canceled += ctx => { onAbilityARelease.Invoke(); };
        controls.AbilityB.started += ctx => { onAbilityB.Invoke(); };
        controls.AbilityB.performed += ctx => { onAbilityBHold.Invoke(); };
        controls.AbilityB.canceled += ctx => { onAbilityBRelease.Invoke(); };
        controls.AbilityC.started += ctx => { onAbilityC.Invoke(); };
        controls.AbilityC.performed += ctx => { onAbilityCHold.Invoke(); };
        controls.AbilityC.canceled += ctx => { onAbilityCRelease.Invoke(); };
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
