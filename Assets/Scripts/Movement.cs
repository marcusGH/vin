using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Movement : MonoBehaviour
{

    public float speed;
    public float jumpSpeed;
    public float friction; //make better model! with energy etc.
   
    // we need the animator component
    private Animator anim;
    private Rigidbody2D rb2D;
    private SpriteRenderer sr;

    // Start is called before the first frame update
    void Start()
    {
        anim = gameObject.GetComponent(typeof(Animator)) as Animator;
        rb2D = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>(); 
    }


    private int groundLayer = 1 << 8;

    private bool IsGrounded()
    {
        Vector2 position = gameObject.transform.position;
        Vector2 direction = Vector2.down;
        float distance = 1.15f;

        RaycastHit2D hit = Physics2D.Raycast(position, direction, distance, groundLayer);
        if (hit.collider != null)
        {
            return true;
        }

        return false;
    }

    // Update is called once per frame
    private void Update()
    {
        // we have som min range to avoid flickering from minimal movement

        // flip back from pointing left if we start going to the right
        if (sr.flipX && rb2D.velocity.x > 0.2)
            sr.flipX = false;
        // flip if we start going left
        if (!sr.flipX && rb2D.velocity.x < -0.02)
            sr.flipX = true;
    }
    void FixedUpdate()
    {

        if (Input.GetKey(KeyCode.A))
            rb2D.velocity = new Vector2(-2.0f, rb2D.velocity.y);
        else if (Input.GetKey(KeyCode.D))
            rb2D.velocity = new Vector2(2.0f, rb2D.velocity.y);
        // apply friction
        else
            rb2D.velocity = new Vector2(rb2D.velocity.x * 0.7f, rb2D.velocity.y);
            

        if (Input.GetKey(KeyCode.Space) && IsGrounded())
            rb2D.velocity = new Vector2(rb2D.velocity.x, jumpSpeed); // AddForce(transform.up * jumpThrust, ForceMode2D.Impulse);

        // update animator components
        anim.SetBool("isGrounded", IsGrounded());
        anim.SetFloat("xVelocity", Math.Abs(rb2D.velocity.x));
        anim.SetFloat("yVelocity", rb2D.velocity.y);

        Debug.Log(IsGrounded());
        Debug.Log(Math.Abs(rb2D.velocity.x));
        Debug.Log(rb2D.velocity.y);
    }
    
}
