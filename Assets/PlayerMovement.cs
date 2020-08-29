using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    
    public float moveSpeed = 5f;
    public float sizeSpeed = 0.1f;

    public Rigidbody2D rb;

    Vector2 movement;
    Vector3 size;

    // Update is called once per frame
    void Update()
    {
        // Movement Input
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // Size Input

        //TODO: Change inputs to be decrease/increase instead of x and y
        size.x = Input.GetAxisRaw("Fire1");
        size.y = Input.GetAxisRaw("Jump");
    }

    // Since framerates are variable do movement on fixed update
    void FixedUpdate()
    {
    	// Move Character
    	rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    	transform.localScale = transform.localScale + size * sizeSpeed;
    }
}
