using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightGOD : MonoBehaviour
{

    #region 전역변수
    private float currentTime;                  // 시간이 흐른다

    [Header("빛번짐 Pool")]
    public GameObject lightFactory;             // 게임오브젝트 팩토리
    public int poolSize = 10;                   // 풀 사이즈
    [HideInInspector]
    public GameObject[] lightPool;              // 게임오브젝트 풀

    public float minSize = 0.4f;                // 생성할 light의 minSize를 저장
    public float maxSize = 1.0f;                // 생성할 light의 maxSize를 저장
    float preSize;                              // 생성할 light의 Size를 저장하고 있음
    [HideInInspector]
    public float nowSize;                       // 이미 나와있는 light의 Size를 저장하고 있음
    float bigSize;                              // nowSize와 preSize의 비교

    [Header("색 지정")]
    public static Color[] colors;               // light의 색 랜덤들
    Color preColor;                             // 나올 light의 알파값색
    public float minAlpha = 0.3f;               // 나올 light의 알파값 min
    public float maxAlpha = 0.8f;               // 나올 light의 알파값 max
    int colorIndex;                             // 나올 light의 색

    [Header("비례값")]
    // 사이즈에 비례해서 생성시간을 조절한다
    // 사이즈에 비례해서 Y축 최소최대값을 조절한다
    public float createTime = 0.3f;             // 생성 시간
    float minusTime;                            // 추가 시간
    float preYPos;                              // Y축 생성될 곳
    float preXPos;                              // X축 생성될 곳
    public float YPosLimit = 4.5f;              // Y축 리밋
    Vector3 nowPos;                             // 생성되었던 곳

    float xMinPos;                              // 최소 x 위치
    float xMaxPos;                              // 최대 x 위치
    float yMinPos;                              // 최소 y 위치
    float yMaxPos;                              // 최대 y 위치

    float yAmplitude = 1;                       // Y진폭 -1, 1
    int createAmount;                           // 생성된 갯수
    int ranAmount = 3;                          // 랜덤으로 정해진 갯수
    int minAmount = 2;                          // 최소 갯수
    int maxAmount = 5;                          // 최대 갯수

    bool isAmpStarted;                          // 진폭 중인지 체크
    int ampStartedScore;                        // 진폭이 시작되었던 이전 스코어를 저장
    int ampEndedScore;                          // 진폭 끝 저장될 스코어
    int ampMinScore = 150;                      // 진폭 시작 min 스코어
    int ampMaxScore = 300;                      // 진폭 시작 max 스코어
    int ampOnScore;                             // 진폭 시작 저장될 스코어
    int ampOffScore;                            // 진폭 끝날 스코어


    #endregion

    // 싱글톤
    public static LightGOD Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        // 라이트들을 생성해서
        // 비활성화한 뒤 Pool에 저장해둔다
        lightPool = new GameObject[poolSize];
        for (int i = 0; i < poolSize; i++)
        {
            lightPool[i] = Instantiate(lightFactory);
            lightPool[i].SetActive(false);
        }

        // 맨 처음 진폭이 시작될 때를 구한다
        ampOnScore = Random.Range(ampMinScore, ampMaxScore);
        ampOffScore = Random.Range(ampMinScore, ampMaxScore);
    }

    private void OnEnable()
    {
        nowSize = 1;
        nowPos = new Vector3(GameObject.Find("Player").transform.position.x - 0.5f,
            GameObject.Find("Player").transform.position.y, transform.position.z);

        if (colors == null)
        {
            colors = ThemeManager.Instance.themes[0].themeColor;
        }
    }

    void Update()
    {
        // 생성시간마다 light를 생성한다
        CreateLight();
    }


    void CreateLight()
    {
        currentTime += Time.deltaTime;
        // 생성시간 - (1 - 삭제시간)
        // 삭제시간 = 현재원 / max원
        // 만약 max원이라면 1이나옴 -> 삭제시간은 0
        // 작은 원이라면 0에 가까움 -> 삭제시간은 1에 가까움
        // ==> 작은 원일수록 더욱 빨리 생성된다
        if (currentTime > createTime - (1 - minusTime))
        {
            currentTime = 0;

            for (int i = 0; i < poolSize; i++)
            {
                if (lightPool[i].activeSelf == false)
                {
                    // 크기, 색, 알파값을 랜덤으로 설정해준다.
                    SetRandom(lightPool[i]);

                    // 진폭을 결정한다
                    SetAmplitude();

                    // 사이즈를 통해 위치를 결정한다
                    SetPosition(lightPool[i]);

                    // 사이즈를 통해 생성시간을 조절한다
                    SetCreateTime();

                    // 크기와 위치 갱신
                    nowSize = preSize;
                    nowPos = lightPool[i].transform.position;

                    // 만일 머니를 먹을 수 있게 되었다면 
                    if (HO_MoneyManager.Instance._eatMoney)
                    {
                        // ComboStar 생성
                        CreateComboStar(i);
                    }

                    lightPool[i].SetActive(true);
                    createAmount++;
                    break;
                }
            }
        }
    }
    
    void CreateComboStar(int index)
    {

        int _randomUpDown = Random.Range(0, HO_MoneyManager.Instance._createComboProbability);
        // 1/n 확률로 light 위에 위치한다.
        if (_randomUpDown == 1)
        {
            HO_MoneyManager.Instance.starList[0].transform.position = lightPool[index].transform.position + Vector3.up * nowSize;
            HO_MoneyManager.Instance.starList[0].SetActive(true);
            HO_MoneyManager.Instance.starList.RemoveAt(0);
        }
        // 1/n 확률로 light 아래에 위치한다.
        else if (_randomUpDown == 0)
        {
            HO_MoneyManager.Instance.starList[0].transform.position = lightPool[index].transform.position - Vector3.up * nowSize;
            HO_MoneyManager.Instance.starList[0].SetActive(true);
            HO_MoneyManager.Instance.starList.RemoveAt(0);
        }
    }

    void SetRandom(GameObject light)
    {
        // 랜덤한 크기와
        preSize = Random.Range(minSize, maxSize);
        // 랜덤한 색을 갖는다
        colorIndex = Random.Range(0, colors.Length);
        light.GetComponent<SpriteRenderer>().color = colors[colorIndex];
        // 알파값도 랜덤으로 받는다
        preColor = light.GetComponent<SpriteRenderer>().color;
        preColor.a = Random.Range(minAlpha, maxAlpha);
        light.GetComponent<SpriteRenderer>().color = new Color(preColor.r, preColor.g, preColor.b, preColor.a);
    }

    void SetAmplitude()
    {
        // Y축에 진폭을 주자
        // 만약 생성빛번짐이 랜덤빛번짐보다 크다면
        if (createAmount > ranAmount)
        {
            createAmount = 0;

            // 진폭을 할 수 있다면
            if (isAmpStarted)
            {
                // 랜덤 어마운트를 
                // Y축을 서로 바꾸자
                yAmplitude = -yAmplitude;
            }

            // 다음 진폭 시의 나올 빛의 갯수를 뽑는다
            ranAmount = Random.Range(minAmount, maxAmount);
        }

        // 진폭 시작
        // 현재스코어 > 진폭 꺼졌던 스코어 + 현재랜덤스코어
        if (!isAmpStarted && ScoreManager.Instance.Score > ampEndedScore + ampOnScore)
        {
            // 진폭 시작할 수 있도록 한다
            isAmpStarted = true;
            // 진폭이 시작된 이전 스코어를 저장
            ampStartedScore = ScoreManager.Instance.Score;
            // 다음 진폭 시작 스코어를 정해준다
            ampOnScore = Random.Range(ampMinScore, ampMaxScore);
        }
        // 진폭이 시작된 스코어 + 일정스코어 이상 넘어가면
        // 진폭 꺼진다
        else if (isAmpStarted && ScoreManager.Instance.Score > ampStartedScore + ampOffScore)
        {
            isAmpStarted = false;
            // 진폭 꺼지는 이전 스코어를 저장
            ampEndedScore = ScoreManager.Instance.Score;
            // 다음 진폭 꺼지는 스코어를 정해준다
            ampOffScore = Random.Range(ampMinScore, ampMaxScore);
        }



        // 만약 이전에 생성한 위치값이
        // 리밋보다 같거나 크면
        // 아래로 내려가도록
        if (nowPos.y >= YPosLimit)
        {
            yAmplitude = -1;
        }
        else if (nowPos.y <= -YPosLimit)
        {
            yAmplitude = 1;
        }

    }

    void SetPosition(GameObject light)
    {
        // 위치 설정 min과 max
        // 이전에 생성된 아이와 지금 생성된 아이 중 누가 더 큰가?
        if (nowSize > preSize)
        {
            bigSize = nowSize;
        }
        else
        {
            bigSize = preSize;
        }

        // min
        // 이전에 생성된 위치 + 큰 아이의 반지름을 더한값
        xMinPos = nowPos.x + nowSize * 0.75f;
        // 이전에 생성된 위치 + 이전에 생성된 아이의 반지름 + 나의 반지름
        xMaxPos = nowPos.x + nowSize * 0.5f + preSize * 0.5f;
        // 이전에 생성된 위치 + 큰 아이의 반지름을 더한값;
        yMinPos = nowPos.y + (bigSize * 0.25f) * yAmplitude;
        yMaxPos = nowPos.y + (nowSize * 0.5f) * yAmplitude + (preSize * 0.25f) * yAmplitude;

        preXPos = Random.Range(xMinPos, xMaxPos);
        preYPos = Random.Range(yMinPos, yMaxPos);
        light.transform.position = new Vector3(preXPos, preYPos, light.transform.position.z);
    }

    void SetCreateTime()
    {
        // 사이즈에 비례해서
        // 새롭게 생성시간을 조절한다
        // 추가 시간 = 현재원 / max원
        // 시간 = 생성시간 - 추가시간
        if (!DelayZone.Instance.isSoFast)
        {
            minusTime = preSize / maxSize;
        }
        else
        {
            minusTime = 1f;
            // 초기화
            DelayZone.Instance.isSoFast = false;
        }
    }
}
