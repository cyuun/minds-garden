﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadScript : MonoBehaviour
{
    public bool matchTerrainHeight = true;
    public float rotationSpeed = 300f;
    public GameObject terrain;
    public TerrainScript terrainScript;

    private MeshFilter _meshFilter;
    private MeshCollider _meshCollider;
    private bool _rotating = false;
    public bool isRotating { get { return _rotating; } set { _rotating = value; } }

    public void Activate()
    {
        ColorController.S.SetActiveHead(gameObject);
        // add head to the audio listener too
    }

    private void Start()
    {
        terrain = transform.Find("Terrain").gameObject;
        terrainScript = terrain.GetComponent<TerrainScript>();
        
        _meshFilter = GetComponent<MeshFilter>();
        _meshCollider = GetComponent<MeshCollider>();

        if (matchTerrainHeight)
        {
            AdjustMaxVerticesHeight(5);
        }

        if(ColorController.S) StartCoroutine("DelayedActivate");
    }

    void Update()
    {
        if (_rotating)
        {
            transform.Rotate(Vector3.up, Time.deltaTime * rotationSpeed);

        }
    }

    private void AdjustMaxVerticesHeight(float tolerance)
    {
        Mesh mesh = _meshFilter.mesh;
        Vector3[] vertices = mesh.vertices;
        float maxHeight = GetMaxVertexHeight();

        for (int i = 0; i < vertices.Length; i++)
        {
            if (CompareFloats(vertices[i].y, maxHeight, tolerance))
            {
                Vector3 worldSpaceVertex = VectorObjectToWorld(vertices[i]);
                vertices[i].y = -transform.position.y + 9 + terrainScript.GetTerrainHeight(worldSpaceVertex.x, worldSpaceVertex.z);
            }
        }

        mesh.vertices = vertices;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();
        _meshFilter.mesh = mesh;
        _meshCollider.sharedMesh = mesh;
    }

    private float GetMaxVertexHeight()
    {
        Vector3[] vertices = _meshFilter.mesh.vertices;
        float maxHeight = 0;

        for (int i = 0; i < vertices.Length; i++)
        {
            if (vertices[i].y > maxHeight)
            {
                maxHeight = vertices[i].y;
            }
        }

        return maxHeight;
    }

    private Vector3 VectorObjectToWorld(Vector3 vector)
    {
        return transform.rotation * new Vector3(vector.x - transform.position.x, vector.y - transform.position.y, vector.z - transform.position.z);
    }
    
    private bool CompareFloats(float f1, float f2, float tolerance)
    {
        return (f1 <= f2 + tolerance && f1 >= f2 - tolerance);
    }

    private IEnumerator DelayedActivate()
    {
        yield return new WaitForSeconds(2f);
        
        Activate();
    }
}