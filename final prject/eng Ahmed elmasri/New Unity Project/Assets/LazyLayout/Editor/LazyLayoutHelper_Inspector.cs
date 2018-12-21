using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(LazyLayout.LazyLayoutHelper), true)]
public class LazyLayoutHelper_Inspector : Editor
{
    LazyLayout.LazyLayoutHelper LLH;

    bool showDeleteDialog;

    int delCounter;

    public void OnEnable()
    {
        LLH = target as LazyLayout.LazyLayoutHelper;    
    }

    void OnInspectorUpdate()
    { this.Repaint(); }

    public override void OnInspectorGUI()
    {
        if (!showDeleteDialog)
        {
            GUI.color = Color.red;
            if (GUILayout.Button("Delete GameObject", GUILayout.Width(150.0F), GUILayout.Height(30.0F)))
            {
                showDeleteDialog = true;
            }
            GUI.color = Color.white;
        }
        else
        {
            ShowDeleteDialogue();
        }
        
    }

    public void ShowDeleteDialogue()
    {
        EditorGUILayout.LabelField("ARE YOU SHURE YOU WANT TO DELETE THIS GAMEOBJECT?");

        GUI.color = Color.red;
        if (GUILayout.Button("Yes", GUILayout.Width(60.0F)))
        {
            DeleteGO();
        }
        GUI.color = Color.white;

        GUI.color = Color.green;
        if (GUILayout.Button("No", GUILayout.Width(60.0F)))
        {
            showDeleteDialog = false;
        }
        GUI.color = Color.white;

    }

    public void DeleteGO()
    {
        delCounter = 0;
        LazyLayoutController LLC = GameObject.FindObjectOfType<LazyLayoutController>();

        //Check every saved layout for ll_GUID of the object that has to be delted
        for (int i = 0; i < LLC.savedLazyLayouts.Count; i++)
        {
            for (int j = 0; j < LLC.savedLazyLayouts[i].savedRectTransformsInScene.Count; j++)
            {
                if (LLC.savedLazyLayouts[i].savedRectTransformsInScene[j].ll_GUID == LLH.ll_GUID)
                {
                    delCounter++;
                    LLC.savedLazyLayouts[i].savedRectTransformsInScene.RemoveAt(j);
                }
            }
        }

        Debug.LogWarning("--LazyLayout-- Removed " + delCounter + " entries in all saved layouts");
        
        //finally destroy the GO
        DestroyImmediate(LLH.gameObject);

    }
}
