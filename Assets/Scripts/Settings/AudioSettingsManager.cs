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
        Debug.LogFormat("Setting Master Vol to {0}", amount);
        SetVolume("MasterVolumeParam", amount);
    }

    public void SetMusicVolume(float amount)
    {
        Debug.LogFormat("Setting Master Music Vol to {0}", amount);
        SetVolume("MusicMasterVolumeParam", amount);
    }

    public void SetEffectsVolume(float amount)
    {
        Debug.LogFormat("Setting Effects Master Vol to {0}", amount);
        SetVolume("EffectsMasterVolumeParam", amount);
    }

    public void SetUIVolume(float amount)
    {
        Debug.LogFormat("Setting UI Vol to {0}", amount);
        SetVolume("UIVolumeParam", amount);
    }

    public void SetVolume(string parameter, float amount)
    {
        float v;
        mixer.GetFloat(parameter, out v);
        float vol = GetDBVolume(Mathf.Max(amount, 0.009f));
        mixer.SetFloat(parameter, vol);
    }
}
