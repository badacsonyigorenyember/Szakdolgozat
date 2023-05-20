using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private float speed;
    private float gravity;
    private float jump;
    
    private CharacterController controller;
    
    private Transform groundCheck;
    private LayerMask groundMask;

    Vector3 velocity;
    bool isGrounded;

    public void Initialize(float s, float g, float j, Transform gc) {
        speed = s;
        gravity = g;
        jump = j;
        groundMask = LayerMask.GetMask("Ground");
        groundCheck = transform.Find("GroundCheck");
        controller = GetComponent<CharacterController>();
    }

    public void Move() {
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.4f, groundMask);

        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Transform trans = transform;
        Vector3 move = trans.right * x + trans.forward * z;

        controller.Move(move * speed * Time.deltaTime);

        if(Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jump * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
        
    }
}