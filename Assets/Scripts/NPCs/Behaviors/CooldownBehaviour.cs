using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CooldownBehaviour : StateMachineBehaviour
{
    public string boolParameter;
    public bool boolParameterValue;
    public float minCooldownTime;
    public float maxCooldownTime;
    float cooldownTime = 1f;
    protected float t;
    protected float cooldownEndTime;
    protected bool cooldownStarted;

    public int animLayer;
    public string[] checkAnimStates;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(
        Animator animator,
        AnimatorStateInfo stateInfo,
        int layerIndex)
    {
        cooldownStarted = false;

        if (minCooldownTime == maxCooldownTime)
            cooldownTime = minCooldownTime;
        else
            cooldownTime = Random.Range(minCooldownTime, maxCooldownTime);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(
        Animator animator,
        AnimatorStateInfo stateInfo,
        int layerIndex)
    {
        if (checkAnimStates == null || checkAnimStates.Length == 0)
        {
            HandleCooldown(animator, boolParameter, boolParameterValue);
        }
        else
        {
            var state = animator.GetCurrentAnimatorStateInfo(animLayer);
            foreach (string name in checkAnimStates)
            {
                if (state.IsName(name))
                {
                    HandleCooldown(animator, boolParameter, boolParameterValue);
                    break;
                }
            }
        }
    }

    protected virtual void HandleCooldown(Animator animator, string param, bool value)
    {
        if (!cooldownStarted)
        {
            cooldownEndTime = Time.time + cooldownTime;
            cooldownStarted = true;
        }
        else
        {
            t = Time.time;
            if (t >= cooldownEndTime)
                animator.SetBool(param, value);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
