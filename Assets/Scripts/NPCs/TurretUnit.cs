using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretUnit : BaseNPC
{
    [HeaderAttribute("Aim Base")]
    public Transform aimBase;
    public float minLookAngle;
    public float maxLookAngle;

    int maxShotCount = 6;
    int shotCount;
    float nextAttackTime;
    float nextAttackEventTime;

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if(hasTarget)
        {
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
            shotCount = 0;

            float dt = UnityEngine.Random.Range(data.minAttackEventRate, data.maxAttackEventRate);
            nextAttackEventTime = Time.time + dt;
            nextAttackTime = nextAttackEventTime;
        }
    }
}
