using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFXController : MonoBehaviour
{
    [Header("Effects")]
    public ParticleSystem landingImpactFX;
    public ParticleSystem[] dashLeftFX;
    public ParticleSystem[] dashRightFX;
    public ParticleSystem[] dashForwardFX;
    public ParticleSystem[] dashBackwardFX;
    public ParticleSystem[] verticalThrustFX;
    public ParticleSystem[] MoveFX;
    public ParticleSystem[] SkateFX;

    // Components
    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();

        landingImpactFX.Stop();
        foreach(var fx in dashLeftFX)
            fx.Stop();
        foreach(var fx in dashRightFX)
            fx.Stop();
        foreach(var fx in dashForwardFX)
            fx.Stop();
        foreach(var fx in dashBackwardFX)
            fx.Stop();
        foreach(var fx in verticalThrustFX)
            fx.Stop();
        foreach(var fx in MoveFX)
            fx.Stop();
        foreach(var fx in SkateFX)
            fx.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        foreach(var fx in SkateFX)
        {
            if(anim.GetFloat("Speed") >= 0.2f && anim.GetBool("IsGrounded"))
            {
                if(!fx.isPlaying)
                    fx.Play();
            }
            else
            {
                if(fx.isPlaying)
                    fx.Stop();
            }
        }

        foreach(var fx in verticalThrustFX)
        {
            if(anim.GetBool("VertThrust"))
            {
                if(!fx.isPlaying)
                    fx.Play();
            }
            else
            {
                if(fx.isPlaying)
                    fx.Stop();
            }
        }

        var animState = anim.GetCurrentAnimatorStateInfo(1);
        bool isDashing = (animState.IsTag("Dash") && animState.normalizedTime <= 0.6f);
        foreach(var fx in MoveFX)
        {
            if(anim.GetFloat("Speed") >= 0.3f && !isDashing)
            {
                if(!fx.isPlaying)
                    fx.Play();
            }
            else
            {
                if(fx.isPlaying)
                    fx.Stop();
            }
        }
    }

    public void PlayDashForwardFX()
    {
        foreach(var fx in dashForwardFX)
        {
            if(!fx.isPlaying)
                    fx.Play();
        }
    }

    public void PlayDashBackwardFX()
    {
        foreach(var fx in dashBackwardFX)
        {
            if(!fx.isPlaying)
                    fx.Play();
        }
    }

    public void PlayDashLeftFX()
    {
        foreach(var fx in dashLeftFX)
        {
            if(!fx.isPlaying)
                    fx.Play();
        }
    }

    public void PlayDashRightFX()
    {
        foreach(var fx in dashRightFX)
        {
            if(!fx.isPlaying)
                    fx.Play();
        }
    }

    public void PlayLandingFX() 
    {
        landingImpactFX.Play();
    }
}