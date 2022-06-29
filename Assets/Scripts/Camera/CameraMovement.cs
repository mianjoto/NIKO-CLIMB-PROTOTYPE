using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraMovement : MonoBehaviour
{
    Vector3 newCameraPos;
    float attemptedCameraSize;
    float newCameraSize;
    GameManager gm;
    string mouseScrollWheelAxis = "Mouse ScrollWheel";
    float floorHeight;
    float cameraOffset = 5f;
    Camera mainCamera;
    [SerializeField]
    Vector3 cameraVelocity;
    GameObject player;

    [SerializeField]
    float dampTime = 0.075f, cameraZoomStep = 2f, maxZoom = 20f, minZoom = 12.5f, zoomDuration = 0.4f;


    void Awake()
    {
        gm = FindObjectOfType<GameManager>();
        newCameraPos = transform.position;
        newCameraSize = Camera.main.orthographicSize;
        attemptedCameraSize = newCameraSize;
    }

    private void Start() {
        mainCamera = Camera.main;
        if (mainCamera==null)
        {
            mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        }
        if (mainCamera==null)
        {
            mainCamera = FindObjectOfType(typeof(Camera)) as Camera;
        }
        if (mainCamera==null)
        {
            mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        }


    }

    void LateUpdate()
    {
        if (gm.playerIsDead || Camera.main == null) 
            return;
        AscendCamera();
        float scrollAmount = Input.GetAxis(mouseScrollWheelAxis);
        if (scrollAmount > 0) 
        {
            newCameraSize = Camera.main.orthographicSize - cameraZoomStep * Mathf.Abs(scrollAmount)*10;
        } 
        else if (scrollAmount < 0)
        {
            newCameraSize = Camera.main.orthographicSize + cameraZoomStep * Mathf.Abs(scrollAmount)*10;
        }

        // Ignore zoom if not in the bounds
        if (!inZoomBounds()) newCameraSize = Camera.main.orthographicSize;

        Camera.main.DOOrthoSize(newCameraSize, zoomDuration);
    }

    void AscendCamera()
    {
        cameraVelocity = Vector3.zero;
        newCameraPos = new Vector3(transform.position.x, gm.currentLevel * gm.floorHeight, transform.position.z);
        transform.position = Vector3.SmoothDamp(transform.position, newCameraPos, ref cameraVelocity, dampTime);
    }

    bool inZoomBounds()
    {
        return newCameraSize < maxZoom && newCameraSize > minZoom;
    }
}
