using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[CreateAssetMenu(fileName = "Environment", menuName = "Scriptable Objects/Environment")]

public class Tileset_ENV : ScriptableObject
{

    [Header("Map Audio")]
    public AudioClip ambient;
    public AudioClip song;
    public AudioReverbPreset reverb;

    [Header("Gameobject containing weather effects")]
    public GameObject weather;

    [Header("Visual Settings")]
    public VolumeProfile volume;
    public Material sky;
    public float fogLevel = .01f;
    public Color fogColor;

}
