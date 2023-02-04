using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HyperCasual.Runner;

public class PlayerOwnPropsNum : MonoBehaviour
{
    public int JiaFen, TiShenJuanZhou, ZhuQingTing, TaiYangSan, HongSeNeiKu, LingYiTiaoGuiYu, ChongLangBan, YiDaLiPao;  //获得的道具数量
    public bool isDashing;  //正在冲刺状态

    private void Start()
    {
        TiShenJuanZhou = ZhuQingTing = TaiYangSan = HongSeNeiKu = LingYiTiaoGuiYu = ChongLangBan = YiDaLiPao = 0;
        isDashing = false;
    }

    #region 竹蜻蜓 
    private float flightTime, flightHeight, riseTime, fallTime;




    public IEnumerator GetZhuQingTing(float flightT, float flightH, float riseT, float fallT)  //被道具的脚本调用
    {
        flightTime = flightT;
        flightHeight = flightH;
        riseTime = riseT;
        fallTime = fallT;
        Debug.Log("zhu");
        InvokeRepeating("ZhuQingTingRise", 0, 0.01f);  //开始上升
        yield return new WaitForSeconds(riseTime);
        CancelInvoke("ZhuQingTingRise");  //停止上升

        InvokeRepeating("ZhuQingTingKeepFlying", 0, 0.01f);  //保持飞行
        yield return new WaitForSeconds(flightTime);
        CancelInvoke("ZhuQingTingKeepFlying");

        InvokeRepeating("ZhuQingTingFall", 0, 0.01f);  //开始下落
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



    #region 红色内裤
    private float dashTime, dashSpeed;
    public IEnumerator GetHondSeNeiKu(float dT, float dS)
    {
        dashTime = dT;
        dashSpeed = dS;

        PlayerController playerController = GetComponent<PlayerController>();
        float originalZMaxSpeed = playerController.ZMoveSpeed;
        float originalZAccelaration = playerController.ZMoveAcceraltion;

        playerController.ZMoveSpeed = dashSpeed;  //更改最大x速度
        playerController.ZMoveAcceraltion *= 5;  //加速度翻5倍
        isDashing = true;  //冲刺状态


        yield return new WaitForSeconds(dashTime);
        playerController.ZMoveSpeed = originalZMaxSpeed;
        playerController.ZMoveAcceraltion = originalZAccelaration;
    }
    #endregion

    #region 冲浪板
    #endregion


    #region 意大利炮
    #endregion





}
