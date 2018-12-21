using UnityEngine;
using UnityEditor;
using System.Collections;

public class LazyLayoutWindow : EditorWindow
{

    [MenuItem("Window/LazyLayout/Create LazyLayout Controller")]
    static void CreateLLCTRL()
    {
        LazyLayoutController[] existingLLCtrls = GameObject.FindObjectsOfType<LazyLayoutController>();
        if (existingLLCtrls.Length <= 0)
        {
            if (GameObject.Find("LazyLayout"))
            { GameObject.Find("LazyLayout").AddComponent<LazyLayoutController>(); }
            else
            {
                GameObject llCtrl = new GameObject();
                Undo.RegisterCreatedObjectUndo(llCtrl, "LazyLayout Controller Object");
                llCtrl.name = "LazyLayout";

                Undo.AddComponent<LazyLayoutController>(llCtrl);
            }
        }
        else
        {
            Debug.LogWarning("--LazyLayout-- Scene allready contains a LazyLayout Controller");
        }
    }

    [MenuItem("Window/LazyLayout/Visit Unity Forum Thread")]
    static void OpenForum()
    {
        Application.OpenURL("http://forum.unity3d.com/threads/lazylayout-easy-to-use-layout-manager-switcher-for-unity-gui.324219/");
    }



}