using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGamecs : MonoBehaviour
{
    InJson injson;

    // 필요 변수 및 오브젝트
    int statechange = 0;
    int equiponoff = 0;
    GameObject p_panel1, p_panel2;
    GameObject showequipbtn, equiplist;
    GameObject txt_world, txt_stage;

    // 장비 및 무기 타입
    string[] equiptype = new string[4] { "none", "helmet", "armor", "shoes" };
    string[] weapontype = new string[6] { "검", "망치", "창", "단검", "지팡이", "X" };

    // 게임 내 필요 오브젝트 (1)
    GameObject txt_speed, txt_def, txt_atk, txt_buff, txt_snerge, txt_hp;
    // 게임 내 필요 오브젝트 (2)
    GameObject img_equip1, img_equip2, img_equip3, img_artifact1, img_artifact2, img_artifact3;

    // 게임 내 플레이어 및 현재 상태 정보
    public int NowStage, NowSpeed, NowDEF, NowATK, NowHP, NowmaxHP;
    public float NowWorld;
    public string NowBuff;
    public string[] NowEquip = new string[3];
    public string[] NowWeapon = new string[10];
    public string[] NowArtifact = new string[3];


    // Start is called before the first frame update
    void Start()   
    {
        injson = GameObject.Find("EventSystem").GetComponent<InJson>();

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

        // 오브젝트 연결 (2)
        img_equip1 = GameObject.Find("Eq_Slot1");
        img_equip2 = GameObject.Find("Eq_Slot2");
        img_equip3 = GameObject.Find("Eq_Slot3");
        img_artifact1 = GameObject.Find("At_Slot1");
        img_artifact2 = GameObject.Find("At_Slot2");
        img_artifact3 = GameObject.Find("At_Slot3");


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

        // 초기 버프 및 장비 상태
        NowBuff = PlayerPrefs.GetString("Buff");
        NowEquip = PlayerPrefs.GetString("Equipment").Split(',');
        NowWeapon = PlayerPrefs.GetString("Weapon").Split(',');
        NowArtifact = PlayerPrefs.GetString("Artifact").Split(',');
        


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


        // 시각화 (2)      현재 아직 이미지 파일이 없습니다. 이미지 파일이 생기면 번호에 맞는 artifact를 가져옵니다.
        /*
        img_equip1.GetComponent<Image>().sprite = Resources.Load<Sprite>("");
        img_equip2.GetComponent<Image>().sprite = Resources.Load<Sprite>("");
        img_equip3.GetComponent<Image>().sprite = Resources.Load<Sprite>("");
        */
        if (NowArtifact[0] != "99") img_artifact1.GetComponent<Image>().sprite = Resources.Load<Sprite>($"artifact{NowArtifact[0]}");
        if (NowArtifact[1] != "99") img_artifact2.GetComponent<Image>().sprite = Resources.Load<Sprite>($"artifact{NowArtifact[1]}");
        if (NowArtifact[2] != "99") img_artifact3.GetComponent<Image>().sprite = Resources.Load<Sprite>($"artifact{NowArtifact[2]}");
        
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

    // 현재 장비 확인
    public void ShowEquip()
    {
        if (equiponoff == 0)
        {
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
