using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public float MAX_SPEED;
    public Transform playerTransform;
    Rigidbody2D rb2D;

    // Start is called before the first frame update
    void Start()
    {
        rb2D = gameObject.GetComponent(typeof(Rigidbody2D)) as Rigidbody2D;
    }

    // Update is called once per frame
    void Update()
    {
        // add a max velocity
        if (rb2D.velocity.magnitude > MAX_SPEED)
        {
            rb2D.velocity = rb2D.velocity.normalized * MAX_SPEED;
        }
        // ignore collisions with the player
        Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), playerTransform.GetComponent<Collider2D>(), true);

    }
}
