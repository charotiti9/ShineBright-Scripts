using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Light : MonoBehaviour {

    // 태어나고 LightGOD에서 preSize를 받아와서
    // Lerp 로 커진다
    float scaleSpeed = 5;
    float rotSpeed = 6;

    // z축 회전값
    float rot;
    float size;

    // 알파값
    float origAlpha;
    float newAlpha;
    Color origColor;
    Color newColor;

    // 시간
    float currentTime;
    float ranTime = 0;
    float delayTime = 0.02f;
    public float minTime = 0.3f;
    public float maxTime = 1.0f;

    bool isOver;

    private void OnEnable()
    {
        // 초기화
        transform.localScale = Vector3.zero;
        transform.rotation = Quaternion.identity;
        size = LightGOD.Instance.nowSize;
        // 커지고 회전값 지정하는거 실행
        StartCoroutine("SizeUp");
        // 나의 알파값 저장
        origColor = GetComponent<SpriteRenderer>().color;
        origAlpha = origColor.a;
    }

    private void Update()
    {
        if (!isOver && GameManager.Instance.gState == GameManager.GameState.Over)
        {
            isOver = true;
            StopCoroutine("SizeUp");
            StartCoroutine("SizeDown");
        }
    }

    IEnumerator SizeUp()
    {
        // 회전값 뽑기
        rot = Random.Range(0, 360);
        // 크기값 저장
        while (size - transform.localScale.x > 0.1f)
        {
            transform.localScale = Vector3.Lerp(transform.localScale,
                new Vector3(size, size, 0), scaleSpeed * Time.deltaTime);

            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, rot), rotSpeed * Time.deltaTime);

            yield return null;
        }
        while (gameObject.activeSelf)
        {
            // 다음 랜덤타임 뽑는다
            ranTime = Random.Range(minTime, maxTime);

            yield return new WaitForSecondsRealtime(ranTime);

            // 랜덤한 알파값으로 바뀐다
            newAlpha = Random.Range(LightGOD.Instance.minAlpha, LightGOD.Instance.maxAlpha);
            while (GetComponent<SpriteRenderer>().color.a != newAlpha)
            {
                //GetComponent<SpriteRenderer>().color = new Color(myColor.r, myColor.g, myColor.b, newAlpha);
                newColor = new Color(origColor.r, origColor.g, origColor.b, newAlpha);
                GetComponent<SpriteRenderer>().color = Color.Lerp(GetComponent<SpriteRenderer>().color, newColor, 20*Time.deltaTime);
                yield return null;
            }

            // 몇초 뒤
            yield return new WaitForSecondsRealtime(delayTime);

            // 저장된 알파값으로 바뀐다
            while (GetComponent<SpriteRenderer>().color.a != origAlpha)
            {
                //GetComponent<SpriteRenderer>().color = new Color(myColor.r, myColor.g, myColor.b, newAlpha);
                GetComponent<SpriteRenderer>().color = Color.Lerp(GetComponent<SpriteRenderer>().color, origColor, 20 * Time.deltaTime);
                yield return null;
            }

            yield return null;
        }
    }

    IEnumerator SizeDown()
    {
        while (transform.localScale.x > 0)
        {
            transform.localScale += new Vector3(-0.1f, -0.1f, -0.1f);

            if (transform.localScale.x < 0.05f)
            {
                transform.localScale = new Vector3(0f, 0f, 0f);
            }

            yield return new WaitForSecondsRealtime(0);
        }
    }


}
