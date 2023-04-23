using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float camMoveSpeed;
    public float camZoomSpeed;
    Vector3 offset;
    Camera cam;
    Transform pc;

    private void Start()
    {
        cam = GetComponent<Camera>();
        pc = GameObject.Find("PlayerCube").transform;
        offset = transform.position - pc.position;
    }
    void Update()
    {
        if (GameManager.gameOver) return;
        /*
        float dx = Input.GetAxis("Horizontal") * camMoveSpeed * Time.deltaTime;
        float dy = Input.GetAxis("Vertical") * camMoveSpeed * Time.deltaTime;
        float dz = Input.GetAxis("Mouse ScrollWheel") * camZoomSpeed * Time.deltaTime;
        transform.Translate(dx, dy, 0);
        cam.fieldOfView = cam.fieldOfView - dz;
        */
        transform.position = pc.position + offset;
    }
}
