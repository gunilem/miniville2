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
            Debug.Log("Il y a trop de GameManagerSandro");
            return;
        }
        instance = this;
    }

    public Dictionary<CardName, int> PileCards = new Dictionary<CardName, int>();

    private void Start()
    {
        FillPile();
    }


    void Game()
    {
        
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

    int RollDices(bool doubleDice = false)
    {
        //script de Ando
        return 0;
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
}
