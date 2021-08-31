using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class CameraUICanvas : MonoBehaviour
{
    public GameObject blink;             // ���� ���� �� ������ ��
    public GameObject shareButtons;      // ���� ��ư

    bool isCoroutinePlaying;             // �ڷ�ƾ ���߿� ������ ������ �ʵ��� ��

    // ���� �ҷ��� �� �ʿ�
    string[] files;                      // PNG���ϵ� ��� ����
    string albumName = "Shine Bright";   // ������ �ٹ��� �̸�
    [SerializeField]
    GameObject panel;                    // ���� ������ �� �г�

    bool firstCount;                     // ù��° �������� üũ�ϱ� ���� ����

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
            // ������ �޼��Ǿ����� ������ ���� ����� �޼�

            // ���� ó�� ���͸� ������ ��
            if (!firstCount)
            {
                // ������ �޼��Ǿ����� ������ �޼���Ű�� �ƴϸ� �����Ѵ�
                GameCenterManager.Instance.SuccessDeviceAchievement(GameCenterManager.MyAchievement.ClickTheShutter);
                firstCount = true;
            }
        }
    }

    IEnumerator captureScreenshot()
    {
        isCoroutinePlaying = true;

        // UI ���ش�!
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

        // ��ũ���� + ����������
        ScreenshotAndGallery();

        yield return new WaitForEndOfFrame();

        // ��ũ
        BlinkUI();

        // ���� ����
        AudioManager.Instance.UiPlay((AudioManager.UiSound)Random.Range(5, 8));

        yield return new WaitForEndOfFrame();

        // UI �ٽ� ���´�
        ShowAgainUI();

        // ���� ������ ����
        GetPirctureAndShowIt();

        isCoroutinePlaying = false;
    }

    // ��� ��ũ ����
    void BlinkUI()
    {
        GameObject b = Instantiate(blink);
        b.transform.SetParent(transform);
        b.transform.localPosition = new Vector3(0, 0, 0);
        b.transform.localScale = new Vector3(1, 1, 1);
    }

    // ��ũ���� ��� �������� ����
    void ScreenshotAndGallery()
    {
        // ��ũ����
        Texture2D ss = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        ss.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        ss.Apply();

        // ����������
        Debug.Log("" + NativeGallery.SaveImageToGallery(ss, albumName,
            "ShineBrightScreenshot_" + System.DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss") + "{0}.png"));

        // To avoid memory leaks.
        // ���� �Ϸ�Ʊ� ������ ���� �޸� ����
        Destroy(ss);
    }

    // ���� ������ Panel�� �����ش�.
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
    // ���� ������ �ҷ��´�.
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
        // UI �ٽ� ���´�
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }
        transform.Find("Panel").gameObject.SetActive(false);
    }


}