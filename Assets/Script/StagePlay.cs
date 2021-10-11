using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StagePlay : MonoBehaviour
{
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


    string[] checkrank = new string[6] { "NONE", "커먼", "언커먼", "레어", "유니크", "레전더리", };

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

    // 2. 보물상자 변수
    int BEnum = 0;


    // 2. 장비 및 무기 보상용 패널 변수
    public GameObject WeaponGet_Panel, EquipmentGet_Panel;
    public GameObject[] W = new GameObject[10];
    public GameObject[] txtW = new GameObject[10];
    public GameObject BeforeE, AfterE, BeforeW, AfterW;
    public GameObject BeforeTxtE, AfterTxtE, BeforeTxtW, AfterTxtW;
    public Sprite helmet, armor, shoes;
    public Sprite hammer, dagger, fist, spear, sword, wand;
    int takeweaponnum = 0;      // 교체 무기 인덱스
    int getweaponATK = 0;       // 얻은 무기 공격력
    string getEorW;


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
        per = Random.Range(1, 100);
        for (int j = 0; j < 4; j++)
        {
            if (per <= eq_or_weapon_per[j])
            {
                eorw = $"{j}";
                break;
            }
        }

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
        ingamecs.NowEquip[int.Parse(getEorW.Substring(0, 1)) - 1] = $"{getEorW}";
        Debug.Log($"바뀐 Equip 설정 : {ingamecs.NowEquip[int.Parse(getEorW.Substring(0, 1)) - 1]}");
        // 스탯 반영 해줘야 합니다.
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

    }


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

        MPanel_Title.GetComponent<Text>().text = "몬스터 전투";
        Monster_name.GetComponent<Text>().text = "몬스터 OOO이 등장했습니다!";
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
            per = Random.Range(1, 100);
            for (int j = 0; j < 4; j++)
            {
                if (per <= eq_or_weapon_per[j])
                {
                    eorw = $"{j}";
                    break;
                }
            }

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
                if (per <= dr_modifier_per[j])
                {
                    eorw += $"{j}";
                    break;
                }
            }

            // 배열에 저장
            eorwArray[i] = eorw;
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
