using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIData : MonoBehaviour
{
    public GameObject[] uis;
}


public class HJ_UIManager : UIData {

    // 싱글톤
    public static HJ_UIManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        // 그래픽에서 설정 저장한 것 실행
        uis[12].GetComponent<GraphicCanvas_B>().Initialize();
    }

    [HideInInspector]
    public enum Page
    {
        Home,
        Play,
        Over,
        Pause,
        Picture,
        Setting,
        Music,
        Theme,
        Graphic,
        Languages,
        Reward,
        Credit
    }

    public Page _currentPage = Page.Home;

    int[][] booldata =
    {
        // { scoreText, topScoreText, moneyCanvas, optionCanvas,
        //   homeCanvas_w, overCanvas, pauseCanvas, cameraCanvas,
        //   settingCanvas, musicCanvas, themeCanvas, preThemeCanvas_w, 
        //   graphicCanvas, languagesCanvas, rewardCanvas, creditCanvas }
        new int[]{ 0, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },    // Home : a보임, b안보임, c보임
        new int[]{ 1, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },    // Play : a안보임, b안보임, c안보임
        new int[]{ 0, 0, 1, 1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },    // Over 
        new int[]{ 1, 1, 1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0 },    // Pause 
        new int[]{ 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0 },    // Picture
        new int[]{ 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0 },    // Settings
        new int[]{ 0, 0, 1, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0 },    // Music
        new int[]{ 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 1, 1, 0, 0, 0, 0 },    // Theme
        new int[]{ 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0 },    // Graphic
        new int[]{ 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0 },    // Languages
        new int[]{ 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0 },    // Reward
        new int[]{ 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1 }     // Credit
    };

    public void SetPageUI(Page page)
    {
        // 현재 page 상태 받아오기 
        _currentPage = page;

        int[] bools = booldata[(int)page];                     // booldata[상황(Page(Enum))]

        for (int i = 0; i < uis.Length; i++)
        {
            uis[i].SetActive(bools[i] == 1);                   // bools의 0번째가 1인가? true/false
        }
    }
}


