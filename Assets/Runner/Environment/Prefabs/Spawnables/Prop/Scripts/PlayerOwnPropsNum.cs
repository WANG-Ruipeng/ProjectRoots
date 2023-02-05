using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HyperCasual.Runner;
using UnityEngine.UI;
using System;
using TMPro;

public class PlayerOwnPropsNum : MonoBehaviour
{
    public GameObject bulletPrefab;  //炮弹的预设
    public GameObject surfboardPrefab;  //冲浪板预设
    public GameObject scoreTipPrefab;  //分数提示预设
    public GameObject tailGasPrefab;  //冲刺尾气粒子效果预设

    public bool isDashing;  //正在冲刺状态
    public bool isSurfing;  //正在冲浪状态

    public float totalPoints;
    public int ZhuQingTing, HongSeNeiKu, ChongLangBan, YiDaLiPao;  //获得的道具数量
    //替身卷轴->三叉戟，太阳伞->小帮手
    [SerializeField] private int _TiShenJuanZhou, _TaiYangSan, _LingYiTiaoGuiYu;  //这几个道具不是立即使用的
    #region   修改这些非立即使用道具时跳出提示
    public int TiShenJuanZhou
    {
        get { return _TiShenJuanZhou; }
        set
        {
            if (value > _TiShenJuanZhou) { StartCoroutine(PropTip("三叉戟 + 1")); _TiShenJuanZhou++; }
            else if (value < _TiShenJuanZhou) { StartCoroutine(PropTip("三叉戟 - 1")); _TiShenJuanZhou--; }
        }
    }

    public int TaiYangSan
    {
        get { return _TaiYangSan; }
        set
        {
            if (value > _TaiYangSan) { StartCoroutine(PropTip("小帮手 + 1")); _TaiYangSan++; }
            else if (value < _TaiYangSan) { StartCoroutine(PropTip("小帮手 - 1")); _TaiYangSan--; }
        }
    }

    public int LingYiTiaoGuiYu
    {
        get { return _LingYiTiaoGuiYu; }
        set
        {
            if (value > _LingYiTiaoGuiYu) { StartCoroutine(PropTip("另一条鲑鱼 + 1")); _LingYiTiaoGuiYu++; }
            else if (value < _LingYiTiaoGuiYu) { StartCoroutine(PropTip("另一条鲑鱼 - 1")); _LingYiTiaoGuiYu--; }
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


    #region 加分
    public IEnumerator GetJiaFen(float score, float tipTime, float tipUpwardDis)
    {
        totalPoints += score;  //计入总分
                               //跳出加分提示
        GameObject scoreTip = Instantiate(scoreTipPrefab);  //实例化
        Vector3 pos = scoreTip.transform.position;
        scoreTip.transform.SetParent(this.transform);  //设置为player的子对象
        scoreTip.transform.localPosition = pos;  //设置了父对象后position会有变动，所以重新赋值
        scoreTip.GetComponent<TextMeshPro>().text = "+" + score + "pts";  //设置文本内容
        StartCoroutine(ScoreTipUpward(scoreTip, tipUpwardDis / tipTime));  //使tip上升

        yield return new WaitForSeconds(tipTime);
        StopCoroutine("ScoreTipUpward");
        Destroy(scoreTip);  //销毁提示
    }

    IEnumerator ScoreTipUpward(GameObject scoreTip, float upwardSpeed)
    {
        if (scoreTip == null) { Debug.Log("scoreTip==null"); yield return 0; }

        Vector3 pos = scoreTip.transform.localPosition;
        scoreTip.transform.localPosition = new Vector3(pos.x, (float)(pos.y + upwardSpeed * 0.02), pos.z);
        yield return new WaitForSeconds(0.02f);  //每0.02s执行一次
        StartCoroutine(ScoreTipUpward(scoreTip, upwardSpeed));
    }
    #endregion


    #region 部分道具增减时弹出提示
    [Header("部分道具增减时弹出提示的参数")]
    public float propTipTime = 1;  //持续时间
    public float propTipUpwardDis = 5;  //提示上移距离
    IEnumerator PropTip(string words)
    {
        //跳出加分提示
        GameObject propTip = Instantiate(scoreTipPrefab);  //实例化
        Vector3 pos = propTip.transform.position;
        propTip.transform.SetParent(this.transform);  //设置为player的子对象
        propTip.transform.localPosition = pos;
        propTip.GetComponent<TextMeshPro>().text = words;  //设置文本内容
        StartCoroutine(ScoreTipUpward(propTip, propTipUpwardDis / propTipTime));  //使tip上升

        yield return new WaitForSeconds(propTipTime);
        StopCoroutine("PropTipUpward");
        Destroy(propTip);  //销毁提示
    }

    IEnumerator PropTipUpward(GameObject propTip, float upwardSpeed)
    {
        if (propTip == null) { Debug.Log("propTip==null"); yield return 0; }

        Vector3 pos = propTip.transform.localPosition;
        propTip.transform.localPosition = new Vector3(pos.x, (float)(pos.y + upwardSpeed * 0.02), pos.z);
        yield return new WaitForSeconds(0.02f);  //每0.02s执行一次
        StartCoroutine(ScoreTipUpward(propTip, upwardSpeed));
    }
    #endregion


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


    #region 红色内裤（冲刺）
    private float dashTime, dashSpeed, _originalZMaxSpeed, _originalZAccelaration;
    private GameObject tailGas;
    public IEnumerator GetHondSeNeiKu(float dT, float dS)
    {
        dashTime = dT;
        dashSpeed = dS;
        //保存原速度
        PlayerController playerController = GetComponent<PlayerController>();
        _originalZMaxSpeed = playerController.ZMoveSpeed;
        _originalZAccelaration = playerController.ZMoveAcceraltion;
        //修改速度
        playerController.ZMoveSpeed = dashSpeed;  //更改最大x速度
        playerController.ZMoveAcceraltion *= 5;  //加速度翻5倍
        isDashing = true;  //冲刺状态
                           //增加尾气粒子效果
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
        Destroy(tailGas);  //消除粒子
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
