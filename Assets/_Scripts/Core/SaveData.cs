[System.Serializable]
public class SaveData {

    public string[,] biomMap;
    public string[,] objectMap;


    public SaveData(string[,] biomMap, string[,] objectMap) {
        this.biomMap = biomMap;
        this.objectMap = objectMap;
    }
}
