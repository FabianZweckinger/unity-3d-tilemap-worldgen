using UnityEngine;

public class ObjectMapTranslation : MonoBehaviour {
    
    private Transform[] tileTransformObjects;
    private NatureTile[] natureTiles;
    private string[] tileObjectNames;

    public static ObjectMapTranslation instance;



    private void Awake() {
        instance = this;

        tileTransformObjects = Resources.LoadAll<Transform>("Tiles/");

        //Create worldObjectsNames array:
        tileObjectNames = new string[tileTransformObjects.Length];
        int tileObjectsCounter = 0;

        for (int i = 0; i < tileTransformObjects.Length; i++) {
            tileObjectNames[i] = tileTransformObjects[i].name;

            if (tileTransformObjects[i].GetComponent<NatureTile>()) {
                tileObjectsCounter++;
            }
        }

        //Create tile object array:
        natureTiles = new NatureTile[tileObjectsCounter];
        int c = 0;

        for (int i = 0; i < tileTransformObjects.Length; i++) {
            if (tileTransformObjects[i].GetComponent<NatureTile>()) {
                natureTiles[c] = tileTransformObjects[i].GetComponent<NatureTile>();
                c++;
            }
        }
    }


    public Transform TranslateStringToTransform(string name) {

        for (int i = 0; i < tileObjectNames.Length; i++) {
            if (name.Equals(tileObjectNames[i])) {
                return tileTransformObjects[i];
            }
        }

        return null;
    }


    public NatureTile[] GetTileObjects() {
        return natureTiles;
    }
}
