using UnityEngine;

public class MapMeshGenerator : MonoBehaviour {
    //Creates the map mesh

    [Header("Map mesh Settings:")]
    public MeshFilter mainMeshFilter;
    public MeshFilter subMeshFilter;
    public float shoreAngle = .25f;
    public float shoreDepth = -1;
    public Vector2 roughnessMinMax = new Vector2(-.05f, .1f);

    private Mesh terrainMesh;



    private void Start() {
        terrainMesh = mainMeshFilter.mesh;
    }


    public void CreateMapMesh(Vector2Int tileSize, string[,] biomMap, Color[,] colorMap) {
        float[,] roughnessMap = new float[tileSize.x, tileSize.y];

        GenerateRoughnessMap(biomMap, ref roughnessMap);
        CreateMesh(biomMap, ref roughnessMap, colorMap);
        CreateSubMesh(biomMap);

        //Center terrain:
        mainMeshFilter.transform.parent.transform.position = new Vector3(-tileSize.x / 2f, transform.position.y, -tileSize.y / 2f);
    }


    public void CreateMapMeshCollider() {
        //Destroy old mesh collider:
        if(mainMeshFilter.gameObject.GetComponent<MeshCollider>() != null) {
            Destroy(mainMeshFilter.gameObject.GetComponent<MeshCollider>());
        }

        mainMeshFilter.gameObject.AddComponent<MeshCollider>();
    }


    private void GenerateRoughnessMap(string[,] biomMap, ref float[,] roughnessMap) {
        for (int x = 0; x < biomMap.GetLength(0); x++) {
            for (int y = 0; y < biomMap.GetLength(1); y++) {
                if (biomMap[x, y] != null) {
                    roughnessMap[x, y] = 0 + Random.Range(roughnessMinMax.x, roughnessMinMax.y);
                }
            }
        }
    }


    private void CreateMesh(string[,] biomMap, ref float[,] roughnessMap, Color[,] colorMap) {

        #region Count Mesh Vertices:

        int vc = 0; //Vertice Counter

        for (int x = 0; x < biomMap.GetLength(0); x++) {
            for (int y = 0; y < biomMap.GetLength(1); y++) {
                if (!biomMap[x, y].Equals("")) {
                    vc += 24;
                }
            }
        }
        #endregion


        #region Create vertices:

        Vector3[] vertices = new Vector3[vc];
        int[] triangles = new int[vc];
        Color[] colors = ConvertToMeshColorFormat(biomMap, colorMap, vc);

        int vi = 0;

        for (int x = 0; x < biomMap.GetLength(0); x++) {
            for (int y = 0; y < biomMap.GetLength(1); y++) {
                if (!biomMap[x, y].Equals("")) {

                    //Quad (Upper Left): 
                    CreateTriangle(ref vertices, ref triangles, new Vector3(x, 0, y + 1), new Vector3(x + .5f, roughnessMap[x, y], y + .5f), new Vector3(x, 0, y + .5f), ref vi);
                    CreateTriangle(ref vertices, ref triangles, new Vector3(x, 0, y + 1), new Vector3(x + .5f, 0, y + 1), new Vector3(x + .5f, roughnessMap[x, y], y + .5f), ref vi);

                    //Quad (Upper Right): 
                    CreateTriangle(ref vertices, ref triangles, new Vector3(x + .5f, 0, y + 1), new Vector3(x + 1, 0, y + 1), new Vector3(x + .5f, roughnessMap[x, y], y + .5f), ref vi);
                    CreateTriangle(ref vertices, ref triangles, new Vector3(x + 1, 0, y + .5f), new Vector3(x + .5f, roughnessMap[x, y], y + .5f), new Vector3(x + 1, 0, y + 1), ref vi);

                    //Quad (Lower Left): 
                    CreateTriangle(ref vertices, ref triangles, new Vector3(x, 0, y + .5f), new Vector3(x + .5f, roughnessMap[x, y], y + .5f), new Vector3(x, 0, y), ref vi);
                    CreateTriangle(ref vertices, ref triangles, new Vector3(x + .5f, 0, y), new Vector3(x, 0, y), new Vector3(x + .5f, roughnessMap[x, y], y + .5f), ref vi);

                    //Quad (Lower Right): 
                    CreateTriangle(ref vertices, ref triangles, new Vector3(x + .5f, roughnessMap[x, y], y + .5f), new Vector3(x + 1f, 0, y), new Vector3(x + .5f, 0, y), ref vi);
                    CreateTriangle(ref vertices, ref triangles, new Vector3(x + .5f, roughnessMap[x, y], y + .5f), new Vector3(x + 1f, 0, y + .5f), new Vector3(x + 1f, 0, y), ref vi);
                }
            }
        }
        #endregion


        #region Create mesh:

        terrainMesh = new Mesh {
            vertices = vertices,
            triangles = triangles,
            colors = colors
        };

        terrainMesh.RecalculateNormals();
        mainMeshFilter.mesh = terrainMesh;
        #endregion

    }


