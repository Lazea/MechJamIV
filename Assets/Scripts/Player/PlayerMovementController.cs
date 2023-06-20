using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    Vector2 inputMove;
    public float moveSmooth = 1f;
    public float downForce = 5f;

    public PhysicMaterial staticMaterial;
    public PhysicMaterial dynamicMaterial;

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
    }

    void HandleColliderMaterial()
    {
        float targetFric = (anim.GetFloat("Speed") >= 0.05f) ? dynamicMaterial.staticFriction : staticMaterial.staticFriction;
        float fric = Mathf.Lerp(coll.material.staticFriction, targetFric, 0.25f);
        coll.material.staticFriction = fric;
        coll.material.dynamicFriction = fric;
    }

    public void Move(Vector2 move)
    {
        inputMove = move;
    }
}
