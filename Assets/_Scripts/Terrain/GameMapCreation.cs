using UnityEngine;
using TMPro;

public class GameMapCreation : MonoBehaviour {
    //Stores all map data and controlles it

    [Header("Map Settings:")]
    public Vector2Int tileSize = new Vector2Int(80, 80);
    public int seed = 100;
    public Transform mapObjectsHolder;
    public TextMeshProUGUI seedText;

    //Main maps:
    public string[,] biomMap;
    public string[,] objectMap;
    public Transform[,] transformMap;

    private MapMeshGenerator mapMeshGenerator;
    private ObjectMapTranslation objectMapTranslation;
    private BiomManager biomManager;

    #region Cache:
    private float randomValue, randomStart;
    #endregion

    public static GameMapCreation instance;


    private void Start() {
        instance = this;

        biomManager = BiomManager.instance;

        System.Array.Sort(biomManager.bioms);

        mapMeshGenerator = GetComponent<MapMeshGenerator>();
        objectMapTranslation = GetComponent<ObjectMapTranslation>();

        CreateMap();
    }



    public void CreateMap() {

        #region Seed:
        //New seed:
        seed = Random.Range(0, 5000000);

        //Set seed text:
        if (seedText != null) {
            SetSeedText();
        }

        //Set seed:
        Random.InitState(seed);
        #endregion

        float[,] heightMap = CreateHeightMap();

        CreateMaps(ref heightMap, out Color[,] colorMap);

        mapMeshGenerator.CreateMapMesh(tileSize, biomMap, colorMap);
        mapMeshGenerator.CreateMapMeshCollider();

        CenterMap();

        DestroyMapContent();
        SpawnWorldObjects();
    }


    private float[,] CreateHeightMap() {
        return GetComponent<HeightMapCreatorV2>().CreateHeightMap(tileSize, seed);
    }


    private void CreateMaps(ref float[,] heightMap, out Color[,] colorMap) {
        //Create biom, object, transform and color maps:
        biomMap = new string[tileSize.x, tileSize.y];
        objectMap = new string[tileSize.x, tileSize.y];
        transformMap = new Transform[tileSize.x, tileSize.y];
        colorMap = new Color[tileSize.x, tileSize.y];
        bool foundBiom = false;

        for (int x = 0; x < biomMap.GetLength(0); x++) {
            for (int y = 0; y < biomMap.GetLength(1); y++) {
                if (heightMap[x, y] >= 0) {

                    #region Bioms:

                    foundBiom = false;

                    foreach (Biom biom in biomManager.bioms) {
                        if (!foundBiom || biom.overwrite) {
                            foreach (BiomOctave octave in biom.octaves) {
                                if (Mathf.PerlinNoise((float)x / tileSize.x * octave.scale + seed, (float)y / tileSize.y * octave.scale + seed) > octave.spawnChance) {
                                    
                                    objectMap[x, y] = GetBiomsObjectMap(biom);

                                    if (!biom.notUseColor) {
                                        biomMap[x, y] = biom.biomName;
                                        colorMap[x, y] = biom.groundColor;
                                    }

                                    foundBiom = true;
                                }
                            }
                        }
                    }

                    #endregion


                    #region Default biom fallback:

                    if (biomMap[x, y] == null) {

                        biomMap[x, y] = biomManager.defaultBiom.biomName;
                        objectMap[x, y] = "";

                        if (biomManager.defaultBiom.tileObjects.Length > 0) {
                            objectMap[x, y] = GetBiomsObjectMap(biomManager.defaultBiom);
                        }

                        colorMap[x, y] = biomManager.defaultBiom.groundColor;
                    }

                    #endregion

                } else {
                    //Water:
                    biomMap[x, y] = "";
                    objectMap[x, y] = "";
                }
            }
        }
    }


    private void DestroyMapContent() {
        foreach (Transform child in mapObjectsHolder) {
            Destroy(child.gameObject);
        }
    }


    private void SpawnWorldObjects() {
        for (int x = 0; x < objectMap.GetLength(0); x++) {
            for (int y = 0; y < objectMap.GetLength(1); y++) {
                SpawnTerrainObjectAt(x, y, objectMap[x, y]);
            }
        }
    }


    private void SpawnTerrainObjectAt(int x, int y, string newTerrainObjectName) {
        Transform tileTransformObject = objectMapTranslation.TranslateStringToTransform(newTerrainObjectName);

        if (tileTransformObject != null) {
            tileTransformObject = Instantiate(tileTransformObject, new Vector3(x - tileSize.x / 2, 0, y - tileSize.y / 2), Quaternion.identity);

            tileTransformObject.parent = mapObjectsHolder;
            objectMap[x, y] = newTerrainObjectName;
            transformMap[x, y] = tileTransformObject;
        }
    }


    private string GetBiomsObjectMap(Biom biom) {
        if (biom.tileObjects.Length == 1) {
            //Save performance to skip the random calc if there is only 1 tile object:
            return biom.tileObjects[0].tileObject.name;

        } else if (biom.tileObjects.Length > 1) {
            //Randomly choose tile object (if there are more than 1 tile object):
            randomValue = 0;

            foreach (TileObjectRandom tor in biom.tileObjects) {
                randomValue += tor.chance;
            }

            randomValue = Random.Range(0, randomValue);
            randomStart = 0;

            for (int i = 0; i < biom.tileObjects.Length; i++) {
                if (randomValue >= randomStart && randomValue < randomStart + biom.tileObjects[i].chance) {
                    if(biom.tileObjects[i].tileObject == null) {
                        return "";
                    }
                    return biom.tileObjects[i].tileObject.name;
                }
                randomStart += biom.tileObjects[i].chance;
            }
        }

        return "";
    }


    public void Save(string name) {
        SaveGameManager.SaveGame(new SaveData(biomMap, objectMap), "Map");
    }


    private Biom GetBiomFromName(string biomMap) {

        if (!biomMap.Equals("")) {
            if (biomMap.Equals(biomManager.defaultBiom.biomName)) {
                return biomManager.defaultBiom;
            }

            foreach (Biom biom in biomManager.bioms) {
                if (biom.biomName.Equals(biomMap)) {

                    if (biom.notUseColor) {
                        return biomManager.defaultBiom;
                    }
                    return biom;
                }
            }
        }

        return null;
    }


    private void CenterMap() {
        Vector3 replaceValue = transform.position;
        replaceValue.x = tileSize.x / 2;
        replaceValue.z = tileSize.y / 2;
        mapObjectsHolder.transform.position = transform.position - replaceValue;
    }


    private void SetSeedText() {
        seedText.text = "" + seed;
    }
}