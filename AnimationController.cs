using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    int movex = Animator.StringToHash("X_move");
    int movey = Animator.StringToHash("Y_move");
    int isJumping = Animator.StringToHash("isJumping");
    int isGrounded = Animator.StringToHash("isGrounded");

    Animator anim;
    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }

    public void JumpingAnimation(bool value)
    {
        if (anim.GetBool(isJumping) != value)
            anim.SetBool(isJumping, value);
    }

    public void isGruondedFunc(bool value)
    {
        if (anim.GetBool(isGrounded) != value)
            anim.SetBool(isGrounded, value);
    }

    public void playerMovementAnimation(Vector2 value)
    {
        anim.SetFloat(movex, value.x);
        anim.SetFloat(movey, value.y);
    }
}
