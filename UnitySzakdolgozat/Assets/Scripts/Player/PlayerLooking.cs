using System;
using UnityEngine;

public class PlayerLooking : MonoBehaviour
{
    public static PlayerLooking instance;
    
    private float sensitivity;

    public Transform player;

    private float xRotation;

    private bool canLook;

    private void Awake() {
        instance = this;
    }

    public void SetProperties(float s, Transform p) {
        sensitivity = s;
        player = p;
    }

    public void SetSensitivity(float value) {
        sensitivity = value;
    }

    private void Start() {
        xRotation = 0;
        canLook = true;
    }

    void Update() {
        if (canLook) {
            float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * sensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * sensitivity;
        
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90, 90);
        
            transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
            player.Rotate(Vector3.up * mouseX);
        }
    }
}
