using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class CameraUICanvas : MonoBehaviour
{
    public GameObject blink;             // 사진 찍을 때 깜빡일 것
    public GameObject shareButtons;      // 공유 버튼

    bool isCoroutinePlaying;             // 코루틴 도중엔 사진이 찍히지 않도록 함

    // 파일 불러올 때 필요
    string[] files;                      // PNG파일들 경로 저장
    string albumName = "Shine Bright";   // 생성될 앨범의 이름
    [SerializeField]
    GameObject panel;                    // 찍은 사진이 뜰 패널

    bool firstCount;                     // 첫번째 실행인지 체크하기 위한 변수

    private void OnEnable()
    {
        transform.Find("Panel").gameObject.SetActive(false);
    }

    public void Captureutton()
    {
        if (!isCoroutinePlaying)
        {
            StartCoroutine("captureScreenshot");
            GameManager.Instance.youCanPauseEsc = false;
            // 업적이 달성되어있지 않으면 최초 실행시 달성

            // 만일 처음 셔터를 눌렀을 때
            if (!firstCount)
            {
                // 업적이 달성되어있지 않으면 달성시키고 아니면 종료한다
                GameCenterManager.Instance.SuccessDeviceAchievement(GameCenterManager.MyAchievement.ClickTheShutter);
                firstCount = true;
            }
        }
    }

    IEnumerator captureScreenshot()
    {
        isCoroutinePlaying = true;

        // UI 없앤다!
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
        GameObject moneyCanvas = GameObject.Find("MoneyCanvas");
        if (moneyCanvas != null)
        {
            moneyCanvas.SetActive(false);
        }

        yield return new WaitForEndOfFrame();

        // 스크린샷 + 갤러리갱신
        ScreenshotAndGallery();

        yield return new WaitForEndOfFrame();

        // 블링크
        BlinkUI();

        // 셔터 사운드
        AudioManager.Instance.UiPlay((AudioManager.UiSound)Random.Range(5, 8));

        yield return new WaitForEndOfFrame();

        // UI 다시 나온다
        ShowAgainUI();

        // 찍은 사진이 등장
        GetPirctureAndShowIt();

        isCoroutinePlaying = false;
    }

    // 흰색 블링크 생성
    void BlinkUI()
    {
        GameObject b = Instantiate(blink);
        b.transform.SetParent(transform);
        b.transform.localPosition = new Vector3(0, 0, 0);
        b.transform.localScale = new Vector3(1, 1, 1);
    }

    // 스크린샷 찍고 갤러리에 갱신
    void ScreenshotAndGallery()
    {
        // 스크린샷
        Texture2D ss = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        ss.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        ss.Apply();

        // 갤러리갱신
        Debug.Log("" + NativeGallery.SaveImageToGallery(ss, albumName,
            "ShineBrightScreenshot_" + System.DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss") + "{0}.png"));

        // To avoid memory leaks.
        // 복사 완료됐기 때문에 원본 메모리 삭제
        Destroy(ss);
    }

    // 찍은 사진을 Panel에 보여준다.
    void GetPirctureAndShowIt()
    {
        string pathToFile = GetPicture.GetLastPicturePath();
        if (pathToFile == null)
        {
            return;
        }
        Texture2D texture = GetScreenshotImage(pathToFile);
        Sprite sp = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        panel.SetActive(true);
        shareButtons.SetActive(true);
        panel.GetComponent<Image>().sprite = sp;
    }
    // 찍은 사진을 불러온다.
    Texture2D GetScreenshotImage(string filePath)
    {
        Texture2D texture = null;
        byte[] fileBytes;
        if (File.Exists(filePath))
        {
            fileBytes = File.ReadAllBytes(filePath);
            texture = new Texture2D(2, 2, TextureFormat.RGB24, false);
            texture.LoadImage(fileBytes);
        }
        return texture;
    }

    void ShowAgainUI()
    {
        // UI 다시 나온다
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }
        transform.Find("Panel").gameObject.SetActive(false);
    }


}