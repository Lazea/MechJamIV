using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MeshClickOn : MonoBehaviour
{
    [Header("Events")]
    public UnityEvent onEnter;
    public UnityEvent onExit;
    public UnityEvent onClick;

    // Start is called before the first frame update
    void Start()
    {
        onExit.Invoke();
    }
}
