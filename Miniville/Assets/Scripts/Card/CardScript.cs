using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class CardScript
{
    public CardData cardData;
    public int index;

    public CardScript(CardData _cardData)
    {
        cardData = _cardData;
    }
    public virtual int Action()
    {
        return 0;
    }
}

public class TakeCoins : CardScript
{
    
    public int income;

    public TakeCoins(CardData _cardData, int _income) : base(_cardData)
    {
        this.income = _income;
        this.cardData = _cardData;
    }

    public override int Action()
    {
        return income;
    }
}

public class CoinsFromType : CardScript
{
    int income;
    CardType cardType;

    public CoinsFromType(CardData _cardData, int _income, CardType type) : base(_cardData)
    {
        this.cardData = _cardData;
        this.income = _income;
        this.cardType = type;
    }

    public override int Action()
    {
        int numberOfTypeCard = 0;
        foreach (CardName name in Game.instance.players[this.index].PileCards.Keys) //pour toutes les cartes
        {
            if (AllCards.CardsData[name].type == cardType)//si c'est une carte avec le bon type
            {
                numberOfTypeCard += Game.instance.players[index].PileCards[name];
                Debug.Log("Joueur" + (index + 1) + " à " + Game.instance.players[index].PileCards[name] + " " + name);
            }

        }
        
        Debug.Log(string.Format("J'ajoute {0} coins à joueur {1}", income * numberOfTypeCard, index));
        //verifier le nombre de carte du type cardType et multiplier income par ça
        return income * numberOfTypeCard;
    }
}