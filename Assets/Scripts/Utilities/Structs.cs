namespace Structs
{

    [System.Serializable]
    public struct Stats
    {
        public int health;
        public int energy;
        public int ammo;

        public int physicalDamage;
        public int physicalArmor;

        public int magicDamage;
        public int magicArmor;

        public float buffBonus;

        public int accuracy;
        public int critRate;
        public int xpYield;
    }
}
