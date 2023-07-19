using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechFX : MonoBehaviour
{
    public ParticleSystem[] backThrusters;
    public ParticleSystem[] footGrindFX;

    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void LateUpdate()
    {
        if(anim.GetBool("Dead") || anim.GetBool("Stunned"))
        {
            foreach (var fx in backThrusters)
            {
                if (fx.isPlaying)
                    fx.Stop();
            }
            foreach (var fx in footGrindFX)
            {
                if (fx.isPlaying)
                    fx.Stop();
            }
        }
        else if(anim.GetFloat("Speed") >= 0.1f)
        {
            foreach (var fx in backThrusters)
            {
                if (!fx.isPlaying)
                    fx.Play();
            }

            if(anim.GetBool("IsGrounded"))
            {
                foreach (var fx in footGrindFX)
                {
                    if (!fx.isPlaying)
                        fx.Play();
                }
            }
        }
        else
        {
            foreach (var fx in backThrusters)
            {
                if (fx.isPlaying)
                    fx.Stop();
            }
            foreach (var fx in footGrindFX)
            {
                if (fx.isPlaying)
                    fx.Stop();
            }
        }
    }
}
