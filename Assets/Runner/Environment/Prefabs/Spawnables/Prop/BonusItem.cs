using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class BonusItem : MonoBehaviour
{
    public enum ePropType { JiaFen, TiShenJuanZhou, ZhuQingTing, TaiYangSan, HongSeNeiKu, LingYiTiaoGuiYu, ChongLangBan, YiDaLiPao }  //道具类型枚举，用拼音写了好理解
    public ePropType propType = ePropType.JiaFen;  //inspector中设置道具类型
    public Text text;  //引用text组件
    public string words;  //展示给玩家的物品描述
    public float showWordsDuration = 2;  //展示时间
    [Header("------所有加分道具------")]
    public float score = 100;  //得分
    public float jiaFenTipTime = 1f;  //得分时的提示持续时间  
    public float jiaFenTipUpwardDis = 0.5f;  //得分提示上升距离
    [Header("------竹蜻蜓------")]
    public float flightTime = 4;  //飞行时间
    public float flightHeight = 3;  //高度
    public float riseTime = 0.2f;  //上升时间
    public float fallTime = 0.2f;  //下降时间
    [Header("------红色内裤------")]
    public float dashTime = 3;  //冲刺时间
    public float dashSpeed = 20;  //速度
    [Header("------冲浪板------")]
    public float surf_Time = 4f;  //冲浪持续时间
    public float surf_Speed = 0;  //冲浪时的速度，取0时默认以player自动行进速度
    public float surfboardYOffset = -1;  //冲浪板生成时的Y偏移

    [Header("------意大利炮------")]
    public float bulletSpeed = 20f;  //炮弹速度
    public float bulletLifeTime = 4;  //炮弹存在时间
    public float bulletAppearZOffset = 2;  //炮弹在player前面多远处生成
    public float bulletAppearYOffset = 1;  //炮弹相对player偏移

    private GameObject player;  //发生碰撞时引用
    private void Start()
    {
        text.text = "";
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            player = collision.gameObject;
            switch (propType)
            {
                case ePropType.JiaFen: StartCoroutine(JiaFen()); break;
                case ePropType.TiShenJuanZhou: StartCoroutine(TiShenJuanZhou()); break;
                case ePropType.ZhuQingTing: StartCoroutine(ZhuQingTing()); break;
                case ePropType.TaiYangSan: StartCoroutine(TaiYangSan()); break;
                case ePropType.HongSeNeiKu: StartCoroutine(HongSeNeiKu()); break;
                case ePropType.LingYiTiaoGuiYu: StartCoroutine(LingYiTiaoGuiYu()); break;
                case ePropType.ChongLangBan: StartCoroutine(ChongLangBan()); break;
                case ePropType.YiDaLiPao: StartCoroutine(YiDaLiPao()); break;
            }

        }
    }
    private void OnTriggerEnter(Collider other)
    {
        player = other.gameObject;
        if (other.gameObject.tag == "Player")
        {
            switch (propType)
            {
                case ePropType.JiaFen: StartCoroutine(JiaFen()); break;
                case ePropType.TiShenJuanZhou: StartCoroutine(TiShenJuanZhou()); break;
                case ePropType.ZhuQingTing: StartCoroutine(ZhuQingTing()); break;
                case ePropType.TaiYangSan: StartCoroutine(TaiYangSan()); break;
                case ePropType.HongSeNeiKu: StartCoroutine(HongSeNeiKu()); break;
                case ePropType.LingYiTiaoGuiYu: StartCoroutine(LingYiTiaoGuiYu()); break;
                case ePropType.ChongLangBan: StartCoroutine(ChongLangBan()); break;
                case ePropType.YiDaLiPao: StartCoroutine(YiDaLiPao()); break;
            }

        }
    }


    IEnumerator JiaFen()//加分道具
    {
        text.text = words;
        StartCoroutine(player.GetComponent<PlayerOwnPropsNum>().GetJiaFen(score, jiaFenTipTime, jiaFenTipUpwardDis));

        yield return new WaitForSeconds(showWordsDuration);
        text.text = "";
    }

    IEnumerator TiShenJuanZhou()  //替身卷轴
    {
        text.text = words;
        player.GetComponent<PlayerOwnPropsNum>().TiShenJuanZhou++;

        yield return new WaitForSeconds(showWordsDuration);
        text.text = "";
    }

    IEnumerator ZhuQingTing()  //竹蜻蜓
    {
        text.text = words;
        player.GetComponent<PlayerOwnPropsNum>().ZhuQingTing++;
        StartCoroutine(player.GetComponent<PlayerOwnPropsNum>().GetZhuQingTing(flightTime, flightHeight, riseTime, fallTime));

        yield return new WaitForSeconds(showWordsDuration);
        text.text = "";
    }

    IEnumerator TaiYangSan()  //太阳伞
    {
        text.text = words;
        player.GetComponent<PlayerOwnPropsNum>().TaiYangSan++;

        yield return new WaitForSeconds(showWordsDuration);
        text.text = "";
    }
    IEnumerator HongSeNeiKu()  //红色内裤
    {
        text.text = words;
        player.GetComponent<PlayerOwnPropsNum>().HongSeNeiKu++;
        StartCoroutine(player.GetComponent<PlayerOwnPropsNum>().GetHondSeNeiKu(dashTime, dashSpeed));

        yield return new WaitForSeconds(showWordsDuration);
        text.text = "";
    }
    IEnumerator ChongLangBan()  //冲浪板
    {
        text.text = words;
        player.GetComponent<PlayerOwnPropsNum>().ChongLangBan++;
        StartCoroutine(player.GetComponent<PlayerOwnPropsNum>().GetChongLangBan(surf_Time, surf_Speed, surfboardYOffset));

        yield return new WaitForSeconds(showWordsDuration);
        text.text = "";
    }

    IEnumerator LingYiTiaoGuiYu()  //另一条鲑鱼
    {
        text.text = words;
        player.GetComponent<PlayerOwnPropsNum>().LingYiTiaoGuiYu++;

        yield return new WaitForSeconds(showWordsDuration);
        text.text = "";
    }

    IEnumerator YiDaLiPao()  //意大利炮
    {
        text.text = words;
        player.GetComponent<PlayerOwnPropsNum>().YiDaLiPao++;
        StartCoroutine(player.GetComponent<PlayerOwnPropsNum>().GetYiDaLiPao(bulletSpeed, bulletLifeTime, bulletAppearZOffset, bulletAppearYOffset));

        yield return new WaitForSeconds(showWordsDuration);
        text.text = "";
    }
}

