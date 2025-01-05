[System.Serializable]
public class BiomOctave {
    public float spawnChance = .5f;
    public float scale = 5;

    public BiomOctave(float spawnChance, float scale) {
        this.spawnChance = spawnChance;
        this.scale = scale;
    }
}