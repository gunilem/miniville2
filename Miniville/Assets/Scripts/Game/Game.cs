using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Game : MonoBehaviour
{
    public static Game instance;

    [SerializeField] GameObject oneDiceUi;
    [SerializeField] GameObject twoDiceUi;
    [SerializeField] GameObject toNextStateUi;
    [SerializeField] GameObject rerollUi;

    bool playerMadeChoice = false;

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
    [SerializeField][Range(1,4)] int numberOfPlayers = 1;

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

        ReloadCard();


        state = PlayerTrowDices;
    }
    void Update()
    {
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
        if (playerHasStation) twoDiceUi.SetActive(true);
        else oneDiceUi.SetActive(true);
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
        twoDiceUi.SetActive(false);
        oneDiceUi.SetActive(false);
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
                        Debug.Log(string.Format("Player{0} donen argent à Player{1} grâce à ces {2}{3}", currentPlayerIndex + 1, i + 1, players[i].PileCards[name], name));
                        for (int y = 0; y < players[i].PileCards[name]; y++)
                        {
                            players[currentPlayerIndex].PaidOtherPlayer(players[i], AllCards.allCards[name].Action());
                        }
                    }

                }
            }
        }

        state = PlayersReceivesMoney;
    }


    public void PlayersReceivesMoney()
    {
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
                            if (AllCards.CardsData[name].color == CardColor.Purple) PurpleCardAction(name); //si c'est une carte violette
                            else //si c'est une carte bleu ou verte
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
        state = WaitForPlayerToSelectCard;

    }
    public void WaitForPlayerToSelectCard()
    {
        isPurchasing = true;
        toNextStateUi.SetActive(true);

        if (ToNextState)
        {
            isPurchasing = false;
            toNextStateUi.SetActive(false);
            ToNextState = false;
            state = CheckPlayerHasWon;
        }
    }
    public bool PlayerBuild(CardName cardPlayerWantToBuy)
    {
        var currentPlayer = players[currentPlayerIndex];

        bool hasBought = PileCards[cardPlayerWantToBuy] > 0 && currentPlayer.TryBuyCard(cardPlayerWantToBuy);
        if (hasBought)
            state = CheckPlayerHasWon;
        return hasBought;
    }

    public bool PlayerBuildMonument(MonumentName cardPlayerWantToBuy)
    {
        var currentPlayer = players[currentPlayerIndex];

        bool hasBought = currentPlayer.TryBuyMonument(cardPlayerWantToBuy);
        if (hasBought)
            state = CheckPlayerHasWon;
        return hasBought;
    }

    public void CheckPlayerHasWon()
    {
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
                currentPlayerIndex = currentPlayerIndex % numberOfPlayers;
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

    private void PurpleCardAction(CardName name)
    {
        if (name == CardName.Stadium)
            StadiumAction();
        else if (name == CardName.BusinessCenter)
            BuisnessCenterAction();
        else if (name == CardName.TVStation)
            TVStationAction();
        else
        {
            Debug.LogError("isnt a purple card");
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

    private void BuisnessCenterAction()
    {
        //Ici choisir le joueur à qui échanger une carte
        int indexPlayerToStealCard = numberOfPlayers - 1;
        //ici choisi la carte à voler parmis les cartes qu'il à (à savoir celle qui ont 1 ou plus comme value dans leur dictionnaire PileCards )
        CardName cardToSteal = CardName.Forest;
        //ici on choisi la carte à donner dans son jeu (à savoir les cartes qui ont plus de 1 dans notre PileCard)
        CardName cardToGive = CardName.Restaurant;

        players[currentPlayerIndex].GiveCard(cardToSteal);
        players[currentPlayerIndex].PileCards[cardToGive]--;

        players[indexPlayerToStealCard].GiveCard(cardToGive);
        players[indexPlayerToStealCard].PileCards[cardToSteal]--;

    }

    private void TVStationAction() {
        int indexPlayerToStealCoins = numberOfPlayers - 1; //Ici choisir le joueur à qui voler de l'argent (faire attention à ce que ce ne soit pas le joueur qui joue actuellement)
        players[indexPlayerToStealCoins].PaidOtherPlayer(players[currentPlayerIndex],5);
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
                    

                    card.GetComponent<CardDisplayData>().cardName = cards[i];

                    ChangeMaterial(AllCards.CardsData[cards[i]].material, card);
                    card.GetComponent<CardDisplayData>().size = cardSizeMultiplier;

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
