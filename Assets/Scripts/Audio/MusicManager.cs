using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    private AudioSource mainThemeMusic;
    private AudioSource offCombatMusic;
    private AudioSource repairShopMusic;
    private AudioSource inCombatMusic;
    private AudioSource inCombatMusicLayer_1;
    private AudioSource inCombatMusicLayer_2;
    private AudioSource stinger_1;
    private AudioSource stinger_2;

    public int testIndex = 0; //to be replaced by currentScene.buildIndex
    public int musicIntensity = 0; //0 = base combat music, 1 = base + layer 1, 2 = base + layer 1 + layer 2

    public bool endStage; //to be triggered by ScriptableObject Event when the stage/wave has ended
    public bool playerDead; //to be triggered by ScriptableObject Event when the player dies
    private bool passedStageMusic_1;
    private bool passedStageMusic_2;
    private bool staged_1;
    private bool staged_2;

    public float fadeDuration = 1f; //time it takes for all the music fade
    private float timer; //timer for fade



    void Awake()
    {
        //Music Manager
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Music");

        if(objs.Length > 1)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);

        //To be dragged instead...
        mainThemeMusic = GameObject.Find("Main_Theme_Music").GetComponent<AudioSource>();
        offCombatMusic = GameObject.Find("Off_Combat_Music").GetComponent<AudioSource>();
        repairShopMusic = GameObject.Find("Repair_Shop_Music").GetComponent<AudioSource>();
        inCombatMusic = GameObject.Find("In_Combat_Music").GetComponent<AudioSource>();
        inCombatMusicLayer_1 = GameObject.Find("In_Combat_Layer_1").GetComponent<AudioSource>();
        inCombatMusicLayer_2 = GameObject.Find("In_Combat_Layer_2").GetComponent<AudioSource>();
        stinger_1 = GameObject.Find("Stinger_1").GetComponent<AudioSource>();
        stinger_2 = GameObject.Find("Stinger_2").GetComponent<AudioSource>();
    }

    private void Start()
    {
        //Check if at Main Menu Scene
        if(testIndex == 0)
        {
            mainThemeMusic.volume = 0f;
            mainThemeMusic.Play();
        }


        //Staging volume to 0 for fade in
        offCombatMusic.volume = 0f;
        repairShopMusic.volume = 0f;
        inCombatMusic.volume = 0f;
        inCombatMusicLayer_1.volume = 0f;
        inCombatMusicLayer_2.volume = 0f;
        stinger_1.volume = 1f;
        stinger_2.volume = 0.25f;

        endStage = false;
        staged_1 = false;
        staged_2 = false;
        passedStageMusic_1 = false;
        passedStageMusic_2 = false;
        playerDead = false;
        timer = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        testIndex = SceneManager.GetActiveScene().buildIndex;

        switch (testIndex)
        {
            case 0:

                if (!mainThemeMusic.isPlaying)
                {
                    mainThemeMusic.Play();
                    inCombatMusic.Stop();
                    inCombatMusicLayer_1.Stop();
                    inCombatMusicLayer_2.Stop();
                    stinger_1.Stop();
                    stinger_2.Stop();
                }

                if (timer < fadeDuration)
                {
                    timer += Time.deltaTime;
                    float normalizedTime = timer / fadeDuration;
                    float targetVolumeFadeIn = Mathf.Lerp(0.0f, 1.0f, normalizedTime);
                    float targetVolumeFadeOut = Mathf.Lerp(offCombatMusic.volume, 0.0f, normalizedTime);
                    mainThemeMusic.volume = targetVolumeFadeIn;
                    offCombatMusic.volume = targetVolumeFadeOut;
                }

                if(offCombatMusic.volume < 0.1f)
                {
                    offCombatMusic.Stop();
                    repairShopMusic.Stop();
                }

                passedStageMusic_1 = false;
                passedStageMusic_2 = false;
                playerDead = false;
                endStage = false;
                musicIntensity = 0;
                break;


            case 1:
                inCombatMusic.Stop();
                inCombatMusicLayer_1.Stop();
                inCombatMusicLayer_2.Stop();

                if (!offCombatMusic.isPlaying)
                {
                    offCombatMusic.Play();
                    stinger_1.Play();
                }

                timer = 0.0f;

                if(mainThemeMusic.volume <= 0.1f)
                {
                    mainThemeMusic.Stop();
                }


                if (timer < fadeDuration)
                {
                    timer += Time.deltaTime;
                    float normalizedTime = timer / fadeDuration;
                    float targetVolumeFadeOut = Mathf.Lerp(mainThemeMusic.volume, 0.0f, normalizedTime);
                    float targetVolumeFadeIn = Mathf.Lerp(offCombatMusic.volume, 1.0f, normalizedTime);
                    mainThemeMusic.volume = targetVolumeFadeOut;
                    offCombatMusic.volume = targetVolumeFadeIn;
                }

                passedStageMusic_1 = false;
                passedStageMusic_2 = false;
                playerDead = false;
                endStage = false;
                musicIntensity = 0;
                break;

            case 3:
                mainThemeMusic.Stop();
                if (!endStage)
                {
                    staged_1 = false;
                    staged_2 = false;
                }

                if (!inCombatMusic.isPlaying)
                {
                    stinger_2.Play();
                    inCombatMusic.Play();
                    inCombatMusicLayer_1.Play();
                    inCombatMusicLayer_2.Play();
                    offCombatMusic.Play();
                    repairShopMusic.Play();
                }

                timer = 0.0f;

                if (timer < fadeDuration && playerDead == false && endStage == false)
                {
                    timer += Time.deltaTime;
                    float normalizedTime = timer / fadeDuration;
                    float targetVolumeFadeOut = Mathf.Lerp(offCombatMusic.volume, 0.0f, normalizedTime);
                    float targetVolumeFadeOutRepair = Mathf.Lerp(repairShopMusic.volume, 0.0f, normalizedTime);
                    float targetVolumeFadeIn = Mathf.Lerp(inCombatMusic.volume, 1.0f, normalizedTime);
                    offCombatMusic.volume = targetVolumeFadeOut;
                    repairShopMusic.volume = targetVolumeFadeOutRepair;
                    inCombatMusic.volume = targetVolumeFadeIn;
                }

                if(musicIntensity == 1 || passedStageMusic_1 == true && musicIntensity != 0)
                {
                    passedStageMusic_1 = true;
                    timer = 0.0f;

                    if(timer < fadeDuration && staged_1 == false)
                    {
                        timer += Time.deltaTime;
                        float normalizedTime = timer / fadeDuration;
                        float targetVolumeFadeInLayer_1 = Mathf.Lerp(inCombatMusicLayer_1.volume, 0.6f, normalizedTime);
                        inCombatMusicLayer_1.volume = targetVolumeFadeInLayer_1;
                    }
                }

                if(musicIntensity == 2 || passedStageMusic_2 == true && musicIntensity != 0)
                {
                    passedStageMusic_2 = true;
                    timer = 0.0f;

                    if(timer < fadeDuration && staged_2 == false)
                    {
                        timer += Time.deltaTime;
                        float normalizedTime = timer / fadeDuration;
                        float targetVolumeFadeInLayer_2 = Mathf.Lerp(inCombatMusicLayer_2.volume, 0.6f, normalizedTime);
                        inCombatMusicLayer_2.volume = targetVolumeFadeInLayer_2;
                    }
                }
                break;

            case 2:
                inCombatMusic.Stop();
                inCombatMusicLayer_1.Stop();
                inCombatMusicLayer_2.Stop();

                timer = 0.0f;

                if (timer < fadeDuration)
                {
                    timer += Time.deltaTime;
                    float normalizedTime = timer / fadeDuration;
                    float targetVolumeFadeIn = Mathf.Lerp(repairShopMusic.volume, 1.0f, normalizedTime);
                    repairShopMusic.volume = targetVolumeFadeIn;
                }

                if (repairShopMusic.volume > 0.95f)
                {
                    endStage = false;
                    stinger_1.Stop();
                    stinger_2.Stop();
                }
                break;
        }

        
        if (endStage)
        {
            staged_1 = true;
            staged_2 = true;

            if (!stinger_2.isPlaying && inCombatMusic.volume > 0.8f)
            {
                stinger_2.Play();
            }

            timer = 0.0f;

            if (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                float normalizedTime = timer / fadeDuration;
                float targetVolumeFadeOut = Mathf.Lerp(inCombatMusic.volume, 0.0f, normalizedTime);
                float targetVolumeFadeOutLayer_1 = Mathf.Lerp(inCombatMusicLayer_1.volume, 0.0f, normalizedTime);
                float targetVolumeFadeOutLayer_2 = Mathf.Lerp(inCombatMusicLayer_2.volume, 0.0f, normalizedTime);
                float targetVolumeFadeIn = Mathf.Lerp(offCombatMusic.volume, 1.0f, normalizedTime);
                inCombatMusic.volume = targetVolumeFadeOut;
                inCombatMusicLayer_1.volume = targetVolumeFadeOutLayer_1;
                inCombatMusicLayer_2.volume = targetVolumeFadeOutLayer_2;
                offCombatMusic.volume = targetVolumeFadeIn;
            }
        }

        if (playerDead)
        {
            if(!stinger_1.isPlaying && inCombatMusic.volume > 0.8f)
            {
                stinger_1.Play();
            }

            timer = 0.0f;

            if (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                float normalizedTime = timer / fadeDuration;
                float targetVolumeFadeOut = Mathf.Lerp(inCombatMusic.volume, 0.0f, normalizedTime);
                float targetVolumeFadeOutLayer_1 = Mathf.Lerp(inCombatMusicLayer_1.volume, 0.0f, normalizedTime);
                float targetVolumeFadeOutLayer_2 = Mathf.Lerp(inCombatMusicLayer_2.volume, 0.0f, normalizedTime);
                float targetVolumeFadeIn = Mathf.Lerp(offCombatMusic.volume, 1.0f, normalizedTime);
                inCombatMusic.volume = targetVolumeFadeOut;
                inCombatMusicLayer_1.volume = targetVolumeFadeOutLayer_1;
                inCombatMusicLayer_2.volume = targetVolumeFadeOutLayer_2;
                offCombatMusic.volume = targetVolumeFadeIn;
            }

            passedStageMusic_1 = false;
            passedStageMusic_2 = false;
            endStage = false;
            musicIntensity = 0;

        }

    }

    public void EndStage()
    {
        endStage = true;

        stinger_1.PlayOneShot(stinger_1.clip);
    }

    public void StageEnded()
    {
        endStage = false;

            stinger_1.PlayOneShot(stinger_1.clip);
    }

    public void PlayerDied()
    {
        playerDead = true;
    }

}
