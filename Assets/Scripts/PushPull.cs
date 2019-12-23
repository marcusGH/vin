using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PushPull : MonoBehaviour
{
    public Camera camera;
    public GameObject metalHandlerObject;
    public GameObject mouseHandlerObject;
    public float PUSH_PULL_FORCE_FACTOR;
    public float PUSH_PULL_DISTANCE_DECAY_FACTOR;
    public float MAX_PUSH_PULL_FORCE;

    //private GameObject METAL;
    private Rigidbody2D rb2D;

    // we need metal handler methods
    private MetalHandler METAL;
    MouseHandler MOUSE;


    // Start is called before the first frame update
    void Start()
    {
        // fetch rigid body
        rb2D = GetComponent<Rigidbody2D>();

        // get the metal handler
        METAL = metalHandlerObject.transform.GetComponent(typeof(MetalHandler)) as MetalHandler;
        MOUSE = mouseHandlerObject.transform.GetComponent(typeof(MouseHandler)) as MouseHandler;

    }

    private void applyPushPullForces(float dir)
    {

        // get a list of vectors to the different metal objects
        LinkedList<GameObject> metObjs = METAL.getMetalObjects();

        // vin don't have infinite allomatic strength, so there's a limit
        // to the maximum push and pull force, so if she oversteps that,
        // the force will just be the maximum instead
        // therefore we need to go through all the forces and find
        // out if they overstep the limit or not
        Vector2 totalForce = new Vector2(0, 0);
        for (var metalObjIt = metObjs.First; metalObjIt != null; metalObjIt = metalObjIt.Next)
        {
            // see for loop below for comments
            Vector3 playerMetalVector = METAL.getPlayerMetalVector(metalObjIt.Value);
            float dist = playerMetalVector.magnitude;
            float angleNorm = MOUSE.getNormalisedCoefficient(playerMetalVector, MOUSE.getAimDirection());
            totalForce += PUSH_PULL_FORCE_FACTOR * dir * new Vector2(playerMetalVector.x, playerMetalVector.y).normalized *
                (angleNorm / (float)Math.Pow(dist, PUSH_PULL_DISTANCE_DECAY_FACTOR));
        }

        // modifier for if the total force is too big
        float forceModifier = 1.0f;

        // keep the direction, but adjust the magnitude
        if (totalForce.magnitude > MAX_PUSH_PULL_FORCE)
            forceModifier = MAX_PUSH_PULL_FORCE / totalForce.magnitude;

        for (var metalObjIt = metObjs.First; metalObjIt != null; metalObjIt = metalObjIt.Next)
        {
            // fetch the metal object
            GameObject metObj = metalObjIt.Value;

            // Vector to the metal object from the player
            Vector3 playerMetalVector = METAL.getPlayerMetalVector(metObj);

            //find the distance to the object
            float dist = playerMetalVector.magnitude;

            // get a normalised coefficient for the pull strength
            float angleNorm = MOUSE.getNormalisedCoefficient(playerMetalVector, MOUSE.getAimDirection());

            // find the forces to all of the objects and add it to the total force
            Vector2 force = PUSH_PULL_FORCE_FACTOR * dir * new Vector2(playerMetalVector.x, playerMetalVector.y).normalized *
                (angleNorm / (float)Math.Pow(dist, PUSH_PULL_DISTANCE_DECAY_FACTOR));

            /* We have that F = ma so a = F / m
             * That means that delta v / delta T = F / m
             * so we get that delta v = F * deltaT / m
             */
            rb2D.velocity += forceModifier * force * Time.deltaTime / rb2D.mass;

            //// we also get a counter force in the opposite direction, so get the rb of that object
            Rigidbody2D metalObjRb2d = metObj.transform.GetComponent(typeof(Rigidbody2D)) as Rigidbody2D;
            if (metalObjRb2d != null)
                metalObjRb2d.velocity -= forceModifier * force * Time.deltaTime / metalObjRb2d.mass;
        }

        Debug.Log("Total force atm: " + Math.Min(totalForce.magnitude, MAX_PUSH_PULL_FORCE));
    }

    // Update is called once per frame
    void Update()
    { 
        // which way should the force go? Depends on if we push or pull
        float dir = 0;
        if (Input.GetMouseButton(0))
            dir = -1;
        if (Input.GetMouseButton(1))
            dir = 1;

        applyPushPullForces(dir);
    }
}
