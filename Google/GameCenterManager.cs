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
                    CompleteAhievement();
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
        CompleteAhievement();
        Social.ShowAchievementsUI();
    }


    public void CheckAchievement()
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

    public void CompleteAhievement(string id)
    {
#if UNITY_ANDROID
        // ReportProgress("업적ID", 업적 진행도 0f~100f(단순조건충족은 100), callback);
        PlayGamesPlatform.Instance.ReportProgress(id, 100f, null);
#elif UNITY_IOS
            Social.ReportProgress("Reward ID", 100f, null);
#endif
    }

    #endregion

    #region 리더보드
    string leaderBoardID = "leaderBoardID";

public void RankButton()
    {
        PlayGamesPlatform.Activate();
        Social.localUser.authenticate(AuthenticateHandler);
    }

    void AuthenticateHandler(bool isSuccess)
    {
        if (isSuccess)
        {
            float highScore = PlayerPrefs.GetFloat("TopScore", ScoreManager.Instance.topScore);
            Social.ReportScore((long)highScore, leaderBoardID, (bool success) =>
            {
                if (success)
                {
                    PlayGamesPlatform.Instance.ShowLeaderboardUI(leaderBoardID);
                }
                else
                {
                    // 점수 저장 실패
                }
            });
        }
        else
        {
            // 로그인 실패
        }
    }
    #endregion

}