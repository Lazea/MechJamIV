using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAimTarget : MonoBehaviour
{
    public float minDistance = 5f;
    public float maxDistance = 300f;
    public LayerMask mask;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;
        if(Physics.Raycast(
            ray,
            out hit,
            maxDistance,
            mask
        ))
        {
            if(Vector3.Distance(Camera.main.transform.position, hit.point) >= minDistance)
            {
                transform.position = hit.point;
            }
            else
            {
                transform.position = Camera.main.transform.position + Camera.main.transform.forward * minDistance;
            }
        }
        else
        {
            transform.position = Camera.main.transform.position + Camera.main.transform.forward * maxDistance;
        }
    }
}
