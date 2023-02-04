using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HyperCasual.Runner;
public class Dasher : Enemy
{
    [Header("冲刺速度")]
    public float dashSpeed;

    [Header("冲刺的距离")]
    public float dashDistanceZ;

    [Header("感知到玩家的距离（转头）")]
    public float perceptionRange;

    [Header("冲刺曲线系数，越大越高（二次函数）")]
    public float JumpK;


    PlayerController Player => PlayerController.Instance;
    Vector3 ToPlayer => Player.transform.position - transform.position;
    float startTime;
    Vector3 target;
    float zeroX;



    bool ReadyToDash()
    {
        return transform.position.z - Player.transform.position.z < perceptionRange && transform.position.z - Player.transform.position.z > dashDistanceZ;
    }

    void Dash()
    {
        float dis = target.x - zeroX;
        float x = transform.position.x - zeroX;
        x += dis / Mathf.Abs(dis) * dashSpeed * Time.deltaTime;
        float y = -JumpK * (x * x - dis * x);
        Vector3 p = transform.position;
        p.x = zeroX + x;
        p.y = y;
        transform.position = p;
    }
    protected override void Update()
    {
        base.Update();
        if (!Application.isPlaying)//防止在编辑界面也会动
            return;


        if (ReadyToDash())
        {
            Debug.Log("2st");
            transform.forward = ToPlayer;
            startTime = Time.time;
            target = Player.transform.position;
            target.z = transform.position.z;
            zeroX = transform.position.x;
        }
        else if (transform.position.z - Player.transform.position.z < dashDistanceZ)
        {
            Debug.Log("3st");
            if (transform.position.z < Camera.main.transform.position.z) Destroy(gameObject);
            Dash();
        }

    }
}
