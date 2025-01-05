 using UnityEngine;

public class HeightMapCreatorV2 : MonoBehaviour {
    //Creates a heightmap for iland map generation:

    [Header("Heightmap Settings:")]
    [Range(-1000, 1000)] public int maxHeightValue = 1200;
    [Range(-1000, 1000)] public int minHeightValue = 400;
    [Range(-1000, 0)] public int borderValue = -400;
    [Range(0, 256)] public int smoothing = 16;
    [Range(0, .3f)] public float transfer = 0.05f;
    

    //Cache:
    private float tempTransData = 0;
    private int x = 0, y = 0, i = 0;



    public float[,] CreateHeightMap (Vector2Int mapSize, int seed) {

        float[,] heightMap = new float[mapSize.x, mapSize.y];

        #region Init heigthmap with random values and border values:

        for (y = 0; y < heightMap.GetLength(1); y++) {
            for (x = 0; x < heightMap.GetLength(0); x++) {
                if (y == 0 || y == heightMap.GetLength(1) - 1) {
                    heightMap[x, y] = borderValue;
                } else if (x == 0 || x == heightMap.GetLength(0) - 1) {
                    heightMap[x, y] = borderValue;
                } else {
                    heightMap[x, y] = Random.Range(minHeightValue, maxHeightValue);
                }
            }
        }
        #endregion

        #region Smooth x times:

        for (int i = 0; i < smoothing; i++) {
            Smooth(ref heightMap);
        }
        #endregion

        return heightMap;
    }


    private void Smooth(ref float[,] heightMap) {
        for (y = 0; y < heightMap.GetLength(1); y++) {
            for (x = 0; x < heightMap.GetLength(0); x++) {
                tempTransData = transfer * heightMap[x, y];
                i = 0;


                if (x != 0 && y != heightMap.GetLength(1) - 1) {
                    heightMap[x - 1, y + 1] = heightMap[x - 1, y + 1] + tempTransData;
                    i++;
                }

                if (x != 0 && y != heightMap.GetLength(1) - 1) {
                    heightMap[x - 1, y] = heightMap[x - 1, y] + tempTransData;
                    i++;
                }

                if (x != 0 && y != 0) {
                    heightMap[x - 1, y - 1] = heightMap[x - 1, y - 1] + tempTransData;
                    i++;
                }

                if (x != 0 && y != heightMap.GetLength(1) - 1) {
                    heightMap[x, y + 1] = heightMap[x, y + 1] + tempTransData;
                    i++;
                }

                if (x != 0 && y != 0) {
                    heightMap[x, y - 1] = heightMap[x, y - 1] + tempTransData;
                    i++;
                }

                if (x != heightMap.GetLength(0) - 1 && y != heightMap.GetLength(1) - 1) {
                    heightMap[x + 1, y + 1] = heightMap[x + 1, y + 1] + tempTransData;
                    i++;
                }

                if (x != heightMap.GetLength(0) - 1 && y != heightMap.GetLength(1) - 1) {
                    heightMap[x + 1, y] = heightMap[x + 1, y] + tempTransData;
                    i++;
                }

                if (x != heightMap.GetLength(0) - 1 && y != 0) {
                    heightMap[x + 1, y - 1] = heightMap[x + 1, y - 1] + tempTransData;
                    i++;
                }

                heightMap[x, y] -= transfer * heightMap[x, y] * i;
            }
        }
    }
}