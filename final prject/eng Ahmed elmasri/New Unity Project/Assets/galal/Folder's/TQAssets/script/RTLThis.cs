using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ArabicSupport;

/// <summary>
/// This script applies the RTLFix from the ArabicSupport plugin to any text it's attached to. It works with many Right To Left languages such as Arabic, Farsi, Hebrew, etc
/// </summary>
[RequireComponent(typeof(Text))]
public class RTLThis : MonoBehaviour
{
    // Holds the current text so it can be compared to see if the text changed during play, and then apply the RTL fix accordingly
    internal string currentText = "";

    [Tooltip("If we press the updateNow button, force the RTL fix on the current text. This is good when you want to see the text result inside the editor before playing the game.")]
    public bool updateNow = false;

    public void Start()
    {
        CheckText();
    }

    public void Update()
    {
        CheckText();
    }

    public void OnValidate()
    {
        // If we press the updateNow button, force the RTL fix on the current text. This is good when you want to see the text result inside the editor before playing the game.
        if ( updateNow == true )
        {
            CheckText();

            updateNow = false;
        }
    }

    /// <summary>
    /// Checks if the current text has changed, and applies the Right-to-Left accordingly
    /// </summary>
    public void CheckText()
    {
        // If the text is different, update it
        if (GetComponent<Text>().text != currentText)
        {
            // Turn the text into Right To Left format
            FixRTL(GetComponent<Text>());

            // Set the new text as the default text to check next time
            currentText = GetComponent<Text>().text;
        }
    }

    /// <summary>
    /// Runs the Right-To-Left function from the ArabicSupport plugin
    /// </summary>
    /// <param name="textObject"></param>
    public void FixRTL(Text textObject)
    {
        textObject.text = ArabicFixer.Fix(textObject.text, false, false);
    }

}
