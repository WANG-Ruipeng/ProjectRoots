using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HyperCasual.Runner;

public class PlayerOwnPropsNum : MonoBehaviour
{
    public int JiaFen, TiShenJuanZhou, ZhuQingTing, TaiYangSan, HongSeNeiKu, LingYiTiaoGuiYu, ChongLangBan, YiDaLiPao;  //��õĵ�������
    public bool isDashing;  //���ڳ��״̬
    public bool isSurfing;  //���ڳ���״̬

    public GameObject bulletPrefab;  //�ڵ���Ԥ��
    public GameObject surfboardPrefab;  //���˰�Ԥ��

    private void Start()
    {
        TiShenJuanZhou = ZhuQingTing = TaiYangSan = HongSeNeiKu = LingYiTiaoGuiYu = ChongLangBan = YiDaLiPao = 0;
        isDashing = false;
        isSurfing = false;
    }





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



    #region ��ɫ�ڿ�
    private float dashTime, dashSpeed;
    public IEnumerator GetHondSeNeiKu(float dT, float dS)
    {
        dashTime = dT;
        dashSpeed = dS;

        PlayerController playerController = GetComponent<PlayerController>();
        float originalZMaxSpeed = playerController.ZMoveSpeed;
        float originalZAccelaration = playerController.ZMoveAcceraltion;

        playerController.ZMoveSpeed = dashSpeed;  //�������x�ٶ�
        playerController.ZMoveAcceraltion *= 5;  //���ٶȷ�5��
        isDashing = true;  //���״̬


        yield return new WaitForSeconds(dashTime);
        playerController.ZMoveSpeed = originalZMaxSpeed;
        playerController.ZMoveAcceraltion = originalZAccelaration;
        isDashing = false;
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
