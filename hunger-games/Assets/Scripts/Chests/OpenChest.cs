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
