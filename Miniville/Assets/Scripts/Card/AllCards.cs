
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllCards : MonoBehaviour
{
    [SerializeField] CardData[] allCardsData;
    public static Dictionary<CardName, CardData> CardsData = new Dictionary<CardName, CardData>();

    [SerializeField] MonumentData[] allmonumentsData;
    public static Dictionary<MonumentName, MonumentData> MonumentsData = new Dictionary<MonumentName, MonumentData>();

    public static Dictionary<CardName, CardScript> allCards = new Dictionary<CardName, CardScript>();

    private void Awake()
    {
        foreach (var card in allCardsData)
        {
            CardsData.Add(card.cardName, card);
        }
        foreach (var mon in allmonumentsData)
        {
            MonumentsData.Add(mon.monumentName, mon);
        }

        allCards.Add(CardName.WheatFields,new TakeCoins(CardsData[CardName.WheatFields], 1)); //Champ de blé 
        allCards.Add(CardName.Farm, new TakeCoins(CardsData[CardName.Farm], 1)); //ferme
        allCards.Add(CardName.Bakery, new TakeCoins(CardsData[CardName.Bakery], 1)); //Bakery
        allCards.Add(CardName.CoffeeShop, new TakeCoins(CardsData[CardName.CoffeeShop], 1)); //Café
        allCards.Add(CardName.Store, new TakeCoins(CardsData[CardName.Store], 3)); //supérette
        allCards.Add(CardName.Forest, new TakeCoins(CardsData[CardName.Forest], 1)); //Forest
        allCards.Add(CardName.Dairy, new CoinsFromType(CardsData[CardName.Dairy], 3, CardType.Animal)); //Fromagerie
        allCards.Add(CardName.FurnitureFactory, new CoinsFromType(CardsData[CardName.FurnitureFactory], 3, CardType.Gear)); //Frabique de meubles
        allCards.Add(CardName.Mine, new TakeCoins(CardsData[CardName.Mine], 5)); //Mine
        allCards.Add(CardName.Restaurant, new TakeCoins(CardsData[CardName.Restaurant], 2)); //Restaurant
        allCards.Add(CardName.Ochard, new TakeCoins(CardsData[CardName.Ochard], 3)); //Verger
        allCards.Add(CardName.VegetableStore, new CoinsFromType(CardsData[CardName.VegetableStore], 2, CardType.Hay)); //Restaurant




    }
}
