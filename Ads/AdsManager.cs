using UnityEngine;
#if UNITY_ADS
using UnityEngine.Advertisements;
#endif

public class AdsManager : MonoBehaviour
{
    // - ������ ���� ī��Ʈ
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
                // ���������� 5�� ���� �Ǹ� ���� �Ϸ�
                GameCenterManager.Instance.SuccessDeviceAchievement(GameCenterManager.MyAchievement.ThankYouSoMuch);
                PlayerPrefs.SetInt("ADCOUNT", adsRewardCount);
            }
        }
    }

    // ���� �����ش�.
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

    // ���� �ô����� ���� ��� ������ ������ �����´�.
    private void HandleShowResult(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                Debug.Log("The ad was successfully shown.");
                // - ���� ��û �Ϸ� �� �˾�
                GameObject.Find("MoneyCanvas").GetComponent<Animator>().SetBool("MoneyEarn", true);
                // - ���� ��û Ƚ�� ī��Ʈ
                AdsCount++;
                // - Player ���� �� 10�� �߰�
                MoneyManager.Instance.Combo += 10;
                break;
            case ShowResult.Skipped:
                Debug.Log("The ad was skipped before reaching the end.");
                // - Skipped : ���� ��ŵ�� ���
                break;
            case ShowResult.Failed:
                Debug.LogError("The ad failed to be shown.");
                // - Failed : � ���� ������ ���� ��û�� ������ ���
                break;
        }
    }

#endif
}