using System.Collections.Generic;
using UnityEngine;

public enum WeaponAnimState
{
    Idle = 0,
    Equip = 1,
    Attack = 2,
    Reload = 3,
    Recovery = 4,
}

public class WeaponAnimation : MonoBehaviour
{
    public Animator animator;
    public Weapon weapon;

    private Dictionary<string, AnimationClip> clips = new();

    private void Awake()
    {
        AnimationClip[] clipsArr = animator.runtimeAnimatorController.animationClips; // Get the AnimationClip array
        foreach (AnimationClip clip in clipsArr) // Loop through the array
        {
            clips[clip.name] = clip;
        }
    }

    public void PlayAnim(WeaponAnimState state, float time = -1)
    {
        PlayAnim(state.ToString(), time);
    }
    public void PlayAnim(string anim, float time = -1)
    {
        if (time != -1)
        {
            float speed = clips[anim].length / time;
            animator.speed = speed;
        }
        else animator.speed = 1;
        if (animator) animator.Play(anim, 0, 0);
    }
}