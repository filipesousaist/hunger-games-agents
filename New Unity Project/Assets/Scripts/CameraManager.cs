using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public UnityEngine.Camera floorCamera;
    public UnityEngine.Camera flexibleCamera;
    public UnityEngine.Camera highViewCamera;
    // Start is called before the first frame update
    void Start() {
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            floorCamera.gameObject.SetActive(true);
            flexibleCamera.gameObject.SetActive(false);
            highViewCamera.gameObject.SetActive(false);
        } else if (Input.GetKeyDown(KeyCode.Alpha2)) {
            flexibleCamera.gameObject.SetActive(true);
            floorCamera.gameObject.SetActive(false);
            highViewCamera.gameObject.SetActive(false);
        } else if (Input.GetKeyDown(KeyCode.Alpha3)) {
            highViewCamera.gameObject.SetActive(true);
            floorCamera.gameObject.SetActive(false);
            flexibleCamera.gameObject.SetActive(false);
        }
    }
}
