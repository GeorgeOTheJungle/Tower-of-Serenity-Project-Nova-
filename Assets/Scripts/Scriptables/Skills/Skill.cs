using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;
[CreateAssetMenu(fileName ="New Skill",menuName ="Skill")]
public class Skill : ScriptableObject
{
    [Header("Skill data"), Space(10)]
    public bool unlocked;
    public bool isSelfTarget;
    public string skillName; // Use it for UI.
    public float baseDamage; // Total damage the skill will do,
    public DamageType damageType;
    public ResourceType resourceType;
    public TargetingStyle targetingStyle;
    public int resourceAmount;

    public string animationKey = ""; // Use this to call the animation you need on the entity

    [Space(10)]
    public Sprite smallIcon;
    public Sprite largeIcon;
}
