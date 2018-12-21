using UnityEngine;
using System.Collections;

public class LazyLayout_DPIHelper : MonoBehaviour
{
    public static float GetCorrectScreenSize()
    {
        #if UNITY_ANDROID
        return LazyLayout.DPIHelper.ScreenSize_Android();
        #else
        return LazyLayout.DPIHelper.ScreenSize_Other();
        #endif
    }
}
