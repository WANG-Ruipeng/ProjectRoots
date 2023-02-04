using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingPropellor : MonoBehaviour
{
    public float rotateSpeed;
    void Update()
    {
        transform.RotateAroundLocal(new Vector3(0, Mathf.Tan(Mathf.PI / 6), 1), rotateSpeed * Time.deltaTime);
    }
}
