using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Aesthetics : MonoBehaviour
{
    UI_MenuDirector sceneMain;

    public float effTime = .075f;
    public Vector3 selectOffset;

    public Color flashColor;
    public Color highlightColor;
    public Color imgFlash;
    public Color warnColor;

    public UI_SubElement heldElement;

    public float blinkTime = .5f;

    public float menuCool = .3f;
    public bool swappingMenu;

    public float messageRead = .02f;
    public float menuSpawnTime = .1f;

    public float scale = 1.5f;

    [Range(0, 1)]
    float menuVolumeScale = .8f;
    public AudioClip clickClip;
    public AudioClip clickReleaseClip;
    public AudioClip enterClip;
    public AudioClip exitClip;
    public AudioClip lockClip;

    private void Start()
    {
        sceneMain = FindObjectOfType<UI_MenuDirector>();
    }

    public IEnumerator Slide(UI_SubElement e)
    {
        TextMeshProUGUI text = e.t;
        RectTransform r = text.GetComponent<RectTransform>();
        Vector3 startPos = e.startTextPos;

        float t = 0;
        float period = 0;

        while (period < effTime)
        {
            //mod time while applicable, and while current text object is selected
            if (period < effTime)
            {
                period += Time.unscaledDeltaTime;
                t = period / effTime;
            }


            //mod position over time
            Vector3 targetPos = Vector3.Lerp(startPos, startPos + selectOffset, t);
            r.anchoredPosition = targetPos;

            yield return null;
        }

        yield return new WaitUntil(() => !e.isActive || !e.gameObject.activeSelf);

        while (period > 0)
        {
            //mod time while applicable
            period -= Time.unscaledDeltaTime;
            t = period / effTime;

            //mod position over time
            Vector3 targetPos = Vector3.Lerp(startPos, startPos + selectOffset, t);
            r.anchoredPosition = targetPos;

            yield return null;
        }

        //reset to default values as a safety
        r.anchoredPosition = startPos;

    }

    public void StartSlide(UI_SubElement e)
    {
        StartCoroutine(Slide(e));
    }

    public IEnumerator Scale(UI_SubElement e, bool holdScale)
    {
        //perform initial highlight setup
        RectTransform r = e.r;
        Vector3 startScale = e.startScale;

        float t = 0;
        float period = 0;

        while (period < effTime)
        {
            //mod time while applicable, and while current text object is selected
            if (period < effTime)
            {
                period += Time.unscaledDeltaTime;
                t = period / effTime;
            }

            Vector3 targetSize = Vector3.Lerp(startScale, startScale * scale, t);
            r.sizeDelta = targetSize;

            yield return null;
        }

        yield return new WaitUntil(() => !e.isActive || !e.gameObject.activeSelf);

        while (period > 0)
        {
            //mod time while applicable
            period -= Time.unscaledDeltaTime;
            t = period / effTime;

            Vector3 targetSize = Vector3.Lerp(startScale, startScale * scale, t);
            r.sizeDelta = targetSize;

            yield return null;
        }



    }




    public IEnumerator Flash(UI_SubElement e)
    {

        //perform initial highlight setup
        TextMeshProUGUI text = e.t;
        Color startColor = e.startTextColor;

        float t = 0;
        float period = 0;

        while (period < effTime)
        {
            //mod time while applicable, and while current text object is selected
            if (period < effTime)
            {
                period += Time.unscaledDeltaTime;
                t = period / effTime;
            }

            //handle flashing
            Color colorTrans = Color.Lerp(startColor, highlightColor, t);
            Color targetColor = Color.Lerp(flashColor, colorTrans, t);


            text.faceColor = targetColor;

            yield return null;
        }

        yield return new WaitUntil(() => !e.isActive || !e.gameObject.activeSelf);

        while (period > 0)
        {
            //mod time while applicable
            period -= Time.unscaledDeltaTime;
            t = period / effTime;

            //handle flashing
            Color targetColor = Color.Lerp(startColor, highlightColor, t);
            text.fontMaterial.color = targetColor;

            yield return null;
        }

        //reset to default values as a safety
        text.faceColor = startColor;
    }
    public void StartFlash(UI_SubElement s)
    {
        StartCoroutine(Flash(s));
    }


    public IEnumerator FlashIMG(UI_SubElement e, bool hold)
    {
        //perform initial highlight setup
        Color startColor = e.startIMGColor;

        float t = 0;
        float period = 0;

        while (period < effTime)
        {
            //mod time while applicable, and while current text object is selected
            if (period < effTime)
            {
                period += Time.unscaledDeltaTime;
                t = period / effTime;
            }

            //handle flashing
            Color holdColor = hold ? e.holdIMGColor : startColor;
            
            Color colorTrans = Color.Lerp(startColor, holdColor, t);
            Color targetColor = Color.Lerp(colorTrans, imgFlash, Mathf.Sin(t * 180 * Mathf.Deg2Rad));



            e.img.material.SetColor("_Color", targetColor);

            yield return null;
        }

        if(hold)
        {
            yield return new WaitUntil(() => !e.isActive);

            while (period > 0)
            {
                //mod time while applicable, and while current text object is selected
                if (period > 0)
                {
                    period -= Time.unscaledDeltaTime;
                    t = period / effTime;
                }

                //handle flashing
                Color targetColor = Color.Lerp(e.holdIMGColor, startColor, t);



                e.img.material.SetColor("_Color", targetColor);

                yield return null;
            }
        }

        //reset to default values as a safety
        e.img.material.SetColor("_Color", startColor);

    }

    //used to toggle a UI item on/off
    public IEnumerator Blink(UI_SubElement u)
    {
        if (u != null && u.gameObject != null)
            yield return null;

        u.isBlinking = true;

        float period = 0;

        while(u.isBlinking)
        {
            if (period < blinkTime)
                period += Time.unscaledDeltaTime;
            else
            {
                period = 0;
                u.gameObject.SetActive(!u.gameObject.activeSelf);
            }


            yield return null;
        }

        u.gameObject.SetActive(true);


        yield return null;
    }

    //used to spawn list
    public IEnumerator GrowMenu(UI_HeadElement shell)
    {

        foreach(UI_SubElement u in shell.items)
        {
            if (u != null && u.gameObject != null)
            {
                u.gameObject.SetActive(true);
                u.onAwake.Invoke();
            }

            yield return new WaitForSecondsRealtime(menuSpawnTime);
        }
    }

    public void DisplayText(UI_SubElement u)
    {

        StartCoroutine(DisplayText(u, messageRead, 0));
    }

    public IEnumerator Fade(CanvasGroup g, bool fadeIn, float time)
    {
        //perform initial highlight setup
        float t = 0;
        float period = 0;

        while (period < time)
        {
            //mod time while applicable, and while current text object is selected
            if (period < time)
            {
                period += Time.unscaledDeltaTime;
                t = period / time;
            }

            if (fadeIn)
                g.alpha = t;
            else
                g.alpha = 1 - t;

            yield return null;
        }
    }



    //used to grow message
    public IEnumerator DisplayText(UI_SubElement u, float time, float delay)
    {

        if (!u.isReading)
        {
            u.isReading = true;
            TextMeshProUGUI t = u.t;


            string s = t.text;

            t.text = "";
            int i = 0;

            yield return new WaitForSecondsRealtime(delay);

            while (t.text != s)
            {
                t.text += s[i];
                i++;

                yield return new WaitForSecondsRealtime(time);
            }

            u.isReading = false;
        }

    }

    
    public void SwapIstant(UI_HeadElement t)
    {
        StartCoroutine(SwapPanel(sceneMain, t));
    }


    //used to swap from one panel in a list to another. 
    public IEnumerator SwapPanel(UI_MenuDirector dir, UI_HeadElement target)
    {
        swappingMenu = true;

        if (target.frame.Count > 0)
        {
            foreach(UI_PanelController f in target.frame)
            {
                f.holdOpen = true;
            }
        }
        //SendClip(clickClip);

        foreach(UI_HeadElement g in dir.UIMajor)
        {
            g.gameObject.SetActive(false);

            //if menu is provided, hide menu
            if (g.items.Length > 0)
            {
                foreach(UI_SubElement u in g.items)
                {
                    if(u != null && u.gameObject != null)
                        u.gameObject.SetActive(false);
                }
            }
        }

        yield return new WaitForSecondsRealtime(target.swapTime);

        foreach (UI_HeadElement g in dir.UIMajor)
        {
            g.gameObject.SetActive(g == target);

            //if menu is provided, show menu
            if (g.items.Length > 0 && g.gameObject.activeSelf)
                StartCoroutine(GrowMenu(g));
        }

        swappingMenu = false;

        if (target.frame.Count > 0)
        {
            foreach (UI_PanelController f in target.frame)
            {
                f.holdOpen = false;
            }
        }
    }

    public void PlayHover()
    {
        SendClip(enterClip);
    }

    public void PlayClick()
    {
        SendClip(clickClip);
    }

    public void SendClip(AudioClip clip)
    {
        var manager = UIAudioManager.Instance;

        if (!manager)
            Debug.LogWarningFormat("[{0}] No UIAudioManager found. Can't play clip!!", name);
        else
            manager.AudioSource.PlayOneShot(clip, menuVolumeScale);
    }
}
