using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RearCam : MonoBehaviour {

    #region 테스트1
    private bool camAvailabe;
    private WebCamDevice[] devices;

    static WebCamTexture backCam;

    float scaleY;
    int orient;

    void Start()
    {
        //StartCoroutine("DetectCamera");
        DetectCamera();
    }

    void DetectCamera()
    {
        // 카메라에 맞추기
        Camera cam = Camera.main;
        float pos = (cam.nearClipPlane + 17.5f);
        transform.position = cam.transform.position + cam.transform.forward * pos;
        float h = Mathf.Tan(cam.fieldOfView * Mathf.Deg2Rad * 0.5f) * pos * 2f;
        transform.localScale = new Vector3(h * cam.aspect, h, 0f);

        transform.SetParent(Camera.main.transform);

        // 디바이스 검색
        devices = WebCamTexture.devices;

        // 카메라가 있는지 검색
        if (devices.Length == 0)
        {
            //guideText.text = "No camera detected";
            camAvailabe = false;
            return;
        }

        //있다면 디바이스 중에 후면 카메라가 있는지 검사
        for (int i = 0; i < devices.Length; i++)
        {
            if (!devices[i].isFrontFacing)
            {
                // 중요!!!!!
                // 스크린 width와 height 크기에 맞게 다시 갱신해줄것~!
                backCam = new WebCamTexture(WebCamTexture.devices[0].name, Screen.width, Screen.height);
            }
        }

        // 후면 카메라가 없다
        if (backCam == null)
        {
            //guideText.text = "Unable to find back cam";
            camAvailabe = false;
            return;
        }

        //if (backCam == null)
        //    backCam = new WebCamTexture(WebCamTexture.devices[0].name, Screen.width, Screen.height);

        GetComponent<Renderer>().material.mainTexture = backCam;

        // 있다면 플레이
        if (!backCam.isPlaying)
            backCam.Play();

        camAvailabe = true;
    }

    void Update()
    {
        //if (!camAvailabe)
        //{
        //    gameObject.SetActive(false);
        //    return;
        //}

        // 거울반전
        scaleY = backCam.videoVerticallyMirrored ? -1f : 1f;
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y * scaleY, transform.localScale.z);
        // 상하 돌리기
        orient = -backCam.videoRotationAngle;
        transform.localEulerAngles = new Vector3(0, 0, orient);
    }

    #endregion

    #region 테스트2
    //IEnumerator Start()
    //{
    //    yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);

    //    if (Application.HasUserAuthorization(UserAuthorization.WebCam))
    //    {
    //        WebCamTexture web = new WebCamTexture();

    //        GetComponentInChildren<RawImage>().texture = web;
    //        web.Play();
    //    }
    //}
    #endregion

    #region 테스트3

    //private bool camAvailabe;
    //private WebCamTexture backCam;
    ////private Texture defaultBackground;

    //public RawImage background;
    //public AspectRatioFitter fit;

    //private WebCamDevice[] devices;

    //float ratio;
    //float scaleY;
    //int orient;

    //public Text guideText;

    //void Start()
    //{
    //    //defaultBackground = background.texture;
    //    devices = WebCamTexture.devices;

    //    // 카메라가 있는지 검색
    //    if (devices.Length == 0)
    //    {
    //        guideText.text = "No camera detected";
    //        //print("No camera detected");
    //        camAvailabe = false;
    //        return;
    //    }

    //    // 있다면 디바이스 중에 후면 카메라가 있는지 검사
    //    for (int i = 0; i < devices.Length; i++)
    //    {
    //        if (!devices[i].isFrontFacing)
    //        {
    //            backCam = new WebCamTexture(devices[i].name, Screen.width, Screen.height);

    //        }
    //    }

    //    // 후면 카메라가 없다
    //    if (backCam == null)
    //    {
    //        guideText.text = "Unable to find back cam";
    //        //print("Unable to find back cam");
    //        return;
    //    }

    //    // 있다면 플레이
    //    backCam.Play();
    //    background.texture = backCam;

    //    camAvailabe = true;
    //}


    //private void Update()
    //{
    //    if (!camAvailabe)
    //    {
    //        return;
    //    }

    //    ratio = (float)backCam.width / (float)backCam.height;
    //    fit.aspectRatio = ratio;

    //    scaleY = backCam.videoVerticallyMirrored ? -1f : 1f;
    //    background.rectTransform.localScale = new Vector3(1f, scaleY, 1f);

    //    orient = -backCam.videoRotationAngle;
    //    background.rectTransform.localEulerAngles = new Vector3(0, 0, orient);

    //    guideText.text = "" + background.rectTransform.localScale;
    //}
    #endregion

}
