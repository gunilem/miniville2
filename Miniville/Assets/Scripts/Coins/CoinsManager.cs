using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinsManager : MonoBehaviour
{
    public GameObject coin1;
    public GameObject coin5;
    public GameObject coin10;

    public List<Player_TEST> players = new List<Player_TEST>();
    private List<int> prePlayersMoney = new List<int>();
    public bool updatePlayer;

    private List<GameObject> displayedCoin = new List<GameObject>();
    private List<List<GameObject>> displayedCoinPerPlayer = new List<List<GameObject>>();

    void Start()
    {
        for (int i = 0; i < players.Count; i++)
        {
            prePlayersMoney.Add(0);
            displayedCoinPerPlayer.Add(new List<GameObject>());
        }

        updatePlayer = false;
        
    }

    void Update()
    {
        if (updatePlayer)
        {
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].coin > prePlayersMoney[i])
                {
                    InstantiateCoins(players[i], players[i].coin - prePlayersMoney[i]);
                    prePlayersMoney[i] = players[i].coin;
                }

                if (players[i].coin < prePlayersMoney[i])
                {
                    RemoveCoins(players[i], prePlayersMoney[i] - players[i].coin);
                    prePlayersMoney[i] = players[i].coin;
                }

            }
        }

        updatePlayer = false;
    }

    private void InstantiateCoins(Player_TEST player, int nbCoins)
    {
        int coinToInstantiate = nbCoins;
        bool instantiated = false;
        int nbOfcoin1 = 0;

        while (coinToInstantiate >= 10)
        {
            displayedCoinPerPlayer[players.IndexOf(player)].Add(Instantiate(coin10, player.gameObject.transform));
            coinToInstantiate -= 10;
        }

        while (coinToInstantiate >= 5)
        {
            instantiated = false;
            foreach (GameObject coin in displayedCoinPerPlayer[players.IndexOf(player)])
            {
                if (coin.name == "Coins_5(Clone)")
                {
                    RemoveCoins(player, 5);
                    InstantiateCoins(player, 10);
                    coinToInstantiate -= 5;
                    instantiated = true;
                    break;
                }
            }
            if (!instantiated)
            {
                displayedCoinPerPlayer[players.IndexOf(player)].Add(Instantiate(coin5, player.gameObject.transform));
                coinToInstantiate -= 5;
            }
        }

        while (coinToInstantiate >= 1)
        {
            instantiated = false;
            foreach (GameObject coin in displayedCoinPerPlayer[players.IndexOf(player)])
            {
                if (coin.name == "Coins_1(Clone)")
                    nbOfcoin1++;

                if (nbOfcoin1 >= 5)
                {
                    RemoveCoins(player, 1);
                    RemoveCoins(player, 1);
                    RemoveCoins(player, 1);
                    RemoveCoins(player, 1);
                    RemoveCoins(player, 1);

                    InstantiateCoins(player, 5);
                    coinToInstantiate -= 5;
                    instantiated = true;
                    break;
                }
            }
            if (!instantiated)
            {
                displayedCoinPerPlayer[players.IndexOf(player)].Add(Instantiate(coin1, player.gameObject.transform));
                coinToInstantiate -= 1;
            }
        }
    }

    private void RemoveCoins(Player_TEST player, int nbCoins)
    {
        int nbCoinToRemove = nbCoins;
        List<GameObject> coinsToRemove = new List<GameObject>();
        int coinsToAdd = 0;

        if (player.coin >= nbCoinToRemove)
        {
            for (int i = nbCoinToRemove; i >= 10; i -= 10)
            {
                foreach (GameObject coin in displayedCoinPerPlayer[players.IndexOf(player)])
                {
                    if (coin.name == "Coins_10(Clone)")
                    {
                        coinsToRemove.Add(coin);
                        displayedCoinPerPlayer[players.IndexOf(player)].Remove(coin);
                        nbCoinToRemove -= 10;
                        break;
                    }
                }
            }

            for (int i = nbCoinToRemove; i >= 5; i -= 5)
            {
                bool coinRemoved = false;

                foreach (GameObject coin in displayedCoinPerPlayer[players.IndexOf(player)])
                {
                    if (coin.name == "Coins_5(Clone)")
                    {
                        coinsToRemove.Add(coin);
                        displayedCoinPerPlayer[players.IndexOf(player)].Remove(coin);
                        nbCoinToRemove -= 5;
                        coinRemoved = true;
                        break;
                    }
                }
                
                if (!coinRemoved)
                {
                    foreach (GameObject coin in displayedCoinPerPlayer[players.IndexOf(player)])
                    {
                        if (coin.name == "Coins_10(Clone)")
                        {
                            coinsToRemove.Add(coin);
                            displayedCoinPerPlayer[players.IndexOf(player)].Remove(coin);
                            coinsToAdd += 5;
                            nbCoinToRemove -= 5;
                            coinRemoved = true;
                            break;
                        }
                    }
                }
            }

            for (int i = nbCoinToRemove; i >= 1; i -= 1)
            {
                bool coinRemoved = false;

                foreach (GameObject coin in displayedCoinPerPlayer[players.IndexOf(player)])
                {
                    if (coin.name == "Coins_1(Clone)")
                    {
                        coinsToRemove.Add(coin);
                        displayedCoinPerPlayer[players.IndexOf(player)].Remove(coin);
                        nbCoinToRemove -= 1;
                        coinRemoved = true;
                        break;
                    }
                }

                if (!coinRemoved)
                {
                    foreach (GameObject coin in displayedCoinPerPlayer[players.IndexOf(player)])
                    {
                        if (coin.name == "Coins_5(Clone)")
                        {
                            coinsToRemove.Add(coin);
                            displayedCoinPerPlayer[players.IndexOf(player)].Remove(coin);
                            coinsToAdd += 4;
                            nbCoinToRemove -= 1;
                            coinRemoved = true;
                            break;
                        }
                    }

                    if (!coinRemoved)
                    {
                        foreach (GameObject coin in displayedCoinPerPlayer[players.IndexOf(player)])
                        {
                            if (coin.name == "Coins_10(Clone)")
                            {
                                coinsToRemove.Add(coin);
                                displayedCoinPerPlayer[players.IndexOf(player)].Remove(coin);
                                coinsToAdd += 9;
                                nbCoinToRemove -= 1;
                                coinRemoved = true;
                                break;
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < coinsToRemove.Count; i++)
            {
                GameObject coin = coinsToRemove[i];
                Destroy(coin);
            }

            if (coinsToAdd > 0)
                InstantiateCoins(player, coinsToAdd);
        }
    }
}
