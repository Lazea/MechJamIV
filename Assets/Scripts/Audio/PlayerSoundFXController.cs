using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundFXController : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource oneShotSource;
    public AudioSource thrusterSource;

    [Header("Movement Clips")]
    public AudioClip dashSoundClip;
    public AudioClip jumpSoundClip;
    public AudioClip landingSoundClip;

    public float thrusterMaxVolume;
    public float thrusterFadeInTime;
    public float thrusterFadeOutTime;

    [Header("Weapon Clips")]
    public AudioClip[] ballisticFireClips;
    public AudioClip[] plasmaFireClips;
    public AudioClip[] rocketFireClips;

    [Header("Weapon Pickup Clips")]
    public AudioClip weaponPickupClip;

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
            thrusterSource.volume = Mathf.Lerp(
                thrusterSource.volume,
                thrusterMaxVolume,
                thrusterFadeOutTime * Time.deltaTime);
            thrusterSource.pitch = Mathf.Lerp(
                thrusterSource.pitch,
                1.6f,
                thrusterFadeInTime * 3f * Time.deltaTime);
        }
        else
        {
            thrusterSource.volume = Mathf.Lerp(
                thrusterSource.volume,
                0.0f,
                thrusterFadeOutTime * Time.deltaTime);
            thrusterSource.pitch = Mathf.Lerp(
                thrusterSource.pitch,
                1.0f,
                thrusterFadeOutTime * Time.deltaTime);
        }
    }

    public void PlayWeaponFireClip(WeaponData data)
    {
        switch(data.projectileType)
        {
            case ProjectileType.Ballistic:
                PlayRandomClip(ballisticFireClips, 0.95f, 1.1f);
                break;
            case ProjectileType.EnergyBeam:
                PlayRandomClip(plasmaFireClips, 0.95f, 1.1f);
                break;
            case ProjectileType.Rocket:
                PlayRandomClip(rocketFireClips, 0.95f, 1.1f);
                break;
        }
    }

    public void PlayRandomClip(AudioClip[] clips, float minPitch, float maxPitch)
    {
        int i = Random.Range(0, clips.Length);
        oneShotSource.pitch = minPitch;
        if (minPitch < maxPitch)
            oneShotSource.pitch = Random.Range(minPitch, maxPitch);
        oneShotSource.PlayOneShot(clips[i]);
    }

    public void PlayWeaponPickupSound()
    {
        UIAudioManager.Instance.AudioSource.PlayOneShot(weaponPickupClip);
    }
}
