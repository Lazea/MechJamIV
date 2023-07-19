using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SOGESys = SOGameEventSystem;

/// <summary>
/// Controls the movement of the player character using Unity's Rigidbody component.
/// </summary>
public class PlayerMovementController : MonoBehaviour
{
    public PlayerData data;

    [Header("Move Params")]
    public float moveSmooth = 1f;
    public float downForce = 5f;
    Vector2 inputMove;

    // Lateral Thrusters
    float lateralThrustCooldownTime;
    bool lateralThrustHold;

    // Jump
    bool jump;

    // Vertical Thrusters
    float verticalThrusterFuel;
    bool verticalThrusting;
    bool verticalThrustLock;
    float verticalThrustCooldownTime;

    [Header("Ground Check")]
    public float groundCheckDistance = 0.2f;
    [SerializeField]
    bool isGrounded;
    RaycastHit groundHit;
    public LayerMask groundMask;

    [Header("Physics Material")]
    public PhysicMaterial staticMaterial;
    public PhysicMaterial dynamicMaterial;


    [Header("Events")]
    public SOGESys.Events.FloatGameEvent onVerticalThrustersFuelChange;
    public SOGESys.BaseGameEvent onLateralThrustersReady;

    // Components
    Rigidbody rb;
    CapsuleCollider coll;
    Animator anim;

    bool dashed;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<CapsuleCollider>();
        coll.material = new PhysicMaterial();
        coll.material.staticFriction = staticMaterial.staticFriction;
        coll.material.dynamicFriction = staticMaterial.dynamicFriction;
        coll.material.frictionCombine = staticMaterial.frictionCombine;
        coll.material.bounciness = staticMaterial.bounciness;
        coll.material.bounceCombine = staticMaterial.bounceCombine;

        anim = GetComponent<Animator>();

