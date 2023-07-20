using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundFXController : MonoBehaviour
{
    public AudioSource oneShotSource;
    public AudioSource thrusterSource;

    public AudioClip dashSoundClip;
    public AudioClip jumpSoundClip;
    public AudioClip landingSoundClip;

    bool dashed;
    bool landed;

    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponentInParent<Animator>();
        thrusterSource.volume = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        var baseState = anim.GetCurrentAnimatorStateInfo(0);
        if (baseState.IsName("Dash Blend Tree"))
        {
            if (!dashed)
            {
                oneShotSource.pitch = Random.Range(0.95f, 1.06f);
                oneShotSource.PlayOneShot(dashSoundClip, 1.0f);
                dashed = true;
            }
        }
        else
        {
            dashed = false;
        }

        var jumpState = anim.GetCurrentAnimatorStateInfo(2);
        if (jumpState.IsName("Landing"))
        {
            if (!landed)
            {
                oneShotSource.pitch = Random.Range(0.95f, 1.06f);
                oneShotSource.PlayOneShot(landingSoundClip, 0.45f);
                landed = true;
            }
        }
        else
        {
            landed = false;
        }

        Vector2 moveSpeed = new Vector2(
            anim.GetFloat("SpeedX"),
            anim.GetFloat("SpeedY"));
        if(anim.GetBool("VertThrust") || moveSpeed.magnitude > 0.1f)
        {
            thrusterSource.volume = Mathf.Lerp(thrusterSource.volume, 0.25f, 1.5f * Time.deltaTime);
            thrusterSource.pitch = Mathf.Lerp(thrusterSource.pitch, 1.6f, 1.5f * Time.deltaTime);
        }
        else
        {
            thrusterSource.volume = Mathf.Lerp(thrusterSource.volume, 0.0f, 3f * Time.deltaTime);
            thrusterSource.pitch = Mathf.Lerp(thrusterSource.pitch, 1.0f, 3f * Time.deltaTime);
        }
    }
}
