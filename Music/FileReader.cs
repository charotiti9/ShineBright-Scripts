using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class FileReader : MonoBehaviour
{

    #region 디렉토리/확장자 관련
    // 루트가 되는 디렉토리를 stirng값으로 저장해놓기
    string[] androidRootlDirs = new string[]
    {
        "/storage",
        "/sdcard",
        "/mnt/sdcard",
        "/mnt/extSdCard"
    };
    string[] windowRootlDirs = new string[]
    {
        @"E:\"
    };

    // 찾아올 파일 확장자들
    string[] fildTypes = new string[]
    {
        "*.mp3",
        "*.wav"
    };
    // 세부 디렉토리
    // 파일정보
    DirectoryInfo[] rootFolder;
    FileInfo[] subFile;
    #endregion

    // 오디오 소스
    AudioSource audios;
    // 노래목록
    List<string> rootPaths = new List<string>();
    List<string> clipPaths = new List<string>();
    [HideInInspector]
    public List<AudioClip> clips = new List<AudioClip>();

    // 저장용
    string tempString = "";
    string[] clipsFullPaths;

    public Transform gridImage;
    // 버튼 풀
    public GameObject customClipButtonFactory;
    public GameObject defaultClipButtonFactory;
    int clipListCount = 10;
    GameObject[] customClipButtonPool;
    List<GameObject> customPlusClips = new List<GameObject>();
    GameObject[] defaultClipButtonPool;

    // 중복인지 검사
    bool isTwice;



    // UI
    public MyUiText searchMusicText;
    public Text searchMusic;

    // 싱글톤
    public static FileReader Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    // Use this for initialization
    void Start()
    {
        audios = GameObject.Find("AudioManager").GetComponent<AudioSource>();

        // 클립버튼 풀
        customClipButtonPool = new GameObject[clipListCount];
        for (int i = 0; i < clipListCount; i++)
        {
            GameObject customClipButton = Instantiate(customClipButtonFactory, gridImage);
            customClipButtonPool[i] = customClipButton;
            customClipButtonPool[i].SetActive(false);
        }
        defaultClipButtonPool = new GameObject[AudioManager.Instance.bgmClips.Length];
        for (int i = 0; i < defaultClipButtonPool.Length; i++)
        {
            GameObject defaultClipButton = Instantiate(defaultClipButtonFactory, gridImage);
            defaultClipButtonPool[i] = defaultClipButton;
            defaultClipButtonPool[i].SetActive(false);
        }

        // 경로 불러오기
        if (PlayerPrefs.HasKey("ClipsPaths"))
        {
            clipsFullPaths = PlayerPrefs.GetString("ClipsPaths").Split(',');

            // 패스가 비지 않았다면
            if (clipsFullPaths[0] != "")
            {
                for (int i = 0; i < clipsFullPaths.Length; i++)
                {
                    clipPaths.Add(clipsFullPaths[i]);
                }
            }
        }
        else
        {
            PlayerPrefs.SetString("ClipsPaths", "");
        }

    }

    public void SearchYes_Button()
    {
        AudioManager.Instance.UiPlay((AudioManager.UiSound)UnityEngine.Random.Range(0, 5));
        GetComponent<MusicUICanvas_B>().SetPageUI(MusicUICanvas_B.MusicPage.SearchMusic);
        StartCoroutine("SearchMusic");
    }

    // 텍스트와 함께 검색
    IEnumerator SearchMusic()
    {
        // 커스텀 클립 없애기
        DeleteCustomClipList();

        // 지금까지의 리스트 제거
        rootPaths.Clear();
        clipPaths.Clear();
        clips.Clear();
        customPlusClips.Clear();

        // 리스트 작성중 text 활성화
        searchMusicText.gameObject.SetActive(true);
        searchMusic.gameObject.SetActive(true);

        #region 루트폴더 검색
        // 만약 안드로이드라면
        if (Application.platform == RuntimePlatform.Android)
        {
            searchMusicText.UID = "SEARCHFOLDER";
            yield return new WaitForSecondsRealtime(1f);

            // 길이만큼 검색
            for (int i = 0; i < androidRootlDirs.Length; i++)
            {
                // 존재한다면
                if (Directory.Exists(androidRootlDirs[i]))
                {
                    // 존재하는 파일경로 저장
                    rootPaths.Add(androidRootlDirs[i]);
                    //musicText.text = "루트파일 " + i + "번 째: " + androidRootlDirs[i];
                    //yield return new WaitForSecondsRealtime(2f);
                }
                else
                {
                    //musicText.text = "루트파일 " + i + "번 째: " + androidRootlDirs[i] + "는 없습니다.";
                    //yield return new WaitForSecondsRealtime(2f);
                }
            }
        }
        // 윈도우라면
        else if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
        {
            searchMusicText.UID = "SEARCHFOLDER";
            yield return new WaitForSecondsRealtime(1f);

            for (int i = 0; i < windowRootlDirs.Length; i++)
            {
                if (Directory.Exists(windowRootlDirs[i]))
                {
                    rootPaths.Add(windowRootlDirs[i]);
                    //musicText.text = "루트파일 " + i + "번 째: " + windowRootlDirs[i];
                    //yield return new WaitForSecondsRealtime(0.5f);
                }
                else
                {
                    //musicText.text = "루트파일 " + i + "번 째: " + windowRootlDirs[i] + "는 없습니다.";
                    //yield return new WaitForSecondsRealtime(0.5f);
                }
            }
        }
        #endregion

        //musicText.text = "루트파일 검사 완료, 갯수: " + rootPaths.Count;
        //yield return new WaitForSecondsRealtime(1f);

        searchMusicText.UID = "SEARCHCLIP";
        yield return new WaitForSecondsRealtime(1f);

        #region 서브폴더의 클립 저장
        // 루트 폴더의 크기 지정
        rootFolder = new DirectoryInfo[rootPaths.Count];

        // 루트 패스의 숫자만큼 반복
        for (int i = 0; i < rootPaths.Count; i++)
        {
            // string으로 된 Path를 디렉토리인포로 변환하여 저장
            rootFolder[i] = new DirectoryInfo(rootPaths[i]);

            // 파일의 타입만큼 반복
            for (int j = 0; j < fildTypes.Length; j++)
            {
                //musicText.text = rootFolder[i] + "의 하위 폴더 중" + fildTypes[j] + "확장자를 검색중입니다.";
                //yield return new WaitForSecondsRealtime(1f);

                // 서브파일을 저장
                subFile = rootFolder[i].GetFiles(fildTypes[j], SearchOption.AllDirectories);

                //musicText.text = rootFolder[i] + "의 하위 폴더 중" + fildTypes[j] + " 확장자의 갯수: " + subFile.Length;
                //musicText.text = "found " + subFile.Length + " '" + fildTypes[j] + "' files";
                //yield return new WaitForSecondsRealtime(0.5f);

                // 서브파일의 갯수만큼 반복
                foreach (FileInfo file in subFile)
                {
                    // 저장된 클립패스 수만큼 반복
                    for (int k = 0; k < clipPaths.Count; k++)
                    {
                        // 파일의 이름이
                        // clips[].name 중에 같은 것이 없을 때만
                        if (file.Name == Path.GetFileName(clipPaths[k]))
                        {
                            isTwice = true;
                            break;
                        }
                    }

                    // 중복이 아닐때만 저장
                    if (!isTwice)
                    {
                        // 서브 폴더에 있는 파일경로값을 저장
                        clipPaths.Add(file.FullName);

                        searchMusicText.UID = "SEARCHED";
                        searchMusic.text = Path.GetFileName(clipPaths[clipPaths.Count - 1]);
                        yield return new WaitForSecondsRealtime(0);
                    }

                    // 초기화
                    isTwice = false;
                    yield return new WaitForSecondsRealtime(0);
                }
                searchMusicText.UID = "SEARCHING";
                searchMusic.text = "";
                yield return new WaitForSecondsRealtime(0);
            }
            yield return new WaitForSecondsRealtime(0);
        }
        #endregion

        // 플레이어 프랩스에 저장
        ClipsPathSave();

        if (clipPaths.Count <= 0)
        {
            searchMusicText.UID = "SEARCHDONE";
            searchMusic.text = "" + clipPaths.Count;
            yield return new WaitForSecondsRealtime(1f);
            searchMusic.text = "";
        }
        else
        {
            searchMusicText.UID = "SEARCHDONE";
            searchMusic.text = "" + clipPaths.Count;
            yield return new WaitForSecondsRealtime(1f);
            searchMusicText.UID = "SEARCHCREATE";
            searchMusic.text = "";
        }
        yield return new WaitForSecondsRealtime(1f);

        // UI
        GetComponent<MusicUICanvas_B>().SetPageUI(MusicUICanvas_B.MusicPage.CustomList);

        // 클립 리스트 설정
        CheckCustomClipList();

        // 리스트 작성중 text 비활성화
        searchMusicText.gameObject.SetActive(false);
        searchMusic.gameObject.SetActive(false);

        // 다시 일시정지 가능
        GameManager.Instance.youCanPauseEsc = true;
    }


    // 클립의 패스 저장하기(PlayerPrefs)
    void ClipsPathSave()
    {
        tempString = "";
        // 저장하기
        // 클립의 full.Name를 저장해놓자
        // 클립의 갯수만큼 반복
        for (int i = 0; i < clipPaths.Count; i++)
        {
            // 클립의 임시string += full.Name
            tempString += clipPaths[i];
            // 마지막 거엔 쉼표를 붙이지 않는다.
            if (i < clipPaths.Count - 1)
            {
                // 쉼표를 붙인다
                tempString += ",";
            }
        }

        PlayerPrefs.SetString("ClipsPaths", tempString);
    }

    // 디폴트 클립 리스트 만들기
    public void CheckDefaultClipList()
    {
        // clipButtonPool를 활성화
        for (int i = 0; i < defaultClipButtonPool.Length; i++)
        {
            defaultClipButtonPool[i].SetActive(true);
            // 이름 바꿔주기
            defaultClipButtonPool[i].GetComponentInChildren<Text>().text = AudioManager.Instance.bgmClips[i].name;
            // 클립 패스 넘겨주기
            defaultClipButtonPool[i].GetComponentInChildren<ClipSelectButton>().musicClip = AudioManager.Instance.bgmClips[i];
        }

        // 비활성화
        // 만약 clipListCount의 숫자가 clips.Count보다 작다면
        if (clipListCount >= clipPaths.Count)
        {
            // clips.Count만큼 clipButtonPool를 활성화
            for (int i = 0; i < clipPaths.Count; i++)
            {
                customClipButtonPool[i].SetActive(false);
            }
        }
        // 만약 클립의 수가 배열 수를 넘는다면
        else if (clipListCount < clipPaths.Count)
        {
            // 모든 clipButtonPool를 활성화
            for (int i = 0; i < clipListCount; i++)
            {
                customClipButtonPool[i].SetActive(false);
            }
            // 추가적으로 생성 (clips.Count - clipListCount)된 애들은 지워준다
            for (int i = 0; i < customPlusClips.Count; i++)
            {
                Destroy(customPlusClips[i]);
            }
        }

    }
    // 커스텀 클립 리스트 만들기
    public void CheckCustomClipList()
    {
        // 만약 clipListCount의 숫자가 clips.Count보다 작다면
        // 클립 패스의 갯수가 0이 아니라면
        if (clipListCount >= clipPaths.Count && clipPaths.Count != 0)
        {
            // clips.Count만큼 clipButtonPool를 활성화
            for (int i = 0; i < clipPaths.Count; i++)
            {

                customClipButtonPool[i].SetActive(true);
                // 이름 바꿔주기
                customClipButtonPool[i].GetComponentInChildren<Text>().text = Path.GetFileName(clipPaths[i]);
                // 클립 패스 넘겨주기
                customClipButtonPool[i].GetComponentInChildren<ClipSelectButton>().musicPath = clipPaths[i];
            }
        }
        // 만약 클립의 수가 배열 수를 넘는다면
        else if (clipListCount < clipPaths.Count)
        {
            // 모든 clipButtonPool를 활성화
            for (int i = 0; i < clipListCount; i++)
            {
                customClipButtonPool[i].SetActive(true);
                // 이름 바꿔주기
                customClipButtonPool[i].GetComponentInChildren<Text>().text = Path.GetFileName(clipPaths[i]);
                // 클립값 넘겨주기
                customClipButtonPool[i].GetComponentInChildren<ClipSelectButton>().musicPath = clipPaths[i];
            }
            // 추가적으로 생성 (clips.Count - clipListCount)
            for (int i = clipListCount; i < clipPaths.Count; i++)
            {
                GameObject clipButtonPlus = Instantiate(customClipButtonFactory, gridImage);
                clipButtonPlus.SetActive(true);
                // 추가된 아이들 리스트에 넣어주기
                customPlusClips.Add(clipButtonPlus);
                // 이름 바꿔주기
                clipButtonPlus.GetComponentInChildren<Text>().text = Path.GetFileName(clipPaths[i]);
                // 클립값 넘겨주기
                clipButtonPlus.GetComponentInChildren<ClipSelectButton>().musicPath = clipPaths[i];
            }
        }

        // clipButtonPool를 비활성화
        for (int i = 0; i < defaultClipButtonPool.Length; i++)
        {
            defaultClipButtonPool[i].SetActive(false);
        }

    }

    // 디폴트 클립 없애기
    public void DeleteDefaultClipList()
    {
        // 모든 clipButtonPool를 비활성화
        for (int i = 0; i < defaultClipButtonPool.Length; i++)
        {
            defaultClipButtonPool[i].SetActive(false);
        }
    }
    // 커스텀 클립 없애기
    public void DeleteCustomClipList()
    {
        // 모든 clipButtonPool를 비활성화
        for (int i = 0; i < customClipButtonPool.Length; i++)
        {
            customClipButtonPool[i].SetActive(false);
        }
        // 추가적으로 생성 (clips.Count - clipListCount)된 애들은 지워준다
        for (int i = 0; i < customPlusClips.Count; i++)
        {
            Destroy(customPlusClips[i]);
        }
        
    }
}
