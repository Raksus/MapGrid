using UnityEngine;
using System.Collections;

public class WorldManager : MonoBehaviour {

    #region fields
    public Mesh flatHexagonSharedMesh;
    public float hexRadiusSize;

    // hexInstances
    [HideInInspector]
    public Vector3 hexExt;
    [HideInInspector]
    public Vector3 hexSize;
    [HideInInspector]
    public Vector3 hexCenter;
    [HideInInspector]
    public GameObject chunkHolder;

    public Texture2D terrainTexture;

    int xSectors;
    int zSectors;

    public HexChunk[,] hexChunks;

    public Vector2 mapSize;
    public int chunkSize;
    #endregion

    #region GetHexProperties
    private void GetHexProperties()
    {
        // Create mesh to calculate bounds
        GameObject inst = new GameObject("Bounds Set Up: Flat");
        // Add mesh filter
        inst.AddComponent<MeshFilter>();
        // Add renderer
        inst.AddComponent<MeshRenderer>();
        // Add collider
        inst.AddComponent<MeshCollider>();
        // Reset the position to zero
        inst.transform.position = Vector3.zero;
        // reset all rotation
        inst.transform.rotation = Quaternion.identity;

        Vector3[] vertices;
        int[] triangles;
        Vector2[] uv;

        #region verts

        float floorLevel = 0;

        // Posicionar los vertices para hacer un hexagono
        vertices = new Vector3[]
        {
            new Vector3((hexRadiusSize * Mathf.Cos((float)(2*Mathf.PI*(3+0.5)/6))), floorLevel, (hexRadiusSize * Mathf.Sin((float)(2*Mathf.PI*(3+0.5)/6)))),
            new Vector3((hexRadiusSize * Mathf.Cos((float)(2*Mathf.PI*(2+0.5)/6))), floorLevel, (hexRadiusSize * Mathf.Sin((float)(2*Mathf.PI*(2+0.5)/6)))),
            new Vector3((hexRadiusSize * Mathf.Cos((float)(2*Mathf.PI*(1+0.5)/6))), floorLevel, (hexRadiusSize * Mathf.Sin((float)(2*Mathf.PI*(1+0.5)/6)))),
            new Vector3((hexRadiusSize * Mathf.Cos((float)(2*Mathf.PI*(0+0.5)/6))), floorLevel, (hexRadiusSize * Mathf.Sin((float)(2*Mathf.PI*(0+0.5)/6)))),
            new Vector3((hexRadiusSize * Mathf.Cos((float)(2*Mathf.PI*(5+0.5)/6))), floorLevel, (hexRadiusSize * Mathf.Sin((float)(2*Mathf.PI*(5+0.5)/6)))),
            new Vector3((hexRadiusSize * Mathf.Cos((float)(2*Mathf.PI*(4+0.5)/6))), floorLevel, (hexRadiusSize * Mathf.Sin((float)(2*Mathf.PI*(4+0.5)/6))))
        };

        #endregion

        #region triangles

        // Triangulos que conectan los vertices
        triangles = new int[]
        {
            1,5,0,
            1,4,5,
            1,2,4,
            2,3,4
        };

        #endregion

        #region uv

        // UV mapping
        uv = new Vector2[]
        {
            new Vector2(0,0.25f),
            new Vector2(0,0.75f),
            new Vector2(0.5f,1),
            new Vector2(1,0.75f),
            new Vector2(1,0.25f),
            new Vector2(0.5f,0)
        };

        #endregion

        #region finalize
        // Create a new mesh to hold the data for the hexagon
        flatHexagonSharedMesh = new Mesh();
        // Assign verts
        flatHexagonSharedMesh.vertices = vertices;
        // Assign triangles
        flatHexagonSharedMesh.triangles = triangles;
        // Assign UV
        flatHexagonSharedMesh.uv = uv;
        // Set the mesh to the flat hexagon mesh
        inst.GetComponent<MeshFilter>().mesh = flatHexagonSharedMesh;
        // Recalculate normals
        inst.GetComponent<MeshFilter>().mesh.RecalculateNormals();
        // Set mesh collider's mesh to the hexagon
        inst.GetComponent<MeshCollider>().sharedMesh = flatHexagonSharedMesh;

        #endregion

        // Calculate extens of the hexagon
        hexExt = new Vector3(inst.gameObject.GetComponent<Collider>().bounds.extents.x, inst.gameObject.GetComponent<Collider>().bounds.extents.y, inst.gameObject.GetComponent<Collider>().bounds.extents.z);
        // Calculate the size of the hexagon
        hexSize = new Vector3(inst.gameObject.GetComponent<Collider>().bounds.size.x, inst.gameObject.GetComponent<Collider>().bounds.size.y, inst.gameObject.GetComponent<Collider>().bounds.size.z);
        // Calculate the center
        hexCenter = new Vector3(inst.gameObject.GetComponent<Collider>().bounds.center.x, inst.gameObject.GetComponent<Collider>().bounds.center.y, inst.gameObject.GetComponent<Collider>().bounds.center.z);
        // Destruir el objeto temporal
        Destroy(inst);
    }
    #endregion

