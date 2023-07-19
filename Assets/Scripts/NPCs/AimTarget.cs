using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimTarget : MonoBehaviour
{
    public Transform aimTransform;
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
        Transform _aimTransform = aimTransform;
        if (_aimTransform == null)
            _aimTransform = Camera.main.transform;
        Ray ray = new Ray(
            _aimTransform.position,
            _aimTransform.forward);
        RaycastHit hit;
        if(Physics.Raycast(
            ray,
            out hit,
            maxDistance,
            mask
        ))
        {
            if(Vector3.Distance(_aimTransform.position, hit.point) >= minDistance)
            {
                transform.position = hit.point;
            }
            else
            {
                transform.position = _aimTransform.position + _aimTransform.forward * minDistance;
            }
        }
        else
        {
            transform.position = _aimTransform.position + _aimTransform.forward * maxDistance;
        }
    }
}
