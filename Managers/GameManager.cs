using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.PostProcessing;

public class GameManager : MonoBehaviour
{

    #region 전역변수
    float currentTime;                          // 현재시간
    GameObject player;                          // 플레이어
    [HideInInspector]
    public HO_PlayerMove playerMove;            // 플레이어 움직이는 스크립트

    PostProcessingBehaviour post;               // 포스트 프로세싱
    BloomModel.Settings setting;                // 블룸
    HO_CameraManager camManager;                // 카메라 매니저
    float postLerp;

    #region UI
    public Image[] homeTitle;                     // 홈 타이틀
    public Text homeSlideText;                  // 홈 슬라이드 투 플레이 텍스트
    public GameObject fingerSlide;              // 튜토리얼의 핑거슬라이드
    public Text countText;

    // 호진오빠가 추가한 것
    GameObject playerRound;                     // 플레이어 주변을 도는 원 객체
    Image playerRoundImg;                       // 플레이어 주변을 도는 원 객체의 이미지 자체
    #endregion

    // 오디오매니저
    AudioManager audioManager;

    [HideInInspector]
    public int nowState;                         // Pause상태에서도 현재 어떤 상태인지 체크할 수 있다

    [HideInInspector]
    public bool youCanDead;                      // 죽는 것 체크할 수 있다
    [HideInInspector]
    public bool youCanPauseEsc = true;           // 일시정지 중 되돌아가기 누를 수 있다
    // 코루틴중
    [HideInInspector]
    public bool isCoroutinePlaying;
    // 한번만 실행
    bool isPause;
    // ToastMessage 체크용 변수 
    [HideInInspector]
    public int firstCount;
    bool escDown;
    #endregion

    #region 상태머신/싱글톤

    // 상태머신
    [HideInInspector]
    public enum GameState
    {
        Home,
        Play,
        Pause,
        Over
    }
    [HideInInspector]
    public GameState gState;

    // 싱글톤
    public static GameManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    #endregion


    void Start()
    {
        // 해상도 비율 고정
        Screen.SetResolution(Screen.width, (Screen.width * 9) / 16, Screen.fullScreen);
        // 핸드폰 꺼짐 방지
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        player = GameObject.Find("Player");
        playerMove = player.GetComponent<HO_PlayerMove>();
        camManager = GameObject.Find("CameraManager").GetComponent<HO_CameraManager>();
        post = Camera.main.GetComponent<PostProcessingBehaviour>();
        setting = post.profile.bloom.settings;

        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();

        // 호진오빠가 추가
        // "Slide To Move" 문구 이후 사용자 터치 유도 이미지 표시를 위한 오브젝트 가져오기
        playerRound = player.transform.Find("Canvas").gameObject;
        playerRoundImg = playerRound.transform.GetChild(0).GetComponent<Image>();
        // - 알파값 제로
        Color c = playerRoundImg.color;
        c.a = 0;
        playerRoundImg.color = c;

        // 시작은 home에서!
        HomeProcess();
    }

