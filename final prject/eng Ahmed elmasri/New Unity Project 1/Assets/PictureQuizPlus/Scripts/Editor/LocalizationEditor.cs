using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class LocalizedTextEditor : EditorWindow //Class for the editor, to make changes in the game data and save them into extended file
{
    public LocalizationData localizationData;
    private static string path;
    Vector2 scrollPos;
    private bool localizationDataIsReady = false;

    [MenuItem("Tools/PictureQuiz/Localized Text Editor", false, 2)]
    static void Init()
    {
        string path = Application.dataPath + "/StreamingAssets";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        EditorWindow.GetWindow(typeof(LocalizedTextEditor)).Show();
    }

    private void OnGUI() //Its like Update() method but for editor tab
    {

        path = Application.streamingAssetsPath;
        ReplaceEnglishJsonFromResources();

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        if (GUILayout.Button("Load data"))
        {
            LoadGameData();
        }

        if (GUILayout.Button("Create new data"))
        {
            SetUpTasksData();
            SetUpLocalizedItemsData();
        }
        if (localizationDataIsReady)
        {
            if (GUILayout.Button("Save data"))
            {
                SaveGameData();
            }
            SerializedObject serializedObject = new SerializedObject(this);
            SerializedProperty serializedProperty = serializedObject.FindProperty("localizationData");
            EditorGUILayout.PropertyField(serializedProperty, true);
            serializedObject.ApplyModifiedProperties();

        }

        EditorGUILayout.EndScrollView();
    }

    private static void ReplaceEnglishJsonFromResources()
    {
        if (!File.Exists(Path.Combine(path, "english.json")))
        {
            string oldPath = Path.Combine(Application.dataPath + "/PictureQuizPlus/Resources", "english.json");
            if (File.Exists(oldPath))
            {
                File.Move(oldPath, Path.Combine(path, "english.json"));
            }
        }
    }

    private void SetUpLocalizedItemsData() //With default international values
    {
        localizationData.gameItems = new LocalizationItem[] {
        new LocalizationItem(ElementType.play_button,"play"),
        new LocalizationItem(ElementType.info_button,"info"),
        new LocalizationItem(ElementType.quit_button,"quit"),
        new LocalizationItem(ElementType.back_button,"back"),
        new LocalizationItem(ElementType.info_text,"Picture Quiz 2018\nTry to quess what is on the picture!\nGet stucked? Spend your coins for hints!\nEarn coins by completing tasks or by watching short AD!\nHave fun!"),
        new LocalizationItem(ElementType.locked_text,"locked"),
        new LocalizationItem(ElementType.reveal_a_letter,"Reveal a letter"),
        new LocalizationItem(ElementType.remove_letters,"Remove letters"),
        new LocalizationItem(ElementType.get_an_answer,"Get an answer"),
        new LocalizationItem(ElementType.get_coins,"Get coins"),
        new LocalizationItem(ElementType.watch_ad_button,"Watch AD"),
        new LocalizationItem(ElementType.purchase_button,"Buy for 1$"),
        new LocalizationItem(ElementType.continue_button,"continue"),
        new LocalizationItem(ElementType.back_to_menu_button,"main menu"),
        new LocalizationItem(ElementType.level_complete_header,"Level complete!"),
        new LocalizationItem(ElementType.rate_text, "If you liked the game, please rate it on Google Play!"),
        new LocalizationItem(ElementType.rate, "rate"),
        new LocalizationItem(ElementType.later, "later"),
        new LocalizationItem(ElementType.never, "never"),
        new LocalizationItem(ElementType.gdpr_consent_button, "Data collection consent"),
        new LocalizationItem(ElementType.personalized_ads_button, "Personalized advertisements"),
        new LocalizationItem(ElementType.ads_dialog, "Allow AD networks too use collected data and show personalized advertisements"),
        new LocalizationItem(ElementType.yes, "yes"),
        new LocalizationItem(ElementType.no, "no")

        };

        localizationData.randomLetters = new char[] { 'y', 'b', 'k', 'w', 't', 'r', 'j', 'v', 'u', 'c', 'q', 'i', 'z', 'x', 'l', 'p' };

    }

    private void LoadGameData()//Loading localized data to json file
    {
        string filePath = EditorUtility.OpenFilePanel("Select localization data file", path, "json");

        if (!string.IsNullOrEmpty(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);

            localizationData = JsonUtility.FromJson<LocalizationData>(dataAsJson);
            StaticBehaviors.CheckAndWriteFilenames();
        }
        localizationDataIsReady = true;
    }

    private void SaveGameData() //Saving localized data to json file
    {
        string filePath = EditorUtility.SaveFilePanel("Save localization data file", path, "", "json");

        if (!string.IsNullOrEmpty(filePath))
        {
            string dataAsJson = JsonUtility.ToJson(localizationData, true);
            File.WriteAllText(filePath, dataAsJson);
            StaticBehaviors.CheckAndWriteFilenames();
        }
    }

    //All above are utility methods to manage filenames and catalogs
    private void SetUpTasksData()
    {
        localizationData = new LocalizationData();
        string path = Application.dataPath + "/PictureQuizPlus/Resources";
        string[] directoryNames = CutNames(Directory.GetDirectories(path));
        int[] filesInDirs = GetFilesCount(directoryNames, path);
        TasksToData(directoryNames, filesInDirs); //Fill localizationData with values
        localizationDataIsReady = true;
    }
    private string[] CutNames(string[] array)
    {
        string[] cuttedArr = new string[array.Length];
        for (int i = 0; i < array.Length; i++)
        {
            string[] temp = array[i].Split('\\');
            cuttedArr[i] = temp[temp.Length - 1];
        }
        return cuttedArr;
    }

    private int[] GetFilesCount(string[] directorynames, string path)
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

    private void TasksToData(string[] directorys, int[] files)
    {
        localizationData.tasksData = new TasksDictionary[directorys.Length];
        for (int i = 0; i < localizationData.tasksData.Length; i++)
        {
            localizationData.tasksData[i] = new TasksDictionary(directorys[i], files[i], GameType.Default, i);
        }
    }
}
