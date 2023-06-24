using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SOGameEventSystem.Events;

public class SkillTreeManager : MonoBehaviour
{
    [Header("Data")]
    public SkillTreeData data;
    public PlayerData playerData;

    public Skill selectedSkill;

    [Header("Skill Tree")]
    public RectTransform skillTreeParent;

    public GameObject skillNodePrefab;
    public GameObject connectorPrefab;
    public Color lockedNodeColor;

    public TMPro.TextMeshProUGUI creditsText;

    [System.Serializable]
    public struct SkillNode
    {
        public RectTransform node;
        public Skill skill;
        public List<SkillNode> parentNodes;
        public List<SkillNode> childrenNodes;
    }
    public List<SkillNode> skillNodes;

    [Header("Skill Details UI Elements")]
    public Image skillIcon;
    public TMPro.TextMeshProUGUI skillTitleText;
    public TMPro.TextMeshProUGUI skillDescriptionText;
    public TMPro.TextMeshProUGUI skillCostText;
    public Button unlockButton;

    void Awake()
    {
        CreateTree();
        SelectSkill(skillNodes[0].skill);
        UpdateSkillInfo(skillNodes[0].skill);
        UpdateCreditsText();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region [Skill Tree Builder]
    [ContextMenu("Create Tree")]
    public void CreateTree()
    {
        float w = skillTreeParent.rect.width;
        float h = skillTreeParent.rect.height;

        int k = 0;
        float heightOffset = h / (data.layers.Length + 1f);
        for (int i = 0; i < data.layers.Length; i++)
        {
            float widthOffset = w / (data.layers[i].skills.Length + 1f);
            for (int j = 0; j < data.layers[i].skills.Length; j++)
            {
                SkillNode skillNode = new SkillNode();

                skillNode.node = Instantiate(
                    skillNodePrefab,
                    skillTreeParent).GetComponent<RectTransform>();
                skillNode.skill = data.layers[i].skills[j];

                SkillTreeNodeButton skillNodeBtn = skillNode.node.
                    GetComponent<SkillTreeNodeButton>();
                skillNodeBtn.skill = skillNode.skill;
                skillNodeBtn.onSelect.AddListener(SelectSkill);
                skillNodeBtn.onSelect.AddListener(UpdateSkillInfo);

                skillNode.skill.id = k;
                if (!skillNode.skill.unlocked)
                {
                    skillNode.node.GetComponent<Image>().color = lockedNodeColor;
                    Image iconImg = skillNode.node.Find("Icon").GetComponent<Image>();
                    iconImg.sprite = skillNode.skill.icon;
                    iconImg.color = lockedNodeColor;
                }

                skillNode.node.anchorMin = Vector2.zero;
                skillNode.node.anchorMax = Vector2.zero;
                skillNode.node.anchoredPosition3D = new Vector3(
                    widthOffset * (j + 1),
                    heightOffset * (i + 1),
                    0f);

                skillNode.parentNodes = new List<SkillNode>();
                skillNode.childrenNodes = new List<SkillNode>();

                skillNodes.Add(skillNode);
                k++;
            }
        }

        foreach (SkillNode s in skillNodes)
        {
            Vector2 pointA = s.node.anchoredPosition;
            foreach (int i in s.skill.childrenInLayer)
            {
                int j = GetSkillNodeID(
                    s.skill.skillTreeLayer,
                    i);
                Vector2 pointB = skillNodes[j].node.anchoredPosition;
                s.childrenNodes.Add(skillNodes[j]);
                SpawnConnector(pointA, pointB).transform.SetSiblingIndex(0);
            }
            foreach (int i in s.skill.childrenInNextLayer)
            {
                int j = GetSkillNodeID(
                    s.skill.skillTreeLayer + 1,
                    i);
                Vector2 pointB = skillNodes[j].node.anchoredPosition;
                s.childrenNodes.Add(skillNodes[j]);
                SpawnConnector(pointA, pointB).transform.SetSiblingIndex(0);
            }
        }

        foreach (SkillNode s in skillNodes)
        {
            foreach(SkillNode _s in s.childrenNodes)
            {
                _s.parentNodes.Add(s);
            }
        }
    }

    int GetSkillNodeID(int layerID, int layerChildID)
    {
        int nodeCount = 0;
        for (int i = 0; i < layerID; i++)
        {
            nodeCount += data.layers[i].skills.Length;
        }

        return nodeCount + layerChildID;
    }

    public UILine SpawnConnector(Vector2 pointA, Vector2 pointB)
    {
        UILine connector = Instantiate(
            connectorPrefab,
            skillTreeParent).GetComponent<UILine>();
        connector.rectTransform.anchorMin = Vector2.zero;
        connector.rectTransform.anchorMax = Vector2.zero;
        connector.startPoint = pointA;
        connector.endPoint = pointB;

        return connector;
    }

    public void SetUnlockColor(SkillNode s)
    {
        Color c = (s.skill.unlocked) ? Color.white : lockedNodeColor;
        s.node.GetComponent<Image>().color = c;
        s.node.Find("Icon").GetComponent<Image>().color = c;
    }

    [ContextMenu("Update Tree")]
    public void UpdateTree()
    {
        foreach(SkillNode s in skillNodes)
        {
            SetUnlockColor(s);
        }
    }

    [ContextMenu("Reset Tree")]
    public void ResetSkillTree()
    {
        foreach (SkillNode s in skillNodes)
        {
            s.skill.unlocked = false;
        }
        UpdateTree();
    }
    #endregion

    #region [Skill Select & Unlock]
    public void SelectSkill(Skill skill)
    {
        selectedSkill = skill;
    }

    public bool IsSkillUnlockable(Skill skill)
    {
        foreach (SkillNode skillNode in skillNodes)
        {
            if (skillNode.skill == skill)
            {
                foreach (SkillNode parentSkillNode in skillNode.parentNodes)
                {
                    if (!parentSkillNode.skill.unlocked)
                        return false;
                }
            }
        }

        return true;
    }

    public bool UnlockSkill(Skill skill)
    {
        if (skill == null)
            return false;

        if (!IsSkillUnlockable(skill))
            return false;

        if (playerData.credits < skill.cost)
            return false;

        playerData.credits -= skill.cost;
        skill.unlocked = true;

        ApplySkill(skill);

        return true;
    }

    public void ApplySkill(Skill skill)
    {
        if(skill.statBoostType != Skill.StatBoostType.None)
        {
            switch (skill.statBoostType)
            {
                case Skill.StatBoostType.Health:
                    playerData.MaxHealth += (int)skill.statBoostValue;
                    break;
                case Skill.StatBoostType.Shield:
                    playerData.MaxShield += (int)skill.statBoostValue;
                    break;
                case Skill.StatBoostType.Damage:
                    playerData.damageMultiplier = skill.statBoostValue;
                    break;
                case Skill.StatBoostType.Speed:
                    playerData.speedScaler = skill.statBoostValue;
                    break;
            }
        }
        else
        {
            // TODO: Implement ability
        }
    }

    public void UnlockSelectedSkill()
    {
        UnlockSkill(selectedSkill);
        UpdateTree();
        UpdateCreditsText();
        UpdateSkillInfo(selectedSkill);
    }
    #endregion

    public void UpdateCreditsText()
    {
        creditsText.text = string.Format(
            "Credits Earned: {0}",
            playerData.credits.ToString("D4"));
    }

    #region [Skill Details]
    public void ClearSkillInfo()
    {
        skillIcon.enabled = false;
        skillTitleText.text = "";
        skillDescriptionText.text = "";
        skillCostText.text = "Credits Cost: ----/----";

        unlockButton.interactable = false;
        unlockButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Unlock";
    }

    public void UpdateSkillInfo(Skill skill)
    {
        skillIcon.sprite = skill.icon;
        skillIcon.enabled = true;
        skillTitleText.text = skill.name;

        skillDescriptionText.text = string.Format(
            skill.description,
            skill.statBoostValue);

        string colorText = (playerData.credits < skill.cost) ? "red" : "green";
        skillCostText.text = string.Format(
            "Credits Cost: <color={0}>{1}</color>/{2}",
            colorText,
            playerData.credits,
            skill.cost);

        unlockButton.interactable = !(playerData.credits < skill.cost ||
            skill.unlocked ||
            !IsSkillUnlockable(skill));
        string unlockText = (skill.unlocked) ? "Unlocked" : "Unlock";
        unlockButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = unlockText;
    }
    #endregion
}
