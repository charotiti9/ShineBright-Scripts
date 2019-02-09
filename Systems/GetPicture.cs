using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GetPicture : MonoBehaviour {

#if !UNITY_EDITOR && UNITY_ANDROID
    private static AndroidJavaClass m_ajc = null;
    private static AndroidJavaClass AJC
    {
        get
        {
            if (m_ajc == null)
                m_ajc = new AndroidJavaClass("com.yasirkula.unity.NativeGallery");

            return m_ajc;
        }
    }
#endif

    public static string GetLastPicturePath()
    {
        List<string> shotFiles = new List<string>();
        string saveDir;
#if !UNITY_EDITOR && UNITY_ANDROID
		saveDir = AJC.CallStatic<string>( "GetMediaPath", "Shine Bright" );
#else
        saveDir = Application.persistentDataPath;
#endif
        string[] files = Directory.GetFiles(saveDir, "*.png");


        for (int i = 0; i < files.Length; i++)
        {
            if (files[i].Contains("ShineBrightScreenshot_"))
            {
                shotFiles.Add(files[i]);
            }
        }

        if (shotFiles.Count > 0)
        {
            return shotFiles[shotFiles.Count - 1];
        }
        return null;
    }


}
