using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    public static Game instance;

    [SerializeField] GameObject rerollUi;
    [SerializeField] public GameObject tradingCardUI1;
    [SerializeField] public GameObject tradingCardUI2;
    [SerializeField] public GameObject tradingMoneyUI;

    Color32 selectedColor = new Color32(138, 142, 163, 255);

    public bool isTrading;
    public bool isStealingMoney;

    public bool playerMadeChoice = false;
    public Player playerToSteal;

    bool rerollDice = false;

    int DiceThrown;


    [Header("Data")]
    [SerializeField] Dice[] dices = new Dice[2];

    public List<Player> players = new List<Player>();
    public int currentPlayerIndex;
    [SerializeField]int currentDiceResult = 11;
    int playerWhosWin;
    public Dictionary<CardName, int> PileCards = new Dictionary<CardName, int>();
    Dictionary<CardName, GameObject> cardObjects = new Dictionary<CardName, GameObject>();

    [Header("Settings")]
    [Range(1, 4)]public int numberOfPlayers = 1;

    [Header("cardDisplay")]
    [SerializeField] GameObject cardPrefab;
    [SerializeField] Transform cardContent;
    public float xOffSet = 1.5f;
    public float yOffSet = 2f;

    public float cardSizeMultiplier = 1f;

    int x = 0;
    int z = 0;

    public int cardPerRow = 5;

    public bool ToNextState = false;

    public bool isPurchasing = false;

    [Header("Dice Debug")]
    [SerializeField] bool useTrickDiceResult;
    [SerializeField] int trickDiceResult;

    delegate void del(); del state;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("Il y a trop de GameManager");
            return;
        }
        instance = this;
    }
    void Start()
    {
        FillPile();
        
        currentPlayerIndex = 0;
        

        for (int i = 0; i < numberOfPlayers; i++)
        {
            players[i].Ui.SetActive(true);
            players[i].UIButtonStealMoney.interactable = false;
        }

        players[currentPlayerIndex].UiImage.color = new Color32(255, 255, 255, 255);

        ReloadCard();


        state = PlayerTrowDices;
    }
    void Update()
    {
        /*Sandro DEBUG
        if (Input.GetKeyDown(KeyCode.D))
        {
            
            players[1].PileCards[CardName.CoffeeShop] = 1;
            players[1].PileMonuments[MonumentName.Mall] = true;
            Debug.Log(string.Format("Le joueur 2 possède {0} cofee shop et possède le Mall est {1}", players[1].PileCards[CardName.CoffeeShop], players[1].PileMonuments[MonumentName.Mall]));
        }Sandro DEBUG */
        state();
    }

    private void FillPile()
    {
        foreach (CardData data in AllCards.CardsData.Values)
        {
            if (data.color == CardColor.Purple) //si c'est une carte violet tu met 4 de chaque dans la pille
                PileCards.Add(data.cardName, 4);
            else //sinon c'est 6 de chaque
                PileCards.Add(data.cardName, 6);
        }
    }


    public void PlayerTrowDices()
    {
        

        bool playerHasStation = players[currentPlayerIndex].PileMonuments[MonumentName.Station];
        if (playerHasStation) players[currentPlayerIndex].roll2DiceButton.interactable = true;
        players[currentPlayerIndex].roll1DiceButton.interactable = true;
    }

    public void ThrowDice(int nbOFDice)
    {
        if (nbOFDice == 1)
        {
            dices[0].ResetDice(players[currentPlayerIndex].diceThrowingPos.transform.position);
            dices[1].ResetDice(dices[1].DicePosAtBegin);
            dices[0].TrowDice();
        }
        else
        {
            dices[0].ResetDice(players[currentPlayerIndex].diceThrowingPos.transform.position);
            dices[0].TrowDice();
            dices[1].ResetDice(players[currentPlayerIndex].diceThrowingPos.transform.position);
            dices[1].TrowDice();
        }
        players[currentPlayerIndex].roll1DiceButton.interactable = false;
        players[currentPlayerIndex].roll2DiceButton.interactable = false;
        DiceThrown = nbOFDice;
        state = WaitForDiceResult;
    }

    public void WaitForDiceResult()
    {
        int _result = 0;
        bool _allDicesHaveAResult = true;
        for (int i = 0; i < DiceThrown; i++)
        {
            int _diceResult = dices[i].result;
            if (_diceResult != -1)
            {
                _result += _diceResult;
            }
            else
            {
                _allDicesHaveAResult = false;
            }
        }

        if (_allDicesHaveAResult)
        {
            if (useTrickDiceResult)
                currentDiceResult = trickDiceResult;
            else
                currentDiceResult = _result;
            
            if (players[currentPlayerIndex].PileMonuments[MonumentName.AmusementPark] && DiceThrown > 1 && dices[0].result == dices[1].result) //si ta le parc d'attraction et que ta fait un double tu met replay true pour rejouer au prochain tour
                players[currentPlayerIndex].replay = true;
            if (players[currentPlayerIndex].PileMonuments[MonumentName.RadioTower] && players[currentPlayerIndex].firstThrow) //si le joueur à la tour radio et que c'est son premier lancer, demander s'il veux relancer
            {
                players[currentPlayerIndex].firstThrow = false;
                rerollUi.SetActive(true);
                state = WaitForRerollDice;
            }
            else state = PaidOtherPlayers;
        }
    }

    public void WaitForRerollDice()
    {
        if (playerMadeChoice)
        {
            if (rerollDice)
            {
                state = PlayerTrowDices;
                rerollDice = false;
            }
            else
            {
                state = PaidOtherPlayers;
            }
            rerollUi.SetActive(false);

            playerMadeChoice = false;
        }
    }

    public void SetRerollDices(bool choice)
    {
        playerMadeChoice = true;
        rerollDice = choice;
    }

    public void PaidOtherPlayers()
    {
        for(int i = 0; i < numberOfPlayers; i++)
        {
            if (i != currentPlayerIndex)
            {
                foreach (CardName name in AllCards.CardsData.Keys)
                {
                    //si c'est un carte rouge, qu'elle a le bon numéro et que le joueur la possède
                    if (AllCards.CardsData[name].color == CardColor.Red && players[i].PileCards[name] > 0 && AllCards.HaveTheRightDice(name, currentDiceResult)) 
                    {
                        int amusementParkExtra = 0;
                        if (players[i].PileMonuments[MonumentName.Mall]) amusementParkExtra = 1; //verifier que le joueur à qui on doit de l'argent à le centre commercial car si c'est le cas c'est café(donc carte rouge) coute +1
                        Debug.Log(string.Format("Player{0} donen argent à Player{1} grâce à ces {2}{3}", currentPlayerIndex + 1, i + 1, players[i].PileCards[name], name));
                        for (int y = 0; y < players[i].PileCards[name]; y++)
                        {
                            players[currentPlayerIndex].PaidOtherPlayer(players[i], AllCards.allCards[name].Action() + amusementParkExtra);
                        }
                    }

                }
            }
        }

        state = PlayersReceivesMoney;
    }


    public void PlayersReceivesMoney()
    {
        if(currentDiceResult == 6) //si le dice = 6 ça active seulement les cartes violettes donc on passe dans le state PurpleCard
        {
            state = PurpleCardAction;  
            return;
        }

        for (int i = 0; i < numberOfPlayers; i++)
        {
            if (i == currentPlayerIndex) //si c'est le joueur dont c'est le tour
            {
                
                foreach (CardName name in AllCards.CardsData.Keys)
                {
                    if (name != CardName.None)
                    {
                        //si c'est pas une carte rouge, qu'elle a le bon numéro et que le joueur la possède
                        if (!(AllCards.CardsData[name].color == CardColor.Red) && players[i].PileCards[name] > 0 && AllCards.HaveTheRightDice(name, currentDiceResult))
                        {
                            int amusementParkExtra = 0;
                            if (players[i].PileMonuments[MonumentName.Mall] && AllCards.CardsData[name].type == CardType.Store) amusementParkExtra = 1; //si le joueur à le centre commercial ces cartes de type store augmente leur rente de 1

                            for (int y = 0; y < players[i].PileCards[name]; y++)
                            {
                                AllCards.allCards[name].index = i;
                                players[i].Coins += AllCards.allCards[name].Action() + amusementParkExtra;

                            }
                        }
                    }

                }
            }
            else //si c'est un joueur dont c'est pas le tour
            {
                foreach (CardName name in AllCards.CardsData.Keys)
                {
                    //si c'est une carte bleu, qu'elle a le bon numéro et que le joueur la possède
                    if ( AllCards.CardsData[name].color == CardColor.Blue && players[i].PileCards[name] > 0 && AllCards.HaveTheRightDice(name, currentDiceResult))
                    {
                        for (int y = 0; y < players[i].PileCards[name]; y++)
                        {
                            AllCards.allCards[name].index = i;
                            players[i].Coins += AllCards.allCards[name].Action();
                        }
                    }

                }
            }
        }
        players[currentPlayerIndex].nextRoundButton.interactable = true;
        state = WaitForPlayerToSelectCard;

    }
    public void WaitForPlayerToSelectCard()
    {
        isPurchasing = true;

        if (ToNextState)
        {
            isPurchasing = false;
            players[currentPlayerIndex].nextRoundButton.interactable = false;
            ToNextState = false;
            state = CheckPlayerHasWon;
        }
    }
    public bool PlayerBuild(CardName cardPlayerWantToBuy)
    {
        var currentPlayer = players[currentPlayerIndex];

        bool hasBought = PileCards[cardPlayerWantToBuy] > 0 && currentPlayer.TryBuyCard(cardPlayerWantToBuy);
        return hasBought;
    }

    public bool PlayerBuildMonument(MonumentName cardPlayerWantToBuy)
    {
        var currentPlayer = players[currentPlayerIndex];

        bool hasBought = currentPlayer.TryBuyMonument(cardPlayerWantToBuy);
        return hasBought;
    }

    public void CheckPlayerHasWon()
    {
        CameraScript.instance.GoToOriginalPos();
        var currentPlayer = players[currentPlayerIndex];
        var hasWon = true;

        foreach (bool monu in currentPlayer.PileMonuments.Values)
        {
            if (!monu)
            {
                hasWon = false;
                break;
            }
        }

        if (hasWon)
        {
            state = EndGame;
            playerWhosWin = currentPlayerIndex;
        }
        else
        {
            if (!players[currentPlayerIndex].replay) //replay est true si le joueur avait fait un double et possedait le parc d'attraction
            {
                currentPlayerIndex++;
                players[(currentPlayerIndex - 1) % numberOfPlayers].UiImage.color = selectedColor;
                currentPlayerIndex = currentPlayerIndex % numberOfPlayers;
                players[currentPlayerIndex].UiImage.color = Color.white;
            }
            players[currentPlayerIndex].replay = false;
            players[currentPlayerIndex].firstThrow = true;
            state = PlayerTrowDices;
        }
    }
    public void EndGame()
    {
        Debug.Log("Fin du jeu");
    }

    private void PurpleCardAction()
    {
        Player currentPlayer = players[currentPlayerIndex];
        if (currentPlayer.PileCards[CardName.Stadium] > 0)
            StadiumAction();
        if (currentPlayer.PileCards[CardName.BusinessCenter] > 0)
        {
            tradingCardUI1.SetActive(true);
            isTrading = true;
            state = WaitToChooseTheCardToSteal; return;
        }
            
        else if (currentPlayer.PileCards[CardName.TVStation] > 0)
        {
            isStealingMoney = true;
            tradingMoneyUI.SetActive(true);

            players[currentPlayerIndex].UIButtonStealMoney.interactable = false;
            foreach (Player p in players)
            {
                if (p != players[currentPlayerIndex])
                {
                    p.UiImage.color = Color.white;
                    p.UIButtonStealMoney.interactable = true;
                }
            }

            state = WaitToChoosePlayerToStealMoney; return;
        }
        else
        {
            players[currentPlayerIndex].nextRoundButton.interactable = true;
            state = WaitForPlayerToSelectCard;
        }

    }

    private void StadiumAction() //chaque joueur donne 2 pièce au joueur qui à cette carte
    {
        for(int i = 0; i < numberOfPlayers; i++)
        {
            if(i != currentPlayerIndex)
            {
                players[i].PaidOtherPlayer(players[currentPlayerIndex], 2);
            }
        }
    }

    void WaitToChooseTheCardToSteal() //étape 1 Buisness center action
    {
        if (playerMadeChoice)
        {
            //ICI redéfinir cardToSteal et lier playerToSteal au joueur qui avait la carte
            playerMadeChoice = false;
            state = BuisnessCenterAction;
        }
    }

    private void BuisnessCenterAction()
    {
        int indexPlayerToStealCard = 0;
        for(int i = 0; i < numberOfPlayers; i++)
        {
            if (players[i] == playerToSteal)
                indexPlayerToStealCard = i;
        }
        players[currentPlayerIndex].ReloadCard();
        players[indexPlayerToStealCard].ReloadCard();
        
        isTrading = false;

        if (players[currentPlayerIndex].PileCards[CardName.TVStation] > 0)//si le joueur à la crate TVStation, tu demandes à qui il veux volr de la thunasse
        {
            isStealingMoney = true;
            tradingMoneyUI.SetActive(true);

            players[currentPlayerIndex].UIButtonStealMoney.interactable = false;
            foreach (Player p in players)
            {
                if (p != players[currentPlayerIndex])
                {
                    p.UiImage.color = Color.white;
                    p.UIButtonStealMoney.interactable = true;
                }
            }

            state = WaitToChoosePlayerToStealMoney;
        }
        else //sinon tu skipe directement à l'achat de nouvelle carte
        {
            players[currentPlayerIndex].nextRoundButton.interactable = true;
            state = WaitForPlayerToSelectCard;
        }

    }

    void WaitToChoosePlayerToStealMoney() //étape 1 TV station action
    {
        if (playerMadeChoice)
        {
            //Ici definir dans playerToSteal le joueur à qui voler 5 argent car on adore largen JE DETESTE LES STATES MACHINE DE TA MER
            playerMadeChoice = false;
            state = TVStationAction;
        }
    }

    private void TVStationAction() {
        int indexPlayerToStealCoins = 0;
        for (int i = 0; i < numberOfPlayers; i++)
        {
            if (players[i] == playerToSteal)
                indexPlayerToStealCoins = i;
        }
        players[indexPlayerToStealCoins].PaidOtherPlayer(players[currentPlayerIndex],5);
        players[currentPlayerIndex].nextRoundButton.interactable = true;
        isStealingMoney = false;
        tradingMoneyUI.SetActive(false);

        players[currentPlayerIndex].UIButtonStealMoney.interactable = false;
        foreach (Player p in players)
        {
            if (p != players[currentPlayerIndex])
            {
                p.UiImage.color = selectedColor;
            }
        }

        state = WaitForPlayerToSelectCard;
    }

    void ReloadCard()
    {
        List<CardName> cards = PileCards.Keys.ToList<CardName>();

        x = cardPerRow - 1;

        for (int i = 0; i < cards.Count; i++)
        {
            if (PileCards[cards[i]] != 0)
            {
                if (!cardObjects.ContainsKey(cards[i]))
                {
                    GameObject card = Instantiate(cardPrefab, cardContent);
                    card.transform.localScale *= cardSizeMultiplier;
                    card.transform.position += cardContent.right * (x % cardPerRow) * xOffSet * cardSizeMultiplier + cardContent.forward * ((int)(z / cardPerRow)) * yOffSet * cardSizeMultiplier;
                    
                    CardDisplayData data = card.GetComponent<CardDisplayData>();

                    data.cardName = cards[i];
                    data.cardType = AllCards.CardsData[cards[i]].type; 
                    data.x = x % cardPerRow * xOffSet * cardSizeMultiplier;
                    data.y = z / cardPerRow * yOffSet * cardSizeMultiplier;

                    ChangeMaterial(AllCards.CardsData[cards[i]].material, card);
                    data.size = cardSizeMultiplier;

                    cardObjects[cards[i]] = card;
                    x--;
                    if (x < 0)
                    {
                        x = cardPerRow - 1;
                    }
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
                x++;
                if (x > cardPerRow - 1)
                {
                    x = 0;
                }
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

    public bool PlayerHasCard(GameObject go)
    {
        foreach (CardName name in cardObjects.Keys.ToList<CardName>())
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

}
