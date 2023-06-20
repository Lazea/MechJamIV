using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    [Header("Move Params")]
    public float moveSmooth = 1f;
    public float downForce = 5f;
    Vector2 inputMove;

    [Header("Jumping")]
    public float jumpHeight = 2.5f;
    int airJumpsPerformed;
    public int airJumps = 1;

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
        coll.material = staticMaterial;

        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
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

        HandleColliderMaterial();
    }

    private void FixedUpdate()
    {
        rb.AddForce(Vector3.down * downForce);
        bool grounded = GroundCheck();
        if(!isGrounded && grounded)
        {
            airJumpsPerformed = 0;
        }
        isGrounded = GroundCheck();
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
        float targetFric = (inputMove.magnitude >= 0.05f) ? dynamicMaterial.staticFriction : staticMaterial.staticFriction;
        float fric = Mathf.Lerp(coll.material.staticFriction, targetFric, 0.25f);
        coll.material.staticFriction = fric;
        coll.material.dynamicFriction = fric;
    }

    public void Move(Vector2 move)
    {
        inputMove = move;
    }

    public bool CanJump()
    {
        if(isGrounded)
        {
            return true;
        }
        else
        {
            return (airJumpsPerformed < airJumps);
        }
    }

    public void Jump()
    {
        if(!CanJump())
            return;
        
        if(!isGrounded)
        {
            airJumpsPerformed++;
        }
        
        float jumpSpeed = Mathf.Sqrt(-2f * (Physics.gravity.y - downForce) * jumpHeight);
        Vector3 vel = rb.velocity;
        vel.y = jumpSpeed;
        rb.velocity = vel;
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
