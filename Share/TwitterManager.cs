using UnityEngine;
using System.Collections;
using TwitterKit.Unity;
using System.IO;

public class TwitterManager : MonoBehaviour
{

    string frontStr;
    string hashtagStr = "#ShineBright";


    public void startLogin()
    {
        UnityEngine.Debug.Log("startLogin");
        // API 키 설정
        Twitter.Init();

        Twitter.LogIn(LoginCompleteWithCompose, (ApiError error) =>
        {
            Debug.Log(error.message);
        });
    }

    public void LoginCompleteWithEmail(TwitterSession session)
    {
        // 이메일 인증
        UnityEngine.Debug.Log("LoginCompleteWithEmail");
        Twitter.RequestEmail(session, RequestEmailComplete, (ApiError error) => { UnityEngine.Debug.Log(error.message); });
    }

    public void RequestEmailComplete(string email)
    {
        UnityEngine.Debug.Log("email=" + email);
        LoginCompleteWithCompose(Twitter.Session);
    }

    public void LoginCompleteWithCompose(TwitterSession session)
    {
        if (LanguageManager.nowLoc == Location.Korean)
        {
            frontStr = "Download: ";
        }
        else if (LanguageManager.nowLoc == Location.Japanese)
        {
            frontStr = "Download: ";
        }
        else
        {
            frontStr = "Download: ";
        }
        string imageUri = "file://" + GetPicture.GetLastPicturePath();
        Twitter.Compose(session, imageUri, frontStr, new string[] { hashtagStr },
            (string tweetId) => { UnityEngine.Debug.Log("Tweet Success, tweetId=" + tweetId); },
            (ApiError error) => { UnityEngine.Debug.Log("Tweet Failed " + error.message); },
            () => { Debug.Log("Compose cancelled"); }
         );
    }
}