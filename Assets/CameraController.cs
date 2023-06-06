using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // For zoom in and out
    [SerializeField] float zoom = 3f;
    private Camera thisCam;

    private void Start()
    {
        thisCam = GetComponent<Camera>();
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
}
