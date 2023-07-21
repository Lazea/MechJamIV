using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIElementAudioDispatch : MonoBehaviour
{
    public AudioClip clickClip;
    public AudioClip clickReleaseClip;
    public AudioClip enterClip;
    public AudioClip exitClip;

    public void SendClip(AudioClip clip)
    {
        var manager = UIAudioManager.Instance;
        if (manager == null)
        {
            Debug.LogWarningFormat(
                "[{0}] No UIAudioManager found. Can't play clip!!",
                name);
        }
        else
        {
            manager.AudioSource.PlayOneShot(clip);
        }
    }

    public void OnClick()
    {
        SendClip(clickClip);
    }

    public void OnClickRelease()
    {
        SendClip(clickReleaseClip);
    }

    public void OnEnter()
    {
        SendClip(enterClip);
    }

    public void OnExit()
    {
        SendClip(exitClip);
    }
}
