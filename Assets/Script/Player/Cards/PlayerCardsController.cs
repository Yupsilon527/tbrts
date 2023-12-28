using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCardsController : MonoBehaviour
{
    #region Deck
    CardList<PropertyCard> Hand = new CardList<PropertyCard>();
    public void ShuffleCard(PropertyCard card)
    {
        DeckController.main.AddCard(card);
        if (Hand.Contains(card))
            Hand.Remove(card);
    }
    public PropertyCard[] GetCardsInHand()
    {
        return Hand.ToArray();
    }
    public void PutCardInHand(PropertyCard card)
    {
        if (card != null)
        {
            Hand.Add(card);
            DeckController.main.RemoveCard(card);
        }
    }
    public void DiscardHand()
    {
        foreach (PropertyCard card in Hand)
        {
            DeckController.main.DiscardCard(card);
        }
        Hand.Clear();
    }
    public void DiscardCardFromHand(PropertyCard card)
    {
        Hand.Remove(card);
        DeckController.main.DiscardCard(card);
    }
    public void RemoveCardFromHand(PropertyCard card)
    {
        Hand.Remove(card);
    }
    public bool HasCard(PropertyCard check)
    {
        return Hand.Contains(check);
    }
    #endregion
    #region Play
    CardList<PropertyCard> Play = new CardList<PropertyCard>();
    public void PlayCard(PropertyCard card)
    {
        Hand.Remove(card);
        Play.Add(card);
    }
    public void UnplayCard(PropertyCard card)
    {
        Play.Remove(card);
        Hand.Add(card);

    }
    public PropertyCard GetCardInPlay(int index)
    {
        if (index < Play.Count)
            return Play[index];
        return null;
    }
    public CardList<PropertyCard> GetCardsInPlay()
    {
        return Play;
    }
    public int NumCardsInPlay()
    {
        return Play.Count;
    }
    public void ClearCardsInPlay()
    {
        foreach (PropertyCard card in Play)
        {
            DeckController.main.DiscardCard(card);
        }
        Play.Clear();
    }
    public bool HasFolded()
    {
        return Play.Count == 0 || Hand.Count == 0;
    }
    #endregion
}
