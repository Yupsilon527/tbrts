using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerResourceManager : MonoBehaviour
{
    [Header("Limits")]
    public int ManaLimit = 8;
    public int HandLimit = 5;
    [Header("Starting")]
    public float StartingGold = 400;
    public float DrawGoldCost = 100;
    public Resource gold;
    public Resource mana;

    private void Awake()
    {
        InitResources();
    }
    public void InitResources()
    {
        mana = new Resource(1, "Player Mana", false, true);
        gold = new Resource(0, "Player Gold", false, false);
        gold.OnValueChanged.AddListener(() =>
        {
            TryDrawCard();
        });
    }
    public void InitStart()
    {
        gold.SetValue(StartingGold); 
    }
    void TryDrawCard()
    {
        int draw = Mathf.FloorToInt(gold.GetValue() / DrawGoldCost);
        draw = Mathf.Min(draw, HandLimit - PlayerHandController.main.GetCardsInHand());

        if (draw > 0)
        {
            PlayerHandController.main.DrawMultipleCards(draw);
            gold.ChargeValue(draw * DrawGoldCost);
        }
    }
    public void IncreaseMana()
    {
        mana.SetLimit(Mathf.Min(ManaLimit, mana.GetLimit(false) + 1), Resource.LimitRule.fullheal_value, true);
    }
}
