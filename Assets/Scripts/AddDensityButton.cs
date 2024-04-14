using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddDensityButton : MonoBehaviour
{

    public Cell cellToAddDensity;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddDensity() 
    {
        cellToAddDensity.AddSourceToCell(Random.Range(0f, 1f));
    }
    public void AddVelocity()
    {
        cellToAddDensity.AddVelocitySourceToCell(new Vector3(1.0f, 0f, 0f));
    }
}
