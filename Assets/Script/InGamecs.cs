using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InGamecs : MonoBehaviour
{
    InJson injson;
    StagePlay stageplay;

    // 필요 변수 및 오브젝트
    int statechange = 0;
    int equiponoff = 0;
    GameObject p_panel1, p_panel2;
    GameObject showequipbtn, equiplist;
    GameObject txt_world, txt_stage;

    // 장비 및 무기 타입
    string[] equiptype = new string[4] { "none", "helmet", "armor", "shoes" };
    string[] weapontype = new string[6] { "검", "망치", "창", "단검", "지팡이", "X" };
    string[] checkrank = new string[6] { "NONE", "커먼", "언커먼", "레어", "유니크", "레전더리", };

    // 게임 내 필요 오브젝트 (1)
    GameObject txt_speed, txt_def, txt_atk, txt_buff, txt_snerge, txt_hp, txt_gold;
    

    // 게임 내 플레이어 및 현재 상태 정보
    public int NowStage, NowSpeed, NowDEF, NowATK, NowHP, NowmaxHP, NowGold;
    public float NowWorld;
    public string NowBuff;
    public string[] NowEquip = new string[3];
    public string[] NowWeapon = new string[10];
    public string[] NowWeaponATK = new string[10];
    public string[] NowArtifact = new string[3];

    // 무기, 장비 확인용 UI 오브젝트
    public GameObject[] nowW = new GameObject[10];
    GameObject[] nowtxtW = new GameObject[10];
    public GameObject[] nowE = new GameObject[3];
    GameObject[] nowA = new GameObject[3];
    GameObject checkEorA, checktxtEorA, checkExplainEorA;

    // 현재 몬스터 정보
    public int NowMSpeed, NowMDEF, NowMATK, NowMHP, NowMmaxHP, NowMGetGold;
    public string NowMName;


    // Start is called before the first frame update
    void Start()   
    {
        injson = GameObject.Find("EventSystem").GetComponent<InJson>();
        stageplay = GameObject.Find("EventSystem").GetComponent<StagePlay>();

        // 무기, 장비 확인용 UI 오브젝트
        for (int i = 0; i < 10; i++)
        {
            if ( i < 3 )
            {
                nowE[i] = GameObject.Find($"nowE{i}");
                nowA[i] = GameObject.Find($"nowA{i}");
            }

            nowW[i] = GameObject.Find($"nowW{i}");
            nowtxtW[i] = GameObject.Find($"nowtxtW{i}");
        }

        checkEorA = GameObject.Find("Img_ClickImg");
        checktxtEorA = GameObject.Find("Txt_ClickTxt");
        checkExplainEorA = GameObject.Find("Txt_ClickExplain");

        // 필요 오브젝트 연결
        p_panel1 = GameObject.Find("P_Panel1");
        p_panel2 = GameObject.Find("P_Panel2");
        showequipbtn = GameObject.Find("Btn_Equip");
        equiplist = GameObject.Find("List_Equip");
        txt_world = GameObject.Find("Txt_World");
        txt_stage = GameObject.Find("Txt_Stage");
        equiplist.SetActive(false);
        p_panel2.SetActive(false);

        // 오브젝트 연결 (1)
        txt_speed = GameObject.Find("Txt_Speed");
        txt_def = GameObject.Find("Txt_Def");
        txt_atk = GameObject.Find("Txt_Atk");
        txt_buff = GameObject.Find("Txt_Buff");
        txt_snerge = GameObject.Find("Txt_Sn2");
        txt_hp = GameObject.Find("HP_state");
        txt_gold = GameObject.Find("Txt_Gold");

        // 게임 시작 시 게임 설정 불러오기

        // 초기 스테이지 정보
        NowWorld = PlayerPrefs.GetFloat("World");
        NowStage = PlayerPrefs.GetInt("Stage");

        // 초기 스탯
        NowSpeed = PlayerPrefs.GetInt("Speed");
        NowDEF = PlayerPrefs.GetInt("DEF");
        NowATK = PlayerPrefs.GetInt("ATK");
        NowmaxHP = PlayerPrefs.GetInt("maxHP");
        NowHP = PlayerPrefs.GetInt("HP");
        NowGold = PlayerPrefs.GetInt("Gold");

        // 초기 버프 및 장비 상태
        NowBuff = PlayerPrefs.GetString("Buff");
        NowEquip = PlayerPrefs.GetString("Equipment").Split(',');
        NowWeapon = PlayerPrefs.GetString("Weapon").Split(',');
        NowWeaponATK = PlayerPrefs.GetString("WeaponATK").Split(',');
        NowArtifact = PlayerPrefs.GetString("Artifact").Split(',');

        // 초기 몬스터 상태
        NowMName = PlayerPrefs.GetString("MName");
        NowMSpeed = PlayerPrefs.GetInt("MSpeed");
        NowMDEF = PlayerPrefs.GetInt("MDEF");
        NowMATK = PlayerPrefs.GetInt("MATK");
        NowMmaxHP = PlayerPrefs.GetInt("MmaxHP");
        NowMHP = PlayerPrefs.GetInt("MHP");
        NowMGetGold = PlayerPrefs.GetInt("MGetGold");



        // 시각화
        StateUpdate();
    }

    // 실시간 상태 시각화
    public void StateUpdate()
    {
        // 스테이지 시각화
        txt_stage.GetComponent<Text>().text = $"Stage {NowStage}";
        txt_world.GetComponent<Text>().text = $"W{NowWorld}";

        // 스탯 시각화
        txt_speed.GetComponent<Text>().text = $"{NowSpeed}";
        txt_def.GetComponent<Text>().text = $"{NowDEF}";
        txt_atk.GetComponent<Text>().text = $"기본 {NowATK}";
        txt_hp.GetComponent<Text>().text = $"{NowHP} / {NowmaxHP}";
        txt_gold.GetComponent<Text>().text = $"{NowGold}";

        // 버프 시각화
        if (NowBuff == "") txt_buff.GetComponent<Text>().text = "버프 없음";
        else if (NowBuff.Substring(0, 1) == "b") txt_buff.GetComponent<Text>().text = $"{injson.jbuffData[int.Parse(NowBuff.Substring(4, 2))]["description"].ToString()}";      // json 활용해서 id에 맞는 효과 출력해주어야 합니다.
        else txt_buff.GetComponent<Text>().text = $"{injson.jbuffData[int.Parse(NowBuff.Substring(6, 2)) + 12]["description"].ToString()}";

        // 무기 시너지 시각화
        int[] WeaponSnCheck = new int[6] { 0, 0, 0, 0, 0, 0 };
        int maxnum = 0;
        string sntxt = "";

        for (int i = 0; i < 10; i++)
        {
            WeaponSnCheck[int.Parse(NowWeapon[i].Substring(0, 1))]++;
            if (maxnum < WeaponSnCheck[int.Parse(NowWeapon[i].Substring(0, 1))]) maxnum = WeaponSnCheck[int.Parse(NowWeapon[i].Substring(0, 1))];
        }

        for (int i = 0; i < 6; i++)
        {
            if (maxnum == WeaponSnCheck[int.Parse(NowWeapon[i].Substring(0, 1))])
            {
                if (sntxt == "") sntxt = weapontype[i];
                else sntxt = sntxt + "," + weapontype[i];

                if (i == 5) sntxt = "X";
            }
        }

        txt_snerge.GetComponent<Text>().text = sntxt;        // 시너지의 경우 무기 종류로 계산해야 합니다.


        // 장비/아티펙트 띄워주기
        for (int i = 0; i < 3; i++)
        {
            if (NowEquip[i].Substring(0, 1) == "0")
            {
                nowE[i].GetComponent<Image>().sprite = stageplay.uimask; // 장비 NONE
            }
            else if (NowEquip[i].Substring(0, 1) == "1")
            {
                nowE[i].GetComponent<Image>().sprite = stageplay.helmet;
            }
            else if (NowEquip[i].Substring(0, 1) == "2")
            {
                nowE[i].GetComponent<Image>().sprite = stageplay.armor;
            }
            else
            {
                nowE[i].GetComponent<Image>().sprite = stageplay.shoes;
            }

            if (NowArtifact[i].Substring(0, 2) == "99") nowA[i].GetComponent<Image>().sprite = stageplay.uimask;
            else nowA[i].GetComponent<Image>().sprite = Resources.Load($"image/Loveworld_At/artifact{NowArtifact[i].Substring(0, 2)}", typeof(Sprite)) as Sprite;
        }
    }

    // 플레이어 상태 변경
    public void StateChange()
    {
        StateUpdate();          // 임시

        if (statechange == 0)
        {
            statechange++;
            p_panel1.SetActive(false);
            p_panel2.SetActive(true);
        }
        else
        {
            statechange--;
            p_panel1.SetActive(true);
            p_panel2.SetActive(false);
        }
    }


    // 플레이어 장비/아티펙트 정보 확인
    public void StateEorAInfo()
    {
        string EorAname = EventSystem.current.currentSelectedGameObject.name;


        if (EorAname.Substring(3,1) == "E")         // 장비 선택 시
        {
            checkEorA.GetComponent<Image>().sprite = nowE[int.Parse(EorAname.Substring(4, 1))].GetComponent<Image>().sprite;

            int checkEindex = 3;
            if (NowEquip[int.Parse(EorAname.Substring(4, 1))].Substring(0, 1) == "0") checkEindex = int.Parse(EorAname.Substring(4, 1));
            else
            {
                checkEindex += 125 * (int.Parse(NowEquip[int.Parse(EorAname.Substring(4, 1))].Substring(0, 1)) - 1) +
                    25 * int.Parse(NowEquip[int.Parse(EorAname.Substring(4, 1))].Substring(1, 1)) +
                    5 * int.Parse(NowEquip[int.Parse(EorAname.Substring(4, 1))].Substring(2, 1)) +
                    int.Parse(NowEquip[int.Parse(EorAname.Substring(4, 1))].Substring(3, 1));
            }

            checktxtEorA.GetComponent<Text>().text = $"[{checkrank[int.Parse(injson.jequipData[checkEindex]["rank"].ToString()) + 1]}] {injson.jequipData[checkEindex]["name"].ToString()}";
            checkExplainEorA.GetComponent<Text>().text = $"";
        }
        else                                        // 아티펙트 선택 시
        {
            checkEorA.GetComponent<Image>().sprite = nowA[int.Parse(EorAname.Substring(4, 1))].GetComponent<Image>().sprite;

            checktxtEorA.GetComponent<Text>().text = $"{injson.jartifactData[int.Parse(EorAname.Substring(4, 1))]["name"]}";
            checkExplainEorA.GetComponent<Text>().text = $"";
        }
    }

    // 현재 무기 업데이트
    public void NowWeaponUpdate()
    {
        // 가진 무기에 맞는 이미지 및 공격력 넣어주어야 합니다. (맨주먹까지 6종류, 0 : 검, 1 : 망치, 2 : 창, 3 : 단검, 4 : 요술봉, 5 : 맨주먹)
        // 무기 체크해서 능력치 텍스트로 출력해야합니다.
        for (int i = 0; i < 10; i++)
        {
            int weaponindex = 1;
            if (NowWeapon[i].Substring(0, 1) == "5") weaponindex = 0;
            else
            {
                weaponindex += (int.Parse(NowWeapon[i].Substring(0, 1))) +
                    25 * (int.Parse(NowWeapon[i].Substring(1, 1))) +
                    5 * (int.Parse(NowWeapon[i].Substring(2, 1)));
            }

            for (int j = 0; j < 6; j++)
            {
                switch (NowWeapon[i].Substring(0, 1))
                {
                    case "0":
                        nowW[i].GetComponent<Image>().sprite = stageplay.sword;
                        nowtxtW[i].GetComponent<Text>().text = $"{NowWeaponATK[i]}";
                        break;
                    case "1":
                        nowW[i].GetComponent<Image>().sprite = stageplay.hammer;
                        nowtxtW[i].GetComponent<Text>().text = $"{NowWeaponATK[i]}";
                        break;
                    case "2":
                        nowW[i].GetComponent<Image>().sprite = stageplay.spear;
                        nowtxtW[i].GetComponent<Text>().text = $"{NowWeaponATK[i]}";
                        break;
                    case "3":
                        nowW[i].GetComponent<Image>().sprite = stageplay.dagger;
                        nowtxtW[i].GetComponent<Text>().text = $"{NowWeaponATK[i]}";
                        break;
                    case "4":
                        nowW[i].GetComponent<Image>().sprite = stageplay.wand;
                        nowtxtW[i].GetComponent<Text>().text = $"{NowWeaponATK[i]}";
                        break;
                    case "5":
                        nowW[i].GetComponent<Image>().sprite = stageplay.fist;
                        nowtxtW[i].GetComponent<Text>().text = $"{NowWeaponATK[i]}";
                        break;
                    default:
                        break;
                }
            }
        }
    }

    // 현재 무기 확인
    public void ShowEquip()
    {
        if (equiponoff == 0)
        {
            NowWeaponUpdate();
            equiponoff++;
            equiplist.SetActive(true);
        }
        else
        {
            equiponoff--;
            equiplist.SetActive(false);
        }
    }
}
