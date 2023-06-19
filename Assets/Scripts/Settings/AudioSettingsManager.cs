using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioSettingsManager : Singleton<AudioSettingsManager>
{
    public AudioMixer mixer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float GetNormalizedVolume(float value)
    {
        return Mathf.Pow(10f, value / 20f);
    }

    public float GetDBVolume(float value)
    {
        return 20f * Mathf.Log10(value);
    }

    public void SetMasterVolume(float amount)
    {
        SetVolume("MasterVolumeParam", amount);
    }

    public void SetMusicVolume(float amount)
    {
        SetVolume("MusicVolumeParam", amount);
    }

    public void SetEffectsVolume(float amount)
    {
        SetVolume("EffectsVolumeParam", amount);
    }

    public void SetUIVolume(float amount)
    {
        SetVolume("UIVolumeParam", amount);
    }

    public void SetVolume(string parameter, float amount)
    {
        float vol = GetDBVolume(amount);
        mixer.SetFloat(parameter, vol);
    }
}
