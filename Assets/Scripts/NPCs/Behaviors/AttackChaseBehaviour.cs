using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackChaseBehaviour : StateMachineBehaviour
{
    [System.Serializable]
    public struct ExitBoolParameter
    {
        public string exitBoolParameter;
        public bool exitBoolParameterValue;
    }
    public ExitBoolParameter[] exitBoolParameters;

    public float lifeTime;
    float exitTime;
    public float attackRange;

    public string attackTriggerParameter;

    public float offsetFromTarget;
    Vector3 offset;

    IActor actor;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(
        Animator animator,
        AnimatorStateInfo stateInfo,
        int layerIndex)
    {
        actor = animator.GetComponent<IActor>();
        offset = Random.insideUnitCircle * offsetFromTarget;

        exitTime = Time.time + lifeTime;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(
        Animator animator,
        AnimatorStateInfo stateInfo,
        int layerIndex)
    {
        if(actor.HasTarget)
        {
            float t = Time.time;
            if(t >= exitTime)
            {
                ExitState(animator);
            }
            else if(actor.TargetDistance <= attackRange)
            {
                animator.SetTrigger(attackTriggerParameter);
            }
            else
            {
                actor.SetPlayerAsDestination(new Vector3(offset.x, 0f, offset.y));
                actor.GoToDestination();
            }
        }
        else
        {
            ExitState(animator);
        }
    }

    void ExitState(Animator anim)
    {
        foreach (var p in exitBoolParameters)
        {
            anim.SetBool(p.exitBoolParameter, p.exitBoolParameterValue);
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
