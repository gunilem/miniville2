using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Game : MonoBehaviour
{
    public static Game instance;
    [Header("Data")]
    [SerializeField] Dice[] dices = new Dice[2];
    List<Player> players = new List<Player>();
    int currentPlayerIndex;
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
        Debug.Log("nmb de joueur : " + numberOfPlayers);
        for(int i = 0; i < numberOfPlayers; i++) { players.Add(new Player()); }
        currentPlayerIndex = 0;
        
        
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
            Debug.Log("result = " + _result);
            state = PaidPlayers;
        }
    }
    public void PaidPlayers()
    {
        for(int i = 0; i < players.Count; i++)
        {
            if (i == currentPlayerIndex) i++;
            foreach (CardName name in AllCards.CardsData.Keys)
            {
                if (AllCards.CardsData[name].color == CardColor.Red && players[i].PileCards[name] > 0) //si c'est un carte rouge et que le joueur la poss�de
                {
                    players[currentPlayerIndex].PaidOtherPlayer(players[i], AllCards.allCards[name].Action());
                }
            }
        }
    }
    public void PlayerBuild()
    {
    
    }
        
    //Instancier les joueurs

    //Lancer de d�s :

    //Joueur lance le d�s en fonction de s'il a une gare
    //peut choisir une fois de relancer s'il � la tour radio
    //refait un tour s'il fait un double et a le parc d'attraction

    //Payement :

    //faire les payement (le joueur paye les autres joueur avce des cartes rouge)
    //recup�re l'argent et les effets de ces autres cartes

    //Construction :
    //Le joueur choisit d'acheter une carte ou de rien faire

}
