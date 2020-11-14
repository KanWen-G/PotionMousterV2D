using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;
    public Transform focus;
    public float smoothTime = 2;

    [NonSerialized] public Vector3 offset;

    private void Awake()
    {
        Instance = this;
        offset = focus.position - transform.position;
    }

    private void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, focus.position - offset, Time.deltaTime * smoothTime);
        //To whomever wrote this:
        //Please kindly refer to https://chicounity3d.wordpress.com/2014/05/23/how-to-lerp-like-a-pro/
        //:)
    }
}
