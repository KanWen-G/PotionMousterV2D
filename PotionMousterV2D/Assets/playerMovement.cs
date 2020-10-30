using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovement : MonoBehaviour
{
    
    public float Speed = 1f;
    public Rigidbody2D RB2D;
    public Animator animator;
    Vector2 movement;

    // Update is called once per frame
    void Update()
    {
        #region PlayerInput
            movement.x = -Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");
        #endregion

        #region animation
        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Speed",movement.sqrMagnitude);
        #endregion

    }
    
    void FixedUpdate(){
        #region PlayerMovement
            RB2D.MovePosition(RB2D.position + movement * Speed * Time.fixedDeltaTime);
            Debug.Log(movement);
        #endregion

    }

}
