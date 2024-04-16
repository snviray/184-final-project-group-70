using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    private Vector3 gridCenter;
    private float theta = 0f; 
    private float phi = 0f;
    public float rotationSpeed = 100.0f;
    public float distanceFromGrid = 10.0f;

    void Start() {
        FindCenter();
        UpdateCameraPosition();
    }

    void Update() {
        if (Input.GetMouseButton(0) && Input.GetKey(KeyCode.Space)) {
            RotateCamera();
            UpdateCameraPosition();
        }
    }

    void FindCenter() {
        // Vector3 center = new Vector3(0, 0, 0);
        // Cell[] cells = FindObjectsOfType<Cell>;
        // foreach (Cell c in cells) 
        // {
        //     center += c.transform.position;
        //     Debug.Log("add center");
        // }
        // // Debug.Log("Center: ", center);
        // Debug.Log(center);
        // Debug.Log(cells.Length);
        // // Debug.Log("cells.Length: ", cells.Length);
        // gridCenter = center / cells.Length;
        gridCenter = new Vector3(0, 0, 0);
    }

    void RotateCamera() {
        theta -= Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
        phi -= Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;
        phi = Mathf.Clamp(phi, -89f, 89f);
    }

    void UpdateCameraPosition() {
        float thetaRad = theta * Mathf.Deg2Rad; 
        float phiRad = phi * Mathf.Deg2Rad;

        Vector3 offset = new Vector3(
            distanceFromGrid * Mathf.Cos(phiRad) * Mathf.Cos(thetaRad),
            distanceFromGrid * Mathf.Sin(phiRad),
            distanceFromGrid * Mathf.Cos(phiRad) * Mathf.Sin(thetaRad) 
        );

        transform.position = gridCenter + offset;
        transform.LookAt(gridCenter);
    }
}
