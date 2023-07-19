using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBehaviour : StateMachineBehaviour
{
    public int minShotCount = 1;
    public int maxShotCount = 1;
    int shotCount;
    int count;
    float t;
    float nextShotTime;
    public float shotIntervalTime = 1f;

    public string exitStateParam = "Firing";
    public bool exitStateParamValue;

    IActor actor;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(
        Animator animator,
        AnimatorStateInfo stateInfo,
        int layerIndex)
    {
        count = 0;
        t = Time.time;
        nextShotTime = Time.time + shotIntervalTime;

        if(minShotCount == maxShotCount)
            shotCount = minShotCount;
        else
            shotCount = Random.Range(minShotCount, maxShotCount);

        actor = animator.GetComponent<IActor>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(
        Animator animator,
        AnimatorStateInfo stateInfo,
        int layerIndex)
    {
        t = Time.time;
        if(t >= nextShotTime)
        {
            actor.SpawnProjectiles();
            nextShotTime = Time.time + shotIntervalTime;
            count++;
        }

        if (count >= shotCount)
            animator.SetBool(exitStateParam, exitStateParamValue);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{

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
