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
    [SerializeField] Vector3 ResetCamera; // For if I ever want to add cam reset in the future
    private bool drag = false;
    // For camera shaking effect
    [SerializeField] Vector3 cameraInitialPosition;
    [SerializeField] float shakeMagnitude = 0.05f, shakeTime = 0.5f;
    // For zooming into character
    [SerializeField] float zoomDuration = 0.3f;

    private void Start()
    {
        thisCam = GetComponent<Camera>();
        ResetCamera = Camera.main.transform.position;
        cameraInitialPosition = Camera.main.transform.position;
    }

    // Update method for zoom
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

    // Update method for dragging camera
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

    // Methods for camera shake upon damage
    public void CameraShake()
    {
        cameraInitialPosition = thisCam.transform.position;
        InvokeRepeating("StartCameraShaking", 0f, 0.005f);
        Invoke("StopCameraShaking", shakeTime);
    }
    void StartCameraShaking()
    {
        // Shaking offsets
        float cameraShakingOffsetX = Random.value * shakeMagnitude * 2 - shakeMagnitude;
        float cameraShakingOffsetY = Random.value * shakeMagnitude * 2 - shakeMagnitude;

        // Shaky shaky
        Vector3 cameraIntermediatePosition = thisCam.transform.position;
        cameraIntermediatePosition.x += cameraShakingOffsetX;
        cameraIntermediatePosition.y += cameraShakingOffsetY;
        thisCam.transform.position = cameraIntermediatePosition;
    }
    void StopCameraShaking()
    {
        CancelInvoke("StartCameraShaking");
        thisCam.transform.position = cameraInitialPosition;
    }

    // Methods for zooming into a character upon clicking it
    public IEnumerator ZoomAtCharacter(Vector3 characterPosition)
    {
        float elapsedTime = 0f;
        Vector3 initCamPos = thisCam.transform.position;

        while (elapsedTime < zoomDuration)
        {
            float t = elapsedTime / zoomDuration;

            float newX = Mathf.Lerp(initCamPos.x, characterPosition.x, t);
            float newY = Mathf.Lerp(initCamPos.y, characterPosition.y, t);
            thisCam.transform.position = new Vector3(newX, newY, thisCam.transform.position.z);
            //if (thisCam.orthographicSize >= 4f)
            //{
            //    thisCam.orthographicSize -= 0.008f;
            //}

            elapsedTime += Time.deltaTime;
            //yield return new WaitUntil(() => thisCam.transform.position.x == characterPosition.x && thisCam.transform.position.y == characterPosition.y);
            yield return null;
        }

        //Debug.Log("Zoom done");
        // Change camera position
        //Vector3 positionToGo = new Vector3(characterPosition.x, characterPosition.y, thisCam.transform.position.z);
        //thisCam.transform.position = positionToGo;
    }
}
