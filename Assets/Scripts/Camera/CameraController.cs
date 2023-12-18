using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Camera mainCamera;

    // Camera settings.
    [Header("Movement")]
    public float cameraDragSpeed = 2.4f;
    public float cameraKeySpeed = 30;

    [Header("Zoom")]
    public float cameraMinZoom = 10f;
    public float cameraMaxZoom = 60f;
    public float cameraZoomSpeed = 20f;

    // Local state.
    private Vector3 _cursorPosition;
    private float _zoom = 30f;

    // Get camera speed based on zoom.
    float FixCameraSpeed(float initial)
    {
        return initial * (this._zoom / 45);
    }

    // Handle camera moving
    void HandleCameraMovement()
    {
        float speed = FixCameraSpeed(this.cameraKeySpeed);
        float fixedSpeed = speed * Time.fixedDeltaTime;

        // With keys.
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (horizontal != 0 || vertical != 0)
        {
            Vector3 delta = new(horizontal * fixedSpeed, 0, vertical * fixedSpeed);
            this.transform.Translate(delta);
        }

        // With mouse dragging.
        float dragSpeed = FixCameraSpeed(this.cameraDragSpeed);
        float fixedDragSpeed = dragSpeed * Time.fixedDeltaTime;

        if (Input.GetMouseButtonDown(1))
        {
            this._cursorPosition = Input.mousePosition;
        }
        else if (Input.GetMouseButton(1))
        {
            Vector3 delta = Input.mousePosition - this._cursorPosition;
            this.transform.Translate(-delta.x * fixedDragSpeed, 0, -delta.y * fixedDragSpeed);
            this._cursorPosition = Input.mousePosition;
        }
    }

    // Handle camera zooming with mouse scroll.
    void HandleCameraZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        float camKey = Input.GetKeyDown(KeyCode.E) ? 0.5f : Input.GetKeyDown(KeyCode.Q) ? -0.5f : 0;

        // Zoom in and out.
        if (scroll != 0.0f)
        {
            this._zoom = Mathf.Clamp(this._zoom - scroll * cameraZoomSpeed, cameraMinZoom, cameraMaxZoom);
        }

        // Zoom in and out with keys.
        if (camKey != 0)
        {
            this._zoom = Mathf.Clamp(this._zoom + camKey * cameraZoomSpeed, cameraMinZoom, cameraMaxZoom);
        }

        // Update camera zoom.
        this.mainCamera.fieldOfView = Mathf.Lerp(this.mainCamera.fieldOfView, this._zoom, Time.deltaTime * 5);
    }

    // Update is called once per frame
    void Update()
    {
        HandleCameraMovement();
        HandleCameraZoom();
    }

    // Get as string
    public override string ToString()
    {
        Vector3 pos = this.transform.position;
        return $"Zoom={this._zoom}, x={pos.x.ToString("0.0")}, z={pos.z.ToString("0.0")}";
    }
}
