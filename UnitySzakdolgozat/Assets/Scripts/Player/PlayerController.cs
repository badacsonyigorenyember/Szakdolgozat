using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public const float gravity = -9.81f;
    public float jump = 1f;
    public float mouseSensitivity = 400;
    public float interactRange;
    public Transform groundCheck;
    
    
    private void Start() {
        PlayerMovement.instance.SetProperties(speed, gravity, jump, groundCheck);
        PlayerInteraction.instance.SetProperties(interactRange);
        PlayerLooking.instance.SetProperties(mouseSensitivity, transform);
    }

    public void SetMouseSensitivity(float value) {
        Debug.Log(value);
        mouseSensitivity = value;
        PlayerLooking.instance.SetSensitivity(value);
    }
    
}
