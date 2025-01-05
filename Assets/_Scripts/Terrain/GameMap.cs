using UnityEngine;

public class GameMap : MonoBehaviour {
    //Stores all map data and controlles it

    [Header("Map Settings:")]
    public Transform mapObjectsHolder;
    public BiomManager biomManager;

    //Main maps:
    public string[,] biomMap;
    public string[,] objectMap;
    public Transform[,] transformMap;
    
    [HideInInspector] public Vector2Int tileSize;

    private MapMeshGenerator mapMeshGenerator;
    private ObjectMapTranslation objectMapTranslation;

    public static GameMap instance;



    private void Awake() {
        instance = this;
    }



    private void Start() {
        mapMeshGenerator = GetComponent<MapMeshGenerator>();
        objectMapTranslation = GetComponent<ObjectMapTranslation>();

        LoadMap();
    }



    public void LoadMap() {
        SaveData sd = SaveGameManager.LoadGame("Map");
        biomMap = sd.biomMap;
        objectMap = sd.objectMap;

        tileSize = new Vector2Int(biomMap.GetLength(0), biomMap.GetLength(1));

        transformMap = new Transform[tileSize.x, tileSize.y];
        Color[,] colorMap = ColorMapFromBiomMap(biomMap);
        
        mapMeshGenerator.CreateMapMesh(tileSize, biomMap, colorMap);
        mapMeshGenerator.CreateMapMeshCollider();

        CenterMap();

        DestroyMapContent();
        SpawnWorldObjects();
    }
    


    public void DestroyMapContent() {
        foreach (Transform child in mapObjectsHolder) {
            Destroy(child.gameObject);
        }
    }


    public void SpawnWorldObjects() {
        for (int x = 0; x < objectMap.GetLength(0); x++) {
            for (int y = 0; y < objectMap.GetLength(1); y++) {
                SpawnTerrainObjectAt(x, y, objectMap[x, y]);
            }
        }
    }


    public void SpawnTerrainObjectAt(int x, int y, string newTerrainObjectName) {
        Transform tileTransformObject = objectMapTranslation.TranslateStringToTransform(newTerrainObjectName);

        if (tileTransformObject != null) {
            tileTransformObject = Instantiate(tileTransformObject, new Vector3(x - tileSize.x / 2, 0, y - tileSize.y / 2), Quaternion.identity);

            tileTransformObject.parent = mapObjectsHolder;
            objectMap[x, y] = newTerrainObjectName;
            transformMap[x, y] = tileTransformObject;
        }
    }


    public void DestroyTerrainObjectAt(int x, int y) {
        if (transformMap[x, y] != null) {
            Destroy(transformMap[x, y].gameObject);
            transformMap[x, y] = null;
        }

        objectMap[x, y] = "";
    }


    public void DestroyTerrainObjectAt(int x, int y, string newTerrainObjectName) {
        if (transformMap[x, y] != null) {
            Destroy(transformMap[x, y].gameObject);
            transformMap[x, y] = null;
        }

        objectMap[x, y] = newTerrainObjectName;
    }

    private Color[,] ColorMapFromBiomMap(string[,] biomMap) {
        Color[,] colorMap = new Color[tileSize.x, tileSize.x];
        Biom biomBuffer;
        for (int x = 0; x < biomMap.GetLength(0); x++) {
            for (int y = 0; y < biomMap.GetLength(1); y++) {
                if (biomMap[x, y] != "") {
                    biomBuffer = GetBiomFromName(biomMap[x, y]);
                    if (!biomBuffer.notUseColor) {
                        colorMap[x, y] = biomBuffer.groundColor;
                    }
                }
            }
        }

        return colorMap;
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


    public void Save(string name) {
        SaveGameManager.SaveGame(new SaveData(biomMap, objectMap), "Map");
    }


    public Vector2Int GetGridPosition(Vector3 position) {
        int x = (int)(position.x + tileSize.x / 2);
        int y = (int)(position.z + tileSize.y / 2);
        return new Vector2Int(x, y);
    }
}