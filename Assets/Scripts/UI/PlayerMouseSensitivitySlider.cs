using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMouseSensitivitySlider : MonoBehaviour
{
    public Slider xSlider;
    public Slider ySlider;

    private void OnEnable()
    {
        xSlider.value = GameSettings.xMouseSensitivity / 100f;
        ySlider.value = GameSettings.yMouseSensitivity / 100f;
    }

    private void Update()
    {
        GameSettings.xMouseSensitivity = xSlider.value * 100f;
        GameSettings.yMouseSensitivity = ySlider.value * 100f;
    }
}
