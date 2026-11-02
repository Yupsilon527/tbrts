using System;
using UnityEngine;
using static UnitDefines;

[CreateAssetMenu(fileName = "Character", menuName = "Data/Visual/Character")]
public class CharacterSO : ScriptableBase
{
    public CharacterSheet character;
}

[Serializable]
public class CharacterSprite
{
    public Sprite sprite;
    public SpriteFrame frame;
}