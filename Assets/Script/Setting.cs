using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SceneManagement;

public class Setting : MonoBehaviour
{

    // Player State    ex) Now ..
    InGamecs ingamecs;

    // intro Skip 기능용 오브젝트
    public GameObject introskipBtn;
    public Sprite skipon, skipoff;

    // 구글 로그인 용 변수 및 텍스트
    public Text GoogleTitle, GoogleExplain;
    // private bool bWaitingForAuth = false;

    // Start is called before the first frame update
    void Start()
    {
        ingamecs = GameObject.Find("EventSystem").GetComponent<InGamecs>();

        if (PlayerPrefs.GetInt("intro_skip") == 0) introskipBtn.GetComponent<Image>().sprite = skipon;
        else introskipBtn.GetComponent<Image>().sprite = skipoff;


        //구글 게임 서비스 활성화(초기화)
        PlayGamesPlatform.InitializeInstance(new PlayGamesClientConfiguration.Builder().Build());
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
    }

    // 인트로 스킵 버튼
    public void IntroSkip()
    {
        if (PlayerPrefs.GetInt("intro_skip") == 0)
        {
            PlayerPrefs.SetInt("intro_skip", 1);
            Debug.Log("Skip On");
            introskipBtn.GetComponent<Image>().sprite = skipoff;
        }
        else
        {
            PlayerPrefs.SetInt("intro_skip", 0);
            Debug.Log("Skip Off");
            introskipBtn.GetComponent<Image>().sprite = skipon;
        }
    }

    // 데이터 초기화 버튼
    public void DataReset()
    {
        // player 세팅 이후
    }

    // 저장 후 로비 화면으로 돌아가기
    public void Move_Lobby_And_Save()
    {
        // 이어하기 데이터 초기화용 스탯 설정
        PlayerPrefs.SetFloat("World", ingamecs.NowWorld);
        PlayerPrefs.SetInt("Stage", ingamecs.NowStage);

        // 초기 스탯 설정

        // 초기 스탯
        PlayerPrefs.SetInt("Speed", ingamecs.NowSpeed);
        PlayerPrefs.SetInt("DEF", ingamecs.NowDEF);
        PlayerPrefs.SetInt("ATK", ingamecs.NowATK);
        PlayerPrefs.SetInt("maxHP", ingamecs.NowmaxHP);
        PlayerPrefs.SetInt("HP", ingamecs.NowHP);

        // 초기 버프 및 장비 상태
        PlayerPrefs.SetString("Buff", ingamecs.NowBuff);               // 앞 1자리 [None :0, Buff :1, DeBuff:2], 뒤 2자리 [해당 버프, 디버프 번호]
        PlayerPrefs.SetString("Equipment", $"{ingamecs.NowEquip[0]},{ingamecs.NowEquip[1]},{ingamecs.NowEquip[2]}");    // 순서대로 4자리씩 종류(helmet, armor, shoes)와 equip type, 해당 값이 0000인 경우 None

        // 초기 무기 상태
        PlayerPrefs.SetString("Weapon", $"{ingamecs.NowWeapon[0]},{ingamecs.NowWeapon[1]},{ingamecs.NowWeapon[2]},{ingamecs.NowWeapon[3]},{ingamecs.NowWeapon[4]}," +
            $"{ingamecs.NowWeapon[5]},{ingamecs.NowWeapon[6]},{ingamecs.NowWeapon[7]},{ingamecs.NowWeapon[8]},{ingamecs.NowWeapon[9]}");    // 순서대로 3자리씩 Weapon type, 값이 555인 경우 bare_fist

        // 초기 아티팩트 상태
        PlayerPrefs.SetString("Artifact", $"{ingamecs.NowArtifact[0]},{ingamecs.NowArtifact[1]},{ingamecs.NowArtifact[2]}");  // 순서대로 2자리씩 Artifact type, 값이 99인 경우 None


        // Lobby로 돌아가기
        SceneManager.LoadScene("01_Lobby");
    }


    // 구글 로그인
    public void GoogleLogin()
    {
        // 현재 플레이어의 이어하기 및 티켓, record 데이터를 불러와야 합니다.

        //if (bWaitingForAuth)
        //    return;

        Debug.Log("GoogleLogin Not Return");

        if(!Social.localUser.authenticated)
        {
            Social.localUser.Authenticate((bool isSuccess) =>
            {
                if (isSuccess)
                {
                    Debug.Log("Login Success!");
                    GoogleTitle.GetComponent<Text>().text = $"{Social.localUser.userName}님 환영합니다!";
                    GoogleExplain.GetComponent<Text>().text = "현재 게임에 연결되셨습니다.";
                }
                else
                {
                    Debug.Log("Login Fail..");
                    GoogleTitle.GetComponent<Text>().text = $"계정 연결 실패";
                    GoogleExplain.GetComponent<Text>().text = "다시 시도해주세요.";
                }
            });
        }

        /*
        if (!Social.localUser.authenticated)
        {

            Debug.Log("CallBAck 실행");
            Social.localUser.Authenticate(AuthenticateCallback);
        }


        if (PlayerPrefs.GetInt("Playing") == 1)         // 현재 플레이 중인 게임이 있을 경우
        {
            Debug.Log("현재 진행하던 게임을 종료하고 데이터를 덮어씌웁니까? \n (주의) 현재 저장된 이어하기 데이터가 날아갑니다.");
        }
        */
    }

    // 콜백 함수
    void AuthenticateCallback(bool success)
    {
        if (success)
        {
            StartCoroutine(UserIDLoad());
        }
    }

    // 유저 이미지 받아오기
    IEnumerator UserIDLoad()
    {
        GoogleTitle.GetComponent<Text>().text = $"{Social.localUser.userName}님 환영합니다!";
        GoogleExplain.GetComponent<Text>().text = "현재 게임에 연결되셨습니다.";

        yield return null;

    }
}
