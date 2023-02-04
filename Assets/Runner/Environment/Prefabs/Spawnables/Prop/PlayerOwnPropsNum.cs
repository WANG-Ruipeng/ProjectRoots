using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HyperCasual.Runner;

public class PlayerOwnPropsNum : MonoBehaviour
{
    public int JiaFen, TiShenJuanZhou, ZhuQingTing, TaiYangSan, HongSeNeiKu, LingYiTiaoGuiYu, ChongLangBan, YiDaLiPao;  //获得的道具数量
    public bool isDashing;  //正在冲刺状态
    public bool isSurfing;  //正在冲浪状态

    public GameObject bulletPrefab;  //炮弹的预设
    public GameObject surfboardPrefab;  //冲浪板预设

    private void Start()
    {
        TiShenJuanZhou = ZhuQingTing = TaiYangSan = HongSeNeiKu = LingYiTiaoGuiYu = ChongLangBan = YiDaLiPao = 0;
        isDashing = false;
        isSurfing = false;
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
        isDashing = false;
    }
    #endregion

    #region 冲浪板
    public IEnumerator GetChongLangBan(float surf_Time, float surf_Speed, float surfboardYOffset)
    {
        GameObject surfboard = Instantiate(surfboardPrefab);  //生成冲浪板
        surfboard.transform.SetParent(this.transform);  //设置冲浪板为player的子对象
        surfboard.transform.localPosition = new Vector3(0, surfboardYOffset, 0);  //初始化冲浪板位置(注意是localPosition)
        isSurfing = true;  //状态为冲浪

        //设置速度
        PlayerController playerController = GetComponent<PlayerController>();
        float originalZMaxSpeed = playerController.ZMoveSpeed;  //保存原速度
        float originalZAccelaration = playerController.ZMoveAcceraltion;
        if (surf_Speed != 0)
        {
            playerController.ZMoveSpeed = surf_Speed;  //更改最大z速度
            playerController.ZMoveAcceraltion *= 5;  //加速度翻5倍
        }
        yield return new WaitForSeconds(surf_Time);
        Destroy(surfboard);  //销毁滑板
        isSurfing = false;
        if (surf_Speed != 0)  //复原
        {
            playerController.ZMoveSpeed = originalZMaxSpeed;
            playerController.ZMoveAcceraltion = originalZAccelaration;
        }
    }
    #endregion


    #region 意大利炮
    public IEnumerator GetYiDaLiPao(float bulletSpeed, float bulletLifeTime, float bulletAppearZOffset, float bulletAppearYOffset)
    {
        GameObject bullet = Instantiate(bulletPrefab);  //生成炮弹
        bullet.transform.position = new Vector3(transform.position.x, transform.position.y + bulletAppearYOffset, transform.position.z + bulletAppearZOffset);  //初始化炮弹位置
        Physics.IgnoreCollision(this.GetComponent<Collider>(), bullet.GetComponent<Collider>(), true);  //忽略player和炮弹的碰撞
        bullet.GetComponent<Rigidbody>().useGravity = false;
        bullet.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, bulletSpeed);  //设置炮弹速度

        yield return new WaitForSeconds(bulletLifeTime);
        Destroy(bullet);  //销毁
    }
    #endregion





}
