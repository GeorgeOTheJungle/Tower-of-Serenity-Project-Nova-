using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance;

    [SerializeField] private int currentXP;
    [SerializeField] private int totalXPStored;
    [SerializeField] private List<PlayerSkill> allPlayerSkills;

    private void Awake()
    {
        Instance = this;
       // ResetSkillsToDefault(); // TODO REMOVE THIS LATER?
 
    }

    [ContextMenu("Initialize Skills")]
    public void InitializeSkills()
    {
        foreach (PlayerSkill skill in allPlayerSkills)
        {
            skill.Initialize();
        }
    }

    [ContextMenu("Reset skills to default")]
    public void ResetSkillsToDefault()
    {
        Debug.LogWarning("RESETING SKILLS FROM THE SKILL MANAGER");
        foreach (PlayerSkill skill in allPlayerSkills)
        {
            skill.RestoreToDefault();
        }
    }

    [ContextMenu("Skills Factory Reset (WILL RESET TO 0 ALL)")]
    public void FactoryReset()
    {
        foreach (PlayerSkill skill in allPlayerSkills)
        {
            skill.ResetToFactory();
        }
    }

    public void GetXP(int total)
    {
        currentXP += total;
        totalXPStored += total;

        if(currentXP < 0)currentXP = 0;
        if(totalXPStored < 0)totalXPStored = 0;
    }

    public void ResetXP()
    {
        currentXP = totalXPStored;
        foreach(PlayerSkill skill in allPlayerSkills)
        {
            skill.ResetToLevel1();
        }
    }

    public bool SkillCanBeUpgraded(PlayerSkill skill)
    {
        if (skill.unlocked == false) return false;
        else if (skill.level > 3) return false; 
        else if (skill.RequiredXp() <= currentXP) return true;
        else return false;
    }

    public bool SkillCanBeUnlocked(PlayerSkill skill)
    {
        return skill.initialUnlockCost <= currentXP;
    }

    public bool HaveEnoughXP(int xpNeed)
    {
        return currentXP >= xpNeed;
    }

    public void UpgradeSkill(PlayerSkill skillToUpgrade)
    {
        currentXP -= skillToUpgrade.RequiredXp();
        skillToUpgrade.UpdgradeSkill();
    }

    public void UnlockSkill(PlayerSkill skillToUnlock)
    {
        currentXP -= skillToUnlock.initialUnlockCost;
        skillToUnlock.unlocked = true;
        skillToUnlock.level = 1;
    }

    public List<PlayerSkill> GetAvaliableSkills()
    {
        List<PlayerSkill> package = new List<PlayerSkill>();

        foreach(PlayerSkill skill in allPlayerSkills)
        {
            if(skill.unlocked == true) package.Add(skill);
        }

        return package;
    }

    public List<PlayerSkill> GetAllSkills() => allPlayerSkills;

    public string GetCurrentXP() => currentXP.ToString();

    public int GetCurXP() => currentXP;

}
