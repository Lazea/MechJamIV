using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneUnit : BaseNPC
{
    [HeaderAttribute("Aim Base")]
    public Transform aimBase;
    public float minLookAngle;
    public float maxLookAngle;

    int maxShotCount = 6;
    int shotCount;
    float nextAttackTime;
    float nextAttackEventTime;
    float nextMoveTime;

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if(hasTarget)
        {
            if(targetInReach)
            {
                float t = Time.time;
                if(IsAtDestination())
                {
                    Stop();
                    agent.SetDestination(GetStrafePosition());
                    nextMoveTime = Time.time + UnityEngine.Random.Range(2f, 8f);
                }
                else if(t > nextMoveTime)
                {
                    agent.isStopped = false;
                }
            }
            else
            {
                GoToTarget();
            }

            HandleAim(aimBase, aim, minLookAngle, maxLookAngle);
            
            if(targetInCombatRange && targetInAim)
            {
                float t = Time.time;
                if(t >= nextAttackEventTime)
                {
                    if(t >= nextAttackTime)
                    {
                        SpawnProjectile();

                        shotCount++;
                        if(shotCount >= maxShotCount)
                        {
                            shotCount = 0;
                            
                            float dt = UnityEngine.Random.Range(data.minAttackEventRate, data.maxAttackEventRate);
                            nextAttackEventTime = Time.time + dt;
                            nextAttackTime = nextAttackEventTime;
                        }
                        else
                        {
                            nextAttackTime = Time.time + data.attackInterval;
                        }
                    }
                }
            }
        }
        else
        {
            agent.isStopped = true;

            shotCount = 0;

            float dt = UnityEngine.Random.Range(data.minAttackEventRate, data.maxAttackEventRate);
            nextAttackEventTime = Time.time + dt;
            nextAttackTime = nextAttackEventTime;
        }
    }
}
