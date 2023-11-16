using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int pieces;

    public List<CardStack> cards = new List<CardStack>();

    public bool CanBuyCard(Card cardToBuy)
    {
        return cardToBuy.price < pieces;
    }

    public void BuyCard(Card cardToBuy)
    {
        foreach (CardStack stack in cards)
        {
            if (stack.card.Equals(cardToBuy))
            {
                stack.nbOfCard++;
            }
            break;
        }
        transform.find
    }
}
