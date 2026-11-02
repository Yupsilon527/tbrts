using System;
using UnityEngine;
using static UnitDefines;

[Serializable]
public class CharacterSheet
{
    public CharacterSprite[] states = new CharacterSprite[]
    {
        new  CharacterSprite(){frame= 0},
        new  CharacterSprite(){frame= (SpriteFrame)1},
        new  CharacterSprite(){frame= (SpriteFrame)2},
        new  CharacterSprite(){frame= (SpriteFrame)3},
        new  CharacterSprite(){frame= (SpriteFrame)4},
        new  CharacterSprite(){frame= (SpriteFrame)5},
        new  CharacterSprite(){frame= (SpriteFrame)6},
        new  CharacterSprite(){frame= (SpriteFrame)7},
        new  CharacterSprite(){frame= (SpriteFrame)8},

    };
    public Sprite GetSprite(int sprite)
    {
        return GetSprite((SpriteFrame)sprite);
    }
    public Sprite GetSprite(SpriteFrame sprite)
    {
        if (states == null || states.Length == 0)
            return null;


        foreach (var state in states)
        {
            if (state.frame == sprite)
                return state.sprite;
        }

        foreach (var state in states)
        {
            if (state.frame == SpriteFrame.Idle)
                return state.sprite;
        }

        return states[0].sprite;
    }
}
