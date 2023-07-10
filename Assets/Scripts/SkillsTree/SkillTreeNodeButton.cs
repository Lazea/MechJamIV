using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class SkillTreeNodeButton : MonoBehaviour,
    IPointerEnterHandler,
    ISelectHandler
{
    public Skill skill;

    [Header("Events")]
    public UnityEvent<Skill> onSelect = new UnityEvent<Skill>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        onSelect.Invoke(skill);
    }

    public void OnSelect(BaseEventData eventData)
    {
        onSelect.Invoke(skill);
    }
}
