using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HyperCasual.Runner;
using UnityEngine.Animations;
using UnityEditor;
public class Enemy : Spawnable
{
    const string k_PlayerTag = "Player";
    const string k_ProjectileTag = "Projectile";

    Animator[] animatorsChildren;
    bool died = false;
    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag(k_PlayerTag))
        {
            GameManager.Instance.Lose();
        }
        else if (col.CompareTag(k_ProjectileTag) && died == false)
        {
            died = true;
            Slain();
        }
    }

    void Slain()
    {
        animatorsChildren = GetComponentsInChildren<Animator>();
        for (int i = 0; i < animatorsChildren.Length; i++)
        {
            animatorsChildren[i].SetTrigger("Die");
        }
        StartCoroutine(RemoveItself());


    }
    IEnumerator RemoveItself()
    {
        while (true)
        {
            AnimatorStateInfo info = animatorsChildren[1].GetCurrentAnimatorStateInfo(0);
            if (info.normalizedTime >= 1 && info.IsName("Death"))
            {
                break;
            }
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(0.1f);
        Debug.Log("摧毁物体:" + gameObject.name);
        Destroy(gameObject);
        yield return null;
    }


    protected override void Update()
    {
        base.Update();
    }


    /// <summary>
    /// 返回距离相机为distance时的x边界值（绝对值）
    /// </summary>
    /// <param name="distance"></param>
    /// <returns></returns>
    protected float GetCameraXBound(float distance)
    {
        Camera camera = Camera.main;
        float halfFOV = camera.fieldOfView * 0.5f * Mathf.Deg2Rad;
        float aspect = camera.aspect;

        return distance * Mathf.Tan(halfFOV) * aspect;
    }
    protected float GetCameraYBound(float distance)
    {
        Camera camera = Camera.main;
        float halfFOV = camera.fieldOfView * 0.5f * Mathf.Deg2Rad;

        return distance * Mathf.Tan(halfFOV);
    }
}
