using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class FluidGrid : MonoBehaviour
{


    [SerializeField]
    public int gridX;
    [SerializeField]
    public int gridY; 
    [SerializeField]
    public int gridZ; 
    [SerializeField]
    float cubeDimension;
    [SerializeField]
    GameObject cellPrefab;

    // Start is called before the first frame update
   public void Start()
    {
        MakeGrid();
    }

    // make 3D grid of gridWidth and gridHeight of cubes, calling MakeCube
    public void MakeGrid() 
    {
        for (int x = 0; x < gridX; x++) {
            for (int y = 0; y < gridY; y++) {
                for (int z = 0; z < gridZ; z++) {
                    MakeCell(x, y, z);
                }
            }
        }
    }

    // make cube of cubeDimension size at correct index
    public void MakeCell(int x, int y, int z) 
    { 
        GameObject cell = Instantiate(cellPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        float offset = cubeDimension / 2;
        cell.transform.position = new Vector3((float) x * offset, (float) y * offset, (float) z * offset); 
        cell.transform.localScale = new Vector3(cubeDimension, cubeDimension, cubeDimension);
        // update cell components
        cell.GetComponent<Cell>().locationIndices = new Vector3(x, y, z);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Diffuse() {
        //TODO: define dt, diff, N later
        int k;
        float a = dt * diff * N * N * N;

        GameObject[] cells = GameObject.FindGameObjectsWithTag("cell");
        for (k = 0; k < 20; k++) {
            for (int i = 0; i < cells.Length; i++) {
                Cell currCell = cells[i].GetComponent<Cell>();
                float densitySum = 0;
                GameObject[] neighbors = currCell.GetAllNeighbors();
                for (int s = 0; s < neighbors.Length; s++) {
                    densitySum += neighbors[s];
                }
                currCell.densityCurrent = (currCell.densityPast + a * densitySum) / (1 + 4 * a)
            }
            // set_bnd
        }
    }
}
