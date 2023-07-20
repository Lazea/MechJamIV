using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Skill
{
    public SkillData skillData;

    [Header("Unlocked")]
    public bool unlocked;

    [Header("Hierarchy")]
    public int skillTreeLayer;
    public int[] childrenInNextLayer;
    public int[] childrenInLayer;
}