        verticalThrustCooldownTime = data.verticalThrustCooldown;
    }

    // Update is called once per frame
    void Update()
    {
        var state = anim.GetCurrentAnimatorStateInfo(0);
        if (state.IsName("Dash Blend Tree"))
        {
            if(state.normalizedTime >= 0.05f && !dashed)
            {
                dashed = true;
                Debug.Log("DASH!!!");
            }
        }
        else
        {
            dashed = false;
        }
    }

    private void FixedUpdate()
    {
        HandleColliderMaterial();

        // Set speed for the animator
        float targetSpeed = inputMove.magnitude;
        if (lateralThrustHold)
            targetSpeed *= (data.speedScaler + 0.55f);
        else
            targetSpeed *= data.speedScaler;

        anim.SetFloat(
            "Speed",
            Mathf.Lerp(anim.GetFloat("Speed"), targetSpeed, moveSmooth));
        Vector2 move = Vector2.Lerp(
            new Vector2(anim.GetFloat("SpeedX"), anim.GetFloat("SpeedY")),
            inputMove,
            moveSmooth
        );
        anim.SetFloat("SpeedX", move.x);
        anim.SetFloat("SpeedY", move.y);

        // Handle ground
        isGrounded = GroundCheck();
        anim.SetBool("IsGrounded", isGrounded);

        if (jump)
        {
            jump = false;
            rb.AddForce(Vector3.up * data.jumpForce, ForceMode.VelocityChange);
        }
        else
        {
            // Applies a downward force
            rb.AddForce(Vector3.down * downForce);
            anim.SetBool("VertThrust", (verticalThrusting && verticalThrusterFuel > 0f));
            HandleVerticalThrust();
        }
        HandleLateralThrust();
    }

    /// <summary>
    /// Checks if the player character is grounded by performing a raycast downward.
    /// </summary>
    /// <returns>True if the character is grounded, false otherwise.</returns>
    bool GroundCheck()
    {
        Ray ray = new Ray(
            transform.position + coll.center,
            -transform.up);
        return Physics.Raycast(
            ray,
            out groundHit,
            coll.height * 0.5f + groundCheckDistance,
            groundMask
        );
    }

    /// <summary>
    /// Handles the collider material properties based on the grounded state and input movement.
    /// </summary>
    void HandleColliderMaterial()
    {
        // Set the target friction values
        float targetStaticFriction = 0f;
        float targetDynamicFriction = 0f;

        if (isGrounded)
        {
            // Check if the character is moving
            bool isMoving = (inputMove.magnitude >= 0.01f);
            coll.material.frictionCombine = isMoving ?
                dynamicMaterial.frictionCombine : staticMaterial.frictionCombine;
            targetStaticFriction = (inputMove.magnitude >= 0.01f) ?
                dynamicMaterial.staticFriction : staticMaterial.staticFriction;
            targetDynamicFriction = (inputMove.magnitude >= 0.01f) ?
                dynamicMaterial.dynamicFriction : staticMaterial.dynamicFriction;
        }
        else
        {
            coll.material.frictionCombine = dynamicMaterial.frictionCombine;
            targetStaticFriction = dynamicMaterial.staticFriction;
            targetDynamicFriction = dynamicMaterial.dynamicFriction;
        }

        // Smoothly interpolate the static and dynamic friction values
        coll.material.staticFriction = Mathf.Lerp(
            coll.material.staticFriction,
            targetStaticFriction,
            0.65f);
        coll.material.dynamicFriction = Mathf.Lerp(
            coll.material.dynamicFriction,
            targetDynamicFriction,
            0.65f);
    }

    /// <summary>
    /// Handles the vertical thrusting behavior of the player character.
    /// </summary>
    void HandleVerticalThrust()
    {
        if (verticalThrusting)
            ApplyVerticalThrustForce();

        if (!isGrounded)
        {
            verticalThrustCooldownTime = 0f;
        }
        else if(verticalThrusterFuel < data.maxVerticalThrusterFuel)
        {
            verticalThrustCooldownTime += Time.fixedDeltaTime;
            if (verticalThrustCooldownTime >= data.verticalThrustCooldown)
            {
                verticalThrustCooldownTime = data.verticalThrustCooldown;
                float refillRate = data.verticalThrusterFuleRate * 2f * Time.fixedDeltaTime;
                verticalThrusterFuel = Mathf.Min(
                    verticalThrusterFuel + refillRate,
                    data.maxVerticalThrusterFuel);
            }
        }

        onVerticalThrustersFuelChange.Raise(verticalThrusterFuel);
    }

    void ApplyVerticalThrustForce()
    {
        if (verticalThrusterFuel <= 0f)
            return;

        rb.AddForce(Vector3.up * data.verticalThrust);
        float _maxVerticalSpeed = data.maxVerticalSpeedFuelFalloff.Evaluate(
            verticalThrusterFuel / data.maxVerticalThrusterFuel);
        if (rb.velocity.y >= _maxVerticalSpeed)
        {
            Vector3 rbVel = rb.velocity;
            rbVel.y = _maxVerticalSpeed;
            rb.velocity = rbVel;
        }

        verticalThrusterFuel = Mathf.Max(
            verticalThrusterFuel - data.verticalThrusterFuleRate * Time.fixedDeltaTime,
            0f);
    }

    /// <summary>
    /// Handles the cooldown time for lateral thrust.
    /// </summary>
    void HandleLateralThrust()
    {
        float prevCooldown = lateralThrustCooldownTime;
        lateralThrustCooldownTime = Mathf.Max(
            lateralThrustCooldownTime - Time.fixedDeltaTime,
            0f);

        // Check if cooldown is completed this frame
        if (prevCooldown > 0f && lateralThrustCooldownTime <= 0f)
            onLateralThrustersReady.Raise();
    }

    /// <summary>
    /// Updates the input for character movement.
    /// </summary>
    /// <param name="move">The movement vector representing the input.</param>
    public void Move(Vector2 move)
    {
        inputMove = move;
    }

    public void Jump()
    {
        if (!isGrounded)
            return;

        jump = true;
    }

    /// <summary>
    /// Initiates vertical thrusting if not currently locked.
    /// </summary>
    public void VerticalThrust()
    {
        verticalThrusting = !verticalThrustLock;
    }

    /// <summary>
    /// Stops vertical thrusting.
    /// </summary>
    public void VerticalThrustRelease()
    {
        verticalThrusting = false;
    }

    /// <summary>
    /// Initiates lateral thrusting if the lateral thrust cooldown is not active.
    /// </summary>
    public void LateralThrust()
    {
        if (lateralThrustCooldownTime > 0f)
            return;

        anim.SetTrigger("LatThrust");
        lateralThrustCooldownTime = data.lateralThrustCooldown;
    }

    public void LateralThrustHold()
    {
        lateralThrustHold = true;
    }

    public void LateralThrustRelease()
    {
        lateralThrustHold = false;
    }

    private void OnDrawGizmos()
    {
        if(!Application.isPlaying)
        {
            Gizmos.color = Color.red;
            CapsuleCollider _coll = GetComponent<CapsuleCollider>();
            Vector3 a = transform.position + _coll.center;
            Gizmos.DrawLine(
                a, a - transform.up * (_coll.height * 0.5f + groundCheckDistance)
            );
        }
        else
        {
            Gizmos.color = (isGrounded) ? Color.green : Color.red;
            Vector3 a = transform.position + coll.center;
            Gizmos.DrawLine(
                a, a - transform.up * (coll.height * 0.5f + groundCheckDistance)
            );
        }
    }
}
