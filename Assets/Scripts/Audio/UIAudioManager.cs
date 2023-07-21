using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class UIAudioManager : Singleton<UIAudioManager>
{
    AudioSource audioSource;
    public AudioSource AudioSource { get { return audioSource; } }

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
}
