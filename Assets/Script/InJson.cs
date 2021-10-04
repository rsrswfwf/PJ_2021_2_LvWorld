using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using System.IO;

public class InJson : MonoBehaviour
{
    InGamecs ingamecs;

    string jbuff, jequip, jachievment, jmonster, jweapon, jartifact;
    public JsonData jbuffData, jequipData, jachievmentData, jmonsterData, jweaponData, jartifactData;

    public 
    // Start is called before the first frame update
    void Start()
    {
        ingamecs = GameObject.Find("EventSystem").GetComponent<InGamecs>();

        JToSset();
    }

    // Json 버프 파일을 String 및 data화 시킴
    public void JToSset()
    {
        // 장비
        jequip = File.ReadAllText(Application.dataPath + "/Script/Json/equipment.json");
        jequipData = JsonMapper.ToObject(jequip);
        // 버프
        jbuff = File.ReadAllText(Application.dataPath + "/Script/Json/buff.json");
        jbuffData = JsonMapper.ToObject(jbuff);
        // 아티펙트
        jartifact = File.ReadAllText(Application.dataPath + "/Script/Json/artifact.json");
        jartifactData = JsonMapper.ToObject(jartifact);
        // 몬스터
        jmonster = File.ReadAllText(Application.dataPath + "/Script/Json/monster.json");
        jmonsterData = JsonMapper.ToObject(jmonster);
        // 무기
        jweapon = File.ReadAllText(Application.dataPath + "/Script/Json/weapon.json");
        jweaponData = JsonMapper.ToObject(jweapon);
        // 업적
        jachievment = File.ReadAllText(Application.dataPath + "/Script/Json/achievment.json");
        jachievmentData = JsonMapper.ToObject(jachievment);
    }

}
