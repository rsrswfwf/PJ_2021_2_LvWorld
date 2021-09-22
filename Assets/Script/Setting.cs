using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Setting : MonoBehaviour
{

    // intro Skip 기능용 오브젝트
    public GameObject introskipBtn;
    public Sprite skipon, skipoff;

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.GetInt("intro_skip") == 0) introskipBtn.GetComponent<Image>().sprite = skipon;
        else introskipBtn.GetComponent<Image>().sprite = skipoff;
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

    // 구글 로그인
    public void GoogleLogin()
    {
        // 현재 플레이어의 이어하기 및 티켓, record 데이터를 불러와야 합니다.

        if (PlayerPrefs.GetInt("Playing") == 1)         // 현재 플레이 중인 게임이 있을 경우
        {
            Debug.Log("현재 진행하던 게임을 종료하고 데이터를 덮어씌웁니까? \n (주의) 현재 저장된 이어하기 데이터가 날아갑니다.");
        }
    }
}
