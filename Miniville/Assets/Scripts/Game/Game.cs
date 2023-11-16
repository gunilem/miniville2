using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    List<Player> players;
    [SerializeField] Dice[] dices = new Dice[2];
    delegate void del(); del state;

    // Start is called before the first frame update
    void Start()
    {
        state = PlayerTrowDices;
    }

    // Update is called once per frame
    void Update()
    {
        state();
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
        foreach (Dice dice in dices)
        {
            int _diceResult = dice.result;
            if (_diceResult != -1)
            {
                _result += _diceResult;
            }
        }
        Debug.Log("result = " + _result);
    }

    public void Wait()
    {
       
    }


    //Instancier les joueurs

    //Lancer de dés :

    //Joueur lance le dés en fonction de s'il a une gare
    //peut choisir une fois de relancer s'il à la tour radio
    //refait un tour s'il fait un double et a le parc d'attraction

    //Payement :

    //faire les payement (le joueur paye les autres joueur avce des cartes rouge)

    //Construction :
    //Le joueur choisit d'acheter une carte ou de rien faire
}
