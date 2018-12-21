using System.IO;
using UnityEngine;
using UnityEditor;

//Extensions for editor. Look at Tools tab
public class EditorExtension : MonoBehaviour
{
    [MenuItem("Tools/PictureQuiz/Clear Saved Data", false, 1)]
    private static void ClearSavedData()
    {
        string path = System.IO.Path.Combine(Application.persistentDataPath, "saves.json");
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        PlayerPrefs.DeleteAll();
    }

    [MenuItem("Tools/PictureQuiz/Open ADs&IAP Settings", false, 0)]

    public static void Autoselect()
    {
        string[] guids = AssetDatabase.FindAssets("AdsSettings");
        Selection.activeObject = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guids[0]), typeof(AdsIapSettings));
    }
    [MenuItem("Tools/PictureQuiz/HOW TO")]

    public static void AutoselectInfo()
    {
        string[] guids = AssetDatabase.FindAssets("HowTo-Guide");
        Selection.activeObject = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guids[0]), typeof(Object));
    }
    [MenuItem("Assets/Create/AdsSettings")]
    public static void CreateAdIds()
    {
        CreateAsset<AdsIapSettings>("AdsSettings");
    }

    public static void CreateAsset<T>(string name) where T : ScriptableObject
    {
        var asset = ScriptableObject.CreateInstance<T>();
        ProjectWindowUtil.CreateAsset(asset, name + ".asset");
    }



    private static int[] GetFilesCount(string[] directorynames, string path)
    {
        int[] levelsInfo = new int[directorynames.Length];
        string[] fileExtensions = { "*.jpg", "*.jpeg", "*.png" };
        for (int i = 0; i < directorynames.Length; i++)
        {
            int filesCount = 0;
            foreach (var item in fileExtensions)
            {
                filesCount += Directory.GetFiles(path + "\u002F" + directorynames[i], item).Length;
            }
            levelsInfo[i] = filesCount;
        }
        return levelsInfo;
    }

    private static void ProvideToGameManager(string[] directorys, int[] files)
    {
        DataManager sceneInstance = GameObject.FindObjectOfType<DataManager>() ??
        new GameObject("DataManager").AddComponent<DataManager>();
        sceneInstance.levelsData = new TasksDictionary[directorys.Length];
        for (int i = 0; i < sceneInstance.levelsData.Length; i++)
        {
            sceneInstance.levelsData[i] = new TasksDictionary(directorys[i], files[i], sceneInstance.defaultGameType, i);
        }
    }

    private static string[] CutNames(string[] array)
    {
        string[] cuttedArr = new string[array.Length];
        for (int i = 0; i < array.Length; i++)
        {
            string[] temp = array[i].Split(char.Parse("\u005C"));
            cuttedArr[i] = temp[temp.Length - 1];
        }
        return cuttedArr;
    }

}
