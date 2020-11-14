using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallPseudo3DPhysics : MonoBehaviour
{
    public GameObject BallSprite;
    public LineRenderer ArcRenderer;
    public LineRenderer LineRenderer;
    public int LineSegments = 10;

    public Vector3 Gravity = new Vector3(0, 0, -9.81f);
    public float Drag = 0.95f;
    public float Bounciness = 0.69f;
    public float Friction = 0.83f;
    [NonSerialized] public Vector3 Velocity;

    [NonSerialized] public Material ArcMaterial;
    [NonSerialized] public Material LineMaterial;

    private void Awake()
    {
        ArcRenderer.positionCount = LineSegments;
        LineRenderer.positionCount = 2;

        ArcMaterial = ArcRenderer.material;
        LineMaterial = LineRenderer.material;

        ArcRenderer.sharedMaterial = ArcMaterial;
        LineRenderer.sharedMaterial = LineMaterial;
    }

    private void Update()
    {
        BallSprite.transform.localScale = new Vector3(transform.position.z + 1.0f, transform.position.z + 1.0f, 1.0f);
    }

    private void FixedUpdate()
    {
        //This is very experimental
        if (transform.position.z > 0.0f)
        {
            Velocity.z += Gravity.z * Time.fixedDeltaTime;
        }
        else
        {
            Velocity.x *= Friction;
            Velocity.y *= Friction;
            Velocity.z = Math.Abs((Velocity.z + transform.position.z) * Bounciness);
        }

        transform.position += Velocity * Time.fixedDeltaTime;
    }

    private Vector3 CalculatePosInTime(Vector3 vo, float time)
    {
        //This is the real deal that calculates for trajectory indicator
        Vector3 result = transform.position + vo * time;
        result.z = (-0.5f * Mathf.Abs(Gravity.z) * (time * time)) + (vo.z * time) + transform.position.z;

        return result;
    }

    public float CalculateFlightTime(Vector3 vo, float timeSteps)
    {
        Vector3 pos = Vector3.zero;
        float t = 0.0f;

        while (true)
        {
            t += timeSteps;
            pos = CalculatePosInTime(vo, t);
            if (pos.z <= 0.0f)
                break;
        }

        return t;
    }

    public void Visualize(Vector3 vo, float flightTime)
    {
        for (int i = 0; i < LineSegments; i++)
        {
            Vector3 pos = CalculatePosInTime(vo, (i / (float)LineSegments * flightTime));
            
            if (i == 0)
                LineRenderer.SetPosition(0, pos);
            else if(i == LineSegments - 1)
                LineRenderer.SetPosition(1, pos);

            //Offset x by height of the ball.
            pos.x += pos.z / 4;

            ArcRenderer.SetPosition(i, pos);
        }
    }
}
