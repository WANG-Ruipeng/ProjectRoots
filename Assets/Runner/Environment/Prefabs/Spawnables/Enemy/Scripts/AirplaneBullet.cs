using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HyperCasual.Runner;
public class AirplaneBullet : MonoBehaviour
{
    const string k_PlayerTag = "Player";
    public float speed;
    private void Awake()
    {
        transform.up = transform.parent.transform.up;//与炮筒同步
    }
    private void Update()
    {
        transform.Translate(transform.forward * speed * Time.deltaTime);
        if (transform.position.y < -2) Destroy(gameObject);
    }
    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag(k_PlayerTag))
        {
            GameManager.Instance.Lose();
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
