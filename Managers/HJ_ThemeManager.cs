using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HJ_ThemeManager : MonoBehaviour
{
    // 테마 저장
    [System.Serializable]
    public struct ThemeData
    {
        public string themeName;
        public Color[] themeColor;              // 컬러값 (2차원배열을 인스펙터 창에서 보이게 하기 위해 이렇게 씀)
        public Sprite bgImage;                  // 배경
        public int themeMoney;                  // 머니~
        public Sprite themeSprite;              // 테마 버튼 이미지
        [HideInInspector]
        public bool isPurchased;                       // 구매 했는지?
    }
    public ThemeData[] themes = new ThemeData[4];

    // 싱글톤
    public static HJ_ThemeManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        // 구매했는지 여부도 저장!
        if (PlayerPrefs.HasKey("ThemePurchased"))
        {
            LoadPurchased();
        }
        // 저장된 테마가격이 없을때 ==> 저장해놓는다
        else
        {
            SavePurchased();
        }

        //// 저장된 테마가격이 있을때 ==> 불러온다
        //if (PlayerPrefs.HasKey("ThemePrice"))
        //{
        //    LoadThemePrice();
        //}
        //// 저장된 테마가격이 없을때 ==> 저장해놓는다
        //else
        //{
        //    SaveThemePrice();
        //}


        // 저장된 현재 테마
        if (PlayerPrefs.HasKey("NowTheme"))
        {
            LoadThemePrefs();
        }
        else
        {
            PlayerPrefs.SetInt("NowTheme", 0);
        }

    }

    // 가격 재설정
    public void SetThemePrice(int themeNum, int themeNewPrice)
    {
        // 테마 넘버를 입력하고 이 함수를 실행하면
        // 테마의 가격을 설정할 수 있다
        themes[themeNum].themeMoney = themeNewPrice;
        themes[themeNum].isPurchased = true;
        // 변경된 가격 저장
        SavePurchased();
        //SaveThemePrice();
    }
    //// 저장하기
    //void SaveThemePrice()
    //{
    //    string themeSaveStr = "";
    //    // 테마의 갯수만큼 반복
    //    for (int i = 0; i < themes.Length; i++)
    //    {
    //        // 만약 구매한 거라면 가격은 0!
    //        if (themes[i].isPurchased)
    //        {
    //            themes[i].themeMoney = 0;
    //        }

    //        themeSaveStr = themeSaveStr + themes[i].themeMoney;
    //        if (i < themes.Length - 1)
    //        {
    //            themeSaveStr = themeSaveStr + ",";
    //        }
    //    }
    //    PlayerPrefs.SetString("ThemePrice", themeSaveStr);
    //}
    //// 불러오기
    //void LoadThemePrice()
    //{
    //    //string[] themeLoadStr = PlayerPrefs.GetString("ThemePrice").Split(',');
    //    //int[] themeLoadInt = new int[themeLoadStr.Length];
    //    for (int i = 0; i < themes.Length; i++)
    //    {
    //        //themeLoadInt[i] = System.Convert.ToInt32(themeLoadStr[i]);
    //        //themes[i].themeMoney = themeLoadInt[i];

    //    }
    //}

    // 테마 구매 여부 저장하기
    void SavePurchased()
    {
        string themeSaveStr = "";
        // 테마의 갯수만큼 반복
        for (int i = 0; i < themes.Length; i++)
        {
            themeSaveStr = themeSaveStr + System.Convert.ToInt32(themes[i].isPurchased);
            if (i < themes.Length - 1)
            {
                themeSaveStr = themeSaveStr + ",";
            }
        }
        PlayerPrefs.SetString("ThemePurchased", themeSaveStr);
    }
    // 불러오기
    void LoadPurchased()
    {
        string[] themeLoadStr = PlayerPrefs.GetString("ThemePurchased").Split(',');
        int[] themeLoadInt = new int[themeLoadStr.Length];
        for (int i = 0; i < themeLoadStr.Length; i++)
        {
            themeLoadInt[i] = System.Convert.ToInt32(themeLoadStr[i]);
            themes[i].isPurchased = (themeLoadInt[i] == 1);
            // 만약 구매한 거라면 가격은 0!
            if (themes[i].isPurchased)
            {
                themes[i].themeMoney = 0;
            }
        }
    }


    // 현재 테마 저장하기
    public void SaveThemePrefs()
    {
        PlayerPrefs.SetInt("NowTheme", ThemeCanvas.nowThemeNum);
    }
    // 현재 테마 불러오기
    void LoadThemePrefs()
    {
        ThemeCanvas.nowThemeNum = PlayerPrefs.GetInt("NowTheme");
        GameObject.Find("UIs_new").transform.Find("ThemeCanvas")
            .GetComponent<ThemeCanvas>().ThemeSelect(themes[ThemeCanvas.nowThemeNum].themeColor, ThemeCanvas.nowThemeNum);
    }
}


