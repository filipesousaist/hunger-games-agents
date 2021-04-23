using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class openChest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseDown()
    {
        GetComponent<Rigidbody>().angularVelocity = new Vector3(0.8f, 0, 0);
        StartCoroutine(stopOpening());
    }

    IEnumerator stopOpening()
    {
        yield return new WaitForSeconds(1.1f);
        GetComponent<Rigidbody>().angularVelocity = new Vector3(0, 0, 0);   
    }
}
