using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Recodecs : MonoBehaviour
{
    float[,] RecordState = new float[5, 6];             // 월드, 스테이지, 체력, 스피드, 공격력, 방어력
    string[] RecordWeapon = new string[5];              // 무기 10개
    string[] RecordEquip = new string[5];               // 장비 3개
    string[] RecordArtifact = new string[5];            // 아티펙트 3개

    int RecordCount = 0;


    public GameObject[] WandSTxt = new GameObject[5];
    public GameObject[] HPTxt = new GameObject[5];


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    // 레코드 패널 시각화
    public void LoadRecord()
    {
        SaveRecord();

        for (int i = 0; i < RecordCount; i++)
        {
            WandSTxt[i].GetComponent<Text>().text = $"World{(int)RecordState[i,0]}_{(int)RecordState[i, 1]}";
            HPTxt[i].GetComponent<Text>().text = $"{(int)RecordState[i, 2]}";
        }
        for (int i = RecordCount; i < 5; i++)
        {
            WandSTxt[i].GetComponent<Text>().text = "None";
            HPTxt[i].GetComponent<Text>().text = "None";
        }
    }


    // 게임 오버 시 레코드 기록
    public void SaveRecord()
    {
        if (PlayerPrefs.GetInt("SaveOnOff") == 0) return;
        else if (PlayerPrefs.GetInt("HP") != 0) return;

        Debug.Log("Save Record");

        var list = new List<int> { RecordCount - 1, 3 };
        Debug.Log("min 값 : " + list.Min());

        // 이전 기록 덮어씌우기
        for (int i = list.Min(); i >= 0; i--)
        {
            RecordWeapon[i + 1] = RecordWeapon[i];
            RecordEquip[i + 1] = RecordEquip[i];
            RecordArtifact[i + 1] = RecordArtifact[i];

            for (int j = 0; j < 6; j++)
            {
                RecordState[i + 1, j] = RecordState[i, j];
            }
        }

        // 새로운 기록 씌우기
        if (RecordCount < 5) RecordCount++;


        // State 기록
        RecordState[0, 0] = PlayerPrefs.GetFloat("World");
        RecordState[0, 1] = PlayerPrefs.GetInt("Stage");
        RecordState[0, 2] = PlayerPrefs.GetInt("maxHP");
        RecordState[0, 3] = PlayerPrefs.GetInt("Speed");
        RecordState[0, 4] = PlayerPrefs.GetInt("ATK");
        RecordState[0, 5] = PlayerPrefs.GetInt("DEF");

        // 무기 장비 및 아티펙트 기록
        RecordWeapon[0] = PlayerPrefs.GetString("Weapon");
        RecordEquip[0] = PlayerPrefs.GetString("Equipment");
        RecordArtifact[0] = PlayerPrefs.GetString("Artifact");

        BestRecord();

        PlayerPrefs.SetInt("SaveOnOff", 0);


        Debug.Log($"Save Record {RecordState[0, 0]}_{RecordState[0, 1]}");

    }

    // 최고 기록 경신 체크
    void BestRecord()
    {
        if (RecordState[0, 0] >= PlayerPrefs.GetFloat("BestWorld") && RecordState[0, 1] > PlayerPrefs.GetFloat("BestStage"))
        {
            PlayerPrefs.SetInt("BestWorld", (int)RecordState[0, 0]);
            PlayerPrefs.SetInt("BestStage", (int)RecordState[0, 1]);
            PlayerPrefs.SetInt("BestMaxHP", (int)RecordState[0, 2]);
            PlayerPrefs.SetInt("BestSpeed", (int)RecordState[0, 3]);
            PlayerPrefs.SetInt("BestATK", (int)RecordState[0, 4]);
            PlayerPrefs.SetInt("BestDEF", (int)RecordState[0, 5]);
            PlayerPrefs.SetString("BestWeapon", $"{RecordWeapon[0]}");    // 순서대로 3자리씩 Weapon type, 값이 555인 경우 bare_fist
            PlayerPrefs.SetString("BestEquipment", $"{RecordEquip[0]}");    // 순서대로 4자리씩 종류(helmet, armor, shoes)와 equip type, 해당 값이 0000인 경우 None
            PlayerPrefs.SetString("BestArtifact", $"{RecordArtifact[0]}");  // 순서대로 2자리씩 Artifact type, 값이 99인 경우 None
        }
    }
}
