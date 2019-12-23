using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MouseHandler : MonoBehaviour
{
    // public paramter fields
    public Camera camera;

    // default value
    private Vector3 aimDirection = new Vector3(0, 0, 0);

    public Vector3 getAimDirection()
    {
        return aimDirection;
    }

    // computes a normalised coefficient between two vectors using some
    // exponential decay
    public float getNormalisedCoefficient(Vector3 v1, Vector3 v2)
    {
        // angle between aim and direction to it (som value between 0 and 180
        float angle = Vector3.Angle(v1, v2);
        // we also want to normalize it polynomically to ease aiming
        float angleNorm = (float)Math.Pow(1 - (angle / 180f), 5);
        return angleNorm;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // get the position of the player and the mouse cursor
        Vector3 playerScreenPos = camera.WorldToScreenPoint(transform.position);
        Vector3 mouseScreenPos = Input.mousePosition;
        // find which direction the player is aiming
        aimDirection = -(playerScreenPos - mouseScreenPos).normalized;
    }
}
