﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GeneralShare : MonoBehaviour {

    public GameObject ShareButtons;
    bool firstCount;                        // 첫번째 실행인지 체크하기 위한 변수

    public void Share_Button()
    {
        if (!firstCount)
        {
            GameCenterManager.Instance.SuccessDeviceAchievement(GameCenterManager.MyAchievement.ShareYourLight);
            firstCount = true;
        }

        // 공유버튼 사라짐
        ShareButtons.SetActive(false);

        new NativeShare().AddFile(GetPicture.GetLastPicturePath()).SetSubject("Shine Bright")
            .SetText("Now Playing♪: " + AudioManager.Instance.audios[0].clip.name + ", \n"
            + "Download Link: https://play.google.com/store/apps/details?id=com.TeamSalmon.ShineBright \n" +
            " #ShineBright").Share();
    }
}