    private void CreateSubMesh(string[,] biomMap) {

        #region Count Mesh Vertices:

        int vc = 0; //Vertice Counter

        for (int x = 0; x < biomMap.GetLength(0); x++) {
            for (int y = 0; y < biomMap.GetLength(1); y++) {
                if (!biomMap[x, y].Equals("")) {
                    vc += 48;
                }
            }
        }
        #endregion


        #region Create vertices:
        Vector3[] vertices = new Vector3[vc];
        int[] triangles = new int[vc];
        Color[] colors = new Color[vc];


        int vi = 0;
        Color colorBuffer;

        for (int x = 0; x < biomMap.GetLength(0); x++) {
            for (int y = 0; y < biomMap.GetLength(1); y++) {
                if (!biomMap[x, y].Equals("")) {

                    colorBuffer = Color.yellow;

                    if (y == 0 || biomMap[x, y - 1].Equals("")) {
                        //Sub Quad (Lower):
                        CreateTriangleAndBackface(ref vertices, ref triangles, ref colors, new Vector3(x - shoreAngle, shoreDepth, y - shoreAngle), new Vector3(x, 0, y), new Vector3(x + 1, 0, y), ref vi, colorBuffer);
                        CreateTriangleAndBackface(ref vertices, ref triangles, ref colors, new Vector3(x + 1f, 0, y), new Vector3(x + 1 + shoreAngle, shoreDepth, y - shoreAngle), new Vector3(x - shoreAngle, shoreDepth, y - shoreAngle), ref vi, colorBuffer);
                    }

                    if (x == 0 || biomMap[x - 1, y].Equals("")) {
                        //Sub Quad: (Left)
                        CreateTriangleAndBackface(ref vertices, ref triangles, ref colors, new Vector3(x, 0, y + 1), new Vector3(x, 0, y), new Vector3(x - shoreAngle, shoreDepth, y - shoreAngle), ref vi, colorBuffer);
                        CreateTriangleAndBackface(ref vertices, ref triangles, ref colors, new Vector3(x - shoreAngle, shoreDepth, y - shoreAngle), new Vector3(x - shoreAngle, shoreDepth, y + 1 + shoreAngle), new Vector3(x, 0, y + 1), ref vi, colorBuffer);
                    }

                    if (y == biomMap.GetLength(1) - 1 || biomMap[x, y + 1].Equals("")) {
                        //Sub Quad: (Top)
                        CreateTriangleAndBackface(ref vertices, ref triangles, ref colors, new Vector3(x + 1 + shoreAngle, shoreDepth, y + 1 + shoreAngle), new Vector3(x + 1, 0, y + 1), new Vector3(x, 0, y + 1), ref vi, colorBuffer);
                        CreateTriangleAndBackface(ref vertices, ref triangles, ref colors, new Vector3(x - shoreAngle, shoreDepth, y + 1 + shoreAngle), new Vector3(x, 0, y + 1), new Vector3(x + 1 + shoreAngle, shoreDepth, y + 1 + shoreAngle), ref vi, colorBuffer);
                    }

                    if (x == biomMap.GetLength(0) - 1 || biomMap[x + 1, y].Equals("")) {
                        //Sub Quad: (Right)
                        CreateTriangleAndBackface(ref vertices, ref triangles, ref colors, new Vector3(x + 1, 0, y + 1), new Vector3(x + 1, 0, y), new Vector3(x + 1 + shoreAngle, shoreDepth, y - shoreAngle), ref vi, colorBuffer);
                        CreateTriangleAndBackface(ref vertices, ref triangles, ref colors, new Vector3(x + 1 + shoreAngle, shoreDepth, y - shoreAngle), new Vector3(x + 1 + shoreAngle, shoreDepth, y + 1 + shoreAngle), new Vector3(x + 1, 0, y + 1), ref vi, colorBuffer);
                    }
                }
            }
        }
        #endregion


        #region Create mesh:

        Mesh mesh = new Mesh {
            vertices = vertices,
            triangles = triangles,
            colors = colors
        };

        mesh.RecalculateNormals();
        subMeshFilter.mesh = mesh;
        #endregion
    }


    private void CreateTriangle(ref Vector3[] vertices, ref int[] triangles, Vector3 vertexPosition1, Vector3 vertexPosition2, Vector3 vertexPosition3, ref int vi) {
        vertices[vi] = vertexPosition1;
        triangles[vi] = vi;
        vi++;

        vertices[vi] = vertexPosition2;
        triangles[vi] = vi;
        vi++;

        vertices[vi] = vertexPosition3;
        triangles[vi] = vi;
        vi++;
    }


    private void CreateTriangleAndBackface(ref Vector3[] vertices, ref int[] triangles, ref Color[] colors, Vector3 vertexPosition1, Vector3 vertexPosition2, Vector3 vertexPosition3, ref int vi, Color vertexColor) {
        vertices[vi] = vertexPosition1;
        triangles[vi] = vi;
        colors[vi] = vertexColor;
        vi++;

        vertices[vi] = vertexPosition2;
        triangles[vi] = vi;
        colors[vi] = vertexColor;
        vi++;

        vertices[vi] = vertexPosition3;
        triangles[vi] = vi;
        colors[vi] = vertexColor;
        vi++;

        vertices[vi] = vertexPosition3;
        triangles[vi] = vi;
        colors[vi] = vertexColor;
        vi++;

        vertices[vi] = vertexPosition2;
        triangles[vi] = vi;
        colors[vi] = vertexColor;
        vi++;

        vertices[vi] = vertexPosition1;
        triangles[vi] = vi;
        colors[vi] = vertexColor;
        vi++;
    }


    private Color[] ConvertToMeshColorFormat(string[,] biomMap, Color[,] colorMap, int vc) {
        Color[] ret = new Color[vc];
        int vi = 0;

        for (int x = 0; x < colorMap.GetLength(0); x++) {
            for (int y = 0; y < colorMap.GetLength(1); y++) {
                if (!biomMap[x, y].Equals("")) {
                    for (int i = 0; i < 24; i++) {
                        ret[vi] = colorMap[x, y];
                        vi++;
                    }
                }
            }
        }

        return ret;
    }


    public void RepaintColor(string[,] biomMap, Color[,] newColorMap) {
        terrainMesh.colors = ConvertToMeshColorFormat(biomMap, newColorMap, terrainMesh.vertexCount);
    }
}