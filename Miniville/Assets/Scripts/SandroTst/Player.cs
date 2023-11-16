using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private int coins;
    public int Coins {
        get { return coins; }
        set {
            if(value < 0) coins = 0;
            else coins = value;
        }
    }
    public Dictionary<CardName, int> PileCards = new Dictionary<CardName, int>();
    public Dictionary<MonumentName, bool> PileMonuments = new Dictionary<MonumentName, bool>();

    Player()
    {
        Coins = 3;
        //Rempli le dico et met à 1 WheatFields et Bkery
        foreach(CardName name in AllCards.allCards.Keys)
        {
            PileCards.Add(name, 0);
        }
        PileCards[CardName.WheatFields] = 1;
        PileCards[CardName.Bakery] = 1;
        //ajouter tous les monuments désactivé
        foreach (MonumentName name in AllCards.MonumentsData.Keys)
        {
            PileMonuments.Add(name, false);
        }
    }

    public bool TryBuyCard(CardName who)
    {
        if(coins >= AllCards.allCards[who].cardData.Cost)
        {
            PileCards[who]++;
            Coins -= AllCards.allCards[who].cardData.Cost;
            return true;
        }
        return false;
    }

    public bool TryBuyMonument(MonumentName who)
    {
        if (coins >= AllCards.MonumentsData[who].Cost)
        {
            PileMonuments[who] = true;
            Coins -= AllCards.MonumentsData[who].Cost;
            return true;
        }
        return false;
    }


}
