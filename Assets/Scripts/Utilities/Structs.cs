using System.Collections.Generic;
using UnityEngine;

namespace Structs
{

    [System.Serializable]
    public struct Stats
    {
        public int health;
        public int energy;
        public int ammo;

        public int physicalDamage; // How much damage its going to use from the skill, keep it between 0 to 3, 3 being triple the damage of the skill
        public int physicalArmor;

        public int magicDamage;
        public int magicArmor;

        [Range(0.0f,1.0f,order =1)] public float defenseBonus; // For guarding
        [Range(0.0f,1.0f)] public float buffBonus; // Buffing Keep it between 0 and 1
        public float speed;
        public int accuracy;
        [Range(0.0f, 0.75f)] public float critRate;
        public int xpYield;
    }

    [System.Serializable]
    public struct StatsLevel
    {
        public int healthLevel;
        public int energyLevel;
        public int ammoLevel;

        public int physicalDamageLevel;
        public int physicalArmorLevel;
        public int magicDamageLevel;
        public int magicArmorLevel;

        public int critRateLevel;
    }

    [System.Serializable]
    public struct Encounter
    {
        public List<Entity> entites;
        public List<EntityData> entitiesData;
    }

    [System.Serializable]
    public struct EncounterInfo
    {
        public string encounterName;
        public int arenaId;
        public EntityData[] encounters;

        public Sprite chibiPreview;
        public RuntimeAnimatorController chibiAnimator;
    }

    [System.Serializable]
    public struct PlayerKeyInventory
    {
        public bool desertKey;
        public bool piramidKey;
        public bool dungeonKey;
        public bool scifiKey;
    }
}
