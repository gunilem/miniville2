using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CardScript
{
    public CardData cardData;

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
    
    int income;

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
        this.cardData.type = type;
    }

    public override int Action()
    {
        //verifier le nombre de carte du type cardType et multiplier income par ça
        return income;
    }
}