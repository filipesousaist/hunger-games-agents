using UnityEngine;
using UnityEngine.UI;

public class CameraManager : MonoBehaviour
{
    public Camera floorCamera;
    public Camera flexibleCamera;
    public Camera highViewCamera;

    public Text nameText;

    private int cameraIndex = -1;
    private Camera[] cameras;

    private AgentController agentController;

    private void Awake()
    {
        agentController = FindObjectOfType<AgentController>();
        cameras = new Camera[] { floorCamera, flexibleCamera, highViewCamera };
    }

    private void Start()
    {
        SwitchToNextCamera();
        UpdateCameraInfo();
    }

    // Update is called once per frame
    private void Update() {

        if (Input.GetKeyDown(KeyCode.Tab))
            SwitchToNextCamera();
    }

    private void SwitchToNextCamera()
    {
        cameraIndex = (cameraIndex + 1) % 3;

        for (int i = 0; i < cameras.Length; i++)
            cameras[i].gameObject.SetActive(i == cameraIndex);

        UpdateCameraInfo();

        agentController.DisableAllCameras();
    }

    public void UpdateCameraInfo()
    {
        nameText.text = cameras[cameraIndex].name;
        agentController.RemoveInfo();
    }

    public void DisableAll()
    {
        floorCamera.gameObject.SetActive(false);
        flexibleCamera.gameObject.SetActive(false);
        highViewCamera.gameObject.SetActive(false);
    }
}
