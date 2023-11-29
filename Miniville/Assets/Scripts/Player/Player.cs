using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Player : MonoBehaviour 
{
    [SerializeField] private int coins;

    [SerializeField] int coinsAtStart = 3;
    [SerializeField] TextMeshProUGUI coinDisplayer;
    [SerializeField] TextMeshProUGUI nbMonumentDisplayer;

    [SerializeField] public Button roll1DiceButton;
    [SerializeField] public Button roll2DiceButton;
    [SerializeField] public Button purchaseCardButton;
    [SerializeField] public Button nextRoundButton;

    public GameObject Ui;
    public Image UiImage;
    public Button UIButtonStealMoney;

    public bool hasStation = false;

    int x = 0;
    int z = 0;


    [Header("cardDisplay")]
    [SerializeField] GameObject cardPrefab;

    public Transform cardContent;
    public float xOffSet = 1.5f;
    public float yOffSet = 2f;

    public float cardSizeMultiplier = 1f;
    public float monumentSizeMultiplier = 1f;

    public float xMonumentPreOffSet = 1.5f;

    public int cardPerRow = 5;

    [Header("coinDisplay")]
    public GameObject coinInstantiatePos;

    public GameObject diceThrowingPos;


    public int Coins {
        get { return coins; }
        set {
            if(value < 0) coins = 0;
            else coins = value;

            coinDisplayer.text = coins.ToString();
            UpdateDisplayedCoins();
        }
    }
    public Dictionary<CardName, int> PileCards = new Dictionary<CardName, int>();
    public Dictionary<MonumentName, bool> PileMonuments = new Dictionary<MonumentName, bool>();

    public Dictionary<CardName, GameObject> cardObjects = new Dictionary<CardName, GameObject>();
    Dictionary<MonumentName, GameObject> MonumentsObjects = new Dictionary<MonumentName, GameObject>();

    public bool firstThrow = true;
    public bool replay = false;

    public void Start()
    {
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
        
        LoadCard();

        Coins = coinsAtStart;
    }

    public bool TryBuyCard(CardName who)
    {
        if (AllCards.CardsData[who].color == CardColor.Purple && PileCards[who] > 0) //si c'est une carte violette et que tu la déjà return fale (on peut avoir chaque exemplaire de carte violette en 1fois max)
            return false;
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
            if (who == MonumentName.Station)
            {
                hasStation = true;
                roll2DiceButton.gameObject.SetActive(true);
            }
            nbMonumentDisplayer.text = (int.Parse(nbMonumentDisplayer.text) + 1).ToString();
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

    public void LoadCard()
    {
        List<CardName> cards = PileCards.Keys.ToList<CardName>();
        List<MonumentName> monuments = PileMonuments.Keys.ToList<MonumentName>();

        x = 0;
        z = 0;

        for (int i = 0; i < cards.Count; i++)
        {
            if (PileCards[cards[i]] != 0)
            {
                if (!cardObjects.ContainsKey(cards[i]))
                {

                    GameObject card = Instantiate(cardPrefab, cardContent);
                    card.transform.localScale *= cardSizeMultiplier;
                    card.transform.localPosition = NextPlaceOnDeck();
                    card.transform.rotation = Quaternion.identity;

                    CardDisplayData data = card.GetComponent<CardDisplayData>();

                    data.cardName = cards[i];
                    data.size = cardSizeMultiplier;
                    data.player = this;
                    data.cardType = AllCards.CardsData[cards[i]].type;
                    data.x = x % cardPerRow * xOffSet * cardSizeMultiplier;
                    data.y = z * yOffSet * cardSizeMultiplier;

                    ChangeMaterial(AllCards.CardsData[cards[i]].material, card, 2);

                    cardObjects[cards[i]] = card;

                    x++;
                    if (x % cardPerRow == 0)
                        z++;
                }
                else
                {
                    cardObjects[cards[i]].transform.localPosition = NextPlaceOnDeck();
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

        int x1 = 0;
        for (int i = 0; i < monuments.Count; i++)
        {
            if (!MonumentsObjects.ContainsKey(monuments[i]))
            {
                GameObject monument = Instantiate(cardPrefab, cardContent);
                monument.transform.localScale *= cardSizeMultiplier;
                monument.transform.localPosition = Vector3.right * ((x1 % 4) + xMonumentPreOffSet) * xOffSet * monumentSizeMultiplier + Vector3.forward * (z + 1) * yOffSet * monumentSizeMultiplier;
                monument.transform.rotation = Quaternion.identity;
                monument.transform.Rotate(Vector3.forward, 180);

                CardDisplayData data = monument.GetComponent<CardDisplayData>();
                data.monumentName = monuments[i];
                data.size = monumentSizeMultiplier;
                data.player = this;
                data.x = x1 % cardPerRow * xOffSet * cardSizeMultiplier;
                data.y = z * yOffSet * cardSizeMultiplier;

                ChangeMaterial(AllCards.MonumentsData[monuments[i]].tailMaterial, monument, 1);
                ChangeMaterial(AllCards.MonumentsData[monuments[i]].headMaterial, monument, 2);

                MonumentsObjects[monuments[i]] = monument;
                x1++;
            }
            else
            {
                MonumentsObjects[monuments[i]].transform.localPosition = Vector3.right * ((x1 % 4) + xMonumentPreOffSet) * xOffSet * monumentSizeMultiplier + Vector3.forward * (z + 1) * yOffSet * monumentSizeMultiplier;
            }
        }
    }

    public void ReloadCard()
    {
        List<CardName> cards = PileCards.Keys.ToList<CardName>();
        List<MonumentName> monuments = PileMonuments.Keys.ToList<MonumentName>();

        x = 0;
        z = 0;

        for (int i = 0; i < cards.Count; i++)
        {
            if (NbCard(cards[i]) > 0)
            {
                Debug.Log(cards[i]);
                cardObjects[cards[i]].transform.localPosition = NextPlaceOnDeck();
                cardObjects[cards[i]].GetComponent<CardDisplayData>().x = x % cardPerRow * xOffSet * cardSizeMultiplier;
                cardObjects[cards[i]].GetComponent<CardDisplayData>().y = z * yOffSet * cardSizeMultiplier;
                x++;
                if (x % cardPerRow == 0)
                    z++;
            }
            else if (cardObjects.ContainsKey(cards[i]))
            {
                Debug.Log("hjvkgcbi");
                Destroy(cardObjects[cards[i]]);
                cardObjects.Remove(cards[i]);
                x--;
            }
        }

        int x1 = 0;
        for (int i = 0; i < monuments.Count; i++)
        {
            MonumentsObjects[monuments[i]].transform.localPosition = Vector3.right * ((x1 % 4) + xMonumentPreOffSet) * xOffSet * monumentSizeMultiplier + Vector3.forward * (z + 1) * yOffSet * monumentSizeMultiplier;
            MonumentsObjects[monuments[i]].GetComponent<CardDisplayData>().x = x1 % cardPerRow * xOffSet * cardSizeMultiplier;
            MonumentsObjects[monuments[i]].GetComponent<CardDisplayData>().y = z * yOffSet * cardSizeMultiplier;
            x1++;
        }
    }

    void ChangeMaterial(Material material, GameObject go, int idMaterial)
    {
        Material[] materialsArray = go.GetComponent<MeshRenderer>().materials;
        materialsArray[idMaterial] = material;
        go.GetComponent<MeshRenderer>().materials = materialsArray;
    }

    Vector3 NextPlaceOnDeck()
    {
        return Vector3.right * (x % cardPerRow) * xOffSet * cardSizeMultiplier + Vector3.forward * z * yOffSet * cardSizeMultiplier;
    }

    public Vector3 AddToDict(GameObject card)
    {
        CardDisplayData cardData = card.GetComponent<CardDisplayData>();
        if (!cardObjects.ContainsKey(cardData.cardName))
        {
            cardObjects[cardData.cardName] = card;
            cardData.x = x % cardPerRow * xOffSet * cardSizeMultiplier;
            cardData.y = z * yOffSet * cardSizeMultiplier;
            return NextPlaceOnDeck();
        }
        else
        {
            return new Vector3(cardObjects[cardData.cardName].GetComponent<CardDisplayData>().x , 0, cardObjects[cardData.cardName].GetComponent<CardDisplayData>().y);
        }
    }

    public bool PlayerHasCard(GameObject go)
    {
        foreach(CardName name in cardObjects.Keys.ToList<CardName>())
        {
            if (cardObjects[name] == go)
                return true;
        }
        return false;
    }

    public int NbCard(CardName cardName)
    {
        return PileCards[cardName];
    }


    #region DisplayCoins

    [Header("Display Coins")]
    int sumDiplayedCoins = 0;

    [SerializeField] GameObject Coins1;
    [SerializeField] GameObject Coins5;
    [SerializeField] GameObject Coins10;

    List<GameObject> displayedCoins = new List<GameObject>();


    [SerializeField] float ejectMaxSpeed = 500f;
    [SerializeField] float ejectMinSpeed = 200f;

    void UpdateDisplayedCoins()
    {
        if (Coins > sumDiplayedCoins)
        {
            InstantiateCoins(Coins - sumDiplayedCoins);
            sumDiplayedCoins = Coins;
        }

        if (Coins < sumDiplayedCoins)
        {
            RemoveCoins(sumDiplayedCoins - Coins);
            sumDiplayedCoins = Coins;
        }
    }

    private void InstantiateCoins(int nbCoins)
    {
        int CoinsToInstantiate = nbCoins;
        bool instantiated = false;
        int nbOfCoins1 = 0;

        while (CoinsToInstantiate >= 10)
        {
            displayedCoins.Add(Instantiate(Coins10, coinInstantiatePos.transform));
            CoinsToInstantiate -= 10;
        }

        while (CoinsToInstantiate >= 5)
        {
            instantiated = false;
            foreach (GameObject Coins in displayedCoins)
            {
                if (Coins.name == "Coins_5(Clone)")
                {
                    RemoveCoins(5);
                    InstantiateCoins(10);
                    CoinsToInstantiate -= 5;
                    instantiated = true;
                    break;
                }
            }
            if (!instantiated)
            {
                displayedCoins.Add(Instantiate(Coins5, coinInstantiatePos.transform));
                CoinsToInstantiate -= 5;
            }
        }

        while (CoinsToInstantiate >= 1)
        {
            nbOfCoins1 = 0;
            instantiated = false;
            foreach (GameObject Coins in displayedCoins)
            {
                if (Coins.name == "Coins_1(Clone)")
                    nbOfCoins1++;
            }

            if (nbOfCoins1 == 4)
            {
                RemoveCoins(1);
                RemoveCoins(1);
                RemoveCoins(1);
                RemoveCoins(1);

                InstantiateCoins(5);
                CoinsToInstantiate -= 1;
            }
            else
            {
                displayedCoins.Add(Instantiate(Coins1, coinInstantiatePos.transform));
                CoinsToInstantiate -= 1;
            }
        }
    }

    private void RemoveCoins(int nbCoins)
    {
        int nbCoinsToRemove = nbCoins;
        List<GameObject> CoinsToRemove = new List<GameObject>();
        int CoinsToAdd = 0;

        // ENLEVE LES PIECES DE 10
        for (int i = nbCoinsToRemove; i >= 10; i -= 10)
        {
            foreach (GameObject Coins in displayedCoins)
            {
                if (Coins.name == "Coins_10(Clone)")
                {
                    CoinsToRemove.Add(Coins);
                    displayedCoins.Remove(Coins);
                    nbCoinsToRemove -= 10;
                    break;
                }
            }
        }

        // ENLEVE LES PIECES DE 5
        for (int i = nbCoinsToRemove; i >= 5; i -= 5)
        {
            bool CoinsRemoved = false;

            foreach (GameObject Coins in displayedCoins)
            {
                if (Coins.name == "Coins_5(Clone)")
                {
                    CoinsToRemove.Add(Coins);
                    displayedCoins.Remove(Coins);
                    nbCoinsToRemove -= 5;
                    CoinsRemoved = true;
                    break;
                }
            }

            if (!CoinsRemoved)
            {
                foreach (GameObject Coins in displayedCoins)
                {
                    if (Coins.name == "Coins_10(Clone)")
                    {
                        CoinsToRemove.Add(Coins);
                        displayedCoins.Remove(Coins);
                        CoinsToAdd += 5;
                        nbCoinsToRemove -= 5;
                        CoinsRemoved = true;
                        break;
                    }
                }
            }
        }

        // ENLEVE LES PIECES DE 1
        for (int i = nbCoinsToRemove; i >= 1; i -= 1)
        {
            bool CoinsRemoved = false;

            foreach (GameObject Coins in displayedCoins)
            {
                if (Coins.name == "Coins_1(Clone)")
                {
                    CoinsToRemove.Add(Coins);
                    displayedCoins.Remove(Coins);
                    nbCoinsToRemove -= 1;
                    CoinsRemoved = true;
                    break;
                }
            }

            if (!CoinsRemoved)
            {
                foreach (GameObject Coins in displayedCoins)
                {
                    if (Coins.name == "Coins_5(Clone)")
                    {
                        CoinsToRemove.Add(Coins);
                        displayedCoins.Remove(Coins);
                        CoinsToAdd += 4;
                        nbCoinsToRemove -= 1;
                        CoinsRemoved = true;
                        break;
                    }
                }

                if (!CoinsRemoved)
                {
                    foreach (GameObject Coins in displayedCoins)
                    {
                        if (Coins.name == "Coins_10(Clone)")
                        {
                            CoinsToRemove.Add(Coins);
                            displayedCoins.Remove(Coins);
                            CoinsToAdd += 9;
                            nbCoinsToRemove -= 1;
                            CoinsRemoved = true;
                            break;
                        }
                    }
                }
            }
        }

        for (int i = 0; i < CoinsToRemove.Count; i++)
        {
            GameObject Coins = CoinsToRemove[i];

            Coins.GetComponent<MeshCollider>().enabled = false;
            Coins.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-10, 11), Random.Range(1, 11), Random.Range(-10, 11)).normalized * Random.Range(ejectMinSpeed, ejectMaxSpeed), ForceMode.Impulse);
            Coins.GetComponent<Rigidbody>().AddTorque(new Vector3(Random.Range(1, 11), Random.Range(1, 11), Random.Range(1, 11)).normalized * Random.Range(ejectMinSpeed, ejectMaxSpeed));

            StartCoroutine(DeleteDisplayedCoin(Coins));
        }

        if (CoinsToAdd > 0)
            InstantiateCoins(CoinsToAdd);
    }

    IEnumerator DeleteDisplayedCoin(GameObject Coins)
    {
        yield return new WaitForSeconds(1.0f);
        Destroy(Coins);
    }
    #endregion

}
