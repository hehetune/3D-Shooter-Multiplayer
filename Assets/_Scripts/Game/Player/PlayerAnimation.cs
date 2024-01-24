using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public Animator animator;

    public float HorizontalSpeed
    {
        get => animator.GetFloat("HorizontalSpeed");
        set
        {
            animator.SetFloat("HorizontalSpeed", value);
        }
    }
    public float VerticalSpeed
    {
        get => animator.GetFloat("VerticalSpeed");
        set
        {
            animator.SetFloat("VerticalSpeed", value);
        }
    }
    public bool IsGrounded
    {
        get => animator.GetBool("IsGrounded");
        set
        {
            animator.SetBool("IsGrounded", value);
        }
    }
    public float ForwardSpeed
    {
        get => animator.GetFloat("ForwardSpeed");
        set
        {
            animator.SetFloat("ForwardSpeed", value);
        }
    }
    public float StrafeSpeed
    {
        get => animator.GetFloat("StrafeSpeed");
        set
        {
            animator.SetFloat("StrafeSpeed", value);
        }
    }
}