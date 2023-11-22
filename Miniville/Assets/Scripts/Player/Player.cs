using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour 
{
    private int coins;


    [Header("cardDisplay")]
    [SerializeField] GameObject cardPrefab;

    [SerializeField] Transform cardContent;
    public float xOffSet = 1.5f;
    public float yOffSet = 2f;

    public float cardSizeMultiplier = 1f;

    int x = 0;
    int z = 0;

    public int cardPerRow = 5;


    public int Coins {
        get { return coins; }
        set {
            if(value < 0) coins = 0;
            else coins = value;
        }
    }
    public Dictionary<CardName, int> PileCards = new Dictionary<CardName, int>();
    public Dictionary<MonumentName, bool> PileMonuments = new Dictionary<MonumentName, bool>();

    Dictionary<CardName, GameObject> cardObjects = new Dictionary<CardName, GameObject>();

    public void Start()
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

        ReloadCard();
    }

    public bool TryBuyCard(CardName who)
    {
        if(coins >= AllCards.allCards[who].cardData.Cost)
        {
            GiveCard(who);
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

    public void PaidOtherPlayer(Player other, int price)
    {
        if(Coins <= 0)
        {
            return;
        }
        else if(Coins - price <= 0)
        {
            other.Coins += this.Coins;
            this.Coins -= price;
        }
        else
        {
            other.Coins += price;
            this.Coins -= price;
        }
    }
    
    public void GiveCard(CardName who)
    {
        PileCards[who]++;
    }

    void ReloadCard()
    {
        List<CardName> cards = PileCards.Keys.ToList<CardName>();

        for (int i = 0; i < cards.Count; i++)
        {
            if (PileCards[cards[i]] != 0)
            {
                if (!cardObjects.ContainsKey(cards[i]))
                {
                    GameObject card = Instantiate(cardPrefab, cardContent);
                    card.transform.localScale *= cardSizeMultiplier;
                    card.transform.position += cardContent.right * (x % cardPerRow) * xOffSet * cardSizeMultiplier + cardContent.forward * ((int)(z / cardPerRow)) * yOffSet * cardSizeMultiplier;
                    card.transform.rotation = Quaternion.identity;

                    card.GetComponent<CardDisplayData>().CardName = cards[i];

                    ChangeMaterial(AllCards.CardsData[cards[i]].material, card);

                    cardObjects[cards[i]] = card;
                    x++;
                    z++;
                }
                else
                {
                    cardObjects[cards[i]].transform.position += cardContent.right * (x % cardPerRow) * xOffSet * cardSizeMultiplier + cardContent.forward * ((int)(z / cardPerRow)) * yOffSet * cardSizeMultiplier;
                }


            }
            else if (cardObjects.ContainsKey(cards[i]))
            {
                Destroy(cardObjects[cards[i]]);
                cardObjects.Remove(cards[i]);
                x--;
                z--;
            }
        }
    }
    void ChangeMaterial(Material material, GameObject go)
    {
        Material[] materialsArray = go.GetComponent<MeshRenderer>().materials;
        materialsArray[2] = material;
        go.GetComponent<MeshRenderer>().materials = materialsArray;
    }

}
