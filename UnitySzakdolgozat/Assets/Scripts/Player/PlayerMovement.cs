using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement instance; 
    
    private float speed;
    private float gravity;
    private float jump;
    
    private CharacterController controller;
    
    private Transform groundCheck;
    private LayerMask groundMask;

    Vector3 velocity;
    bool isGrounded;

    private void Awake() {
        instance = this;
    }

    public void SetProperties(float s, float g, float j, Transform gc) {
        speed = s;
        gravity = g;
        jump = j;
        groundMask = LayerMask.GetMask("Ground");
        groundCheck = transform.Find("GroundCheck");
        controller = GetComponent<CharacterController>();
    }

    public void Update() {
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.4f, groundMask);

        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * speed * Time.deltaTime);

        if(Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jump * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
        
    }
}