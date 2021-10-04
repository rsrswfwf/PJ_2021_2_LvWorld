using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class StagePlay : MonoBehaviour
{
    // 사용 확률 모음
    // 1. 스테이지 카드 확률 ( 일반, 보스 )
    int[] card_ran_per = new int[] { 70, 75, 80, 90, 100 };    // 0 = 몬스터, 1 = 보물상자, 2 = 버프, 3 = 랜덤 이벤트, 4 = 마을 NPC, 5 = 보스
    int[] boss_card_ran_per = new int[] { 76, 82, 88, 100 };   // 보스는 마을이 뜨지 않아 4를 제외하고, 5를 고정으로 가운데 카드에 넣습니다.
    // 2. 보물상자 확률 ( 함정, 몬스터, 체력 전부 회복, 체력 일정 회복, 장비 획득 )
    int[] box_ran_per = new int[] { 15, 25, 30, 45, 100 };
    // 3. 장비 드랍 확률 ( 월드 별 등급, 수식어, 종류, 무기/장비 부여 )
    int[] eq_world1_per = new int[] { 100 };
    int[] eq_world2_per = new int[] { 75, 100 };
    int[] eq_world3_per = new int[] { 58, 98, 100 };
    int[] eq_world4_per = new int[] { 35, 85, 100 };
    int[] eq_world5_per = new int[] { 15, 58, 98, 100 };
    int[] eq_world6_per = new int[] { 10, 40, 94, 99, 100 };
    int[] eq_world7_per = new int[] { 3, 28, 88, 98, 100 };
    int[] eq_world8_per = new int[] { 1, 21, 85, 97, 100 };
    int[] eq_worldx_per = new int[] { 1, 16, 80, 95, 100 };
    int[] eq_modifier_per = new int[] { 5, 25, 78, 98, 100 };
    int[] eq_what_per = new int[] { 20, 40, 60, 80, 100 };
    int[] eq_or_weapon_per = new int[] { 30, 100 };

    // Player State    ex) Now ..
    InGamecs ingamecs;
    InJson injson;

    // 0. 스테이지 카드 세팅 단계 변수
    GameObject Stage_card0, Stage_card1, Stage_card2;           // Stage Card Image
    int[] stage_card_num = new int[3];

    // 2. 카드 별 패널 띄움 변수
    public GameObject Monster_Panel, Box_Panel, Buff_Panel, Event_Panel, Town_Panel;   // 각 패널
    public GameObject Monster_name, MPanel_Title;
    public GameObject Txt_bpanel, Btn_bpanel, Img_bpanel;

    // 2. 상인 장비 리스트용 변수
    string[] eorwArray = new string[3];

    // 3. 스테이지 넘김용 변수
    bool bossClear = false;

    // Start is called before the first frame update
    void Start()
    {
        ingamecs = GameObject.Find("EventSystem").GetComponent<InGamecs>();
        injson = GameObject.Find("EventSystem").GetComponent<InJson>();
        Stage_card0 = GameObject.Find("FrontCard0");
        Stage_card1 = GameObject.Find("FrontCard1");
        Stage_card2 = GameObject.Find("FrontCard2");

        StageCheck();
    }

    // 0. 스테이지 카드 세팅
    public void StageCheck()
    {
        if (ingamecs.NowWorld == 1 && ingamecs.NowStage >= 15) BossCardSetting();
        else if (ingamecs.NowWorld <= 4 && ingamecs.NowStage >= 20) BossCardSetting();
        else if (ingamecs.NowWorld <= 6 && ingamecs.NowStage >= 25) BossCardSetting();
        else if (ingamecs.NowWorld <= 8 && ingamecs.NowStage >= 20) BossCardSetting();
        else CardSetting();

        // 카드 세팅 이후 이미지 변경 필요
    }

    // 보스 등장 이전 세팅
    void CardSetting()
    {
        int percent;

        for (int i = 0; i < 3; i++)
        {
            percent = Random.Range(1, 100);
            for (int j = 0; j < 5; j++)
            {
                if (percent <= card_ran_per[j])
                {
                    stage_card_num[i] = j;
                    break;
                }
            }
        }
    }

    // 보스 등장 이후 세팅
    void BossCardSetting()
    {
        int percent;

        for (int i = 0; i < 3; i++)
        {
            if (i == 1)
            {
                stage_card_num[i] = 5;
                continue;
            }

            percent = Random.Range(1, 100);
            for (int j = 0; j < 4; j++)
            {
                if (percent <= boss_card_ran_per[j])
                {
                    stage_card_num[i] = j;
                    break;
                }
            }
        }
    }



    // 1. 카드 선택 시 화면 이동
    public void CardSelect()
    {
        string ButtonName = EventSystem.current.currentSelectedGameObject.name;

        int select_card_num = int.Parse(ButtonName.Substring(9));

        if (stage_card_num[select_card_num] == 1) TresureBox();
        else if (stage_card_num[select_card_num] == 2) Buff();
        else if (stage_card_num[select_card_num] == 3) RandomEvent();
        else if (stage_card_num[select_card_num] == 4) CityIn();
        else VSMonster(stage_card_num[select_card_num]);
    }


    // 2. 종류 별 화면 이동

        // 2.1 몬스터&보스
    void VSMonster(int select_card_num)
    {
        Monster_Panel.SetActive(true);

        if (select_card_num == 5)
        {
            MPanel_Title.GetComponent<Text>().text = "보스 전투";
            Monster_name.GetComponent<Text>().text = "보스 OOO이 등장했습니다!";
            bossClear = true;
            Debug.Log("Boss");
        }
        else
        {
            MPanel_Title.GetComponent<Text>().text = "몬스터 전투";
            Monster_name.GetComponent<Text>().text = "몬스터 OOO이 등장했습니다!";
            Debug.Log("Monster");
        }

        NextStage();
    }

        // 2.2 보물상자
    void TresureBox()
    {
        Box_Panel.SetActive(true);
        Debug.Log("AddTresureBox");
        NextStage();
    }

    // 박스 관련 적용 ( 몬스터, 및 장비 획득 창으로 이동해야 함 )
    public void BoxApply()
    {
        int box_percent = Random.Range(1, 100);
        // 보물상자 확률 ( 함정, 몬스터, 체력 전부 회복, 체력 일정 회복, 장비 획득 )

        int hurt_percent = Random.Range(5, 10);
        int heal_percent = Random.Range(10, 20);

        for (int j = 0; j < 5; j++)
        {
            if (box_percent <= box_ran_per[j])
            {
                if (j == 0)         // 함정 ( 현재 체력을 최대 체력의 5~10%만큼 감소 )
                {
                    ingamecs.NowHP = ingamecs.NowHP - (int)(ingamecs.NowmaxHP / hurt_percent);
                    if (ingamecs.NowHP <= 0) GameOver();
                    Txt_bpanel.GetComponent<Text>().text = $"상자 안에서 장난감이 튀어나왔습니다! 누군가의 장난이었네요..\n 최대 HP의 {hurt_percent}% 만큼 감소합니다.";
                    Btn_bpanel.GetComponent<Text>().text = "다음 스테이지로";
                    NextStage();
                    break;
                }
                else if (j == 1)    // 몬스터 등장
                {
                    Txt_bpanel.GetComponent<Text>().text = $"몬스터가 나타났습니다. 어서 전투를 준비하십시오!";
                    Btn_bpanel.GetComponent<Text>().text = "전투 시작";
                    BoxMonster();
                    break;
                }
                else if (j == 2)    // 체력 전부 회복
                {
                    ingamecs.NowHP = ingamecs.NowmaxHP;
                    Txt_bpanel.GetComponent<Text>().text = $"신비한 물약을 마셔 체력을 전부 회복했습니다.";
                    Btn_bpanel.GetComponent<Text>().text = "다음 스테이지로";
                    NextStage();
                    break;
                }
                else if (j == 3)    // 체력 일정 회복 ( 최대 체력의 10~20%만큼 증가 )
                {
                    ingamecs.NowHP = ingamecs.NowHP + (int)(ingamecs.NowmaxHP / heal_percent);
                    if (ingamecs.NowHP > ingamecs.NowmaxHP) ingamecs.NowHP = ingamecs.NowmaxHP;
                    Txt_bpanel.GetComponent<Text>().text = $"음료수를 마셔 최대 HP의 {heal_percent}% 만큼 증가합니다.";
                    Btn_bpanel.GetComponent<Text>().text = "다음 스테이지로";
                    NextStage();
                    break;
                }
                else                // 장비 획득
                {
                    Txt_bpanel.GetComponent<Text>().text = $"장비를 획득했습니다. 나에게 맞는 장비인지 확인해보도록 합시다.";
                    Btn_bpanel.GetComponent<Text>().text = "장비 획득 창으로";
                }
            }
        }
    }

    // 몬스터 창 이동
    public void BoxMonster()
    {
        
        Monster_Panel.SetActive(true);

        MPanel_Title.GetComponent<Text>().text = "몬스터 전투";
        Monster_name.GetComponent<Text>().text = "몬스터 OOO이 등장했습니다!";
        Debug.Log("Monster");

        NextStage();
    }


    // 장비 획득 창 필요

    // 2.3 버프
    void Buff()
    {
        Buff_Panel.SetActive(true);
        // 버프를 랜덤으로 json을 통해 가져오고, 스테이지를 넘김
        Debug.Log("AddBuff");
        NextStage();
    }

    public void AddBuff()
    {
        // 버프 12개만 사용
        int b = Random.Range(0, 11);
        ingamecs.NowBuff = injson.jbuffData[b]["id"].ToString();

        ingamecs.StateUpdate();
    }

        // 2.4 이벤트
    void RandomEvent()
    {
        Event_Panel.SetActive(true);
        Debug.Log("RandomEvent");
        NextStage();
    }

        // 2.5 마을
    void CityIn()
    {
        ShopEquipSetting((int)ingamecs.NowWorld);
        Town_Panel.SetActive(true);
        Debug.Log("CityIn");
        NextStage();
    }

    // 상점에 장비 및 무기 세팅
    void ShopEquipSetting(int world)
    {
        // 무기와 장비 구분, 등급, 수식어, 종류 총 4개의 세팅을 3번 해야합니다

        // 월드별 등급 확률 세팅
        int[] eq_world_per;

        switch (world)
        {
            case 1:
                eq_world_per = eq_world1_per;
                break;
            case 2:
                eq_world_per = eq_world2_per;
                break;
            case 3:
                eq_world_per = eq_world3_per;
                break;
            case 4:
                eq_world_per = eq_world4_per;
                break;
            case 5:
                eq_world_per = eq_world5_per;
                break;
            case 6:
                eq_world_per = eq_world6_per;
                break;
            case 7:
                eq_world_per = eq_world7_per;
                break;
            case 8:
                eq_world_per = eq_world8_per;
                break;
            default:
                eq_world_per = eq_worldx_per;
                break;
        }

        int per;        // 퍼센트 용
        string eorw;       // 무기 세팅 용

        // 등급 부여
        for (int i = 0; i < 3; i++)
        {
            // 무기, 장비 구분      
            per = Random.Range(1, 100);
            if (per <= 30) eorw = "0";
            else eorw = "1";

            // 종류 구분            숫자 3개 중 1번째
            per = Random.Range(0, 4);
            eorw += $"{per}";

            // 등급 구분            숫자 3개 중 2번째
            per = Random.Range(1, 100);
            for (int j = 0; j < 5; j++)
            {
                if (per <= eq_world_per[j])
                {
                    eorw += $"{j}";
                    break;
                }
            }

            // 수식어 구분          숫자 3개 중 3번째
            per = Random.Range(1, 100);
            for (int j = 0; j < 5; j++)
            {
                if (per <= eq_modifier_per[j])
                {
                    eorw += $"{j}";
                    break;
                }
            }

            // 배열에 저장
            eorwArray[i] = eorw;
        }

    }


    // 상점 구매 버튼
    public void BuyStore()
    {

        string BuyButtonName = EventSystem.current.currentSelectedGameObject.name;

        int select_EorW = int.Parse(BuyButtonName.Substring(7));

        if (eorwArray[select_EorW].Substring(0, 1) == "0") Debug.Log("장비 구매");
        else Debug.Log("무기 구매");
    }

    // 힐러 회복 버튼
    public void TownHeal()
    {
        if (true) // 필요한 골드가 있는지 체크
        {
            // 골드 차감
            ingamecs.NowHP = ingamecs.NowmaxHP; // 체력 전부 회복
        }
    }

    // 무기 강화 버튼
    

    // 3. 스테이지 넘김

    void NextStage()
    {
        if (bossClear)
        {
            ingamecs.NowWorld++;
            ingamecs.NowStage = 1;
            bossClear = false;
        }
        else ingamecs.NowStage++;

        StageCheck();
        ingamecs.StateUpdate();
    }


    // 4. 플레이어 사망

    void GameOver()
    {
        // 게임 오버 및 결과창 띄워주고
        // 해당 게임 기록창으로 옮겨주어야 함
    }
}
