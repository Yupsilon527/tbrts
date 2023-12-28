using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CardDefines
{
    public enum ArmorType
    {
        unarmored,
        soft,   //blocks stagger/CC, recieves damage
        hard,   //blocks entirely
    }
    public enum CardNumbers
    {
        Joker = 0,
        One = 1,
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8,
        Nine = 9,
        Ten = 10,
        Jack = 11,
        Queen = 12,
        King = 13,
        Ace = 14,
    }
    public enum CardColors
    {
        punch,    //attacks
        kick, //strong attacks
        guard,   //parry
        movement,  //movement
        special,
		ultimate
    }
    public enum CardFace
    {
        punch = 0,
        kick = 1,
        guard = 2,
        jump = 3,
        crouch = 4,
        forward = 5,
        backward = 6,
        total = 7
    }
    public static GameObject CardPrefab = Resources.Load<GameObject>("Prefabs/Interface/Cards/Card");
    public static float CardDrawInterval = .3f;
    public static float CardSpeedNormal = .3f;
    public static float CardFollowSpeed = .2f;
}
