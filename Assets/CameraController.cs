using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // For zoom in and out
    [SerializeField] float zoom = 3f;
    private Camera thisCam;
    // For dragging the camera around
    [SerializeField] Vector3 origin;
    [SerializeField] Vector3 difference;
    [SerializeField] Vector3 ResetCamera;
    private bool drag = false;

    private void Start()
    {
        thisCam = GetComponent<Camera>();
        ResetCamera = Camera.main.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // Zooming in and out
        if (thisCam.orthographic)
        {
            thisCam.orthographicSize -= Input.GetAxis("Mouse ScrollWheel") * zoom;
        }
        else
        {
            thisCam.fieldOfView -= Input.GetAxis("Mouse ScrollWheel") * zoom;
        }
        

        // Clamping the zoom in and out distance:
        if (thisCam.fieldOfView <= 1f)
        {
            thisCam.fieldOfView = 1f;
        }
        else if (thisCam.orthographicSize <= 1f)
        {
            thisCam.orthographicSize = 1f;
        }
        if (thisCam.orthographicSize >= 9f)
        {
            thisCam.orthographicSize = 9f;
        }

        //Debug.Log(thisCam.orthographicSize);
    }
    private void LateUpdate()
    {
        if (Input.GetMouseButton(1))
        {
            difference = (Camera.main.ScreenToWorldPoint(Input.mousePosition)) - Camera.main.transform.position;
            if (drag == false)
            {
                drag = true;
                origin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
        }
        else
        {
            drag = false;
        }

        if (drag)
        {
            Camera.main.transform.position = origin - difference;
            // Add constraints
            Camera.main.transform.position = new Vector3(Mathf.Clamp(Camera.main.transform.position.x, -10, 10), Mathf.Clamp(Camera.main.transform.position.y, -6, 6), transform.position.z);
        }
    }
}
