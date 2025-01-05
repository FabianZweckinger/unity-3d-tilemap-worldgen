using UnityEngine;

public class BiomManager : MonoBehaviour {

    public Biom defaultBiom;
    public Biom[] bioms;

    public static BiomManager instance;



    private void Awake() {
        instance = this;
    }
}
