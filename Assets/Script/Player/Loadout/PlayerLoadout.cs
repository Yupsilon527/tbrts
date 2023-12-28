using System;

[Serializable]
public class PlayerLoadout 
{
    public HeroSlot[] heroSlots;
    [Serializable]
    public class HeroSlot
    {
        public HeroStatTableSO heroClass;
        public AbilitySO[] cardLoadout;
    }
}
