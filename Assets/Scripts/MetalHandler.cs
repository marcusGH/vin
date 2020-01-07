using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MetalHandler : MonoBehaviour
{
    // public field paramters

    // general needed paramters
    public GameObject playerObj;
    public GameObject mouseHandlerObj;

    //fields for the metal lines
    public GameObject lineSourceObj;        // we just need an empty object
    public float lineWidth;
    public Material lineMaterial;
    public Color defaultLineColour;
    public Color aimedLineColour;
    public float lineZValue;
    public float lineOpacityDistanceRange;

    // private fields
    private MouseHandler MOUSE;

    // list of all our metal objects
    private  LinkedList<GameObject> metalObjects = new LinkedList<GameObject>();

    // setup methods ----------------------------------------------------------------------------------------

    private void setUpInitialMetalObjects(GameObject cur)
    {
        // go through all our metal object children
        for (int i = 0; i < cur.transform.childCount; i++)
        {
            // fetch a metal object
            GameObject metalObj = cur.transform.GetChild(i).gameObject;
            // se if it's a group, in which case we recursively set up its children
            if (metalObj.transform.childCount > 0)
                setUpInitialMetalObjects(metalObj);
            // otherwise (base case) we set it up
            else
            {
                // add a line to it
                attachLineObject(metalObj);
                // the static object do not have any rigid bodies as they cannot be moved
                //attachRigidbody2D(metalObj);
                // then add it to our list
                metalObjects.AddLast(metalObj);
            }
        }
        //// also go through all the objects labeled with the tag "Metal"
        //GameObject[] metObjs = GameObject.FindGameObjectsWithTag("Metal");
        //foreach (GameObject metObj in metObjs)
        //{
        //    attachLineObject(metObj);
        //    metalObjects.AddLast(metObj);
        //}
    }

    private void attachLineObject(GameObject destinationObj)
    {
        // create an empty object
        GameObject lineObj = Instantiate(lineSourceObj, new Vector3(0, 0, 0), Quaternion.identity);
        lineObj.name = "Line Object";
        // attach a line renderer to it
        LineRenderer lineRenderer = lineObj.AddComponent(typeof(LineRenderer)) as LineRenderer;
        lineRenderer = lineObj.GetComponent<LineRenderer>();
        // fill out parameters
        lineRenderer.SetWidth(lineWidth, lineWidth);
        lineRenderer.material = lineMaterial;
        lineRenderer.SetColors(defaultLineColour, defaultLineColour);
        lineRenderer.SetVertexCount(2);
        // attach it as a child to the metal object in destination
        lineObj.transform.parent = destinationObj.transform;
    }

    private void attachRigidbody2D(GameObject destinationObj, float mass = float.MaxValue, float gravityScale = 0)
    {
        // make the component and attach it
        Rigidbody2D rb2D = destinationObj.AddComponent(typeof(Rigidbody2D)) as Rigidbody2D;

        // set the paramters
        rb2D.mass = mass;
        rb2D.gravityScale = gravityScale;
    }

    // public getter methods ---------------------------------------

    public LinkedList<GameObject> getMetalObjects()
    {
        return metalObjects;
    }

    public Vector3 getPlayerMetalVector(GameObject metObj)
    {
        return metObj.transform.position - playerObj.transform.position;
    }

    // public interface methods ------------------------------------

    public GameObject makeMetalObject(GameObject metalSourceObj, Vector3 pos)
    {
        // make a copy of the source object
        GameObject metalObj = Instantiate(metalSourceObj, pos, Quaternion.identity);
        // make it a metal object by attaching it to this (group it)
        metalObj.transform.parent = gameObject.transform;
        // now add a line to it
        attachLineObject(metalObj);
        // and finally keep track of it
        metalObjects.AddLast(metalObj);
        // return it som that one can make further modificaitons
        return metalObj;
    }

    // Start is called before the first frame update
    void Start()
    {
        setUpInitialMetalObjects(gameObject);
        // fetch mouse handler
        MOUSE = mouseHandlerObj.transform.GetComponent(typeof(MouseHandler)) as MouseHandler;
    }

    // update interface  -----------------------------------------------------------------------------------

    private void updateLinePositions()
    {
        for (var metObjIt = metalObjects.First; metObjIt != null; metObjIt = metObjIt.Next)
        {
            // fetch the value
            GameObject metObj = metObjIt.Value;

            // get the line renderer (it should be the one and only child)
            GameObject lineObj = metObj.transform.GetChild(0).gameObject;                                                              // TODO: make explicit name
            LineRenderer lr = lineObj.GetComponent<LineRenderer>();
            // position of player object
            lr.SetPosition(0, new Vector3(playerObj.transform.position.x, playerObj.transform.position.y, lineZValue));
            // make the metal's position be the other anchor
            lr.SetPosition(1, new Vector3(metObj.transform.position.x, metObj.transform.position.y, lineZValue));
            // update the colour depending on where we aim
            float factor = MOUSE.getNormalisedCoefficient(getPlayerMetalVector(metObj), MOUSE.getAimDirection());
            Color col = defaultLineColour + factor * (aimedLineColour - defaultLineColour);
            // set the alpha channel depending on distance
            col.a = 1 - Math.Min(getPlayerMetalVector(metObj).magnitude, lineOpacityDistanceRange) / lineOpacityDistanceRange;
            // now update the colours of the line
            lr.SetColors(col, col);
        }
    }

    // Update is called once per frame
    void Update()
    {
        updateLinePositions();
    }
}
