using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the rotation of a camera target object based on input.
/// </summary>
public class CameraTarget : MonoBehaviour
{
    Vector2 look;
    public float xSensitivity;
    public float ySensitivity;
    public bool invertedY;
    public float maxVerticalAngle = 340f;
    public float minVerticalAngle = 60f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation *= Quaternion.AngleAxis(
            look.x * xSensitivity, Vector3.up);

        float yLook = look.y * ySensitivity;
        if (!invertedY)
            yLook *= -1f;
        transform.rotation *= Quaternion.AngleAxis(yLook, Vector3.right);

        Vector3 angles = transform.localEulerAngles;
        angles.z = 0f;

        // Clamp up and down roation
        float angle = transform.localEulerAngles.x;
        if (angle > 180f && angle < maxVerticalAngle)
        {
            angles.x = maxVerticalAngle;
        }
        else if (angle < 180f && angle > minVerticalAngle)
        {
            angles.x = minVerticalAngle;
        }

        transform.localEulerAngles = angles;
    }

    /// <summary>
    /// Sets the input for camera rotation.
    /// </summary>
    /// <param name="look">The input vector representing the camera rotation.</param>
    public void SetLook(Vector2 look)
    {
        this.look = look;
    }
}