    #region GenerateMap
    /// <summary>
    /// Generate chunks to make the map
    /// </summary>
    
    void GenerateMap()
    {
        // Determine the number of chunks
        xSectors = Mathf.CeilToInt(mapSize.x / chunkSize);
        zSectors = Mathf.CeilToInt(mapSize.y / chunkSize);

        // Allocate chunk array
        hexChunks = new HexChunk[xSectors, zSectors];

        // Cycle through all chunks
        for(int x = 0; x < xSectors; x++)
        {
            for(int z = 0; z < zSectors; z++)
            {
                // Create the new chunk
                hexChunks[x, z] = NewChunk(x, z);
                // Set the position of the new chunk
                hexChunks[x, z].gameObject.transform.position = new Vector3(x * (chunkSize * hexSize.x), 0f, (z * (chunkSize * hexSize.z) * (.75f)));
                // Set hex size for hexagon positioning
                hexChunks[x, z].hexSize = hexSize;
                // Set the number of hexagons for the chunk to generate
                hexChunks[x, z].SetSize(chunkSize, chunkSize);
                // Set the width interval of the chunk
                hexChunks[x, z].xSector = x;
                // Set the height interval of the chunk
                hexChunks[x, z].ySector = z;
                // Assign the world manager(this)
                hexChunks[x, z].worldManager = this;
            }
        }

        // Cycle throygh all chunks
        foreach(HexChunk chunk in hexChunks)
        {
            // Begin chunk operations since we are done with value generation
            chunk.Begin();
        }
    }

    #endregion

    #region NewChunk

    /// <summary>
    /// Creates a new chunk
    /// </summary>
    /// <param name="x">The width interval of the chunks</param>
    /// <param name="y">The height interval of the chunks</param>
    /// <returns>The new chunk's script</returns>
    public HexChunk NewChunk(int x, int y)
    {
        // If this is the first chunk made?
        if (x == 0 && y == 0)
        {
            chunkHolder = new GameObject("ChunkHolder");
        }
        // Create the chunk object
        GameObject chunkObj = new GameObject("Chunk[" + x + "," + y + "]");
        // Add the hexChunk script and set it's size
        chunkObj.AddComponent<HexChunk>();
        // Allocate the hexagon array
        chunkObj.GetComponent<HexChunk>().AllocateHexArray();
        // Set the texture map for this chunk and add the mesh render
        chunkObj.AddComponent<MeshRenderer>().material.mainTexture = terrainTexture;
        // Add the mesh filter
        chunkObj.AddComponent<MeshFilter>();
        // Make this chunk a child of "ChunkHolder"
        chunkObj.transform.parent = chunkHolder.transform;

        // Return the script on the new chunk
        return chunkObj.GetComponent<HexChunk>();
    }

    #endregion

    #region awake
    public void Awake()
    {
        // Get the flat hexagons size
        GetHexProperties();
        //Generate the chunks
        GenerateMap();
    }

    #endregion
}
