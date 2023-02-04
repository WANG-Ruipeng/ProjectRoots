using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HyperCasual.Runner;
public class Enemy : Spawnable
{
    const string k_PlayerTag = "Player";
    const string k_ProjectileTag = "Projectile";

    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag(k_PlayerTag))
        {
            GameManager.Instance.Lose();
        }
        else if (col.CompareTag(k_ProjectileTag))
        {
            Slain();
        }
    }

    void Slain()
    {
        Destroy(gameObject);
    }
    protected override void Update()
    {
        base.Update();
    }
}
