using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class RepairCam : MonoBehaviour
{
    Volume post;
    DepthOfField DOF;

    float fD;

    public Transform[] points;

    public Transform currentPoint;

    public float rate = 2;
    public float rotRate = 1;

    // Start is called before the first frame update
    void Start()
    {
        post = FindObjectOfType<Volume>();
        post.profile.TryGet(out DOF);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, currentPoint.position, rate * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, currentPoint.rotation, rotRate * Time.deltaTime);

        fD = Vector3.Distance(transform.position, currentPoint.GetChild(0).position);
        DOF.focusDistance.value = Mathf.Lerp(DOF.focusDistance.value, fD, 5 * Time.deltaTime);
    }

    public void SelectPoint(Transform t)
    {
        currentPoint = t;
    }
}
