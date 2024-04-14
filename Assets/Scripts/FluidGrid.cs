using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FluidGrid : MonoBehaviour
{
    public float cubeDimension;
    [SerializeField]
    GameObject cellPrefab;
    [SerializeField]
    AddDensityButton button;
    [SerializeField]
    AddDensityButton button2;
    public float dt = 0.1f; // time step
    public float diff  = 10f; // diffusion constant

    public float visc = 10f; // viscosity constant
    [SerializeField]
    public int N;
    public Vector3 velocitySource;
    private int gridX;
    private int gridY;
    private int gridZ;
    public float timer = 0f;

    // boundary cells, excluding corners
    public List<GameObject> bottomCells = new List<GameObject>();
    public List<GameObject> topCells = new List<GameObject>();
    public List<GameObject> rightCells = new List<GameObject>();
    public List<GameObject> leftCells = new List<GameObject>();
    public List<GameObject> frontCells = new List<GameObject>();
    public List<GameObject> backCells = new List<GameObject>();
    public Dictionary<Vector3, GameObject> cornerCells = new Dictionary<Vector3, GameObject>();
    public List<Vector3> cornerCellsIndices = new List<Vector3>();
    public List<GameObject> nonBoundaryCells = new List<GameObject>();

    public bool added = false;

    [SerializeField]
    public Vector3 addDensityCellCoordinates;

    // Start is called before the first frame update
    public void Start()
    {
        // set constants
        gridX = N + 2;
        gridY = N + 2;
        gridZ = N + 2;
        cubeDimension = (float) 1 / N;
        cornerCellsIndices.Add(new Vector3(0, 0, 0));
        cornerCellsIndices.Add(new Vector3(0, 0, N + 1));
        cornerCellsIndices.Add(new Vector3(N + 1, 0, 0));
        cornerCellsIndices.Add(new Vector3(N + 1, N + 1, N + 1));
        cornerCellsIndices.Add(new Vector3(0, N + 1, 0));
        cornerCellsIndices.Add(new Vector3(0, N + 1, N + 1));
        cornerCellsIndices.Add(new Vector3(N + 1, N + 1, 0));
        cornerCellsIndices.Add(new Vector3(N + 1, 0,  N + 1));
        MakeGrid();
    }
    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= dt) {
            // every timestep generate a source in a random Source
            // GameObject chosen = nonBoundaryCells[(int) Random.Range(0, nonBoundaryCells.Count - 1)];
            // chosen.GetComponent<Cell>().AddSourceToCell(Random.Range(0f, 0.99f));
            
            // add density and velocity to random cell
            if (Random.Range(0, 100) < 50) {
                int ind = Random.Range(0, nonBoundaryCells.Count);
                int ind2 = Random.Range(0, nonBoundaryCells.Count);
                nonBoundaryCells[ind].GetComponent<Cell>().AddVelocitySourceToCell(new Vector3(-5f, 0f,0f));
                if (Random.Range(0, 100) < 50 || !added) {
                    nonBoundaryCells[ind].GetComponent<Cell>().AddSourceToCell(4f);
                    added = true;
                }
            }

            VelocityStep();
            DensityStep();
            RenderCells();
            timer = timer - dt;
        }
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
        Vector3 locIndices = new Vector3(x, y, z);
        Cell c = cell.GetComponent<Cell>();
        c.locationIndices = locIndices;

        c.cubeDimension = cubeDimension;
        if (cornerCellsIndices.Contains(locIndices)) {
            cornerCells.Add(locIndices, cell);
            return;
        }
        // add to array of boundary cells if necessary
        if (x == 0) {
            leftCells.Add(cell);
        } else if (x == N + 1) {
            rightCells.Add(cell);
        } else if (y == 0) {
            bottomCells.Add(cell);
        } else if (y == N + 1) {
            topCells.Add(cell);
        } else if (z == 0) {
            frontCells.Add(cell);
        } else if (z == N + 1) {
            backCells.Add(cell);
        } else {
            nonBoundaryCells.Add(cell);
        }
    }
    void AdvectDensity()
    {
        for (int i = 0; i < nonBoundaryCells.Count; i++) {
        Cell currCell = nonBoundaryCells[i].GetComponent<Cell>();
        Vector3 position = currCell.transform.position - dt * currCell.velocityCurrent;
        float x = position[0];
        float y = position[1];
        float z = position[2];

        float offset = cubeDimension / 2;
        if (x < offset) {
            x = offset;
        }
        if (x > cubeDimension * N + offset) {
            x = cubeDimension * N + offset;
        }
        if (y < offset) {
            y = offset;
        }
        if (y > cubeDimension * N + offset) {
            y = cubeDimension * N + offset;
        }
        if (z < offset) {
            z = offset;
        }
        if (z > cubeDimension * N + offset) {
            z = cubeDimension * N + offset;
        }
        currCell.densityCurrent = currCell.LerpDensity(new Vector3(x, y, z));
        }
    }


    void AdvectVelocity()
    {
        for (int i = 0; i < nonBoundaryCells.Count; i++) {
        Cell currCell = nonBoundaryCells[i].GetComponent<Cell>();
        Vector3 position = currCell.transform.position - dt * currCell.velocityCurrent;
        float x = position[0];
        float y = position[1];
        float z = position[2];


        float offset = cubeDimension / 2;
        if (x < offset) {
            x = offset;
        }
        if (x > cubeDimension * N + offset) {
            x = cubeDimension * N + offset;
        }
        if (y < offset) {
            y = offset;
        }
        if (y > cubeDimension * N + offset) {
            y = cubeDimension * N + offset;
        }
        if (z < offset) {
            z = offset;
        }
        if (z > cubeDimension * N + offset) {
        z = cubeDimension * N + offset;
        }
            currCell.velocityCurrent = currCell.LerpVelocity(new Vector3(x, y, z));
        }
    }
    void Diffuse()
        {
            int k;
            float a = dt * diff * N * N * N;
            for (k = 0; k < 20; k++) {
                for (int i = 0; i < nonBoundaryCells.Count; i++) {
                    Cell currCell = nonBoundaryCells[i].GetComponent<Cell>();
                    float densitySum = 0;
                    List<GameObject> neighbors = currCell.GetAllNeighbors();
                    for (int s = 0; s < neighbors.Count; s++) {
                        densitySum += neighbors[s].GetComponent<Cell>().densityCurrent;
                    }
                    currCell.densitySource = (currCell.densityCurrent + a * densitySum) / (1 + 6 * a);
                }
            }
        SetBndDensity();
    }


    void DiffuseVelocity()
        {

            // x dimension
            int k;
            float a = dt * visc * N * N * N;
            for (k = 0; k < 20; k++) {
                for (int i = 0; i < nonBoundaryCells.Count; i++) {
                    Cell currCell = nonBoundaryCells[i].GetComponent<Cell>();
                    float velocitySum = 0;
                    List<GameObject> neighbors = currCell.GetAllNeighbors();
                    for (int s = 0; s < neighbors.Count; s++) {
                        velocitySum += neighbors[s].GetComponent<Cell>().velocityCurrent[0];
                    }
                    currCell.velocitySource[0] = (currCell.velocityCurrent[0] + a * velocitySum) / (1 + 6 * a);
                }
            }
            SetBndVelocity(1);

            // y dimension
            for (k = 0; k < 20; k++) {
                for (int i = 0; i < nonBoundaryCells.Count; i++) {
                    Cell currCell = nonBoundaryCells[i].GetComponent<Cell>();
                    float velocitySum = 0;
                    List<GameObject> neighbors = currCell.GetAllNeighbors();
                    for (int s = 0; s < neighbors.Count; s++) {
                        velocitySum += neighbors[s].GetComponent<Cell>().velocityCurrent[1];
                    }
                    currCell.velocitySource[1] = (currCell.velocityCurrent[1] + a * velocitySum) / (1 + 6 * a);
                }
            }
            SetBndVelocity(2);

            // z dimension
            for (k = 0; k < 20; k++) {
                for (int i = 0; i < nonBoundaryCells.Count; i++) {
                    Cell currCell = nonBoundaryCells[i].GetComponent<Cell>();
                    float velocitySum = 0;
                    List<GameObject> neighbors = currCell.GetAllNeighbors();
                    for (int s = 0; s < neighbors.Count; s++) {
                        velocitySum += neighbors[s].GetComponent<Cell>().velocityCurrent[2];
                    }
                    currCell.velocitySource[2] = (currCell.velocityCurrent[2] + a * velocitySum) / (1 + 6 * a);
                }
            }
            SetBndVelocity(3);
    }

    void AddSource()
    {
        GameObject[] cells = GameObject.FindGameObjectsWithTag("cell");
        for (int i = 0; i < cells.Length; i++) {
            Cell currCell = cells[i].GetComponent<Cell>();
            currCell.densityPast = currCell.densityCurrent;
            currCell.densityCurrent = currCell.densitySource;
        }
    }

    void AddVelocitySource()
    { 
        GameObject[] cells = GameObject.FindGameObjectsWithTag("cell");
        for (int i = 0; i < cells.Length; i++) {
            Cell currCell = cells[i].GetComponent<Cell>();
            currCell.velocityPast = currCell.velocityCurrent;
            currCell.velocityCurrent = currCell.velocitySource;
        }
    }
    void RenderCells()
    {
        GameObject[] cells = GameObject.FindGameObjectsWithTag("cell");
        for (int i = 0; i < cells.Length; i++) {
            Cell currCell = cells[i].GetComponent<Cell>();
            currCell.RenderDensity();
        }
    }
    void SetBndDensity()
    {
        // normal boundary cells are the
        for (int i = 0; i < bottomCells.Count; i++) {
            Cell currCell = bottomCells[i].GetComponent<Cell>();
            currCell.densitySource = currCell.up.GetComponent<Cell>().densitySource;
        }
        for (int i = 0; i < topCells.Count; i++) {
            Cell currCell = topCells[i].GetComponent<Cell>();
            currCell.densitySource = currCell.down.GetComponent<Cell>().densitySource;
        }
        for (int i = 0; i < leftCells.Count; i++) {
            Cell currCell = leftCells[i].GetComponent<Cell>();
            currCell.densitySource = currCell.right.GetComponent<Cell>().densitySource;
        }
        for (int i = 0; i < rightCells.Count; i++) {
            Cell currCell = rightCells[i].GetComponent<Cell>();
            currCell.densitySource = currCell.left.GetComponent<Cell>().densitySource;
        }
        for (int i = 0; i < frontCells.Count; i++) {
            Cell currCell = frontCells[i].GetComponent<Cell>();
                currCell.densitySource = currCell.back.GetComponent<Cell>().densitySource;
        }
        for (int i = 0; i < backCells.Count; i++) {
            Cell currCell = backCells[i].GetComponent<Cell>();
            currCell.densitySource = currCell.front.GetComponent<Cell>().densitySource;
        }
        // corners are average of surrounding cells

        // (0, 0, 0)
        Vector3 v1 = new Vector3(0, 0, 0);
        GameObject c1Object = cornerCells[v1];
        Cell c1 = c1Object.GetComponent<Cell>();
        // average of right, top, and back
        c1.densitySource =  0.33f * (c1.right.GetComponent<Cell>().densitySource + c1.up.GetComponent<Cell>().densitySource + c1.back.GetComponent<Cell>().densitySource);

        // (0, 0, 1)
        Vector3 v2 = new Vector3(0, 0, N + 1);
        GameObject c2Object = cornerCells[v2];
        Cell c2 = c2Object.GetComponent<Cell>();
        // average of front, right, and top
        c2.densitySource =  0.33f * (c2.front.GetComponent<Cell>().densitySource + c2.right.GetComponent<Cell>().densitySource + c2.up.GetComponent<Cell>().densitySource);

        // (0, 1, 0)
        Vector3 v3 = new Vector3(0, N + 1, 0);
        GameObject c3Object = cornerCells[v3];
        Cell c3 = c3Object.GetComponent<Cell>();
        // average of bottom, back, and right
        c3.densitySource =  0.33f * (c3.down.GetComponent<Cell>().densitySource + c3.back.GetComponent<Cell>().densitySource + c3.right.GetComponent<Cell>().densitySource);

        // (0, 1, 1)
        Vector3 v4 = new Vector3(0, N + 1, N + 1);
        GameObject c4Object = cornerCells[v4];
        Cell c4 = c4Object.GetComponent<Cell>();
        // average of front, right, and bottom
        c4.densityCurrent =  0.33f * (c4.front.GetComponent<Cell>().densitySource + c4.right.GetComponent<Cell>().densitySource + c4.down.GetComponent<Cell>().densitySource);

        // (1, 1, 0)
        Vector3 v5 = new Vector3(N + 1, N + 1, 0);
        GameObject c5Object = cornerCells[v5];
        Cell c5 = c5Object.GetComponent<Cell>();
        // back, left, bottom
        c5.densitySource =  0.33f * (c5.back.GetComponent<Cell>().densitySource + c5.left.GetComponent<Cell>().densitySource + c5.down.GetComponent<Cell>().densitySource);

        // (1, 0, 0)
        Vector3 v6 = new Vector3(N + 1, 0, 0);
        GameObject c6Object = cornerCells[v6];
        Cell c6 = c6Object.GetComponent<Cell>();
        // top, left , back
        c6.densitySource =  0.33f * (c6.up.GetComponent<Cell>().densitySource + c6.left.GetComponent<Cell>().densitySource + c6.back.GetComponent<Cell>().densitySource);

        // (1, 0, 1)
        Vector3 v7 = new Vector3(N + 1, 0, N + 1);
        GameObject c7Object = cornerCells[v7];
        Cell c7 = c7Object.GetComponent<Cell>();
        // front, top, left
        c7.densitySource =  0.33f * (c7.front.GetComponent<Cell>().densitySource + c7.up.GetComponent<Cell>().densitySource + c7.left.GetComponent<Cell>().densitySource);

        // (1, 1, 1)
        Vector3 v8 = new Vector3(N + 1, 0, N + 1);
        GameObject c8Object = cornerCells[v8];
        Cell c8 = c8Object.GetComponent<Cell>();
        // front, bottom, left
        c8.densitySource =  0.5f * (c8.front.GetComponent<Cell>().densitySource +  c8.left.GetComponent<Cell>().densitySource);
        // c8.down.GetComponent<Cell>().densityCurrent i dont know why adding this makes it error
 }

    void DensityStep()
    {
        AddSource();
        Diffuse();
        AdvectDensity();
        RenderCells();
    }

    void VelocityStep()
    {
        AddVelocitySource();
        DiffuseVelocity();
        Project();
        AdvectVelocity();
        Project();
    }



 void Project()
    {
        for (int i = 0; i < nonBoundaryCells.Count; i++) {
            Cell currCell = nonBoundaryCells[i].GetComponent<Cell>();

            float innerSum = currCell.right.GetComponent<Cell>().velocityCurrent.x - currCell.left.GetComponent<Cell>().velocityCurrent.x;
            innerSum += currCell.up.GetComponent<Cell>().velocityCurrent.y - currCell.down.GetComponent<Cell>().velocityCurrent.y;
            innerSum += currCell.front.GetComponent<Cell>().velocityCurrent.z - currCell.back.GetComponent<Cell>().velocityCurrent.z;

            currCell.div = -0.33f * cubeDimension * innerSum;
            currCell.p = 0f;
        }

        SetBndDiv(); // set bnd div
        // set bnd p

        for (int k = 0 ; k < 20 ; k++) {
            for (int i = 0; i < nonBoundaryCells.Count; i++) {
                Cell currCell = nonBoundaryCells[i].GetComponent<Cell>();

                float innerSum = currCell.div;
                innerSum += currCell.right.GetComponent<Cell>().p + currCell.left.GetComponent<Cell>().p;
                innerSum += currCell.up.GetComponent<Cell>().p + currCell.down.GetComponent<Cell>().p;
                innerSum += currCell.front.GetComponent<Cell>().p + currCell.back.GetComponent<Cell>().p;

                currCell.p = innerSum / 6;
            }
            SetBndP(); // set bnd p
        }

        for (int i = 0; i < nonBoundaryCells.Count; i++) {
            Cell currCell = nonBoundaryCells[i].GetComponent<Cell>();

            currCell.velocityCurrent.x -= 0.33f * (currCell.right.GetComponent<Cell>().p -  currCell.left.GetComponent<Cell>().p) / cubeDimension;
            currCell.velocityCurrent.y -= 0.33f * (currCell.up.GetComponent<Cell>().p - currCell.down.GetComponent<Cell>().p) / cubeDimension;
            currCell.velocityCurrent.z -= 0.33f * (currCell.front.GetComponent<Cell>().p - currCell.back.GetComponent<Cell>().p) / cubeDimension;
        }
        SetBndVelocity(1);
        SetBndVelocity(2);
    }


