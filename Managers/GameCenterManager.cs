using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames.BasicApi;
using GooglePlayGames;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public class GameCenterManager : MonoBehaviour
{
    // 업적 달성용
    // SignIn을 하지 않더라도, 각각 따로 true/false를 기기에서 판별하여 보상을 준다
    // <GameCenterManager>
    // - 업적에 따라 6개의 크기 만들기
    // - PlayrPrefs로 저장
    // - 업적을 달성하면 achieveDeviceSuccess[(int)MyAchievement.TutorialClear] = true;
    // - 업적 순서를 갱신한다 nowAchievedInt = (int)MyAchievement.TutorialClear;
    // - 보상을 준다 GameObject.Find("MoneyCanvas").GetComponent<Animator>().SetBool("AchieveMoneyEarn", true);

    // UI를 여는 순간 로그인을 한다
    // achieveDeviceSuccess로 달성 여부를 판별한다.

    // <각 달성시 호출되는 스크립트>
    // 달성시 GameCenterManager.Instance.SuccessDeviceAchievement(GameCenterManager.MyAchievement.~);

    // <RewardCanvas>
    // - isClear = achieveDeviceSuccess[(int)MyAchievement.TutorialClear] 로 판별

    [HideInInspector]
    public bool[] achieveDeviceSuccess;

    [HideInInspector]
    public enum MyAchievement
    {
        TutorialClear,
        YourFavoriteMusic,
        DeepCityLight,
        ClickTheShutter,
        ShareYourLight,
        ThankYouSoMuch
    }
    [HideInInspector]
    public int nowAchievedInt = 0;

    [HideInInspector]
    public int[] rewardMoneys = new int[]
    {
        10,
        10,
        20,
        20,
        30,
        30
    };

    // 싱글톤
    public static GameCenterManager Instance;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        achieveDeviceSuccess = new bool[rewardMoneys.Length];
        // 저장된 값이 있을때 ==> 불러온다
        if (PlayerPrefs.HasKey("AchieveSuccess"))
        {
            LoadSetting();
        }
        // 저장된 값이 없을때 ==> 저장해놓는다
        else
        {
            SaveSetting();
        }
    }

    // 저장하기
    void SaveSetting()
    {
        string settingStr = "";
        // 테마의 갯수만큼 반복
        for (int i = 0; i < achieveDeviceSuccess.Length; i++)
        {
            settingStr = settingStr + System.Convert.ToInt32(achieveDeviceSuccess[i]);
            if (i < achieveDeviceSuccess.Length - 1)
            {
                settingStr = settingStr + ",";
            }
        }
        PlayerPrefs.SetString("AchieveSuccess", settingStr);
    }
    // 불러오기
    void LoadSetting()
    {
        string[] loadSettingStr = PlayerPrefs.GetString("AchieveSuccess").Split(',');
        int[] loadSettingInt = new int[loadSettingStr.Length];
        for (int i = 0; i < loadSettingStr.Length; i++)
        {
            loadSettingInt[i] = System.Convert.ToInt32(loadSettingStr[i]);
            achieveDeviceSuccess[i] = loadSettingInt[i] == 1;
        }
    }

    // API 호출
    void Start()
    {
        // api 호출
#if UNITY_ANDROID

        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
            .EnableSavedGames()
            .Build();

        PlayGamesPlatform.InitializeInstance(config);

        PlayGamesPlatform.DebugLogEnabled = true;

        PlayGamesPlatform.Activate();

#elif UNITY_IOS
 
        GameCenterPlatform.ShowDefaultAchievementCompletionBanner(true);
 
#endif

        //SignIn();
    }

    // 로그인 팝업
    public void SignIn()
    {
        if (Social.localUser.authenticated == false)
        {
            Social.localUser.Authenticate((bool success) =>
            {
                if (success)
                {
                    // Sign In 성공
                }
                else
                {
                    // Sign In 실패 처리
                    return;
                }
            });
        }

    }


    #region 업적
    // 업적 UI 표시
    public void ShowAchievementUI()
    {
        // Sign In 이 되어있지 않은 상태라면
        // Sign In 후 업적 UI 표시 요청할 것
        if (Social.localUser.authenticated == false)
        {
            Social.localUser.Authenticate((bool success) =>
            {
                if (success)
                {
                    // Sign In 성공
                    // 구글 업적 달성
                    SuccessGoogleAchievement();
                    // 바로 업적 UI 표시 요청
                    Social.ShowAchievementsUI();
                    return;
                }
                else
                {
                    // Sign In 실패 처리
                    return;
                }
            });
        }

        // 구글 업적 달성
        SuccessGoogleAchievement();
        Social.ShowAchievementsUI();
    }

    // 업적 기기 완료
    public void SuccessDeviceAchievement(MyAchievement reward)
    {
        // 이미 완료한 것이라면 돌아가욤
        if (achieveDeviceSuccess[(int)reward] == true)
        {
            return;
        }

        // 완료 여부 갱신
        switch (reward)
        {
            case MyAchievement.TutorialClear:
                // 디바이스 bool 갱신시킨다
                achieveDeviceSuccess[0] = true;
                break;
            case MyAchievement.YourFavoriteMusic:
                achieveDeviceSuccess[1] = true;
                break;
            case MyAchievement.DeepCityLight:
                achieveDeviceSuccess[2] = true;
                break;
            case MyAchievement.ClickTheShutter:
                achieveDeviceSuccess[3] = true;
                break;
            case MyAchievement.ShareYourLight:
                achieveDeviceSuccess[4] = true;
                break;
            case MyAchievement.ThankYouSoMuch:
                achieveDeviceSuccess[5] = true;
                break;
            default:
                break;
        }

        // 저장
        SaveSetting();

        // 기기에서 완료되어있다면
        // 구글에서 완료되지 않았나? -> 완료하자!
        // 구글에서 완료되었나? -> 넘어감

        // 로그인되어있다면 구글업적도 달성시키자
        if (Social.localUser.authenticated == true)
        {
            //SuccessGoogleAchievement();
            // 모든 bool값을 검사
            for (int i = 0; i < achieveDeviceSuccess.Length; i++)
            {
                // 기기에서 완료되어있다면
                if (achieveDeviceSuccess[i] == true)
                {
                    // 구글 완료 여부 갱신
                    switch (i)
                    {
                        case 0:
                            // 5. 해당 업적을 완료시킨다
                            CompleteAhievement("CgkIv4zOnqIIEAIQBQ");
                            break;
                        case 1:
                            CompleteAhievement("CgkIv4zOnqIIEAIQCA");
                            break;
                        case 2:
                            CompleteAhievement("CgkIv4zOnqIIEAIQBg");
                            break;
                        case 3:
                            CompleteAhievement("CgkIv4zOnqIIEAIQBA");
                            break;
                        case 4:
                            CompleteAhievement("CgkIv4zOnqIIEAIQBw");
                            break;
                        case 5:
                            CompleteAhievement("CgkIv4zOnqIIEAIQCg");
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
    // 기기완료에 따라서 업적 구글 완료를 판별
    public void SuccessGoogleAchievement()
    {
        Social.LoadAchievements(achievements =>
        {
            // 1. 만일 업적이 존재한다면 
            if (achievements.Length > 0)
            {
                int i = -1;
                // 2. 모든 업적을 검사한다.
                foreach (IAchievement achievement in achievements)
                {
                    // 순서체크
                    i++;

                    // <구글 UI가 나올 때> => 로그인이 되어있음
                    // 구글에서 완료되어있다면
                    // 기기에서 완료되지 않았나? -> 완료하러가자!
                    // 기기에서 완료되었나? -> 넘어감

                    // 기기에서 완료되어있다면
                    // 구글에서 완료되지 않았나? -> 완료하자!
                    // 구글에서 완료되었나? -> 넘어감

                    // 구글이 완료되어 있다면
                    if (achievement.completed == true)
                    {
                        switch (achievement.id)
                        {
                            case "CgkIv4zOnqIIEAIQBQ":
                                // 기기도 완료시킨다
                                SuccessDeviceAchievement(MyAchievement.TutorialClear);
                                break;
                            case "CgkIv4zOnqIIEAIQCA":
                                SuccessDeviceAchievement(MyAchievement.YourFavoriteMusic);
                                break;
                            case "CgkIv4zOnqIIEAIQBg":
                                SuccessDeviceAchievement(MyAchievement.DeepCityLight);
                                break;
                            case "CgkIv4zOnqIIEAIQBA":
                                SuccessDeviceAchievement(MyAchievement.ClickTheShutter);
                                break;
                            case "CgkIv4zOnqIIEAIQBw":
                                SuccessDeviceAchievement(MyAchievement.ShareYourLight);
                                break;
                            case "CgkIv4zOnqIIEAIQCg":
                                SuccessDeviceAchievement(MyAchievement.ThankYouSoMuch);
                                break;
                            default:
                                break;
                        }
                        //// 기기도 완료시킨다
                        //SuccessDeviceAchievement((MyAchievement)i);
                    }

                    // 기기에서 완료되어있다면
                    if (achieveDeviceSuccess[i] == true)
                    {
                        // 구글 완료 여부 갱신
                        switch (i)
                        {
                            case 0:
                                // 5. 해당 업적을 완료시킨다
                                CompleteAhievement("CgkIv4zOnqIIEAIQBQ");
                                break;
                            case 1:
                                CompleteAhievement("CgkIv4zOnqIIEAIQCA");
                                break;
                            case 2:
                                CompleteAhievement("CgkIv4zOnqIIEAIQBg");
                                break;
                            case 3:
                                CompleteAhievement("CgkIv4zOnqIIEAIQBA");
                                break;
                            case 4:
                                CompleteAhievement("CgkIv4zOnqIIEAIQBw");
                                break;
                            case 5:
                                CompleteAhievement("CgkIv4zOnqIIEAIQCg");
                                break;
                            default:
                                break;
                        }
                    }


                }
            }
            // 1-2. 업적이 존재하지 않는다면
            else
            {

            }
        });

    }
    // 업적 id 에 따라 해당 업적을 완료시키고 싶다 
    void CompleteAhievement(string id)
    {
        switch (id)
        {
            case "CgkIv4zOnqIIEAIQBQ":
                RewardTutorialClear();
                break;
            case "CgkIv4zOnqIIEAIQCA":
                RewardYourFavoriteMusic();
                break;
            case "CgkIv4zOnqIIEAIQBg":
                RewardDeepCityLight();
                break;
            case "CgkIv4zOnqIIEAIQBA":
                RewardClickTheShutter();
                break;
            case "CgkIv4zOnqIIEAIQBw":
                RewardShareYourLight();
                break;
            case "CgkIv4zOnqIIEAIQCg":
                RewardThankYouSoMuch();
                break;
            default:
                break;
        }
    }


    // 안쓰는것들
    // 모든 업적 달성여부 알자
    public void JustCheckAchievement()
    {
        // 로그인
        SignIn();

        Social.LoadAchievements(achievements =>
        {
            // 1. 만일 업적이 존재한다면 
            if (achievements.Length > 0)
            {
                // 2. 존재하는 업적을 검사한다
                for (int i = 0; i < achievements.Length; i++)
                {
                    RewardCanvas.Instance.rewardBools[i] = achievements[i].completed;
                }
            }
        });
    }
    // 업적 리스트 가져오기 
    void GetMyAchievment()
    {
        Social.LoadAchievements(achievements =>
        {
            if (achievements.Length > 0)
            {
                Debug.Log("Got " + achievements.Length + " achievement instances");
                string myAchievements = "My achievements:\n";
                foreach (IAchievement achievement in achievements)
                {
                    myAchievements += "\t" +
                        achievement.id + " " +
                        achievement.percentCompleted + " " +
                        achievement.completed + " " +
                        achievement.lastReportedDate + "\n";
                }
                Debug.Log(myAchievements);
            }
            else
                Debug.Log("No achievements returned");
        });
    }


    // 업적 예시
    #region 업적 목록 
    // 1. 튜토리얼 클리어 
    public void RewardTutorialClear()
    {
#if UNITY_ANDROID
        // ReportProgress("업적ID", 업적 진행도 0f~100f(단순조건충족은 100), callback);
        PlayGamesPlatform.Instance.ReportProgress("CgkIv4zOnqIIEAIQBQ", 100f, null);
#elif UNITY_IOS
            Social.ReportProgress("Reward ID", 100f, null);
#endif
    }

    // 2. Your favorite music
    public void RewardYourFavoriteMusic()
    {
#if UNITY_ANDROID
        // ReportProgress("업적ID", 업적 진행도 0f~100f(단순조건충족은 100), callback);
        PlayGamesPlatform.Instance.ReportProgress("CgkIv4zOnqIIEAIQCA", 100f, null);
#elif UNITY_IOS
            Social.ReportProgress("Reward ID", 100f, null);
#endif
    }

    // 3. Deep City Light
    public void RewardDeepCityLight()
    {
#if UNITY_ANDROID
        // ReportProgress("업적ID", 업적 진행도 0f~100f(단순조건충족은 100), callback);
        PlayGamesPlatform.Instance.ReportProgress("CgkIv4zOnqIIEAIQBg", 100f, null);
#elif UNITY_IOS
            Social.ReportProgress("Reward ID", 100f, null);
#endif
    }

    // 4. Click the Shutter
    public void RewardClickTheShutter()
    {
#if UNITY_ANDROID
        // ReportProgress("업적ID", 업적 진행도 0f~100f(단순조건충족은 100), callback);
        PlayGamesPlatform.Instance.ReportProgress("CgkIv4zOnqIIEAIQBA", 100f, null);
#elif UNITY_IOS
            Social.ReportProgress("Reward ID", 100f, null);
#endif
    }

    // 5. Share your light
    public void RewardShareYourLight()
    {
#if UNITY_ANDROID
        // ReportProgress("업적ID", 업적 진행도 0f~100f(단순조건충족은 100), callback);
        PlayGamesPlatform.Instance.ReportProgress("CgkIv4zOnqIIEAIQBw", 100f, null);
#elif UNITY_IOS
            Social.ReportProgress("Reward ID", 100f, null);
#endif
    }

    // 6. Thank you so much
    public void RewardThankYouSoMuch()
    {
#if UNITY_ANDROID
        // ReportProgress("업적ID", 업적 진행도 0f~100f(단순조건충족은 100), callback);
        PlayGamesPlatform.Instance.ReportProgress("CgkIv4zOnqIIEAIQCg", 100f, null);
#elif UNITY_IOS
            Social.ReportProgress("Reward ID", 100f, null);
#endif
    }


    #endregion // 업적 목록 

    #endregion

    #region 리더보드
    string leaderboard = "CgkIv4zOnqIIEAIQCw";
    // 리더보드 기록
    // ReportScore(점수, "리더보드 고유 ID", callback);
    public void ReportScore(int score)
    {
#if UNITY_ANDROID

        //PlayGamesPlatform.Instance.ReportScore(score, leaderboard, (bool success) =>
        //{
        //    if (success)
        //    {
        //        // Report 성공
        //        // 그에 따른 처리
        //    }
        //    else
        //    {
        //        // Report 실패
        //        // 그에 따른 처리
        //    }
        //});


        Social.ReportScore(score, leaderboard, (bool success) =>
        {
            if (success)
            {

            }
            else
            {
                // 점수 저장에 실패하였습니다.
            }
        });



#elif UNITY_IOS
 
        Social.ReportScore(score, "Leaderboard_ID", (bool success) =>
            {
                if (success)
                {
                    // Report 성공
                    // 그에 따른 처리
                }
                else
                {
                    // Report 실패
                    // 그에 따른 처리
                }
            });
        
#endif
    }

    // 리더보드 UI
    public void ShowLeaderboardUI()
    {
        // 내가 연결하게 될 것이다
        PlayGamesPlatform.Activate();
        //ReportScore(ScoreManager.Instance.topScore);
        Social.localUser.Authenticate(AuthenticateHandler);
    }


    void AuthenticateHandler(bool isSuccess)
    {
        if (isSuccess)
        {
            int highScore = ScoreManager.Instance.topScore;
            Social.ReportScore(highScore, leaderboard, (bool success) =>
            {
                if (success)
                {
                    PlayGamesPlatform.Instance.ShowLeaderboardUI(leaderboard);
                }
                else
                {
                    // 점수 저장에 실패하였습니다.
                }
            });
        }
        else
        {
            // 로그인에 실패하였습니다
        }
    }

    #endregion

}