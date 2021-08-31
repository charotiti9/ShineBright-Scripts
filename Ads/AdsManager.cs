using UnityEngine;
#if UNITY_ADS
using UnityEngine.Advertisements;
#endif

public class AdsManager : MonoBehaviour
{
    // - 보상형 광고 카운트
    int adsRewardCount = 0;
    int AdsCount
    {
        get
        {
            return PlayerPrefs.GetInt("ADCOUNT", adsRewardCount);
        }
        set
        {
            adsRewardCount = value;
            if (adsRewardCount == 5)
            {
                // 보상형광고를 5번 보게 되면 업적 완료
                GameCenterManager.Instance.SuccessDeviceAchievement(GameCenterManager.MyAchievement.ThankYouSoMuch);
                PlayerPrefs.SetInt("ADCOUNT", adsRewardCount);
            }
        }
    }

    // 광고를 보여준다.
    public void ShowDefaultAd()
    {
#if UNITY_ADS
        if (!Advertisement.IsReady())
        {
            Debug.Log("Ads not ready for default placement");
            return;
        }
       
        Advertisement.Show();
#endif
    }

    public void ShowRewardedAd()
    {
        const string RewardedPlacementId = "rewardedVideo";

#if UNITY_ADS
        if (!Advertisement.IsReady(RewardedPlacementId))
        {
            Debug.Log(string.Format("Ads not ready for placement '{0}'", RewardedPlacementId));
            return;
        }

        var options = new ShowOptions { resultCallback = HandleShowResult };
        Advertisement.Show(RewardedPlacementId, options);
#endif
    }

#if UNITY_ADS

    // 광고를 봤는지에 대한 결과 여부의 정보를 가져온다.
    private void HandleShowResult(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                Debug.Log("The ad was successfully shown.");
                // - 광고 시청 완료 시 팝업
                GameObject.Find("MoneyCanvas").GetComponent<Animator>().SetBool("MoneyEarn", true);
                // - 광고 시청 횟수 카운트
                AdsCount++;
                // - Player 에게 별 10개 추가
                MoneyManager.Instance.Combo += 10;
                break;
            case ShowResult.Skipped:
                Debug.Log("The ad was skipped before reaching the end.");
                // - Skipped : 광고를 스킵한 경우
                break;
            case ShowResult.Failed:
                Debug.LogError("The ad failed to be shown.");
                // - Failed : 어떤 이유 때문에 광고 시청에 실패한 경우
                break;
        }
    }

#endif
}