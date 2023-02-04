using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HyperCasual.Runner;
public class AirPlane : Enemy
{
    [Header("进入画面花费的时间")]
    /// <summary>
    /// 进入画面花费的时间
    /// </summary>
    public float enterTime;
    [Header("生成时和玩家的水平距离(z)")]
    /// <summary>
    /// 和镜头的水平距离
    /// </summary>
    public float horizontalDistance;

    [Header("飞行高度(以当前水面为基准)")]
    /// <summary>
    /// 飞行高度
    /// </summary>
    public float height;

    [Header("生成时的X坐标")]
    public float initialX;

    [Header("z轴感知范围")]
    /// <summary>
    /// 感知到主角并且开始追踪的范围
    /// </summary>
    public float perceptionRange = 5f;

    [Header("发射时间间隔")]
    public float transmissionInterval = 0.3f;

    public enum LeftOrRight { left, right }
    [Header("出生在左边或者右边")]
    public LeftOrRight leftOrRight;

    [Header("飞行速度")]
    public float speed;

    public GameObject leftFire;
    public GameObject rightFire;
    public AirplaneBullet bulletPrefab;
    PlayerController Player => PlayerController.Instance;
    Vector3 PlayerPos => PlayerController.Instance.Transform.position;
    Vector3 Pos => transform.position;
    bool hasEnterScreen = false;
    bool hasFindPlayer;
    float startTime;
    Rigidbody rb;
    bool InsideRange
    {
        get
        {
            if (Pos.z - PlayerPos.z < perceptionRange)
                return true;
            return false;
        }
    }
    void Percive()
    {
        hasFindPlayer = true;
        startTime = Time.time;
    }

    //进入画面
    void EnterScreen()
    {
        float u;
        u = (Time.time - startTime) / enterTime;
        if (u >= 1)
        {
            hasEnterScreen = true;
            startTime = Time.time;


            return;
        }
        Vector3 targetPosition = Pos;
        targetPosition.y = height;
        targetPosition.z = PlayerPos.z + horizontalDistance;
        float boundY = GetCameraYBound(PlayerPos.z + horizontalDistance);
        if (leftOrRight == LeftOrRight.right)//在右边
        {
            //cameraBoundX > 0
            targetPosition.x = initialX;
        }
        else//在左边
        {
            targetPosition.x = -initialX;
        }
        transform.position = new Vector3(initialX, u * targetPosition.y + (1 - u) * boundY, targetPosition.z);

    }

    void Fire()
    {
        if (Time.time - startTime < transmissionInterval)
            return;


        Instantiate(bulletPrefab.gameObject, leftFire.transform);
        Instantiate(bulletPrefab.gameObject, rightFire.transform);
        startTime = Time.time;

    }
    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody>();
    }
    protected override void Update()
    {
        base.Update();
        if (!Application.isPlaying)//防止在编辑界面也会动
            return;
        if (Pos.z < Camera.main.transform.position.z) Destroy(gameObject);
        if (InsideRange && !hasFindPlayer)
        {
            Percive();
        }
        else if (!hasEnterScreen)
        {
            EnterScreen();
        }
        else
        {
            Fire();
            rb.MovePosition(transform.position - new Vector3(0, 0, speed * Time.deltaTime));
        }

    }

}
