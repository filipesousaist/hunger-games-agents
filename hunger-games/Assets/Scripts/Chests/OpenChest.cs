using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenChest : MonoBehaviour
{
    public Chest chest;

    //private bool rotating = false;
    private bool closed = true;

    private readonly float ANGULAR_VELOCITY = 90;

    private Coroutine openCo;
    private Coroutine closeCo;

    // Update is called once per frame
    void Update()
    {
        /*
        if (rotating)
        {
            int direction = closed ? 1 : -1;
            transform.parent.Rotate(0, 0, direction * ANGULAR_VELOCITY * Time.deltaTime);

            CheckRotationLimits();
        }
        */

    }


    void CheckRotationLimits()
    {
        float rotZ = Utils.ClampMod(transform.parent.rotation.eulerAngles.z, -180, 180);
        
        if (closed && rotZ >= 90)
        {
            //rotating = false;
            closed = false;
            transform.parent.Rotate(0, 0, 90 - rotZ);
        }
        else if (!closed && rotZ <= 0)
        {
            //rotating = false;
            closed = true;
            transform.parent.Rotate(0, 0, -rotZ);
        }
    }

    void OnMouseDown()
    {
        if (closed)
            openCo = StartCoroutine(OpenCo());
        else
            closeCo = StartCoroutine(CloseCo());
    }

    private IEnumerator OpenCo()
    {
        closed = false;
        if (closeCo != null)
            StopCoroutine(closeCo);
        chest.DisplayWeapon();

        Transform lid = transform.parent;
        while (Utils.ClampMod(lid.rotation.eulerAngles.z, -180, 180) < 90)
        {
            lid.Rotate(0, 0, ANGULAR_VELOCITY * Time.deltaTime);
            yield return null;
        }
        lid.Rotate(0, 0, 90 - Utils.ClampMod(lid.rotation.eulerAngles.z, -180, 180));

        openCo = null;
    }

    private IEnumerator CloseCo()
    {
        closed = true;
        if (openCo != null)
            StopCoroutine(openCo);
        chest.HideWeapon();

        Transform lid = transform.parent;
        while (Utils.ClampMod(lid.rotation.eulerAngles.z, -180, 180) > 0)
        {
            lid.Rotate(0, 0, -ANGULAR_VELOCITY * Time.deltaTime);
            yield return null;
        }
        lid.Rotate(0, 0, -Utils.ClampMod(lid.rotation.eulerAngles.z, -180, 180));

        closeCo = null;
    }
}
