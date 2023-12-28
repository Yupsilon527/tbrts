using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckController : MonoBehaviour
{
    public static DeckController main;
    private void Awake()
    {
        main = this;
    }
    CardList<PropertyCard> Deck = new CardList<PropertyCard>();
    public void InitializeDeck()
    {
        Deck.Clear();
    }
    public void RestartDeck()
    {
        Deck.AddRange(DiscardPile);
        DiscardPile.Clear();
        ShuffleDeck();
    }
    #region Shuffle
    public void ShuffleDeck()
    {
        PropertyCard[] tempDeck = Deck.ToArray();

        Deck.Clear();

        if (tempDeck.Length > 0)
        {

            foreach (PropertyCard card in tempDeck)
            {
                ShuffleCard( card);
            }
        }
    }
    public void ShuffleCard(PropertyCard card)
    {
        Deck.Insert(Random.Range(0, Deck.Count - 1), card);
    }
    #endregion
    public int GetDeckCards()
    {
        return Deck.Count;
    }
    /*public cardtype[] GetCardsByType<cardtype>() where cardtype : CardData
    {
        if (!typeof(cardtype).IsAssignableFrom(typeof(CardData)))
            return null;
        List<cardtype> validCards = new List<cardtype>();
       
            foreach (CardData card in Deck)
            {
                if (card.GetType() == typeof(cardtype))
                {
                    validCards.Add((cardtype)card);
                }
            
        }
        return validCards.ToArray();
    }*/
    public void AddCard(PropertyCard card)
    {
        Deck.Add(card);
    }
    public void RemoveCard(PropertyCard card)
    {
        if (Deck.Contains(card))
            Deck.Remove(card); 
    }
    public PropertyCard GetTopCard()
    {
        if (Deck.Count > 0)
        {
            return Deck[0];
        }
        return null;
    }
    #region Discard Pile
    CardList<PropertyCard> DiscardPile = new CardList<PropertyCard>();
    public void DiscardCard(PropertyCard card, bool forced = false)
    {
       //TODO temp cards if (forced || card.number != CardDefines.CardNumbers.Joker)
        {
            DiscardPile.Add(card);
        }
    }
    public void RemoveCardsByHero(Hero h)
    {
        Deck.RemoveAll((PropertyCard card) => { return card.caster == h; });
        DiscardPile.RemoveAll((PropertyCard card) => { return card.caster == h; });
    }

    #endregion
}
