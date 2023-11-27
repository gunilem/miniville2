using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Game : MonoBehaviour
{
    public static Game instance;
    [Header("Data")]
    [SerializeField] Dice[] dices = new Dice[2];
    public List<Player> players = new List<Player>();
    public int currentPlayerIndex;
    int currentDiceResult = 11;
    int playerWhosWin;
    CardName currentCardToBuy;
    public Dictionary<CardName, int> PileCards = new Dictionary<CardName, int>();

    [Header("Settings")]
    [SerializeField][Range(1,4)] int numberOfPlayers = 1;
    [SerializeField] float waitDiceFinalResult = 50;

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
        for(int i = 0; i < numberOfPlayers; i++) { players.Add(new Player()); }
        currentPlayerIndex = 0;
        players[0].PileCards[CardName.WheatFields] = 5;
        players[0].PileCards[CardName.VegetableStore] = 1;


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

    public void Wait()
    {

    }
    public void PlayerTrowDices()
    {
        Debug.Log("Nombre de monument : " + players[currentPlayerIndex].PileMonuments.Count);
        bool playerHasStation = players[currentPlayerIndex].PileMonuments[MonumentName.Station];
        for (int i = 0; i < 1 + Convert.ToInt16(playerHasStation); i++)
        {
            dices[i].TrowDice();
        }
        state = WaitForDiceResult;
    }
    public void WaitForDiceResult()
    {
        int _result = 0;
        bool _allDicesHaveAResult = true;
        bool playerHasStation = players[currentPlayerIndex].PileMonuments[MonumentName.Station];
        for (int i = 0; i < 1 + Convert.ToInt16(playerHasStation); i++)
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
            waitDiceFinalResult -= 10f * Time.deltaTime;
        

        if (waitDiceFinalResult <= 0)
        {
            currentDiceResult = _result;
            state = PaidOtherPlayers;
        }
    }
    public void PaidOtherPlayers()
    {
        for(int i = 0; i < players.Count; i++)
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
        for (int i = 0; i < players.Count; i++)
        {
            if (i == currentPlayerIndex) //si c'est le joueur dont c'est le tour
            {
                foreach (CardName name in AllCards.CardsData.Keys)
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
        bool PAUL_MET_ICI_LA_CONDITION_POUR_DIRE_QUE_LA_CARTE_A_ETE_CHOISI = false;
        bool PAUL_MET_ICI_LA_CONDITION_POUR_DIRE_QUE_LON_NE_CONSTRUIT_RIEN = false;

        if (PAUL_MET_ICI_LA_CONDITION_POUR_DIRE_QUE_LA_CARTE_A_ETE_CHOISI)
        {
            currentCardToBuy = CardName.Forest;
            state = PlayerBuild;
        }
        else if (PAUL_MET_ICI_LA_CONDITION_POUR_DIRE_QUE_LON_NE_CONSTRUIT_RIEN)
        {
            state = CheckPlayerHasWon;
        }
    }
    public void PlayerBuild()
    {
        var currentPlayer = players[currentPlayerIndex];
        CardName cardPlayerWantToBuy = currentCardToBuy;

        bool hasBought = currentPlayer.TryBuyCard(cardPlayerWantToBuy);
        state = CheckPlayerHasWon;
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
            state = PlayerTrowDices;
            currentPlayerIndex++;
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
        for(int i = 0; i < players.Count; i++)
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
}
