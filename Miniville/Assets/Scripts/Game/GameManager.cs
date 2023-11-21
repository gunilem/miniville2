using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("Il y a trop de GameManager");
            return;
        }
        instance = this;
    }

    [Header("Data")]
    [SerializeField] Dice[] dices = new Dice[2];
    List<Player> players = new List<Player>();
    public Dictionary<CardName, int> PileCards = new Dictionary<CardName, int>();

    [Header("Settings")]
    [SerializeField][Range(1, 4)] int numberOfPlayers;

    delegate void del(); del state;
    float waitDiceFinalResult = 5f;
    Player currentPlayer;

    
    void Start()
    {
        FillPile();

        for (int i = 0; i < numberOfPlayers; i++) { players.Add(new Player()); }
        currentPlayer = players[0];
        currentPlayer.PileMonuments[MonumentName.Station] = false;
        StartCoroutine(CrtWaitForDiceResult());
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

    
    

    IEnumerator CrtWaitForDiceResult()
    {
        int _result = 0;
        bool playerHasStation = currentPlayer.PileMonuments[MonumentName.Station];

        for (int i = 0; i < 1 + Convert.ToInt16(playerHasStation); i++)
        {
            dices[i].TrowDice();
        }
        yield return new WaitForSeconds(waitDiceFinalResult);
        for (int i = 0; i < 1 + Convert.ToInt16(playerHasStation); i++)
        {
            _result += dices[i].result;
        }
        
        Debug.Log("result = " + _result);
    }
    public void PaidPlayers()
    {

    }
    public void PlayerBuild()
    {

    }

    //Instancier les joueurs

    //Lancer de dés :

    //Joueur lance le dés en fonction de s'il a une gare
    //peut choisir une fois de relancer s'il à la tour radio
    //refait un tour s'il fait un double et a le parc d'attraction

    //Payement :

    //faire les payement (le joueur paye les autres joueur avce des cartes rouge)
    //cartes bleu et violettes : uniquement pour le joueur dont c'est le tour / cartes verte tous les joueurs 

    //Construction :
    //Le joueur choisit d'acheter une carte ou de rien faire
}
