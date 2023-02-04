using HyperCasual.Runner;
using System.Collections;
using UnityEngine;
public class AirAttacker : Enemy
{
    [Header("进入画面花费的时间")]
    /// <summary>
    /// 进入画面花费的时间
    /// </summary>
    public float enterTime;

    [Header("在空中盘旋的时间")]
    /// <summary>
    /// 在空中盘旋的时间
    /// </summary>
    public float hoverTime = 3f;

    [Header("俯冲速度")]
    /// <summary>
    /// 俯冲速度
    /// </summary>
    public float speed = 3f;

    [Header("和玩家的水平距离(z)")]
    /// <summary>
    /// 和镜头的水平距离
    /// </summary>
    public float horizontalDistance;

    [Header("初始高度(y)")]
    /// <summary>
    /// 初始高度
    /// </summary>
    public float initialHeight;

    [Header("盘旋的水平初始位置(x)")]
    /// <summary>
    /// 盘旋的水平初始位置
    /// </summary>
    public float initialX;

    /// <summary>
    /// 俯冲的水平距离x
    /// </summary>
    [Header("俯冲的水平距离(x)")]
    public float diveDistanceX;
    public enum LeftOrRight { left, right }
    [Header("出生在左边或者右边")]
    public LeftOrRight leftOrRight;

    [Header("z轴感知范围")]
    /// <summary>
    /// 感知到主角并且开始追踪的范围
    /// </summary>
    public float perceptionRange = 5f;
    PlayerController player => PlayerController.Instance;
    Vector3 PlayerPos => player.Transform.position;
    Vector3 Pos => transform.position;

    bool hasFindPlayer;
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
        StartCoroutine(PrepareToAttack());
    }
    //盘旋
    IEnumerator PrepareToAttack()
    {
        Vector3 targetPosition = Pos;
        targetPosition.y = initialHeight;
        targetPosition.x = 0;

        float cameraBoundX = GetCameraXBound(PlayerPos.z + horizontalDistance - Camera.main.transform.position.z);
        if (leftOrRight == LeftOrRight.right)//在右边
        {
            transform.forward = Vector3.left;//在右边则朝向左
            //cameraBoundX > 0
            targetPosition.x = initialX;
        }
        else//在左边
        {
            transform.forward = Vector3.right;//在左边则朝向右
            cameraBoundX = -cameraBoundX;
            targetPosition.x = -initialX;
        }
        Debug.Log("空袭：进入画面");
        float startTime = Time.time;
        float u = 0;
        //进入画面
        while (u <= 1)
        {
            yield return new WaitForEndOfFrame();
            u = (Time.time - startTime) / enterTime;
            targetPosition.z = PlayerPos.z + horizontalDistance;
            transform.position = new Vector3(u * targetPosition.x + (1 - u) * cameraBoundX, targetPosition.y, targetPosition.z);
        }

        startTime = Time.time;
        Debug.Log("空袭：盘旋");
        //盘旋
        while (Time.time - startTime < hoverTime)
        {
            yield return new WaitForEndOfFrame();
            targetPosition.z = PlayerPos.z + horizontalDistance;
            transform.position = targetPosition;
        }

        StartCoroutine(Attack());
        yield return null;
    }
    //攻击
    IEnumerator Attack()
    {

        Vector3 direction;
        float x = -diveDistanceX + speed * Time.deltaTime;//防止除0
        float zeroX = Pos.x + diveDistanceX;
        if (leftOrRight == LeftOrRight.left)
            zeroX = Pos.x + diveDistanceX;
        else
            zeroX = Pos.x - diveDistanceX;
        float cameraBoundX = GetCameraXBound(PlayerPos.z + horizontalDistance - Camera.main.transform.position.z);
        Debug.Log("空袭：俯冲");
        //俯冲
        if (leftOrRight == LeftOrRight.left)
            while (x <= 0)
            {
                yield return new WaitForEndOfFrame();
                direction = new Vector3(diveDistanceX * Mathf.Sqrt(diveDistanceX * diveDistanceX - x * x), initialHeight * x, 0).normalized;
                transform.position = (Pos + direction * speed * Time.deltaTime);
                transform.forward = Vector3.right;
                x = (Pos.x - zeroX);
            }
        else
            while (x <= 0)
            {
                yield return new WaitForEndOfFrame();
                direction = new Vector3(diveDistanceX * Mathf.Sqrt(diveDistanceX * diveDistanceX - x * x), initialHeight * x, 0).normalized;
                direction.x = -direction.x;
                transform.position = (Pos + direction * speed * Time.deltaTime);
                transform.forward = Vector3.left;
                x = zeroX - Pos.x;
            }
        Debug.Log("空袭：横冲");
        //横冲
        if (leftOrRight == LeftOrRight.left)
            while (Pos.x <= cameraBoundX)
            {
                yield return new WaitForEndOfFrame();
                transform.position = (Pos + Vector3.right * speed * Time.deltaTime);
            }
        else
            while (Pos.x >= -cameraBoundX)
            {
                yield return new WaitForEndOfFrame();
                transform.position = (Pos + Vector3.left * speed * Time.deltaTime);
            }
        Destroy(gameObject);
        yield return null;
    }


    protected override void Awake()
    {
        base.Awake();
    }
    protected override void Update()
    {
        base.Update();
        if (!Application.isPlaying)//防止在编辑界面也会动
            return;
        if (InsideRange && !hasFindPlayer)
        {
            Percive();
        }
    }


}
