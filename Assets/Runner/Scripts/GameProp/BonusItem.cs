using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class BonusItem : MonoBehaviour
{
    public enum ePropType { JiaFen, TiShenJuanZhou, ZhuQingTing, TaiYangSan, HongSeNeiKu, LingYiTiaoGuiYu, ChongLangBan, YiDaLiPao }  //��������ö�٣���ƴ��д�˺����
    public ePropType propType = ePropType.JiaFen;  //inspector�����õ�������
    public Text text;  //����text���
    public string words;  //չʾ����ҵ���Ʒ����
    public float showWordsDuration = 2;  //չʾʱ��
    [Header("------���мӷֵ���------")]
    public int credit = 100;  //�÷�
    [Header("------������------")]
    public float flightTime = 4;  //����ʱ��
    public float flightHeight = 3;  //�߶�
    public float riseTime = 0.2f;  //����ʱ��
    public float fallTime = 0.2f;  //�½�ʱ��
    [Header("------��ɫ�ڿ�------")]
    public float dashTime = 3;  //���ʱ��
    public float dashSpeed = 20;  //�ٶ�


    private GameObject player;  //������ײʱ����
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


    IEnumerator JiaFen()//�ӷֵ���
    {
        text.text = words;
        player.GetComponent<PlayerOwnPropsNum>().JiaFen++;

        yield return new WaitForSeconds(showWordsDuration);
        text.text = "";
    }

    IEnumerator TiShenJuanZhou()  //�������
    {
        text.text = words;
        player.GetComponent<PlayerOwnPropsNum>().TiShenJuanZhou++;

        yield return new WaitForSeconds(showWordsDuration);
        text.text = "";
    }

    IEnumerator ZhuQingTing()  //������
    {
        text.text = words;
        player.GetComponent<PlayerOwnPropsNum>().ZhuQingTing++;
        StartCoroutine(player.GetComponent<PlayerOwnPropsNum>().GetZhuQingTing(flightTime, flightHeight, riseTime, fallTime));

        yield return new WaitForSeconds(showWordsDuration);
        text.text = "";
    }

    IEnumerator TaiYangSan()  //̫��ɡ
    {
        text.text = words;
        player.GetComponent<PlayerOwnPropsNum>().TaiYangSan++;

        yield return new WaitForSeconds(showWordsDuration);
        text.text = "";
    }
    IEnumerator HongSeNeiKu()  //��ɫ�ڿ�
    {
        text.text = words;
        player.GetComponent<PlayerOwnPropsNum>().HongSeNeiKu++;
        StartCoroutine(player.GetComponent<PlayerOwnPropsNum>().GetHondSeNeiKu(dashTime, dashSpeed));

        yield return new WaitForSeconds(showWordsDuration);
        text.text = "";
    }
    IEnumerator ChongLangBan()  //���˰�
    {
        text.text = words;
        player.GetComponent<PlayerOwnPropsNum>().ChongLangBan++;

        yield return new WaitForSeconds(showWordsDuration);
        text.text = "";
    }

    IEnumerator LingYiTiaoGuiYu()  //��һ������
    {
        text.text = words;
        player.GetComponent<PlayerOwnPropsNum>().LingYiTiaoGuiYu++;

        yield return new WaitForSeconds(showWordsDuration);
        text.text = "";
    }

    IEnumerator YiDaLiPao()  //�������
    {
        text.text = words;
        player.GetComponent<PlayerOwnPropsNum>().YiDaLiPao++;

        yield return new WaitForSeconds(showWordsDuration);
        text.text = "";
    }
}

