using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepairShopUI : MonoBehaviour
{
    private AudioSource audioSource;

    public AudioClip repairHover;
    public AudioClip exitLeft;
    public AudioClip exitRight;
    public AudioClip deposit;
    public AudioClip confirmUI;
    public AudioClip fixUI;



    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void RepairHighlight()
    {
        audioSource.PlayOneShot(repairHover);
    }

    public void ExitLeftHighlight()
    {
        audioSource.PlayOneShot(exitLeft);
    }

    public void ExitRightHighlight()
    {
        audioSource.PlayOneShot(exitRight, 0.8f);
    }

    public void DepositHighlight()
    {
        audioSource.PlayOneShot(deposit);
    }

    public void OnClick()
    {
        audioSource.PlayOneShot(confirmUI, 0.5f);
    }

    public void OnClickMech()
    {
        audioSource.PlayOneShot(fixUI, 0.6f);
    }
}
