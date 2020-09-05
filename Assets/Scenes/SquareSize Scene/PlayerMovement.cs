﻿using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    
    public float moveSpeed = 5f;
    public float sizeSpeed = 0.1f;
    public float minSize = 0.5f;
    public float maxSize = 10f;

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
        size.x = Input.GetAxisRaw("Jump")*1 + Input.GetAxisRaw("Fire1")*-1;
        size.y = Input.GetAxisRaw("Jump")*1 + Input.GetAxisRaw("Fire1")*-1;
    }

    // Since framerates are variable do movement on fixed update
    void FixedUpdate()
    {   
        size.x = Mathf.Clamp(transform.localScale.x + size.x * sizeSpeed, minSize, maxSize);
        size.y = Mathf.Clamp(transform.localScale.x + size.x * sizeSpeed, minSize, maxSize);


        // Move Character
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        transform.localScale = size;
    }
}