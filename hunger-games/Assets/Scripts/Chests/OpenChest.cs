using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenChest : MonoBehaviour
{
    private bool rotating = false;
    private bool closed = true;

    private readonly float ANGULAR_VELOCITY = 90;

    // Update is called once per frame
    void Update()
    {
        if (rotating)
        {
            int direction = closed ? 1 : -1;
            transform.parent.Rotate(0, 0, direction * ANGULAR_VELOCITY * Time.deltaTime);

            CheckRotationLimits();
        }
    }

    void CheckRotationLimits()
    {
        float rotZ = transform.parent.rotation.eulerAngles.z;
        
        if (closed && rotZ >= 90)
        {
            rotating = false;
            closed = false;
            transform.parent.Rotate(0, 0, 90 - rotZ);
        }
        else if (!closed & rotZ >= 270)
        {
            rotating = false;
            closed = true;
            transform.parent.Rotate(0, 0, -rotZ);
        }
    }

    void OnMouseDown()
    {
        rotating = true;
    }
}
