using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{

    // cell's location
    public Vector3 locationIndices;

    // cell's neighbors
    public GameObject right;
    public GameObject left;
    public GameObject up;
    public GameObject down;
    public GameObject front;
    public GameObject back;

    // cell's attributes
    public float testNumber;
    public Vector3 densityCurrent;
    public Vector3 densityPast;



    // Start is called before the first frame update
    void Start()
    {

        // find neighbors
        right = FindRightNeighbor();
        left = FindLeftNeighbor();
        up = FindUpNeighbor();
        down = FindDownNeighbor();
        front = FindFrontNeighbor();
        back = FindBackNeighbor();
        Color oldColor = gameObject.GetComponent<Renderer>().material.color;
        gameObject.GetComponent<Renderer>().material.color = new Color(Color.blue.r, Color.blue.g, Color.blue.b,testNumber);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Note: To get these to work, I made the prefab's collision box smaller

    GameObject FindRightNeighbor() 
    {
        RaycastHit hit;
        if (Physics.Raycast(gameObject.transform.position, new Vector3(0.5f, 0, 0), out hit) && hit.collider.gameObject.tag == "cell") {
            return hit.collider.gameObject;
        }
        return null;
    }

    GameObject FindLeftNeighbor() 
    {
        RaycastHit hit;
        if (Physics.Raycast(gameObject.transform.position, new Vector3(-0.5f, 0, 0), out hit) && hit.collider.gameObject.tag == "cell") {
            return hit.collider.gameObject;
        }
        return null;
    }

    GameObject FindDownNeighbor() 
    {
        RaycastHit hit;
        if (Physics.Raycast(gameObject.transform.position, new Vector3(0, -0.5f, 0), out hit) && hit.collider.gameObject.tag == "cell") {
            return hit.collider.gameObject;
        }
        return null;
    }

    GameObject FindUpNeighbor() 
    {
        RaycastHit hit;
        if (Physics.Raycast(gameObject.transform.position, new Vector3(0, 0.5f, 0), out hit) && hit.collider.gameObject.tag == "cell") {
            return hit.collider.gameObject;
        }
        return null;
    }


    GameObject FindFrontNeighbor() 
    {
        RaycastHit hit;
        if (Physics.Raycast(gameObject.transform.position, new Vector3(0, 0, 0.5f), out hit) && hit.collider.gameObject.tag == "cell") {
            return hit.collider.gameObject;
        }
        return null;
    }

    GameObject FindBackNeighbor() 
    {
        RaycastHit hit;
        if (Physics.Raycast(gameObject.transform.position, new Vector3(0, 0, -0.5f), out hit) && hit.collider.gameObject.tag == "cell") {
            return hit.collider.gameObject;
        }
        return null;
    }
}
