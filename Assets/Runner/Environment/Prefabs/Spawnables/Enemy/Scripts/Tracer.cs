using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HyperCasual.Runner;

[ExecuteInEditMode]
[RequireComponent(typeof(Collider))]
public class Tracer : Enemy
{
    /// <summary>
    /// time that tail after
    /// </summary>
    [Header("持续时间")]
    public float duration = 5f;
    [Header("移动速度")]
    public float speed = 1f;
    /// <summary>
    /// 感知到主角并且开始追踪的范围
    /// </summary>
    [Header("追踪半径")]
    public float perceptionRange = 5f;
    /// <summary>
    /// 水平视野范围
    /// </summary>
    [Header("水平视野范围")]
    public float horizontalEyeFov = 60f;
    [Header("转向玩家时使用的帧数（越大越慢）")]
    public float firstRotFrame = 30f;




    Rigidbody rb;
    float startTime;
    bool isTracing;
    protected bool hasFindPlayer;
    protected PlayerController Player => PlayerController.Instance;
    protected Vector3 ToPlayer => Player.transform.position - transform.position;



    /// <summary>
    /// 花费一定时间旋转向玩家
    /// </summary>
    /// <param name="direction">最终方向</param>
    /// <returns></returns>
    IEnumerator TargetToPlayer(Vector3 direction)
    {
        float i = 1;
        Vector3 up = Vector3.up;
        //根据玩家刚进入范围内的角度来确认转速
        float theta = Mathf.Acos(Vector3.Dot(direction, transform.forward)) / Mathf.PI * 180 / firstRotFrame;
        while (i <= firstRotFrame)
        {
            if (Vector3.Dot((ToPlayer).normalized, transform.forward) > 0.99) break;//如果已经指向玩家了
            transform.Rotate(up, theta);
            i++;
            yield return new WaitForEndOfFrame();
        }
        StartTrace();
        yield return null;
    }
    protected bool InsideRange
    {
        get
        {
            if (!(ToPlayer.magnitude <= perceptionRange))
                return false;
            //检查视野
            if (Vector3.Dot(ToPlayer.normalized, transform.forward) < Mathf.Cos(horizontalEyeFov / 2))
                return false;
            return true;
        }
    }
    void StartTrace()
    {
        startTime = Time.time;
        isTracing = true;
    }

    protected void Percive()
    {
        StartCoroutine(TargetToPlayer(ToPlayer.normalized));
        hasFindPlayer = true;
    }

    void Trace()
    {
        if (Time.time - startTime > duration) Destroy(gameObject);

        Transform pTrans = PlayerController.Instance.Transform;
        Vector3 direction = ToPlayer.normalized;
        rb.MovePosition(transform.position + direction * Time.deltaTime * speed);
        //Debug.Log("tracing" + transform.position);
        transform.forward = direction;
    }


    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody>();
        isTracing = false;
        hasFindPlayer = false;
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
        else if (isTracing)
        {
            Trace();
        }
    }


}
