using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    //https://dev.to/matthewodle/simple-3d-camera-movement-1lcj
    private float moveSpeed = 0.075f;
    private float scrollSpeed = 15.0f;

    void Start() {
        transform.LookAt(Vector3.zero);
        Camera.main.transform.position = new Vector3 (0.0f, MainMenuManager.starMass * 5, 0.0f);      
    }

    void Update () {
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0) {
            transform.position += moveSpeed * new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        }

        if (Input.GetAxis("Mouse ScrollWheel") != 0) {
            transform.position += scrollSpeed * new Vector3(0, -Input.GetAxis("Mouse ScrollWheel"), 0);
        }
    }
}
