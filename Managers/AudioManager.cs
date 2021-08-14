using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour {

    // 스스로 몇개 있는지 검사
    GameObject[] audioGameObject;

    // 오디오들
    [HideInInspector]
    public AudioSource[] audios;

    // 일시정지 때 저장해 놓을 볼륨
    static float vol_0 = 0.5f;
    static float vol_1 = 0.5f;

    // 클립들
    public AudioClip[] bgmClips;
    public AudioClip[] rainClips;
    public AudioClip[] uiClips;
    public AudioClip[] dieClips;

    // 슬라이더
    static Slider[] volumeBar = new Slider[2];
    public Slider[] _volumeBar = new Slider[2];

    #region 사운드 관리
    public enum BGSounds
    {
        BG_1,
        BG_2,
        BG_3
    }

    public enum RainSound
    {
        Rain
    }

    public enum UiSound
    {
        UiSound1,
        UiSound2,
        UiSound3,
        UiSound4,
        UiSound5,
        shutter1,
        shutter2,
        shutter3
    }

    public enum DieSound
    {
        Thunder1,
        Thunder2
    }
    #endregion

    // 싱글톤
    public static AudioManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        audioGameObject = GameObject.FindGameObjectsWithTag("AudioManager");

        if (audioGameObject.Length > 1)
        {
                Destroy(this.gameObject);
        }

        // 씬이 재로드 될 때, 이건 유지된다
        // 왜? 음악이 계속 이어지게 하기 위해서
        DontDestroyOnLoad(this.gameObject);

        audios = GetComponents<AudioSource>();
        for (int i = 0; i < volumeBar.Length; i++)
        {
            volumeBar[i] = _volumeBar[i];
        }

        // 재시작 시에 볼륨바의 값 조정
        volumeBar[0].value = vol_0;
        volumeBar[1].value = vol_1;
    }

    // Use this for initialization
    void Start () {
        // 배경음과 빗소리 기본음향
        audios[0].volume = vol_0;
        audios[1].volume = vol_1;
    }
	
    // BGM 관리는 0번째 오디오 소스가 한다
	public void BGPlay(BGSounds bgm)
    {
        if (audios[0].clip != bgmClips[(int)bgm])
        {
            audios[0].clip = bgmClips[(int)bgm];
            audios[0].Play();
        }
    }

    // 빗소리는 1번째 오디오 소스가 한다
    public void RainPlay(RainSound rain)
    {
        if (audios[1].clip != rainClips[(int)rain])
        {
            audios[1].clip = rainClips[(int)rain];
            audios[1].Play();
        }
    }

    // Ui사운드 관리는 3번째 오디오 소스가 한다
    public void UiPlay(UiSound ui)
    {
        audios[2].Stop();
        audios[2].clip = uiClips[(int)ui];
        audios[2].Play();
    }

    // Die사운드 관리는 4번째 오디오 소스가 한다
    public void DiePlay(DieSound die)
    {
        audios[3].Stop();
        audios[3].clip = dieClips[(int)die];
        audios[3].Play();
    }


    // 몇번째 오디오 소스를 멈출까?
    public void StopSounds(int soundNum)
    {
        audios[soundNum].Stop();
    }

    private void Update()
    {
        if (GameManager.Instance.gState == GameManager.GameState.Pause)
        {
            audios[0].volume = volumeBar[0].value;
            audios[1].volume = volumeBar[1].value;
            vol_0 = volumeBar[0].value;
            vol_1 = volumeBar[1].value;
        }

    }
}
