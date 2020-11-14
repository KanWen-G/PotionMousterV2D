using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    walk,
    golf
}

public class PlayerMovement : MonoBehaviour
{
    #region Fields
    //References
    public Rigidbody2D RB2D;
    public Animator Animator;
    public BallPseudo3DPhysics Ball;
    public GameObject TargetIndicator;

    //Values
    public float Speed = 1f;
    public float BallInteractionDistanceThreshold = 3f;
    public KeyCode InteractionKey = KeyCode.E;
    public KeyCode CancelKey = KeyCode.Escape;

    //NonSerialized
    [NonSerialized] public PlayerState CurrentState;
    private Vector2 Movement;
    private float GolfAngle = 90.0f;
    private float GolfPower = 5.0f;
    #endregion

    #region Awake Method
    void Awake()
    {
        CurrentState = PlayerState.walk;

        if(RB2D == null)
            RB2D = GetComponent<Rigidbody2D>();

        if(Animator == null)
            Animator = GetComponent<Animator>();
    }
    #endregion

    #region Update Method
    void Update()
    {
        float distanceToBall = Vector3.Distance(transform.position, Ball.transform.position);

        if (CurrentState == PlayerState.walk)
        {
            Ball.ArcRenderer.enabled = false;
            Ball.LineRenderer.enabled = false;
            if (CameraController.Instance.focus != Ball.transform || Ball.Velocity.sqrMagnitude < 0.01f)
                CameraController.Instance.focus = transform;

            Movement.x = -Input.GetAxisRaw("Horizontal");
            Movement.y = Input.GetAxisRaw("Vertical");

            Animator.SetFloat("Horizontal", Movement.x);
            Animator.SetFloat("Vertical", Movement.y);
            Animator.SetFloat("Speed", Movement.sqrMagnitude);

            bool canInteractWithBall = distanceToBall < BallInteractionDistanceThreshold && Input.GetKeyDown(InteractionKey);

            if (canInteractWithBall)
            {
                CurrentState = PlayerState.golf;
                Vector3 pos = Ball.transform.position;
                pos.x -= 0.5f;
                pos.y += 0.5f;
                pos.z = 0.0f;
                transform.position = pos;
            }
        }
        else if(CurrentState == PlayerState.golf)
        {
            bool canCancelInteractionWithBall = Input.GetKeyDown(CancelKey);

            if (canCancelInteractionWithBall)
            {
                CurrentState = PlayerState.walk;
            }
            else
            {
                Ball.ArcRenderer.enabled = true;
                Ball.LineRenderer.enabled = true;
                CameraController.Instance.focus = TargetIndicator.transform;

                //Input Receiving
                if (Input.GetKey(KeyCode.LeftArrow))
                    GolfAngle += 90.0f * Time.deltaTime;
                else if (Input.GetKey(KeyCode.RightArrow))
                    GolfAngle -= 90.0f * Time.deltaTime;

                if (GolfAngle > 180.0f)
                    GolfAngle = -180.0f - (180 - GolfAngle);
                else if (GolfAngle < -180.0f)
                    GolfAngle = 180.0f - (-180.0f - GolfAngle);

                if (Input.GetKey(KeyCode.UpArrow))
                    GolfPower += 5.0f * Time.deltaTime;
                else if (Input.GetKey(KeyCode.DownArrow))
                    GolfPower -= 5.0f * Time.deltaTime;

                if (GolfPower < 0.0f)
                    GolfPower = 0.0f;

                bool wantsToShoot = Input.GetKeyDown(KeyCode.Space);

                //Input Processing
                Vector3 dir = Vector3.one;
                dir.x = Mathf.Cos(GolfAngle * Mathf.Deg2Rad);
                dir.y = Mathf.Sin(GolfAngle * Mathf.Deg2Rad);
                Vector3 velocity = dir * GolfPower;

                if (wantsToShoot)
                {
                    Ball.Velocity = velocity;
                    CurrentState = PlayerState.walk;
                    CameraController.Instance.focus = Ball.transform;
                }
                else
                {
                    //Trajectory Indicator
                    float flightTime = Ball.CalculateFlightTime(velocity, 0.001f);
                    float density = 10 * flightTime;
                    Ball.ArcMaterial.SetFloat("_Density", density);
                    Ball.LineMaterial.SetFloat("_Density", density);
                    Ball.Visualize(velocity, flightTime);

                    TargetIndicator.transform.position = Ball.LineRenderer.GetPosition(1);
                }
            }
        }
    }
    #endregion

    #region FixedUpdate Method
    void FixedUpdate()
    {
        if (CurrentState == PlayerState.walk)
        {
            RB2D.position += Movement * Speed * Time.fixedDeltaTime;
        }
        else if(CurrentState == PlayerState.golf)
        {

        }
    }
    #endregion

}