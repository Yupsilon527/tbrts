using System;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "Data/Visual/Character")]
public class CharacterSO : ScriptableBase
{
    public enum SpriteFrame
    {
        idle = 0,
        walk = 1,
        attack1 = 2,
        attack2 = 3,
        cast = 4,
        dead = 5,
    }
    public CharacterSprite[] sprites = new CharacterSprite[]
    {
        new  CharacterSprite(){frame= (SpriteFrame)0},
        new  CharacterSprite(){frame= (SpriteFrame)1},
        new  CharacterSprite(){frame= (SpriteFrame)2},
        new  CharacterSprite(){frame= (SpriteFrame)3},
        new  CharacterSprite(){frame= (SpriteFrame)4},
        new  CharacterSprite(){frame= (SpriteFrame)5},

    };
    public Sprite GetSprite (int index)
    {
        return sprites.FirstOrDefault(s => (int)s.frame == index)?.sprite;
    }
    [Serializable]
    public class CharacterSprite
    {
        public Sprite sprite;
        public SpriteFrame frame;
    }
}