    void Update()
    {
        switch (gState)
        {
            case GameState.Home:
                Home();
                break;
            case GameState.Play:
                Play();
                break;
            case GameState.Pause:
                Pause();
                break;
            case GameState.Over:
                Over();
                break;
        }
        // esc 키 & firstCount 변수로 뒤로가기버튼을 통해 토스트메시지 띄우는 경우의 수 체크
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            escDown = true;
            AndroidBackKey();
        }
        else if (Input.GetKeyUp(KeyCode.Escape))
        {
            escDown = false;
        }
    }

    public bool isGamePlaying { get { return gState == GameState.Play; } }

    #region 홈

    public void HomeProcess()
    {
        // 초기화
        InitializeGame();

        // 블룸 인텐시티 설정
        setting.bloom.intensity = 5;

        // 빗소리와 음악 시작
        if (!audioManager.audios[0].isPlaying)
        {
            audioManager.BGPlay(AudioManager.BGSounds.BG_1);
        }
        if (!audioManager.audios[1].isPlaying)
        {
            audioManager.RainPlay(AudioManager.RainSound.Rain);
        }

        // 상태저장
        nowState = 0;

        // UI
        UIManager.Instance.SetPageUI(UIManager.Page.Home);
        // Home 상태로
        gState = GameState.Home;
        StartCoroutine("Title");
    }
    private void InitializeGame()
    {
        // 스코어 초기화
        ScoreManager.Instance.Score = 0;
        youCanDead = false;
        youCanPauseEsc = true;
        // 카메라 멈춤
        camManager.enabled = false;
        // 그리고 시간은 움직인다...
        Time.timeScale = 1.0f;
    }
    IEnumerator Title()
    {
        isCoroutinePlaying = true;

        float wantedColor = 1;
        float finalColor;
        Color titleC = homeTitle[0].color;
        while (homeTitle[1].color.a != wantedColor)
        {
            finalColor = Mathf.Lerp(homeTitle[0].color.a, wantedColor, 5 * Time.deltaTime);
            homeTitle[0].color = new Color(titleC.r, titleC.g, titleC.b, finalColor);
            homeTitle[1].color = new Color(titleC.r, titleC.g, titleC.b, finalColor);
            if (homeTitle[1].color.a > 0.95f)
            {
                homeTitle[0].color = new Color(titleC.r, titleC.g, titleC.b, 1);
                homeTitle[1].color = new Color(titleC.r, titleC.g, titleC.b, 1);
            }
            yield return null;
        }

        yield return new WaitForSeconds(3);

        wantedColor = 0;
        while (homeTitle[1].color.a != wantedColor)
        {
            finalColor = Mathf.Lerp(homeTitle[0].color.a, wantedColor, 5 * Time.deltaTime);
            homeTitle[0].color = new Color(titleC.r, titleC.g, titleC.b, finalColor);
            homeTitle[1].color = new Color(titleC.r, titleC.g, titleC.b, finalColor);
            if (homeTitle[1].color.a < 0.05f)
            {
                homeTitle[0].color = new Color(titleC.r, titleC.g, titleC.b, 0);
                homeTitle[1].color = new Color(titleC.r, titleC.g, titleC.b, 0);
            }
            yield return null;
        }

        yield return new WaitForSeconds(1);

        wantedColor = 1;
        while (homeSlideText.color.a != wantedColor)
        {
            finalColor = Mathf.Lerp(homeSlideText.color.a, wantedColor, 5 * Time.deltaTime);
            homeSlideText.color = new Color(homeSlideText.color.r, homeSlideText.color.g, homeSlideText.color.b, finalColor);
            if (homeSlideText.color.a > 0.95f)
            {
                homeSlideText.color = new Color(homeSlideText.color.r, homeSlideText.color.g, homeSlideText.color.b, 1);
            }
            yield return null;
        }

        isCoroutinePlaying = false;

        // Finger Slide 활성화
        fingerSlide.SetActive(true);

        // "Slide To Move" 문구가 뜬 후에, 클릭하라고 이미지를 표시해 유도하고 싶다.
        // - 만약 파괴가 되지 않았다면 실행
        if (null != playerRoundImg)
        {
            // - 알파값 증가 (투명->불투명)
            Color c = playerRoundImg.color;
            while (c.a < 1)
            {
                c.a += 0.1f;
                playerRoundImg.color = c;
                yield return new WaitForSeconds(0.1f);
            }
            c.a = 1;
            playerRoundImg.color = c;
        }
    }
    private void Home()
    {
        // 뒤로가기 버튼을 누르면 일시정지
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            PausePressKey();
        }

        // 탭을 하면
        if (playerMove.playerHitCheck)
        {
            // 상태 전환
            HomeToPlay();
        }

    }
    #endregion

    #region 플레이
    public void HomeToPlay()
    {
        // 초기화
        InitializeGame();

        StartCoroutine("PostBloom");
        StartCoroutine("BefroePlayCountDown");

        // 플레이어 둥글둥글 파괴
        Destroy(playerRound);

        // 상태저장
        nowState = 1;
        // Play 상태로 간다
        gState = GameState.Play;
        // UI
        UIManager.Instance.SetPageUI(UIManager.Page.Play);
    }

    IEnumerator BefroePlayCountDown()
    {
        // 3
        countText.text = "3";
        yield return new WaitForSeconds(1);
        // 2
        countText.text = "2";
        yield return new WaitForSeconds(1);
        // 1
        countText.text = "1";
        // 빛번짐 시작
        GameObject.Find("LightGOD").GetComponent<LightGOD>().enabled = true;
        yield return new WaitForSeconds(1);
        // DIE 체크
        youCanDead = true;
        countText.text = "";
        // 화면이 흐르고
        // 제목이 옆으로 흐르고
        // Slide to Start 옆으로 흐른다~
        camManager.enabled = true;

        // 특정 시간이후에 별이 생성되게 하고 싶다.
        yield return new WaitForSeconds(1f);
        HO_MoneyManager.Instance._eatMoney = true;
    }
    IEnumerator PostBloom()
    {
        while (postLerp < 15)
        {
            postLerp = Mathf.Lerp(post.profile.bloom.settings.bloom.intensity, 15, 3 * Time.deltaTime);
            setting.bloom.intensity = postLerp;
            yield return null;
        }
    }
    private void Play()
    {
        // 스코어가 100 이하라면
        if (ScoreManager.Instance.Score < 100)
        {
            //tutorialWorldCanvas.SetActive(true);
            // 플레이어를 조작하고 있지 않다면
            if (playerMove.buttonUpCheck)
            {
                fingerSlide.SetActive(true);
            }
            else
            {
                fingerSlide.SetActive(false);
            }
        }
        // 스코어가 100 초과라면
        else
        {
            //tutorialWorldCanvas.SetActive(false);
            fingerSlide.SetActive(false);
        }

        // 뒤로가기 버튼을 누르면 일시정지
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            PausePressKey();
        }
    }
    #endregion

    #region 일시정지
    private void Pause()
    {
        if (!isPause)
        {
            isPause = true;
            StopCoroutine("BefroePlayCountDown");
            youCanDead = false;
        }

        // 뒤로가기 버튼을 누르면 가장 초기의 일시정지 상태로 돌아감
        if (Input.GetKeyUp(KeyCode.Escape) && youCanPauseEsc)
        {
            PausePressKey();
        }
    }

    public void ContinueGameProcess()
    {
        StartCoroutine("AgainPlay");
    }
    IEnumerator AgainPlay()
    {
        // Play 상태로 간다
        gState = GameState.Play;
        // UI
        UIManager.Instance.SetPageUI(UIManager.Page.Play);

        // Die 체크 끔
        youCanDead = false;
        // 플레이어 잡을 수 있다
        playerMove.enabled = true;

        // 3
        countText.text = "3";
        yield return new WaitForSecondsRealtime(1);
        // 2
        countText.text = "2";
        yield return new WaitForSecondsRealtime(1);
        // 1
        // 빛번짐 시작
        Time.timeScale = 1.0f;
        GameObject.Find("LightGOD").GetComponent<LightGOD>().enabled = true;
        countText.text = "1";
        isPause = false;
        yield return new WaitForSecondsRealtime(1);

        // DIE 체크
        youCanDead = true;
        countText.text = "";

        // 화면이 흐르고
        // 제목이 옆으로 흐르고
        // Slide to Start 옆으로 흐른다~
        camManager.enabled = true;
    }
    #endregion

    #region 게임오버
    public void GameOverProcess()
    {
        // 더 월드!
        Time.timeScale = 0.0f;

        // 상태저장
        nowState = 2;

        // UI
        UIManager.Instance.SetPageUI(UIManager.Page.Over);
        fingerSlide.SetActive(false);

        // 다이상태 효과음
        AudioManager.Instance.DiePlay((AudioManager.DieSound)UnityEngine.Random.Range(0, 2));

        //  만약 탑 스코어보다 현재 스코어가 높다면
        if (ScoreManager.Instance.Score > ScoreManager.Instance.topScore)
        {
            // 탑스코어 = 현재 스코어
            ScoreManager.Instance.topScore = ScoreManager.Instance.Score;
            // 기기에 저장
            PlayerPrefs.SetInt("TopScore", ScoreManager.Instance.topScore);
            // 탑스코어 texet 갱신
            for (int i = 0; i < ScoreManager.Instance.topScoreText.Length; i++)
            {
                ScoreManager.Instance.topScoreText[i].text = "" + ScoreManager.Instance.topScore;
            }

            //// 랭킹에 저장
            //GameCenterManager.Instance.ReportScore(ScoreManager.Instance.topScore);
        }

        // 게임오버 상태로 간다
        gState = GameState.Over;
    }

    private void Over()
    {
        // 재시작 ==> Play 씬 다시 불러오기
        // 중지하고 홈화면으로 돌아가기 ==> Home 상태로, Home 씬 다시 불러오기
        // 최고점수 나타난다
        // 뒤로가기 버튼을 누르면 일시정지
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            PausePressKey();
        }

    }
    #endregion


    public void PausePressKey()
    {
        StopCoroutine("AgainPlay");
        countText.text = "";

        // 사운드 재생
        AudioManager.Instance.UiPlay((AudioManager.UiSound)UnityEngine.Random.Range(0, 5));

        // 튜토리얼 체크
        TutorialManager.Instance.CheckTutorial_Button();

        // 비활성화
        fingerSlide.SetActive(false);
        UIManager.Instance.SetPageUI(UIManager.Page.Pause);

        // 시간이 멈춘다
        Time.timeScale = 0.0f;
        // 플레이어멈춤
        if (playerMove != null)
            playerMove.enabled = false;
        // Pause 상태로 간다
        gState = GameState.Pause;
    }

    // Android 에서 BackKey를 누르면 뒤로 이동(Setting 화면)하고 종료시키고 싶다.
    void AndroidBackKey()
    {
        // ========= 호진 ==========
        // 1. 만약 처음 들어왔다면 firstcount를 1로 만들어준다.
        if (escDown && UIManager.Instance._currentPage != UIManager.Page.Pause && firstCount == 0)
        {
            print("처음 옴");
            firstCount=1;
        }
        // 2. pause Page에서 한 번 더 뒤로가기를 눌렀을 경우(firstcount == 1), 토스트메시지 출력한다.
        //  - 현재 Page 가 Setting 이라면 토스트 메시지를 한번 띄워주고 그 상태에서 한 번 더 누르면 종료시킨다
        else if (escDown && UIManager.Instance._currentPage == UIManager.Page.Pause && firstCount == 1)
        {
            print("토스트메시지 띄움");
            // toast message가 출력되는 동안 체크 
            StopCoroutine("CheckTimeToastMessage");
            StartCoroutine("CheckTimeToastMessage");
            HO_ToastMessage.Instance.ShowToastOnUiThread("Press the Back button one more time to end the game");
        }
        // 3. 토스트메시지가 띄어진 상태(firsutcount==2)에서 뒤로가기버튼을 누르면 종료된다.
        //  - ToastMessage 가 띄어진 상태에서 한 번 더 뒤로가기 버튼을 누르면 종료
        else if (escDown && UIManager.Instance._currentPage == UIManager.Page.Pause && firstCount == 2)
        {
            print("종료");
            // 토스트메시지 취소
            //GameObject.Find("ToastManager").GetComponent<HO_ToastMessage>().CancleToastOnUiThread();
            HO_ToastMessage.Instance.CancleToastOnUiThread();
            // App 종료
            Application.Quit();
        }
    }
    // ToastMessage 가 뜬 상태에서 뒤로가기 버튼을 누르면 종료시키고 싶다 
    IEnumerator CheckTimeToastMessage()
    {
        // toast message 가 출력되는 동안 == firstCount가 2인 경우
        firstCount = 2;
        yield return new WaitForSecondsRealtime(1.5f);
        // toast message 가 출력이 끝나고 난 후 firstCount 1으로 초기화해 토스트메시지가 뜰 수 있도록 한다.
        firstCount = 1;
    }
}
