using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public const float gravity = -9.81f;
    public float jump = 1f;
    public float interactRange;
    public Transform groundCheck;
    
    public PlayerLooking looking;
    public PlayerInteraction interaction;
    public PlayerMovement movement;
    
    private void Start() {
        movement.Initialize(speed, gravity, jump, groundCheck);
        interaction.Initialize(interactRange);
        looking.Initialize(400f, transform);
    }

    private void Update() {
        looking.Look();
        interaction.InteractionHandling();
        movement.Move();
    }

    public void CanLook(bool value) {
        looking.canLook = value;
    }
    
}
