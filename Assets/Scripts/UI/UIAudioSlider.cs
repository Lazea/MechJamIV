using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAudioSlider : MonoBehaviour
{
    public string mixerParameter;

    private void OnEnable()
    {
        float value;
        AudioSettingsManager.Instance.mixer.GetFloat(mixerParameter, out value);
        GetComponent<Slider>().value = AudioSettingsManager.Instance.GetNormalizedVolume(value);
    }
}
