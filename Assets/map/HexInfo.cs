using UnityEngine;
using System.Collections;

public class HexInfo { 

    private Vector3 gridPosition; // Cube coordinates stored(x, y == axial)
    public Vector3 localPosition;
    public Vector3 worldPosition;

    public Vector3 hexExt;
    public Vector3 hexCenter;

    public HexChunk parentChunk;

    public Mesh localMesh;

    // Basic hexagon mesh making
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uv;

    #region update

   
    #endregion
    // Get axial grid position
    public Vector2 AxialGridPosition
    {
        get { return new Vector2(CubeGridPosition.x, CubeGridPosition.y); }
    }

    // Get/Set cube grid position
    public Vector3 CubeGridPosition
    {
        get { return gridPosition; }
        set { gridPosition = value; }
    }

    public void Start()
    {
        MeshSetup();
    }

    void MeshSetup()
    {
        localMesh = new Mesh();

        localMesh.vertices = parentChunk.worldManager.flatHexagonSharedMesh.vertices;
        localMesh.triangles = parentChunk.worldManager.flatHexagonSharedMesh.triangles;
        localMesh.uv = parentChunk.worldManager.flatHexagonSharedMesh.uv;

        localMesh.RecalculateNormals();
    }

    void fixedUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Camera.main.nearClipPlane, Input.mousePosition.z));
        }
    }




}
