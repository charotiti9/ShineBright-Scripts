using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageManager : MonoBehaviour
{
    // 현재 로케이션
    public static Location nowLoc;

    // 싱글톤
    public static LanguageManager _Instance;
    public static LanguageManager Instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = new LanguageManager();
            }
            return _Instance;
        }
    }

    private void Start()
    {
        // 저장된 값이 없을때 ==> 저장해놓는다
        if (!PlayerPrefs.HasKey("Language"))
        {
            SetLan();
            SaveLan();
        }
        // 있을 때 ==> 불러온다
        else
        {
            LoadLan();
        }
    }
    public void LoadLan()
    {
        nowLoc = (Location)Enum.Parse(typeof(Location), PlayerPrefs.GetString("Language"));
    }
    public void SaveLan()
    {
        PlayerPrefs.SetString("Language", Enum.GetName(typeof(Location), nowLoc));
    }
    public void SetLan()
    {
        // Enum의 길이 알기
        int locCount = Enum.GetNames(typeof(Location)).Length;
        // 현재 디바이스가 사용하는 문자 가져오기
        // SystemLanguage sl = Application.systemLanguage;
        for (int i = 0; i < locCount; i++)
        {
            // 로케이션 중에
            // 현재 디바이스가 사용하는 문자와 같은 것이 있다면
            if (Enum.GetNames(typeof(Location))[i]
                == Enum.GetName(typeof(SystemLanguage), Application.systemLanguage))
            {
                nowLoc = (Location)i;
                break;
            }
            // 없다면
            else
            {
                // 기본값 영어
                nowLoc = Location.English;
            }
        }
    }
}
