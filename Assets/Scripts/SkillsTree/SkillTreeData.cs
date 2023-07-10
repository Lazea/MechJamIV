using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
        fileName = "SO_SkillTreeData",
        menuName = "Scriptable Objects/Skill Tree Data")]
public class SkillTreeData : ScriptableObject
{
    [System.Serializable]
    public struct SkillLayer
    {
        public Skill[] skills;
    }
    public SkillLayer[] layers;

    [ContextMenu("Reset Skills Unlock State")]
    public void ResetSkillsUnlockState()
    {
        foreach(SkillLayer l in layers)
        {
            foreach(Skill s in l.skills)
            {
                s.unlocked = false;
            }
        }
    }
}
