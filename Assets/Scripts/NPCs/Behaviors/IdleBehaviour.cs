using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleBehaviour : StateMachineBehaviour
{
    public string boolParameter;
    public bool boolParameterValue;

    public int animLayer;
    public string[] checkAnimStates;

    IActor actor;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(
        Animator animator,
        AnimatorStateInfo stateInfo,
        int layerIndex)
    {
        actor = animator.GetComponent<IActor>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(
        Animator animator,
        AnimatorStateInfo stateInfo,
        int layerIndex)
    {
        if(actor.HasTarget && actor.TargetInReach)
        {
            animator.SetBool(boolParameter, boolParameterValue);
            //if (checkAnimStates == null || checkAnimStates.Length == 0)
            //{
            //    animator.SetBool(boolParameter, boolParameterValue);
            //}
            //else
            //{
            //    var state = animator.GetCurrentAnimatorStateInfo(animLayer);
            //    foreach (string name in checkAnimStates)
            //    {
            //        if (state.IsName(name))
            //        {
            //            animator.SetBool(boolParameter, boolParameterValue);
            //            break;
            //        }
            //    }
            //}
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
