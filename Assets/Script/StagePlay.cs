using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StagePlay : MonoBehaviour
{
    // 필요 오브젝트
    public Sprite[] Card_Img = new Sprite[6];               // 스테이지 카드 확률 참고

    // 사용 확률 모음
    // 1. 스테이지 카드 확률 ( 일반, 보스 )
    int[] card_ran_per = new int[] { 70, 75, 80, 90, 100 };    // 0 = 몬스터, 1 = 보물상자, 2 = 버프, 3 = 랜덤 이벤트, 4 = 마을 NPC, 5 = 보스
    int[] boss_card_ran_per = new int[] { 76, 82, 88, 100 };   // 보스는 마을이 뜨지 않아 4를 제외하고, 5를 고정으로 가운데 카드에 넣습니다.
    // 2. 보물상자 확률 ( 함정, 몬스터, 체력 전부 회복, 체력 일정 회복, 장비 획득 )
    int[] box_ran_per = new int[] { 15, 25, 30, 45, 100 };
    // 3. 장비 드랍 확률 ( 월드 별 등급, 수식어, 종류, 무기/장비 부여 )
    int[] dr_world1_per = new int[] { 100 };
    int[] dr_world2_per = new int[] { 75, 100 };
    int[] dr_world3_per = new int[] { 58, 98, 100 };
    int[] dr_world4_per = new int[] { 35, 85, 100 };
    int[] dr_world5_per = new int[] { 15, 58, 98, 100 };
    int[] dr_world6_per = new int[] { 10, 40, 94, 99, 100 };
    int[] dr_world7_per = new int[] { 3, 28, 88, 98, 100 };
    int[] dr_world8_per = new int[] { 1, 21, 85, 97, 100 };
    int[] dr_worldx_per = new int[] { 1, 16, 80, 95, 100 };
    int[] dr_modifier_per = new int[] { 5, 25, 78, 98, 100 };
    int[] eq_or_weapon_per = new int[] { 70, 80, 90, 100 };
    // 4. 강화 비용
    int[] weapon_money = new int[] { 0, 25, 100, 250, 500, 750, 1000 };
    int[] heal_money = new int[] { };
    // 5. 몬스터 월드 구분 및 주는 골드
    int[] monster_worldnum = new int[] { -2, 6, 14, 22, 30, 38, 46, 54, 62, 69, 86, 93 };
    int[] monster_Gold = new int[] { 0, 15, 25, 25, 35, 35, 45, 50, 60, 75, 90, 90, 100, 100, 110, 120, 130, 120, 150};
    int[] boss_Gold = new int[] { 0, 200, 450, 700, 1000, 1500, 0, 2000, 2500, 0, 0 };



    string[] checkrank = new string[6] { "NONE", "커먼", "언커먼", "레어", "유니크", "레전더리", };

    // Player State    ex) Now ..
    InGamecs ingamecs;
    InJson injson;

    // 0. 스테이지 카드 세팅 단계 변수
    GameObject Stage_card0, Stage_card1, Stage_card2;           // Stage Card Image
    int[] stage_card_num = new int[3];

    // 2. 카드 별 패널 띄움 변수
    public GameObject Monster_Panel, Box_Panel, Buff_Panel, Event_Panel, Town_Panel, MVic_Panel;   // 각 패널
    public GameObject Monster_name, MPanel_Title;
    public GameObject Txt_bpanel, Btn_bpanel, Img_bpanel;

    // 2. 상인 장비 리스트용 변수
    string[] eorwArray = new string[3];
    public GameObject[] ShopImg = new GameObject[3];
    public GameObject HcostTxt;
    double healPay;

    // 2. 보물상자 변수
    int BEnum = 0;

    // 2. 몬스터 배틀 카드 핸드
    Stack Deck = new Stack();
    public GameObject BCardPanel, MAtkPanel;
    public GameObject[] BattleCardHand = new GameObject[4];
    public GameObject[] BattleCardHandTxt = new GameObject[4];
    int[] CardHand = new int[4];
    int[] CardExistCheck = new int[5] { 0, 0, 0, 0, 0 };
    int hand_count = 0;

    // 2. 카드 사용 여부 체크
    int[] CardUseCheck = { 0, 0, 0, 0 };
    int CardUseCount = 0;

    // 2. 몬스터 배틀
    float pturn = 0;
    float mturn = 0;
    bool morp = false;                              // false 몬스터, true 플레이어
    public GameObject MBtnTxt, MAtkRecord, MhpTxt, MVGoldTxt;
    public GameObject[] CardWImg = new GameObject[4];   // 배틀 시 카드 무기 체크


    // 2. 장비 및 무기 보상용 패널 변수
    public GameObject WeaponGet_Panel, EquipmentGet_Panel;
    public GameObject[] W = new GameObject[10];
    public GameObject[] txtW = new GameObject[10];
    public GameObject BeforeE, AfterE, BeforeW, AfterW;
    public GameObject BeforeTxtE, AfterTxtE, BeforeTxtW, AfterTxtW;
    public Sprite helmet, armor, shoes, uimask;
    public Sprite hammer, dagger, fist, spear, sword, wand;
    int takeweaponnum = 0;      // 교체 무기 인덱스
    int getweaponATK = 0;       // 얻은 무기 공격력
    string getEorW;


    // 2. 강화 오브젝트
    public GameObject[] nowRW = new GameObject[10];
    public GameObject[] nowtxtRW = new GameObject[10];
    public GameObject[] nowRE = new GameObject[3];
    public GameObject[] nowtxtRE = new GameObject[3];

    public GameObject R1, R2;
    public GameObject Bf_R, Af_R, Txt_TakeGold;
    string ReenChoiceEorW;

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
        Stage_card0.GetComponent<Image>().sprite = Card_Img[stage_card_num[0]];
        Stage_card1.GetComponent<Image>().sprite = Card_Img[stage_card_num[1]];
        Stage_card2.GetComponent<Image>().sprite = Card_Img[stage_card_num[2]];
    }

    // 보스 등장 이전 세팅
    void CardSetting()
    {
        int percent;

        for (int i = 0; i < 3; i++)
        {
            percent = Random.Range(1, 101);
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

            percent = Random.Range(1, 101);
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

    // 랜덤 장비 획득 함수
    void EorWRandGet(int world)
    {
        // 월드별 등급 확률 세팅
        int[] eq_world_per;

        switch (world)
        {
            case 1:
                eq_world_per = dr_world1_per;
                break;
            case 2:
                eq_world_per = dr_world2_per;
                break;
            case 3:
                eq_world_per = dr_world3_per;
                break;
            case 4:
                eq_world_per = dr_world4_per;
                break;
            case 5:
                eq_world_per = dr_world5_per;
                break;
            case 6:
                eq_world_per = dr_world6_per;
                break;
            case 7:
                eq_world_per = dr_world7_per;
                break;
            case 8:
                eq_world_per = dr_world8_per;
                break;
            default:
                eq_world_per = dr_worldx_per;
                break;
        }

        int per;        // 퍼센트 용
        string eorw = "";       // 무기 세팅 용

        // 등급 부여
        // 무기, 장비 구분      0이면 무기, 1은 helmet, 2는 armor, 3은 shoes
        per = Random.Range(1, 101);
        for (int j = 0; j < 4; j++)
        {
            if (per <= eq_or_weapon_per[j])
            {
                eorw = $"{j}";
                break;
            }
        }

        // 종류 구분            숫자 3개 중 1번째
        per = Random.Range(0, 5);
        eorw += $"{per}";

        // 등급 구분            숫자 3개 중 2번째
        per = Random.Range(1, 101);
        for (int j = 0; j < 5; j++)
        {
            if (per <= eq_world_per[j])
            {
                eorw += $"{j}";
                break;
            }
        }

        // 수식어 구분          숫자 3개 중 3번째
        per = Random.Range(1, 101);
        for (int j = 0; j < 5; j++)
        {
            if (per <= dr_modifier_per[j])
            {
                eorw += $"{j}";
                break;
            }
        }

        getEorW = eorw;
    }

    // 장비 획득 창
    void EquipSetting()
    {
        // 기존 장비 인덱스
        int nowindex = 3;
        if (ingamecs.NowEquip[int.Parse(getEorW.Substring(0, 1)) - 1].Substring(0, 1) == "0") nowindex = int.Parse(getEorW.Substring(0, 1)) - 1;
        else
        {
            nowindex += 125 * (int.Parse(getEorW.Substring(0, 1)) - 1) +
                25 * int.Parse(ingamecs.NowEquip[int.Parse(getEorW.Substring(0, 1)) - 1].Substring(1, 1)) +
                5 * int.Parse(ingamecs.NowEquip[int.Parse(getEorW.Substring(0, 1)) - 1].Substring(2, 1)) +
                int.Parse(ingamecs.NowEquip[int.Parse(getEorW.Substring(0, 1)) - 1].Substring(3, 1));
        }


        // 획득한 장비 인덱스
        int checkindex = 3;
        checkindex += 125 * (int.Parse(getEorW.Substring(0, 1)) - 1) +
            25 * int.Parse(getEorW.Substring(1, 1)) +
            5 * int.Parse(getEorW.Substring(2, 1)) +
            int.Parse(getEorW.Substring(3, 1));

        // 장비 종류에 따른 현재 장비 이미지 셋팅
        if (getEorW.Substring(0, 1) == "1")
        {
            BeforeE.GetComponent<Image>().sprite = helmet;
            AfterE.GetComponent<Image>().sprite = helmet;
        }
        else if (getEorW.Substring(0, 1) == "2")
        {
            BeforeE.GetComponent<Image>().sprite = armor;
            AfterE.GetComponent<Image>().sprite = armor;
        }
        else
        {
            BeforeE.GetComponent<Image>().sprite = shoes;
            AfterE.GetComponent<Image>().sprite = shoes;
        }

        // Before After 장비 정보 탐색 및 셋팅      ( 기본 3, 헬멧 125, 아머 125, 신발 125 )
        BeforeTxtE.GetComponent<Text>().text = $"{injson.jequipData[nowindex]["name"].ToString()}\n{checkrank[int.Parse(injson.jequipData[nowindex]["rank"].ToString()) + 1]}";
        AfterTxtE.GetComponent<Text>().text = $"{injson.jequipData[checkindex]["name"].ToString()}\n{checkrank[int.Parse(injson.jequipData[checkindex]["rank"].ToString()) + 1]}";

    }

    // 장비 교체 확인
    public void EquipChange()
    {
        int nowindex = 3;
        if (ingamecs.NowEquip[int.Parse(getEorW.Substring(0, 1)) - 1].Substring(0, 1) == "0") nowindex = int.Parse(getEorW.Substring(0, 1)) - 1;
        else
        {
            nowindex += 125 * (int.Parse(getEorW.Substring(0, 1)) - 1) +
                25 * int.Parse(ingamecs.NowEquip[int.Parse(getEorW.Substring(0, 1)) - 1].Substring(1, 1)) +
                5 * int.Parse(ingamecs.NowEquip[int.Parse(getEorW.Substring(0, 1)) - 1].Substring(2, 1)) +
                int.Parse(ingamecs.NowEquip[int.Parse(getEorW.Substring(0, 1)) - 1].Substring(3, 1));
        }


        int checkindex = 3;
        checkindex += 125 * (int.Parse(getEorW.Substring(0, 1)) - 1) +
            25 * int.Parse(getEorW.Substring(1, 1)) +
            5 * int.Parse(getEorW.Substring(2, 1)) +
            int.Parse(getEorW.Substring(3, 1));

        // 추가 장비 스탯 +
        try { ingamecs.NowSpeed += int.Parse(injson.jequipData[checkindex]["statEffect"]["speed"].ToString()); } catch { }
        try { ingamecs.NowATK += int.Parse(injson.jequipData[checkindex]["statEffect"]["attack"].ToString()); } catch { }
        try { ingamecs.NowDEF += int.Parse(injson.jequipData[checkindex]["statEffect"]["defense"].ToString()); } catch { }
        try { ingamecs.NowmaxHP += int.Parse(injson.jequipData[checkindex]["statEffect"]["maxHp"].ToString()); } catch { }

        // 기존 장비 스탯 -
        try { ingamecs.NowSpeed -= int.Parse(injson.jequipData[nowindex]["statEffect"]["speed"].ToString()); } catch { }
        try { ingamecs.NowATK -= int.Parse(injson.jequipData[nowindex]["statEffect"]["attack"].ToString()); } catch { }
        try { ingamecs.NowDEF -= int.Parse(injson.jequipData[nowindex]["statEffect"]["defense"].ToString()); } catch { }
        try { ingamecs.NowmaxHP -= int.Parse(injson.jequipData[nowindex]["statEffect"]["maxHp"].ToString()); } catch { }

        ingamecs.NowEquip[int.Parse(getEorW.Substring(0, 1)) - 1] = $"{getEorW}";
        Debug.Log($"바뀐 Equip 설정 : {ingamecs.NowEquip[int.Parse(getEorW.Substring(0, 1)) - 1]}");

        // 스탯 반영 해줘야 합니다.
        ingamecs.StateUpdate();
    }


    // 무기 획득 창
    void WeaponSetting()
    {

        // 가진 무기에 맞는 이미지 및 공격력 넣어주어야 합니다. (맨주먹까지 6종류, 0 : 검, 1 : 망치, 2 : 창, 3 : 단검, 4 : 요술봉, 5 : 맨주먹)
        // 무기 체크해서 능력치 텍스트로 출력해야합니다.
        for (int i = 0; i < 10; i++)
        {
            int weaponindex = 1;
            if (ingamecs.NowWeapon[i].Substring(0, 1) == "5") weaponindex = 0;
            else
            {
                weaponindex += (int.Parse(ingamecs.NowWeapon[i].Substring(0, 1))) +
                    25 * (int.Parse(ingamecs.NowWeapon[i].Substring(1, 1))) +
                    5 * (int.Parse(ingamecs.NowWeapon[i].Substring(2, 1)));
            }

            if (i == 0)
            {
                for (int j = 0; j < 6; j++)
                {
                    switch (ingamecs.NowWeapon[i].Substring(0, 1))
                    {
                        case "0":
                            BeforeW.GetComponent<Image>().sprite = sword;
                            BeforeTxtW.GetComponent<Text>().text = $"{injson.jweaponData[weaponindex]["name"].ToString()}\n{checkrank[int.Parse(injson.jweaponData[weaponindex]["rank"].ToString()) + 1]}\n+{ingamecs.NowWeaponATK[i]}";
                            break;
                        case "1":
                            BeforeW.GetComponent<Image>().sprite = hammer;
                            BeforeTxtW.GetComponent<Text>().text = $"{injson.jweaponData[weaponindex]["name"].ToString()}\n{checkrank[int.Parse(injson.jweaponData[weaponindex]["rank"].ToString()) + 1]}\n+{ingamecs.NowWeaponATK[i]}";
                            break;
                        case "2":
                            BeforeW.GetComponent<Image>().sprite = spear;
                            BeforeTxtW.GetComponent<Text>().text = $"{injson.jweaponData[weaponindex]["name"].ToString()}\n{checkrank[int.Parse(injson.jweaponData[weaponindex]["rank"].ToString()) + 1]}\n+{ingamecs.NowWeaponATK[i]}";
                            break;
                        case "3":
                            BeforeW.GetComponent<Image>().sprite = dagger;
                            BeforeTxtW.GetComponent<Text>().text = $"{injson.jweaponData[weaponindex]["name"].ToString()}\n{checkrank[int.Parse(injson.jweaponData[weaponindex]["rank"].ToString()) + 1]}\n+{ingamecs.NowWeaponATK[i]}";
                            break;
                        case "4":
                            BeforeW.GetComponent<Image>().sprite = wand;
                            BeforeTxtW.GetComponent<Text>().text = $"{injson.jweaponData[weaponindex]["name"].ToString()}\n{checkrank[int.Parse(injson.jweaponData[weaponindex]["rank"].ToString()) + 1]}\n+{ingamecs.NowWeaponATK[i]}";
                            break;
                        case "5":
                            BeforeW.GetComponent<Image>().sprite = fist;
                            BeforeTxtW.GetComponent<Text>().text = $"{injson.jweaponData[weaponindex]["name"].ToString()}\n{checkrank[int.Parse(injson.jweaponData[weaponindex]["rank"].ToString()) + 1]}\n+{ingamecs.NowWeaponATK[i]}";
                            break;
                        default:
                            break;
                    }
                }
            }   // 초기값 첫번째 무기 자동 선택 
            takeweaponnum = 0;  // 교체 무기 0번으로 초기화

            for (int j = 0; j < 6; j++)
            {
                switch (ingamecs.NowWeapon[i].Substring(0, 1))
                {
                    case "0":
                        W[i].GetComponent<Image>().sprite = sword;
                        txtW[i].GetComponent<Text>().text = $"{injson.jweaponData[weaponindex]["name"].ToString()}\n{checkrank[int.Parse(injson.jweaponData[weaponindex]["rank"].ToString()) + 1]}\n+{ingamecs.NowWeaponATK[i]}";
                        break;
                    case "1":
                        W[i].GetComponent<Image>().sprite = hammer;
                        txtW[i].GetComponent<Text>().text = $"{injson.jweaponData[weaponindex]["name"].ToString()}\n{checkrank[int.Parse(injson.jweaponData[weaponindex]["rank"].ToString()) + 1]}\n+{ingamecs.NowWeaponATK[i]}";
                        break;
                    case "2":
                        W[i].GetComponent<Image>().sprite = spear;
                        txtW[i].GetComponent<Text>().text = $"{injson.jweaponData[weaponindex]["name"].ToString()}\n{checkrank[int.Parse(injson.jweaponData[weaponindex]["rank"].ToString()) + 1]}\n+{ingamecs.NowWeaponATK[i]}";
                        break;
                    case "3":
                        W[i].GetComponent<Image>().sprite = dagger;
                        txtW[i].GetComponent<Text>().text = $"{injson.jweaponData[weaponindex]["name"].ToString()}\n{checkrank[int.Parse(injson.jweaponData[weaponindex]["rank"].ToString()) + 1]}\n+{ingamecs.NowWeaponATK[i]}";
                        break;
                    case "4":
                        W[i].GetComponent<Image>().sprite = wand;
                        txtW[i].GetComponent<Text>().text = $"{injson.jweaponData[weaponindex]["name"].ToString()}\n{checkrank[int.Parse(injson.jweaponData[weaponindex]["rank"].ToString()) + 1]}\n+{ingamecs.NowWeaponATK[i]}";
                        break;
                    case "5":
                        W[i].GetComponent<Image>().sprite = fist;
                        txtW[i].GetComponent<Text>().text = $"{injson.jweaponData[weaponindex]["name"].ToString()}\n{checkrank[int.Parse(injson.jweaponData[weaponindex]["rank"].ToString()) + 1]}\n+{ingamecs.NowWeaponATK[i]}";
                        break;
                    default:
                        break;
                }
            }
        }

        // 얻은 무기 정보 인덱스
        int getweaponindex = 1;
        getweaponindex += (int.Parse(getEorW.Substring(1, 1))) +
            25 * (int.Parse(getEorW.Substring(2, 1))) +
            5 * (int.Parse(getEorW.Substring(3, 1)));

        // 얻은 무기 공격력
        getweaponATK = Random.Range(int.Parse(injson.jweaponData[getweaponindex]["minAttack"].ToString()), int.Parse(injson.jweaponData[getweaponindex]["maxAttack"].ToString()));

        // After 무기 세팅
        for (int j = 0; j < 6; j++)
        {
            switch (getEorW.Substring(1, 1))
            {
                case "0":
                    AfterW.GetComponent<Image>().sprite = sword;
                    AfterTxtW.GetComponent<Text>().text = $"{injson.jweaponData[getweaponindex]["name"].ToString()}\n{checkrank[int.Parse(injson.jweaponData[getweaponindex]["rank"].ToString()) + 1]}\n+{getweaponATK}";
                    break;
                case "1":
                    AfterW.GetComponent<Image>().sprite = hammer;
                    AfterTxtW.GetComponent<Text>().text = $"{injson.jweaponData[getweaponindex]["name"].ToString()}\n{checkrank[int.Parse(injson.jweaponData[getweaponindex]["rank"].ToString()) + 1]}\n+{getweaponATK}";
                    break;
                case "2":
                    AfterW.GetComponent<Image>().sprite = spear;
                    AfterTxtW.GetComponent<Text>().text = $"{injson.jweaponData[getweaponindex]["name"].ToString()}\n{checkrank[int.Parse(injson.jweaponData[getweaponindex]["rank"].ToString()) + 1]}\n+{getweaponATK}";
                    break;
                case "3":
                    AfterW.GetComponent<Image>().sprite = dagger;
                    AfterTxtW.GetComponent<Text>().text = $"{injson.jweaponData[getweaponindex]["name"].ToString()}\n{checkrank[int.Parse(injson.jweaponData[getweaponindex]["rank"].ToString()) + 1]}\n+{getweaponATK}";
                    break;
                case "4":
                    AfterW.GetComponent<Image>().sprite = wand;
                    AfterTxtW.GetComponent<Text>().text = $"{injson.jweaponData[getweaponindex]["name"].ToString()}\n{checkrank[int.Parse(injson.jweaponData[getweaponindex]["rank"].ToString()) + 1]}\n+{getweaponATK}";
                    break;
                case "5":
                    AfterW.GetComponent<Image>().sprite = fist;
                    AfterTxtW.GetComponent<Text>().text = $"{injson.jweaponData[getweaponindex]["name"].ToString()}\n{checkrank[int.Parse(injson.jweaponData[getweaponindex]["rank"].ToString()) + 1]}\n+{getweaponATK}";
                    break;
                default:
                    break;
            }
        }


    }

    // 무기 선택 창
    public void TakeWeapon()
    {
        string Weaponname = EventSystem.current.currentSelectedGameObject.name;

        takeweaponnum = int.Parse(Weaponname.Substring(5));
        Debug.Log(takeweaponnum);


        Sprite Weaponimg = W[takeweaponnum].GetComponent<Image>().sprite;
        string Weaponinfo = txtW[takeweaponnum].GetComponent<Text>().text;

        BeforeW.GetComponent<Image>().sprite = Weaponimg;
        BeforeTxtW.GetComponent<Text>().text = Weaponinfo;
    }

    // 무기 교체
    public void WeaponChange()
    {
        ingamecs.NowWeapon[takeweaponnum] = getEorW.Substring(1, 3);
        ingamecs.NowWeaponATK[takeweaponnum] = $"{getweaponATK}";
    }

    // 몬스터 전투 입력
    public void MonsterBettle()
    {
        // 스피드로 턴 체크해 해당 턴인 주체에게 턴 잡아주기

        Debug.Log($"{ingamecs.NowMSpeed} : m, {ingamecs.NowSpeed} : p");

        while (pturn < 1000 && mturn < 1000)
        {
            pturn += (float)ingamecs.NowSpeed / 100;
            mturn += (float)ingamecs.NowMSpeed / 100;
        }

        if (pturn > 1000 && mturn > 1000)
        {
            if (pturn > mturn) { PlayerTurn(); pturn -= 1000; }
            else { MonsterAttack(); mturn -= 1000; }
        }
        else if (pturn > 1000)
        {
            PlayerTurn();
            pturn -= 1000;
        }
        else
        {
            MonsterAttack();
            mturn -= 1000;
        }

    }

    // 몬스터 턴
    public void MonsterAttack()
    {
        morp = false;

        MAtkPanel.SetActive(true);
        MBtnTxt.GetComponent<Text>().text = "턴 진행";

        int mdamage = ingamecs.NowMATK * (40/(40+ingamecs.NowDEF));
        if (mdamage == 0) mdamage = 1;
        ingamecs.NowHP -= mdamage;
        if (ingamecs.NowHP < 0) ingamecs.NowHP = 0;

        ingamecs.StateUpdate();

        MAtkRecord.GetComponent<Text>().text = $"몬스터 {ingamecs.NowMName}의 \"공격!\"\n\n데미지 {mdamage}의 타격을 입어,\n현재 체력이 {ingamecs.NowHP} 남았습니다.";
    }

    // 플레이어 턴
    public void PlayerTurn()
    {
        morp = true;

        BCardPanel.SetActive(true);
        MBtnTxt.GetComponent<Text>().text = "공격!";
        
        for (int i = 0; i < 4; i++)
        {
            BattleCardHandTxt[i].GetComponent<Text>().text = $"공격력 {ingamecs.NowWeaponATK[CardHand[i]]}";

            Debug.Log(ingamecs.NowWeapon[CardHand[i]].Substring(0, 1));

            switch (ingamecs.NowWeapon[CardHand[i]].Substring(0, 1))
            {
                case "0":
                    CardWImg[i].GetComponent<Image>().sprite = sword;
                    break;
                case "1":
                    CardWImg[i].GetComponent<Image>().sprite = hammer;
                    break;
                case "2":
                    CardWImg[i].GetComponent<Image>().sprite = spear;
                    break;
                case "3":
                    CardWImg[i].GetComponent<Image>().sprite = dagger;
                    break;
                case "4":
                    CardWImg[i].GetComponent<Image>().sprite = wand;
                    break;
                case "5":
                    CardWImg[i].GetComponent<Image>().sprite = fist;
                    break;
                default:
                    break;
            }
        }
    }

    // 사용할 무기 선택
    public void UseWeaponOnOff()
    {

        string Cardname = EventSystem.current.currentSelectedGameObject.name;

        if (CardUseCheck[int.Parse(Cardname.Substring(8))] == 1)
        {
            CardUseCheck[int.Parse(Cardname.Substring(8))] = 0;
            CardUseCount--;
        }
        else
        {
            if (CardUseCount == 3)
            {
                Debug.Log("안됩니다");
                return;
            }
            CardUseCheck[int.Parse(Cardname.Substring(8))] = 1;
            CardUseCount++;
        }

        Debug.Log($"<< {CardUseCheck[0]} {CardUseCheck[1]} {CardUseCheck[2]} {CardUseCheck[3]} >>");
    }

    // 배틀 넘기는 버튼
    public void MonsterBattleBtn()
    {
        if (morp)
        {
            if (CardUseCount == 0) return;
            else
            {
                int damage = 0;
                for (int i = 0; i < 4; i++)
                {
                    damage += int.Parse(ingamecs.NowWeaponATK[CardHand[i]]) * CardUseCheck[i];
                    CardExistCheck[i] = 1 - CardUseCheck[i];
                    CardUseCheck[i] = 0;
                }

                hand_count = 0;
                ingamecs.NowMHP -= (int)((ingamecs.NowATK + damage) * (40 / (40 + (float)ingamecs.NowMDEF)));
                Debug.Log($"{(int)((ingamecs.NowATK + damage) * (40 / (40 + (float)ingamecs.NowMDEF)))}");
                TakeHand();

                CardUseCount = 0;
            }
        }

        MhpTxt.GetComponent<Text>().text = $"남은 체력 : {ingamecs.NowMHP}";

        MAtkPanel.SetActive(false);
        BCardPanel.SetActive(false);

        if (ingamecs.NowHP <= 0) GameOver();
        else if (ingamecs.NowMHP <= 0) Victory();
        else MonsterBettle();

    }


    // 2.1 몬스터&보스
    void VSMonster(int select_card_num)
    {
        Monster_Panel.SetActive(true);
        TakeHand();
        Debug.Log($"{CardHand[0]} {CardHand[1]} {CardHand[2]} {CardHand[3]} <Card Hand>");
        // Debug.Log($"{Deck.ToArray()[0]} {Deck.ToArray()[1]} {Deck.ToArray()[2]} {Deck.ToArray()[3]} <hDeck> {Deck.Count}");

        if (select_card_num == 5)
        {
            TakeBoss(ingamecs.NowWorld);
            MPanel_Title.GetComponent<Text>().text = "보스 전투";
            Monster_name.GetComponent<Text>().text = $"보스 \"{ingamecs.NowMName}\" 출현!";
            MhpTxt.GetComponent<Text>().text = $"남은 체력 : {ingamecs.NowMHP}";
            // bossClear = true;
            Debug.Log("Boss");
        }
        else
        {
            TakeMonster(ingamecs.NowWorld);
            MPanel_Title.GetComponent<Text>().text = "몬스터 전투";
            Monster_name.GetComponent<Text>().text = $"몬스터 \"{ingamecs.NowMName}\" 출현!";
            MhpTxt.GetComponent<Text>().text = $"남은 체력 : {ingamecs.NowMHP}";
            Debug.Log("Monster");
        }

        NextStage();
    }

    // 몬스터 불러오기
    void TakeMonster(float world)
    {
        int world_check;

        if (world < 5) world_check = (int)world;
        else if (world < 6) world_check = (int)world + (int)((world % 1) * 10);
        else if (world < 7) world_check = (int)world + 1;
        else if (world < 8) world_check = (int)world + (int)((world % 1) * 10) + 1;
        else world_check = (int)world + 2;

        int monsterNum = Random.Range(monster_worldnum[world_check-1]+2, monster_worldnum[world_check]+1);

        ingamecs.NowMName = injson.jmonsterData[monsterNum]["name"].ToString();
        ingamecs.NowMSpeed = int.Parse(injson.jmonsterData[monsterNum]["baseStat"]["speed"].ToString());
        ingamecs.NowMDEF = int.Parse(injson.jmonsterData[monsterNum]["baseStat"]["defense"].ToString());
        ingamecs.NowMATK = int.Parse(injson.jmonsterData[monsterNum]["baseStat"]["attack"].ToString());
        ingamecs.NowMmaxHP = int.Parse(injson.jmonsterData[monsterNum]["baseStat"]["maxHp"].ToString());
        ingamecs.NowMHP = int.Parse(injson.jmonsterData[monsterNum]["baseStat"]["maxHp"].ToString());
        ingamecs.NowMGetGold = Random.Range(monster_Gold[(int)world * 2 - 1], monster_Gold[(int)world * 2] + 1);
    }

    // 보스 불러오기
    void TakeBoss(float world)
    {
        int world_check;

        if (world < 5) world_check = (int)world;
        else if (world < 6) world_check = (int)world + (int)((world % 1) * 10);
        else if (world < 7) world_check = (int)world + 1;
        else if (world < 8) world_check = (int)world + (int)((world % 1) * 10) + 1;
        else world_check = (int)world + 2;

        int bossNum = monster_worldnum[world_check] + 1;

        ingamecs.NowMName = injson.jmonsterData[bossNum]["name"].ToString();
        ingamecs.NowMSpeed = int.Parse(injson.jmonsterData[bossNum]["baseStat"]["speed"].ToString());
        ingamecs.NowMDEF = int.Parse(injson.jmonsterData[bossNum]["baseStat"]["defense"].ToString());
        ingamecs.NowMATK = int.Parse(injson.jmonsterData[bossNum]["baseStat"]["attack"].ToString());
        ingamecs.NowMmaxHP = int.Parse(injson.jmonsterData[bossNum]["baseStat"]["maxHp"].ToString());
        ingamecs.NowMHP = int.Parse(injson.jmonsterData[bossNum]["baseStat"]["maxHp"].ToString());
        ingamecs.NowMGetGold = boss_Gold[(int)world];
    }

    // 카드 가져오기
    void TakeHand()
    {
        while (hand_count != 4)       // 4장 채워질 때까지
        {
            if (CardExistCheck[hand_count] == 1)    // 이미 카드가 채워져있다면
            {
                hand_count += 1;
                continue;
            }

            if (Deck.Count == 0) ShuppleDeck();             // 가져올 카드가 없는 경우 셔플
            CardHand[hand_count] = (int)Deck.Pop();         // 덱 pop으로 가져오기
            CardExistCheck[hand_count] = 1;                   // 핸드 카드 idx 사용 가능 표시
            hand_count += 1;
        }
    }

    public void TakeWeaponImg()
    {
        // 카드 내 이미지 채우기
        for (int i = 0; i < 4; i++)
        {
            Debug.Log(ingamecs.NowWeapon[CardHand[i]].Substring(0, 1));

            switch (ingamecs.NowWeapon[CardHand[i]].Substring(0, 1))
            {
                case "0":
                    CardWImg[i].GetComponent<Image>().sprite = sword;
                    break;
                case "1":
                    CardWImg[i].GetComponent<Image>().sprite = hammer;
                    break;
                case "2":
                    CardWImg[i].GetComponent<Image>().sprite = spear;
                    break;
                case "3":
                    CardWImg[i].GetComponent<Image>().sprite = dagger;
                    break;
                case "4":
                    CardWImg[i].GetComponent<Image>().sprite = wand;
                    break;
                case "5":
                    CardWImg[i].GetComponent<Image>().sprite = fist;
                    break;
                default:
                    break;
            }
        }
    }

    // 덱 셔플
    void ShuppleDeck()
    {
        int[] ranNum = new int[10];

        // 순서 세팅 (숫자 작은 순부터 나오게 하려고 합니다)
        for (int i = 0; i < 10; i++)
        {
            ranNum[i] = Random.Range(0, 101);
        }

        // 세팅 후 덱에 순서 입력
        for (int i = 0; i < 10; i++)
        {
            int idxCard = ranNum.ToList().IndexOf(ranNum.Max());
            ranNum[idxCard] = -1;
            Deck.Push(idxCard);
        }

        Debug.Log($"{Deck.ToArray()[0]} {Deck.ToArray()[1]} {Deck.ToArray()[2]} {Deck.ToArray()[3]} <Deck>");
    }

    // 몬스터 퇴치
    public void Victory()
    {
        Monster_Panel.SetActive(false);
        MVic_Panel.SetActive(true);

        Deck = new Stack();
        CardHand = new int[4];
        CardExistCheck = new int[5] { 0, 0, 0, 0, 0 };
        hand_count = 0;

        ingamecs.NowGold += ingamecs.NowMGetGold;
        ingamecs.StateUpdate();
        MVGoldTxt.GetComponent<Text>().text = $"Gold {ingamecs.NowMGetGold}\n무기/장비 1종";

        morp = false;
    }

    // 승리 버튼
    public void VictoryBtn()
    {
        MVic_Panel.SetActive(false);

        EorWRandGet((int)ingamecs.NowWorld);

        if (getEorW.Substring(0, 1) == "0")
        {
            WeaponGet_Panel.SetActive(true);
            WeaponSetting();
        }
        else
        {
            EquipmentGet_Panel.SetActive(true);
            EquipSetting();
        }
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
        int box_percent = Random.Range(1, 101);
        // 보물상자 확률 ( 함정, 몬스터, 체력 전부 회복, 체력 일정 회복, 장비 획득 )

        int hurt_percent = Random.Range(5, 11);
        int heal_percent = Random.Range(10, 21);

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
                    BEnum = 0;
                    NextStage();
                    break;
                }
                else if (j == 1)    // 몬스터 등장
                {
                    Txt_bpanel.GetComponent<Text>().text = $"몬스터가 나타났습니다. 어서 전투를 준비하십시오!";
                    Btn_bpanel.GetComponent<Text>().text = "전투 시작";
                    BEnum = 1;
                    NextStage();
                    break;
                }
                else if (j == 2)    // 체력 전부 회복
                {
                    ingamecs.NowHP = ingamecs.NowmaxHP;
                    Txt_bpanel.GetComponent<Text>().text = $"신비한 물약을 마셔 체력을 전부 회복했습니다.";
                    Btn_bpanel.GetComponent<Text>().text = "다음 스테이지로";
                    BEnum = 0;
                    NextStage();
                    break;
                }
                else if (j == 3)    // 체력 일정 회복 ( 최대 체력의 10~20%만큼 증가 )
                {
                    ingamecs.NowHP = ingamecs.NowHP + (int)(ingamecs.NowmaxHP / heal_percent);
                    if (ingamecs.NowHP > ingamecs.NowmaxHP) ingamecs.NowHP = ingamecs.NowmaxHP;
                    Txt_bpanel.GetComponent<Text>().text = $"음료수를 마셔 최대 HP의 {heal_percent}% 만큼 증가합니다.";
                    Btn_bpanel.GetComponent<Text>().text = "다음 스테이지로";
                    BEnum = 0;
                    NextStage();
                    break;
                }
                else                // 장비 획득
                {
                    Txt_bpanel.GetComponent<Text>().text = $"무기/장비 아이템을 획득했습니다.\n나에게 맞는 무기/장비인지 확인해보도록 합시다.";
                    Btn_bpanel.GetComponent<Text>().text = "무기/장비 획득 창으로";
                    BEnum = 2;
                    NextStage();
                }
            }
        }
    }

    // 박스 이벤트
    public void BoxEvent()
    {
        if (BEnum == 1) BoxMonster();
        else if (BEnum == 2)
        {
            EorWRandGet((int)ingamecs.NowWorld);
            BoxEorW();
        }
        else return;
    }

    // 몬스터 창 이동
    public void BoxMonster()
    {
        Monster_Panel.SetActive(true);

        TakeMonster(ingamecs.NowWorld);
        MPanel_Title.GetComponent<Text>().text = "몬스터 전투";
        Monster_name.GetComponent<Text>().text = $"몬스터 \"{ingamecs.NowMName}\" 출현!";
        MhpTxt.GetComponent<Text>().text = $"남은 체력 : {ingamecs.NowMHP}";

        Debug.Log("Monster");
    }

    // 무기/장비 획득 창 이동
    public void BoxEorW()
    {
        if (getEorW.Substring(0, 1) == "0")
        {
            WeaponGet_Panel.SetActive(true);
            WeaponSetting();
        }
        else
        {
            EquipmentGet_Panel.SetActive(true);
            EquipSetting();
        }
    }


    // 2.3 버프
    void Buff()
    {
        Buff_Panel.SetActive(true);
        // 버프를 랜덤으로 json을 통해 가져오고, 스테이지를 넘김
        Debug.Log("AddBuff");
        NextStage();
    }

    // 버프 추가
    public void AddBuff()
    {
        // 버프 12개만 사용
        int b = Random.Range(0, 12);
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
        TownEnchant1A();

        healPay = (ingamecs.NowmaxHP - ingamecs.NowHP) * ingamecs.NowWorld * 100 / ingamecs.NowmaxHP;
        HcostTxt.GetComponent<Text>().text = $"{healPay} Gold";

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
                eq_world_per = dr_world1_per;
                break;
            case 2:
                eq_world_per = dr_world2_per;
                break;
            case 3:
                eq_world_per = dr_world3_per;
                break;
            case 4:
                eq_world_per = dr_world4_per;
                break;
            case 5:
                eq_world_per = dr_world5_per;
                break;
            case 6:
                eq_world_per = dr_world6_per;
                break;
            case 7:
                eq_world_per = dr_world7_per;
                break;
            case 8:
                eq_world_per = dr_world8_per;
                break;
            default:
                eq_world_per = dr_worldx_per;
                break;
        }

        int per;        // 퍼센트 용
        string eorw = "";       // 무기 세팅 용

        // 등급 부여
        for (int i = 0; i < 3; i++)
        {
            // 무기, 장비 구분      0이면 무기, 1은 helmet, 2는 armor, 3은 shoes
            per = Random.Range(1, 101);
            for (int j = 0; j < 4; j++)
            {
                if (per <= eq_or_weapon_per[j])
                {
                    eorw = $"{j}";
                    break;
                }
            }

            // 종류 구분            숫자 3개 중 1번째
            per = Random.Range(0, 5);
            eorw += $"{per}";

            // 등급 구분            숫자 3개 중 2번째
            per = Random.Range(1, 101);
            for (int j = 0; j < 5; j++)
            {
                if (per <= eq_world_per[j])
                {
                    eorw += $"{j}";
                    break;
                }
            }

            // 수식어 구분          숫자 3개 중 3번째
            per = Random.Range(1, 101);
            for (int j = 0; j < 5; j++)
            {
                if (per <= dr_modifier_per[j])
                {
                    eorw += $"{j}";
                    break;
                }
            }

            // 배열에 저장
            eorwArray[i] = eorw;
        }

        // 무기나 장비 이미지 띄우기
        for (int i = 0; i < 3; i++)
        {
            if (eorwArray[i].Substring(0, 1) == "0")
            {
                switch (eorwArray[i].Substring(1, 1))
                {
                    case "0":
                        ShopImg[i].GetComponent<Image>().sprite = sword;
                        break;
                    case "1":
                        ShopImg[i].GetComponent<Image>().sprite = hammer;
                        break;
                    case "2":
                        ShopImg[i].GetComponent<Image>().sprite = spear;
                        break;
                    case "3":
                        ShopImg[i].GetComponent<Image>().sprite = dagger;
                        break;
                    case "4":
                        ShopImg[i].GetComponent<Image>().sprite = wand;
                        break;
                    case "5":
                        ShopImg[i].GetComponent<Image>().sprite = fist;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (eorwArray[i].Substring(0, 1))
                {
                    case "1":
                        ShopImg[i].GetComponent<Image>().sprite = helmet;
                        break;
                    case "2":
                        ShopImg[i].GetComponent<Image>().sprite = armor;
                        break;
                    case "3":
                        ShopImg[i].GetComponent<Image>().sprite = shoes;
                        break;
                    default:
                        break;
                }
            }
            
        }

        Debug.Log(eorwArray[0]);

    }


    // 상점 구매 버튼
    public void BuyStore()
    {

        string BuyButtonName = EventSystem.current.currentSelectedGameObject.name;

        int select_EorW = int.Parse(BuyButtonName.Substring(7));
        getEorW = eorwArray[select_EorW];

        if (getEorW.Substring(0, 1) == "0")
        {
            WeaponGet_Panel.SetActive(true);
            WeaponSetting();
            Debug.Log("무기 구매");
        }
        else
        {
            EquipmentGet_Panel.SetActive(true);
            EquipSetting();
            Debug.Log("장비 구매");
        }
    }

    // 힐러 회복 버튼
    public void TownHeal()
    {
        Debug.Log("Heal??");

        if (ingamecs.NowGold > (int)healPay) // 필요한 골드가 있는지 체크
        {
            ingamecs.NowGold -= (int)healPay;// 골드 차감
            ingamecs.NowHP = ingamecs.NowmaxHP; // 체력 전부 회복
        }
        else
        {
            Debug.Log("Don't Heal Enough money.");
        }

        ingamecs.StateUpdate();
    }

    // 무기 강화 1_A(정보 업데이트)
    void TownEnchant1A()
    {
        ingamecs.StateUpdate();
        ingamecs.NowWeaponUpdate();

        for (int i = 0; i < 10; i++)
        {
            int weaponindex = 1;
            if (ingamecs.NowWeapon[i].Substring(0, 1) == "5") weaponindex = 0;
            else
            {
                weaponindex += (int.Parse(ingamecs.NowWeapon[i].Substring(0, 1))) +
                    25 * (int.Parse(ingamecs.NowWeapon[i].Substring(1, 1))) +
                    5 * (int.Parse(ingamecs.NowWeapon[i].Substring(2, 1)));
            }

            for (int j = 0; j < 6; j++)
            {
                switch (ingamecs.NowWeapon[i].Substring(0, 1))
                {
                    case "0":
                        nowRW[i].GetComponent<Image>().sprite = sword;
                        nowtxtRW[i].GetComponent<Text>().text = $"{ingamecs.NowWeaponATK[i]}";
                        break;
                    case "1":
                        nowRW[i].GetComponent<Image>().sprite = hammer;
                        nowtxtRW[i].GetComponent<Text>().text = $"{ingamecs.NowWeaponATK[i]}";
                        break;
                    case "2":
                        nowRW[i].GetComponent<Image>().sprite = spear;
                        nowtxtRW[i].GetComponent<Text>().text = $"{ingamecs.NowWeaponATK[i]}";
                        break;
                    case "3":
                        nowRW[i].GetComponent<Image>().sprite = dagger;
                        nowtxtRW[i].GetComponent<Text>().text = $"{ingamecs.NowWeaponATK[i]}";
                        break;
                    case "4":
                        nowRW[i].GetComponent<Image>().sprite = wand;
                        nowtxtRW[i].GetComponent<Text>().text = $"{ingamecs.NowWeaponATK[i]}";
                        break;
                    case "5":
                        nowRW[i].GetComponent<Image>().sprite = fist;
                        nowtxtRW[i].GetComponent<Text>().text = $"{ingamecs.NowWeaponATK[i]}";
                        break;
                    default:
                        break;
                }
            }

            if (i < 3)
            {
                int Equipindex = 3;
                if (ingamecs.NowEquip[i].Substring(0, 1) == "0") Equipindex = i;
                else
                {
                    Equipindex += 125 * (int.Parse(ingamecs.NowEquip[i].Substring(0, 1)) - 1) +
                        25 * int.Parse(ingamecs.NowEquip[i].Substring(1, 1)) +
                        5 * int.Parse(ingamecs.NowEquip[i].Substring(2, 1)) +
                        int.Parse(ingamecs.NowEquip[i].Substring(3, 1));
                }

                nowRE[i].GetComponent<Image>().sprite = ingamecs.nowE[i].GetComponent<Image>().sprite;
                nowtxtRE[i].GetComponent<Text>().text = $"[{checkrank[int.Parse(injson.jequipData[Equipindex]["rank"].ToString()) + 1]}]";
            }
        }
    }

    // 무기 강화 1_B(무기 or 장비 선택)
    public void TownEnchant1B()
    {
        ReenChoiceEorW = EventSystem.current.currentSelectedGameObject.name;

        Debug.Log(ReenChoiceEorW);

        if (ReenChoiceEorW.Substring(4, 1) == "W")        // 무기 선택 시
        {
            if (ingamecs.NowWeapon[int.Parse(ReenChoiceEorW.Substring(5, 1))].Substring(0, 1) == "5") Debug.Log("강화 불가한 아이템");
            else if (ingamecs.NowWeapon[int.Parse(ReenChoiceEorW.Substring(5, 1))].Substring(2, 1) == "4") Debug.Log("최고 등급의 아이템");
            else
            {
                R1.SetActive(false);
                R2.SetActive(true);
                TownEnchant2A();
            }
        }
        else                                        // 장비 선택 시
        {
            if (ingamecs.NowEquip[int.Parse(ReenChoiceEorW.Substring(5, 1))].Substring(0, 1) == "0") Debug.Log("강화 불가한 아이템");
            else if (ingamecs.NowEquip[int.Parse(ReenChoiceEorW.Substring(5, 1))].Substring(3, 1) == "4") Debug.Log("최고 등급의 아이템");
            else
            {
                R1.SetActive(false);
                R2.SetActive(true);
                TownEnchant2A();
            }
        }
    }

    // 무기 강화 2_A(시각화)
    void TownEnchant2A()
    {
        // UI 띄워주기

        if (ReenChoiceEorW.Substring(4, 1) == "W")          // 무기 선택 시
        {
            Bf_R.GetComponent<Image>().sprite = ingamecs.nowW[int.Parse(ReenChoiceEorW.Substring(5, 1))].GetComponent<Image>().sprite;
            Af_R.GetComponent<Image>().sprite = ingamecs.nowW[int.Parse(ReenChoiceEorW.Substring(5, 1))].GetComponent<Image>().sprite;

            int weaponindex = 1;
            weaponindex += (int.Parse(ingamecs.NowWeapon[int.Parse(ReenChoiceEorW.Substring(5, 1))].Substring(0, 1))) +
                25 * (int.Parse(ingamecs.NowWeapon[int.Parse(ReenChoiceEorW.Substring(5, 1))].Substring(1, 1))) +
                5 * (int.Parse(ingamecs.NowWeapon[int.Parse(ReenChoiceEorW.Substring(5, 1))].Substring(2, 1)));

            Txt_TakeGold.GetComponent<Text>().text = $"필요 골드 : {weapon_money[int.Parse(ingamecs.NowWeapon[int.Parse(ReenChoiceEorW.Substring(5,1))].Substring(2,1))]}";
        }
        else                                                // 장비 선택 시
        {
            Bf_R.GetComponent<Image>().sprite = ingamecs.nowE[int.Parse(ReenChoiceEorW.Substring(5, 1))].GetComponent<Image>().sprite;
            Af_R.GetComponent<Image>().sprite = ingamecs.nowE[int.Parse(ReenChoiceEorW.Substring(5, 1))].GetComponent<Image>().sprite;

            int equipindex = 3;
            equipindex += 125 * (int.Parse(ReenChoiceEorW.Substring(5, 1))) +
                25 * int.Parse(ingamecs.NowEquip[int.Parse(ReenChoiceEorW.Substring(5, 1))].Substring(1, 1)) +
                5 * int.Parse(ingamecs.NowEquip[int.Parse(ReenChoiceEorW.Substring(5, 1))].Substring(2, 1)) +
                int.Parse(ingamecs.NowEquip[int.Parse(ReenChoiceEorW.Substring(5, 1))].Substring(3, 1));

            Txt_TakeGold.GetComponent<Text>().text = $"필요 골드 : {weapon_money[int.Parse(ingamecs.NowWeapon[int.Parse(ReenChoiceEorW.Substring(5, 1))].Substring(3, 1))]}";
        }
    }

    // 무기 강화 2_B(강화 적용)
    public void TownEnchant2B()
    {
        if (ingamecs.NowGold < int.Parse(Txt_TakeGold.GetComponent<Text>().text.Substring(8))) Debug.Log("골드 부족");
        else
        {
            ingamecs.NowGold -= int.Parse(Txt_TakeGold.GetComponent<Text>().text.Substring(8));

            if (ReenChoiceEorW.Substring(4, 1) == "W")          // 무기 선택 시
            {
                ingamecs.NowWeapon[int.Parse(ReenChoiceEorW.Substring(5, 1))] = $"{ingamecs.NowWeapon[int.Parse(ReenChoiceEorW.Substring(5, 1))].Substring(0, 2)}" +
                    $"{int.Parse(ingamecs.NowWeapon[int.Parse(ReenChoiceEorW.Substring(5, 1))].Substring(2, 1)) + 1}";


                int weaponindex = 1;
                weaponindex += (int.Parse(ingamecs.NowWeapon[int.Parse(ReenChoiceEorW.Substring(5, 1))].Substring(0, 1))) +
                    25 * (int.Parse(ingamecs.NowWeapon[int.Parse(ReenChoiceEorW.Substring(5, 1))].Substring(1, 1))) +
                    5 * (int.Parse(ingamecs.NowWeapon[int.Parse(ReenChoiceEorW.Substring(5, 1))].Substring(2, 1)));

                ingamecs.NowWeaponATK[int.Parse(ReenChoiceEorW.Substring(5, 1))] = $"{Random.Range(int.Parse(injson.jweaponData[weaponindex]["minAttack"].ToString()), int.Parse(injson.jweaponData[weaponindex]["maxAttack"].ToString()))}";
            }
            else                                                // 장비 선택 시
            {
                ingamecs.NowEquip[int.Parse(ReenChoiceEorW.Substring(5, 1))] = $"{ingamecs.NowEquip[int.Parse(ReenChoiceEorW.Substring(5, 1))].Substring(0, 3)}" +
                    $"{int.Parse(ingamecs.NowEquip[int.Parse(ReenChoiceEorW.Substring(5, 1))].Substring(3, 1)) + 1}";
            }
        }

        ingamecs.StateUpdate();
    }

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

        // 로비 이동
        SceneManager.LoadScene("01_Lobby");

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
        PlayerPrefs.SetInt("Gold", ingamecs.NowGold);

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

        // 초기 몬스터 상태
        PlayerPrefs.SetString("MName", $"{ingamecs.NowMName}");
        PlayerPrefs.SetInt("MSpeed", ingamecs.NowMSpeed);
        PlayerPrefs.SetInt("MDEF", ingamecs.NowMDEF);
        PlayerPrefs.SetInt("MATK", ingamecs.NowMATK);
        PlayerPrefs.SetInt("MmaxHP", ingamecs.NowMmaxHP);
        PlayerPrefs.SetInt("MHP", ingamecs.NowMHP);
        PlayerPrefs.SetInt("MGetGold", ingamecs.NowMGetGold);

    }
}