void SetBndVelocity(int b)
    {
       
        for (int i = 0; i < bottomCells.Count; i++) {
            Cell currCell = bottomCells[i].GetComponent<Cell>();

            if (b == 2) {
                currCell.velocitySource = -1 * currCell.up.GetComponent<Cell>().velocitySource;
            } else {
                currCell.velocitySource = currCell.up.GetComponent<Cell>().velocitySource;
            }
           
        }
        for (int i = 0; i < topCells.Count; i++) {
            Cell currCell = topCells[i].GetComponent<Cell>();

            if (b == 2) {
                currCell.velocitySource = -1 * currCell.down.GetComponent<Cell>().velocitySource;
            } else {
                currCell.velocitySource = currCell.down.GetComponent<Cell>().velocitySource;
            }
           
        }
        for (int i = 0; i < leftCells.Count; i++) {
            Cell currCell = leftCells[i].GetComponent<Cell>();
            if (b == 1) {
                currCell.velocitySource = -1 * currCell.right.GetComponent<Cell>().velocitySource;
            } else {
                currCell.velocitySource = currCell.right.GetComponent<Cell>().velocitySource;
            }
           
        }
        for (int i = 0; i < rightCells.Count; i++) {
            Cell currCell = rightCells[i].GetComponent<Cell>();


            if (b == 1) {
                currCell.velocitySource = -1 * currCell.left.GetComponent<Cell>().velocitySource;
            } else {
                currCell.velocitySource = currCell.left.GetComponent<Cell>().velocitySource;
            }
           
        }
        for (int i = 0; i < frontCells.Count; i++) {
            Cell currCell = frontCells[i].GetComponent<Cell>();
            if (b == 3) {
                currCell.velocitySource = -1 * currCell.back.GetComponent<Cell>().velocitySource;
            } else {
                currCell.velocitySource = currCell.back.GetComponent<Cell>().velocitySource;
            }
           
        }
        for (int i = 0; i < backCells.Count; i++) {
            Cell currCell = backCells[i].GetComponent<Cell>();
            if (b == 3) {
                currCell.velocitySource = -1 * currCell.front.GetComponent<Cell>().velocitySource;
            } else {
                currCell.velocitySource = currCell.front.GetComponent<Cell>().velocitySource;
            }
        }
        //rest of function...
        // corners are average of surrounding cells
        // (0, 0, 0)
        Vector3 v1 = new Vector3(0, 0, 0);
        GameObject c1Object = cornerCells[v1];
        Cell c1 = c1Object.GetComponent<Cell>();
        // average of right, top, and back
        c1.velocitySource =  0.33f * (c1.right.GetComponent<Cell>().velocitySource + c1.up.GetComponent<Cell>().velocitySource + c1.back.GetComponent<Cell>().velocitySource);


        // (0, 0, 1)
        Vector3 v2 = new Vector3(0, 0, N + 1);
        GameObject c2Object = cornerCells[v2];
        Cell c2 = c2Object.GetComponent<Cell>();
        // average of front, right, and top
        c2.velocitySource =  0.33f * (c2.front.GetComponent<Cell>().velocitySource + c2.right.GetComponent<Cell>().velocitySource + c2.up.GetComponent<Cell>().velocitySource);




        // (0, 1, 0)
        Vector3 v3 = new Vector3(0, N + 1, 0);
        GameObject c3Object = cornerCells[v3];
        Cell c3 = c3Object.GetComponent<Cell>();
        // average of bottom, back, and right
        c3.velocitySource =  0.33f * (c3.down.GetComponent<Cell>().velocitySource + c3.back.GetComponent<Cell>().velocitySource + c3.right.GetComponent<Cell>().velocitySource);




        // (0, 1, 1)
        Vector3 v4 = new Vector3(0, N + 1, N + 1);
        GameObject c4Object = cornerCells[v4];
        Cell c4 = c4Object.GetComponent<Cell>();
        // average of front, right, and bottom
        c4.velocitySource =  0.33f * (c4.front.GetComponent<Cell>().velocitySource + c4.right.GetComponent<Cell>().velocitySource + c4.down.GetComponent<Cell>().velocitySource);




        // (1, 1, 0)
        Vector3 v5 = new Vector3(N + 1, N + 1, 0);
        GameObject c5Object = cornerCells[v5];
        Cell c5 = c5Object.GetComponent<Cell>();
        // back, left, bottom
        c5.velocitySource =  0.33f * (c5.back.GetComponent<Cell>().velocitySource + c5.left.GetComponent<Cell>().velocitySource + c5.down.GetComponent<Cell>().velocitySource);




        // (1, 0, 0)
        Vector3 v6 = new Vector3(N + 1, 0, 0);
        GameObject c6Object = cornerCells[v6];
        Cell c6 = c6Object.GetComponent<Cell>();
        // top, left , back
        c6.velocitySource =  0.33f * (c6.up.GetComponent<Cell>().velocitySource + c6.left.GetComponent<Cell>().velocitySource + c6.back.GetComponent<Cell>().velocitySource);


       
        // (1, 0, 1)
        Vector3 v7 = new Vector3(N + 1, 0, N + 1);
        GameObject c7Object = cornerCells[v7];
        Cell c7 = c7Object.GetComponent<Cell>();
        // front, top, left
        c7.velocitySource =  0.33f * (c7.front.GetComponent<Cell>().velocitySource + c7.up.GetComponent<Cell>().velocitySource + c7.left.GetComponent<Cell>().velocitySource);

        // (1, 1, 1)
        Vector3 v8 = new Vector3(N + 1, 0, N + 1);
        GameObject c8Object = cornerCells[v8];
        Cell c8 = c8Object.GetComponent<Cell>();
        // front, bottom, left
        c8.velocitySource =  0.5f * (c8.front.GetComponent<Cell>().velocitySource +  c8.left.GetComponent<Cell>().velocitySource);


// // c8.down.GetComponent<Cell>().densityCurrent i dont know why adding this makes it error
    }




    void SetBndP()
    {
        // normal boundary cells are the
        for (int i = 0; i < bottomCells.Count; i++) {
            Cell currCell = bottomCells[i].GetComponent<Cell>();
            currCell.p = currCell.up.GetComponent<Cell>().p;
        }
        for (int i = 0; i < topCells.Count; i++) {
            Cell currCell = topCells[i].GetComponent<Cell>();
            currCell.p = currCell.down.GetComponent<Cell>().p;
        }
        for (int i = 0; i < leftCells.Count; i++) {
            Cell currCell = leftCells[i].GetComponent<Cell>();
            currCell.p = currCell.right.GetComponent<Cell>().p;
        }
        for (int i = 0; i < rightCells.Count; i++) {
            Cell currCell = rightCells[i].GetComponent<Cell>();
            currCell.p = currCell.left.GetComponent<Cell>().p;
        }
        for (int i = 0; i < frontCells.Count; i++) {
            Cell currCell = frontCells[i].GetComponent<Cell>();
                currCell.p = currCell.back.GetComponent<Cell>().p;
        }
        for (int i = 0; i < backCells.Count; i++) {
            Cell currCell = backCells[i].GetComponent<Cell>();
            currCell.p = currCell.front.GetComponent<Cell>().p;
        }
        // corners are average of surrounding cells

        // (0, 0, 0)
        Vector3 v1 = new Vector3(0, 0, 0);
        GameObject c1Object = cornerCells[v1];
        Cell c1 = c1Object.GetComponent<Cell>();
        // average of right, top, and back
        c1.p =  0.33f * (c1.right.GetComponent<Cell>().p + c1.up.GetComponent<Cell>().p + c1.back.GetComponent<Cell>().p);

        // (0, 0, 1)
        Vector3 v2 = new Vector3(0, 0, N + 1);
        GameObject c2Object = cornerCells[v2];
        Cell c2 = c2Object.GetComponent<Cell>();
        // average of front, right, and top
        c2.p =  0.33f * (c2.front.GetComponent<Cell>().p + c2.right.GetComponent<Cell>().p + c2.up.GetComponent<Cell>().p);

        // (0, 1, 0)
        Vector3 v3 = new Vector3(0, N + 1, 0);
        GameObject c3Object = cornerCells[v3];
        Cell c3 = c3Object.GetComponent<Cell>();
        // average of bottom, back, and right
        c3.p =  0.33f * (c3.down.GetComponent<Cell>().p + c3.back.GetComponent<Cell>().p + c3.right.GetComponent<Cell>().p);

        // (0, 1, 1)
        Vector3 v4 = new Vector3(0, N + 1, N + 1);
        GameObject c4Object = cornerCells[v4];
        Cell c4 = c4Object.GetComponent<Cell>();
        // average of front, right, and bottom
        c4.p =  0.33f * (c4.front.GetComponent<Cell>().p + c4.right.GetComponent<Cell>().p + c4.down.GetComponent<Cell>().p);

        // (1, 1, 0)
        Vector3 v5 = new Vector3(N + 1, N + 1, 0);
        GameObject c5Object = cornerCells[v5];
        Cell c5 = c5Object.GetComponent<Cell>();
        // back, left, bottom
        c5.densitySource =  0.33f * (c5.back.GetComponent<Cell>().p + c5.left.GetComponent<Cell>().p + c5.down.GetComponent<Cell>().p);

        // (1, 0, 0)
        Vector3 v6 = new Vector3(N + 1, 0, 0);
        GameObject c6Object = cornerCells[v6];
        Cell c6 = c6Object.GetComponent<Cell>();
        // top, left , back
        c6.densitySource =  0.33f * (c6.up.GetComponent<Cell>().p + c6.left.GetComponent<Cell>().p + c6.back.GetComponent<Cell>().p);

        // (1, 0, 1)
        Vector3 v7 = new Vector3(N + 1, 0, N + 1);
        GameObject c7Object = cornerCells[v7];
        Cell c7 = c7Object.GetComponent<Cell>();
        // front, top, left
        c7.densitySource =  0.33f * (c7.front.GetComponent<Cell>().p + c7.up.GetComponent<Cell>().p + c7.left.GetComponent<Cell>().p);

        // (1, 1, 1)
        Vector3 v8 = new Vector3(N + 1, 0, N + 1);
        GameObject c8Object = cornerCells[v8];
        Cell c8 = c8Object.GetComponent<Cell>();
        // front, bottom, left
        c8.densitySource =  0.5f * (c8.front.GetComponent<Cell>().p +  c8.left.GetComponent<Cell>().p);
        // c8.down.GetComponent<Cell>().densityCurrent i dont know why adding this makes it error
 }



    void SetBndDiv()
    {
        // normal boundary cells are the
        for (int i = 0; i < bottomCells.Count; i++) {
            Cell currCell = bottomCells[i].GetComponent<Cell>();
            currCell.div = currCell.up.GetComponent<Cell>().div;
        }
        for (int i = 0; i < topCells.Count; i++) {
            Cell currCell = topCells[i].GetComponent<Cell>();
            currCell.div = currCell.down.GetComponent<Cell>().div;
        }
        for (int i = 0; i < leftCells.Count; i++) {
            Cell currCell = leftCells[i].GetComponent<Cell>();
            currCell.div = currCell.right.GetComponent<Cell>().div;
        }
        for (int i = 0; i < rightCells.Count; i++) {
            Cell currCell = rightCells[i].GetComponent<Cell>();
            currCell.div = currCell.left.GetComponent<Cell>().div;
        }
        for (int i = 0; i < frontCells.Count; i++) {
            Cell currCell = frontCells[i].GetComponent<Cell>();
                currCell.div = currCell.back.GetComponent<Cell>().div;
        }
        for (int i = 0; i < backCells.Count; i++) {
            Cell currCell = backCells[i].GetComponent<Cell>();
            currCell.div = currCell.front.GetComponent<Cell>().div;
        }
        // corners are average of surrounding cells

        // (0, 0, 0)
        Vector3 v1 = new Vector3(0, 0, 0);
        GameObject c1Object = cornerCells[v1];
        Cell c1 = c1Object.GetComponent<Cell>();
        // average of right, top, and back
        c1.div =  0.33f * (c1.right.GetComponent<Cell>().div + c1.up.GetComponent<Cell>().div + c1.back.GetComponent<Cell>().div);

        // (0, 0, 1)
        Vector3 v2 = new Vector3(0, 0, N + 1);
        GameObject c2Object = cornerCells[v2];
        Cell c2 = c2Object.GetComponent<Cell>();
        // average of front, right, and top
        c2.div =  0.33f * (c2.front.GetComponent<Cell>().div + c2.right.GetComponent<Cell>().div + c2.up.GetComponent<Cell>().div);

        // (0, 1, 0)
        Vector3 v3 = new Vector3(0, N + 1, 0);
        GameObject c3Object = cornerCells[v3];
        Cell c3 = c3Object.GetComponent<Cell>();
        // average of bottom, back, and right
        c3.div =  0.33f * (c3.down.GetComponent<Cell>().div + c3.back.GetComponent<Cell>().div + c3.right.GetComponent<Cell>().div);

        // (0, 1, 1)
        Vector3 v4 = new Vector3(0, N + 1, N + 1);
        GameObject c4Object = cornerCells[v4];
        Cell c4 = c4Object.GetComponent<Cell>();
        // average of front, right, and bottom
        c4.div =  0.33f * (c4.front.GetComponent<Cell>().div + c4.right.GetComponent<Cell>().div + c4.down.GetComponent<Cell>().div);

        // (1, 1, 0)
        Vector3 v5 = new Vector3(N + 1, N + 1, 0);
        GameObject c5Object = cornerCells[v5];
        Cell c5 = c5Object.GetComponent<Cell>();
        // back, left, bottom
        c5.densitySource =  0.33f * (c5.back.GetComponent<Cell>().div + c5.left.GetComponent<Cell>().div + c5.down.GetComponent<Cell>().div);

        // (1, 0, 0)
        Vector3 v6 = new Vector3(N + 1, 0, 0);
        GameObject c6Object = cornerCells[v6];
        Cell c6 = c6Object.GetComponent<Cell>();
        // top, left , back
        c6.densitySource =  0.33f * (c6.up.GetComponent<Cell>().div + c6.left.GetComponent<Cell>().div + c6.back.GetComponent<Cell>().div);

        // (1, 0, 1)
        Vector3 v7 = new Vector3(N + 1, 0, N + 1);
        GameObject c7Object = cornerCells[v7];
        Cell c7 = c7Object.GetComponent<Cell>();
        // front, top, left
        c7.densitySource =  0.33f * (c7.front.GetComponent<Cell>().div+ c7.up.GetComponent<Cell>().div + c7.left.GetComponent<Cell>().div);

        // (1, 1, 1)
        Vector3 v8 = new Vector3(N + 1, 0, N + 1);
        GameObject c8Object = cornerCells[v8];
        Cell c8 = c8Object.GetComponent<Cell>();
        // front, bottom, left
        c8.densitySource =  0.5f * (c8.front.GetComponent<Cell>().div +  c8.left.GetComponent<Cell>().div);
        // c8.down.GetComponent<Cell>().densityCurrent i dont know why adding this makes it error
 }
}
































