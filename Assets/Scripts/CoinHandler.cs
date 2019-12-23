using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinHandler : MonoBehaviour
{
    public GameObject mouseHandlerObj;
    public GameObject metalHandlerObj;
    public GameObject coinObjSource;
    public float offsetDistance;
    public GameObject parentObj;

    private MouseHandler MOUSE;
    private MetalHandler METAL;

    // Start is called before the first frame update
    void Start()
    {
        MOUSE = mouseHandlerObj.transform.GetComponent(typeof(MouseHandler)) as MouseHandler;
        METAL = metalHandlerObj.transform.GetComponent(typeof(MetalHandler)) as MetalHandler;
    }

    void createCoin()
    {
        // make a metal object from a copy of the coin object
        // position it slightly ahead of where we are aiming
        GameObject coinObj = METAL.makeMetalObject(coinObjSource, 
            gameObject.transform.position + MOUSE.getAimDirection() * offsetDistance);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            createCoin();
        }
            // spawn coin...
    }
}
