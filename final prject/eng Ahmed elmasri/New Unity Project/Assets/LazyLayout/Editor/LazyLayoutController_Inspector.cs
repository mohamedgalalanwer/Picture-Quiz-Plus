using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[InitializeOnLoad]
[CustomEditor(typeof(LazyLayoutController), true)]
public class LazyLayoutController_Inspector : Editor
{
    LazyLayoutController LLC;

    [SerializeField]
    public RectTransform[] rectTransformsInScene;


    GameObject canvas;

    EditorWindow mainGameView;

    

    float currentSizeInInches, gameView_Width, gameView_Height, currentMinSizeForLayout,
          customAR_Length = 1, customAR_Heigth;

    string[] dumpLines = new string[0];

    bool showRemLLWarning;


    bool choosed_16_9_Landscape, choosed_16_9_Portrait, choosed_4_3_Landscape, choosed_4_3_Portrait,
         choosed_3_2_Landscape, choosed_3_2_Portrait,
         choosed_Other, choosed_Custom;

    static LazyLayoutController_Inspector()
    {
        EditorApplication.update += CheckForDuplicate;
    }

    static void CheckForDuplicate()
    {
        Event e = Event.current;
        if (e != null)
        {
            Debug.Log(e.commandName);
            if (e.commandName == "Duplicate")
            {
                Debug.Log("<ksdjfyksndf");
            }
        }

        if(Input.GetKey(KeyCode.LeftCommand) && Input.GetKeyDown(KeyCode.D))
        {
            Debug.Log("k.sjdak.sfdh");
        }
    }

    //void OnSceneGUI()
    //{
    //    Event e = Event.current;
    //    if (e != null)
    //    {
    //        Debug.Log(e.commandName);
    //        if (e.commandName == "Duplicate")
    //        {
    //            Debug.Log("<ksdjfyksndf");
    //        }
    //    }
    //}

    public void OnEnable()
    {
        LLC = target as LazyLayoutController;
        mainGameView = GetMainGameView();

        int layoutCount = LLC.savedLazyLayouts.Count;
        if (layoutCount > 0)
        {
            for (int i = 0; i < layoutCount; i++)
            {
                if (i <= LLC.savedLazyLayouts.Count - 1)
                {
                    if (i == LLC.index_CurrentlySelectedLayout - 1)
                    {
                        choosed_16_9_Landscape = LLC.savedLazyLayouts[i].capableOf_16_9_Landscape;
                        choosed_16_9_Portrait = LLC.savedLazyLayouts[i].capableOf_16_9_Portrait;
                        choosed_4_3_Landscape = LLC.savedLazyLayouts[i].capableOf_4_3_Landscape;
                        choosed_4_3_Portrait = LLC.savedLazyLayouts[i].capableOf_4_3_Portrait;
                        choosed_3_2_Landscape = LLC.savedLazyLayouts[i].capableOf_3_2_Landscape;
                        choosed_3_2_Portrait = LLC.savedLazyLayouts[i].capableOf_3_2_Portrait;
                        choosed_Other = LLC.savedLazyLayouts[i].capableOf_Other;
                        choosed_Custom = LLC.savedLazyLayouts[i].capableOf_Custom;

                        customAR_Length = LLC.savedLazyLayouts[i].customAR_Length;
                        customAR_Heigth = LLC.savedLazyLayouts[i].customAR_Height;

                        LLC.layoutName = LLC.savedLazyLayouts[i].layoutName;
                        currentMinSizeForLayout = LLC.savedLazyLayouts[i].minDeviceSizeForLayout;
                    }
                }
            }
        }
    }

    void OnInspectorUpdate()
    { this.Repaint(); }

    public override void OnInspectorGUI()
    {
        GUILayout.Space(10.0F);

        DisplayCurrentGameViewSize();
        GUILayout.Space(30.0F);

        DisplayCanvasField();
        GUILayout.Space(30.0F);

        DisplayCurrentlySelectedLayout();
        GUILayout.Space(10.0F);

        DisplaySavedLayouts();
        GUILayout.Space(40.0F);

        DisplayNewLayoutOption();
        GUILayout.Space(20.0F);          

        DisplayLoadInAwake();
        GUILayout.Space(5.0F);

        DisplayAdaptLayoutsAtRuntime();
        GUILayout.Space(5.0F);

        DisplaySwitchTextContent();
        GUILayout.Space(25.0F);

        DisplayRemoveLazyLayout();


        mainGameView = GetMainGameView();

        SaveAllValues();
    }

    


