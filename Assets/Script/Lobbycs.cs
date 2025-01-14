﻿using GooglePlayGames;
using GooglePlayGames.BasicApi;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Lobbycs : MonoBehaviour
{
    Recodecs recodecs;
    Setting setting;

    // Start is called before the first frame update
    void Start()
    {
        recodecs = GameObject.Find("EventSystem").GetComponent<Recodecs>();

        PlayerPrefs.SetInt("LogInOut", 0);          // 로그인 여부
        PlayerPrefs.SetInt("intro_skip", 0);        // 인트로 스킵여부
        PlayerPrefs.SetInt("Playing", 0);           // 플레이 중인지 확인용

        if (PlayerPrefs.GetInt("logInOut") == 0)    // 로그인 여부
        {

        }
        else
        {
            // 데이터 가져온 후 이어하기 데이터에 맞는 플레이어 스탯 구현해주어야 합니다. (여기서 말고 로그인 시)
        }

        // 베스트 기록 초기화용
        PlayerPrefs.SetInt("BestWorld", 0);
        PlayerPrefs.SetInt("BestStage", 0);
        PlayerPrefs.SetInt("BestMaxHP", 0);
        PlayerPrefs.SetInt("BestSpeed", 0);
        PlayerPrefs.SetInt("BestATK", 0);
        PlayerPrefs.SetInt("BestDEF", 0);
        PlayerPrefs.SetString("BestWeapon", "555,555,555,555,555,555,555,555,555,555");    // 순서대로 3자리씩 Weapon type, 값이 555인 경우 bare_fist
        PlayerPrefs.SetString("BestEquipment", "0000,0000,0000");    // 순서대로 4자리씩 종류(helmet, armor, shoes)와 equip type, 해당 값이 0000인 경우 None
        PlayerPrefs.SetString("BestArtifact", "99,99,99");  // 순서대로 2자리씩 Artifact type, 값이 99인 경우 None

    }

    // 스탯 초기화
    public void StateReset()
    {
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
        PlayerPrefs.SetInt("Gold", 0);

        // 초기 버프 및 장비 상태
        PlayerPrefs.SetString("Buff", "");               // 버프 id 없을 땐 ""
        PlayerPrefs.SetString("Equipment", "0000,0000,0000");    // 순서대로 4자리씩 종류(helmet, armor, shoes)와 equip type, 해당 값이 0000인 경우 None

        // 초기 무기 상태
        PlayerPrefs.SetString("Weapon", "555,555,555,555,555,555,555,555,555,555");    // 순서대로 3자리씩 Weapon type, 값이 555인 경우 bare_fist
        PlayerPrefs.SetString("WeaponATK", "0,0,0,0,0,0,0,0,0,0");    // 무기 공격력

        // 초기 아티팩트 상태
        PlayerPrefs.SetString("Artifact", "99,99,99");  // 순서대로 2자리씩 Artifact type, 값이 99인 경우 None

        // 초기 몬스터 상태
        PlayerPrefs.SetString("MName", "Monster Name");
        PlayerPrefs.SetInt("MSpeed", 0);
        PlayerPrefs.SetInt("MDEF", 0);
        PlayerPrefs.SetInt("MATK", 0);
        PlayerPrefs.SetInt("MmaxHP", 0);
        PlayerPrefs.SetInt("MHP", 0);
        PlayerPrefs.SetInt("MGetGold", 0);

    }

    // 새로하기 버튼 기능
    public void NewStart()
    {
        recodecs.SaveRecord();
        StateReset();

        PlayerPrefs.SetInt("SaveOnOff", 1);

        // 인트로 스킵 여부 체크
        if (PlayerPrefs.GetInt("intro_skip") == 0)
        {
            Debug.Log("인트로 실행");
        }
        SceneManager.LoadScene("02_InGame");
    }

    // 이어하기 버튼 기능
    public void LoadStart()
    {
        if (PlayerPrefs.GetInt("HP") == 0)
        {
            NewStart();
            return;
        }

        SceneManager.LoadScene("02_InGame");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
