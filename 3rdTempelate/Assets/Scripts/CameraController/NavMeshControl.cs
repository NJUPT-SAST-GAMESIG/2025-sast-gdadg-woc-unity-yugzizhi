using UnityEngine.AI;
using UnityEngine;
using System.Collections.Generic;

public class NavMeshControl : MonoBehaviour
{
    [SerializeField] private Material mapMaterial;
    private int mapLayerIndex = 10;
    [SerializeField] private float minLineLength;

    void Start()
    {
        GenerateMap();
    }

    private void GenerateMap()
    {
        NavMeshTriangulation triangulatedNavMesh = NavMesh.CalculateTriangulation();

        Mesh mapMesh = new Mesh();
        mapMesh.name = "map_NavMesh";
        Vector3[] flatVertices = triangulatedNavMesh.vertices;
        for(int i=1;i<flatVertices.Length; i++)
        {
            flatVertices[i].y = 0;
        }
        mapMesh .vertices = flatVertices;
        mapMesh.triangles = triangulatedNavMesh.indices;
        mapMesh.RecalculateBounds();

        GameObject mapObject = new GameObject("map_WalkableAreaOutlines");
        mapObject.layer = mapLayerIndex;
        MeshFilter meshFilter = mapObject.AddComponent<MeshFilter>();
        meshFilter.mesh = mapMesh;

        MeshRenderer meshRenderer = mapObject.AddComponent<MeshRenderer>();
        meshRenderer.material = mapMaterial;
    }
}
