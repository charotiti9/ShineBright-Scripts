using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RearCam : MonoBehaviour
{

    bool camAvailabe;
    WebCamDevice[] devices;

    WebCamTexture backCam;

    float scaleY;
    int orient;

    public void DetectCamera()
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
            camAvailabe = false;
            return;
        }

        // 디바이스 중에 후면 카메라가 있는지 검사
        for (int i = 0; i < devices.Length; i++)
        {
            if (!devices[i].isFrontFacing)
            {
                // 스크린 width와 height 크기에 맞게 다시 갱신해줄 것
                backCam = new WebCamTexture(WebCamTexture.devices[0].name, Screen.width, Screen.height);
            }
        }

        // 후면 카메라가 없다
        if (backCam == null)
        {
            camAvailabe = false;
            return;
        }

        GetComponent<Renderer>().material.mainTexture = backCam;

        // 있다면 플레이
        if (!backCam.isPlaying)
            backCam.Play();

        camAvailabe = true;
    }

    public void CameraActivate()
    {
        if (!camAvailabe)
        {
            gameObject.SetActive(false);
            return;
        }

        // 거울반전
        scaleY = backCam.videoVerticallyMirrored ? -1f : 1f;
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y * scaleY, transform.localScale.z);
        // 상하 돌리기
        orient = -backCam.videoRotationAngle;
        transform.localEulerAngles = new Vector3(0, 0, orient);
    }
}