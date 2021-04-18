using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchCamera : MonoBehaviour
{
    public Camera camera1;

    public Camera camera2;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            camera1.gameObject.SetActive(true);
            camera2.gameObject.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            camera2.gameObject.SetActive(true);
            camera1.gameObject.SetActive(false);
        }
    }
}
