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
        // 일단 인게임 세팅 초기화
        // 이어하기 데이터 초기화용 스탯 설정
        PlayerPrefs.SetFloat("World", 1);
        PlayerPrefs.SetInt("Stage", 1);

        // 초기 스탯 설정

        // 초기 스탯
        PlayerPrefs.SetInt("Speed", 10);
        PlayerPrefs.SetInt("DEF", 0);
        PlayerPrefs.SetInt("ATK", 10);
        PlayerPrefs.SetInt("maxHP", 40);
        PlayerPrefs.SetInt("HP", 40);

        // 초기 버프 및 장비 상태
        PlayerPrefs.SetString("Buff", "");               // 버프 id 없을 땐 ""
        PlayerPrefs.SetString("Equipment", "0000,0000,0000");    // 순서대로 4자리씩 종류(helmet, armor, shoes)와 equip type, 해당 값이 0000인 경우 None

        // 초기 무기 상태
        PlayerPrefs.SetString("Weapon", "555,555,555,555,555,555,555,555,555,555");    // 순서대로 3자리씩 Weapon type, 값이 555인 경우 bare_fist
        PlayerPrefs.SetString("WeaponATK", "0,0,0,0,0,0,0,0,0,0");    // 무기 공격력

        // 초기 아티팩트 상태
        PlayerPrefs.SetString("Artifact", "99,99,99");  // 순서대로 2자리씩 Artifact type, 값이 99인 경우 None



        // 업적, 능력 해금, 그 외 추가 기능 등을 아예 초기화
        // 해당 기능은, 추후에 추가해야 함


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
        PlayerPrefs.SetString("Buff", ingamecs.NowBuff);               // 앞 1자리 버프 id 부여, 값 없으면 Null
        PlayerPrefs.SetString("Equipment", $"{ingamecs.NowEquip[0]},{ingamecs.NowEquip[1]},{ingamecs.NowEquip[2]}");    // 순서대로 4자리씩 종류(helmet, armor, shoes)와 equip type, 해당 값이 0000인 경우 None

        // 초기 무기 상태
        PlayerPrefs.SetString("Weapon", $"{ingamecs.NowWeapon[0]},{ingamecs.NowWeapon[1]},{ingamecs.NowWeapon[2]},{ingamecs.NowWeapon[3]},{ingamecs.NowWeapon[4]}," +
            $"{ingamecs.NowWeapon[5]},{ingamecs.NowWeapon[6]},{ingamecs.NowWeapon[7]},{ingamecs.NowWeapon[8]},{ingamecs.NowWeapon[9]}");    // 순서대로 3자리씩 Weapon type, 값이 555인 경우 bare_fist
        PlayerPrefs.SetString("WeaponATK", $"{ingamecs.NowWeaponATK[0]},{ingamecs.NowWeaponATK[1]},{ingamecs.NowWeaponATK[2]},{ingamecs.NowWeaponATK[3]},{ingamecs.NowWeaponATK[4]}," +
            $"{ingamecs.NowWeaponATK[5]},{ingamecs.NowWeaponATK[6]},{ingamecs.NowWeaponATK[7]},{ingamecs.NowWeaponATK[8]},{ingamecs.NowWeaponATK[9]},");    // 무기 공격력

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
