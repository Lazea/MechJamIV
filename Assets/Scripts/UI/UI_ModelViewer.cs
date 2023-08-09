using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_ModelViewer : MonoBehaviour
{
    public Transform model;

    public float speed;
    public Vector3 axis;

    void FixedUpdate()
    {
        model.Rotate(axis * speed * Time.deltaTime);
    }
}