        // _-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_- //
        void DisplayCanvasField()
    {
        if (LLC.canvasInScene)
        {
            GUI.color = Color.green;
            GUILayout.Label("(OK) Canvas in Scene: ", EditorStyles.boldLabel);
            GUI.color = Color.white;
        }
        else
        {
            GUI.color = Color.red;
            GUILayout.Label("(WARNING) -ASSIGN CANVAS OBJECT FIRST-", EditorStyles.boldLabel);
            GUI.color = Color.white;
        }

        canvas = EditorGUILayout.ObjectField(LLC.canvasInScene, typeof(GameObject), true) as GameObject;

    }

    void DisplayCurrentGameViewSize()
    {
        GUILayout.Label("GameView Info", EditorStyles.boldLabel);
        if (mainGameView)
        {
            gameView_Width = mainGameView.position.width;
            gameView_Height = mainGameView.position.height;

            float dp = Mathf.Sqrt((gameView_Width * gameView_Width) + (gameView_Height * gameView_Height));
            currentSizeInInches = dp / Screen.dpi;

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Current Size in Inches:", EditorStyles.miniBoldLabel, GUILayout.Width(130.0F));
            GUILayout.Label(currentSizeInInches.ToString(), EditorStyles.miniBoldLabel);
            EditorGUILayout.EndHorizontal();

            if (gameView_Width > gameView_Height)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Orientation: ", EditorStyles.miniBoldLabel, GUILayout.Width(130.0F));
                GUILayout.Label("Landscape", EditorStyles.miniBoldLabel);
                EditorGUILayout.EndHorizontal();
                this.Repaint();
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Orientation: ", EditorStyles.miniBoldLabel, GUILayout.Width(130.0F));
                GUILayout.Label("Portrait", EditorStyles.miniBoldLabel);
                EditorGUILayout.EndHorizontal();
                this.Repaint();
            }
        }
        else
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Game-View is currently closed", EditorStyles.miniBoldLabel, GUILayout.Width(180.0F));
            EditorGUILayout.EndHorizontal();
        }

    }

    void DisplayCurrentlySelectedLayout()
    {
        EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Currently selected Layout: ", EditorStyles.boldLabel);

            GUI.color = Color.cyan;
            if (LLC.savedLazyLayouts.Count > 0)
            {
                if (LLC.savedLazyLayouts.Count >= LLC.index_CurrentlySelectedLayout)
                {
                    EditorGUILayout.LabelField(LLC.savedLazyLayouts[LLC.index_CurrentlySelectedLayout - 1].layoutName);
                }
                else
                {
                    EditorGUILayout.LabelField("None. Click 'change to'", EditorStyles.miniLabel);
                }
            }
            else
            {
                EditorGUILayout.LabelField("No saved layouts", EditorStyles.miniLabel);
            }
            
        EditorGUILayout.EndHorizontal();
        GUI.color = Color.white;
    }

    void DisplaySavedLayouts()
    {
        LLC.showSavedLayouts = EditorGUILayout.Foldout(LLC.showSavedLayouts, "SAVED LAYOUTS");
        if (LLC.showSavedLayouts)
        {
            EditorGUILayout.BeginVertical();

            int layoutCount = LLC.savedLazyLayouts.Count;
            if (layoutCount > 0)
            {
                for (int i = 0; i < layoutCount; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Layout-Name: ", GUILayout.Width(90.0F));
                    if (i <= LLC.savedLazyLayouts.Count - 1)
                    {
                        EditorGUILayout.LabelField(LLC.savedLazyLayouts[i].layoutName, GUILayout.Width(100.0F));
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    if (i <= LLC.savedLazyLayouts.Count - 1)
                    {
                        DisplayDetails(i);
                        DisplayButtons(i);
                    }

                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                }
            }
            else
            {
                EditorGUILayout.LabelField("No layouts have been saved yet");
            }

            EditorGUILayout.EndVertical();
        }
        
    }

    void DisplayButtons(int _i)
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(150.0F));
        GUI.color = Color.green;
        if (GUILayout.Button("Change to", GUILayout.Width(100.0F)))
        {
            ApplySavedLayout(_i);
        }
        if (_i == LLC.index_CurrentlySelectedLayout - 1)
        {
            GUI.color = Color.cyan;
            if (GUILayout.Button("update", GUILayout.Width(60.0F)))
            {
                UpdateLayout(_i);
            }

        }
        else
        {
            GUI.color = Color.grey;
            if (GUILayout.Button("update", GUILayout.Width(60.0F)))
            {
                //do nothing
            }
        }
        GUI.color = Color.red;
        if (GUILayout.Button("del", GUILayout.Width(40.0F)))
        {
            DeleteSingleLayout(_i);
        }
        GUI.color = Color.white;
        EditorGUILayout.EndVertical();
    }

    void DisplayDetails(int _i)
    {
        EditorGUILayout.BeginVertical();

        EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("", GUILayout.Width(10.0F));
            if (LLC.savedLazyLayouts[_i].minDeviceSizeForLayout != 0)
            { EditorGUILayout.LabelField("Min device size: " + LLC.savedLazyLayouts[_i].minDeviceSizeForLayout + " Inch", EditorStyles.miniBoldLabel, GUILayout.Width(200.0F)); }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("", GUILayout.Width(10.0F));
            EditorGUILayout.LabelField("=>" ,EditorStyles.miniBoldLabel, GUILayout.Width(20.0F));
            if (LLC.savedLazyLayouts[_i] != null)
            {
                if (LLC.savedLazyLayouts[_i].capableOf_16_9_Landscape)
                { EditorGUILayout.LabelField("16:9_L", EditorStyles.miniBoldLabel, GUILayout.Width(45.0F)); }
                if (LLC.savedLazyLayouts[_i].capableOf_16_9_Portrait)
                { EditorGUILayout.LabelField("16:9_P", EditorStyles.miniBoldLabel, GUILayout.Width(45.0F)); }
                if (LLC.savedLazyLayouts[_i].capableOf_4_3_Landscape)
                { EditorGUILayout.LabelField("4:3_L", EditorStyles.miniBoldLabel, GUILayout.Width(45.0F)); }
                if (LLC.savedLazyLayouts[_i].capableOf_4_3_Portrait)
                { EditorGUILayout.LabelField("4:3_P", EditorStyles.miniBoldLabel, GUILayout.Width(45.0F)); }
                if (LLC.savedLazyLayouts[_i].capableOf_3_2_Landscape)
                { EditorGUILayout.LabelField("3:2_L", EditorStyles.miniBoldLabel, GUILayout.Width(45.0F)); }
                if (LLC.savedLazyLayouts[_i].capableOf_3_2_Portrait)
                { EditorGUILayout.LabelField("3:2_P", EditorStyles.miniBoldLabel, GUILayout.Width(45.0F)); }
                if (LLC.savedLazyLayouts[_i].capableOf_Other)
                { EditorGUILayout.LabelField("Other", EditorStyles.miniBoldLabel, GUILayout.Width(65.0F)); }
                if (LLC.savedLazyLayouts[_i].capableOf_Custom)
                { EditorGUILayout.LabelField("Custom " + LLC.savedLazyLayouts[_i].customAR_Length + ":" + LLC.savedLazyLayouts[_i].customAR_Height, EditorStyles.miniLabel, GUILayout.Width(65.0F)); }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUI.color = Color.grey;
        EditorGUILayout.LabelField("", GUILayout.Width(15.0F));
        if (LLC.savedLazyLayouts[_i] != null)
        {
            if (LLC.savedLazyLayouts[_i].savedRectTransformsInScene.Count != 0)
            { EditorGUILayout.LabelField("Saved Rect-Transforms: " + LLC.savedLazyLayouts[_i].savedRectTransformsInScene.Count, EditorStyles.miniLabel, GUILayout.Width(180.0F)); }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("", GUILayout.Width(15.0F));
        int imageCount = 0;
        if (LLC.savedLazyLayouts[_i] != null)
        {
            for (int j = 0; j < LLC.savedLazyLayouts[_i].savedRectTransformsInScene.Count; j++)
            {
                if (LLC.savedLazyLayouts[_i] != null)
                { imageCount += LLC.savedLazyLayouts[_i].savedRectTransformsInScene[j].imageComponentsHelper.Count; }
            }
        }
        EditorGUILayout.LabelField("Saved Images: " + imageCount, EditorStyles.miniLabel, GUILayout.Width(180.0F));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("", GUILayout.Width(15.0F));
        int textCount = 0;
        if (LLC.savedLazyLayouts[_i] != null)
        {
            for (int j = 0; j < LLC.savedLazyLayouts[_i].savedRectTransformsInScene.Count; j++)
            {
                if (LLC.savedLazyLayouts[_i] != null)
                { textCount += LLC.savedLazyLayouts[_i].savedRectTransformsInScene[j].textComponentsHelper.Count; }
            }
        }
        EditorGUILayout.LabelField("Saved Texts: " + textCount, EditorStyles.miniLabel, GUILayout.Width(180.0F));
        GUI.color = Color.white;
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();

    }

    void DisplayNewLayoutOption()
    {
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.BeginVertical(GUILayout.Width(250.0F));
        GUILayout.Label("SAVE NEW LAYOUT", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        if (LayoutNameIsOkay(LLC.layoutName) == 0)
        {
            GUILayout.Label("Layout Name");
            LLC.layoutName = EditorGUILayout.TextArea(LLC.layoutName, GUILayout.Width(250.0F));
        }
        else if (LayoutNameIsOkay(LLC.layoutName) == 1)
        {
            GUI.color = Color.yellow;
            GUILayout.Label("Layout Name - Name can not be empty");
            GUI.color = Color.red;
            LLC.layoutName = EditorGUILayout.TextArea(LLC.layoutName, GUILayout.Width(250.0F));
            GUI.color = Color.white;
        }
        else
        {
            GUI.color = Color.yellow;
            GUILayout.Label("Layout Name - Name already exists");
            GUI.color = Color.red;
            LLC.layoutName = EditorGUILayout.TextArea(LLC.layoutName, GUILayout.Width(250.0F));
            GUI.color = Color.white;
        }
        
        DisplaySizeInput();
        DisplayAspectRatioChooser();
        DisplayCustomARChooser();

        EditorGUILayout.Space();
        if (LayoutNameIsOkay(LLC.layoutName) == 0 && LLC.canvasInScene && minDeviceSizeIsOkay(currentMinSizeForLayout) == 0 && choosenARIsOkay())
        {
            if (GUILayout.Button("Save current layout", GUILayout.Width(150.0F), GUILayout.Height(90.0F)))
            {
                SaveCurrentLayout(currentMinSizeForLayout);
            }
        }

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
    }

    void DisplayLoadInAwake()
    {
        EditorGUILayout.BeginHorizontal(GUILayout.Width(120.0F));
            LLC.loadLayoutInAwake = EditorGUILayout.Toggle(LLC.loadLayoutInAwake);
            GUILayout.Label("Load LazyLayout in Awake");
        EditorGUILayout.EndHorizontal();

    }

    void DisplayAdaptLayoutsAtRuntime()
    {
        EditorGUILayout.BeginHorizontal(GUILayout.Width(120.0F));
        LLC.adaptLayoutsInRuntime = EditorGUILayout.Toggle(LLC.adaptLayoutsInRuntime);
        GUILayout.Label("Adapt layouts at runtime");
        EditorGUILayout.EndHorizontal();

    }

    void DisplaySwitchTextContent()
    {
        EditorGUILayout.BeginHorizontal(GUILayout.Width(120.0F));
        LLC.switchTextContent = EditorGUILayout.Toggle(LLC.switchTextContent);
        GUILayout.Label("Switch content of text fields");
        EditorGUILayout.EndHorizontal();

    }

    void DisplayRemoveLazyLayout()
    {
        GUI.color = Color.black;
        EditorGUILayout.LabelField("Remove LazyLayout in this scene", EditorStyles.boldLabel);
        GUI.color = Color.grey;
        if (!showRemLLWarning)
        {
            if (GUILayout.Button("Remove LazyLayout", GUILayout.Width(200.0F)))
            {
                showRemLLWarning = true;
            }
        }
        else
        {
            ShowRemoveLazyLayoutDialogue();
        }

    }

    void DisplaySizeInput()
    {
        if (minDeviceSizeIsOkay(currentMinSizeForLayout) == 0)
        {
            GUILayout.Label("Min device size for this layout");
            currentMinSizeForLayout = EditorGUILayout.FloatField(currentMinSizeForLayout, GUILayout.Width(250.0F));
        }
        else if (minDeviceSizeIsOkay(currentMinSizeForLayout) == 1)
        {
            GUI.color = Color.yellow;
            GUILayout.Label("Min device size cannot be zero");
            GUI.color = Color.red;
            currentMinSizeForLayout = EditorGUILayout.FloatField(currentMinSizeForLayout, GUILayout.Width(250.0F));
            GUI.color = Color.white;
        }
        else
        {
            GUI.color = Color.yellow;
            GUILayout.Label("Combination with device size and aspect ratio already exists");
            GUI.color = Color.red;
            currentMinSizeForLayout = EditorGUILayout.FloatField(currentMinSizeForLayout, GUILayout.Width(250.0F));
            GUI.color = Color.white;
        }

    }

    void DisplayAspectRatioChooser()
    {
        if (choosenARIsOkay())
        { GUILayout.Label("Aspect Ratio for this layout"); }
        else
        {
            GUI.color = Color.yellow;
            GUILayout.Label("You have to choose an aspect ratio!");
            GUI.color = Color.white;
        }
        EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();

                EditorGUILayout.BeginHorizontal(GUILayout.Width(110.0F));
                    choosed_16_9_Landscape = EditorGUILayout.Toggle(choosed_16_9_Landscape);
                    GUILayout.Label("16:9 Landscape");                    
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal(GUILayout.Width(110.0F));
                    choosed_16_9_Portrait = EditorGUILayout.Toggle(choosed_16_9_Portrait);
                    GUILayout.Label("16:9 Portrait");                
                EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();

                EditorGUILayout.BeginHorizontal(GUILayout.Width(70.0F));
                    choosed_4_3_Landscape = EditorGUILayout.Toggle(choosed_4_3_Landscape);
                    GUILayout.Label("4:3 Landscape");
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal(GUILayout.Width(70.0F));
                choosed_4_3_Portrait = EditorGUILayout.Toggle(choosed_4_3_Portrait);
                GUILayout.Label("4:3 Portrait");
                EditorGUILayout.EndHorizontal();                

            EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical();

                EditorGUILayout.BeginHorizontal(GUILayout.Width(70.0F));
                    choosed_3_2_Landscape = EditorGUILayout.Toggle(choosed_3_2_Landscape);
                    GUILayout.Label("3:2 Landscape");
                    EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal(GUILayout.Width(70.0F));
                    choosed_3_2_Portrait = EditorGUILayout.Toggle(choosed_3_2_Portrait);
                    GUILayout.Label("3:2 Portrait");
                EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginHorizontal(GUILayout.Width(50.0F));
                choosed_Other = EditorGUILayout.Toggle(choosed_Other);
                GUILayout.Label("Other");
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(20.0F);
            EditorGUILayout.BeginHorizontal(GUILayout.Width(60.0F));
                choosed_Custom = EditorGUILayout.Toggle(choosed_Custom);
                GUILayout.Label("Custom");
            EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndHorizontal();
    }

    void DisplayCustomARChooser()
    {
        if(choosed_Custom)
        {
            EditorGUILayout.BeginHorizontal();

                EditorGUILayout.BeginVertical();
                    EditorGUILayout.BeginHorizontal(GUILayout.Width(90.0F));
                        GUILayout.Label("Length: ", GUILayout.Width(50.0F));
                        customAR_Length = EditorGUILayout.FloatField(customAR_Length);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal(GUILayout.Width(90.0F));
                        GUILayout.Label("Height ", GUILayout.Width(50.0F));
                        customAR_Heigth = EditorGUILayout.FloatField(customAR_Heigth);
                    EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical(GUILayout.Width(250.0F));
                    float aspect = customAR_Length / customAR_Heigth;
                    EditorGUILayout.BeginHorizontal(GUILayout.Width(250.0F));
                        GUILayout.Space(10.0F);
                        EditorGUILayout.LabelField("Resulting aspect: " + aspect, EditorStyles.whiteBoldLabel);
                    EditorGUILayout.EndHorizontal();

                    if (customAR_Length > customAR_Heigth)
                    {
                        EditorGUILayout.BeginHorizontal(GUILayout.Width(250.0F));
                        GUILayout.Space(10.0F);
                        EditorGUILayout.LabelField("Layout will be -landscape-");
                        EditorGUILayout.EndHorizontal();
                    } 
                    else if (customAR_Length < customAR_Heigth)
                    {
                        EditorGUILayout.BeginHorizontal(GUILayout.Width(250.0F));
                        GUILayout.Space(10.0F);
                        EditorGUILayout.LabelField("Layout will be -portrait-");
                        EditorGUILayout.EndHorizontal();
                    }
                    else
                    {
                        EditorGUILayout.BeginHorizontal(GUILayout.Width(250.0F));
                        GUILayout.Space(10.0F);
                        EditorGUILayout.LabelField("Layout will be -square-");
                        EditorGUILayout.EndHorizontal();
                    }
            EditorGUILayout.EndVertical();



            EditorGUILayout.EndHorizontal();
            GUILayout.Space(20.0F);
        }
    }

    void ShowRemoveLazyLayoutDialogue()
    {
        GUI.color = Color.yellow;

        EditorGUILayout.LabelField("- W A R N I N G -", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Do you really want to completely remove Lazy Layout in this Scene?", EditorStyles.wordWrappedLabel, GUILayout.Height(30.0F));
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        GUI.color = Color.red;
        if (GUILayout.Button("YES", GUILayout.Width(200.0F)))
        {
            RemoveLazyLayoutInScene();
        }
        GUI.color = Color.green;
        if (GUILayout.Button("NO", GUILayout.Width(200.0F)))
        {
            showRemLLWarning = false;
        }
        EditorGUILayout.EndHorizontal();
        GUI.color = Color.white;

    }

    void RemoveLazyLayoutInScene()
    {
        if (LLC.canvasInScene)
        {
            LazyLayout.LazyLayoutHelper[] allLLHelperComponents = LLC.canvasInScene.GetComponentsInChildren<LazyLayout.LazyLayoutHelper>(true);
            Debug.LogWarning("--LazyLayout-- Removing " + allLLHelperComponents.Length + " Lazy-Layout _HELPER_ Components");
            for (int i = 0; i < allLLHelperComponents.Length; i++)
            {
                Undo.DestroyObjectImmediate(allLLHelperComponents[i]);
                DestroyImmediate(allLLHelperComponents[i]);
            }
        }

        Debug.LogWarning("--LazyLayout-- Removing " + LLC.savedLazyLayouts.Count + " saved Layouts");
        Undo.RecordObject(LLC, "remove LazyLayout");
        LLC.savedLazyLayouts.Clear();

        Debug.LogWarning("--LazyLayout-- Removing Lazy-Layout Controller & Object");
        Undo.DestroyObjectImmediate(LLC);
        DestroyImmediate(LLC);

        if (GameObject.Find("LazyLayout"))
        {
            GameObject LLCToDestroy = GameObject.Find("LazyLayout");
            Undo.DestroyObjectImmediate(LLCToDestroy);
            DestroyImmediate(LLCToDestroy);
        }
    }



    void SaveCurrentLayout(float _minDeviceSizeForLayout)
    {
        LazyLayout.LL_Layout lazyLayout = new LazyLayout.LL_Layout();
        lazyLayout.layoutName = LLC.layoutName;
        lazyLayout.minDeviceSizeForLayout = _minDeviceSizeForLayout;

        lazyLayout.capableOf_16_9_Landscape = choosed_16_9_Landscape;
        lazyLayout.capableOf_16_9_Portrait = choosed_16_9_Portrait;
        lazyLayout.capableOf_4_3_Landscape = choosed_4_3_Landscape;
        lazyLayout.capableOf_4_3_Portrait = choosed_4_3_Portrait;
        lazyLayout.capableOf_3_2_Landscape = choosed_3_2_Landscape;
        lazyLayout.capableOf_3_2_Portrait = choosed_3_2_Portrait;
        lazyLayout.capableOf_Other = choosed_Other;
        lazyLayout.capableOf_Custom = choosed_Custom;

        lazyLayout.customAR_Length = customAR_Length;
        lazyLayout.customAR_Height = customAR_Heigth; 

        rectTransformsInScene = LLC.canvasInScene.GetComponentsInChildren<RectTransform>(true);

        for (int i = 0; i < rectTransformsInScene.Length; i++)
        {
            if (!rectTransformsInScene[i].gameObject.GetComponent<LazyLayout.LazyLayoutHelper>())
            {
                Debug.Log("--LazyLayout-- Object -" + rectTransformsInScene[i].gameObject.name + "- seems to be new since no LL-Helper was detected. Adding it!");
                LazyLayout.LazyLayoutHelper ll_Helper = rectTransformsInScene[i].gameObject.AddComponent<LazyLayout.LazyLayoutHelper>();

                System.Guid newGuid = System.Guid.NewGuid();
                ll_Helper.ll_GUID = newGuid.ToString();
                lazyLayout.AddnewRectObject(rectTransformsInScene[i], newGuid.ToString());
            }
            else
            {
                lazyLayout.AddnewRectObject(rectTransformsInScene[i], rectTransformsInScene[i].gameObject.GetComponent<LazyLayout.LazyLayoutHelper>().ll_GUID);
            }
        }

        LLC.savedLazyLayouts.Add(lazyLayout);
        LLC.index_CurrentlySelectedLayout = LLC.savedLazyLayouts.Count;
    }

    void UpdateLayout(int _index)
    {
        string oldLayoutName = LLC.savedLazyLayouts[_index].layoutName;
        LLC.savedLazyLayouts[_index] = null;

        LazyLayout.LL_Layout lazyLayout = new LazyLayout.LL_Layout();
        lazyLayout.layoutName = oldLayoutName;
        lazyLayout.minDeviceSizeForLayout = currentMinSizeForLayout;

        rectTransformsInScene = LLC.canvasInScene.GetComponentsInChildren<RectTransform>(true);

        for (int i = 0; i < rectTransformsInScene.Length; i++)
        {
            if (!rectTransformsInScene[i].gameObject.GetComponent<LazyLayout.LazyLayoutHelper>())
            {
                Debug.Log("--LazyLayout-- Object -" + rectTransformsInScene[i].gameObject.name + "- seems to be new since no ll-Heler was detected. Adding it!");
                LazyLayout.LazyLayoutHelper ll_Helper = rectTransformsInScene[i].gameObject.AddComponent<LazyLayout.LazyLayoutHelper>();

                System.Guid newGuid = System.Guid.NewGuid();
                ll_Helper.ll_GUID = newGuid.ToString();
                lazyLayout.AddnewRectObject(rectTransformsInScene[i], newGuid.ToString());
            }
            else
            {
                lazyLayout.AddnewRectObject(rectTransformsInScene[i], rectTransformsInScene[i].gameObject.GetComponent<LazyLayout.LazyLayoutHelper>().ll_GUID);
            }
        }

        lazyLayout.capableOf_16_9_Landscape = choosed_16_9_Landscape;
        lazyLayout.capableOf_16_9_Portrait = choosed_16_9_Portrait;
        lazyLayout.capableOf_4_3_Landscape = choosed_4_3_Landscape;
        lazyLayout.capableOf_4_3_Portrait = choosed_4_3_Portrait;
        lazyLayout.capableOf_3_2_Landscape = choosed_3_2_Landscape;
        lazyLayout.capableOf_3_2_Portrait = choosed_3_2_Portrait;
        lazyLayout.capableOf_Other = choosed_Other;
        lazyLayout.capableOf_Custom = choosed_Custom;

        lazyLayout.customAR_Length = customAR_Length;
        lazyLayout.customAR_Height = customAR_Heigth;

        LLC.savedLazyLayouts[_index] = lazyLayout;
    }

    void DeleteSingleLayout(int _index)
    {
        Undo.RecordObject(LLC, "delete saved layout");
        LLC.savedLazyLayouts.RemoveAt(_index);
        LLC.index_CurrentlySelectedLayout = LLC.savedLazyLayouts.Count;
    }

    void ApplySavedLayout(int _index)
    {
        Undo.RecordObject(LLC, "change current Layout");    
        LazyLayout.LayoutApplier layoutApplier = new LazyLayout.LayoutApplier();

        layoutApplier.ll = LLC.savedLazyLayouts[_index];
        layoutApplier.rectTransformsInScene = LLC.canvasInScene.GetComponentsInChildren<RectTransform>(true);

        layoutApplier.ApplyLayout(LLC.switchTextContent);



        LLC.index_CurrentlySelectedLayout = _index + 1;

        choosed_16_9_Landscape = layoutApplier.ll.capableOf_16_9_Landscape;
        choosed_16_9_Portrait = layoutApplier.ll.capableOf_16_9_Portrait;
        choosed_4_3_Landscape = layoutApplier.ll.capableOf_4_3_Landscape;
        choosed_4_3_Portrait = layoutApplier.ll.capableOf_4_3_Portrait;
        choosed_3_2_Landscape = layoutApplier.ll.capableOf_3_2_Landscape;
        choosed_3_2_Portrait = layoutApplier.ll.capableOf_3_2_Portrait;
        choosed_Other = layoutApplier.ll.capableOf_Other;
        choosed_Custom = layoutApplier.ll.capableOf_Custom;

        customAR_Length = layoutApplier.ll.customAR_Length;
        customAR_Heigth = layoutApplier.ll.customAR_Height; 

        LLC.layoutName = layoutApplier.ll.layoutName;
        currentMinSizeForLayout = layoutApplier.ll.minDeviceSizeForLayout;

        Debug.Log("--LazyLayout-- applying layout -" + LLC.savedLazyLayouts[_index].layoutName + "-");
    }

    int LayoutNameIsOkay(string _givenName)
    {
        if (string.IsNullOrEmpty(_givenName))
        { return 1; }

        else
        {
            for (int i = 0; i < LLC.savedLazyLayouts.Count; i++)
            {
                if (_givenName == LLC.savedLazyLayouts[i].layoutName)
                { return 2; }
            }
        }

        return 0;
    }

    int minDeviceSizeIsOkay (float _givenSize)
    {
        if (_givenSize == 0)
        { return 1; }

        else
        {
            for (int i = 0; i < LLC.savedLazyLayouts.Count; i++)
            {
                if (_givenSize == LLC.savedLazyLayouts[i].minDeviceSizeForLayout)
                {
                    if (choosed_16_9_Landscape == true && LLC.savedLazyLayouts[i].capableOf_16_9_Landscape == true)
                    { return 2; }
                    if (choosed_16_9_Portrait == true && LLC.savedLazyLayouts[i].capableOf_16_9_Portrait == true)
                    { return 2; }
                    if (choosed_4_3_Landscape == true && LLC.savedLazyLayouts[i].capableOf_4_3_Landscape == true)
                    { return 2; }
                    if (choosed_4_3_Portrait == true && LLC.savedLazyLayouts[i].capableOf_4_3_Portrait == true)
                    { return 2; }
                    if (choosed_3_2_Landscape == true && LLC.savedLazyLayouts[i].capableOf_3_2_Landscape == true)
                    { return 2; }
                    if (choosed_3_2_Portrait == true && LLC.savedLazyLayouts[i].capableOf_3_2_Portrait == true)
                    { return 2; }
                    if (choosed_Other == true && LLC.savedLazyLayouts[i].capableOf_Other == true)
                    { return 2; }
                    if (choosed_Custom == true && LLC.savedLazyLayouts[i].capableOf_Custom == true)
                    { return 2; }
                }                
            }
        }

        return 0;
    }

    bool choosenARIsOkay ()
    {
        if(!choosed_16_9_Landscape && !choosed_16_9_Portrait && !choosed_4_3_Landscape && !choosed_4_3_Portrait && !choosed_3_2_Landscape && !choosed_3_2_Portrait && !choosed_Other &&!choosed_Custom)
        { return false; }
        else
        { return true; }
    }

    // _-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_- //

    void SaveAllValues()
    {
        if (canvas)
        {
            if (!canvas.Equals(LLC.canvasInScene))
            {
                LLC.canvasInScene = canvas;
            }
        }
    }

    EditorWindow GetMainGameView()
    {
        System.Type T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
        System.Reflection.MethodInfo GetMainGameView = T.GetMethod("GetMainGameView", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        System.Object Res = GetMainGameView.Invoke(null, null);
        return (EditorWindow)Res;
    }

    RectTransform GetCorrentRectTransform(string _GUIDtoSearchFor)
    {
        int count = rectTransformsInScene.Length;
        for (int i = 0; i < count; i++)
        {
            if (rectTransformsInScene[i].GetComponent<LazyLayout.LazyLayoutHelper>().ll_GUID == _GUIDtoSearchFor)
            {
                return rectTransformsInScene[i];
            }
        }

        Debug.LogError("--LAZYLAYOUT ERROR-- Could not find object for instanceID -" + _GUIDtoSearchFor + "-");
        return null;
    }

    void CacheDumpChanges(string _newLine)
    {
        dumpLines = new string[dumpLines.Length + 1];
        dumpLines[dumpLines.Length - 1] = _newLine;
    }
}
