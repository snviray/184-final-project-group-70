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
    public float densityCurrent;
    public float densityPast;

    public float densityAdded; // accumulate added density to be added on the timestep

    public float currSource; // fill if the user clicks on this, clear after processing

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
        gameObject.GetComponent<Renderer>().enabled = false; // set this when the density is non zero
        densityCurrent = 0f;
        densityPast = 0f;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnMouseDown() 
    { 
        if (Input.GetMouseButtonDown(0))
        { 
            AddSourceToCell(0.9f); // todo, change this later to maybe be user input in a UI box
        }
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
        if (Physics.Raycast(gameObject.transform.position, new Vector3(0, 0, -0.5f), out hit) && hit.collider.gameObject.tag == "cell") {
            return hit.collider.gameObject;
        }
        return null;
    }

    GameObject FindBackNeighbor() 
    {
        RaycastHit hit;
        if (Physics.Raycast(gameObject.transform.position, new Vector3(0, 0, 0.5f), out hit) && hit.collider.gameObject.tag == "cell") {
            return hit.collider.gameObject;
        }
        return null;
    }
    
    public List<GameObject> GetAllNeighbors()
    { 
        List<GameObject> neighbors = new List<GameObject>();
        if (right != null) {
            neighbors.Add(right);
        }
        if (left != null) {
            neighbors.Add(left);
        }
        if (front != null) {
            neighbors.Add(front);
        }
        if (back != null) {
            neighbors.Add(back);
        }
        if (up != null) {
            neighbors.Add(up);
        }
        if (down != null) {
            neighbors.Add(down);
        }

        return neighbors;
    }
    
    // accumulate source to a cell to be added on the next timestep
    public void AddSourceToCell(float source) 
    {
        Debug.Log("added density to ");
        Debug.Log(locationIndices);
        densityAdded = densityAdded + source; 
    }


    // clear added density
    public void ClearDensityAdded() 
    {
        densityAdded = 0f;
    }

    public void RenderDensity() // might want to change rendering later
    {
        if (densityCurrent > 0.01f) 
        {
            gameObject.GetComponent<Renderer>().enabled = true; 
            gameObject.GetComponent<Renderer>().material.color = new Color(Color.blue.r, Color.blue.g, Color.blue.b, densityCurrent);
        } else {
            gameObject.GetComponent<Renderer>().enabled = false;
        }
         
    }

}
