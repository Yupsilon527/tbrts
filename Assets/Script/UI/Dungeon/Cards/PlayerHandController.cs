using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHandController : MonoBehaviour
{
	public static PlayerHandController main;
	public RectTransform rectTransform;
	public Transform deck;

	[HideInInspector] public List<CardController> myHand = new List<CardController>();
    #region Unity
    private void Awake()
	{
		main = this;
	}
	public bool isDraggingCard = false;
	public bool IsIdle()
	{
		return !isDraggingCard && drawMultipleCardsCoroutine == null;

	}
	#endregion
	#region Card Pooling
	public ObjectPool cardPool;
	public CardController PoolNewCard()
	{
		GameObject gameobjectCard = cardPool.PoolItem(CardDefines.CardPrefab);
		gameobjectCard.transform.SetParent(PlayerHandController.main.transform);
		CardController newCard = gameobjectCard.GetComponent<CardController>();
		newCard.Flip();
		newCard.SetHighlight(false);

		return newCard;
	}
	#endregion
	#region Discard Cards
	public void DiscardHand()
	{
		foreach (CardController card in myHand)
		{
			card.Drop();
		}
		myHand.Clear();
	}
	public void DiscardCard(CardController card, bool reset = true)
	{
		PlayerController.main.cards.DiscardCardFromHand(card.GetAssignedAbility());
		myHand.Remove(card);
		card.Drop();
		if (reset) ResetCardPosition(true, true);
	}
	public void DiscardCardsForHero(Hero hero)
    {
		for (int c = 0; c<myHand.Count; c++)
        {
			if (myHand[c]!=null && myHand[c].GetAssignedAbility().caster == hero)
            {
				DiscardCard(myHand[c], false);
				c--;

			}
        }
		ResetCardPosition(true, true); 
	}
	#endregion
	#region Draw Multiple
	Coroutine drawMultipleCardsCoroutine;
	int poolCards = 0;
	public void DrawMultipleCards(int nCards)
	{
		poolCards+= nCards;
		if (drawMultipleCardsCoroutine == null)
		{
			drawMultipleCardsCoroutine = StartCoroutine(DrawCardsOverTime());
		}
	}
	public IEnumerator DrawCardsOverTime()
	{
		while (poolCards > 0)//TODO player
		{
			PropertyCard topcard = DeckController.main.GetTopCard();
			if (topcard == null)
			{
				poolCards=0;
			}
			else
			{
				poolCards--;
				PutCardInHand(topcard);
				yield return new WaitForSeconds(CardDefines.CardDrawInterval);
			}
		}
		drawMultipleCardsCoroutine = null;
	}
	public void DrawMultipleCards(PropertyCard[] nCards)
	{
		if (drawMultipleCardsCoroutine == null)
		{
			drawMultipleCardsCoroutine = StartCoroutine(DrawCardsOverTime(nCards));
		}
	}
	public IEnumerator DrawCardsOverTime(PropertyCard[] nCards)
	{
		for (int I = 0; I< nCards.Length; I++)
		{
			if (!IsCardInHand(nCards[I]))
			{
				PutCardInHand(nCards[I]);
				yield return new WaitForSeconds(CardDefines.CardDrawInterval);
			}
		}
		drawMultipleCardsCoroutine = null;
	}
	CardController DrawCard(PropertyCard data)
	{
		Vector3 origin = deck.position;

		CardController card = PoolNewCard();
		card.transform.rotation = Quaternion.Euler(180, 0, 0);
		card.GiveOrder(new CardController.CardMoveOrder(card, 0, origin, 1, Vector3.right * 180, true));
		card.LoadAbilityData(data);

		return card;
	}
    public void PutCardInHand(PropertyCard data)
    {
        CardController card = DrawCard(data);
        myHand.Add(card);

        PlayerController.main.cards.PutCardInHand(data);
        ResetCardPosition(true, true);
    }
    #endregion
    #region Player Hand
	public void UpdatePlayerHand()
    {
		for (int ic = 0; ic< myHand.Count;ic++)
        {
            CardController card = myHand[ic];
			if (!PlayerController.main.cards.HasCard(card.GetAssignedAbility()))
			{
				DiscardCard(card, false);
				ic--;
			}
		}
		DrawMultipleCards(PlayerController.main.cards.GetCardsInHand());
    }
    public bool IsCardInHand(PropertyCard data)
	{
		foreach (var card in myHand)
		{
			if (card.GetAssignedAbility() == data)
				return true;
		}
		return false;
	}
	public int GetCardsInHand()
    {
		return myHand.Count + poolCards;
    }
    #endregion
    #region Selection
    public CardController SelectedCard;
	public float SelectionScale = 1.25f;
	public void SelectCard(CardController card)
	{
		if (IsIdle() && card.isIdle())
		{
			DeselectCard();
			SelectedCard = card;
			card.GiveOrder(new CardController.CardMoveOrder(card, .2f, card.transform.localPosition, SelectionScale, card.transform.rotation.eulerAngles, true));
			card.zIndex = card.transform.GetSiblingIndex();
			card.transform.SetAsLastSibling();
		}
	}
	public void DeselectCard()
    {
		if (SelectedCard == null)
			return;
		SelectedCard.GiveOrder(new CardController.CardMoveOrder(SelectedCard, .2f, SelectedCard.transform.localPosition, 1, SelectedCard.transform.rotation.eulerAngles, true));
		SelectedCard.transform.SetSiblingIndex(SelectedCard.zIndex);

	}
    #endregion
    #region Play
    public void PlayCard(CardController card, Vector2 poitonerPosition)//todo ACCOUNT ABILITY TYPE
	{
        PropertyCard castingAbility = card.GetAssignedAbility();
        Mob casterMob = castingAbility.caster;

        Vector3 castPoint = CameraController.main.camera.ScreenToWorldPoint(poitonerPosition);
        Mob targetMob = CameraController.main.MobFromScreenPointForAbility(poitonerPosition,castingAbility);


		CastTable castData = null;
		
		if (targetMob != null && castingAbility.CanCastOnTarget(targetMob))

		{
			castData = casterMob.abilities.CastAbilityOnTarget(castingAbility, targetMob);

		}
		else if (castingAbility.RequiresUnitTarget())
		{
			return;
		}
		else if (castingAbility.CanCastOnPoint(castPoint))
		{
			castData = casterMob.abilities.CastAbilityOnPoint(castingAbility, castPoint);
			
		}
		if (castData!=null && casterMob.abilities.ResolveCastData(castData))
        {
			DiscardCard(card);
		}
		

		/*if (!card.IsInPlay() && !myPlay.Contains(card) && myPlay.Count < 5)//TODO define
		{
			myHand.Remove(card);
			myPlay.Add(card);
		}*/
	}
	#endregion

	#region Card Positions
	public void ResetCardPosition(bool sort, bool Force)
	{
		RepositionCardsInHand(sort, Force);

	}
	public void SortHand()
	{
		myHand.Sort((CardController cA, CardController cB) =>
		{
			return cA.rectTransform.position.x.CompareTo(cB.rectTransform.position.x);
		});
	}
	public void RepositionCardsInHand(bool sort = true, bool Force = false)
	{
	if (sort    )SortHand();

		Vector2 vSize = new Vector2( 180,70);	//TODO define
		foreach (CardController cData in myHand)
		{
			if (cData.isActiveAndEnabled && (cData.isIdle() || Force))
			{
				float index =  myHand.IndexOf(cData) - myHand.Count;// - (myHand.Count - 1f) / 2f;

				cData.transform.SetSiblingIndex(myHand.IndexOf(cData));

				CardController.CardMoveOrder cardOrder = new CardController.CardMoveOrder(cData, 1f,	Vector3.zero, 1, new Vector3(0, 0, -5 * index / myHand.Count), Force);
				cardOrder.Pos = new Vector2((index) * vSize.x + rectTransform.sizeDelta.x*.5f, -Mathf.Abs(index * vSize.y * .1f));

				cData.GiveOrder(cardOrder);
			}
		}
	}
	#endregion
}