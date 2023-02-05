using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HyperCasual.Runner;
using UnityEngine.UI;
using System;
using TMPro;

public class PlayerOwnPropsNum : MonoBehaviour
{
    public GameObject bulletPrefab;  //�ڵ���Ԥ��
    public GameObject surfboardPrefab;  //���˰�Ԥ��
    public GameObject scoreTipPrefab;  //������ʾԤ��
    public GameObject tailGasPrefab;  //���β������Ч��Ԥ��

    public bool isDashing;  //���ڳ��״̬
    public bool isSurfing;  //���ڳ���״̬

    public float totalPoints;
    public int ZhuQingTing, HongSeNeiKu, ChongLangBan, YiDaLiPao;  //��õĵ�������
    //�������->����ꪣ�̫��ɡ->С����
    [SerializeField] private int _TiShenJuanZhou, _TaiYangSan, _LingYiTiaoGuiYu;  //�⼸�����߲�������ʹ�õ�
    #region   �޸���Щ������ʹ�õ���ʱ������ʾ
    public int TiShenJuanZhou
    {
        get { return _TiShenJuanZhou; }
        set
        {
            if (value > _TiShenJuanZhou) { StartCoroutine(PropTip("����� + 1")); _TiShenJuanZhou++; }
            else if (value < _TiShenJuanZhou) { StartCoroutine(PropTip("����� - 1")); _TiShenJuanZhou--; }
        }
    }

    public int TaiYangSan
    {
        get { return _TaiYangSan; }
        set
        {
            if (value > _TaiYangSan) { StartCoroutine(PropTip("С���� + 1")); _TaiYangSan++; }
            else if (value < _TaiYangSan) { StartCoroutine(PropTip("С���� - 1")); _TaiYangSan--; }
        }
    }

    public int LingYiTiaoGuiYu
    {
        get { return _LingYiTiaoGuiYu; }
        set
        {
            if (value > _LingYiTiaoGuiYu) { StartCoroutine(PropTip("��һ������ + 1")); _LingYiTiaoGuiYu++; }
            else if (value < _LingYiTiaoGuiYu) { StartCoroutine(PropTip("��һ������ - 1")); _LingYiTiaoGuiYu--; }
        }
    }
    #endregion




    private void Start()
    {
        totalPoints = 0;
        _TiShenJuanZhou = ZhuQingTing = _TaiYangSan = HongSeNeiKu = _LingYiTiaoGuiYu = ChongLangBan = YiDaLiPao = 0;
        isDashing = false;
        isSurfing = false;
    }


    #region �ӷ�
    public IEnumerator GetJiaFen(float score, float tipTime, float tipUpwardDis)
    {
        totalPoints += score;  //�����ܷ�
                               //�����ӷ���ʾ
        GameObject scoreTip = Instantiate(scoreTipPrefab);  //ʵ����
        Vector3 pos = scoreTip.transform.position;
        scoreTip.transform.SetParent(this.transform);  //����Ϊplayer���Ӷ���
        scoreTip.transform.localPosition = pos;  //�����˸������position���б䶯���������¸�ֵ
        scoreTip.GetComponent<TextMeshPro>().text = "+" + score + "pts";  //�����ı�����
        StartCoroutine(ScoreTipUpward(scoreTip, tipUpwardDis / tipTime));  //ʹtip����

        yield return new WaitForSeconds(tipTime);
        StopCoroutine("ScoreTipUpward");
        Destroy(scoreTip);  //������ʾ
    }

    IEnumerator ScoreTipUpward(GameObject scoreTip, float upwardSpeed)
    {
        if (scoreTip == null) { Debug.Log("scoreTip==null"); yield return 0; }

        Vector3 pos = scoreTip.transform.localPosition;
        scoreTip.transform.localPosition = new Vector3(pos.x, (float)(pos.y + upwardSpeed * 0.02), pos.z);
        yield return new WaitForSeconds(0.02f);  //ÿ0.02sִ��һ��
        StartCoroutine(ScoreTipUpward(scoreTip, upwardSpeed));
    }
    #endregion


    #region ���ֵ�������ʱ������ʾ
    [Header("���ֵ�������ʱ������ʾ�Ĳ���")]
    public float propTipTime = 1;  //����ʱ��
    public float propTipUpwardDis = 5;  //��ʾ���ƾ���
    IEnumerator PropTip(string words)
    {
        //�����ӷ���ʾ
        GameObject propTip = Instantiate(scoreTipPrefab);  //ʵ����
        Vector3 pos = propTip.transform.position;
        propTip.transform.SetParent(this.transform);  //����Ϊplayer���Ӷ���
        propTip.transform.localPosition = pos;
        propTip.GetComponent<TextMeshPro>().text = words;  //�����ı�����
        StartCoroutine(ScoreTipUpward(propTip, propTipUpwardDis / propTipTime));  //ʹtip����

        yield return new WaitForSeconds(propTipTime);
        StopCoroutine("PropTipUpward");
        Destroy(propTip);  //������ʾ
    }

    IEnumerator PropTipUpward(GameObject propTip, float upwardSpeed)
    {
        if (propTip == null) { Debug.Log("propTip==null"); yield return 0; }

        Vector3 pos = propTip.transform.localPosition;
        propTip.transform.localPosition = new Vector3(pos.x, (float)(pos.y + upwardSpeed * 0.02), pos.z);
        yield return new WaitForSeconds(0.02f);  //ÿ0.02sִ��һ��
        StartCoroutine(ScoreTipUpward(propTip, upwardSpeed));
    }
    #endregion


