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

    public float cubeDimension;
    public Vector3 velocityCurrent;
    public Vector3 velocityPast;
    public Vector3 velocitySource;
    public float densitySource; // accumulate added density to be added on the timestep

    // project variables
    public float div;
    public float p;

    public Color cellColor; 

    public float waveHeight = 0.5f;
    public float waveFrequency = 0.5f;
    public float waveSpeed = 0.01f;
    private float wavePhase;

    private void Awake()
    {
        wavePhase = Random.Range(0f, Mathf.PI * 2f);
    } 

    void OnMouseOver()
    {
        Debug.Log("Hover");
        AddSourceToCell(10f); // todo, change this later to maybe be user input in a UI box
        AddVelocitySourceToCell(new Vector3(10f, 0f, 0f));
    }

    // Start is called before the first frame update
    void Start()
    {
       cellColor = gameObject.GetComponent<Renderer>().material.color;
        // find neighbors
        right = FindRightNeighbor();
        left = FindLeftNeighbor();
        up = FindUpNeighbor();
        down = FindDownNeighbor();
        front = FindFrontNeighbor();
        back = FindBackNeighbor();
        // gameObject.GetComponent<Renderer>().enabled = false; // set this when the density is non zero
        densityCurrent = 0f;
        velocityCurrent = new Vector3(0f, 0f, 0f);
        densityPast = 0f;
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
       densitySource = densitySource + source;
    //    cellColor = Random.ColorHSV();
   }

   public void AddVelocitySourceToCell(Vector3 source)
   {
        velocitySource = velocitySource + source;
   }

   public void RenderDensity() // might want to change rendering later
   {
        if (densityCurrent > 0.01f) {
            gameObject.GetComponent<Renderer>().enabled = true;
            densityCurrent = Mathf.Max(0.012f, densityCurrent);
            Color c = cellColor;
            if (densityCurrent < 0.1f) {
                c = Color.white;
            }
            if (right) {
                c = Color.Lerp(cellColor, right.GetComponent<Cell>().cellColor, densityCurrent);
            }
            if (left) {
                c = Color.Lerp(c, left.GetComponent<Cell>().cellColor, densityCurrent);
            }
            if (up) {
                c = Color.Lerp(c, up.GetComponent<Cell>().cellColor, densityCurrent);
            }
            if (down) {
                c = Color.Lerp(c, down.GetComponent<Cell>().cellColor, densityCurrent);
            }
            if (front) {
                c = Color.Lerp(c, front.GetComponent<Cell>().cellColor, densityCurrent);
            }
            if (back) {
                c = Color.Lerp(c, back.GetComponent<Cell>().cellColor, densityCurrent);
            }

            gameObject.GetComponent<Renderer>().material.color = new Color(c.r, c.g, c.b, Mathf.Min(Mathf.Min(densityCurrent, 0.5f), 1.0f));

        } else {
            gameObject.GetComponent<Renderer>().enabled = false; // set this when the density is non zero
        }



      
   }

   public float LerpDensity(Vector3 v)
   {
       // the center vertex of 8 nearest surrounding cells
       float xR = RoundToMultiple(v[0]);
       float yR = RoundToMultiple(v[1]);
       float zR = RoundToMultiple(v[2]);


       float offset = cubeDimension / 2;
       Vector3 dist = gameObject.transform.position - new Vector3(xR - offset, yR - offset, zR - offset);


       Cell u000 = gameObject.GetComponent<Cell>();
       for (int i = 0; i < Mathf.Abs(dist[0]); i++) {
           if (dist[0] > 0 && u000.right != null) {
               u000 = u000.right.GetComponent<Cell>();
           }
           if (dist[0] < 0 && u000.left != null) {
               u000 = u000.left.GetComponent<Cell>();
           }
       }


       for (int i = 0; i < Mathf.Abs(dist[1]); i++) {
           if (dist[1] > 0 && u000.up != null) {
               u000 = u000.up.GetComponent<Cell>();
           }            
           if (dist[1] < 0 && u000.down != null) {
               u000 = u000.down.GetComponent<Cell>();
           }
       }


       for (int i = 0; i < Mathf.Abs(dist[2]); i++) {
           if (dist[2] > 0 && u000.front != null) {
               u000 = u000.front.GetComponent<Cell>();
           }
           if (dist[2] < 0 && u000.back != null) {
               u000 = u000.back.GetComponent<Cell>();
           }
       }

       // Calculate neighbors based on u000's position
       Cell u100 = (u000.right != null) ? u000.right.GetComponent<Cell>() : u000;
       Cell u010 = (u000.front != null) ? u000.front.GetComponent<Cell>() : u000;
       Cell u110 = (u010.right != null) ? u010.right.GetComponent<Cell>() : u010;
       Cell u001 = (u000.up != null) ? u000.up.GetComponent<Cell>() : u000;
       Cell u101 = (u100.up != null) ? u100.up.GetComponent<Cell>() : u100;
       Cell u011 = (u010.up != null) ? u010.up.GetComponent<Cell>() : u010;
       Cell u111 = (u110.up != null) ? u110.up.GetComponent<Cell>() : u110;

       // find the fractional offsets between v and the neighboring cells of c
       // s: right left, t: front back, q: up down
       float s = (u000.transform.position[0] - v[0]) / cubeDimension;
       float t = (u000.transform.position[2] - v[2]) / cubeDimension;
       float q = (u001.transform.position[1] - v[1]) / cubeDimension;

       // Perform the trilinear interpolation
       float bottomFront = Lerp1D(u000.densityPast, u100.densityPast, s);
       float bottomBack = Lerp1D(u010.densityPast, u110.densityPast, s);
       float topFront = Lerp1D(u001.densityPast, u101.densityPast, s);
       float topBack = Lerp1D(u011.densityPast, u111.densityPast, s);

       float bottom = Lerp1D(bottomFront, bottomBack, t);
       float top = Lerp1D(topFront, topBack, t);

       return Lerp1D(bottom, top, q);
   }


   public Vector3 LerpVelocity(Vector3 v)
   {
       // the center vertex of 8 nearest surrounding cells
       float xR = RoundToMultiple(v[0]);
       float yR = RoundToMultiple(v[1]);
       float zR = RoundToMultiple(v[2]);

       float offset = cubeDimension / 2;
       Vector3 dist = gameObject.transform.position - new Vector3(xR - offset, yR - offset, zR - offset);

       Cell u000 = gameObject.GetComponent<Cell>();
       for (int i = 0; i < Mathf.Abs(dist[0]); i++) {
           if (dist[0] > 0 && u000.right != null) {
               u000 = u000.right.GetComponent<Cell>();
           }
           if (dist[0] < 0 && u000.left != null) {
               u000 = u000.left.GetComponent<Cell>();
           }
       }

       for (int i = 0; i < Mathf.Abs(dist[1]); i++) {
           if (dist[1] > 0 && u000.up != null) {
               u000 = u000.up.GetComponent<Cell>();
           }            
           if (dist[1] < 0 && u000.down != null) {
               u000 = u000.down.GetComponent<Cell>();
           }
       }

       for (int i = 0; i < Mathf.Abs(dist[2]); i++) {
           if (dist[2] > 0 && u000.front != null) {
               u000 = u000.front.GetComponent<Cell>();
           }
           if (dist[2] < 0 && u000.back != null) {
               u000 = u000.back.GetComponent<Cell>();
           }
       }

       // Calculate neighbors based on u000's position
       Cell u100 = (u000.right != null) ? u000.right.GetComponent<Cell>() : u000;
       Cell u010 = (u000.front != null) ? u000.front.GetComponent<Cell>() : u000;
       Cell u110 = (u010.right != null) ? u010.right.GetComponent<Cell>() : u010;
       Cell u001 = (u000.up != null) ? u000.up.GetComponent<Cell>() : u000;
       Cell u101 = (u100.up != null) ? u100.up.GetComponent<Cell>() : u100;
       Cell u011 = (u010.up != null) ? u010.up.GetComponent<Cell>() : u010;
       Cell u111 = (u110.up != null) ? u110.up.GetComponent<Cell>() : u110;

       // find the fractional offsets between v and the neighboring cells of c
       // s: right left, t: front back, q: up down
       float s = (u000.transform.position[0] - v[0]) / cubeDimension;
       float t = (u000.transform.position[2] - v[2]) / cubeDimension;
       float q = (u001.transform.position[1] - v[1]) / cubeDimension;

       // Perform the trilinear interpolation
       Vector3 bottomFront = Lerp1D(u000.velocityPast, u100.velocityPast, s);
       Vector3 bottomBack = Lerp1D(u010.velocityPast, u110.velocityPast, s);
       Vector3 topFront = Lerp1D(u001.velocityPast, u101.velocityPast, s);
       Vector3 topBack = Lerp1D(u011.velocityPast, u111.velocityPast, s);

       Vector3 bottom = Lerp1D(bottomFront, bottomBack, t);
       Vector3 top = Lerp1D(topFront, topBack, t);

       return Lerp1D(bottom, top, q);
   }


   Vector3 Lerp1D(Vector3 v1, Vector3 v2, float t)
   {
       return v1 + (v2 - v1) * t;
   }

   float Lerp1D(float d1, float d2, float t)
   {
       return d1 + (d2 - d1) * t;
   }

   float RoundToMultiple(float n)
   {
       return (float) (Mathf.Round(n / cubeDimension) * cubeDimension);
   }
}