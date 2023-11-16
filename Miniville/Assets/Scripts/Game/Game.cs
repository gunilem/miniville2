using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Game : MonoBehaviour
{
    public static Game instance;
    

    [SerializeField] Dice[] dices = new Dice[2];
    List<Player> players = new List<Player>();
    public Dictionary<CardName, int> PileCards = new Dictionary<CardName, int>();

    delegate void del(); del state;
    float waitDiceFinalResult = 120f;

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
        foreach(Dice dice in dices)
        {
            dice.TrowDice();
        }
        state = WaitForDiceResult;
    }
    public void WaitForDiceResult()
    {
        int _result = 0;
        bool _allDicesHaveAResult = true;
        foreach (Dice dice in dices)
        {
            int _diceResult = dice.result;
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
            waitDiceFinalResult -= 1 * Time.deltaTime;
        }

        if (waitDiceFinalResult <= 0)
        {
            Debug.Log("result = " + _result);
            state = PaidPlayers;
        }
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
    //rece

    //Construction :
    //Le joueur choisit d'acheter une carte ou de rien faire

}
