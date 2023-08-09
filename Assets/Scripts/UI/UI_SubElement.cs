using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;


public class UI_SubElement : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler
{
    UI_Aesthetics uA;
    public UI_MenuDirector menu;
    public TextMeshProUGUI t;
    public RectTransform r;
    public Image img;
    public CanvasGroup group;

    public AudioClip clickClip;

    [HideInInspector]
    public Color startTextColor;
    [HideInInspector]
    public Vector3 startScale;
    [HideInInspector]
    public Vector3 startTextPos;

    [HideInInspector]
    public Color startIMGColor;
    public Color holdIMGColor;

    [HideInInspector]
    public bool isSliding;
    [HideInInspector]
    public bool isFlashing;
    [HideInInspector]
    public bool isScaling;
    [HideInInspector]
    public bool isBlinking;
    [HideInInspector]
    public bool isReading;


    bool isHovering;
    public bool elementLocked;
    public bool holdUntilRelease;

    public bool isActive;

    public UnityEvent onClick;
    public UnityEvent onAwake;
    public UnityEvent onEnter;

    void Awake()
    {
        uA = FindObjectOfType<UI_Aesthetics>();
        menu = FindObjectOfType<UI_MenuDirector>();
        t = GetComponentInChildren<TextMeshProUGUI>();
        startTextColor = t.faceColor;
        

        if(!r)
            r = GetComponent<RectTransform>();

        startScale = r.sizeDelta;

        startTextPos = t.GetComponent<RectTransform>().anchoredPosition;


        if (!img)
        {
           TryGetComponent(out img);
        }

        if (img)
        {
            img.material = new Material(img.material);
            startIMGColor = img.material.color;
        }

        if(!group)
        {
            TryGetComponent(out group);
        }
    }

    // Update is called once per frame
    void Update()
    {
        isActive = ((isHovering && !uA.heldElement) || uA.heldElement == this);
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        if (!uA.heldElement)
        {
            onEnter.Invoke();
            isHovering = true;
        }
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
        if(uA.heldElement == this)
        {
            uA.heldElement = null;
            
        }
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        if (!uA.heldElement || uA.heldElement == this)
        {
            if (!elementLocked)
            {
                if (!clickClip)
                    uA.SendClip(uA.clickClip);
                else
                    uA.SendClip(clickClip);

                if (holdUntilRelease)
                    uA.heldElement = this;
            }
            else
                uA.SendClip(uA.lockClip);

            if (!elementLocked)
                onClick.Invoke();
        }
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {

        isHovering = false;
    }

    public void SetLockState(bool l)
    {
        elementLocked = l;
    }

    public void StartFade(bool fadeIN)
    {
        StartCoroutine(uA.Fade(group, fadeIN, .5f));
    }

    public void FlashIMG(bool fHold)
    {
        uA.StartCoroutine(uA.FlashIMG(this, fHold));
    }
    public void StartScale(bool s)
    {
        StartCoroutine(uA.Scale(this, s));
    }
}
