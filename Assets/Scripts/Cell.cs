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
   public Vector3 velocity;


  public float densitySource; // accumulate added density to be added on the timestep




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
      densitySource = densitySource + source;
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




 public float LerpDensity(Vector3 v)
 {
     // the center vertex of 8 nearest surrounding cells
     float xR = RoundToMultiple(v[0]);
     float yR = RoundToMultiple(v[1]);
     float zR = RoundToMultiple(v[2]);


     float offset = cubeDimension / 2;
     Vector3 dist = gameObject.transform.position - new Vector3(xR - offset, yR - offset, zR - offset);


     GameObject u000 = gameObject;
     for (int i = 0; i < Mathf.Abs(dist[0]); i++) {
         if (dist[0] > 0) {
             u000 = u000.GetComponent<Cell>().right;
         } else {
             u000 = u000.GetComponent<Cell>().left;
         }
     }


     for (int i = 0; i < Mathf.Abs(dist[1]); i++) {
         if (dist[0] > 0) {
             u000 = u000.GetComponent<Cell>().up;
         } else {
             u000 = u000.GetComponent<Cell>().down;
         }
     }


     for (int i = 0; i < Mathf.Abs(dist[2]); i++) {
         if (dist[0] > 0) {
             u000 = u000.GetComponent<Cell>().front;
         } else {
             u000 = u000.GetComponent<Cell>().back;
         }
     }


     // Calculate neighbors based on u000's position
     Cell u100 = u000.GetComponent<Cell>().right.GetComponent<Cell>();
     Cell u010 = u000.GetComponent<Cell>().front.GetComponent<Cell>();
     Cell u110 = u010.GetComponent<Cell>().right.GetComponent<Cell>();
     Cell u001 = u000.GetComponent<Cell>().up.GetComponent<Cell>();
     Cell u101 = u100.GetComponent<Cell>().up.GetComponent<Cell>();
     Cell u011 = u010.GetComponent<Cell>().up.GetComponent<Cell>();
     Cell u111 = u110.GetComponent<Cell>().up.GetComponent<Cell>();




     // find the fractional offsets between v and the neighboring cells of c
     // s: right left, t: front back, q: up down
     float s = (u000.transform.position[0] - v[0]) / cubeDimension;
     float t = (u000.transform.position[2] - v[2]) / cubeDimension;
     float q = (u001.transform.position[1] - v[1]) / cubeDimension;




     // Perform the trilinear interpolation
     float bottomFront = Lerp1D(u000.GetComponent<Cell>().densityPast, u100.GetComponent<Cell>().densityPast, s);
     float bottomBack = Lerp1D(u010.GetComponent<Cell>().densityPast, u110.GetComponent<Cell>().densityPast, s);
     float topFront = Lerp1D(u001.GetComponent<Cell>().densityPast, u101.GetComponent<Cell>().densityPast, s);
     float topBack = Lerp1D(u011.GetComponent<Cell>().densityPast, u111.GetComponent<Cell>().densityPast, s);


     float bottom = Lerp1D(bottomFront, bottomBack, t);
     float top = Lerp1D(topFront, topBack, t);


     return Lerp1D(bottom, top, q);
 }


 // 1D Linear Interpolate density if density is true, otherwise velocity
 float Lerp1D(float d1, float d2, float t)
 {
     return d1 + (d2 - d1) * t;
 }


 float RoundToMultiple(float n)
 {
     return (float) (Mathf.Round(n / cubeDimension) * cubeDimension);
 }
}









