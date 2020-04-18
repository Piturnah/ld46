using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Camera cam;
    Vector3 mousePosLastFrame;
    Vector3 screenCentre;

    public float rotateSpeed;
    public float scrollSensitivity;
    public float panSpeed;

    private void Start() {
        cam = GetComponentInChildren<Camera>();
        screenCentre = new Vector3(Screen.width / 2, Screen.height / 2, 0);
    }

    private void Update() {
        HandleRotation();
        HandleZoom();
        HandlePan();
    }

    void HandleRotation() {
        Vector3 pos = cam.ScreenToViewportPoint(Input.mousePosition - mousePosLastFrame);
        int mouseDir = (Vector3.Cross((screenCentre - Input.mousePosition).normalized, pos).z > 0) ? -1 : 1;

        if (Input.GetMouseButton(0)) {
            transform.Rotate(new Vector3(0, pos.magnitude * rotateSpeed * mouseDir, 0));
        }

        mousePosLastFrame = Input.mousePosition;
    }

    void HandleZoom() {
        float multiplier = 1f / cam.orthographicSize * Input.GetAxisRaw("Mouse ScrollWheel");
        cam.transform.position += (cam.ScreenToWorldPoint(Input.mousePosition) - cam.transform.position) * multiplier * scrollSensitivity;
        cam.orthographicSize -= Input.GetAxisRaw("Mouse ScrollWheel") * scrollSensitivity;
    }

    void HandlePan() {
        Vector3 moveDir = cam.transform.right * Input.GetAxisRaw("Horizontal") + transform.forward * Input.GetAxisRaw("Vertical");
        transform.position += moveDir.normalized * panSpeed * Time.deltaTime;
    }
}
