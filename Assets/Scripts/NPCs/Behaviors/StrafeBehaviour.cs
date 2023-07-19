using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrafeBehaviour : StateMachineBehaviour
{
    public string boolParameter;
    public bool boolParameterValue;

    public int minStrafeCount;
    public int maxStrafeCount;
    int strafeCount;
    int _strafeCount;
    public float minStrafeRange;
    public float maxStrafeRange;

    public string strafeEndBoolParameter;
    public bool strafeEndBoolParameterValue;

    IActor actor;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(
        Animator animator,
        AnimatorStateInfo stateInfo,
        int layerIndex)
    {
        actor = animator.GetComponent<IActor>();
        actor.SetStrafePosition(minStrafeRange, maxStrafeRange);

        strafeCount = minStrafeCount;
        if (minStrafeCount != maxStrafeCount)
            strafeCount = Random.Range(minStrafeCount, maxStrafeCount + 1);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(
        Animator animator,
        AnimatorStateInfo stateInfo,
        int layerIndex)
    {
        if(!actor.HasTarget || !actor.TargetInCombatRange || !actor.TargetInReach)
        {
            animator.SetBool(boolParameter, boolParameterValue);
        }

        if(actor.IsAtDestination())
        {
            _strafeCount++;
            if (_strafeCount >= strafeCount)
                animator.SetBool(strafeEndBoolParameter, strafeEndBoolParameterValue);
            actor.SetStrafePosition(minStrafeRange, maxStrafeRange);
        }
        actor.GoToDestination();
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
