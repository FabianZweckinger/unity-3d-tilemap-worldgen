using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveGameManager {

    const string FOLDER_NAME = "SaveGames";
    const string FILE_EXTENSION = ".dat";



    public static void SaveGame(SaveData data, string saveName) {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        using (FileStream fileStream = File.Open(saveName, FileMode.OpenOrCreate)) {
            binaryFormatter.Serialize(fileStream, data);
        }
    }


    public static SaveData LoadGame(string saveName) {
        BinaryFormatter binaryFormatter = new BinaryFormatter();

        using (FileStream fileStream = File.Open(saveName, FileMode.Open)) {
            return (SaveData)binaryFormatter.Deserialize(fileStream);
        }
    }
}