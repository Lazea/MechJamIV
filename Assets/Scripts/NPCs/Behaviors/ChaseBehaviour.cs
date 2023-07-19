using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseBehaviour : StateMachineBehaviour
{
    public string boolParameter;
    public bool boolParameterValue;

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
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(
        Animator animator,
        AnimatorStateInfo stateInfo,
        int layerIndex)
    {
        if(actor.HasTarget)
        {
            if(actor.TargetInCombatRange)
            {
                animator.SetBool(boolParameter, boolParameterValue);
            }
            else
            {
                actor.SetPlayerAsDestination(new Vector3(offset.x, 0f, offset.y));
                actor.GoToDestination();
            }
        }
        else
        {
            animator.SetBool(boolParameter, !boolParameterValue);
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
