using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayRandomAudioClip : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] clips;

    public float minPitch;
    public float maxPitch;

    // Start is called before the first frame update
    void Start()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        audioSource.pitch = minPitch;
        if(minPitch < maxPitch)
        {
            audioSource.pitch = Random.Range(minPitch, maxPitch);
        }

        int i = Random.Range(0, clips.Length);
        audioSource.PlayOneShot(clips[i]);
    }
}
