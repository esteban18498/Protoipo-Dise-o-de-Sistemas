using System.Collections.Generic;

namespace ZenUtils
{
    public class Stats
    {
        public float maxStamina;
        public float stamina;
        public float maxHealth;
        public float health;
    }
    public enum state
    {
        Idle,
        OnGuard,
        Performing,
        Vulnerable
    }
/*
    public abstract class GuardClass
    {
        public override string ToString()
        {
            return this.GetType().ToString();
        }

        public abstract string Step();
    }
*/
    public class ComboMod
    {
        public comboType type;
        public ComboMod(comboType t)
        {
            type = t;
        }
    }


    public class Combo
    {
        public comboType type;
        public List<ComboMod> mods;
        public Combo(comboType t)
        {
            type = t;
        }
    }

    public enum comboType{
        atack1,
        atack2,
        atack3,
        buff,
        dodge,
        mod1,
        mod2,
        mod3,
        mod4,
        missClick
    }

}
