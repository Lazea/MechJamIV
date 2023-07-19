using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Debris : MonoBehaviour
{
    public float minLifeTime = 1f;
    public float maxLifeTime = 1f;
    float lifeTime;

    public float launchForce;
    public float upwardLaunchForce;
    public float launchTorque;

    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if(rb != null)
        {
            Vector3 force = UnityEngine.Random.insideUnitSphere;
            force.y = Mathf.Clamp(force.y, 0.25f, 1f);
            force *= launchForce;
            rb.AddForce(force, ForceMode.Impulse);
            rb.AddForce(Vector3.up * upwardLaunchForce, ForceMode.Impulse);
            rb.AddTorque(UnityEngine.Random.insideUnitSphere * launchTorque, ForceMode.Impulse);
        }

        lifeTime = minLifeTime;
        if (minLifeTime != maxLifeTime)
            lifeTime = UnityEngine.Random.Range(minLifeTime, maxLifeTime);

        Destroy(gameObject, lifeTime);
    }

    private void OnDestroy()
    {

    }
}
