using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class Welcome : EditorWindow //Welcome screen
{
    private const float width = 500;
    private const float height = 200;
    private const string connectUrl = "https://connect.unity.com/u/5a8539f032b30600171a79c2";
    private const string ShowAtStartUP = "ShowAtStartUP";
    private static bool showAtStartup;
    private static bool interfaceInitialized;
    private static Texture buttonIcon;
    private static Welcome inst;
    private static DataManager inst2;

    [MenuItem("Tools/PictureQuiz/Welcome Screen")]
    public static void OpenWelcomeWindowFromEditor()
    {
        GetWindow<Welcome>(true);
    }

    public static void OpenWelcomeWindow()
    {
        showAtStartup = EditorPrefs.GetInt(ShowAtStartUP, 1) == 1;
        if (showAtStartup)
        {
            GetWindow<Welcome>(true);
        }
        EditorPrefs.SetInt(ShowAtStartUP, 0);
        EditorApplication.update -= OpenWelcomeWindow;
    }

    private static void CheckLanguages()
    {
        EditorApplication.update -= CheckLanguages;
        StaticBehaviors.CheckAndWriteFilenames();
    }
    static Welcome()
    {
        EditorApplication.update += OpenWelcomeWindow;
        //EditorApplication.update += CheckLanguages;
    }
    
    void OnEnable()
    {
        maxSize = new Vector2(width, height);
        minSize = maxSize;
    }

    public void OnGUI()
    {
        InitInterface();
        GUIStyle myStyle = new GUIStyle();
        myStyle.fontSize = 15;
        myStyle.fontStyle = FontStyle.Bold;
        myStyle.alignment = TextAnchor.MiddleCenter;
        GUI.Box(new Rect(0, 0, width, 60), "PICTURE QUIZ PLUS 2018 \n Pay attention to the Tools tab", myStyle);
        //		GUI.Label( new Rect(width-90,45,200,20),new GUIContent("Version : " +VERSION));
        GUILayoutUtility.GetRect(position.width, 64);
        GUILayout.Space(20);
        GUILayout.BeginVertical();

        if (Button(buttonIcon, "A QUESTION? A SUGGESTION? A REQUEST?", "Don't hesitate to contact us!"))
        {
            Application.OpenURL(connectUrl);
        }
        if (Button(buttonIcon, "DONT FORGET TO CHECK HOWTO GUIDE ", "To have a better start with the asset"))
        {
            string[] guids = AssetDatabase.FindAssets("HowTo-Guide");
            Selection.activeObject = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guids[0]), typeof(UnityEngine.Object));
        }

        GUILayout.EndVertical();

    }

    void InitInterface()
    {

        if (!interfaceInitialized)
        {
            buttonIcon = (Texture)Resources.Load("arrow") as Texture;
            interfaceInitialized = true;
        }
    }

    bool Button(Texture texture, string heading, string body, int space = 10)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Space(24);
        GUILayout.Box(texture, GUIStyle.none, GUILayout.MaxWidth(48), GUILayout.MaxHeight(30));
        GUILayout.Space(10);
        GUILayout.BeginVertical();
        GUILayout.Space(1);
        GUILayout.Label(heading, EditorStyles.boldLabel);
        GUILayout.Label(body);
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();

        var rect = GUILayoutUtility.GetLastRect();
        EditorGUIUtility.AddCursorRect(rect, MouseCursor.Link);

        bool returnValue = false;
        if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
        {
            returnValue = true;
        }

        GUILayout.Space(space);

        return returnValue;
    }
}
