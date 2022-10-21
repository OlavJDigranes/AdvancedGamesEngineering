using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class TileMap : MonoBehaviour
{
    public int sizeX = 100;
    public int sizeZ = 50;
    public float tileSize = 1.0f;
    public int tileResolution = 8; 

    // Start is called before the first frame update
    void Start()
    {
        BuildMesh(); 
    }

    // Update is called once per frame
    public void BuildMesh()
    {
        int numTiles = sizeX * sizeZ;
        int numTriangles = numTiles * 2;

        int vSizeX = sizeX + 1;
        int vSizeZ = sizeZ + 1;
        int numVertices = vSizeX * vSizeZ;

        //Generate mesh data
        Vector3[] vertices = new Vector3[numVertices];
        Vector3[] normals = new Vector3[numVertices];
        Vector2[] uv = new Vector2[numVertices];

        int[] triangles = new int[numTriangles * 3];

        int x, z;

        //Vertices
        for (z = 0; z < vSizeZ; z++)
        {
            for (x = 0; x < vSizeX; x++)
            {
                vertices[z * vSizeX + x] = new Vector3(x * tileSize, 0, z * tileSize);
                normals[z * vSizeX + x] = Vector3.up;
                uv[z * vSizeX + x] = new Vector2((float)x/vSizeX, (float)z/vSizeZ); 
            }
        }

        //Triangles
        for (z = 0; z < vSizeZ; z++)
        {
            for (x = 0; x < vSizeX; x++) 
            {
                int squareIndex = z * sizeX + x;
                int triangleOffset = squareIndex * 6;

                triangles[triangleOffset + 0] = z * vSizeX + x + 0; 
                triangles[triangleOffset + 1] = z * vSizeX + x + vSizeX + 0; 
                triangles[triangleOffset + 2] = z * vSizeX + x + vSizeX + 1; 
                triangles[triangleOffset + 3] = z * vSizeX + x + 0; 
                triangles[triangleOffset + 4] = z * vSizeX + x + vSizeX + 1; 
                triangles[triangleOffset + 5] = z * vSizeX + x + 1; 
            }
        }

        //Create a new mesh and populate with the data
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.normals = normals;
        mesh.uv = uv;

        //Assign mesh to filter/renderer/collider
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        MeshCollider meshCollider = GetComponent<MeshCollider>();

        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;

        BuildTexture(); 
    }

    void BuildTexture()
    {
        int texWidth = sizeX; 
        int texHeight = sizeZ; 

        Texture2D tex = new Texture2D(texWidth, texHeight);

        for (int y = 0; y < texHeight; y++)
        {
            for (int x = 0; x < texWidth; x++)
            {
                //tex.SetPixel(x, y, ); 
            }
        }

        tex.Apply();

        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.sharedMaterial.mainTexture = tex;  
    }
}
