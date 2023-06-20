using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    [Header("Move Params")]
    public float moveSmooth = 1f;
    public float downForce = 5f;
    Vector2 inputMove;

    [Header("Lateral Thrusters")]
    public float lateralThrustCooldown = 1.5f;
    float lateralThrustCooldownTime;

    [Header("Vertical Thrusters")]
    public float verticalThrust = 15f;
    public AnimationCurve maxVerticalSpeedFuleFalloff;
    public float maxVerticalThrusterFuel = 100f;
    public float verticalThrusterFuleRate = 10f;
    float verticalThrusterFuel;
    bool verticalThrusting;
    bool verticalThrustLock;
    public float verticalThrustCooldown = 1.5f;
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

    // Components
    Rigidbody rb;
    CapsuleCollider coll;
    Animator anim;

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

        verticalThrustCooldownTime = verticalThrustCooldown;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        HandleColliderMaterial();

        anim.SetFloat(
            "Speed",
            Mathf.Lerp(anim.GetFloat("Speed"), inputMove.magnitude, moveSmooth)
        );
        Vector2 move = Vector2.Lerp(
            new Vector2(anim.GetFloat("SpeedX"), anim.GetFloat("SpeedY")),
            inputMove,
            moveSmooth
        );
        anim.SetFloat("SpeedX", move.x);
        anim.SetFloat("SpeedY", move.y);

        rb.AddForce(Vector3.down * downForce);
        bool grounded = GroundCheck();
        if(!isGrounded && grounded)
        {
            verticalThrusterFuel = maxVerticalThrusterFuel;
            verticalThrustLock = true;
        }
        if(verticalThrusterFuel != maxVerticalThrusterFuel)
        {
            if(!verticalThrustLock && isGrounded)
            {
                verticalThrusterFuel = maxVerticalThrusterFuel;
            }
        }
        isGrounded = GroundCheck();

        HandleVerticalThrust();
        lateralThrustCooldownTime = Mathf.Max(
            lateralThrustCooldownTime - Time.fixedDeltaTime,
            0f);
    }

    public bool GroundCheck()
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

    void HandleColliderMaterial()
    {
        float targetStaticFriction = 0f;
        float targetDynamicFriction = 0f;
        if (isGrounded)
        {
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

        coll.material.staticFriction = Mathf.Lerp(
            coll.material.staticFriction,
            targetStaticFriction,
            0.65f);
        coll.material.dynamicFriction = Mathf.Lerp(
            coll.material.dynamicFriction,
            targetDynamicFriction,
            0.65f);
    }

    public void HandleVerticalThrust()
    {
        if(verticalThrustLock)
        {
            verticalThrustCooldownTime += Time.fixedDeltaTime;
            if(verticalThrustCooldownTime >= verticalThrustCooldown)
            {
                verticalThrustCooldownTime = 0f;
                verticalThrustLock = false;
            }
        }
        else if (verticalThrusting)
        {
            if (verticalThrusterFuel <= 0f)
            {
                verticalThrusterFuel = 0f;
            }
            else
            {
                rb.AddForce(Vector3.up * verticalThrust);
                float _maxVerticalSpeed = maxVerticalSpeedFuleFalloff.Evaluate(
                    verticalThrusterFuel / maxVerticalThrusterFuel);
                if (rb.velocity.y >= _maxVerticalSpeed)
                {
                    Vector3 rbVel = rb.velocity;
                    rbVel.y = _maxVerticalSpeed;
                    rb.velocity = rbVel;
                }

                verticalThrusterFuel -= verticalThrusterFuleRate * Time.fixedDeltaTime;
            }
        }
    }

    public void Move(Vector2 move)
    {
        inputMove = move;
    }

    public void VerticalThrust()
    {
        verticalThrusting = !verticalThrustLock;
    }

    public void VerticalThrustRelease()
    {
        verticalThrusting = false;
    }

    public void LateralThrust()
    {
        if (lateralThrustCooldownTime > 0f)
            return;

        anim.SetTrigger("LatThrust");
        lateralThrustCooldownTime = lateralThrustCooldown;
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
