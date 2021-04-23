using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class openChest : MonoBehaviour
{
    bool rotating=false;
    private bool closed = true;

    private float angularVelocity = -100;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (rotating && closed)
        {
            transform.Rotate(0, angularVelocity * Time.deltaTime, 0);
        } 
        else if (rotating && !closed)
        {
            transform.Rotate(0, - angularVelocity * Time.deltaTime, 0);
        }
    }

    void OnMouseDown()
    {
        rotating = true;
        StartCoroutine(stopOpening());
    }

    IEnumerator stopOpening()
    {
        yield return new WaitForSeconds(1.1f);
        rotating = false;
        closed = !closed;
    }
}