    #region ������ 
    private float flightTime, flightHeight, riseTime, fallTime;
    public IEnumerator GetZhuQingTing(float flightT, float flightH, float riseT, float fallT)  //�����ߵĽű�����
    {
        flightTime = flightT;
        flightHeight = flightH;
        riseTime = riseT;
        fallTime = fallT;
        Debug.Log("zhu");
        InvokeRepeating("ZhuQingTingRise", 0, 0.01f);  //��ʼ����
        yield return new WaitForSeconds(riseTime);
        CancelInvoke("ZhuQingTingRise");  //ֹͣ����

        InvokeRepeating("ZhuQingTingKeepFlying", 0, 0.01f);  //���ַ���
        yield return new WaitForSeconds(flightTime);
        CancelInvoke("ZhuQingTingKeepFlying");

        InvokeRepeating("ZhuQingTingFall", 0, 0.01f);  //��ʼ����
        yield return new WaitForSeconds(fallTime);
        CancelInvoke("ZhuQingTingFall");

        Vector3 pos = transform.position;
        transform.position = new Vector3(pos.x, 0, pos.z);
        GetComponent<Rigidbody>().useGravity = true;
    }
    private void ZhuQingTingRise()
    {
        GetComponent<Rigidbody>().useGravity = false;
        Vector3 pos = transform.position;
        transform.position = new Vector3(pos.x, pos.y + flightHeight / riseTime * 0.01f, pos.z);
    }
    private void ZhuQingTingKeepFlying()
    {
        GetComponent<Rigidbody>().useGravity = false;
        Vector3 pos = transform.position;
        transform.position = new Vector3(pos.x, flightHeight, pos.z);
    }
    private void ZhuQingTingFall()
    {
        GetComponent<Rigidbody>().useGravity = false;
        Vector3 pos = transform.position;
        transform.position = new Vector3(pos.x, pos.y - flightHeight / riseTime * 0.01f, pos.z);
    }
    #endregion


    #region ��ɫ�ڿ㣨��̣�
    private float dashTime, dashSpeed, _originalZMaxSpeed, _originalZAccelaration;
    private GameObject tailGas;
    public IEnumerator GetHondSeNeiKu(float dT, float dS)
    {
        dashTime = dT;
        dashSpeed = dS;
        //����ԭ�ٶ�
        PlayerController playerController = GetComponent<PlayerController>();
        _originalZMaxSpeed = playerController.ZMoveSpeed;
        _originalZAccelaration = playerController.ZMoveAcceraltion;
        //�޸��ٶ�
        playerController.ZMoveSpeed = dashSpeed;  //�������x�ٶ�
        playerController.ZMoveAcceraltion *= 5;  //���ٶȷ�5��
        isDashing = true;  //���״̬
                           //����β������Ч��
        tailGas = Instantiate(tailGasPrefab);
        Vector3 tGPos = tailGas.transform.localPosition;
        tailGas.transform.SetParent(this.transform);
        tailGas.transform.localPosition = tGPos;
        tailGas.GetComponent<ParticleSystem>().Play(true);


        yield return new WaitForSeconds(dashTime);
        Invoke("StopDashing", dashTime);
    }
    public void StopDashing()
    {
        PlayerController playerController = GetComponent<PlayerController>();
        playerController.ZMoveSpeed = _originalZMaxSpeed;
        print("originalZ  " + _originalZMaxSpeed);
        playerController.ZMoveAcceraltion = _originalZAccelaration;
        isDashing = false;
        Destroy(tailGas);  //��������
    }
    #endregion


    #region ���˰�
    public IEnumerator GetChongLangBan(float surf_Time, float surf_Speed, float surfboardYOffset)
    {
        GameObject surfboard = Instantiate(surfboardPrefab);  //���ɳ��˰�
        surfboard.transform.SetParent(this.transform);  //���ó��˰�Ϊplayer���Ӷ���
        surfboard.transform.localPosition = new Vector3(0, surfboardYOffset, 0);  //��ʼ�����˰�λ��(ע����localPosition)
        isSurfing = true;  //״̬Ϊ����

        //�����ٶ�
        PlayerController playerController = GetComponent<PlayerController>();
        float originalZMaxSpeed = playerController.ZMoveSpeed;  //����ԭ�ٶ�
        float originalZAccelaration = playerController.ZMoveAcceraltion;
        if (surf_Speed != 0)
        {
            playerController.ZMoveSpeed = surf_Speed;  //�������z�ٶ�
            playerController.ZMoveAcceraltion *= 5;  //���ٶȷ�5��
        }
        yield return new WaitForSeconds(surf_Time);
        Destroy(surfboard);  //���ٻ���
        isSurfing = false;
        if (surf_Speed != 0)  //��ԭ
        {
            playerController.ZMoveSpeed = originalZMaxSpeed;
            playerController.ZMoveAcceraltion = originalZAccelaration;
        }
    }
    #endregion


    #region �������
    public IEnumerator GetYiDaLiPao(float bulletSpeed, float bulletLifeTime, float bulletAppearZOffset, float bulletAppearYOffset)
    {
        GameObject bullet = Instantiate(bulletPrefab);  //�����ڵ�
        bullet.transform.position = new Vector3(transform.position.x, transform.position.y + bulletAppearYOffset, transform.position.z + bulletAppearZOffset);  //��ʼ���ڵ�λ��
        Physics.IgnoreCollision(this.GetComponent<Collider>(), bullet.GetComponent<Collider>(), true);  //����player���ڵ�����ײ
        bullet.GetComponent<Rigidbody>().useGravity = false;
        bullet.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, bulletSpeed);  //�����ڵ��ٶ�

        yield return new WaitForSeconds(bulletLifeTime);
        Destroy(bullet);  //����
    }
    #endregion





}
