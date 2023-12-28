using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardList <cardType> :List<cardType> where cardType : PropertyCard
{
    public cardType[] FilterCards(string internalName, Mob caster = null)
    {
        List<cardType> validCards = new List<cardType>();

        foreach (cardType card in this)
        {
            if (internalName != "" && card.original.InternalName != internalName)
            {
                continue;
            }
            if (caster != null && card.caster != caster)
            {
                continue;
            }
            validCards.Add(card);

        }
        return validCards.ToArray();
    }

}
