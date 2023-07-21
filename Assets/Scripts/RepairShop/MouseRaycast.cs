using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseRaycast : MonoBehaviour
{
    public LayerMask mask;
    MeshClickOn hoverObject;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, mask))
        {
            if(hoverObject == null)
            {
                hoverObject = hitInfo.transform.GetComponent<MeshClickOn>();
                hoverObject.onEnter.Invoke();
            }
        }
        else
        {
            if(hoverObject != null)
            {
                hoverObject.onExit.Invoke();
                hoverObject = null;
            }
        }

        if(hoverObject != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            hoverObject.onClick.Invoke();
            Debug.Log("Clicked on " + hoverObject.transform.name);
        }
    }
}
