using UnityEngine;

public class PlayerLooking : MonoBehaviour
{
    private float sensitivity;
    public Transform player;
    private float xRotation;
    public bool canLook;

    public void Initialize(float s, Transform p) {
        sensitivity = s;
        player = p;
        xRotation = 0;
        canLook = true;
    }

    public void SetSensitivity(float value) {
        sensitivity = value;
    }

    public void Look() {
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
