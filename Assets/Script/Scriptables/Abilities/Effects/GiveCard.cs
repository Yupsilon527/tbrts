using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Give Card", menuName = "Abilities/Effects/Give Card")]
public class GiveCard : AbilityEffect
{
    public enum CardPlacement
    {
        shuffle,
        inhand
    }
    public int cardAmount = 1;
    public CardPlacement cardPlacement = CardPlacement.shuffle;
    public AbilitySO scriptableData;
    public override void ActivateOnTargets(CastTable table, Mob[] targets, float animdelay)
    {
        var card = new PropertyCard(table.caster, scriptableData);
        switch (cardPlacement)
        {
            case CardPlacement.shuffle:
                DeckController.main.ShuffleCard(card);
                break;
            case CardPlacement.inhand:
                PlayerHandController.main.PutCardInHand(card);
                break;
        }

    }
    }
