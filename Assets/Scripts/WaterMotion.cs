using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class WaterMotion : MonoBehaviour
{
    public float waveHeight = 0.5f;
    public float waveFrequency = 0.5f;
    public float waveLength = 2f;

    private Vector3[] originalVertices;
    private Mesh mesh;

    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        originalVertices = mesh.vertices;
    }

    void Update()
    {
        Vector3[] vertices = mesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 vertex = originalVertices[i];
            vertex.y += waveHeight * Mathf.Sin(Time.time * waveFrequency + originalVertices[i].x * waveLength) * Random.Range(0f, 0.5f);
            vertices[i] = vertex;
        }
        mesh.vertices = vertices;
        mesh.RecalculateNormals();  // Recalculate normals to reflect the new vertex positions
    }
}