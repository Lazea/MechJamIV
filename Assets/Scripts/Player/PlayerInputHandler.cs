using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Handles player input events and invokes corresponding Unity events based on the player's actions.
/// </summary>
public class PlayerInputHandler : MonoBehaviour
{
    public Controls.GameplayActions controls;

    Vector2 look;
    Vector2 move;

    [Header("Movement Events")]
    public UnityEvent<Vector2> onLook;
    public UnityEvent<Vector2> onMove;
    public UnityEvent onVerticalThrustHold;
    public UnityEvent onVerticalThrustRelease;
    public UnityEvent onLateralThrust;

    [Header("Action Events")]
    public UnityEvent onFire;
    public UnityEvent onFireRelease;
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

        controls.Look.performed += ctx => { look = ctx.ReadValue<Vector2>(); };
        controls.Look.canceled += ctx => { look = Vector2.zero; };
        controls.Movement.performed += ctx => { move = ctx.ReadValue<Vector2>(); };
        controls.Movement.canceled += ctx => { move = Vector2.zero; };

        controls.VerticalThrust.started += ctx => { onVerticalThrustHold.Invoke(); };
        controls.VerticalThrust.canceled += ctx => { onVerticalThrustRelease.Invoke(); };
        controls.LateralThrust.started += ctx => { onLateralThrust.Invoke(); };

        controls.Fire.started += ctx => { onFire.Invoke(); };
        controls.Fire.canceled += ctx => { onFireRelease.Invoke(); };
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
        onLook.Invoke(look);
        onMove.Invoke(move);
    }
}
